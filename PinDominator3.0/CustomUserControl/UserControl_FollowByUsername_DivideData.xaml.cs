using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using FollowManagers;
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
    /// Interaction logic for UserControl_FollowByUsername_DivideData.xaml
    /// </summary>
    public partial class UserControl_FollowByUsername_DivideData : UserControl
    {
        public UserControl_FollowByUsername_DivideData()
        {
            InitializeComponent();
        }

        FollowByUsernameManager objFollow = new FollowByUsernameManager();
        private void rdbDivideEqually_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rdbDivideEquallyFollowByUsername = true;
                FollowByUsernameManager.rdbDivideGivenByUserFollowByUsername = false;
                rdbDivideByUser_FollowByUsername.IsEnabled = false;
                txtCountGivenByUser_FollowByUsername.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbDivideByUser_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rdbDivideEquallyFollowByUsername = false;
                FollowByUsernameManager.rdbDivideGivenByUserFollowByUsername = true;
                rdbDivideEqually_FollowByUsername.IsEnabled = false;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        //private void btnSave_FollowByUsername_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (FollowByUsernameManager.rdbDivideGivenByUserFollowByUsername == true)
        //        {
        //            if (!string.IsNullOrEmpty(txtCountGivenByUser_FollowByUsername.Text))
        //            {
        //                FollowByUsernameManager.CountGivenByUserFollowByUsename = Convert.ToInt32(txtCountGivenByUser_FollowByUsername.Text); 
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
