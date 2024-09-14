﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaDevices;
using MediaDevicesUnitTest.TestCases;

namespace MediaDevicesUnitTest;

[TestClass]
public class MicrosoftLumia950UnitTest : WritableUnitTest
{
	public MicrosoftLumia950UnitTest()
	{
		// Device Test
		DeviceDescription = "Lumia 950";
		DeviceFriendlyName = "";
		DeviceManufacture = "Microsoft";
		DeviceFirmwareVersion = "10.0.15254.0";
		DeviceModel = "Lumia 650 Dual SIM";
		DeviceSerialNumber = "9aaa36ebd8901685d39337ced4b30893";
		DeviceDeviceType = DeviceType.Phone;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.Battery;

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];
		FunctionalCategories = [FunctionalCategory.Storage, FunctionalCategory.RenderingInformation];

		// ContentLocation Test
		ContentLocations = [@"\Phone\Pictures", @"\Phone\Pictures", @"\SD card\Pictures"];

		WorkingFolder = @"\Phone\Documents";

		// Exists Test
		//this.existingFile = @"\Phone\Music\Artist\05 - Decoupage.mp3";

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
