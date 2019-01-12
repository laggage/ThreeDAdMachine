using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;

namespace ThreeDAdMachine.ViewModel
{
    internal enum MediaWaringType
    {
        [Description("当前没有选中任何图片或视频")]
        NoMediaWaring,
        [Description("列表中已存在相同的图片或视频项,不需要重复添加")]
        ExistSameMediaWaring
    }

    internal enum DeviceWaringType
    {
        [Description("当前没有可用的设备")]
        NoneUsefulDeviceWarning,
        [Description("列表中已存在相同的设备,不需要重复添加")]
        ExistSameDeviceWarning
    }

    public class WarningFlyoutViewModel:FlyoutBaseViewModel
    {
        #region Constructor 

        public WarningFlyoutViewModel(){ }

        #endregion

        #region Property

        #region NotifyProperty

        #region WarningInfo 

        private string _warningInfo;

        public string WarningInfo
        {
            get => _warningInfo;
            set
            {
                if (_warningInfo == value)
                    return;
                _warningInfo = value;
                RaisePropertyChanged(nameof(WarningInfo));
            }
        }

        #endregion

        #endregion

        #endregion

        #region Method

        #region public

        public void ShowWaring(string warningInfo)
        {
            WarningInfo = warningInfo;
            ToggleFlyout();
        }

        #endregion

        #endregion
    }
}
