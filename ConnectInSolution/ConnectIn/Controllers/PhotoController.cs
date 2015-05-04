using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConnectIn.Controllers
{
    public class PhotoController : Controller
    {
        //
        // GET: /Photo/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            List<Photo> all = new List<Photo>();
            //DataEntities are our datacontext
            using (DatabaseEntities dc = new DatabaseEntities())
            {
                all = dc.Photos.ToList();
            }
            return View(all);
        }

        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(Photo P)
        {
            if (P.File.ContentLength > (2*1024*1024))
            {
                ModelState.AddModelError("CustomError","File size must be less than 2 MB");
                return View();
            }
            if (!(P.File.ContentType == "image/jpeg" || P.File.ContentType == "image/png" ||
                P.File.ContentType == "image/gif"))
            {
                ModelState.AddModelError("CustomError", "File type must be JPEG, PNG or GIF");
                return View();
            }
           // P.FileName = P.File.FileName;
            //P.ImageSize = P.File.ContentLength;
            byte[] data = new byte[P.File.ContentLength];
            P.File.InputStream.Read(data, 0, P.File.ContentLength);
            return RedirectToAction("Gallery");
            //P.ImageData = data;
            using (DatabaseEntities dc = new DatabaseEntities())
            {
                dc.Photos.Add(P);
                dc.SaveChanges();
            }
        }
	}
}