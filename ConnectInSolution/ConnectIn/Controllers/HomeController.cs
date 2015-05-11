using System;
using ConnectIn.DAL;
using ConnectIn.Services;
using System.Collections.Generic;
using System.IO;
using System.Web;
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
            return View();
        }

        public ActionResult NewsFeed()
        {
            if (User.Identity.IsAuthenticated == false) return RedirectToAction("Login", "Account");
            var userId = User.Identity.GetUserId();
            
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var postService = new PostService(context);
            var likedislikeService = new LikeDislikeService(context);

            var profilePicture = userService.GetProfilePicture(User.Identity.GetUserId());
            string profilePicturePath;

            if (profilePicture == null)
            {
                profilePicturePath = "~/Content/images/largeProfilePic.jpg";
            }
            else
            {
                profilePicturePath = profilePicture.PhotoPath;
            }

            var newsFeed = new NewsFeedViewModel
            {
                Id = "-1",
                Posts = new List<PostsViewModel>(),
                User = new UserViewModel()
                {
                    ProfilePicture = profilePicturePath
                }
            };

            var postList = userService.GetEveryNewsFeedPostsForUser(userId);

            foreach (var id in postList)
            {

                var post = postService.GetPostById(id);
                profilePicture = userService.GetProfilePicture(post.UserId);

                if (profilePicture == null)
                {
                    profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                }
                else
                {
                    profilePicturePath = profilePicture.PhotoPath;
                }
                // checka ef það er til færsla... fyrir unlike og undislike
                string lPic, dPic;
                if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), id) == null)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), id).Like)
                {
                    lPic = "~/Content/images/smileyGREEN.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), id).Dislike)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceRED.png";
                }
                else
                {
                    return View("Error");
                }
                newsFeed.Posts.Add(
                    new PostsViewModel()
                    {
                        PostId = id,
                        Body = post.Text,
                        DateInserted = post.Date,
                        LikeDislikeComment = new LikeDislikeCommentViewModel()
                        {
                            Likes = postService.GetPostsLikes(post.PostId),
                            Dislikes = postService.GetPostsDislikes(post.PostId),
                            Comments = postService.GetPostsCommentsCount(post.PostId)
                        },
                        User = new UserViewModel()
                        {
                            UserId = post.UserId,
                            Name = userService.GetUserById(post.UserId).Name,
                            ProfilePicture = profilePicturePath
                        },
                        LikePic = lPic,
                        DislikePic = dPic
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

        [HttpPost]
        public ActionResult BestFriend(FormCollection collection)
        {
            var count = 0;
            string friendId = collection["friendId"];

            if (friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), friendId);

            if (friendShip.UserId == User.Identity.GetUserId())
            {
                // if considered as best friend, then disbestfriend, else consider as best friend
                friendShip.UserConsidersFriendAsBestFriend = !friendShip.UserConsidersFriendAsBestFriend;
                if (friendShip.UserConsidersFriendAsBestFriend) count = 1;
            }
            else if (friendShip.FriendUserId == User.Identity.GetUserId())
            {
                // if considered as best friend, then disbestfriend, else consider as best friend
                friendShip.FriendConsidersUsersAsBestFriend = !friendShip.FriendConsidersUsersAsBestFriend;
                if (friendShip.FriendConsidersUsersAsBestFriend) count = 1;
            }
            else
            {
                return View("Error");
            }
            db.SaveChanges();

            var json = new
            {
                FullStar = count
            };

            return Json(json, JsonRequestBehavior.AllowGet);

            /*if (location.Equals("Profile")) return RedirectToAction("Profile", "Home", new {id = friendId});
            return RedirectToAction("FriendsList", "Home");*/
        }
        public ActionResult Family(FormCollection collection)
        {
            var count = 0;
            string friendId = collection["friendId"];

            if (friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), friendId);

            if (friendShip.UserId == User.Identity.GetUserId())
            {
                // if considered as best friend, then disbestfriend, else consider as best friend
                friendShip.UserConsidersFriendAsFamily = !friendShip.UserConsidersFriendAsFamily;
                if (friendShip.UserConsidersFriendAsFamily) count = 1;
            }
            else if (friendShip.FriendUserId == User.Identity.GetUserId())
            {
                // if considered as best friend, then disbestfriend, else consider as best friend
                friendShip.FriendConsidersUsersAsFamily = !friendShip.FriendConsidersUsersAsFamily;
                if (friendShip.FriendConsidersUsersAsFamily) count = 1;
            }
            else
            {
                return View("Error");
            }
            db.SaveChanges();

            var json = new
            {
                FullStar = count
            };

            return Json(json, JsonRequestBehavior.AllowGet);
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
            var likedislikeService = new LikeDislikeService(context);

            var user = userService.GetUserById(id);
            var posts = userService.GetAllPostsFromUser(id);

            var profilePicture = userService.GetProfilePicture(id);
            string profilePicturePath, lPic, dPic, 
                bfStarPick = "~/Content/images/emptystar.png", fStarPick = "~/Content/images/emptystar.png";
            profilePicturePath = profilePicture == null 
                ? "~/Content/images/largeProfilePic.jpg" 
                : profilePicture.PhotoPath;

            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), id);

            if (friendShip != null)
            {
                if (friendShip.UserId == User.Identity.GetUserId())
                {
                    // if considered as best friend, then disbestfriend, else consider as best friend
                    bfStarPick = friendShip.UserConsidersFriendAsBestFriend
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                    fStarPick = friendShip.UserConsidersFriendAsFamily
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                }
                else if (friendShip.FriendUserId == User.Identity.GetUserId())
                {
                    // if considered as best friend, then disbestfriend, else consider as best friend
                    bfStarPick = friendShip.FriendConsidersUsersAsBestFriend
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                    fStarPick = friendShip.FriendConsidersUsersAsFamily
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                }
                else
                {
                    return View("Error");
                }
            }

            var postsViewModels = new List<PostsViewModel>();

            foreach (var postId in posts)
            {
                if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId) == null)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId).Like)
                {
                    lPic = "~/Content/images/smileyGREEN.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId).Dislike)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceRED.png";
                }
                else
                {
                    return View("Error");
                }
                var post = postService.GetPostById(postId);
                postsViewModels.Add(
                 new PostsViewModel()
                 {
                     PostId = postId,
                     Body = post.Text,
                     DateInserted = post.Date,
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
                         ProfilePicture = profilePicturePath,
                         Gender = user.Gender,
                         Birthday = user.Birthday,
                         Work = user.Work,
                         School = user.School,
                         Address = user.Address,
                         BfStar = bfStarPick,
                         FStar = fStarPick
                     },
                     LikePic = lPic,
                     DislikePic = dPic
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
                    ProfilePicture = profilePicturePath,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    Work = user.Work,
                    School = user.School,
                    Address = user.Address,
                    BfStar = bfStarPick,
                    FStar = fStarPick
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

                var profilePicture = userService.GetProfilePicture(item.UserId);
                string profilePicturePath;
                if (profilePicture == null)
                {
                    profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                }
                else
                {
                    profilePicturePath = profilePicture.PhotoPath;
                }
                

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
                            ProfilePicture = profilePicturePath
                        },
                        NotificationId = item.NotificationId,
                        Date = item.Date
                    }
                );

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult NotificationCounter()
        {
            var db = new ApplicationDbContext();
            var userService = new UserService(db);

            var notifications = userService.GetAllNotificationsForUser(User.Identity.GetUserId()).Count;

            return Json(notifications, JsonRequestBehavior.AllowGet);
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
                var profilePicture = userService.GetProfilePicture(id);
                string profilePicturePath;
                if (profilePicture == null)
                {
                    profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                }
                else
                {
                    profilePicturePath = profilePicture.PhotoPath;
                }

                var user = userService.GetUserById(id);
                searchResult.Add(
                    new SearchViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = profilePicturePath,
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
                string bfStarPick, fStarPick;
                var profilePicture = userService.GetProfilePicture(id);
                string profilePicturePath = profilePicture == null 
                    ? "~/Content/images/largeProfilePic.jpg" 
                    : profilePicture.PhotoPath;
                var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), id);

                if (friendShip.UserId == User.Identity.GetUserId())
                {
                    // if considered as best friend, then disbestfriend, else consider as best friend
                    bfStarPick = friendShip.UserConsidersFriendAsBestFriend 
                        ? "~/Content/images/fullstar.png" 
                        : "~/Content/images/emptystar.png";
                    fStarPick = friendShip.UserConsidersFriendAsFamily 
                        ? "~/Content/images/fullstar.png" 
                        : "~/Content/images/emptystar.png";
                } else if (friendShip.FriendUserId == User.Identity.GetUserId())
                {
                    // if considered as best friend, then disbestfriend, else consider as best friend
                    bfStarPick = friendShip.FriendConsidersUsersAsBestFriend
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                    fStarPick = friendShip.FriendConsidersUsersAsFamily
                        ? "~/Content/images/fullstar.png"
                        : "~/Content/images/emptystar.png";
                }
                else
                {
                    return View("Error");
                }
                var user = userService.GetUserById(id);
                friends.Add(
                    new FriendViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = profilePicturePath,
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Work = user.Work,
                            School = user.School,
                            Address = user.Address,
                            BfStar = bfStarPick,
                            FStar = fStarPick
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
                var profilePicture = userService.GetProfilePicture(id);
                string profilePicturePath;
                if (profilePicture == null)
                {
                    profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                }
                else
                {
                    profilePicturePath = profilePicture.PhotoPath;
                }

                var user = userService.GetUserById(id);
                birthdays.Add(
                    new BirthdayViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = profilePicturePath,
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

        [HttpGet]
        public ActionResult BirthdayCounter()
        {
            var db = new ApplicationDbContext();
            var userService = new UserService(db);
            var birthdays = userService.GetAllFriendsBirthdays(User.Identity.GetUserId()).Count;
            return Json(birthdays, JsonRequestBehavior.AllowGet);
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
                    ImagePath = item.PhotoPath,
                    PhotoId = item.PhotoId
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
            HttpPostedFileBase file = Request.Files["Image"];
            Int32 length = file.ContentLength;
            
            byte[] tempImage = new byte[length];
            file.InputStream.Read(tempImage, 0, length);

            var photo = new Photo()
            {
                ContentType = file.ContentType,
                PhotoBytes = tempImage,
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                IsProfilePicture = false
            };

            var context = new ApplicationDbContext();
            context.Photos.Add(photo);
            context.SaveChanges();

            photo.PhotoPath = "/Home/ShowPhoto/" + photo.PhotoId.ToString();
            context.SaveChanges();

            return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
        }

        public ActionResult ShowPhoto(int? id)
        {
            if (!id.HasValue)
            {
                return View("Error");
            }
            int photoId = id.Value;

            var context = new ApplicationDbContext();
            var photoService = new PhotoService(context);

            Photo image = photoService.GetPhotoById(photoId);
            ImageResult result = new ImageResult(image.PhotoBytes, image.ContentType);

            return result;
        }

        public ActionResult PickProfilePicture(FormCollection collection)
        {
            string id = collection["photoId"];

            if (id == "PUT PHOTOID")
            {
                return RedirectToAction("Images", new { userId = User.Identity.GetUserId() });
            }

            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            int photoId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var userService = new UserService(context);

            var oldProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId());
            if (oldProfilePicture != null)
            {
                oldProfilePicture.IsProfilePicture = false;
            }
            var photo = userService.GetPhotoById(photoId);
            photo.IsProfilePicture = true;
            context.SaveChanges();

            return RedirectToAction("Profile", new { id = User.Identity.GetUserId() });
        }
    }

}