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

            return View(userService.GetPhotoById(photoId));
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

            int photoId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            return View(userService.GetPhotoById(photoId));
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

            int cropPointX = Int32.Parse(scropPointX);
            int cropPointY = Int32.Parse(scropPointY);
            int imageCropWidth = Convert.ToInt32(double.Parse(simageCropWidth, CultureInfo.InvariantCulture));
            int imageCropHeight = Convert.ToInt32(double.Parse(simageCropHeight, CultureInfo.InvariantCulture));

            var context = new ApplicationDbContext();
            var photoService = new PhotoService(context);
            var userService = new UserService(context);

            // Crop photo according to the parameters
            var photo = photoService.GetPhotoById(Int32.Parse(sphotoId));
            byte[] imageBytes = photo.PhotoBytes;
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX, cropPointY, imageCropWidth, imageCropHeight);

            // Create a photo instance for the new cropped photo
            var croppedPhoto = new Photo()
            {
                UserId = photo.UserId,
                Date = DateTime.Now,
                PhotoBytes = croppedImage,
                ContentType = photo.ContentType
            };

            // Profilepicture or coverPicture
            if (location == "profile")
            {
                var profilePicture = userService.GetProfilePicture(User.Identity.GetUserId());
                if (profilePicture.PhotoBytes != null)
                {
                    context.Photos.Remove(profilePicture);
                }
                croppedPhoto.IsProfilePicture = true;
                croppedPhoto.IsCoverPhoto = false;
            }
            else if (location == "cover")
            {
                var coverPicture = userService.GetCoverPhoto(User.Identity.GetUserId());
                if (coverPicture != null)
                {
                    context.Photos.Remove(coverPicture);
                }
                croppedPhoto.IsProfilePicture = false;
                croppedPhoto.IsCoverPhoto = true;
            }

            context.Photos.Add(croppedPhoto);
            context.SaveChanges();

            croppedPhoto.PhotoPath = "/Photo/ShowPhoto/" + croppedPhoto.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }
    }
}