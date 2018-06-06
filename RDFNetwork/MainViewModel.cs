using System.Windows.Input;

namespace RDFNetwork
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            _taskName = "Choose Task";
        }

        public string TaskName
        {
            get => _taskName;
            set
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }

        public ICommand ApproximationClick { get; }

        private string _taskName;
    }
}