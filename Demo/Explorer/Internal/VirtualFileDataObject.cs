﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
#if NET8_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif
using System.Windows;

// http://dlaa.me/blog/post/9917797

namespace ExplorerCtrl.Internal;

/// <summary>
/// Class implementing drag/drop and clipboard support for virtual files.
/// Also offers an alternate interface to the IDataObject interface.
/// </summary>
internal sealed partial class VirtualFileDataObject : System.Runtime.InteropServices.ComTypes.IDataObject, IAsyncOperation
{
	/// <summary>
	/// Gets or sets a value indicating whether the data object can be used asynchronously.
	/// </summary>
	public bool IsAsynchronous { get; set; }

	/// <summary>
	/// Identifier for CFSTR_FILECONTENTS.
	/// </summary>
	private static readonly short FILECONTENTS = (short)DataFormats.GetDataFormat(NativeMethods.CFSTR_FILECONTENTS).Id;

	/// <summary>
	/// Identifier for CFSTR_FILEDESCRIPTORW.
	/// </summary>
	private static readonly short FILEDESCRIPTORW = (short)DataFormats.GetDataFormat(NativeMethods.CFSTR_FILEDESCRIPTORW).Id;

	/// <summary>
	/// Identifier for CFSTR_PASTESUCCEEDED.
	/// </summary>
	private static readonly short PASTESUCCEEDED = (short)DataFormats.GetDataFormat(NativeMethods.CFSTR_PASTESUCCEEDED).Id;

	/// <summary>
	/// Identifier for CFSTR_PERFORMEDDROPEFFECT.
	/// </summary>
	private static readonly short PERFORMEDDROPEFFECT = (short)DataFormats.GetDataFormat(NativeMethods.CFSTR_PERFORMEDDROPEFFECT).Id;

	/// <summary>
	/// Identifier for CFSTR_PREFERREDDROPEFFECT.
	/// </summary>
	private static readonly short PREFERREDDROPEFFECT = (short)DataFormats.GetDataFormat(NativeMethods.CFSTR_PREFERREDDROPEFFECT).Id;

	/// <summary>
	/// In-order list of registered data objects.
	/// </summary>
	private readonly List<DataObject> _dataObjects = [];

	/// <summary>
	/// Tracks whether an asynchronous operation is ongoing.
	/// </summary>
	private bool _inOperation;

	/// <summary>
	/// Stores the user-specified start action.
	/// </summary>
	private readonly Action<VirtualFileDataObject> _startAction;

	/// <summary>
	/// Stores the user-specified end action.
	/// </summary>
	private readonly Action<VirtualFileDataObject> _endAction;

	private readonly Action<Stream, FileDescriptor> _streamContents;

	/// <summary>
	/// Initializes a new instance of the VirtualFileDataObject class.
	/// </summary>
	public VirtualFileDataObject() => IsAsynchronous = true;

	/// <summary>
	/// Initializes a new instance of the VirtualFileDataObject class.
	/// </summary>
	/// <param name="startAction">Optional action to run at the start of the data transfer.</param>
	/// <param name="endAction">Optional action to run at the end of the data transfer.</param>
	public VirtualFileDataObject(Action<VirtualFileDataObject> startAction, Action<VirtualFileDataObject> endAction, Action<Stream, FileDescriptor> streamContents)
		: this()
	{
		_startAction = startAction;
		_endAction = endAction;
		_streamContents = streamContents;
	}

	#region IDataObject Members
	// Explicit interface implementation hides the technical details from users of VirtualFileDataObject.

