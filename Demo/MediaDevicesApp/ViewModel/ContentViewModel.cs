using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class ContentViewModel(MediaDeviceServiceContent content) : BaseViewModel
{
	public string Name { get; private set; } = content.Name;

	public List<ContentViewModel> Contents => content?.GetContent()?.Select(c => new ContentViewModel(c)).ToList();

	public IEnumerable<KeyValuePair<string, string>> Properties => content?.GetAllProperties()?.ToList();
}
