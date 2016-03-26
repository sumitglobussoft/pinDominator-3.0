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
using BoardManager;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using PinsManager;
using PinDominator.CustomUserControl;

namespace PinDominator.Pages.Pageboard
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddBoardName : UserControl
    {
        public AddBoardName()
        {
            InitializeComponent();          
            AddPinWithNewBoardManager.objDelegateAccountReport = new AccountReport_AddBoardName(AccountReport_AddBoardName);
            AccountReport_AddBoardName();
        }

        AddBoardNameManager objAddBoardNameManager = new AddBoardNameManager();
        QueryManager QM = new QueryManager();
        private void btnCreateBaord_AddBoardName_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstBoardNames.Clear();

                txtBoardCreate.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                try
                {
                    if (result == true)
                    {
                        txtBoardCreate.Text = dlg.FileName.ToString();
                        ClGlobul.lstBoardNames = GlobusFileHelper.ReadFile(txtBoardCreate.Text.Trim());

                    }
                    if (ClGlobul.lstBoardNames.Count > 0)
                    {
                        string checkBoardName = string.Empty;
                        foreach (string st in ClGlobul.lstBoardNames)
                        {
                            checkBoardName += st;
                        }

                        objAddBoardNameManager.noOfBoard = System.Text.RegularExpressions.Regex.Split(checkBoardName, ",");
                    }
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

        private void btnStart_AddBoardName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startAddBoardName();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startAddBoardName()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objAddBoardNameManager.rdbSingleUserAddBoardName == true || objAddBoardNameManager.rdbMultipleUserAddBoardName == true)
                        {
                            if (objAddBoardNameManager.rdbSingleUserAddBoardName == true)
                            {
                                try
                                {                                
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                            }//end of single user
                            else if (objAddBoardNameManager.rdbMultipleUserAddBoardName == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtBoardCreate.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Board Name With Niche");
                                        ModernDialog.ShowMessage("Please Upload Board Name With Niche", "Upload Board Name With Niche", MessageBoxButton.OK);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of multiple User                      
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

                    clsSettingDB Database = new clsSettingDB();
                    Database.UpdateSettingData("Board", "AddBoardName", StringEncoderDecoder.Encode(txtBoardCreate.Text));

                    objAddBoardNameManager.isStopAddBoardName = false;
                    objAddBoardNameManager.lstThreadsAddBoardName.Clear();
                    if (objAddBoardNameManager._Isfevorite)
                    {
                        objAddBoardNameManager._Isfevorite = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtil.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddBoardNameManager.minDelayAddBoardName = Convert.ToInt32(txtBoardCreate_DelayMin.Text);
                            objAddBoardNameManager.maxDelayAddBoardName = Convert.ToInt32(txtBoardCreate_DelayMax.Text);
                            objAddBoardNameManager.Nothread_AddBoardName = Convert.ToInt32(txtBoardCreate_NoOfThreads.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtBoardCreate_NoOfThreads.Text) && checkNo.IsMatch(txtBoardCreate_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtBoardCreate_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddBoardNameManager.NoOfThreadsAddBoardName = threads;

                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                        Thread AddBoardNameThread = new Thread(objAddBoardNameManager.StartAddBoardName);
                        AddBoardNameThread.Start();
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

        private void btnStop_AddBoardName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopAddBoard = new Thread(stopAddBoardName);
                objStopAddBoard.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopAddBoardName()
        {
            try
            {
                objAddBoardNameManager._Isfevorite = true;
                List<Thread> lstTemp_AddBoardName = objAddBoardNameManager.lstThreadsAddBoardName.Distinct().ToList();
                foreach (Thread item in lstTemp_AddBoardName)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch
                    {
                    }
                }

                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

         //UserControl_SingleUser_AddBoardName objUserControl_SingleUser_AddBoardName = new  UserControl_SingleUser_AddBoardName();

        public void closeEvent()
        { }

        private void rdbSingleUser_AddBoardName_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserAddBoardName();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserAddBoardName()
        {
            try
            {
                objAddBoardNameManager.rdbMultipleUserAddBoardName = false;
                objAddBoardNameManager.rdbSingleUserAddBoardName = true;
                btnCreateBaord_AddBoardName_Browse.Visibility = Visibility.Hidden;
                txtBoardCreate.Visibility = Visibility.Hidden;
                lbBoardName.Visibility = Visibility.Hidden;
                lblHint_AddBoardName.Visibility = Visibility.Hidden;
                ClGlobul.lstBoardNames.Clear();

                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter Board Name Here ";
                obj.txtEnterSingleMessages.ToolTip = "Format :-  Niche:Board Name 1 , Board Name 2";
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
                                    ClGlobul.lstBoardNames.Add(arr_item);
                                }

                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Upload Board Name With Niche");
                            ModernDialog.ShowMessage("Please Upload Board Name With Niche", "Upload Board Name With Niche", MessageBoxButton.OK);
                            return;
                        }
                        GlobusLogHelper.log.Info("Board Name Loaded : " + ClGlobul.lstBoardNames.Count);
                        GlobusLogHelper.log.Debug("Board Name Loaded : " + ClGlobul.lstBoardNames.Count);                   
                }           
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
            }
        }

        private void rdbMultipleUser_AddBoardName_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBoardCreate.Text = string.Empty;
                objAddBoardNameManager.rdbSingleUserAddBoardName = false;
                objAddBoardNameManager.rdbMultipleUserAddBoardName = true;
                txtBoardCreate.IsReadOnly = true;
                btnCreateBaord_AddBoardName_Browse.Visibility = Visibility.Visible;                      
                txtBoardCreate.Visibility = Visibility.Visible;
                lbBoardName.Visibility = Visibility.Visible;
                lblHint_AddBoardName.Visibility = Visibility.Visible;
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
            }
        }
   
        public void AccountReport_AddBoardName()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet ds = null;
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("BoardName");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                ds = new DataSet();
                ds.Tables.Add(dt);   
                try
                {
                    ds= QM.SelectAddReport("AddBoardName");
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                foreach (DataRow dt_item in ds.Tables[0].Rows)
                {
                    try
                    {
                        count++;
                        id = int.Parse(dt_item.ItemArray[0].ToString());
                        string AccountName = dt_item.ItemArray[1].ToString();
                        string ModuleName = dt_item.ItemArray[2].ToString();
                        string BoardName = dt_item.ItemArray[4].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, BoardName, Status, DateAndTime);
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                try
                {
                    DataView dv;
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        dgvAddBoardName_AccountsReport.ItemsSource = dt.DefaultView;

                    }));
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }             
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkDeleteData_AddBoardName(object sender, RoutedEventArgs e)
        {
            try
            {              
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                   QM.DeleteAccountReport("AddBoardName");
                   GlobusLogHelper.log.Info(" => [All Data is Deleted ] ");
                }
                AccountReport_AddBoardName();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_AddBoardName(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReportAddBoardName();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        public void ExportAccountReportAddBoardName()
        {
            try
            {
                if (dgvAddBoardName_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvAddBoardName_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "BoardName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\AddBoardName.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("AddBoardName");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string BoardName = item.ItemArray[4].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), BoardName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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
                    catch(Exception Ex)
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
