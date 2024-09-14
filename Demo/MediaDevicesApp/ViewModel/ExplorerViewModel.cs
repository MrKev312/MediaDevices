﻿using MediaDeviceApp.Mvvm;

using MediaDevices;

using System.Collections.Generic;

namespace MediaDeviceApp.ViewModel;

public class ExplorerViewModel : BaseViewModel
    {
        MediaDevice device;
        List<ItemViewModel> explorerRoot;

        public ExplorerViewModel()
        { }

        public void Update(MediaDevice device)
        {
            this.device = device;
            MediaDirectoryInfo root = null;
            root = this.device?.GetRootDirectory();
            ExplorerRoot = root != null ? [new(root)] : null;
        }

        public List<ItemViewModel> ExplorerRoot
        {
            get
            {
                return explorerRoot;
            }
            set
            {
                explorerRoot = value;
                NotifyPropertyChanged(nameof(ExplorerRoot));
            }
        }
    }
