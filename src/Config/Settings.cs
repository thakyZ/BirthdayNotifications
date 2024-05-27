using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

namespace BirthdayNotifications.Config {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class Settings {
    #region Birthday Notifications Setting
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("config_version", Order = 1, Required = Required.Always)]
    public int? ConfigVersion { get; internal set; } = 0;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("birthday_users", Order = 3, Required = Required.Always)]
    public List<BirthdayUser> BirthdayUsers { get; set; } = new List<BirthdayUser>();

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("own_birthday", Order = 2, Required = Required.Always)]
    public BirthdayUser OwnBirthday { get; set; } = BirthdayUser.Default;
    #endregion

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonIgnore]
    private static string _configPath = "";

    #region Methods
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="configPath"></param>
    /// <returns></returns>
    private static Settings CreateDefault(string configPath) {
      Settings config = new Settings {
        BirthdayUsers = new List<BirthdayUser>()
      };
      config.SaveConfig(configPath);
      return config;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="configPath">Path to the config on the file system</param>
    /// <returns>An instanced Settings class</returns>
    public static Settings Load(string configPath) {
      _configPath = configPath;
      Settings config;
      if (!File.Exists(configPath)) {
        Console.WriteLine($"Could not find configuration file at {configPath}");
        Console.WriteLine("Creating default configuration file");
        return CreateDefault(configPath);
      }

      using (StreamReader reader = File.OpenText(configPath)) {
        Settings birthdayNotifSettings = JToken.ReadFromAsync(new JsonTextReader(reader)).Result.ToObject<Settings>() ?? throw new NullReferenceException("Settings reutned null.");
        config = birthdayNotifSettings ?? CreateDefault(configPath);
      }

      return config;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="configPath">Path to the config on the file system</param>
    public void SaveConfig(string configPath) {
      var config = this;
      try {
        if (File.Exists(Path.GetFileName(configPath))) {
          File.Copy(Path.GetFileName(configPath), $"{Path.GetFileNameWithoutExtension(configPath)}.backup.json");
        }
      } catch (Exception e) {
        Log.Error("Failed to backup config file.\n{0}\n{1}", e.Message, e.StackTrace);
      }
      try {
        TextWriter tw = File.CreateText(configPath);
        var serializer = new JsonSerializer {
          NullValueHandling = NullValueHandling.Ignore,
          TypeNameHandling = TypeNameHandling.Auto,
          Formatting = Formatting.Indented
        };
        using (JsonWriter writer = new JsonTextWriter(tw)) {
          serializer.Serialize(writer, config, config.GetType());
        }
        tw.Close();
      } catch (Exception e) {
        Log.Error("Failed to write config to file: {0}\n{1}\n{2}", configPath, e.Message, e.StackTrace);
      }
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    internal void SaveConfig() {
      SaveConfig(_configPath);
    }
    #endregion
  }
}
