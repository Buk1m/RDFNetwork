using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using IAD_zadanie02;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RDFNetwork.RBFClassification.Model;

namespace RDFNetwork.RBFClassification.ViewModel
{
    public class PlotViewModel : BaseViewModel
    {
        public PlotModel PlotModel { get; set; }

        #region Constructor

        public PlotViewModel()
        {
            PlotModel = new PlotModel();
            SetUpModel();
        }

        #endregion


        public void SetUpPlotModelData( Classification classification, int selectedPlot, int selectedDataRange )
        {
            PlotModel.Series.Clear();
            _centroids = classification.Centroids;
            _samplePoints = classification.SamplePoints;
            _networkOutput = classification.Outputs;
            CreateCentroidLineSerie( selectedPlot, selectedDataRange );
            CreateSamplePointsLineSeries( selectedPlot, selectedDataRange );
        }


        #region Privates

        private List<Centroid> _centroids;
        private List<SamplePoint> _samplePoints;
        private List<double> _networkOutput;


        public void SetUpModel()
        {
            PlotModel.PlotType = PlotType.XY;

            var axisX = new LinearAxis()
            {
                Title = "X",
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                TicklineColor = OxyColor.FromRgb( 0, 0, 0 ),
            };
            PlotModel.Axes.Add( axisX );

            var axisY = new LinearAxis()
            {
                Title = "Y",
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };
            PlotModel.Axes.Add( axisY );
        }

        public void CreateSamplePointsLineSeries( int selectedPlot, int selectedDataRange )
        {
            var ones = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 3,
                MarkerFill = OxyColor.FromRgb( 0, 0, 255 ),
                MarkerType = MarkerType.Circle,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var none = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 3,
                MarkerFill = OxyColor.FromRgb( 127, 127, 50 ),
                MarkerType = MarkerType.Circle,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var twos = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 3,
                MarkerFill = OxyColor.FromRgb( 0, 255, 0 ),
                MarkerType = MarkerType.Circle,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var threes = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 3,
                MarkerFill = OxyColor.FromRgb( 255, 0, 0 ),
                MarkerType = MarkerType.Circle,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var mistejkenOne = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 5,
                MarkerFill = OxyColor.FromRgb( 0, 0, 255 ),
                MarkerType = MarkerType.Diamond,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var mistejkenTwo = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 5,
                MarkerFill = OxyColor.FromRgb( 0, 255, 0 ),
                MarkerType = MarkerType.Diamond,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var mistejkenThree = new LineSeries
            {
                LineStyle = LineStyle.None,
                MarkerSize = 5,
                MarkerFill = OxyColor.FromRgb( 255, 0, 0 ),
                MarkerType = MarkerType.Diamond,
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            int[,] errorMatrix = new int[3, 3];
            for (var i = 0; i < _samplePoints.Count; i++)
            {
                DataPoint dataPoint = new DataPoint();

                double X = _samplePoints[i].Coordinates[selectedPlot];
                if ( selectedPlot == selectedDataRange )
                {
                    selectedDataRange = 3;
                }
                double Y = _samplePoints[i].Coordinates[selectedDataRange];
    
                dataPoint = new DataPoint(X,Y);
                if (_networkOutput.Any())
                {
                    if (_networkOutput[i] < 1.5)
                    {
                        if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 2.5)
                        {
                            errorMatrix[0, 2]++;

                            mistejkenThree.Points.Add( dataPoint );
                        }
                        else if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 1.5)
                        {
                            errorMatrix[0, 1]++;
                            mistejkenTwo.Points.Add( dataPoint );
                        }

                        ones.Points.Add( dataPoint );
                        errorMatrix[0, 0]++;
                    }
                    else if (_networkOutput[i] < 2.5)
                    {
                        if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 2.5)
                        {
                            errorMatrix[1, 2]++;
                            mistejkenThree.Points.Add( dataPoint );

                        }
                        else if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 1.5)
                        {
                            mistejkenOne.Points.Add( dataPoint );
                            errorMatrix[1, 0]++;
                        }

                        twos.Points.Add( dataPoint );
                        errorMatrix[1, 1]++;
                    }
                    else
                    {
                        if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 1.5)
                        {
                            errorMatrix[2, 0]++;
                            mistejkenOne.Points.Add( dataPoint );
                        }
                        else if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 2.5)
                        {
                            mistejkenTwo.Points.Add( dataPoint );
                            errorMatrix[2, 1]++;
                        }


                        threes.Points.Add( dataPoint );
                        errorMatrix[2, 2]++;
                    }

                }
                else
                {
                    none.Points.Add(dataPoint);
                }
            }
            if ( _networkOutput.Any() )
            {
                StringBuilder chceSpac = new StringBuilder();
                for ( int j = 0; j < 3; j++ )
                {
                    for ( int k = 0; k < 3; k++ )
                    {
                        chceSpac.Append( errorMatrix[j, k] );
                        chceSpac.Append( "\t" );
                    }
                    chceSpac.Append( "\n" );

                }

                MessageBox.Show( chceSpac.ToString() );
            }
            PlotModel.Series.Add( mistejkenOne );
            PlotModel.Series.Add( mistejkenTwo );
            PlotModel.Series.Add( mistejkenThree );
            PlotModel.Series.Add( ones );
            PlotModel.Series.Add( twos );
            PlotModel.Series.Add( threes );
            PlotModel.Series.Add( none );
        }

        public void CreateCentroidLineSerie(int selectedPlot, int selectedDataRange)
        {
            foreach (var centroid in _centroids)
            {
                var lineSerie = new LineSeries
                {
                    LineStyle = LineStyle.None,
                    MarkerSize = 6,
                    MarkerFill = OxyColor.FromRgb( 0, 0, 0 ),
                    MarkerType = MarkerType.Square,
                    DataFieldX = "xData",
                    DataFieldY = "yData"
                };

                DataPoint dataPoint = new DataPoint();
                double X = centroid.Coordinates[selectedPlot];
               
                if ( selectedPlot == selectedDataRange )
                {
                    selectedDataRange = 3;
                }

                double Y = centroid.Coordinates[selectedDataRange];

                dataPoint = new DataPoint( X, Y);

                lineSerie.Points.Add( dataPoint );
                PlotModel.Series.Add( lineSerie );

            }
        }
        #endregion
    }
}