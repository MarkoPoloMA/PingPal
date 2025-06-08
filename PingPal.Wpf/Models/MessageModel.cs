using GalaSoft.MvvmLight;

namespace PingPal.Wpf.Models;

public class MessageModel : ViewModelBase, ICloneable
{
	public object Clone()
	{
		return MemberwiseClone();
	}
}