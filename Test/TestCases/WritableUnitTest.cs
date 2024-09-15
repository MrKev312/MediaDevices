using MediaDevices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MediaDevicesUnitTest.TestCases;

public abstract class WritableUnitTest : UnitTest
{
	protected static string TestDataFolder => Path.GetFullPath(@".\TestData");

	protected string WorkingFolder { get; set; }
	protected List<string> TreeList { get; set; } =
	[
		"\\UploadTree\\Aaa",
			"\\UploadTree\\Aaa\\A.txt",
			"\\UploadTree\\Aaa\\Abb",
			"\\UploadTree\\Aaa\\Abb\\Acc",
			"\\UploadTree\\Aaa\\Abb\\Acc\\Ctest.txt",
			"\\UploadTree\\Aaa\\Abb\\Add",
			"\\UploadTree\\Aaa\\Abb\\Aee.txt",
			"\\UploadTree\\Aaa\\Abb\\Aff.txt",
			"\\UploadTree\\Aaa\\Abb\\Agg.txt",
			"\\UploadTree\\Aaa\\Abb\\B.txt",
			"\\UploadTree\\Aaa\\Acc",
			"\\UploadTree\\Baa",
			"\\UploadTree\\Baa\\Bxx.txt",
			"\\UploadTree\\Bbb",
			"\\UploadTree\\Caa",
			"\\UploadTree\\Caa\\Cxx.txt",
			"\\UploadTree\\Ccc",
			"\\UploadTree\\Root.txt"
	];
	protected List<string> TreeListFull { get; set; }

	protected void UploadTestTree(MediaDevice device)
	{
		TreeListFull = TreeList.Select(p => WorkingFolder + p).ToList();

		string sourceFolder = Path.Combine(TestDataFolder, "UploadTree");

		// create empty folders not checked in
		_ = Directory.CreateDirectory(Path.Combine(sourceFolder, @"Aaa\Abb\Add"));
		_ = Directory.CreateDirectory(Path.Combine(sourceFolder, @"Aaa\Acc"));
		_ = Directory.CreateDirectory(Path.Combine(sourceFolder, "Bbb"));
		_ = Directory.CreateDirectory(Path.Combine(sourceFolder, "Ccc"));

		List<string> l = [.. Directory.EnumerateFileSystemEntries(sourceFolder, "*", SearchOption.AllDirectories).OrderBy(s => s)];
		List<string> x = [.. Directory.GetFileSystemEntries(sourceFolder, "*", SearchOption.AllDirectories).OrderBy(s => s)];

		string destFolder = Path.Combine(WorkingFolder, "UploadTree");

		bool exists = device.DirectoryExists(destFolder);
		if (exists)
			device.DeleteDirectory(destFolder, true);

		device.UploadFolder(sourceFolder, destFolder);
	}

	[TestMethod]
	[Description("Test event handling.")]
	public void EventTest()
	{
		//if (!this.supEvent) return;

		AutoResetEvent fired = new(false);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.ObjectRemoved += (s, a) => fired.Set();
		device.Connect();

		string filePath = Path.Combine(WorkingFolder, "Test.txt");
		if (device.FileExists(filePath))
			device.DeleteFile(filePath);

		using (MemoryStream stream = new(Encoding.UTF8.GetBytes("This is a test.")))
		{
			device.UploadFile(stream, filePath);
		}

		device.DeleteFile(filePath);

		bool isFired = fired.WaitOne(new TimeSpan(0, 2, 0));
		device.Disconnect();

		Assert.IsTrue(isFired);
	}

	[TestMethod]
	[Description("Creating a new folder.")]
	public void CreateFolderTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();
		MediaDirectoryInfo root = device.GetRootDirectory();
		List<MediaFileSystemInfo> list = root.EnumerateFileSystemInfos().ToList();

