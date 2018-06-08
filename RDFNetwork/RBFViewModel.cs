using System.ComponentModel;
using System.Linq;
using OxyPlot;
using RDFNetwork.Commands;

namespace RDFNetwork
{
    public class RBFViewModel : BaseViewModel
    {

        #region Constructor
        public RBFViewModel()
        {
            OpenInTextEditorClick = new RelayCommand( OpenInTextEditor,
                () => _pathToFile != null && _pathToFile.Any() );
        }
        #endregion

        #region Commands
        public RelayCommand OpenInTextEditorClick { get; }
        #endregion

        #region Public Properties

        // File
        public string TrainFileName
        {
            get => _trainfileName;
            internal set
            {
                if ( _trainfileName == value )
                    return;

                _trainfileName = value;
                OpenInTextEditorClick.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        // Plot
        public PlotModel ErrorPlotModel
        {
            get => _errorPlotModelViewModel.PlotModel;
            internal set => _errorPlotModelViewModel.PlotModel = value;
        }

        // Network Settings
        public double LerningRate { get; set; } = 0.05;
        public double Momentum { get; set; } = 0.5;
        public double Alfa { get; set; } = 1;
        public int EpochsNumber { get; set; } = 500;
        public int NeuronNumber { get; set; } = 10;
        public int NeighbourNumber { get; set; } = 4;

        #endregion

        #region Private
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

        #region Protected
        protected string _trainfileName;
        protected string _pathToFile;
        protected readonly ErrorPlotViewModel _errorPlotModelViewModel = new ErrorPlotViewModel(); 
        #endregion

    }
}