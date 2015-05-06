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

        #region get the user by id and name
        // Get all information about the user with a given Id
        public User GetUserById(string userId)
        {
            var theUser = (from u in db.Users
                           where userId == u.Id
                           select u).SingleOrDefault();
            return theUser;
        }

        // Get all users that contain that name
        public List<string> GetPossibleUsersByName(string userName)
        {
            var theUser = (from u in db.Users
                           where u.Name.ToLower().Contains(userName)
                           select u.Id).ToList();
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

        // Get the Id of all the users birthdays by a given Id of the user
        public List<string> GetAllFriendsBirthdays(string userId)
        {
            var friends = GetFriendsFromUser(userId);
            var birthdays = (from b in db.Users
                             where (friends.Contains(b.Id) || b.Id == userId)
                             && b.Birthday.Day == DateTime.Today.Day
                             select b.Id).ToList();
            return birthdays;
        }

        // Get one row from the Friends table where userId and friendId are in the row
        public Friend GetFriendShip(string userId, string friendId)
        {
            var fs = (from f in db.Friends
                where f.UserId == userId
                      && f.FriendUserId == friendId
                select f).SingleOrDefault();

            return fs;
        }
        #endregion

        #region queries regarding posts
        // Get the Id of all the users posts by a given Id of user
        public List<int> GetAllPostsFromUser(string userId)
        {
            // Create a list of all posts from the user
            var list = (from up in db.Posts
                       where up.UserId == userId
                       && up.GroupId == null
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
                            where (friends.Contains(s.UserId)
                            || s.UserId == userId)
                            && s.GroupId == null
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
                            where (bestFriends.Contains(s.UserId) 
                            || s.UserId == userId)
                            && s.GroupId == null
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
                            where (family.Contains(s.UserId)
                            || s.UserId == userId)
                            && s.GroupId == null
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

        #region queries regarding the user's groups

        public List<int> GetAllGroupsOfUser(string userId)
        {
            var gr = (from g in db.Members
                where g.UserId == userId
                select g.GroupId).ToList();
            return gr;
        }
        #endregion

        #region queries regarding users notification
        //Get a notification by notification Id
        public Notification GetNotificationById(int notificationId)
        {
            var notification = (from n in db.Notifications
                where n.NotificationId == notificationId
                select n).SingleOrDefault();

            return notification;
        }

        //Get a list of all notifications for the user with userId 
        public List<Notification> GetAllNotificationsForUser(string userId)
        {
            var notifications = (from n in db.Notifications
                where n.UserId == userId
                select n).ToList();

            return notifications;
        } 
        #endregion
    }
}