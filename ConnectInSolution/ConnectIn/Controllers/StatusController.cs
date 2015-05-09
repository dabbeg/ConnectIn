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

        public ActionResult Comment(int ? PostId)
        {
            if (PostId == null)
            {
                return RedirectToAction("NewsFeed", "Home");
            }
            int postId = (int) PostId;
            string lPic, dPic;
            var db = new ApplicationDbContext();
            var userService = new UserService(db);
            var commentService = new CommentService(db);
            var postService = new PostService(db);
            var likedislikeService = new LikeDislikeService(db);
            if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId) == null)
            {
                lPic = "~/Content/images/smileySMALL.png";
                dPic = "~/Content/images/sadfaceSMALL.png";
            }
            else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId).Like)
            {
                lPic = "~/Content/images/smileyGREEN.png";
                dPic = "~/Content/images/sadfaceSMALL.png";
            }
            else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), postId).Dislike)
            {
                lPic = "~/Content/images/smileySMALL.png";
                dPic = "~/Content/images/sadfaceRED.png";
            }
            else
            {
                return View("Error");
            }
            var comments = new CommentHelperViewModel
            {
                Comments = new List<CommentViewModel>(),
                Post = new PostsViewModel()
                {
                    PostId = postId,
                    Body = postService.GetPostById(postId).Text,
                    DateInserted = postService.GetPostById(postId).Date,
                    LikeDislikeComment = new LikeDislikeCommentViewModel()
                    {
                        Likes = postService.GetPostsLikes(postId),
                        Dislikes = postService.GetPostsDislikes(postId),
                        Comments = postService.GetPostsCommentsCount(postId)
                    },
                    User = new UserViewModel()
                    {
                        UserId = postService.GetPostById(postId).UserId,
                        Name = userService.GetUserById(postService.GetPostById(postId).UserId).Name,
                        ProfilePicture = "~/Content/Images/profilepic.png"
                    },
                    LikePic = lPic,
                    DislikePic = dPic
                }
            };
            var commentIdList = postService.GetPostsComments(postId);
            foreach (var id in commentIdList)
            {
                var commentList = commentService.GetCommentById(id);
                comments.Comments.Add(
                    new CommentViewModel()
                    {
                        CommentId = commentList.CommentId,
                        PostId = commentList.PostId,
                        Body = commentList.Text,
                        DateInserted = commentList.Date,
                        User = new UserViewModel()
                        {
                            UserId = commentList.UserId,
                            Name = userService.GetUserById(commentList.UserId).Name,
                            UserName = userService.GetUserById(commentList.UserId).UserName,
                            ProfilePicture = "~/Content/Images/profilepic.png"
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

            return RedirectToAction("Comment", "Status", new {postId = collection["postId"].AsInt()});
        }

        public ActionResult RemoveComment(int ? commentId)
        {
            if (!commentId.HasValue)
            {
                return View("Error");
            }
            int id = commentId.Value;

            var db = new ApplicationDbContext();
            var commentService = new CommentService(db);
            db.Comments.Remove(commentService.GetCommentById(id));
            int postId = commentService.GetCommentById(id).PostId;

            db.SaveChanges();

            return RedirectToAction("Comment", "Status", new {postId});

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

            var likedislikeService = new LikeDislikeService(context);
            var id = Int32.Parse(postId);

            var ld = likedislikeService.GetLikeDislike(User.Identity.GetUserId(), id);
            if (ld != null)
            {
                if (ld.Dislike)
                {
                    context.LikesDislikes.Remove(ld);
                }
                else if (ld.Like) // UnLike
                {
                    context.LikesDislikes.Remove(ld);
                    context.SaveChanges();
                    if (location.Equals("Comment"))
                    {
                        return RedirectToAction("Comment", "Status", new { postId });
                    }
                    return RedirectToAction(location, "Home", new { id = profileOrGroupId });
                }
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

            if (location.Equals("Comment"))
            {
                return RedirectToAction("Comment", "Status", new { postId });
            }
            return RedirectToAction(location, "Home", new { id = profileOrGroupId });
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
            var likedislikesService = new LikeDislikeService(context);

            var pid = Int32.Parse(postId);

            var ld = likedislikesService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                if (ld.Like)
                {
                    context.LikesDislikes.Remove(ld);
                }
                else if (ld.Dislike) // UnDislike
                {
                    context.LikesDislikes.Remove(ld);
                    context.SaveChanges();
                    if (location.Equals("Comment"))
                    {
                        return RedirectToAction("Comment", "Status", new { postId });
                    }
                    return RedirectToAction(location, "Home", new { id = profileOrGroupId });
                }
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
            if (location.Equals("Comment"))
            {
                return RedirectToAction("Comment", "Status", new {postId});
            }
            return RedirectToAction(location, "Home", new { id = profileOrGroupId });
        }

    }
}