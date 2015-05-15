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
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();
        
        [HttpPost, Authorize]
        public ActionResult Add(FormCollection collection)
        {
            string userId = collection["userId"];
            string friendId = collection["friendId"];

            if (userId.IsNullOrWhiteSpace() || friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);

            var friendship = userService.GetFriendShip(userId, friendId);
            // If they are already friends, then it should not be possible to add him again
            if (friendship != null) return View("Error");
            var notification = new Notification()
            {
                UserId = userId,
                FriendUserId = friendId,
                GroupId = -1,
                Date = DateTime.Now
            };

            // Else add the friend
            DbContext.Notifications.Add(notification);
            DbContext.SaveChanges();

            return new EmptyResult();
        }

        [HttpPost, Authorize]
        public ActionResult Remove(FormCollection collection)
        {
            string userId = collection["userId"];
            string friendId = collection["friendId"];

            if (userId.IsNullOrWhiteSpace() || friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);

            var friendShip = userService.GetFriendShip(userId, friendId);
            // If there is not friendship to remove, return a error
            if (friendShip == null) return View("Error");
            DbContext.Friends.Remove(friendShip);
            DbContext.SaveChanges();

            // If friend is removed from the profile, refresh the page
            var url = ControllerContext.HttpContext.Request.UrlReferrer;
            if (url != null && url.AbsolutePath.Contains("Profile"))
            {
                return RedirectToAction("Profile", "Home", new { id = friendId });
            }

            // Else asynchronously remove the friend
            return new EmptyResult();
        }

        [HttpPost, Authorize]
        public ActionResult AcceptFriendRequest(FormCollection collection)
        {
            string id = collection["notificationId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var notificationService = new NotificationService(DbContext);

            int notificationId = Int32.Parse(id);
            var notification = notificationService.GetNotificationById(notificationId);
            // if the notification doesn't exist, return a error
            if (notification == null) return View("Error");

            var friends = new Friend
            {
                UserId = notification.UserId,
                FriendUserId = notification.FriendUserId
            };

            // Accept the friend and remove the notification
            DbContext.Notifications.Remove(notification);
            DbContext.Friends.Add(friends);
            DbContext.SaveChanges();
            
            return new EmptyResult();
        }

        [HttpPost, Authorize]
        public ActionResult DeclineFriendRequest(FormCollection collection)
        {
            string id = collection["notificationId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var notificationService = new NotificationService(DbContext);

            int notificationId = Int32.Parse(id);
            var notification = notificationService.GetNotificationById(notificationId);
            // if the notification doesn't exist, return a error
            if (notification == null) return View("Error");
            
            // Reject the friend and remove the notification
            DbContext.Notifications.Remove(notification);
            DbContext.SaveChanges();

            return new EmptyResult();
        }

        [HttpPost, Authorize]
        public ActionResult HideGroupNotification(FormCollection collection)
        {
            string id = collection["groupNotificationId"];

            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var notificationService = new NotificationService(DbContext);

            int notificationId = Int32.Parse(id);
            var notification = notificationService.GetNotificationById(notificationId);
            // if the notification doesn't exist, return a error
            if (notification == null) return View("Error");
            
            // Remove the group notification
            DbContext.Notifications.Remove(notification);
            DbContext.SaveChanges();

            return new EmptyResult();
        }
    }
}