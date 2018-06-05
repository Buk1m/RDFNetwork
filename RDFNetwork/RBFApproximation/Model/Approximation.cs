using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using IAD_zadanie02;

namespace RDFNetwork.RBFApproximation
{
    public class Approximation
    {
        private readonly Random random = new Random();
        private int _neuronNumber;
        private readonly List<double> _beta = new List<double>();
        public List<SamplePoint1D> SamplePoints { get; set; } = SampleRepository.GetInputSamplePoints();
        public List<Centroid1D> Centroids = new List<Centroid1D>();
        private List<int> _excludedSampleIndexes = new List<int>();
        private Neuron _neuron = new Neuron();
        private List<List<double>> hiddenLayerOutputs = new List<List<double>>();
        public List<double> TotalErrors = new List<double>();
        public double LearningRate = 0.05;
        public double Momentum = 0.1;
        private double _prevOutputWeightError;
        private int _epochsNumber;

        public Approximation( int neuronNumber, double alfa, int epochNumber )
        {
            _epochsNumber = epochNumber;
            _neuronNumber = neuronNumber;
            GenerateCentroids();
            AssignSamplePointsToNearestCentroids();
            CalculateBetaForEachCentroid( alfa );
        }

        public void StuffDooer()
        {
            CalculateHiddenLayerOutputs();
            initOutputLayerWeights();
            //Normalize();
            TotalErrors.Clear();
            for (int j = 0; j < _epochsNumber; j++)
            {
                var error = 0.0;
                Console.WriteLine( "\n\nEpoch number " + ( j + 1 ) );
                for (var i = 0; i < hiddenLayerOutputs.Count; i++)
                {
                    _neuron.Inputs = hiddenLayerOutputs[i];
                    _neuron.CalculateOutput();
                    Console.WriteLine( ( i + 1 ) + " " + _neuron.Output );
                    Train( hiddenLayerOutputs[i], SampleRepository.TrainSamples[i].ExpectedValues.First() );

                    error += _neuron.CalculateError( SampleRepository.TrainSamples[i].ExpectedValues.First());
                }
                TotalErrors.Add( error );

            }
        }

        public List<double> DoStuffer()
        {
            List<double> outputs = new List<double>();
            CalculateHiddenLayerOutputs();
            for (var i = 0; i < hiddenLayerOutputs.Count; i++)
            {
                _neuron.Inputs = hiddenLayerOutputs[i];
                _neuron.CalculateOutput();
                outputs.Add( _neuron.Output );
            }
            return outputs;
        }

        public void AssignSamplePointsToNearestCentroids()
        {
            foreach ( var samplePoint in SamplePoints )
            {
                samplePoint.NearsetPointId = GetTheNearestCentroidsId( samplePoint );
            }
        }

        #region Private Members

        private void Normalize() //TODO make it right :)
        {
            foreach (var hiddenLayerOutput in hiddenLayerOutputs)
            {
                double sum = 0;
                for (int i = 0; i < hiddenLayerOutput.Count; i++)
                {
                    sum += hiddenLayerOutput[i];
                }

                for (int i = 0; i < hiddenLayerOutput.Count; i++)
                {
                    hiddenLayerOutput[i] /= sum;
                }
            }
        }

        private void CalculateBetaForEachCentroid( double alfa )
        {
            for (var i = 0; i < Centroids.Count; i++)
            {
                double distanceToNearestCentroid = FindDistanceToNearstCentorid( Centroids[i] );
               // double distanceToNearestCentroid = EvaluateMeanDistanceToCentorid( Centroids[i] );
                double sigma = alfa * distanceToNearestCentroid;
                _beta.Add( sigma );
            }
        }

        private double EvaluateMeanDistanceToCentorid( Centroid1D centroid )
        {
            double sum = 0;
            IEnumerable<SamplePoint1D> centroidSamplePoints =
                SamplePoints.Where( samplePoint => samplePoint.NearsetPointId == centroid.Id );
            foreach (var centroidSamplePoint in centroidSamplePoints)
            {
                Console.WriteLine( centroidSamplePoint.X );
            }

            foreach (var point in centroidSamplePoints)
            {
                sum += CalculateDistance( point, centroid );
            }

            if (sum == 0)
                return 1;
            return sum / centroidSamplePoints.Count();
        }

        private double FindDistanceToNearstCentorid( Centroid1D centroid )
        {
            if (!Centroids.Any())
                throw new ArgumentNullException( "centroids" );

            double nearestDistance = Math.Abs( centroid.X - Centroids.First( e => e != centroid ).X );

            foreach (var element in Centroids)
            {
                if (centroid != element)
                {
                    double examinedDistance = Math.Abs( element.X - centroid.X );
                    if (nearestDistance > examinedDistance)
                    {
                        nearestDistance = examinedDistance;
                    }
                }
            }

            return nearestDistance;
        }

        private void CalculateHiddenLayerOutputs()
        {
            hiddenLayerOutputs.Clear();
            for (var j = 0; j < SamplePoints.Count; j++)
            {
                SamplePoint1D samplePoint1D = SamplePoints[j];
                hiddenLayerOutputs.Add( new List<double>() );
                for (int i = 0; i < _neuronNumber; i++)
                {
                    hiddenLayerOutputs[j]
                        .Add( BasisFunction( CalculateDistance( samplePoint1D, Centroids[i] ), _beta[i] ) );
                }
            }
        }

        public double CalculateOutput( SamplePoint1D samplePoint1D )
        {
            List<double> ho = new List<double>();

            for (int i = 0; i < _neuronNumber; i++)
            {
                ho.Add( BasisFunction( CalculateDistance( samplePoint1D, Centroids[i] ), _beta[i] ) );
            }

            _neuron.Inputs = ho;
            _neuron.CalculateOutput();
            return _neuron.Output;
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

        private void initOutputLayerWeights()
        {
            for (var j = 0; j < hiddenLayerOutputs.First().Count; j++)
            {
                _neuron.Weights.Add( random.NextDouble() );
            }
        }

        private double BasisFunction( double radius, double beta )
        {
            return Math.Exp( -beta * Math.Pow( radius, 2 ) );
        }

        private void GenerateCentroids()
        {
            for (int i = 0; i < _neuronNumber; i++)
            {
                int choosenSampleIndex;

                do
                {
                    choosenSampleIndex = random.Next( SamplePoints.Count );
                } while (_excludedSampleIndexes.Contains( choosenSampleIndex ));

                Centroids.Add( new Centroid1D( SamplePoints[choosenSampleIndex].X ) );
                _excludedSampleIndexes.Add( choosenSampleIndex );
            }
        }

        private int GetTheNearestCentroidsId( SamplePoint1D samplePoint1D )
        {
            int nearestCentroidId = Centroids.First().Id;
            double nearestDistance = CalculateDistance( samplePoint1D, Centroids.First() );
            foreach (var centroid in Centroids)
            {
                double distance = CalculateDistance( samplePoint1D, centroid );
                if (distance < nearestDistance)
                {
                    nearestCentroidId = centroid.Id;
                    nearestDistance = distance;
                }
            }

            return nearestCentroidId;
        }

        private double CalculateDistance( SamplePoint1D samplePoint1D, Centroid1D centroid )
        {
            return Math.Abs( samplePoint1D.X - centroid.X );
        }

        #endregion
    }
}