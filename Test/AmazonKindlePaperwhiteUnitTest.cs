﻿using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class AmazonKindlePaperwhiteUnitTest : WritableUnitTest
{
	private readonly string deviceLetter = "E";

	public AmazonKindlePaperwhiteUnitTest()
	{
		// Device Test
		DeviceDescription = "Internal Storage";
		DeviceFriendlyName = "KINDLE";
		DeviceManufacture = "Kindle  ";
		DeviceFirmwareVersion = "0100";
		DeviceModel = "Internal Storage";
		DeviceSerialNumber = ""; // G090G10573570BNQ
		DeviceDeviceType = DeviceType.Generic;
		DeviceTransport = DeviceTransport.Unspecified;
		DevicePowerSource = PowerSource.External;
		DeviceProtocol = "MSC:";

		// Capability Test
		SupportedEvents = [Events.ObjectAdded, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [ Commands.ObjectEnumerationStartFind, Commands.ObjectEnumerationFindNext, Commands.ObjectEnumerationEndFind,
				Commands.ObjectManagementDeleteObjects, Commands.ObjectManagementCreateObjectWithPropertiesOnly, Commands.ObjectManagementCreateObjectWithPropertiesAndData,
				Commands.ObjectManagementWriteObjectData, Commands.ObjectManagementCommitObject, Commands.ObjectManagementRevertObject
		];
		SupportedContents = [ContentType.Unspecified, ContentType.Folder, ContentType.Audio, ContentType.Video, ContentType.Image, ContentType.Contact];
		FunctionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		ContentLocations = [];

		// PersistentUniqueId
		FolderPersistentUniqueId = $"{deviceLetter}%3B%5Csystem%5Cstartactions";
		FolderPersistentUniqueIdPath = $@"\{deviceLetter}:\system\startactions";
		FilePersistentUniqueId = $"{deviceLetter}%3B%5Csystem%5Cversion.txt";
		FilePersistentUniqueIdPath = $@"\{deviceLetter}:\system\version.txt";

		// Writable Tests
		WorkingFolder = $@"\{deviceLetter}:\documents";

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
