using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;

namespace ConnectIn.Controllers
{
    public class GroupController : Controller
    {
        public ActionResult Create(FormCollection collection)
        {
            var group = new Group()
            {
                Name = collection["groupName"]
            };

            var context = new ApplicationDbContext();
            context.Groups.Add(group);
            context.SaveChanges();

            return RedirectToAction("GroupsList", "Group");
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
            List<Group> groupsList = new List<Group>();

            Group g1 = new Group();
            g1.GroupId = 1;
            g1.Name = "Sogn";
            groupsList.Add(g1);

            Group g2 = new Group();
            g2.GroupId = 2;
            g2.Name = "Kleppur";
            groupsList.Add(g2);

            return View(groupsList);
        }
    }
}