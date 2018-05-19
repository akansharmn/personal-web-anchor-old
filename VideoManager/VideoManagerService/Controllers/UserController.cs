using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ADL;
using Models;
using VideoManagerService.Filters;

namespace VideoManagerService.Controllers
{
    /// <summary>
    /// The controller which deals with the action methods invoked on User Resources. This class handles Get, Post and Delete of Users.
    /// </summary>
    public class UserController : ApiController
    {
        private DbAccessor dbAccessor;

        /// <summary>
        /// The default constructor which initializes the controller with the default database accessor instance.
        /// </summary>
        public UserController()
        {
            var connectionString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DatabaseFilePath"]);
            dbAccessor = new DbAccessor(connectionString);
        }

        /// <summary>
        /// The constructor which initializes the controller with the user supplied instance of database accessor.
        /// </summary>
        /// <param name="context"></param>
        public UserController(IVideoManagerContext context)
        {
            dbAccessor = new DbAccessor(context);
        }

        /// <summary>
        /// Returns a user
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>user</returns>
        [HttpGet]
        [Route("api/User/{username}", Name = "GetUserByUsername")]
        public async Task<IHttpActionResult> GetUser(string username)
        {
            UserModel user = await dbAccessor.GetUser(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// creates a user
        /// </summary>
        /// <param name="user">username</param>
        /// <returns>a user</returns>
        [HttpPost]
        [ValidateModel]
        [ResponseType(typeof(UserModel))]
        [Route("api/User")]
        public async Task<HttpResponseMessage> PostUser(UserModel user)
        {
            if (user.UserId == 0 || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (!ModelState.IsValid)
            {
               
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            var result = await dbAccessor.PostUser(user);
            if (result.Equals("error"))
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            string uri = Url.Link("GetUserByUsername", new { username = user.Username });
            response.Headers.Location = new Uri(uri);
            response.Content = new StringContent("User Created");
            return response;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user">username</param>
        /// <returns>a string denoting the result of the operation</returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteUser(string user)
        {   if(string.IsNullOrEmpty(user))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            var result = await dbAccessor.DeleteUser(user);
            if (result.Equals("error"))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
