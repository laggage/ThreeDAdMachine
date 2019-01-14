using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace ThreeDAdMachine.ViewModel
{

    internal enum DataFilePath
    {
        [Description(@"./Cache/ImageListCache.dat")]
        ImageListInfoPath,
        [Description(@"./Cache/VideoListCache.dat")]
        VideoListInfoPath,
        [Description("./Cache/DevicesCache.dat")]
        DevicesInfoPath
    }
    public class AppViewModel:NotificationObject
    {
        #region Constructor 

        public AppViewModel()
        {
            OpenDevicesViewCommand = new DelegateCommand(OpenDevicesView);

            DevicesAndConnectViewModel = new DevicesAndConnectViewModel();
            MediaListViewModel = new MediaListViewModel();
            WarningFlyoutViewModel = new WarningFlyoutViewModel();
        }

        #endregion


        #region Property

        #region Property

        public MediaListViewModel MediaListViewModel { get; set; }

        public WarningFlyoutViewModel WarningFlyoutViewModel { get; set; }

        #endregion

        #region NotifyProperty

        private DevicesAndConnectViewModel _devicesAndConnectViewModel;

        public DevicesAndConnectViewModel DevicesAndConnectViewModel
        {
            get => _devicesAndConnectViewModel;
            set
            {
                if (_devicesAndConnectViewModel == value)
                    return;
                _devicesAndConnectViewModel = value;
                RaisePropertyChanged(nameof(DevicesAndConnectViewModel));
            }
        }

        #endregion

        #endregion


        #region Commands

        public DelegateCommand OpenDevicesViewCommand { get; set; }

        private void OpenDevicesView()
        {
            DevicesAndConnectViewModel.ToggleFlyout();
        }

        #endregion


        #region Method

        #region public  

        public void ShowWarning(string info)
        {
            WarningFlyoutViewModel.ShowWaring(info);
        }

        #endregion

        #endregion
    }
}
