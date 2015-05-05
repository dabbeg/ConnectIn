using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Birthday { get; set; }
        public string ProfilePicture { get; set; }
    }
}