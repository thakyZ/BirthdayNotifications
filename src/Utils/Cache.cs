﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using ABI.Windows.ApplicationModel.Activation;

using BirthdayNotifications.Config;
using BirthdayNotifications.Properties;

using Serilog;

using Windows.Media.Audio;

namespace BirthdayNotifications.Utils {
  internal class Cache {
    /// <summary>
    /// 
    /// </summary>
    private static string CacheDirectory {
      get => Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BirthdayNotifications", "Cache");
    }

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<string, string> _ApplicationFileHash = new();

    /// <summary>
    /// 
    /// </summary>
    public Action Finished;

    /// <summary>
    /// 
    /// </summary>
    private static bool Changed {
      get; set;
    } = false;

    internal Cache(Action finished) {
      Finished = finished;
      CheckChecksums();
    }

    internal Cache() {
      CheckChecksums();
    }

    private void CreateChecksums() {
      using (var md5 = MD5.Create()) {
        var stream = new MemoryStream();
        Resources.birthdaycat.Save(stream, Resources.birthdaycat.RawFormat);
        using (stream) {
          _ApplicationFileHash.Add("birthdaycat.png", BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant());
        }
        stream.Close();

        foreach (var birthdayUser in App.Settings.BirthdayUsers.FindAll(b => b.Enabled.Equals(true))) {
          if (birthdayUser.UserAvatar is null) {
            continue;
          }
          using (var avatarStream = new MemoryStream(Convert.FromBase64String(birthdayUser.UserAvatar.Data.Substring("data:image/png;base64,".Length)))) {
            if (string.IsNullOrEmpty(birthdayUser.UserAvatar.Name) || string.IsNullOrWhiteSpace(birthdayUser.UserAvatar.Name)) {
              birthdayUser.UserAvatar.Name = ReturnFileName(birthdayUser.Name);
              if (App.Settings.BirthdayUsers.Find(b => b.Name == birthdayUser.Name) is not null && App.Settings.BirthdayUsers.Find(b => b.Name == birthdayUser.Name).UserAvatar is not null) {
                App.Settings.BirthdayUsers.Find(b => b.Name == birthdayUser.Name).UserAvatar.Name = birthdayUser.UserAvatar.Name;
                Changed = true;
              }
            }
            _ApplicationFileHash.Add(ReturnFileName(birthdayUser.Name), BitConverter.ToString(md5.ComputeHash(avatarStream)).Replace("-", "").ToLowerInvariant());
            avatarStream.Close();
          }
        }
      }
      if (Changed) {
        App.Settings.SaveConfig();
      }
    }

    private static string ReturnFileName(string name) {
      foreach (char c in System.IO.Path.GetInvalidFileNameChars()) {
        name = name.Replace(c, '_');
      }
      if (name.Split('.').Length > 0) {
        name = name.Split('.')[0];
      }
      name += ".png";
      return name;
    }

    private static Dictionary<string, string> LoadChecksums() {
      var tempData = new Dictionary<string, string>();
      using (var md5 = MD5.Create()) {
        if (File.Exists(new Uri(Path.Join(CacheDirectory, "birthdaycat.png")).AbsolutePath)) {
          using (var stream = File.OpenRead(new Uri(Path.Join(CacheDirectory, "birthdaycat.png")).AbsolutePath)) {
            tempData.Add("birthdaycat.png", BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant());
          }
        }
      }
      return tempData;
    }

    private static bool CreateCacheFolder() {
      try {
        _ = Directory.CreateDirectory(CacheDirectory);
        return true;
      } catch (Exception e) {
        Log.Error("Failed to create cache directory");
        Log.Error(e.Message);
        Log.Error(e.StackTrace);
        return false;
      }
    }

    private void CheckChecksums() {
      CreateChecksums();
      if (!CreateCacheFolder()) {
        Finished.Invoke();
        return;
      }
      var loadedChecksums = LoadChecksums();
      foreach (var (filename, hash) in _ApplicationFileHash) {
        if (loadedChecksums.Keys.Contains(filename) && hash.Equals(loadedChecksums[filename])) {
          continue;
        }
        if (filename == "birthdaycat.png") {
          using (var stream = File.Create(new Uri(Path.Join(CacheDirectory, filename)).AbsolutePath)) {
            Resources.birthdaycat.Save(stream, Resources.birthdaycat.RawFormat);
          }
        }
      }
      if (Finished is not null) {
        Finished.Invoke();
      }
    }

    public static void SetCacheFile(BirthdayUser birthdayUser, string file) {
      var filename = ReturnFileName(birthdayUser.Name);
      byte[] manipulate;
      using (Stream stream = File.OpenWrite(Path.Join(CacheDirectory, filename))) {
        manipulate = ManipulateFile(file, Path.Join(CacheDirectory, filename));
      }
      try {
        birthdayUser.UserAvatar = new Avatar() {
          Name = filename,
          Data = $"data:image/png;base64,{Convert.ToBase64String(manipulate)}"
        };
      } catch (Exception e) {
        Log.Error($"Failed to write to file: {new Uri(Path.Join(CacheDirectory, filename)).AbsolutePath}");
        Log.Error(e.Message);
        Log.Error(e.StackTrace);
      }
    }

    private static byte[] ManipulateFile(string file, string stream) {
      using (MemoryStream stream2 = new MemoryStream(File.ReadAllBytes(file))) {
        var bi3 = new Bitmap(stream2);

        var destRect = new Rectangle(0, 0, 128, 128);
        var destImage = new Bitmap(128, 128);

        destImage.SetResolution(bi3.HorizontalResolution, bi3.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage)) {
          graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
          graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
          graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
          graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

          using (var wrapMode = new ImageAttributes()) {
            wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipY);
            graphics.DrawImage(bi3, destRect, 0, 0, bi3.Width, bi3.Height, GraphicsUnit.Pixel, wrapMode);
          }
        }

        ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);

        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

        EncoderParameters myEncoderParameters = new EncoderParameters(2);

        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        destImage.Save(stream, pngEncoder, myEncoderParameters);
        return File.ReadAllBytes(stream);
      }
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format) {

      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

      foreach (ImageCodecInfo codec in codecs) {
        if (codec.FormatID == format.Guid) {
          return codec;
        }
      }
      return null;
    }

    public static string GetCacheFile(string fileObject) {
      if (fileObject.Equals("birthdaycat")) {
        return Path.Join(CacheDirectory, "birthdaycat.png");
      } else {
        if (File.Exists(Path.Join(CacheDirectory, fileObject))) {
          return Path.Join(CacheDirectory, fileObject);
        } else {
          Log.Error($"Cached file, {fileObject}, somehow missing.");
        }
      }
      return string.Empty;
    }
  }
}
