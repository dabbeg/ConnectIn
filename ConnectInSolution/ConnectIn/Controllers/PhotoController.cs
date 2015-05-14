using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Models.ViewModels;
using ConnectIn.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using ConnectIn.Utilities;
using System.Globalization;

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

            var profilePhotos = userService.GetAllProfilePhotosFromUser(userId);
            var coverPhotos = userService.GetAllCoverPhotosFromUser(userId);

            var model = new PhotoAlbumViewModel();
            model.ProfilePhotos = new List<PhotoViewModel>();
            model.CoverPhotos = new List<PhotoViewModel>();
            model.UserId = userId;

            foreach (var item in profilePhotos)
            {
                model.ProfilePhotos.Add(new PhotoViewModel()
                {
                    PhotoPath = item.PhotoPath,
                    PhotoId = item.PhotoId
                });
            }

            foreach (var item in coverPhotos)
            {
                model.CoverPhotos.Add(new PhotoViewModel()
                {
                    PhotoPath = item.PhotoPath,
                    PhotoId = item.PhotoId
                });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult UploadProfilePhoto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadProfilePhoto(FormCollection collection)
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
                IsProfilePhoto = false,
                IsCoverPhoto = false,
                IsCurrentCoverPhoto = false,
                IsCurrentProfilePhoto = false
            };

            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            photo.PhotoPath = "/Photo/ShowPhoto/" + photo.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("CropProfilePhoto", new { photoId = photo.PhotoId });
        }

        [HttpGet]
        public ActionResult UploadCoverPhoto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadCoverPhoto(FormCollection collection)
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
                IsProfilePhoto = false,
                IsCoverPhoto = false,
                IsCurrentCoverPhoto = false,
                IsCurrentProfilePhoto = false
            };

            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            photo.PhotoPath = "/Photo/ShowPhoto/" + photo.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("CropCoverPhoto", new { photoId = photo.PhotoId });
        }

        [HttpPost]
        public ActionResult DeletePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            context.Photos.Remove(userService.GetPhotoById(photoId.Value));
            context.SaveChanges();

            return new EmptyResult();
        }

        [HttpGet]
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

        [HttpPost]
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
            var oldPhoto = userService.GetProfilePicture(User.Identity.GetUserId());
            if(oldPhoto != null)
            {
                oldPhoto.IsCurrentProfilePhoto = false;
            }
            userService.GetPhotoById(photoId).IsCurrentProfilePhoto = true;
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }

        [HttpPost]
        public ActionResult PickCoverPhoto(FormCollection collection)
        {
            string id = collection["photoId"];

            if (id == "PUT PHOTO")
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
            var oldPhoto = userService.GetCoverPhoto(User.Identity.GetUserId());
            if (oldPhoto != null)
            {
                oldPhoto.IsCurrentCoverPhoto = false;
            }
            userService.GetPhotoById(photoId).IsCurrentCoverPhoto = true;
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }

        [HttpGet]
        public ActionResult CropProfilePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return View(userService.GetPhotoById(photoId.Value));
        }

        [HttpGet]
        public ActionResult CropCoverPhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return View(userService.GetPhotoById(photoId.Value));
        }

        [HttpPost]
        public virtual ActionResult CropImage(FormCollection collection)
        {
            string location = collection["location"];
            string sphotoId = collection["photoId"];
            string scropPointX = collection["cropPointX"];
            string scropPointY = collection["cropPointY"];
            string simageCropWidth = collection["imageCropWidth"];
            string simageCropHeight = collection["imageCropHeight"];
            if (location.IsNullOrWhiteSpace() || sphotoId.IsNullOrWhiteSpace() || scropPointX.IsNullOrWhiteSpace()
                || scropPointY.IsNullOrWhiteSpace() || simageCropWidth.IsNullOrWhiteSpace() || simageCropHeight.IsNullOrWhiteSpace())
            {
                if (location == "profile")
                {
                    return RedirectToAction("PickProfilePicture", new {photoId = Int32.Parse(sphotoId)});
                }
                else if(location == "cover")
                {
                    return RedirectToAction("PickCoverPhoto", new { photoId = Int32.Parse(sphotoId) });
                }
            }

            int cropPointX = Convert.ToInt32(double.Parse(scropPointX, CultureInfo.InvariantCulture));
            int cropPointY = Convert.ToInt32(double.Parse(scropPointY, CultureInfo.InvariantCulture));
            int imageCropWidth = Convert.ToInt32(double.Parse(simageCropWidth, CultureInfo.InvariantCulture));
            int imageCropHeight = Convert.ToInt32(double.Parse(simageCropHeight, CultureInfo.InvariantCulture));

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            // Crop photo according to the parameters
            var photo = userService.GetPhotoById(Int32.Parse(sphotoId));
            byte[] imageBytes = photo.PhotoBytes;
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX, cropPointY, imageCropWidth, imageCropHeight);

            // Create a photo instance for the new cropped photo
            var croppedPhoto = new Photo()
            {
                UserId = photo.UserId,
                Date = DateTime.Now,
                PhotoBytes = croppedImage,
                ContentType = photo.ContentType,
                IsProfilePhoto = false,
                IsCurrentProfilePhoto = false,
                IsCoverPhoto = false,
                IsCurrentCoverPhoto = false
            };


            if (location == "profile")
            {
                croppedPhoto.IsProfilePhoto = true;
            }
            else if (location == "cover")
            {
                croppedPhoto.IsCoverPhoto = true;
            }

            context.Photos.Remove(photo);
            context.Photos.Add(croppedPhoto);
            context.SaveChanges();

            croppedPhoto.PhotoPath = "/Photo/ShowPhoto/" + croppedPhoto.PhotoId;
            context.SaveChanges();

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }

        [HttpPost]
        public ActionResult IsProfilePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }
            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return Json(userService.GetPhotoById(photoId.Value).IsProfilePhoto, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult IsCoverPhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }
            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return Json(userService.GetPhotoById(photoId.Value).IsCoverPhoto, JsonRequestBehavior.AllowGet);
        }
    }
}