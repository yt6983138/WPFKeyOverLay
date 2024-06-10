using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFKeyOverLay;

public record class KeyActionRecord(Key Key, HotkeyActions HotkeyActions);
public class Config
{
	public float TrailPixelPerSecond { get; set; } = 200;
	public int KpsUpdatesPerSecond { get; set; } = 20;
	public int DefaultHeight { get; set; } = 700;
	public float TrailUpdateFps { get; set; } = 60;
	public float WindowOpacity { get; set; } = 0.75f;
	public float KeyButtonHeight { get; set; } = 64 + 48 - 5;
	public SolidColorBrush WindowBackground { get; set; } = new(Color.FromArgb(255, 255, 255, 255));
	public Thickness WindowPadding { get; set; } = new(0, 0, 0, 0);
	public List<KeyInfo> Keys { get; set; } = new()
	{ new(Key.A), new(Key.S), new(Key.D), new(Key.J), new(Key.K), new(Key.L) };
	public string? ExternalUISource { get; set; } = "./Default.xaml";
	public string? ForAllKeysUISource { get; set; } = "./ForEachKey.xaml";

	public List<KeyActionRecord> SpecialHotKeys { get; set; } = new()
	{
		new(Key.Subtract, HotkeyActions.ClearStatistics),
		new(Key.Multiply, HotkeyActions.ToggleClickThrough),
		new(Key.Divide, HotkeyActions.ToggleAlwaysOnTop),
		new(Key.NumLock, HotkeyActions.ToggleHiding),
		new(Key.Add, HotkeyActions.Close)
	}; // bruh why the fuck is system.json not able to serialize tuples
}
