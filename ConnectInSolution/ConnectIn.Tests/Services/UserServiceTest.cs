using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;

namespace ConnectIn.Tests.Services
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public void TestGetFriendsFromUser()
        {
            // Arrange
            const string user = "dabs";
            var service = new UserService();
            
            // Act
            var result = service.GetFriendsFromUser(user);

            // Assert
            Assert.AreEqual(2, result.Count);
        }
    }
}
