using System;
using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;


namespace RDFNetwork.RBFApproximation
{
    public class Approximation
    {
        private readonly Random random = new Random();
        private int _neuronNumber;
        private readonly List<double> _beta = new List<double>();
        public List<SamplePoint> SamplePoints { get; set; } = SampleRepository.GetInputSamplePoints();
        public List<Centroid> Centroids = new List<Centroid>();
        private List<int> _excludedSampleIndexes = new List<int>();
        private Neuron _neuron = new Neuron();
        private List<List<double>> hiddenLayerOutputs = new List<List<double>>();
        public List<double> TotalErrors = new List<double>();
        public double LearningRate = 0.05;
        public double Momentum = 0.1;
        private double _prevOutputWeightError;
        public List<double> Outputs { get; } = new List<double>();

        private int _k = 4;

        public Approximation( int neuronNumber, double alfa )
        {
            _neuronNumber = neuronNumber;
            GenerateCentroids();
            AssignSamplePointsToNearestCentroids();
            CalculateBetaForEachCentroid( alfa );
        }

        public void StuffDooer()
        {
            Centroid.ResetNextId();
            CalculateHiddenLayerOutputs();
            initOutputLayerWeights();
            TotalErrors.Clear();
        }

        public void Learn()
        {
            Outputs.Clear();
            var error = 0.0;
            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                Train( SampleRepository.TrainSamples[i].ExpectedValues.First() );
                Outputs.Add( _neuron.Output );
                error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First() );
            }

            TotalErrors.Add( error );
        }

        public List<double> DoStuffer()
        {
            List<double> outputs = new List<double>();
            Outputs.Clear();

            CalculateHiddenLayerOutputs();
            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                outputs.Add( _neuron.Output );
                Outputs.Add( _neuron.Output );
            }

            return outputs;
        }

        public void AssignSamplePointsToNearestCentroids()
        {
            foreach (var samplePoint in SamplePoints)
            {
                samplePoint.NearsetPointId = GetTheNearestCentroidsId( samplePoint );
            }
        }

        #region Private Members

        private void CalculateBetaForEachCentroid( double alfa )
        {
            for (var i = 0; i < Centroids.Count; i++)
            {
                double distanceToNearestCentroid = MeanDistanceToKNearestCentroids( Centroids[i] );
                double sigma = alfa * distanceToNearestCentroid;
                _beta.Add( 1 / ( 2 * sigma * sigma ) );
            }
        }

        private double MeanDistanceToKNearestCentroids( Centroid centroid )
        {
            List<double> distances = new List<double>();
            foreach (var examinedCentorid in Centroids.Where( e => centroid.Id != e.Id ))
            {
                distances.Add( Math.Abs( examinedCentorid.Coordinates.First() - centroid.Coordinates.First() ) );
            }

            distances.Sort();

            return distances.Where( e => e < distances[_k] ).Sum() / _k;
        }

        private void CalculateHiddenLayerOutputs()
        {
            hiddenLayerOutputs.Clear();
            for (var j = 0; j < SamplePoints.Count; j++)
            {
                SamplePoint samplePoint = SamplePoints[j];
                hiddenLayerOutputs.Add( new List<double>() );
                for (int i = 0; i < _neuronNumber; i++)
                {
                    hiddenLayerOutputs[j]
                        .Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ), _beta[i] ) );
                }
            }
        }

        public double CalculateOutput( SamplePoint samplePoint )
        {
            List<double> ho = new List<double>();

            for (int i = 0; i < _neuronNumber; i++)
            {
                ho.Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ), _beta[i] ) );
            }

            _neuron.Inputs = ho;
            _neuron.CalculateOutput();
            return _neuron.Output;
        }

        private void Train( double expected )
        {
            // Output neuron deltas
            double outputError = _neuron.ErrorBackPropagationLinear( expected );

            // Update output neuron weights
            double weightError = 0;
            for (var j = 0; j < _neuron.Weights.Count; j++)
            {
                weightError = outputError * _neuron.Inputs[j];

                _neuron.Weights[j] -= LearningRate * weightError +
                                      Momentum * ( LearningRate * weightError - _prevOutputWeightError );


                _prevOutputWeightError = LearningRate * weightError;
            }

            _neuron.Bias -= LearningRate * outputError +
                            Momentum * ( LearningRate * weightError - _prevOutputWeightError );
        }

        private void initOutputLayerWeights()
        {
            _neuron.Weights.Clear();
            for (var j = 0; j < hiddenLayerOutputs.First().Count; j++)
            {
                _neuron.Weights.Add( random.NextDouble() );
            }
        }

        private double BasisFunction( double radius, double beta )
        {
            beta = 10;
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }

        public void GenerateCentroids()
        {
            Centroids.Clear();
            for (int i = 0; i < _neuronNumber; i++)
            {
                int choosenSampleIndex;

                do
                {
                    choosenSampleIndex = random.Next( SamplePoints.Count );
                } while (_excludedSampleIndexes.Contains( choosenSampleIndex ));

                Centroids.Add(
                    new Centroid( choosenSampleIndex, SamplePoints[choosenSampleIndex].Coordinates.First() ) );
                _excludedSampleIndexes.Add( choosenSampleIndex );
            }
        }

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

        #endregion
    }
}