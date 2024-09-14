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

public class ItemViewModel(MediaFileSystemInfo item) : BaseViewModel, IExplorerItem
{
	private static readonly ImageSource imgFolder;
	private static readonly ImageSource imgFile;

	static ItemViewModel()
	{
		imgFolder = new BitmapImage(new Uri("pack://application:,,,/Images/Folder.png"));
		imgFile = new BitmapImage(new Uri("pack://application:,,,/Images/File.png"));
	}

	public string Name
	{
		get => item.Name;
		set
		{ }
	}

	public string FullName => item.FullName;

	public string Link => null;

	public long Size => (long)item.Length;

	public DateTime? CreationDate => item.LastWriteTime;

	public ExplorerItemType Type => IsDirectory ? ExplorerItemType.Directory : ExplorerItemType.File;

	public ImageSource Icon => IsDirectory ? imgFolder : imgFile;

	public bool IsDirectory => item.Attributes.HasFlag(MediaFileAttributes.Directory) || item.Attributes.HasFlag(MediaFileAttributes.FileObject);

	public bool HasChildren => Children?.Any() ?? false;

	public IEnumerable<IExplorerItem> Children
	{
		get
		{
			if (item.Attributes is MediaFileAttributes.Directory or MediaFileAttributes.FileObject)
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

	public void CreateFolder(string path) => throw new NotImplementedException();

	public bool Equals(IExplorerItem other) => FullName == other.FullName;

	public void Pull(string path, Stream stream) => throw new NotImplementedException();

	public void Push(Stream stream, string path) => throw new NotImplementedException();
}
