using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConnectIn.Models.Entity
{
    public class Group
    {
        #region Columns
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string AdminID { get; set; }
        #endregion

        #region ForeignKeys
        public ICollection<Member> Members { get; set; }
        #endregion
    }
}