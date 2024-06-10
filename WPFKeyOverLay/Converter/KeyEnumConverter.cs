using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace WPFKeyOverLay.Converter;
internal class KeyEnumConverter : JsonConverter<Key>
{
	public override Key Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Enum.Parse<Key>(reader.GetString()!);
	}

	public override void Write(Utf8JsonWriter writer, Key value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}
