using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Models.Models;

namespace Models
{
    public class VideoModel : Links
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }

        [Required]
        public string VideoLink { get; set; }

        public long likes { get; set; }

        public long dislikes { get; set; }
        
        public string UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public CategoryModel Category { get; set; }

        public DateTime WatchedDate { get; set; }

        public string Title { get; set; }


        public int WatchCount { get; set; }

        public DateTime LastWatched { get; set; }

        public decimal LastWatchOffset { get; set; }

        public decimal VideoDuration { get; set; }

        public IDomain Domain { get; set; }

        public string Username { get; set; }

       
    }
}