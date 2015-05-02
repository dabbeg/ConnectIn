using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Member
    {
        [Key]
        [Column(Order = 0)]
        public string userID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int groupID { get; set; }

        [ForeignKey("userID")]
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("groupID")]
        public Group Group { get; set; }
    }
}