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
        public List<SamplePoint4D> SamplePoints { get; set; } = SampleRepository.GetInputSamplePoints4D();
        private List<Centroid4D> Centroids = new List<Centroid4D>();
        private Neuron _neuron = new Neuron();
        private List<List<double>> hiddenLayerOutputs = new List<List<double>>();
        public List<double> TotalErrors = new List<double>();
        public double LearningRate = 0.05;
        public double Momentum = 0.1;
        private double _prevOutputWeightError;
        private int _epochsNumber;
        private int _neuronNumber;
        private KMeansAlgorithm _kMeansAlgorithm = new KMeansAlgorithm();
        private readonly List<double> _beta = new List<double>();

        public Classification( int neuronNumber, double alfa, int epochNumber )
        {
            _epochsNumber = epochNumber;
            _neuronNumber = neuronNumber;
            _kMeansAlgorithm.SamplePoints = SampleRepository.GetInputSamplePoints4D();
            _kMeansAlgorithm.GenrateCentroids( _neuronNumber );
            Centroids = _kMeansAlgorithm.Centroids;
            for (int i = 0; i < epochNumber; i++)
            {
                _kMeansAlgorithm.AssignSamplePointsToNearestCentroids();
                _kMeansAlgorithm.UpdateCentroidsCoordinates();
            }

            Centroids = _kMeansAlgorithm.Centroids;
            CalculateBetaForEachCentroid( alfa );
        }

        private void CalculateBetaForEachCentroid( double alfa )
        {
            for ( var i = 0; i < Centroids.Count; i++ )
            {
                double distanceToNearestCentroid = FindDistanceToNearstCentorid( Centroids[i] );
                // double distanceToNearestCentroid = EvaluateMeanDistanceToCentorid( Centroids[i] );
                double sigma = alfa * distanceToNearestCentroid;
                _beta.Add( sigma );
            }
        }

        private double FindDistanceToNearstCentorid( Centroid4D centroid )
        {
            if ( !Centroids.Any() )
                throw new ArgumentNullException( "centroids" );

            Centroid4D examinedCentroid = Centroids.First( e => e != centroid );
            double nearestDistance = CalculateDistanceBetweenCentroids( examinedCentroid, centroid );
            foreach ( var element in Centroids )
            {
                if ( centroid != element )
                {
                    double examinedDistance = CalculateDistanceBetweenCentroids( element, centroid ); ;
                    if ( nearestDistance > examinedDistance )
                    {
                        nearestDistance = examinedDistance;
                    }
                }
            }

            return nearestDistance;
        }

        public void StuffDooer()
        {
            CalculateHiddenLayerOutputs();
            InitOutputLayerWeights();
            //Normalize();
            TotalErrors.Clear();
            for ( int j = 0; j < _epochsNumber; j++ )
            {
                var error = 0.0;
                Console.WriteLine( "\n\nEpoch number " + (j + 1) );
                for ( var i = 0; i < hiddenLayerOutputs.Count; i++ )
                {
                    _neuron.Inputs = hiddenLayerOutputs[i];
                    _neuron.CalculateOutput();
                    Console.WriteLine( (i + 1) + " " + _neuron.Output );
                    Train( hiddenLayerOutputs[i], SampleRepository.TrainSamples[i].ExpectedValues.First() );

                    error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First() );
                }
                TotalErrors.Add( error );

            }
        }

        public List<double> DoStuffer()
        {
            List<double> outputs = new List<double>();
            CalculateHiddenLayerOutputs();
            for ( var i = 0; i < hiddenLayerOutputs.Count; i++ )
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                outputs.Add( _neuron.Output );
                Console.WriteLine((i+1) + "\t" + _neuron.Output );
            }
            return outputs;
        }

        private void Train( List<double> hiddenLayerOutput, double expected )
        {
            // Output neuron deltas
            double outputError = _neuron.ErrorBackPropagationLinear( expected );

            // Update output neuron weights
            double weightError = 0;
            for ( var j = 0; j < _neuron.Weights.Count; j++ )
            {
                weightError = outputError * _neuron.Inputs[j];

                _neuron.Weights[j] -= LearningRate * weightError +
                                      Momentum * (LearningRate * weightError - _prevOutputWeightError);


                _prevOutputWeightError = LearningRate * weightError;
            }

            _neuron.Bias -= LearningRate * outputError +
                            Momentum * (LearningRate * weightError - _prevOutputWeightError);
        }

        private double CalculateDistanceBetweenCentroids( Centroid4D centroidA, Centroid4D centroidB )
        {
            return Math.Sqrt( Math.Pow( centroidA.X - centroidB.X, 2 ) +
                              Math.Pow( centroidA.Y - centroidB.Y, 2 ) +
                              Math.Pow( centroidA.Z - centroidB.Z, 2 ) +
                              Math.Pow( centroidA.V - centroidB.V, 2 ) );
        }

        private void InitOutputLayerWeights()
        {
            for ( var j = 0; j < hiddenLayerOutputs.First().Count; j++ )
            {
                _neuron.Weights.Add( random.NextDouble() );
            }
        }

        private void CalculateHiddenLayerOutputs()
        {
            hiddenLayerOutputs.Clear();
            for ( var j = 0; j < SamplePoints.Count; j++ )
            {
                SamplePoint4D samplePoint4D = SamplePoints[j];
                hiddenLayerOutputs.Add( new List<double>() );
                for ( int i = 0; i < _neuronNumber; i++ )
                {
                    hiddenLayerOutputs[j]
                        .Add( BasisFunction( _kMeansAlgorithm.CalculateDistance(samplePoint4D,Centroids[i]), 0.1 /*_beta[i]*/ ) );
                }
            }
        }

        private double BasisFunction( double radius, double beta )
        {
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }
    }
}