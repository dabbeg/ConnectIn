﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;

namespace ConnectIn.Models.ViewModels
{
    public class GroupDetailViewModel
    {
        public string Name { get; set; }
        public int GroupId { get; set; }
        public UserViewModel User { get; set; } 
    }
}