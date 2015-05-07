using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Models.ViewModels
{
    public class PhotoAlbumViewModel
    {
        public List<ImageViewModel> Images { get; set; }
        public string UserId { get; set; }
    }
}