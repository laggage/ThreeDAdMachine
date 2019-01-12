using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json.Serialization;

namespace ThreeDAdMachine.ViewModel
{
    public class FlyoutBaseViewModel:NotificationObject
    {
        #region Constructor 

        public FlyoutBaseViewModel()
        {
            //initialize command    
            OpenFlyoutCommand = new DelegateCommand(OpenFlyout);
            CloseFlyoutCommand = new DelegateCommand(CloseFlyout);
            //initialize variable
            IsOpen = false;
        }

        #endregion


        #region Field

        public event EventHandler FlyoutClosed;

        #endregion


        #region Property

        #region NotifyProperty

        #region IsFlyout

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (value == _isOpen)
                    return;
                _isOpen = value;

                if (value == false) FlyoutClosed?.Invoke(this, null);

                RaisePropertyChanged(nameof(IsOpen));
            }
        }

        #endregion

        #endregion

        #region Command

        public DelegateCommand OpenFlyoutCommand { get; set; }
        public DelegateCommand CloseFlyoutCommand { get; set; }

        #endregion

        #endregion


        #region Method

        public virtual void ToggleFlyout() => IsOpen = !IsOpen;
        private void CloseFlyout() => IsOpen = false;
        private void OpenFlyout() => IsOpen = true;

        #endregion
    }
}
