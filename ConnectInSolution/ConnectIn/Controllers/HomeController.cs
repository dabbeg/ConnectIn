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
using Microsoft.Ajax.Utilities;
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
                        PostId = id,
                        Body = post.Text,
                        DateInserted = post.Date,
                        Comments = new List<CommentViewModel>(),
                        LikesDislikes = new LikeDislikeViewModel()
                        {
                            Likes = postService.GetPostsLikes(id),
                            Dislikes = postService.GetPostsDislikes(id)
                        },
                        User = new UserViewModel()
                        {
                            UserId = User.Identity.GetUserId(),
                            UserName = User.Identity.Name,
                            ProfilePicture = "~/Content/Images/profilepic.png"
                        }
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

        public ActionResult Profile(string id)
        {
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var postService = new PostService(context);

            var user = userService.GetUserById(id);
            var posts = userService.GetAllPostsFromUser(id);

            var postsViewModels = new List<PostsViewModel>();
            foreach (var postId in posts)
            {
                var post = postService.GetPostById(postId);
                postsViewModels.Add(
                 new PostsViewModel()
                 {
                     PostId = postId,
                     Body = post.Text,
                     DateInserted = post.Date,
                     Comments = new List<CommentViewModel>(),
                     LikesDislikes = new LikeDislikeViewModel()
                     {
                         Likes = postService.GetPostsLikes(postId),
                         Dislikes = postService.GetPostsDislikes(postId)
                     },
                     User = new UserViewModel()
                     {
                         UserId = User.Identity.GetUserId(),
                         UserName = User.Identity.Name,
                         ProfilePicture = "~/Content/Images/profilepic.png"
                     }
                 });
            }

            var model = new ProfileViewModel()
            {
                
                Posts = postsViewModels,
                User = new UserViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                    Gender = user.gender,
                    Birthday = user.birthday,
                    Work = user.work,
                    School = user.school,
                    Address = user.address
                }
            };

            return View(model);
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

            var birthdays = new List<BirthdayViewModel>();
            
            foreach (var id in birthdayList)
            {
                var user = userService.GetUserById(id);
                birthdays.Add(
                    new BirthdayViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            Name = user.Name,
                            UserName = user.UserName,
                            Birthday = user.birthday,
                            ProfilePicture = "~/Content/Images/profilepic.png"
                        },
                    });
            }

            return View(birthdays);
        }
        public ActionResult GroupsList()
        {
            return View();
        }
    }
}