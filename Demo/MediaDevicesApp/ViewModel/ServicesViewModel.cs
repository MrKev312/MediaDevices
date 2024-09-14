using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class ServicesViewModel : BaseViewModel
{
	private MediaDevice device;

	public ServicesViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;
		NotifyAllPropertiesChanged();
	}

	public List<string> All => device?.GetServices(MediaDeviceServices.All)?.Select(s => s.ToString()).ToList();

	public List<string> Contacts => device?.GetServices(MediaDeviceServices.Contact)?.Select(s => s.ToString()).ToList();

	public List<string> Calendars => device?.GetServices(MediaDeviceServices.Calendar)?.Select(s => s.ToString()).ToList();

	public List<string> Notes => device?.GetServices(MediaDeviceServices.Notes)?.Select(s => s.ToString()).ToList();

	public List<string> Tasks => device?.GetServices(MediaDeviceServices.Task)?.Select(s => s.ToString()).ToList();

	public List<string> Statuses => device?.GetServices(MediaDeviceServices.Status)?.Select(s => s.ToString()).ToList();

	public List<string> Hints => device?.GetServices(MediaDeviceServices.Hints)?.Select(s => s.ToString()).ToList();

	public List<string> DeviceMetadatas => device?.GetServices(MediaDeviceServices.Metadata)?.Select(s => s.ToString()).ToList();

	public List<string> Ringtones => device?.GetServices(MediaDeviceServices.Ringtone)?.Select(s => s.ToString()).ToList();

	public List<string> EnumerationSynchronizations => device?.GetServices(MediaDeviceServices.EnumerationSynchronization)?.Select(s => s.ToString()).ToList();

	public List<string> AnchorSynchronizations => device?.GetServices(MediaDeviceServices.AnchorSynchronization)?.Select(s => s.ToString()).ToList();
}
