using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADL;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
           

            var context = new masterEntities();

            //context.Users.Add(new User { DateOfRegistartion = DateTime.UtcNow, Email = "abc@gmail.com", Domain = "youtube.com" });
            //context.SaveChanges();

            //VideoResult r = context.GetVideoUsingLink("youtube.com/asfddhd").FirstOrDefault();
            
        }
    }
}
