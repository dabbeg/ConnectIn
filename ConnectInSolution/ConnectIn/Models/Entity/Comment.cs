using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public System.DateTime Date { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }

        //[ForeignKey("postID")]
        public Post Post { get; set; }


        public Comment()
        {
            Date = DateTime.Now;
        }
    }
}