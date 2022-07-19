using System;
using System.Windows.Input;

namespace EmployeesList
{
    public class RelayCommand : ICommand
    {
        private Action? mAction;
        private Action<object>? mActionWithParam;
        public event EventHandler? CanExecuteChanged;
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public RelayCommand(Action<object> action)
        {
            mActionWithParam = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if(parameter == null && mAction != null)
            {
                mAction();
            }
            else if(parameter != null && mActionWithParam != null)
            {
                mActionWithParam(parameter);
            }
        }
    }
}
