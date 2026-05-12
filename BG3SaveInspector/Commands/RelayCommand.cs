using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BG3SaveInspector.Commands
{
    public class RelayCommand : ICommand
    {
        // use Action delegate to pass along the work without needing to know what it is
        private readonly Action _execute;

        // enable/disable button
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func <bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // event accessor, allows command manager to handle button states
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value; 
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) =>_execute();
        
    }
}
