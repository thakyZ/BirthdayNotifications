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
  public class BirthdayNotifSettings {
    #region Birthday Notifications Setting
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("config_version")]
    public int? ConfigVersion {
      get;
      internal set;
    } = 0;
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("birthday_users")]
    public List<BirthdayUser> BirthdayUsers {
      get; set;
    } = new List<BirthdayUser>();
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("own_birthday")]
    public BirthdayUser OwnBirthday {
      get; set;
    } = BirthdayUser.Default;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    private static string _configPath = "";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configPath"></param>
    /// <returns></returns>
    private static BirthdayNotifSettings CreateDefault(string configPath) {
      BirthdayNotifSettings config = new BirthdayNotifSettings {
        BirthdayUsers = new List<BirthdayUser>()
      };
      config.SaveConfig(configPath);
      return config;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configPath"></param>
    /// <returns></returns>
    public static BirthdayNotifSettings Load(string configPath) {
      _configPath = configPath;
      BirthdayNotifSettings config;
      if (!File.Exists(configPath)) {
        Console.WriteLine($"Could not find configuration file at {configPath}");
        Console.WriteLine($"Creating default configuration file");
        config = CreateDefault(configPath);
        return config;
      }

      using (StreamReader reader = File.OpenText(configPath)) {
        BirthdayNotifSettings birthdayNotifSettings = JToken.ReadFromAsync(new JsonTextReader(reader)).Result.ToObject<BirthdayNotifSettings>();
        config = birthdayNotifSettings is not null ? birthdayNotifSettings : CreateDefault(configPath);
      }

      return config;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configPath"></param>
    public void SaveConfig(string configPath) {
      var config = this;
      try {
        if (File.Exists(Path.GetFileName(configPath))) {
          File.Copy(Path.GetFileName(configPath), $"{Path.GetFileNameWithoutExtension(configPath)}.backup.json");
        }
      } catch (Exception e) {
        Log.Error("Failed to backup config file.");
        Log.Error($"{e.Message}");
        Log.Error($"{e.StackTrace}");
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
        Log.Error($"Failed to write config to file: {configPath}");
        Log.Error(e.Message);
        Log.Error(e.StackTrace);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    internal void SaveConfig() {
      SaveConfig(_configPath);
    }
  }

  [Serializable]
  public class Avatar {
    /// <summary>
    /// 
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
  /// 
  /// </summary>
  [Serializable]
  public class BirthdayUser {
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("birthday", Order = 2, Required = Required.Always)]
    [JsonConverter(typeof(DateParser))]
    public DateTimeOffset BirthdayUnix {
      get; set;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name {
      get; set;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("avatar", Order = 3, Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
    public Avatar UserAvatar {
      get; set;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("enabled", Order = 4, Required = Required.Always)]
    public bool Enabled {
      get; set;
    } = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="birthdate"></param>
    /// <param name="name"></param>
    /// <param name="avatarBase64"></param>
    [JsonConstructor]
    public BirthdayUser(DateTimeOffset birthdate, string name, Avatar avatar = null) {
      Name = name;
      BirthdayUnix = birthdate;
      UserAvatar = avatar;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="birthdate"></param>
    /// <param name="name"></param>
    /// <param name="avatar"></param>
    public BirthdayUser(long birthdate, string name, Avatar avatar = null) {
      Name = name;
      BirthdayUnix = DateTimeOffset.FromUnixTimeMilliseconds(birthdate);
      UserAvatar = avatar;
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    [JsonIgnore]
    public static BirthdayUser Default => new(0, "None", null);
  }
}
