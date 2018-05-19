using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADL
{
    class SQLiteDbContext :DbContext, IVideoManagerContext
    {
        public SQLiteDbContext(string conn):
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder()
                {
                    DataSource = conn, ForeignKeys = true
                }.ConnectionString
            }, true)
        {
            Database.SetInitializer<SQLiteDbContext>(new Initializer());
        }

        public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
        public DbSet<User> Users { get ; set ; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Channel> Channels  { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Video>().HasMany<Playlist>(v => v.Playlists).WithMany(x => x.Videos);
            //modelBuilder.Entity<Playlist>().HasMany<Video>(p => p.Videos).WithMany(v => v.Playlists);
            base.OnModelCreating(modelBuilder);
        }

    }

    internal class Initializer : CreateDatabaseIfNotExists<SQLiteDbContext>
    {
        protected  override void Seed(SQLiteDbContext context)
        {
            //context.Users.Add(new User { DateOfRegistartion = DateTime.Now, Domain = "youtube.com", Email = "testuser@mail.com", Username = "testuser" });
            //context.Users.Add(new User { DateOfRegistartion = DateTime.Now, Domain = "youtube.com", Email = "sampleuser@mail.com", Username = "sampleuser" });

            //context.Videos.Add(new Video { UserId = 1, Category = 1, dislikes = 100, DomainName = "youtueb.com", LastWatched = DateTime.Now, LastWatchOffset = new Decimal(2.3), likes = 1000, Title = "Testing", UploadedBy = "Tester", UploadedDate = DateTime.Now, VideoDuration = 6, VideoLink = "youtube.com/test", WatchCount = 300, WatchedDate = DateTime.Today });
            //context.Videos.Add(new Video { UserId = 2, Category = 1, dislikes = 100, DomainName = "youtueb.com", LastWatched = DateTime.Now, LastWatchOffset = new Decimal(2.3), likes = 1000, Title = "Testing", UploadedBy = "Tester", UploadedDate = DateTime.Now, VideoDuration = 6,  VideoLink = "youtube.com/test1", WatchCount = 300, WatchedDate = DateTime.Today });

            context.Participants.Add(new Participant
            {
                Name = "TestParticipant1",
                VideoId = 1
            });
            context.Participants.Add(new Participant
            {
                Name = "TestParticipant2",
                VideoId = 1
            });
            context.Participants.Add(new Participant
            {
                Name = "TestParticipant1",
                VideoId = 2
            });
            context.Tags.Add(new Tag
            {
                TagName = "movie",
                VideoId = 1
            });
            context.Tags.Add(new Tag
            {
                TagName = "favourite",
                VideoId = 1
            });
            context.Tags.Add(new Tag
            {
                TagName = "movie",
                VideoId = 2
            });


           

            context.SaveChanges();


        }

       
    }
}
