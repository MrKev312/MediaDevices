using MediaDeviceApp.Mvvm;
using MediaDevices;
using System.Collections.Generic;
using System.Linq;

namespace MediaDeviceApp.ViewModel;

    public class CapabilityViewModel : BaseViewModel
    {
        MediaDevice device;
        private List<FunctionalCategory> functionalCategories;
        private FunctionalCategory selectedFunctionalCategory;
        
        public CapabilityViewModel()
        { }

        public void Update(MediaDevice device)
        {
            this.device = device;

            FunctionalCategories = this.device?.FunctionalCategories()?.ToList();
            SelectedFunctionalCategory = FunctionalCategories?.FirstOrDefault() ?? FunctionalCategory.Unknown;

            NotifyAllPropertiesChanged();
        }

        public List<string> SupportedCommands
        {
            get
            {
                return device?.SupportedCommands()?.Select(c => c.ToString()).ToList();
            }
        }

        public List<string> SupportedEvents
        {
            get
            {
                return device?.SupportedEvents()?.Select(c => c.ToString()).ToList();
            }
        }
        
        public List<FunctionalCategory> FunctionalCategories
        {
            get
            {
                return functionalCategories;
            }
            set
            {
                functionalCategories = value;
                NotifyPropertyChanged(nameof(FunctionalCategories));
            }
        }

        public FunctionalCategory SelectedFunctionalCategory
        {
            get
            {
                return selectedFunctionalCategory;
            }
            set
            {
                selectedFunctionalCategory = value;
                NotifyPropertyChanged(nameof(SelectedFunctionalCategory));
                NotifyPropertyChanged(nameof(FunctionalObjects));
                NotifyPropertyChanged(nameof(SupportedContentTypes));
            }
        }

        public List<string> FunctionalObjects
        {
            get
            {
                return device?.FunctionalObjects(selectedFunctionalCategory)?.Select(c => c.ToString()).ToList();
            }
        }

        public List<string> SupportedContentTypes
        {
            get
            {
                return device?.SupportedContentTypes(selectedFunctionalCategory)?.Select(c => c.ToString()).ToList();
            }
        }
    }
