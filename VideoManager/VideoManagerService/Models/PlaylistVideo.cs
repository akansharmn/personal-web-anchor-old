using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class PlaylistVideo
    {
        public int VideoId { get; set; }

        public Video Video { get; set; }

        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }
    }
}