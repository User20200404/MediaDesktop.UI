using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaDesktop.UI.ViewModels
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class DelegateCommand : IDelegateCommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;
        public event EventHandler CanExecuteChanged;
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
        }






        public bool CanExecute(object parameter)
        {
            if(canExecute is not null)
            {
                return canExecute.Invoke(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            execute.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
