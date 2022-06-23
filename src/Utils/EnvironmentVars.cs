using Serilog;

namespace BirthdayNotifications.Utils {
  internal static class EnvironmentVars {
    internal static bool BN_DEBUG => CheckEnv("BN_DEBUG");
    private static bool CheckEnv(string vari) {
#if DEBUG 
      return bool.Parse(System.Environment.GetEnvironmentVariable(vari) ?? "false");
#else
      return false;
#endif
    }
  }
}
