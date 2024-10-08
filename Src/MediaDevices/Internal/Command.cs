﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MediaDevices.Internal;

internal sealed class Command
{
	private readonly IPortableDeviceValues values;
	private IPortableDeviceValues? result;

	private Command(PropertyKey commandKey)
	{
		values = (IPortableDeviceValues)new PortableDeviceValues();
		values.SetGuidValue(ref WPD.PROPERTY_COMMON_COMMAND_CATEGORY, ref commandKey.fmtid);
		values.SetUnsignedIntegerValue(ref WPD.PROPERTY_COMMON_COMMAND_ID, commandKey.pid);
	}

	public static Command Create(PropertyKey commandKey) => new(commandKey);

	public void Add(PropertyKey key, Guid value) => values.SetGuidValue(ref key, ref value);

	public void Add(PropertyKey key, int value) => values.SetSignedIntegerValue(ref key, value);

	public void Add(PropertyKey key, uint value) => values.SetUnsignedIntegerValue(ref key, value);

	public void Add(PropertyKey key, IPortableDevicePropVariantCollection value) => values.SetIPortableDevicePropVariantCollectionValue(ref key, value);

	public void Add(PropertyKey key, IEnumerable<int> values)
	{
		IPortableDevicePropVariantCollection col = (IPortableDevicePropVariantCollection)new PortableDevicePropVariantCollection();
		foreach (int value in values)
		{
			PropVariantFacade var = PropVariantFacade.IntToPropVariant(value);
			col.Add(ref var.Value);
		}

		this.values.SetIPortableDevicePropVariantCollectionValue(ref key, col);
	}

	public void Add(PropertyKey key, string value) => values.SetStringValue(ref key, value);

	//public void Add(PropertyKey key, byte[] buffer, int size)
	//{
	//    Marshal..
	//    this.values.SetBufferValue(key, ref buffer, (uint)size);
	//}

	public Guid GetGuid(PropertyKey key)
	{
		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		result.GetGuidValue(ref key, out Guid value);
		return value;
	}

	public int GetInt(PropertyKey key)
	{
		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		result.GetSignedIntegerValue(ref key, out int value);
		return value;
	}

	public string GetString(PropertyKey key)
	{
		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		result.GetStringValue(ref key, out string value);
		return value;
	}

	public IEnumerable<PropVariantFacade> GetPropVariants(PropertyKey key)
	{
		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		result.GetIUnknownValue(ref key, out object obj);
		IPortableDevicePropVariantCollection col = (IPortableDevicePropVariantCollection)obj;

		uint count = 0;
		col.GetCount(ref count);
		for (uint i = 0; i < count; i++)
		{
			PropVariantFacade val = new();
			col.GetAt(i, ref val.Value);
			yield return val;
		}
	}

	public bool Has(PropertyKey key)
	{
		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		uint count = 0;
		result.GetCount(ref count);
		for (uint i = 0; i < count; i++)
		{
			PropertyKey k = new();
			PropVariant v = new();
			result.GetAt(i, ref k, ref v);
			if (key == k)
			{
				return true;
			}
		}

		return false;
	}

	public bool Send(IPortableDevice device)
	{
		device.SendCommand(0, values, out result);

		result.GetErrorValue(ref WPD.PROPERTY_COMMON_HRESULT, out int error);
		switch ((HResult)error)
		{
			case HResult.S_OK:
				return true;
			case HResult.E_NOT_IMPLEMENTED:
				Debug.WriteLine("Command not implemented!");
				return false;
			default:
				throw new InvalidOperationException($"Error {error:X}");
		}
	}

	[Conditional("COMTRACE")]
	public void WriteResults()
	{

		if (result == null)
		{
			throw new InvalidOperationException("Command not sent!");
		}

		ComTrace.WriteObject(result);
	}
}
