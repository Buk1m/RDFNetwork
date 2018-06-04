using System;
using System.Collections.Generic;
using System.Linq;
using RDFNetwork;

namespace IAD_zadanie02.KMeansClustering.Model
{
    public class KMeansAlgorithm
    {
        #region Lists Of Points

        public List<Centroid1D> Centroids { get; internal set; }
        public List<SamplePoint> SamplePoints { get; internal set; }

        #endregion

        #region Constructor

        public KMeansAlgorithm()
        {
            Centroids = new List<Centroid1D>();
        }

        #endregion

        public double MaxCentroidShift { get; private set; } = 1;

        public void GenrateCentroids( int K )
        {
            MaxCentroidShift = 1;
            CalculateBorderValues();
            Centroids.Clear();
            for ( int i = 0; i < K; i++ )
            {
                Centroids.Add( new Centroid1D( GetRandomXFromRange() ) );
            }
        }

        public void AssignSamplePointsToNearestCentroids()
        {
            foreach ( var samplePoint in SamplePoints )
            {
                samplePoint.NearsetPointId = GetTheNearestCentroidsId( samplePoint );
            }
        }

        public void UpdateCentroidsCoordinates()
        {
            List<double> maxShift = new List<double>();
            foreach ( var centroid in Centroids )
            {
                SetAverageCoordinates( centroid );
                maxShift.Add( MaxCentroidShift );
            }

            MaxCentroidShift = maxShift.Max();
        }

        #region Privates

        private static readonly Random Random = new Random();

        private int GetTheNearestCentroidsId( SamplePoint samplePoint )
        {
            int nearestCentroidId = Centroids.First().Id;
            double nearestDistance = CalculateDistance( samplePoint, Centroids.First() );
            foreach ( var centroid in Centroids )
            {
                double distance = CalculateDistance( samplePoint, centroid );
                if ( distance < nearestDistance )
                {
                    nearestCentroidId = centroid.Id;
                    nearestDistance = distance;
                }
            }

            return nearestCentroidId;
        }

        private void SetAverageCoordinates( Centroid1D centroid )
        {
            var samplePoints = SamplePoints.Where( point => point.NearsetPointId == centroid.Id ).ToArray();
            if ( samplePoints.Length != 0 )
            {
                double averageX = 0.0;

                foreach ( var samplePoint in samplePoints )
                {
                    averageX += samplePoint.X;
                }

                averageX /= (double)samplePoints.Count();

                MaxCentroidShift = CalculateDistance( new SamplePoint( averageX ), centroid );

                centroid.X = averageX;
            }
        }

        private double CalculateDistance( SamplePoint samplePoint, Centroid1D centroid )
        {
            return Math.Abs( samplePoint.X - centroid.X );
        }

        private void CalculateBorderValues()
        {
            _minX = SamplePoints.Min( x => x.X );
            _maxX = SamplePoints.Max( x => x.X );
        }

        private double GetRandomXFromRange()
        {
            return Random.NextDouble() * (_maxX - _minX) + _minX;
        }

        #region BorderCoordinates

        private double _minX;
        private double _maxX;

        #endregion

        #endregion
    }
}