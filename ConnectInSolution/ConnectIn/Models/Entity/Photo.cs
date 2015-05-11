using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Photo
    {
        #region Columns
        [Key]
        public int PhotoId { get; set; }
        public int PhotoId2 { get; set; }
        public string PhotoPath { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }
        public bool IsProfilePicture { get; set; }
        public bool IsCoverPhoto { get; set; }
        #endregion

        #region ForeignKeys
        public User User { get; set; }
        #endregion


        #region Contructors
        public Photo()
        {
            Date = DateTime.Now;
        }
        #endregion
    }
}