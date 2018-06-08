using System;
using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;


namespace RDFNetwork.RBFApproximation
{
    public class Approximation
    {
        // Data
        public List<SamplePoint> SamplePoints { get; set; }
        public List<Centroid> Centroids { get; } = new List<Centroid>();
        public List<double> TotalErrors { get; } = new List<double>();
        public List<double> TotalTestErrors { get; } = new List<double>();
        public List<double> Outputs { get; } = new List<double>();

        // Network Settings
        public double LearningRate { get; internal set; }
        public double Momentum { get; internal set; }
        public int HiddenNeuronsNumber { get; internal set; }
        public int NeighbourNumber { get; internal set; }

        public void SetupAproximationAlgorithm()
        {
            _prevOutputWeightError = 0;
            Centroid.ResetNextId();
            CalculateHiddenLayerOutputs();
            InitOutputLayerWeights();
            TotalErrors.Clear();
            TotalTestErrors.Clear();
        }

        public void Learn()
        {
            Outputs.Clear();
            double error = 0.0;
            double testError = 0.0;
            for (int i = 0; i < _hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = _hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                Train( SampleRepository.TrainSamples[i].ExpectedValues.First() );
                Outputs.Add( _neuron.Output );
            }

            for (int i = 0; i < _hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = _hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First() );
            }

            for ( int i = 0; i < SampleRepository.TestSamples.Count; i++ )
            { 
                CalculateNetworkOutput( new SamplePoint( SampleRepository.TestSamples[i].Inputs ) );
            testError += _neuron.CalculateError( SampleRepository.TestSamples[i].ExpectedValues.First() );
            }

            TotalErrors.Add( error/ _hiddenLayerOutputs.Count );
            TotalTestErrors.Add( testError/ SampleRepository.TestSamples.Count );
        }

        public List<double> DoStuffer()
        {
            List<double> outputs = new List<double>();
            Outputs.Clear();

            CalculateHiddenLayerOutputs();
            for (var i = 0; i < _hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = _hiddenLayerOutputs[i];
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

        public void GenerateCentroids()
        {
            Centroids.Clear();
            List<int> excludedSampleIndexes = new List<int>();
            for (int i = 0; i < HiddenNeuronsNumber; i++)
            {
                int choosenSampleIndex;

                do
                {
                    choosenSampleIndex = Random.Next( SamplePoints.Count );
                } while (excludedSampleIndexes.Contains( choosenSampleIndex ));

                Centroids.Add(
                    new Centroid( choosenSampleIndex, SamplePoints[choosenSampleIndex] ) );
                excludedSampleIndexes.Add( choosenSampleIndex );
            }
        }

        public double CalculateNetworkOutput( SamplePoint samplePoint )
        {
            List<double> hiddenLayerOutput = new List<double>();

            for (int i = 0; i < HiddenNeuronsNumber; i++)
            {
                hiddenLayerOutput.Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ),
                    _betas[i] ) );
            }

            _neuron.Inputs = hiddenLayerOutput;
            _neuron.CalculateOutput();
            return _neuron.Output;
        }

        public void CalculateBetaForEachCentroid( double alfa )
        {
            _betas.Clear();
            for (var i = 0; i < Centroids.Count; i++)
            {
                double distanceToNearestCentroid = MeanDistanceToKNearestCentroids( Centroids[i] );
                double sigma = distanceToNearestCentroid;
                _betas.Add( alfa * ( 1 / ( 2 * sigma * sigma ) ) );
            }
        }

        #region Private Members

        // Methods
        private static double BasisFunction( double radius, double beta )
        {
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }

        private double MeanDistanceToKNearestCentroids( Centroid centroid )
        {
            List<double> distances = new List<double>();
            foreach (var examinedCentorid in Centroids.Where( e => centroid.Id != e.Id ))
            {
                distances.Add( Euclides.CalculateDistance( examinedCentorid, centroid ) );
            }

            distances.Sort();

            return distances.Where( e => e <= distances[NeighbourNumber-1] ).Sum() / NeighbourNumber;
        }

        private void CalculateHiddenLayerOutputs()
        {
            _hiddenLayerOutputs.Clear();
            for (var j = 0; j < SamplePoints.Count; j++)
            {
                SamplePoint samplePoint = SamplePoints[j];
                _hiddenLayerOutputs.Add( new List<double>() );
                for (int i = 0; i < HiddenNeuronsNumber; i++)
                {
                    _hiddenLayerOutputs[j]
                        .Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ), _betas[i] ) );
                }
            }
        }

        private void Train( double expected )
        {
            double outputError = _neuron.ErrorBackPropagationLinear( expected );

            double weightError = 0.0;
            for (int j = 0; j < _neuron.Weights.Count; j++)
            {
                weightError = outputError * _neuron.Inputs[j];

                _neuron.Weights[j] -= LearningRate * weightError +
                                      Momentum * ( LearningRate * weightError - _prevOutputWeightError );

                _prevOutputWeightError = LearningRate * weightError;
            }

            _neuron.Bias -= LearningRate * outputError +
                            Momentum * ( LearningRate * weightError - _prevOutputWeightError );
        }

        private void InitOutputLayerWeights()
        {
            _neuron.Bias = 1;
            _neuron.Weights.Clear();
            for (var j = 0; j < HiddenNeuronsNumber; j++)
            {
                _neuron.Weights.Add( Random.NextDouble() );
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

        // Variables
        private readonly Neuron _neuron = new Neuron();
        private readonly List<List<double>> _hiddenLayerOutputs = new List<List<double>>();
        private readonly List<double> _betas = new List<double>();
        private double _prevOutputWeightError;

        private static readonly Random Random = new Random();

        #endregion
    }
}