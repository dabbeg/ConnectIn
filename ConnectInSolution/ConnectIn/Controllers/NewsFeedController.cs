using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class NewsFeedController : Controller
    {
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();

        [Authorize]
        [HttpGet]
        public ActionResult Everyone()
        {
            var userService = new UserService(DbContext);
            var posts = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult BestFriends()
        {
            var userService = new UserService(DbContext);
            var posts = userService.GetBestFriendsPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Family()
        {
            var userService = new UserService(DbContext);
            var posts = userService.GetFamilyPostsForUser(User.Identity.GetUserId());

            return Json(posts, JsonRequestBehavior.AllowGet);
        }
    }
}