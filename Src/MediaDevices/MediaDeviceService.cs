﻿using MediaDevices.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaDevices;

// C:\Program Files (x86)\Windows Kits\10\Include\10.0.17763.0\um\propkey.h

/// <summary>
/// MediaDevice service class
/// </summary>
public class MediaDeviceService : IDisposable
{
	internal MediaDevice? device;
	internal IPortableDeviceService? privateService = (IPortableDeviceService)new PortableDeviceService();
	//protected IPortableDeviceValues values;
	internal IPortableDeviceServiceCapabilities? capabilities;
	internal IPortableDeviceContent2? content;

	private MediaDeviceService()
	{ }

	internal MediaDeviceService(MediaDevice device, string serviceId)
	{
		this.device = device;
		ServiceId = serviceId;

		//Match match = Regex.Match(serviceId, @".*#(?<service>\{.*\})\\(?<name>\{.*\})");
		//if (match.Success)
		//{
		//    string service = match.Groups["service"].Value;
		//    Guid serviceGuid = new Guid(service);
		//    this.Service = serviceGuid.GetEnum<Services>();
		//    string serviceName = match.Groups["name"].Value;
		//    this.ServiceName = $"{this.Service} : {service} : {serviceName}";
		//}
		//else
		//{
		//    this.ServiceName = "Unknown";
		//}
		//this.ServiceName = serviceId.Substring(serviceId.LastIndexOf(@"\") + 1);

		IPortableDeviceValues values = (IPortableDeviceValues)new PortableDeviceValues();
		privateService.Open(ServiceId, values);

		privateService.GetServiceObjectID(out string serviceObjectID);
		ServiceObjectID = serviceObjectID;

		privateService.GetPnPServiceID(out string pnPServiceID);
		PnPServiceID = pnPServiceID;

		privateService.Capabilities(out capabilities);

		privateService.Content(out content);

		content.Properties(out IPortableDeviceProperties properties);

		properties.GetSupportedProperties(ServiceObjectID, out IPortableDeviceKeyCollection keyCol);

		properties.GetValues(ServiceObjectID, keyCol, out IPortableDeviceValues deviceValues);

		ComTrace.WriteObject(deviceValues);

		using (PropVariantFacade value = new())
		{
			deviceValues.GetValue(ref WPD.OBJECT_NAME, out value.Value);
			Name = value;
		}

		using (PropVariantFacade value = new())
		{
			deviceValues.GetValue(ref WPD.FUNCTIONAL_OBJECT_CATEGORY, out value.Value);

			Guid serviceGuid = new((string)value);
			Service = serviceGuid.GetEnum<MediaDeviceServices>();
			ServiceName = Service != MediaDeviceServices.Unknown ? Service.ToString() : serviceGuid.ToString();
		}

		using (PropVariantFacade value = new())
		{
			deviceValues.GetValue(ref WPD.SERVICE_VERSION, out value.Value);
			ServiceVersion = value;
		}

		Update();

		//var x = GetContent().ToArray();

	}

