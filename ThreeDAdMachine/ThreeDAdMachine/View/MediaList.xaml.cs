using System.Windows;
using System.Windows.Controls;

namespace ThreeDAdMachine.View
{
    /// <summary>
    /// Interaction logic for MediaList.xaml
    /// </summary>
    public partial class MediaList : UserControl
    {
        public MediaList()
        {
            InitializeComponent();
        }

        private void MediaListHamburger_OptionsItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs e)
        {//Without this handler,the item can not response to its command,due to user clicked option items,but no response
         //so this handler can't be deleted !!!
            var clickedItem = e.ClickedItem;
        }

    }
}
