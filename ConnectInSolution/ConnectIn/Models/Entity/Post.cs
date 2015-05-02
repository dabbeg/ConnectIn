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
        public int ID { get; set; }
        public string text { get; set; }
        public string userID { get; set; }
        public System.DateTime date { get; set; }

        //[ForeignKey("userID")]
        //public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like_Dislike> LikesDislikes { get; set; }

        public Post()
        {
            date = DateTime.Now;
        }
    }
}