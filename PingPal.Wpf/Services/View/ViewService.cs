using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using Grace.DependencyInjection;
using PingPal.Wpf.Base;
using PingPal.Wpf.Messages;
using System.Windows;

namespace PingPal.Wpf.Services.View;

public class ViewService : IViewService, IDisposable
{
    private readonly IMessenger _messenger;
    private readonly ILocatorService _locatorService;
    private readonly List<Window> _openedAllWindows;
    private readonly List<Window> _openedMainWindows;

    private readonly object _lockObject = new();

    public ViewService(
        IMessenger messenger,
        ILocatorService locatorService)
    {
        _messenger = messenger;
        _locatorService = locatorService;
        _openedAllWindows = new List<Window>();
        _openedMainWindows = new List<Window>();

        _messenger.Register<RequestCloseMessage>(this, OnRequestClose);
    }

    public void OpenWindow<TViewModel>(WindowMode windowMode = WindowMode.LastActiveOwner) where TViewModel : IViewModel
    {
        CreateWindow<TViewModel>(windowMode).Show();
    }

    public void OpenWindow(IViewModel viewModel, WindowMode windowMode = WindowMode.LastActiveOwner)
    {
        CreateWindow(viewModel, windowMode).Show();
    }

    public bool? OpenDialog<TViewModel>(WindowMode windowMode = WindowMode.LastActiveOwner) where TViewModel : IViewModel
    {
        return CreateWindow<TViewModel>(windowMode).ShowDialog();
    }

    public bool? OpenDialog(IViewModel viewModel, WindowMode windowMode = WindowMode.LastActiveOwner)
    {
        return CreateWindow(viewModel, windowMode).ShowDialog();
    }

    public Window CreateWindow<TViewModel>(WindowMode windowMode) where TViewModel : IViewModel
    {
        var viewModel = _locatorService.Locate<TViewModel>();
        return CreateWindow(viewModel, windowMode);
    }

    public Window CreateWindow(IViewModel viewModel, WindowMode windowMode)
    {
        var window = (Window)viewModel.View;
        window.DataContext = viewModel;
        window.Closed += OnClosed;

        lock (_lockObject)
        {
            var lastOpened = windowMode switch
            {
                WindowMode.LastMainOwner => _openedMainWindows.LastOrDefault(),
                WindowMode.LastActiveOwner => _openedAllWindows.LastOrDefault(w => w.IsActive) ?? _openedMainWindows.LastOrDefault(),
                _ => null
            };

            if (lastOpened != null && !Equals(window, lastOpened))
                window.Owner = lastOpened;

            _openedAllWindows.Add(window);
            if (windowMode is WindowMode.Main)
                _openedMainWindows.Add(window);
        }

        return window;
    }

    public int GetOpenedWindowsCount()
    {
        lock (_lockObject)
            return _openedAllWindows.Count;
    }

    private void OnRequestClose(RequestCloseMessage message)
    {
        var window = _openedAllWindows.SingleOrDefault(w => w.DataContext == message.ViewModel);
        if (window == null)
            return;

        if (message.DialogResult != null)
            window.DialogResult = message.DialogResult;
        else
            window.Close();
    }

    private void OnClosed(object sender, EventArgs e)
    {
        var window = (Window)sender;
        window.Closed -= OnClosed;
        (window.DataContext as ICleanup)?.Cleanup();

        lock (_lockObject)
        {
            _openedAllWindows.Remove(window);
            _openedMainWindows.Remove(window);
        }
    }

    public void Dispose()
    {
        _messenger.Unregister(this);
    }
}