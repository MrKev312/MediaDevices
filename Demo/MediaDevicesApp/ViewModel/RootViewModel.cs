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

	public string Id
	{
		get
		{
			return root?.Id;
		}
	}

	public string Name
	{
		get
		{
			return root?.Name;
		}
	}

	public string FullName
	{
		get
		{
			return root?.FullName;
		}
	}

	public ulong Length
	{
		get
		{
			return root?.Length ?? 0;
		}
	}

	public string Attributes
	{
		get
		{
			return root?.Attributes.ToString();
		}
	}

}
