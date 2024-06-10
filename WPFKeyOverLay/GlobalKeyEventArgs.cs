using System.Windows.Input;

namespace WPFKeyOverLay;
public class GlobalKeyEventArgs : EventArgs
{
	public bool Handled { get; set; }
	public Key Key { get; set; }

	public GlobalKeyEventArgs(Key key)
	{
		this.Key = key;
	}
}
