﻿using MediaDevices;

using MediaDevicesApp.Mvvm;

using System.Collections.Generic;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class ServicesViewModel : BaseViewModel
{
	private MediaDevice device;

	public ServicesViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;
		NotifyAllPropertiesChanged();
	}

	public List<string> All
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.All)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Contacts
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Contact)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Calendars
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Calendar)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Notes
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Notes)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Tasks
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Task)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Statuses
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Status)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Hints
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Hints)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> DeviceMetadatas
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Metadata)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> Ringtones
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.Ringtone)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> EnumerationSynchronizations
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.EnumerationSynchronization)?.Select(s => s.ToString()).ToList();
		}
	}

	public List<string> AnchorSynchronizations
	{
		get
		{
			return device?.GetServices(MediaDeviceServices.AnchorSynchronization)?.Select(s => s.ToString()).ToList();
		}
	}
}
