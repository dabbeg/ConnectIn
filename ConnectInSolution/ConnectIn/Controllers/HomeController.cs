using ConnectIn.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.Models.Entity;

namespace ConnectIn.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewsFeed()
        {
            List<Post> NewsFeed = new List<Post>();
            Post post1 = new Post();
            post1.text = "Er jarðskjálft­inn varð í Nepal fyr­ir viku var níu ára göm­ul stúlka, sem dýrkuð er sem gyðja, að und­ir­búa sig fyr­ir að taka á móti til­biðjend­um á heim­ili sínu sem stend­ur við Dur­bar-torgið í Kat­mandú.";
            post1.ID = 1;
            post1.userID = "1";
            NewsFeed.Add(post1);

            Post post2 = new Post();
            post2.text = "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet.";
            post2.ID = 2;
            post2.userID = "2";
            NewsFeed.Add(post2);

            return View(NewsFeed);
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