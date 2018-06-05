using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OxyPlot;
using RDFNetwork.Commands;
using RDFNetwork.RBFClassification.Model;

namespace RDFNetwork.RBFClassification.ViewModel
{
    public class ClassificationViewModel : BaseViewModel
    {
        public ClassificationViewModel()
        {
            OpenFileClick = new RelayCommand( OpenFile );
            OpenInTextEditorClick =
                new RelayCommand( OpenInTextEditor, () => _pathToFile != null && _pathToFile.Any() );
            ApproximateClick = new RelayCommand( Aproximate );
            DoStuffClick = new RelayCommand( DoStuff );
            _plotModelViewModel = new PlotViewModel();
            _errorPlotModelViewModel = new ErrorPlotViewModel();
        }

        #region Commands

        public ICommand OpenFileClick { get; }
        public RelayCommand OpenInTextEditorClick { get; }
        public ICommand ApproximateClick { get; }
        public ICommand DoStuffClick { get; }

        #endregion

        #region Public Properties

        public string FileName
        {
            get => _fileName;
            private set
            {
                if ( _fileName == value )
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

        #endregion


        #region Private Members

        private string _fileName;

        private string _pathToFile;

        private readonly PlotViewModel _plotModelViewModel;
        private readonly ErrorPlotViewModel _errorPlotModelViewModel;

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TextFiles (*.txt)|*.txt";
            openFileDialog.RestoreDirectory = true;

            if ( openFileDialog.ShowDialog() == true )
            {
                _pathToFile = openFileDialog.FileName;
                FileName = openFileDialog.FileName.Split( '\\' ).Last();
                try
                {
                    SampleRepository.TrainSamples = FileReader.ReadFromFile( openFileDialog.FileName );
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

        private Classification _classification;

        private void Aproximate()
        {
            _classification = new Classification( NeuronNumber, Alfa, EpochsNumber );

            Centroid1D._nextId = 0;
            _classification.StuffDooer();
        }

        private void DoStuff()
        {
            _classification.SamplePoints = SampleRepository.GetInputSamplePoints4D();
            List<double> outputsList = _classification.DoStuffer();
            _plotModelViewModel.SetUpPlotModelData( _classification.Centroids, _classification.SamplePoints, outputsList, _classification );
            _errorPlotModelViewModel.SetUpPlotModelData( _classification.TotalErrors );
            _plotModelViewModel.PlotModel.InvalidatePlot( true );
            _errorPlotModelViewModel.PlotModel.InvalidatePlot( true );
            foreach ( var ax in _plotModelViewModel.PlotModel.Axes )
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
            foreach ( var ax in _errorPlotModelViewModel.PlotModel.Axes )
                ax.Maximum = ax.Minimum = Double.NaN;
            PlotModel.ResetAllAxes();
        }

        private void OpenInTextEditor()
        {
            try
            {
                System.Diagnostics.Process.Start( "notepad++.exe", _pathToFile );
            }
            catch ( Win32Exception )
            {
                System.Diagnostics.Process.Start( "explorer.exe", _pathToFile );
            }
        }

        #endregion

    }
}