using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using ManageAccountManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PinDominator3.Pages.PageAccount
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class ManageAccount : UserControl
    {
        public ManageAccount()
        {
            InitializeComponent();
            ManageAccManager.objManageAccountDelegate = new AccountReport_ManageAccount(AccountReport_ManageAccount);
            AccountReport_ManageAccount();
        }

        Utils.Utils objUtls = new Utils.Utils();
        ManageAccManager objManageAccManager = new ManageAccManager();
        QueryManager Qm = new QueryManager();

        private void rdbChangeEmail_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objManageAccManager.rdbChangeEmailManageAccount = true;
                objManageAccManager.rdbChangePwdManageAccount = false;
                gbChangePassword.IsEnabled = false;
                gpbChangeEmail_ManageAccount.IsEnabled = true;
               // gpbChangeScreenName.IsEnabled = false;
                objManageAccManager.rdbChangeScreenNameManageAccount = false;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbChangePassword_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objManageAccManager.rdbChangeEmailManageAccount = false;
                objManageAccManager.rdbChangePwdManageAccount = true;
                gpbChangeEmail_ManageAccount.IsEnabled = false;
                gbChangePassword.IsEnabled = true;
               //gpbChangeScreenName.IsEnabled = false;
                objManageAccManager.rdbChangeScreenNameManageAccount = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnLoadEmail_ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstEmailManageAcc.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtEmail_ManageAccount.Text = dlg.FileName.ToString();
                    ClGlobul.lstEmailManageAcc = GlobusFileHelper.ReadFile(txtEmail_ManageAccount.Text.Trim());
                }
                GlobusLogHelper.log.Info(" => [ Total Email Uploaded : " + ClGlobul.lstEmailManageAcc.Count + " ]");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnLoadPassword_ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstPwdManageAcc.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtPassword_ManageAccount.Text = dlg.FileName.ToString();
                    ClGlobul.lstPwdManageAcc = GlobusFileHelper.ReadFile(txtPassword_ManageAccount.Text.Trim());
                }
                GlobusLogHelper.log.Info(" => [ Total Email Uploaded : " + ClGlobul.lstPwdManageAcc.Count + " ]");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnStart_ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            LoadManageAccount();
        }
        public void LoadManageAccount()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objManageAccManager.rdbChangeEmailManageAccount == true || objManageAccManager.rdbChangePwdManageAccount == true || objManageAccManager.rdbChangeScreenNameManageAccount == true)
                        {
                            try
                            {
                                if (objManageAccManager.rdbChangeEmailManageAccount == true)
                                {
                                    if (string.IsNullOrEmpty(txtEmail_ManageAccount.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Email");
                                        ModernDialog.ShowMessage("Please Upload Email", "Upload Email", MessageBoxButton.OK);
                                        return;
                                    }       
                                }

                                if (objManageAccManager.rdbChangePwdManageAccount == true)
                                {
                                    try
                                    {
                                        if (string.IsNullOrEmpty(txtPassword_ManageAccount.Text))
                                        {
                                            GlobusLogHelper.log.Info("Please Upload Password");
                                            ModernDialog.ShowMessage("Please Upload Password", "Upload Password", MessageBoxButton.OK);
                                            return;
                                        }      
                                    }
                                    catch (Exception ex)
                                    { };                                                         
                                }

                                //if (objManageAccManager.rdbChangeScreenNameManageAccount == true)
                                //{
                                //     try
                                //    {
                                //        if (string.IsNullOrEmpty(txtScreenName_ManageAccount.Text))
                                //        {
                                //            GlobusLogHelper.log.Info("Please Upload ScreenName");
                                //            ModernDialog.ShowMessage("Please Upload ScreenName", "Upload ScreenName", MessageBoxButton.OK);
                                //            return;
                                //        }      
                                //    }
                                //    catch (Exception ex)
                                //    { };       
                                //}

                            }
                            catch(Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Change Type First");
                            ModernDialog.ShowMessage("Please Select Change Type First", "Select Change Type First", MessageBoxButton.OK);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    objManageAccManager.isStopManageAcc = false;
                    objManageAccManager.lstThreadsManageAcc.Clear();
                    if (objManageAccManager._IsfevoriteManageAcc)
                    {
                        objManageAccManager._IsfevoriteManageAcc = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtls.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {                     
                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objManageAccManager.NoOfThreadsManageAcc = threads;

                        Thread AddBoardNameThread = new Thread(objManageAccManager.StartLoadManageAccount);
                        AddBoardNameThread.Start();
                    }

                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }             
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbChangeScreenName_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {             
                objManageAccManager.rdbChangeEmailManageAccount = false;
                objManageAccManager.rdbChangePwdManageAccount = false;
                gpbChangeEmail_ManageAccount.IsEnabled = false;
                gbChangePassword.IsEnabled = false;
               // gpbChangeScreenName.IsEnabled = true;
                objManageAccManager.rdbChangeScreenNameManageAccount = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnLoadScreenName_ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    ClGlobul.lstScreenNameManageAcc.Clear();
            //    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //    dlg.DefaultExt = ".txt";
            //    dlg.Filter = "Text documents (.txt)|*.txt";
            //    Nullable<bool> result = dlg.ShowDialog();
            //    if (result == true)
            //    {
            //        txtScreenName_ManageAccount.Text = dlg.FileName.ToString();
            //        ClGlobul.lstScreenNameManageAcc = GlobusFileHelper.ReadFile(txtScreenName_ManageAccount.Text.Trim());
            //    }
            //    GlobusLogHelper.log.Info(" => [ Total Email Uploaded : " + ClGlobul.lstScreenNameManageAcc.Count + " ]");
            //}
            //catch (Exception ex)
            //{
            //    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            //}
        }

        private void btnStop_ManageAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopManage = new Thread(stopManageAcc);
                objStopManage.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopManageAcc()
        {
            try
            {
                objManageAccManager._IsfevoriteManageAcc = true;
                List<Thread> lstTempManageAcc = objManageAccManager.lstThreadsManageAcc.Distinct().ToList();
                foreach (Thread item in lstTempManageAcc)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void chkRandomEmailChange_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objManageAccManager.RandomEmailChangeManageAccount = true;
                objManageAccManager.RandomPasswordChangeManageAccount = false;
                objManageAccManager.RandomScreenNameChangeManageAccount = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void chkRandomPasswordChange_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objManageAccManager.RandomEmailChangeManageAccount = false;
                objManageAccManager.RandomPasswordChangeManageAccount = true;
                objManageAccManager.RandomScreenNameChangeManageAccount = false;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void chkRandomScreenNameChange_ManageAccount_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objManageAccManager.RandomEmailChangeManageAccount = false;
                objManageAccManager.RandomPasswordChangeManageAccount = false;
                objManageAccManager.RandomScreenNameChangeManageAccount = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_ManageAccount()
        {
            try
            {
                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("NewEmail");
                dt.Columns.Add("NewPassword");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = Qm.SelectAddReport("ManageAccount");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                foreach (DataRow dt_item in DS.Tables[0].Rows)
                {
                    try
                    {
                        string AccountName = dt_item.ItemArray[1].ToString();
                        string ModuleName = dt_item.ItemArray[2].ToString();
                        string NewEmail = dt_item.ItemArray[10].ToString();
                        string NewPassword = dt_item.ItemArray[11].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, NewEmail, NewPassword, Status, DateAndTime);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                try
                {
                    DataView dv;
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        dgvManageAccount_AccountsReport.ItemsSource = dt.DefaultView;

                    }));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ExportData_ManageAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReportManageAcc();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccountReportManageAcc()
        {
            try
            {
                if (dgvManageAccount_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvManageAccount_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "NewEmail", "NewPassword", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\ManageAccount.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = Qm.SelectAddReport("ManageAccount");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string NewEmail = item.ItemArray[10].ToString();
                                        string NewPassword = item.ItemArray[11].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), NewEmail.Replace("'", ""), NewPassword.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Content, ExportDataLocation);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void DeleteData_ManageAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Qm.DeleteAccountReport("ManageAccount");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                ExportAccountReportManageAcc();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

 

    }
}
