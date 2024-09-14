using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public abstract class ServiceBaseViewModel : BaseViewModel
{
	protected MediaDevice device { get; private set; }
	protected MediaDeviceServices availableServices { get; set; } = MediaDeviceServices.All;

	public virtual void Update(MediaDevice device)
	{
		this.device = device;

		AvailableDeviceServices = this.device?.GetServices(availableServices)?.ToList();
		NotifyAllPropertiesChanged();
	}

	public List<MediaDeviceService> AvailableDeviceServices { get; private set; }

	private MediaDeviceService selectedService;
	public MediaDeviceService SelectedService
	{
		get => selectedService;
		set
		{
			if (selectedService != value)
			{
				//if (this.selectedService != null)
				//{
				//    this.selectedService.Close();
				//}
				selectedService = value;
				//if (this.selectedService != null)
				//{
				//    this.selectedService.Open();
				//}
				NotifyAllPropertiesChanged();
			}
		}
	}
}
