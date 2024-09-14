using ExplorerCtrl;
using MediaDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaDevicesApp.Mvvm;

/* Unmerged change from project 'MediaDevicesApp (net7.0-windows)'
Added:
using MediaDevicesApp;
using MediaDevicesApp.ViewModel;
using MediaDevicesApp.ViewModel;
*/

namespace MediaDevicesApp.ViewModel;

public class ItemViewModel : BaseViewModel, IExplorerItem
{
	private static ImageSource imgFolder;
	private static ImageSource imgFile;

	private MediaFileSystemInfo item;

	static ItemViewModel()
	{
		imgFolder = new BitmapImage(new Uri("pack://application:,,,/Images/Folder.png"));
		imgFile = new BitmapImage(new Uri("pack://application:,,,/Images/File.png"));
	}

	public ItemViewModel(MediaFileSystemInfo item)
	{
		this.item = item;
		//this.Refresh += (o, a) => this.item.Refresh();
	}

	public string Name
	{
		get
		{
			return item.Name;
		}
		set
		{ }
	}

	public string FullName { get { return item.FullName; } }

	public string Link { get { return null; } }

	public long Size { get { return (long)item.Length; } }

	public DateTime? CreationDate { get { return item.LastWriteTime; } }

	public ExplorerItemType Type { get { return IsDirectory ? ExplorerItemType.Directory : ExplorerItemType.File; } }

	public ImageSource Icon { get { return IsDirectory ? imgFolder : imgFile; } }

	public bool IsDirectory { get { return item.Attributes.HasFlag(MediaFileAttributes.Directory) || item.Attributes.HasFlag(MediaFileAttributes.Object); } }

	public bool HasChildren { get { return Children?.Any() ?? false; } }

	public IEnumerable<IExplorerItem> Children
	{
		get
		{
			if (item.Attributes is MediaFileAttributes.Directory or MediaFileAttributes.Object)
			{
				MediaDirectoryInfo dir = item as MediaDirectoryInfo;
				List<ItemViewModel> children = dir.EnumerateFileSystemInfos().Select(i => new ItemViewModel(i)).ToList();
				return children;
			}
			else
			{
				return null;
			}
		}
	}

#pragma warning disable CS0067
	public event EventHandler<RefreshEventArgs> Refresh;
#pragma warning restore CS0067

	public void CreateFolder(string path)
	{
		throw new NotImplementedException();
	}

	public bool Equals(IExplorerItem other)
	{
		return FullName == other.FullName;
	}

	public void Pull(string path, Stream stream)
	{
		throw new NotImplementedException();
	}

	public void Push(Stream stream, string path)
	{
		throw new NotImplementedException();
	}
}
