using System;
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
            Assert.IsTrue(ans.IsEqualAsSet(res));
        }

        [Test]
        public void LineSegmentHor()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(5, 1);
            var ans = Enumerable.Range(1, 5).Select(y => Tuple.Create(y, 1)).ToList();
            var res = MathHelper.GetLineSegment(start, end);
            Assert.IsTrue(ans.IsEqualAsSet(res));
        }

        [Test]
        public void LineSegmentDiagPlus()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(5, 5);
            var ans = Enumerable.Range(1, 5).Select(y => Tuple.Create(y, y)).ToList();
            var res = MathHelper.GetLineSegment(start, end);
            Assert.IsTrue(ans.IsEqualAsSet(res));
        }

        [Test]
        public void LineSegmentDiagMinus()
        {
            var start = Tuple.Create(1, 1);
            var end = Tuple.Create(-5, -5);
            var res = MathHelper.GetLineSegment(start, end);
            var ans = Enumerable.Range(0, 7).Select(y => Tuple.Create(1 - y, 1 - y)).ToList();
            Assert.IsTrue(ans.IsEqualAsSet(res));
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
            Assert.IsTrue(ans.IsEqualAsSet(res));
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
            Assert.AreEqual(-System.Math.PI*0.75,Tuple.Create(-300,-300).GetAngle(),0.01);
        }

        [Test]
        public void GetAngle315()
        {
            Assert.AreEqual(-System.Math.PI/4,Tuple.Create(12,-12).GetAngle(),0.01);
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
        public void Rotation135()
        {
            var a = Tuple.Create(100, 0);
            Assert.AreEqual(Tuple.Create((-100*System.Math.Sqrt(2)/2).Round(),(100*System.Math.Sqrt(2)/2).Round()), a.RotateAroundZero(3*System.Math.PI/4));
        }

        [Test]
        public void Rotation270()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(Tuple.Create(1,-1), a.RotateAroundZero(System.Math.PI*1.5));
        }

        [Test]
        public void RotationMinus270()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(Tuple.Create(-1,1), a.RotateAroundZero(-System.Math.PI*1.5));
        }

        [Test]
        public void Rotation270Big()
        {
            var a = Tuple.Create(100, 27);
            Assert.AreEqual(Tuple.Create(27,-100), a.RotateAroundZero(System.Math.PI*1.5));
        }

        [Test]
        public void Rotation360()
        {
            var a = Tuple.Create(1, 1);
            Assert.AreEqual(a, a.RotateAroundZero(System.Math.PI*2));
        }

        [Test]
        public void Get1Norm1_0()
        {
            var v = 1.MakeTupleWith(0);
            var ans = 0.MakeTupleWith(1);
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm1_1()
        {
            var v = 1.MakeTupleWith(1);
            var ans = (-1).MakeTupleWith(1).Divide(System.Math.Sqrt(2));
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm0_1()
        {
            var v = 0.MakeTupleWith(1);
            var ans = (-1).MakeTupleWith(0);
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm_min1_1()
        {
            var v = (-1).MakeTupleWith(1);
            var ans = (-1).MakeTupleWith(-1).Divide(System.Math.Sqrt(2));
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm_min1_0()
        {
            var v = (-1).MakeTupleWith(0);
            var ans = 0d.MakeTupleWith(-1d);
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm_min1_min1()
        {
            var v = (-1).MakeTupleWith(-1);
            var ans = 1.MakeTupleWith(-1).Divide(System.Math.Sqrt(2));
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm0_min1()
        {
            var v = (0).MakeTupleWith(-1);
            var ans = 1.MakeTupleWith(0);
            Assert.AreEqual(ans,v.Get1Normal());
        }

        [Test]
        public void Get1Norm1_min1()
        {
            var v = 1.MakeTupleWith(-1);
            var ans = 1.MakeTupleWith(1).Divide(System.Math.Sqrt(2));
            Assert.AreEqual(ans,v.Get1Normal());
        }
    }
}