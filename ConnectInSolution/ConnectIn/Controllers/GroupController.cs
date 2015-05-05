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
        public ActionResult Create(FormCollection collection)
        {
            //return RedirectToAction("GroupsList", "Group");
            return View("CreateGroup");
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
        public ActionResult GroupsList(FormCollection collection)
        {
            /*List<Group> groupsList = new List<Group>();

            Group g1 = new Group();
            g1.GroupId = 1;
            g1.Name = "Sogn";
            groupsList.Add(g1);

            Group g2 = new Group();
            g2.GroupId = 2;
            g2.Name = "Kleppur";
            groupsList.Add(g2);

            return View(groupsList);*/

            var group = new Group()
            {
                Name = collection["groupName"]
            };

            var context = new ApplicationDbContext();
            context.Groups.Add(group);
            context.SaveChanges();

            /*-----*/

            var userId = User.Identity.GetUserId();

            var context2 = new ApplicationDbContext();
            var userService = new UserService(context2);
            var groupService = new GroupService(context2);

            var groupIdList = userService.GetAllGroupsOfUser(userId);
            var groupList = new List<Group>();

            foreach (var id in groupIdList)
            {
                var grp = groupService.GetGroupById(id);
                groupList.Add(grp);
            }

            return View(groupList);
        }
    }
}