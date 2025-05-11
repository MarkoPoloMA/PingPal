using PingPal.Wpf.Base;
using PingPal.Wpf.Dispatcher;
using PingPal.Wpf.Services.View;

namespace PingPal.Wpf.Extensions;

public static class ViewModelExtensions
{
	/// <summary>
	///     Открытие диалога. Использовать только внутри IDispatcherHelper
	/// </summary>
	/// <param name="viewModel"></param>
	/// <returns></returns>
	public static bool? OpenDialog(this IViewModel viewModel)
	{
		var viewService = Locator.Current.Locate<IViewService>();
#pragma warning disable CS0618
		return viewService.OpenDialog(viewModel);
#pragma warning restore CS0618
	}

	/// <summary>
	///     Открытие диалога. Использовать при отсутствии необходимости результата
	/// </summary>
	/// <param name="viewModel"></param>
	public static void OpenDialogInUi(this IViewModel viewModel)
	{
		var dispatcherHelper = Locator.Current.Locate<IDispatcherHelper>();
		dispatcherHelper.CheckBeginInvokeOnUI(() => viewModel.OpenDialog());
	}

	/// <summary>
	///     Открытие диалога. Использовать при необходимости результата
	/// </summary>
	/// <param name="viewModel"></param>
	/// <returns></returns>
	public static Task<bool?> OpenDialogInUiAsync(this IViewModel viewModel)
	{
		var dispatcherHelper = Locator.Current.Locate<IDispatcherHelper>();
		var tcs = new TaskCompletionSource<bool?>();
		dispatcherHelper.CheckBeginInvokeOnUI(
			() =>
			{
				var res = viewModel.OpenDialog();
				tcs.SetResult(res);
			});

		return tcs.Task;
	}
}