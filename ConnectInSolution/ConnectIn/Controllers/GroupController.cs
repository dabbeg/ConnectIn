using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Models.ViewModels;
using ConnectIn.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Member = ConnectIn.Models.Entity.Member;

namespace ConnectIn.Controllers
{
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();

        [HttpGet, Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult Create(FormCollection collection)
        {
            string groupName = collection["groupName"];
            if (groupName.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            // Add the new group to the database
            var newGroup = new Group()
            {
                Name = groupName,
                AdminID = User.Identity.GetUserId(),
                Members = new List<Member>()
            };
            DbContext.Groups.Add(newGroup);

            // Make the creator a member of the group
            var userService = new UserService(DbContext);
            DbContext.Members.Add(new Member()
            {
                Group = newGroup,
                User = userService.GetUserById(User.Identity.GetUserId()),
                GroupId = newGroup.GroupId,
                UserId = User.Identity.GetUserId()
            });
            DbContext.SaveChanges();

            return RedirectToAction("GroupsList", "Group");
        }

        [HttpGet, Authorize]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return View("Error");
            }
            int grpId = id.Value;

            // Getting group information
            var userService = new UserService(DbContext);
            var groupService = new GroupService(DbContext);
            var group = groupService.GetGroupById(grpId);
            var myGroup = new GroupDetailViewModel()
            {
                Name = group.Name,
                AdminId = group.AdminID,
                GroupId = grpId,
                Members = new List<UserViewModel>(),
                Posts = new NewsFeedViewModel()
                {
                    Posts = new List<PostsViewModel>(),
                    Id = grpId.ToString()
                },
                User = new UserViewModel()
                {
                    ProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId()).PhotoPath
                },
                FriendsOfUser = new List<UserViewModel>()
            };

            // Getting all members of this group
            var memberList = groupService.GetMembersOfGroup(grpId);
            foreach (var userId in memberList)
            {
                var currMember = userService.GetUserById(userId);
                myGroup.Members.Add(new UserViewModel()
                {
                    Name = currMember.Name,
                    UserId = currMember.Id,
                    UserName = currMember.UserName,
                    Birthday = currMember.Birthday,
                    ProfilePicture = userService.GetProfilePicture(userId).PhotoPath
                });
            }

