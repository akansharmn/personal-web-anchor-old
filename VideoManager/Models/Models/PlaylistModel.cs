﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class PlaylistModel : Links
    {
        public string PlaylistName { get; set; }

        public int PlaylistId { get; set; }


        public int DomainPlaylistId { get; set; }

        public string Description { get; set; }
    }
}