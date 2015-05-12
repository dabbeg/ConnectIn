﻿using System;
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

            var oldProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId());
            if (oldProfilePicture.PhotoBytes != null)
            {
                context.Photos.Remove(oldProfilePicture);
            }
            context.SaveChanges();

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

        [HttpPost]
        public virtual ActionResult CropImage(FormCollection collection)
        {
            string sphotoId = collection["photoId"];
            string scropPointX = collection["cropPointX"];
            string scropPointY = collection["cropPointY"];
            string simageCropWidth = collection["imageCropWidth"];
            string simageCropHeight = collection["imageCropHeight"];
            if (sphotoId.IsNullOrWhiteSpace() || scropPointX.IsNullOrWhiteSpace() || scropPointY.IsNullOrWhiteSpace() || simageCropWidth.IsNullOrWhiteSpace() || simageCropHeight.IsNullOrWhiteSpace())
            {
                return RedirectToAction("test", new { photoId = Int32.Parse(sphotoId) });
            }

            int cropPointX = Int32.Parse(scropPointX);
            int cropPointY = Int32.Parse(scropPointY);
            int imageCropWidth = Convert.ToInt32(double.Parse(simageCropWidth, CultureInfo.InvariantCulture));
            int imageCropHeight = Convert.ToInt32(double.Parse(simageCropHeight, CultureInfo.InvariantCulture));

            var context = new ApplicationDbContext();
            var photoService = new PhotoService(context);

            var photo = photoService.GetPhotoById(Int32.Parse(sphotoId));
            byte[] imageBytes = photo.PhotoBytes;
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX, cropPointY, imageCropWidth, imageCropHeight);

            var croppedPhoto = new Photo()
            {
                UserId = photo.UserId,
                Date = DateTime.Now,
                PhotoBytes = croppedImage,
                ContentType = photo.ContentType,
                IsProfilePicture = true,
                IsCoverPhoto = false
            };
            context.Photos.Add(croppedPhoto);
            context.SaveChanges();

            croppedPhoto.PhotoPath = "/Photo/ShowPhoto/" + croppedPhoto.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = User.Identity.GetUserId() });
        }
    }
}