using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class StorageViewModel : BaseViewModel
{
	MediaDevice device;
	private List<string> storages;
	private string selectedStorage;
	private MediaStorageInfo mediaStorageInfo;

	public StorageViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;
		Storages = this.device?.FunctionalObjects(FunctionalCategory.Storage)?.ToList();
		SelectedStorage = Storages?.FirstOrDefault();
	}

	public List<string> Storages
	{
		get => storages;
		set
		{
			if (storages != value)
			{
				storages = value;
				NotifyPropertyChanged(nameof(Storages));
			}
		}
	}

	public string SelectedStorage
	{
		get => selectedStorage;
		set
		{
			if (selectedStorage != value)
			{
				selectedStorage = value;
				if (!string.IsNullOrEmpty(selectedStorage))
					mediaStorageInfo = device?.GetStorageInfo(selectedStorage);
				else
				{
					mediaStorageInfo = null;
				}

				NotifyAllPropertiesChanged();
			}
		}
	}

	public MediaStorageInfo Info => mediaStorageInfo;

	public StorageType Type => mediaStorageInfo?.Type ?? StorageType.Undefined;

	public string FileSystemType => mediaStorageInfo?.FileSystemType;

	public ulong Capacity => mediaStorageInfo?.Capacity ?? 0;

	public ulong FreeSpaceInBytes => mediaStorageInfo?.FreeSpaceInBytes ?? 0;

	public ulong FreeSpaceInObjects => mediaStorageInfo?.FreeSpaceInObjects ?? 0;

	public string Description => mediaStorageInfo?.Description;

	public string SerialNumber => mediaStorageInfo?.SerialNumber;

	public ulong MaxObjectSize => mediaStorageInfo?.MaxObjectSize ?? 0;

	public ulong CapacityInObjects => mediaStorageInfo?.CapacityInObjects ?? 0;

	public StorageAccessCapability AccessCapability => mediaStorageInfo?.AccessCapability ?? 0;
}
