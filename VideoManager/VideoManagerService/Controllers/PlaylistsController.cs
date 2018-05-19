using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ADL;
using Models;
using VideoManagerService.Filters;
using VideoManagerService.Models;

namespace VideoManagerService.Controllers
{
    /// <summary>
    /// The controller which deals with the action methods invoked on Playlist Resources. This class handles Get, Post and Delete of Playlist.
    /// </summary>
    [IdentityBasicAuthenticationAttribute]
    public class PlaylistsController : ApiController
    {
       
        private DbAccessor dbAccessor;

        /// <summary>
        /// The default constructor which initializes the controller with the default database accessor instance.
        /// </summary>
        public PlaylistsController()
        {
            var connectionString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DatabaseFilePath"]);
            dbAccessor = new DbAccessor(connectionString);
        }

        public PlaylistsController(IVideoManagerContext context)
        {
            dbAccessor = new DbAccessor(context);
        }

        /// <summary>
        /// Adds links to a list of playlists
        /// </summary>
        /// <param name="playlistModels">list of playlists</param>
        /// <param name="uname">username</param>
        /// <returns>a list of playlist after adding links</returns>
        private IList<PlaylistModel> AddLinks(IList<PlaylistModel> playlistModels, string uname)
        {
            if (playlistModels == null)
                return null;
            foreach (PlaylistModel playlistModel in playlistModels)
            {
                playlistModel.links.Add(new Link { link = Url.Link("GetPlaylist", new { username = uname, name = playlistModel.PlaylistName }), Method = "GET", Rel = "Self" });
                playlistModel.links.Add(new Link { link = Url.Link("GetVideosInPlaylist", new { playlist = playlistModel.PlaylistName, user = uname }), Method = "GET", Rel = "Videos" });

            }
            return playlistModels;
        }

        /// <summary>
        /// Adds links to a playlist
        /// </summary>
        /// <param name="playlistModel"></param>
        /// <param name="uname">username</param>
        /// <returns>playlist after adding links</returns>
        private PlaylistModel AddLinks(PlaylistModel playlistModel, string uname)
        {
            if (playlistModel == null)
                return null;

            playlistModel.links.Add(new Link { link = Url.Link("GetPlaylist", new { username = uname, name = playlistModel.PlaylistName }), Method = "GET", Rel = "Self" });
            playlistModel.links.Add(new Link { link = Url.Link("GetVideosInPlaylist", new { playlist = playlistModel.PlaylistName, user = uname }), Method = "GET", Rel = "Videos" });


            return playlistModel;
        }

        /// <summary>
        /// Gets a playlist of a user
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="name">playlist name</param>
        /// <returns>playlist</returns>
        [ResponseType(typeof(PlaylistModel))]
        [HttpGet]
        [Route("api/Playlist/{user}/{name}", Name = "GetPlaylist")]
        public async Task<HttpResponseMessage> GetPlaylist(string user, string name)
        {
            var playlist = await dbAccessor.GetPlaylist(user, name);
            if (playlist == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            playlist = AddLinks(playlist, user);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent(typeof(PlaylistModel), playlist, new JsonMediaTypeFormatter());
            return response;
        }

        /// <summary>
        /// Gets playlist of a user
        /// </summary>
        /// <param name="user">username</param>
        /// <returns>a list of playlist</returns>
        [ResponseType(typeof(IList<PlaylistModel>))]
        [Route("api/Playlists/{user}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPlaylists(string user)
        {
            var list = await dbAccessor.GetPlaylists(user);
            if (list == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            list = AddLinks(list, user);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent(typeof(IList<PlaylistModel>), list, new JsonMediaTypeFormatter());
            return response;
        }

        /// <summary>
        /// gets list of playlist in which a video is saved
        /// </summary>
        /// <param name="videoLink">link of video to be searched</param>
        /// <param name="user">username</param>
        /// <returns>a list of playlist</returns>
        [HttpGet]
        [ResponseType(typeof(IList<PlaylistModel>))]
        [Route("api/Playlist", Name = "GetPlaylistVideos")]
        public async Task<HttpResponseMessage> GetPlaylistsOfVideo([FromUri]string videoLink,[FromUri] string user)
        {
            IList<PlaylistModel> list = await dbAccessor.GetVideoPlaylist(videoLink, user);
            if (list == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            list = AddLinks(list, user);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent(typeof(IList<PlaylistModel>), list, new JsonMediaTypeFormatter());
            return response;
        }

        /// <summary>
        /// Creates playlist for a user
        /// </summary>
        /// <param name="name">name of playlist</param>
        /// <param name="user">username</param>
        /// <param name="domain">domain of the playlist</param>
        /// <param name="description">"description of playlist</param>
        /// <returns>playlist created</returns>
        [HttpPost]
        [Route("api/Playlist")]
        public async Task<HttpResponseMessage> PostPlaylist(string name, string user, string domain, string description)
        {
            var result = await dbAccessor.CreatePlaylist(name, user, domain, description);
            if (result.Equals("error"))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("Playlist created");
            response.Headers.Location = new Uri(Url.Link("GetPlaylist", new { user = user, name = name }));
            return response;

        }

        /// <summary>
        /// Deletes a playlist of a user
        /// </summary>
        /// <param name="name">the name of the playlist</param>
        /// <param name="user">username</param>
        /// <returns>the playlist</returns>
        [HttpDelete]
        [Route("api/Playlist")]
        public async Task<HttpResponseMessage> DeletePlaylist(string name, string user)
        {
            var result = await dbAccessor.DeletePlaylist(name, user);
            if (result.Equals("error"))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            return response;

        }
    }
}