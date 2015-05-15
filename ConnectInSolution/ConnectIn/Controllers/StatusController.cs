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
using Microsoft.Ajax.Utilities;

namespace ConnectIn.Controllers
{
    public class StatusController : Controller
    {
        private readonly ApplicationDbContext DbContext = new ApplicationDbContext();

        [HttpPost, Authorize]
        public ActionResult AddPost(FormCollection collection)
        {
            string amount = collection["amount"];
            string status = collection["status"];
            string groupId = collection["idOfGroup"];
            if(status.IsNullOrWhiteSpace() || amount.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            // Create the new post and add it into the database
            DateTime now = DateTime.Now;
            var newPost = new Post
            {
                UserId = User.Identity.GetUserId(),
                Date = now,
                Text = status
            };

            if (!groupId.IsNullOrWhiteSpace())
            {
                newPost.GroupId = Int32.Parse(groupId);
            }

            DbContext.Posts.Add(newPost);
            DbContext.SaveChanges();

            // Find out if this post belongs to the newsfeed or a group
            var groupService = new GroupService(DbContext);
            var userService = new UserService(DbContext);
            var list = new List<int>();
            if (groupId.IsNullOrWhiteSpace())
            {
                list = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());
            }
            else
            {
                list = groupService.GetAllPostsOfGroup(Int32.Parse(groupId));
            }

            var postService = new PostService(DbContext);
            var likeDislikeService = new LikeDislikeService(DbContext);
            var postList = new List<PostsViewModel>();
            int nOfCurrentStatuses = Int32.Parse(amount);

            // Get all statuses that are not currently displayed and display them
            for (int i = (list.Count - nOfCurrentStatuses) - 1; i >= 0 ; i--)
            {
                var post = postService.GetPostById(list[i]);

                // Assign a smiley according to if this user has liked or dislike or not done either
                var likeDislike = likeDislikeService.GetLikeDislike(post.UserId, post.PostId);
                string lPic = null, dPic = null;
                if (likeDislike == null)
                {
                    lPic = "/Content/images/smileySMALL.png";
                    dPic = "/Content/images/sadfaceSMALL.png";
                }
                else if (likeDislike.Like)
                {
                    lPic = "/Content/images/smileyGREEN.png";
                    dPic = "/Content/images/sadfaceSMALL.png";
                }
                else if (likeDislike.Dislike)
                {
                    lPic = "/Content/images/smileySMALL.png";
                    dPic = "/Content/images/sadfaceRED.png";
                }

                // Fill in the viewmodel
                postList.Add(
                    new PostsViewModel()
                    {
                        PostId = post.PostId,
                        Body = post.Text,
                        User = new UserViewModel()
                        {
                            UserId = post.UserId,
                            Name = userService.GetUserById(post.UserId).Name,
                            ProfilePicture = userService.GetProfilePicture(post.UserId).PhotoPath
                        },
                        DateInserted = post.Date,
                        LikeDislikeComment = new LikeDislikeCommentViewModel()
                        {
                            Likes = postService.GetPostsLikes(post.PostId),
                            Dislikes = postService.GetPostsDislikes(post.PostId),
                            Comments = postService.GetPostsComments(post.PostId).Count
                        },
                        LikePic = lPic,
                        DislikePic = dPic,
                        isUserPostOwner = (post.UserId == User.Identity.GetUserId()) 
                    });
            }

            return Json(postList, JsonRequestBehavior.AllowGet);        
        }


