using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using ADL;
using VideoManagerService.Models;

namespace VideoManagerService.Controllers
{
    /// <summary>
    /// The controller which deals with the action methods invoked on Tags Resources. This class handles Get, Post and Delete of Tags.
    /// </summary>
    public class TagsController : ApiController
    {
        private DbAccessor dbAccessor = new DbAccessor();

        /// <summary>
        /// The default constructor which initializes the controller with the default database accessor instance.
        /// </summary>
        public TagsController()
        {
            var connectionString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DatabaseFilePath"]);
            dbAccessor = new DbAccessor(connectionString);
        }

        /// <summary>
        /// The constructor which initializes the controller with the user supplied instance of database accessor.
        /// </summary>
        /// <param name="context">the IVideoManager type instance</param>
        public TagsController(IVideoManagerContext context)
        {
            dbAccessor = new DbAccessor(context);
        }
        /// <summary>
        /// Returns tags in a video
        /// </summary>
        /// <param name="videoLink">link of video</param>
        /// <param name="user">username</param>
        /// <returns>a list of tags</returns>
        [ResponseType(typeof(IList<string>))]
        [HttpGet]
        [Route("api/Tags", Name = "GetTags")]
        public async Task<IHttpActionResult> GetTagsInVideo(string videoLink, string user)
        {
            var list = await dbAccessor.GetTagsInVideo(videoLink, user);
            return Ok(list);
        }


        /// <summary>
        /// Adds tags to a video
        /// </summary>
        /// <param name="tags">a list of tags</param>
        /// <param name="videoLink">link of video</param>
        /// <param name="user">username</param>
        /// <returns>a response with link of video updated</returns>
        [HttpPost]
        [Route("api/Tags", Name = "AddTags")]
        public async Task<HttpResponseMessage> AddTagToVideo(List<string> tags, string videoLink, string user)
        {
            var result = await dbAccessor.AddTagsToVideo(tags, videoLink, user);
            var link = Url.Link("GetVideoByLink", new { link = videoLink, user = user });
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(link);
            return response;
        }

        /// <summary>
        /// Removes tags from a video
        /// </summary>
        /// <param name="tags">list of tags</param>
        /// <param name="videoLink">video link</param>
        /// <param name="user">user name</param>
        /// <returns>a response containing location of updated video</returns>
        [HttpDelete]
        [Route("api/Tags", Name = "RemoveTags")]
        public async Task<HttpResponseMessage> RemoveTagFromVideo(List<string> tags, string videoLink, string user)
        {
            var result = await dbAccessor.RemoveTagsFromVideo(tags, videoLink, user);
            if (result == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            var link = Url.Link("GetVideoByLink", new { link = videoLink, user = user });
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(link);
            return response;

        }

    }
}