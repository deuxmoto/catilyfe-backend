using CatiLyfe.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatiLyfe.Common.MSTest
{
    /// <summary>
    /// Basic tests on the utilities.
    /// </summary>
    [TestClass]
    public class UtilitiesTests
    {
        /// <summary>
        /// Test to see that the generic dispose works.
        /// </summary>
        [TestMethod]
        public void TestGenericDisposable()
        {
            var value = 0;
            using (var dispose = GenericDisposable.Create(() => value++))
            {
                Assert.AreEqual(0, value);
            }

            Assert.AreEqual(1, value);
        }
    }
}
