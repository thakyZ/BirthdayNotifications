using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
