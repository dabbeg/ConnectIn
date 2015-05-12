﻿using ConnectIn.DAL;
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

        // Get all users that contain that search word
        public List<string> GetPossibleUsersByName(string searchWord)
        {
            var theUser = (from u in db.Users
                           where u.Name.ToLower().Contains(searchWord.ToLower())
                           orderby u.Name ascending
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
                       && fc.UserConsidersFriendAsBestFriend
                       select fc).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         && fc.UserConsidersFriendAsBestFriend
                         select fc).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);

            // Sort by name
            var list3 = (from fc in list1
                         orderby fc.UserId == userId ? GetUserById(fc.FriendUserId).Name : GetUserById(fc.UserId).Name
                         select fc.UserId == userId ? fc.FriendUserId : fc.UserId).ToList();


            return list3;
        }

        // Get the Id of all the users family by a given Id of user
        public List<string> GetFamilyFromUser(string userId)
        {
            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                         where fc.UserId == userId
                         && fc.UserConsidersFriendAsFamily == true
                         select fc).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         && fc.UserConsidersFriendAsFamily == true
                         select fc).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);

            // Sort by name
            var list3 = (from fc in list1
                         orderby fc.UserId == userId ? GetUserById(fc.FriendUserId).Name : GetUserById(fc.UserId).Name
                         select fc.UserId == userId ? fc.FriendUserId : fc.UserId).ToList();


            return list3;
        }

        // Get the Id of all the users friends by a given Id of user
        public List<string> GetFriendsFromUser(string userId)
        {
            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                         where fc.UserId == userId
                         select fc).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.FriendUserId == userId
                         select fc).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);

            // Sort by name
            var list3 = (from fc in list1
                         orderby fc.UserId == userId ? GetUserById(fc.FriendUserId).Name : GetUserById(fc.UserId).Name
                         select fc.UserId == userId ? fc.FriendUserId : fc.UserId).ToList();


            return list3;
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
                where (f.UserId == userId
                      && f.FriendUserId == friendId)
                      || (f.UserId == friendId
                      && f.FriendUserId == userId)
                select f).SingleOrDefault();

            return fs;
        }

        public bool UserConsidersFriendClose(string userId, string friendId)
        {
            var friendship = GetFriendShip(userId, friendId);
            if (friendship == null) return false;
            if (userId == friendship.FriendUserId)
            {
                if (friendship.UserConsidersFriendAsBestFriend || friendship.UserConsidersFriendAsFamily)
                {
                    return true;
                }
            }
            else
            {
                if (friendship.FriendConsidersUsersAsBestFriend || friendship.FriendConsidersUsersAsFamily)
                {
                    return true;
                }
            }

            return false;
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

        // Get the id of all posts of all the users news feeds posts by a given Id of user
        public List<int> GetEveryNewsFeedPostsForUser(string userId)
        {
            // Get the users friends
            var fri = GetFriendsFromUser(userId);
            var friends = new List<String>();

            foreach (var item in fri)
            {
                if (UserConsidersFriendClose(userId, item))
                {
                    friends.Add(item);
                }
            }

            // Get all the posts from friends
            var statuses = (from s in db.Posts
                            where s.User.Privacy == 2
                            ? (friends.Contains(s.UserId) || s.UserId == userId) 
                            : (fri.Contains(s.UserId) || s.UserId == userId)
                            /*(friends.Contains(s.UserId) || s.UserId == userId)*/
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

        // Get the Id of all posts of the users family posts by a given Id of user
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
        // Get photo by Id
        public Photo GetPhotoById(int photoId)
        {
            var photo = (from p in db.Photos
                where p.PhotoId == photoId
                select p).SingleOrDefault();
            
            return photo;
        }

        // Get profile picture from user
        public Photo GetProfilePicture(string userId)
        {
            var pPhoto = (from p in db.Photos
                where p.UserId == userId
                      && p.IsProfilePicture == true
                select p).SingleOrDefault();

            if (pPhoto == null)
            {
                var extraPhoto = new Photo()
                {
                    PhotoPath = "/Content/Images/largeProfilePic.jpg"
                };
                return extraPhoto;
            }
            return pPhoto;
        }
        //Get Cover photo fropm user
        public Photo GetCoverPhoto(string userId)
        {
            var cPhoto = (from c in db.Photos
                          where c.UserId == userId
                          && c.IsCoverPhoto == true
                          select c).SingleOrDefault();
            return cPhoto;
        }

        // Get all the users photos by a given Id of user
        public List<Photo> GetAllPhotosFromUser(string userId)
        {
            // Create a list of all photos from the user
            var list = (from up in db.Photos
                        where up.UserId == userId
                        && up.IsProfilePicture == false
                        && up.IsCoverPhoto == false
                        orderby up.Date descending
                        select up).ToList();

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
                where n.FriendUserId == userId
                select n).ToList();

            return notifications;
        }

        public Notification GetIfFriendRequestIsPending(string userId, string friendId)
        {
            var notification = (from n in db.Notifications
                where (n.UserId == userId
                      && n.FriendUserId == friendId)
                      || (n.UserId == friendId
                      && n.FriendUserId == userId)
                select n).SingleOrDefault();

            return notification;
        }
        #endregion
    }
}