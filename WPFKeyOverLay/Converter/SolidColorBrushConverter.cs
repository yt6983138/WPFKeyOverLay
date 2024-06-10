using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace WPFKeyOverLay.Converter;
internal class SolidColorBrushConverter : JsonConverter<SolidColorBrush>
{
	public override SolidColorBrush? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return new(this.StringToColor(reader.GetString()!));
	}

	public override void Write(Utf8JsonWriter writer, SolidColorBrush value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Color.ToString());
	}

	public Color StringToColor(string value)
	{
		string str = value.Replace("#", "").Trim();

		if (str.Any(x => !char.IsAsciiHexDigit(x)))
			goto Final;

		if (str.Length == 6)
		{
			return Color.FromRgb(
				Convert.ToByte(str[0..2], 16),
				Convert.ToByte(str[2..4], 16),
				Convert.ToByte(str[4..6], 16));
		}
		if (str.Length == 8)
		{
			return Color.FromArgb(
				Convert.ToByte(str[0..2], 16),
				Convert.ToByte(str[2..4], 16),
				Convert.ToByte(str[4..6], 16),
				Convert.ToByte(str[6..8], 16));
		}

	Final:
		throw new ArgumentException("Invalid string", nameof(value));
	}
}
