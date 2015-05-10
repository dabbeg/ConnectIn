using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Friend
    {
        #region Columns
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string FriendUserId { get; set; }
        public bool UserConsidersFriendAsBestFriend { get; set; }
        public bool FriendConsidersUsersAsBestFriend { get; set; }
        public bool UserConsidersFriendAsFamily { get; set; }
        public bool FriendConsidersUsersAsFamily { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        #endregion
    }
}