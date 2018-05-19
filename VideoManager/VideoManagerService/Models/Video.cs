using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class Video
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }

        public string VideoLink { get; set; }

        public long likes { get; set; }

        public long dislikes { get; set; }
        
        public string UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public Category Category { get; set; }

        public DateTime WatchedDate { get; set; }

        public string Title { get; set; }


        public int WatchCount { get; set; }

        public DateTime LastWatched { get; set; }

        public decimal LastWatchOffset { get; set; }

        public decimal VideoDuration { get; set; }

        public string DomainName { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}