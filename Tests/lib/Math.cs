using lib;
using NUnit.Framework;

namespace ConsoleApplication1.lib
{
    [TestFixture]
    public class Math
    {
        [Test]
        public void Quantile02()
        {
            var arr = new double[,]
            {
                {1,2},
                {3,4},
                {5,6},
                {7,8},
                {9,10},
            };
            
            Assert.AreEqual(MathHelper.GetQuantile(arr,0.2), 2);
        }
        
        [Test]
        public void Quantile0()
        {
            var arr = new double[,]
            {
                {1,2},
                {3,4},
                {5,6},
                {7,8},
                {9,10},
            };
            
            Assert.AreEqual(MathHelper.GetQuantile(arr,0.1), 1);
        }
        
        [Test]
        public void Quantile1()
        {
            var arr = new double[,]
            {
                {1,2},
                {3,4},
                {5,6},
                {7,8},
                {9,10},
            };
            
            Assert.AreEqual(MathHelper.GetQuantile(arr,1), 10);
        }
    }
}