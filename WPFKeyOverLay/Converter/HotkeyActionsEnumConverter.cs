using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPFKeyOverLay.Converter;
internal class HotkeyActionsEnumConverter : JsonConverter<HotkeyActions>
{
	public override HotkeyActions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Enum.Parse<HotkeyActions>(reader.GetString()!);
	}

	public override void Write(Utf8JsonWriter writer, HotkeyActions value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}
