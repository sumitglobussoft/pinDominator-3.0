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
using BaseLib;
using Globussoft;
using FollowManagers;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using PinDominator3.CustomUserControl;

namespace PinDominator3.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class FollowByKeyword : UserControl
    {
        public FollowByKeyword()
        {
            InitializeComponent();
            FollowByKeywordManager.objFollowByKeywordDelegate = new AccountReport_FollowByKeyword(AccountReport_FollowByKeyword);
            AccountReport_FollowByKeyword();
        } 
        private void btnKeyword_FollowByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstkeyword.Clear();

                txtKeywordUpload.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtKeywordUpload.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstkeyword = GlobusFileHelper.ReadFile(txtKeywordUpload.Text.Trim());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }


        FollowByKeywordManager objFollowByKeywordManager = new FollowByKeywordManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnFollowByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startFollowByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startFollowByKeyword()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objFollowByKeywordManager.rdbSingleUserFollowByKeyword == true || objFollowByKeywordManager.rdbMultipleUserFollowByKeyword == true)
                        {
                            if (objFollowByKeywordManager.rdbSingleUserFollowByKeyword == true)
                            {
                                try
                                {

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of single User
                            if (objFollowByKeywordManager.rdbMultipleUserFollowByKeyword == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtKeywordUpload.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Keyword ");
                                        ModernDialog.ShowMessage("Please Upload Comment Keyword", "Upload Keyword", MessageBoxButton.OK);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of Multiple user  
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Use Single User or Use Multiple User");
                            ModernDialog.ShowMessage("Please Select Use Single User or Use Multiple User", "Select Use Single User or Use Multiple User", MessageBoxButton.OK);
                            return;
                        }

                        objFollowByKeywordManager.isStopFollowByKeyword = false;
                        objFollowByKeywordManager.lstThreadsFollowByKeyword.Clear();

                        if (objFollowByKeywordManager._IsfevoriteFollowByKeyword)
                        {
                            objFollowByKeywordManager._IsfevoriteFollowByKeyword = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objFollowByKeywordManager.minDelayFollowByKeyword = Convert.ToInt32(txtFollowByKeyword_DelayMin.Text);
                                objFollowByKeywordManager.maxDelayFollowByKeyword = Convert.ToInt32(txtFollowByKeyword_DelayMax.Text);
                                objFollowByKeywordManager.Nothread_FollowByKeyword = Convert.ToInt32(txtFollowByKeyword_NoOfThreads.Text);
                                objFollowByKeywordManager.NoOfUserFollowByKeyword = Convert.ToInt32(txtFollowPerDay.Text);
                                objFollowByKeywordManager.AccPerDayUserFollowByKeyword = Convert.ToInt32(txtBoxHours_FollowByKeyword.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtKeywordUpload.Text));
                            GlobusLogHelper.log.Info(" => [ Starting Follow Through Keyword ]");

                            if (!string.IsNullOrEmpty(txtFollowByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtFollowByKeyword_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtFollowByKeyword_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }

                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                            objFollowByKeywordManager.NoOfThreadsFollowByKeyword = threads;

                            Thread FollowKeywordThread = new Thread(objFollowByKeywordManager.StartFollowKeyword);
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

        private void btnFollowByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objstopFollowByKeyword = new Thread(stopFollowByKeyword);
                objstopFollowByKeyword.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopFollowByKeyword()
        {
            try
            {
                objFollowByKeywordManager._IsfevoriteFollowByKeyword = true;
                List<Thread> lstTemp = objFollowByKeywordManager.lstThreadsFollowByKeyword.Distinct().ToList();
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

        public void closeEvent()
        {
        }

        private void rdbSingleUser_FollowByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserFollowByKeyword();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserFollowByKeyword()
        {
            try
            {
                objFollowByKeywordManager.rdbSingleUserFollowByKeyword = true;
                objFollowByKeywordManager.rdbMultipleUserFollowByKeyword = false;
                btnKeyword_FollowByKeyword_Browse.Visibility = Visibility.Hidden;
                txtKeywordUpload.IsReadOnly = true;
                lbKeyword.Visibility = Visibility.Hidden;
                txtKeywordUpload.Visibility = Visibility.Hidden;
                lbKeywordEg.Visibility = Visibility.Hidden;
                ClGlobul.lstkeyword.Clear();
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Keyword Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword 1 ,Keyword 2";
                    var window = new ModernDialog
                    {

                        Content = obj
                    };
                    window.ShowInTaskbar = true;
                    window.MinWidth = 100;
                    window.MinHeight = 300;
                    Button customButton = new Button() { Content = "SAVE" };
                    customButton.Click += (ss, ee) => { closeEvent(); window.Close(); };
                    window.Buttons = new Button[] { customButton };

                    window.ShowDialog();

                    MessageBoxButton btnC = MessageBoxButton.YesNo;
                    var result = ModernDialog.ShowMessage("Are you sure want to save ?", "Message Box", btnC);

                    if (result == MessageBoxResult.Yes)
                    {
                        TextRange textRange = new TextRange(obj.txtEnterSingleMessages.Document.ContentStart, obj.txtEnterSingleMessages.Document.ContentEnd);

                        if (!string.IsNullOrEmpty(textRange.Text))
                        {
                            string enterText = textRange.Text;
                            string[] arr = Regex.Split(enterText, "\r\n");

                            foreach (var arr_item in arr)
                            {
                                if (!string.IsNullOrEmpty(arr_item) || !arr_item.Contains(""))
                                {
                                    ClGlobul.lstkeyword.Add(arr_item);
                                }

                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Keyword Loaded : " + ClGlobul.lstkeyword.Count + " ]");
                        GlobusLogHelper.log.Debug("Keyword : " + ClGlobul.lstkeyword.Count);
                    }                             
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

        private void rdbMultipleUser_FollowByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtKeywordUpload.Text = string.Empty;
                objFollowByKeywordManager.rdbSingleUserFollowByKeyword = false;
                objFollowByKeywordManager.rdbMultipleUserFollowByKeyword = true;
                btnKeyword_FollowByKeyword_Browse.Visibility = Visibility.Visible;
                txtKeywordUpload.IsReadOnly = true;
                lbKeyword.Visibility = Visibility.Visible;
                txtKeywordUpload.Visibility = Visibility.Visible;
                lbKeywordEg.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_FollowByKeyword()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet DS = null; 
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Keyword");
                dt.Columns.Add("UserName");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("FollowByKeyword");
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
                        string Keyword = dt_item.ItemArray[7].ToString();
                        string UserName = dt_item.ItemArray[5].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, Keyword, UserName, Status, DateAndTime);
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
                        dgvFollowByKeyword_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkDelete_FollowByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("FollowByKeyword");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_FollowByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExport_FollowByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReportFollowByKeyword();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccountReportFollowByKeyword()
        {
            try
            {
                if (dgvFollowByKeyword_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvFollowByKeyword_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Keyword", "UserName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\FollowByKeyword.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("FollowByKeyword");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Keyword = item.ItemArray[7].ToString();
                                        string UserName = item.ItemArray[5].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Keyword.Replace("'", ""), UserName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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
