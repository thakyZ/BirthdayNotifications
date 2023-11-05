using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

using BirthdayNotifications.Config;
using BirthdayNotifications.Utils;
using BirthdayNotifications.Windows;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace BirthdayNotifications {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
    /// <summary>
    /// Specifies the application's repository url.
    /// </summary>
    public const string REPO_URL = "https://github.com/thakyZ/BirthdayNotifications"; // "${RepositoryUrl}";

    /// <summary>
    /// Static instance of the BirthdayNotifSettings class.
    /// </summary>
    [AllowNull]
    [NotNull]
    internal static Settings Settings { get; private set; }

    /// <summary>
    /// Instance of the MainWindow class.
    /// </summary>
    private static MainWindow? _mainWindow { get; set; }

    /// <summary>
    /// Static instance of the Cache class.
    /// </summary>
    [AllowNull]
    [NotNull]
    internal static Cache Cache { get; private set; }

    /// <summary>
    /// CommandLine Option
    /// </summary>
    private bool CheckOnly {
      get; set;
    } = false;

    /// <summary>
    /// CommandLine Option
    /// </summary>
    private bool NoClose {
      get; set;
    } = false;

    /// <summary>
    /// CommandLine Option
    /// </summary>
    private bool None {
      get; set;
    } = false;

    /// <summary>
    /// The program's main function
    /// </summary>
    public App() {
      foreach (var arg in Environment.GetCommandLineArgs()) {
        if (arg.StartsWith("--check", StringComparison.Ordinal) ) {
          CheckOnly = true;
        }
        if (arg.StartsWith("--noClose", StringComparison.Ordinal) && CheckOnly) {
          NoClose = true;
        }
        var Embedding = false;
        if (arg.StartsWith("-Embedding", StringComparison.Ordinal) && !CheckOnly && !NoClose) {
          Embedding = true;
        }
        if (arg.StartsWith("-ToastActivated", StringComparison.Ordinal) && Embedding) {
          None = true;
        }
      }

      if (None) {
        Current.Shutdown();
      }

      var release = $"birthdaynotifications-{AppUtils.GetAssemblyVersion()}-{AppUtils.GetGitHash()}";

      try {
        Log.Logger = new LoggerConfiguration()
                     .WriteTo.Async(a => a.File(Path.Combine(AppUtils.GetInstanceDirectory(), "output.log")))
                     .WriteTo.Sink(SerilogEventSink.Instance)
#if DEBUG
                     .WriteTo.Debug()
                     .MinimumLevel.Verbose()
#else
                     .MinimumLevel.Information()
#endif
                     .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
      } catch (Exception ex) {
        MessageBox.Show("Could not set up logging. Please report this error.\n\n" + ex.Message, "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      SerilogEventSink.Instance.LogLine += OnSerilogLogLine;

      try {
        SetupSettings();
      } catch (Exception e) {
        Log.Error(e, "Settings were corrupted, resetting");
        File.Delete(GetConfigPath());
        SetupSettings();
      }

      Log.Information("Birthday Notifications started as {Release}", release);

      if (!EnvironmentVars.BN_DEBUG) {
        if (CheckOnly) {
          Birthdays _birthdays = new Birthdays(() => DoShutdown());
          Cache = new Cache(() => _birthdays.CheckBirthdays());
        } else {
          Cache = new Cache();
        }
      } else {
        Birthdays _birthdays = new Birthdays();
        Cache = new Cache(() => _birthdays.CheckBirthdays());
      }

      if (CheckOnly) {
        _mainWindow = (MainWindow)MainWindow;
      }
    }
    private static void DoShutdown() {
      Current.Shutdown();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnSerilogLogLine(object? sender, (string Line, LogEventLevel Level, DateTimeOffset TimeStamp, Exception? Exception) e) {
      if (e.Exception is null)
        return;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private static void SetupSettings() => Settings = Settings.Load(GetConfigPath());

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="finishUp"></param>
    private void OnUpdateCheckFinished(bool finishUp) {
      Dispatcher.Invoke(() => {
        _useFullExceptionHandler = true;

        if (!finishUp)
          return;

        _mainWindow = new MainWindow();
        if (!EnvironmentVars.BN_DEBUG) {
          if (!CheckOnly) {
            _mainWindow.Initialize();
          }
        } else {
          _mainWindow.Initialize();
        }
      });
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
      if (!e.Observed) {
        EarlyInitExceptionHandler(sender, new UnhandledExceptionEventArgs(e.Exception, true));
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private bool _useFullExceptionHandler = false;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private void EarlyInitExceptionHandler(object? sender, UnhandledExceptionEventArgs e) {
      this.Dispatcher.Invoke(() => {
        Log.Error((Exception)e.ExceptionObject, "Unhandled exception");

        if (_useFullExceptionHandler) {
          _ = CustomMessageBox.Builder
                          .NewFrom((Exception)e.ExceptionObject, "Unhandled", CustomMessageBox.ExitOnCloseModes.ExitOnClose)
                          .WithAppendText("\n\nError during early initialization. Please report this error.\n\n" + e.ExceptionObject)
                          .ShowMessageBox();
        } else {
          _ = MessageBox.Show(
              "Error during early initialization. Please report this error.\n\n" + e.ExceptionObject,
              "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Environment.Exit(-1);
      });
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private static string GetConfigPath() => Path.Combine(AppUtils.GetInstanceDirectory(), "config.json");

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private void App_OnStartup(object? sender, StartupEventArgs e) {
      var check = true;
      Log.Verbose("Loading MainWindow...");

      if (e.Args.Length > 0) {
        foreach (string arg in e.Args) {
          if (arg != "--check") {
            check = false;
          }
        }
      }

      OnUpdateCheckFinished(check);
    }
  }
}