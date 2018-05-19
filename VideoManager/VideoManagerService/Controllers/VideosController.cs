using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ADL;
using Models;
using VideoManagerService.Filters;

namespace VideoManagerService.Controllers
{
    /// <summary>
    /// Controller containing all the actions invokded regarding Video. This includes GET, POST, PUT, DELETE video methods.
    /// </summary>
    [IdentityBasicAuthenticationAttribute]
    public class VideosController : ApiController
    {
        private DbAccessor dbAccessor;

        /// <summary>
        /// The default constructor initializes the database accessor instance
        /// </summary>
        public VideosController()
        {
            //var connectionString = System.Web.HttpContext.Current.Server.MapPath(@"~\App_Data\SQLite.db");
            var connectionString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DatabaseFilePath"]);
            dbAccessor = new DbAccessor(connectionString);
        }


        /// <summary>
        /// The constructor which initializes database accessor instance given by user
        /// </summary>
        /// <param name="context"></param>
        public VideosController(IVideoManagerContext context)
        {
            dbAccessor = new DbAccessor(context);
        }

        /// <summary>
        /// Adds links to a list of videos
        /// </summary>
        /// <param name="videoModels">the list of video object</param>
        /// <param name="uname">username</param>
        /// <returns>list of videos after adding links</returns>

        private IList<VideoModel> AddLinks(IList<VideoModel> videoModels, string uname)
        {
            if (videoModels == null)
                return null;
            foreach (VideoModel videoModel in videoModels)
            {
                videoModel.links.Add(new Link { link = Url.Link("GetUserByUsername", new { username = uname }), Method = "GET", Rel = "User" });
                videoModel.links.Add(new Link { link = Url.Link("GetVideos", new { username = uname }), Method = "GET", Rel = "Self" });
                videoModel.links.Add(new Link { link = Url.Link("GetTags", new { videoLink = videoModel.VideoLink, user = uname }), Method = "GET", Rel = "Tags" });
                videoModel.links.Add(new Link { link = Url.Link("AddTags", new { videoLink = videoModel.VideoLink, user = uname }), Method = "POST", Rel = "Tags" });
                videoModel.links.Add(new Link { link = Url.Link("GetPlaylist", new { videoLink = videoModel.VideoLink, user = uname }), Method = "GET", Rel = "Playlists" });
            }
            return videoModels;
        }

        /// <summary>
        /// Add links to a video
        /// </summary>
        /// <param name="videoModel">the video object</param>
        /// <param name="uname">username</param>
        /// <returns>the video after adding links</returns>
        private VideoModel AddLinks(VideoModel videoModel, string uname)
        {
            if (videoModel == null)
                return null;

            videoModel.links.Add(new Link { link = Url.Link("GetUserByUsername", new { username = uname }), Method = "GET", Rel = "User" });
            videoModel.links.Add(new Link { link = Url.Link("GetVideos", new { username = uname }), Method = "GET", Rel = "Self" });
            videoModel.links.Add(new Link { link = Url.Link("GetTags", new { videoLink = videoModel.VideoLink, user = uname }), Method = "GET", Rel = "Tags" });
            videoModel.links.Add(new Link { link = Url.Link("AddTags", new { videoLink = videoModel.VideoLink, user = uname }), Method = "POST", Rel = "Tags" });
            videoModel.links.Add(new Link { link = Url.Link("GetPlaylist", new { videoLink = videoModel.VideoLink, user = uname }), Method = "GET", Rel = "Playlists" });

            return videoModel;
        }

        /// <summary>
        /// Returns the list of videos of a user
        /// </summary>
        /// <param name="uname">username</param>
        /// <returns>list of videos</returns>
        [ResponseType(typeof(IList<VideoModel>))]
        [HttpGet]
        [Route("api/Videos/{uname}", Name = "GetVideos")]
        public async Task<IHttpActionResult> GetVideos(string uname)           // add query for domain name as well
        {
            IList<VideoModel> list = await dbAccessor.GetVideos(uname);
            if (list == null)
                return BadRequest("error occured");
            list = AddLinks(list, uname);
            return Ok(list);
        }

