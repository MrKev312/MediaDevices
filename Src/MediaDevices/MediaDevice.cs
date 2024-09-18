using MediaDevices.Internal;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MediaDevices;

/// <summary>
/// Represents a portable device.
/// </summary>
[DebuggerDisplay("{FriendlyName}, {Manufacturer}, {Description}")]
public sealed class MediaDevice : IDisposable
{
	// https://msdn.microsoft.com/en-us/ie/aa645736%28v=vs.94%29?f=255&MSPPError=-2147217396

	#region Fields

	internal IPortableDevice? device;
	internal IPortableDeviceContent? deviceContent;
	internal IPortableDeviceProperties? deviceProperties;
	private IPortableDeviceCapabilities? deviceCapabilities;
	private IPortableDeviceValues? deviceValues;
	private string friendlyName = string.Empty;
	private string? eventCookie;
	private EventCallback eventCallback;

	#endregion

	#region events

	/// <summary>
	/// This event is sent after a new object is available on the device.
	/// </summary>
	public event EventHandler<ObjectAddedEventArgs> ObjectAdded = delegate { };

	/// <summary>
	/// This event is sent after a previously existing object has been removed from the device.
	/// </summary>
	/// 
	public event EventHandler<MediaDeviceEventArgs> ObjectRemoved = delegate { };

	/// <summary>
	/// This event is sent after an object has been updated such that any connected client should refresh its view of that object.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> ObjectUpdated = delegate { };

	/// <summary>
	/// This event indicates that the device is about to be reset, and all connected clients should close their connection to the device. 
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> DeviceReset = delegate { };

	/// <summary>
	/// This event indicates that the device capabilities have changed. Clients should re-query the device if they have made any decisions based on device capabilities.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> DeviceCapabilitiesUpdated = delegate { };

	/// <summary>
	/// This event indicates the progress of a format operation on a storage object.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> StorageFormat = delegate { };

	/// <summary>
	/// This event is sent to request an application to transfer a particular object from the device.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> ObjectTransferRequest = delegate { };

	/// <summary>
	/// This event is sent when a driver for a device is being unloaded. This is typically a result of the device being unplugged.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> DeviceRemoved = delegate { };

	/// <summary>
	/// This event is sent when a driver has completed invoking a service method. This event must be sent even when the method fails.
	/// </summary>
	public event EventHandler<MediaDeviceEventArgs> ServiceMethodComplete = delegate { };

	#endregion

	#region static

	private static readonly IPortableDeviceManager? deviceManager;
	private static readonly IPortableDeviceServiceManager? serviceManager;

	private static List<MediaDevice> devices = [];
	private static List<MediaDevice> privateDevices = [];

	static MediaDevice()
	{
		try
		{
			deviceManager = (IPortableDeviceManager)new PortableDeviceManager();
			serviceManager = (IPortableDeviceServiceManager)deviceManager;

			//var x = new MediaDevMgr();
			//var f = new MediaDevMgrClassFactory();
			//IWMDeviceManager3 devManager = (IWMDeviceManager3)f;

			//devManager.GetRevision(out var revision);
			//devManager.GetDeviceCount(out var count);
			//devManager.EnumDevices2(out var e);
		}
		catch (Exception ex)
		{
			Trace.TraceError(ex.ToString());
		}

		// #define WMDM_E_NOTCERTIFIED                     0x80045005L
	}

	/// <summary>
	/// Returns an enumerable collection of currently available portable devices.
	/// </summary>
	/// <returns>>An enumerable collection of portable devices currently available.</returns>
	public static IEnumerable<MediaDevice> GetDevices()
	{
		deviceManager.RefreshDeviceList();

		// get number of devices
		uint count = 0;
		deviceManager.GetDevices(null, ref count);

		if (count == 0)
		{
			return [];
		}

		// get device IDs
		string[] deviceIds = new string[count];
		deviceManager.GetDevices(deviceIds, ref count);

		if (devices == null)
		{
			devices = deviceIds.Select(d => new MediaDevice(d)).ToList();
		}
		else
		{
			UpdateDeviceList(devices, deviceIds);
		}

		return devices;
	}

	private static void UpdateDeviceList(List<MediaDevice> deviceList, string[] deviceIdList)
	{
		List<string> idList = [.. deviceIdList];

		// remove
		List<string> remove = deviceList.Where(d => !idList.Contains(d.DeviceId)).Select(d => d.DeviceId).ToList();
		_ = deviceList.RemoveAll(d => remove.Contains(d.DeviceId));

		// add
		List<string> add = idList.Where(id => !deviceList.Select(d => d.DeviceId).Contains(id)).ToList();
		deviceList.AddRange(add.Select(id => new MediaDevice(id)));
	}

	//public static IEnumerable<MediaDevice> GetDevices(FunctionalCategory category)
	//{
	//    if (category == FunctionalCategory.All)
	//    {
	//        return GetDevices();
	//    }

	//    var devices = GetDevices();

	//    var dev = devices.FirstOrDefault();

	//    dev.deviceCapabilities
	//}

	/// <summary>
	/// Returns an enumerable collection of currently available private portable devices.
	/// </summary>
	/// <returns>>An enumerable collection of private portable devices currently available.</returns>
	public static IEnumerable<MediaDevice> GetPrivateDevices()
	{
		deviceManager.RefreshDeviceList();

		// get number of devices
		uint count = 0;
		deviceManager.GetPrivateDevices(null, ref count);

		if (count == 0)
		{
			return [];
		}

		// get device IDs
		string[] deviceIds = new string[count];
		deviceManager.GetPrivateDevices(deviceIds, ref count);

		if (privateDevices == null)
		{
			privateDevices = deviceIds.Select(d => new MediaDevice(d)).ToList();
		}
		else
		{
			UpdateDeviceList(privateDevices, deviceIds);
		}

		return privateDevices;
	}

	#endregion

	#region constructor

	private MediaDevice(string deviceId)
	{
		DeviceId = deviceId;
		IsCaseSensitive = false;

		uint count = 256;
		try
		{
			count = 256;
			StringBuilder sb = new((int)count);
			deviceManager.GetDeviceDescription(deviceId, sb, ref count);
			Description = sb.ToString(); //new string(buffer, 0, (int)count - 1);
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
			Description = string.Empty;
		}

		try
		{
			count = 256;
			StringBuilder sb = new((int)count);
			deviceManager.GetDeviceFriendlyName(deviceId, sb, ref count);
			friendlyName = sb.ToString();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
			friendlyName = string.Empty;
		}

		try
		{
			count = 256;
			StringBuilder sb = new((int)count);
			deviceManager.GetDeviceManufacturer(deviceId, sb, ref count);
			Manufacturer = sb.ToString();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
			Description = string.Empty;
		}

		//this.device = new PortableDeviceApiLib.PortableDevice();
		device = (IPortableDevice)new PortableDevice();
	}

