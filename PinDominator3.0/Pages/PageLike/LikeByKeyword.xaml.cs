using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using LikeManager;
using PinDominator3.Classes;
using PinDominator3.CustomUserControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace PinDominator3.Pages.PageLike
{
    /// <summary>
    /// Interaction logic for LikeByKeyword.xaml
    /// </summary>
    public partial class LikeByKeyword : UserControl
    {
        public LikeByKeyword()
        {
            InitializeComponent();
            LikeByKeywordManager.objLikeByKeywordDelegate = new AccountReport_LikeByKeyword(AccountReport_LikeByKeyword);
            AccountReport_LikeByKeyword();
        }
        QueryManager QM = new QueryManager();
        private void btnKeyword_LikeByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstLikeByKeyword.Clear();

                txt_KeywordLike.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txt_KeywordLike.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstLikeByKeyword = GlobusFileHelper.ReadFile(txt_KeywordLike.Text.Trim());
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

        private void clkExportData_LikeByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReportLikeByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccountReportLikeByKeyword()
        {
            try
            {
                if (dgvLikeByKeyword_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvLikeByKeyword_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Keyword", "Pin", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\LikeByKeywordAccountReport.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("LikeByKeyword");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Keyword = item.ItemArray[7].ToString();
                                        string Pin = item.ItemArray[3].ToString();                                      
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Keyword.Replace("'", ""), Pin.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        private void clkDeleteData_LikeByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("LikeByKeyword");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_LikeByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnLikeByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startLikeByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        Utils.Utils objUtils = new Utils.Utils();
        LikeByKeywordManager objLikeByKeywordManager = new LikeByKeywordManager();

        public void startLikeByKeyword()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objLikeByKeywordManager.rdbSingleUserLikeByKeyword == true || objLikeByKeywordManager.rdbMultipleUserLikeByKeyword == true)
                        {
                            try
                            {
                                if (objLikeByKeywordManager.rdbSingleUserLikeByKeyword == true)
                                {
                                    try
                                    {
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                                    }
                                }//end of single user
                                if (objLikeByKeywordManager.rdbMultipleUserLikeByKeyword == true)
                                {
                                    if (string.IsNullOrEmpty(txt_KeywordLike.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Keyword ");
                                        ModernDialog.ShowMessage("Please Upload Keyword", "Upload Keyword", MessageBoxButton.OK);
                                        return;
                                    }
                                }//end of multiple users
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Use Single User or Use Multiple User");
                            ModernDialog.ShowMessage("Please Select Use Single User or Use Multiple User", "Select Use Single User or Use Multiple User", MessageBoxButton.OK);
                            return;
                        }

                        objLikeByKeywordManager.isStopLikeByKeyword = false;
                        objLikeByKeywordManager.lstThreadsLikeByKeyword.Clear();

                        if (objLikeByKeywordManager._IsfevoriteLikeByKeyword)
                        {
                            objLikeByKeywordManager._IsfevoriteLikeByKeyword = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objLikeByKeywordManager.minDelayLikeByKeyword = Convert.ToInt32(txtLikeByKeyword_DelayMin.Text);
                                objLikeByKeywordManager.maxDelayLikeByKeyword = Convert.ToInt32(txtLikeByKeyword_DelayMax.Text);
                                objLikeByKeywordManager.Nothread_LikeByKeyword = Convert.ToInt32(txtLikeByKeyword_NoOfThreads.Text);
                                objLikeByKeywordManager.MaxLikeByKeyword = Convert.ToInt32(txtLikeByKeyword_Count.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtLikeByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtLikeByKeyword_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtLikeByKeyword_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                            objLikeByKeywordManager.NoOfThreadsLikeByKeyword = threads;

                            Thread CommentKeywordThread = new Thread(objLikeByKeywordManager.StartLikeKeyword);
                            CommentKeywordThread.Start();
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
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rdbSingleUser_LikeByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserLikeByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void closeEvent()
        { }

        public void rdbSingleUserLikeByKeyword()
        {
            try
            {
                objLikeByKeywordManager.rdbSingleUserLikeByKeyword = true;
                objLikeByKeywordManager.rdbMultipleUserLikeByKeyword = false;
                btnKeyword_LikeByKeyword_Browse.Visibility = Visibility.Hidden;
                lbKeyword_LikeByKeyword.Visibility = Visibility.Hidden;
                txt_KeywordLike.Visibility = Visibility.Hidden;
                lbHint_LikeByKeyword.Visibility = Visibility.Hidden;
                ClGlobul.lstLikeByKeyword.Clear();
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Keyword Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword";
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
                                    ClGlobul.lstLikeByKeyword.Add(arr_item);
                                }
                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Keyword Loaded : " + ClGlobul.lstLikeByKeyword.Count + " ]");
                        GlobusLogHelper.log.Debug("Keyword : " + ClGlobul.lstLikeByKeyword.Count);
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

        private void rdbMultipleUser_LikeByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txt_KeywordLike.Text = string.Empty;
                objLikeByKeywordManager.rdbSingleUserLikeByKeyword = false;
                objLikeByKeywordManager.rdbMultipleUserLikeByKeyword = true;
                btnKeyword_LikeByKeyword_Browse.Visibility = Visibility.Visible;
                txt_KeywordLike.IsReadOnly = true;
                lbKeyword_LikeByKeyword.Visibility = Visibility.Visible;
                txt_KeywordLike.Visibility = Visibility.Visible;
                lbHint_LikeByKeyword.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        public void AccountReport_LikeByKeyword()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName = string.Empty;
                string ModuleName = string.Empty;
                string Keyword = string.Empty;
                string Pin = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportLikeByKeyword.Clear();
                }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Keyword");
                dt.Columns.Add("Pin");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("LikeByKeyword");
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
                        AccountName = dt_item.ItemArray[1].ToString();
                        ModuleName = dt_item.ItemArray[2].ToString();
                        Keyword = dt_item.ItemArray[7].ToString();
                        Pin = dt_item.ItemArray[3].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Keyword, Pin, Status, DateAndTime);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }

                    try
                    {
                        AccountNotifyPropertyChanged objAccountNotifyPropertyChanged = new AccountNotifyPropertyChanged();
                        objAccountNotifyPropertyChanged.ID = count;
                        objAccountNotifyPropertyChanged.AccName = AccountName;
                        objAccountNotifyPropertyChanged.ModuleName = ModuleName;
                        objAccountNotifyPropertyChanged.Keyword = Keyword;
                        objAccountNotifyPropertyChanged.PinNo = Pin;       
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportLikeByKeyword.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportLikeByKeyword.Add(objAccountNotifyPropertyChanged);
                            }
                        }));

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
                        dgvLikeByKeyword_AccountsReport.ItemsSource = AccountViewModel._listAccReportLikeByKeyword;
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

        private void btnLikeByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStop = new Thread(stopLikeByKeyword);
                objStop.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopLikeByKeyword()
        {
            try
            {
                objLikeByKeywordManager.isStopLikeByKeyword = true;
                List<Thread> lstTempLikeByKeyword = objLikeByKeywordManager.lstThreadsLikeByKeyword.Distinct().ToList();
                foreach (Thread item in lstTempLikeByKeyword)
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



    }
}