	/// <summary>
	/// Creates a connection between a data object and an advisory sink.
	/// </summary>
	/// <param name="pFormatetc">A FORMATETC structure that defines the format, target device, aspect, and medium that will be used for future notifications.</param>
	/// <param name="advf">One of the ADVF values that specifies a group of flags for controlling the advisory connection.</param>
	/// <param name="adviseSink">A pointer to the IAdviseSink interface on the advisory sink that will receive the change notification.</param>
	/// <param name="connection">When this method returns, contains a pointer to a DWORD token that identifies this connection.</param>
	/// <returns>HRESULT success code.</returns>
	int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.OLE_E_ADVISENOTSUPPORTED);
		throw new NotImplementedException();
	}

	/// <summary>
	/// Destroys a notification connection that had been previously established.
	/// </summary>
	/// <param name="connection">A DWORD token that specifies the connection to remove.</param>
	void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise(int connection)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.OLE_E_ADVISENOTSUPPORTED);
		throw new NotImplementedException();
	}

	/// <summary>
	/// Creates an object that can be used to enumerate the current advisory connections.
	/// </summary>
	/// <param name="enumAdvise">When this method returns, contains an IEnumSTATDATA that receives the interface pointer to the new enumerator object.</param>
	/// <returns>HRESULT success code.</returns>
	int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
	{
		Marshal.ThrowExceptionForHR(NativeMethods.OLE_E_ADVISENOTSUPPORTED);
		throw new NotImplementedException();
	}

	/// <summary>
	/// Creates an object for enumerating the FORMATETC structures for a data object.
	/// </summary>
	/// <param name="direction">One of the DATADIR values that specifies the direction of the data.</param>
	/// <returns>IEnumFORMATETC interface.</returns>
	IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
	{
		if (direction == DATADIR.DATADIR_GET)
		{
			if (0 == _dataObjects.Count)
			{
				// Note: SHCreateStdEnumFmtEtc fails for a count of 0; throw helpful exception
				throw new InvalidOperationException("VirtualFileDataObject requires at least one data object to enumerate.");
			}

			// Create enumerator and return it
			if (NativeMethods.SUCCEEDED(NativeMethods.SHCreateStdEnumFmtEtc((uint)_dataObjects.Count, _dataObjects.Select(d => d.FORMATETC).ToArray(), out IEnumFORMATETC enumerator)))
			{
				return enumerator;
			}

			// Returning null here can cause an AV in the caller; throw instead
			Marshal.ThrowExceptionForHR(NativeMethods.E_FAIL);
		}

		throw new NotImplementedException();
	}

	/// <summary>
	/// Provides a standard FORMATETC structure that is logically equivalent to a more complex structure.
	/// </summary>
	/// <param name="formatIn">A pointer to a FORMATETC structure that defines the format, medium, and target device that the caller would like to use to retrieve data in a subsequent call such as GetData.</param>
	/// <param name="formatOut">When this method returns, contains a pointer to a FORMATETC structure that contains the most general information possible for a specific rendering, making it canonically equivalent to formatetIn.</param>
	/// <returns>HRESULT success code.</returns>
	int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();

	/// <summary>
	/// Obtains data from a source data object.
	/// </summary>
	/// <param name="format">A pointer to a FORMATETC structure that defines the format, medium, and target device to use when passing the data.</param>
	/// <param name="medium">When this method returns, contains a pointer to the STGMEDIUM structure that indicates the storage medium containing the returned data through its tymed member, and the responsibility for releasing the medium through the value of its pUnkForRelease member.</param>
	void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
	{
		medium = new STGMEDIUM();
		int hr = ((System.Runtime.InteropServices.ComTypes.IDataObject)this).QueryGetData(ref format);
		if (NativeMethods.SUCCEEDED(hr))
		{
			// Find the best match
			FORMATETC formatCopy = format; // Cannot use ref or out parameter inside an anonymous method, lambda expression, or query expression
			DataObject dataObject = _dataObjects
					.Where(d =>
						(d.FORMATETC.cfFormat == formatCopy.cfFormat) &&
						(d.FORMATETC.dwAspect == formatCopy.dwAspect) &&
						0 != (d.FORMATETC.tymed & formatCopy.tymed) &&
						(d.FORMATETC.lindex == formatCopy.lindex))
					.FirstOrDefault();
			if (dataObject != null)
			{
				if (!IsAsynchronous && (FILEDESCRIPTORW == dataObject.FORMATETC.cfFormat) && !_inOperation)
				{
					// Enter the operation and call the start action
					_inOperation = true;
					_startAction?.Invoke(this);
				}

				// Populate the STGMEDIUM

				/* Unmerged change from project 'Explorer (net7.0-windows)'
				Before:
									medium.tymed = dataObject.FORMATETC.tymed;
									var result = dataObject.GetData(); // Possible call to user code
									hr = result.Item2;
				After:
									medium.tymed = dataObject.FORMATETC.tymed;
								Tuple<nint, int> result = dataObject.GetData(); // Possible call to user code
									hr = result.Item2;
				*/
				medium.tymed = dataObject.FORMATETC.tymed;
				(IntPtr, int) result = dataObject.GetData(); // Possible call to user code
				hr = result.Item2;
				if (NativeMethods.SUCCEEDED(hr))
				{
					medium.unionmember = result.Item1;
				}
			}
			else
			{
				// Couldn't find a match
				hr = NativeMethods.DV_E_FORMATETC;
			}
		}

		if (!NativeMethods.SUCCEEDED(hr)) // Not redundant; hr gets updated in the block above
		{
			Marshal.ThrowExceptionForHR(hr);
		}
	}

	/// <summary>
	/// Obtains data from a source data object.
	/// </summary>
	/// <param name="format">A pointer to a FORMATETC structure that defines the format, medium, and target device to use when passing the data.</param>
	/// <param name="medium">A STGMEDIUM that defines the storage medium containing the data being transferred.</param>
	void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();

	/// <summary>
	/// Determines whether the data object is capable of rendering the data described in the FORMATETC structure.
	/// </summary>
	/// <param name="format">A pointer to a FORMATETC structure that defines the format, medium, and target device to use for the query.</param>
	/// <returns>HRESULT success code.</returns>
	int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData(ref FORMATETC format)
	{
		FORMATETC formatCopy = format; // Cannot use ref or out parameter inside an anonymous method, lambda expression, or query expression
		IEnumerable<DataObject> formatMatches = _dataObjects.Where(d => d.FORMATETC.cfFormat == formatCopy.cfFormat);
		if (!formatMatches.Any())
		{
			return NativeMethods.DV_E_FORMATETC;
		}

		IEnumerable<DataObject> tymedMatches = formatMatches.Where(d => 0 != (d.FORMATETC.tymed & formatCopy.tymed));
		if (!tymedMatches.Any())
		{
			return NativeMethods.DV_E_TYMED;
		}

		IEnumerable<DataObject> aspectMatches = tymedMatches.Where(d => d.FORMATETC.dwAspect == formatCopy.dwAspect);
		if (!aspectMatches.Any())
		{
			return NativeMethods.DV_E_DVASPECT;
		}

		return NativeMethods.S_OK;
	}

	/// <summary>
	/// Transfers data to the object that implements this method.
	/// </summary>
	/// <param name="formatIn">A FORMATETC structure that defines the format used by the data object when interpreting the data contained in the storage medium.</param>
	/// <param name="medium">A STGMEDIUM structure that defines the storage medium in which the data is being passed.</param>
	/// <param name="release">true to specify that the data object called, which implements SetData, owns the storage medium after the call returns.</param>
	void System.Runtime.InteropServices.ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
	{
		bool handled = false;
		if ((formatIn.dwAspect == DVASPECT.DVASPECT_CONTENT) &&
			(formatIn.tymed == TYMED.TYMED_HGLOBAL) &&
			(medium.tymed == formatIn.tymed))

		/* Unmerged change from project 'Explorer (net7.0-windows)'
		Before:
						var ptr = NativeMethods.GlobalLock(medium.unionmember);
						if (IntPtr.Zero != ptr)
		After:
						nint ptr = NativeMethods.GlobalLock(medium.unionmember);
						if (IntPtr.Zero != ptr)
		*/
		{
			// Supported format; capture the data
			IntPtr ptr = NativeMethods.GlobalLock(medium.unionmember);
			if (IntPtr.Zero != ptr)
			{
				try
				{
					int length = NativeMethods.GlobalSize(ptr).ToInt32();
					byte[] data = new byte[length];
					Marshal.Copy(ptr, data, 0, length);
					// Store it in our own format
					SetData(formatIn.cfFormat, data);
					handled = true;
				}
				finally
				{
					_ = NativeMethods.GlobalUnlock(medium.unionmember);
				}
			}

			// Release memory if we now own it
			if (release)
			{
				Marshal.FreeHGlobal(medium.unionmember);
			}
		}

		// Handle synchronous mode
		if (!IsAsynchronous && (PERFORMEDDROPEFFECT == formatIn.cfFormat) && _inOperation)
		{
			// Call the end action and exit the operation
			_endAction?.Invoke(this);

			_inOperation = false;
		}

		// Throw if unhandled
		if (!handled)
		{
			throw new NotImplementedException();
		}
	}

	#endregion

	/// <summary>
	/// Provides data for the specified data format (HGLOBAL).
	/// </summary>
	/// <param name="dataFormat">Data format.</param>
	/// <param name="data">Sequence of data.</param>
	public void SetData(short dataFormat, IEnumerable<byte> data)
	{
		_dataObjects.Add(
			new DataObject
			{
				FORMATETC = new FORMATETC
				{
					cfFormat = dataFormat,
					ptd = IntPtr.Zero,
					dwAspect = DVASPECT.DVASPECT_CONTENT,
					lindex = -1,
					tymed = TYMED.TYMED_HGLOBAL
				},
				GetData = () =>

				/* Unmerged change from project 'Explorer (net7.0-windows)'
				Before:
									{
										var dataArray = data.ToArray();
										var ptr = Marshal.AllocHGlobal(dataArray.Length);
										Marshal.Copy(dataArray, 0, ptr, dataArray.Length);
				After:
									{
										byte[] dataArray = data.ToArray();
										nint ptr = Marshal.AllocHGlobal(dataArray.Length);
										Marshal.Copy(dataArray, 0, ptr, dataArray.Length);
				*/
				{
					byte[] dataArray = data.ToArray();
					IntPtr ptr = Marshal.AllocHGlobal(dataArray.Length);
					Marshal.Copy(dataArray, 0, ptr, dataArray.Length);
					return (ptr, NativeMethods.S_OK);
				},
			});
	}

	/// <summary>
	/// Provides data for the specified data format and index (ISTREAM).
	/// </summary>
	/// <param name="dataFormat">Data format.</param>
	/// <param name="index">Index of data.</param>
	/// <param name="streamData">Action generating the data.</param>
	/// <remarks>
	/// Uses Stream instead of IEnumerable(T) because Stream is more likely
	/// to be natural for the expected scenarios.
	/// </remarks>
	public void SetData(short dataFormat, int index, FileDescriptor fileDescriptor, Action<Stream, FileDescriptor> streamData)
	{
		_dataObjects.Add(
			new DataObject
			{
				FORMATETC = new FORMATETC
				{
					cfFormat = dataFormat,
					ptd = IntPtr.Zero,
					dwAspect = DVASPECT.DVASPECT_CONTENT,
					lindex = index,
					tymed = TYMED.TYMED_ISTREAM
				},
				GetData = () =>

				/* Unmerged change from project 'Explorer (net7.0-windows)'
				Before:
										var ptr = IntPtr.Zero;
										var iStream = NativeMethods.CreateStreamOnHGlobal(IntPtr.Zero, true);
										if (streamData != null)
				After:
										nint ptr = IntPtr.Zero;
										IStream iStream = NativeMethods.CreateStreamOnHGlobal(IntPtr.Zero, true);
										if (streamData != null)
				*/
				{
					// Create IStream for data
					IntPtr ptr = IntPtr.Zero;
					IStream iStream = NativeMethods.CreateStreamOnHGlobal(IntPtr.Zero, true);
					if (streamData != null)
					{
						// Wrap in a .NET-friendly Stream and call provided code to fill it
						using IStreamWrapper stream = new(iStream);
						streamData(stream, fileDescriptor);
					}
					// Return an IntPtr for the IStream
					ptr = Marshal.GetComInterfaceForObject(iStream, typeof(IStream));
					_ = Marshal.ReleaseComObject(iStream);
					return (ptr, NativeMethods.S_OK);
				},
			});
	}

	/// <summary>
	/// Provides data for the specified data format (FILEGROUPDESCRIPTOR/FILEDESCRIPTOR)
	/// </summary>
	/// <param name="fileDescriptors">Collection of virtual files.</param>
	public void SetData(IEnumerable<FileDescriptor> fileDescriptors)
	{
		// Prepare buffer
		List<byte> bytes =
		[
			// Add FILEGROUPDESCRIPTOR header
			.. StructureBytes(new NativeMethods.FILEGROUPDESCRIPTOR { cItems = (uint)fileDescriptors.Count() }),
		];
		// Add n FILEDESCRIPTORs
		foreach (FileDescriptor fileDescriptor in fileDescriptors)
		{
			// Set required fields
			NativeMethods.FILEDESCRIPTOR FILEDESCRIPTOR = new()
			{
				cFileName = fileDescriptor.Name,
			};
			// Set optional timestamp
			if (fileDescriptor.ChangeTimeUtc.HasValue)
			{

				/* Unmerged change from project 'Explorer (netframework4.8)'
				Before:
									FILEDESCRIPTOR.dwFlags |= NativeMethods.FD_CREATETIME | NativeMethods.FD_WRITESTIME;
									var changeTime = fileDescriptor.ChangeTimeUtc.Value.ToLocalTime().ToFileTime();
									var changeTimeFileTime = new System.Runtime.InteropServices.ComTypes.FILETIME
									{
				After:
									FILEDESCRIPTOR.dwFlags |= NativeMethods.FD_CREATETIME | NativeMethods.FD_WRITESTIME;
								long changeTime = fileDescriptor.ChangeTimeUtc.Value.ToLocalTime().ToFileTime();
								System.Runtime.InteropServices.ComTypes.FILETIME changeTimeFileTime = new System.Runtime.InteropServices.ComTypes.FILETIME
									{
				*/
				FILEDESCRIPTOR.dwFlags |= NativeMethods.FD_CREATETIME | NativeMethods.FD_WRITESTIME;
				long changeTime = fileDescriptor.ChangeTimeUtc.Value.ToLocalTime().ToFileTime();
				System.Runtime.InteropServices.ComTypes.FILETIME changeTimeFileTime = new()
				{
					dwLowDateTime = (int)(changeTime & 0xffffffff),
					dwHighDateTime = (int)(changeTime >> 32),
				};
				FILEDESCRIPTOR.ftLastWriteTime = changeTimeFileTime;
				FILEDESCRIPTOR.ftCreationTime = changeTimeFileTime;
			}
			// Set optional length
			if (fileDescriptor.Length.HasValue)
			{
				FILEDESCRIPTOR.dwFlags |= NativeMethods.FD_FILESIZE;
				FILEDESCRIPTOR.nFileSizeLow = (uint)(fileDescriptor.Length & 0xffffffff);
				FILEDESCRIPTOR.nFileSizeHigh = (uint)(fileDescriptor.Length >> 32);
			}

			if (fileDescriptor.IsDirectory)
			{
				FILEDESCRIPTOR.dwFlags |= NativeMethods.FD_ATTRIBUTES;
				FILEDESCRIPTOR.dwFileAttributes = NativeMethods.FILE_ATTRIBUTE_DIRECTORY;
			}
			// Add structure to buffer
			bytes.AddRange(StructureBytes(FILEDESCRIPTOR));
		}

		// Set CFSTR_FILEDESCRIPTORW
		SetData(FILEDESCRIPTORW, bytes);
		// Set n CFSTR_FILECONTENTS
		int index = 0;
		foreach (FileDescriptor fileDescriptor in fileDescriptors)
		{
			// TODO: check
			//SetData(FILECONTENTS, index, fileDescriptor.Data, fileDescriptor.StreamContents);
			// TODO: check
			SetData(FILECONTENTS, index, fileDescriptor, _streamContents);

			index++;
		}
	}

	/// <summary>
	/// Gets or sets the CFSTR_PASTESUCCEEDED value for the object.
	/// </summary>
	public DragDropEffects? PasteSucceeded
	{
		get => GetDropEffect(PASTESUCCEEDED);
		set => SetData(PASTESUCCEEDED, BitConverter.GetBytes((uint)value));
	}

	/// <summary>
	/// Gets or sets the CFSTR_PERFORMEDDROPEFFECT value for the object.
	/// </summary>
	public DragDropEffects? PerformedDropEffect
	{
		get => GetDropEffect(PERFORMEDDROPEFFECT);
		set => SetData(PERFORMEDDROPEFFECT, BitConverter.GetBytes((uint)value));
	}

	/// <summary>
	/// Gets or sets the CFSTR_PREFERREDDROPEFFECT value for the object.
	/// </summary>
	public DragDropEffects? PreferredDropEffect
	{
		get => GetDropEffect(PREFERREDDROPEFFECT);
		set => SetData(PREFERREDDROPEFFECT, BitConverter.GetBytes((uint)value));
	}

	/// <summary>
	/// Gets the DragDropEffects value (if any) previously set on the object.
	/// </summary>
	/// <param name="format">Clipboard format.</param>
	/// <returns>DragDropEffects value or null.</returns>
	private DragDropEffects? GetDropEffect(short format)
	{
		// Get the most recent setting
		DataObject dataObject = _dataObjects
				.Where(d =>
					(format == d.FORMATETC.cfFormat) &&
					(DVASPECT.DVASPECT_CONTENT == d.FORMATETC.dwAspect) &&
					(TYMED.TYMED_HGLOBAL == d.FORMATETC.tymed))
				.LastOrDefault();
		if (null != dataObject)

		/* Unmerged change from project 'Explorer (net7.0-windows)'
		Before:
					{
						// Read the value and return it
						var result = dataObject.GetData();
						if (NativeMethods.SUCCEEDED(result.Item2))
		After:
					{
					// Read the value and return it
					Tuple<nint, int> result = dataObject.GetData();
						if (NativeMethods.SUCCEEDED(result.Item2))
		*/
		{
			// Read the value and return it
			(IntPtr, int) result = dataObject.GetData();
			if (NativeMethods.SUCCEEDED(result.Item2))

			/* Unmerged change from project 'Explorer (net7.0-windows)'
			Before:
								var ptr = NativeMethods.GlobalLock(result.Item1);
								if (IntPtr.Zero != ptr)
			After:
								nint ptr = NativeMethods.GlobalLock(result.Item1);
								if (IntPtr.Zero != ptr)
			*/
			{
				IntPtr ptr = NativeMethods.GlobalLock(result.Item1);
				if (IntPtr.Zero != ptr)
				{
					try
					{
						int length = NativeMethods.GlobalSize(ptr).ToInt32();
						if (4 == length)
						{
							byte[] data = new byte[length];
							Marshal.Copy(ptr, data, 0, length);
							return (DragDropEffects)BitConverter.ToUInt32(data, 0);
						}
					}
					finally
					{
						_ = NativeMethods.GlobalUnlock(result.Item1);
					}
				}
			}
		}

		return null;
	}

	#region IAsyncOperation Members
	// Explicit interface implementation hides the technical details from users of VirtualFileDataObject.

	/// <summary>
	/// Called by a drop source to specify whether the data object supports asynchronous data extraction.
	/// </summary>
	/// <param name="fDoOpAsync">A Boolean value that is set to VARIANT_TRUE to indicate that an asynchronous operation is supported, or VARIANT_FALSE otherwise.</param>
	void IAsyncOperation.SetAsyncMode(int fDoOpAsync) => IsAsynchronous = !(NativeMethods.VARIANT_FALSE == fDoOpAsync);

	/// <summary>
	/// Called by a drop target to determine whether the data object supports asynchronous data extraction.
	/// </summary>
	/// <param name="pfIsOpAsync">A Boolean value that is set to VARIANT_TRUE to indicate that an asynchronous operation is supported, or VARIANT_FALSE otherwise.</param>
	void IAsyncOperation.GetAsyncMode(out int pfIsOpAsync) => pfIsOpAsync = IsAsynchronous ? NativeMethods.VARIANT_TRUE : NativeMethods.VARIANT_FALSE;

	/// <summary>
	/// Called by a drop target to indicate that asynchronous data extraction is starting.
	/// </summary>
	/// <param name="pbcReserved">Reserved. Set this value to NULL.</param>
	void IAsyncOperation.StartOperation(IBindCtx pbcReserved)
	{
		_inOperation = true;
		_startAction?.Invoke(this);
	}

	/// <summary>
	/// Called by the drop source to determine whether the target is extracting data asynchronously.
	/// </summary>
	/// <param name="pfInAsyncOp">Set to VARIANT_TRUE if data extraction is being handled asynchronously, or VARIANT_FALSE otherwise.</param>
	void IAsyncOperation.InOperation(out int pfInAsyncOp) => pfInAsyncOp = _inOperation ? NativeMethods.VARIANT_TRUE : NativeMethods.VARIANT_FALSE;

	/// <summary>
	/// Notifies the data object that that asynchronous data extraction has ended.
	/// </summary>
	/// <param name="hResult">An HRESULT value that indicates the outcome of the data extraction. Set to S_OK if successful, or a COM error code otherwise.</param>
	/// <param name="pbcReserved">Reserved. Set to NULL.</param>
	/// <param name="dwEffects">A DROPEFFECT value that indicates the result of an optimized move. This should be the same value that would be passed to the data object as a CFSTR_PERFORMEDDROPEFFECT format with a normal data extraction operation.</param>
	void IAsyncOperation.EndOperation(int hResult, IBindCtx pbcReserved, uint dwEffects)
	{
		_endAction?.Invoke(this);

		_inOperation = false;
	}

	#endregion

	/// <summary>
	/// Returns the in-memory representation of an interop structure.
	/// </summary>
	/// <param name="source">Structure to return.</param>
	/// <returns>In-memory representation of structure.</returns>
	private static byte[] StructureBytes(object source)

	/* Unmerged change from project 'Explorer (net7.0-windows)'
	Before:
			{
				// Set up for call to StructureToPtr
				var size = Marshal.SizeOf(source.GetType());
				var ptr = Marshal.AllocHGlobal(size);
				var bytes = new byte[size];
				try
	After:
			{
			// Set up for call to StructureToPtr
			int size = Marshal.SizeOf(source.GetType());
				nint ptr = Marshal.AllocHGlobal(size);
			byte[] bytes = new byte[size];
				try
	*/
	{
		// Set up for call to StructureToPtr
		int size = Marshal.SizeOf(source.GetType());
		IntPtr ptr = Marshal.AllocHGlobal(size);
		byte[] bytes = new byte[size];
		try
		{
			Marshal.StructureToPtr(source, ptr, false);
			// Copy marshalled bytes to buffer
			Marshal.Copy(ptr, bytes, 0, size);
		}
		finally
		{
			Marshal.FreeHGlobal(ptr);
		}

		return bytes;
	}

	/// <summary>
	/// Class representing the result of a SetData call.
	/// </summary>
	private sealed class DataObject
	{
		/// <summary>
		/// FORMATETC structure for the data.
		/// </summary>
		public FORMATETC FORMATETC { get; set; }

		/// <summary>
		/// Func returning the data as an IntPtr and an HRESULT success code.
		/// </summary>
		public Func<(IntPtr, int)> GetData { get; set; }
	}

	/// <summary>
	/// Simple class that exposes a write-only IStream as a Stream.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the IStreamWrapper class.
	/// </remarks>
	/// <param name="iStream">IStream instance to wrap.</param>
	private sealed class IStreamWrapper(IStream iStream) : Stream
	{
		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead => false;

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek => false;

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite => true;

		/// <summary>
		/// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush() => throw new NotImplementedException();

		/// <summary>
		/// Gets the length in bytes of the stream.
		/// </summary>
		public override long Length => throw new NotImplementedException();

		/// <summary>
		/// Gets or sets the position within the current stream.
		/// </summary>
		public override long Position
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

		/// <summary>
		/// Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the origin parameter.</param>
		/// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

		/// <summary>
		/// Sets the length of the current stream.
		/// </summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		public override void SetLength(long value) => throw new NotImplementedException();

		/// <summary>
		/// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (offset == 0)
			{
				// Optimize common case to avoid creating extra buffers
				iStream.Write(buffer, count, IntPtr.Zero);
			}
			else
			{
				// Easy way to provide the relevant byte[]
				iStream.Write(buffer.Skip(offset).ToArray(), count, IntPtr.Zero);
			}
		}
	}

	/// <summary>
	/// Initiates a drag-and-drop operation.
	/// </summary>
	/// <param name="dataObject">A data object that contains the data being dragged.</param>
	/// <param name="allowedEffects">One of the DragDropEffects values that specifies permitted effects of the drag-and-drop operation.</param>
	/// <returns>One of the DragDropEffects values that specifies the final effect that was performed during the drag-and-drop operation.</returns>
	/// <remarks>
	/// Call this method instead of System.Windows.DragDrop.DoDragDrop because this method handles IDataObject better.
	/// </remarks>
	public static DragDropEffects DoDragDrop(System.Runtime.InteropServices.ComTypes.IDataObject dataObject, DragDropEffects allowedEffects)
	{
		int[] finalEffect = new int[1];
		try
		{
			NativeMethods.DoDragDrop(dataObject, new DropSource(), (int)allowedEffects, finalEffect);
		}
		finally
		{
			if ((dataObject is VirtualFileDataObject virtualFileDataObject) && !virtualFileDataObject.IsAsynchronous && virtualFileDataObject._inOperation)
			{
				// Call the end action and exit the operation
				virtualFileDataObject._endAction?.Invoke(virtualFileDataObject);

				virtualFileDataObject._inOperation = false;
			}
		}

		return (DragDropEffects)finalEffect[0];
	}

	/// <summary>
	/// Contains the methods for generating visual feedback to the end user and for canceling or completing the drag-and-drop operation.
	/// </summary>
