using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
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

        public ActionResult Details(int Id)
        {
            var context = new ApplicationDbContext();
            var groupService = new GroupService(context);
            var grp = groupService.GetGroupById(Id);
            return View("GroupDetails", grp);
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
            var groupSercvice = new GroupService(context);

            var groupIdList = userService.GetAllGroupsOfUser(userId);
            var groupList = new List<Group>();

            foreach (var id in groupIdList)
            {
                var group = groupSercvice.GetGroupById(id);
                groupList.Add(group);
            }
            return View(groupList);
        }
    }
}