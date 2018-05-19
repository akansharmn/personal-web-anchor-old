using System;
using System.Net.Http;
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
    public class UserControllerTest
    {
       
        [TestMethod]
        public async Task Test_GetUser_ResturnsUser()
        {
            var context = new TestContext();
            var controller = new UserController(context);
            var mockurl = new Mock<UrlHelper>();
            mockurl.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("https:\\testlocation");
            controller.Url = mockurl.Object;
            context.Users.Add(
                new User
                {
                    UserId = 1,
                    Username = "testuser"
                }
            );


            var result =await controller.GetUser("testuser") as OkNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(result.Content.UserId, 1);

        }

        [TestMethod]
        public async  Task Test_GetUser_ReturnsNotFound()
        {
            var context = new TestContext();
            var controller = new UserController(context);
            context.Users.Add(
                new User
                {
                    UserId = 1,
                    Username = "testuser"
                }
            );


            var result = await controller.GetUser("testuser") as OkNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
            Assert.AreEqual(result.Content.UserId, 1);

        }

        [TestMethod]
        public async Task TestPostUser_ResturnsSuccess()
        {
            var context = new TestContext();
            
            var controller = new UserController(context);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/User")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "GetUserByUsername",
                routeTemplate: "api/{controller}/{username}"               
                );

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "User" } });

            UserModel user = new UserModel()
            {
                UserId = 1,
                Username = "testuser",
                DateOfRegistartion = DateTime.UtcNow,
                Domain = "test.com",
                Email = "testuser@test.com"
            };
            var response = await controller.PostUser(user);
            Assert.AreEqual(response.Headers.Location, "http://localhost/api/User/testuser");
        }


        [TestMethod]
        public async Task  TestPostUser_ReturnsInvallidObject()
        {
            var context = new TestContext();

            var controller = new UserController(context);
            string url = "http://location/";

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(url);
            controller.Url = mockUrlHelper.Object;
            UserModel user = new UserModel()
            {
                UserId = 1,
                DateOfRegistartion = DateTime.UtcNow,
                Domain = "test.com",
                Email = "hdhddjdjjdjdj"
            };
            var response = await controller.PostUser(user);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task TestDeleteUser_ReturnsDeleted()
        {
            var context = new TestContext();

            var controller = new UserController(context);
            string url = "http://location/";

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(url);
            controller.Url = mockUrlHelper.Object;
            User user = new User()
            {
                UserId = 1,
                DateOfRegistartion = DateTime.UtcNow,
                Domain = "test.com",
                Email = "test",
                Username = "tester"
            };
            context.Users.Add(user);
            var response = await controller.DeleteUser("tester");
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
        }


        [TestMethod]
        public async Task TestDeleteUser_ReturnsBadRequest()
        {
            var context = new TestContext();

            var controller = new UserController(context);
            string url = "http://location/";

            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(url);
            controller.Url = mockUrlHelper.Object;
            User user = new User()
            {
                UserId = 1,
                DateOfRegistartion = DateTime.UtcNow,
                Domain = "test.com",
                Email = "test",
                Username = "tester"
            };
            context.Users.Add(user);
            var response = await controller.DeleteUser("testernotfound");
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.BadRequest);
        }
    }
}
