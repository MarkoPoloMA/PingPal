using GalaSoft.MvvmLight.Messaging;

namespace PingPal.Wpf;

public class RegesterDemoModel : MainViewModel
{
	public RegesterDemoModel() : base(Messenger.Default, null)
	{
	}
}