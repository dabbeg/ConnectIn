using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn.Models.Entity;
using ConnectIn.Services;

namespace ConnectIn.Tests.Service
{
    [TestClass]
    public class PhotoServiceTest
    {
        private PhotoService service;

        // Initialize is run before every unit test
        [TestInitialize]
        public void Initialize()
        {
            var mockDb = new MockDatabase();

            #region Photos
            var ph1 = new Photo()
            {
                PhotoId = 1,
                PhotoPath = "bla1",
                UserId = "1",
                Date = new DateTime(2000, 1, 1),
                IsProfilePhoto = true
            };
            mockDb.Photos.Add(ph1);

            var ph2 = new Photo()
            {
                PhotoId = 2,
                PhotoPath = "bla2",
                UserId = "1",
                Date = new DateTime(2001, 1, 1),
                IsProfilePhoto = false
            };
            mockDb.Photos.Add(ph2);

            var ph3 = new Photo()
            {
                PhotoId = 3,
                PhotoPath = "bla3",
                UserId = "2",
                Date = new DateTime(2002, 1, 1),
                IsProfilePhoto = false
            };
            mockDb.Photos.Add(ph3);

            var ph4 = new Photo()
            {
                PhotoId = 4,
                PhotoPath = "bla4",
                UserId = "4",
                Date = new DateTime(2003, 1, 1),
                IsProfilePhoto = false
            };
            mockDb.Photos.Add(ph4);
            #endregion

            service = new PhotoService(mockDb);
        }
        
        #region Get photo by id
        [TestMethod]
        public void TestGetPhotoById()
        {
            // Arrange
            const int photo1 = 1;
            const int photo2 = 2;

            // Act
            var result1 = service.GetPhotoById(photo1);
            var result2 = service.GetPhotoById(photo2);

            // Assert
            Assert.AreEqual("bla1", result1.PhotoPath);
            Assert.AreEqual("bla2", result2.PhotoPath);
        }
        #endregion
    }
}
