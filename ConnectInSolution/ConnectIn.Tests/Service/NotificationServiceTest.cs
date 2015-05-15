using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;
using ConnectIn.Models.Entity;

namespace ConnectIn.Tests.Service
{
    [TestClass]
    public class NotificationServiceTest
    {
        private NotificationService service;

        // Initialize is run before every unit test
        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();

            #region Notifications
            var n1 = new Notification()
            {
                NotificationId = 1,
                FriendUserId = "2",
                UserId = "1",
                Date = new DateTime(2000, 1, 1)
            };
            mockDb.Notifications.Add(n1);

            var n2 = new Notification()
            {
                NotificationId = 2,
                FriendUserId = "2",
                UserId = "4",
                Date = new DateTime(2001, 1, 1)
            };
            mockDb.Notifications.Add(n2);

            var n3 = new Notification()
            {
                NotificationId = 3,
                FriendUserId = "3",
                UserId = "1",
                Date = new DateTime(2002, 1, 1)
            };
            mockDb.Notifications.Add(n3);
            #endregion
            
            service = new NotificationService(mockDb);
        }
        
        #region notificationqueries
        [TestMethod]
        public void TestGetNotificationById()
        {
            // Arrange
            const int not1 = 1;

            // Act
            var result1 = service.GetNotificationById(not1);

            // Assert
            Assert.AreEqual("2", result1.FriendUserId);
            Assert.AreEqual("1", result1.UserId);
        }

        [TestMethod]
        public void TestGetIfFriendRequestIsPending()
        {
            // Arrange
            const string user1 = "1";
            const string user3 = "4";

            // Act
            var result2 = service.GetIfFriendRequestIsPending(user1, user3);

            // Assert
            Assert.IsNull(result2);
        }
        #endregion
    }
}
