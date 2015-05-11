using System;
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
            
            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            //?
            var friendship = userService.GetFriendShip(userId, friendId);
            if (friendship == null)
            {

                var notification = new Notification()
                {
                    UserId = userId,
                    FriendUserId = friendId,
                    GroupId = "-1",
                    Date = DateTime.Now
                };


                context.Notifications.Add(notification);
                context.SaveChanges();
            }

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

            var friendShip = userService.GetFriendShip(userId, friendId);
            context.Friends.Remove(friendShip);
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = friendId });
        }

        public ActionResult AcceptFriendRequest(FormCollection collection)
        {
            string id = collection["notificationId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            
            int notificationId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = userService.GetNotificationById(notificationId);

            var friends = new Friend
            {
                UserId = notification.UserId,
                FriendUserId = notification.FriendUserId
            };

            context.Notifications.Remove(notification);
            context.Friends.Add(friends);
            context.SaveChanges();

            return RedirectToAction("Notifications", "Home");
        }

        public ActionResult DeclineFriendRequest(FormCollection collection)
        {
            string id = collection["notificationId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            int notificationId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = userService.GetNotificationById(notificationId);
            context.Notifications.Remove(notification);
            context.SaveChanges();

            return RedirectToAction("Notifications", "Home");
        }

        public ActionResult HideGroupNotification(FormCollection collection)
        {
            string id = collection["groupNotificationId"];

            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            int notificationId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var notification = userService.GetNotificationById(notificationId);
            context.Notifications.Remove(notification);
            context.SaveChanges();

            return RedirectToAction("Notifications", "Home");
        }
    }
}