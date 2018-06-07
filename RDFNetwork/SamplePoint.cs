using System.Collections.Generic;
using System.Linq;
using RDFNetwork;

namespace IAD_zadanie02
{
    public class SamplePoint : Point
    {
        public int NearsetPointId { get; internal set; }
        public float Expected { get; set; }

        #region Constructor

        public SamplePoint( params double[] coordinates ) : base(coordinates.ToList())
        {
            if (!coordinates.Any())
            {
                Coordinates.Add( 0.0 );
            }
        }

        public SamplePoint( List<double> coordinates ) : base( coordinates )
        {
            if (!coordinates.Any())
            {
                Coordinates.Add( 0.0 );
            }
        }

        public SamplePoint( float expected, params double[] coordinates ) : base(coordinates.ToList())
        {
            Expected = expected;

            if (!coordinates.Any())
            {
                Coordinates.Add( 0.0 );
            }
        }

        #endregion
    }
}