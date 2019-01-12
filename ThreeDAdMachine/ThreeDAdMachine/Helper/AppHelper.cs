using ThreeDAdMachine.ViewModel;

namespace ThreeDAdMachine.Helper
{
    public static class AppHelper
    {
        public static AppViewModel AppViewModel => (AppViewModel) App.Current.MainWindow?.DataContext;
    }
}
