using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;

namespace MediaDevicesUnitTest;

[TestClass]
public class NiconCoolpixA300 : ReadonlyUnitTest
{
	public NiconCoolpixA300()
	{
		// Device Test
		deviceDescription = "A300";
		deviceFriendlyName = "A300";
		deviceManufacture = "NIKON";
		deviceFirmwareVersion = "COOLPIX A300 V1.4";
		deviceModel = "A300";
		deviceSerialNumber = "";
		deviceDeviceType = DeviceType.Camera;
		deviceTransport = DeviceTransport.USB;
		devicePowerSource = PowerSource.Battery;
		deviceProtocol = "MTP: 1.00";

		// Capability Test
		supportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		supportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		supportedContents = [ContentType.Image];
		functionalCategories = [FunctionalCategory.Storage, FunctionalCategory.StillImageCapture];

		// ContentLocation Test
		contentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = "{00000003-0000-0000-0000-000000000000}";
		FolderPersistentUniqueIdPath = @"\A300\DCIM\100NIKON";
		FilePersistentUniqueId = "{00000004-0000-0000-0000-000000000000}";
		FilePersistentUniqueIdPath = @"\A300\DCIM\100NIKON\DSCN0005.JPG";

		// Exists Test
		existingFile = @"\A300\DCIM\100NIKON\DSCN0005.JPG";

		// Directory Info Test
		infoDirectoryName = "DCIM";
		infoDirectoryPath = @"\A300\DCIM";
		infoDirectoryCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoDirectoryLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoDirectoryAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		infoDirectoryParentName = "A300";
		infoDirectoryParentPath = @"\A300";
		infoDirectoryParentCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoDirectoryParentLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoDirectoryParentAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		// File InfoTest
		infoFileName = "DSCN0005.JPG";
		infoFilePath = @"\A300\DCIM\100NIKON\DSCN0005.JPG";
		infoFileLength = 4784013ul;
		infoFileCreationTime = new DateTime(2017, 11, 15, 20, 55, 54);
		infoFileLastWriteTime = new DateTime(2017, 11, 15, 20, 55, 54);
		infoFileAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		infoFileParentName = "100NIKON";
		infoFileParentPath = @"\A300\DCIM\100NIKON";
		infoFileParentCreationTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoFileParentLastWriteTime = new DateTime(0001, 1, 1, 0, 0, 0);
		infoFileParentAuthoredTime = new DateTime(0001, 1, 1, 0, 0, 0);

		enumDirectory = @"\A300\DCIM\100NIKON";
		enumFolderMask = "*";
		enumFilesmask = "*_0002*";
		enumItemMask = "*_0003*";

		enumAllFolders = [];
		enumMaskFolders = [];

		enumAllFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG", @"\A300\DCIM\100NIKON\DSCN0006.JPG", @"\A300\DCIM\100NIKON\DSCN0007.JPG"];
		enumMaskFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
		enumMaskRecursiveFiles = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];

		enumAllItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG", @"\A300\DCIM\100NIKON\DSCN0006.JPG", @"\A300\DCIM\100NIKON\DSCN0007.JPG"];
		enumMaskItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
		enumMaskRecursiveItems = [@"\A300\DCIM\100NIKON\DSCN0005.JPG"];
	}
}
