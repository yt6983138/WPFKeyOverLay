using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFKeyOverLay;
public class KeyInfo
{
	private string? _text = null;

	public float Width { get; set; } = 64;
	public SolidColorBrush BeforeClickButtonColor { get; set; } = new(Color.FromArgb(200, 255, 255, 255));
	public SolidColorBrush AfterClickButtonColor { get; set; } = new(Color.FromArgb(200, 150, 150, 150));
	public SolidColorBrush TextColor { get; set; } = new(Color.FromArgb(255, 0, 0, 0));
	public SolidColorBrush TrailColor { get; set; } = new(Color.FromArgb(200, 150, 150, 150));
	public SolidColorBrush TrailBackgroundColor { get; set; } = new(Color.FromArgb(0, 0, 0, 0));
	public Thickness TrailMargin { get; set; } = new(5, 0, 5, 0);
	public Thickness Margin { get; set; } = new(5, 0, 5, 5 + 48);
	public Key Key { get; set; }
	public float TextSize { get; set; } = 11.4514f;
	public string Text
	{
		get => this._text ?? this.Key.ToString();
		set => this._text = value;
	}

	[JsonIgnore]
	public Button? Button { get; set; } = null;
	[JsonIgnore]
	public Canvas? Canvas { get; set; } = null;
	[JsonIgnore]
	public Rectangle? LastRectangle { get; set; } = null;
	[JsonIgnore]
	public KpsHandler KpsHandler { get; set; } = new();

	[JsonIgnore]
	public bool HasBeenHolding { get; set; } = false;
	[JsonIgnore]
	public int ClickCount { get; set; } = 0;

	public KeyInfo()
	{
	}
	public KeyInfo(Key key)
	{
		this.Key = key;
	}
}
