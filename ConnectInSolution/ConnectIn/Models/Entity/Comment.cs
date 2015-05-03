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
        #region Columns
        [Key]
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public System.DateTime Date { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        public Post Post { get; set; }
        #endregion

        public Comment()
        {
            Date = DateTime.Now;
        }
    }
}