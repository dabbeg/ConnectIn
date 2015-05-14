using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ConnectIn.DAL;
using ConnectIn.Models.Entity;

namespace ConnectIn.Services
{
    public class NotificationService
    {
        #region setting up IAppDataContext db
        // readonly is something that cannot change, except in constructor
        // _db is a member variable
        private readonly IAppDataContext db;
        public NotificationService(IAppDataContext context)
        {
            // if context is null, then use new ApplicationDbContext();
            db = context ?? new ApplicationDbContext();
        }
        #endregion

        #region notificationqueries
        //Get a notification by notification Id
        public Notification GetNotificationById(int notificationId)
        {
            var notification = (from n in db.Notifications
                                where n.NotificationId == notificationId
                                select n).SingleOrDefault();

            return notification;
        }

        // return if the notification is pending
        public Notification GetIfFriendRequestIsPending(string userId, string friendId)
        {
            var notification = (from n in db.Notifications
                                where ((n.UserId == userId
                                      && n.FriendUserId == friendId)
                                      || (n.UserId == friendId
                                      && n.FriendUserId == userId))
                                      && n.GroupId == -1
                                select n).SingleOrDefault();

            return notification;
        }
        #endregion
    }
}