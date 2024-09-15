using MediaDevices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MediaDevicesUnitTest.TestCases;

public abstract class UnitTest
{
	// Device Select
	protected Func<MediaDevice, bool> DeviceSelect { get; set; }

	// Device Test
	protected string DeviceDescription { get; set; }
	protected string DeviceFriendlyName { get; set; }
	protected string DeviceManufacture { get; set; }
	protected string DeviceFirmwareVersion { get; set; }
	protected string DeviceModel { get; set; }
	protected string DeviceSerialNumber { get; set; }
	protected DeviceType DeviceDeviceType { get; set; }
	protected DeviceTransport DeviceTransport { get; set; }
	protected PowerSource DevicePowerSource { get; set; }
	protected string DeviceProtocol { get; set; }

	// Capability Test
	protected List<Events> SupportedEvents { get; set; }
	protected List<Commands> SupportedCommands { get; set; }
	protected List<ContentType> SupportedContents { get; set; }
	protected List<FunctionalCategory> FunctionalCategories { get; set; }

	// ContentLocation Test
	protected List<string> ContentLocations { get; set; }

	// PersistentUniqueId
	protected string FolderPersistentUniqueId { get; set; }
	protected string FolderPersistentUniqueIdPath { get; set; }
	protected string FilePersistentUniqueId { get; set; }
	protected string FilePersistentUniqueIdPath { get; set; }

	/// <summary>
	/// Gets or sets the test context which provides information about and functionality for the current test run.
	///</summary>
	public TestContext TestContext { get; set; }

	public UnitTest() => DeviceSelect = d => d.Description == DeviceDescription && d.FriendlyName == DeviceFriendlyName;

	//protected void FindDeviceLetter(string pnpDeviceId)
	//{
	//    foreach (ManagementObject device in new ManagementObjectSearcher(@"SELECT * FROM Win32_DiskDrive WHERE InterfaceType LIKE 'USB%'").Get())
	//    {
	//        string s1 = ((string)device.GetPropertyValue("DeviceID"));
	//        string s2 = ((string)device.GetPropertyValue("PNPDeviceID"));

	//        foreach (ManagementObject partition in new ManagementObjectSearcher(
	//            "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + device.Properties["DeviceID"].Value
	//            + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
	//        {
	//            foreach (ManagementObject disk in new ManagementObjectSearcher(
	//                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
	//                            + partition["DeviceID"]
	//                            + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
	//            {
	//                string s3 = ("Drive letter " + disk["Name"]);
	//            }
	//        }
	//    }
	//}

	[TestMethod]
	[Description("Basic device tests")]
	public void DeviceTest()
	{
		MediaDevice[] devices = MediaDevice.GetDevices().ToArray();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");

		string description = device.Description;
		string friendlyName = device.FriendlyName;
		string manufacture = device.Manufacturer;

		device.Connect();
		string firmwareVersion = device.FirmwareVersion;
		PowerSource powerSource = device.PowerSource;
		int? powerLevel = device.PowerLevel;
		string model = device.Model;
		string serialNumber = device.SerialNumber;
		DeviceType? deviceType = device.DeviceType;
		DeviceTransport transport = device.Transport;
		string protocol = device.Protocol;
		device.Disconnect();

		Assert.AreEqual(DeviceDescription, description, "Description");
		Assert.AreEqual(DeviceFriendlyName, friendlyName, "FriendlyName");
		Assert.AreEqual(DeviceManufacture, manufacture, "Manufacture");

		Assert.AreEqual(DeviceFirmwareVersion, firmwareVersion, "FirmwareVersion");
		Assert.AreEqual(DeviceModel, model, "Model");
		Assert.AreEqual(DeviceSerialNumber, serialNumber, "SerialNumber");
		Assert.AreEqual(DeviceDeviceType, deviceType, "DeviceType");
		Assert.AreEqual(DeviceTransport, transport, "Transport");
		Assert.AreEqual(DevicePowerSource, powerSource, "PowerSource");
		Assert.AreEqual(DeviceProtocol, protocol, "Protocol");
		Assert.IsTrue(powerLevel > 0, "PowerLevel");
	}

