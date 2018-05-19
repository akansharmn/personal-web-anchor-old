using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;


namespace ADL
{
    public class DbAccessor
    {
        IVideoManagerContext context;

        public DbAccessor()
        {
            this.context = new masterEntities();
           // this.context = new SQLiteDbContext(conn);
        }

        public DbAccessor(string conn)
        {
            //  this.context = new masterEntities();
            this.context = new SQLiteDbContext(conn);
        }

        public DbAccessor(IVideoManagerContext context)
        {
            this.context = context;
        }

        public async Task<string> PostUser(UserModel userModel)
        {
            try
            {

                User user = new User { DateOfRegistartion = userModel.DateOfRegistartion, Domain = userModel.Domain, Email = userModel.Email, Username = userModel.Username };
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }


        }

        public async Task<IList<PlaylistModel>> GetPlaylistsOfVideo(string link, string user)
        {
            try
            {

                IQueryable<PlaylistModel> playlists = from playlist in context.Playlists
                                                      where playlist.Videos.Any(video => video.VideoLink == link)
                                                      where playlist.username == user
                                                      select new PlaylistModel
                                                      {
                                                          PlaylistName = playlist.PlaylistName,
                                                          Description = playlist.Description,
                                                          DomainPlaylistId = playlist.DomainPlaylistId
                                                      };
                return await playlists.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public async Task<string> RemoveTagsFromVideo(List<string> tags, string videoLink, string user)
        {
            try
            {
                int videoId = (from video in context.Videos where video.VideoLink == videoLink select video.VideoId).First();
                foreach (string tag in tags)
                {
                    Tag tagModel = await context.Tags.FirstAsync(x => (x.TagName == tag && x.VideoId == videoId));
                    context.Tags.Remove(tagModel);

                }
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }

        }

        public async Task<string> DeletePlaylist(string name, string user)
        {
            try
            {
                var playlist = await context.Playlists.FirstAsync(x => x.PlaylistName == name && x.username == user);
                context.Playlists.Remove(playlist);
                return "success";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> DeleteUser(string username)
        {
            try
            {
                var user = await context.Users.FirstAsync(x => x.Username == username);
                context.Users.Remove(user);
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }

        }

        public Task<PlaylistModel> GetPlaylist(string user, string name)
        {
            try
            {
                var playlistValue = (from playlist in context.Playlists
                                     where playlist.PlaylistName == name && playlist.username == user
                                     select
                                        new PlaylistModel
                                        {
                                            PlaylistName = playlist.PlaylistName,
                                            PlaylistId = playlist.PlaylistId,
                                            DomainPlaylistId = playlist.DomainPlaylistId,
                                            Description = playlist.Description
                                        }).FirstAsync();
                return playlistValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> CreatePlaylist(string name, string user, string domain, string desc)
        {
            try
            {

                Playlist playlist = new Playlist
                {
                    PlaylistName = name,
                    username = user,
                    domain = domain,
                    Description = desc
                };

                context.Playlists.Add(playlist);
                await context.SaveChangesAsync();

                return "success";
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public async Task<IList<PlaylistModel>> GetPlaylists(string user)
        {
            try
            {
                IQueryable<PlaylistModel> playlists = from playlist in context.Playlists
                                                      where playlist.username == user
                                                      select new PlaylistModel
                                                      {
                                                          PlaylistName = playlist.PlaylistName,
                                                          Description = playlist.Description,
                                                          DomainPlaylistId = playlist.DomainPlaylistId
                                                      };
                return await playlists.ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<string> AddTagsToVideo(List<string> tags, string videoLink, string user)
        {
            try
            {
                int videoId = (from video in context.Videos where video.VideoLink == videoLink select video.VideoId).First();
                foreach (string tag in tags)
                {
                    context.Tags.Add(new Tag { TagName = tag, VideoId = videoId });
                }

                await context.SaveChangesAsync();
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }

        }

        public async Task<string> DeleteVideo(string link, string uname)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == uname select user.UserId).First();
                var video = context.Videos.Single(x => x.VideoLink == link && x.UserId == userid);
                context.Videos.Remove(video);
                await context.SaveChangesAsync();
                return "successful";
            }
            catch (Exception ex)
            {
                return "Could Not Delete";
            }

        }

        public async Task<UserModel> GetUser(string username)
        {
            try
            {
                return await (from user in context.Users
                              where user.Username == username
                              select new UserModel
                              {
                                  Username = user.Username,
                                  UserId = user.UserId,
                                  DateOfRegistartion = user.DateOfRegistartion,
                                  Domain = user.Domain,
                                  Email = user.Email
                              }).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<VideoModel> GetVideoByLink(string url, string uname)
        {
            try
            {

                int userid = (from user in context.Users where user.Username == uname select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                IQueryable<VideoModel> videos = from video in context.Videos
                                                where video.UserId == userid && video.VideoLink == url
                                                select
                   new VideoModel
                   {
                       VideoLink = video.VideoLink,
                       Title = video.Title,
                       WatchCount = video.WatchCount,
                       UploadedBy = video.UploadedBy,
                       UploadedDate = video.UploadedDate,
                       dislikes = video.dislikes,
                       likes = video.likes,
                       LastWatchOffset = video.LastWatchOffset,
                       Category = (CategoryModel)video.Category,
                       DomainName = video.DomainName,
                       VideoDuration = video.VideoDuration,
                       LastWatched = video.WatchedDate,
                       VideoId = video.VideoId
                   };

                return await videos.FirstAsync();
            }
            catch (Exception)
            {
                return null;
            }


        }
        
        public async Task<IList<VideoModel>> getVideosInPlayList(string playlistName, string user)
        {
            try
            {
                int playlistId = await (from playlist in context.Playlists where playlist.PlaylistName == playlistName && playlist.username == user select playlist.PlaylistId).FirstOrDefaultAsync();
                if (playlistId == 0)
                    return null;
                IQueryable<Video> videos = from video in context.Videos where video.Playlists.Any(x => x.PlaylistId == playlistId) select video;
                return await videos.Select(video => new VideoModel
                {
                    VideoLink = video.VideoLink,
                    Title = video.Title,
                    WatchCount = video.WatchCount,
                    UploadedBy = video.UploadedBy,
                    UploadedDate = video.UploadedDate,
                    dislikes = video.dislikes,
                    likes = video.likes,
                    LastWatchOffset = video.LastWatchOffset,
                    Category = (CategoryModel)video.Category,
                    DomainName = video.DomainName,
                    VideoDuration = video.VideoDuration,
                    LastWatched = video.WatchedDate,
                    VideoId = video.VideoId
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<IList<VideoModel>> GetVideosWithAllTags(List<string> tags, string uname)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == uname select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;

                // context.Videos.Join(context.Tags, video => video.VideoId, tag => tag.VideoId, (video, tag) => new { tag.TagName, tag.VideoId }).GroupBy(tag => tag.VideoId).Where(grp => grp.Count() == tags.Count());

                // IList<string> list = context.Tags.Where(tag => tags.Contains(tag.TagName)).Join(context.Videos, tag => tag.VideoId, video => video.VideoId, (tag, video) => new { tag.TagName, tag.VideoId }).GroupBy(tag => tag.VideoId).Where(grp => grp.Count() == tags.Count()).Select(grp => grp

                IList<Video> list = context.Tags.Where(tag => tags.Contains(tag.TagName)).Join(context.Videos.Where(x => x.UserId == userid), tag => tag.VideoId, video => video.VideoId, (tag, video) => new { tag.TagName, video }).GroupBy(video => video.video.VideoId).Where(grp => grp.Count() == tags.Count()).First().Select(grp => grp.video).Distinct().ToList();


                var result = new List<VideoModel>();

                foreach (Video video in list)
                {

                    result.Add(
                        new VideoModel
                        {
                            VideoLink = video.VideoLink,
                            Title = video.Title,
                            WatchCount = video.WatchCount,
                            UploadedBy = video.UploadedBy,
                            UploadedDate = video.UploadedDate,
                            dislikes = video.dislikes,
                            likes = video.likes,
                            LastWatchOffset = video.LastWatchOffset,
                            Category = (CategoryModel)video.Category,
                            DomainName = video.DomainName,
                            VideoDuration = video.VideoDuration,
                            LastWatched = video.WatchedDate,
                            VideoId = video.VideoId,
                            Username = uname
                        });
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<string> AddVideoToPlaylist(string playlistName, string username, string link)
        {
            try
            {
                int userid = await (from user in context.Users where user.Username == username select user.UserId).FirstOrDefaultAsync();
                if (userid == 0)
                    return "error";
                int playlistId = await context.Playlists.Where(x => x.PlaylistName == playlistName && x.username == username).Select(x => x.PlaylistId).FirstOrDefaultAsync();
                if (playlistId == 0)
                    return "error";


                var videoId = await context.Videos.Where(x => x.VideoLink == link && x.UserId == userid).Select(x => x.VideoId).FirstOrDefaultAsync();
                context.PlaylistVideos.Add(new PlaylistVideo
                {
                    PlaylistId = playlistId,
                    VideoId = videoId
                });
               
                context.SaveChanges();
               // var videoCheck = await context.Videos.Include(x => x.Playlists).Where(x => x.VideoLink == link && x.UserId == userid).FirstOrDefaultAsync();
                return "success";
            }
            catch(Exception ex)
            {
                return "error";
            }
        }

        public async Task<VideoModel> GetVideoByTitle(string title, string uname)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == uname select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                IQueryable<VideoModel> videos = from video in context.Videos
                                                where video.UserId == userid && video.Title == title
                                                select
                   new VideoModel
                   {
                       VideoLink = video.VideoLink,
                       Title = video.Title,
                       WatchCount = video.WatchCount,
                       UploadedBy = video.UploadedBy,
                       UploadedDate = video.UploadedDate,
                       dislikes = video.dislikes,
                       likes = video.likes,
                       LastWatchOffset = video.LastWatchOffset,
                       Category = (CategoryModel)video.Category,
                       DomainName = video.DomainName,
                       VideoDuration = video.VideoDuration,
                       LastWatched = video.WatchedDate,
                       VideoId = video.VideoId
                   };

                return await videos.FirstAsync();
            }
            catch (Exception)
            {
                return null;
            }


        }

        public async Task<IList<VideoModel>> GetVideos(string username)
        {
            try
            {

                int userid = (from user in context.Users where user.Username == username select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                var query = from video in context.Videos
                            where video.UserId == userid
                            select
              new VideoModel
              {
                  VideoLink = video.VideoLink,
                  Title = video.Title,
                  WatchCount = video.WatchCount,
                  UploadedBy = video.UploadedBy,
                  UploadedDate = video.UploadedDate,
                  dislikes = video.dislikes,
                  likes = video.likes,
                  LastWatchOffset = video.LastWatchOffset,
                  Category = (CategoryModel)video.Category,
                  DomainName = video.DomainName,
                  VideoDuration = video.VideoDuration,
                  LastWatched = video.WatchedDate,
                  VideoId = video.VideoId
              }; ;

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //return vms;


        public async Task<string> PostVideo(VideoModel videoModel)
        {
            int userid = (from user in context.Users where user.Username == videoModel.Username select user.UserId).FirstOrDefault();
            if (userid == 0)
                return "error";
            try
            {
                Video video = new Video
                {
                    VideoLink = videoModel.VideoLink,
                    Title = videoModel.Title,
                    WatchCount = videoModel.WatchCount,
                    UploadedBy = videoModel.UploadedBy,
                    UploadedDate = videoModel.UploadedDate,
                    dislikes = videoModel.dislikes,
                    likes = videoModel.likes,
                    LastWatchOffset = videoModel.LastWatchOffset,
                    Category = Convert.ToInt32(videoModel.Category),
                    DomainName = videoModel.DomainName,
                    VideoDuration = videoModel.VideoDuration,
                    LastWatched = videoModel.WatchedDate,
                    VideoId = videoModel.VideoId,
                    UserId = userid
                };


                context.Videos.Add(video);
                await context.SaveChangesAsync();

                return "created";
            }
            catch (Exception)
            {
                return "error";
            }
        }


        public async Task<string> PatchVideo(string link, string user, VideoModel videoModel)
        {
            try
            {
                VideoModel video = await GetVideoByLink(link, user);
                if (videoModel.likes != 0)
                    video.Category = videoModel.Category;
                if (videoModel.dislikes != 0)
                    video.dislikes = videoModel.dislikes;
                if (videoModel.LastWatched != null)
                    video.LastWatched = videoModel.LastWatched;
                if (!videoModel.LastWatchOffset.Equals(0))
                    video.LastWatchOffset = videoModel.LastWatchOffset;
                context.SaveChanges();
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public async Task<IList<VideoModel>> GetVideosWithPeople(List<string> people, string username)
        {
            try
            {
                people.ForEach(x => x.ToLower());

                int userid = (from user in context.Users where user.Username == username select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                IQueryable<VideoModel> list = from video in context.Videos.Include(y => y.Participants)
                                              where video.Participants.Any(x => people.Contains(x.Name))
                                              select new VideoModel
                                              {
                                                  VideoLink = video.VideoLink,
                                                  Title = video.Title,
                                                  WatchCount = video.WatchCount,
                                                  UploadedBy = video.UploadedBy,
                                                  UploadedDate = video.UploadedDate,
                                                  dislikes = video.dislikes,
                                                  likes = video.likes,
                                                  LastWatchOffset = video.LastWatchOffset,
                                                  Category = (CategoryModel)video.Category,
                                                  DomainName = video.DomainName,
                                                  VideoDuration = video.VideoDuration,
                                                  LastWatched = video.WatchedDate,
                                                  VideoId = video.VideoId,
                                                  Username = username
                                              };
                var result = await list.ToListAsync();
                if (result == null)
                    return new List<VideoModel>();
                return result;   

            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<IList<string>> GetTagsInVideo(string link, string username)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == username select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                IQueryable <ICollection<Tag>> tags = (from video in context.Videos
                                                     join tag in context.Videos on video.VideoId equals tag.VideoId
                                                     where video.UserId == userid
                                                     select video.Tags);
                return await tags.First().Select(x => x.TagName).AsQueryable().ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IList<VideoModel>> GetVideosWithCategory(List<CategoryModel> categories, string username)
        {
            try
            {
                List<int> categoryInt = categories.Select(x => Convert.ToInt32(x)).ToList();

                int userid = (from user in context.Users where user.Username == username select user.UserId).First();
                return await (from video in context.Videos
                              where categoryInt.Contains(video.Category) && video.UserId == userid
                              select new VideoModel
                              {
                                  VideoLink = video.VideoLink,
                                  Title = video.Title,
                                  WatchCount = video.WatchCount,
                                  UploadedBy = video.UploadedBy,
                                  UploadedDate = video.UploadedDate,
                                  dislikes = video.dislikes,
                                  likes = video.likes,
                                  LastWatchOffset = video.LastWatchOffset,
                                  Category = (CategoryModel)video.Category,
                                  DomainName = video.DomainName,
                                  VideoDuration = video.VideoDuration,
                                  LastWatched = video.WatchedDate,
                                  VideoId = video.VideoId,
                                  Username = username
                              }).ToListAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<IList<VideoModel>> GetVideosWithAnyTag(List<string> tags, string username)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == username select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
               

               var results = context.Tags.Where(tag => tags.Contains(tag.TagName)).Join(context.Videos, tag => tag.VideoId, video => video.VideoId, (tag, video) => new { video }).Select(x => x.video).Distinct().ToList();
                var list = new List<VideoModel>();
                foreach(var video in results)
                {
                    list.Add(new VideoModel
                    {
                        VideoLink = video.VideoLink,
                        Title = video.Title,
                        WatchCount = video.WatchCount,
                        UploadedBy = video.UploadedBy,
                        UploadedDate = video.UploadedDate,
                        dislikes = video.dislikes,
                        likes = video.likes,
                        LastWatchOffset = video.LastWatchOffset,
                        Category = (CategoryModel)video.Category,
                        DomainName = video.DomainName,
                        VideoDuration = video.VideoDuration,
                        LastWatched = video.WatchedDate,
                        VideoId = video.VideoId,
                        Username = username
                    });
                }
                return list;
                //return  (from video in results select new VideoModel
                //                {
                //                    VideoLink = video.VideoLink,
                //                    Title = video.Title,
                //                    WatchCount = video.WatchCount,
                //                    UploadedBy = video.UploadedBy,
                //                    UploadedDate = video.UploadedDate,
                //                    dislikes = video.dislikes,
                //                    likes = video.likes,
                //                    LastWatchOffset = video.LastWatchOffset,
                //                    Category = (CategoryModel)video.Category,
                //                    DomainName = video.DomainName,
                //                    VideoDuration = video.VideoDuration,
                //                    LastWatched = video.WatchedDate,
                //                    VideoId = video.VideoId,
                //                    Username = username
                //                });
               // return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<PlaylistModel>> GetVideoPlaylist(string link, string username)
        {
            try
            {
                int userid = (from user in context.Users where user.Username == username select user.UserId).FirstOrDefault();
                if (userid == 0)
                    return null;
                var videoId = await context.Videos.Where(x => x.VideoLink == link && x.UserId == userid).Select(x => x.VideoId).FirstOrDefaultAsync();
                List<int> list = await context.PlaylistVideos.Where(x => x.VideoId == videoId).Select(x => x.PlaylistId).ToListAsync();
               return await context.Playlists.Where(p => list.Contains(p.PlaylistId)).Select(playlist => new PlaylistModel
                {
                    PlaylistName = playlist.PlaylistName,
                    Description = playlist.Description,
                    DomainPlaylistId = playlist.DomainPlaylistId
                }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<List<VideoModel>> GetPlaylistVideos(string name, string username)
        {
            try
            {
                List<VideoModel> videoModels = new List<VideoModel>();

                ICollection<Video> videos = await (from playlist in context.Playlists where playlist.PlaylistName == name && playlist.username == username select playlist.Videos).FirstAsync();

                await videos.AsQueryable().ForEachAsync(video =>
                   videoModels.Add(new VideoModel
                   {
                       VideoLink = video.VideoLink,
                       Title = video.Title,
                       WatchCount = video.WatchCount,
                       UploadedBy = video.UploadedBy,
                       UploadedDate = video.UploadedDate,
                       dislikes = video.dislikes,
                       likes = video.likes,
                       LastWatchOffset = video.LastWatchOffset,
                       Category = (CategoryModel)video.Category,
                       DomainName = video.DomainName,
                       VideoDuration = video.VideoDuration,
                       LastWatched = video.WatchedDate,
                       VideoId = video.VideoId,
                       Username = username
                   }
               )
                   );

                return videoModels;
            }
            catch (Exception)
            {
                return null;
            }


        }



    }
}
