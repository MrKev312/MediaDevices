using MediaDevices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaDevicesUnitTest.TestCases;

public abstract class ReadonlyUnitTest : UnitTest
{
	// Exists Test
	protected string ExistingFile { get; set; }

	// parent is object and grandparent is root
	protected string InfoDirectoryName { get; set; }
	protected string InfoDirectoryPath { get; set; }
	protected DateTime? InfoDirectoryCreationTime { get; set; }
	protected DateTime? InfoDirectoryLastWriteTime { get; set; }
	protected DateTime? InfoDirectoryAuthoredTime { get; set; }

	// object and root is parent
	protected string InfoDirectoryParentName { get; set; }
	protected string InfoDirectoryParentPath { get; set; }
	protected DateTime? InfoDirectoryParentCreationTime { get; set; }
	protected DateTime? InfoDirectoryParentLastWriteTime { get; set; }
	protected DateTime? InfoDirectoryParentAuthoredTime { get; set; }

	protected string InfoFileName { get; set; }
	protected string InfoFilePath { get; set; }
	protected ulong InfoFileLength { get; set; }
	protected DateTime? InfoFileCreationTime { get; set; }
	protected DateTime? InfoFileLastWriteTime { get; set; }
	protected DateTime? InfoFileAuthoredTime { get; set; }

	protected string InfoFileParentName { get; set; }
	protected string InfoFileParentPath { get; set; }
	protected DateTime? InfoFileParentCreationTime { get; set; }
	protected DateTime? InfoFileParentLastWriteTime { get; set; }
	protected DateTime? InfoFileParentAuthoredTime { get; set; }

	protected string EnumDirectory { get; set; }
	protected string EnumFolderMask { get; set; }
	protected string EnumFilesmask { get; set; }
	protected string EnumItemMask { get; set; }

	protected List<string> EnumAllFolders { get; set; }
	protected List<string> EnumMaskFolders { get; set; }

	protected List<string> EnumAllFiles { get; set; }
	protected List<string> EnumMaskFiles { get; set; }
	protected List<string> EnumMaskRecursiveFiles { get; set; }

	protected List<string> EnumAllItems { get; set; }
	protected List<string> EnumMaskItems { get; set; }
	protected List<string> EnumMaskRecursiveItems { get; set; }

	[TestMethod]
	[Description("Check if files and folders exists.")]
	public void ExistsTest()
	{
		string existingDirectory = Path.GetDirectoryName(ExistingFile);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists1 = device.DirectoryExists(existingDirectory);
		bool exists2 = device.DirectoryExists(ExistingFile);
		bool exists3 = device.FileExists(existingDirectory);
		bool exists4 = device.FileExists(ExistingFile);

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsFalse(exists2, "exists2");
		Assert.IsFalse(exists3, "exists3");
		Assert.IsTrue(exists4, "exists4");
	}

	[TestMethod]
	[Description("Download a file to the target.")]
	public void DownloadTest()
	{
		long position;
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists = device.FileExists(ExistingFile);
		Assert.IsTrue(exists, "exists");

		string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(ExistingFile));
		using (MemoryStream stream = new())
		{
			device.DownloadFile(ExistingFile, stream);
			position = stream.Length;

			using FileStream file = File.Create(tempFile);
			stream.Position = 0;
			stream.CopyTo(file);
		}

		device.Disconnect();

