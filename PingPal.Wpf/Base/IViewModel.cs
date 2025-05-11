using System.Windows;

namespace PingPal.Wpf.Base;

public interface IViewModel
{
	object Header { get; }
	FrameworkElement View { get; }
}