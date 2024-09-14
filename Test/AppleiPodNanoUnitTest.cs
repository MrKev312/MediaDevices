using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;
using MediaDevicesUnitTest.TestCases;

namespace MediaDevicesUnitTest;

[TestClass]
public class AppleiPodNanoUnitTest : ReadonlyUnitTest
{
	public AppleiPodNanoUnitTest()
	{
		// Device Test
		DeviceDescription = "iPod";
		DeviceFriendlyName = "AVALON";
		DeviceManufacture = "Apple";
		DeviceFirmwareVersion = "1.62";
		DeviceModel = "iPod";
		DeviceSerialNumber = "YM8382LQ5BD";
		DeviceDeviceType = DeviceType.Generic;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.Battery;

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];

		// ContentLocation Test
		ContentLocations = [""];

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
