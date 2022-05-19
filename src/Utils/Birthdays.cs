using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using BirthdayNotifications.Config;

using Microsoft.Toolkit.Uwp.Notifications;

using Serilog;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Xml.Linq;
using System.DirectoryServices.ActiveDirectory;

namespace BirthdayNotifications.Utils {
  /// <summary>
  /// 
  /// </summary>
  public class Birthdays {
    /// <summary>
    /// 
    /// </summary>
    private List<BirthdayUser> BirthdayUsers { get; set; } = new List<BirthdayUser>();

    /// <summary>
    /// 
    /// </summary>
    private List<Task> NotificationTasks { get; set; } = new List<Task>();

    /// <summary>
    /// 
    /// </summary>
    public Birthdays() {
      try {
        BirthdayUsers = GetBirthdayOfToday();
      } catch (Exception e) {
        // Test
      } finally {
        if (BirthdayUsers.Count > 0) {
          NotifyMeOfBirthday();
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task<List<BirthdayUser>?> GetBirthdayOfTodayAsync() {
      List<BirthdayUser>? who = await Task.Run(() => {
        var whoes = new List<BirthdayUser>();
        if (App.Settings.OwnBirthday.Enabled) {
          var borthday = DateTimeOffset.FromUnixTimeSeconds(App.Settings.OwnBirthday.BirthdayUnix).DateTime;
          if (borthday.Month == DateTime.UtcNow.Month && borthday.DayOfYear == DateTime.UtcNow.DayOfYear && borthday.Hour <= DateTime.UtcNow.Hour) {
            whoes.Add(App.Settings.OwnBirthday);
          }
        }
        foreach (BirthdayUser user in App.Settings.BirthdayUsers) {
          if (!user.Enabled) {
            continue;
          }
          DateTime userBirthday = DateTimeOffset.FromUnixTimeSeconds(user.BirthdayUnix).ToLocalTime().DateTime;
          if (userBirthday.Month == DateTime.Now.Month && userBirthday.Day == DateTime.Now.Day && userBirthday.Hour <= DateTime.Now.Hour) {
            whoes.Add(user);
            Log.Information(user.Name);
          }
        }
        return whoes;
      });
      if (who.Count > 0) {
        return who;
      }
      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private List<BirthdayUser> GetBirthdayOfToday() {
      var who = GetBirthdayOfTodayAsync().Result;
      if (who is null) {
        return new List<BirthdayUser>();
      }

      var testOwn = who.Find(e => e.Equals(App.Settings.OwnBirthday));
      if (testOwn is not null) {
        int indexOf = who.IndexOf(testOwn);
        var item = who[indexOf];
        who.RemoveAt(indexOf);
        who.Insert(0, item);
      }
      return who;
    }

    public string GetUserAvatar(string avatar) {
      if (string.IsNullOrEmpty(avatar) || string.IsNullOrWhiteSpace(avatar)) {
        return "pack://application:,,,/Resources/birthdaycat.png";
      }
      return avatar;
    }

    /// <summary>
    /// 
    /// </summary>
    private void NotifyMeOfBirthday() {
      var toasts = new List<ToastNotification>();
      BirthdayUsers.ForEach((birthdayUser) => {
        ToastVisual visual = new ToastVisual() {
          BindingGeneric = new ToastBindingGeneric() {
            Children = {
              new AdaptiveText()
              {
                  Text = "It's a birthday!"
              },
              new AdaptiveText()
              {
                  Text = $"{birthdayUser.Name}'s birthday is today!"
              }
            },
            AppLogoOverride = new ToastGenericAppLogo()
            {
              Source = GetUserAvatar(birthdayUser.UserAvatar),
              HintCrop = ToastGenericAppLogoCrop.Circle
            }
          }
        };
        ToastContent toastContent = new ToastContent()
        {
          Visual = visual
        };
        toasts.Add(new ToastNotification(toastContent.GetXml()));
      });
      var toastNotifier = ToastNotificationManager.CreateToastNotifier(AppUtils.GetAssemblyGUID());
      foreach (ToastNotification toast in toasts) {
        toast.Failed += (o, args) => {
          var message = args.ErrorCode;
          Log.Information(message.Message);
        };
        toastNotifier.Show(toast);
      }
    }
  }
}
