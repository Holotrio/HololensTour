using System;
using System.Windows.Input;

namespace TourBackendUI
{
    public class RelayCommand : ICommand
    {
        private readonly Action _a;

        public RelayCommand(Action a)
        {
            this._a = a;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _a();
        }

        public event EventHandler CanExecuteChanged;
    }
}