using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VideoManagerService.Models
{

    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string Username;

        public string Email { get; set; }

        public  DateTime DateOfRegistartion {get; set;}

        public string Domain { get; set; }

    }
}