using ConnectIn.DAL;
using ConnectIn.Models;
using ConnectIn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectIn.Services
{
    public class UserService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public UserService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region get the user by id
        // Get all information about the user with a given Id
        public User GetUserById(string userId)
        {
            var theUser = (from u in db.Users
                           where userId == u.Id
                           select u).SingleOrDefault();
            return theUser;
        }
        #endregion

        #region queries regarding friends of the user
        // Get the Id of all the users best friends by a given Id of user
        public List<string> GetBestFriendsFromUser(string userId)
        {
            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                       where fc.UserId == userId
                       && fc.BestFriend == true
                       select fc.FriendUserId).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         && fc.BestFriend == true
                         select fc.UserId).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);
            list1.Sort();
        
            return list1;
        }

        // Get the Id of all the users family by a given Id of user
        public List<string> GetFamilyFromUser(string userId)
        {
            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                         where fc.UserId == userId
                         && fc.Family == true
                         select fc.FriendUserId).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         && fc.Family == true
                         select fc.UserId).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);
            list1.Sort();

            return list1;
        }

        // Get the Id of all the users friends by a given Id of user
        public List<string> GetFriendsFromUser(string userId)
        {
            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                         where fc.UserId == userId
                         select fc.FriendUserId).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         select fc.UserId).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);
            list1.Sort();

            return list1;
        }
        #endregion

        #region queries regarding posts
        // Get the Id of all the users posts by a given Id of user
        public List<int> GetAllPostsFromUser(string userId)
        {
            // Create a list of all posts from the user
            var list = (from up in db.Posts
                       where up.UserId == userId
                       orderby up.Date descending
                       select up.PostId).ToList();

            return list;
        }

        // Get the Id of all the users news feeds posts by a given Id of user
        public List<int> GetEveryNewsFeedPostsForUser(string userId)
        {
            // Get the users friends
            var friends = GetFriendsFromUser(userId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where friends.Contains(s.UserId)
                            || s.UserId == userId
                            orderby s.Date descending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }
       
        // Get the Id of all the users best friends posts by a given Id of user
        public List<int> GetBestFriendsPostsForUser(string userId)
        {
            // Get the users bestfriends
            var bestFriends = GetBestFriendsFromUser(userId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where bestFriends.Contains(s.UserId) 
                            || s.UserId == userId
                            orderby s.Date descending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }

        // Get the Id of all the users family posts by a given Id of user
        public List<int> GetFamilyPostsForUser(string userId)
        {
            // Get the users family
            var family = GetFamilyFromUser(userId);

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where family.Contains(s.UserId)
                            || s.UserId == userId
                            orderby s.Date descending
                            select s.PostId).Take(20).ToList();
            return statuses;
        }
        #endregion

        #region queries regarding the users photos
        // Get the Id of all the users photos by a given Id of user
        public List<int> GetAllPhotosFromUser(string userId)
        {
            // Create a list of all photos from the user
            var list = (from up in db.Photos
                        where up.UserId == userId
                        orderby up.Date descending
                        select up.PhotoId).ToList();

            return list;
        }
        #endregion

    }
}