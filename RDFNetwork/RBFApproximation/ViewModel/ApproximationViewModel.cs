using System;
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
        #region Constructor

        public ApproximationViewModel()
        {
            _approximation = new Approximation();
            OpenTrainFileClick = new RelayCommand( OpenTrainFile );
            OpenTestFileClick = new RelayCommand( OpenTestFile );
            OpenInTextEditorClick = new RelayCommand( OpenInTextEditor,
                () => _pathToFile != null && _pathToFile.Any() );
            ApproximateClick = new RelayCommand( AproximateAndPlot );
            DoStuffClick = new RelayCommand( ShowPlotFromDataWithoutLearning );
            _plotModelViewModel = new PlotViewModel();
            _errorPlotModelViewModel = new ErrorPlotViewModel();
            GenerateClick = new RelayCommand( Generate );
        }

        #endregion

        #region Commands

        public ICommand OpenTrainFileClick { get; }
        public ICommand OpenTestFileClick { get; }
        public RelayCommand OpenInTextEditorClick { get; }
        public ICommand ApproximateClick { get; }
        public ICommand GenerateClick { get; }
        public ICommand DoStuffClick { get; }

        #endregion

        #region Public Properties

        // File
        public string TrainFileName
        {
            get => _trainFileName;
            private set
            {
                if (_trainFileName == value)
                    return;

                _trainFileName = value;
                OpenInTextEditorClick.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public string TestFileName
        {
            get => _testFileName;
            private set
            {
                if ( _testFileName == value)
                    return;

                _testFileName = value;
                OpenInTextEditorClick.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        // Plot
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

        public bool Flatten { get; set; } = false;
        public int AnimationStep { get; set; } = 20;

        // Netwok Settings
        public double LerningRate { get; set; } = 0.05;
        public double Momentum { get; set; } = 0.5;
        public double Alfa { get; set; } = 1.0;
        public int EpochsNumber { get; set; } = 500;
        public int NeuronNumber { get; set; } = 10;
        public int NeighbourNumber { get; set; } = 4;

        #endregion

        #region Methods

        //TODO refactor
        public void Generate()
        {
            _approximation.SamplePoints = SampleRepository.GetInputSamplePoints();
            SetUpNetworkParameters();
            _approximation.GenerateCentroids();
            _approximation.AssignSamplePointsToNearestCentroids();
            _plotModelViewModel.ShowGeneratedCentroids( _approximation.SamplePoints, _approximation.Centroids,
                Flatten );
        }

        //TODO refactor
        public void ShowPlotFromDataWithoutLearning()
        {
            _approximation.SamplePoints = SampleRepository.GetInputSamplePoints();
            _approximation.AssignSamplePointsToNearestCentroids();
            _approximation.DoStuffer();

            _plotModelViewModel.SetUpPlotModelData( _approximation );
            _errorPlotModelViewModel.SetUpPlotModelData( _approximation.TotalErrors, _approximation.TotalTestErrors );
            _plotModelViewModel.PlotModel.InvalidatePlot( true );
            _errorPlotModelViewModel.PlotModel.InvalidatePlot( true );
            foreach (var ax in _plotModelViewModel.PlotModel.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
            foreach (var ax in _errorPlotModelViewModel.PlotModel.Axes)
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
        }

        #endregion

        #region Private Members

        // Variables
        private readonly Approximation _approximation;

        private string _trainFileName;
        private string _testFileName;
        private string _pathToFile;

        private readonly PlotViewModel _plotModelViewModel;
        private readonly ErrorPlotViewModel _errorPlotModelViewModel;

        private DispatcherTimer _timer;
        private int _iterator;

        private void OpenTrainFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "TextFiles (*.txt)|*.txt",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _pathToFile = openFileDialog.FileName;
                TrainFileName = openFileDialog.FileName.Split( '\\' ).Last();
                try
                {
                    SampleRepository.TrainSamples = FileReader.ReadFromFile( openFileDialog.FileName );
                    _plotModelViewModel.PlotModel.Series.Clear();
                    _plotModelViewModel.ShowSamples( Flatten );
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

        private void OpenTestFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "TextFiles (*.txt)|*.txt",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _pathToFile = openFileDialog.FileName;
                TestFileName = openFileDialog.FileName.Split( '\\' ).Last();
                try
                {
                    SampleRepository.TestSamples = FileReader.ReadFromFile( openFileDialog.FileName );
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

        //Methods
        private void AproximateAndPlot()
        {
            SetUpNetworkParameters();
            _approximation.CalculateBetaForEachCentroid( Alfa );
            _iterator = 0;
            _approximation.SetupAproximationAlgorithm();
            StartLearningProcess();
        }

        private void StartLearningProcess()
        {
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds( 0 )};
            _timer.Tick += AutoLearningTicker;
            _timer.Start();
        }

        private void AutoLearningTicker( object sender, EventArgs e )
        {
            _iterator += AnimationStep;
            if (_iterator >= EpochsNumber)
                _timer.Stop();

            for (int i = 0; i < AnimationStep; i++)
                _approximation.Learn();
            DrawPlot();
            DrawErrorPlot();
        }

        private void DrawPlot()
        {
            _plotModelViewModel.SetUpPlotModelData( _approximation );
            _plotModelViewModel.PlotModel.InvalidatePlot( true );
            foreach ( var ax in _plotModelViewModel.PlotModel.Axes )
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
        }

        private void DrawErrorPlot()
        {
            _errorPlotModelViewModel.SetUpPlotModelData( _approximation.TotalErrors, _approximation.TotalTestErrors );
            _errorPlotModelViewModel.PlotModel.InvalidatePlot( true );
            foreach ( var ax in _errorPlotModelViewModel.PlotModel.Axes )
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
        }

        private void SetUpNetworkParameters()
        {
            _approximation.LearningRate = LerningRate;
            _approximation.Momentum = Momentum;
            _approximation.HiddenNeuronsNumber = NeuronNumber;
            _approximation.NeighbourNumber = NeighbourNumber;
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