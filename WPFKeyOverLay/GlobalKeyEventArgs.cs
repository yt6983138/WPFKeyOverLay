using System.Windows.Input;

namespace WPFKeyOverLay;
public struct GlobalKeyEventArgs
{
	public bool Handled { get; set; }
	public Key Key { get; set; }

	public GlobalKeyEventArgs(Key key)
	{
		this.Key = key;
	}
}
