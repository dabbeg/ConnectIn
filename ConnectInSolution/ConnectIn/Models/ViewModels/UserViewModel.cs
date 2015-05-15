using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ConnectIn.Models.ViewModels
{
    public class UserViewModel
    {
        
        [Required(ErrorMessage = "Name required")]  
        public string Name { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Birthday { get; set; }
        public string ProfilePicture { get; set; }
        public string CoverPhoto { get; set; }
        public string Gender { get; set; }        
        public string Work { get; set; }
        public string School { get; set; }
        public string Address { get; set; }
        public string BfStar { get; set; }
        public string FStar { get; set; }
         
    }
 
}