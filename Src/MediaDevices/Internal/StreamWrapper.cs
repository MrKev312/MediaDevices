using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MediaDevices.Internal;

internal sealed class StreamWrapper(IStream stream, ulong size = 0) : Stream
{
	private IStream stream = stream ?? throw new ArgumentNullException(nameof(stream));
	private IntPtr pLength = Marshal.AllocHGlobal(16);

	private void CheckDisposed() =>
#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(stream == null, stream);
#else
		if (stream == null)
		{
			throw new ObjectDisposedException("StreamWrapper");
		}
#endif


	protected override void Dispose(bool disposing)
	{
		if (stream != null)
		{
			Marshal.ReleaseComObject(stream);
			stream = null;
		}

		if (pLength != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(pLength);
			pLength = IntPtr.Zero;
		}

		base.Dispose(disposing);
	}

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override void Flush() => stream.Commit(0);

	public override long Length
	{
		get
		{
			CheckDisposed();
			return (long)size;
		}
	}

	public override long Position
	{
		get => Seek(0, SeekOrigin.Current);
		set => Seek(value, SeekOrigin.Begin);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		CheckDisposed();

		if (offset < 0 || count < 0 || offset + count > buffer.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(offset));
		}

		byte[] localBuffer = buffer;

		if (offset > 0)
		{
			localBuffer = new byte[count];
		}

		try
		{
			stream.Read(localBuffer, count, pLength);
			int bytesRead = Marshal.ReadInt32(pLength);

			if (offset > 0)
			{
				Array.Copy(localBuffer, 0, buffer, offset, bytesRead);
			}

			return bytesRead;
		}
		catch (Exception ex)
		{
			Trace.WriteLine(ex.ToString());
		}

		return 0;
	}

	public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException("Seek not implemented");

	public override void SetLength(long value)
	{
		CheckDisposed();

		stream.SetSize(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		CheckDisposed();

		if (offset < 0 || count < 0 || offset + count > buffer.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(offset));
		}

		byte[] localBuffer = buffer;

		if (offset > 0)
		{
			localBuffer = new byte[count];
			Array.Copy(buffer, offset, localBuffer, 0, count);
		}

		// workaround for Windows 10 Update 1703 problem 
		// https://social.msdn.microsoft.com/Forums/en-US/7f7a045d-9d9d-4ff4-b8e3-de2d7477a177/windows-10-update-1703-problem-with-wpd-and-mtp?forum=csharpgeneral
		stream.Write(localBuffer, count, pLength);
	}
}
