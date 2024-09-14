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

	public string DeviceId => device?.DeviceId;

	public string Description => device?.Description;

	public string FriendlyName
	{
		get => device?.FriendlyName;
		set
		{
			device.FriendlyName = value;
			NotifyPropertyChanged(nameof(FriendlyName));
		}
	}

	public string Manufacturer => device?.Manufacturer;

	public string SyncPartner => device?.SyncPartner;

	public string FirmwareVersion => device?.FirmwareVersion;

	public string PowerLevel => device?.PowerLevel.ToString(CultureInfo.InvariantCulture);

	public string PowerSource => device?.PowerSource.ToString();

	public string Protocol => device?.Protocol;

	public string Model => device?.Model;

	public string SerialNumber => device?.SerialNumber;

	public string SupportsNonConsumable => device?.SupportsNonConsumable.ToString();

	public string DateTime => device?.DateTime.ToString();

	public string SupportedFormatsAreOrdered => device?.SupportedFormatsAreOrdered.ToString();

	public string DeviceType => device?.DeviceType.ToString();

	public string NetworkIdentifier => device?.NetworkIdentifier.ToString(CultureInfo.InvariantCulture);

	public string FunctionalUniqueId => device?.FunctionalUniqueId?.Select(b => b.ToString(CultureInfo.InvariantCulture)).Aggregate((a, b) => $"{a},{b}");

	public string ModelUniqueId => device?.ModelUniqueId?.Select(b => b.ToString(CultureInfo.InvariantCulture)).Aggregate((a, b) => $"{a},{b}");

	public string Transport => device?.Transport.ToString();

	public string UseDeviceStage => device?.UseDeviceStage.ToString();

	public string PnPDeviceID => device?.PnPDeviceID;
}
