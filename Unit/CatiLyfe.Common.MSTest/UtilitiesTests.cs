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

        /// <summary>
        /// Test the rounding code with zero.
        /// </summary>
        [TestMethod]
        public void PowerOfTwoRoundDown0()
        {
            Assert.AreEqual(0, ComplexMath.RoundDownP2(0));
        }

        /// <summary>
        /// Test the rounding code with 1.
        /// </summary>
        [TestMethod]
        public void PowerOfTwoRoundDown1()
        {
            Assert.AreEqual(1, ComplexMath.RoundDownP2(1));
        }

        /// <summary>
        /// Test the rounding code with 1024.
        /// </summary>
        [TestMethod]
        public void PowerOfTwoRoundDown1024()
        {
            Assert.AreEqual(1024, ComplexMath.RoundDownP2(1024));
        }

        /// <summary>
        /// Test the rounding code with other number.
        /// </summary>
        [TestMethod]
        public void PowerOfTwoRoundDown()
        {
            Assert.AreEqual(512, ComplexMath.RoundDownP2(600));
        }
    }
}
