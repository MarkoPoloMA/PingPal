using System.Windows;

namespace PingPal.Wpf.Services.MessageBox;

public class MessageBoxService : IMessageBoxService
{
	public void Show(string text)
	{
		System.Windows.MessageBox.Show(text);
	}

	public void Show(string text, string caption)
	{
		System.Windows.MessageBox.Show(text, caption);
	}

	public void Show(string text, string caption, MessageBoxButton button)
	{
		System.Windows.MessageBox.Show(text, caption, button);
	}

	public void Show(string text, string caption, MessageBoxButton button, MessageBoxImage image)
	{
		System.Windows.MessageBox.Show(text, caption, button, image);
	}

	public void Show(string text, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult defaultResult)
	{
		System.Windows.MessageBox.Show(text, caption, button, image, defaultResult);
	}

	public void Show(string text, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult defaultResult, MessageBoxOptions options)
	{
		System.Windows.MessageBox.Show(text, caption, button, image, defaultResult, options);
	}
}