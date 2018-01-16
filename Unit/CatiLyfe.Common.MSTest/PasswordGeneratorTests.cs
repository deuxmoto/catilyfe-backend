namespace CatiLyfe.Common.MSTest
{
    using CatiLyfe.Common.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    /// <summary>
    /// Tests for the password generator.
    /// </summary>
    [TestClass]
    public class PasswordGeneratorTests
    {
        /// <summary>
        /// Test to see if the random bytes stuff works.
        /// </summary>
        [TestMethod]
        public void TestGetRandomBytes()
        {
            var bytes = PasswordGenerator.GenerateRandom(100);

            Assert.AreEqual(100, bytes.Length);
            Assert.IsTrue(bytes.Any(b => b != 0));
        }
    }
}
