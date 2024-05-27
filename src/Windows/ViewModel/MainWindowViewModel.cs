using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using BirthdayNotifications.Config;
using BirthdayNotifications.Utils;

using MaterialDesignThemes.Wpf;

using Serilog;

namespace BirthdayNotifications.Windows.ViewModel {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  class MainWindowViewModel : INotifyPropertyChanged {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private readonly MainWindow _window;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public Action Activate { get; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="window"></param>
    public MainWindowViewModel(MainWindow window) {
      _window = window;

      BirthdayUserList = LoadUserList();
      _window.Users.SelectedIndex = 0;

      Activate ??= new Action(() => Log.Warning("A MainWindowViewModel class was implemented without an Activate action specified."));
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static ListBoxItem CreateListBoxItem(string name) {
      var listBoxItem = new ListBoxItem() {
        Content = $"{name}",
        HorizontalContentAlignment = HorizontalAlignment.Left,
        VerticalContentAlignment = VerticalAlignment.Center
      };
      listBoxItem.SetValue(ListBoxItemAssist.ShowSelectionProperty, true);
      return listBoxItem;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    internal void UpdateSelectedBirthdayUser() {
      CurrentUser_IsEnabled = _window.CurrentlySelected_BirthdayUser.Item2.Enabled;
      CurrentUser_Name = _window.CurrentlySelected_BirthdayUser.Item2.Name;
      CurrentUser_BirthDate = _window.CurrentlySelected_BirthdayUser.Item2.BirthdayUnix.Date;
      SetCurrentAvatar(_window.CurrentlySelected_BirthdayUser.Item2.UserAvatar);
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="avatar"></param>
    private void SetCurrentAvatar(Avatar? avatar) {
      if (avatar is null) {
        return;
      }

      var bi3 = new BitmapImage();
      bi3.BeginInit();
      bi3.UriSource = Birthdays.GetUserAvatar(avatar);
      bi3.EndInit();
      CurrentUser_Avatar = bi3;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    internal void OnWindowClosed(object? sender, EventArgs args) {
      Application.Current.Shutdown();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    internal void OnWindowClosing(object? sender, CancelEventArgs args) {
      // Method intentionally left empty.
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

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private ObservableCollection<ListBoxItem> _birthdayUserList = new();
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public ObservableCollection<ListBoxItem> BirthdayUserList {
      get => _birthdayUserList;
      set {
        _birthdayUserList = value;
        OnPropertyChanged(nameof(BirthdayUserList));
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private bool _currentUser_isEnabled;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public bool CurrentUser_IsEnabled {
      get => _currentUser_isEnabled;
      set {
        _currentUser_isEnabled = value;
        OnPropertyChanged(nameof(CurrentUser_IsEnabled));
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private string _currentUser_name = string.Empty;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public string CurrentUser_Name {
      get => _currentUser_name;
      set {
        _currentUser_name = value;
        OnPropertyChanged(nameof(CurrentUser_Name));
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private DateTime _currentUser_birthDate;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public DateTime CurrentUser_BirthDate {
      get => _currentUser_birthDate;
      set {
        _currentUser_birthDate = value;
        OnPropertyChanged(nameof(CurrentUser_BirthDate));
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private ImageSource? _currentUser_avatar;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public ImageSource? CurrentUser_Avatar {
      get => _currentUser_avatar;
      set {
        _currentUser_avatar = value;
        OnPropertyChanged(nameof(CurrentUser_Avatar));
      }
    }
  }
}
