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
		DeviceSelect = d => d.Description == DeviceDescription;

		// Device Test
		DeviceDescription = "Apple iPhone";
		DeviceFriendlyName = "iPhone von Egon";
		DeviceManufacture = "Apple Inc.";
		DeviceFirmwareVersion = "4.2.1";
		DeviceModel = "Apple iPhone";
		DeviceSerialNumber = "889155UBY7H";
		DeviceDeviceType = DeviceType.Camera;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.Battery;
		DeviceProtocol = "PTP: 1.10";

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];
		FunctionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		ContentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = "{00430045-0049-004D-0100-010000000000}";
		FolderPersistentUniqueIdPath = @"\Internal Storage\DCIM";
		FilePersistentUniqueId = "{00070064-0015-0018-3100-3100D6213600}";
		FilePersistentUniqueIdPath = @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";

		// Exists Test
		ExistingFile = @"Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";

		InfoDirectoryName = "DCIM";
		InfoDirectoryPath = @"\Internal Storage\DCIM";
		InfoDirectoryCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		InfoDirectoryLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		InfoDirectoryParentName = "Internal Storage";
		InfoDirectoryParentPath = @"\Internal Storage";
		InfoDirectoryParentCreationTime = null;
		InfoDirectoryParentLastWriteTime = null;

		InfoFileName = "IMG_0001.JPG";
		InfoFilePath = @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";
		InfoFileLength = 467430ul;
		InfoFileCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		InfoFileLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		InfoFileParentName = "800AAAAA";
		InfoFileParentPath = @"\Internal Storage\DCIM\800AAAAA";
		InfoFileParentCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		InfoFileParentLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		EnumDirectory = @"\Internal Storage\DCIM\800AAAAA";
		EnumFolderMask = "*";
		EnumFilesmask = "*_0002*";
		EnumItemMask = "*_0003*";

		EnumAllFolders = [];
		EnumMaskFolders = [];

		EnumAllFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		EnumMaskFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG"];
		EnumMaskRecursiveFiles = [@"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG"];

		EnumAllItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		EnumMaskItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
		EnumMaskRecursiveItems = [@"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG"];
	}
}
