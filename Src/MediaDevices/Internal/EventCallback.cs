namespace MediaDevices.Internal;

internal sealed class EventCallback(MediaDevice device) : IPortableDeviceEventCallback
{
	public void OnEvent(IPortableDeviceValues pEventParameters) => device.CallEvent(pEventParameters);
}
