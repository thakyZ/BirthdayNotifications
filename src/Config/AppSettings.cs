using BirthdayNotifications.Utils;

namespace BirthdayNotifications.Config {
  /// <summary>
  /// INI Config in the directory where the exectuable is in.
  /// </summary>
  internal sealed class AppSettings {
    public string ConfigDirectory = string.Empty;

    private static AppSettings? Instance { get; set; }
    private BaseIniFile IniFile { get; }

    private AppSettings() {
      IniFile = new(null);
    }

    private static BaseIniFile InternalLoad() {
      Instance ??= new();
      return Instance.IniFile;
    }

    public static AppSettings Load() {
      var iniFile = InternalLoad()!;

      if (iniFile.KeyExists("ConfigDirectory", "AppSettings")) {
        Instance!.ConfigDirectory = iniFile.Read("ConfigDirectory", "AppSettings");
      }
      iniFile.Write("ConfigDirectory", AppUtils.GetInstanceDirectory(), "AppSettings");
      return Instance!;
    }
  }
}
