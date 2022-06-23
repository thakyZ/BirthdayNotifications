using System;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using BirthdayNotifications.Utils;
using BirthdayNotifications.Windows.ViewModel;
using BirthdayNotifications.Xaml;

using MaterialDesignThemes.Wpf;

using Serilog;


namespace BirthdayNotifications.Windows {
  /// <summary>
  /// Interaction logic for CustomMessageBox.xaml
  /// </summary>
  public partial class CustomMessageBox : Window {
    private readonly Builder _builder;
    private MessageBoxResult _result;
#pragma warning disable CS8603 // Possible null reference return.
    private CustomMessageBoxViewModel ViewModel => DataContext as CustomMessageBoxViewModel;
#pragma warning restore CS8603 // Possible null reference return.

    public const string ErrorExplanation = "An error in Birthday Notifications occurred. If this issue persists, please report\r\nit on GitHub by clicking the button below, describing the issue and copying the text in the box.";

    public CustomMessageBox(Builder builder) {
      _builder = builder;
      _result = _builder.CancelResult;

      InitializeComponent();

      DataContext = new CustomMessageBoxViewModel();

      ViewModel.CopyMessageTextCommand = new SyncCommand(p => Clipboard.SetText(_builder.Text));

      if (builder.ParentWindow?.IsVisible ?? false) {
        Owner = builder.ParentWindow;
        ShowInTaskbar = false;
      } else {
        ShowInTaskbar = true;
      }

      Title = builder.Caption;
      MessageTextBlock.Text = builder.Text;

      if (string.IsNullOrWhiteSpace(builder.Description))
        DescriptionTextBox.Visibility = Visibility.Collapsed;
      else {
        DescriptionTextBox.Document.Blocks.Clear();
        DescriptionTextBox.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(builder.Description)));
      }

