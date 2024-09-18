using MediaDevices.Internal;

using System.Collections.Generic;

namespace MediaDevices;

/// <summary>
/// Content service class
/// </summary>
public class MediaDeviceServiceContent
{
	private readonly MediaDeviceService service;

	private MediaDeviceServiceContent()
	{ }

	internal MediaDeviceServiceContent(MediaDeviceService service, string objectId)
	{
		this.service = service;
		ObjectId = objectId;

		UpdateProperties();
	}

	internal virtual void UpdateProperties()
	{
		if (service.content == null)
		{
			return;
		}

		service.content.Properties(out IPortableDeviceProperties properties);

		//IPortableDeviceKeyCollection keyCol = (IPortableDeviceKeyCollection)new PortableDeviceKeyCollection();
		if (ObjectId == null)
		{
			return;
		}

		properties.GetSupportedProperties(ObjectId, out IPortableDeviceKeyCollection keyCol);

		properties.GetValues(ObjectId, keyCol, out IPortableDeviceValues deviceValues);

		using (PropVariantFacade value = new())
		{
			deviceValues.GetValue(ref WPD.ParentId, out value.Value);
			ParentId = value;
		}

		using (PropVariantFacade value = new())
		{
			deviceValues.GetValue(ref WPD.Name, out value.Value);
			Name = value;
		}

		ComTrace.WriteObject(deviceValues);
	}

	/// <summary>
	/// Object ID of teh content
	/// </summary>
	public string? ObjectId { get; private set; }

	/// <summary>
	/// Parent ID of the content
	/// </summary>
	public string? ParentId { get; private set; }

	/// <summary>
	/// Name of the content
	/// </summary>
	public string? Name { get; private set; }

	/// <summary>
	/// Get the content
	/// </summary>
	/// <returns>Content list</returns>
	public IEnumerable<MediaDeviceServiceContent> GetContent() => ObjectId != null ? service.GetContent(ObjectId) : [];

	/// <summary>
	/// Get all properties of the content
	/// </summary>
	/// <returns>List of properties</returns>
	public IEnumerable<KeyValuePair<string, string>> GetAllProperties() => ObjectId != null ? service.GetAllProperties(ObjectId) : [];
}
