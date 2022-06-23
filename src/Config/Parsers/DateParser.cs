using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

namespace BirthdayNotifications.Config.Parsers {
  internal class DateParser : JsonConverter<DateTimeOffset> {
    public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer) {
      if (reader.Value is string) {
        Log.Information((string)reader.Value);
        return DateTimeOffset.ParseExact((string)reader.Value, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
      } else if (reader.Value is long) {
        return DateTimeOffset.FromUnixTimeSeconds((long)reader.Value);
      } else {
        return DateTimeOffset.FromUnixTimeSeconds(0);
      }
    }

    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer) {
      writer.WriteValue(value.ToString("yyyy-MM-dd"));
    }
  }
}