        /// <summary>
        /// Gets videos by title
        /// </summary>
        /// <param name="title">title of video</param>
        /// <param name="user">usernaem of the user</param>
        /// <returns>video</returns>
        [HttpGet]
        [ResponseType(typeof(VideoModel))]
        [Route("api/GetVideoByTitle", Name = "GetVideoByTitle")]
        public async Task<IHttpActionResult> GetVideoByTitle(string title, string user)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(user))
                return BadRequest("Invalid Parameters");
            VideoModel vm = await dbAccessor.GetVideoByTitle(title, user);
            if (vm == null)
                return BadRequest("Could not get video by title");
            else
                return Ok(vm);
        }

        /// <summary>
        /// Gets videos with any person in the list
        /// </summary>
        /// <param name="people">list of persons to be searched</param>
        /// <param name="username">username of user</param>
        /// <returns>list of videos</returns>
        [HttpGet]
        [ResponseType(typeof(IList<VideoModel>))]
        [Route("api/Video/People", Name = "GetVideoWithPeople")]
        public async Task<IHttpActionResult> GetVideosWithPeople(List<string> people, string username)
        {
            if (people == null || people.Count == 0 || string.IsNullOrEmpty(username))
                return BadRequest("Invalid Parameters");
            IList<VideoModel> list = await dbAccessor.GetVideosWithPeople(people, username);
            if (list == null)
                return BadRequest("Error occured");
            if (list.Count == 0)
                return NotFound();
            list = AddLinks(list, username);
            return Ok(list);
        }

        /// <summary>
        /// Gets videos which has all the tags from the list
        /// </summary>
        /// <param name="tags">list of tags</param>
        /// <param name="user">username</param>
        /// <returns>list of videos</returns>
        [HttpGet]
        [ResponseType(typeof(IList<VideoModel>))]
        [Route("api/Video/AllTags", Name = "GetVideosWithAllTags")]
        public async Task<IHttpActionResult> GetVideosWithAllTags(List<string> tags, string user)
        {
            if (tags == null || tags.Count == 0 || string.IsNullOrEmpty(user))
                return BadRequest("Invalid Parameters");
            IList<VideoModel> list = await dbAccessor.GetVideosWithAllTags(tags, user);
            if (list == null)
                return BadRequest("Error occured");
            if (list.Count == 0)
                return NotFound();
            list = AddLinks(list, user);
            return Ok(list);
        }

        /// <summary>
        /// Returns videos which has evena single tag from the list
        /// </summary>
        /// <param name="tags">the list of tags to be searched</param>
        /// <param name="user">username of the user</param>
        /// <returns>list of videos</returns>
        [HttpGet]
        [ResponseType(typeof(IList<VideoModel>))]
        [Route("api/Video/AnyTag", Name = "GetVideoWithAnyTag")]
        public async Task<IHttpActionResult> GetVideosWithAnyTag(List<string> tags, string user)
        {
            if (tags == null || tags.Count == 0 || string.IsNullOrEmpty(user))
                return BadRequest("Invalid Parameters");
            var list = await dbAccessor.GetVideosWithAnyTag(tags, user);
            if (list == null)
                return BadRequest("Error occured");
            if (list.Count == 0)
                return NotFound();
            list = AddLinks(list, user);
            return Ok(list);
        }

        /// <summary>
        /// gets videos with a category
        /// </summary>
        /// <param name="categoryModels">the category of videos to be searched</param>
        /// <param name="user"> username of the user</param>
        /// <returns>list of videos</returns>
        [HttpGet]
        [ResponseType(typeof(IList<VideoModel>))]
        [Route("api/Video/Category", Name = "GetVideosWithCategory")]
        public async Task<IHttpActionResult> GetVideosWithCategory(List<CategoryModel> categoryModels, string user)
        {
            var list = await dbAccessor.GetVideosWithCategory(categoryModels, user);
            if (list == null)
                return BadRequest("Error occured");
            list = AddLinks(list, user);
            return Ok(list);
        }

        /// <summary>
        /// Gets videos of a playlist
        /// </summary>
        /// <param name="playlist">name of playlist</param>
        /// <param name="user">username</param>
        /// <returns>videos</returns>
        [HttpGet]
        [ResponseType(typeof(IList<VideoModel>))]
        [Route("api/Videos/playlist/{playlist}/{user}", Name = "GetVideosInPlaylist")]
        public async Task<IHttpActionResult> GetVideosInPlayList(string playlist, string user)
        {
            if (string.IsNullOrEmpty(playlist) || string.IsNullOrEmpty(user))
                return BadRequest("Invalid Parameters");
            var list = await dbAccessor.getVideosInPlayList(playlist, user);
            if (list == null)
                return BadRequest("Error occured");
            if (list.Count == 0)
                return NotFound();
            list = AddLinks(list, user);
            return Ok(list);
        }

        /// <summary>
        /// Gets a video by link
        /// </summary>
        /// <param name="link">link of video to be searched</param>
        /// <param name="user">the username of user</param>
        /// <returns>the video</returns>
        [HttpGet]
        [ResponseType(typeof(VideoModel))]
        [Route("api/VideoByLink", Name = "GetVideoByLink")]
        public async Task<IHttpActionResult> GetVideoByLink(string link, string user)
        {
            if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(user))
                return BadRequest("Invalid arguments");
            VideoModel vm = await dbAccessor.GetVideoByLink(link, user);
            if (vm == null)
                return BadRequest("Error occured");
            vm = AddLinks(vm, user);
            return Ok(vm);
        }

        /// <summary>
        /// Updates a video
        /// </summary>
        /// <param name="link">link of video to be updated</param>
        /// <param name="user">the user for which video has to be updated</param>
        /// <param name="videoModel">the changes which have to be done</param>
        /// <returns>the video after updation</returns>
        [ResponseType(typeof(IList<VideoModel>))]
        [HttpPatch]
        [Route("api/Video")]
        public async Task<IHttpActionResult> PatchVideo([FromUri] string link, [FromUri] string user, [FromBody]VideoModel videoModel)           // add query for domain name as well
        {
            var result = await dbAccessor.PatchVideo(link, user, videoModel);
            if (result.Equals("success"))
            {
                var video = await dbAccessor.GetVideoByLink(link, user);
                return Ok(video);
            }
            else
                return BadRequest("Could not update");
        }

        /// <summary>
        /// Create a video for user
        /// </summary>
        /// <param name="videoModel">the video to be created</param>
        /// <returns>the video created</returns>
        [HttpPost]

        [Route("api/Video")]
        public async Task<IHttpActionResult> PostVideo([FromBody]VideoModel videoModel)
        {
            if (string.IsNullOrEmpty(videoModel.Username) || string.IsNullOrEmpty(videoModel.VideoLink) || string.IsNullOrEmpty(videoModel.Title))
                return BadRequest("Invalid State");
            var result = await dbAccessor.PostVideo(videoModel);
            if (result.Equals("error"))
                return BadRequest("Could not create video");
            var linkCreated = Url.Link("GetVideoByLink", new { link = videoModel.VideoLink, user = videoModel.Username });
            videoModel.links.Add(new Link { link = linkCreated, Method = "GET", Rel = "Self" });
            //var response = new HttpResponseMessage(HttpStatusCode.Created);
            //response.Content = new ObjectContent(typeof(VideoModel), videoModel, new JsonMediaTypeFormatter());
            return CreatedAtRoute("GetVideoByLink", new { link = videoModel.VideoLink, user = videoModel.Username }, videoModel);
        }



        
        [HttpPost]
        [Route("api/Videos/AddToPlaylist", Name = "AddToPlaylist")]
        public async Task<IHttpActionResult> AddVideoToPlaylist([FromUri]string playlist, [FromUri] string username,[FromBody] string link)           // add query for domain name as well
        {
            var result = await dbAccessor.AddVideoToPlaylist(playlist, username, link);
            if(result.Equals("error"))
                return BadRequest("error occured");
            return Ok("done");
        }


        /// <summary>
        /// Deletes a video of a user
        /// </summary>
        /// <param name="link">link of the video to be deleted</param>
        /// <param name="user">the user for which video has to be deleted</param>
        /// <returns>a string denoting status of operation</returns>
        [HttpDelete]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> DeleteVideo(string link, string user)
        {
            var result = await dbAccessor.DeleteVideo(link, user);
            if (result.Equals("success"))
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}