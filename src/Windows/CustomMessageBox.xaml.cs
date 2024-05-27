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
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private readonly Builder _builder;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private MessageBoxResult _result;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private CustomMessageBoxViewModel ViewModel => (DataContext as CustomMessageBoxViewModel)!;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public const string ErrorExplanation = "An error in Birthday Notifications occurred. If this issue persists, please report\r\nit on GitHub by clicking the button below, describing the issue and copying the text in the box.";

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="builder"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public CustomMessageBox(Builder builder) {
      _builder = builder;
      _result = _builder.CancelResult;

      InitializeComponent();

      DataContext = new CustomMessageBoxViewModel();

      ViewModel.CopyMessageTextCommand = new SyncCommand(_ => Clipboard.SetText(_builder.Text));

      if (builder.ParentWindow?.IsVisible ?? false) {
        Owner = builder.ParentWindow;
        ShowInTaskbar = false;
      } else {
        ShowInTaskbar = true;
      }

      Title = builder.Caption;
      MessageTextBlock.Text = builder.Text;

      if (string.IsNullOrWhiteSpace(builder.Description)) {
        DescriptionTextBox.Visibility = Visibility.Collapsed;
      } else {
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
            _ => throw new ArgumentOutOfRangeException(nameof(builder), builder.DefaultResult, null),
          }).Focus();
          break;
        case MessageBoxButton.OKCancel:
          Button1.Content = builder.OkButtonText ?? "Ok";
          Button2.Content = builder.CancelButtonText ?? "Cancel";
          Button3.Visibility = Visibility.Collapsed;
          _ = (builder.DefaultResult switch {
            MessageBoxResult.OK => Button1,
            MessageBoxResult.Cancel => Button2,
            _ => throw new ArgumentOutOfRangeException(nameof(builder), builder.DefaultResult, null),
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
            _ => throw new ArgumentOutOfRangeException(nameof(builder), builder.DefaultResult, null),
          }).Focus();
          break;
        case MessageBoxButton.YesNo:
          Button1.Content = builder.YesButtonText ?? "Yes";
          Button2.Content = builder.NoButtonText ?? "No";
          Button3.Visibility = Visibility.Collapsed;
          _ = (builder.DefaultResult switch {
            MessageBoxResult.Yes => Button1,
            MessageBoxResult.No => Button2,
            _ => throw new ArgumentOutOfRangeException(nameof(builder), builder.DefaultResult, null),
          }).Focus();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(builder), builder.Buttons, null);
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
          throw new ArgumentOutOfRangeException(nameof(builder), builder.Image, null);
      }

      NewGitHubIssueButton.Visibility = builder.ShowNewGitHubIssue ? Visibility.Visible : Visibility.Collapsed;

      Topmost = builder.OverrideTopMostFromParentWindow ? builder.ParentWindow?.Topmost ?? builder.TopMost : builder.TopMost;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyDown(KeyEventArgs e) {
      if (e.Key == Key.Escape)
        Close();

      base.OnKeyDown(e);
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CustomMessageBox_MouseMove(object sender, MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed && e.Source != DescriptionTextBox)
        DragMove();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
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

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void Button2_Click(object sender, RoutedEventArgs e) {
      _result = _builder.Buttons switch {
        MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
        MessageBoxButton.YesNoCancel => MessageBoxResult.No,
        MessageBoxButton.YesNo => MessageBoxResult.No,
        _ => throw new NotImplementedException(),
      };
      Close();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void Button3_Click(object sender, RoutedEventArgs e) {
      _result = _builder.Buttons switch {
        MessageBoxButton.YesNoCancel => MessageBoxResult.Cancel,
        _ => throw new NotImplementedException(),
      };
      Close();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseButton_Click(object sender, RoutedEventArgs e) {
      Close();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewGitHubIssueButton_OnClick(object sender, RoutedEventArgs e) {
      _ = Process.Start($"{App.REPO_URL}/issues/new?assignees=octocat&labels=bug%2Ctriage&template=bugreport.yml");
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public enum ExitOnCloseModes {
      DontExitOnClose,
      ExitOnClose,
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public class Builder {
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string Text = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string Caption = "Birthday Notifications";
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string Description = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal MessageBoxButton Buttons = MessageBoxButton.OK;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal MessageBoxResult DefaultResult = MessageBoxResult.None;  // On enter
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal MessageBoxResult CancelResult = MessageBoxResult.None;  // On escape
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal MessageBoxImage Image = MessageBoxImage.None;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string OkButtonText = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string CancelButtonText = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string YesButtonText = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal string NoButtonText = string.Empty;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool TopMost = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal ExitOnCloseModes ExitOnCloseMode = ExitOnCloseModes.DontExitOnClose;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool ShowHelpLinks = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool ShowDiscordLink = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool ShowIntegrityReportLinks = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool ShowOfficialLauncher = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool ShowNewGitHubIssue = false;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal Window? ParentWindow = null;
      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      internal bool OverrideTopMostFromParentWindow = true;

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      public Builder() {
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithText(string text) {
        Text = text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="format"></param>
      /// <param name="args"></param>
      /// <returns></returns>
      public Builder WithTextFormatted(string format, params object[] args) {
        Text = string.Format(format, args);
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithAppendText(string text) {
        Text = (Text ?? "") + text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="format"></param>
      /// <param name="args"></param>
      /// <returns></returns>
      public Builder WithAppendTextFormatted(string format, params object[] args) {
        Text = (Text ?? "") + string.Format(format, args);
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="caption"></param>
      /// <returns></returns>
      public Builder WithCaption(string caption) {
        Caption = caption;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="description"></param>
      /// <returns></returns>
      public Builder WithDescription(string description) {
        Description = description;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="description"></param>
      /// <returns></returns>
      public Builder WithAppendDescription(string description) {
        Description = (Description ?? "") + description;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="buttons"></param>
      /// <returns></returns>
      public Builder WithButtons(MessageBoxButton buttons) {
        Buttons = buttons;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="result"></param>
      /// <returns></returns>
      public Builder WithDefaultResult(MessageBoxResult result) {
        DefaultResult = result;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="result"></param>
      /// <returns></returns>
      public Builder WithCancelResult(MessageBoxResult result) {
        CancelResult = result;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="image"></param>
      /// <returns></returns>
      public Builder WithImage(MessageBoxImage image) {
        Image = image;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="topMost"></param>
      /// <returns></returns>
      public Builder WithTopMost(bool topMost = true) {
        TopMost = topMost;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="exitOnCloseMode"></param>
      /// <returns></returns>
      public Builder WithExitOnClose(ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.ExitOnClose) {
        ExitOnCloseMode = exitOnCloseMode;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithOkButtonText(string text) {
        OkButtonText = text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithCancelButtonText(string text) {
        CancelButtonText = text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithYesButtonText(string text) {
        YesButtonText = text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public Builder WithNoButtonText(string text) {
        NoButtonText = text;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="showHelpLinks"></param>
      /// <returns></returns>
      public Builder WithShowHelpLinks(bool showHelpLinks = true) {
        ShowHelpLinks = showHelpLinks;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="showDiscordLink"></param>
      /// <returns></returns>
      public Builder WithShowDiscordLink(bool showDiscordLink = true) {
        ShowDiscordLink = showDiscordLink;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="showOfficialLauncher"></param>
      /// <returns></returns>
      public Builder WithShowOfficialLauncher(bool showOfficialLauncher = true) {
        ShowOfficialLauncher = showOfficialLauncher;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="showReportLinks"></param>
      /// <returns></returns>
      public Builder WithShowIntegrityReportLink(bool showReportLinks = true) {
        ShowIntegrityReportLinks = showReportLinks;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="showNewGitHubIssue"></param>
      /// <returns></returns>
      public Builder WithShowNewGitHubIssue(bool showNewGitHubIssue = true) {
        ShowNewGitHubIssue = showNewGitHubIssue;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="window"></param>
      /// <returns></returns>
      public Builder WithParentWindow(Window? window) {
        ParentWindow = window;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="window"></param>
      /// <param name="overrideTopMost"></param>
      /// <returns></returns>
      public Builder WithParentWindow(Window window, bool overrideTopMost) {
        ParentWindow = window;
        OverrideTopMostFromParentWindow = overrideTopMost;
        return this;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <returns></returns>
      public Builder WithExceptionText() {
        return this.WithText("An error in Birthday Notifications occurred. If this issue persists, please report\r\nit on GitHub by clicking the button below, describing the issue and copying the text in the box.");
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="context"></param>
      /// <returns></returns>
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

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="text"></param>
      /// <returns></returns>
      public static Builder NewFrom(string text) => new Builder().WithText(text);

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="exception"></param>
      /// <param name="context"></param>
      /// <param name="exitOnCloseMode"></param>
      /// <returns></returns>
      public static Builder NewFrom(Exception exception, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
        var builder = new Builder()
                    .WithText(ErrorExplanation)
                    .WithExitOnClose(exitOnCloseMode)
                    .WithImage(MessageBoxImage.Error)
                    .WithShowHelpLinks(true)
                    .WithShowDiscordLink(true)
                    .WithShowNewGitHubIssue(true)
                    .WithAppendDescription(exception.ToString())
                    .WithAppendSettingsDescription(context);

        if (exitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
          _ = builder.WithButtons(MessageBoxButton.YesNo)
              .WithYesButtonText("Restart")
              .WithNoButtonText("Exit");
        }

        return builder;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <param name="exception"></param>
      /// <param name="context"></param>
      /// <param name="exitOnCloseMode"></param>
      /// <returns></returns>
      public static Builder NewFromUnexpectedException(Exception exception, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
        return NewFrom(exception, context, exitOnCloseMode)
            .WithAppendTextFormatted("Unexpected error has occurred. ({0})",
                exception.Message)
            .WithAppendText("\n")
            .WithAppendText("Please report this error.");
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <returns></returns>
      /// <exception cref="NotImplementedException"></exception>
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

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <returns></returns>
      public MessageBoxResult ShowInNewThread() {
        MessageBoxResult? res = null;
        var newWindowThread = new Thread(() => res = ShowAssumingDispatcherThread());
        newWindowThread.SetApartmentState(ApartmentState.STA);
        newWindowThread.IsBackground = true;
        newWindowThread.Start();
        newWindowThread.Join();
        return res ?? CancelResult;
      }

      /// <summary>
      /// TODO: Descriptor
      /// </summary>
      /// <returns></returns>
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
            _ = Process.Start(fileName ?? "Birthday Notifications", string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(x => EncodeParameterArgument(x))));
          }
          Environment.Exit(-1);
        }

        return result;
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="text"></param>
    /// <param name="caption"></param>
    /// <param name="buttons"></param>
    /// <param name="image"></param>
    /// <param name="parentWindow"></param>
    /// <returns></returns>
    public static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Asterisk, Window? parentWindow = null) {
      return new Builder()
          .WithCaption(caption)
          .WithText(text)
          .WithButtons(buttons)
          .WithImage(image)
          .WithParentWindow(parentWindow)
          .ShowMessageBox();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="context"></param>
    /// <param name="fatal"></param>
    /// <param name="parentWindow"></param>
    /// <returns></returns>
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

    /// <summary>
    /// TODO: Descriptor
    /// https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="force"></param>
    /// <returns></returns>
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