using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Models.ViewModels;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class GroupController : Controller
    {
        public ActionResult Create()
        {
            return View("CreateGroup");
        }

        public ActionResult CreateGroup(FormCollection collection)
        {
            var context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            UserService userService = new UserService(context);
            GroupService groupService = new GroupService(context);

            var newGroup = new Group()
            {
                Name = collection["groupName"],
                AdminID = User.Identity.GetUserId(),
                Members = new List<Member>()
            };

            context.Groups.Add(newGroup);
            //Make the creator a member of the group
            context.Members.Add(new Member()
            {
                Group = newGroup,
                User = userService.GetUserById(userId),
                GroupId = newGroup.GroupId,
                UserId = userId
            });

            context.SaveChanges();

            return RedirectToAction("GroupsList", "Group");
        }

        public ActionResult Details(int? id)
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var groupService = new GroupService(context);
            var postService = new PostService(context);
            var likedislikeService = new LikeDislikeService(context);

            if (!id.HasValue)
            {
                return View("Error");
            }
            else
            {
                int grpId = id.Value;
                var memberList = groupService.GetMembersOfGroup(grpId);
                var group = groupService.GetGroupById(grpId);
                
                var profilePicture = userService.GetProfilePicture(User.Identity.ToString());
                    string profilePicturePath;

                    if (profilePicture == null)
                    {
                        profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                    }
                    else
                    {
                        profilePicturePath = profilePicture.PhotoPath;
                    }

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
                        ProfilePicture = profilePicturePath
                    },
                    FriendsOfUser = new List<UserViewModel>()
                };
                foreach (var userId in memberList)
                {
                    var currMember = userService.GetUserById(userId);
                    myGroup.Members.Add(new UserViewModel()
                    {
                        Name = currMember.Name,
                        UserId = currMember.Id,
                        UserName = currMember.UserName,
                        Birthday = currMember.Birthday,
                        ProfilePicture = "~/Content/Images/profilepic.png/"
                        /*Gender = currMember.Gender,
                        Work = currMember.Work,
                        School = currMember.School,
                        Address = currMember.Address*/
                    });
                }
                var postsOfGroup = groupService.GetAllPostsOfGroup(grpId);
                int myId = id.Value;
               
                
                foreach (var userId in postsOfGroup)
                {
                    string lPic, dPic;
                    var post = postService.GetPostById(userId);
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
                    else
                    {
                        return View("Error");
                    }
                    //If the user hasn't picked a profile pic, set a default one.
                    profilePicture = userService.GetProfilePicture(userId.ToString());
                    

                    if (profilePicture == null)
                    {
                        profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                    }
                    else
                    {
                        profilePicturePath = profilePicture.PhotoPath;
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
                            ProfilePicture = profilePicturePath
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

                var userFriendList = userService.GetFriendsFromUser(User.Identity.GetUserId());
                
                foreach (var userId in userFriendList)
                {
                    if (!groupService.isMemberOfGroup(grpId, userId))
                    {
                        var friend = userService.GetUserById(userId);

                        //If the user hasn't picked a profile pic, set a default one.
                        profilePicture = userService.GetProfilePicture(userId);
                        

                        if (profilePicture == null)
                        {
                            profilePicturePath = "~/Content/images/largeProfilePic.jpg";
                        }
                        else
                        {
                            profilePicturePath = profilePicture.PhotoPath;
                        }

                        myGroup.FriendsOfUser.Add(new UserViewModel()
                        {
                            Name = friend.Name,
                            UserId = friend.Id,
                            UserName = friend.UserName,
                            Birthday = friend.Birthday,
                            ProfilePicture = profilePicturePath
                        });
                        
                    }
                }
                return View("GroupDetails", myGroup);
            }
        }

        public ActionResult Delete()
        {
            return View();
        }

        public ActionResult Edit(FormCollection collection)
        {
            var context = new ApplicationDbContext();
            var groupService = new GroupService(context);
            var userService = new UserService(context);

            var grpId = collection["groupID"];
            var grp = groupService.GetGroupById(Int32.Parse(grpId));
            var groupToEdit = new GroupDetailViewModel()
            {
                AdminId = grp.AdminID,
                GroupId = grp.GroupId,
                Name = grp.Name,
                Members = new List<UserViewModel>()
            };

            var membersOfGroup = groupService.GetMembersOfGroup(Int32.Parse(grpId));

            foreach (var Id in membersOfGroup)
            {
                var currUser = userService.GetUserById(Id);
                groupToEdit.Members.Add(new UserViewModel()
                {
                    UserId = currUser.Id,
                    Name = currUser.Name
                });
            }
            
            return View("EditGroup", groupToEdit);
        }

        public ActionResult EditGroup(FormCollection collection)
        {
            var context = new ApplicationDbContext();
            var groupService = new GroupService(context);
            var userService = new UserService(context);

            var newName = collection["nameofgroup"];
            var membersToBeDeleted = collection["toBeDeleted"];

            string[] deleteMembersIds = membersToBeDeleted.Split(',');
            var grpId = collection["grpId"];

            bool isAdmin = false;
            foreach (var Id in deleteMembersIds)
            {
                var memberToDelete = groupService.GetMemberByUserIdAndGroupId(Int32.Parse(grpId), Id);
                if (memberToDelete.UserId == User.Identity.GetUserId())
                {
                    isAdmin = true;
                }
                context.Members.Remove(memberToDelete);
            }
            context.SaveChanges();

            if (isAdmin == true)
            {
                return RedirectToAction("GroupsList", "Group");
            }
            return RedirectToAction("Details", "Group", new {id = grpId});
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult AddFriend(FormCollection collection)
        {
            string listOfNewMembers = collection["newFriendsInGroup"];
            string[] userIdArray = listOfNewMembers.Split(',');

            string groupId = collection["idOfGroup"];
            var context = new ApplicationDbContext();
            GroupService groupService = new GroupService(context);
            UserService userService = new UserService(context);
            var currentGroup = groupService.GetGroupById(Int32.Parse(groupId));

            foreach (var id in userIdArray)
            {
                var user = userService.GetUserById(id);
                context.Members.Add(new Member()
                {
                    Group = currentGroup,
                    GroupId = Int32.Parse(groupId),
                    User = user,
                    UserId = user.Id
                });
                context.Notifications.Add(new Notification()
                {
                    FriendUserId = user.Id,
                    Date = DateTime.Now,
                    GroupId = groupId,
                    UserId = User.Identity.GetUserId(),
                });
            }
            context.SaveChanges();

            return RedirectToAction("Details", "Group", new { id = groupId });
        }

        public ActionResult RemoveFriend()
        {
            return View();
        }

        public ActionResult GetUser(string Id)
        {
            return RedirectToAction("Profile", "Home", new { id = Id });
        }

        public ActionResult GroupsList()
        {
            var userId = User.Identity.GetUserId();

            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var groupService = new GroupService(context);

            var groupIdList = userService.GetAllGroupsOfUser(userId);
            var groupList = new List<GroupListViewModel>();

            foreach (var id in groupIdList)
            {
                var groupById = groupService.GetGroupById(id);
                var newGroup = new GroupListViewModel();
                newGroup.Name = groupById.Name;
                newGroup.GroupId = groupById.GroupId;
                groupList.Add(newGroup);
            }
            return View(groupList);
        }

        public ActionResult LeaveGroup(FormCollection collection)
        {
            var context = new ApplicationDbContext();
            var groupService = new GroupService(context);
            var userService = new UserService(context);

            var grpId = Int32.Parse(collection["groupID"]);
            var userId = collection["userID"];

            var memberToDelete = groupService.GetMemberByUserIdAndGroupId(grpId, userId);
            context.Members.Remove(memberToDelete);

            var notifications = userService.GetAllNotificationsForUser(userId);
            foreach (var ntf in notifications)
            {
                var notification = userService.GetNotificationById(ntf.NotificationId);
                if (notification.GroupId == grpId.ToString())
                {
                    context.Notifications.Remove(notification);
                }
            }
            context.SaveChanges();
            return RedirectToAction("GroupsList");
        }
    }
}