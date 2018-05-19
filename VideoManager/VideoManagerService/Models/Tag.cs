using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{
    public class Tag
    {
        [Key, Column(Order = 0)]
        public string TagName { get; set; }

        [Key, Column(Order = 1)]
        public int VideoId { get; set; }

        public Video Video { get; set; }
    }
}