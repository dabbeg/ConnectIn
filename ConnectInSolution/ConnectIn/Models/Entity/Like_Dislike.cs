using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Like_Dislike
    {
        public int postID { get; set; }
        public int userID { get; set; }
        public bool like { get; set; }
        public bool dislike { get; set; }
    }
}