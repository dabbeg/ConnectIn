using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Group
    {
        #region Columns
        [Key]
        public string Id { get; set; }
        public int Name { get; set; }
        #endregion

        #region ForeignKeys
        public ICollection<Member> Members { get; set; }
        #endregion
    }
}