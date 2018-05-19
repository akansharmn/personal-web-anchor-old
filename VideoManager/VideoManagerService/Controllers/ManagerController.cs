using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ADL;
using VideoManagerService.Models;

namespace VideoManagerService.Controllers
{
    public class ManagerController : ApiController
    {
        DbAccessor helper = new DbAccessor();

        [Route("api/Manager/{url}")]
        public GetVideoUsingLink_Result GetVideo(string url)
        {
          return helper.GetVideoByLink(url);
        }

        [Route("api/Manager/User/{id:int}")]
        public GetUserById1_Result GetUser(int id)
        {
            return helper.GetUserById(id);
        }
    }
}
