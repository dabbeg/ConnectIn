using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;

namespace ConnectIn.Controllers
{
    public class StatusController : Controller
    {
        public ActionResult AddPost(FormCollection collection)
        {
            var post = new Post
            {
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                Text = collection["status"]
            };

            var context = new ApplicationDbContext();
            context.Posts.Add(post);
            context.SaveChanges();

            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult RemovePost()
        {
            return View();
        }

        public ActionResult AddComment()
        {
            return View();
        }

        public ActionResult RemoveComment()
        {
            return View();
        }

        public ActionResult Like()
        {
            return View();
        }

        public ActionResult UnLike()
        {
            return View();
        }

        public ActionResult Dislike()
        {
            return View();
        }

        public ActionResult UnDislike()
        {
            return View();
        }
    }
}