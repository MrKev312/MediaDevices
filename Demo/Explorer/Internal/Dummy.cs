using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace ExplorerCtrl.Internal;

public class Dummy : IExplorerItem
{
	public event EventHandler<RefreshEventArgs> Refresh;

	public IEnumerable<IExplorerItem> Children => null;

	public DateTime? CreationDate => null;

	public bool HasChildren => false;

	public string Name { get => "Dummy"; set { } }

	public string FullName => "Dummy";

	public string Link => "";

	public long Size => 0;

	public ImageSource Icon => null;

	public bool IsDirectory => true;

	public ExplorerItemType Type => ExplorerItemType.Directory;

	public void DoRefresh(bool recursive) => Refresh?.Invoke(this, new RefreshEventArgs(recursive));

	public void Pull(string path, Stream stream) => throw new NotImplementedException();

	public void Push(Stream stream, string path) => throw new NotImplementedException();

	public void CreateFolder(string path) => throw new NotImplementedException();

	public bool Equals(IExplorerItem other) => FullName == other.FullName;
}