#if NET8_0_OR_GREATER
	[GeneratedComClass]
#endif
	private sealed partial class DropSource : NativeMethods.IDropSource
	{
		/// <summary>
		/// Determines whether a drag-and-drop operation should continue.
		/// </summary>
		/// <param name="fEscapePressed">Indicates whether the Esc key has been pressed since the previous call to QueryContinueDrag or to DoDragDrop if this is the first call to QueryContinueDrag. A TRUE value indicates the end user has pressed the escape key; a FALSE value indicates it has not been pressed.</param>
		/// <param name="grfKeyState">The current state of the keyboard modifier keys on the keyboard. Possible values can be a combination of any of the flags MK_CONTROL, MK_SHIFT, MK_ALT, MK_BUTTON, MK_LBUTTON, MK_MBUTTON, and MK_RBUTTON.</param>
		/// <returns>This method returns S_OK/DRAGDROP_S_DROP/DRAGDROP_S_CANCEL on success.</returns>
		public int QueryContinueDrag(int fEscapePressed, uint grfKeyState)
		{
			bool escapePressed = 0 != fEscapePressed;
			DragDropKeyStates keyStates = (DragDropKeyStates)grfKeyState;
			if (escapePressed)
			{
				return NativeMethods.DRAGDROP_S_CANCEL;
			}
			else if (DragDropKeyStates.None == (keyStates & DragDropKeyStates.LeftMouseButton))
			{
				return NativeMethods.DRAGDROP_S_DROP;
			}

			return NativeMethods.S_OK;
		}

		/// <summary>
		/// Gives visual feedback to an end user during a drag-and-drop operation.
		/// </summary>
		/// <param name="dwEffect">The DROPEFFECT value returned by the most recent call to IDropTarget::DragEnter, IDropTarget::DragOver, or IDropTarget::DragLeave. </param>
		/// <returns>This method returns S_OK on success.</returns>
		public int GiveFeedback(uint dwEffect) => NativeMethods.DRAGDROP_S_USEDEFAULTCURSORS;
	}

	/// <summary>
	/// Provides access to Win32-level constants, structures, and functions.
	/// </summary>
	public static partial class NativeMethods
	{
		public const int DRAGDROP_S_DROP = 0x00040100;
		public const int DRAGDROP_S_CANCEL = 0x00040101;
		public const int DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102;
		public const int DV_E_DVASPECT = -2147221397;
		public const int DV_E_FORMATETC = -2147221404;
		public const int DV_E_TYMED = -2147221399;
		public const int E_FAIL = -2147467259;
		public const uint FD_CREATETIME = 0x00000008;
		public const uint FD_WRITESTIME = 0x00000020;
		public const uint FD_FILESIZE = 0x00000040;
		public const uint FD_ATTRIBUTES = 0x00000004;
		public const int OLE_E_ADVISENOTSUPPORTED = -2147221501;
		public const int S_OK = 0;
		public const int S_FALSE = 1;
		public const int VARIANT_FALSE = 0;
		public const int VARIANT_TRUE = -1;

		public const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
		public const int FILE_ATTRIBUTE_NORMAL = 0x80;

		public const string CFSTR_FILECONTENTS = "FileContents";
		public const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
		public const string CFSTR_PASTESUCCEEDED = "Paste Succeeded";
		public const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
		public const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";

		[StructLayout(LayoutKind.Sequential)]
		public struct FILEGROUPDESCRIPTOR
		{
			public uint cItems;
			// Followed by 0 or more FILEDESCRIPTORs
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct FILEDESCRIPTOR
		{
			public uint dwFlags;
			public Guid clsid;
			public int sizelcx;
			public int sizelcy;
			public int pointlx;
			public int pointly;
			public uint dwFileAttributes;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string cFileName;
		}

#if NET8_0_OR_GREATER
		[GeneratedComInterface]
#else
		[ComImport]
#endif
		[Guid("00000121-0000-0000-C000-000000000046")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public partial interface IDropSource
		{
			[PreserveSig]
			int QueryContinueDrag(int fEscapePressed, uint grfKeyState);
			[PreserveSig]
			int GiveFeedback(uint dwEffect);
		}

		[DllImport("shell32.dll")]
		public static extern int SHCreateStdEnumFmtEtc(uint cfmt, FORMATETC[] afmt, out IEnumFORMATETC ppenumFormatEtc);

		[return: MarshalAs(UnmanagedType.Interface)]
		[DllImport("ole32.dll", PreserveSig = false)]
		public static extern IStream CreateStreamOnHGlobal(IntPtr hGlobal, [MarshalAs(UnmanagedType.Bool)] bool fDeleteOnRelease);

		[DllImport("ole32.dll", CharSet = CharSet.Auto, ExactSpelling = true, PreserveSig = false)]
		public static extern void DoDragDrop(System.Runtime.InteropServices.ComTypes.IDataObject dataObject, IDropSource dropSource, int allowedEffects, int[] finalEffect);

#if NET7_0_OR_GREATER
		[LibraryImport("kernel32.dll")]
		public static partial IntPtr GlobalLock(IntPtr hMem);
#else
		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalLock(IntPtr hMem);
#endif

		[return: MarshalAs(UnmanagedType.Bool)]
#if NET7_0_OR_GREATER
		[LibraryImport("kernel32.dll")]
		public static partial bool GlobalUnlock(IntPtr hMem);
#else
		[DllImport("kernel32.dll")]
		public static extern bool GlobalUnlock(IntPtr hMem);
#endif

#if NET7_0_OR_GREATER
		[LibraryImport("kernel32.dll")]
		public static partial IntPtr GlobalSize(IntPtr handle);
#else
		[DllImport("kernel32.dll")]
		public static extern IntPtr GlobalSize(IntPtr handle);
#endif

		/// <summary>
		/// Returns true iff the HRESULT is a success code.
		/// </summary>
		/// <param name="hr">HRESULT to check.</param>
		/// <returns>True iff a success code.</returns>
		public static bool SUCCEEDED(int hr) => 0 <= hr;
	}
}

/// <summary>
/// Definition of the IAsyncOperation COM interface.
/// </summary>
/// <remarks>
/// Pseudo-public because VirtualFileDataObject implements it.
/// </remarks>
[ComImport]
[Guid("3D8B0590-F691-11d2-8EA9-006097DF5BD4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAsyncOperation
{
	void SetAsyncMode([In] int fDoOpAsync);
	void GetAsyncMode([Out] out int pfIsOpAsync);
	void StartOperation([In] IBindCtx pbcReserved);
	void InOperation([Out] out int pfInAsyncOp);
	void EndOperation([In] int hResult, [In] IBindCtx pbcReserved, [In] uint dwEffects);
}
