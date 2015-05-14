using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Services;
using ConnectIn.Models.Entity;

namespace ConnectIn.Tests.Service
{
    [TestClass]
    public class LikeDislikeServiceTest
    {
        private LikeDislikeService service;

        // Initialize is run before every unit test
        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();

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

            service = new LikeDislikeService(mockDb);
        }

        #region get likes/dislikes from userid and postid
        [TestMethod]
        public void TestGetLikeDislike()
        {
            // Arrange
            const string user1 = "1";
            const int post1 = 1;

            // Act
            var result1 = service.GetLikeDislike(user1, post1);

            // Assert
            Assert.IsTrue(result1.Like);
            Assert.IsFalse(result1.Dislike);
        }
        #endregion
    }
}
