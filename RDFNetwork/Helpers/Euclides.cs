using System;

namespace RDFNetwork
{
    public static class Euclides
    {
        public static double CalculateDistance( Point n, Point p )
        {
            double distance = 0.0;
            for ( int i = 0; i < n.Coordinates.Count; i++ )
            {
                distance += Math.Pow( (n.Coordinates[i] - p.Coordinates[i]), 2 );
            }

            return Math.Sqrt( distance );
        }
    }
}