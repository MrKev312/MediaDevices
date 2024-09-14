using MediaDevices;

using MediaDevicesUnitTest.TestCases;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class NokiaLumia630UnitTest : WritableUnitTest
{
	public NokiaLumia630UnitTest()
	{
		// Device Select
		DeviceSelect = device => device.Description == DeviceDescription;

		// Device Test
		DeviceDescription = "Lumia 630 Dual SIM";
		DeviceFriendlyName = "Ralf Phone";
		DeviceManufacture = "NOKIA";
		DeviceFirmwareVersion = "8.10.14234.0";
		DeviceModel = "Lumia 630 Dual SIM";
		DeviceSerialNumber = "29918575d76ae110111b8ced6429e399";
		DeviceDeviceType = DeviceType.Phone;
		DeviceTransport = DeviceTransport.USB;
		DevicePowerSource = PowerSource.External;

		// Capability Test
		SupportedEvents = [Events.DeviceReset, Events.ObjectRemoved, Events.ObjectUpdated];
		SupportedCommands = [Commands.ObjectEnumerationStartFind, Commands.ObjectManagementDeleteObjects];
		SupportedContents = [ContentType.Image];

		// ContentLocation Test
		ContentLocations = [@"\SD card\Pictures", @"\Phone\Pictures", @"\SD card\Pictures"];

		WorkingFolder = @"\Phone\Documents";

		// Exists Test
		//this.existingFile = @"\Phone\Music\Artist\OMD";

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

		//this.enumAllFolders = new List<string> { @"\Phone\Pictures\Camera Roll", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots" };
		//this.enumMaskFolders = new List<string> { @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots" };

		//this.enumAllFiles = new List<string> { @"\Phone\Pictures\bs.jpg", @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskFiles = new List<string> { @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskRecursiveFiles = new List<string> { @"\Phone\Pictures\Camera Roll\desktop.ini", @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Saved Pictures\desktop.ini" };

		//this.enumAllItems = new List<string> { @"\Phone\Pictures\Camera Roll", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Screenshots", @"\Phone\Pictures\bs.jpg", @"\Phone\Pictures\desktop.ini" };
		//this.enumMaskItems = new List<string> { @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures" };
		//this.enumMaskRecursiveItems = new List<string> { @"\Phone\Pictures\Camera Roll\desktop.ini", @"\Phone\Pictures\desktop.ini", @"\Phone\Pictures\Sample Pictures", @"\Phone\Pictures\Saved Pictures", @"\Phone\Pictures\Saved Pictures\desktop.ini" };
	}
}
