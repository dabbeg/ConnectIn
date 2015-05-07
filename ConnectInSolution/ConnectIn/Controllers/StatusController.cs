using System;
using System.Web.Mvc;
using System.Web.WebPages;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;
using ConnectIn.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;
using ConnectIn.Models.ViewModels;
using System.Collections.Generic;

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

        public ActionResult Comment(FormCollection collection)
        {
            var pId = collection["postId"];
            int postId = pId.AsInt();

            var db = new ApplicationDbContext();
            var userService = new UserService(db);
            var commentService = new CommentService(db);
            var postService = new PostService(db);

            var comments = new List<CommentViewModel>();
            var commentIdList = postService.GetPostsComments(postId);
            foreach (var id in commentIdList)
            {
                var commentList = commentService.GetCommentById(id);
                comments.Add(
                    new CommentViewModel()
                    {
                        Body = commentList.Text,
                        DateInserted = commentList.Date,
                        CommentId = commentList.CommentId,
                        PostId = commentList.PostId,
                        User = new UserViewModel()
                        {
                            UserId = commentList.UserId,
                            Name = userService.GetUserById(commentList.UserId).Name
                        }
                    });
            }

            return View(comments);
        }

        public ActionResult AddComment(FormCollection collection)
        {
            var comment = new Comment
            {
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                Text = collection["status"],
                PostId = collection["postId"].AsInt()
            };
            var db = new ApplicationDbContext();
            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Comment", "Status");
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