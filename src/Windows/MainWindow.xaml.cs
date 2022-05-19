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

using BirthdayNotifications.Config;
using BirthdayNotifications.Utils;
using BirthdayNotifications.Windows.ViewModel;

using Serilog;

namespace BirthdayNotifications {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {
    /// <summary>
    /// 
    /// </summary>
    private MainWindowViewModel Model => this.DataContext as MainWindowViewModel;

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
    private void SetDefaults() {

      // Clean up invalid addons
      if (App.Settings.BirthdayUsers.Count < 1 || (App.Settings.BirthdayUsers.Count == 1 && App.Settings.BirthdayUsers[0] == BirthdayUser.Default))
        App.Settings.BirthdayUsers = App.Settings.BirthdayUsers.Where(x => x.BirthdayUnix != 0).ToList();

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
      var fakeStartMenuItem = new MenuItem
            {
        Header = "Fake start"
      };
      fakeStartMenuItem.Click += FakeStart_OnClick;
#endif

      this.SetDefaults();

      Log.Information("MainWindow initialized.");

      Show();
      Activate();
    }

    private void FakeStart_OnClick(object sender, RoutedEventArgs e) {
      Log.Information("FakeStart Clicked.");
    }
  }
}
