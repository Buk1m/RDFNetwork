using System.Collections.Generic;
using System.Linq;
using IAD_zadanie02;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using RDFNetwork.RBFApproximation;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace RDFNetwork
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


        public void SetUpPlotModelData( List<Centroid1D> centroids, List<SamplePoint> samplePoints, List<double> networkOutput, RBFApproximation.Approximation app )
        {
            PlotModel.Series.Clear();

            _centroids = centroids;
            _samplePoints = samplePoints;
            _networkOutput = networkOutput;
            _app = app;
            CreateCentroidLineSerie();
            CreateSamplePointsLineSeries();
            CreateExpectedPointsLineSeries();

        }

        
        #region Privates

        private List<Centroid1D> _centroids;
        private List<SamplePoint> _samplePoints;
        private List<double> _networkOutput;
        private Approximation _app;
 
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

        private void CreateSamplePointsLineSeries()
        {

            foreach ( var centroid in _centroids )
            {
                var lineSerie = new LineSeries
                {
                    LineStyle = LineStyle.None,
                    MarkerSize = 3,
                    MarkerFill = OxyColor.FromRgb( centroid.Rgb[0], centroid.Rgb[1], centroid.Rgb[2] ),
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "xData",
                    DataFieldY = "yData"
                };

                IEnumerable<SamplePoint> sampleList =_samplePoints.Where( sample => sample.NearsetPointId == centroid.Id );
                foreach (var sample in sampleList)
                {
                        lineSerie.Points.Add(new DataPoint(sample.X, _app.CalculateOutput(sample)));
                }

                //( sample => lineSerie.Points.Add( new DataPoint( sample.X, _app.CalculateOutput(sample) ) ) );
                 PlotModel.Series.Add( lineSerie );
            }

        }

        private void CreateCentroidLineSerie()
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

            _centroids.ForEach( centroid => lineSerie.Points.Add( new DataPoint( centroid.X, _app.CalculateOutput( new SamplePoint( centroid.X ) ) ) ) );
            PlotModel.Series.Add( lineSerie );
        }


        private void CreateExpectedPointsLineSeries()
        {

      
                var lineSerie = new LineSeries
                {
                    LineStyle = LineStyle.None,
                    MarkerSize = 3,
                    MarkerFill = OxyColor.FromAColor(70, OxyColor.FromRgb( 255, 0, 0 ) ),
                    MarkerType = MarkerType.Triangle,
                    DataFieldX = "xData",
                    DataFieldY = "yData"
                };


            for (var index = 0; index < _samplePoints.Count; index++)
            {
                var sample = _samplePoints[index];
                lineSerie.Points.Add( new DataPoint( sample.X, SampleRepository.TrainSamples[index].ExpectedValues.First() ) );
            }

            PlotModel.Series.Add( lineSerie );

        }
        #endregion
    }
}