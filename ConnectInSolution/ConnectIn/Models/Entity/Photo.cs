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
        public int photoID { get; set; }
        public string photo { get; set; }
        public string userID { get; set; }
        public bool isProfilePicture { get; set; }

        [ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}