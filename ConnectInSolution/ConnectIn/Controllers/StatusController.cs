using System;
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

            if (collection["location"].Equals("newsfeed"))
            {
                context.Posts.Add(post);
                context.SaveChanges();
                return RedirectToAction("NewsFeed", "Home");
            }
            if (collection["location"].Equals("group"))
            {
                var grpId = Int32.Parse(collection["idOfGroup"]);
                post.GroupId = grpId;

                context.Posts.Add(post);
                context.SaveChanges();
                return RedirectToAction("Details", "Group", new {Id = grpId});
                
            }
            return View();

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

        public ActionResult Like(int? postId)
        {
            if (!postId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var id = postId.Value;

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

            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult UnLike(int? postId)
        {
            return View();
        }

        public ActionResult Dislike(int? postId)
        {
            if (!postId.HasValue)
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var id = postId.Value;

            var ld = postService.GetLikeDislike(User.Identity.GetUserId(), id);
            if (ld != null)
            {
                context.LikesDislikes.Remove(ld);
            }

            var model = new LikeDislike()
            {
                PostId = id,
                UserId = User.Identity.GetUserId(),
                Like = false,
                Dislike = true
            };

            context.LikesDislikes.Add(model);
            context.SaveChanges();

            return RedirectToAction("NewsFeed", "Home");
        }

        public ActionResult UnDislike(int? postId)
        {
            return View();
        }
    }
}