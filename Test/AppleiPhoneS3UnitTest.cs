using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;
using MediaDevicesUnitTest.TestCases;

namespace MediaDevicesUnitTest;

[TestClass]
public class AppleiPhoneS3UnitTest : ReadonlyUnitTest
{
	public AppleiPhoneS3UnitTest()
	{
		// Find function
		deviceSelect = d => d.Description == deviceDescription;

		// Device Test
		deviceDescription = "Apple iPhone";
		deviceFriendlyName = "iPhone von Egon";
		deviceManufacture = "Apple Inc.";
		deviceFirmwareVersion = "4.2.1";
		deviceModel = "Apple iPhone";
		deviceSerialNumber = "889155UBY7H";
		deviceDeviceType = DeviceType.Camera;
		deviceTransport = DeviceTransport.USB;
		devicePowerSource = PowerSource.Battery;
		deviceProtocol = "PTP: 1.10";

		// Capability Test
		supportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		supportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		supportedContents = [ContentType.Image];
		functionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		contentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = "{00430045-0049-004D-0100-010000000000}";
		FolderPersistentUniqueIdPath = @"\Internal Storage\DCIM";
		FilePersistentUniqueId = "{00070064-0015-0018-3100-3100D6213600}";
		FilePersistentUniqueIdPath = @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";

		// Exists Test
		existingFile = @"Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";

		infoDirectoryName = "DCIM";
		infoDirectoryPath = @"\Internal Storage\DCIM";
		infoDirectoryCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		infoDirectoryLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		infoDirectoryParentName = "Internal Storage";
		infoDirectoryParentPath = @"\Internal Storage";
		infoDirectoryParentCreationTime = null;
		infoDirectoryParentLastWriteTime = null;

		infoFileName = "IMG_0001.JPG";
		infoFilePath = @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";
		infoFileLength = 467430ul;
		infoFileCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		infoFileLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		infoFileParentName = "800AAAAA";
		infoFileParentPath = @"\Internal Storage\DCIM\800AAAAA";
		infoFileParentCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		infoFileParentLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		enumDirectory = @"\Internal Storage\DCIM\800AAAAA";
		enumFolderMask = "*";
		enumFilesmask = "*_0002*";
		enumItemMask = "*_0003*";

		enumAllFolders = [];
		enumMaskFolders = [];

		enumAllFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		enumMaskFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG"];
		enumMaskRecursiveFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG"];

		enumAllItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		enumMaskItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		enumMaskRecursiveItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
	}
}
