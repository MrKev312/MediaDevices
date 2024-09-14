using MediaDeviceApp.Mvvm;
using MediaDevices;
using System.Collections.Generic;
using System.Linq;

namespace MediaDeviceApp.ViewModel;

    public class ContentViewModel : BaseViewModel
    {
        private readonly MediaDeviceServiceContent content;

        public ContentViewModel(MediaDeviceServiceContent content)
        {
            this.content = content;
            Name = content.Name;
        }
        public string Name { get; private set; }

        public List<ContentViewModel> Contents
        {
            get
            {
                return content?.GetContent()?.Select(c => new ContentViewModel(c)).ToList(); 
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Properties
        {
            get
            {
                return content?.GetAllProperties()?.ToList();
            }
        }
    }
