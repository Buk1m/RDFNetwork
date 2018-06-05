using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using IAD_zadanie02;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using RDFNetwork.RBFApproximation;
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

        public void SetUpPlotModelData( List<double> totalErrors )
        {
            _totalErrors = totalErrors;
            PlotModel.Series.Clear();
            CreateSamplePointsLineSeries();
        }

        #region Privates

        private List<double> _totalErrors;

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
            var lineSerie = new LineSeries
            {
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb( 255,0,0 ),
                DataFieldX = "xData",
                DataFieldY = "yData"
            };


            for (var i = 0; i < _totalErrors.Count; i++)
            {
                var error = _totalErrors[i];
                lineSerie.Points.Add( new DataPoint( i, error ) );
            }

            PlotModel.Series.Add( lineSerie );
        }
    }

    #endregion
}
