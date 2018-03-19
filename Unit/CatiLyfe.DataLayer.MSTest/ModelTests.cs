using CatiLyfe.DataLayer.Models.Images;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CatiLyfe.DataLayer.MSTest
{
    /// <summary>
    /// Tests for the model constructors.
    /// </summary>
    [TestClass]
    public class ModelTests
        /// <summary>
    {
        /// Test the image model.
        /// </summary>
        [TestMethod]
        public void TestImage()
        {
            var testTime = new DateTime(2000, 1, 1);

            var link = new ImageLink(0, 0, 0, 0, "asdf", ImageAdapter.Unknown, "");
            var image = new Image(id: 5, slug: "slug", description: "description", whenCreated: testTime, links: new[] { link });

            Assert.AreEqual(5, image.Id);
            Assert.AreEqual("slug", image.Slug);
            Assert.AreEqual("description", image.Description);
            Assert.AreEqual(1, image.Links.Count);
            Assert.AreEqual(testTime, image.WhenCreated);
        }

        /// <summary>
        /// Test the link model.
        /// </summary>
        [TestMethod]
        public void TestImageLink()
        {
            var link = new ImageLink(imageid: 9, linkId: 3, width: 2, height: 1000, format: "asdf", adapter: ImageAdapter.AzureFile, metadata: "la la la");

            Assert.AreEqual(9, link.ImageId);
            Assert.AreEqual(3, link.LinkId);
            Assert.AreEqual(2, link.Width);
            Assert.AreEqual(1000, link.Height);
            Assert.AreEqual("asdf", link.Format);
            Assert.AreEqual(ImageAdapter.AzureFile, link.Adapter);
            Assert.AreEqual("la la la", link.Metadata);
        }

        /// <summary>
        /// Test the image adapter enum.
        /// </summary>
        [TestMethod]
        public void TestImageAdapater()
        {
            Assert.AreEqual(default(ImageAdapter), ImageAdapter.Unknown);
        }
    }
}
