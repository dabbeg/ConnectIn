using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Comment
    {
        public int ID { get; set; }
        public int userID { get; set; }
        public int postID { get; set; }
        public string comment { get; set; }
        public System.DateTime date { get; set; }
    }
}