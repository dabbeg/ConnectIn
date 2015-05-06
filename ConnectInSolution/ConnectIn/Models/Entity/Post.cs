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
        #region Columns
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }
        public int? GroupId { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<LikeDislike> LikesDislikes { get; set; }
        #endregion

        public Post()
        {
            Date = DateTime.Now;
        }
    }
}