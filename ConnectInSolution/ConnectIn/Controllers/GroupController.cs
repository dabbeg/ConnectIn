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
            if (!Id.HasValue)
            {
                return View("Error");
            }
            else
            {
                int myId = Id.Value;
                var memberList = groupService.GetMembersOfGroup(myId);
                var group = groupService.GetGroupById(myId);

                var members = new List<GroupDetailViewModel>();

                foreach (var id in memberList)
                {
                    var user = userService.GetUserById(id);

                    members.Add(
                        new GroupDetailViewModel()
                        {
                            Name = group.Name,
                            GroupId = group.GroupId,
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
                return View("GroupDetails", members);
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

        public ActionResult AddFriend()
        {
            return View();
        }

        public ActionResult RemoveFriend()
        {
            return View();
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