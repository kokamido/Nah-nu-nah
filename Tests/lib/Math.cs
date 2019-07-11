using System;
using System.Collections.Generic;
using System.Linq;
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
            var res = MathHelper.GetLineSegment(start, end);
            var ans = Enumerable.Range(1, 5).Select(y => Tuple.Create(1, y)).ToList();
            Assert.AreEqual(res.Count,ans.Count);
            foreach (var t in res)
                ans.Remove(t);
            Assert.IsEmpty(ans);
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
        public void LineSegmentNear90()
        {
            var end = Tuple.Create(2, 5);
            var start = Tuple.Create(0, 3);
            var res = MathHelper.GetLineSegment(start, end);
            var ans = new[]
            {
                Tuple.Create(0, 3),
                Tuple.Create(1, 4),
                Tuple.Create(2, 5),
            };
            Assert.AreEqual(ans.Length,res.Count);
            foreach (var tuple in ans)
            {
                res.Remove(tuple);
            }
            Assert.IsEmpty(res);
        }

        [Test]
        public void FilledTriangle()
        {
            var a = Tuple.Create(1, 0);
            var b = Tuple.Create(1, 5);
            var c = Tuple.Create(3, 5);
            var ans = new HashSet<Tuple<int,int>>
            {
                Tuple.Create(1, 0),
                Tuple.Create(1, 1),
                Tuple.Create(1, 2),
                Tuple.Create(1, 3),
                Tuple.Create(1, 4),
                Tuple.Create(1, 5),
                Tuple.Create(2, 2),
                Tuple.Create(2, 3),
                Tuple.Create(2, 4),
                Tuple.Create(2, 5),
                Tuple.Create(3, 4),
                Tuple.Create(3, 5),
            };
            var triangle = new HashSet<Tuple<int, int>>(MathHelper.GetFilledTriangle(a, b, c));
            Console.WriteLine(string.Join(", ",triangle));
            Assert.AreEqual(ans.Count, triangle.Count);
            ans.ExceptWith(triangle);
            Assert.IsEmpty(ans);
        }

        [Test]
        public void FilledTriangle1()
        {
            var a = Tuple.Create(-1, 0);
            var b = Tuple.Create(-1, -5);
            var c = Tuple.Create(-3, -5);
            var ans = new HashSet<Tuple<int,int>>
            {
                Tuple.Create(-1, -0),
                Tuple.Create(-1, -1),
                Tuple.Create(-1, -2),
                Tuple.Create(-1, -3),
                Tuple.Create(-1, -4),
                Tuple.Create(-1, -5),
                Tuple.Create(-2, -2),
                Tuple.Create(-2, -3),
                Tuple.Create(-2, -4),
                Tuple.Create(-2, -5),
                Tuple.Create(-3, -4),
                Tuple.Create(-3, -5),
            };
            var triangle = new HashSet<Tuple<int, int>>(MathHelper.GetFilledTriangle(a, b, c));
            Assert.AreEqual(ans.Count, triangle.Count);
            ans.ExceptWith(triangle);
            Assert.IsEmpty(ans);
        }

        [Test]
        public void GetAngle0()
        {
            Assert.AreEqual(0,Tuple.Create(1,0).GetAngle());
        }

        [Test]
        public void GetAngle90()
        {
            Assert.AreEqual(System.Math.PI/2,Tuple.Create(0,1000).GetAngle(),0.01);
        }

        [Test]
        public void GetAngle225()
        {
            Assert.AreEqual(System.Math.PI*1.25,Tuple.Create(-300,-300).GetAngle(),0.01);
        }

        [Test]
        public void GetAngle315()
        {
            Assert.AreEqual(System.Math.PI*2-System.Math.PI/4,Tuple.Create(12,-12).GetAngle(),0.01);
        }

        [Test]
        public void Rotation0()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(a, a.RotateAroundZero(0));
        }

        [Test]
        public void Rotation90()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(Tuple.Create(-1,1), a.RotateAroundZero(System.Math.PI/2));
        }

        [Test]
        public void Rotation270()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(Tuple.Create(1,-1), a.RotateAroundZero(System.Math.PI*1.5));
        }

        [Test]
        public void Rotation360()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(a, a.RotateAroundZero(System.Math.PI*2));
        }
    }
}