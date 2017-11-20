using System;
using System.Windows.Input;

namespace iDentalManager.Commands
{
    public class RelayCommand : ICommand
    {
        readonly Action _execute;
        readonly Func<bool> _canexecute;

        public RelayCommand(Action execute, Func<bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canexecute = canexecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canexecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canexecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter)
        {
            return _canexecute == null ? true : _canexecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