            // Getting posts for the group
            var postService = new PostService(DbContext);
            var likedislikeService = new LikeDislikeService(DbContext);
            var postsOfGroup = groupService.GetAllPostsOfGroup(grpId);             
            foreach (var userId in postsOfGroup)
            {
                // Assign a smiley according to if this user has liked or dislike or not done either
                var post = postService.GetPostById(userId);
                string lPic = null, dPic = null;
                if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), userId) == null)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), userId).Like)
                {
                    lPic = "~/Content/images/smileyGREEN.png";
                    dPic = "~/Content/images/sadfaceSMALL.png";
                }
                else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), userId).Dislike)
                {
                    lPic = "~/Content/images/smileySMALL.png";
                    dPic = "~/Content/images/sadfaceRED.png";
                }

                myGroup.Posts.Posts.Add(new PostsViewModel()
                {
                    PostId = post.PostId,
                    Body = post.Text,
                    User = new UserViewModel()
                    {
                        Name = post.User.Name,
                        UserId = post.User.Id,
                        UserName = post.User.UserName,
                        Birthday = post.User.Birthday,
                        ProfilePicture = userService.GetProfilePicture(post.UserId).PhotoPath
                    },
                    DateInserted = post.Date,
                    LikeDislikeComment = new LikeDislikeCommentViewModel()
                    {
                        Likes = postService.GetPostsLikes(post.PostId),
                        Dislikes = postService.GetPostsDislikes(post.PostId),
                        Comments = postService.GetPostsCommentsCount(post.PostId)
                    },
                    GroupId = post.GroupId,
                    LikePic = lPic,
                    DislikePic = dPic
                });
            }

            // Getting all friends of the user so he can possibly add them to the group
            var userFriendList = userService.GetFriendsFromUser(User.Identity.GetUserId());
            foreach (var userId in userFriendList)
            {
                if (!groupService.IsMemberOfGroup(grpId, userId))
                {
                    var friend = userService.GetUserById(userId);
                    myGroup.FriendsOfUser.Add(new UserViewModel()
                    {
                        Name = friend.Name,
                        UserId = friend.Id,
                        UserName = friend.UserName,
                        Birthday = friend.Birthday,
                        ProfilePicture = userService.GetProfilePicture(userId).PhotoPath
                    });
                        
                }
            }

            return View("GroupDetails", myGroup);
        }

        [HttpGet, Authorize]
        public ActionResult Edit(string groupId)
        {
            if (groupId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int grpId = Int32.Parse(groupId);

            // Get the group
            var groupService = new GroupService(DbContext);
            var grp = groupService.GetGroupById(grpId);
            var model = new GroupDetailViewModel()
            {
                AdminId = grp.AdminID,
                GroupId = grp.GroupId,
                Name = grp.Name,
                Members = new List<UserViewModel>()
            };

            // Get the members of the group
            var userService = new UserService(DbContext);
            var membersOfGroup = groupService.GetMembersOfGroup(grpId);
            foreach (var id in membersOfGroup)
            {
                var currUser = userService.GetUserById(id);
                if (currUser.Id != User.Identity.GetUserId())
                {
                    model.Members.Add(new UserViewModel()
                    {
                        UserId = currUser.Id,
                        Name = currUser.Name
                    });
                }
            }
            
            return View(model);
        }

        [HttpPost, Authorize]
        public ActionResult Edit(FormCollection collection)
        {
            var grpId = collection["grpId"];
            var newName = collection["nameofgroup"];
            var membersToBeDeleted = collection["toBeDeleted"];
            if (grpId.IsNullOrWhiteSpace() || newName.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int groupId = Int32.Parse(grpId);

            // Changing the name of the group to newName
            var groupService = new GroupService(DbContext);
            var group = groupService.GetGroupById(groupId);
            group.Name = newName;

            // Delete all members selected from the group
            if (membersToBeDeleted != null)
            {
                string[] deleteMembersIds = membersToBeDeleted.Split(',');
                foreach (var id in deleteMembersIds)
                {
                    var userService = new UserService(DbContext);
                    var notify = userService.GetAllNotificationsForUser(id);

                    foreach (var notification in notify)
                    {
                        if (notification.GroupId == groupId)
                        {
                            DbContext.Notifications.Remove(notification);
                        }
                    }
                    var memberToDelete = groupService.GetMemberByUserIdAndGroupId(groupId, id);
                    DbContext.Members.Remove(memberToDelete);
                }
            }
            DbContext.SaveChanges();

            return RedirectToAction("Details", "Group", new {id = grpId});
        }

        [HttpPost, Authorize]
        public ActionResult AddFriend(FormCollection collection)
        {
            string listOfNewMembers = collection["newFriendsInGroup"];
            string idOfGroup = collection["idOfGroup"];
            if (idOfGroup.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int groupId = Int32.Parse(idOfGroup);

            var usersinfo = new List<UserViewModel>();
            if (!listOfNewMembers.IsNullOrWhiteSpace())
            {
                string[] userIdArray = listOfNewMembers.Split(',');

                GroupService groupService = new GroupService(DbContext);
                UserService userService = new UserService(DbContext);

                foreach (var id in userIdArray)
                {
                    var user = userService.GetUserById(id);
                    // Add the member into the group
                    var member = new Member()
                    {
                        Group = groupService.GetGroupById(groupId),
                        GroupId = groupId,
                        User = user,
                        UserId = user.Id
                    };
                    DbContext.Members.Add(member);
                    
                    // Send the member a notification about being added to this group
                    var notification = new Notification()
                    {
                        FriendUserId = user.Id,
                        Date = DateTime.Now,
                        GroupId = groupId,
                        UserId = User.Identity.GetUserId(),
                    };
                    DbContext.Notifications.Add(notification);

                    var birth = user.Birthday.Date;
                    var date = birth.Day + "." + birth.Month + "." + birth.Year;
                    usersinfo.Add(
                        new UserViewModel()
                        {
                            Name = user.Name,
                            UserName = user.UserName,
                            Work = date
                        });
                }
                DbContext.SaveChanges();

                return Json(usersinfo, JsonRequestBehavior.AllowGet);
            }
            return View("Error");
        }

        [HttpGet, Authorize]
        public ActionResult GroupsList()
        {
            var userService = new UserService(DbContext);
            var groupService = new GroupService(DbContext);

            // Get all groups for the current user
            var groupIdList = userService.GetAllGroupsOfUser(User.Identity.GetUserId());
            var model = new List<GroupListViewModel>();
            foreach (var id in groupIdList)
            {
                var groupById = groupService.GetGroupById(id);
                model.Add(
                    new GroupListViewModel
                    {
                        Name = groupById.Name,
                        GroupId = groupById.GroupId
                    });
            }

            return View(model);
        }

        [HttpPost, Authorize]
        public ActionResult LeaveGroup(FormCollection collection)
        {
            var grpId = collection["groupID"];
            var userId = collection["userID"];
            if (grpId.IsNullOrWhiteSpace() || userId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int groupId = Int32.Parse(grpId);

            // Remove the user from this group
            var groupService = new GroupService(DbContext);
            var memberToDelete = groupService.GetMemberByUserIdAndGroupId(groupId, userId);
            DbContext.Members.Remove(memberToDelete);

            // If the user has a notification about the group he is about to leave, the notification is removed.
            // Otherwise, he could still visit the group after he has left it.
            var userService = new UserService(DbContext);
            var notificationService = new NotificationService(DbContext);
            var notifications = userService.GetAllNotificationsForUser(userId);
            foreach (var ntf in notifications)
            {
                var notification = notificationService.GetNotificationById(ntf.NotificationId);
                if (notification.GroupId == groupId)
                {
                    DbContext.Notifications.Remove(notification);
                }
            }
            DbContext.SaveChanges();

            return RedirectToAction("GroupsList");
        }

        [HttpPost, Authorize]
        public ActionResult DeleteGroup(FormCollection collection)
        {
            var grpId = collection["groupID"];
            if (grpId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int groupId = Int32.Parse(grpId);

            // Delete the group
            var groupService = new GroupService(DbContext);
            var groupToDelete = groupService.GetGroupById(groupId);
            DbContext.Groups.Remove(groupToDelete);

            // Remove all notifications about the group
            var notifications = groupService.GetNotificatonsForGroup(groupId);
            foreach (var n in notifications)
            {
                DbContext.Notifications.Remove(n);
            }
            DbContext.SaveChanges();

            return RedirectToAction("GroupsList");
        }
    }
}