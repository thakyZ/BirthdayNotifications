using System;

using BirthdayNotifications.Config.Parsers;

using Newtonsoft.Json;

namespace BirthdayNotifications.Config {
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
    public DateTimeOffset BirthdayUnix { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("avatar", Order = 3, Required = Required.AllowNull, NullValueHandling = NullValueHandling.Include)]
    public Avatar? UserAvatar { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("enabled", Order = 4, Required = Required.Always)]
    public bool Enabled { get; set; }

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
    /// <param name="birthDate">The unix timestamp representing a birthday</param>
    /// <param name="name">The name associated with the birthday</param>
    /// <param name="enabled">The state of the birthday user notifications being enabled.</param>
    /// <param name="avatar">An instanced class of a loaded avatar</param>
    public BirthdayUser(long birthDate, string name, bool enabled = false, Avatar? avatar = null) {
      BirthdayUnix = DateTimeOffset.FromUnixTimeMilliseconds(birthDate);
      Name = name;
      Enabled = enabled;
      UserAvatar = avatar;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public BirthdayUser() {
      BirthdayUnix = Default.BirthdayUnix;
      Name = Default.Name;
      Enabled = Default.Enabled;
      UserAvatar = Default.UserAvatar;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonIgnore]
    public static BirthdayUser Default => new(0, "None", false, null);
  }
}
