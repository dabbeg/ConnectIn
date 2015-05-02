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
        public string userID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int friendUserID { get; set; }
        public bool bestFriend { get; set; }
        public bool family { get; set; }

        [ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}