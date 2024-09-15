using System;

namespace MediaDevices.Internal;

internal struct PropertyKey : IEquatable<PropertyKey>
{
	public Guid fmtid;
	public uint pid;

	public static bool operator ==(PropertyKey obj1, PropertyKey obj2)
	{
		return obj1.fmtid == obj2.fmtid && obj1.pid == obj2.pid;
	}

	public static bool operator !=(PropertyKey obj1, PropertyKey obj2)
	{
		return obj1.fmtid != obj2.fmtid || obj1.pid != obj2.pid;
	}

	public override readonly bool Equals(object? obj)
	{
		PropertyKey? pk = obj as PropertyKey?;
		if (pk == null)
		{
			return false;
		}
		return fmtid == pk.Value.fmtid && pid == pk.Value.pid;
	}

	public readonly bool Equals(PropertyKey other) => fmtid == other.fmtid && pid == other.pid;

	public override readonly int GetHashCode() => fmtid.GetHashCode() ^ pid.GetHashCode();
}
