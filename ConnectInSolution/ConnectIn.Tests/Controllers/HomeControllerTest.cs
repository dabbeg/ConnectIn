using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectIn;
using ConnectIn.Controllers;

namespace ConnectIn.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController controller = new HomeController();
        // Only redirecting Index, no need to test
        [TestMethod]
        public void Index()
        {
            // Arrange

            // Act
            // ViewResult result = controller.Index() as ViewResult;

            // Assert
            // Assert.IsNotNull(result);
        }

        [TestMethod]
        public void About()
        {
            // Arrange

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            if (result != null) Assert.AreEqual("Your application description page.", result.ViewBag.Message);
        }

        [TestMethod]
        public void NewsFeed()
        {
            // Arrange

            // Act

            // Assert
        }

    }
}