	/// <summary>
	/// Releases the resources used by the PortableDevices.PortableDevice.
	/// </summary>
	public void Dispose() => Disconnect();

	#endregion

	#region Properties

	/// <summary>
	/// Is portable device connected.
	/// </summary>
	public bool IsConnected { get; private set; }

	/// <summary>
	/// Select if path is case sensitive or not. Default is not. 
	/// </summary>
	public bool IsCaseSensitive { get; private set; }

	/// <summary>
	/// Device Id of the portable device.
	/// </summary>
	/// <remarks>Readable when not connected.</remarks>
	public string DeviceId { get; private set; }

	/// <summary>
	/// Description of the portable device.
	/// </summary>
	/// <remarks>Readable when not connected.</remarks>
	public string Description { get; private set; }

	/// <summary>
	/// Friendly name of the portable device.
	/// </summary>
	/// <remarks>Readable when not connected.</remarks>
	/// <exception cref="NotConnectedException">device is not connected. only for setter</exception>
	public string FriendlyName
	{
		get => IsConnected && deviceValues.TryGetStringValue(WPD.DEVICE_FRIENDLY_NAME, out string val)
				? val
				: friendlyName;
		set
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			// set new friendly name
			IPortableDeviceValues devInValues = (IPortableDeviceValues)new PortableDeviceValues();
			devInValues.SetStringValue(ref WPD.DEVICE_FRIENDLY_NAME, value);
			deviceProperties.SetValues(Item.RootId, devInValues, out _);

			// reload device values with new friendly name 
			deviceProperties.GetValues(Item.RootId, null, out deviceValues);

			// reload disconnected friendly name
			try
			{
				char[] buffer = new char[260];
				uint count = 256;
				StringBuilder sb = new((int)count);
				deviceManager.GetDeviceFriendlyName(DeviceId, sb, ref count);
				friendlyName = sb.ToString();
			}
			catch (COMException ex)
			{
				Trace.WriteLine(ex.ToString());
				friendlyName = string.Empty;
			}
		}
	}

	/// <summary>
	/// Manufacturer of the portable device.
	/// </summary>
	/// <remarks>Readable when not connected.</remarks>
	public string Manufacturer { get; private set; }

	/// <summary>
	/// Sync partner of the device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string SyncPartner
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetStringValue(WPD.DEVICE_SYNC_PARTNER, out string val);
			return val;
		}
	}

	/// <summary>
	/// Firmware version of the portable device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string? FirmwareVersion
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryGetStringValue(WPD.DEVICE_FIRMWARE_VERSION, out string val);
			return val;
		}
	}

	/// <summary>
	/// Battery level of the portable device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public int? PowerLevel
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryGetSignedIntegerValue(WPD.DEVICE_POWER_LEVEL, out int val);
			return val;
		}
	}

	/// <summary>
	/// Power source of the device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public PowerSource PowerSource
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			if (deviceValues.TryGetSignedIntegerValue(WPD.DEVICE_POWER_SOURCE, out int val))
			{
				return (PowerSource)val;
			}

			return PowerSource.Unknown;
		}
	}

	/// <summary>
	/// Protocol of the device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string Protocol
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetStringValue(WPD.DEVICE_PROTOCOL, out string val);
			return val;
		}
	}

	/// <summary>
	/// Model of the portable device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string Model
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetStringValue(WPD.DEVICE_MODEL, out string val);
			return val;
		}
	}

	/// <summary>
	/// Serial number of the portable device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string SerialNumber
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetStringValue(WPD.DEVICE_SERIAL_NUMBER, out string val);
			return val;
		}
	}

	/// <summary>
	/// Supports non consumable.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public bool? SupportsNonConsumable
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			if (deviceValues.TryGetBoolValue(WPD.DEVICE_SUPPORTS_NON_CONSUMABLE, out bool val))
			{
				return val;
			}

			return null;
		}
	}

	/// <summary>
	/// Date and time of the media device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public DateTime? DateTime
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryGetDateTimeValue(WPD.DEVICE_DATETIME, out DateTime? val);
			return val;
		}
	}

	/// <summary>
	/// Supported formats are ordered.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public bool? SupportedFormatsAreOrdered
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			if (deviceValues.TryGetBoolValue(WPD.DEVICE_SUPPORTED_FORMATS_ARE_ORDERED, out bool val))
			{
				return val;
			}

			return null;
		}
	}

	/// <summary>
	/// Device type of the portable device.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public DeviceType? DeviceType
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryGetSignedIntegerValue(WPD.DEVICE_TYPE, out int val);
			return (DeviceType)val;
		}
	}

	/// <summary>
	/// Network Identifier
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public ulong NetworkIdentifier
	{
		get
		{

			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetUnsignedLargeIntegerValue(WPD.DEVICE_NETWORK_IDENTIFIER, out ulong val);
			return val;
		}
	}

	/// <summary>
	/// Functional unique id od the media device
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public Collection<byte>? FunctionalUniqueId
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryByteArrayValue(WPD.DEVICE_FUNCTIONAL_UNIQUE_ID, out byte[]? value);
			return value != null
				? new Collection<byte>(value)
				: null;
		}
	}

	/// <summary>
	/// Model unique id od the media device
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public Collection<byte>? ModelUniqueId
	{
		get
		{
			if (!IsConnected)
			{
				return null;
			}

			_ = deviceValues.TryByteArrayValue(WPD.DEVICE_MODEL_UNIQUE_ID, out byte[]? value);
			return value != null
				? new Collection<byte>(value)
				: null;
		}
	}

	/// <summary>
	/// Device transport.
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public DeviceTransport Transport
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetSignedIntegerValue(WPD.DEVICE_TRANSPORT, out int val);
			return (DeviceTransport)val;
		}
	}

	/// <summary>
	/// Use device stage
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public DeviceTransport UseDeviceStage
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			_ = deviceValues.TryGetUnsignedIntegerValue(WPD.DEVICE_USE_DEVICE_STAGE, out uint val);
			return (DeviceTransport)val;
		}
	}

	/// <summary>
	/// PnP device ID
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string PnPDeviceID
	{
		get
		{
			if (!IsConnected)
			{
				throw new NotConnectedException("Not connected");
			}

			device.GetPnPDeviceID(out string pnPDeviceID);
			return pnPDeviceID;
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Connect to the portable device.
	/// </summary>
	/// <param name="access">Specifies the desired access the client is requesting to this device.</param>
	/// <param name="share">Specifies the share mode the client is requesting to this device.</param>
	/// <param name="enableCache">Enable or disable file list cache. Disabled cache is used by Explorer for a better performance.</param>
	public void Connect(MediaDeviceAccess access = MediaDeviceAccess.Default, MediaDeviceShare share = MediaDeviceShare.Default, bool enableCache = false)
	{
		if (IsConnected)
		{
			return;
		}

		// find the app name for client name
		string appName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? "MediaDevices";

		// set open device parameters
		IPortableDeviceValues clientInfo = (IPortableDeviceValues)new PortableDeviceValues();
		clientInfo.SetStringValue(ref WPD.CLIENT_NAME, appName);

		clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_MAJOR_VERSION, 1);
		clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_MINOR_VERSION, 0);
		clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_REVISION, 0);
		// Some device drivers need to impersonate the caller in order to function correctly. Since our application does not
		// need to restrict its identity, specify SECURITY_IMPERSONATION so that we work with all devices.
		clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_SECURITY_QUALITY_OF_SERVICE, (uint)Security.IMPERSONATION);

		if (access != MediaDeviceAccess.Default)
		{
			clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_DESIRED_ACCESS, (uint)access);
		}

		if (share != MediaDeviceShare.Default)
		{
			clientInfo.SetUnsignedIntegerValue(ref WPD.CLIENT_SHARE_MODE, (uint)share);
		}

		if (!enableCache)
		{
			// disable file list cache
			clientInfo.SetGuidValue(ref WPD.CLIENT_EVENT_COOKIE, ref WPD.CLSID_PORTABLE_DEVICES);

			//clientInfo.SetStringValue(ref WPD.CLIENT_EVENT_COOKIE, "{35786D3C-B075-49B9-88DD-029876E11C01}");

		}

		// open device
		device.Open(DeviceId, clientInfo);
		device.Capabilities(out deviceCapabilities);
		device.Content(out deviceContent);
		deviceContent.Properties(out deviceProperties);
		deviceProperties.GetValues(Item.RootId, null, out deviceValues);

		ComTrace.WriteObject(deviceValues);

		// advice event handler
		eventCallback = new EventCallback(this);
		device.Advise(0, eventCallback, null, out eventCookie);

		IsConnected = true;
	}

	/// <summary>
	/// Disconnect from the portable device.
	/// </summary>
	public void Disconnect()
	{
		if (!IsConnected)
		{
			return;
		}

		if (!string.IsNullOrEmpty(eventCookie))
		{
			device.Unadvise(eventCookie!);
			eventCookie = null;
		}

		device.Close();
		IsConnected = false;
	}

	/// <summary>
	/// The Cancel method cancels a pending operation on this device. 
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void Cancel()
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		device.Cancel();
	}

	/// <summary>
	/// Returns an enumerable collection of directory names in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An enumerable collection of directory names in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateDirectories(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		return item.GetChildren().Where(i => i.Type != ItemType.File).Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an enumerable collection of directory information that matches a specified search pattern and search subdirectory option. 
	/// </summary>
	/// <param name="path">The directory to search in.</param>
	/// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
	/// <returns>An enumerable collection of directories that matches searchPattern and searchOption.</returns>
	/// <remarks>searchPattern can be a combination of literal and wildcard characters, but doesn't support regular expressions.</remarks>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		return item.GetChildren(FilterToRegex(searchPattern), searchOption).Where(i => i.Type != ItemType.File).Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an enumerable collection of file names in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An enumerable collection of file names in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateFiles(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		return item.GetChildren().Where(i => i.Type == ItemType.File).Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an enumerable collection of file names that match a search pattern in a specified path, and optionally searches subdirectories.
	/// </summary>
	/// <param name="path">The absolute path to the directory to search. This string is case-sensitive.</param>
	/// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
	/// <returns>An enumerable collection of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern and option.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		string? pattern = FilterToRegex(searchPattern);
		return item.GetChildren(pattern, searchOption).Where(i => i.Type == ItemType.File).Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an enumerable collection of file-system entries in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An enumerable collection of file-system entries in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateFileSystemEntries(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		return item.GetChildren().Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an enumerable collection of file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.
	/// </summary>
	/// <param name="path">The absolute path to the directory to search. This string is case-sensitive.</param>
	/// <param name="searchPattern">The search string to match against file-system entries in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
	/// <returns>An enumerable collection of file-system entries in the directory specified by path and that match the specified search pattern and option.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		return item.GetChildren(FilterToRegex(searchPattern), searchOption).Select(i => i.FullName!);
	}

	/// <summary>
	/// Returns an array of directory names in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An array of directory names in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetDirectories(string path) => EnumerateDirectories(path).ToArray();

	/// <summary>
	/// Returns an array of directory information that matches a specified search pattern and search subdirectory option. 
	/// </summary>
	/// <param name="path">The directory to search in.</param>
	/// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see Remarks), but doesn't support regular expressions. The default pattern is "*", which returns all files.</param>
	/// <param name="searchOption">One of the values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is TopDirectoryOnly.</param>
	/// <returns>An array of directories that matches searchPattern and searchOption.</returns>
	/// <remarks>searchPattern can be a combination of literal and wildcard characters, but doesn't support regular expressions.</remarks>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => EnumerateDirectories(path, searchPattern, searchOption).ToArray();

	/// <summary>
	/// Returns an array of file names in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An array of file names in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetFiles(string path) => EnumerateFiles(path).ToArray();

	/// <summary>
	/// Returns an array of file names that match a search pattern in a specified path, and optionally searches subdirectories.
	/// </summary>
	/// <param name="path">The absolute path to the directory to search. This string is case-sensitive.</param>
	/// <param name="searchPattern">The search string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
	/// <returns>An array of the full names (including paths) for the files in the directory specified by path and that match the specified search pattern and option.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => EnumerateFiles(path, searchPattern, searchOption).ToArray();

	/// <summary>
	/// Returns an array of file-system entries in a specified path.
	/// </summary>
	/// <param name="path">The directory to search.</param>
	/// <returns>An array of file-system entries in the directory specified by path.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetFileSystemEntries(string path) => EnumerateFileSystemEntries(path).ToArray();

	/// <summary>
	/// Returns an array of file names and directory names that match a search pattern in a specified path, and optionally searches subdirectories.
	/// </summary>
	/// <param name="path">The absolute path to the directory to search. This string is case-sensitive.</param>
	/// <param name="searchPattern">The search string to match against file-system entries in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
	/// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or should include all subdirectories.</param>
	/// <returns>An array of file-system entries in the directory specified by path and that match the specified search pattern and option.</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly) => EnumerateFileSystemEntries(path, searchPattern, searchOption).ToArray();

	/// <summary>
	/// Creates all directories and subdirectories in the specified path.
	/// </summary>
	/// <param name="path">The directory path to create.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void CreateDirectory(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		_ = Item.GetRoot(this).CreateSubdirectory(path);
	}

	/// <summary>
	/// Deletes the specified directory and, if indicated, any subdirectories and files in the directory.
	/// </summary>
	/// <param name="path">The name of the directory to remove.</param>
	/// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DeleteDirectory(string path, bool recursive = false)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"Director {path} not found.");
		item.Delete(recursive);
	}

	/// <summary>
	/// Determines whether the given path refers to an existing directory on disk.
	/// </summary>
	/// <param name="path">The path to test.</param>
	/// <returns>true if path refers to an existing directory; otherwise, false.</returns>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public bool DirectoryExists(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException(nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return Item.FindFolder(this, path) != null;
	}

	/// <summary>
	/// Download data from a file on a portable device to a stream.
	/// </summary>
	/// <param name="path">The path to the file.</param>
	/// <param name="stream">The stream to download to.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DownloadFile(string path, Stream stream)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(stream);
