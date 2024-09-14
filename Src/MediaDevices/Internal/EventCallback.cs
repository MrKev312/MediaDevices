namespace MediaDevices.Internal;

internal sealed class EventCallback : IPortableDeviceEventCallback
{
	private MediaDevice device;

	public EventCallback(MediaDevice device)
	{
		this.device = device;
	}

	public void OnEvent(IPortableDeviceValues pEventParameters)
	{
		device.CallEvent(pEventParameters);
	}
}
