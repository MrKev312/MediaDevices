using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;

namespace MediaDevices.Internal;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// The Facade is necessary because structs used in using are readonly and can not be filled with ref or out.
/// </remarks>
internal sealed class PropVariantFacade : IDisposable
{
	// cannot be a property because it will be filled by reference
	public PropVariant Value;

	public PropVariantFacade() => Value = new PropVariant();

	public void Dispose()
	{
		// clear only if filled
		if (Value.vt != 0)
		{
			try
			{
				// clear propvariant clears also included objects like strings
				int HRESULT = NativeMethods.PropVariantClear(ref Value);
				if (HRESULT != 0)
				{
					Debug.WriteLine($"PropVariantClear failed with HRESULT 0x{HRESULT:X}");
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError(ex.ToString());
			}
		}
	}

	public PropVariantType VariantType => Value.vt;

	public string ToDebugString()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			int error = ToError();
			string name = Enum.GetName(typeof(HResult), error) ?? error.ToString("X", CultureInfo.InvariantCulture);
			return $"Error: {name}";
		}

		return ToString();
	}

	public override string ToString()
	{
		switch (Value.vt)
		{
			case PropVariantType.VT_LPSTR:
				return Marshal.PtrToStringAnsi(Value.ptrVal);

			case PropVariantType.VT_LPWSTR:
				return Marshal.PtrToStringUni(Value.ptrVal);

			case PropVariantType.VT_BSTR:
				return Marshal.PtrToStringBSTR(Value.ptrVal);

			case PropVariantType.VT_CLSID:
				return ToGuid().ToString();

			case PropVariantType.VT_DATE:
				return ToDate().ToString(CultureInfo.InvariantCulture);

			case PropVariantType.VT_BOOL:
				return ToBool().ToString();

			case PropVariantType.VT_INT:
			case PropVariantType.VT_I1:
			case PropVariantType.VT_I2:
			case PropVariantType.VT_I4:
				return ToInt().ToString(CultureInfo.InvariantCulture);

			case PropVariantType.VT_UINT:
			case PropVariantType.VT_UI1:
			case PropVariantType.VT_UI2:
			case PropVariantType.VT_UI4:
				return ToUInt().ToString(CultureInfo.InvariantCulture);

			case PropVariantType.VT_I8:
				return ToLong().ToString(CultureInfo.InvariantCulture);

			case PropVariantType.VT_UI8:
				return ToUlong().ToString(CultureInfo.InvariantCulture);

			case PropVariantType.VT_ERROR:
				Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
				return "";

			default:
				return "";
		}
	}

	public int ToInt()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return 0;
		}

		if (Value.vt is not PropVariantType.VT_INT
		 and not PropVariantType.VT_I1
		 and not PropVariantType.VT_I2
		 and not PropVariantType.VT_I4)
		{
			throw new InvalidOperationException($"ToInt does not work for value type {Value.vt}");
		}

		return Value.intVal;
	}

	public uint ToUInt()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return 0;
		}

		if (Value.vt is not PropVariantType.VT_UINT
		 and not PropVariantType.VT_UI1
		 and not PropVariantType.VT_UI2
		 and not PropVariantType.VT_UI4)
		{
			throw new InvalidOperationException($"ToUInt does not work for value type {Value.vt}");
		}

		return Value.uintVal;
	}

	public long ToLong()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return 0;
		}

		if (Value.vt is not PropVariantType.VT_INT
		 and not PropVariantType.VT_I1
		 and not PropVariantType.VT_I2
		 and not PropVariantType.VT_I4
		 and not PropVariantType.VT_I8)
		{
			throw new InvalidOperationException($"ToLong does not work for value type {Value.vt}");
		}

		return Value.longVal;
	}

	public ulong ToUlong()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return 0;
		}

		if (Value.vt is not PropVariantType.VT_UINT
		 and not PropVariantType.VT_UI1
		 and not PropVariantType.VT_UI2
		 and not PropVariantType.VT_UI4
		 and not PropVariantType.VT_UI8)
		{
			throw new InvalidOperationException($"ToUlong does not work for value type {Value.vt}");
		}

		return Value.ulongVal;
	}

	public DateTime ToDate()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return new DateTime();
		}

		if (Value.vt != PropVariantType.VT_DATE)
		{
			throw new InvalidOperationException($"ToDate does not work for value type {Value.vt}");
		}

		return DateTime.FromOADate(Value.dateVal);
	}

	public bool ToBool()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return false;
		}

		if (Value.vt != PropVariantType.VT_BOOL)
		{
			throw new InvalidOperationException($"ToBool does not work for value type {Value.vt}");
		}

		return Value.boolVal != 0;
	}

	public Guid ToGuid()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return new Guid();
		}

		if (Value.vt != PropVariantType.VT_CLSID)
		{
			throw new InvalidOperationException($"ToGuid does not work for value type {Value.vt}");
		}

		return Marshal.PtrToStructure<Guid>(Value.ptrVal);
	}

