using System.Windows;
using GalaSoft.MvvmLight;

namespace PingPal.Wpf.Base;

public abstract class ViewModel : ViewModelBase, IViewModel
{
	private FrameworkElement _view;

	public abstract object Header { get; }

	public virtual FrameworkElement View
	{
		get => _view;
		set
		{
			if (Set(ref _view, value) && _view != null)
				_view.DataContext = this;
		}
	}
}