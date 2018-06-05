using System;
using System.Collections.Generic;
using System.Linq;
using RDFNetwork;

namespace IAD_zadanie02.KMeansClustering.Model
{
    public class KMeansAlgorithm
    {
        #region Lists Of Points

        public List<Centroid4D> Centroids { get; internal set; }
        public List<SamplePoint4D> SamplePoints { get; internal set; }

        #endregion

        #region Constructor

        public KMeansAlgorithm()
        {
            Centroids = new List<Centroid4D>();
        }

        #endregion

        public double MaxCentroidShift { get; private set; } = 1;

        public void GenrateCentroids( int _centoridsNumber )
        {
            MaxCentroidShift = 1;
            Centroids.Clear();
            _excludedSampleIndexes.Clear();
            for (int i = 0; i < _centoridsNumber; i++)
            {
                int choosenSampleIndex;

                do
                {
                    choosenSampleIndex = Random.Next( SamplePoints.Count );
                } while (_excludedSampleIndexes.Contains( choosenSampleIndex ));

                Centroids.Add( new Centroid4D( SamplePoints[choosenSampleIndex] ) );
                _excludedSampleIndexes.Add( choosenSampleIndex );
            }
        }


        public void AssignSamplePointsToNearestCentroids()
        {
            foreach (var samplePoint in SamplePoints)
            {
                samplePoint.NearsetPointId = GetTheNearestCentroidsId( samplePoint );
            }
        }

        public void UpdateCentroidsCoordinates()
        {
            List<double> maxShift = new List<double>();
            foreach (var centroid in Centroids)
            {
                SetAverageCoordinates( centroid );
                maxShift.Add( MaxCentroidShift );
            }

            MaxCentroidShift = maxShift.Max();
        }

        #region Privates

        private static readonly Random Random = new Random();
        private List<int> _excludedSampleIndexes = new List<int>();

        private int GetTheNearestCentroidsId( SamplePoint4D samplePoint4D )
        {
            int nearestCentroidId = Centroids.First().Id;
            double nearestDistance = CalculateDistance( samplePoint4D, Centroids.First() );
            foreach (var centroid in Centroids)
            {
                double distance = CalculateDistance( samplePoint4D, centroid );
                if (distance < nearestDistance)
                {
                    nearestCentroidId = centroid.Id;
                    nearestDistance = distance;
                }
            }

            return nearestCentroidId;
        }

        private void SetAverageCoordinates( Centroid4D centroid )
        {
            var samplePoints = SamplePoints.Where( point => point.NearsetPointId == centroid.Id ).ToArray();
            if (samplePoints.Length != 0)
            {
                double averageX = 0.0;
                double averageY = 0.0;
                double averageZ = 0.0;
                double averageV = 0.0;

                foreach (var samplePoint in samplePoints)
                {
                    averageX += samplePoint.X;
                    averageY += samplePoint.Y;
                    averageZ += samplePoint.Z;
                    averageV += samplePoint.V;
                }

                averageX /= samplePoints.Count();
                averageY /= samplePoints.Count();
                averageZ /= samplePoints.Count();
                averageV /= samplePoints.Count();

                MaxCentroidShift = CalculateDistance( new SamplePoint4D( averageX, averageY, averageZ, averageV ),
                    centroid );

                centroid.X = averageX;
                centroid.Y = averageY;
                centroid.Z = averageZ;
                centroid.V = averageV;
            }
        }


        public double CalculateDistance( SamplePoint4D samplePoint4D, Centroid4D centroid )
        {
            return Math.Sqrt( Math.Pow( samplePoint4D.X - centroid.X, 2 ) +
                              Math.Pow( samplePoint4D.Y - centroid.Y, 2 ) +
                              Math.Pow( samplePoint4D.Z - centroid.Z, 2 ) +
                              Math.Pow( samplePoint4D.V - centroid.V, 2 ) );
        }


        #endregion
    }
}