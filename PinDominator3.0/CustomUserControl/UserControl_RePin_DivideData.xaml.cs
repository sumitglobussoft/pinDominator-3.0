using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using PinsManager;
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

namespace PinDominator.CustomUserControl
{
    /// <summary>
    /// Interaction logic for UserControl_RePin_DivideData.xaml
    /// </summary>
    public partial class UserControl_RePin_DivideData : UserControl
    {
        public UserControl_RePin_DivideData()
        {
            InitializeComponent();
        }

        RePinManager objRePin = new RePinManager();
        
        private void rdbDivideEqually_RePinDivideData_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RePinManager.rdbDivideEqually_RePin = true;
                RePinManager.rdbDivideGivenByUser_RePin = false;
                rdbDivideByUser_RePinDivideData.IsEnabled = false;
                txtCountGivenByUser_RePinDivideData.Visibility = Visibility.Hidden;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbDivideByUser_RePinDivideData_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RePinManager.rdbDivideGivenByUser_RePin = true;
                RePinManager.rdbDivideEqually_RePin = false;
                rdbDivideEqually_RePinDivideData.IsEnabled = false;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        //private void btnsave_RePinDivideData_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (RePinManager.rdbDivideGivenByUser_RePin == true)
        //        {
        //            if (!string.IsNullOrEmpty(txtCountGivenByUser_RePinDivideData.Text))
        //            {
        //                RePinManager.CountGivenByUser_RePin = Convert.ToInt32(txtCountGivenByUser_RePinDivideData.Text);
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
