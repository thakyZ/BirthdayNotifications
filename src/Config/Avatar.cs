using System;

using Newtonsoft.Json;

namespace BirthdayNotifications.Config {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class Avatar {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("file", Order = 2, Required = Required.AllowNull)]
    public string? File { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("data", Order = 3, Required = Required.AllowNull)]
    public string? Data { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("url", Order = 4, Required = Required.AllowNull)]
    public string? Url { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public Avatar(string name = "Unknown", string? file = null, string? data = null, string? url = null) {
      Name = name;
      File = file;
      Data = data;
      Url = url;
    }
  }
}
