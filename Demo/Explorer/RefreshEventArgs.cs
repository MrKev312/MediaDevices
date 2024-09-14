using System;

namespace ExplorerCtrl;

public class RefreshEventArgs(bool recursive) : EventArgs
{
	public bool Recursive { get; private set; } = recursive;
}
