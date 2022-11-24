using System;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    /// <summary>
    /// A base command that runs an Action
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    return _canExecute == null ? true : _canExecute((T)parameter);
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _execute((T)parameter);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
