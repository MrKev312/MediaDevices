using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class VendorViewModel : BaseViewModel
{
	MediaDevice device;
	private string description;
	private List<int> opCodes;

	public VendorViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;
		try
		{
			OpCodes = this.device.VendorOpcodes().ToList();
			Description = this.device.VendorExtentionDescription();
		}
		catch
		{
			OpCodes = null;
			Description = null;
		}
	}

	public string Description
	{
		get => description;
		set
		{
			if (description != value)
			{
				description = value;
				NotifyPropertyChanged(nameof(Description));
			}
		}
	}

	public List<int> OpCodes
	{
		get => opCodes;
		set
		{
			if (opCodes != value)
			{
				opCodes = value;
				NotifyPropertyChanged(nameof(OpCodes));
			}
		}
	}

}
