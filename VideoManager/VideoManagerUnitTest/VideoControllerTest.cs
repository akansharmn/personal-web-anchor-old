using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using ADL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using VideoManagerService.Controllers;

namespace VideoManagerUnitTest
{ 
    
    [TestClass]
    public class VideoControllerTest
    {
    private VideoModel GetDemoVideoModelInstance(int id)
    {
        return new VideoModel
        {
            VideoDuration = 6,
            VideoLink = "test.com/check",
            WatchCount = 4,
            UploadedDate = DateTime.Now,
            UploadedBy = "aknsh",
            likes = 2000,
            dislikes = 100,
            DomainName = "youtube",
            Category = CategoryModel.Audio,
            LastWatched = DateTime.Now,
            Title = "test",
            WatchedDate = DateTime.Now,
            LastWatchOffset = 4,
            Username = "testuser",
            VideoId = id

        };
    }

        private Video GetDemoVideoInstance(int id)
        {
        return new Video
        {
            VideoDuration = 6,
            VideoLink = "test.com/check",
            WatchCount = 4,
            UploadedDate = DateTime.Now,
            UploadedBy = "aknsh",
            likes = 2000,
            dislikes = 100,
            DomainName = "youtube",
            Category = 1,
            LastWatched = DateTime.Now,
            Title = "test",
            WatchedDate = DateTime.Now,
            LastWatchOffset = 4,
            UserId = 1,
            VideoId = id
            };
        }

        //private IList<VideoModel> GetDemoVideoList()
        //{

        //         List<VideoModel> list = new List<VideoModel>();
        //         for(int i = 0; i< 10; i++)
        //        {
        //            list.Add(GetDemoVideoInstance());
        //        }
        //        return list;
        //}


