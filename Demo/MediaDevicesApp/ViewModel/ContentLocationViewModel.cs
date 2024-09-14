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
#if NET5_0_OR_GREATER
			Contents = Enum.GetValues<ContentType>().Where(c => this.device?.GetContentLocations(c)?.Any() ?? false).ToList();
#else
			Contents = Enum.GetValues(typeof(ContentType)).Cast<ContentType>().Where(c => this.device?.GetContentLocations(c)?.Any() ?? false).ToList();
#endif
		}
		catch
		{
			Contents = null;
		}

		SelectedContent = Contents?.FirstOrDefault() ?? ContentType.Unknown;
	}

	public bool IsContentLocationSupported
	{
		get => isContentLocationSupported;
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
		get => contents;
		set
		{
			contents = value;
			NotifyPropertyChanged(nameof(Contents));
		}
	}

	public ContentType SelectedContent
	{
		get => selectedContent;
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
