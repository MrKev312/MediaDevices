using MediaDevices;

namespace MediaDeviceApp.ViewModel;

    public class ServiceMetadataViewModel : ServiceBaseViewModel
    {
        public ServiceMetadataViewModel()
        {
            services = MediaDeviceServices.Metadata;
        }
    }
