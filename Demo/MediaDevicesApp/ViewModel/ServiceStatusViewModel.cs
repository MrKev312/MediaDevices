using MediaDevices;

namespace MediaDeviceApp.ViewModel;

    public class ServiceStatusViewModel : ServiceBaseViewModel
    {
        public ServiceStatusViewModel()
        {
            services = MediaDeviceServices.Status;
        }

    }
