using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Post
    {
        [Key]
        public int ID { get; set; }
        public string text { get; set; }
        public int userID { get; set; }
        public System.DateTime date { get; set; }
    }
}