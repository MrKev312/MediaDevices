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
	protected string existingFile;

	// parent is object and grandparent is root
	protected string infoDirectoryName;
	protected string infoDirectoryPath;
	protected DateTime? infoDirectoryCreationTime;
	protected DateTime? infoDirectoryLastWriteTime;
	protected DateTime? infoDirectoryAuthoredTime;

	// object and root is parent
	protected string infoDirectoryParentName;
	protected string infoDirectoryParentPath;
	protected DateTime? infoDirectoryParentCreationTime;
	protected DateTime? infoDirectoryParentLastWriteTime;
	protected DateTime? infoDirectoryParentAuthoredTime;

	protected string infoFileName;
	protected string infoFilePath;
	protected ulong infoFileLength;
	protected DateTime? infoFileCreationTime;
	protected DateTime? infoFileLastWriteTime;
	protected DateTime? infoFileAuthoredTime;

	protected string infoFileParentName;
	protected string infoFileParentPath;
	protected DateTime? infoFileParentCreationTime;
	protected DateTime? infoFileParentLastWriteTime;
	protected DateTime? infoFileParentAuthoredTime;

	protected string enumDirectory;
	protected string enumFolderMask;
	protected string enumFilesmask;
	protected string enumItemMask;

	protected List<string> enumAllFolders;
	protected List<string> enumMaskFolders;

	protected List<string> enumAllFiles;
	protected List<string> enumMaskFiles;
	protected List<string> enumMaskRecursiveFiles;

	protected List<string> enumAllItems;
	protected List<string> enumMaskItems;
	protected List<string> enumMaskRecursiveItems;

	[TestMethod]
	[Description("Check if files and folders exists.")]
	public void ExistsTest()
	{
		string existingDirectory = Path.GetDirectoryName(existingFile);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists1 = device.DirectoryExists(existingDirectory);
		bool exists2 = device.DirectoryExists(existingFile);
		bool exists3 = device.FileExists(existingDirectory);
		bool exists4 = device.FileExists(existingFile);

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
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists = device.FileExists(existingFile);
		Assert.IsTrue(exists, "exists");

		string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(existingFile));
		using (MemoryStream stream = new())
		{
			device.DownloadFile(existingFile, stream);
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
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaFileInfo file = device.GetFileInfo(infoFilePath);
		MediaDirectoryInfo parent = file.Directory;

		Assert.AreEqual(infoFileName, file.Name, "file Name");
		Assert.AreEqual(infoFilePath, file.FullName, "file FullName");
		Assert.AreEqual(infoFileLength, file.Length, "file Length");
		Assert.AreEqual(infoFileCreationTime, file.CreationTime, "file CreationTime");
		Assert.AreEqual(infoFileLastWriteTime, file.LastWriteTime, "file LastWriteTime");
		Assert.AreEqual(infoFileAuthoredTime, file.DateAuthored, "file DateAuthored");
		Assert.IsTrue(file.Attributes.HasFlag(MediaFileAttributes.Normal), "file Normal");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.Hidden), "file Hidden");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.System), "file System");
		Assert.IsFalse(file.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "file DRMProtected");

		Assert.AreEqual(infoFileParentName, parent.Name, "parent Name");
		Assert.AreEqual(infoFileParentPath, parent.FullName, "parent FullName");
		Assert.AreEqual(0ul, parent.Length, "parent Length");
		Assert.AreEqual(infoFileParentCreationTime, parent.CreationTime, "parent CreationTime");
		Assert.AreEqual(infoFileParentLastWriteTime, parent.LastWriteTime, "parent LastWriteTime");
		Assert.AreEqual(infoFileParentAuthoredTime, parent.DateAuthored, "parent DateAuthored");
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

			Assert.AreEqual(infoFileLength, (ulong)mem.Position, "file read size");
		}

		device.Disconnect();

	}

	[TestMethod]
	[Description("Check directory infos.")]
	public void DirectoryInfoTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo dir = device.GetDirectoryInfo(infoDirectoryPath);
		MediaDirectoryInfo parent = dir.Parent;
		MediaDirectoryInfo root = parent.Parent;
		MediaDirectoryInfo empty = root.Parent;

		Assert.AreEqual(infoDirectoryName, dir.Name, "dir Name");
		Assert.AreEqual(infoDirectoryPath, dir.FullName, "dir FullName");
		Assert.AreEqual(0ul, dir.Length, "dir Length");
		Assert.AreEqual(infoDirectoryCreationTime, dir.CreationTime, "dir CreationTime");
		Assert.AreEqual(infoDirectoryLastWriteTime, dir.LastWriteTime, "dir LastWriteTime");
		Assert.AreEqual(infoDirectoryAuthoredTime, dir.DateAuthored, "dir DateAuthored");
		Assert.IsTrue(dir.Attributes.HasFlag(MediaFileAttributes.Directory), "dir Directory");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.Hidden), "dir Hidden");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.System), "dir System");
		Assert.IsFalse(dir.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "dir DRMProtected");

		Assert.AreEqual(infoDirectoryParentName, parent.Name, "parent Name");
		Assert.AreEqual(infoDirectoryParentPath, parent.FullName, "parent FullName");
		Assert.AreEqual(0ul, parent.Length, "parent Length");
		Assert.AreEqual(infoDirectoryParentCreationTime, parent.CreationTime, "parent CreationTime");
		Assert.AreEqual(infoDirectoryParentLastWriteTime, parent.LastWriteTime, "parent LastWriteTime");
		Assert.AreEqual(infoDirectoryParentAuthoredTime, parent.DateAuthored, "parent DateAuthored");
		Assert.IsTrue(parent.Attributes.HasFlag(MediaFileAttributes.Object), "parent Object");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.Hidden), "parent Hidden");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.System), "parent System");
		Assert.IsFalse(parent.Attributes.HasFlag(MediaFileAttributes.DRMProtected), "parent DRMProtected");

		Assert.AreEqual(@"\", root.Name, "root Name");
		Assert.AreEqual(@"\", root.FullName, "root FullName");
		Assert.AreEqual(0ul, root.Length, "root Length");
		Assert.AreEqual(null, root.CreationTime, "root CreationTime");
		Assert.AreEqual(null, root.LastWriteTime, "root LastWriteTime");
		Assert.AreEqual(null, root.DateAuthored, "root DateAuthored");
		Assert.IsTrue(root.Attributes.HasFlag(MediaFileAttributes.Object), "root Object");
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
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo dir = device.GetDirectoryInfo(enumDirectory);

		List<string> enum1 = dir.EnumerateDirectories().Select(e => e.FullName).ToList();
		List<string> enum2 = dir.EnumerateDirectories(enumFolderMask).Select(e => e.FullName).ToList();

		List<string> enum3 = dir.EnumerateFiles().Select(e => e.FullName).ToList();
		List<string> enum4 = dir.EnumerateFiles(enumFilesmask).Select(e => e.FullName).ToList();
		List<string> enum5 = dir.EnumerateFiles(enumFilesmask, SearchOption.AllDirectories).Select(e => e.FullName).ToList();

		List<string> enum6 = dir.EnumerateFileSystemInfos().Select(e => e.FullName).ToList();
		List<string> enum7 = dir.EnumerateFileSystemInfos(enumItemMask).Select(e => e.FullName).ToList();
		List<string> enum8 = dir.EnumerateFileSystemInfos(enumItemMask, SearchOption.AllDirectories).Select(e => e.FullName).ToList();

		device.Disconnect();

		CollectionAssert.AreEquivalent(enumAllFolders, enum1, "enum1");
		CollectionAssert.AreEquivalent(enumMaskFolders, enum2, "enum2");

		CollectionAssert.AreEquivalent(enumAllFiles, enum3, "enum3");
		CollectionAssert.AreEquivalent(enumMaskFiles, enum4, "enum4");
		CollectionAssert.AreEquivalent(enumMaskRecursiveFiles, enum5, "enum5");

		CollectionAssert.AreEquivalent(enumAllItems, enum6, "enum6");
		CollectionAssert.AreEquivalent(enumMaskItems, enum7, "enum7");
		CollectionAssert.AreEquivalent(enumMaskRecursiveItems, enum8, "enum8");
	}

	[TestMethod]
	[Description("Download a file to the target.")]
	public void DownloadFileTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(deviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists = device.FileExists(existingFile);
		Assert.IsTrue(exists, "exists");

		string tempFile = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(existingFile));

		device.DownloadFile(existingFile, tempFile);

		device.Disconnect();

		Assert.IsTrue(File.Exists(tempFile), "Exists");

	}
}
