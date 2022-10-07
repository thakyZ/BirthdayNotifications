﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BirthdayNotifications.Xaml {
  public class SyncCommand : ICommand {
    private readonly Action<object> _command;
    private readonly Func<bool> _canExecute;

    public SyncCommand(Action<object> command) {
      _command = command;
      _canExecute = () => true;
    }

    public SyncCommand(Action<object> command, Func<bool> canExecute) {
      _command = command;
      _canExecute = canExecute;
    }

#nullable enable
    public event EventHandler? CanExecuteChanged {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }
#nullable disable

#nullable enable
    public bool CanExecute(object? parameter) {
      return _canExecute();
    }
#nullable disable

#nullable enable
    public void Execute(object? parameter) {
      if (parameter is not null) {
        _command(parameter);
      }
    }
#nullable disable
  }
}
