using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Photo
    {
        [Key]
        public int PhotoId { get; set; }
        public string PhotoPath { get; set; }
        public string UserId { get; set; }
        public bool IsProfilePicture { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}