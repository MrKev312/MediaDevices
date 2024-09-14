using MediaDeviceApp.Mvvm;

using MediaDevices;

using System.Collections.Generic;
using System.Linq;

namespace MediaDeviceApp.ViewModel;

public class StorageViewModel : BaseViewModel
    {
        MediaDevice device;
        private List<string> storages;
        private string selectedStorage;
        private MediaStorageInfo mediaStorageInfo;

        public StorageViewModel()
        { }

        public void Update(MediaDevice device)
        {
            this.device = device;
            Storages = this.device?.FunctionalObjects(FunctionalCategory.Storage)?.ToList();
            SelectedStorage = Storages?.FirstOrDefault();
        }
        
        public List<string> Storages
        {
            get
            {
                return storages;
            }
            set
            {
                if (storages != value)
                {
                    storages = value;
                    NotifyPropertyChanged(nameof(Storages));
                }
            }
        }

        public string SelectedStorage
        {
            get
            {
                return selectedStorage;
            }
            set
            {
                if (selectedStorage != value)
                {
                    selectedStorage = value;
                    if (!string.IsNullOrEmpty(selectedStorage))
                    {
                        mediaStorageInfo = device?.GetStorageInfo(selectedStorage);
                    }
                    else
                    {
                        mediaStorageInfo = null;
                    }

                    NotifyAllPropertiesChanged();
                }
            }
        }

        public MediaStorageInfo Info
        { 
            get
            {
                return mediaStorageInfo;
            }
        }

        public StorageType Type
        {
            get
            {
                return mediaStorageInfo?.Type ?? StorageType.Undefined;
            }
        }

        public string FileSystemType
        {
            get
            {
                return mediaStorageInfo?.FileSystemType;
            }
        }

        public ulong Capacity
        {
            get
            {
                return mediaStorageInfo?.Capacity ?? 0;
            }
        }

        public ulong FreeSpaceInBytes
        {
            get
            {
                return mediaStorageInfo?.FreeSpaceInBytes ?? 0;
            }
        }

        public ulong FreeSpaceInObjects
        {
            get
            {
                return mediaStorageInfo?.FreeSpaceInObjects ?? 0;
            }
        }

        public string Description
        {
            get
            {
                return mediaStorageInfo?.Description;
            }
        }

        public string SerialNumber
        {
            get
            {
                return mediaStorageInfo?.SerialNumber;
            }
        }

        public ulong MaxObjectSize
        {
            get
            {
                return mediaStorageInfo?.MaxObjectSize ?? 0;
            }
        }

        public ulong CapacityInObjects
        {
            get
            {
                return mediaStorageInfo?.CapacityInObjects ?? 0;
            }
        }

        public StorageAccessCapability AccessCapability
        {
            get
            {
                return mediaStorageInfo?.AccessCapability ?? 0;
            }
        }
    }
