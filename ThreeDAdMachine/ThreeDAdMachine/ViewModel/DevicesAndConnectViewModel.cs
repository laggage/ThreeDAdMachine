using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace ThreeDAdMachine.ViewModel
{
    public class DevicesAndConnectViewModel:FlyoutBaseViewModel
    {
        #region Constructor

        public DevicesAndConnectViewModel()
        {
            //initialize command
            OpenEditDeviceViewCommand = new DelegateCommand<Device>(OpenEditDeviceView);
            OpenAddDeviceViewCommand = new DelegateCommand(OpenAddDeviceView);
            AddDeviceCommand = new DelegateCommand(AddDeviceToDeviceList);
            //listen event
            FlyoutClosed += DevicesAndConnectViewModel_FlyoutClosed;
            //initialize property
            IsEditDeviceViewOpen = false;
            IsOpen = false;
        }

        #endregion


        #region Field

        private bool _isCurrentDeviceNew;

        #endregion


        #region Property

        #region NotifyProperty

        #region Devices

        private ObservableCollection<Device> _devices;

        public ObservableCollection<Device> Devices
        {
            get => _devices;
            set
            {
                if (value == _devices)
                    return;
                _devices = value;
                RaisePropertyChanged(nameof(Devices));
            }
        }

        #endregion

        #region CurrentDevice

        private Device _currentDevice;

        public Device CurrentDevice
        {
            get => _currentDevice;
            set
            {
                if (value == _currentDevice)
                    return;
                _currentDevice = value;
                RaisePropertyChanged(nameof(CurrentDevice));
            }
        }


        #endregion

        #region IsEditDeviceViewOpen

        private bool _isEditDeviceViewOpen;

        public bool IsEditDeviceViewOpen
        {
            get => _isEditDeviceViewOpen;
            set
            {
                if (value == _isEditDeviceViewOpen)
                    return;
                _isEditDeviceViewOpen = value;
                RaisePropertyChanged(nameof(IsEditDeviceViewOpen));
            }
        }

        #endregion

        #endregion

        #region Command

        public DelegateCommand<Device> OpenEditDeviceViewCommand { get; set; }
        public DelegateCommand OpenAddDeviceViewCommand { get; set; }
        public DelegateCommand AddDeviceCommand { get; set; }

        #endregion

        #endregion


        #region Method

        private void OpenEditDeviceView(Device device)
        {
            if (device == null) return;
            CurrentDevice = device;
            _isCurrentDeviceNew = false;
            IsEditDeviceViewOpen = true;
        }

        private void OpenAddDeviceView()
        {
            CurrentDevice = null;
            CurrentDevice = new Device();
            _isCurrentDeviceNew = true;
            IsEditDeviceViewOpen = true;
        }

        private void AddDeviceToDeviceList()
        {
            if (CurrentDevice != null && _isCurrentDeviceNew)
            {
                Devices = Devices ?? new ObservableCollection<Device>();
                Devices.Add(CurrentDevice);
            }
        }

        /// <summary>
        /// DeviceAndConnectView Closed event handler,to close EditDeviceView if it was opened;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevicesAndConnectViewModel_FlyoutClosed(object sender, EventArgs e)
        {
            if (IsEditDeviceViewOpen) IsEditDeviceViewOpen = false;
        }

        #endregion
    }
}
