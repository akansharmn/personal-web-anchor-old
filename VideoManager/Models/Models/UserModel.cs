using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace   Models
{

    public class UserModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Username;

        [Required(AllowEmptyStrings = false), MaxLength(4)]
        public string Email { get; set; }


        public  DateTime DateOfRegistartion {get; set;}

        public string Domain { get; set; }

    }
}