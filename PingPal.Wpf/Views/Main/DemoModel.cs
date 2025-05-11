using GalaSoft.MvvmLight.Messaging;

namespace PingPal.Wpf;

public class DemoModel : MainViewModel
{
	public DemoModel() : base(Messenger.Default, null)
	{
	}
}