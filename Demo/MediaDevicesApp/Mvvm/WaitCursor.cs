using System;
using System.Windows.Input;

namespace MediaDevicesApp.Mvvm;

public class WaitCursor : IDisposable
{
	private readonly Cursor previousCursor;

	public WaitCursor()
	{
		previousCursor = Mouse.OverrideCursor;
		Mouse.OverrideCursor = Cursors.Wait;
	}

	public void Dispose()
	{
		Mouse.OverrideCursor = previousCursor;

		GC.SuppressFinalize(this);
	}
}
