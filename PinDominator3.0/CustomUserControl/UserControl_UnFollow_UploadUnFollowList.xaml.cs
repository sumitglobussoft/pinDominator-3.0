using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PinDominator.CustomUserControl
{
    /// <summary>
    /// Interaction logic for UserControl_UnFollow_UploadUnFollowList.xaml
    /// </summary>
    public partial class UserControl_UnFollow_UploadUnFollowList : UserControl
    {
        public UserControl_UnFollow_UploadUnFollowList()
        {
            InitializeComponent();
        }

        private void btnUnFollowlstBrowse_UnFollow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstUploadUnFollowList.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                try
                {
                    if (result == true)
                    {
                        txtUnFollowList_UnFollow.Text = dlg.FileName.ToString();
                    }
                    ClGlobul.lstUploadUnFollowList = GlobusFileHelper.ReadFile(txtUnFollowList_UnFollow.Text.Trim());
                    GlobusLogHelper.log.Info(" =>  [ " + ClGlobul.lstUploadUnFollowList.Count + " UnFollow List Uploaded ]");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(txtUnFollowList_UnFollow.Text))
        //        {
        //            GlobusLogHelper.log.Info("Please Upload UnFollow list");
        //            ModernDialog.ShowMessage("Please Upload UnFollow list", "Upload UnFollow list", MessageBoxButton.OK);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
        //    }
        //}

        


    }
}
