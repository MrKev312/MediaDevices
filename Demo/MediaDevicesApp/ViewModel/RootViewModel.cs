using MediaDevices;

using MediaDevicesApp.Mvvm;

namespace MediaDevicesApp.ViewModel;

public class RootViewModel : BaseViewModel
{
	MediaDevice device;
	MediaDirectoryInfo root;

	public RootViewModel()
	{

	}

	public void Update(MediaDevice device)
	{
		this.device = device;
		root = this.device?.GetRootDirectory();
		NotifyAllPropertiesChanged();
	}

	public string Id => root?.Id;

	public string Name => root?.Name;

	public string FullName => root?.FullName;

	public ulong Length => root?.Length ?? 0;

	public string Attributes => root?.Attributes.ToString();

}
