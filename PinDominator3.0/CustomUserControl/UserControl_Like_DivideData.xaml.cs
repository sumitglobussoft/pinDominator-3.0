using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using LikeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PinDominator3.CustomUserControl
{
    /// <summary>
    /// Interaction logic for UserControl_Like_DivideData.xaml
    /// </summary>
    public partial class UserControl_Like_DivideData : UserControl
    {
        public UserControl_Like_DivideData()
        {
            InitializeComponent();
        }

        LikeManagers objLikeManagers = new LikeManagers();

        private void rdbDivideEqually_Like_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.rdbDivideEqually = true;
                rdbDivideByUser_Like.IsEnabled = false;
                txtGiveByUser_Like.Visibility = Visibility.Hidden;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbDivideByUser_Like_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.rdbDivideByUser = true;
                rdbDivideEqually_Like.IsEnabled = false;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        //private void btnDivideData_Like_Save_Click(object sender, RoutedEventArgs e)
        //{
        //     try
        //     {
        //         if (rdbDivideByUser_Like.IsChecked == true)
        //         {
        //             if (!string.IsNullOrEmpty(txtGiveByUser_Like.Text))
        //             {
        //                 LikeManagers.CountGivenByUser = Convert.ToInt32(txtGiveByUser_Like.Text);
        //                 ModernDialog.ShowMessage("Notice", "Data Successfully Save", MessageBoxButton.OK);
        //                 GlobusLogHelper.log.Info("=> [ Your Data Successfully Save ]");
        //             }
        //             else
        //             {
        //                 GlobusLogHelper.log.Info("Please Give Count Given By User");
        //                 ModernDialog.ShowMessage("Please Give Count Given By User", "Count Given By User", MessageBoxButton.OK);
        //                 return;
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
        //     }
       // }

        //private void btnDivideData_Like_Save_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (rdbDivideByUser_Like.IsChecked==true)
        //        {
        //            if (!string.IsNullOrEmpty(txtGiveByUser_Like.Text))
        //            {
        //                LikeManagers.CountGivenByUser = Convert.ToInt32(txtGiveByUser_Like.Text);
        //            }
        //            else
        //            {
        //                GlobusLogHelper.log.Info("Please Give Count Given By User");
        //                ModernDialog.ShowMessage("Please Give Count Given By User", "Count Given By User", MessageBoxButton.OK);
        //                return;
        //            }
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
        //    }
        //}





    }
}
