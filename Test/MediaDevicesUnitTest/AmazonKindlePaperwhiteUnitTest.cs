﻿using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class AmazonKindlePaperwhiteUnitTest : WritableUnitTest
{
	private string deviceLetter = "E";

	public AmazonKindlePaperwhiteUnitTest()
	{
		// Device Test
		deviceDescription = "Internal Storage";
		deviceFriendlyName = "KINDLE";
		deviceManufacture = "Kindle  ";
		deviceFirmwareVersion = "0100";
		deviceModel = "Internal Storage";
		deviceSerialNumber = ""; // G090G10573570BNQ
		deviceDeviceType = DeviceType.Generic;
		deviceTransport = DeviceTransport.Unspecified;
		devicePowerSource = PowerSource.External;
		deviceProtocol = "MSC:";

		// Capability Test
		supportedEvents = [Events.ObjectAdded, Events.ObjectRemoved, Events.ObjectUpdated];
		supportedCommands = [ Commands.ObjectEnumerationStartFind, Commands.ObjectEnumerationFindNext, Commands.ObjectEnumerationEndFind,
				Commands.ObjectManagementDeleteObjects, Commands.ObjectManagementCreateObjectWithPropertiesOnly, Commands.ObjectManagementCreateObjectWithPropertiesAndData,
				Commands.ObjectManagementWriteObjectData, Commands.ObjectManagementCommitObject, Commands.ObjectManagementRevertObject
		];
		supportedContents = [ContentType.Unspecified, ContentType.Folder, ContentType.Audio, ContentType.Video, ContentType.Image, ContentType.Contact];
		functionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		contentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = $"{deviceLetter}%3B%5Csystem%5Cstartactions";
		FolderPersistentUniqueIdPath = $@"\{deviceLetter}:\system\startactions";
		FilePersistentUniqueId = $"{deviceLetter}%3B%5Csystem%5Cversion.txt";
		FilePersistentUniqueIdPath = $@"\{deviceLetter}:\system\version.txt";

		// Writable Tests
		workingFolder = $@"\{deviceLetter}:\documents";

		// Exists Test
		//this.existingFile = @"\E:\documents\Old Firehand_B004WLCSLC.sdr\Old Firehand_B004WLCSLC.phl";

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
