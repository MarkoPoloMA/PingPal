using PingPal.Wpf.Logic;
using System.IO;
using System.Windows;


namespace PingPal.Wpf;
public partial class App : Application
{
	private readonly IMainWindowProvider _mainWindowProvider;

	public App(IMainWindowProvider mainWindowProvider)
	{
		_mainWindowProvider = mainWindowProvider;

		InitializeComponent();
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		_mainWindowProvider.Show();
	}
}