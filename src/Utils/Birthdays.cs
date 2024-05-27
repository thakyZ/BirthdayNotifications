using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BirthdayNotifications.Config;

using CommunityToolkit.WinUI.Notifications;

using Serilog;

using Windows.UI.Notifications;

namespace BirthdayNotifications.Utils {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  public class Birthdays {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private List<BirthdayUser> BirthdayUsers { get; set; } = new();

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    private Action Finished { get; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public Birthdays() {
      Finished = new Action(() => Log.Warning("A Birthdays class created without an action specified."));
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    public Birthdays(Action finished) {
      this.Finished = finished;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    public void CheckBirthdays() {
      try {
        var birthdaysToday = GetBirthdayOfToday();
        BirthdayUsers = birthdaysToday ?? new List<BirthdayUser>();
      } catch (Exception e) {
        Log.Error("Failed to get birthdays:\n{0}\n{1}", e.Message, e.StackTrace);
      } finally {
        if (BirthdayUsers?.Count > 0) {
          NotifyMeOfBirthday();
        }
      }

      Finished?.Invoke();
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    private static async Task<List<BirthdayUser>> GetBirthdayOfTodayAsync() {
      List<BirthdayUser>? who = await Task.Run(() =>
      {
        var whoes = new List<BirthdayUser>();
        if (App.Settings.OwnBirthday.Enabled) {
          var borthday = App.Settings.OwnBirthday.BirthdayUnix.ToLocalTime().DateTime;
          if (borthday.Month == DateTime.UtcNow.Month && borthday.DayOfYear == DateTime.UtcNow.DayOfYear) {
            whoes.Add(App.Settings.OwnBirthday);
          }
        }
        foreach (BirthdayUser user in App.Settings.BirthdayUsers) {
          if (!user.Enabled) {
            continue;
          }
          DateTime userBirthday = user.BirthdayUnix.ToLocalTime().DateTime;
          if (userBirthday.Month == DateTime.Now.Month && userBirthday.Day == DateTime.Now.Day) {
            whoes.Add(user);
          }
        }
        return whoes;
      });
      return who.Count > 0 ? who : new();
    }
#nullable disable

    /// <summary>
    /// TODO: Descriptor
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

#nullable enable
    public static Uri GetUserAvatar(Avatar? avatar) {
      Uri? uri = new(Cache.GetCacheFile("birthdaycat"));
      if (avatar is not null && (!string.IsNullOrEmpty(avatar.Name) || !string.IsNullOrWhiteSpace(avatar.Name))) {
        uri = new(Cache.GetCacheFile(avatar.Name));
      }
      return uri;
    }
#nullable disable

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    private void NotifyMeOfBirthday() {
      var toasts = new List<ToastContentBuilder>();

      BirthdayUsers.ForEach((birthdayUser) => {
        toasts.Add(new ToastContentBuilder()
          .AddText("It's a birthday!")
          .AddText($"{birthdayUser.Name}'s birthday is today!")
          .AddAppLogoOverride(GetUserAvatar(birthdayUser.UserAvatar), ToastGenericAppLogoCrop.Circle)
          .SetToastScenario(ToastScenario.Reminder));
        Log.Information("It was {0}'s birthday on this day.", birthdayUser.Name);
      });

      var toastNotifier = ToastNotificationManagerCompat.CreateToastNotifier();

      foreach (ToastContentBuilder toast in toasts) {
        toastNotifier.Show(new ToastNotification(toast.GetXml()));
      }

      _ = Task.Run(() => Task.Delay(5000));
    }
  }
}
