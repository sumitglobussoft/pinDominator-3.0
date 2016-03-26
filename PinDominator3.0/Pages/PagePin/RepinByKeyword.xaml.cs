using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using PinDominator.Classes;
using PinDominator.CustomUserControl;
using PinsManager;
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

namespace PinDominator.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for RepinByKeyword.xaml
    /// </summary>
    public partial class RepinByKeyword : UserControl
    {
        public RepinByKeyword()
        {
            InitializeComponent();
            RepinByKeywordManager.objRepinByKeywordDelegate = new AccountReport_RepinByKeyword(AccountReport_RepinByKeyword);
            AccountReport_RepinByKeyword();
        }

        QueryManager QM = new QueryManager();

        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        private void btnKeyword_RepinByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstRepinByKeyword.Clear();
                txtKeywordBoard.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtKeywordBoard.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstRepinByKeyword = GlobusFileHelper.ReadFile(txtKeywordBoard.Text.Trim());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info("Please Select File");
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnMessage_RepinByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstMsgRepinByKeyword.Clear();

                txtMessage_RepinByKeyword.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtMessage_RepinByKeyword.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstMsgRepinByKeyword = GlobusFileHelper.ReadFile(txtMessage_RepinByKeyword.Text.Trim());
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

        Utils.Utils objUtil = new Utils.Utils();
        RepinByKeywordManager objRepinByKeywordManager = new RepinByKeywordManager();

        private void btnRepinByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startRepinByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startRepinByKeyword()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objRepinByKeywordManager.rdbSingleUserRepinByKeyword == true || objRepinByKeywordManager.rdbMultipleUserRepinByKeyword == true)
                        {
                            if (objRepinByKeywordManager.rdbSingleUserRepinByKeyword == true)
                            {
                                try
                                {
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                                }
                            }//end of single user
                            if (objRepinByKeywordManager.rdbMultipleUserRepinByKeyword == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtKeywordBoard.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Enter Keyword");
                                        ModernDialog.ShowMessage("Please Enter Keyword", "Enter Keyword", MessageBoxButton.OK);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                                }
                            }//end of multiple user
                        }
                        else
                        {
                            MessageBox.Show("Please Select Use Single User or Use Multiple User");
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    clsSettingDB Database = new clsSettingDB();
                    // Database.UpdateSettingData("Board", "AddBoardUrl", StringEncoderDecoder.Encode(txtBoardName.Text));
                    //Database.UpdateSettingData("Board", "AddBoardMessage", StringEncoderDecoder.Encode(txtMessage.Text));

                    objRepinByKeywordManager.isStopRepinByKeyword = false;
                    objRepinByKeywordManager.lstThreadsRepinByKeyword.Clear();
                    if (objRepinByKeywordManager._IsfevoriteRepinByKeyword)
                    {
                        objRepinByKeywordManager._IsfevoriteRepinByKeyword = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtil.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        try
                        {
                            objRepinByKeywordManager.minDelayRepinByKeyword = Convert.ToInt32(txtRepinByKeyword_DelayMin.Text);
                            objRepinByKeywordManager.maxDelayRepinByKeyword = Convert.ToInt32(txtRepinByKeyword_DelayMax.Text);
                            objRepinByKeywordManager.NoOfThreadsRepinByKeyword = Convert.ToInt32(txtRepinByKeyword_NoOfThreads.Text);
                            objRepinByKeywordManager.MaxCountRepinByKeyword = Convert.ToInt32(txtNoOfPinRepin_RepinByKeyword.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtRepinByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtRepinByKeyword_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtRepinByKeyword_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objRepinByKeywordManager.NoOfThreadsRepinByKeyword = threads;

                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                        Thread BoardsThread = new Thread(objRepinByKeywordManager.StartRepinKeyword);
                        BoardsThread.Start();
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

        private void rdbSingleUser_RepinByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserRepinByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void closeEvent()
        { }

        public void rdbSingleUserRepinByKeyword()
        {
            try
            {
                objRepinByKeywordManager.rdbSingleUserRepinByKeyword = true;
                objRepinByKeywordManager.rdbMultipleUserRepinByKeyword = false;
                btnKeyword_RepinByKeyword_Browse.Visibility = Visibility.Hidden;
                btnMessage_RepinByKeyword_Browse.Visibility = Visibility.Hidden;
                txtKeywordBoard.Visibility = Visibility.Hidden;
                txtMessage_RepinByKeyword.Visibility = Visibility.Hidden;
                lbBoardWithKeyword_RepinByKeyword.Visibility = Visibility.Hidden;
                lblMessage_RepinByKeyword.Visibility = Visibility.Hidden;
                lblHints_RepinByKeyword.Visibility = Visibility.Hidden;

                ClGlobul.lstRepinByKeyword.Clear();
                ClGlobul.lstMsgRepinByKeyword.Clear();
                #region BoardUrl
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter BoardName and Keyword Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::BoardName::Keyword";
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
                                    ClGlobul.lstRepinByKeyword.Add(arr_item);
                                }

                            }
                        }
                        GlobusLogHelper.log.Info(" => [ BoardName and BoardUrl with Niche Loaded : " + ClGlobul.lstRepinByKeyword.Count + " ]");
                        GlobusLogHelper.log.Debug("BoardName and BoardUrl with Niche : " + ClGlobul.lstRepinByKeyword.Count);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
                #endregion

                SingleUserMessageRepinByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


        public void SingleUserMessageRepinByKeyword()
        {
            #region Message
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
                                ClGlobul.lstMsgRepinByKeyword.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Message Loaded : " + ClGlobul.lstMsgRepinByKeyword.Count + " ]");
                    GlobusLogHelper.log.Debug("Message : " + ClGlobul.lstMsgRepinByKeyword.Count);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            #endregion
        }

        private void rdbMultipleUser_RepinByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtKeywordBoard.Text = string.Empty;
                txtMessage_RepinByKeyword.Text = string.Empty;
                objRepinByKeywordManager.rdbSingleUserRepinByKeyword = false;
                objRepinByKeywordManager.rdbMultipleUserRepinByKeyword = true;
                btnKeyword_RepinByKeyword_Browse.Visibility = Visibility.Visible;             
                btnMessage_RepinByKeyword_Browse.Visibility = Visibility.Visible;
                txtKeywordBoard.Visibility = Visibility.Visible;
                txtMessage_RepinByKeyword.Visibility = Visibility.Visible;
                lbBoardWithKeyword_RepinByKeyword.Visibility = Visibility.Visible;
                lblMessage_RepinByKeyword.Visibility = Visibility.Visible;
                lblHints_RepinByKeyword.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_RepinByKeyword()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName = string.Empty;
                string ModuleName = string.Empty;
                string Keyword = string.Empty;
                string Pin = string.Empty;
                string BoardName = string.Empty;
                string Msg = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportRepinByKeyword.Clear();
                }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Keyword");
                dt.Columns.Add("Pin");
                dt.Columns.Add("BoardName");
                dt.Columns.Add("Message");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("RepinByKeyword");
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
                        BoardName = dt_item.ItemArray[4].ToString();
                        Msg = dt_item.ItemArray[6].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Keyword, Pin, BoardName, Msg, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.BoardName = BoardName;
                        objAccountNotifyPropertyChanged.Message = Msg;
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportRepinByKeyword.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportRepinByKeyword.Add(objAccountNotifyPropertyChanged);
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
                        dgvRepinByKeyword_AccountsReport.ItemsSource = AccountViewModel._listAccReportRepinByKeyword;
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

        private void btnRepinByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStop = new Thread(stopRepinByKeyword);
                objStop.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopRepinByKeyword()
        {
            try
            {
                objRepinByKeywordManager.isStopRepinByKeyword = true;
                List<Thread> lstTempRepinByKeyword = objRepinByKeywordManager.lstThreadsRepinByKeyword.Distinct().ToList();
                foreach (Thread item in lstTempRepinByKeyword)
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

        private void ExportData_RepinByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReport();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccountReport()
        {
            try
            {
                if (dgvRepinByKeyword_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvRepinByKeyword_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Keyword", "Pin", "BoardName", "Message", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\RepinByKeywordAccountReport.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("RepinByKeyword");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Keyword = item.ItemArray[7].ToString();                                      
                                        string Pin = item.ItemArray[3].ToString();
                                        string BoardName = item.ItemArray[4].ToString();
                                        string Msg = item.ItemArray[6].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Keyword.Replace("'", ""), Pin.Replace("'", ""), BoardName.Replace("'", ""), Msg.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        private void DeleteData_RepinByKeyword(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("RepinByKeyword");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_RepinByKeyword();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }






    }
}
