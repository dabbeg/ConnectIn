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

            Group newGroup = new Group()
            {
                Name = collection["groupName"],
            };
            newGroup.Members = new List<Member>();
            //Make the creator a member of the group
            Member groupMember = new Member();

            groupMember.GroupId = newGroup.GroupId;
            groupMember.UserId = userId;
            groupMember.Group = newGroup;
            Models.Entity.User currentUser = userService.GetUserById(userId);
            groupMember.User = currentUser;

            newGroup.Members.Add(groupMember);

            context.Groups.Add(newGroup);
            context.SaveChanges();

            return RedirectToAction("GroupsList", "Group");
        }

        public ActionResult Details(int ? Id)
        {
            var context = new ApplicationDbContext();
            var userService = new UserService(context);
            var groupService = new GroupService(context);
            var postService = new PostService(context);

            if (!Id.HasValue)
            {
                return View("Error");
            }
            else
            {
                int grpId = Id.Value;
                var memberList = groupService.GetMembersOfGroup(grpId);
                var group = groupService.GetGroupById(grpId);

                var myGroup = new GroupDetailViewModel()
                {
                    Name = group.Name,
                    GroupId = grpId,
                    Users = new List<UserViewModel>(),
                    Posts = new NewsFeedViewModel()
                    {
                        Posts = new List<PostsViewModel>(),
                        Id = grpId.ToString()
                    }
                };
                foreach (var id in memberList)
                {
                    var currMember = userService.GetUserById(id);
                    myGroup.Users.Add(new UserViewModel()
                    {
                        Name = currMember.Name,
                        UserId = currMember.Id,
                        UserName = currMember.UserName,
                        Birthday = currMember.Birthday,
                        ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                        Gender = currMember.Gender,
                        Work = currMember.Work,
                        School = currMember.School,
                        Address = currMember.Address
                    });
                }
                var postsOfGroup = groupService.GetAllPostsOfGroup(grpId);

                foreach (var id in postsOfGroup)
                {
                    var post = postService.GetPostById(id);
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
                            ProfilePicture = "~/Content/images/largeProfilePic.jpg",
                            Gender = post.User.Gender,
                            Work = post.User.Work,
                            School = post.User.School,
                            Address = post.User.Address
                        },
                        DateInserted = post.Date,
                        Comments = new List<CommentViewModel>(),
                        LikeDislike = new LikeDislikeViewModel()
                        {
                            Likes = postService.GetPostsLikes(post.PostId),
                            Dislikes = postService.GetPostsDislikes(post.PostId)
                        },
                        GroupId = post.GroupId
                    });
                }
                return View("GroupDetails", myGroup);
            }
            return View();
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

        public ActionResult AddFriend()
        {
            return View();
        }

        public ActionResult RemoveFriend()
        {
            return View();
        }

        public ActionResult GetUser(string Id)
        {
            return RedirectToAction("Profile", "Home", new {id = Id});
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