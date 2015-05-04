using System;
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
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public PostService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        public Post GetPostById(string userId)
        {
            var post = (from p in db.Posts
                        where userId == p.PostId
                        select p).SingleOrDefault();
            return post;
        }

        public List<string> GetEveryLatestPostsForUser(string userId)
        {
            // Get the users friends
            var us = new UserService(db);
            var friends = us.GetFriendsFromUser(userId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where friends.Contains(s.UserId)
                            orderby s.Date ascending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }
        public List<string> GetFriendsLatestPostsForUser(string userId)
        {
            // Get the users friends
            var us = new UserService(db);
            var friends = us.GetFriendsFromUser(userId);

            /*foreach(var u in friends)
            {
                if (u.best)
            }*/

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where friends.Contains(s.UserId) /*&&
                            us.GetUserById(friends)*/
                            orderby s.Date ascending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }
        
        // comments likes dislikes
        // group service
    }
}