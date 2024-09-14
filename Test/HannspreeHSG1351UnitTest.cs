using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class HannspreeHSG1351UnitTest : WritableUnitTest
{
	public HannspreeHSG1351UnitTest()
	{
		//this.supDevicePowerLevel = false;
		//this.supWritable = false;
		//this.supEvent = false;
		//this.supContentLocation = false;

		// Device Test
		deviceDescription = "HSG1351";
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

		workingFolder = @"\SD\DCIM\100CANON";

		// Exists Test
		//this.existingFile = @"\SD\DCIM\100CANON\IMG_2568.JPG";

		//this.infoDirectoryName = "DCIM";
		//this.infoDirectoryPath = @"\Internal Storage\DCIM";
		//this.infoDirectoryCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		//this.infoDirectoryLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		//this.infoDirectoryParentName = "Internal Storage";
		//this.infoDirectoryParentPath = @"\Internal Storage";
		//this.infoDirectoryParentCreationTime = null;
		//this.infoDirectoryParentLastWriteTime = null;

		//this.infoFileName = "IMG_0001.JPG";
		//this.infoFilePath = @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG";
		//this.infoFileLength = 467430ul;
		//this.infoFileCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		//this.infoFileLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		//this.infoFileParentName = "800AAAAA";
		//this.infoFileParentPath = @"\Internal Storage\DCIM\800AAAAA";
		//this.infoFileParentCreationTime = new DateTime(2000, 1, 27, 19, 47, 54);
		//this.infoFileParentLastWriteTime = new DateTime(2000, 1, 27, 19, 47, 54);

		//this.enumDirectory = @"\Internal Storage\DCIM\800AAAAA";
		//this.enumFolderMask = "*";
		//this.enumFilesmask = "*_0002*";
		//this.enumItemMask = "*_0003*";

		//this.enumAllFolders = new List<string> { };
		//this.enumMaskFolders = new List<string> { };

		//this.enumAllFiles = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG" };
		//this.enumMaskFiles = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG" };
		//this.enumMaskRecursiveFiles = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG" };

		//this.enumAllItems = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0001.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0002.JPG", @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG" };
		//this.enumMaskItems = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG" };
		//this.enumMaskRecursiveItems = new List<string> { @"\Internal Storage\DCIM\800AAAAA\IMG_0003.JPG" };
	}
}
