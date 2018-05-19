using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADL
{
    public interface IVideoManagerContext 
    {
        int SaveChanges();
         DbSet<User> Users { get; set; }
         DbSet<Video> Videos { get; set; }
        DbSet<Playlist> Playlists { get; set; }
        DbSet<Channel> Channels { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<Tag> Tags { get; set; }
         DbSet<PlaylistVideo> PlaylistVideos { get; set; }

        Task<int> SaveChangesAsync();



    }
}
