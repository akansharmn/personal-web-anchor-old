using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VideoManagerService.Filters
{
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected  override async Task<IPrincipal> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            try
            {
                UserManager<IdentityUser> userManager = CreateUserManager();

                cancellationToken.ThrowIfCancellationRequested();
                IdentityUser user = await userManager.FindAsync(username, password);
                if (user == null)
                    return null;
                user.SecurityStamp = "random";
                cancellationToken.ThrowIfCancellationRequested();
                ClaimsIdentity identity = await userManager.ClaimsIdentityFactory.CreateAsync(userManager, user, "Basic");
                return new ClaimsPrincipal(identity);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private UserManager<IdentityUser> CreateUserManager()
        {
            var connectionString = System.Web.HttpContext.Current.Server.MapPath(@"~\App_Data\SQLite.db");
            var conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return new UserManager<IdentityUser>(new UserStore<IdentityUser>(new UsersDbContext()));
        }
    }
}