using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Photo
    {
        public int ID { get; set; }
        public string photo { get; set; }
        public int userID { get; set; }
        public bool isProfilePicture { get; set; }
    }
}