#if !NETCOREAPP
	[HandleProcessCorruptedStateExceptions]
#endif
	[SecurityCritical]
	public byte[] ToByteArray()
	{
		if (Value.vt == PropVariantType.VT_ERROR)
		{
			Debug.WriteLine($"VT_ERROR: 0x{Value.errorCode:X}");
			return null;
		}

		if (Value.vt != (PropVariantType.VT_VECTOR | PropVariantType.VT_UI1))
		{
			throw new InvalidOperationException($"ToByteArray does not work for value type {Value.vt}");
		}

		int size = (int)Value.dataVal.cData;
		byte[] managedArray = new byte[size];

		// bug fixed with manual COM wrapper classes
		Marshal.Copy(Value.dataVal.pData, managedArray, 0, size);
		return managedArray;
	}

	public int ToError()
	{
		if (Value.vt != PropVariantType.VT_ERROR)
		{
			return 0;
		}

		return Value.errorCode;
	}

	public static PropVariantFacade StringToPropVariant(string value)
	{
		PropVariantFacade pv = new();
		pv.Value.vt = PropVariantType.VT_LPWSTR;
		// Hack, see GetString
		pv.Value.ptrVal = Marshal.StringToCoTaskMemUni(value);
		return pv;
	}

	//public static PropVariantFacade UIntToPropVariant(uint value)
	//{
	//    PropVariantFacade pv = new PropVariantFacade();
	//    pv.Value.vt = PropVariantType.VT_UI4;
	//    pv.Value.inner.ulVal = value;
	//    return pv;
	//}

	public static PropVariantFacade IntToPropVariant(int value)
	{
		PropVariantFacade pv = new();
		pv.Value.vt = PropVariantType.VT_INT;
		pv.Value.intVal = value;
		return pv;
	}

	public static PropVariantFacade DateTimeToPropVariant(DateTime value)
	{
		PropVariantFacade pv = new();
		pv.Value.vt = PropVariantType.VT_DATE;
		pv.Value.dateVal = value.ToOADate();
		return pv;
	}

	public static implicit operator string(PropVariantFacade val)
	{
		return val.ToString();
	}

	public static implicit operator bool(PropVariantFacade val)
	{
		return val.ToBool();
	}

	public static implicit operator DateTime(PropVariantFacade val)
	{
		return val.ToDate();
	}

	public static implicit operator Guid(PropVariantFacade val)
	{
		return val.ToGuid();
	}

	public static implicit operator int(PropVariantFacade val)
	{
		return val.ToInt();
	}

	public static implicit operator byte(PropVariantFacade val)
	{
		return (byte)val.ToUInt();
	}

	public static implicit operator ulong(PropVariantFacade val)
	{
		return val.ToUlong();
	}

	public static implicit operator byte[](PropVariantFacade val)
	{
		return val.ToByteArray();
	}

	private static class NativeMethods
	{
		[DllImport("ole32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern public int PropVariantClear(ref PropVariant val);
	}
}
