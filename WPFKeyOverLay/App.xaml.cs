using System.IO;
using System.Text.Json;
using System.Windows;
using WPFKeyOverLay.Converter;

namespace WPFKeyOverLay;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public const string ConfigPath = "./Config.json";

	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
	{
		WriteIndented = true,
		Converters =
		{
			new SolidColorBrushConverter(),
			new KeyEnumConverter(),
			new HotkeyActionsEnumConverter()
		}
	};
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public Config Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

#if DEBUG
		File.Delete(ConfigPath);
#endif

		try
		{
			this.Config = JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigPath), this.JsonSerializerOptions)!;
		}
		catch
		{
			this.Config = new();
			File.WriteAllText(ConfigPath, JsonSerializer.Serialize(this.Config, this.JsonSerializerOptions));
		}
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		MessageBox.Show(e.ExceptionObject.ToString());
		Environment.Exit(-1);
	}
}

