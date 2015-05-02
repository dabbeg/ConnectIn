using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Like_Dislike
    {
        [Key]
        [Column(Order = 0)]
        public int postID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int userID { get; set; }
        public bool like { get; set; }
        public bool dislike { get; set; }

        [ForeignKey("postID")]
        public Post Post { get; set; }

        //[ForeignKey("userID")]
        //public ApplicationUser ApplicationUser { get; set; }
    }
}