#else
		if (stream == null)
		{
			throw new ArgumentNullException(nameof(stream));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFile(this, path) ?? throw new FileNotFoundException($"File {path} not found.");
		using Stream sourceStream = item.OpenRead();
		sourceStream.CopyTo(stream);
	}

	/// <summary>
	/// Download icon from a file on a portable device to a stream.
	/// </summary>
	/// <param name="path">The path to the file.</param>
	/// <param name="stream">The stream to download to.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DownloadIcon(string path, Stream stream)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(stream);
#else
		if (stream == null)
		{
			throw new ArgumentNullException(nameof(stream));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFile(this, path) ?? throw new FileNotFoundException($"File {path} not found.");
		using Stream sourceStream = item.OpenReadIcon();
		sourceStream.CopyTo(stream);
	}

	/// <summary>
	/// Download thumbnail from a file on a portable device to a stream.
	/// </summary>
	/// <param name="path">The path to the file.</param>
	/// <param name="stream">The stream to download to.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DownloadThumbnail(string path, Stream stream)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(stream);
#else
		if (stream == null)
		{
			throw new ArgumentNullException(nameof(stream));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFile(this, path) ?? throw new FileNotFoundException($"File {path} not found.");

#if NET6_0_OR_GREATER
#else
#endif

		using Stream sourceStream = item.OpenReadThumbnail();
		sourceStream.CopyTo(stream);
	}
	/// <summary>
	/// Upload data from a stream to a file on a portable device.
	/// </summary>
	/// <param name="stream">The stream to upload from.</param>
	/// <param name="path">The path to the file.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void UploadFile(Stream stream, string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(stream);
