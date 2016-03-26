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
using System.Collections;
using PinDominator.CustomUserControl;

namespace PinDominator.Pages.Pageboard
{

    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddUsersToBoard : UserControl
    {
        public AddUsersToBoard()
        {
            InitializeComponent();
            AddUsersToBoardManager.objAddUserToBoarddelegate = new AccountReport_AddUserToBoard(AccountReport_AddUserToBoard);
            AccountReport_AddUserToBoard();
        }

        AddUsersToBoardManager objAddUsersToBoardManager = new AddUsersToBoardManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnUserNames_AddUsersToBoard_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstAddToBoardUserNames.Clear();
                txtEmailOrUserNames.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtEmailOrUserNames.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtEmailOrUserNames.Text.Trim());
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnStart_AddToBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startAddUserToBoard();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startAddUserToBoard()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objAddUsersToBoardManager.rdbSingleUserAddUserToBoard == true || objAddUsersToBoardManager.rdbMultipleUserAddUserToBoard == true)
                        {
                            if (objAddUsersToBoardManager.rdbSingleUserAddUserToBoard == true)
                            {
                                try
                                {                                  
                                    if (string.IsNullOrEmpty(txtBoardName.Text)) // && string.IsNullOrEmpty(objUserControl_SingleUserEmailOrUsername_AddUserToBoard.txtEmailorUsername_SingleUser_AddUserToBoard.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Enter BoardName ");
                                        ModernDialog.ShowMessage("Please Enter BoardName ", "Please Enter BoardName ", MessageBoxButton.OK);
                                        return;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(txtBoardName.Text))
                                        {
                                            objAddUsersToBoardManager.BoardName = txtBoardName.Text.Trim();
                                        }                                     
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of single User
                            if (objAddUsersToBoardManager.rdbMultipleUserAddUserToBoard == true)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(txtBoardName.Text))
                                    {
                                        objAddUsersToBoardManager.BoardName = txtBoardName.Text.Trim();
                                    }
                                    else
                                    {
                                        GlobusLogHelper.log.Info("Please Enter Board Name");
                                        ModernDialog.ShowMessage("Please Enter Board Name", "Enter Board Name", MessageBoxButton.OK);
                                        return;
                                    }
                                    if (string.IsNullOrEmpty(txtEmailOrUserNames.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Email or UserName");
                                        ModernDialog.ShowMessage("Please Upload Email or UserName", "Upload Email or UserName", MessageBoxButton.OK);
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
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    //objAddUsersToBoardManager.BoardName = txtBoardName.Text.Trim();

                    objAddUsersToBoardManager.isStopAddUserToBoard = false;
                    objAddUsersToBoardManager.lstThreadsAddUserToBoard.Clear();
                    if (objAddUsersToBoardManager._IsfevoriteAddUserToBoard)
                    {
                        objAddUsersToBoardManager._IsfevoriteAddUserToBoard = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        try
                        {
                            objAddUsersToBoardManager.minDelayAddUserToBoard = Convert.ToInt32(txtUserBoard_DelayMin.Text);
                            objAddUsersToBoardManager.maxDelayAddUserToBoard = Convert.ToInt32(txtUserBoard_DelayMax.Text);
                            objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = Convert.ToInt32(txtUserBoard_NoOfThreads.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtUserBoard_NoOfThreads.Text) && checkNo.IsMatch(txtUserBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtUserBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = threads;

                        clsSettingDB Database = new clsSettingDB();
                        Database.UpdateSettingData("UserToBoard", "UserToBoard", StringEncoderDecoder.Encode(txtEmailOrUserNames.Text));

                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                        Thread AddUserToBoardThread = new Thread(objAddUsersToBoardManager.StartAddUsersToBoard);
                        AddUserToBoardThread.Start();
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

        private void btnStop_AddToBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopAddUserToBoard = new Thread(stopAddUsersToBoard);
                objStopAddUserToBoard.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopAddUsersToBoard()
        {
            try
            {
                objAddUsersToBoardManager._IsfevoriteAddUserToBoard = true;
                List<Thread> lstTemp = objAddUsersToBoardManager.lstThreadsAddUserToBoard.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" => [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

      
        public void closeEvent()
        {

        }

        private void rdbSingleUser_AddUserToBoard_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SingleUserAddUserToBoard();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void SingleUserAddUserToBoard()
        {
            try
            {
                objAddUsersToBoardManager.rdbSingleUserAddUserToBoard = true;
                objAddUsersToBoardManager.rdbMultipleUserAddUserToBoard = false;
                btnUserNames_AddUsersToBoard_Browse.Visibility = Visibility.Hidden;
                txtEmailOrUserNames.Visibility = Visibility.Hidden;
                lblEmailOrUsername.Visibility = Visibility.Hidden;
                ClGlobul.lstAddToBoardUserNames.Clear();
                #region Email or Username
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Email or UserName Here ";
                    //obj.txtEnterSingleMessages.ToolTip = "Format :-  Niche:Board Name 1 , Board Name 2";
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
                        else
                        {
                            GlobusLogHelper.log.Info("Please Upload Email or Username");
                            ModernDialog.ShowMessage("Please Upload Email or Username", "Upload Email or Username", MessageBoxButton.OK);
                            return;
                        }
                        GlobusLogHelper.log.Info("Email or UserName Loaded : " + ClGlobul.lstAddToBoardUserNames.Count);
                        GlobusLogHelper.log.Debug("Email or UserName Loaded : " + ClGlobul.lstAddToBoardUserNames.Count);
                    }           
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
                #endregion   
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbMultipleUser_AddUserToBoard_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBoardName.Text = string.Empty;
                txtEmailOrUserNames.Text = string.Empty;
                objAddUsersToBoardManager.rdbMultipleUserAddUserToBoard = true;
                objAddUsersToBoardManager.rdbSingleUserAddUserToBoard = false;
                btnUserNames_AddUsersToBoard_Browse.Visibility = Visibility.Visible;
                txtEmailOrUserNames.IsReadOnly = true;
                txtEmailOrUserNames.Visibility = Visibility.Visible;
                lblEmailOrUsername.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_AddUserToBoard()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet ds = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("BoardName");
                dt.Columns.Add("UserName");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                ds = new DataSet();
                ds.Tables.Add(dt);
                try
                {
                    ds = QM.SelectAddReport("AddUserToBoard");
                }
                catch (Exception ex)
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
                        string UserName = dt_item.ItemArray[5].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, BoardName, UserName, Status, DateAndTime);
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
                        dgvAddUsersToBoard_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkExportData_AddUserToBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                exportData_AddUserToBoard();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }           
        }

        public void exportData_AddUserToBoard()
        {
            try
            {
                if ( dgvAddUsersToBoard_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvAddUsersToBoard_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "BoardName", "UserName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\AddUserToBoard.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("AddUserToBoard");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string BoardName = item.ItemArray[4].ToString();
                                        string UserName = item.ItemArray[5].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), BoardName.Replace("'", ""), UserName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkDeleteData_AddUserToBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("AddUserToBoard");
                    GlobusLogHelper.log.Info(" => [All Data is Deleted ] ");
                }
                AccountReport_AddUserToBoard();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }



    }
}
