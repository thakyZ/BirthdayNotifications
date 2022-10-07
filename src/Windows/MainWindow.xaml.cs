using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

using BirthdayNotifications.Config;
using BirthdayNotifications.Utils;
using BirthdayNotifications.Windows.ViewModel;

using MaterialDesignThemes.Wpf;

using Serilog;

using Windows.Networking.NetworkOperators;

using static System.Net.Mime.MediaTypeNames;

namespace BirthdayNotifications {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    /// <summary>
    ///
    /// </summary>
#pragma warning disable CS8603 // Possible null reference return.
    private MainWindowViewModel Model => DataContext as MainWindowViewModel;
#pragma warning restore CS8603 // Possible null reference return.

    /// <summary>
    ///
    /// </summary>
    internal (int, BirthdayUser) CurrentlySelected_BirthdayUser = (0, BirthdayUser.Default);

    /// <summary>
    ///
    /// </summary>
    public MainWindow() {
      InitializeComponent();

      this.DataContext = new MainWindowViewModel(this);

      Closed += Model.OnWindowClosed;
      Closing += Model.OnWindowClosing;

      Title += " v" + AppUtils.GetAssemblyVersion();
      Title += " " + AppUtils.GetGitHash();

#if DEBUG
      Title += " - Debugging";
#endif
    }

    /// <summary>
    ///
    /// </summary>
    private const int CURRENT_VERSION_LEVEL = 0;

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static void SetDefaults() {

      // Clean up invalid addons
      if (App.Settings.BirthdayUsers.Count < 1 || (App.Settings.BirthdayUsers.Count == 1 && App.Settings.BirthdayUsers[0] == BirthdayUser.Default))
        App.Settings.BirthdayUsers = App.Settings.BirthdayUsers.Where(x => x.BirthdayUnix.ToUnixTimeMilliseconds() != 0).ToList();

      var versionLevel = App.Settings.ConfigVersion.GetValueOrDefault(0);

      while (versionLevel < CURRENT_VERSION_LEVEL) {
        // If not newer version
        /*switch (versionLevel) {
          case 0:
            try {
            } catch (Exception ex) {
            }

            break;

          default:
            throw new ArgumentOutOfRangeException();
        }*/

        versionLevel++;
      }

      App.Settings.ConfigVersion = versionLevel;
    }

    /// <summary>
    ///
    /// </summary>
    public void Initialize() {
#if DEBUG
      var fakeStartMenuItem = new MenuItem {
        Header = "Fake start"
      };
      fakeStartMenuItem.Click += FakeStart_OnClick;
#endif

      SetDefaults();

      Log.Information("MainWindow initialized.");

      Show();
      Activate();
    }

    private static void ChangeUserOption(int index, string property, object value) {
      BirthdayUser user = BirthdayUser.Default;
      if (index == 0) {
        user = App.Settings.OwnBirthday;
      } else {
        try {
          user = App.Settings.BirthdayUsers[index - 1];
        } catch (Exception e) {
          Log.Error($"Could not edit birthday of user by index of: {index - 1}");
          Log.Error(e.Message);
          Log.Error(e.StackTrace);
        }
      }
      switch (property) {
        case "Enabled":
          user.Enabled = (bool)value;
          break;
        case "Name":
          user.Name = (string)value;
          break;
        case "Birthday":
          if (value is not null) {
            user.BirthdayUnix = new DateTimeOffset((DateTime)value);
          }
          break;
        case "Avaatar":
          break;
        default:
          throw new InvalidOperationException("property variable did not get assigned correctly.");
      }
    }

    private void AnyListBox_Selected(object sender, RoutedEventArgs e) {
      var previousIndex = CurrentlySelected_BirthdayUser.Item1;
      if (previousIndex > App.Settings.BirthdayUsers.Count + 1) {
        previousIndex = App.Settings.BirthdayUsers.Count - 1;
      }
      var index = ((ListBox)sender).SelectedIndex;
      if (index == 0) {
        CurrentlySelected_BirthdayUser = (0, App.Settings.OwnBirthday);
      } else if (index == unchecked((int)0xffffffff)) {
        var birthdayUser = App.Settings.BirthdayUsers[previousIndex - 2];
        CurrentlySelected_BirthdayUser = (previousIndex - 1, birthdayUser);
      } else {
        var birthdayUser = App.Settings.BirthdayUsers[index - 1];
        CurrentlySelected_BirthdayUser = ((index), birthdayUser);
      }
      Model.UpdateSelectedBirthdayUser();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FakeStart_OnClick(object sender, RoutedEventArgs e) {
      Log.Information("FakeStart Clicked.");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveButton_OnClick(object sender, RoutedEventArgs e) {
      App.Settings.SaveConfig();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddUserButton_OnClick(object sender, RoutedEventArgs e) {
      var tempBirthdayUser = BirthdayUser.Default;
      App.Settings.BirthdayUsers.Add(tempBirthdayUser);
      Model.UpdateUserList();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SubUserButton_OnClick(object sender, RoutedEventArgs e) {
      if (CurrentlySelected_BirthdayUser.Item1 == 0) {
        return;
      }
      var SelectedIndex = CurrentlySelected_BirthdayUser.Item1;
      App.Settings.BirthdayUsers.RemoveAt(SelectedIndex - 1);
      SelectedIndex = SelectedIndex - 1;

      Model.UpdateUserList();
      var MouseDevice = Mouse.PrimaryDevice;
      Users.SelectedIndex = SelectedIndex;
      ((ListBoxItem)Users.Items[SelectedIndex]).RaiseEvent(new MouseButtonEventArgs(MouseDevice, new TimeSpan(DateTime.Now.Ticks).Milliseconds, MouseButton.Left) { RoutedEvent = UIElement.MouseLeftButtonDownEvent, Source = Users });
      ((ListBoxItem)Users.Items[SelectedIndex]).IsSelected = true;
      Users.ScrollIntoView(SelectedIndex);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IsEnabledCheckbox_OnClick(object sender, RoutedEventArgs e) {
      ChangeUserOption(CurrentlySelected_BirthdayUser.Item1, "Enabled", ((CheckBox)sender).IsChecked);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
      ChangeUserOption(CurrentlySelected_BirthdayUser.Item1, "Name", ((TextBox)sender).Text);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BirthdayDatePicker_OnSelectedDatedChanged(object sender, RoutedEventArgs e) {
      ChangeUserOption(CurrentlySelected_BirthdayUser.Item1, "Birthday", ((DatePicker)sender).SelectedDate);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UserAvatarUpload_Click(object sender, RoutedEventArgs e) {

    }

        private void Search_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
