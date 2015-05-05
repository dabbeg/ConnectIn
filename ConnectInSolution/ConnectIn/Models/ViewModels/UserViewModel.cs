using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Birthday { get; set; }
        public string ProfilePicture { get; set; }
    }
}