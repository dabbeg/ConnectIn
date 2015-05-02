using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class LikeDislike
    {
        [Key]
        [Column(Order = 0)]
        public int PostId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        public bool Like { get; set; }
        public bool Dislike { get; set; }

        //[ForeignKey("postID")]
        public Post Post { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}