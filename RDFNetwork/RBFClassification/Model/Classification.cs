using System;
using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;
using IAD_zadanie02.KMeansClustering.Model;

namespace RDFNetwork.RBFClassification.Model
{
    public class Classification
    {
        private readonly Random random = new Random();
        public List<SamplePoint> SamplePoints { get; set; } = SampleRepository.GetInputSamplePoints4D();
        internal List<Centroid> Centroids { get; set; } = new List<Centroid>();
        private Neuron _neuron = new Neuron();
        private List<List<double>> hiddenLayerOutputs = new List<List<double>>();
        public List<double> TotalErrors = new List<double>();
        public List<double> Outputs = new List<double>();
        public double LearningRate = 0.05;
        public double Momentum = 0.1;
        private double _prevOutputWeightError;
        private int _epochsNumber;
        private int _neuronNumber;
        public KMeansAlgorithm KMeansAlgorithm = new KMeansAlgorithm();
        private readonly List<double> _beta = new List<double>();
        private int _k = 3;

        public Classification( int neuronNumber, double alfa, int epochNumber )
        {
            _epochsNumber = epochNumber;
            _neuronNumber = neuronNumber;
            KMeansAlgorithm.SamplePoints = SamplePoints;
            KMeansAlgorithm.GenrateCentroids( _neuronNumber );
            Centroids = KMeansAlgorithm.Centroids;
            for (int i = 0; i < epochNumber; i++)
            {
                KMeansAlgorithm.AssignSamplePointsToNearestCentroids();
                KMeansAlgorithm.UpdateCentroidsCoordinates();
            }

            Centroids = KMeansAlgorithm.Centroids;
            CalculateBetaForEachCentroid( alfa );
        }

        private void CalculateBetaForEachCentroid( double alfa )
        {
            for (var i = 0; i < Centroids.Count; i++)
            {
                double distanceToNearestCentroid = MeanDistanceToKNearestCentroids( Centroids[i] );
                double sigma = alfa * distanceToNearestCentroid;
                _beta.Add( ( 1 / ( 2 * sigma * sigma ) ) );
            }
        }

        private double MeanDistanceToKNearestCentroids( Centroid centroid )
        {
            List<double> distances = new List<double>();
            foreach (var examinedCentorid in Centroids.Where( e => centroid.Id != e.Id ))
            {
                distances.Add( Euclides.CalculateDistance( examinedCentorid, centroid ) );
            }

            distances.Sort();

            return distances.Where( e => e <= distances[_k] ).Sum() / _k;
        }

        public void StuffDooer()
        {
            CalculateHiddenLayerOutputs();
            InitOutputLayerWeights();
            TotalErrors.Clear();
            for (int j = 0; j < _epochsNumber; j++)
            {
                var error = 0.0;
                for (var i = 0; i < hiddenLayerOutputs.Count; i++)
                {
                    _neuron.Inputs = hiddenLayerOutputs[i];
                    _neuron.CalculateOutput();

                    Train( hiddenLayerOutputs[i], SampleRepository.TrainSamples[i].ExpectedValues.First() );

                    error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First() );
                }

                TotalErrors.Add( error );
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

        public void DoStuffer()
        {
            Outputs.Clear();
            CalculateHiddenLayerOutputs();
            KMeansAlgorithm.AssignSamplePointsToNearestCentroids();
            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                Outputs.Add( _neuron.Output );
            }
        }

        private void Train( List<double> hiddenLayerOutput, double expected )
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

        private void InitOutputLayerWeights()
        {
            _neuron.Weights.Clear();
            for (var j = 0; j < hiddenLayerOutputs.First().Count; j++)
            {
                _neuron.Weights.Add( random.NextDouble() );
            }
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
                        .Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ),
                            _beta[i] ) );
                }
            }
        }

        private double BasisFunction( double radius, double beta )
        {
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }
    }
}