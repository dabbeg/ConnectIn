using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Friend
    {
        public int ID { get; set; }
        public int userID { get; set; }
        public int friendUserID { get; set; }
        public bool bestFriend { get; set; }
        public bool family { get; set; }
    }
}