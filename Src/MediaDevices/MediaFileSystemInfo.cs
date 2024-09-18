using MediaDevices.Internal;

using System;
using System.Diagnostics;

namespace MediaDevices;

/// <summary>
/// Provides the base class for both MediaFileInfo and MediaDirectoryInfo objects.
/// </summary>
[DebuggerDisplay("{FullName}")]
public abstract class MediaFileSystemInfo
{
	/// <summary>
	///corresponding MediaDevice instance
	/// </summary>
	protected MediaDevice Device { get; private set; }

	internal Item item;

	private MediaDirectoryInfo? parent;

	internal MediaFileSystemInfo(MediaDevice device, Item item)
	{
		this.Device = device;
		this.item = item;
		Refresh();
	}

	/// <summary>
	/// Refreshes the state of the object.
	/// </summary>
	public virtual void Refresh() => item.Refresh();

	/// <summary>
	/// Gets the parent directory of a specified subdirectory.
	/// </summary>
	protected MediaDirectoryInfo? ParentDirectoryInfo
	{
		get
		{
			if (parent == null && item.Parent != null)
			{
				parent = new MediaDirectoryInfo(Device, item.Parent);
			}

			return parent;
		}
	}

	/// <summary>
	/// Gets the full path of the directory or file.
	/// </summary>
	public string? FullName => item.FullName;

	/// <summary>
	/// For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. Otherwise, the Name property gets the name of the directory.
	/// </summary>
	public string? Name => item.Name;

	/// <summary>
	/// Gets the size, in bytes, of the current file.   
	/// </summary>
	public ulong Length => item.Size;

	/// <summary>
	/// Gets the creation time of the current file or directory.
	/// </summary>
	public DateTime? CreationTime
	{
		get => item.DateCreated;
		set => item.SetDateCreated(value.GetValueOrDefault(DateTime.Now));
	}

	/// <summary>
	/// Gets the time when the current file or directory was last written to.
	/// </summary>
	public DateTime? LastWriteTime
	{
		get => item.DateModified;
		set => item.SetDateModified(value.GetValueOrDefault(DateTime.Now));
	}

	/// <summary>
	/// Gets the time when the current file was authored.
	/// </summary>
	public DateTime? DateAuthored
	{
		get => item.DateAuthored;
		set => item.SetDateAuthored(value.GetValueOrDefault(DateTime.Now));
	}

	/// <summary>
	/// Gets the attributes for the current file, directory or object.
	/// </summary>
	public MediaFileAttributes Attributes
	{
		get
		{
			MediaFileAttributes attributes = MediaFileAttributes.Normal;
			switch (item.Type)
			{
				case ItemType.File:
					attributes = MediaFileAttributes.Normal;
					break;
				case ItemType.Folder:
					attributes = MediaFileAttributes.Directory;
					break;
				case ItemType.Object:
					attributes = MediaFileAttributes.FileObject;
					break;
			}

			attributes |= item.CanDelete ? MediaFileAttributes.CanDelete : 0;
			attributes |= item.IsSystem ? MediaFileAttributes.System : 0;
			attributes |= item.IsHidden ? MediaFileAttributes.Hidden : 0;
			attributes |= item.IsDRMProtected ? MediaFileAttributes.DRMProtected : 0;
			return attributes;
		}
	}

	/// <summary>
	/// Gets the id of the MTP object.
	/// </summary>
	public string? Id => item.Id;

	/// <summary>
	/// Gets the persistent unique id of the MTP object.
	/// </summary>
	/// <remarks>
	/// A unique cross session object ID, that is not changing when device is disconnected.
	/// </remarks>
	public string? PersistentUniqueId => item.PersistentUniqueId;

	/// <summary>
	/// Rename the folder of file
	/// </summary>
	/// <param name="newName">New name of the file or folder.</param>
	public void Rename(string newName) => item.Rename(newName);

	/// <summary>
	/// Gets the hash code for the current object.
	/// </summary>
	/// <returns>A hash code for the current object.</returns>
#if NET5_0_OR_GREATER
	public override int GetHashCode() => Id?.GetHashCode(StringComparison.Ordinal) ?? 0;
#else
#pragma warning disable CA1307 // Specify StringComparison for clarity - not available before .NET 5.0
	public override int GetHashCode() => Id?.GetHashCode() ?? 0;
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif


#if NET5_0_OR_GREATER
	/// <summary>
	/// Gets the hash code for the current object.
	/// </summary>
	/// <param name="comparison">The comparison.</param>
	/// <returns>A hash code for the current object.</returns>
	public int GetHashCode(StringComparison comparison) => Id?.GetHashCode(comparison) ?? 0;
#endif

	/// <summary>
	/// Determines whether the specified object is equal to the current object.
	/// </summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
	public override bool Equals(object? obj)
	{
		if (obj is MediaFileSystemInfo other)
		{
			return Id == other.Id;
		}

		return false;
	}
}