#else
		if (stream == null)
		{
			throw new ArgumentNullException(nameof(stream));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		string? folder = Path.GetDirectoryName(path);
		string? fileName = Path.GetFileName(path);

		if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
		{
			throw new DirectoryNotFoundException($"Directory {folder} not found.");
		}

		Item item = Item.FindFolder(this, folder) ?? throw new DirectoryNotFoundException($"Directory {folder} not found.");

		if (item.GetChildren().Any(i => EqualsName(i.Name, fileName)))
		{
			throw new IOException($"File {path} already exists");
		}

		item.UploadFile(fileName, stream);
	}

	/// <summary>
	/// Determines whether the specified file exists.
	/// </summary>
	/// <param name="path">The file to check.</param>
	/// <returns>true if the  path contains the name of an existing file; otherwise, false.</returns>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public bool FileExists(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item? objectId = Item.FindFile(this, path);
		return objectId != null;
	}

	/// <summary>
	/// Deletes the specified file.
	/// </summary>
	/// <param name="path">The name of the file to be deleted. Wildcard characters are not supported.</param>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DeleteFile(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFile(this, path) ?? throw new FileNotFoundException($"File {path} not found.");

#if NET6_0_OR_GREATER
#else
#endif

		item.Delete();
	}

	/// <summary>
	/// Rename a file or folder.
	/// </summary>
	/// <param name="path">Path to the file or folder to rename.</param>
	/// <param name="newName">New name of the file or folder.</param>
	public void Rename(string path, string newName)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(newName))
		{
			throw new ArgumentNullException(nameof(newName));
		}

		Item item = Item.FindItem(this, path) ?? throw new FileNotFoundException($"Path {path} not found.", path);

#if NET6_0_OR_GREATER
#else
#endif

		item.Rename(newName);
	}

	/// <summary>
	/// Gets a new instance of the MediaFileInfo class, which acts as a wrapper for a file path.
	/// </summary>
	/// <param name="path">The fully qualified name of the file, directory or object.</param>
	/// <returns>New instance of the MediaFileInfo class</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public MediaFileInfo GetFileInfo(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindItem(this, path) ?? throw new FileNotFoundException($"{path} not found.", path);

