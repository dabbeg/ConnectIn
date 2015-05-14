using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;
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

            #region Users
            var u1 = new User()
            {
                Id = "1",
                Email = "user1@m.com",
                UserName = "user1",
                Name = "Darri",
                Birthday = new DateTime(2000, 1, 1),
                Work = "HR",
                School = "HR",
                Gender = "Male",
                Address = "add1",
                Privacy = 1
            };
            mockDb.Users.Add(u1);

            var u2 = new User()
            {
                Id = "2",
                Email = "user2@m.com",
                UserName = "user2",
                Name = "Jan",
                Birthday = DateTime.Today,
                Work = "HI",
                School = "HI",
                Gender = "Male",
                Address = "add2",
                Privacy = 1
            };
            mockDb.Users.Add(u2);

            var u3 = new User()
            {
                Id = "3",
                Email = "user3@m.com",
                UserName = "user3",
                Name = "Logi",
                Birthday = new DateTime(2002, 1, 1),
                Work = "HA",
                School = "HA",
                Gender = "Female",
                Address = "add3",
                Privacy = 1
            };
            mockDb.Users.Add(u3);

            var u4 = new User()
            {
                Id = "4",
                Email = "user4@m.com",
                UserName = "user4",
                Name = "Davíð",
                Birthday = new DateTime(2003, 1, 1),
                Work = "FG",
                School = "FG",
                Gender = "Female",
                Address = "add4",
                Privacy = 1
            };
            mockDb.Users.Add(u4);

            var u5 = new User()
            {
                Id = "5",
                Email = "user5@m.com",
                UserName = "user5",
                Name = "Ingþór",
                Birthday = DateTime.Today,
                Work = "FG",
                School = "FG",
                Gender = "Female",
                Address = "add5",
                Privacy = 1
            };

            mockDb.Users.Add(u5);
            #endregion

            #region Posts
            var p1 = new Post()
            {
                PostId = 1,
                Text = "someText 1",
                UserId = "1",
                Date = new DateTime(2000, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p1);

            var p2 = new Post()
            {
                PostId = 2,
                Text = "someText 2",
                UserId = "2",
                Date = new DateTime(2001, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p2);

            var p3 = new Post()
            {
                PostId = 3,
                Text = "someText 3",
                UserId = "4",
                Date = new DateTime(2002, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p3);

            var p4 = new Post()
            {
                PostId = 4,
                Text = "someText 4",
                UserId = "1",
                Date = new DateTime(2003, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p4);

            var p5 = new Post()
            {
                PostId = 5,
                Text = "someText 5",
                UserId = "3",
                Date = new DateTime(2004, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p5);

            var p6 = new Post()
            {
                PostId = 6,
                Text = "someText 6",
                UserId = "2",
                Date = new DateTime(2005, 1, 1),
                GroupId = null
            };
            mockDb.Posts.Add(p6);
            #endregion

            #region Photos
            var ph1 = new Photo()
            {
                PhotoId = 1,
                PhotoPath = "bla1",
                UserId = "1",
                Date = new DateTime(2000, 1, 1),
                IsProfilePhoto = true,
                IsCurrentProfilePhoto = true,
            };
            mockDb.Photos.Add(ph1);

            var ph2 = new Photo()
            {
                PhotoId = 2,
                PhotoPath = "bla2",
                UserId = "1",
                Date = new DateTime(2001, 1, 1),
                IsProfilePhoto = true,
                IsCurrentProfilePhoto = false
            };
            mockDb.Photos.Add(ph2);

            var ph3 = new Photo()
            {
                PhotoId = 3,
                PhotoPath = "bla3",
                UserId = "2",
                Date = new DateTime(2002, 1, 1),
                IsProfilePhoto = false,
                IsCoverPhoto = true,
                IsCurrentCoverPhoto = true
            };
            mockDb.Photos.Add(ph3);

            var ph4 = new Photo()
            {
                PhotoId = 4,
                PhotoPath = "bla4",
                UserId = "4",
                Date = new DateTime(2003, 1, 1),
                IsProfilePhoto = false,
                IsCoverPhoto = true
            };
            mockDb.Photos.Add(ph4);
            #endregion

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

            #region Members
            var m1 = new Member()
            {
                UserId = "1",
                GroupId = 1
            };
            mockDb.Members.Add(m1);

            var m2 = new Member()
            {
                UserId = "1",
                GroupId = 2
            };
            mockDb.Members.Add(m2);

            var m3 = new Member()
            {
                UserId = "1",
                GroupId = 3
            };
            mockDb.Members.Add(m3);

            var m4 = new Member()
            {
                UserId = "2",
                GroupId = 1
            };
            mockDb.Members.Add(m4);

            var m5 = new Member()
            {
                UserId = "2",
                GroupId = 3
            };
            mockDb.Members.Add(m5);

            var m6 = new Member()
            {
                UserId = "3",
                GroupId = 2
            };
            mockDb.Members.Add(m6);

            var m7 = new Member()
            {
                UserId = "4",
                GroupId = 3
            };
            mockDb.Members.Add(m7);

            #endregion

            #region LikesDislikes
            var ld1 = new LikeDislike()
            {
                PostId = 1,
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld1);

            var ld2 = new LikeDislike()
            {
                PostId = 1,
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld2);

            var ld3 = new LikeDislike()
            {
                PostId = 2,
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld3);

            var ld4 = new LikeDislike()
            {
                PostId = 3,
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld4);

            var ld5 = new LikeDislike()
            {
                PostId = 3,
                UserId = "3",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld5);

            var ld6 = new LikeDislike()
            {
                PostId = 4,
                UserId = "4",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld6);

            var ld7 = new LikeDislike()
            {
                PostId = 4,
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld7);

            var ld8 = new LikeDislike()
            {
                PostId = 4,
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld8);
            #endregion

            #region Groups
            var g1 = new Group()
            {
                GroupId = 1,
                Name = "Group1"
            };
            mockDb.Groups.Add(g1);

            var g2 = new Group()
            {
                GroupId = 2,
                Name = "Group2"
            };
            mockDb.Groups.Add(g2);

            var g3 = new Group()
            {
                GroupId = 3,
                Name = "Group3"
            };
            mockDb.Groups.Add(g3);
            #endregion

            #region Friends
            var f1 = new Friend()
            {
                UserId = "1",
                FriendUserId = "2",
                UserConsidersFriendAsBestFriend = true,
                UserConsidersFriendAsFamily = true,
                FriendConsidersUsersAsBestFriend = true,
                FriendConsidersUsersAsFamily = true
            };
            mockDb.Friends.Add(f1);

            var f2 = new Friend()
            {
                UserId = "2",
                FriendUserId = "3",
                UserConsidersFriendAsBestFriend = false,
                UserConsidersFriendAsFamily = true
            };
            mockDb.Friends.Add(f2);

            var f3 = new Friend()
            {
                UserId = "3",
                FriendUserId = "1",
                UserConsidersFriendAsBestFriend = false,
                UserConsidersFriendAsFamily = false
            };
            mockDb.Friends.Add(f3);

            var f4 = new Friend()
            {
                UserId = "3",
                FriendUserId = "4",
                UserConsidersFriendAsBestFriend = true,
                UserConsidersFriendAsFamily = false
            };
            mockDb.Friends.Add(f4);

            var f5 = new Friend()
            {
                UserId = "1",
                FriendUserId = "4",
                UserConsidersFriendAsBestFriend = false,
                UserConsidersFriendAsFamily = false
            };
            mockDb.Friends.Add(f5);

            var f6 = new Friend()
            {
                UserId = "5",
                FriendUserId = "2",
                UserConsidersFriendAsBestFriend = false,
                UserConsidersFriendAsFamily = false,
                FriendConsidersUsersAsBestFriend = false,
                FriendConsidersUsersAsFamily = false
            };
            mockDb.Friends.Add(f6);
            #endregion

            #region Comments
            var c1 = new Comment()
            {
                CommentId = 1,
                UserId = "1",
                PostId = 1,
                Text = "bla 1",
                Date = new DateTime(2000, 11, 11)
            };
            mockDb.Comments.Add(c1);

            var c2 = new Comment()
            {
                CommentId = 2,
                UserId = "1",
                PostId = 1,
                Text = "bla 2",
                Date = new DateTime(2001, 11, 11)
            };
            mockDb.Comments.Add(c2);

            var c3 = new Comment()
            {
                CommentId = 3,
                UserId = "2",
                PostId = 2,
                Text = "bla 3",
                Date = new DateTime(2002, 11, 11)
            };
            mockDb.Comments.Add(c3);

            var c4 = new Comment()
            {
                CommentId = 4,
                UserId = "1",
                PostId = 3,
                Text = "bla 4",
                Date = new DateTime(2003, 11, 11)
            };
            mockDb.Comments.Add(c4);

            var c5 = new Comment()
            {
                CommentId = 5,
                UserId = "2",
                PostId = 4,
                Text = "bla 5",
                Date = new DateTime(2004, 11, 11)
            };
            mockDb.Comments.Add(c5);

            var c6 = new Comment()
            {
                CommentId = 6,
                UserId = "3",
                PostId = 6,
                Text = "bla 6",
                Date = new DateTime(2005, 11, 11)
            };
            mockDb.Comments.Add(c6);

            var c7 = new Comment()
            {
                CommentId = 7,
                UserId = "4",
                PostId = 6,
                Text = "bla 7",
                Date = new DateTime(2006, 11, 11)
            };
            mockDb.Comments.Add(c7);

            var c8 = new Comment()
            {
                CommentId = 8,
                UserId = "4",
                PostId = 4,
                Text = "bla 8",
                Date = new DateTime(2007, 11, 11)
            };
            mockDb.Comments.Add(c8);
            #endregion

            service = new UserService(mockDb);
        }

        #region get the user by id and name
        [TestMethod]
        public void TestGetUserById()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetUserById(user1);
            var result2 = service.GetUserById(user2);

            // Assert
            User u1 = new User();
            u1.Id = "1";
            u1.Email = "user1@m.com";
            u1.UserName = "user1";
            u1.Birthday = new DateTime(2000, 1, 1);
            u1.Work = "HR";
            u1.School = "HR";
            u1.Gender = "Male";
            u1.Address = "add1";
            u1.Privacy = 1;

            Assert.AreEqual(u1.Email, result1.Email);
            Assert.AreEqual(u1.UserName, result1.UserName);
            Assert.AreEqual(u1.Address, result1.Address);

            User u2 = new User();
            u2.Id = "2";
            u2.Email = "user2@m.com";
            u2.UserName = "user2";
            u2.Birthday = new DateTime(2001, 1, 1);
            u2.Work = "HI";
            u2.School = "HI";
            u2.Gender = "Male";
            u2.Address = "add2";
            u2.Privacy = 1;

            Assert.AreEqual(u2.Email, result2.Email);
            Assert.AreEqual(u2.UserName, result2.UserName);
            Assert.AreEqual(u2.Address, result2.Address);
        }

        [TestMethod]
        public void TestGetPossibleUsersByName()
        {
            // Arrange
            const string search1 = "a";
            const string search2 = "r";

            // Act
            var result1 = service.GetPossibleUsersByName(search1);
            var result2 = service.GetPossibleUsersByName(search2);

            // Assert
            string[] list1 = { "1", "4", "2" };
            string[] list2 = { "1", "5" };

            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
            Assert.AreEqual(3, result1.Count);
            Assert.AreEqual(2, result2.Count);
        }
        #endregion

        #region queries regarding friends of the user
        [TestMethod]
        public void TestGetFriendsFromUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";
            
            // Act
            var result1 = service.GetFriendsFromUser(user1);
            var result2 = service.GetFriendsFromUser(user2);

            // Assert
            string[] list1 = { "4", "2", "3" };
            string[] list2 = { "1", "5" , "3" };
            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);

            foreach (var item in result1)
            {
                Assert.AreNotEqual(item, user1);
            }

            Assert.AreEqual(3, result1.Count);
            Assert.AreEqual(3, result2.Count);
        }

        [TestMethod]
        public void TestGetBestFriendsFromUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetBestFriendsFromUser(user1);
            var result2 = service.GetBestFriendsFromUser(user2);

            // Assert
            string[] list1 = { "2" };
            string[] list2 = { "1" };
            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
        }

        [TestMethod]
        public void TestGetFamilyFromUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetFamilyFromUser(user1);
            var result2 = service.GetFamilyFromUser(user2);

            // Assert
            string[] list1 = { "2" };
            string[] list2 = { "1" , "3" };
            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
        }

        [TestMethod]
        public void TestGetAllFriendsBirthdays()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetAllFriendsBirthdays(user1);
            var result2 = service.GetAllFriendsBirthdays(user2);

            // Assert
            string[] list1 = { "2" };
            string[] list2 = { "2", "5" };

            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(2, result2.Count);
        }

        [TestMethod]
        public void TestGetFriendShip()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";
            const string user5 = "5";

            // Act
            var result1 = service.GetFriendShip(user1, user2);
            var result2 = service.GetFriendShip(user1, user5);
            var result3 = service.GetFriendShip(user5, user2);

            // Assert
            Assert.AreEqual("1", result1.UserId);
            Assert.AreEqual("2", result1.FriendUserId);
            Assert.AreEqual(true, result1.UserConsidersFriendAsBestFriend);
            Assert.AreEqual(true, result1.UserConsidersFriendAsFamily);
            Assert.AreEqual(null, result2);
            Assert.AreEqual("5", result3.UserId);
            Assert.AreEqual("2", result3.FriendUserId);
        }

        [TestMethod]
        public void TestUserConsidersFriendClose()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";
            const string user5 = "5";

            // Act
            var result1 = service.UserConsidersFriendClose(user1, user2);
            var result2 = service.UserConsidersFriendClose(user1, user5);
            var result3 = service.UserConsidersFriendClose(user5, user2);

            // Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(false, result2);
            Assert.AreEqual(false, result3);
        }

        [TestMethod]
        public void TestFriendConsidersUserClose()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";
            const string user5 = "5";

            // Act
            var result1 = service.UserConsidersFriendClose(user1, user2);
            var result2 = service.UserConsidersFriendClose(user1, user5);
            var result3 = service.UserConsidersFriendClose(user5, user2);

            // Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(false, result2);
            Assert.AreEqual(false, result3);
        }
        #endregion

        #region queries regarding posts
        [TestMethod]
        public void TestGetAllPostsFromUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";
            const string user3 = "3";

            // Act
            var result1 = service.GetAllPostsFromUser(user1);
            var result2 = service.GetAllPostsFromUser(user2);
            var result3 = service.GetAllPostsFromUser(user3);

            // Assert
            int[] list1 = { 4, 1 };
            int[] list2 = { 6, 2 };
            int[] list3 = { 5 };

            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
            CollectionAssert.AreEqual(list3, result3);
        }

        [TestMethod]
        public void TestGetEveryNewsfeedPostsForUser()
        {
            // This method can't be tested because of if-else statement in a LINQ query
        }

        [TestMethod]
        public void TestGetBestFriendsPostsForUser()
        {
            // Arrange
            const string user1 = "1";

            // Act
            var result1 = service.GetBestFriendsPostsForUser(user1);

            // Assert
            int[] list1 = { 6, 4, 2, 1 };

            CollectionAssert.AreEqual(list1, result1);
            Assert.AreEqual(4, result1.Count);
        }

        [TestMethod]
        public void TestGetFamilyPostsForUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetFamilyPostsForUser(user1);
            var result2 = service.GetFamilyPostsForUser(user2);

            // Assert
            int[] list1 = { 6, 4, 2, 1 };
            int[] list2 = { 6, 5, 4, 2, 1 };

            Assert.AreEqual(4, result1.Count);
            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
        }
        #endregion

        #region queries regarding the users photos

        [TestMethod]
        public void TestGetProfilePicture()
        {
            // Arrange
            const string user1 = "1";

            // Act
            var result1 = service.GetProfilePicture(user1);

            // Assert
            Assert.AreEqual(1, result1.PhotoId);
        }

        [TestMethod]
        public void TestGetCoverPhoto()
        {
            // Arrange
            const string user1 = "2";
            const string user2 = "4";

            // Act
            var result1 = service.GetCoverPhoto(user1);

            // Assert
            Assert.AreEqual(3, result1);
        }

        [TestMethod]
        public void TestGetAllProfilePhotosFromUser()
        {
            // Arrange
            const string user1 = "1";

            // Act
            var result1 = service.GetAllProfilePhotosFromUser(user1);

            // Assert
            int i = 0;
            int [] l1 = {2, 1};
            foreach (var item in result1)
            {
                Assert.AreEqual(l1[i], item.PhotoId);
                i++;
            }
            Assert.AreEqual(2, result1.Count);
        }

        [TestMethod]
        public void TestGetAllCoverPhotosFromUser()
        {
            // Arrange
            const string user1 = "2";

            // Act
            var result1 = service.GetAllCoverPhotosFromUser(user1);

            // Assert
            int i = 0;
            int[] l1 = { 3 };
            foreach (var item in result1)
            {
                Assert.AreEqual(l1[i], item.PhotoId);
                i++;
            }
        }

        #endregion

        #region queries regarding the user's groups
        [TestMethod]
        public void TestGetAllGroupsOfUser()
        {
            const string user1 = "1";

            var result = service.GetAllGroupsOfUser(user1);

            int[] list = { 1, 2, 3 };

            CollectionAssert.AreEqual(list, result);
        }
        #endregion

        #region queries regarding users notification
        [TestMethod]
        public void TestGetAllNotificationsForUser()
        {
            // Arrange
            const string user1 = "1";

            // Act
            var result1 = service.GetAllNotificationsForUser(user1);

            // Assert
            int i = 0;
            int[] l1 = { 2, 1 };
            foreach (var item in result1)
            {
                Assert.AreEqual(l1[i], item.NotificationId);
                i++;
            }
        }
        #endregion


    }
}
