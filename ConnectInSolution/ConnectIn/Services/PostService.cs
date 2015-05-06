﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;
using ConnectIn.Models;
using ConnectIn.DAL;

namespace ConnectIn.Services
{
    public class PostService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public PostService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region get post by id
        // Get post by a given user Id
        public Post GetPostById(int postId)
        {
            var post = (from p in db.Posts
                        where postId == p.PostId
                        select p).SingleOrDefault();
            return post;
        }
        #endregion

        #region comments, likes and dislikes
        // Get the Id of all comments of the given Id of a post
        public List<int> GetPostsComments(int postId)
        {
            var pc = (from n in db.Comments
                      where n.PostId == postId
                      orderby n.Date descending
                      select n.CommentId).ToList();
            return pc;
        }

        // Get count of all likes of the given Id of a post
        public int GetPostsLikes(int postId)
        {
            var pl = (from n in db.LikesDislikes
                      where n.PostId == postId
                      select n).ToList();
            int count = 0;
            foreach(var item in pl)
            {
                if (item.Like == true)
                {
                    count++;
                }
            }
            return count;
        }

        // Get count of all dislikes of the given Id of a post
        public int GetPostsDislikes(int postId)
        {
            var pl = (from n in db.LikesDislikes
                      where n.PostId == postId
                      select n).ToList();
            int count = 0;
            foreach (var item in pl)
            {
                if (item.Dislike == true)
                {
                    count++;
                }
            }
            return (int) count;
        }

        // Return the LikeDislike from userid and postid
        public LikeDislike GetLikeDislike(string userId, int postId)
        {
            var ld = (from n in db.LikesDislikes
                where n.UserId == userId
                      && n.PostId == postId
                select n).SingleOrDefault();

            return ld;
        }
        #endregion
    }
}