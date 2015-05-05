using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class ProfileViewModel
    {
        public List<PostsViewModel> Posts { get; set; }
        public UserViewModel User { get; set; }
    }
}