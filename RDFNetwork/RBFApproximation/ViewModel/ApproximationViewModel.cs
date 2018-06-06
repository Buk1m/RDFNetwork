using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using OxyPlot;
using RDFNetwork.Commands;

namespace RDFNetwork.RBFApproximation.ViewModel
{
    class ApproximationViewModel : BaseViewModel
    {
        public ApproximationViewModel()
        {
            OpenFileClick = new RelayCommand( OpenFile );
            OpenInTextEditorClick = new RelayCommand( OpenInTextEditor,
                () => _pathToFile != null && _pathToFile.Any() );
            ApproximateClick = new RelayCommand( Aproximate );
            DoStuffClick = new RelayCommand( DoStuff );
            _plotModelViewModel = new PlotViewModel();
            _errorPlotModelViewModel = new ErrorPlotViewModel();
            GenerateClick = new RelayCommand( Generate );
        }

        #region Commands

        public ICommand OpenFileClick { get; }
        public RelayCommand OpenInTextEditorClick { get; }
        public ICommand ApproximateClick { get; }
        public ICommand GenerateClick { get; }
        public ICommand DoStuffClick { get; }

        #endregion

        #region Public Properties

        public string FileName
        {
            get => _fileName;
            private set
            {
                if (_fileName == value)
                    return;

                _fileName = value;
                OpenInTextEditorClick.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public PlotModel PlotModel
        {
            get => _plotModelViewModel.PlotModel;
            internal set => _plotModelViewModel.PlotModel = value;
        }

        public PlotModel ErrorPlotModel
        {
            get => _errorPlotModelViewModel.PlotModel;
            internal set => _errorPlotModelViewModel.PlotModel = value;
        }

        public double LerningRate { get; set; } = 0.05;
        public double Momentum { get; set; } = 0.5;
        public double Alfa { get; set; } = 1.1;
        public int EpochsNumber { get; set; } = 500;
        public int NeuronNumber { get; set; } = 10;
        public bool Flatten { get; set; } = false;

        #endregion


        #region Private Members

        private string _fileName;
        private string _pathToFile;
        private readonly PlotViewModel _plotModelViewModel;
        private readonly ErrorPlotViewModel _errorPlotModelViewModel;
        private Approximation _app;
        private DispatcherTimer _timer;
        private int iterator = 0;
        private bool TemporaryTemper = true;

        public void Generate()
        {
            _app = new Approximation( NeuronNumber, Alfa );
            _app.SamplePoints = SampleRepository.GetInputSamplePoints();
            _app.AssignSamplePointsToNearestCentroids();
            _plotModelViewModel.ShowGeneratedCentroids( _app.SamplePoints, _app.Centroids, TemporaryTemper );
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog =
                new OpenFileDialog() {Filter = "TextFiles (*.txt)|*.txt", RestoreDirectory = true};

            if (openFileDialog.ShowDialog() == true)
            {
                _pathToFile = openFileDialog.FileName;
                FileName = openFileDialog.FileName.Split( '\\' ).Last();
                try
                {
                    SampleRepository.TrainSamples = FileReader.ReadFromFile( openFileDialog.FileName );
                    _plotModelViewModel.PlotModel.Series.Clear();
                    _plotModelViewModel.ShowSamples( TemporaryTemper );
                }
                catch (FileNotFoundException fnfe)
                {
                    MessageBox.Show( fnfe.Message );
                }
                catch (FormatException fe)
                {
                    MessageBox.Show( fe.Message );
                }
            }
        }


        private void Aproximate()
        {
            iterator = 0;
            _app.StuffDooer();
            StartLearningProcess();
        }


        private void StartLearningProcess()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 0 ) };
            _timer.Tick += AutoLearningTicker;
            _timer.Start();
        }

        private void AutoLearningTicker( object sender, EventArgs e )
        {
            if (iterator++ > EpochsNumber)
                _timer.Stop();
            _app.Learn();
            _plotModelViewModel.SetUpPlotModelData( _app.Centroids, _app.SamplePoints, _app.Outputs, _app );
            _errorPlotModelViewModel.SetUpPlotModelData( _app.TotalErrors );
            _plotModelViewModel.PlotModel.InvalidatePlot( true );
            _errorPlotModelViewModel.PlotModel.InvalidatePlot( true );

        }

        public void DoStuff()
        {
            _app.SamplePoints = SampleRepository.GetInputSamplePoints();
            _app.AssignSamplePointsToNearestCentroids();
            List<double> outputsList = _app.DoStuffer();
            _plotModelViewModel.SetUpPlotModelData( _app.Centroids, _app.SamplePoints, _app.Outputs, _app );
            _errorPlotModelViewModel.SetUpPlotModelData( _app.TotalErrors );
            _plotModelViewModel.PlotModel.InvalidatePlot( true );
            _errorPlotModelViewModel.PlotModel.InvalidatePlot( true );
            foreach (var ax in _plotModelViewModel.PlotModel.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
            foreach (var ax in _errorPlotModelViewModel.PlotModel.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
        }

        private void OpenInTextEditor()
        {
            try
            {
                System.Diagnostics.Process.Start( "notepad++.exe", _pathToFile );
            }
            catch (Win32Exception)
            {
                System.Diagnostics.Process.Start( "explorer.exe", _pathToFile );
            }
        }

        #endregion
    }
}