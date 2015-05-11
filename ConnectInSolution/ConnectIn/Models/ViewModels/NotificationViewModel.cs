using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; }
        public UserViewModel User { get; set; }
        public UserViewModel Friend { get; set; }
        public string GroupId { get; set; }
        public GroupDetailViewModel Group { get; set; }
        public DateTime Date { get; set; }
    }
}