using System.Windows.Threading;

namespace PingPal.Wpf.Dispatcher;

public class DispatcherHelper : IDispatcherHelper
{
	public System.Windows.Threading.Dispatcher UiDispatcher => GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher;

	public DispatcherHelper()
	{
		GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();
	}

	public void CheckBeginInvokeOnUI(Action action)
	{
		GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(action);
	}

	public DispatcherOperation RunAsync(Action action)
	{
		return GalaSoft.MvvmLight.Threading.DispatcherHelper.RunAsync(action);
	}

	public void Reset()
	{
		GalaSoft.MvvmLight.Threading.DispatcherHelper.Reset();
	}
}