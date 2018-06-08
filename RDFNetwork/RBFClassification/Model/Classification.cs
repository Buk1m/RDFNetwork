using System;
using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;
using IAD_zadanie02.KMeansClustering.Model;

namespace RDFNetwork.RBFClassification.Model
{
    public class Classification
    {
        //TODO make it private
        public KMeansAlgorithm KMeansAlgorithm { get; } = new KMeansAlgorithm();

        // Data
        public List<SamplePoint> SamplePoints { get; set; }
        public List<Centroid> Centroids { get; set; } = new List<Centroid>();
        public List<double> TotalErrors { get; } = new List<double>();
        public List<double> TotalTestErrors { get; } = new List<double>();
        public List<double> Outputs { get; } = new List<double>();

        // Network Settings
        public double LearningRate { get; internal set; }
        public double Momentum { get; internal set; }
        public int HiddenNeuronsNumber { get; internal set; }
        public int NeighbourNumber { get; internal set; }

        public void SetUpClassificationAlgorithm()
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
            var error = 0.0;
            double testError = 0.0;

            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();

                Train( hiddenLayerOutputs[i], SampleRepository.TrainSamples[i].ExpectedValues.First() );
                Outputs.Add( _neuron.Output );
            }

            for ( int i = 0; i < hiddenLayerOutputs.Count; i++ )
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First() );
            }

            for ( int i = 0; i < SampleRepository.TestSamples.Count; i++ )
            {
                CalculateNetworkOutput( new SamplePoint( SampleRepository.TestSamples[i].Inputs ) );
                testError += _neuron.CalculateError( SampleRepository.TestSamples[i].ExpectedValues.First() );
            }

            TotalErrors.Add( error / hiddenLayerOutputs.Count );
            TotalTestErrors.Add( testError / SampleRepository.TestSamples.Count );

        }

        public void GenerateCentroids()
        {
            KMeansAlgorithm.SamplePoints = SamplePoints;
            KMeansAlgorithm.GenrateCentroids( HiddenNeuronsNumber );
            Centroids = KMeansAlgorithm.Centroids;
        }

        public void StuffDooer()
        {
            CalculateHiddenLayerOutputs();
            InitOutputLayerWeights();
            TotalErrors.Clear();

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

        public void DoStuffer()
        {
            Outputs.Clear();
            CalculateHiddenLayerOutputs();
            KMeansAlgorithm.SamplePoints = SamplePoints;
            KMeansAlgorithm.AssignSamplePointsToNearestCentroids();
            SamplePoints = KMeansAlgorithm.SamplePoints;
            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                Outputs.Add( _neuron.Output );
            }
        }


        #region Private

        // Methods
        public void CalculateBetaForEachCentroid( double alfa )
        {
            _betas.Clear();
            for (var i = 0; i < Centroids.Count; i++)
            {
                double distanceToNearestCentroid = MeanDistanceToKNearestCentroids( Centroids[i] );
                double sigma = alfa * distanceToNearestCentroid;
                _betas.Add( ( 1 / ( 2 * sigma * sigma ) ) );
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

            return distances.Where( e => e <= distances[NeighbourNumber] ).Sum() / NeighbourNumber;
        }

        private void Train( List<double> hiddenLayerOutput, double expected )
        {
            double outputError = _neuron.ErrorBackPropagationLinear( expected );

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
                _neuron.Weights.Add( Random.NextDouble() );
            }
        }

        private void CalculateHiddenLayerOutputs()
        {
            hiddenLayerOutputs.Clear();
            for (var j = 0; j < SamplePoints.Count; j++)
            {
                SamplePoint samplePoint = SamplePoints[j];
                hiddenLayerOutputs.Add( new List<double>() );
                for (int i = 0; i < HiddenNeuronsNumber; i++)
                {
                    hiddenLayerOutputs[j]
                        .Add( BasisFunction( Euclides.CalculateDistance( samplePoint, Centroids[i] ),
                            _betas[i] ) );
                }
            }
        }

        private double BasisFunction( double radius, double beta )
        {
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }

        // Variables
        private readonly Neuron _neuron = new Neuron();
        private readonly List<List<double>> hiddenLayerOutputs = new List<List<double>>();
        private static readonly Random Random = new Random();
        private double _prevOutputWeightError;
        private readonly List<double> _betas = new List<double>();

        #endregion
    }
}