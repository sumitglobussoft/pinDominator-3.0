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
using System.Threading;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using CommentManager;
using System.Text.RegularExpressions;
using System.Data;
using PinDominator.CustomUserControl;
using PinDominator.Classes;
using System.Diagnostics;

namespace PinDominator.Pages.PageComment
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Comment : UserControl
    {
        public Comment()
        {
            InitializeComponent();
            CommentManagers.objCommentDelegate = new AccountReport_Comments(AccountReport_Comments);
            AccountReport_Comments();
        }
        QueryManager QM = new QueryManager();
       
        private void btnMessage_Comment_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.CommentMessagesList.Clear();

                txtCommentMessage.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtCommentMessage.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.CommentMessagesList = GlobusFileHelper.ReadFile(txtCommentMessage.Text.Trim());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        CommentManagers objCommentManagers = new CommentManagers();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnComment_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startComment();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startComment()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objCommentManagers.rdbSingleUserComment == true || objCommentManagers.rdbMultipleUserComment == true)
                        {
                            try
                            {
                                if (objCommentManagers.rdbSingleUserComment == true)
                                {
                                    try
                                    {                                                                         
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }//end of single User
                                if (objCommentManagers.rdbMultipleUserComment == true)
                                {
                                    try
                                    {
                                        if (string.IsNullOrEmpty(txtCommentMessage.Text))
                                        {
                                            GlobusLogHelper.log.Info("Please Upload Comment Message");
                                            ModernDialog.ShowMessage("Please Upload Comment Message", "Upload Message", MessageBoxButton.OK);
                                            return;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }//end of multiple user
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Use Single User or Use Multiple User");
                            ModernDialog.ShowMessage("Please Select Use Single User or Use Multiple User", "Select Use Single User or Use Multiple User", MessageBoxButton.OK);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objCommentManagers.isStopComment = false;
                    objCommentManagers.lstThreadsComment.Clear();

                    if (objCommentManagers._IsfevoriteComment)
                    {
                        objCommentManagers._IsfevoriteComment = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            CommentManagers.minDelayComment = Convert.ToInt32(txtComment_DelayMin.Text);
                            CommentManagers.maxDelayComment = Convert.ToInt32(txtComment_DelayMax.Text);
                            CommentManagers.Nothread_Comment = Convert.ToInt32(txtComment_NoOfThreads.Text);
                            CommentManagers.MaxComment = Convert.ToInt32(txtCommentCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtComment_NoOfThreads.Text) && checkNo.IsMatch(txtComment_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtComment_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }

                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                        objCommentManagers.NoOfThreadsComment = threads;

                        Thread CommentThread = new Thread(objCommentManagers.StartComment);
                        CommentThread.Start();
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
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btnComment_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopComment = new Thread(stopComment);
                objStopComment.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopComment()
        {
            try
            {
                objCommentManagers._IsfevoriteComment = true;
                List<Thread> lstTemp = objCommentManagers.lstThreadsComment.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void closeEvent()
        {    }

        private void rdbSingleUser_Comment_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserComment();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserComment()
        {
            try
            {
                objCommentManagers.rdbSingleUserComment = true;
                objCommentManagers.rdbMultipleUserComment = false;
                btnMessage_Comment_Browse.Visibility = Visibility.Hidden;
                txtCommentMessage.Visibility = Visibility.Hidden;
                lbMsg_Comment.Visibility = Visibility.Hidden;
                ClGlobul.CommentMessagesList.Clear();
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Message Here ";
                   // obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::BoardName::BoardUrl";
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
                                     ClGlobul.CommentMessagesList.Add(arr_item);
                                }
                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Message Loaded : " + ClGlobul.CommentMessagesList.Count + " ]");
                        GlobusLogHelper.log.Debug("Message : " + ClGlobul.CommentMessagesList.Count);
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

        private void rdbMultipleUser_Comment_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtCommentMessage.Text = string.Empty;
                objCommentManagers.rdbSingleUserComment = false;
                objCommentManagers.rdbMultipleUserComment = true;
                btnMessage_Comment_Browse.Visibility = Visibility.Visible;
                txtCommentMessage.IsReadOnly = true;
                txtCommentMessage.Visibility = Visibility.Visible;
                lbMsg_Comment.Visibility = Visibility.Visible;
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

        public void AccountReport_Comments()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName = string.Empty;
                string ModuleName = string.Empty;
                string Pin = string.Empty;
                string Msg = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                    {
                        AccountViewModel._listAccReportComment.Clear();
                    }));

                DataSet DS = null; 
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Pin");
                dt.Columns.Add("Message");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("Comment");
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
                        Pin = dt_item.ItemArray[3].ToString();
                        Msg = dt_item.ItemArray[6].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Pin, Msg, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.Message = Msg;
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportComment.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportComment.Add(objAccountNotifyPropertyChanged);
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
                        dgvComment_AccountsReport.ItemsSource = AccountViewModel._listAccReportComment;                                               
                        //dgvComment_AccountsReport.ItemsSource = dt.DefaultView;                    
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

        private void clkExportData_Comment(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReportComment();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        public void ExportAccountReportComment()
        {
            try
            {
                if (dgvComment_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvComment_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Pin", "Message", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\Comment.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("Comment");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Pin = item.ItemArray[3].ToString();
                                        string Message = item.ItemArray[6].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Pin.Replace("'", ""), Message.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        private void clkDeleteData_Comment(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("Comment");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_Comments();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        
    }
}
