using System;
using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;

namespace RDFNetwork
{
    public class Centroid : Point
    {
        public int Id { get; }
        public List<byte> Rgb;
        public int Expected { get; set; }

        public static void ResetNextId()
        {
            _nextId = 0;
        }

        #region Constructor

        public Centroid( params double[] coordinates ) : base(coordinates.ToList())
        {
            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }


        public Centroid( SamplePoint samplePoint ) : base( samplePoint.Coordinates )
        {
            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }

        public Centroid( int expected, params double[] coordinates ) : base( coordinates.ToList() )
        {
            Expected = expected;

            Id = _nextId++;
            Rgb = new List<byte>();

            GenerateAreaColor();
        }

        #endregion

        #region Privates
        private static int _nextId;
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