﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.Entity
{
    public class Group
    {
        [Key]
        public int ID { get; set; }
        public int name { get; set; }
    }
}