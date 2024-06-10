using System.Runtime.InteropServices;
using System.Windows.Input;

namespace WPFKeyOverLay;
public class GlobalKeyboardHook : IDisposable
{
	#region Constant, Structure and Delegate Definitions
	/// <summary>
	/// defines the callback type for the Hook
	/// </summary>
	private delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHook lParam);
	public delegate void GlobalKeyEventHandler(object sender, GlobalKeyEventArgs args);

	public struct KeyboardHook
	{
		public int vkCode;
		public int scanCode;
		public int flags;
		public int time;
		public int dwExtraInfo;
	}

	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x100;
	private const int WM_KEYUP = 0x101;
	private const int WM_SYSKEYDOWN = 0x104;
	private const int WM_SYSKEYUP = 0x105;
	#endregion

	#region Instance Variables
	/// <summary>
	/// Handle to the Hook, need this to Unhook and call the next Hook
	/// </summary>
	private IntPtr HookHandle = IntPtr.Zero;
	private KeyboardHookProc _hookProcDelegate;
	#endregion

	#region Events
	/// <summary>
	/// Occurs when one of the hooked keys is pressed
	/// </summary>
	public event GlobalKeyEventHandler? KeyDown;
	/// <summary>
	/// Occurs when one of the hooked keys is released
	/// </summary>
	public event GlobalKeyEventHandler? KeyUp;
	#endregion

	#region Constructors and Destructors
	/// <summary>
	/// Initializes a new instance of the <see cref="GlobalKeyboardHook"/> class and installs the keyboard Hook.
	/// </summary>
	public GlobalKeyboardHook()
	{
		this._hookProcDelegate = this.HookProc;
		this.Hook();
	}

	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the
	/// <see cref="GlobalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard Hook.
	/// </summary>
	~GlobalKeyboardHook()
	{
		this.Unhook();
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Installs the global Hook
	/// </summary>
	public void Hook()
	{
		IntPtr hInstance = LoadLibrary("User32");
		this.HookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, this._hookProcDelegate, hInstance, 0);
	}

	/// <summary>
	/// Uninstalls the global Hook
	/// </summary>
	public void Unhook()
	{
		UnhookWindowsHookEx(this.HookHandle);
	}

	/// <summary>
	/// The callback for the keyboard Hook
	/// </summary>
	/// <param name="code">The Hook code, if it isn't >= 0, the function shouldn't do anyting</param>
	/// <param name="wParam">The event type</param>
	/// <param name="lParam">The keyhook event information</param>
	/// <returns></returns>
	public int HookProc(int code, int wParam, ref KeyboardHook lParam)
	{
		if (code >= 0)
		{
			Key key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
			if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
			{
				this.KeyDown?.Invoke(this, new(key));
			}
			else if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
			{
				this.KeyUp?.Invoke(this, new(key));
			}
		}
		return CallNextHookEx(this.HookHandle, code, wParam, ref lParam);
	}

	void IDisposable.Dispose()
	{
		this.Unhook();
		GC.SuppressFinalize(this);
	}
	#endregion

	#region DLL imports
	/// <summary>
	/// Sets the windows Hook, do the desired event, one of hInstance or threadId must be non-null
	/// </summary>
	/// <param name="idHook">The id of the event you want to Hook</param>
	/// <param name="callback">The callback.</param>
	/// <param name="hInstance">The handle you want to attach the event to, can be null</param>
	/// <param name="threadId">The thread you want to attach the event to, can be null</param>
	/// <returns>a handle to the desired Hook</returns>
	[DllImport("user32.dll")]
	private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

	/// <summary>
	/// Unhooks the windows Hook.
	/// </summary>
	/// <param name="hInstance">The Hook handle that was returned from SetWindowsHookEx</param>
	/// <returns>True if successful, false otherwise</returns>
	[DllImport("user32.dll")]
	private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

	/// <summary>
	/// Calls the next Hook.
	/// </summary>
	/// <param name="idHook">The Hook id</param>
	/// <param name="nCode">The Hook code</param>
	/// <param name="wParam">The wparam.</param>
	/// <param name="lParam">The lparam.</param>
	/// <returns></returns>
	[DllImport("user32.dll")]
	private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHook lParam);

	/// <summary>
	/// Loads the library.
	/// </summary>
	/// <param name="lpFileName">Name of the library</param>
	/// <returns>A handle to the library</returns>
	[DllImport("kernel32.dll")]
	private static extern IntPtr LoadLibrary(string lpFileName);
	#endregion
}
