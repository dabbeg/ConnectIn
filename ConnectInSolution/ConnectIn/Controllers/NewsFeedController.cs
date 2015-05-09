using System.Web.Mvc;
using System.Web.UI;
using ConnectIn.DAL;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace ConnectIn.Controllers
{
    public class NewsFeedController : Controller
    {
        [HttpGet]
        public ActionResult Everyone()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BestFriends()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts = userService.GetBestFriendsPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Family()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts = userService.GetFamilyPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }
    }
}