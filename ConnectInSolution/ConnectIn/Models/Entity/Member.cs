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
        #region Columns
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        public Group Group { get; set; }
        #endregion
    }
}