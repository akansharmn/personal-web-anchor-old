using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADL;

namespace VideoManagerUnitTest
{
    internal class TestContext : DbContext, IVideoManagerContext
    {
        public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Channel> Channels{get; set;}
        public DbSet<Participant> Participants { get; set; }
        public TestContext()
        {
            this.Users = new UserDbSet();
            this.Videos = new VideoDbSet();
            this.Playlists = new PlaylistDbSet();
            this.Channels = new ChannelDbSet();
            this.Tags = new TagDbSet();
            this.Participants = new ParticipantDbList();
        }

        public int SaveChanges()
        {
            return 1;
        }

        public async Task<int> SaveChangesAsync()
        {
            await Task.Delay(2000);
            return 1;
        }
    }
}
