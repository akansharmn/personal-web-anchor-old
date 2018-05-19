using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class VideoManagerServiceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public VideoManagerServiceContext() : base("name = VideoManagerServiceContext")
        { }

        public System.Data.Entity.DbSet<VideoManagerService.Models.Video> Videos { get; set; }

        public System.Data.Entity.DbSet<VideoManagerService.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<VideoManagerService.Models.Channel> Channels { get; set; }

        public System.Data.Entity.DbSet<VideoManagerService.Models.Playlist> Playlists { get; set; }

        public System.Data.Entity.DbSet<VideoManagerService.Models.Tag> Tags { get; set; }

        public System.Data.Entity.DbSet<VideoManagerService.Models.Participant> Participants { get; set; }
    }
}
