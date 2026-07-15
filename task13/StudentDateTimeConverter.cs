using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class StudentDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "dd.MM.yyyy";

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (!DateTime.TryParseExact(
                value,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            throw new JsonException($"Дата должна быть указана в формате {DateFormat}");
        }

        return date;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
