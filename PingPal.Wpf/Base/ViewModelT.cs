using System.Windows;

namespace PingPal.Wpf.Base;

public abstract class ViewModel<TView> : ViewModel where TView : FrameworkElement, new()
{
	public TView TypedView => (TView)View;

	public override FrameworkElement View
	{
		get => base.View ??= new TView();
		set => base.View = value;
	}
}