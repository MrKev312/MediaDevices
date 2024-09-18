using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaDevices.Internal;

[DebuggerDisplay("{this.Type} - {this.Name} - {this.Id}")]
internal sealed class Item
{
	private static readonly IPortableDeviceKeyCollection keyCollection;

	static Item()
	{
		// key collection with all used properties
		keyCollection = (IPortableDeviceKeyCollection)new PortableDeviceKeyCollection();
		keyCollection.Add(ref WPD.OBJECT_CONTENT_TYPE);
		keyCollection.Add(ref WPD.OBJECT_NAME);
		keyCollection.Add(ref WPD.OBJECT_ORIGINAL_FILE_NAME);

		keyCollection.Add(ref WPD.OBJECT_HINT_LOCATION_DISPLAY_NAME);
		keyCollection.Add(ref WPD.OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID);
		keyCollection.Add(ref WPD.OBJECT_SIZE);
		keyCollection.Add(ref WPD.OBJECT_DATE_CREATED);
		keyCollection.Add(ref WPD.OBJECT_DATE_MODIFIED);
		keyCollection.Add(ref WPD.OBJECT_DATE_AUTHORED);
		keyCollection.Add(ref WPD.OBJECT_CAN_DELETE);
		keyCollection.Add(ref WPD.OBJECT_ISSYSTEM);
		keyCollection.Add(ref WPD.OBJECT_ISHIDDEN);
		keyCollection.Add(ref WPD.OBJECT_IS_DRM_PROTECTED);
		keyCollection.Add(ref WPD.OBJECT_PARENT_ID);
		keyCollection.Add(ref WPD.OBJECT_PERSISTENT_UNIQUE_ID);
	}

	private readonly MediaDevice device;
	private string? name;
	private readonly string? path;
	private Item? parent;

	private const uint PORTABLE_DEVICE_DELETE_NO_RECURSION = 0;
	private const uint PORTABLE_DEVICE_DELETE_WITH_RECURSION = 1;

	internal char DirectorySeparatorChar = '\\';

	private const int numObjectsToRequest = 32;

	public const string RootId = "DEVICE";

