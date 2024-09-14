using MediaDevices.Internal;

using System.IO;

namespace MediaDevices;

/// <summary>
/// Provides properties for drives.
/// </summary>
public sealed class MediaDriveInfo
{
	private readonly MediaDevice device;
	private readonly string objectId;
	private readonly MediaStorageInfo info;

	internal MediaDriveInfo(MediaDevice device, string objectId)
	{
		this.device = device;
		this.objectId = objectId;
		info = device.GetStorageInfo(objectId);

		if (info != null)
		{
			TotalSize = (long)info.Capacity;
			TotalFreeSpace = AvailableFreeSpace = (long)info.FreeSpaceInBytes;

			DriveFormat = info.FileSystemType;

			DriveType = info.Type switch
			{
				StorageType.FixedRam or StorageType.FixedRom => DriveType.Fixed,
				StorageType.RemovableRam or StorageType.RemovableRom => DriveType.Removable,
				_ => DriveType.Unknown,
			};

			RootDirectory = new MediaDirectoryInfo(this.device, Item.Create(this.device, this.objectId));
			Name = RootDirectory.FullName;
			VolumeLabel = info.Description;
		}
	}

	/// <summary>
	/// Indicates the available space in bytes.
	/// </summary>
	public long AvailableFreeSpace { get; private set; }

	/// <summary>
	/// Format of the drive.
	/// </summary>
	public string DriveFormat { get; private set; }

	/// <summary>
	/// Type of the drive
	/// </summary>
	public DriveType DriveType { get; private set; }

	/// <summary>
	/// True is the drive is ready; false if not.
	/// </summary>
	public static bool IsReady => true;

	/// <summary>
	/// Name of the drive
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Get the root directory of the drive.
	/// </summary>
	public MediaDirectoryInfo RootDirectory { get; private set; }

	/// <summary>
	/// Gets the total free space of the device in bytes.
	/// </summary>
	public long TotalFreeSpace { get; private set; }

	/// <summary>
	/// Gets the total size of the device in bytes.
	/// </summary>
	public long TotalSize { get; private set; }

	/// <summary>
	/// Get the volume lable of the drive.
	/// </summary>
	public string VolumeLabel { get; private set; }

	/// <summary>
	/// Eject the drive.
	/// </summary>
	public void Eject() => device.InternalEject(objectId);

	/// <summary>
	/// Format the drive.
	/// </summary>
	public void Format() => device.Format(objectId);
}
