using GalaSoft.MvvmLight.Messaging;
using PingPal.Wpf.Base;
using PingPal.Wpf.Services.MessageBox;
using PingPal.Wpf.Views.Main;

namespace PingPal.Wpf;

public class RegesterViewModel : ViewModel<MainWindow>
{
	private readonly IMessenger _messenger;
	private readonly IMessageBoxService _messageBoxService;

	public override object Header => "NameWindow";

	public RegesterViewModel(
		IMessenger messenger,
		IMessageBoxService messageBoxService)
	{
		_messenger = messenger;
		_messageBoxService = messageBoxService;

	}
	
	public override void Cleanup()
	{
		base.Cleanup();

		_messenger.Unregister(this);
	}
}