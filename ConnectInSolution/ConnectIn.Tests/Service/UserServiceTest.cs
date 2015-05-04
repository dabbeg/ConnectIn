﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;
using ConnectIn.Tests;
using ConnectIn.Models.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

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
                birthday = new DateTime(2000,1,1),
                work = "HR",
                school = "HR",
                gender = "Male",
                address = "add1",
                privacy = false
            };
            mockDb.Users.Add(u1);

            var u2 = new User()
            {
                Id = "2",
                Email = "user2@m.com",
                UserName = "user2",
                birthday = new DateTime(2001, 1, 1),
                work = "HI",
                school = "HI",
                gender = "Male",
                address = "add2",
                privacy = false
            };
            mockDb.Users.Add(u2);

            var u3 = new User()
            {
                Id = "3",
                Email = "user3@m.com",
                UserName = "user3",
                birthday = new DateTime(2002, 1, 1),
                work = "HA",
                school = "HA",
                gender = "Female",
                address = "add3",
                privacy = false
            };
            mockDb.Users.Add(u3);

            var u4 = new User()
            {
                Id = "4",
                Email = "user4@m.com",
                UserName = "user4",
                birthday = new DateTime(2003, 1, 1),
                work = "FG",
                school = "FG",
                gender = "Female",
                address = "add4",
                privacy = true
            };
            mockDb.Users.Add(u4);
            #endregion

            #region Posts
            var p1 = new Post()
            {
                PostId = "1",
                Text = "someText 1",
                UserId = "1",
                Date = new DateTime(2000,1,1)
            };
            mockDb.Posts.Add(p1);

            var p2 = new Post()
            {
                PostId = "2",
                Text = "someText 2",
                UserId = "2",
                Date = new DateTime(2001, 1, 1)
            };
            mockDb.Posts.Add(p2);

            var p3 = new Post()
            {
                PostId = "3",
                Text = "someText 3",
                UserId = "4",
                Date = new DateTime(2002, 1, 1)
            };
            mockDb.Posts.Add(p3);

            var p4 = new Post()
            {
                PostId = "4",
                Text = "someText 4",
                UserId = "1",
                Date = new DateTime(2003, 1, 1)
            };
            mockDb.Posts.Add(p4);

            var p5 = new Post()
            {
                PostId = "5",
                Text = "someText 5",
                UserId = "3",
                Date = new DateTime(2004, 1, 1)
            };
            mockDb.Posts.Add(p5);

            var p6 = new Post()
            {
                PostId = "6",
                Text = "someText 6",
                UserId = "2",
                Date = new DateTime(2005, 1, 1)
            };
            mockDb.Posts.Add(p6);
            #endregion

            #region Photos
            var ph1 = new Photo()
            {
                PhotoId = 1,
                PhotoPath = "bla1",
                UserId = "1",
                Date = new DateTime(2000,1,1),
                IsProfilePicture = true
            };
            mockDb.Photos.Add(ph1);

            var ph2 = new Photo()
            {
                PhotoId = 2,
                PhotoPath = "bla2",
                UserId = "1",
                Date = new DateTime(2001, 1, 1),
                IsProfilePicture = false
            };
            mockDb.Photos.Add(ph2);

            var ph3 = new Photo()
            {
                PhotoId = 3,
                PhotoPath = "bla3",
                UserId = "2",
                Date = new DateTime(2002, 1, 1),
                IsProfilePicture = false
            };
            mockDb.Photos.Add(ph3);

            var ph4 = new Photo()
            {
                PhotoId = 4,
                PhotoPath = "bla4",
                UserId = "4",
                Date = new DateTime(2003, 1, 1),
                IsProfilePicture = false
            };
            mockDb.Photos.Add(ph4);
            #endregion

            #region Notifications
            var n1 = new Notification()
            {
                NotificationId = 1,
                FriendUserId = "2",
                UserId = "1",
                Date = new DateTime(2000,1,1)
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
                GroupId = "1"
            };
            mockDb.Members.Add(m1);

            var m2 = new Member()
            {
                UserId = "1",
                GroupId = "2"
            };
            mockDb.Members.Add(m2);

            var m3 = new Member()
            {
                UserId = "1",
                GroupId = "3"
            };
            mockDb.Members.Add(m3);

            var m4 = new Member()
            {
                UserId = "2",
                GroupId = "1"
            };
            mockDb.Members.Add(m4);

            var m5 = new Member()
            {
                UserId = "2",
                GroupId = "3"
            };
            mockDb.Members.Add(m5);

            var m6 = new Member()
            {
                UserId = "3",
                GroupId = "2"
            };
            mockDb.Members.Add(m6);

            var m7 = new Member()
            {
                UserId = "4",
                GroupId = "3"
            };
            mockDb.Members.Add(m7);

            #endregion

            #region LikesDislikes
            var ld1 = new LikeDislike()
            {
                PostId = "1",
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld1);

            var ld2 = new LikeDislike()
            {
                PostId = "1",
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld2);

            var ld3 = new LikeDislike()
            {
                PostId = "2",
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld3);

            var ld4 = new LikeDislike()
            {
                PostId = "3",
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld4);

            var ld5 = new LikeDislike()
            {
                PostId = "3",
                UserId = "3",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld5);

            var ld6 = new LikeDislike()
            {
                PostId = "4",
                UserId = "4",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld6);

            var ld7 = new LikeDislike()
            {
                PostId = "4",
                UserId = "1",
                Like = true,
                Dislike = false
            };
            mockDb.LikesDislikes.Add(ld7);

            var ld8 = new LikeDislike()
            {
                PostId = "4",
                UserId = "2",
                Like = false,
                Dislike = true
            };
            mockDb.LikesDislikes.Add(ld8);
            #endregion

            #region Groups
            var g1 = new Group()
            {
                Id = "1",
                Name = "Group1"
            };

            var g2 = new Group()
            {
                Id = "2",
                Name = "Group2"
            };

            var g3 = new Group()
            {
                Id = "3",
                Name = "Group3"
            };
            #endregion

            #region Friends
            var f1 = new Friend()
            {
                UserId = "1",
                FriendUserId = "2",
                BestFriend = true,
                Family = true
            };
            mockDb.Friends.Add(f1);

            var f2 = new Friend()
            {
                UserId = "2",
                FriendUserId = "3",
                BestFriend = false,
                Family = true
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

            var f4 = new Friend()
            {
                UserId = "3",
                FriendUserId = "4",
                BestFriend = true,
                Family = false
            };
            mockDb.Friends.Add(f4);

            var f5 = new Friend()
            {
                UserId = "1",
                FriendUserId = "4",
                BestFriend = false,
                Family = false
            };
            mockDb.Friends.Add(f5);
            #endregion

            #region Comments
            var c1 = new Comment()
            {
                CommentId = 1,
                UserId = "1",
                PostId = "1",
                Text = "bla 1",
                Date = new DateTime(2000,11,11)
            };
            mockDb.Comments.Add(c1);

            var c2 = new Comment()
            {
                CommentId = 2,
                UserId = "1",
                PostId = "1",
                Text = "bla 2",
                Date = new DateTime(2001, 11, 11)
            };
            mockDb.Comments.Add(c2);

            var c3 = new Comment()
            {
                CommentId = 3,
                UserId = "2",
                PostId = "2",
                Text = "bla 3",
                Date = new DateTime(2002, 11, 11)
            };
            mockDb.Comments.Add(c3);

            var c4 = new Comment()
            {
                CommentId = 4,
                UserId = "1",
                PostId = "3",
                Text = "bla 4",
                Date = new DateTime(2003, 11, 11)
            };
            mockDb.Comments.Add(c4);

            var c5 = new Comment()
            {
                CommentId = 5,
                UserId = "2",
                PostId = "4",
                Text = "bla 5",
                Date = new DateTime(2004, 11, 11)
            };
            mockDb.Comments.Add(c5);

            var c6 = new Comment()
            {
                CommentId = 6,
                UserId = "3",
                PostId = "6",
                Text = "bla 6",
                Date = new DateTime(2005, 11, 11)
            };
            mockDb.Comments.Add(c6);

            var c7 = new Comment()
            {
                CommentId = 7,
                UserId = "4",
                PostId = "6",
                Text = "bla 7",
                Date = new DateTime(2006, 11, 11)
            };
            mockDb.Comments.Add(c7);

            var c8 = new Comment()
            {
                CommentId = 8,
                UserId = "1",
                PostId = "1",
                Text = "bla 8",
                Date = new DateTime(2007, 11, 11)
            };
            mockDb.Comments.Add(c8);
            #endregion

            service = new UserService(mockDb);
        }
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
            string[] list1 = { "2", "3", "4" };
            string[] list2 = { "1", "3" };
            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);

            foreach (var item in result1)
            {
                Assert.AreNotEqual(item, user1);
            }

            Assert.AreEqual(3, result1.Count);
            Assert.AreEqual(2, result2.Count);
        }

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
            string[] list1 = { "4", "1" };
            string[] list2 = { "6", "2" };
            string[] list3 = { "5" };

            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
            CollectionAssert.AreEqual(list3, result3);
        }

        [TestMethod]
        public void TestGetAllPhotosFromUser()
        {
            // Arrange
            const string user1 = "1";
            const string user2 = "2";

            // Act
            var result1 = service.GetAllPhotosFromUser(user1);
            var result2 = service.GetAllPhotosFromUser(user2);

            // Assert
            int[] list1 = { 2, 1 };
            int[] list2 = { 3 };

            CollectionAssert.AreEqual(list1, result1);
            CollectionAssert.AreEqual(list2, result2);
        }
    }
}
