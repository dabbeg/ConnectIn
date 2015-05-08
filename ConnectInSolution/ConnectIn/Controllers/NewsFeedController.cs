using System.Web.Mvc;
using System.Web.UI;
using ConnectIn.DAL;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class NewsFeedController : Controller
    {
        public ActionResult Everyone()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BestFriends()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts1 = userService.GetBestFriendsPostsForUser(User.Identity.GetUserId());
            var posts2 = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());

            var pair = new Pair(posts1, posts2);
            
            return Json(pair, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Family()
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var posts = userService.GetFamilyPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }
    }
}