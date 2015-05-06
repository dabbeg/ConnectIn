using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Services;
using Microsoft.Ajax.Utilities;

namespace ConnectIn.Controllers
{
    public class FriendController : Controller
    {
        public ActionResult Add(FormCollection collection)
        {
            string userId = collection["userId"];
            string friendId = collection["friendId"];

            if (userId.IsNullOrWhiteSpace() || friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var notification = new Notification()
            {
                UserId = userId,
                FriendUserId = friendId,
                Date = DateTime.Now,
                IsPending = true,
                IsFriendRequest = true
            };

            var context = new ApplicationDbContext();
            context.Notifications.Add(notification);
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = friendId });
        }

        public ActionResult Remove(FormCollection collection)
        {
            string userId = collection["userId"];
            string friendId = collection["friendId"];

            if (userId.IsNullOrWhiteSpace() || friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = new Notification()
            {
                UserId = userId,
                FriendUserId = friendId,
                Date = DateTime.Now,
                IsPending = false,
                IsFriendRequest = false
            };
            context.Notifications.Add(notification);

            var friendShip = userService.GetFriendShip(userId, friendId);
            context.Friends.Remove(friendShip);
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = friendId });
        }

        public ActionResult AcceptFriendRequest(int? id)
        {
            if (!id.HasValue)
            {
                return View("Error");
            }
            
            int notificationId = id.Value;

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = userService.GetNotificationById(notificationId);
            notification.IsPending = false;

            var friends = new Friend
            {
                UserId = notification.UserId,
                FriendUserId = notification.FriendUserId
            };
            
            context.Friends.Add(friends);
            context.SaveChanges();

            return RedirectToAction("Notifications", "Home");
        }

        public ActionResult DeclineFriendRequest(int? id)
        {
            if (!id.HasValue)
            {
                return View("Error");
            }

            int notificationId = id.Value;

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = userService.GetNotificationById(notificationId);
            notification.IsPending = false;
            context.SaveChanges();

            return RedirectToAction("Notifications", "Home");
        }

        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult FriendsList()
        {
            return View();
        }

        public ActionResult Birthdays()
        {
            return View();
        }
    }
}