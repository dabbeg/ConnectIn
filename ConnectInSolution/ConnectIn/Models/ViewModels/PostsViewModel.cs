﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class PostsViewModel
    {
        public string Body { get; set; }
        public DateTime DateInserted { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}