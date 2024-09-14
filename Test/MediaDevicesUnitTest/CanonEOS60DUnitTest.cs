using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;

namespace MediaDevicesUnitTest;

[TestClass]
public class CanonEOS60DUnitTest : ReadonlyUnitTest
{
	public CanonEOS60DUnitTest()
	{


		// Device Test
		deviceDescription = "Canon EOS 60D";
		deviceFriendlyName = "Canon EOS 60D";
		deviceManufacture = "Canon Inc.";
		deviceFirmwareVersion = "3-1.0.9";
		deviceModel = "Canon EOS 60D";
		deviceSerialNumber = "ff76057ad3e84a228e406f41cdd778a6";
		deviceDeviceType = DeviceType.Camera;
		deviceTransport = DeviceTransport.USB;
		devicePowerSource = PowerSource.Battery;

		// Capability Test
		supportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		supportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		supportedContents = [ContentType.Image];

		// ContentLocation Test
		contentLocations = [""];

		// Exists Test
		existingFile = @"\SD\DCIM\100CANON\IMG_2568.JPG";


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
