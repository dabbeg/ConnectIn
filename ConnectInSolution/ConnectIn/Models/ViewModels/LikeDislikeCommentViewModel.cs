using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class LikeDislikeCommentViewModel
    {
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
    }
}