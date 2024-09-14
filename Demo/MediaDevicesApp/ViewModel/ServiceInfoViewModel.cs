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

	public List<string> SupportedMethods
	{
		get
		{
			return SelectedService?.GetSupportedMethods()?.Select(c => c.ToString()).ToList();
		}
	}

	public List<string> SupportedCommands
	{
		get
		{
			return SelectedService?.GetSupportedCommands()?.Select(c => c.ToString()).ToList();
		}
	}

	public List<string> SupportedEvents
	{
		get
		{
			return SelectedService?.GetSupportedEvents()?.Select(c => c.ToString()).ToList();
		}
	}

	public List<string> SupportedFormats
	{
		get
		{
			return SelectedService?.GetSupportedFormats()?.Select(c => c.ToString()).ToList();
		}
	}

	public List<ContentViewModel> Contents
	{
		get
		{
			return SelectedService?.GetContent()?.Select(c => new ContentViewModel(c)).ToList();
		}
	}

	private ContentViewModel selectedContent;
	public ContentViewModel SelectedContent
	{
		get { return selectedContent; }
		set { selectedContent = value; NotifyPropertyChanged(nameof(SelectedContent)); }
	}
}
