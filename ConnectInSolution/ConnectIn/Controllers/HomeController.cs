using ConnectIn.DAL;
using ConnectIn.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.Models.Entity;
using ConnectIn.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // if i send null, the other one will be used (database?)
            // var service = new UserService(null);

            // var friendlist = service.GetFriendsFromUser(this.User.Identity.Name);

            // return View(friendlist);
            return View();
        }

        public ActionResult NewsFeed()
        {
            var userId = User.Identity.GetUserId();
         
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var postService = new PostService(context);

            var postIdList = userService.GetEveryNewsFeedPostsForUser(userId);
            var newsFeed = new List<PostsViewModel>();

            foreach (var id in postIdList)
            {
                var post = postService.GetPostById(id);
                newsFeed.Add(
                    new PostsViewModel()
                    {
                        Body = post.Text,
                        DateInserted = post.Date,
                        Comments = new List<CommentViewModel>()
                    });
            }

            return View(newsFeed);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Profile()
        {
            List<Post> Profile = new List<Post>();
            Post post1 = new Post();
            post1.Text = "Er jarðskjálft­inn varð í Nepal fyr­ir viku var níu ára göm­ul stúlka, sem dýrkuð er sem gyðja, að und­ir­búa sig fyr­ir að taka á móti til­biðjend­um á heim­ili sínu sem stend­ur við Dur­bar-torgið í Kat­mandú.";
            post1.PostId = 1;
            post1.UserId = "1";
            Profile.Add(post1);

            Post post2 = new Post();
            post2.Text = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet.";
            post2.PostId = 2;
            post2.UserId = "2";
            Profile.Add(post2);

            return View(Profile);
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
            var userId = User.Identity.GetUserId();

            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var birthdayList = userService.GetAllFriendsBirthdays(userId);

            var context = new ApplicationDbContext();

            var postService = new PostService(context);

            var postIdList = userService.GetEveryNewsFeedPostsForUser(userId);
            var newsFeed = new List<PostsViewModel>();

            foreach (var id in postIdList)
            {
                var post = postService.GetPostById(id);
                newsFeed.Add(
                    new PostsViewModel()
                    {
                        Body = post.Text,
                        DateInserted = post.Date,
                        Comments = new List<CommentViewModel>()
                    });
            }

            return View(newsFeed);
            // return View();
        }
        public ActionResult GroupsList()
        {
            return View();
        }
    }
}