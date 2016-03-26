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
using PinsManager;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using PinDominator.CustomUserControl;
using System.Diagnostics;
using PinDominator.Classes;

namespace PinDominator.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class EditPinDiscription : UserControl
    {
        public EditPinDiscription()
        {
            InitializeComponent();
            EditPinDiscriptionManager.objEditPinDescriptionDelegate = new AccountReport_EditPinDescription(AccountReport_EditPinDescription);
            AccountReport_EditPinDescription();
        }

        private void btu_Browse_PinDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.CommentNicheMessageList.Clear();
               // txtPinDescription.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtPinDescription.Text = ofd.FileName.ToString();
                    ClGlobul.CommentNicheMessageList = GlobusFileHelper.ReadFile(txtPinDescription.Text.Trim());
                    GlobusLogHelper.log.Info(" => [ Total Message Uploaded : " + ClGlobul.CommentNicheMessageList.Count + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        EditPinDiscriptionManager objEditPinDiscriptionManager = new EditPinDiscriptionManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnEditPinDescription_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startEditDescription();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startEditDescription()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objEditPinDiscriptionManager.rdbSingleUserEditPinDisc == true || objEditPinDiscriptionManager.rdbMultipleUserEditPinDisc == true)
                        {
                            if (objEditPinDiscriptionManager.rdbSingleUserEditPinDisc == true)
                            {
                                try
                                {                                  
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of single User
                            if (objEditPinDiscriptionManager.rdbMultipleUserEditPinDisc == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtPinDescription.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Description");
                                        ModernDialog.ShowMessage("Please Upload Description", "Upload Description", MessageBoxButton.OK);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of multiple user
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
                    objEditPinDiscriptionManager.isStopEditPinDisc = false;
                    objEditPinDiscriptionManager.lstThreadsEditPinDisc.Clear();

                    if (objEditPinDiscriptionManager._IsfevoriteEditPinDes)
                    {
                        objEditPinDiscriptionManager._IsfevoriteEditPinDes = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objEditPinDiscriptionManager.minDelayEditPinDisc = Convert.ToInt32(txtEditPinDescription_DelayMin.Text);
                            objEditPinDiscriptionManager.maxDelayEditPinDisc = Convert.ToInt32(txtEditPinDescription_DelayMax.Text);
                            objEditPinDiscriptionManager.Nothread_EditPinDisc = Convert.ToInt32(txtEditPinDescription_NoOfThreads.Text);
                            objEditPinDiscriptionManager.NoOfPagesEditPinDisc = Convert.ToInt32(txtNoOfPages_EditPinDisc.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }


                        if (ClGlobul.CommentNicheMessageList.Count > 0)
                        {
                            clsSettingDB Database = new clsSettingDB();
                            Database.UpdateSettingData("PinDescription", "PinDescriptionMessage", StringEncoderDecoder.Encode(txtPinDescription.Text));
                            GlobusLogHelper.log.Info(" => [ Start Edit Description ]");
                            ClGlobul.lstPins.Clear();

                        }
                        if (!string.IsNullOrEmpty(txtEditPinDescription_NoOfThreads.Text) && checkNo.IsMatch(txtEditPinDescription_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtEditPinDescription_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                        objEditPinDiscriptionManager.NoOfThreadsEditPinDisc = threads;

                        Thread EditPinDiscThread = new Thread(objEditPinDiscriptionManager.StartEditPinDisc);
                        EditPinDiscThread.Start();
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

        private void btnEditPinDescription_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                stopEditDescription();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopEditDescription()
        {
            try
            {
                objEditPinDiscriptionManager._IsfevoriteEditPinDes = true;
                List<Thread> lstTempEditPin = objEditPinDiscriptionManager.lstThreadsEditPinDisc.Distinct().ToList();
                foreach (Thread item in lstTempEditPin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
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
        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        public void AccountReport_EditPinDescription()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName=string.Empty;
                string ModuleName =string.Empty;
                string Pin = string.Empty;
                string Description=string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportEditPin.Clear();
                }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");           
                dt.Columns.Add("Pin");
                dt.Columns.Add("Description"); 
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("EditPinDiscription");
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
                         Description = dt_item.ItemArray[6].ToString();
                         Status = dt_item.ItemArray[9].ToString();
                         DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, Pin, Description, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.Message = Description;
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;

                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportEditPin.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportEditPin.Add(objAccountNotifyPropertyChanged);
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
                        dgvEditPinDescription_AccountsReport.ItemsSource = AccountViewModel._listAccReportEditPin;

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
    
        public void closeEvent() { }

        private void SingleUser_EditPinDesc_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUser_EditPinDesc();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUser_EditPinDesc()
        {
            try
            {
                objEditPinDiscriptionManager.rdbSingleUserEditPinDisc = true;
                objEditPinDiscriptionManager.rdbMultipleUserEditPinDisc = false;
                btu_Browse_PinDescription.Visibility = Visibility.Hidden;
                lblDescription.Visibility = Visibility.Hidden;
                txtPinDescription.Visibility = Visibility.Hidden;
                lblHints_EditDesc.Visibility = Visibility.Hidden;
                try
                {
                    ClGlobul.CommentNicheMessageList.Clear();
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Description With Niche Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::Description";
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
                                    ClGlobul.CommentNicheMessageList.Add(arr_item);
                                }

                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Description Loaded : " + ClGlobul.CommentNicheMessageList.Count + " ]");
                        GlobusLogHelper.log.Debug("Description : " + ClGlobul.CommentNicheMessageList.Count);
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

        private void MultipleUser_EditPinDesc_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtPinDescription.Text = string.Empty;
                objEditPinDiscriptionManager.rdbSingleUserEditPinDisc = false;
                objEditPinDiscriptionManager.rdbMultipleUserEditPinDisc = true;
                btu_Browse_PinDescription.Visibility = Visibility.Visible;
                txtPinDescription.IsReadOnly = true;
                lblDescription.Visibility = Visibility.Visible;
                txtPinDescription.Visibility = Visibility.Visible;
                lblHints_EditDesc.Visibility = Visibility.Visible; 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkDeleteAccReport_EditPinDesc(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("EditPinDiscription");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_EditPinDescription();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ClkExportData_EditPinDesc(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportEditPinDesc();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportEditPinDesc()
        {
            try
            {
                if (dgvEditPinDescription_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvEditPinDescription_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "Pin", "Description", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\EditPinDiscription.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("EditPinDiscription");
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
                    catch (Exception Ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
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
