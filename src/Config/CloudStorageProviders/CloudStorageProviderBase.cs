using System;

using Newtonsoft.Json;

namespace BirthdayNotifications.Config.CloudStorageProviders {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class CloudStorageProviderBase {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("name", Order = 1, Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("api_key", Order = 2, Required = Required.AllowNull)]
    public string? ApiKey { get; set; } = string.Empty;
  }
}
