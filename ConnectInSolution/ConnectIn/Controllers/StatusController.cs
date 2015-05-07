using System;
using System.Web.Mvc;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;

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

        public ActionResult RemovePost(int? postId)
        {
            if (!postId.HasValue)
            {
                return View("Error");
            }
            int id = postId.Value;

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            context.Posts.Remove(postService.GetPostById(id));
            context.SaveChanges();

            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult Comment()
        {
            return View();
        }

        public ActionResult AddComment()
        {
            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult RemoveComment()
        {
            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult Like(FormCollection collection)
        {
            string postId = collection["postId"];
            string location = collection["location"];
            string profileOrGroupId = collection["id"];


            if (postId.IsNullOrWhiteSpace() || location.IsNullOrWhiteSpace() || profileOrGroupId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var id = Int32.Parse(postId);

            var ld = postService.GetLikeDislike(User.Identity.GetUserId(), id);
            if (ld != null)
            {
                context.LikesDislikes.Remove(ld);
            }
            
            var model = new LikeDislike()
            {
                PostId = id,
                UserId = User.Identity.GetUserId(),
                Like = true,
                Dislike = false
            };
            
            context.LikesDislikes.Add(model);
            context.SaveChanges();

            return RedirectToAction(location, "Home", new { id = profileOrGroupId });
        }

        public ActionResult UnLike(int? postId)
        {
            return View();
        }

        public ActionResult Dislike(FormCollection collection)
        {
            string postId = collection["postId"];
            string location = collection["location"];
            string profileOrGroupId = collection["id"];

            if (postId.IsNullOrWhiteSpace() || location.IsNullOrWhiteSpace() || profileOrGroupId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var pid = Int32.Parse(postId);

            var ld = postService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                context.LikesDislikes.Remove(ld);
            }

            var model = new LikeDislike()
            {
                PostId = pid,
                UserId = User.Identity.GetUserId(),
                Like = false,
                Dislike = true
            };

            context.LikesDislikes.Add(model);
            context.SaveChanges();

            return RedirectToAction(location, "Home", new { id = profileOrGroupId });
        }

        public ActionResult UnDislike(int? postId)
        {
            return View();
        }
    }
}