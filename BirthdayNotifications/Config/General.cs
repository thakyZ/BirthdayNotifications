using System;
using System.Collections.Generic;

using BirthdayNotifications.Config.CloudStorageProviders;

using Newtonsoft.Json;

namespace BirthdayNotifications.Config {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class General {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("cloud_storage", Order = 1, Required = Required.Always)]
    public bool CloudStorage { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("cloud_storage_provider", Order = 2, Required = Required.AllowNull)]
    public string? CloudStorageProvider { get; set; }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("cloud_storage_providers_list", Order = 2, Required = Required.Always)]
    public List<CloudStorageProviderBase> CloudStorageProvidersList { get; set; } = new();

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("save_directory", Order = 2, Required = Required.AllowNull)]
    public string? SaveDirectory { get; set; }
  }
}
