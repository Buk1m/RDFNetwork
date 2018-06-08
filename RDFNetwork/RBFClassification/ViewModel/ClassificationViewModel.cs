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
using RDFNetwork.RBFClassification.Model;

namespace RDFNetwork.RBFClassification.ViewModel
{
	public class ClassificationViewModel : RBFViewModel
	{
		#region Constructor
		public ClassificationViewModel()
		{
			_classification = new Classification();
			OpenTrainFileClick = new RelayCommand( OpenTrainFile );
			OpenTestFileClick = new RelayCommand( OpenTestFile );
			ApproximateClick = new RelayCommand( Aproximate );
			DoStuffClick = new RelayCommand( DoStuff );
			DataRangeX = new RelayCommand(()=>SelectedDataRange = 0);
			DataRangeY = new RelayCommand(()=>SelectedDataRange = 1);
			DataRangeZ = new RelayCommand(()=>SelectedDataRange = 2);
			Kmeans = new RelayCommand(KmeansClick);
			SomeButtonJustToldMeTheWorldIsGonnaRollme = new RelayCommand( SomeButtonJustToldMeTheWorldIsGonnaRollmeClick);
		}
		#endregion

		#region Commands

		public ICommand OpenTrainFileClick { get; }
		public ICommand OpenTestFileClick { get; }
		public ICommand ApproximateClick { get; }
		public ICommand DoStuffClick { get; }
		public ICommand DataRangeX { get; }
		public ICommand DataRangeY { get; }
		public ICommand DataRangeZ { get; }
		public ICommand Kmeans { get; }
		public ICommand SomeButtonJustToldMeTheWorldIsGonnaRollme { get; }

		#endregion

		#region Public Properties
		public string TestFileName
		{
			get => _testFileName;
			private set
			{
				if ( _testFileName == value )
					return;

				_testFileName = value;
				OpenInTextEditorClick.RaiseCanExecuteChanged();
				OnPropertyChanged();
			}
		}

		public int SelectedPlot
		{
			get => _selectedPlot;
			set
			{
				if ( _selectedPlot == value )
					return;

				_selectedPlot = value;
				DrawPlot();
				OnPropertyChanged();
			}
		}

		public int SelectedDataRange
		{
			get => _selectedDataRange;
			set
			{
				if ( _selectedDataRange == value )
					return;

				_selectedDataRange = value;
				DrawPlot();
				OnPropertyChanged();
			}
		}

		public PlotModel PlotModel
		{
			get => _plotModelViewModel.PlotModel;
			internal set => _plotModelViewModel.PlotModel = value;
		}

		#endregion

		#region Private Members

		// Variables
		private readonly Classification _classification;
		private readonly PlotViewModel _plotModelViewModel = new PlotViewModel();

		private int _selectedPlot;
		private int _selectedDataRange;
		private string _testFileName;
		private DispatcherTimer _timer;

		// Methods
		private void OpenTrainFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
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

			if ( openFileDialog.ShowDialog() == true )
			{
				_pathToFile = openFileDialog.FileName;
				TestFileName = openFileDialog.FileName.Split( '\\' ).Last();
				try
				{
					SampleRepository.TestSamples = FileReader.ReadFromFile( openFileDialog.FileName );
				}
				catch ( FileNotFoundException fnfe )
				{
					MessageBox.Show( fnfe.Message );
				}
				catch ( FormatException fe )
				{
					MessageBox.Show( fe.Message );
				}
			}
		}

		private void Aproximate()
		{
			_classification.CalculateBetaForEachCentroid(Alfa);

			Centroid.ResetNextId();
			
			_classification.SetUpClassificationAlgorithm();
			for (int i = 0; i < EpochsNumber; i++)
			{
				_classification.Learn();
			}
		}

		private void DoStuff()
		{
			_classification.SamplePoints = SampleRepository.GetInputSamplePoints4D();
			_classification.DoStuffer();
			DrawPlot();
			DrawErrorPlot();
		}

		private void DrawPlot()
		{
			_plotModelViewModel.SetUpPlotModelData( _classification, SelectedPlot, SelectedDataRange );
			_plotModelViewModel.PlotModel.InvalidatePlot( true );
			foreach ( var ax in _plotModelViewModel.PlotModel.Axes )
				ax.Maximum = ax.Minimum = Double.NaN;
			PlotModel.ResetAllAxes();
		}

		private void DrawErrorPlot()
		{
			_errorPlotModelViewModel.SetUpPlotModelData( _classification.TotalErrors, _classification.TotalTestErrors );
			_errorPlotModelViewModel.PlotModel.InvalidatePlot( true );
			foreach ( var ax in _errorPlotModelViewModel.PlotModel.Axes )
				ax.Maximum = ax.Minimum = Double.NaN;
			PlotModel.ResetAllAxes();
		}

		private void SetUpNetworkParameters()
		{
			_classification.LearningRate = LerningRate;
			_classification.Momentum = Momentum;
			_classification.HiddenNeuronsNumber = NeuronNumber;
			_classification.NeighbourNumber = NeighbourNumber;
		}

		private void SomeButtonJustToldMeTheWorldIsGonnaRollmeClick()
		{
			_classification.SamplePoints = SampleRepository.GetInputSamplePoints4D();
			SetUpNetworkParameters();
			_classification.GenerateCentroids();
			DrawPlot();
		}

		private void KmeansClick()
		{
			StartLearningProcess();
			_classification.Centroids = _classification.KMeansAlgorithm.Centroids;
			_classification.SamplePoints = _classification.KMeansAlgorithm.SamplePoints;
		}

		private void StartLearningProcess()
		{
			_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 10 ) };
			_timer.Tick += AutoLearningTicker;
			_timer.Start();
		}

		private void AutoLearningTicker( object sender, EventArgs e )
		{
			if ( _classification.KMeansAlgorithm.MaxCentroidShift <= 0.001 )
			{
				_timer.Stop();
			}

			_classification.KMeansAlgorithm.AssignSamplePointsToNearestCentroids();
			_classification.KMeansAlgorithm.UpdateCentroidsCoordinates();
			DrawPlot();
		}

		#endregion
	}
}