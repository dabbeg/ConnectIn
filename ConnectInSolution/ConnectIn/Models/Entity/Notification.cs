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
        #region Columns
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
        public string FriendUserId { get; set; }
        public string UserId { get; set; }
        public int GroupId { get; set; }
        public System.DateTime Date { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        #endregion

        public Notification()
        {
            Date = DateTime.Now;
        }
    }
}