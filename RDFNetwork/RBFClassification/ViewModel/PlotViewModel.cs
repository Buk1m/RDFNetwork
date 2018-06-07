using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;
using IAD_zadanie02.KMeansClustering.Model;
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
            _classification = classification;
            CreateCentroidLineSerie( selectedPlot, selectedDataRange );
            CreateSamplePointsLineSeries( selectedPlot, selectedDataRange );
        }


        #region Privates

        private List<Centroid> _centroids;
        private List<SamplePoint> _samplePoints;
        private List<double> _networkOutput;
        private Classification _classification;

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

            var axisMarginRight = new LinearAxis()
            {
                Position = AxisPosition.Right,
                FontSize = 0
            };
            PlotModel.Axes.Add( axisMarginRight );
        }

        private void CreateSamplePointsLineSeries( int selectedPlot, int selectedDataRange )
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

            for (var i = 0; i < _samplePoints.Count; i++)
            {
                DataPoint dataPoint = new DataPoint();

                double X = _samplePoints[i].Coordinates[selectedPlot];
                double Y = _samplePoints[i].Coordinates[selectedDataRange];

                dataPoint = new DataPoint(X,Y);

                if (_networkOutput[i] < 1.5)
                {
                    if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 2.5)
                        mistejkenThree.Points.Add( dataPoint );
                    else if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 1.5)
                        mistejkenTwo.Points.Add( dataPoint );

                    ones.Points.Add( dataPoint );
                }
                else if (_networkOutput[i] < 2.5)
                {
                    if (SampleRepository.TrainSamples[i].ExpectedValues.First() >= 2.5)
                        mistejkenThree.Points.Add( dataPoint );
                    else if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 1.5)
                        mistejkenOne.Points.Add( dataPoint );

                    twos.Points.Add( dataPoint );
                }
                else
                {
                    if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 1.5)
                        mistejkenOne.Points.Add( dataPoint );

                    else if (SampleRepository.TrainSamples[i].ExpectedValues.First() < 2.5)
                        mistejkenTwo.Points.Add( dataPoint );

                    threes.Points.Add( dataPoint );
                }
            }

            PlotModel.Series.Add( mistejkenOne );
            PlotModel.Series.Add( mistejkenTwo );
            PlotModel.Series.Add( mistejkenThree );
            PlotModel.Series.Add( ones );
            PlotModel.Series.Add( twos );
            PlotModel.Series.Add( threes );
        }

        private void CreateCentroidLineSerie(int selectedPlot, int selectedDataRange)
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