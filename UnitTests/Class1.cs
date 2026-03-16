using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int x = 5;
            Assert.IsTrue(x == 5);
        }
    }
}
