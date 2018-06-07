using System;
using System.Collections.Generic;
using System.Linq;
using RDFNetwork;

namespace IAD_zadanie02.KMeansClustering.Model
{
    public class KMeansAlgorithm
    {
        #region Lists Of Points

        public List<Centroid> Centroids { get; internal set; }
        public List<SamplePoint> SamplePoints { get; internal set; }

        #endregion

        #region Constructor

        public KMeansAlgorithm()
        {
            Centroids = new List<Centroid>();
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

                Centroids.Add( new Centroid( SamplePoints[choosenSampleIndex] ) );
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

        private int GetTheNearestCentroidsId( SamplePoint samplePoint )
        {
            int nearestCentroidId = Centroids.First().Id;
            double nearestDistance = Euclides.CalculateDistance( samplePoint, Centroids.First() );
            foreach (var centroid in Centroids)
            {
                double distance = Euclides.CalculateDistance( samplePoint, centroid );
                if (distance < nearestDistance)
                {
                    nearestCentroidId = centroid.Id;
                    nearestDistance = distance;
                }
            }

            return nearestCentroidId;
        }

        private void SetAverageCoordinates( Centroid centroid )
        {
            var samplePoints = SamplePoints.Where( point => point.NearsetPointId == centroid.Id ).ToArray();
            if (samplePoints.Length != 0)
            {
                List<double> averages = new List<double>();

                for (var i = 0; i < samplePoints.First().Coordinates.Count; i++)
                {
                    averages.Add( 0.0 );
                }

                foreach (var samplePoint in samplePoints)
                {
                    for (var i = 0; i < averages.Count; i++)
                    {
                        averages[i] += samplePoint.Coordinates[i];
                    }

                }

                for (var i = 0; i < averages.Count; i++)
                {
                    averages[i] /= samplePoints.Length;
                }

                MaxCentroidShift = Euclides.CalculateDistance( new SamplePoint( averages ),
                    centroid );

                centroid.Coordinates = averages;
            }
        }

        #endregion
    }
}