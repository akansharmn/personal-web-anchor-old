using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class VideoChannel
    {

        public int VideoId { get; set; }

        public Video Video { get; set; }

        public int ChannelId { get; set; }

        public Channel Channel { get; set; }
    }
}