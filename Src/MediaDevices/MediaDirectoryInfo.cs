﻿using MediaDevices.Internal;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaDevices;

/// <summary>
/// Exposes instance methods for creating, moving, and enumerating through directories and subdirectories. This class cannot be inherited.
/// </summary>
public class MediaDirectoryInfo : MediaFileSystemInfo
{
	internal MediaDirectoryInfo(MediaDevice device, Item item) : base(device, item)
	{ }

	/// <summary>
	/// Refreshes the state of the object.
	/// </summary>
	public override void Refresh() => base.Refresh();

	/// <summary>
	/// Gets the parent directory of a specified subdirectory.
	/// </summary>
	public MediaDirectoryInfo Parent => ParentDirectoryInfo;

	/// <summary>
	/// Creates a subdirectory or subdirectories on the specified path. The specified path is relative to this instance of the DirectoryInfo class.
	/// </summary>
	/// <param name="path">The specified path. </param>
	/// <returns>The last directory specified in path.</returns>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public MediaDirectoryInfo CreateSubdirectory(string path)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(path);
#else
		if (path == null)
		{
			throw new ArgumentNullException(nameof(path));
		}
#endif

		if (!MediaDevice.IsPath(path))
		{
			throw new ArgumentException("Path is not valid", nameof(path));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = this.item.CreateSubdirectory(path);
		return new MediaDirectoryInfo(device, item);
	}

	/// <summary>
	/// Returns an enumerable collection of directory information in the current directory.
	/// </summary>
	/// <returns>An enumerable collection of directories in the current directory.</returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaDirectoryInfo> EnumerateDirectories()
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren().Where(i => i.Type != ItemType.File).Select(i => new MediaDirectoryInfo(device, i));
	}

	/// <summary>
	/// Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option. 
	/// </summary>
	/// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
	/// <returns>An enumerable collection of directories that matches searchPattern and searchOption.</returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren(MediaDevice.FilterToRegex(searchPattern), searchOption).Where(i => i.Type != ItemType.File).Select(i => new MediaDirectoryInfo(device, i));
	}

	/// <summary>
	/// Returns an enumerable collection of file information in the current directory.
	/// </summary>
	/// <returns>An enumerable collection of the files in the current directory.</returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaFileInfo> EnumerateFiles()
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren().Where(i => i.Type == ItemType.File).Select(i => new MediaFileInfo(device, i));
	}

	/// <summary>
	/// Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.
	/// </summary>
	/// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
	/// <returns>An enumerable collection of files that matches searchPattern and searchOption.</returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren(MediaDevice.FilterToRegex(searchPattern), searchOption).Where(i => i.Type == ItemType.File).Select(i => new MediaFileInfo(device, i));
	}

	/// <summary>
	/// Returns an enumerable collection of file system information in the current directory.
	/// </summary>
	/// <returns>An enumerable collection of file system information in the current directory. </returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaFileSystemInfo> EnumerateFileSystemInfos()
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren().Select(i => i.Type == ItemType.File ?
					(MediaFileSystemInfo)new MediaFileInfo(device, i) :
					(MediaFileSystemInfo)new MediaDirectoryInfo(device, i));
	}

	/// <summary>
	/// Returns an enumerable collection of file system information that matches a specified search pattern and search subdirectory option.
	/// </summary>
	/// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
	/// <returns>An enumerable collection of file system information objects that matches searchPattern and searchOption.</returns>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public IEnumerable<MediaFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return item.GetChildren(MediaDevice.FilterToRegex(searchPattern), searchOption).Select(i => i.Type == ItemType.File ?
					(MediaFileSystemInfo)new MediaFileInfo(device, i) :
					(MediaFileSystemInfo)new MediaDirectoryInfo(device, i));
	}
}
