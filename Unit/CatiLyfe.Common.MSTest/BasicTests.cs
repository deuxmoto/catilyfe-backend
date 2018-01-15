using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatiLyfe.Common.MSTest
{
    /// <summary>
    /// Basic test class.
    /// </summary>
    [TestClass]
    public class BasicTests
    {
        /// <summary>
        /// Im just worried.
        /// </summary>
        [TestMethod]
        public void BasicTest()
        {
            Assert.IsTrue(true);
        }

        /// <summary>
        /// It could happen.
        /// </summary>
        [TestMethod]
        public void BasicTest2()
        {
            Assert.IsFalse(false);
        }
    }
}
