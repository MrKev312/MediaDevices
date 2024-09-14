using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Globalization;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class InfoViewModel : BaseViewModel
{
	MediaDevice device;

	public InfoViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;
		NotifyAllPropertiesChanged();
	}

	public string DeviceId
	{
		get
		{
			return device?.DeviceId;
		}
	}

	public string Description
	{
		get
		{
			return device?.Description;
		}
	}

	public string FriendlyName
	{
		get
		{
			return device?.FriendlyName;
		}
		set
		{
			device.FriendlyName = value;
			NotifyPropertyChanged(nameof(FriendlyName));
		}
	}

	public string Manufacturer
	{
		get
		{
			return device?.Manufacturer;
		}
	}

	public string SyncPartner
	{
		get
		{
			return device?.SyncPartner;
		}
	}

	public string FirmwareVersion
	{
		get
		{
			return device?.FirmwareVersion;
		}
	}

	public string PowerLevel
	{
		get
		{
			return device?.PowerLevel.ToString(CultureInfo.InvariantCulture);
		}
	}

	public string PowerSource
	{
		get
		{
			return device?.PowerSource.ToString();
		}
	}

	public string Protocol
	{
		get
		{
			return device?.Protocol;
		}
	}

	public string Model
	{
		get
		{
			return device?.Model;
		}
	}

	public string SerialNumber
	{
		get
		{
			return device?.SerialNumber;
		}
	}

	public string SupportsNonConsumable
	{
		get
		{
			return device?.SupportsNonConsumable.ToString();
		}
	}

	public string DateTime
	{
		get
		{
			return device?.DateTime.ToString();
		}
	}

	public string SupportedFormatsAreOrdered
	{
		get
		{
			return device?.SupportedFormatsAreOrdered.ToString();
		}
	}

	public string DeviceType
	{
		get
		{
			return device?.DeviceType.ToString();
		}
	}

	public string NetworkIdentifier
	{
		get
		{
			return device?.NetworkIdentifier.ToString(CultureInfo.InvariantCulture);
		}
	}

	public string FunctionalUniqueId
	{
		get
		{
			return device?.FunctionalUniqueId?.Select(b => b.ToString(CultureInfo.InvariantCulture)).Aggregate((a, b) => $"{a},{b}");
		}
	}

	public string ModelUniqueId
	{
		get
		{
			return device?.ModelUniqueId?.Select(b => b.ToString(CultureInfo.InvariantCulture)).Aggregate((a, b) => $"{a},{b}");
		}
	}

	public string Transport
	{
		get
		{
			return device?.Transport.ToString();
		}
	}

	public string UseDeviceStage
	{
		get
		{
			return device?.UseDeviceStage.ToString();
		}
	}

	public string PnPDeviceID
	{
		get
		{
			return device?.PnPDeviceID;
		}
	}
}
