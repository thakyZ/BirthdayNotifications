namespace BirthdayNotifications.Utils {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  internal static class EnvironmentVars {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    internal static bool BN_DEBUG => CheckEnv("BN_DEBUG");
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="vari"></param>
    /// <returns></returns>
    private static bool CheckEnv(string vari) {
#if DEBUG
      return bool.Parse(System.Environment.GetEnvironmentVariable(vari) ?? "false");
#else
      return false;
#endif
    }
  }
}
