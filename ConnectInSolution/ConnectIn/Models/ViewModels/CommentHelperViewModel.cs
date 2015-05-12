using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;

namespace ConnectIn.Models.ViewModels
{
    public class CommentHelperViewModel
    {
        public List<CommentViewModel> Comments { get; set; }
        public PostsViewModel Post { get; set; }

        public UserViewModel User { get; set; }
    }
}