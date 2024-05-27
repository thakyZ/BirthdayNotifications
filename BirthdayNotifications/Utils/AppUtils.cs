using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BirthdayNotifications.Utils {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  public static class AppUtils {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    public static string AppId => "NekoBoiNick.BirthdayNotifications.App";

    /// <summary>
    /// Gets the git hash value from the assembly
    /// or null if it cannot be found.
    /// </summary>
    /// <returns></returns>
    public static string GetGitHash() {
      var asm = typeof(AppUtils).Assembly;
      var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
      var gitHash = attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
      return string.IsNullOrEmpty(gitHash) ? "None" : gitHash;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    public static string GetAssemblyVersion() {
      var assembly = Assembly.GetExecutingAssembly();
      var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return string.IsNullOrEmpty(fvi.FileVersion) ? "dev" : fvi.FileVersion;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <returns></returns>
    public static string GetAssemblyGUID() {
      var assembly = Assembly.GetExecutingAssembly();
      var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
      return attribute.Value;
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="createDefault"></param>
    /// <returns></returns>
    public static string GetInstanceDirectory(bool createDefault = false) {
      if (createDefault && !string.IsNullOrEmpty(App.AppSettings.ConfigDirectory)) {
        return App.AppSettings.ConfigDirectory;
      }
      var path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BirthdayNotifications");
      if (!Directory.Exists(path)) {
        _ = Directory.CreateDirectory(path);
      }
      return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BirthdayNotifications");
    }

    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static ImageSource ToImageSource(Bitmap bitmap) {
      IntPtr hBitmap = bitmap.GetHbitmap();

      ImageSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
        hBitmap,
        IntPtr.Zero,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions());

      bitmapSource.Freeze();

      return bitmapSource;
    }
  }
}
