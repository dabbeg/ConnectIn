using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Notification
    {
        public int friendUserID { get; set; }
        public int userID { get; set; }
        public System.DateTime date { get; set; }
    }
}