using ConnectIn.DAL;
using ConnectIn.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConnectIn.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // if i send null, the other one will be used (database?)
            // var service = new UserService(null);

            // var friendlist = service.GetFriendsFromUser(this.User.Identity.Name);

            // return View(friendlist);
            return View();
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

        public ActionResult Profile()
        {
            return View();
        }
    }
}