using System;
using System.Windows.Input;

namespace WPFEcommerceApp {
    /// <summary>
    /// A base command that runs an Action
    /// </summary>
    public class ImmediateCommand<T> : ICommand {
        private readonly Action<T> _execute;

        public ImmediateCommand(Action<T> execute) {
            if(execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
        }

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter = null) {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
