using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.Models.Entity;
using ConnectIn.Models;

namespace ConnectIn.Services
{
    public class PostService
    {
        public List<Post> GetLatestForUser(string userId)
        {
            var db = new ApplicationDbContext();

            // Get the users friends
            var us = new UserService();
            var friends = us.GetFriendsFromUser(userId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where friends.Contains(s.UserId)
                            orderby s.Date ascending
                            select s).Take(20).ToList();
            return statuses;
        }


        // comments likes dislikes
        // group service
    }
}