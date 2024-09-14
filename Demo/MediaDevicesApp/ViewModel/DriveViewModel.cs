using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class DriveViewModel : BaseViewModel
{
	MediaDevice device;
	private List<MediaDriveInfo> drives;
	private MediaDriveInfo selectedDrive;
	public DelegateCommand EjectCommand { get; private set; }
	public DelegateCommand FormatCommand { get; private set; }

	public DriveViewModel()
	{
		EjectCommand = new DelegateCommand(OnEject, () => SelectedDrive != null);
		FormatCommand = new DelegateCommand(OnFormat, () => SelectedDrive != null);
	}

	public void Update(MediaDevice device)
	{
		this.device = device;
		Drives = device?.GetDrives()?.ToList();
		SelectedDrive = Drives?.FirstOrDefault();
	}

	public List<MediaDriveInfo> Drives
	{
		get
		{
			return drives;
		}
		set
		{
			if (drives != value)
			{
				drives = value;
				NotifyPropertyChanged(nameof(Drives));
			}
		}
	}

	public MediaDriveInfo SelectedDrive
	{
		get
		{
			return selectedDrive;
		}
		set
		{
			if (selectedDrive != value)
			{
				selectedDrive = value;
				NotifyPropertyChanged(nameof(SelectedDrive));
			}
		}
	}

	private void OnEject()
	{
		SelectedDrive?.Eject();
	}

	private void OnFormat()
	{
		SelectedDrive?.Format();
	}
}