      switch (builder.Buttons) {
        case MessageBoxButton.OK:
          Button1.Content = builder.OkButtonText ?? "Ok";
          Button2.Visibility = Visibility.Collapsed;
          Button3.Visibility = Visibility.Collapsed;
          _ = (builder.DefaultResult switch {
            MessageBoxResult.OK => Button1,
            _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult).ToString(), builder.DefaultResult, null),
          }).Focus();
          break;
        case MessageBoxButton.OKCancel:
          Button1.Content = builder.OkButtonText ?? "Ok";
          Button2.Content = builder.CancelButtonText ?? "Cancel";
          Button3.Visibility = Visibility.Collapsed;
          _ = (builder.DefaultResult switch {
            MessageBoxResult.OK => Button1,
            MessageBoxResult.Cancel => Button2,
            _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult).ToString(), builder.DefaultResult, null),
          }).Focus();
          break;
        case MessageBoxButton.YesNoCancel:
          Button1.Content = builder.YesButtonText ?? "Yes";
          Button2.Content = builder.NoButtonText ?? "No";
          Button3.Content = builder.CancelButtonText ?? "Cancel";
          _ = (builder.DefaultResult switch {
            MessageBoxResult.Yes => Button1,
            MessageBoxResult.No => Button2,
            MessageBoxResult.Cancel => Button3,
            _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult).ToString(), builder.DefaultResult, null),
          }).Focus();
          break;
        case MessageBoxButton.YesNo:
          Button1.Content = builder.YesButtonText ?? "Yes";
          Button2.Content = builder.NoButtonText ?? "No";
          Button3.Visibility = Visibility.Collapsed;
          _ = (builder.DefaultResult switch {
            MessageBoxResult.Yes => Button1,
            MessageBoxResult.No => Button2,
            _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult).ToString(), builder.DefaultResult, null),
          }).Focus();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(builder.Buttons).ToString(), builder.Buttons, null);
      }

      switch (builder.Image) {
        case MessageBoxImage.None:
          ErrorPackIcon.Visibility = Visibility.Collapsed;
          break;
        case MessageBoxImage.Hand:
          ErrorPackIcon.Visibility = Visibility.Visible;
          ErrorPackIcon.Kind = PackIconKind.Error;
          ErrorPackIcon.Foreground = Brushes.Red;
          SystemSounds.Hand.Play();
          break;
        case MessageBoxImage.Question:
          ErrorPackIcon.Visibility = Visibility.Visible;
          ErrorPackIcon.Kind = PackIconKind.QuestionMarkCircle;
          ErrorPackIcon.Foreground = Brushes.DodgerBlue;
          SystemSounds.Question.Play();
          break;
        case MessageBoxImage.Exclamation:
          ErrorPackIcon.Visibility = Visibility.Visible;
          ErrorPackIcon.Kind = PackIconKind.Warning;
          ErrorPackIcon.Foreground = Brushes.Yellow;
          SystemSounds.Exclamation.Play();
          break;
        case MessageBoxImage.Asterisk:
          ErrorPackIcon.Visibility = Visibility.Visible;
          ErrorPackIcon.Kind = PackIconKind.Information;
          ErrorPackIcon.Foreground = Brushes.DodgerBlue;
          SystemSounds.Asterisk.Play();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(builder.Image).ToString(), builder.Image, null);
      }

      NewGitHubIssueButton.Visibility = builder.ShowNewGitHubIssue ? Visibility.Visible : Visibility.Collapsed;

      Topmost = builder.OverrideTopMostFromParentWindow ? builder.ParentWindow?.Topmost ?? builder.TopMost : builder.TopMost;
    }

    protected override void OnKeyDown(KeyEventArgs e) {
      if (e.Key == Key.Escape)
        Close();

      base.OnKeyDown(e);
    }

    private void CustomMessageBox_MouseMove(object sender, MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed && e.Source != DescriptionTextBox)
        DragMove();
    }

    private void Button1_Click(object sender, RoutedEventArgs e) {
      _result = _builder.Buttons switch {
        MessageBoxButton.OK => MessageBoxResult.OK,
        MessageBoxButton.OKCancel => MessageBoxResult.OK,
        MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
        MessageBoxButton.YesNo => MessageBoxResult.Yes,
        _ => throw new NotImplementedException(),
      };
      Close();
    }

    private void Button2_Click(object sender, RoutedEventArgs e) {
      _result = _builder.Buttons switch {
        MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
        MessageBoxButton.YesNoCancel => MessageBoxResult.No,
        MessageBoxButton.YesNo => MessageBoxResult.No,
        _ => throw new NotImplementedException(),
      };
      Close();
    }

    private void Button3_Click(object sender, RoutedEventArgs e) {
      _result = _builder.Buttons switch {
        MessageBoxButton.YesNoCancel => MessageBoxResult.Cancel,
        _ => throw new NotImplementedException(),
      };
      Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) {
      Close();
    }

    private void NewGitHubIssueButton_OnClick(object sender, RoutedEventArgs e) {
      _ = Process.Start($"{App.REPO_URL}/issues/new?assignees=octocat&labels=bug%2Ctriage&template=bugreport.yml");
    }
    public enum ExitOnCloseModes {
      DontExitOnClose,
      ExitOnClose,
    }

    public class Builder {
      internal string Text = string.Empty;
      internal string Caption = "Birthday Notifications";
      internal string Description = string.Empty;
      internal MessageBoxButton Buttons = MessageBoxButton.OK;
      internal MessageBoxResult DefaultResult = MessageBoxResult.None;  // On enter
      internal MessageBoxResult CancelResult = MessageBoxResult.None;  // On escape
      internal MessageBoxImage Image = MessageBoxImage.None;
      internal string OkButtonText = string.Empty;
      internal string CancelButtonText = string.Empty;
      internal string YesButtonText = string.Empty;
      internal string NoButtonText = string.Empty;
      internal bool TopMost = false;
      internal ExitOnCloseModes ExitOnCloseMode = ExitOnCloseModes.DontExitOnClose;
      internal bool ShowHelpLinks = false;
      internal bool ShowDiscordLink = false;
      internal bool ShowIntegrityReportLinks = false;
      internal bool ShowOfficialLauncher = false;
      internal bool ShowNewGitHubIssue = false;
      internal Window? ParentWindow = null;
      internal bool OverrideTopMostFromParentWindow = true;

      public Builder() {
      }
      public Builder WithText(string text) {
        Text = text;
        return this;
      }
      public Builder WithTextFormatted(string format, params object[] args) {
        Text = string.Format(format, args);
        return this;
      }
      public Builder WithAppendText(string text) {
        Text = (Text ?? "") + text;
        return this;
      }
      public Builder WithAppendTextFormatted(string format, params object[] args) {
        Text = (Text ?? "") + string.Format(format, args);
        return this;
      }
      public Builder WithCaption(string caption) {
        Caption = caption;
        return this;
      }
      public Builder WithDescription(string description) {
        Description = description;
        return this;
      }
      public Builder WithAppendDescription(string description) {
        Description = (Description ?? "") + description;
        return this;
      }
      public Builder WithButtons(MessageBoxButton buttons) {
        Buttons = buttons;
        return this;
      }
      public Builder WithDefaultResult(MessageBoxResult result) {
        DefaultResult = result;
        return this;
      }
      public Builder WithCancelResult(MessageBoxResult result) {
        CancelResult = result;
        return this;
      }
      public Builder WithImage(MessageBoxImage image) {
        Image = image;
        return this;
      }
      public Builder WithTopMost(bool topMost = true) {
        TopMost = topMost;
        return this;
      }
      public Builder WithExitOnClose(ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.ExitOnClose) {
        ExitOnCloseMode = exitOnCloseMode;
        return this;
      }
      public Builder WithOkButtonText(string text) {
        OkButtonText = text;
        return this;
      }
      public Builder WithCancelButtonText(string text) {
        CancelButtonText = text;
        return this;
      }
      public Builder WithYesButtonText(string text) {
        YesButtonText = text;
        return this;
      }
      public Builder WithNoButtonText(string text) {
        NoButtonText = text;
        return this;
      }
      public Builder WithShowHelpLinks(bool showHelpLinks = true) {
        ShowHelpLinks = showHelpLinks;
        return this;
      }
      public Builder WithShowDiscordLink(bool showDiscordLink = true) {
        ShowDiscordLink = showDiscordLink;
        return this;
      }
      public Builder WithShowOfficialLauncher(bool showOfficialLauncher = true) {
        ShowOfficialLauncher = showOfficialLauncher;
        return this;
      }
      public Builder WithShowIntegrityReportLink(bool showReportLinks = true) {
        ShowIntegrityReportLinks = showReportLinks;
        return this;
      }
      public Builder WithShowNewGitHubIssue(bool showNewGitHubIssue = true) {
        ShowNewGitHubIssue = showNewGitHubIssue;
        return this;
      }
      public Builder WithParentWindow(Window? window) {
        ParentWindow = window;
        return this;
      }
      public Builder WithParentWindow(Window window, bool overrideTopMost) {
        ParentWindow = window;
        OverrideTopMostFromParentWindow = overrideTopMost;
        return this;
      }

      public Builder WithExceptionText() {
        return this.WithText("An error in Birthday Notifications occurred. If this issue persists, please report\r\nit on GitHub by clicking the button below, describing the issue and copying the text in the box.");
      }

      public Builder WithAppendSettingsDescription(string context) {
        this.WithAppendDescription("\n\nVersion: " + AppUtils.GetAssemblyVersion())
            .WithAppendDescription("\nGit Hash: " + AppUtils.GetGitHash())
            .WithAppendDescription("\nContext: " + context)
            .WithAppendDescription("\nOS: " + Environment.OSVersion)
            .WithAppendDescription("\n64bit? " + Environment.Is64BitProcess);
        if (App.Settings != null) {
          this.WithAppendDescription("\nConfig Version? " + App.Settings.ConfigVersion);
        }

#if DEBUG
        this.WithAppendDescription("\nDebugging");
#endif

        return this;
      }

      public static Builder NewFrom(string text) => new Builder().WithText(text);
      public static Builder NewFrom(Exception exc, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
        var builder = new Builder()
                    .WithText(ErrorExplanation)
                    .WithExitOnClose(exitOnCloseMode)
                    .WithImage(MessageBoxImage.Error)
                    .WithShowHelpLinks(true)
                    .WithShowDiscordLink(true)
                    .WithShowNewGitHubIssue(true)
                    .WithAppendDescription(exc.ToString())
                    .WithAppendSettingsDescription(context);

        if (exitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
          _ = builder.WithButtons(MessageBoxButton.YesNo)
              .WithYesButtonText("Restart")
              .WithNoButtonText("Exit");
        }

        return builder;
      }

      public static Builder NewFromUnexpectedException(Exception exc, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
        return NewFrom(exc, context, exitOnCloseMode)
            .WithAppendTextFormatted("Unexpected error has occurred. ({0})",
                exc.Message)
            .WithAppendText("\n")
            .WithAppendText("Please report this error.");
      }

      public MessageBoxResult ShowAssumingDispatcherThread() {
        DefaultResult = DefaultResult != MessageBoxResult.None ? DefaultResult : Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.OK,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
          MessageBoxButton.YesNo => MessageBoxResult.Yes,
          _ => throw new NotImplementedException(),
        };

        CancelResult = CancelResult != MessageBoxResult.None ? CancelResult : Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNo => MessageBoxResult.No,
          _ => throw new NotImplementedException(),
        };

        var res = new CustomMessageBox(this);
        _ = res.ShowDialog();
        return res._result;
      }

      public MessageBoxResult ShowInNewThread() {
        MessageBoxResult? res = null;
        var newWindowThread = new Thread(() => res = ShowAssumingDispatcherThread());
        newWindowThread.SetApartmentState(ApartmentState.STA);
        newWindowThread.IsBackground = true;
        newWindowThread.Start();
        newWindowThread.Join();
        return res.GetValueOrDefault(CancelResult);
      }

      public MessageBoxResult ShowMessageBox() {
        MessageBoxResult result;
        if (ParentWindow is not null) {
          result = System.Windows.Threading.Dispatcher.CurrentDispatcher == ParentWindow.Dispatcher
            ? ShowAssumingDispatcherThread()
            : ParentWindow.Dispatcher.Invoke(ShowAssumingDispatcherThread);
        } else {
          result = Thread.CurrentThread.GetApartmentState() == ApartmentState.STA
            ? ShowAssumingDispatcherThread()
            : Application.Current.Dispatcher.Invoke(ShowAssumingDispatcherThread);
        }

        if (ExitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
          Log.CloseAndFlush();
          if (result == MessageBoxResult.Yes) {
            var fileName = Process.GetCurrentProcess().MainModule?.FileName;
            _ = Process.Start(fileName is not null ? fileName : "Birthday Notifications", string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(x => EncodeParameterArgument(x))));
          }
          Environment.Exit(-1);
        }

        return result;
      }
    }

    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Asterisk, Window? parentWindow = null) {
      return new Builder()
          .WithCaption(caption)
          .WithText(text)
          .WithButtons(buttons)
          .WithImage(image)
          .WithParentWindow(parentWindow)
          .ShowMessageBox();
    }

    public static bool AssertOrShowError(bool condition, string context, bool fatal = false, Window? parentWindow = null) {
      if (condition)
        return false;

      try {
        throw new InvalidOperationException("Assertion failure.");
      } catch (Exception e) {
        _ = Builder.NewFrom(e, context, fatal ? ExitOnCloseModes.ExitOnClose : ExitOnCloseModes.DontExitOnClose)
            .WithAppendText("\n\n")
            .WithAppendText("Something that cannot happen happened.")
            .WithParentWindow(parentWindow)
            .ShowMessageBox();
      }

      return true;
    }

    // https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
    private static string EncodeParameterArgument(string argument, bool force = false) {
      if (!force && argument.Length > 0 && argument.IndexOfAny(" \t\n\v\"".ToCharArray()) == -1)
        return argument;

      var quoted = new StringBuilder(argument.Length * 2);
      _ = quoted.Append('"');

      var numberBackslashes = 0;

      foreach (var chr in argument) {
        switch (chr) {
          case '\\':
            numberBackslashes++;
            continue;

          case '"':
            _ = quoted.Append('\\', (numberBackslashes * 2) + 1);
            _ = quoted.Append(chr);
            break;

          default:
            _ = quoted.Append('\\', numberBackslashes);
            _ = quoted.Append(chr);
            break;
        }
        numberBackslashes = 0;
      }

      _ = quoted.Append('\\', numberBackslashes * 2);
      _ = quoted.Append('"');

      return quoted.ToString();
    }
  }
}