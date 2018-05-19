using System.Configuration;
using System.Data.Entity;
using System.Web.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VideoManagerService.Filters
{
    public class UsersDbContext : IdentityDbContext<IdentityUser>
    {
         public UsersDbContext() 
        {
           
          //  Database.SetInitializer(new Initializer(this));
        }

        private class Initializer :CreateDatabaseIfNotExists<UsersDbContext>
        {
            public Initializer(UsersDbContext context)
            {
                //IdentityRole role = context.Roles.Add(new IdentityRole("User"));

                //IdentityUser user = new IdentityUser("SampleUser");
                //var userRole = new IdentityUserRole();
                //userRole.RoleId = role.Id;
                //userRole.UserId = user.Id;
                //user.Roles.Add(userRole);
                //user.Claims.Add(new IdentityUserClaim
                //{
                //    ClaimType = "hasRegistered",
                //    ClaimValue = "true",
                //    UserId = user.Id
                //});


                //user.PasswordHash = new PasswordHasher().HashPassword("secret");
                //context.Users.Add(user);
                //context.SaveChanges();
                //base.Seed(context);

            }
            protected override void Seed(UsersDbContext context)
            {
                IdentityRole role = context.Roles.Add(new IdentityRole("User"));

                IdentityUser user = new IdentityUser("SampleUser");
                var userRole = new IdentityUserRole();
                userRole.RoleId = role.Id;
                userRole.UserId = user.Id;
                user.Roles.Add(userRole);
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = "hasRegistered",
                    ClaimValue = "true",
                    UserId = user.Id
                });
                 

                user.PasswordHash = new PasswordHasher().HashPassword("secret");
                context.Users.Add(user);
                context.SaveChanges();
                base.Seed(context);
            }
        }
        }
    }
