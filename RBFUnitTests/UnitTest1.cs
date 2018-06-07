using System;
using IAD_zadanie02;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFNetwork;

namespace RBFUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Centroid centr1 = new Centroid(1.0);
            Centroid centr2 = new Centroid(1.0,2.0);
            Centroid centr3 = new Centroid(1.0,2.0,3.0);
            Centroid centr4 = new Centroid(1.0);

            var sp = new SamplePoint();

            Assert.AreEqual(centr1.Coordinates.Count, 1 );
            Assert.AreEqual( centr2.Coordinates.Count, 2 );
            Assert.AreEqual( centr3.Coordinates.Count, 3 );
            Assert.AreEqual( centr4.Coordinates.Count, 4 );
        }
    }
}
