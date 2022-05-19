using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace BirthdayNotifications.Utils {
  public static class AppUtils {
    /// <summary>
    ///     Gets the git hash value from the assembly
    ///     or null if it cannot be found.
    /// </summary>
    public static string GetGitHash() {
      var asm = typeof(AppUtils).Assembly;
      var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
      var gitHash = attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
      return string.IsNullOrEmpty(gitHash) ? "None" : gitHash;
    }

    public static string GetAssemblyVersion() {
      var assembly = Assembly.GetExecutingAssembly();
      var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return string.IsNullOrEmpty(fvi.FileVersion) ? "dev" : fvi.FileVersion;
    }

    public static string GetAssemblyGUID() {
      var assembly = Assembly.GetExecutingAssembly();
      var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute),true)[0];
      return attribute.Value;
    }
  }
}
