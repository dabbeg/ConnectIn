using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class FriendViewModel
    {
        public UserViewModel User { get; set; }
        public string BfStar { get; set; }
        public string FStar { get; set; }
    }
}