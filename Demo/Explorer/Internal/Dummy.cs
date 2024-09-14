using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace ExplorerCtrl.Internal;

public class Dummy : IExplorerItem
    {
        public event EventHandler<RefreshEventArgs> Refresh;

        public IEnumerable<IExplorerItem> Children { get { return null; } }

        public DateTime? CreationDate { get { return null; } }

        public bool HasChildren { get { return false; } }

        public string Name { get { return "Dummy"; } set { } }

        public string FullName { get { return "Dummy"; } }

        public string Link { get { return ""; } }

        public long Size { get { return 0; } }

        public ImageSource Icon { get { return null; } }

        public bool IsDirectory { get { return true; } }

        public ExplorerItemType Type { get { return ExplorerItemType.Directory; } }

        public void DoRefresh(bool recursive)
        {
            Refresh?.Invoke(this, new RefreshEventArgs(recursive));
        }
        
        public void Pull(string path, Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Push(Stream stream, string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IExplorerItem other)
        {
            return FullName == other.FullName;
        }
    }
