using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using ManageAccountManager;
using System;
using System.Collections.Generic;
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
    public partial class AcountChecker : UserControl
    {
        public AcountChecker()
        {
            InitializeComponent();
            AccountChecker.objDelegateNoOfAcc = new CheckAccount(AccountReportNoOfAccount_AccChecker);
            AccountChecker.objDelegateNoOfActiveAcc = new CheckAccount(AccountReportNoOfActiveAccount);
            AccountChecker.objDelegateNoOfDeadAcc = new CheckAccount(AccountReportNoOfDeadAccount);
        }

        AccountChecker objAccountChecker = new AccountChecker();
        Utils.Utils objUtils = new Utils.Utils();

        public void AccountReportNoOfActiveAccount(int Count)
        {
            try
            {
                lblNoOfActiveAccount_AccChecker.Dispatcher.Invoke(new Action(delegate
                {
                    lblNoOfActiveAccount_AccChecker.Content = Count.ToString();
                }));
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

        }

        public void AccountReportNoOfDeadAccount(int Count)
        {
            try
            {
                lblNoOfDeadAccount_AccChecker.Dispatcher.Invoke(new Action(delegate
                {
                    lblNoOfDeadAccount_AccChecker.Content = Count.ToString();
                }));
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnBrowseAccounts_AccChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadAccoundChecker();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void LoadAccoundChecker()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt"; //txtLoadAcc_AccountChecker
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtLoadAcc_AccountChecker.Text = dlg.FileName.ToString();
                    ClGlobul.lstAccountForAccountChecker = GlobusFileHelper.ReadFile(txtLoadAcc_AccountChecker.Text.Trim());
                }
                GlobusLogHelper.log.Info("=> [ Total Uploaded Account Is " + ClGlobul.lstAccountForAccountChecker.Count + " ]");
                //string Path = dlg.ToString().Replace("Microsoft.Win32.OpenFileDialog: Title: , FileName", "");
                //this.Dispatcher.Invoke(new Action(delegate
                //    {
                //        lblNoOfAccount_AccChecker.Content = ClGlobul.lstAccountForAccountChecker.Count;
                //    }));
               // ClGlobul.lstAccountForAccountChecker = GlobusFileHelper.ReadFile(dlg.FileName);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnStart_AccountChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startAccountChecker();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startAccountChecker()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (rdoBtn_CheckAccountFromLoadedAccount.IsChecked == true || rdoBtn_CheckAccountFromLoadedFilesAccount.IsChecked == true)
                        {
                           
                            
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Check From Loaded Account or Check From User Files");
                            ModernDialog.ShowMessage("Please Select Check From Loaded Account or Check From User Files", "Select Check From Loaded Account or Check From User Files", MessageBoxButton.OK);
                            return;
                        }

                        objAccountChecker.isStopAccountChecker = false;
                        objAccountChecker.lstThreadsAccountChecker.Clear();

                        if (objAccountChecker._IsfevoriteAccountChecker)
                        {
                            objAccountChecker._IsfevoriteAccountChecker = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objAccountChecker.minDelayAccountChecker = Convert.ToInt32(txtAccountChecker_DelayMin.Text);
                                objAccountChecker.maxDelayAccountChecker = Convert.ToInt32(txtAccountChecker_DelayMax.Text);
                                objAccountChecker.Nothread_AccountChecker = Convert.ToInt32(txtAccountChecker_NoOfThreads.Text);   
                                
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtAccountChecker_NoOfThreads.Text) && checkNo.IsMatch(txtAccountChecker_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtAccountChecker_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }

                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                            objAccountChecker.NoOfThreadsAccountChecker = threads;

                            Thread FollowKeywordThread = new Thread(objAccountChecker.StartAccountChecker);
                            FollowKeywordThread.Start();
                        }

                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnStop_AccountChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objstopAccountChecker = new Thread(stopAccountChecker);
                objstopAccountChecker.Start();               
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopAccountChecker()
        {
            try
            {
                objAccountChecker._IsfevoriteAccountChecker = true;
                List<Thread> lstTemp = objAccountChecker.lstThreadsAccountChecker.Distinct().ToList();
                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch
                    {
                    }
                }

                GlobusLogHelper.log.Info("-----------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-----------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdoBtn_CheckAccountFromLoadedAccount_Checked(object sender, RoutedEventArgs e)
        {
            lblHint_AccountChecker.Visibility = Visibility.Hidden;
            btnBrowseAccounts_AccChecker.Visibility = Visibility.Hidden;
            txtLoadAcc_AccountChecker.Visibility = Visibility.Hidden;
            objAccountChecker.rdCheckAccountFromLoadedAccount = true;
            objAccountChecker.rdCheckAccountFromLoadedFilesAccount=false;
            btnExpoxtActiveAcc_AccountChecker.Visibility = Visibility.Visible;
            btnExportDeadAcc_AccChecker.Visibility = Visibility.Visible;
        }

        private void rdoBtn_CheckAccountFromLoadedFilesAccount_Checked(object sender, RoutedEventArgs e)
        {
            lblHint_AccountChecker.Visibility = Visibility.Visible;
            btnBrowseAccounts_AccChecker.Visibility = Visibility.Visible;
            txtLoadAcc_AccountChecker.Visibility = Visibility.Visible;
            objAccountChecker.rdCheckAccountFromLoadedAccount = false;
            objAccountChecker.rdCheckAccountFromLoadedFilesAccount = true;
            btnExpoxtActiveAcc_AccountChecker.Visibility = Visibility.Visible;
            btnExportDeadAcc_AccChecker.Visibility = Visibility.Visible;
        }

        public void AccountReportNoOfAccount_AccChecker(int Count)
        {
            try
            {
                lblNoOfAccount_AccChecker.Dispatcher.Invoke(new Action(delegate
                {
                    lblNoOfAccount_AccChecker.Content = Count.ToString();
                }));
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }
        }

        private void btnExportDeadAcc_AccChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objAccountChecker.lstOfDeadAcount.Count > 0)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "UserName", "Password", "Niche", "ProxyAddress", "ProxyPort", "ProxyUsername", "ProxyPassword", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\DeadAccount.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                              
                                foreach (string item in objAccountChecker.lstOfDeadAcount)
                                {
                                    try
                                    {
                                        string[] item_Data = Regex.Split(item, ":");
                                        string UserName = item_Data[0].ToString();
                                        string Password = item_Data[1].ToString();
                                        string Niche = item_Data[2].ToString();
                                        string ProxyAddress = item_Data[3].ToString();
                                        string ProxyPort = item_Data[4].ToString();
                                        string ProxyUsername = item_Data[5].ToString();
                                        string ProxyPassword = item_Data[6].ToString();
                                        string DateAndTime = DateTime.Now.ToString();
                                        CSV_Content = string.Join(",", UserName.Replace("'", ""), Password.Replace("'", ""), Niche.Replace("'", ""), ProxyAddress.Replace("'", ""), ProxyPort.Replace("'", ""), ProxyUsername.Replace("'", ""), ProxyPassword.Replace("'", ""), DateAndTime.Replace("'", "")); //, DateAndTime.Replace("'", ""));

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
                    catch (Exception ex)
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

        private void btnExpoxtActiveAcc_AccountChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (objAccountChecker.lstOfActiveAccount.Count > 0)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "UserName", "Password", "Niche", "ProxyAddress", "ProxyPort", "ProxyUsername", "ProxyPassword", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\ActiveAccount.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                    
                                foreach (string item in objAccountChecker.lstOfActiveAccount)
                                {
                                    try
                                    {
                                        string[] item_Data = Regex.Split(item, ":");
                                        string UserName = item_Data[0].ToString();
                                        string Password = item_Data[1].ToString();
                                        string Niche = item_Data[2].ToString();
                                        string ProxyAddress = item_Data[3].ToString();
                                        string ProxyPort = item_Data[4].ToString();
                                        string ProxyUsername = item_Data[5].ToString();
                                        string ProxyPassword = item_Data[6].ToString();
                                        string DateAndTime = DateTime.Now.ToString();
                                        CSV_Content = string.Join(",", UserName.Replace("'", ""), Password.Replace("'", ""), Niche.Replace("'", ""), ProxyAddress.Replace("'", ""), ProxyPort.Replace("'", ""), ProxyUsername.Replace("'", ""), ProxyPassword.Replace("'", ""), DateAndTime.Replace("'", ""));

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
                    catch (Exception ex)
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






    }
}