#if NET6_0_OR_GREATER
#else
#endif

		return new MediaFileInfo(this, item);
	}

	/// <summary>
	/// Gets a new instance of the MediaDirectoryInfo class, which acts as a wrapper for a directory path.
	/// </summary>
	/// <param name="path">The fully qualified name of the directory or object.</param>
	/// <returns>New instance of the MediaDirectoryInfo class</returns>
	/// <exception cref="IOException">path is a file name.</exception>
	/// <exception cref="ArgumentException">path is a zero-length string, contains only white space, or contains invalid characters as defined by System.IO.Path.GetInvalidPathChars.</exception>
	/// <exception cref="ArgumentNullException">path is null.</exception>
	/// <exception cref="DirectoryNotFoundException">path is invalid.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public MediaDirectoryInfo GetDirectoryInfo(string path)
	{
		if (!IsPath(path))
		{
			throw new ArgumentException("Invalid path", nameof(path));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.FindFolder(this, path) ?? throw new DirectoryNotFoundException($"{path} not found.");

#if NET6_0_OR_GREATER
#else
#endif

		return new MediaDirectoryInfo(this, item);
	}

	/// <summary>
	/// Get all drives of the device.
	/// </summary>
	/// <returns>Array with all drives of the device.</returns>
	public MediaDriveInfo[]? GetDrives() => FunctionalObjects(FunctionalCategory.Storage)?.Select(o => new MediaDriveInfo(this, o)).ToArray();

	/// <summary>
	/// Gets a new instance of the root MediaDirectoryInfo class, which acts as a wrapper for the root directory path.
	/// </summary>
	/// <returns>New instance of the root MediaDirectoryInfo class</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public MediaDirectoryInfo GetRootDirectory()
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		return new MediaDirectoryInfo(this, Item.GetRoot(this));
	}

	/// <summary>
	/// Download data from a file on a portable device to a stream identified by a Persistent Unique Id.
	/// </summary>
	/// <param name="persistentUniqueId">Persistent Unique Id of the file.</param>
	/// <param name="stream">The stream to download to.</param>
	/// <exception cref="ArgumentNullException">persistentUniqueId is null or empty.</exception>
	/// <exception cref="ArgumentNullException">stream is null.</exception>
	/// <exception cref="FileNotFoundException">persistentUniqueId not found.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public void DownloadFileFromPersistentUniqueId(string persistentUniqueId, Stream stream)
	{
		if (string.IsNullOrEmpty(persistentUniqueId))
		{
			throw new ArgumentNullException(nameof(persistentUniqueId));
		}

#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(stream);
#else
		if (stream == null)
		{
			throw new ArgumentNullException(nameof(stream));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		using Stream sourceStream = OpenReadFromPersistentUniqueId(persistentUniqueId);
		sourceStream.CopyTo(stream);
	}

	/// <summary>
	/// Opens a files stream from an Persistent Unique ID to read from.
	/// </summary>
	/// <param name="persistentUniqueId">Persistent unique ID of the item.</param>
	/// <returns>A new read-only FileStream object.</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">persistentUniqueId is null or empty.</exception>
	/// <exception cref="FileNotFoundException">persistentUniqueId not found.</exception>
	public Stream OpenReadFromPersistentUniqueId(string persistentUniqueId)
	{
		if (string.IsNullOrEmpty(persistentUniqueId))
		{
			throw new ArgumentNullException(nameof(persistentUniqueId));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item? item = Item.GetFromPersistentUniqueId(this, persistentUniqueId);
		if (item == null || !item.IsFile)
		{
			throw new FileNotFoundException($"{persistentUniqueId} not found.");
		}

		return item.OpenRead();
	}

	/// <summary>
	/// Opens a stream reader with UTF-8 encoding from an Persistent Unique ID to read from.
	/// </summary>
	/// <param name="persistentUniqueId">Persistent unique ID of the item.</param>
	/// <returns>A new StreamReader object.</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">persistentUniqueId is null or empty.</exception>
	/// <exception cref="FileNotFoundException">persistentUniqueId not found.</exception>
	public StreamReader OpenTextFromPersistentUniqueId(string persistentUniqueId)
	{
		if (string.IsNullOrEmpty(persistentUniqueId))
		{
			throw new ArgumentNullException(nameof(persistentUniqueId));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item? item = Item.GetFromPersistentUniqueId(this, persistentUniqueId);
		if (item == null || !item.IsFile)
		{
			throw new FileNotFoundException($"{persistentUniqueId} not found.");
		}

		return new StreamReader(item.OpenRead());
	}

	/// <summary>
	/// Create a <see cref="MediaFileSystemInfo"/> instance from the Persistent Unique Id.
	/// </summary>
	/// <param name="persistentUniqueId">Persistent Unique Id of the file or folder.</param>
	/// <returns>New instance of the <see cref="MediaFileInfo"/> or <see cref="MediaDirectoryInfo"/> class.</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">persistentUniqueId is null or empty.</exception>
	/// <exception cref="FileNotFoundException">persistentUniqueId not found.</exception>
	public MediaFileSystemInfo GetFileSystemInfoFromPersistentUniqueId(string persistentUniqueId)
	{
		if (string.IsNullOrEmpty(persistentUniqueId))
		{
			throw new ArgumentNullException(nameof(persistentUniqueId));
		}

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Item item = Item.GetFromPersistentUniqueId(this, persistentUniqueId) ?? throw new FileNotFoundException($"{persistentUniqueId} not found.");
		if (item.IsFile)
		{
			return new MediaFileInfo(this, item);
		}

		return new MediaDirectoryInfo(this, item);
	}

	#endregion

	#region Device Capabilities

	/// <summary>
	/// Retrieves all commands supported by the device.
	/// </summary>
	/// <returns>List with supported commands</returns>
	public IEnumerable<Commands>? SupportedCommands()
	{
		if (!IsConnected)
		{
			return null;
		}

		try
		{
			deviceCapabilities.GetSupportedCommands(out IPortableDeviceKeyCollection commands);
			return commands.ToEnum<Commands>();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	/// <summary>
	/// Retrieves all functional categories by the device.
	/// </summary>
	/// <returns>List with functional categories</returns>
	public IEnumerable<FunctionalCategory>? FunctionalCategories()
	{
		if (!IsConnected)
		{
			return null;
		}

		try
		{
			deviceCapabilities.GetFunctionalCategories(out IPortableDevicePropVariantCollection categories);
			return categories.ToEnum<FunctionalCategory>();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	/// <summary>
	/// Retrieves all functional objects of a functional category by the device.
	/// </summary>
	/// <param name="functionalCategory">Select functional category</param>
	/// <returns>List with functional objects</returns>
	public IEnumerable<string>? FunctionalObjects(FunctionalCategory functionalCategory)
	{
		if (!IsConnected)
		{
			return null;
		}

		try
		{
			Guid? g = functionalCategory.Guid();
			Guid? guidNull = functionalCategory.Guid();
			if (guidNull == null)
			{
				return null;
			}

			Guid guid = guidNull.Value;
			deviceCapabilities.GetFunctionalObjects(ref guid, out IPortableDevicePropVariantCollection objects);
			ComTrace.WriteObject(objects);
			return objects.ToStrings();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	/// <summary>
	/// Get supported content types
	/// </summary>
	/// <param name="functionalCategory">Select functional category</param>
	/// <returns>List with supported content types</returns>
	public IEnumerable<ContentType>? SupportedContentTypes(FunctionalCategory functionalCategory)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		try
		{
			Guid? guidNull = functionalCategory.Guid();
			if (guidNull == null)
			{
				return null;
			}

			Guid guid = guidNull.Value;
			deviceCapabilities.GetSupportedContentTypes(ref guid, out IPortableDevicePropVariantCollection types);
			return types.ToEnum<ContentType>();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	/// <summary>
	/// Retrieves all events supported by the device.
	/// </summary>
	/// <returns>List with supported events</returns>
	public IEnumerable<Events>? SupportedEvents()
	{
		if (!IsConnected)
		{
			return null;
		}

		try
		{
			deviceCapabilities.GetSupportedEvents(out IPortableDevicePropVariantCollection events);
			return events.ToEnum<Events>();
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	#endregion

	#region Commands 

	/// <summary>
	/// Reset device
	/// </summary>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="NotSupportedException">not supported by device.</exception>
	public void ResetDevice()
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		_ = Command.Create(WPD.COMMAND_COMMON_RESET_DEVICE).Send(device);
	}

	/// <summary>
	/// Get content locations
	/// </summary>
	/// <param name="contentType">Content type to find the locations for.</param>
	/// <returns>List with the location paths.</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<string>? GetContentLocations(ContentType contentType)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		try
		{
			Command cmd = Command.Create(WPD.COMMAND_DEVICE_HINTS_GET_CONTENT_LOCATION);
			Guid? guid = contentType.Guid();
			if (guid == null)
			{
				return null;
			}

			cmd.Add(WPD.PROPERTY_DEVICE_HINTS_CONTENT_TYPE, guid.Value);
			if (!cmd.Send(device))
			{
				cmd.WriteResults();
				return [];
			}

			return cmd.GetPropVariants(WPD.PROPERTY_DEVICE_HINTS_CONTENT_LOCATIONS).Select(c => Item.Create(this, c).FullName!);
		}
		catch (COMException ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return null;
	}

	//public void Supported(string id)
	//{
	//    var values = (IPortableDeviceValues)new PortableDeviceValues();
	//    IPortableDeviceValues result;
	//    values.SetGuidValue(WPD_PROPERTY_COMMON_COMMAND_CATEGORY, WPD_COMMAND_OBJECT_PROPERTIES_GET_SUPPORTED.fmtid);
	//    values.SetUnsignedIntegerValue(WPD_PROPERTY_COMMON_COMMAND_ID, WPD_COMMAND_OBJECT_PROPERTIES_GET_SUPPORTED.pid);

	//    values.SetStringValue(WPD_PROPERTY_OBJECT_PROPERTIES_OBJECT_ID, id);
	//    this.device.SendCommand(0, values, out result);

	//    object keys = null;
	//    result.GetIUnknownValue(WPD_PROPERTY_OBJECT_PROPERTIES_PROPERTY_KEYS, out keys);
	//    CommandCheckResult(result);
	//}

	/// <summary>
	/// Eject storage
	/// </summary>
	/// <param name="path">Path of storage to eject.</param>
	/// <returns>true is success and false if not supported.</returns>
	public bool EjectStorage(string path)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException(nameof(path));
		}

		Item? item = Item.FindFolder(this, path);
		if (item == null || item.Id == null)
		{
			throw new DirectoryNotFoundException($"Directory {path} not found.");
		}

		return InternalEject(item.Id);
	}

	internal bool InternalEject(string id)
	{
		Command cmd = Command.Create(WPD.COMMAND_STORAGE_EJECT);
		cmd.Add(WPD.PROPERTY_STORAGE_OBJECT_ID, id);
		return cmd.Send(device);
	}

	/// <summary>
	/// Format storage
	/// </summary>
	/// <param name="path">Path of storage to format.</param>
	public void FormatStorage(string path)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(path))
		{
			throw new ArgumentNullException(nameof(path));
		}

		Item? item = Item.FindFolder(this, path);
		if (item == null || item.Id == null)
		{
			throw new DirectoryNotFoundException($"Directory {path} not found.");
		}

		Format(item.Id);
		//Command cmd = Command.Create(WPD.COMMAND_STORAGE_FORMAT);
		//cmd.Add(WPD.PROPERTY_STORAGE_OBJECT_ID, item.Id);
		//cmd.Send(this.device);
	}

	internal void Format(string id)
	{
		Command cmd = Command.Create(WPD.COMMAND_STORAGE_FORMAT);
		cmd.Add(WPD.PROPERTY_STORAGE_OBJECT_ID, id);
		_ = cmd.Send(device);
	}

	/// <summary>
	/// Send a text SMS
	/// </summary>
	/// <param name="functionalObject">Functional object of the SMS</param>
	/// <param name="recipient">Recipient of the SMS</param>
	/// <param name="text">Text of the SMS</param>
	/// <returns>true is success; false if not</returns>
	/// <example>
	/// <code>
	/// var devices = MediaDevice.GetDevices();
	/// using (var device = devices.First(d => d.FriendlyName == "My Cell Phone"))
	/// {
	///     device.Connect();
	///     if (device.FunctionalCategories().Any(c => c == FunctionalCategory.SMS))
	///     {
	///         // get list of available SIM cards
	///         var objects = device.FunctionalObjects(FunctionalCategory.SMS);
	///         device.SendTextSMS(objects.First());
	///     }
	///     device.Disconnect();
	/// }
	/// </code>
	/// </example>
	public bool SendTextSMS(string functionalObject, string recipient, string text)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(functionalObject))
		{
			throw new ArgumentNullException(nameof(functionalObject));
		}

		Command cmd = Command.Create(WPD.COMMAND_SMS_SEND);
		cmd.Add(WPD.PROPERTY_COMMON_COMMAND_TARGET, functionalObject);
		cmd.Add(WPD.PROPERTY_SMS_RECIPIENT, recipient);
		cmd.Add(WPD.PROPERTY_SMS_MESSAGE_TYPE, (uint)SmsMessageType.Text);
		cmd.Add(WPD.PROPERTY_SMS_TEXT_MESSAGE, text);
		return cmd.Send(device);
	}

	/// <summary>
	/// Initiate a still image capturing
	/// </summary>
	/// <param name="functionalObject">Functional object of the camera</param>
	/// <returns>true is success; false if not</returns>
	/// <exception cref="ArgumentNullException">path is null or empty.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <example>
	/// <code>
	/// var devices = MediaDevice.GetDevices();
	/// using (var device = devices.First(d => d.FriendlyName == "My Cell Phone"))
	/// {
	///     device.Connect();
	///     if (device.FunctionalCategories().Any(c => c == FunctionalCategory.StillImageCapture))
	///     {
	///         // get list of available cameras (front, rear)
	///         var objects = device.FunctionalObjects(FunctionalCategory.StillImageCapture);
	///         device.StillImageCaptureInitiate(objects.First());
	///         // ObjectAdded event call after image create
	///     }
	///     device.Disconnect();
	/// }
	/// </code>
	/// </example>
	public bool StillImageCaptureInitiate(string functionalObject)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(functionalObject))
		{
			throw new ArgumentNullException(nameof(functionalObject));
		}

		Command cmd = Command.Create(WPD.COMMAND_STILL_IMAGE_CAPTURE_INITIATE);
		cmd.Add(WPD.PROPERTY_COMMON_COMMAND_TARGET, functionalObject);
		return cmd.Send(device);
	}

	internal void CallEvent(IPortableDeviceValues eventParameters)
	{
		//ComTrace.WriteObject(eventParameters);
		eventParameters.GetGuidValue(ref WPD.EVENT_PARAMETER_EVENT_ID, out Guid eventGuid);
		Events? eventEnumNull = eventGuid.GetEnumFromAttrGuid<Events>();

		if (eventEnumNull == null)
		{
			return;
		}

		Events eventEnum = eventEnumNull.Value;

		switch (eventEnum)
		{
			case Events.ObjectAdded:
				ObjectAdded?.Invoke(this, new ObjectAddedEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.ObjectRemoved:
				ObjectRemoved?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.ObjectUpdated:
				ObjectUpdated?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.DeviceReset:
				DeviceReset?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.DeviceCapabilitiesUpdated:
				DeviceCapabilitiesUpdated?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.StorageFormat:
				StorageFormat?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.ObjectTransferRequest:
				ObjectTransferRequest?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.DeviceRemoved:
				DeviceRemoved?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			case Events.ServiceMethodComplete:
				ServiceMethodComplete?.Invoke(this, new MediaDeviceEventArgs(eventEnum, this, eventParameters));
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Get storage informations
	/// </summary>
	/// <param name="storageObjectId">ID of the storage object</param>
	/// <returns>MediaStorageInfo class with storage informations</returns>
	/// <exception cref="ArgumentNullException">path is null or empty.</exception>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <example>
	/// <code>
	/// var devices = MediaDevice.GetDevices();
	/// using (var device = devices.First(d => d.FriendlyName == "My Cell Phone"))
	/// {
	///     device.Connect();
	///     
	///     // get list of available storages (SD-Card, Internal Flash, ...)
	///     var objects = device.FunctionalObjects(FunctionalCategory.Storage);
	///     MediaStorageInfo info = GetStorageInfo(objects.First());
	///     ulong size = info.FreeSpaceInBytes;
	///     
	///     device.Disconnect();
	/// }
	/// </code>
	/// </example>
	public MediaStorageInfo? GetStorageInfo(string storageObjectId)
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		if (string.IsNullOrEmpty(storageObjectId))
		{
			throw new ArgumentNullException(nameof(storageObjectId));
		}

		IPortableDeviceKeyCollection keys = (IPortableDeviceKeyCollection)new PortableDeviceKeyCollection();
		keys.Add(ref WPD.STORAGE_TYPE);
		keys.Add(ref WPD.STORAGE_FILE_SYSTEM_TYPE);
		keys.Add(ref WPD.STORAGE_CAPACITY);
		keys.Add(ref WPD.STORAGE_FREE_SPACE_IN_BYTES);
		keys.Add(ref WPD.STORAGE_FREE_SPACE_IN_OBJECTS);
		keys.Add(ref WPD.STORAGE_DESCRIPTION);
		keys.Add(ref WPD.STORAGE_SERIAL_NUMBER);
		keys.Add(ref WPD.STORAGE_MAX_OBJECT_SIZE);
		keys.Add(ref WPD.STORAGE_CAPACITY_IN_OBJECTS);
		keys.Add(ref WPD.STORAGE_ACCESS_CAPABILITY);

		try
		{
			MediaStorageInfo info = new();

			deviceProperties.GetSupportedProperties(storageObjectId, out IPortableDeviceKeyCollection ppKeys);
			ComTrace.WriteObject(ppKeys);
			deviceProperties.GetValues(storageObjectId, keys, out IPortableDeviceValues values);

			_ = values.TryGetUnsignedIntegerValue(WPD.STORAGE_TYPE, out uint type);
			info.Type = (StorageType)type;

			_ = values.TryGetStringValue(WPD.STORAGE_FILE_SYSTEM_TYPE, out string fileSystemType);
			info.FileSystemType = fileSystemType;

			_ = values.TryGetUnsignedLargeIntegerValue(WPD.STORAGE_CAPACITY, out ulong capacity);
			info.Capacity = capacity;

			_ = values.TryGetUnsignedLargeIntegerValue(WPD.STORAGE_FREE_SPACE_IN_BYTES, out ulong freeBytes);
			info.FreeSpaceInBytes = freeBytes;

			_ = values.TryGetUnsignedLargeIntegerValue(WPD.STORAGE_FREE_SPACE_IN_OBJECTS, out ulong freeObjects);
			info.FreeSpaceInObjects = freeObjects;

			_ = values.TryGetStringValue(WPD.STORAGE_DESCRIPTION, out string description);
			info.Description = description;

			_ = values.TryGetStringValue(WPD.STORAGE_SERIAL_NUMBER, out string serialNumber);
			info.SerialNumber = serialNumber;

			_ = values.TryGetUnsignedLargeIntegerValue(WPD.STORAGE_MAX_OBJECT_SIZE, out ulong maxObjectSize);
			info.MaxObjectSize = maxObjectSize;

			_ = values.TryGetUnsignedLargeIntegerValue(WPD.STORAGE_CAPACITY_IN_OBJECTS, out ulong capacityInObjects);
			info.CapacityInObjects = capacityInObjects;

			_ = values.TryGetUnsignedIntegerValue(WPD.STORAGE_ACCESS_CAPABILITY, out uint accessCapability);
			info.AccessCapability = (StorageAccessCapability)accessCapability;

			return info;
		}
		catch (FileNotFoundException ex)
		{
			Debug.WriteLine(ex.ToString());
		}

		return null;
	}

	#endregion

	#region MTP_EXT_VENDOR

	/// <summary>
	/// Queries for vendor extended operation code.
	/// </summary>
	/// <returns>List of vendor extended operation code.</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public IEnumerable<int> VendorOpcodes()
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_GET_SUPPORTED_VENDOR_OPCODES);
		_ = cmd.Send(device);
		IEnumerable<PropVariantFacade> list = cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_VENDOR_OPERATION_CODES);
		return list.Select(p => p.ToInt());
	}

	/// <summary>
	/// Execute a vendor command.
	/// </summary>
	/// <param name="opCode">Operational code of the vendor command.</param>
	/// <param name="inputParams">Input parameters.</param>
	/// <param name="respCode">Response code</param>
	/// <returns>Output parameters</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">inputParams is null.</exception>
	public IEnumerable<int> VendorExcecute(int opCode, IEnumerable<int> inputParams, out int respCode)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(inputParams, nameof(inputParams));
#else
		if (inputParams == null)
		{
			throw new ArgumentNullException(nameof(inputParams));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_EXECUTE_COMMAND_WITHOUT_DATA_PHASE);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_CODE, opCode);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_PARAMS, inputParams);
		_ = cmd.Send(device);
		respCode = cmd.GetInt(WPD.PROPERTY_MTP_EXT_RESPONSE_CODE);
		return cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_RESPONSE_PARAMS).Select(p => p.ToInt());
	}

	/// <summary>
	/// Sends a MTP command block followed by a data phase with data from Device to Host.
	/// </summary>
	/// <param name="opCode">Operational code of the vendor command.</param>
	/// <param name="inputParams">Input parameters.</param>
	/// <returns>Returned as a context identifier for subsequent data transfer</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">inputParams is null.</exception>
	public IEnumerable<int> VendorExcecuteRead(int opCode, IEnumerable<int> inputParams)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(inputParams, nameof(inputParams));
