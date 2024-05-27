using System.Runtime.InteropServices;

namespace BirthdayNotifications.Utils {
  /// <summary>
  /// TODO: Descriptor
  /// </summary>
  internal partial class LibraryImportHelpers {
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="Section"></param>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
    /// <summary>
    /// TODO: Descriptor
    /// </summary>
    /// <param name="Section"></param>
    /// <param name="Key"></param>
    /// <param name="Default"></param>
    /// <param name="RetVal"></param>
    /// <param name="Size"></param>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetPrivateProfileString(string Section, string Key, string Default, out string RetVal, int Size, string FilePath);
  }
}
