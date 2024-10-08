﻿using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class AmazonFire7UnitTest : WritableUnitTest
{
	public AmazonFire7UnitTest()
	{
		// Device Test
		DeviceDescription = "Fire"; // "MTP-USB-Gerät"; // ;
		DeviceFriendlyName = "Ralf's Fire 7";
		DeviceManufacture = "Amazon";
		DeviceFirmwareVersion = "1.0";
		DeviceModel = "Fire";
		DeviceSerialNumber = "G0W0MB06729405K5";
		DeviceDeviceType = DeviceType.MediaPlayer;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.Battery;
		DeviceProtocol = "MTP: 1.00";

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];
		FunctionalCategories = [FunctionalCategory.Storage];

		// ContentLocation Test
		ContentLocations = []; // new List<string> { @"\Phone\Pictures", @"\Phone\Pictures", @"\SD card\Pictures" };

		// PersistentUniqueId
		FolderPersistentUniqueId = "{00000027-0001-0001-0000-000000000000}";
		FolderPersistentUniqueIdPath = @"\Interner Speicher\Download";
		FilePersistentUniqueId = "{000001A2-0001-0001-0000-000000000000}";
		FilePersistentUniqueIdPath = @"\Interner Speicher\Download\.DS_Store";

		// Writable Tests
		WorkingFolder = @"\Interner Speicher\tmp";

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
