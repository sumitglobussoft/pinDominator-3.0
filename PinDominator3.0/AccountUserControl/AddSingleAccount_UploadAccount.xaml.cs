using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using PinDominator3.Pages.PageAccount;
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

namespace PinDominator3.AccountUserControl
{
    /// <summary>
    /// Interaction logic for AddSingleAccount_UploadAccount.xaml
    /// </summary>
    public partial class AddSingleAccount_UploadAccount : UserControl
    {
        public AddSingleAccount_UploadAccount()
        {
            InitializeComponent();
        }

        private void btnSave_AddSingleAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                QueryManager QM = new QueryManager();
                string accountUser = string.Empty;
                string accountPass = string.Empty;
                string niches = string.Empty;
                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUserName = string.Empty;
                string proxyPassword = string.Empty;
                //string Useragent = string.Empty;
                string BoardsName = string.Empty;
                string ScreenName = string.Empty;
                string LoginStatus = string.Empty;

                if (string.IsNullOrEmpty(txtEmail_AddSingleAcc.Text))
                {
                    try
                    {
                        GlobusLogHelper.log.Info("Please Enter Account");
                        ModernDialog.ShowMessage("Please Enter Account", "Enter Account", MessageBoxButton.OK);
                        return;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                else
                {
                    accountUser = (txtEmail_AddSingleAcc.Text).ToString();
                }

                if (string.IsNullOrEmpty(txtPassword_AddSingleAcc.Text))
                {
                    try
                    {
                        GlobusLogHelper.log.Info("Please Enter Account");
                        ModernDialog.ShowMessage("Please Enter Account", "Enter Account", MessageBoxButton.OK);
                        return;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                else
                {
                    accountPass = (txtPassword_AddSingleAcc.Text).ToString();
                }

                try
                {
                    niches = (txtNiche_AddSinleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyAddress = (txtProxyAddress_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyPort = (txtProxyPort_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyUserName = (txtProxyUsername_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyPassword = (txtProxyPassword_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
                LoginStatus = "NotChecked";

                QM.AddAccountInDataBase(accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName,LoginStatus);
                UploadAccount objUploadAccount = new UploadAccount();
                objUploadAccount.AccounLoad();

                Window parentWindow = (Window)this.Parent;
                parentWindow.Close();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }       
        }

        private void btnClear_AddSingleAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage(" Do you really want to clear Account", " Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    txtEmail_AddSingleAcc.Text = string.Empty;
                    txtPassword_AddSingleAcc.Text = string.Empty;
                    txtNiche_AddSinleAcc.Text = string.Empty;
                    txtProxyAddress_AddSingleAcc.Text = string.Empty;
                    txtProxyPort_AddSingleAcc.Text = string.Empty;
                    txtProxyUsername_AddSingleAcc.Text = string.Empty;
                    txtProxyPassword_AddSingleAcc.Text = string.Empty;
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


       
    }
}
