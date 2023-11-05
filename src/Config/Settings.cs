using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

using BirthdayNotifications.Config.Parsers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;
using Serilog.Core;

namespace BirthdayNotifications.Config {
  [Serializable]
  public class Settings {
    #region Birthday Notifications Setting
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("config_version")]
    public int? ConfigVersion {
      get;
      internal set;
    } = 0;
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("birthday_users")]
    public List<BirthdayUser> BirthdayUsers {
      get; set;
    } = new List<BirthdayUser>();
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("own_birthday")]
    public BirthdayUser OwnBirthday {
      get; set;
    } = BirthdayUser.Default;
    #endregion

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonIgnore]
    private static string _configPath = "";

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
    /// <param name="configPath"></param>
    /// <returns></returns>
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
    /// <param name="configPath"></param>
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
  }

  [Serializable]
  public class Avatar {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name {
      get; set;
    }

    [JsonProperty("data", Order = 2, Required = Required.Always)]
    public string Data {
      get; set;
    }

    public Avatar(string name = "", string data = "") {
      Name = name;
      Data = data;
    }
  }

  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class BirthdayUser {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("birthday", Order = 2, Required = Required.Always)]
    [JsonConverter(typeof(DateParser))]
    public DateTimeOffset BirthdayUnix {
      get; set;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name {
      get; set;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("avatar", Order = 3, Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
    public Avatar? UserAvatar {
      get; set;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("enabled", Order = 4, Required = Required.Always)]
    public bool Enabled {
      get; set;
    } = false;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="birthDate"></param>
    /// <param name="name"></param>
    /// <param name="avatarBase64"></param>
    [JsonConstructor]
    public BirthdayUser(DateTimeOffset birthDate, string name, Avatar? avatar = null) {
      Name = name;
      BirthdayUnix = birthDate;
      UserAvatar = avatar;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="birthDate"></param>
    /// <param name="name"></param>
    /// <param name="avatar"></param>
    public BirthdayUser(long birthDate, string name, Avatar? avatar = null) {
      Name = name;
      BirthdayUnix = DateTimeOffset.FromUnixTimeMilliseconds(birthDate);
      UserAvatar = avatar;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="birthdate"></param>
    /// <param name="name"></param>
    /// <param name="avatarBase64"></param>
    public BirthdayUser() {
      Name = Default.Name;
      BirthdayUnix = Default.BirthdayUnix;
      UserAvatar = Default.UserAvatar;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonIgnore]
    public static BirthdayUser Default => new(0, "None", null);
  }
}
