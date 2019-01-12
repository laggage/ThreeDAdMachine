using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace ThreeDAdMachine.ViewModel
{
    public class MediaSampleViewModel:NotificationObject
    {
        #region NofifyProperty

        #region IsActive

        private bool _isActive = false;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                    return;
                _isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }

        #endregion

        #region ReportProgressDetail

        private bool _reportProgressDetail = false;

        public bool ReportProgressDetail
        {
            get => _reportProgressDetail;
            set
            {
                if (_reportProgressDetail == value)
                    return;
                _reportProgressDetail = value;
                RaisePropertyChanged(nameof(ReportProgressDetail));
            }
        }

        #endregion

        #region ProgressValue   

        private double _progressValue;

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue == value)
                    return;
                _progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }


        #endregion

        #endregion
    }
}
