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
        public List<string> GetFriendsFromUser(string userID)
        {
            var db = new ApplicationDbContext();

            // Get the added friends of the user, and put to a list
            var list1 = (from fc in db.Friends
                       where fc.userID == userID
                       select fc.friendUserID).ToList();

            // Get the friends that added the user, and put to a list
            var list2 = (from fc in db.Friends
                         where fc.friendUserID == userID
                         select fc.userID).ToList();

            // Append list1 and list2 together
            list1.AddRange(list2);
            list1.Sort();
        
            return list1;
        }

        public List<Post> GetAllPostsFromUser(string userID)
        {
            var db = new ApplicationDbContext();

            // Create a list of all posts from the user
            var list = (from up in db.Posts
                       where up.userID == userID
                       orderby up.date ascending
                       select up).ToList();

            return list;
        }

        public List<Photo> GetAllPhotosFromUser(string userID)
        {
            var db = new ApplicationDbContext();

            // Create a list of all photos from the user
            var list = (from up in db.Photos
                        where up.userID == userID
                        orderby up.photoID descending
                        select up).ToList();

            return list;
        }

        
    }
}