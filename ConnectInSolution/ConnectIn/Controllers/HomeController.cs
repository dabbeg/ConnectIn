using System;
using ConnectIn.DAL;
using ConnectIn.Services;
using System.Collections.Generic;
using System.IO;
using System.Web.Helpers;
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
            if (User.Identity.IsAuthenticated) return RedirectToAction("NewsFeed", "Home");
            else return View();
        }

        public ActionResult NewsFeed()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            var userId = User.Identity.GetUserId();
            
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var postService = new PostService(context);

            var postIdList = userService.GetEveryNewsFeedPostsForUser(userId);
            var newsFeed = new NewsFeedViewModel();
            newsFeed.Id = "-1";
            newsFeed.Posts = new List<PostsViewModel>();

            foreach (var id in postIdList)
            {
                var post = postService.GetPostById(id);
                newsFeed.Posts.Add(
                    new PostsViewModel()
                    {
                        PostId = id,
                        Body = post.Text,
                        DateInserted = post.Date,
                        Comments = new List<CommentViewModel>(),
                        LikeDislikeComment = new LikeDislikeCommentViewModel()
                        {
                            Likes = postService.GetPostsLikes(id),
                            Dislikes = postService.GetPostsDislikes(id),
                            Comments = postService.GetPostsCommentsCount(id)
                        },
                        User = new UserViewModel()
                        {
                            UserId = post.UserId,
                            Name = userService.GetUserById(post.UserId).Name,
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
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
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
                     LikeDislikeComment = new LikeDislikeCommentViewModel()
                     {
                         Likes = postService.GetPostsLikes(postId),
                         Dislikes = postService.GetPostsDislikes(postId),
                         Comments = postService.GetPostsCommentsCount(postId)
                     },
                     User = new UserViewModel()
                     {
                         UserId = user.Id,
                         UserName = user.UserName,
                         Name = user.Name,
                         ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                         Gender = user.Gender,
                         Birthday = user.Birthday,
                         Work = user.Work,
                         School = user.School,
                         Address = user.Address
                     }
                 });
            }

            var model = new ProfileViewModel()
            {
                
                NewsFeed = new NewsFeedViewModel()
                {
                    Posts = postsViewModels,
                    Id = id
                },
                User = new UserViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    Work = user.Work,
                    School = user.School,
                    Address = user.Address
                }
            };

            return View(model);
        }

        public ActionResult Notifications()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var notifications = userService.GetAllNotificationsForUser(User.Identity.GetUserId());

            var model = new List<NotificationViewModel>();

            foreach (var item in notifications)
            {
                var user = userService.GetUserById(item.FriendUserId);
                var friend = userService.GetUserById(item.UserId);
                model.Add(
                    new NotificationViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            Name = user.Name,
                            Work = user.Work,
                            School = user.School
                        },
                        Friend = new UserViewModel()
                        {
                            UserId = friend.Id,
                            Name = friend.Name,
                            Work = friend.Work,
                            School = friend.School,
                            ProfilePicture = "http://i.imgur.com/3h6Ha2F.jpg"
                        },
                        NotificationId = item.NotificationId,
                        Date = item.Date
                    }
                );

            }

            return View(model);
        }

        public ActionResult Search(FormCollection collection)
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            var searchWord = collection["status"];

            var userId = User.Identity.GetUserId();

            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var searchList = userService.GetPossibleUsersByName(searchWord);

            var searchResult = new List<SearchViewModel>();

            foreach (var id in searchList)
            {
                var user = userService.GetUserById(id);
                searchResult.Add(
                    new SearchViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Work = user.Work,
                            School = user.School,
                            Address = user.Address
                        }
                    });
            }
            return View(searchResult);
        }
       
        public ActionResult FriendsList()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            var userId = User.Identity.GetUserId();

            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var friendList = userService.GetFriendsFromUser(userId);
            var friends = new List<FriendViewModel>();

            foreach (var id in friendList)
            {
                var user = userService.GetUserById(id);
                friends.Add(
                    new FriendViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Work = user.Work,
                            School = user.School,
                            Address = user.Address
                        }
                    });  
            }
            return View(friends);
        }
        
        public ActionResult Birthdays()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
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
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Work = user.Work,
                            School = user.School,
                            Address = user.Address
                        }
                    });
            }

            return View(birthdays);
        }

        
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
                    ImagePath = item.PhotoPath
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
            var photo = WebImage.GetImageFromRequest("Image");
            if (photo != null)
            {
                var newFileName = Guid.NewGuid().ToString() + "_" +
                                  Path.GetFileName(photo.FileName);
                var imagePath = @"Content\UserImages\" + newFileName;
                photo.Save(@"~\" + imagePath);
                
                var context = new ApplicationDbContext();
                var img = new Photo()
                {
                    PhotoPath = "/Content/UserImages/" + newFileName,
                    UserId = User.Identity.GetUserId(),
                    Date = DateTime.Now,
                    IsProfilePicture = false
                };
                context.Photos.Add(img);
                context.SaveChanges();
            }

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }
    }
}