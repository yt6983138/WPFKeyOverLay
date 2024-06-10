using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFKeyOverLay;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private enum ResizeDirection
	{
		Left = 61441,
		Right = 61442,
		Top = 61443,
		Bottom = 61446,
		BottomRight = 61448
	}

	#region Porperties
	public App App { get; init; } = (App)Application.Current;

	public GlobalKeyboardHook KeyboardHook { get; private set; } = new();
	public TickedExecuter CanvasUpdater { get; private set; } = new();
	public TickedExecuter KpsUpdater { get; private set; } = new();

	public bool IsClickThroughMode { get; private set; } = false;
	#endregion

	#region Constructor
	public MainWindow()
	{
		this.CanvasUpdater.Tps = this.App.Config.TrailUpdateFps;
		this.CanvasUpdater.OnTicked += this.CanvasUpdater_OnTicked;
		this.CanvasUpdater.Start();

		this.KpsUpdater.Tps = this.App.Config.KpsUpdatesPerSecond;
		this.KpsUpdater.OnTicked += this.KpsUpdater_OnTicked;
		this.KpsUpdater.Start();

		this.WindowStyle = WindowStyle.None;
		this.AllowsTransparency = true;
		this.Opacity = this.App.Config.WindowOpacity;
		this.Background = this.App.Config.WindowBackground;
		this.MouseMove += this.MainWindow_MouseMove;

		this.KeyboardHook.KeyDown += this.Keyboard_Down;
		this.KeyboardHook.KeyUp += this.Keyboard_Up;

		this.InitializeComponent();
		this.DataContext = this.Resources;

		this.Root.Margin = this.App.Config.WindowPadding;

		this.MainLayout.RowDefinitions.Add(new() { Height = new(1, GridUnitType.Star) });

		if (this.App.Config.ExternalUISource is not null)
		{
			UIElement ui = (UIElement)XamlReader.Load(new FileStream(this.App.Config.ExternalUISource, FileMode.Open));
			this.Root.Children.Insert(1, ui);
		}

		this.Width = this.App.Config.Keys.Sum(x => x.Width);
		this.Height = this.App.Config.DefaultHeight;

		int i = 0;
		foreach (KeyInfo key in this.App.Config.Keys)
		{
			this.MainLayout.ColumnDefinitions.Add(new() { Width = new(key.Width) });

			Button button = new()
			{
				Margin = key.Margin,
				IsEnabled = true,
				Content = key.Text,
				Foreground = key.TextColor,
				Background = key.BeforeClickButtonColor,
				Focusable = false,
				IsHitTestVisible = false,
				FontSize = key.TextSize
			};
			Canvas canvas = new()
			{
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Background = key.TrailBackgroundColor
			};
			Grid grid = new()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				RowDefinitions =
				{
					new()
					{
						Height = new(1, GridUnitType.Star)
					},
					new()
					{
						Height = new(this.App.Config.KeyButtonHeight)
					}
				}
			};

			this.MainLayout.Children.Add(grid);
			grid.Children.Add(canvas);
			grid.Children.Add(button);
			Grid.SetColumn(grid, i);
			Grid.SetRow(button, 1);
			Grid.SetRow(canvas, 0);
			key.Button = button;
			key.Canvas = canvas;
			key.KpsHandler = new(this.App.Config.KpsUpdatesPerSecond);

			i++;
		}


		if (this.App.Config.ForAllKeysUISource is not null)
		{
			string original = File.ReadAllText(this.App.Config.ForAllKeysUISource, Encoding.UTF8);
			for (int j = 0; j < this.App.Config.Keys.Count; j++)
			{
				string modified = original
					.Replace(@"$CurrentKey$", this.App.Config.Keys[j].Key.ToString())
					.Replace(@"$CurrentKey\$", @"$CurrentKey$");

				UIElement ui = (UIElement)XamlReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(modified)));
				this.MainLayout.Children.Add(ui);
				Grid.SetColumn(ui, j);
			}
		}
	}
	#endregion

	#region Ticking
	private void KpsUpdater_OnTicked(object? sender, EventArgs e)
	{
		this.Dispatcher.Invoke(() =>
		{
			foreach (KeyInfo key in this.App.Config.Keys)
			{
				this.Resources[$"Key.{key.Key}.Kps"] = key.KpsHandler.Kps;
				this.Resources[$"Key.{key.Key}.PeakKps"] = key.KpsHandler.PeakKps;
			}

			this.Resources[$"Key.All.Kps"] = this.App.Config.Keys.Sum(x => x.KpsHandler.Kps);
			this.Resources[$"Key.All.PeakKps"] = this.App.Config.Keys.Sum(x => x.KpsHandler.PeakKps);
		});
	}
	private void CanvasUpdater_OnTicked(object? sender, EventArgs e)
	{
		float shifts = this.App.Config.PixelPerSecond / this.App.Config.TrailUpdateFps;

		this.Dispatcher.Invoke(() =>
		{
			foreach (KeyInfo key in this.App.Config.Keys)
			{
				if (key.Canvas is null) continue;

				foreach (UIElement obj in key.Canvas.Children)
				{
					if (obj is not Rectangle rect) continue;

					if (rect == key.LastRectangle) continue;

					double bottom = Canvas.GetBottom(rect);

					if (bottom > key.Canvas.Height)
					{
						key.Canvas.Children.Remove(rect);
					}

					Canvas.SetBottom(rect, bottom + shifts);
				}

				if (key.LastRectangle is not null)
				{
					double bottom = Canvas.GetBottom(key.LastRectangle);

					if (bottom > key.Canvas.Height) key.Canvas.Children.Remove(key.LastRectangle);

					key.LastRectangle.Height += shifts;
					continue;
				}

				if (!key.HasBeenHolding) continue;

				key.LastRectangle = new()
				{
					Height = shifts,
					Width = key.Width - key.TrailMargin.Right - key.TrailMargin.Left,
					Fill = key.TrailColor,
					VerticalAlignment = VerticalAlignment.Bottom,
					HorizontalAlignment = HorizontalAlignment.Left
				};

				key.Canvas.Children.Add(key.LastRectangle);
				Canvas.SetBottom(key.LastRectangle, 0);
				Canvas.SetLeft(key.LastRectangle, key.Margin.Left);
			}
		});
	}
	#endregion

	#region Keyboard handling
	private void Keyboard_Up(object sender, GlobalKeyEventArgs e)
	{
		foreach (KeyInfo key in this.App.Config.Keys)
		{
			if (key.Key != e.Key) goto Final;

			if (key.Button is null) continue;

			key.Button.Background = key.BeforeClickButtonColor;

		Final:
			key.HasBeenHolding = false;
			key.LastRectangle = null;
		}
	}
	private void Keyboard_Down(object sender, GlobalKeyEventArgs e)
	{
		foreach (KeyInfo key in this.App.Config.Keys)
		{
			if (key.Key != e.Key) continue;

			if (key.Button is null) goto Final;

			key.Button.Background = key.AfterClickButtonColor;

			if (key.Canvas is null) goto Final;

			Final:
			if (!key.HasBeenHolding)
			{
				key.KpsHandler.Update(1);
				key.ClickCount++;
			}

			key.HasBeenHolding = true;

			this.Resources[$"Key.{key.Key}.ClickCount"] = key.ClickCount;
			continue;
		}
		this.Resources["Key.All.ClickCount"] = this.App.Config.Keys.Sum(x => x.ClickCount);

		foreach ((Key Key, HotkeyActions HotkeyActions) hotkey in this.App.Config.SpecialHotKeys)
		{
			if (hotkey.Key != e.Key) continue;

			switch (hotkey.HotkeyActions)
			{
				case HotkeyActions.ClearStatistics:
					this.ClearStatistics();
					break;
				case HotkeyActions.ToggleClickThrough:
					this.ToggleClickThrough();
					break;
				case HotkeyActions.ToggleAlwaysOnTop:
					this.ToggleAlwaysOnTop();
					break;
				case HotkeyActions.ToggleHiding:
					this.ToggleHiding();
					break;
				case HotkeyActions.Close:
					this.Close();
					break;
				default:
					throw new Exception($"Unhandled hotkey action {hotkey.HotkeyActions}");
			}
		}
	}
	#endregion

	#region Special actions
	private void ClearStatistics()
	{
		foreach (DictionaryEntry pair in this.Resources)
		{
			this.Resources[pair.Key] = 0;
		}

		foreach (KeyInfo key in this.App.Config.Keys)
		{
			key.KpsHandler.Clear();
			key.KpsHandler.PeakKps = 0;
		}
	}
	private void ToggleClickThrough()
	{
		this.IsClickThroughMode = !this.IsClickThroughMode;
		if (this.IsClickThroughMode)
		{
			nint windowHwnd = new WindowInteropHelper(this).Handle;
			SetWindowExTransparent(windowHwnd);
		}
		else
		{
			nint windowHwnd = new WindowInteropHelper(this).Handle;
			SetWindowExNotTransparent(windowHwnd);
		}
	}
	private void ToggleAlwaysOnTop()
		=> this.Topmost = !this.Topmost;
	private void ToggleHiding()
		=> this.WindowState = this.WindowState == WindowState.Minimized ? WindowState.Normal : WindowState.Minimized;
	#endregion

	#region Resize handling
	private void WindowResizeNorth(object sender, MouseButtonEventArgs e)
	{
		HwndSource? hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
		SendMessage(hwndSource!.Handle, 0x112, (IntPtr)ResizeDirection.Top, IntPtr.Zero);
	}
	private void WindowResizeSouth(object sender, MouseButtonEventArgs e)
	{
		HwndSource? hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
		SendMessage(hwndSource!.Handle, 0x112, (IntPtr)ResizeDirection.Bottom, IntPtr.Zero);
	}
	private void BorderHorizontal_OnMouseEnter(object sender, MouseEventArgs e)
	{
		Mouse.OverrideCursor = Cursors.SizeNS;
	}
	private void BorderAll_OnMouseLeave(object sender, MouseEventArgs e)
	{
		Mouse.OverrideCursor = Cursors.Arrow;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
	#endregion

	#region Misc
	private void MainWindow_MouseMove(object sender, MouseEventArgs e)
	{
		if (this.IsClickThroughMode)
		{
			return;
		}

		if (e.LeftButton != MouseButtonState.Pressed)
		{
			return;
		}
		this.DragMove();
	}
	#endregion

	#region Click through handling 
	private const int WS_EX_TRANSPARENT = 0x00000020;
	private const int GWL_EXSTYLE = -20;

	[DllImport("user32.dll")]
	private static extern int GetWindowLong(IntPtr hwnd, int index);

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

	public static void SetWindowExTransparent(IntPtr hwnd)
	{
		int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
		SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
	}

	public static void SetWindowExNotTransparent(IntPtr hwnd)
	{
		int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
		SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
	}
	#endregion

}