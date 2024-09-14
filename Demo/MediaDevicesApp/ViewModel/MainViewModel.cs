using MediaDevices;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Media;
using MediaDevicesApp.Mvvm;

namespace MediaDevicesApp.ViewModel;

public class MainViewModel : BaseViewModel
{
	private List<MediaDevice> devices;
	private MediaDevice selectedDevice;
	private bool usePrivateDevices;
	private bool canReset = true;

	public DelegateCommand RefreshCommand { get; private set; }
	public DelegateCommand ResetCommand { get; private set; }
	public DelegateCommand UsbChangedCommand { get; private set; }

	public InfoViewModel Info { get; private set; }
	public CapabilityViewModel Capability { get; private set; }
	public ContentLocationViewModel ContentLocation { get; private set; }
	public StorageViewModel Storage { get; private set; }
	public DriveViewModel Drive { get; private set; }
	public RootViewModel Root { get; private set; }
	public FilesViewModel Files { get; private set; }
	public StillImageViewModel StillImage { get; private set; }
	public SmsViewModel Sms { get; private set; }
	public ExplorerViewModel Explorer { get; private set; }
	public VendorViewModel Vendor { get; private set; }
	public ServicesViewModel Services { get; private set; }
	public ServiceInfoViewModel ServiceInfo { get; private set; }
	public ServiceStatusViewModel ServiceStatus { get; private set; }
	public ServiceMetadataViewModel ServiceMetadata { get; private set; }

	public MainViewModel()
	{
		RefreshCommand = new DelegateCommand(OnRefresh);
		ResetCommand = new DelegateCommand(OnReset);
		UsbChangedCommand = new DelegateCommand(OnUsbChanged);

		Info = new InfoViewModel();
		Capability = new CapabilityViewModel();
		ContentLocation = new ContentLocationViewModel();
		Storage = new StorageViewModel();
		Drive = new DriveViewModel();
		Root = new RootViewModel();
		Files = new FilesViewModel();
		StillImage = new StillImageViewModel();
		Sms = new SmsViewModel();
		Explorer = new ExplorerViewModel();
		Vendor = new VendorViewModel();
		Services = new ServicesViewModel();
		ServiceInfo = new ServiceInfoViewModel();
		ServiceStatus = new ServiceStatusViewModel();
		ServiceMetadata = new ServiceMetadataViewModel();

		OnRefresh();
	}

	public bool UsePrivateDevices
	{
		get => usePrivateDevices;
		set
		{
			if (usePrivateDevices != value)
			{
				usePrivateDevices = value;
				OnRefresh();
				NotifyPropertyChanged(nameof(UsePrivateDevices));
			}
		}
	}

	private void OnRefresh()
	{
		if (usePrivateDevices)
			Devices = MediaDevice.GetPrivateDevices().ToList();
		else
		{
			Devices = MediaDevice.GetDevices().ToList();
		}

		if (selectedDevice == null)
			SelectedDevice = Devices.FirstOrDefault();
	}

	private void OnUsbChanged()
	{
		SystemSounds.Beep.Play();
		if (usePrivateDevices)
			Devices = MediaDevice.GetPrivateDevices().ToList();
		else
		{
			Devices = MediaDevice.GetDevices().ToList();
		}

		if (selectedDevice == null)
			SelectedDevice = Devices.FirstOrDefault();
	}

	public List<MediaDevice> Devices
	{
		get => devices;
		set { devices = value; NotifyPropertyChanged(nameof(Devices)); }
	}

	public MediaDevice SelectedDevice
	{
		get => selectedDevice;
		set
		{
			if (value != selectedDevice)
			{
				if (selectedDevice != null)
				{
					try
					{
						selectedDevice.Disconnect();
					}
					catch { }
				}

				selectedDevice = value;
				if (selectedDevice != null)
				{
					selectedDevice.Connect();

					canReset = true;
				}
				else
				{
					canReset = false;
				}

				NotifyAllPropertiesChanged();

				Info.Update(selectedDevice);
				Capability.Update(selectedDevice);
				ContentLocation.Update(selectedDevice);
				Storage.Update(selectedDevice);
				Drive.Update(selectedDevice);
				Root.Update(selectedDevice);
				Files.Update(selectedDevice);
				StillImage.Update(selectedDevice);
				Sms.Update(selectedDevice);
				Explorer.Update(selectedDevice);
				Vendor.Update(selectedDevice);
				Services.Update(selectedDevice);
				ServiceInfo.Update(selectedDevice);
				ServiceStatus.Update(selectedDevice);
				ServiceMetadata.Update(selectedDevice);
				//if (selectedDevice.Description != "My Passport 25E2")
				//{
				//    var root = selectedDevice.GetRootDirectory();
				//    var result = root.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).ToList();
				//    var files = result.OfType<MediaFileInfo>().ToList();
				//}

			}
		}
	}

	private void OnReset()
	{
		if (MsgBox.ShowQuestion("Do your really want to reset your device?"))
		{
			try
			{
				selectedDevice.ResetDevice();
			}
			catch (Exception ex)
			{
				MsgBox.ShowError(ex.Message);
			}
		}
	}

	public bool CanReset
	{
		get => canReset;
		set { canReset = value; NotifyPropertyChanged(nameof(CanReset)); }

	}
}
