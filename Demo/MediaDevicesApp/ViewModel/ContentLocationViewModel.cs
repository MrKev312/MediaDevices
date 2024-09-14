using MediaDevices;

using MediaDevicesApp.Mvvm;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MediaDevicesApp.ViewModel;

public class ContentLocationViewModel : BaseViewModel
{
	MediaDevice device;
	private bool isContentLocationSupported = true;
	private List<ContentType> contents;
	private ContentType selectedContent;

	public ContentLocationViewModel()
	{ }

	public void Update(MediaDevice device)
	{
		this.device = device;

		// needed for Nicon
		try
		{
			Contents = Enum.GetValues(typeof(ContentType)).Cast<ContentType>().Where(c => this.device?.GetContentLocations(c)?.Any() ?? false).ToList();
		}
		catch
		{
			Contents = null;
		}

		SelectedContent = Contents?.FirstOrDefault() ?? ContentType.Unknown;
	}

	public bool IsContentLocationSupported
	{
		get
		{
			return isContentLocationSupported;
		}
		set
		{
			isContentLocationSupported = value;
			NotifyPropertyChanged(nameof(IsContentLocationSupported));
		}
	}

	//public List<ContentType> Contents
	//{
	//    get { return Enum.GetValues(typeof(ContentType)).Cast<ContentType>().ToList(); }
	//}

	public List<ContentType> Contents
	{
		get
		{
			return contents;
		}
		set
		{
			contents = value;
			NotifyPropertyChanged(nameof(Contents));
		}
	}

	public ContentType SelectedContent
	{
		get
		{
			return selectedContent;
		}
		set
		{
			selectedContent = value;
			NotifyPropertyChanged(nameof(SelectedContent));
			NotifyPropertyChanged(nameof(Locations));
		}
	}

	public List<string> Locations
	{
		get
		{
			// needed for Nicon
			try
			{
				return device?.GetContentLocations(selectedContent)?.ToList();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return null;
		}
	}

}