	/// <summary>
	/// Dispose service
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Dispose service
	/// </summary>
	/// <param name="disposing">Disposing flag</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (privateService != null)
			{
				privateService.Close();
				privateService = null;
			}
		}
	}

	/// <summary>
	/// ID of the service
	/// </summary>
	public string? ServiceId { get; private set; }

	/// <summary>
	/// Get services
	/// </summary>
	public MediaDeviceServices? Service { get; private set; }

	/// <summary>
	/// Name of the service
	/// </summary>
	public string? Name { get; private set; }

	/// <summary>
	/// Servicename
	/// </summary>
	public string? ServiceName { get; private set; }

	/// <summary>
	/// Version of the service
	/// </summary>
	public string? ServiceVersion { get; private set; }

	/// <summary>
	/// OhjectID of the service
	/// </summary>
	public string? ServiceObjectID { get; private set; }

	/// <summary>
	/// PnP service ID
	/// </summary>
	public string? PnPServiceID { get; private set; }

	/// <summary>
	/// Info of the service
	/// </summary>
	/// <returns>String with the info</returns>
	public override string ToString() => $"{Name} : {ServiceName} : {ServiceVersion}";

	/// <summary>
	/// Get content of the service
	/// </summary>
	/// <returns>List of content services</returns>
	public IEnumerable<MediaDeviceServiceContent> GetContent() => GetContent("DEVICE");

	internal IEnumerable<MediaDeviceServiceContent> GetContent(string objectID)
	{
		if (content == null)
			return [];

		content.EnumObjects(0, objectID, null, out IEnumPortableDeviceObjectIDs enumerator);

		uint num = 0;
		string[] objectIdArray = new string[20];
		enumerator.Next(20, objectIdArray, ref num);

		return objectIdArray.Take((int)num).Select(o => new MediaDeviceServiceContent(this, o));
	}

	internal IEnumerable<KeyValuePair<string, string>> GetAllProperties(string objectID)
	{
		if (content == null)
			return [];

		content.Properties(out IPortableDeviceProperties properties);

		properties.GetSupportedProperties(objectID, out IPortableDeviceKeyCollection keyCol);

		properties.GetValues(objectID, keyCol, out IPortableDeviceValues deviceValues);

		return deviceValues.ToKeyValuePair();
	}

	internal IPortableDeviceValues? GetProperties(IPortableDeviceKeyCollection keyCol)
	{
		if (content == null || ServiceObjectID == null)
			return null;

		content.Properties(out IPortableDeviceProperties properties);

		properties.GetValues(ServiceObjectID, keyCol, out IPortableDeviceValues deviceValues);

		return deviceValues;
	}

	/// <summary>
	/// Update service
	/// </summary>
	protected virtual void Update()
	{
		if (content == null || ServiceObjectID == null)
			return;

		content.Properties(out IPortableDeviceProperties properties);

		properties.GetSupportedProperties(ServiceObjectID, out IPortableDeviceKeyCollection keyCol);

		properties.GetValues(ServiceObjectID, keyCol, out IPortableDeviceValues deviceValues);

		ComTrace.WriteObject(deviceValues);
	}

	/// <summary>
	/// Get all properties
	/// </summary>
	/// <returns>List of properties</returns>
	public IEnumerable<KeyValuePair<string, string>> GetAllProperties() => ServiceObjectID != null ? GetAllProperties(ServiceObjectID) : [];

	/// <summary>
	/// Get supported methods
	/// </summary>
	/// <returns>List of supported methods</returns>
	public IEnumerable<Methods> GetSupportedMethods()
	{
		if (capabilities == null)
			return [];

		capabilities.GetSupportedMethods(out IPortableDevicePropVariantCollection methods);
		ComTrace.WriteObject(methods);
		return methods.ToEnum<Methods>();
	}

	/// <summary>
	/// Get supported commands
	/// </summary>
	/// <returns>List of supported commands</returns>
	public IEnumerable<Commands> GetSupportedCommands()
	{
		if (capabilities == null)
			return [];

		capabilities.GetSupportedCommands(out IPortableDeviceKeyCollection commands);
		ComTrace.WriteObject(commands);
		return commands.ToEnum<Commands>();
	}

	/// <summary>
	/// Get supported events
	/// </summary>
	/// <returns>list of supported events</returns>
	public IEnumerable<Events> GetSupportedEvents()
	{
		if (capabilities == null)
			return [];

		capabilities.GetSupportedEvents(out IPortableDevicePropVariantCollection events);
		ComTrace.WriteObject(events);
		return events.ToEnum<Events>();
	}

	/// <summary>
	/// Get supported formats
	/// </summary>
	/// <returns>List of supported formats</returns>
	public IEnumerable<SupportedFormats> GetSupportedFormats()
	{
		if (capabilities == null)
			return [];

		capabilities.GetSupportedFormats(out IPortableDevicePropVariantCollection formats);
		ComTrace.WriteObject(formats);
		return formats.ToEnum<SupportedFormats>();
	}

	/// <summary>
	/// Call a service method
	/// </summary>
	/// <param name="method">Method GUID</param>
	/// <param name="parameters">Method parameters</param>
	[Obsolete("Use CallMethod(Guid method) instead", false)]
	public void CallMethod(Guid method, object[] parameters)
		=> CallMethod(method);

	/// <summary>
	/// Call a service method
	/// </summary>
	/// <param name="method">Method GUID</param>
	public void CallMethod(Guid method)
	{
#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(privateService == null, privateService);
#else
		if (privateService == null)
		{
			throw new ObjectDisposedException("MediaDeviceService");
		}
#endif

		privateService.Methods(out IPortableDeviceServiceMethods methods);

		IPortableDeviceValues values = (IPortableDeviceValues)new PortableDeviceValues();
		//values.SetStringValue();
		IPortableDeviceValues results = (IPortableDeviceValues)new PortableDeviceValues();
		methods.Invoke(ref method, ref values, ref results);
	}

	internal void SendCommand(PropertyKey commandKey)
	{
		IPortableDeviceValues values = (IPortableDeviceValues)new PortableDeviceValues();
		values.SetGuidValue(ref WPD.PROPERTY_COMMON_COMMAND_CATEGORY, ref commandKey.fmtid);
		values.SetUnsignedIntegerValue(ref WPD.PROPERTY_COMMON_COMMAND_ID, commandKey.pid);
		privateService?.SendCommand(0, ref values, out _);
	}
}
