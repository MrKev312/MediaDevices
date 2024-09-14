using System.Linq;

using MediaDevices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaDevicesUnitTest;

[TestClass]
public class ConnectorsUnitTest
{
	[TestMethod]
	public void ConnectorsTest()
	{
		System.Collections.Generic.List<MediaDeviceConnector> connectors = MediaDeviceConnectors.Connectors()?.ToList();
	}
}
