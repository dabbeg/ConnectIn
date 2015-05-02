using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public int FriendUserId { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }

        public Notification()
        {
            Date = DateTime.Now;
        }
    }
}