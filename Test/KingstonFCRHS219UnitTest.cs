﻿using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class KingstonFCRHS219UnitTest : WritableUnitTest
{
	public KingstonFCRHS219UnitTest()
	{
		// Device Test
		DeviceDescription = "FCR-HS219/1     ";
		DeviceFriendlyName = "T_ASGAR";
		DeviceManufacture = "Kingston";
		DeviceFirmwareVersion = "9722";
		DeviceModel = "FCR-HS219/1     ";
		DeviceSerialNumber = "";
		DeviceDeviceType = DeviceType.Generic;
		DeviceTransport = DeviceTransport.Unspecified;
		DevicePowerSource = PowerSource.External;

		// Capability Test
		SupportedEvents = [Events.ObjectAdded, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Unspecified, ContentType.Folder, ContentType.Audio, ContentType.Video, ContentType.Image, ContentType.Contact];
		FunctionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		ContentLocations = []; // new List<string> { @"\Phone\Pictures", @"\Phone\Pictures", @"\SD card\Pictures" };

		// PersistentUniqueId
		FolderPersistentUniqueId = "K%3B%5CTest";
		FolderPersistentUniqueIdPath = @"\K:\Test";
		FilePersistentUniqueId = "K%3B%5CTest%5Cdesctop.ini";
		FilePersistentUniqueIdPath = @"\K:\Test\desctop.ini";

		// Writable Tests
		WorkingFolder = @"\K:\Test";

		// Exists Test
		//this.existingFile = @"\Interner Speicher\Download\14.jpg";

		//this.infoDirectoryName = "Pictures";
		//this.infoDirectoryPath = @"\SD card\Pictures";
		//this.infoDirectoryCreationTime = new DateTime(2014, 03, 21, 19, 02, 04);
		//this.infoDirectoryLastWriteTime = new DateTime(2017, 01, 07, 16, 54, 38);

		//this.infoDirectoryParentName = "SD card";
		//this.infoDirectoryParentPath = @"\SD card";
		//this.infoDirectoryParentCreationTime = null;
		//this.infoDirectoryParentLastWriteTime = null;

		//this.infoFileName = "Frank2.jpg";
		//this.infoFilePath = @"\SD card\Pictures\Frank2.jpg";
		//this.infoFileLength = 232663ul;
		//this.infoFileCreationTime = new DateTime(2015, 01, 30, 22, 47, 17);
		//this.infoFileLastWriteTime = new DateTime(2015, 01, 30, 22, 47, 22);

		//this.infoFileParentName = "Pictures";
		//this.infoFileParentPath = @"\SD card\Pictures";
		//this.infoFileParentCreationTime = new DateTime(2014, 03, 21, 19, 02, 04);
		//this.infoFileParentLastWriteTime = new DateTime(2017, 01, 07, 16, 54, 38);

		//this.enumDirectory = @"\Phone\Pictures";
		//this.enumFolderMask = "S*";
		//this.enumFilesmask = "desk*";
		//this.enumItemMask = "*es*";

		//this.enumAllFolders = new List<string> { @"\Phone\Pictures\Camera Roll", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots", @"\Phone\Pictures\WhatsApp" };
		//this.enumMaskFolders = new List<string> { @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots", @"\Phone\Pictures\WhatsApp" };

		//this.enumAllFiles = new List<string> { @"\Phone\Pictures\bs.jpg", @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskFiles = new List<string> { @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskRecursiveFiles = new List<string> { @"\Phone\Pictures\Camera Roll\desktop.ini", @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Saved Pictures\desktop.ini" };

		//this.enumAllItems = new List<string> { @"\Phone\Pictures\Camera Roll", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots", @"\Phone\Pictures\WhatsApp", @"\Phone\Pictures\bs.jpg", @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskItems = new List<string> { @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures" };
		//this.enumMaskRecursiveItems = new List<string> { @"\Phone\Pictures\Camera Roll\desktop.ini", @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Saved Pictures\desktop.ini" };

	}
}