	[TestMethod]
	[Description("Check compatibility informations.")]
	public void CapabilityTest()
	{
		List<MediaDevice> devices = MediaDevice.GetDevices().ToList();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		List<Events> events = device.SupportedEvents()?.ToList();
		List<Commands> commands = device.SupportedCommands()?.ToList();
		List<ContentType> contents = device.SupportedContentTypes(FunctionalCategory.All)?.ToList();
		List<FunctionalCategory> categories = device.FunctionalCategories()?.ToList();

		List<string> stillImageCaptureObjects = device.FunctionalObjects(FunctionalCategory.StillImageCapture).ToList();
		List<string> storageObjects = device.FunctionalObjects(FunctionalCategory.Storage).ToList();
		List<string> smsObjects = device.FunctionalObjects(FunctionalCategory.SMS).ToList();

		device.Disconnect();

		CollectionAssert.IsSubsetOf(SupportedEvents, events, "Events");
		CollectionAssert.IsSubsetOf(SupportedCommands, commands, "Commands");
		CollectionAssert.IsSubsetOf(SupportedContents, contents, "Contents");
		CollectionAssert.AreEquivalent(FunctionalCategories, categories, "Categories");
	}

	[TestMethod]
	[Description("Check content locations functionality.")]
	public void ContentLocationTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		List<string> locations = device.GetContentLocations(ContentType.Image).ToList();

		device.Disconnect();

		CollectionAssert.AreEquivalent(ContentLocations, locations, "Locations");
	}

	[TestMethod]
	[Description("Check persistent unique id functionality.")]
	public void PersistentUniqueIdTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo dir = device.GetFileSystemInfoFromPersistentUniqueId(FolderPersistentUniqueId) as MediaDirectoryInfo;

		MediaFileInfo file = device.GetFileSystemInfoFromPersistentUniqueId(FilePersistentUniqueId) as MediaFileInfo;
		device.Disconnect();

		Assert.IsNotNull(dir, "Dir");
		Assert.IsTrue(dir.Attributes.HasFlag(MediaFileAttributes.Directory), "dir.IsDirectory");
		Assert.AreEqual(FolderPersistentUniqueIdPath, dir.FullName, "dir.FullName");

		Assert.IsNotNull(file, "File");
		Assert.IsTrue(file.Attributes.HasFlag(MediaFileAttributes.Normal), "file.IsFile");
		Assert.AreEqual(FilePersistentUniqueIdPath, file.FullName, "file.FullName");
	}

	[TestMethod]
	[Description("Check persistent unique id functionality.")]
	public void FriendlyNameTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");

		string disconnectedFriendlyName = device.FriendlyName;

		device.Connect();

		string connectedFriendlyName = device.FriendlyName;

		// some devices use only upper letters in friendly names
		device.FriendlyName = "DUMMY";

		string dummyFriendlyName = device.FriendlyName;

		device.Disconnect();

		string disconnectedDummyFriendlyName = device.FriendlyName;

		device.Connect();

		string connectedDummyFriendlyName = device.FriendlyName;

		device.FriendlyName = connectedFriendlyName;

		device.Disconnect();

		Assert.AreEqual(DeviceFriendlyName, disconnectedFriendlyName, "disconnectedFriendlyName");
		Assert.AreEqual(DeviceFriendlyName, connectedFriendlyName, "connectedFriendlyName");
		Assert.AreEqual("DUMMY", dummyFriendlyName, "dummyFriendlyName");
		Assert.AreEqual("DUMMY", disconnectedDummyFriendlyName, "disconnectedDummyFriendlyName");
		Assert.AreEqual("DUMMY", connectedDummyFriendlyName, "connectedDummyFriendlyName");
	}

	[TestMethod]
	[Description("Speed test.")]
	public void SpeedTest()
	{
		IEnumerable<MediaDevice> devices = MediaDevice.GetDevices();
		MediaDevice device = devices.FirstOrDefault(DeviceSelect);
		Assert.IsNotNull(device, "Device");
		device.Connect();

		MediaDirectoryInfo root = device.GetRootDirectory();
		Stopwatch stopwatch = Stopwatch.StartNew();

		List<MediaFileSystemInfo> list = root.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).ToList();

		stopwatch.Stop();

		device.Disconnect();

		double milliseconds = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;

		//Assert.AreEqual(0.0, milliseconds, "time");
	}

	[TestMethod]
	[Description("Architecture test.")]
	public void ArchitectureTest()
	{
		int size = IntPtr.Size;
		TestContext.WriteLine($"Pointer size if {size}");
	}
}
