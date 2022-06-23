using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

using BirthdayNotifications.Config;

using Microsoft.Toolkit.Uwp.Notifications;

using Serilog;

using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace BirthdayNotifications.Utils {
  /// <summary>
  /// 
  /// </summary>
  public class Birthdays {
    /// <summary>
    /// 
    /// </summary>
    private List<BirthdayUser> BirthdayUsers { get; set; }

    private Action finished;

    /// <summary>
    /// 
    /// </summary>
    public Birthdays() {
    }

    public Birthdays(Action finished) {
      this.finished = finished;
    }

    public void CheckBirthdays() {
      try {
        var birthdaysToday = GetBirthdayOfToday();
        BirthdayUsers = birthdaysToday is not null ? birthdaysToday : new List<BirthdayUser>();
      } catch (Exception e) {
        Log.Error($"Failed to get birthdays:\n{e.Message}\n{e.StackTrace}");
      } finally {
        if (BirthdayUsers?.Count > 0) {
          NotifyMeOfBirthday();
        }
      }

      if (finished is not null) {
        finished.Invoke();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static async Task<List<BirthdayUser>?> GetBirthdayOfTodayAsync() {
      List<BirthdayUser>? who = await Task.Run(() =>
      {
        var whoes = new List<BirthdayUser>();
        if (App.Settings.OwnBirthday.Enabled)
        {
          var borthday = App.Settings.OwnBirthday.BirthdayUnix.ToLocalTime().DateTime;
          if (borthday.Month == DateTime.UtcNow.Month && borthday.DayOfYear == DateTime.UtcNow.DayOfYear)
          {
            whoes.Add(App.Settings.OwnBirthday);
          }
        }
        foreach (BirthdayUser user in App.Settings.BirthdayUsers)
        {
          if (!user.Enabled)
          {
            continue;
          }
          DateTime userBirthday = user.BirthdayUnix.ToLocalTime().DateTime;
          if (userBirthday.Month == DateTime.Now.Month && userBirthday.Day == DateTime.Now.Day)
          {
            whoes.Add(user);
          }
       }
        return whoes;
      });
      return who.Count > 0 ? who : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static List<BirthdayUser> GetBirthdayOfToday() {
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

    public static Uri GetUserAvatar(Avatar? avatar) {
      Uri? uri = new (Cache.GetCacheFile("birthdaycat"));
      if (avatar is not null && (!string.IsNullOrEmpty(avatar.Name) || !string.IsNullOrWhiteSpace(avatar.Name))) {
        uri = new(Cache.GetCacheFile(avatar.Name));
      }
      return uri;
    }

    /// <summary>
    /// 
    /// </summary>
    private void NotifyMeOfBirthday() {
      var toasts = new List<ToastContentBuilder>();
      
      BirthdayUsers.ForEach((birthdayUser) => {
        toasts.Add(new ToastContentBuilder()
          .AddText("It's a birthday!")
          .AddText($"{birthdayUser.Name}'s birthday is today!")
          .AddAppLogoOverride(GetUserAvatar(birthdayUser.UserAvatar), ToastGenericAppLogoCrop.Circle)
          .SetToastScenario(ToastScenario.Reminder));
        Log.Information($"It was {birthdayUser.Name}'s birthday on this day.");
      });

      var toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();
      
      foreach (ToastContentBuilder toast in toasts) {
        toastNotifier.Show(new ToastNotification(toast.GetXml()));
      }

      _ = Task.Run(() => Task.Delay(5000));
    }
  }
}
