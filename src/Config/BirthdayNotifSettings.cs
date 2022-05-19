using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog.Core;

namespace BirthdayNotifications.Config {
  [Serializable]
  public class BirthdayNotifSettings {
    #region Birthday Notifications Setting
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("config_version")]
    public int? ConfigVersion {
      get;
      internal set;
    } = 0;
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("birthday_users")]
    public List<BirthdayUser> BirthdayUsers {
      get; set;
    } = new List<BirthdayUser>() { BirthdayUser.Default };
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("own_birthday")]
    public BirthdayUser OwnBirthday {
      get; set;
    } = BirthdayUser.Default;
    #endregion

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
      BirthdayNotifSettings config;
      if (!File.Exists(configPath)) {
        Console.WriteLine($"Could not find configuration file at {configPath}");
        Console.WriteLine($"Creating default configuration file");
        config = CreateDefault(configPath);
        return config;
      }

      using (StreamReader reader = File.OpenText(configPath)) {
        BirthdayNotifSettings? birthdayNotifSettings = JToken.ReadFromAsync(new JsonTextReader(reader)).Result.ToObject<BirthdayNotifSettings>();
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
        // None
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
        // None
      }
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
    [JsonPropertyAttribute("birthday", Order = 2, Required = Required.Always)]
    public long BirthdayUnix {
      get; set;
    } = 0;
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("name", Order = 1, Required = Required.Always)]
    public string Name {
      get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("avatar", Order = 3, Required = Required.Always)]
    public string UserAvatar {
      get; set;
    }
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyAttribute("enabled", Order = 4, Required = Required.Always)]
    public bool Enabled {
      get; set;
    } = false;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="birthdate"></param>
    /// <param name="name"></param>
    /// <param name="avatarBase64"></param>
    public BirthdayUser(DateTime birthdate, string name, string avatarBase64) {
      Name = name;
      BirthdayUnix = ((DateTimeOffset)birthdate).ToUnixTimeSeconds();
      UserAvatar = avatarBase64;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="birthdate"></param>
    /// <param name="name"></param>
    /// <param name="avatarBase64"></param>

    [JsonConstructor]
    public BirthdayUser(long birthdate, string name, string avatarBase64) {
      Name = name;
      BirthdayUnix = birthdate;
      UserAvatar = avatarBase64;
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonIgnore]
    public static BirthdayUser Default => new(0, "None", "");
  }
}
