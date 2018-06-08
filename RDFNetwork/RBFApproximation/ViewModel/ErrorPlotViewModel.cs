using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace RDFNetwork
{
    public class ErrorPlotViewModel : BaseViewModel
    {
        public PlotModel PlotModel { get; set; } = new PlotModel();

        #region Constructor

        public ErrorPlotViewModel()
        {
            SetUpModel();
        }

        #endregion

        public void SetUpPlotModelData( List<double> totalErrors, List<double> totalTestErrors )
        {
            _totalErrors = totalErrors;
            _totalTestErrors = totalTestErrors;
            PlotModel.Series.Clear();
            CreateSamplePointsLineSeries();
        }

        #region Privates

        private List<double> _totalErrors;
        private List<double> _totalTestErrors;

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

        private void CreateSamplePointsLineSeries()
        {
            var lineSerie = new LineSeries
            {
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb( 255, 0, 0 ),
                DataFieldX = "xData",
                DataFieldY = "yData"
            };
            var testLineSerie = new LineSeries
            {
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb( 0, 255, 0 ),
                DataFieldX = "xData",
                DataFieldY = "yData"
            };


            for (var i = 0; i < _totalTestErrors.Count; i++)
            {
                var error = _totalTestErrors[i];
                testLineSerie.Points.Add( new DataPoint( i, error ) );
            }

            for (var i = 0; i < _totalErrors.Count; i++)
            {
                var error = _totalErrors[i];
                lineSerie.Points.Add( new DataPoint( i, error ) );
            }

            PlotModel.Series.Add( lineSerie );
            PlotModel.Series.Add( testLineSerie );
        }
    }

    #endregion
}