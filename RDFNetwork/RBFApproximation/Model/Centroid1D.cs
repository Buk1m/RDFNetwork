using System;
using System.Collections.Generic;

namespace RDFNetwork
{
    public class Centroid1D
    {
        public int Id { get; }
        public List<byte> Rgb;

        #region Constructor

        public Centroid1D( double x )
        {
            X = x;
            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }

        public Centroid1D( double x, int expected )
        {
            X = x;
            Id = _nextId++;
            Rgb = new List<byte>();
            Expected = expected;
            GenerateAreaColor();
        }

        public double X { get; set; }
        public int Expected { get; set; }

        public static int _nextId;
        private static readonly Random Random = new Random();

        private void GenerateAreaColor()
        {
            for (int i = 0; i < 3; i++)
            {
                Rgb.Add( RandomValue() );
            }
        }

        private byte RandomValue()
        {
            return (byte) Random.Next( 0, 255 );
        }

        #endregion
    }
}