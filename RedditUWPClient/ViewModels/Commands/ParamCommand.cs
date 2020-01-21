﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace RedditUWPClient.ViewModels.Commands
{
    public class ParamCommand<T> : ICommand
    {
        Action<T> _action;

        internal ParamCommand(Action<T> action)
        {
            _action = action;
        }

#pragma warning disable
        public event EventHandler CanExecuteChanged; //Is part of the interface
#pragma warning restore

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke((T)parameter);
        }
    }
    
}
