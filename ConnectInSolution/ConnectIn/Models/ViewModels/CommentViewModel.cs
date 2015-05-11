using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;

namespace ConnectIn.Models.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public UserViewModel User { get; set; }
        public string Body { get; set; }
        public DateTime DateInserted { get; set; }
        public bool IsUserCommentOwner { get; set; }

    }
}