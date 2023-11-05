using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayNotifications.Utils {
  internal partial class LibraryImportHelpers {
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);
    [LibraryImport("kernel32", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int GetPrivateProfileString(string Section, string Key, string Default, out string RetVal, int Size, string FilePath);
  }
}