        [HttpPost, Authorize]
        public ActionResult RemovePost(FormCollection collection)
        {
            string id = collection["postId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int postId = Int32.Parse(id);


            // Remove the post
            var postService = new PostService(DbContext);
            var post = postService.GetPostById(postId);
            if (post != null)
            {
                DbContext.Posts.Remove(post);
                DbContext.SaveChanges();
            }
            
            // If we are deleting from the comment view the action is redirected to newsfeed
            var url = ControllerContext.HttpContext.Request.UrlReferrer;
            if (url != null && url.AbsolutePath.Contains("Comment"))
            {
                return RedirectToAction("NewsFeed", "Home");
            }

            // If we are in the newsfeed we delete and return nothing to the ajax call
            return new EmptyResult();
        }

        [HttpGet, Authorize]
        public ActionResult Comment(int? postId)
        {
            if (!postId.HasValue)
            {
                return RedirectToAction("NewsFeed", "Home");
            }

            // Assign a smiley according to if this user has liked or dislike or not done either
            var likedislikeService = new LikeDislikeService(DbContext);
            int pid = postId.Value;
            string lPic = null, dPic = null;
            if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid) == null)
            {
                lPic = "~/Content/images/smileySMALL.png";
                dPic = "~/Content/images/sadfaceSMALL.png";
            }
            else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid).Like)
            {
                lPic = "~/Content/images/smileyGREEN.png";
                dPic = "~/Content/images/sadfaceSMALL.png";
            }
            else if (likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid).Dislike)
            {
                lPic = "~/Content/images/smileySMALL.png";
                dPic = "~/Content/images/sadfaceRED.png";
            }

            // Fill in the viewmodel
            var userService = new UserService(DbContext);
            var postService = new PostService(DbContext);
            var model = new CommentHelperViewModel
            {
                User = new UserViewModel()
                {
                    UserId = postService.GetPostById(pid).UserId,
                    Name = userService.GetUserById(postService.GetPostById(pid).UserId).Name,
                    ProfilePicture = userService.GetProfilePicture(User.Identity.GetUserId()).PhotoPath

                },
                Comments = new List<CommentViewModel>(),
                Post = new PostsViewModel()
                {
                    PostId = pid,
                    Body = postService.GetPostById(pid).Text,
                    DateInserted = postService.GetPostById(pid).Date,
                    LikeDislikeComment = new LikeDislikeCommentViewModel()
                    {
                        Likes = postService.GetPostsLikes(pid),
                        Dislikes = postService.GetPostsDislikes(pid),
                        Comments = postService.GetPostsCommentsCount(pid)
                    },
                    User = new UserViewModel()
                    {
                        UserId = postService.GetPostById(pid).UserId,
                        Name = userService.GetUserById(postService.GetPostById(pid).UserId).Name,
                        ProfilePicture = userService.GetProfilePicture(postService.GetPostById(pid).UserId).PhotoPath
                    },
                    LikePic = lPic,
                    DislikePic = dPic
                }
            };

            var commentService = new CommentService(DbContext);
            var commentIdList = postService.GetPostsComments(pid);
            foreach (var id in commentIdList)
            {
                var commentList = commentService.GetCommentById(id);
                model.Comments.Add(
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
                            ProfilePicture = userService.GetProfilePicture(commentList.UserId).PhotoPath
                        }
                    });
            }

            return View(model);
        }

        [HttpPost, Authorize]
        public ActionResult AddComment(FormCollection collection)
        {
            string status = collection["status"];
            string postId = collection["postId"];
            string amount = collection["amount"];
            if (status.IsNullOrWhiteSpace() || postId.IsNullOrWhiteSpace() || amount.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int nOfCurrentComments = Int32.Parse(amount);

            // Add the new comment into the database
            var newComment = new Comment
            {
                UserId = User.Identity.GetUserId(),
                Date = DateTime.Now,
                Text = status,
                PostId = Int32.Parse(postId),

            };
            DbContext.Comments.Add(newComment);
            DbContext.SaveChanges();

            // Fill in the viewmodel
            var userService = new UserService(DbContext);
            var postService = new PostService(DbContext);
            var commentService = new CommentService(DbContext);

            var commentList = postService.GetPostsComments(newComment.PostId);
            var json = new List<CommentViewModel>();
            // Get all comments that are not currently displayed and display them
            for (var i = (commentList.Count - nOfCurrentComments)-1; i >= 0; i--)
            {
                var comment = commentService.GetCommentById(commentList[i]);
                json.Add(
                    new CommentViewModel()
                    {
                        CommentId = comment.CommentId,
                        Body = comment.Text,
                        DateInserted = comment.Date,
                        IsUserCommentOwner = comment.UserId == User.Identity.GetUserId(),
                        User = new UserViewModel()
                        {
                            UserId = comment.UserId,
                            Name = userService.GetUserById(comment.UserId).Name,
                            ProfilePicture = userService.GetProfilePicture(comment.UserId).PhotoPath
                        }
                    });
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Authorize]
        public ActionResult RemoveComment(int? commentId)
        {
            if (!commentId.HasValue)
            {
                return View("Error");
            }
            int id = commentId.Value;

            // Remove the comment out of the database
            var commentService = new CommentService(DbContext);
            var comment = commentService.GetCommentById(id);
            if (comment != null)
            {
                DbContext.Comments.Remove(comment);
                DbContext.SaveChanges();
            }
            
            return new EmptyResult();
        }

        [HttpPost, Authorize]
        public ActionResult Like(FormCollection collection)
        {
            string postId = collection["postId"];
            if (postId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            // Find out if user has disliked, liked or done nothing and like the status according to that
            var pid = Int32.Parse(postId);
            var likedislikeService = new LikeDislikeService(DbContext);
            var ld = likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                if (ld.Dislike) // Switch from dislike to like
                {
                    DbContext.LikesDislikes.Remove(ld);
                    DbContext.LikesDislikes.Add(new LikeDislike()
                    {
                        PostId = pid,
                        UserId = User.Identity.GetUserId(),
                        Like = true,
                        Dislike = false
                    });
                }
                else if (ld.Like) // UnLike
                {
                    DbContext.LikesDislikes.Remove(ld);
                }
            }
            else // Like
            {
                DbContext.LikesDislikes.Add(new LikeDislike()
                {
                    PostId = pid,
                    UserId = User.Identity.GetUserId(),
                    Like = true,
                    Dislike = false
                });
            }
            DbContext.SaveChanges();

            // Fill in the json
            var postService = new PostService(DbContext);
            var json = new
            {
                likes = postService.GetPostsLikes(pid),
                dislikes = postService.GetPostsDislikes(pid),
                action = ld
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Authorize]
        public ActionResult Dislike(FormCollection collection)
        {
            string postId = collection["postId"];
            if (postId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            // Find out if user has disliked, liked or done nothing and like the status according to that
            var pid = Int32.Parse(postId);
            var likedislikeService = new LikeDislikeService(DbContext);
            var ld = likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                if (ld.Like) // Switch from like to dislike
                {
                    DbContext.LikesDislikes.Remove(ld);
                    DbContext.LikesDislikes.Add(new LikeDislike()
                    {
                        PostId = pid,
                        UserId = User.Identity.GetUserId(),
                        Like = false,
                        Dislike = true
                    });
                }
                else if (ld.Dislike) // UnDislike
                {
                    DbContext.LikesDislikes.Remove(ld);
                }
            }
            else // Dislike
            {
                DbContext.LikesDislikes.Add(new LikeDislike()
                {
                    PostId = pid,
                    UserId = User.Identity.GetUserId(),
                    Like = false,
                    Dislike = true
                });
            }
            DbContext.SaveChanges();

            // Fill in the json
            var postService = new PostService(DbContext);
            var json = new
            {
                likes = postService.GetPostsLikes(pid),
                dislikes = postService.GetPostsDislikes(pid),
                action = ld
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}