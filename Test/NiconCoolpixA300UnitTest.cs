using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;
using MediaDevicesUnitTest.TestCases;

namespace MediaDevicesUnitTest;

[TestClass]
public class NiconCoolpixA300 : ReadonlyUnitTest
{
	public NiconCoolpixA300()
	{
		// Device Test
		DeviceDescription = "A300";
		DeviceFriendlyName = "A300";
		DeviceManufacture = "NIKON";
		DeviceFirmwareVersion = "COOLPIX A300 V1.4";
		DeviceModel = "A300";
		DeviceSerialNumber = "";
		DeviceDeviceType = DeviceType.Camera;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.Battery;
		DeviceProtocol = "MTP: 1.00";

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];
		FunctionalCategories = [FunctionalCategory.Storage, FunctionalCategory.StillImageCapture];

		// ContentLocation Test
		ContentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = "{00000003-0000-0000-0000-000000000000}";
		FolderPersistentUniqueIdPath = @"\A300\DCIM\100NIKON";
		FilePersistentUniqueId = "{00000004-0000-0000-0000-000000000000}";
		FilePersistentUniqueIdPath = @"\A300\DCIM\100NIKON\DSCN0005.JPG";

		// Exists Test
		ExistingFile = @"\A300\DCIM\100NIKON\DSCN0005.JPG";

		// Directory Info Test
		InfoDirectoryName = "DCIM";
		InfoDirectoryPath = @"\A300\DCIM";
		InfoDirectoryCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoDirectoryLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoDirectoryAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		InfoDirectoryParentName = "A300";
		InfoDirectoryParentPath = @"\A300";
		InfoDirectoryParentCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoDirectoryParentLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoDirectoryParentAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		// File InfoTest
		InfoFileName = "DSCN0005.JPG";
		InfoFilePath = @"\A300\DCIM\100NIKON\DSCN0005.JPG";
		InfoFileLength = 4784013ul;
		InfoFileCreationTime = new DateTime(2017, 11, 15, 20, 55, 54);
		InfoFileLastWriteTime = new DateTime(2017, 11, 15, 20, 55, 54);
		InfoFileAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		InfoFileParentName = "100NIKON";
		InfoFileParentPath = @"\A300\DCIM\100NIKON";
		InfoFileParentCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoFileParentLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		InfoFileParentAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		EnumDirectory = @"\A300\DCIM\100NIKON";
		EnumFolderMask = "*";
		EnumFilesmask = "*_0002*";
		EnumItemMask = "*_0003*";

		EnumAllFolders = [];
		EnumMaskFolders = [];

		EnumAllFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG", @"\A300\DCIM\100NIKON\DSCN0006.JPG", @"\A300\DCIM\100NIKON\DSCN0007.JPG"];
		EnumMaskFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
		EnumMaskRecursiveFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];

		EnumAllItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG", @"\A300\DCIM\100NIKON\DSCN0006.JPG", @"\A300\DCIM\100NIKON\DSCN0007.JPG"];
		EnumMaskItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
		EnumMaskRecursiveItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
	}
}