    [TestMethod]
    public async Task TestPutVideo_ReturnsSucess()
    {
        var context = new TestContext();
       
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            controller.Request = new System.Net.Http.HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/Video")
            };
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "GetVideoByLink",
                routeTemplate: "api/GetVideo"
               );
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "Video" } });
            var video = GetDemoVideoModelInstance(1);
            var result = await controller.PostVideo(video) as CreatedAtRouteNegotiatedContentResult<VideoModel>;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.RouteName, "GetVideoByLink");
            Assert.AreEqual(result.RouteValues["link"], "test.com/check");
            Assert.AreEqual(result.RouteValues["user"], "testuser");



        }

        [TestMethod]
        public async Task TestPostVideo_NoSuchUser_ResturnError()
        {
            var context = new TestContext();

            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var mockurl = new Mock<UrlHelper>();
            mockurl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("https://testlocation");
            controller.Url = mockurl.Object;
            var video = GetDemoVideoModelInstance(1);
            video.Username = "nosuchuser";

            var result = await controller.PostVideo(video);

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestPostVideo_ReturnError()
        {
            var context = new TestContext();

            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var mockurl = new Mock<UrlHelper>();
            mockurl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("https://testlocation");
            controller.Url = mockurl.Object;
            var video = GetDemoVideoModelInstance(1);
            video.VideoLink = null;
            var result = await controller.PostVideo(video);

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestGetVideos_ReturnVideos()
        {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });

            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
           
        var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
        context.Videos.Add(GetDemoVideoInstance(1));
        context.Videos.Add(GetDemoVideoInstance(2));
        context.Videos.Add(GetDemoVideoInstance(3));
        var result = await controller.GetVideos("testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 3);
                


        }

        [TestMethod]
        public async Task TestGetVideos_NoSuchUser_ReturnsBadRequest()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);

            var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(1));
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Videos.Add(GetDemoVideoInstance(3));
            var result = await controller.GetVideos("nosuchuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));



        }

        [TestMethod]
    public async Task TestGetVideoByLink_ReturnsVideo()
       {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
            
        var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(1));
        var result = await controller.GetVideoByLink("test.com/check", "testuser") as OkNegotiatedContentResult<VideoModel>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.VideoId, 1);
      }

        [TestMethod]
        public async Task TestGetVideoByLink_EmptyParameters_ReturnsBadRequest()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByLink(null, null) ;
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestGetVideoByLink_NoSuchUser_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByLink("test.com/check", "nosuchuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestGetVideoByLink_NoSuchVideo_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByLink("nosuchlink", "testuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
    public async Task TestGetVideoByTitle_ReturnVideo()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
        var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            context.Videos.Add(GetDemoVideoInstance(1));
        var result = await controller.GetVideoByTitle("test", "testuser") as OkNegotiatedContentResult<VideoModel>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Title, "test");
    }


        [TestMethod]
        public async Task TestGetVideoByTitle_NoSuchVideoReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByTitle("test", "nosuchtitle");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestGetVideoByTitle_NoSuchUser_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByTitle("test", "nosuchuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }
              [TestMethod]
        public async Task TestGetVideoByTitle_InvalidParameters_ReturnVideo()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            context.Videos.Add(GetDemoVideoInstance(1));
            var result = await controller.GetVideoByTitle(null, null);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            
        }



    [TestMethod]
    public async Task TestGetVideoWithPeople_ReturnsVideos()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
            
            var video = GetDemoVideoInstance(1);
           
            video.Participants.Add(new Participant
            {
                Name = "tester"
            });
            context.Videos.Add(video);
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
           
        var result = await controller.GetVideosWithPeople(new List<string> { "tester" }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 1);
    }

        [TestMethod]
        public async Task TestGetVideoWithPeople_NoSuchUser_ReturnsBadRequest()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var video = GetDemoVideoInstance(1);

            video.Participants.Add(new Participant
            {
                Name = "tester"
            });
            context.Videos.Add(video);
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;

            var result = await controller.GetVideosWithPeople(new List<string> { "tester" }, "nosuchuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }


        [TestMethod]
        public async Task TestGetVideoWithPeople_NoVideoWithSuchPeople_ReturnsNotFound()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var video = GetDemoVideoInstance(1);

            video.Participants.Add(new Participant
            {
                Name = "tester"
            });
            context.Videos.Add(video);
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;

            var result = await controller.GetVideosWithPeople(new List<string> { "nosuchperson" }, "testuser");
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task TestGetVideoWithPeople_InvalidParameters_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });

            var video = GetDemoVideoInstance(1);

            video.Participants.Add(new Participant
            {
                Name = "tester"
            });
            context.Videos.Add(video);
            var controller = new VideosController(context);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;

            var result = await controller.GetVideosWithPeople(null, "nosuchuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
    public async Task TestGetVideoWithAnyTag_ReturnVideos()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Videos.Add(GetDemoVideoInstance(1));



           
            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var result = await controller.GetVideosWithAnyTag(new List<string> { "test", "nonfun" }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 2);
    }

        [TestMethod]
        public async Task TestGetVideoWithAnyTag_ReturnsNotFound()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Videos.Add(GetDemoVideoInstance(1));




            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var result = await controller.GetVideosWithAnyTag(new List<string> { "nosuchtag", "nosuchtag2" }, "testuser");
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task TestGetVideoWithAnyTag_InvalidParameters_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Videos.Add(GetDemoVideoInstance(1));




            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var result = await controller.GetVideosWithAnyTag(new List<string> { "nosuchtag", "nosuchtag2" }, null);
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
    public async Task TestGetVideosWithAllTags_ReturnsVideos()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
        var controller = new VideosController(context);
        var video = GetDemoVideoInstance(1);
            context.Videos.Add(GetDemoVideoInstance(1));
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithAllTags(new List<string> { "test", "fun" }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 1);
    }

        [TestMethod]
        public async Task TestGetVideosWithAllTags_ReturnsNotFound()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var video = GetDemoVideoInstance(1);
            context.Videos.Add(GetDemoVideoInstance(1));
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithAllTags(new List<string> { "test", "fun" }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(result.Content.Count, 1);
        }

        [TestMethod]
        public async Task TestGetVideosWithAllTags_InvalidParameters__ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var video = GetDemoVideoInstance(1);
            context.Videos.Add(GetDemoVideoInstance(1));
            context.Videos.Add(GetDemoVideoInstance(2));
            context.Tags.Add(new Tag { TagName = "test", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "fun", VideoId = 1 });
            context.Tags.Add(new Tag { TagName = "nontest", VideoId = 2 });
            context.Tags.Add(new Tag { TagName = "nonfun", VideoId = 2 });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithAllTags(new List<string> { "test", "fun" }, "");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
    public async Task TestVideosWithCategory_ReturnsVideos()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
        var controller = new VideosController(context);
        var video = GetDemoVideoInstance(1);
        context.Videos.Add(video);
        video = GetDemoVideoInstance(2);
        video.Category =0;
        context.Videos.Add(video);
        video = GetDemoVideoInstance(3);
        video.Category = 2;
        context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithCategory(new List<CategoryModel> { CategoryModel.Movie, CategoryModel.Audio }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 2);
    }

        [TestMethod]
        public async Task TestVideosWithCategory_ReturnsNotFound()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var video = GetDemoVideoInstance(1);
            context.Videos.Add(video);
            video = GetDemoVideoInstance(2);
            video.Category = 0;
            context.Videos.Add(video);
            video = GetDemoVideoInstance(3);
            video.Category = 2;
            context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithCategory(new List<CategoryModel> { CategoryModel.Movie, CategoryModel.Audio }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(result.Content.Count, 2);
        }

        [TestMethod]
        public async Task TestVideosWithCategory_InvalidParameters_ReturnsBadResult()
        {
            var context = new TestContext();
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            var video = GetDemoVideoInstance(1);
            context.Videos.Add(video);
            video = GetDemoVideoInstance(2);
            video.Category = 0;
            context.Videos.Add(video);
            video = GetDemoVideoInstance(3);
            video.Category = 2;
            context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosWithCategory(new List<CategoryModel> { CategoryModel.Movie, CategoryModel.Audio }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(result.Content.Count, 2);
        }

        [TestMethod]
    public async Task TestGetVideosWithPeople()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            var controller = new VideosController(context);
            controller.Url = mockUrl.Object;
        var video = GetDemoVideoInstance(1);
        video.Participants.Add(new Participant
        {
            Name = "tester"
        });
        video.Participants.Add(new Participant
        {
            Name = "tester1"
        });
        context.Videos.Add(video);
        video = GetDemoVideoInstance(2);
        video.Participants.Add(new Participant
        {
            Name = "tester"
        });
       
        context.Videos.Add(video);
        video = GetDemoVideoInstance(3);
        video.Participants.Add(new Participant
        {
            Name = "tester3"
        });
        video.Participants.Add(new Participant
        {
            Name = "tester4"
        });
        context.Videos.Add(video);
        var result = await controller.GetVideosWithPeople(new List<string> { "tester" }, "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 2);
    }

    [TestMethod]
    public async Task TestGetVideosInPlaylist_ReturnsVideos()
    {
        var context = new TestContext();
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "testuser"
        });
        var controller = new VideosController(context);
            context.Playlists.Add(new Playlist
            {
                PlaylistName = "testPlaylist",
                PlaylistId = 1,
                username = "testuser"
            });
            context.Playlists.Add(new Playlist
            {
                PlaylistName = "testPlaylistfun",
                PlaylistId = 2,
                username = "testuser"
            });
        var video = GetDemoVideoInstance(1);
        video.Playlists.Add(new Playlist
        {            
            PlaylistId = 1,            
        });
        context.Videos.Add(video);

        video = GetDemoVideoInstance(2);
        video.Playlists.Add(new Playlist
        {
            PlaylistId = 1,
        });
        context.Videos.Add(video);

        video = GetDemoVideoInstance(2);
        video.Playlists.Add(new Playlist
        {
            
            PlaylistId = 2
        });
        context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosInPlayList("testPlaylist", "testuser") as OkNegotiatedContentResult<IList<VideoModel>>;
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Content);
        Assert.AreEqual(result.Content.Count, 2);
    } 


        [TestMethod]
        public async Task TestGetVideosInPlaylist_ReturnsBadResult()
        {
            var context = new TestContext();
            var playlist1 = new Playlist
            {
                PlaylistName = "testPlaylist",
                PlaylistId = 1,
                username = "testuser"
            };
            var playlist2 = new Playlist
            {
                PlaylistName = "testPlaylistfun",
                PlaylistId = 2,
                username = "testuser"
            };

            context.Playlists.Add(playlist1);
            context.Playlists.Add(playlist2);

            
            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);
            
            var video = GetDemoVideoInstance(1);
            video.Playlists.Add(playlist1);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(2);
            video.Playlists.Add(playlist2);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(3);
            video.Playlists.Add(playlist2);
            context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosInPlayList("nosuchplaylist", "testuser");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public async Task TestGetVideosInPlaylist_ReturnsNotFound()
        {
            var context = new TestContext();
            var playlist1 = new Playlist
            {
                PlaylistName = "testPlaylist",
                PlaylistId = 1,
                username = "testuser"
            };
            var playlist2 = new Playlist
            {
                PlaylistName = "testPlaylist2",
                PlaylistId = 2,
                username = "testuser"
            };

            context.Playlists.Add(playlist1);
            context.Playlists.Add(playlist2);


            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);

            var video = GetDemoVideoInstance(1);
            video.Playlists.Add(playlist1);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(2);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(3);
            context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosInPlayList("testPlaylist2", "testuser");
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task TestGetVideosInPlaylist_InvalidParaameters_ReturnsBadResult()
        {
            var context = new TestContext();
            var playlist1 = new Playlist
            {
                PlaylistName = "testPlaylist",
                PlaylistId = 1,
                username = "testuser"
            };
            var playlist2 = new Playlist
            {
                PlaylistName = "testPlaylistfun",
                PlaylistId = 2,
                username = "testuser"
            };

            context.Playlists.Add(playlist1);
            context.Playlists.Add(playlist2);


            context.Users.Add(new User
            {
                UserId = 1,
                Username = "testuser"
            });
            var controller = new VideosController(context);

            var video = GetDemoVideoInstance(1);
            video.Playlists.Add(playlist1);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(2);
            video.Playlists.Add(playlist2);
            context.Videos.Add(video);

            video = GetDemoVideoInstance(3);
            video.Playlists.Add(playlist2);
            context.Videos.Add(video);
            var location = "http://location/";
            var mockUrl = new Mock<UrlHelper>();
            mockUrl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
            controller.Url = mockUrl.Object;
            var result = await controller.GetVideosInPlayList("nosuchplaylist", "");
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }
    }
}
