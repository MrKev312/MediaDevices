using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class ServiceInfoViewModel : ServiceBaseViewModel
{
	public ServiceInfoViewModel()
	{
	}

	public List<KeyValuePair<string, string>> Properties
	{
		get
		{
			List<KeyValuePair<string, string>> x = SelectedService?.GetAllProperties()?.ToList();
			return x;
		}
	}

	public List<string> SupportedMethods => SelectedService?.GetSupportedMethods()?.Select(c => c.ToString()).ToList();

	public List<string> SupportedCommands => SelectedService?.GetSupportedCommands()?.Select(c => c.ToString()).ToList();

	public List<string> SupportedEvents => SelectedService?.GetSupportedEvents()?.Select(c => c.ToString()).ToList();

	public List<string> SupportedFormats => SelectedService?.GetSupportedFormats()?.Select(c => c.ToString()).ToList();

	public List<ContentViewModel> Contents => SelectedService?.GetContent()?.Select(c => new ContentViewModel(c)).ToList();

	private ContentViewModel selectedContent;
	public ContentViewModel SelectedContent
	{
		get => selectedContent;
		set { selectedContent = value; NotifyPropertyChanged(nameof(SelectedContent)); }
	}
}
