﻿using MediaDevices;

using MediaDevicesApp.Mvvm;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaDevicesApp.ViewModel;

public class FilesViewModel : BaseViewModel
{
	MediaDevice device;
	private Function selectedFunction;
	private string path = @"\";
	private string filter = "*";
	private bool useRecursive = true;
	private long numOfFiles;
	private List<Info> files;
	private string time;

	public DelegateCommand EnumerateCommand { get; private set; }

	public enum Function
	{
		MediaDevice_EnumerateDirectories,
		MediaDevice_EnumerateFiles,
		MediaDevice_EnumerateFileSystemEntries,

		MediaFileSystemInfo_EnumerateDirectories,
		MediaFileSystemInfo_EnumerateFiles,
		MediaFileSystemInfo_EnumerateFileSystemInfos,

	}

	public class Info
	{
		private static BitmapImage fileImage;
		private static BitmapImage folderImages;

		static Info()
		{
			fileImage = new BitmapImage(new Uri("pack://application:,,,/Images/File.png"));
			folderImages = new BitmapImage(new Uri("pack://application:,,,/Images/Folder.png"));
		}

		public Info(string fullName)
		{
			Name = System.IO.Path.GetFileName(fullName);
			FullName = fullName;
			Image = fullName.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? folderImages : fileImage;
		}

		public Info(MediaFileSystemInfo info)
		{
			Id = info.Id;
			Name = info.Name;
			FullName = info.FullName;
			Length = info.Length;
			PersistentUniqueId = info.PersistentUniqueId;
			Image = info.Attributes.HasFlag(MediaFileAttributes.Normal) ? fileImage : folderImages;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string FullName { get; set; }
		public ulong Length { get; set; }
		public string PersistentUniqueId { get; set; }
		public ImageSource Image { get; private set; }
	}

	public FilesViewModel()
	{
		EnumerateCommand = new DelegateCommand(OnEnumerate);
	}

	public void Update(MediaDevice device)
	{
		this.device = device;
		Files = null;
	}



	public List<Function> Functions
	{
		get { return Enum.GetValues(typeof(Function)).Cast<Function>().ToList(); }
	}

	public Function SelectedFunction
	{
		get
		{
			return selectedFunction;
		}
		set
		{
			selectedFunction = value;
			NotifyPropertyChanged(nameof(SelectedFunction));
		}
	}

	public string Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
			NotifyPropertyChanged(nameof(Path));
		}
	}

	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			filter = value;
			NotifyPropertyChanged(nameof(Filter));
		}
	}

	public bool UseRecursive
	{
		get
		{
			return useRecursive;
		}
		set
		{
			useRecursive = value;
			NotifyPropertyChanged(nameof(UseRecursive));
		}
	}

	public long NumOfFiles
	{
		get
		{
			return numOfFiles;
		}
		set
		{
			numOfFiles = value;
			NotifyPropertyChanged(nameof(NumOfFiles));
		}
	}

	public string Time
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
			NotifyPropertyChanged(nameof(Time));
		}
	}

	public List<Info> Files
	{
		get
		{
			return files;
		}
		set
		{
			files = value;
			NumOfFiles = files?.Count ?? 0;
			NotifyPropertyChanged(nameof(Files));
		}
	}

	private void OnEnumerate()
	{
		Stopwatch stopwatch = new();

		using (new WaitCursor())
		{
			SearchOption searchOption = UseRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			switch (SelectedFunction)
			{
				case Function.MediaDevice_EnumerateDirectories:
					stopwatch.Start();
					List<string> list1 = device.EnumerateDirectories(Path, Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list1.Select(f => new Info(f)).ToList();
					break;

				case Function.MediaDevice_EnumerateFiles:
					stopwatch.Start();
					List<string> list2 = device.EnumerateFiles(Path, Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list2.Select(f => new Info(f)).ToList();
					break;

				case Function.MediaDevice_EnumerateFileSystemEntries:
					stopwatch.Start();
					List<string> list3 = device.EnumerateFileSystemEntries(Path, Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list3.Select(f => new Info(f)).ToList();
					break;

				case Function.MediaFileSystemInfo_EnumerateDirectories:
					stopwatch.Start();
					List<MediaDirectoryInfo> list4 = device.GetDirectoryInfo(Path).EnumerateDirectories(Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list4.Select(f => new Info(f)).ToList();
					break;

				case Function.MediaFileSystemInfo_EnumerateFiles:
					stopwatch.Start();
					List<MediaFileInfo> list5 = device.GetDirectoryInfo(Path).EnumerateFiles(Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list5.Select(f => new Info(f)).ToList();
					break;

				case Function.MediaFileSystemInfo_EnumerateFileSystemInfos:
					stopwatch.Start();
					List<MediaFileSystemInfo> list6 = device.GetDirectoryInfo(Path).EnumerateFileSystemInfos(Filter, searchOption).ToList();
					stopwatch.Stop();
					Files = list6.Select(f => new Info(f)).ToList();
					break;
			}

			Time = stopwatch.Elapsed.ToString(@"mm\:ss\.fffffff");
		}
	}
}
