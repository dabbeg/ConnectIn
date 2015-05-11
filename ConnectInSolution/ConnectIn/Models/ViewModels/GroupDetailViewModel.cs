using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;

namespace ConnectIn.Models.ViewModels
{
    public class GroupDetailViewModel
    {
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string AdminId { get; set; }
        public ICollection<UserViewModel> Members { get; set; }
        public NewsFeedViewModel Posts { get; set; }
        public ICollection<UserViewModel> FriendsOfUser { get; set; }

        public UserViewModel User { get; set; }
    }
}