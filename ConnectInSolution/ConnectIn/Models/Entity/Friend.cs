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
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string FriendUserId { get; set; }
        public bool BestFriend { get; set; }
        public bool Family { get; set; }

        //[ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}