		string newFolder = Path.Combine(WorkingFolder, "Test");
		bool exists1 = device.DirectoryExists(WorkingFolder);
		device.CreateDirectory(newFolder);
		bool exists2 = device.DirectoryExists(newFolder);
		device.DeleteDirectory(newFolder, true);
		bool exists3 = device.DirectoryExists(newFolder);

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsTrue(exists2, "exists2");
		Assert.IsFalse(exists3, "exists3");
	}

	[TestMethod]
	[Description("Upload a file to the target.")]
	public void UploadTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string filePath = Path.Combine(WorkingFolder, "Test.txt");
		if (device.FileExists(filePath))
			device.DeleteFile(filePath);

		using (MemoryStream stream = new(Encoding.UTF8.GetBytes("This is a test.")))
		{
			device.UploadFile(stream, filePath);
		}

		bool exists1 = device.FileExists(filePath);
		device.DeleteFile(filePath);
		bool exists2 = device.FileExists(@"\Phone\Downloads\Test.txt");

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsFalse(exists2, "exists2");
	}

	[TestMethod]
	[Description("Upload a file to the target.")]
	public void UploadFileTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string sourceFile = Path.Combine(TestDataFolder, "TestFile.txt");
		string destFile = Path.Combine(WorkingFolder, "TestFile.txt");

		bool exists1 = device.FileExists(destFile);
		if (exists1)
			device.DeleteFile(destFile);

		device.UploadFile(sourceFile, destFile);

		bool exists = device.FileExists(destFile);

		device.DeleteFile(destFile);

		device.Disconnect();

		Assert.IsTrue(exists, "exists");

	}

	[TestMethod]
	[Description("Upload a tree to the target.")]
	public void UploadTreeTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		UploadTestTree(device);

		string destFolder = Path.Combine(WorkingFolder, "UploadTree");
		int pathLen = WorkingFolder.Length;

		List<string> list = device.EnumerateFileSystemEntries(destFolder, null, SearchOption.AllDirectories).ToList();

		device.Disconnect();

		CollectionAssert.AreEquivalent(TreeListFull, list, "EnumerateFileSystemEntries");
		//CollectionAssert.AreEquivalent(pathes, list, "EnumerateFileSystemEntries");
	}

	[TestMethod]
	[Description("Download a file from the target.")]
	public void DownloadFileTest()
	{
		string filePath = Path.Combine(TestDataFolder, Path.GetFileName(FilePersistentUniqueIdPath));
		File.Delete(filePath);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists1 = device.FileExists(FilePersistentUniqueIdPath);

		device.DownloadFile(FilePersistentUniqueIdPath, filePath);

		device.Disconnect();

		Assert.IsTrue(exists1);
		Assert.IsTrue(File.Exists(filePath), "Exists");
		Assert.IsTrue(new FileInfo(filePath).Length > 100, "Length");

	}

	[TestMethod]
	[Description("Download a file from the target.")]
	public void DownloadIconTest()
	{
		string filePath = Path.Combine(TestDataFolder, Path.GetFileName("MTPTestPic.jpg"));
		File.Delete(filePath);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists1 = device.FileExists(@"\Card\MTPTestPic.jpg");

		device.DownloadIcon(@"\Card\MTPTestPic.jpg", filePath);

		device.Disconnect();

		Assert.IsTrue(exists1);
		Assert.IsTrue(File.Exists(filePath), "Exists");
		Assert.IsTrue(new FileInfo(filePath).Length > 100, "Length");

	}

	[TestMethod]
	[Description("Download a file from the target.")]
	public void DownloadThumbnailTest()
	{
		string filePath = Path.Combine(TestDataFolder, Path.ChangeExtension(Path.GetFileName(FilePersistentUniqueIdPath), ".gif"));
		File.Delete(filePath);

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		bool exists1 = device.FileExists(FilePersistentUniqueIdPath);

		device.DownloadThumbnail(FilePersistentUniqueIdPath, filePath);

		device.Disconnect();

		Assert.IsTrue(exists1);
		Assert.IsTrue(File.Exists(filePath), "Exists");
		Assert.IsTrue(new FileInfo(filePath).Length > 100, "Length");

	}

	[TestMethod]
	[Description("Download a folder tree from the target.")]
	public void DownloadTreeTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string sourceFolder = Path.Combine(TestDataFolder, "UploadTree");
		string destFolder = Path.Combine(WorkingFolder, "UploadTree");

		bool exists1 = device.DirectoryExists(destFolder);
		if (exists1)
			device.DeleteDirectory(destFolder, true);

		device.UploadFolder(sourceFolder, destFolder);

		string downloadFolder = Path.Combine(TestDataFolder, "DownloadTree");

		if (Directory.Exists(downloadFolder))
			Directory.Delete(downloadFolder, true);

		device.DownloadFolder(destFolder, downloadFolder);

		device.Disconnect();

		//Assert.IsTrue(File.Exists(tempFile), "Exists");

	}

	[TestMethod]
	[Description("Rename a file.")]
	public void RenameFileTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string filePath = Path.Combine(WorkingFolder, "RenameTest.txt");
		string newName = "NewName.txt";
		string newPath = Path.Combine(WorkingFolder, newName);

		if (device.FileExists(filePath))
			device.DeleteFile(filePath);

		if (device.FileExists(newPath))
			device.DeleteFile(newPath);

		using (MemoryStream stream = new(Encoding.UTF8.GetBytes("This is a test.")))
		{
			device.UploadFile(stream, filePath);
		}

		bool exists1 = device.FileExists(filePath);

		device.Rename(filePath, newName);

		bool exists2 = device.FileExists(newPath);

		device.DeleteFile(newPath);
		bool exists3 = device.FileExists(newPath);

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsTrue(exists2, "exists2");
		Assert.IsFalse(exists3, "exists3");
	}

	[TestMethod]
	[Description("Rename a folder.")]
	public void RenameFolderTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string filePath = Path.Combine(WorkingFolder, "RenameFolder");
		string newName = "NewFolder";
		string newPath = Path.Combine(WorkingFolder, newName);

		if (device.DirectoryExists(filePath))
			device.DeleteDirectory(filePath);

		if (device.DirectoryExists(newPath))
			device.DeleteDirectory(newPath);

		device.CreateDirectory(filePath);

		bool exists1 = device.DirectoryExists(filePath);

		device.Rename(filePath, newName);

		bool exists2 = device.DirectoryExists(newPath);

		device.DeleteDirectory(newPath);
		bool exists3 = device.DirectoryExists(newPath);

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsTrue(exists2, "exists2");
		Assert.IsFalse(exists3, "exists3");
	}

	[TestMethod]
	[Description("Writable PersistentUniqueId Test")]
	public void WritablePersistentUniqueIdTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		UploadTestTree(device);

		MediaDirectoryInfo dir = device.GetDirectoryInfo(Path.Combine(WorkingFolder, @"UploadTree\Aaa\Abb"));
		string dirPui = dir.PersistentUniqueId;
		MediaDirectoryInfo dirGet = device.GetFileSystemInfoFromPersistentUniqueId(dirPui) as MediaDirectoryInfo;

		MediaFileInfo file = device.GetFileInfo(Path.Combine(WorkingFolder, @"UploadTree\Aaa\Abb\Acc\Ctest.txt"));
		string filePui = file.PersistentUniqueId;
		MediaFileInfo fileGet = device.GetFileSystemInfoFromPersistentUniqueId(filePui) as MediaFileInfo;

		string tmp = Path.GetTempFileName();
		device.DownloadFileFromPersistentUniqueId(filePui, tmp);
		string text = File.ReadAllText(tmp);

		device.Disconnect();

		Assert.IsNotNull(dirPui, "dirPui");
		Assert.AreEqual(dir, dirGet, "dirGet");

		Assert.IsNotNull(filePui, "filePui");
		Assert.AreEqual(file, fileGet, "fileGet");

		Assert.AreEqual("test", text, "text");
	}

	[TestMethod]
	[Description("Creating a new folder.")]
	public void ReadonlyConnectTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect(MediaDeviceAccess.GenericRead);
		MediaDirectoryInfo root = device.GetRootDirectory();
		List<MediaFileSystemInfo> list = root.EnumerateFileSystemInfos().ToList();

		string newFolder = Path.Combine(WorkingFolder, "Test");
		bool exists1 = device.DirectoryExists(WorkingFolder);
		device.CreateDirectory(newFolder);
		bool exists2 = device.DirectoryExists(newFolder);
		//device.DeleteDirectory(newFolder, true);
		//var exists3 = device.DirectoryExists(newFolder);

		device.Disconnect();

		Assert.IsTrue(exists1, "exists1");
		Assert.IsFalse(exists2, "exists2");
		//Assert.IsFalse(exists3, "exists3");
	}

	[TestMethod]
	[Description("Upload a unix file namne to the target.")]
	public void UploadUnixFileNameTest()
	{
		string workingUnixFolder = Path.Combine(WorkingFolder, "UnixTest");

		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		string sourceFile = Path.Combine(TestDataFolder, "TestFile.txt");
		string destFile = Path.Combine(workingUnixFolder, @"Test:File.txt");

		bool exists1 = device.FileExists(destFile);
		if (exists1)
			device.DeleteFile(destFile);

		device.UploadFile(sourceFile, destFile);

		bool exists = device.FileExists(destFile);

		string[] files = device.GetFiles(workingUnixFolder);

		device.DeleteFile(destFile);

		device.Disconnect();

		Assert.IsTrue(exists, "exists");

	}
}