	public static Item GetRoot(MediaDevice device) => new(device, RootId, @"\");

	public static Item Create(MediaDevice device, string id, string? path = null) => new(device, id, path);

	public static Item? FindFolder(MediaDevice device, string path)
	{
		Item? item = FindItem(device, path);
		return item == null || item.Type != ItemType.Folder ? null : item;
	}

	public static Item? FindFile(MediaDevice device, string path)
	{
		Item? item = FindItem(device, path);
		return item == null || item.Type != ItemType.File ? null : item;
	}

	public static Item? FindItem(MediaDevice device, string path)
	{
		Item? item = GetRoot(device);
		if (path == @"\")
		{
			return item;
		}

		string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string folder in folders)
		{
			item = item.GetChildren().FirstOrDefault(i => device.EqualsName(i.Name, folder));
			if (item == null)
			{
				return null;
			}
		}

		return item;
	}

	public static Item? GetFromPersistentUniqueId(MediaDevice device, string persistentUniqueId)
	{
		// fill collection with id to request
		IPortableDevicePropVariantCollection collection = (IPortableDevicePropVariantCollection)new PortableDevicePropVariantCollection();

		using (PropVariantFacade propVariantPUID = PropVariantFacade.StringToPropVariant(persistentUniqueId))
		{
			collection.Add(ref propVariantPUID.Value);
		}

		if (device.deviceContent == null)
		{
			return null;
		}

		// request id collection           
		device.deviceContent.GetObjectIDsFromPersistentUniqueIDs(collection, out IPortableDevicePropVariantCollection results);

		//var s = results.ToStrings().ToArray();
		string? mediaObjectId = results.ToStrings().FirstOrDefault();

		// return result item
		return mediaObjectId == null ? null : Create(device, mediaObjectId);
		//return string.IsNullOrEmpty(mediaObjectId) ? null : Item.Create(device, mediaObjectId);
	}

	private Item(MediaDevice device, string id, string? path)
	{
		this.device = device;
		Id = id;
		this.path = path;

		if (id == RootId)
		{
			Name = @"\";
			FullName = @"\";
			Type = ItemType.Object;
		}
		else
		{
			Refresh();

			// find full name if no path
			if (string.IsNullOrEmpty(this.path))
			{
				string p = GetPath();
				this.path = Path.GetDirectoryName(p);
				FullName = p;
			}
		}
	}

	/// <summary>
	/// Special small constructor for GetPath.
	/// </summary>
	/// <param name="device"></param>
	/// <param name="id"></param>
	private Item(MediaDevice device, string id)
	{
		this.device = device;
		Id = id;
		if (id == RootId)
		{
			Name = @"\";
			FullName = @"\";
			Type = ItemType.Object;
		}
		else
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		if (Id != RootId)
		{
			GetProperties();

			Guid contentType = ContentType;
			if (contentType == WPD.CONTENT_TYPE_FUNCTIONAL_OBJECT)
			{
				Name = name ?? @"\";
				Type = ItemType.Object;

			}
			else if (contentType == WPD.CONTENT_TYPE_FOLDER)
			{
				Name = OriginalFileName;
				Type = ItemType.Folder;
			}
			else
			{
				Name = OriginalFileName;
				Type = ItemType.File;
			}

			if (path != null) // TODO check if we can remove empty pathes
			{
				// don't use Path.Combine
				FullName = path.TrimEnd(DirectorySeparatorChar) + DirectorySeparatorChar + Name;
			}
		}
	}

	private void GetProperties()
	{
		IPortableDeviceValues values;
		try
		{
			if (Id == null)
			{
				throw new InvalidOperationException("Id is null");
			}

			// get all predefined values
			device.deviceProperties!.GetValues(Id, keyCollection, out values);
		}
		catch (InvalidOperationException ex)
		{
			Trace.TraceError($"InvalidOperationException: {ex.Message} for {Id}");
			return;
		}
		catch (COMException ex)
		{
			Trace.TraceError($"COMException: {ex.Message} for {Id}");
			return;
		}
		catch (UnauthorizedAccessException ex)
		{
			Trace.TraceError($"UnauthorizedAccessException: {ex.Message} for {Id}");
			return;
		}
		catch (IOException ex)
		{
			Trace.TraceError($"IOException: {ex.Message} for {Id}");
			return;
		}
		catch (Exception ex)
		{
			Trace.TraceError($"Unexpected exception: {ex.Message} for {Id}");
			throw;
		}

		// read all properties
		// use a loop to prevent exceptions during calling GetValue for non existing values 
		uint count = 0;
		values.GetCount(ref count);
		for (uint i = 0; i < count; i++)
		{
			PropertyKey key = new();
			using PropVariantFacade val = new();
			values.GetAt(i, ref key, ref val.Value);

			if (key.fmtid == WPD.OBJECT_PROPERTIES_V1)
			{
				switch ((ObjectProperties)key.pid)
				{
					case ObjectProperties.ContentType:
						ContentType = val;
						break;

					case ObjectProperties.Name:
						name = val;
						break;

					case ObjectProperties.OriginalFileName:
						OriginalFileName = val;
						break;

					case ObjectProperties.HintLocationDisplayName:
						HintLocationName = val;
						break;

					case ObjectProperties.ContainerFunctionalObjectId:
						ParentContainerId = val;
						break;

					case ObjectProperties.Size:
						Size = val;
						break;

					case ObjectProperties.DateCreated:
						DateCreated = val;
						break;

					case ObjectProperties.DateModified:
						DateModified = val;
						break;

					case ObjectProperties.DateAuthored:
						DateAuthored = val;
						break;

					case ObjectProperties.CanDelete:
						CanDelete = val;
						break;

					case ObjectProperties.IsSystem:
						IsSystem = val.ToBool();
						break;

					case ObjectProperties.IsHidden:
						IsHidden = val;
						break;

					case ObjectProperties.IsDrmProtected:
						IsDRMProtected = val;
						break;

					case ObjectProperties.ParentId:
						ParentId = val;
						break;

					case ObjectProperties.PersistentUniqueId:
						PersistentUniqueId = val;
						break;
				}
			}
		}
	}

	#region Value Properties

	public string? Id { get; private set; }
	public string? Name { get; private set; }
	public string? FullName { get; set; }
	public ItemType Type { get; private set; }
	public Guid ContentType { get; private set; }
	public string? OriginalFileName { get; private set; }
	public string? HintLocationName { get; private set; }
	public string? ParentContainerId { get; private set; }
	public ulong Size { get; private set; }
	public DateTime? DateCreated { get; private set; }
	public DateTime? DateModified { get; private set; }
	public DateTime? DateAuthored { get; private set; }
	public bool CanDelete { get; private set; }
	public bool IsSystem { get; private set; }
	public bool IsHidden { get; private set; }
	public bool IsDRMProtected { get; private set; }
	public string? ParentId { get; private set; }
	public string? PersistentUniqueId { get; private set; }

	public bool IsRoot => Id == RootId;

	public bool IsFile => Type == ItemType.File;

	public Item? Parent
	{
		get
		{
			if (ParentId == null)
			{
				return null;
			}

			parent ??= new Item(device, ParentId, Path.GetDirectoryName(Path.GetDirectoryName(FullName)));

			return parent;
		}
	}

	#endregion

	#region Methods

	public IEnumerable<Item> GetChildren()
	{
		if (device.deviceContent == null)
		{
			yield break;
		}

		device.deviceContent.EnumObjects(0, Id, null, out IEnumPortableDeviceObjectIDs enumerator);
		if (enumerator == null)
		{
			Trace.WriteLine("IPortableDeviceContent.EnumObjects failed");
			yield break;
		}

		uint fetched = 0;
		string[] objectIds = new string[numObjectsToRequest];
		enumerator.Next(numObjectsToRequest, objectIds, ref fetched);
		while (fetched > 0)
		{
			for (int index = 0; index < fetched; index++)
			{
				Item? item = null;

				try
				{
					item = Create(device, objectIds[index], FullName);
				}
				catch (FileNotFoundException)
				{
					// handle system files, that cannot be opened or read.
					// Windows sometimes creates a fake files in e.g. System Volume Information.
					// Let's handle such situations.
				}

				if (item != null)
				{
					yield return item;
				}
			}

			enumerator.Next(numObjectsToRequest, objectIds, ref fetched);
		}
	}

	public IEnumerable<Item> GetChildren(string? pattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
	{
		if (device.deviceContent == null)
		{
			yield break;
		}

		pattern ??= "";

		device.deviceContent.EnumObjects(0, Id, null, out IEnumPortableDeviceObjectIDs enumerator);
		if (enumerator == null)
		{
			Trace.WriteLine("IPortableDeviceContent.EnumObjects failed");
			yield break;
		}

		uint fetched = 0;
		string[] objectIds = new string[numObjectsToRequest];
		enumerator.Next(numObjectsToRequest, objectIds, ref fetched);
		while (fetched > 0)
		{
			for (int index = 0; index < fetched; index++)
			{
				Item? item = null;

				try
				{
					item = Create(device, objectIds[index], FullName);
				}
				catch (FileNotFoundException)
				{
					// handle system files, that cannot be opened or read.
					// Windows sometimes creates a fake files in e.g.System Volume Information.
					// Let's handle such situations.
				}

				if (item != null)
				{
					if (pattern == null || (item.Name != null && Regex.IsMatch(item.Name, pattern, RegexOptions.IgnoreCase)))
					{
						yield return item;
					}

					if (searchOption == SearchOption.AllDirectories && item.Type != ItemType.File)
					{
						IEnumerable<Item> children = item.GetChildren(pattern, searchOption);
						foreach (Item c in children)
						{
							yield return c;
						}
					}
				}
			}

			enumerator.Next(numObjectsToRequest, objectIds, ref fetched);
		}
	}

	internal Item? CreateSubdirectory(string path)
	{
		Item? child = null;
		Item parent = this;
		string[] folders = path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string folder in folders)
		{
			child = parent.GetChildren().FirstOrDefault(i => device.EqualsName(i.Name, folder));
			if (child == null)
			{
				// create a new directory
				IPortableDeviceValues deviceValues = (IPortableDeviceValues)new PortableDeviceValues();
				deviceValues.SetStringValue(ref WPD.OBJECT_PARENT_ID, parent.Id);
				deviceValues.SetStringValue(ref WPD.OBJECT_NAME, folder);
				deviceValues.SetStringValue(ref WPD.OBJECT_ORIGINAL_FILE_NAME, folder);
				deviceValues.SetGuidValue(ref WPD.OBJECT_CONTENT_TYPE, ref WPD.CONTENT_TYPE_FOLDER);
				string id = string.Empty;
				try
				{
					device.deviceContent!.CreateObjectWithPropertiesOnly(deviceValues, ref id);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
					return null;
				}

				child = Create(device, id, parent.FullName);
			}
			else if (child.Type == ItemType.File)
			{
				// folder is already a file
				throw new IOException($"A part of the path '{folder}' is a file");
			}
			else
			{
				// folder exists
				//id = child.Id;
				//new Item()

				// TODO
			}

			parent = child;
		}

		return child;
	}

	public void Delete(bool recursive = false)
	{
		if (Id == null)
		{
			throw new InvalidOperationException("Id is null");
		}

		IPortableDevicePropVariantCollection objectIdCollection = (IPortableDevicePropVariantCollection)new PortableDevicePropVariantCollection();

		PropVariantFacade propVariantValue = PropVariantFacade.StringToPropVariant(Id);
		objectIdCollection.Add(ref propVariantValue.Value);

		IPortableDevicePropVariantCollection results = (IPortableDevicePropVariantCollection)new PortableDevicePropVariantCollection();
		// TODO: get the results back and handle failures correctly

		device.deviceContent!.Delete(recursive ? PORTABLE_DEVICE_DELETE_WITH_RECURSION : PORTABLE_DEVICE_DELETE_NO_RECURSION, objectIdCollection, ref results);

		ComTrace.WriteObject(objectIdCollection);
	}

	public string GetPath()
	{
		if (Id == RootId)
		{
			return @"\";
		}

		Item? item = this;
		StringBuilder sb = new();
		do
		{
			// ++ TODO
			if (string.IsNullOrWhiteSpace(item.ParentId))
			{
				item = TryHandleNonHierarchicalStorage();

				if (item == null)
				{
					throw new InvalidOperationException($"Failed to get the full object path on device {device.FriendlyName}.");
				}
			}

			// -- TODO

			_ = sb.Insert(0, item.Name);
			_ = sb.Insert(0, DirectorySeparatorChar);

		} while (!(item = new Item(device, item.ParentId!)).IsRoot);
		return sb.ToString();
	}

	// TODO

	/// <summary>
	/// Handles DCF storages specific for Apple iPhones.
	/// </summary>
	/// <returns></returns>
	private Item? TryHandleNonHierarchicalStorage()
	{
		// EXPLANATION
		// Some MTP compatible devices uses different storage formats that Generic
		// Hierarchical storage like WP, Android. Good examples are Apple devices,
		// which are using DCF storage. The specific in that storage is a way how
		// directories handles parent object ID. If in Generic Hierarchical storage
		// we check parent ID of root directory, it contains an ID of functional storage
		// so that means storage ID. In DCF when we check parent ID of root object
		// it will have object ID, not storage ID, e.g. parent id is o10001 (object10001),
		// but storage has ID = s10001 (storage10001). So to find a parent of top most folder
		// we need to fetch an object functional container ID. Which is storage for top most
		// directory.
		MediaDriveInfo[]? drives = device.GetDrives();
		MediaDriveInfo? storageRoot = drives?.FirstOrDefault(s => s.RootDirectory?.Id == ParentContainerId);
		return storageRoot?.RootDirectory?.item;
	}

	internal Stream OpenRead()
	{
		if (Id == null)
		{
			throw new InvalidOperationException("Id is null");
		}

		device.deviceContent!.Transfer(out IPortableDeviceResources resources);

		uint optimalTransferSize = 0;

		resources.GetStream(Id, ref WPD.RESOURCE_DEFAULT, 0, ref optimalTransferSize, out IStream wpdStream);

		return new StreamWrapper(wpdStream, Size);
	}

	internal Stream OpenReadThumbnail()
	{
		if (Id == null)
		{
			throw new InvalidOperationException("Id is null");
		}

		device.deviceContent!.Transfer(out IPortableDeviceResources resources);

		uint optimalTransferSize = 0;

		resources.GetStream(Id, ref WPD.RESOURCE_THUMBNAIL, 0, ref optimalTransferSize, out IStream wpdStream);

		return new StreamWrapper(wpdStream, Size);
	}

	internal Stream OpenReadIcon()
	{
		if (Id == null)
		{
			throw new InvalidOperationException("Id is null");
		}

		device.deviceContent!.Transfer(out IPortableDeviceResources resources);

		uint optimalTransferSize = 0;

		resources.GetStream(Id, ref WPD.RESOURCE_ICON, 0, ref optimalTransferSize, out IStream wpdStream);

		return new StreamWrapper(wpdStream, Size);
	}

	internal void UploadFile(string fileName, Stream stream)
	{

		IPortableDeviceValues portableDeviceValues = (IPortableDeviceValues)new PortableDeviceValues();

		portableDeviceValues.SetStringValue(ref WPD.OBJECT_PARENT_ID, Id);
		portableDeviceValues.SetUnsignedLargeIntegerValue(ref WPD.OBJECT_SIZE, (ulong)stream.Length);
		portableDeviceValues.SetStringValue(ref WPD.OBJECT_ORIGINAL_FILE_NAME, fileName);
		portableDeviceValues.SetStringValue(ref WPD.OBJECT_NAME, fileName);
		// test
		using PropVariantFacade now = PropVariantFacade.DateTimeToPropVariant(DateTime.Now);
		portableDeviceValues.SetValue(ref WPD.OBJECT_DATE_CREATED, ref now.Value);
		portableDeviceValues.SetValue(ref WPD.OBJECT_DATE_MODIFIED, ref now.Value);

		uint num = 0u;
		string? text = null;
		device.deviceContent!.CreateObjectWithPropertiesAndData(portableDeviceValues, out IStream wpdStream, ref num, ref text);

		using StreamWrapper destinationStream = new(wpdStream);
		stream.CopyTo(destinationStream);
		destinationStream.Flush();
	}

	internal bool Rename(string newName)
	{
		IPortableDeviceValues portableDeviceValues = (IPortableDeviceValues)new PortableDeviceValues();

		// with OBJECT_NAME does not work for Amazon Kindle Paperwhite
		portableDeviceValues.SetStringValue(ref WPD.OBJECT_ORIGINAL_FILE_NAME, newName);
		device.deviceProperties!.SetValues(Id, portableDeviceValues, out IPortableDeviceValues result);
		ComTrace.WriteObject(result);

		if (result.TryGetStringValue(WPD.OBJECT_ORIGINAL_FILE_NAME, out string check))
		{
			if (check == "Error: S_OK")
			{
				// id can change on rename (e.g. Amazon Kindle Paperwhite) so find new one
				Item? newItem = parent?.GetChildren().FirstOrDefault(i => device.EqualsName(i.Name, newName));
				Id = newItem?.Id;

				Refresh();
				return true;
			}
		}

		return false;
	}

	internal void SetDateCreated(DateTime value)
	{
		IPortableDeviceValues portableDeviceValues = (IPortableDeviceValues)new PortableDeviceValues();

		using (PropVariantFacade val = PropVariantFacade.DateTimeToPropVariant(value))
		{
			portableDeviceValues.SetValue(ref WPD.OBJECT_DATE_CREATED, ref val.Value);
			device.deviceProperties!.SetValues(Id, portableDeviceValues, out IPortableDeviceValues result);
			ComTrace.WriteObject(result);
		}

		Refresh();
	}

	internal void SetDateModified(DateTime value)
	{
		IPortableDeviceValues portableDeviceValues = (IPortableDeviceValues)new PortableDeviceValues();

		using (PropVariantFacade val = PropVariantFacade.DateTimeToPropVariant(value))
		{
			portableDeviceValues.SetValue(ref WPD.OBJECT_DATE_MODIFIED, ref val.Value);
			device.deviceProperties!.SetValues(Id, portableDeviceValues, out IPortableDeviceValues result);
			ComTrace.WriteObject(result);
		}

		Refresh();
	}

	internal void SetDateAuthored(DateTime value)
	{
		IPortableDeviceValues portableDeviceValues = (IPortableDeviceValues)new PortableDeviceValues();

		using (PropVariantFacade val = PropVariantFacade.DateTimeToPropVariant(value))
		{
			portableDeviceValues.SetValue(ref WPD.OBJECT_DATE_AUTHORED, ref val.Value);
			device.deviceProperties!.SetValues(Id, portableDeviceValues, out IPortableDeviceValues result);
			ComTrace.WriteObject(result);
		}

		Refresh();
	}

	#endregion
}

