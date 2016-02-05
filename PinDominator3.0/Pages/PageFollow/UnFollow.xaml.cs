using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using FollowManagers;
using PinDominator3.CustomUserControl;
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

namespace PinDominator3.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class UnFollow : UserControl
    {
        public UnFollow()
        {
            InitializeComponent();
            UnFollowManager.objUnFollowDelegate = new AccountReport_UnFollow(AccountReport_UnFollow);
            AccountReport_UnFollow();
        }

        UnFollowManager objUnFollowManager = new UnFollowManager();
        Utils.Utils objUtil = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnUnFollow_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startUnFollow();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startUnFollow()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        objUnFollowManager.isStopUnFollow = false;
                        objUnFollowManager.lstThreadsUnFollow.Clear();

                        if (objUnFollowManager._IsfevoriteUnFollow)
                        {
                            objUnFollowManager._IsfevoriteUnFollow = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtil.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objUnFollowManager.minDelayUnFollow = Convert.ToInt32(txtUnFollow_DelayMin.Text);
                                objUnFollowManager.maxDelayUnFollow = Convert.ToInt32(txtUnFollow_DelayMax.Text);
                                objUnFollowManager.Nothread_UnFollow = Convert.ToInt32(txtUnFollow_NoOfThreads.Text);
                                objUnFollowManager.MaxUnFollowCount = Convert.ToInt32(txtUnFollowCount.Text);
                                objUnFollowManager.NOofDays = Convert.ToInt32(txtUnfollowDays.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtUnFollow_NoOfThreads.Text) && checkNo.IsMatch(txtUnFollow_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtUnFollow_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                            objUnFollowManager.NoOfThreadsUnFollow = threads;

                            Thread UnFollowThread = new Thread(objUnFollowManager.StartUnFollow);
                            UnFollowThread.Start();
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

        private void chkboxUnfollowNoOFDays_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objUnFollowManager.chkNoOFDays_UnFollow = true;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void closeEvent()
        {

        }

        private void chkUploadUnFollowList_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                UnFollowManager.chkUploadUnFollowList = true;
                UserControl_UnFollow_UploadUnFollowList objUserControl_UnFollow_UploadUnFollowList = new UserControl_UnFollow_UploadUnFollowList();
                    var modernDialog = new ModernDialog()
                    {
                        Content = objUserControl_UnFollow_UploadUnFollowList
                    };
                    modernDialog.MinWidth = 600;
                    modernDialog.MinHeight = 200;
                    Button customButton = new Button() { Content = "SAVE" };
                    customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                    modernDialog.Buttons = new Button[] { customButton };
                    modernDialog.ShowDialog();
                    string s1 = string.Empty;
                    try
                    {
                        if (string.IsNullOrEmpty(objUserControl_UnFollow_UploadUnFollowList.txtUnFollowList_UnFollow.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload UnFollow list");
                            ModernDialog.ShowMessage("Please Upload UnFollow list", "Upload UnFollow list", MessageBoxButton.OK);
                            return;
                        }
                        else if (!string.IsNullOrEmpty(objUserControl_UnFollow_UploadUnFollowList.txtUnFollowList_UnFollow.Text))
                        {
                            ModernDialog.ShowMessage("Notice", "Data Successfully Save", MessageBoxButton.OK);
                            GlobusLogHelper.log.Info("=> [ Your Data Successfully Save ]");
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_UnFollow()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("UserName");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("UnFollow");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                foreach (DataRow dt_item in DS.Tables[0].Rows)
                {
                    try
                    {
                        count++;
                        id = int.Parse(dt_item.ItemArray[0].ToString());
                        string AccountName = dt_item.ItemArray[1].ToString();
                        string ModuleName = dt_item.ItemArray[2].ToString();
                        string UserName = dt_item.ItemArray[5].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, UserName, Status, DateAndTime);
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
                        dgvUnFollow_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void DeleteAccReport_UnFollow(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("UnFollow");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_UnFollow();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ExportData_UnFollow(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportUnFollow();        
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportUnFollow()
        {
            try
            {
                if (dgvUnFollow_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvUnFollow_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "UserName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\UnFollow.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("UnFollow");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string UserName = item.ItemArray[5].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), UserName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        private void btnUnFollow_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopUnFollow = new Thread(stopUnFollow);
                objStopUnFollow.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopUnFollow()
        {
            try
            {
                objUnFollowManager._IsfevoriteUnFollow = true;
                List<Thread> lstTempUnFollow= objUnFollowManager.lstThreadsUnFollow.Distinct().ToList();
                foreach (Thread item in lstTempUnFollow)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Error("---------------------------------------------------------");
                GlobusLogHelper.log.Error(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Error("---------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

    }
}
