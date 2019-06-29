using System;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

        [Test]
        public void LineSegmentVert()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(1, 5);
            Assert.AreEqual(Enumerable.Range(1,5).Select(y =>Tuple.Create(1,y)).ToList(),MathHelper.GetLineSegment(start,end));
        }
        
        [Test]
        public void LineSegmentHor()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(5, 1);
            Assert.AreEqual(Enumerable.Range(1,5).Select(y =>Tuple.Create(y,1)).ToList(),MathHelper.GetLineSegment(start,end));
        }
        
        [Test]
        public void LineSegmentDiagPlus()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(5, 5);
            Assert.AreEqual(Enumerable.Range(1,5).Select(y =>Tuple.Create(y,y)).ToList(),MathHelper.GetLineSegment(start,end));
        }
        
        [Test]
        public void LineSegmentDiagMinus()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(-5, -5);
            var ans = MathHelper.GetLineSegment(start, end);
            ans.Reverse();
            Assert.AreEqual(Enumerable.Range(0,7).Select(y =>Tuple.Create(1-y,1-y)).ToList(),ans);
        }

        [Test]
        public void FilledTriangle()
        {
            var a = Tuple.Create(1, 0);
            var b = Tuple.Create(1, 5);
            var c = Tuple.Create(3, 5);
            Console.WriteLine(string.Join(", ",MathHelper.GetFilledTriangle(a,b,c)));
            
        }
    }
}