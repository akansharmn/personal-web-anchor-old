using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ChannelModel
    {
        public int ChannelId { get; set; }

        public string ChannelName { get; set; }

        public string CreatorName { get; set; }

        public string PlatformName { get; set; }
    }
}