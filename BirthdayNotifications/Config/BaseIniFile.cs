using System;
using System.IO;
using System.Reflection;

using BirthdayNotifications.Utils;

namespace BirthdayNotifications.Config {
  internal class BaseIniFile {
    public string Path { get; } = string.Empty;
    private static string Executable {
      get {
        var output = Assembly.GetExecutingAssembly().GetName().Name;
        if (string.IsNullOrEmpty(output)) {
          throw new NullReferenceException("Unable to get executing assembly.");
        }
        return output;
      }
    }

    public BaseIniFile(string? IniPath = null) {
      Path = new FileInfo(IniPath ?? Executable + ".ini").FullName;
    }

    public string Read(string Key, string Section = "") {
      _ = LibraryImportHelpers.GetPrivateProfileString(Section ?? Executable, Key, "", out string RetVal, 255, Path);
      return RetVal;
    }

    public void Write(string Key, string Value, string Section = "") {
      _ = LibraryImportHelpers.WritePrivateProfileString(Section ?? Executable, Key, Value, Path);
    }

    public void DeleteKey(string Key, string Section = "") {
      Write(Key, string.Empty, Section ?? Executable);
    }

    public void DeleteSection(string Section = "") {
      Write(string.Empty, string.Empty, Section ?? Executable);
    }

    public bool KeyExists(string Key, string Section = "") {
      return Read(Key, Section).Length > 0;
    }
  }
}
