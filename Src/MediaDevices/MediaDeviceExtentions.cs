﻿using System;
using System.IO;

namespace MediaDevices;

/// <summary>
/// MediaDevice extention class
/// </summary>
public static class MediaDeviceExtentions
{

	/// <summary>
	/// Download a file from a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void DownloadFile(this MediaDevice device, string source, string destination)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using FileStream stream = File.Create(destination);
		device.DownloadFile(source, stream);
	}

	/// <summary>
	/// Download a icon from a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void DownloadIcon(this MediaDevice device, string source, string destination)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using FileStream stream = File.Create(destination);
		device.DownloadIcon(source, stream);
	}

	/// <summary>
	/// Download a thumbnail from a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void DownloadThumbnail(this MediaDevice device, string source, string destination)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using FileStream stream = File.Create(destination);
		device.DownloadThumbnail(source, stream);
	}

	/// <summary>
	/// Upload a file to a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void UploadFile(this MediaDevice device, string source, string destination)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using FileStream stream = File.OpenRead(source);
		device.UploadFile(stream, destination);
	}

	/// <summary>
	/// Download a file tree from a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <param name="recursive">Include subdirectories or not</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void DownloadFolder(this MediaDevice device, string source, string destination, bool recursive = true)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (!Directory.Exists(destination))
		{
			_ = Directory.CreateDirectory(destination);
		}

		MediaDirectoryInfo dir = device.GetDirectoryInfo(source);
		if (recursive)
		{
			foreach (MediaFileSystemInfo fsi in dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
			{
				string path = Path.Combine(destination, GetLocalPath(source, fsi.FullName!));
				if (fsi.Attributes.HasFlag(MediaFileAttributes.Directory) || fsi.Attributes.HasFlag(MediaFileAttributes.FileObject))
				{
					if (!Directory.Exists(path))
					{
						_ = Directory.CreateDirectory(path);
					}
				}
				else
				{
					MediaFileInfo mfi = (MediaFileInfo)fsi;
					mfi.CopyTo(path);
				}
			}
		}
		else
		{
			foreach (MediaFileInfo mfi in dir.EnumerateFiles())
			{
				string path = Path.Combine(destination, GetLocalPath(source, mfi.FullName!));
				mfi.CopyTo(path);
			}
		}
	}

	/// <summary>
	/// Upload a file tree to a portable device.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="source">The path to the source.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <param name="recursive">Include subdirectories or not</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void UploadFolder(this MediaDevice device, string source, string destination, bool recursive = true)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (!MediaDevice.IsPath(source))
		{
			throw new ArgumentException("The source path is not valid", nameof(source));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		device.CreateDirectory(destination);

		if (recursive)
		{
			DirectoryInfo di = new(source);
			foreach (FileSystemInfo e in di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
			{
				string path = Path.Combine(destination, GetLocalPath(source, e.FullName));
				if (e.Attributes.HasFlag(FileAttributes.Directory))
				{
					device.CreateDirectory(path);
				}
				else
				{
					FileInfo fi = (FileInfo)e;
					using FileStream stream = fi.OpenRead();
					device.UploadFile(stream, path);
				}
			}
		}
		else
		{
			DirectoryInfo di = new(source);
			foreach (FileInfo fi in di.EnumerateFiles())
			{
				string path = Path.Combine(destination, GetLocalPath(source, fi.FullName));
				using FileStream stream = fi.OpenRead();
				device.UploadFile(stream, path);
			}
		}
	}

	/// <summary>
	/// Download a file from a portable device using a Persistent Unique Id.
	/// </summary>
	/// <param name="device">Device class.</param>
	/// <param name="persistentUniqueId">Persistent Unique Id of the source file.</param>
	/// <param name="destination">The path to the destination.</param>
	/// <exception cref="System.IO.IOException">path is a file name.</exception>
	/// <exception cref="System.ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="System.ArgumentNullException">path is null.</exception>
	/// <exception cref="System.IO.DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="MediaDevices.NotConnectedException">device is not connected.</exception>
	public static void DownloadFileFromPersistentUniqueId(this MediaDevice device, string persistentUniqueId, string destination)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(device, nameof(device));
#else
		if (device == null)
		{
			throw new ArgumentNullException(nameof(device));
		}
#endif

		if (string.IsNullOrEmpty(persistentUniqueId))
		{
			throw new ArgumentNullException(nameof(persistentUniqueId));
		}

		if (!MediaDevice.IsPath(destination))
		{
			throw new ArgumentException("The destination path is not valid", nameof(destination));
		}

		if (!device.IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using FileStream stream = File.Create(destination);
		device.DownloadFileFromPersistentUniqueId(persistentUniqueId, stream);
	}

	private static string GetLocalPath(string basePath, string fullPath)
	{
		return !fullPath.StartsWith(basePath, StringComparison.InvariantCulture)
			? throw new ArgumentException($"{basePath} is not the base path of {fullPath}!")
			: fullPath.Remove(0, basePath.Length).TrimStart('\\', '/');
	}
}
