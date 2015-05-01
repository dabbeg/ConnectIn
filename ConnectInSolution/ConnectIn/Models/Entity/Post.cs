using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Post
    {
        public int ID { get; set; }
        public string text { get; set; }
        public int userID { get; set; }
        public System.DateTime date { get; set; }
    }
}