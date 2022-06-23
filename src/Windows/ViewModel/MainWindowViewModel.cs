using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using BirthdayNotifications.Config;
using BirthdayNotifications.Utils;

using MaterialDesignThemes.Wpf;

namespace BirthdayNotifications.Windows.ViewModel {
  class MainWindowViewModel : INotifyPropertyChanged {
    private readonly MainWindow _window;
    public Action Activate;

    public MainWindowViewModel(MainWindow window) {
      _window = window;

      BirthdayUserList = LoadUserList();
      _window.Users.SelectedIndex = 0;
    }

    private static ObservableCollection<ListBoxItem> LoadUserList() {
      var listBoxItems = new ObservableCollection<ListBoxItem> {
        CreateListBoxItem("YOU")
      };
      if (App.Settings.BirthdayUsers.Count > 0) {
        foreach (BirthdayUser birthdayUser in App.Settings.BirthdayUsers) {
          listBoxItems.Add(CreateListBoxItem($"{birthdayUser.Name}"));
        }
      }
      return listBoxItems;
    }

    private static ListBoxItem CreateListBoxItem(string name) {
      var listBoxItem = new ListBoxItem() {
        Content = $"{name}",
        HorizontalContentAlignment = HorizontalAlignment.Left,
        VerticalContentAlignment = VerticalAlignment.Center
      };
      listBoxItem.SetValue(ListBoxItemAssist.ShowSelectionProperty, true);
      return listBoxItem;
    }

    internal void UpdateSelectedBirthdayUser() {
      CurrentUser_IsEnabled = _window.CurrentlySelected_BirthdayUser.Item2.Enabled;
      CurrentUser_Name = _window.CurrentlySelected_BirthdayUser.Item2.Name;
      CurrentUser_BirthDate = _window.CurrentlySelected_BirthdayUser.Item2.BirthdayUnix.Date;
      SetCurrentAvatar(_window.CurrentlySelected_BirthdayUser.Item2.UserAvatar);
    }

    private void SetCurrentAvatar(Avatar avatar) {
      var bi3 = new BitmapImage();
      bi3.BeginInit();
      bi3.UriSource = Birthdays.GetUserAvatar(avatar);
      bi3.EndInit();
      CurrentUser_Avatar = bi3;
    }

    internal void OnWindowClosed(object sender, EventArgs args) {
      Application.Current.Shutdown();
    }

    internal void OnWindowClosing(object sender, CancelEventArgs args) {
      // Method intentionally left empty.
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName][AllowNull] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ObservableCollection<ListBoxItem> _birthdayUserList;
    public ObservableCollection<ListBoxItem> BirthdayUserList {
      get => _birthdayUserList;
      set {
        _birthdayUserList = value;
        OnPropertyChanged(nameof(BirthdayUserList));
      }
    }
    private bool _currentUser_isEnabled;
    public bool CurrentUser_IsEnabled {
      get => _currentUser_isEnabled;
      set {
        _currentUser_isEnabled = value;
        OnPropertyChanged(nameof(CurrentUser_IsEnabled));
      }
    }
    private string _currentUser_name;
    public string CurrentUser_Name {
      get => _currentUser_name;
      set {
        _currentUser_name = value;
        OnPropertyChanged(nameof(CurrentUser_Name));
      }
    }
    private DateTime _currentUser_birthDate;
    public DateTime CurrentUser_BirthDate {
      get => _currentUser_birthDate;
      set {
        _currentUser_birthDate = value;
        OnPropertyChanged(nameof(CurrentUser_BirthDate));
      }
    }
    private ImageSource _currentUser_avatar;
    public ImageSource CurrentUser_Avatar {
      get => _currentUser_avatar;
      set {
        _currentUser_avatar = value;
        OnPropertyChanged(nameof(CurrentUser_Avatar));
      }
    }
  }
}
