using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFKeyOverLay;
public class Config
{
	public float PixelPerSecond { get; set; } = 200;
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

	public List<(Key Key, HotkeyActions HotkeyActions)> SpecialHotKeys { get; set; } = new()
	{
		(Key.Subtract, HotkeyActions.ClearStatistics),
		(Key.Multiply, HotkeyActions.ToggleClickThrough),
		(Key.Divide, HotkeyActions.ToggleAlwaysOnTop),
		(Key.NumLock, HotkeyActions.ToggleHiding),
		(Key.Add, HotkeyActions.Close)
	};
}
