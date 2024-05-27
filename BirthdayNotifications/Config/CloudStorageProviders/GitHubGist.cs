using System;

using Newtonsoft.Json;

namespace BirthdayNotifications.Config.CloudStorageProviders {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  [Serializable]
  public class GitHubGist : CloudStorageProviderBase {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    [JsonProperty("gist_url", Order = 1, Required = Required.Always)]
    public string GistUrl { get; set; } = string.Empty;
  }
}
