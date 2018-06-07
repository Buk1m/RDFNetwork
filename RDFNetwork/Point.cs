using System.Collections.Generic;

namespace RDFNetwork
{
    public class Point
    {
        #region Coordinates

        public List<double> Coordinates { get; internal set; }

        #endregion

        #region Constructor

        public Point( List<double> coordinates )
        {
            Coordinates = coordinates;
        }

        #endregion
    }
}