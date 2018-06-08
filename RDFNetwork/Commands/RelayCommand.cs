using System;
using System.Windows.Input;

namespace RDFNetwork.Commands
{
    public class RelayCommand : ICommand
    {
        #region Constructors

        public RelayCommand( Action execute ) : this( execute, null )
        {
        }

        public RelayCommand( Action execute, Func<bool> canExecute )
        {
            _execute = execute ?? throw new ArgumentNullException( nameof(execute) );
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand

        public bool CanExecute( object parameter )
        {
            if (_canExecute == null)
                return true;
            if (parameter == null)
                return _canExecute();
            return _canExecute();
        }

        public virtual void Execute( object parameter )
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        #region API

        internal void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke( this, EventArgs.Empty );
        }

        #endregion

        #region Private

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        #endregion
    }
}