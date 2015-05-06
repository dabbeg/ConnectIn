using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class NotificationViewModel
    {
        public UserViewModel User { get; set; }
        public UserViewModel Friend { get; set; }
        public DateTime Date { get; set; }
        public bool IsPending { get; set; }
        public bool IsFriendRequest { get; set; }
    }
}