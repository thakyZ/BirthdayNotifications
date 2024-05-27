using System;

using Newtonsoft.Json;

using Serilog;

namespace BirthdayNotifications.Config.Parsers {
  internal class DateParser : JsonConverter<DateTimeOffset> {
    public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer) {
      if (reader.Value is string stringValue) {
        Log.Debug("DateTimeOffset JsonValue String: {0}", stringValue);
        return DateTimeOffset.ParseExact(stringValue, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"));
      } else if (reader.Value is long longValue) {
        return DateTimeOffset.FromUnixTimeSeconds(longValue);
      } else {
        return DateTimeOffset.FromUnixTimeSeconds(0);
      }
    }

    public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer) {
      writer.WriteValue(value.ToString("yyyy-MM-dd"));
    }
  }
}
