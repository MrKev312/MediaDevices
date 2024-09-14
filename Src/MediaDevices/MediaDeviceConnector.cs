using MediaDevices.Internal;

using System;
using System.Runtime.InteropServices;

namespace MediaDevices;

/// <summary>
/// MediaDive connector
/// </summary>
public class MediaDeviceConnector : IConnectionRequestCallback
{
	private readonly IPortableDeviceConnector connector;

	/// <summary>
	/// Event signals if complete
	/// </summary>
	public event EventHandler<CompleteEventArgs> Complete;

	private MediaDeviceConnector()
	{ }

	internal MediaDeviceConnector(IPortableDeviceConnector connector) => this.connector = connector;

	/// <summary>
	/// Connect to service
	/// </summary>
	public void Connect() => connector.Connect(this);

	/// <summary>
	/// Disconnect from service
	/// </summary>
	public void Disconnect() => connector.Disconnect(this);

	/// <summary>
	/// On completed
	/// </summary>
	/// <param name="hrStatus">Status</param>
	public void OnComplete([In, MarshalAs(UnmanagedType.Error)] int hrStatus) => Complete?.Invoke(this, new CompleteEventArgs(hrStatus));
}
