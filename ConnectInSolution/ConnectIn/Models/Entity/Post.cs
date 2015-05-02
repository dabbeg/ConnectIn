using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<LikeDislike> LikesDislikes { get; set; }

        public Post()
        {
            Date = DateTime.Now;
        }
    }
}