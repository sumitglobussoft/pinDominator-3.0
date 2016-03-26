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
using System.Text.RegularExpressions;
using PinsManager;
using PinDominator.CustomUserControl;
using System.Data;
using PinDominator.Classes;
using System.Diagnostics;

namespace PinDominator.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class RePin : UserControl
    {
        public RePin()
        {
            InitializeComponent();
            RePinManager.objRepinDelegate = new AccountReport_Repin(AccountReport_Repin);
            AccountReport_Repin();
        }
        PinInterestUser objPinUser = new PinInterestUser();
        RePinManager objRePinManager = new RePinManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btu_UploadRepinNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstRepinUrl.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtRepinNo_Repin.Text = dlg.FileName.ToString();
                     readPin(dlg.FileName);
                }                               
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        public void readPin(object filePath)
        {
            try
            {
                List<string> lstRepinUrl1 = GlobusFileHelper.ReadFile(txtRepinNo_Repin.Text.Trim());

                Thread thrAddAndTest = new Thread(() => AddAndTestPins(lstRepinUrl1));
                thrAddAndTest.Start();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
        public void AddAndTestPins(List<string> lstRepinUrl1)
        {
            try
            {
                objPinUser.globusHttpHelper = new GlobusHttpHelper();
                foreach (var lstRepinUrl_item in lstRepinUrl1)
                {
                    if (lstRepinUrl_item.Contains("https://www.pinterest.com/pin/") || lstRepinUrl_item.Contains("http://www.pinterest.com/pin/"))
                    {
                        try
                        {
                            string url = lstRepinUrl_item;
                            string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
                            if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
                            {
                                ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        try
                        {
                            ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);

                        }
                        catch { };
                    }
                }

                GlobusLogHelper.log.Info(" => [ Total Urls Uploaded : " + ClGlobul.lstRepinUrl.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btu_UploadMessageRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.RepinMessagesList.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtRepinMessage_Repin.Text = ofd.FileName.ToString();                  
                }
                ClGlobul.RepinMessagesList = GlobusFileHelper.ReadFileForRepin(txtRepinMessage_Repin.Text.Trim());
                GlobusLogHelper.log.Error(" => [ Total Message Uploaded : " + ClGlobul.RepinMessagesList.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
      
        private void btnStartRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startRePin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startRePin()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    objRePinManager.isStopRePin = false;
                    objRePinManager.lstThreadsRePin.Clear();
                    clsSettingDB Database = new clsSettingDB();
                    Database.UpdateSettingData("Repin", "RepinMsgFile", StringEncoderDecoder.Encode(txtRepinMessage_Repin.Text));
                    Database.UpdateSettingData("Repin", "RepinNO", StringEncoderDecoder.Encode(txtRepinNo_Repin.Text));

                    ClGlobul.lstPins.Clear();

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objRePinManager.minDelayRePin = Convert.ToInt32(txtRepin_DelayMin.Text);
                            objRePinManager.maxDelayRePin = Convert.ToInt32(txtRepin_DelayMax.Text);
                            objRePinManager.Nothread_RePin = Convert.ToInt32(txtRepinNoOfThreads_RePin.Text);
                            objRePinManager.maxNoOfRePinCount = Convert.ToInt32(txtRepinCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }
                        Regex checkNo = new Regex("^[0-9]*$");

                        if (!string.IsNullOrEmpty(txtRepinNoOfThreads_RePin.Text) && checkNo.IsMatch(txtRepinNoOfThreads_RePin.Text))
                        {
                            threads = Convert.ToInt32(txtRepinNoOfThreads_RePin.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                        objRePinManager.NoOfThreadsRePin = threads;

                        Thread RePinThread = new Thread(objRePinManager.StartRepin);
                        RePinThread.Start();
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

        private void rbo_RepinNormalType_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rdbRepinNormalType = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rbo_RepinUserRepin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdbMultipleUser_Repin.IsChecked == true)
                {
                     lblRepinNo.Visibility=Visibility.Hidden;
                      txtRepinNo_Repin.Visibility = Visibility.Hidden;
                      btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                      lblMessage_Repin.Visibility = Visibility.Visible;
                      txtRepinMessage_Repin.Visibility = Visibility.Visible;
                      Brow_Repin_Messge.Visibility = Visibility.Visible;
                }
                if (rdbSingleUser_Repin.IsChecked == true)
                {
                    SingleUserRepin();
                }
                //btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                //Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rbdRepinUserRepin = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rdo_UsePinNo_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rdbMultipleUser_Repin.IsChecked == true)
                {
                    lblRepinNo.Visibility = Visibility.Visible;
                    txtRepinNo_Repin.Visibility = Visibility.Visible;
                    btnRepinUrlUplaod.Visibility = Visibility.Visible;
                    lblMessage_Repin.Visibility = Visibility.Visible;
                    txtRepinMessage_Repin.Visibility = Visibility.Visible;
                    Brow_Repin_Messge.Visibility = Visibility.Visible;
                }
                if (rdbSingleUser_Repin.IsChecked == true)
                {
                     SingleUserRepin();
                }
               // btnRepinUrlUplaod.Visibility = Visibility.Visible;
               // Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rdbUsePinNo = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btnStopRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopRepin = new Thread(stopRepin);
                objStopRepin.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopRepin()
        {
            try
            {
                objRePinManager._IsfevoriteRepin = true;
                List<Thread> lstTempRePin = objRePinManager.lstThreadsRePin.Distinct().ToList();
                foreach (Thread item in lstTempRePin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("----------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("----------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void closeEvent()
        {

        }

        private void chkDivideData_Repin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RePinManager.chkDivideData_RePin = true;
                UserControl_RePin_DivideData objUserControl_RePin_DivideData = new UserControl_RePin_DivideData();
                        var modernDialog = new ModernDialog()
                        {
                            Content = objUserControl_RePin_DivideData
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
                            if (RePinManager.rdbDivideEqually_RePin == true || RePinManager.rdbDivideGivenByUser_RePin == true)
                            {
                                if (RePinManager.rdbDivideGivenByUser_RePin == true)
                                {
                                    if (!string.IsNullOrEmpty(objUserControl_RePin_DivideData.txtCountGivenByUser_RePinDivideData.Text))
                                    {
                                        RePinManager.CountGivenByUser_RePin = Convert.ToInt32(objUserControl_RePin_DivideData.txtCountGivenByUser_RePinDivideData.Text);
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        public void AccountReport_Repin()
        {
            try
            {
                int count = 0;
                int id = 0;
                string AccountName =string.Empty;
                string ModuleName =string.Empty;
                string Pin = string.Empty;
                string Message = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportRepin.Clear();
                }));

                DataSet DS = null; ;
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
                    DS = QM.SelectAddReport("RePin");
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
                        Message = dt_item.ItemArray[6].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Pin, Message, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.Message = Message;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportRepin.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportRepin.Add(objAccountNotifyPropertyChanged);
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
                        dgvRepin_AccountsReport.ItemsSource = AccountViewModel._listAccReportRepin;

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

        private void clkDeleteAccReport_RePin(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("RePin");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_Repin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_RePin(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportRepin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportRepin()
        {
            try
            {
                if (dgvRepin_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvRepin_AccountsReport.Items.Count > 1)
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
                                FilePath = FilePath + "\\RePin.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("RePin");
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

        private void rdbSingleUser_Repin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SingleUserRepin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
      
        public void SingleUserRepin()
        {
            try
            {
                lblRepinNo.Visibility = Visibility.Hidden;
                txtRepinNo_Repin.Visibility = Visibility.Hidden;
                btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                lblMessage_Repin.Visibility = Visibility.Hidden;
                txtRepinMessage_Repin.Visibility = Visibility.Hidden;
                Brow_Repin_Messge.Visibility = Visibility.Hidden;
                ClGlobul.RepinMessagesList.Clear();
                ClGlobul.lstRepinUrl.Clear();

                if (rbo_RepinUserRepin.IsChecked == true)
                {
                    try
                    {
                        MessageRepin();
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }                    
                }
                if (rdo_UsePinNo.IsChecked  == true)
                {
                     try
                    {
                       
                        UserControl_SingleUser obj = new UserControl_SingleUser();
                        obj.UserControlHeader.Text = "Enter Pin Here ";
                        //obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword 1 ,Keyword 2";
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
                                        ClGlobul.lstRepinUrl.Add(arr_item);
                                    }

                                }
                            }
                            GlobusLogHelper.log.Info(" => [ Pin Loaded : " + ClGlobul.lstRepinUrl.Count + " ]");
                            GlobusLogHelper.log.Debug("Pin : " + ClGlobul.lstRepinUrl.Count);
                        }
                        MessageRepin();
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

        private void rdbMultipleUser_Repin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbo_RepinUserRepin.IsChecked == true)
                {
                      lblRepinNo.Visibility=Visibility.Hidden;
                      txtRepinNo_Repin.Visibility = Visibility.Hidden;
                      btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                      lblMessage_Repin.Visibility = Visibility.Visible;
                      txtRepinMessage_Repin.Visibility = Visibility.Visible;
                      Brow_Repin_Messge.Visibility = Visibility.Visible;
                }
                if (rdo_UsePinNo.IsChecked == true)
                {
                    lblRepinNo.Visibility = Visibility.Visible;
                    txtRepinNo_Repin.Visibility = Visibility.Visible;
                    btnRepinUrlUplaod.Visibility = Visibility.Visible;
                    lblMessage_Repin.Visibility = Visibility.Visible;
                    txtRepinMessage_Repin.Visibility = Visibility.Visible;
                    Brow_Repin_Messge.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void MessageRepin()
        {
            try
            {                
                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter Message Here ";
                //obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Keyword 1 ,Keyword 2";
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
                                ClGlobul.RepinMessagesList.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Message Loaded : " + ClGlobul.RepinMessagesList.Count + " ]");
                    GlobusLogHelper.log.Debug("Message : " + ClGlobul.RepinMessagesList.Count);
                }  
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }



    }
}
