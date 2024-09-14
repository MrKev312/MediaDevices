using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public abstract class ServiceBaseViewModel : BaseViewModel
{
	protected MediaDevice device;
	protected MediaDeviceService selectedService;
	protected MediaDeviceServices services = MediaDeviceServices.All;

	public virtual void Update(MediaDevice device)
	{
		this.device = device;

		Services = this.device?.GetServices(services)?.ToList();
		NotifyAllPropertiesChanged();
	}

	public List<MediaDeviceService> Services { get; private set; }

	public MediaDeviceService SelectedService
	{
		get
		{
			return selectedService;
		}
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
