using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADL;

namespace VideoManagerUnitTest
{
    public class UserDbSet : TestDbSet<User>
    {
        public UserDbSet()
        {
            //this.Add(new User
            //{
            //    UserId = 1,
            //    Username = "testuser",
            //    DateOfRegistartion = DateTime.Now,
            //    Domain = "youtube.com",
            //    Email = "test@miracle.com",
            //});
        }
        public override User Find(params object[] keyValues)
        {
           return this.FirstOrDefault(user => user.UserId == (int)keyValues.Single());
        }
    }
}
