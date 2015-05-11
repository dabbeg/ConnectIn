using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class PostsViewModel
    {
        public int PostId { get; set; }
        public string Body { get; set; }
        public UserViewModel User { get; set; }
        public DateTime DateInserted { get; set; }
        public LikeDislikeCommentViewModel LikeDislikeComment { get; set; }
        public int? GroupId { get; set; }
        public string LikePic { get; set; }
        public string DislikePic { get; set; }
        public bool isUserPostOwner { get; set; }
    }
}