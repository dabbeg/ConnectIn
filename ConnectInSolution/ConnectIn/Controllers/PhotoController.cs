using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Models.ViewModels;
using ConnectIn.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System.Net;
using ConnectIn.Utilities;
using System.Configuration;
using System.IO;

namespace ConnectIn.Controllers
{
    public class PhotoController : Controller
    {
        public ActionResult Images(string userId)
        {
            if (userId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var photos = userService.GetAllPhotosFromUser(userId);
            var model = new PhotoAlbumViewModel();
            model.Images = new List<ImageViewModel>();
            model.UserId = userId;

            foreach (var item in photos)
            {
                model.Images.Add(new ImageViewModel()
                {
                    ImagePath = item.PhotoPath,
                    PhotoId = item.PhotoId
                });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(FormCollection collection)
        {
            HttpPostedFileBase file = Request.Files["Image"];
            Int32 length = file.ContentLength;

            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);

            var photo = new Photo()
            {
                ContentType = file.ContentType,
                PhotoBytes = tempImage,
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                IsProfilePicture = false
            };

            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            photo.PhotoPath = "/Photo/ShowPhoto/" + photo.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }

        public ActionResult ShowPhoto(int? id)
        {
            if (!id.HasValue)
            {
                return View("Error");
            }
            int photoId = id.Value;

            var context = new ApplicationDbContext();
            var photoService = new PhotoService(context);

            Photo image = photoService.GetPhotoById(photoId);
            ImageResult result = new ImageResult(image.PhotoBytes, image.ContentType);

            return result;
        }

        public ActionResult PickProfilePicture(FormCollection collection)
        {
            string id = collection["photoId"];

            if (id == "PUT PHOTOID")
            {
                return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
            }

            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            int photoId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var oldProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId());
            if (oldProfilePicture != null)
            {
                oldProfilePicture.IsProfilePicture = false;
            }
            var photo = userService.GetPhotoById(photoId);
            photo.IsProfilePicture = true;
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }
        public ActionResult PickCoverPhoto(FormCollection collection)
        {
            string id = collection["photoId2"];

            if (id == "PUT PHOTO")
            {
                return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
            }

            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            int Coverid = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var oldCoverPhoto = userService.GetCoverPhoto(User.Identity.GetUserId());
            if (oldCoverPhoto != null)
            {
                oldCoverPhoto.IsCoverPhoto = false;
            }
            var photo = userService.GetPhotoById(Coverid);
            photo.IsCoverPhoto = true;
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }

        [HttpGet]
        public ActionResult test()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult CropImage(string imagePath, int? cropPointX, int? cropPointY, int? imageCropWidth, int? imageCropHeight)
        {
            if (string.IsNullOrEmpty(imagePath) || !cropPointX.HasValue || !cropPointY.HasValue || !imageCropWidth.HasValue || !imageCropHeight.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imagePath));
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX.Value, cropPointY.Value, imageCropWidth.Value, imageCropHeight.Value);

            string tempFolderName = Server.MapPath("~/" + ConfigurationManager.AppSettings["Image.TempFolderName"]);
            string fileName = Path.GetFileName(imagePath);

            try
            {
                FileHelper.SaveFile(croppedImage, Path.Combine(tempFolderName, fileName));
            }
            catch (Exception ex)
            {
                //Log an error     
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            string photoPath = string.Concat("/", ConfigurationManager.AppSettings["Image.TempFolderName"], "/", fileName);
            return Json(new { photoPath = photoPath }, JsonRequestBehavior.AllowGet);
        }
    }
}