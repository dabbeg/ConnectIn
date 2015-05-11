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
        public ActionResult AddPost(FormCollection collection)
        {
            string amount = collection["amount"];
            string status = collection["status"];
            string location = collection["location"];
            string groupId = collection["idOfGroup"];
            if(status.IsNullOrWhiteSpace() || location.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            DateTime now = DateTime.Now;
            var newPost = new Post
            {
                UserId = User.Identity.GetUserId(),
                Date = now,
                Text = status
            };

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var userService = new UserService(context);
            var likeDislikeService = new LikeDislikeService(context);

            if (location.Equals("newsfeed"))
            {
                context.Posts.Add(newPost);
                context.SaveChanges();

                var postList = new List<PostsViewModel>();
                var list = userService.GetEveryNewsFeedPostsForUser(User.Identity.GetUserId());
                int nOfCurrentStatuses = Int32.Parse(amount);
                for (int i = (list.Count - nOfCurrentStatuses) - 1; i >= 0 ; i--)
                {
                    var post = postService.GetPostById(list[i]);

                    var profilePicture = userService.GetProfilePicture(post.UserId);
                    string profilePicturePath;
                    if (profilePicture == null)
                    {
                        profilePicturePath = "/Content/images/largeProfilePic.jpg";
                    }
                    else
                    {
                        profilePicturePath = profilePicture.PhotoPath;
                    }

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
                   
                    postList.Add(
                        new PostsViewModel()
                        {
                            PostId = post.PostId,
                            Body = post.Text,
                            User = new UserViewModel()
                            {
                                UserId = post.UserId,
                                Name = userService.GetUserById(post.UserId).Name,
                                ProfilePicture = profilePicturePath
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
            if (location.Equals("group"))
            {
                var grpId = Int32.Parse(groupId);
                newPost.GroupId = grpId;

                context.Posts.Add(newPost);
                context.SaveChanges();
                return RedirectToAction("Details", "Group", new {Id = grpId});
                
            }
            return View("Error");

        }

        [HttpPost]
        public ActionResult RemovePost(FormCollection collection)
        {
            string id = collection["postId"];
            if (id.IsNullOrWhiteSpace())
            {
                return View("Error");
            }
            int postId = Int32.Parse(id);

            var context = new ApplicationDbContext();
            var postService = new PostService(context);
            var post = postService.GetPostById(postId);
            if (post != null)
            {
                context.Posts.Remove(post);
                context.SaveChanges();
            }
            
            var url = ControllerContext.HttpContext.Request.UrlReferrer;
            if (url != null && url.AbsolutePath.Contains("Comment"))
            {
                return RedirectToAction("NewsFeed", "Home");
            }

            // Returns nothing if it is an ajax call
            return new EmptyResult();
        }

        public ActionResult Comment(int? PostId)
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

        [HttpPost]
        public ActionResult Like(FormCollection collection)
        {
            string postId = collection["postId"];
            if (postId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var likedislikeService = new LikeDislikeService(context);
            var pid = Int32.Parse(postId);

            var ld = likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                if (ld.Dislike) // Switch from dislike to like
                {
                    context.LikesDislikes.Remove(ld);
                    context.LikesDislikes.Add(new LikeDislike()
                    {
                        PostId = pid,
                        UserId = User.Identity.GetUserId(),
                        Like = true,
                        Dislike = false
                    });
                }
                else if (ld.Like) // UnLike
                {
                    context.LikesDislikes.Remove(ld);
                }
            }
            else // Like
            {
                context.LikesDislikes.Add(new LikeDislike()
                {
                    PostId = pid,
                    UserId = User.Identity.GetUserId(),
                    Like = true,
                    Dislike = false
                });
            }
            context.SaveChanges();

            var postService = new PostService(context);
            var json = new
            {
                likes = postService.GetPostsLikes(pid),
                dislikes = postService.GetPostsDislikes(pid),
                action = ld
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Dislike(FormCollection collection)
        {
            string postId = collection["postId"];
            if (postId.IsNullOrWhiteSpace())
            {
                return View("Error");
            }

            var context = new ApplicationDbContext();
            var likedislikeService = new LikeDislikeService(context);
            var pid = Int32.Parse(postId);

            var ld = likedislikeService.GetLikeDislike(User.Identity.GetUserId(), pid);
            if (ld != null)
            {
                if (ld.Like) // Switch from like to dislike
                {
                    context.LikesDislikes.Remove(ld);
                    context.LikesDislikes.Add(new LikeDislike()
                    {
                        PostId = pid,
                        UserId = User.Identity.GetUserId(),
                        Like = false,
                        Dislike = true
                    });
                }
                else if (ld.Dislike) // UnDislike
                {
                    context.LikesDislikes.Remove(ld);
                }
            }
            else // Dislike
            {
                context.LikesDislikes.Add(new LikeDislike()
                {
                    PostId = pid,
                    UserId = User.Identity.GetUserId(),
                    Like = false,
                    Dislike = true
                });
            }
            context.SaveChanges();

            var postService = new PostService(context);
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