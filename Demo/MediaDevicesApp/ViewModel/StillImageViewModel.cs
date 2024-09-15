using MediaDevices;

using MediaDevicesApp.Mvvm;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaDevicesApp.ViewModel;

public class StillImageViewModel : BaseViewModel
{
	MediaDevice device;
	private bool isStillImageSupported;
	private List<string> stillImageFunctionalObjects;
	private string selectedStillImageFunctionalObject;
	private ImageSource stillImageSource;

	public DelegateCommand StillImageCommand { get; private set; }

	public StillImageViewModel()
	{
		stillImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Folder.png"));
		StillImageCommand = new DelegateCommand(OnStillImageCapture);
	}

	public void Update(MediaDevice device)
	{
		this.device = device;
		IsStillImageSupported = this.device?.FunctionalCategories()?.Any(c => c == FunctionalCategory.StillImageCapture) ?? false;
		StillImageFunctionalObjects = this.device?.FunctionalObjects(FunctionalCategory.StillImageCapture)?.ToList();
	}

	public bool IsStillImageSupported
	{
		get => isStillImageSupported;
		set
		{
			isStillImageSupported = value;
			NotifyPropertyChanged(nameof(IsStillImageSupported));
		}
	}

	public List<string> StillImageFunctionalObjects
	{
		get => stillImageFunctionalObjects;
		set
		{
			if (stillImageFunctionalObjects != value)
			{
				stillImageFunctionalObjects = value;
				NotifyPropertyChanged(nameof(StillImageFunctionalObjects));
			}
		}
	}

	public string SelectedStillImageFunctionalObject
	{
		get => selectedStillImageFunctionalObject;
		set
		{
			selectedStillImageFunctionalObject = value;
			NotifyPropertyChanged(nameof(SelectedStillImageFunctionalObject));
		}
	}

	public ImageSource StillImageSource
	{
		get => stillImageSource;
		set
		{
			stillImageSource = value;
			NotifyPropertyChanged(nameof(StillImageSource));
		}
	}

	public void OnStillImageCapture()
	{
		device.ObjectAdded += OnStillImage;

		_ = device.StillImageCaptureInitiate(selectedStillImageFunctionalObject);
	}

	private void OnStillImage(object sender, ObjectAddedEventArgs e)
	{
		device.ObjectAdded -= OnStillImage;

		string fullName = e.ObjectFullFileName;
		using MemoryStream mem = new();
		e.ObjectFileStream.CopyTo(mem);
		mem.Position = 0;

		Application.Current.Dispatcher.Invoke(() =>
		{
			BitmapImage image = new();
			image.BeginInit();
			image.StreamSource = mem;
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.EndInit();

			StillImageSource = image;
		});
	}
}
