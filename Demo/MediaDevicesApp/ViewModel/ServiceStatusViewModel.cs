using MediaDevices;

namespace MediaDevicesApp.ViewModel;

public class ServiceStatusViewModel : ServiceBaseViewModel
{
	public ServiceStatusViewModel()
	{
		services = MediaDeviceServices.Status;
	}

}
