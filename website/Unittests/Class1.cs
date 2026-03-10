
namespace Unittests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void Test1()
        {
			int test = 5;
            Assert.IsTrue(test < 6);
        }

        [TestMethod]
        public void Test2()
        {
            Assert.Fail();
        }
    }
}
