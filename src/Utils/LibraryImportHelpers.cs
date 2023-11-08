using System.Runtime.InteropServices;

namespace BirthdayNotifications.Utils {
  internal partial class LibraryImportHelpers {
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetPrivateProfileString(string Section, string Key, string Default, out string RetVal, int Size, string FilePath);
  }
}
