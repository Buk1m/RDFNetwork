using System.Collections.Generic;
using System.IO;

namespace IAD_zadanie02
{
    public class SamplePoint4D
    {
        public int NearsetPointId { get; internal set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double V { get; set; }

        #region Constructor

        public SamplePoint4D( double x, double y, double z, double v )
        {
            X = x;
            Y = y;
            Z = z;
            V = v;
        }

        public SamplePoint4D( List<double> points )
        {
            if (points.Count != 4) throw new InvalidDataException( "points" );
            X = points[0];
            Y = points[1];
            Z = points[2];
            V = points[3];
        }

        #endregion
    }
}