#else
		if (inputParams == null)
		{
			throw new ArgumentNullException(nameof(inputParams));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_READ);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_CODE, opCode);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_PARAMS, inputParams);
		_ = cmd.Send(device);
		List<PropVariantFacade> list = cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_VENDOR_OPERATION_CODES).ToList();
		return list.Select(p => p.ToInt()); //.ToList();
	}

	/// <summary>
	/// Sends a MTP command block followed by a data phase with data from Host to Device 
	/// </summary>
	/// <param name="opCode">Operational code of the vendor command.</param>
	/// <param name="inputParams">Input parameters.</param>
	/// <returns>Returned as a context identifier for subsequent data transfer</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	/// <exception cref="ArgumentNullException">inputParams is null.</exception>
	public IEnumerable<int> VendorExcecuteWrite(int opCode, IEnumerable<int> inputParams)
	{
#if NET6_0_OR_GREATER
		ArgumentNullException.ThrowIfNull(inputParams, nameof(inputParams));
#else
		if (inputParams == null)
		{
			throw new ArgumentNullException(nameof(inputParams));
		}
#endif

		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_EXECUTE_COMMAND_WITH_DATA_TO_WRITE);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_CODE, opCode);
		cmd.Add(WPD.PROPERTY_MTP_EXT_OPERATION_PARAMS, inputParams);
		_ = cmd.Send(device);
		List<PropVariantFacade> list = cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_VENDOR_OPERATION_CODES).ToList();
		return list.Select(p => p.ToInt()); //.ToList();
	}

	//public IEnumerable<byte> VendorRead(string context, int bytesToRead, byte[] input, out int bytesRead)
	//{
	//    Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_READ_DATA);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_CONTEXT, opCode);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_READ, inputParams);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_DATA, inputParams);
	//    cmd.Send(this.device);
	//    var list = cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_VENDOR_OPERATION_CODES).ToList();
	//    return list.Select(p => p.ToInt()).ToList();
	//}

	//public int VendorWrite(string context, int bytesToWrite, byte[] buffer )
	//{
	//    Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_WRITE_DATA);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_CONTEXT, context);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_TO_WRITE, bytesToWrite);
	//    cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_DATA, buffer);
	//    cmd.Send(this.device);
	//    return cmd.GetInt(WPD.PROPERTY_MTP_EXT_TRANSFER_NUM_BYTES_WRITTEN);
	//}

	/// <summary>
	/// completes a data transfer and read response from device. The transfer is initiated by VendorExcecuteWrite
	/// </summary>
	/// <param name="context">The context idetifier returned in previous calls.</param>
	/// <param name="respCode">the response code to the vendor operation code.</param>
	/// <returns>identifying response params if any</returns>
	public IEnumerable<int> VendorEndTransfer(string context, out int respCode)
	{
		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_END_DATA_TRANSFER);
		cmd.Add(WPD.PROPERTY_MTP_EXT_TRANSFER_CONTEXT, context);
		_ = cmd.Send(device);
		respCode = cmd.GetInt(WPD.PROPERTY_MTP_EXT_RESPONSE_CODE);
		return cmd.GetPropVariants(WPD.PROPERTY_MTP_EXT_RESPONSE_PARAMS).Select(p => p.ToInt());
	}

	/// <summary>
	/// Retrieves the vendor extension description string.
	/// </summary>
	/// <returns>Vendor extension description string</returns>
	/// <exception cref="NotConnectedException">device is not connected.</exception>
	public string VendorExtentionDescription()
	{
		if (!IsConnected)
		{
			throw new NotConnectedException("Not connected");
		}

		Command cmd = Command.Create(WPD.COMMAND_MTP_EXT_GET_VENDOR_EXTENSION_DESCRIPTION);
		_ = cmd.Send(device);
		string description = cmd.GetString(WPD.PROPERTY_MTP_EXT_VENDOR_EXTENSION_DESCRIPTION);
		return description;
	}

