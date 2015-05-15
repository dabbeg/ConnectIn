using ConnectIn.DAL;
using ConnectIn.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectIn.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("NewsFeed", "Home");
            return View();
        }

        [HttpGet, Authorize]
        public ActionResult NewsFeed()
        {
            // Fill in the newsfeed viewmodel to display on the newsfeed
            var userService = new UserService(DbContext);
            var newsFeed = new NewsFeedViewModel
            {
                Id = "-1",
                Posts = new List<PostsViewModel>(),
                User = new UserViewModel()
                {
                    ProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId()).PhotoPath
                }
            };

            var postService = new PostService(DbContext);
            var likedislikeService = new LikeDislikeService(DbContext);
            var postList = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());
            foreach (var id in postList)
            {
                var post = postService.GetPostById(id);

                // Assign a smiley according to if this user has liked or dislike or not done either
                string lPic = null, dPic = null;
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
                            ProfilePicture = userService.GetProfilePicture(post.UserId).PhotoPath
                        },
                        LikePic = lPic,
                        DislikePic = dPic
                    });
            }

            return View(newsFeed);
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult BestFriend(FormCollection collection)
        {
            string friendId = collection["friendId"];
            if (friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);
            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), friendId);
            var count = 0;
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
            DbContext.SaveChanges();

            return Json( new { FullStar = count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Authorize]
        public ActionResult Family(FormCollection collection)
        {
            string friendId = collection["friendId"];
            if (friendId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);
            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), friendId);
            var count = 0;
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
            DbContext.SaveChanges();

            return Json(new { FullStar = count }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Authorize]
        public ActionResult Profile(string id)
        {
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);
            var photoService = new PhotoService(DbContext);

            // Get the user cover photo, if he has none then display whitebackground.jpg
            var coverPhotoId = userService.GetCoverPhoto(id);
            var coverPhoto = photoService.GetPhotoById(coverPhotoId);
            string coverPhotoPath = coverPhoto == null ? "~/Content/images/whitebackground.jpg" : coverPhoto.PhotoPath;

            // Get if the current user and the profile user are best friends or family
            var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), id);
            string bfStarPick = "~/Content/images/emptystar.png", fStarPick = "~/Content/images/emptystar.png";
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

            var postService = new PostService(DbContext);
            var likedislikeService = new LikeDislikeService(DbContext);

            // Fill in the postviewmodel with all the profile user posts
            var postsViewModels = new List<PostsViewModel>();
            var user = userService.GetUserById(id);
            var posts = userService.GetAllPostsFromUser(id);
            foreach (var postId in posts)
            {
                // Assign a smiley according to if this user has liked or dislike or not done either
                string lPic = null, dPic = null;
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
                         ProfilePicture = userService.GetProfilePicture(post.UserId).PhotoPath,
                         CoverPhoto = coverPhotoPath,
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

            // Fill in the profileviewmodel with the statuses and information of the profile user
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
                    ProfilePicture = userService.GetProfilePicture(id).PhotoPath,
                    CoverPhoto = coverPhotoPath,
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

        [HttpGet, Authorize]
        public ActionResult Notifications()
        {
            var userService = new UserService(DbContext);
            var groupService = new GroupService(DbContext);

            var model = new List<NotificationViewModel>();
            var notifications = userService.GetAllNotificationsForUser(User.Identity.GetUserId());
            foreach (var item in notifications)
            {
                var user = userService.GetUserById(item.FriendUserId);
                var friend = userService.GetUserById(item.UserId);

                var usersNotifications = new NotificationViewModel()
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
                        ProfilePicture = userService.GetProfilePicture(item.UserId).PhotoPath
                    },
                    NotificationId = item.NotificationId,
                    Date = item.Date,
                    GroupId = item.GroupId
                };

                // If groupId is equal to -1 it is a friend request else it is a group notification
                if (item.GroupId != -1)
                {
                    var myGroup = groupService.GetGroupById(item.GroupId);
                    usersNotifications.Group = new GroupDetailViewModel()
                    {
                        Name = myGroup.Name
                    };
                }
                
                model.Add(usersNotifications);
            }

            return View(model);
        }

        [HttpGet, Authorize]
        public ActionResult NotificationCounter()
        {
            var userService = new UserService(DbContext);
            var notifications = userService.GetAllNotificationsForUser(User.Identity.GetUserId()).Count;

            return Json(notifications, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Authorize]
        public ActionResult Search(string searchWord)
        {
            if (searchWord.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var userService = new UserService(DbContext);

            var searchResult = new List<SearchViewModel>();
            var searchList = userService.GetPossibleUsersByName(searchWord);
            foreach (var id in searchList)
            {
                // Get if the current user and the searched user are best friends or family
                var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), id);
                string bfStarPick = "", fStarPick = "";
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

                var user = userService.GetUserById(id);
                searchResult.Add(
                    new SearchViewModel()
                    {
                        User = new UserViewModel()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Name = user.Name,
                            ProfilePicture = userService.GetProfilePicture(id).PhotoPath,
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Work = user.Work,
                            School = user.School,
                            Address = user.Address,
                            FStar = fStarPick,
                            BfStar = bfStarPick
                        }
                    });
            }

            return View(searchResult);
        }
       
        [HttpGet, Authorize]
        public ActionResult FriendsList()
        {
            var userService = new UserService(DbContext);

            var friends = new List<FriendViewModel>();
            var friendList = userService.GetFriendsFromUser(User.Identity.GetUserId());
            foreach (var id in friendList)
            {
                // Get if the current user and the friend user are best friends or family
                var friendShip = userService.GetFriendShip(User.Identity.GetUserId(), id);
                string bfStarPick, fStarPick;
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
                            ProfilePicture = userService.GetProfilePicture(id).PhotoPath,
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
        
        [HttpGet, Authorize]
        public ActionResult Birthdays()
        {
            var userService = new UserService(DbContext);

            var birthdays = new List<BirthdayViewModel>();
            var birthdayList = userService.GetAllFriendsBirthdays(User.Identity.GetUserId());
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
                            ProfilePicture = userService.GetProfilePicture(id).PhotoPath,
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

        [HttpGet, Authorize]
        public ActionResult BirthdayCounter()
        {
            var userService = new UserService(DbContext);
            var birthdays = userService.GetAllFriendsBirthdays(User.Identity.GetUserId()).Count;

            return Json(birthdays, JsonRequestBehavior.AllowGet);
        }
    }
}