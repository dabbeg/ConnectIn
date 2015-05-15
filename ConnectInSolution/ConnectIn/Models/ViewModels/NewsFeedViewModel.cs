using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class NewsFeedViewModel
    {
        [Required]
        public List<PostsViewModel> Posts { get; set; }
        public string Id { get; set; }
        public UserViewModel User { get; set; }
    }
}