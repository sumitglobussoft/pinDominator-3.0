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
using FirstFloor.ModernUI.Windows.Controls;
using BasePD;
using System.Text.RegularExpressions;
using System.Threading;
using CommentManager;
using PinDominator.CustomUserControl;
using System.Data;
using System.Diagnostics;
using PinDominator.Classes;

namespace PinDominator.Pages.PageComment
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class CommentByKeyword : UserControl
    {
        public CommentByKeyword()
        {
            InitializeComponent();
            CommentByKeywordManager.objCommentByKeywordDelegate = new AccountReport_CommentByKeyword(AccountReport_CommentByKeyword);
            AccountReport_CommentByKeyword();
        }
        QueryManager QM = new QueryManager();
        private void btnKeyword_CommentByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstMessageKeyword.Clear();

                txt_KeywordComment.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txt_KeywordComment.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstMessageKeyword = GlobusFileHelper.ReadFile(txt_KeywordComment.Text.Trim());
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

        CommentByKeywordManager objCommentByKeywordManager = new CommentByKeywordManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnCommentByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 startCommentByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startCommentByKeyword()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objCommentByKeywordManager.rdbSingleUserCommentByKeyword == true || objCommentByKeywordManager.rdbMultipleUserCommentByKeyword == true)
                        {
                            try
                            {
                                if (objCommentByKeywordManager.rdbSingleUserCommentByKeyword == true)
                                {
                                    try
                                    {                                     
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                                    }
                                }//end of single user
                                if (objCommentByKeywordManager.rdbMultipleUserCommentByKeyword == true)
                                {
                                    if (string.IsNullOrEmpty(txt_KeywordComment.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Keyword ");
                                        ModernDialog.ShowMessage("Please Upload Comment Keyword", "Upload Keyword", MessageBoxButton.OK);
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

                        objCommentByKeywordManager.isStopCommentByKeyword = false;
                        objCommentByKeywordManager.lstThreadsCommentByKeyword.Clear();

                        if (objCommentByKeywordManager._IsfevoriteCommentByKeyword)
                        {
                            objCommentByKeywordManager._IsfevoriteCommentByKeyword = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objCommentByKeywordManager.minDelayCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_DelayMin.Text);
                                objCommentByKeywordManager.maxDelayCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_DelayMax.Text);
                                objCommentByKeywordManager.Nothread_CommentByKeyword = Convert.ToInt32(txtCommentByKeyword_NoOfThreads.Text);
                                objCommentByKeywordManager.MaxCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_Count.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtCommentByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtCommentByKeyword_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtCommentByKeyword_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                            objCommentByKeywordManager.NoOfThreadsCommentByKeyword = threads;

                            Thread CommentKeywordThread = new Thread(objCommentByKeywordManager.StartCommentKeyword);
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

        private void btnCommentByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objCommentByKeyword = new Thread(stopCommentByKeyword);
                objCommentByKeyword.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopCommentByKeyword()
        {
            try
            {
                objCommentByKeywordManager._IsfevoriteCommentByKeyword = true;
                List<Thread> lstTemp = objCommentByKeywordManager.lstThreadsCommentByKeyword.Distinct().ToList();
                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("--------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("--------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void DivideData_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CommentByKeywordManager.chkDivideDataCommentByKeyword = true;
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog()
                    {
                        Content = new UserControl_CommentByKeyword_DivideData()
                    };
                    modernDialog.MinWidth = 600;
                    modernDialog.MinHeight = 300;
                    modernDialog.ShowDialog();
                });
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
    
        public void closeEvent()
        {

        }

        private void rdbSingleUser_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserCommentByKeyword();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserCommentByKeyword()
        {
            try
            {
                objCommentByKeywordManager.rdbSingleUserCommentByKeyword = true;
                objCommentByKeywordManager.rdbMultipleUserCommentByKeyword = false;
                btnKeyword_CommentByKeyword_Browse.Visibility = Visibility.Hidden;
                lbKeyword_CommentByKeyword.Visibility = Visibility.Hidden;
                txt_KeywordComment.Visibility = Visibility.Hidden;
                lbHint_CommentByKeyword.Visibility = Visibility.Hidden;
                ClGlobul.lstMessageKeyword.Clear();
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Keyword Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword::Comment";
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
                                    ClGlobul.lstMessageKeyword.Add(arr_item);
                                }
                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Keyword Loaded : " + ClGlobul.lstMessageKeyword.Count + " ]");
                        GlobusLogHelper.log.Debug("Keyword : " + ClGlobul.lstMessageKeyword.Count);
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

        private void rdbMultipleUser_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txt_KeywordComment.Text = string.Empty;
                objCommentByKeywordManager.rdbSingleUserCommentByKeyword = false;
                objCommentByKeywordManager.rdbMultipleUserCommentByKeyword = true;
                btnKeyword_CommentByKeyword_Browse.Visibility = Visibility.Visible;
                txt_KeywordComment.IsReadOnly = true;
                lbKeyword_CommentByKeyword.Visibility = Visibility.Visible;
                txt_KeywordComment.Visibility = Visibility.Visible;
                lbHint_CommentByKeyword.Visibility = Visibility.Visible; 
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

        public void AccountReport_CommentByKeyword()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName = string.Empty;
                string ModuleName = string.Empty;
                string Keyword = string.Empty;
                string Pin = string.Empty;
                string msg = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                    {
                        AccountViewModel._listAccReportCommentByKeyword.Clear();
                    }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Keyword");
                dt.Columns.Add("Pin");
                dt.Columns.Add("Message");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("CommentByKeyword");
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
                        msg = dt_item.ItemArray[6].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Keyword, Pin, msg, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.PinNo = Pin;
                        objAccountNotifyPropertyChanged.Message = msg;
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportCommentByKeyword.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportCommentByKeyword.Add(objAccountNotifyPropertyChanged);
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
                        dgvCommentByKeyword_AccountsReport.ItemsSource = AccountViewModel._listAccReportCommentByKeyword;
                        //dgvCommentByKeyword_AccountsReport.ItemsSource = dt.DefaultView;

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



        private void clkExportData_CommentByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportDataCommentByKeyword();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportDataCommentByKeyword()
        {
            try
            {
                if (dgvCommentByKeyword_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvCommentByKeyword_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Keyword", "Pin", "Message", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\CommentByKeyword.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("CommentByKeyword");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Keyword = item.ItemArray[7].ToString();
                                        string Pin = item.ItemArray[3].ToString();
                                        string Message = item.ItemArray[6].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Keyword.Replace("'", ""), Pin.Replace("'", ""), Message.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkDeleteData_CommentByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("CommentByKeyword");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_CommentByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

      









    }
}
