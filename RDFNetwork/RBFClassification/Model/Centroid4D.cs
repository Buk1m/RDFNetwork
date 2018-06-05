using System;
using System.Collections.Generic;

namespace IAD_zadanie02.KMeansClustering.Model
{
    public class Centroid4D
    {
    public int Id { get; }
        public List<byte> Rgb;

        #region Constructor

        public Centroid4D( double x, double y, double z, double v )
        {
            X = x;
            Y = y;
            Z = z;
            V = v;

            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }

        public Centroid4D( SamplePoint4D samplePoint4D )
        {
            X = samplePoint4D.X;
            Y = samplePoint4D.Y;
            Z = samplePoint4D.Z;
            V = samplePoint4D.V;

            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double V { get; set; }

        public static int _nextId;
        private static readonly Random Random = new Random();

        private void GenerateAreaColor()
        {
            for ( int i = 0; i < 3; i++ )
            {
                Rgb.Add( RandomValue() );
            }
        }

        private byte RandomValue()
        {
            return (byte)Random.Next( 0, 255 );
        }
        #endregion
  }
}