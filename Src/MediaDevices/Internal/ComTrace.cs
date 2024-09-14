using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MediaDevices.Internal;

// to enable COM traces add "COMTRACE" to the Build Conditional compilation symbols of the MediaDevice project.

internal static class ComTrace
{
	private static readonly List<FieldInfo> pkeyFields;
	private static readonly List<FieldInfo> guidFields;

	static ComTrace()
	{
		pkeyFields = typeof(WPD).GetFields().Where(f => f.FieldType == typeof(PropertyKey)).ToList();
		guidFields = typeof(WPD).GetFields().Where(f => f.FieldType == typeof(Guid)).ToList();
	}

	public static FieldInfo FindPropertyKeyField(PropertyKey key) =>
		//return pkeyFields.SingleOrDefault(i => ((PropertyKey)i.GetValue(null)) == key);
		pkeyFields.FirstOrDefault(i => ((PropertyKey)i.GetValue(null)) == key);

	public static FieldInfo FindGuidField(Guid guid) =>
		//return guidFields.SingleOrDefault(i => ((Guid)i.GetValue(null)) == guid);
		guidFields.FirstOrDefault(i => ((Guid)i.GetValue(null)) == guid);

	[Conditional("COMTRACE")]
	public static void WriteObject(IPortableDeviceValues values) => InternalWriteObject(values);

	[Conditional("COMTRACE")]
	public static void WriteObject(IPortableDeviceProperties deviceProperties, string objectId)
	{
		deviceProperties.GetSupportedProperties(objectId, out IPortableDeviceKeyCollection keys);

		deviceProperties.GetValues(objectId, keys, out IPortableDeviceValues values);

		InternalWriteObject(values);
	}

	[Conditional("COMTRACE")]
	private static void InternalWriteObject(IPortableDeviceValues values)
	{
		string func = new StackTrace().GetFrame(2).GetMethod().Name;
		Trace.WriteLine($"############################### {func}");
		uint num = 0;

		values.GetCount(ref num);
		for (uint i = 0; i < num; i++)
		{
			PropertyKey key = new();
			PropVariantFacade val = new();
			values.GetAt(i, ref key, ref val.Value);

			string fieldName = string.Empty;
			FieldInfo propField = FindPropertyKeyField(key);
			if (propField != null)
			{
				fieldName = propField.Name;
			}
			else
			{
				FieldInfo guidField = FindGuidField(key.fmtid);
				if (guidField != null)
				{
					fieldName = $"{guidField.Name}, {key.pid}";
				}
				else
				{
					fieldName = $"{key.fmtid}, {key.pid}";
				}
			}

			switch (val.VariantType)
			{
				case PropVariantType.VT_CLSID:
					Trace.WriteLine($"##### {fieldName} = {FindGuidField(val.ToGuid())?.Name ?? val.ToString()}");
					break;
				default:
					Trace.WriteLine($"##### {fieldName} = {val.ToDebugString()}");
					break;
			}
		}
	}

	[Conditional("COMTRACE")]
	public static void WriteObject(IPortableDevicePropVariantCollection collection)
	{
		Trace.WriteLine("###############################");
		uint num = 0;
		collection.GetCount(ref num);
		for (uint index = 0; index < num; index++)
		{
			using PropVariantFacade val = new();
			collection.GetAt(index, ref val.Value);

			Trace.WriteLine($"##### {val.ToDebugString()}");
		}
	}

	[Conditional("COMTRACE")]
	public static void WriteObject(IPortableDeviceKeyCollection collection)
	{
		Trace.WriteLine("###############################");
		uint num = 0;
		collection.GetCount(ref num);
		for (uint index = 0; index < num; index++)
		{
			PropertyKey key = new();
			collection.GetAt(index, ref key);

			PropertyKeys propertyKey = key.GetEnumFromAttrKey<PropertyKeys>();
			Trace.WriteLine($"##### {propertyKey}");
		}
	}
}
