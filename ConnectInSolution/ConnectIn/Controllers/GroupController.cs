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

            if (!id.HasValue)
            {
                return View("Error");
            }
            else
            {
                int grpId = id.Value;
                var memberList = groupService.GetMembersOfGroup(grpId);
                var group = groupService.GetGroupById(grpId);

                var myGroup = new GroupDetailViewModel()
                {
                    Name = group.Name,
                    GroupId = grpId,
                    Members = new List<UserViewModel>(),
                    Posts = new NewsFeedViewModel()
                    {
                        Posts = new List<PostsViewModel>(),
                        Id = grpId.ToString()
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
                        ProfilePicture = userService.GetProfilePicture(currMember.Id).PhotoPath,
                        Gender = currMember.Gender,
                        Work = currMember.Work,
                        School = currMember.School,
                        Address = currMember.Address
                    });
                }
                var postsOfGroup = groupService.GetAllPostsOfGroup(grpId);
                int myId = id.Value;

                foreach (var userId in postsOfGroup)
                {
                    var post = postService.GetPostById(userId);
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
                            ProfilePicture = userService.GetProfilePicture(post.User.Id).PhotoPath,
                            Gender = post.User.Gender,
                            Work = post.User.Work,
                            School = post.User.School,
                            Address = post.User.Address
                        },
                        DateInserted = post.Date,
                        LikeDislikeComment = new LikeDislikeCommentViewModel()
                        {
                            Likes = postService.GetPostsLikes(post.PostId),
                            Dislikes = postService.GetPostsDislikes(post.PostId),
                            Comments = postService.GetPostsCommentsCount(post.PostId)
                        },
                        GroupId = post.GroupId
                    });
                }

                var userFriendList = userService.GetFriendsFromUser(User.Identity.GetUserId());

                bool isGroupMemberAlready = false;
                
                foreach (var userId in userFriendList)
                {
                    var usersFriend = userService.GetUserById(userId);

                    foreach (var Id in memberList)
                    {
                        var member = userService.GetUserById(Id);
                        if (member.Id == usersFriend.Id)
                        {
                            isGroupMemberAlready = true;
                        }
                        
                    }
                    if (isGroupMemberAlready == false)
                    {
                        var friend = userService.GetUserById(userId);

                        myGroup.FriendsOfUser.Add(new UserViewModel()
                        {
                            Name = friend.Name,
                            UserId = friend.Id,
                            UserName = friend.UserName,
                            Birthday = friend.Birthday,
                            ProfilePicture = userService.GetProfilePicture(friend.Id).PhotoPath,
                            Gender = friend.Gender,
                            Work = friend.Work,
                            School = friend.School,
                            Address = friend.Address
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

        public ActionResult Edit()
        {
            return View();
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
    }
}