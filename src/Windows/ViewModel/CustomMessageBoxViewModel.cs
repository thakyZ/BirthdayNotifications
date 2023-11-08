using System.Windows.Input;

namespace BirthdayNotifications.Windows.ViewModel {
  class CustomMessageBoxViewModel {
    public ICommand CopyMessageTextCommand {
      get; set;
    }

    public CustomMessageBoxViewModel() {
      CopyMessageTextCommand ??= new RoutedCommand();
    }
  }
}