#endregion

	#region Services

	// private static Guid GUID_DEVINTERFACE_WPD_SERVICECATION = new Guid(0x9EF44F80, 0x3D64, 0x4246, 0xA6, 0xAA, 0x20, 0x6F, 0x32, 0x8D, 0x1E, 0xDC);

	/// <summary>
	/// Get device services
	/// </summary>
	/// <param name="service">Service type</param>
	/// <returns>List of services</returns>
	public IEnumerable<MediaDeviceService>? GetServices(MediaDeviceServices service)
	{
		Guid? serviceGuidNull = service.Guid();
		if (serviceGuidNull == null)
		{
			return null;
		}

		Guid serviceGuid = serviceGuidNull.Value;
		uint num = 0;
		serviceManager.GetDeviceServices(DeviceId, ref serviceGuid, null, ref num);

		if (num == 0)
		{
			return null;
		}

		string[] services = new string[num];
		serviceManager.GetDeviceServices(DeviceId, ref serviceGuid, services, ref num);

		//foreach (var ser in services)
		//{
		//    var s = new MediaDeviceStatusService(this, ser);
		//    s.Open();

		//    var x = s.GetContent().ToArray();

		//    s.Close();

		//}

		// not supported by old frameworks
		return service switch
		{
			MediaDeviceServices.Status => services.Select(s => new MediaDeviceStatusService(this, s)),
			MediaDeviceServices.Hints => services.Select(s => new MediaDeviceServiceHints(this, s)),
			MediaDeviceServices.Metadata => services.Select(s => new MediaDeviceServiceMetadata(this, s)),
			_ => services.Select(s => new MediaDeviceService(this, s)),
		};
	}

	#endregion

	#region Internal Methods

	internal static bool IsPath(string path) => !string.IsNullOrWhiteSpace(path) && path.IndexOfAny(Path.GetInvalidPathChars()) < 0;

	internal bool EqualsName(string? a, string? b)
	{
		// If either is null, return false.
		if (a is null || b is null)
		{
			return false;
		}

		return IsCaseSensitive ? a == b : string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
	}

	internal static string? FilterToRegex(string filter)
	{
		if (filter is null or "*" or "*.*")
		{
			return null;
		}

		StringBuilder s = new(filter);
		_ = s.Replace(".", @"\.");
		_ = s.Replace("+", @"\+");
		_ = s.Replace("$", @"\$");
		_ = s.Replace("(", @"\(");
		_ = s.Replace(")", @"\)");
		_ = s.Replace("[", @"\[");
		_ = s.Replace("]", @"\]");
		_ = s.Replace("?", ".?");
		_ = s.Replace("*", ".*");
		return s.ToString();
	}

	#endregion
}