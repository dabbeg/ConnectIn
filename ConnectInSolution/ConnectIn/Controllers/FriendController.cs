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

            var friends = new Friend()
            {
                UserId = userId,
                FriendUserId = friendId
            };

            var context = new ApplicationDbContext();
            context.Friends.Add(friends);
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

            var friendShip = userService.GetFriendShip(userId, friendId);
            context.Friends.Remove(friendShip);
            context.SaveChanges();

            return RedirectToAction("Profile", "Home", new { id = friendId });
        }

        public ActionResult FriendsList()
        {
            return View();
        }
        public ActionResult Notifications()
        {
            return View();
        }
        public ActionResult Birthdays()
        {
            return View();
        }
    }
}