		Assert.IsTrue(position > 0, "Position");
		Assert.IsTrue(File.Exists(tempFile), "Exists");

	}

	[TestMethod]
	[Description("Check file infos.")]
	public void FileInfoTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaFileInfo file = device.GetFileInfo(InfoFilePath);
		MediaDirectoryInfo parent = file.Directory;

		Assert.AreEqual(InfoFileName, file.Name, "file Name");
		Assert.AreEqual(InfoFilePath, file.FullName, "file FullName");
		Assert.AreEqual(InfoFileLength, file.Length, "file Length");
		Assert.AreEqual(InfoFileCreationTime, file.CreationTime, "file CreationTime");
		Assert.AreEqual(InfoFileLastWriteTime, file.LastWriteTime, "file LastWriteTime");
		Assert.AreEqual(InfoFileAuthoredTime, file.DateAuthored, "file DateAuthored");
		Assert.IsTrue(file.Attributes.HasFlag(MediaFileAttributes.Normal), "file Normal");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.Hidden), "file Hidden");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.System), "file System");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "file DRMProtected");

		Assert.AreEqual(InfoFileParentName, parent.Name, "parent Name");
		Assert.AreEqual(InfoFileParentPath, parent.FullName, "parent FullName");
		Assert.AreEqual(0ul, parent.Length, "parent Length");
		Assert.AreEqual(InfoFileParentCreationTime, parent.CreationTime, "parent CreationTime");
		Assert.AreEqual(InfoFileParentLastWriteTime, parent.LastWriteTime, "parent LastWriteTime");
		Assert.AreEqual(InfoFileParentAuthoredTime, parent.DateAuthored, "parent DateAuthored");
		Assert.IsTrue(parent.Attributes.HasFlag(MediaFileAttributes.Directory), "parent Directory");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.Hidden), "parent Hidden");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.System), "parent System");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "parent DRMProtected");

		using (MemoryStream mem = new())
		{
			using (Stream stream = file.OpenRead())
			{
				stream.CopyTo(mem);
			}

			Assert.AreEqual(InfoFileLength, (ulong)mem.Position, "file read size");
		}

		device.Disconnect();

	}

	[TestMethod]
	[Description("Check directory infos.")]
	public void DirectoryInfoTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo dir = device.GetDirectoryInfo(InfoDirectoryPath);
		MediaDirectoryInfo parent = dir.Parent;
		MediaDirectoryInfo root = parent.Parent;
		MediaDirectoryInfo empty = root.Parent;

		Assert.AreEqual(InfoDirectoryName, dir.Name, "dir Name");
		Assert.AreEqual(InfoDirectoryPath, dir.FullName, "dir FullName");
		Assert.AreEqual(0ul, dir.Length, "dir Length");
		Assert.AreEqual(InfoDirectoryCreationTime, dir.CreationTime, "dir CreationTime");
		Assert.AreEqual(InfoDirectoryLastWriteTime, dir.LastWriteTime, "dir LastWriteTime");
		Assert.AreEqual(InfoDirectoryAuthoredTime, dir.DateAuthored, "dir DateAuthored");
		Assert.IsTrue(dir.Attributes.HasFlag(MediaFileAttributes.Directory), "dir Directory");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.Hidden), "dir Hidden");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.System), "dir System");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "dir DRMProtected");

		Assert.AreEqual(InfoDirectoryParentName, parent.Name, "parent Name");
		Assert.AreEqual(InfoDirectoryParentPath, parent.FullName, "parent FullName");
		Assert.AreEqual(0ul, parent.Length, "parent Length");
		Assert.AreEqual(InfoDirectoryParentCreationTime, parent.CreationTime, "parent CreationTime");
		Assert.AreEqual(InfoDirectoryParentLastWriteTime, parent.LastWriteTime, "parent LastWriteTime");
		Assert.AreEqual(InfoDirectoryParentAuthoredTime, parent.DateAuthored, "parent DateAuthored");
		Assert.IsTrue(parent.Attributes.HasFlag(MediaFileAttributes.FileObject), "parent Object");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.Hidden), "parent Hidden");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.System), "parent System");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "parent DRMProtected");

		Assert.AreEqual(@"\", root.Name, "root Name");
		Assert.AreEqual(@"\", root.FullName, "root FullName");
		Assert.AreEqual(0ul, root.Length, "root Length");
		Assert.AreEqual(null, root.CreationTime, "root CreationTime");
		Assert.AreEqual(null, root.LastWriteTime, "root LastWriteTime");
		Assert.AreEqual(null, root.DateAuthored, "root DateAuthored");
		Assert.IsTrue(root.Attributes.HasFlag(MediaFileAttributes.FileObject), "root Object");
		Assert.IsFalse(root.Attributes.HasFlag(MediaFileAttributes.Hidden), "root Hidden");
		Assert.IsFalse(root.Attributes.HasFlag(MediaFileAttributes.System), "root System");
		Assert.IsFalse(root.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "root DRMProtected");

		Assert.IsNull(empty, "empty");

		device.Disconnect();
	}

	[TestMethod]
	[Description("Check directory infos enum.")]
	public void DirectoryInfoEnumTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo dir = device.GetDirectoryInfo(EnumDirectory);

		List<string> enum1 = dir.EnumerateDirectories().Select(e => e.FullName).ToList();
		List<string> enum2 = dir.EnumerateDirectories(EnumFolderMask).Select(e => e.FullName).ToList();

		List<string> enum3 = dir.EnumerateFiles().Select(e => e.FullName).ToList();
		List<string> enum4 = dir.EnumerateFiles(EnumFilesmask).Select(e => e.FullName).ToList();
		List<string> enum5 = dir.EnumerateFiles(EnumFilesmask, SearchOption.AllDirectories).Select(e => e.FullName).ToList();

		List<string> enum6 = dir.EnumerateFileSystemInfos().Select(e => e.FullName).ToList();
		List<string> enum7 = dir.EnumerateFileSystemInfos(EnumItemMask).Select(e => e.FullName).ToList();
		List<string> enum8 = dir.EnumerateFileSystemInfos(EnumItemMask, SearchOption.AllDirectories).Select(e => e.FullName).ToList();

		device.Disconnect();

		CollectionAssert.AreEquivalent(EnumAllFolders, enum1, "enum1");
		CollectionAssert.AreEquivalent(EnumMaskFolders, enum2, "enum2");

		CollectionAssert.AreEquivalent(EnumAllFiles, enum3, "enum3");
		CollectionAssert.AreEquivalent(EnumMaskFiles, enum4, "enum4");
		CollectionAssert.AreEquivalent(EnumMaskRecursiveFiles, enum5, "enum5");

		CollectionAssert.AreEquivalent(EnumAllItems, enum6, "enum6");
		CollectionAssert.AreEquivalent(EnumMaskItems, enum7, "enum7");
		CollectionAssert.AreEquivalent(EnumMaskRecursiveItems, enum8, "enum8");
	}

	[TestMethod]
	[Description("Download a file to the target.")]
	public void DownloadFileTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists = device.FileExists(ExistingFile);
		Assert.IsTrue(exists, "exists");

		string tempFile = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(ExistingFile));

		device.DownloadFile(ExistingFile, tempFile);

		device.Disconnect();

		Assert.IsTrue(File.Exists(tempFile), "Exists");

	}
}
