using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Communication.Models;
using Communication.Repository;
using MahApps.Metro.Controls;
using MediaProcess.Model;
using Microsoft.Practices.Prism.Commands;
using ThreeDAdMachine.Helper;
using ThreeDAdMachine.View;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;

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
            OpenDeviceControlWindowCommand = new DelegateCommand<Device>(OpenDeviceControlWindow);
            AddMediaToDeviceCommand = new DelegateCommand<Device>(AddMediaToDevice);
            PlayOrPauseMediaCommand = new DelegateCommand(PlayOrPauseMedia);
            AddNewDeviceCommandCommand = new DelegateCommand<string>(AddNewDeviceCommand);
            ConnectOrDisconnectDeviceCommand = new DelegateCommand<FrameworkElement>(ConnectOrDisconnectDevice);
            SendDeviceCommandCommand = new DelegateCommand<DeviceCommand?>(SendDeviceCommand);
            //listen event
            FlyoutClosed += DevicesAndConnectViewModel_FlyoutClosed;
            //initialize property and fields
            IsEditDeviceViewOpen = false;
            IsOpen = false;
            _deviceControlWindow = null;
        }

        #endregion


        #region Field

        private bool _isCurrentDeviceNew;
        private MetroWindow _deviceControlWindow;
        private DeviceCommandRepository _deviceCommandRepository;

        #endregion


        #region Property

        public string NewDeviceName { get; set; }

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

        #region ProcessingDevice

        private Device _processingDevice;

        public Device ProcessingDevice
        {
            get => _processingDevice;
            set
            {
                if (value == _processingDevice) return;
                _processingDevice = value;
                RaisePropertyChanged(nameof(ProcessingDevice));

                if(value != null) ProcessingDeviceMediaList = LoadMediaListFromDevice(value);
            }
        }

        #endregion

        #region ProcessingDeviceMediaList

        private ObservableCollection<MediaBaseModel> _processingDeviceMediaList;

        public ObservableCollection<MediaBaseModel> ProcessingDeviceMediaList
        {
            get => _processingDeviceMediaList;
            set
            {
                if (value == _processingDeviceMediaList) return;
                _processingDeviceMediaList = value;
                RaisePropertyChanged(nameof(ProcessingDeviceMediaList));
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

        #region DeviceComands

        private ObservableCollection<DeviceCommand> _deviceCommands;

        public ObservableCollection<DeviceCommand> DeviceCommands
        {
            get => _deviceCommands;
            set
            {
                if (value == _deviceCommands) return;
                _deviceCommands = value;
                RaisePropertyChanged(nameof(DeviceCommands));
            }
        }

        #endregion

        #endregion

        #region Command

        public DelegateCommand<Device> OpenEditDeviceViewCommand { get; set; }
        public DelegateCommand OpenAddDeviceViewCommand { get; set; }
        public DelegateCommand AddDeviceCommand { get; set; }
        public DelegateCommand<Device> OpenDeviceControlWindowCommand { get; set; }
        public DelegateCommand<Device> AddMediaToDeviceCommand { get; set; }
        public DelegateCommand PlayOrPauseMediaCommand { get; set; }
        public DelegateCommand<string> AddNewDeviceCommandCommand { get; set; }
        public DelegateCommand<FrameworkElement> ConnectOrDisconnectDeviceCommand { get; set; }
        public DelegateCommand<DeviceCommand?> SendDeviceCommandCommand { get; set; }

        #endregion

        #endregion


        #region Method

        /// <summary>
        /// 从指定设备对象中加载出MediaList
        /// </summary>
        /// <param name="d">指定的设备</param>
        /// <returns></returns>
        private ObservableCollection<MediaBaseModel> LoadMediaListFromDevice(Device d)=>  new ObservableCollection<MediaBaseModel>(d?.MediaList ?? throw new NullReferenceException());

        /// <summary>
        /// 检查Devices中是否存在与device相同的对象
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private bool ContainsSameDevice(Device device) => Devices.Any((d) => d == device);

        private void OpenEditDeviceView(Device device)
        {
            if (device == null) return;
            ProcessingDevice = device;
            _isCurrentDeviceNew = false;
            IsEditDeviceViewOpen = true;
        }

        private void OpenAddDeviceView()
        {
            ProcessingDevice = null;
            ProcessingDevice = new Device("未命名","127.0.0.1");
            _isCurrentDeviceNew = true;
            IsEditDeviceViewOpen = true;
        }

        /// <summary>
        /// add a device to the device list
        /// 向设备列表中添加一个新设备,该设备不能和已经存在的设备重复
        /// </summary>
        private void AddDeviceToDeviceList()
        {
            if (ProcessingDevice == null || !_isCurrentDeviceNew) return;
            Devices = Devices ?? new ObservableCollection<Device>();
            if (ContainsSameDevice(ProcessingDevice))
            {
                AppHelper.ShowMessageFlyout("已存在相同ip的设备,请不要重复添加!");
                return;
            }
            Devices.Add(ProcessingDevice);
        }

        /// <summary>
        /// Open device control window,in a app lifetime,this window can only have one
        /// 打开设备控制窗口,在应用程序生命周期中该窗口最多只能存在一个
        /// </summary>
        private void OpenDeviceControlWindow(Device device)
        {
            if (_deviceControlWindow != null)
            {   //将窗口显示到最前面并激活窗口
                _deviceControlWindow.Topmost = true; _deviceControlWindow.Topmost = false;
                _deviceControlWindow.Activate();
                return;
            }
            _deviceControlWindow = new DeviceControlWindow();
            DeviceCommands = DeviceCommands ?? new ObservableCollection<DeviceCommand>();
            _deviceCommandRepository = _deviceCommandRepository ?? new DeviceCommandRepository();

            DeviceCommands = _deviceCommandRepository.Load();
            _deviceControlWindow.Closed += (s, e) =>
            {
                _deviceControlWindow = null;
                //释放不在需要的资源
                _deviceCommandRepository = null;
                DeviceCommands = null;
            };
            
            _deviceControlWindow.DataContext = this;
            _deviceControlWindow.Show();
        }

        /// <summary>
        /// 获取 期望被添加到设备 图片/视频列表的 图片/视频,如果为null,则表示没有选中的 图片/视频
        /// </summary>
        /// <returns></returns>
        private MediaBaseModel GetTargetMedia()=> (MediaBaseModel)AppHelper.AppViewModel.MediaListViewModel.EditingImage ??
                                                  AppHelper.AppViewModel.MediaListViewModel.EditingVideo;

        /// <summary>
        /// 向设备的 图片/视频 列表中添加一个 图片/视频 
        /// </summary>
        /// <param name="device">target device</param>
        private void AddMediaToDevice(Device device)
        {
            MediaBaseModel targetMedia = GetTargetMedia();
            if (targetMedia == null)
            {
                AppHelper.ShowMessageFlyout("请选择一个图片或视频!!");
                return;
            }

            device.MediaList.Add(targetMedia);
            //如果目标设备是当前正在处理的设备,则需要更新当前设备的播放列表来更新前台UI
            if (device == ProcessingDevice) ProcessingDeviceMediaList.Add(targetMedia);
        }

        /// <summary>
        /// 与设备连接或断开连接
        /// </summary>
        /// <param name="btn">需要更新的ui控件</param>
        private void ConnectOrDisconnectDevice(FrameworkElement btn)
        {
            if (ProcessingDevice == null)
            {
                MessageBox.Show("请先选中一个设备!", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DeviceState state = ProcessingDevice.State;
            switch (state)
            {
                case DeviceState.OnLine:
                    ProcessingDevice.DisConnectWithDevice();
                    break;
                case DeviceState.OffLine:
                {
                    if (!ProcessingDevice.ConnectToDevice())
                        MessageBox.Show("设备连接失败,请检查设备ip和端口号是否正确", "连接设备失败",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
            }

            //更新绑定源数据到UI界面
            btn?.GetBindingExpression(ContentControl.ContentProperty)?.UpdateTarget();
            FrameworkElement fe = (VisualTreeHelper.GetParent(btn??throw new ArgumentNullException())).FindChild<FrameworkElement>("ConnectionState");
            fe.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }

        /// <summary>
        /// 向指定设备发送指令
        /// </summary>
        /// <param name="command"></param>
        private void SendDeviceCommand(DeviceCommand? command)
        {
            if (command == null)
            {
                MessageBox.Show("命令解析错误", "警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ProcessingDevice == null)
            {
                MessageBox.Show("请先选中一个设备", "警告", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ProcessingDevice.State == DeviceState.OffLine)
            {
                MessageBox.Show("设备没有连接,请先连接设备", "警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if(!ProcessingDevice.SendCommand((DeviceCommand)command))
                MessageBox.Show("发送指令失败", "警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 播放/暂停 
        /// </summary>
        private void PlayOrPauseMedia()
        {
            if (ProcessingDevice == null)
            {
                MessageBox.Show("没有可用的设备", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ProcessingDevice.State == DeviceState.OffLine)
            {
                MessageBox.Show("设备未连接,无法播放!!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


        }

        /// <summary>
        /// 添加一个设备指令
        /// </summary>
        /// <param name="commandCode"></param>
        private void AddNewDeviceCommand(string commandCode)
        {
            if (string.IsNullOrEmpty(NewDeviceName)||
                string.IsNullOrEmpty(commandCode))
            {
                MessageBox.Show("指令的名称或代码不可以为空!!", "添加指令失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(commandCode, out int code))
            {
                MessageBox.Show("指令代码必须是一个10进制整数!!", "添加指令失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_deviceCommandRepository == null) throw new ArgumentException();
            _deviceCommandRepository.Add(NewDeviceName, code);
            DeviceCommands = _deviceCommandRepository.Load();
        }

        /// <summary>
        /// DeviceAndConnectView(a fly-out window) Closed event handler,to close EditDeviceView if it was opened;
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
