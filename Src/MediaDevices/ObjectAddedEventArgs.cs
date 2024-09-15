using MediaDevices.Internal;

using System;
using System.IO;

namespace MediaDevices;

/// <summary>
/// Event argument class for the media device object added event.
/// </summary>
public class ObjectAddedEventArgs : MediaDeviceEventArgs
{
	internal ObjectAddedEventArgs(Events eventEnum, MediaDevice mediaDevice, IPortableDeviceValues eventParameters)
		: base(eventEnum, mediaDevice, eventParameters)
	{
		_ = eventParameters.TryGetStringValue(WPD.OBJECT_ID, out string objectId);
		ObjectId = objectId;

		_ = eventParameters.TryGetStringValue(WPD.OBJECT_PERSISTENT_UNIQUE_ID, out string objectPersistentUniqueId);
		ObjectPersistentUniqueId = objectPersistentUniqueId;

		_ = eventParameters.TryGetStringValue(WPD.OBJECT_NAME, out string objectName);
		ObjectName = objectName;

		if (eventParameters.TryGetGuidValue(WPD.OBJECT_CONTENT_TYPE, out Guid objectContentType))
		{
			ContentType? contentType = ComEnumerable.GetEnumFromAttrGuid<ContentType>(objectContentType);
			if (contentType != null)
			{
				ObjectContentType = contentType.Value;
			}
		}

		if (eventParameters.TryGetGuidValue(WPD.FUNCTIONAL_OBJECT_CATEGORY, out Guid functionalObjectCategory))
		{
			FunctionalCategory? functionalCategory = ComEnumerable.GetEnumFromAttrGuid<FunctionalCategory>(functionalObjectCategory);
			if (functionalCategory != null)
			{
				FunctionalObjectCategory = functionalCategory.Value;
			}
		}

		_ = eventParameters.TryGetStringValue(WPD.OBJECT_ORIGINAL_FILE_NAME, out string objectOriginalFileName);
		ObjectOriginalFileName = objectOriginalFileName;

		_ = eventParameters.TryGetStringValue(WPD.OBJECT_PARENT_ID, out string objectParentId);
		ObjectParentId = objectParentId;

		_ = eventParameters.TryGetStringValue(WPD.OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID, out string objectContainerFuntionalObjectId);
		ObjectContainerFuntionalObjectId = objectContainerFuntionalObjectId;
	}

	/// <summary>
	/// Id of the added object.
	/// </summary>
	public string ObjectId { get; private set; }

	/// <summary>
	/// Persistent unique id of the added object.
	/// </summary>
	public string ObjectPersistentUniqueId { get; private set; }

	/// <summary>
	/// Name of the added object.
	/// </summary>
	public string ObjectName { get; private set; }

	/// <summary>
	/// Content type of the added object.
	/// </summary>
	public ContentType ObjectContentType { get; private set; }

	/// <summary>
	/// Functional category of the added object
	/// </summary>
	public FunctionalCategory FunctionalObjectCategory { get; private set; }

	/// <summary>
	/// Original file name of the added object
	/// </summary>
	public string ObjectOriginalFileName { get; private set; }

	/// <summary>
	/// Parent id of the added object.
	/// </summary>
	public string ObjectParentId { get; private set; }

	/// <summary>
	/// Container functional id of the added object. 
	/// </summary>
	public string ObjectContainerFuntionalObjectId { get; private set; }

	/// <summary>
	/// Full file name of the added object
	/// </summary>
	public string ObjectFullFileName => Item.Create(MediaDevice, ObjectId).FullName;

	/// <summary>
	/// Read stream of the added object
	/// </summary>
	public Stream ObjectFileStream => Item.Create(MediaDevice, ObjectId).OpenRead();
}
