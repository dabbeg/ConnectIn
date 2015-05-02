using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;
using ConnectIn.Tests;
using ConnectIn.Models.Entity;

namespace ConnectIn.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        private UserService service;

        // Initialize is run before every unit test
        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();
            var f1 = new Friend()
            {
                UserId = "1",
                FriendUserId = "2",
                BestFriend = false,
                Family = false
            };
            mockDb.Friends.Add(f1);

            var f2 = new Friend()
            {
                UserId = "2",
                FriendUserId = "3",
                BestFriend = false,
                Family = false
            };
            mockDb.Friends.Add(f2);

            var f3 = new Friend()
            {
                UserId = "3",
                FriendUserId = "1",
                BestFriend = false,
                Family = false
            };
            mockDb.Friends.Add(f3);
            service = new UserService(mockDb);
        }
        [TestMethod]
        public void TestGetFriendsFromUser()
        {
            // Arrange
            const string userId = "1";
            
            // Act
            var result = service.GetFriendsFromUser(userId);

            foreach (var item in result)
            {
                Assert.AreNotEqual(item, userId);
            }

            // Assert
            Assert.AreEqual(2, result.Count);
        }
    }
}
