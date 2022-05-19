using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BirthdayNotifications.Windows.ViewModel {
  class MainWindowViewModel : INotifyPropertyChanged {
    private readonly Window _window;
    public Action Activate;
    public Action Hide;
    public Action ReloadHeadlines;

    public MainWindowViewModel(Window window) {
      _window = window;
    }

    public void OnWindowClosed(object sender, object args) {
      Application.Current.Shutdown();
    }

    public void OnWindowClosing(object sender, CancelEventArgs args) {
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
