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
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();

        [HttpGet, Authorize]
        public ActionResult Images(string userId)
        {
            if (userId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);
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

        [HttpGet, Authorize]
        public ActionResult UploadProfilePhoto()
        {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult UploadProfilePhoto(FormCollection collection)
        {            
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
            DbContext.Photos.Add(photo);
            DbContext.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            photo.PhotoPath = "/Photo/ShowPhoto?photoId=" + photo.PhotoId;
            DbContext.SaveChanges();

            return RedirectToAction("CropProfilePhoto", new { photoId = photo.PhotoId });
        }

        [HttpGet, Authorize]
        public ActionResult UploadCoverPhoto()
        {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult UploadCoverPhoto(FormCollection collection)
        {
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
            DbContext.Photos.Add(photo);
            DbContext.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            photo.PhotoPath = "/Photo/ShowPhoto?photoId=" + photo.PhotoId;
            DbContext.SaveChanges();

            return RedirectToAction("CropCoverPhoto", new { photoId = photo.PhotoId });
        }

        [HttpPost, Authorize]
        public ActionResult DeletePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            // Removing the photo that has photoId
            var photoService = new PhotoService(DbContext);
            DbContext.Photos.Remove(photoService.GetPhotoById(photoId.Value));
            DbContext.SaveChanges();

            return new EmptyResult();
        }

        [HttpGet, Authorize]
        public ActionResult ShowPhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            // Converting the byte array to an instance of ImageResult that can be represented as an image
            var photoService = new PhotoService(DbContext);
            Photo image = photoService.GetPhotoById(photoId.Value);
            ImageResult result = new ImageResult(image.PhotoBytes, image.ContentType);

            return result;
        }

        [HttpPost, Authorize]
        public ActionResult PickProfilePicture(FormCollection collection)
        {            
            string id = collection["photoId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int photoId = Int32.Parse(id);
            
            // Switching profile pictures
            var userService = new UserService(DbContext);
            var oldPhoto = userService.GetProfilePicture(User.Identity.GetUserId());
            if(oldPhoto != null)
            {
                oldPhoto.IsCurrentProfilePhoto = false;
            }

            var photoService = new PhotoService(DbContext);
            photoService.GetPhotoById(photoId).IsCurrentProfilePhoto = true;
            DbContext.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }

        [HttpPost, Authorize]
        public ActionResult PickCoverPhoto(FormCollection collection)
        {            
            string id = collection["photoId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int photoId = Int32.Parse(id);

            // Switching cover photos
            var userService = new UserService(DbContext);
            var photoService = new PhotoService(DbContext);

            var oldPhotoId = userService.GetCoverPhoto(User.Identity.GetUserId());
            var oldPhoto = photoService.GetPhotoById(oldPhotoId);
            if (oldPhoto != null)
            {
                oldPhoto.IsCurrentCoverPhoto = false;
            }
            photoService.GetPhotoById(photoId).IsCurrentCoverPhoto = true;
            DbContext.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }

        [HttpGet, Authorize]
        public ActionResult CropProfilePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            // Get the photo and fill in the view model
            var photoService = new PhotoService(DbContext);
            var photo = photoService.GetPhotoById(photoId.Value);
            var model = new PhotoViewModel()
            {
                PhotoId = photo.PhotoId,
                PhotoPath = photo.PhotoPath
            };

            return View(model);
        }

        [HttpGet, Authorize]
        public ActionResult CropCoverPhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            // Get the photo and fill in the view model
            var photoService = new PhotoService(DbContext);
            var photo = photoService.GetPhotoById(photoId.Value);
            var model = new PhotoViewModel()
            {
                PhotoId = photo.PhotoId,
                PhotoPath = photo.PhotoPath
            };

            return View(model);
        }

        [HttpPost, Authorize]
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
                return View("Error");
            }

            // Converting crop parameters to integers(sometimes they are double so we need to convert to double first)
            int cropPointX = Convert.ToInt32(double.Parse(scropPointX, CultureInfo.InvariantCulture));
            int cropPointY = Convert.ToInt32(double.Parse(scropPointY, CultureInfo.InvariantCulture));
            int imageCropWidth = Convert.ToInt32(double.Parse(simageCropWidth, CultureInfo.InvariantCulture));
            int imageCropHeight = Convert.ToInt32(double.Parse(simageCropHeight, CultureInfo.InvariantCulture));

            // Crop photo according to the parameters
            var photoService = new PhotoService(DbContext);
            var photo = photoService.GetPhotoById(Int32.Parse(sphotoId));
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
            DbContext.Photos.Remove(photo);
            DbContext.Photos.Add(croppedPhoto);
            DbContext.SaveChanges();

            // Getting the new photoId that the database assigns to the photo for the photo path
            croppedPhoto.PhotoPath = "/Photo/ShowPhoto?photoId=" + croppedPhoto.PhotoId;
            DbContext.SaveChanges();

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }

        [HttpGet, Authorize]
        public ActionResult IsProfilePhoto(int? photoId)
        {
            if (!photoId.HasValue)
            {
                return View("Error");
            }

            var photoService = new PhotoService(DbContext);
            return Json(photoService.GetPhotoById(photoId.Value).IsProfilePhoto, JsonRequestBehavior.AllowGet);
        }
    }
}