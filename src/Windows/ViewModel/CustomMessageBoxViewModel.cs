using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BirthdayNotifications.Windows.ViewModel {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  class CustomMessageBoxViewModel : INotifyPropertyChanged {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public ICommand CopyMessageTextCommand {
      get; set;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public CustomMessageBoxViewModel() {
      CopyMessageTextCommand ??= new RoutedCommand();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="propertyName"></param>
    private void OnPropertyChanged([CallerMemberName][AllowNull] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
