﻿using MediaDevices.Internal;

using System.Collections.Generic;

namespace MediaDevices;

/// <summary>
/// MediaDevice connectors
/// </summary>
public static class MediaDeviceConnectors
{

	#region static

	private static readonly IEnumPortableDeviceConnectors connectors;

	static MediaDeviceConnectors()
	{
		IEnumPortableDeviceConnectors inst = (IEnumPortableDeviceConnectors)new EnumPortableDeviceConnectors();
		connectors = inst;
	}

	#endregion

	/// <summary>
	/// Get connextors.
	/// </summary>
	/// <returns>List of connectors</returns>
	public static IEnumerable<MediaDeviceConnector> Connectors()
	{

		//connectors.Reset();

		uint num = 1;
		connectors.Next(1, out IPortableDeviceConnector connector, ref num);

		return [new(connector)];

		//IPortableDeviceConnector[] connectorArray = new IPortableDeviceConnector[10]; 
		//uint num = 10;
		//connectors.Next(10, ref connectorArray, ref num);

		//connectors.Clone(out IEnumPortableDeviceConnectors test);

		//connectorArray = new IPortableDeviceConnector[10];
		//num = 10;
		//test.Next(10, ref connectorArray, ref num);

		//return connectorArray?.Select(c => new MediaDeviceConnector(c));
	}
}
