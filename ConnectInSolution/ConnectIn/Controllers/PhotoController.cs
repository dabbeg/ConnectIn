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
        [HttpGet]
        public ActionResult Images(string userId)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (userId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var profilePhotos = userService.GetAllProfilePhotosFromUser(userId);
            var coverPhotos = userService.GetAllCoverPhotosFromUser(userId);

            // Filling the view model
            var model = new PhotoAlbumViewModel
            {
                ProfilePhotos = new List<PhotoViewModel>(),
                CoverPhotos = new List<PhotoViewModel>(),
                UserId = userId
            };

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
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult UploadProfilePhoto(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            
            // Getting file from input and converting it into byte array
            HttpPostedFileBase file = Request.Files["Image"];
            Int32 length = file.ContentLength;
            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);

            // Creating the model for the new photo
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

            // Saving it into the database
            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            photo.PhotoPath = "/Photo/ShowPhoto?photoId=" + photo.PhotoId;
            context.SaveChanges();

            return RedirectToAction("CropProfilePhoto", new { photoId = photo.PhotoId });
        }

        [HttpGet]
        public ActionResult UploadCoverPhoto()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult UploadCoverPhoto(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");

            // Getting file from input and converting it into byte array
            HttpPostedFileBase file = Request.Files["Image"];
            Int32 length = file.ContentLength;
            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);

            // Creating the model for the new photo
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

            // Saving it into the database
            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            photo.PhotoPath = "/Photo/ShowPhoto?photoId=" + photo.PhotoId;
            context.SaveChanges();

            return RedirectToAction("CropCoverPhoto", new { photoId = photo.PhotoId });
        }

        [HttpPost]
        public ActionResult DeletePhoto(int? photoId)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            // Removing the photo that has photoId
            context.Photos.Remove(userService.GetPhotoById(photoId.Value));
            context.SaveChanges();

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult ShowPhoto(int? photoId)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var photoService = new PhotoService(context);

            // Converting the byte array to an instance of ImageResult that can be represented as an image
            Photo image = photoService.GetPhotoById(photoId.Value);
            ImageResult result = new ImageResult(image.PhotoBytes, image.ContentType);

            return result;
        }

        [HttpPost]
        public ActionResult PickProfilePicture(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            
            string id = collection["photoId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int photoId = Int32.Parse(id);
            
            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            // Switching profile pictures
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
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            
            string id = collection["photoId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int photoId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            // Switching cover photos
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
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var photo = userService.GetPhotoById(photoId.Value);
            var model = new PhotoViewModel()
            {
                PhotoId = photo.PhotoId,
                PhotoPath = photo.PhotoPath
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult CropCoverPhoto(int? photoId)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var photo = userService.GetPhotoById(photoId.Value);
            var model = new PhotoViewModel()
            {
                PhotoId = photo.PhotoId,
                PhotoPath = photo.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult CropImage(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            string location = collection["location"];
            string sphotoId = collection["photoId"];
            string scropPointX = collection["cropPointX"];
            string scropPointY = collection["cropPointY"];
            string simageCropWidth = collection["imageCropWidth"];
            string simageCropHeight = collection["imageCropHeight"];
            if (location.IsNullOrWhiteSpace() || sphotoId.IsNullOrWhiteSpace() || scropPointX.IsNullOrWhiteSpace()
                || scropPointY.IsNullOrWhiteSpace() || simageCropWidth.IsNullOrWhiteSpace() || simageCropHeight.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            // Converting crop parameters to integers(sometimes they are double so we need to convert to double first)
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

            // Remove uncropped photo and add cropped photo
            context.Photos.Remove(photo);
            context.Photos.Add(croppedPhoto);
            context.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            croppedPhoto.PhotoPath = "/Photo/ShowPhoto?photoId=" + croppedPhoto.PhotoId;
            context.SaveChanges();

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }

        [HttpGet]
        public ActionResult IsProfilePhoto(int? photoId)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return Json(userService.GetPhotoById(photoId.Value).IsProfilePhoto, JsonRequestBehavior.AllowGet);
        }
    }
}