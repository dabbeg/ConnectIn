using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class About
    {
        public int ID { get; set; }
        public System.DateTime birthday { get; set; }
        public string work { get; set; }
        public string school { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public bool privacy { get; set; }
    }
}