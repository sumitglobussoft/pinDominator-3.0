using BaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BaseLib;
using Globussoft;
using BasePD;
using LikeManager;
using System.Text.RegularExpressions;
using FirstFloor.ModernUI.Windows.Controls;
using PinDominator3.CustomUserControl;
using System.Data;
using System.Diagnostics;
using PinDominator3.Classes;


namespace PinDominator3.Pages.PageLike
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Like : UserControl
    {
        public Like()
        {
            InitializeComponent();
            LikeManagers.objLikeDelegate = new AccortReport_Like(AccortReport_Like);
            AccortReport_Like();
        }

        LikeManagers objLikeManagers = new LikeManagers();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnPinUrls_Like_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtLikePinUrl.IsReadOnly = true;
                ClGlobul.lstAddToBoardUserNames.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                    txtLikePinUrl.Text = ofd.FileName.ToString();
                    Thread ReadLargeFileThread = new Thread(ReadLargeLikePinUrlsFile);
                    ReadLargeFileThread.Start(ofd.FileName);
                    ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtLikePinUrl.Text.Trim());
                    GlobusLogHelper.log.Info(" => [ " + ClGlobul.lstAddToBoardUserNames.Count + " Pin Urls Uploaded ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }
        List<Thread> lstReLikeThread = new List<Thread>();
        private void ReadLargeLikePinUrlsFile(object filePath)
        {
            try
            {
                lstReLikeThread.Add(Thread.CurrentThread);
                lstReLikeThread.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;

                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddLikePinUrlsList(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void AddLikePinUrlsList(List<string> Messages)
        {
            try
            {
                ClGlobul.lstLikePinUrls.Clear();
                this. Dispatcher.Invoke((Action)delegate
                {
                    foreach (string Message in Messages)
                    {
                        string NewMessages = Message.Replace("\0", "").Trim();
                        if (!ClGlobul.lstLikePinUrls.Contains(NewMessages))
                        {
                            if (!string.IsNullOrEmpty(NewMessages))
                            {
                                if (NewMessages.Length > 400)
                                {
                                    NewMessages = NewMessages.Substring(0, 400);
                                }
                                ClGlobul.lstLikePinUrls.Add(NewMessages.Replace("\0", ""));
                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rboNormalLikePinUrls_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rboMultipleUser_Like.IsChecked == true)
                {
                    LikeManagers.rbNormalLikePinUrls = true;
                    LikeManagers.rbListLikePinUrls = false;
                    btnPinUrls_Like_Browse.Visibility = Visibility.Hidden;
                    lblUserType_Like.Visibility = Visibility.Hidden;
                    rboSingleUser_Like.Visibility = Visibility.Hidden;
                    rboMultipleUser_Like.Visibility = Visibility.Hidden;
                    lblPinNo_Like.Visibility = Visibility.Hidden;
                    txtLikePinUrl.Visibility = Visibility.Hidden;
                }
                if (rboSingleUser_Like.IsChecked == true)
                {
                    LikeManagers.rbNormalLikePinUrls = true;
                    LikeManagers.rbListLikePinUrls = false;
                    btnPinUrls_Like_Browse.Visibility = Visibility.Hidden;
                    lblUserType_Like.Visibility = Visibility.Hidden;
                    rboSingleUser_Like.Visibility = Visibility.Hidden;
                    rboMultipleUser_Like.Visibility = Visibility.Hidden;
                }
                //LikeManagers.rbNormalLikePinUrls = true;
                //LikeManagers.rbListLikePinUrls = false;
                //btnPinUrls_Like_Browse.Visibility = Visibility.Hidden;
                //lblUserType_Like.Visibility = Visibility.Hidden;
                //rboSingleUser_Like.Visibility = Visibility.Hidden;
                //rboMultipleUser_Like.Visibility = Visibility.Hidden;
            }
            catch { };
            try
            {
                txtLikePinUrl.IsReadOnly = true;
              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rboListLikePinUrls_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ListLikePinUrls();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void ListLikePinUrls()
        {
            try
            {
                if (rboMultipleUser_Like.IsChecked == true)
                {
                    LikeManagers.rbListLikePinUrls = true;
                    LikeManagers.rbNormalLikePinUrls = false;
                    btnPinUrls_Like_Browse.Visibility = Visibility.Visible;
                    lblUserType_Like.Visibility = Visibility.Visible;
                    rboSingleUser_Like.Visibility = Visibility.Visible;
                    rboMultipleUser_Like.Visibility = Visibility.Visible;
                    lblPinNo_Like.Visibility = Visibility.Visible;
                    txtLikePinUrl.Visibility = Visibility.Visible;
                }
                if (rboSingleUser_Like.IsChecked == true)
                {
                    lblUserType_Like.Visibility = Visibility.Visible;
                    rboSingleUser_Like.Visibility = Visibility.Visible;
                    rboMultipleUser_Like.Visibility = Visibility.Visible;
                    SingleUserLike();
                }
                //LikeManagers.rbListLikePinUrls = true;
                //LikeManagers.rbNormalLikePinUrls = false;
                //btnPinUrls_Like_Browse.Visibility = Visibility.Visible;
                //lblUserType_Like.Visibility = Visibility.Visible;
                //rboSingleUser_Like.Visibility = Visibility.Visible;
                //rboMultipleUser_Like.Visibility = Visibility.Visible;              
            }
            catch { };
            try
            {
                txtLikePinUrl.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }


        private void btnLike_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startLike();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startLike()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (rboListLikePinUrls.IsChecked == true)
                        {
                            if (rboMultipleUser_Like.IsChecked == true)
                            {
                                if (string.IsNullOrEmpty(txtLikePinUrl.Text))
                                {
                                    GlobusLogHelper.log.Info("Please Upload Comment Message");
                                    ModernDialog.ShowMessage("Please Upload Comment Message", "Upload Message", MessageBoxButton.OK);
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objLikeManagers.isStopLike = false;
                    objLikeManagers.lstThreadsLike.Clear();

                    if (objLikeManagers._IsfevoriteLike)
                    {
                        objLikeManagers._IsfevoriteLike = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            LikeManagers.minDelayLike = Convert.ToInt32(txtLike_DelayMin.Text);
                            LikeManagers.maxDelayLike = Convert.ToInt32(txtLike_DelayMax.Text);
                            objLikeManagers.NoOfThreadsLike = Convert.ToInt32(txtLike_NoOfThreads.Text);
                            LikeManagers.MaxLike = Convert.ToInt32(txtLikeCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtLike_NoOfThreads.Text) && checkNo.IsMatch(txtLike_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtLike_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                        objLikeManagers.NoOfThreadsLike = threads;

                        Thread LikeThread = new Thread(objLikeManagers.StartLike);
                        LikeThread.Start();
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
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void closeEvent()
        {

        }

        private void chkBox_Like_DivideData_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.chkBox_Like_DivideData = true;
                UserControl_Like_DivideData objUserControl_Like_DivideData = new UserControl_Like_DivideData();
                    var modernDialog = new ModernDialog
                    {
                        Content = objUserControl_Like_DivideData
                    };
                    modernDialog.MinWidth = 600;
                    modernDialog.MinHeight = 300;
                    Button customButton = new Button() { Content = "SAVE" };
                    customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                    modernDialog.Buttons = new Button[] { customButton };
                    modernDialog.ShowDialog();
                    string s1 = string.Empty;
                    try
                    {
                        if (LikeManagers.rdbDivideEqually == true || LikeManagers.rdbDivideByUser == true)
                        {
                            if (LikeManagers.rdbDivideByUser == true)
                            {
                                if (!string.IsNullOrEmpty(objUserControl_Like_DivideData.txtGiveByUser_Like.Text))
                                {
                                    LikeManagers.CountGivenByUser = Convert.ToInt32(objUserControl_Like_DivideData.txtGiveByUser_Like.Text);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info("Please Give Count Given By User");
                                    ModernDialog.ShowMessage("Please Give Count Given By User", "Count Given By User", MessageBoxButton.OK);
                                    return;
                                }
                            }
                            ModernDialog.ShowMessage("Notice", "Data Successfully Save", MessageBoxButton.OK);
                            GlobusLogHelper.log.Info("=> [ Your Data Successfully Save ]");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("=> [ Please Select Divide Equally Or Divide Given By User First ]");
                            ModernDialog.ShowMessage("Please Select Divide Equally Or Divide Given By User First", "Select First", MessageBoxButton.OK);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }        
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnLike_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopLike = new Thread(stopLike);
                objStopLike.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void stopLike()
        {
            try
            {
                objLikeManagers._IsfevoriteLike = true;
                List<Thread> lstTempLike = objLikeManagers.lstThreadsLike.Distinct().ToList();
                foreach (Thread item in lstTempLike)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("-------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        public void AccortReport_Like()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName=string.Empty;
                string ModuleName =string.Empty;
                string Pin = string.Empty;
                string Status=string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportLike.Clear();
                }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");              
                dt.Columns.Add("Pin");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("Like");
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
                         Status = dt_item.ItemArray[9].ToString();
                         DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Pin, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportLike.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportLike.Add(objAccountNotifyPropertyChanged);
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
                        dgvLikePin_AccountsReport.ItemsSource = AccountViewModel._listAccReportLike;

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

        private void clkDeletAccReport_Like(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("Like");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccortReport_Like();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_Like(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReport_Like();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReport_Like()
        {
            try
            {
                if(dgvLikePin_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvLikePin_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Pin", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\Like.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("Like");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Pin = item.ItemArray[3].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), Pin.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rboSingleUser_Like_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SingleUserLike();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void SingleUserLike()
        {
            try
            {
                objLikeManagers.rdbSingleUserLike = true;
                objLikeManagers.rdbMultipleUserLike = false;
                btnPinUrls_Like_Browse.Visibility = Visibility.Hidden;
                lblPinNo_Like.Visibility = Visibility.Hidden;
                txtLikePinUrl.Visibility = Visibility.Hidden;

                ClGlobul.lstAddToBoardUserNames.Clear();
                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter Pins Here ";
                // obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword 1 ,Keyword 2";
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
                                ClGlobul.lstAddToBoardUserNames.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Pins Loaded : " + ClGlobul.lstAddToBoardUserNames.Count + " ]");
                    GlobusLogHelper.log.Debug("Pins : " + ClGlobul.lstAddToBoardUserNames.Count);
                } 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rboMultipleUser_Like_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objLikeManagers.rdbSingleUserLike = false;
                objLikeManagers.rdbMultipleUserLike = true;
                btnPinUrls_Like_Browse.Visibility = Visibility.Visible;
                lblPinNo_Like.Visibility = Visibility.Visible;
                txtLikePinUrl.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


    }
}
