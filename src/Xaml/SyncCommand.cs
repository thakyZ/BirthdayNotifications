using System;
using System.Windows.Input;

namespace BirthdayNotifications.Xaml {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  public class SyncCommand : ICommand {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private readonly Action<object> _command;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="command"></param>
    public SyncCommand(Action<object> command) {
      _command = command;
      _canExecute = () => true;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="command"></param>
    /// <param name="canExecute"></param>
    public SyncCommand(Action<object> command, Func<bool> canExecute) {
      _command = command;
      _canExecute = canExecute;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter) {
      return _canExecute();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object? parameter) {
      if (parameter is not null) {
        _command(parameter);
      }
    }
  }
}
