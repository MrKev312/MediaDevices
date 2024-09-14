using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

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

	public List<string> SupportedCommands => device?.SupportedCommands()?.Select(c => c.ToString()).ToList();

	public List<string> SupportedEvents => device?.SupportedEvents()?.Select(c => c.ToString()).ToList();

	public List<FunctionalCategory> FunctionalCategories
	{
		get => functionalCategories;
		set
		{
			functionalCategories = value;
			NotifyPropertyChanged(nameof(FunctionalCategories));
		}
	}

	public FunctionalCategory SelectedFunctionalCategory
	{
		get => selectedFunctionalCategory;
		set
		{
			selectedFunctionalCategory = value;
			NotifyPropertyChanged(nameof(SelectedFunctionalCategory));
			NotifyPropertyChanged(nameof(FunctionalObjects));
			NotifyPropertyChanged(nameof(SupportedContentTypes));
		}
	}

	public List<string> FunctionalObjects => device?.FunctionalObjects(selectedFunctionalCategory)?.Select(c => c.ToString()).ToList();

	public List<string> SupportedContentTypes => device?.SupportedContentTypes(selectedFunctionalCategory)?.Select(c => c.ToString()).ToList();
}
