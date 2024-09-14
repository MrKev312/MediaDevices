using System;

namespace ExplorerCtrl;

public class RefreshEventArgs : EventArgs
{
	public RefreshEventArgs(bool recursive)
	{
		Recursive = recursive;
	}

	public bool Recursive { get; private set; }
}
