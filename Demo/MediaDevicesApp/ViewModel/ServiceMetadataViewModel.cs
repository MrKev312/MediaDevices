using MediaDevices;

namespace MediaDevicesApp.ViewModel;

public class ServiceMetadataViewModel : ServiceBaseViewModel
{
	public ServiceMetadataViewModel()
	{
		services = MediaDeviceServices.Metadata;
	}
}
