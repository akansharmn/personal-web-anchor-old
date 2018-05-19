using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class Channel
    {
        public int ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string CreatorName { get; set; }

        public string PlatformName { get; set; }
    }
}