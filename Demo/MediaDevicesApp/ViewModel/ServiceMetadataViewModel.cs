using MediaDevices;

namespace MediaDevicesApp.ViewModel;

public class ServiceMetadataViewModel : ServiceBaseViewModel
{
	public ServiceMetadataViewModel() => availableServices = MediaDeviceServices.Metadata;
}
