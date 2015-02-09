using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tortoise.Shared.Drawing;
 

namespace SharedUnitTests.Drawing
{
    [TestClass]
    public class PointTest
    {
        [TestMethod]
        public void Point_Addition_Test()
        {
            int ap, bp, cp;
            ap = 10;
            bp = 20;
            cp = ap + bp;
            Point a = new Point(ap, ap);
            Point b = new Point(bp, bp);
            Point canary = new Point(ap, ap);

            //Verify equals works.
            Assert.AreEqual<Point>(a, canary, "Equals verification fail.");


            Point c;
            c = a + b;

            Assert.AreEqual(c, new Point(cp, cp), "Point.+()");
        }

        [TestMethod]
        public void Point_Subtraction_Test()
        {
            int ap, bp, cp;
            ap = 20;
            bp = 10;
            cp = ap - bp;
            Point a = new Point(ap, ap);
            Point b = new Point(bp, bp);
            Point canary = new Point(ap, ap);

            //Verify equals works.
            Assert.AreEqual<Point>(a, canary, "Equals verification fail.");


            Point c;
            c = a - b;

            Assert.AreEqual(c, new Point(cp, cp), "Point.-()");
        }


        [TestMethod]
        public void Point_Multiplication_Test()
        {
            int ap, bp, cp;
            ap = 21;
            bp = 16;
            cp = ap * bp;
            Point a = new Point(ap, ap);
            Point b = new Point(bp, bp);
            Point canary = new Point(ap, ap);

            //Verify equals works.
            Assert.AreEqual<Point>(a, canary, "Equals verification fail.");


            Point c;
            c = a * b;

            Assert.AreEqual(c, new Point(cp, cp), "Point.*()");
        }


        [TestMethod]
        public void Point_Division_Test()
        {
            int ap, bp, cp;
            ap = 100;
            bp = 20;
            cp = ap / bp;
            Point a = new Point(ap, ap);
            Point b = new Point(bp, bp);
            Point canary = new Point(ap, ap);

            //Verify equals works.
            Assert.AreEqual<Point>(a, canary, "Equals verification fail.");


            Point c;
            c = a / b;

            Assert.AreEqual(c, new Point(cp, cp), "Point./()");
        }

    }
}
