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
using System.Threading;
using Globussoft;
using System.Text.RegularExpressions;
using BasePD;
using PinsManager;
using FirstFloor.ModernUI.Windows.Controls;
using System.Data;
using PinDominator.CustomUserControl;


namespace PinDominator.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddPinWithnewBoard : UserControl
    {
        public AddPinWithnewBoard()
        {
            InitializeComponent();
            AddPinWithNewBoardManager.objAddPinWithBoardDelegate = new AccountReport_AddPinWithBoard(AccountReport_AddPinWithBoard);
            AccountReport_AddPinWithBoard();
        }
        AddPinWithNewBoardManager objAddPinWithNewBoardManager = new AddPinWithNewBoardManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnUploadPinFile_AddPinWithnewBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtaddPinwithNewBoard.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtaddPinwithNewBoard.Text = ofd.FileName.ToString();
                    Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFileofAddNewBoardWithNewPin);
                    ReadLargeFileThread.Start(ofd.FileName);
                }    
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void ReadLargeNewPinsFileofAddNewBoardWithNewPin(object filePath)
        {
            try
            {
                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddNewPinsListofAddNewBoardWithNewPin(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void AddNewPinsListofAddNewBoardWithNewPin(List<string> Messages)
        {
            try
            {
                ClGlobul.lstListOfNewUsers.Clear();
              Dispatcher.Invoke((Action)delegate
                {
                    foreach (string Message in Messages)
                    {
                        string NewMessages = Message.Replace("\0", "").Trim();
                        string[] arMessages = Regex.Split(NewMessages, ",");

                        BaseLib.Pins pin = new BaseLib.Pins();

                        if (arMessages.Count() == 1)
                        {
                            pin.ImageUrl = arMessages[0];
                        }
                        else if (arMessages.Count() == 2)
                        {
                            pin.ImageUrl = arMessages[0];
                            pin.Description = arMessages[1];
                        }
                        else if (arMessages.Count() == 3)
                        {
                            pin.Board = arMessages[0];
                            pin.Description = arMessages[1];
                            pin.ImageUrl = (arMessages[2]).Trim();
                        }
                        else if (arMessages.Count() == 4)
                        {
                            pin.Board = arMessages[0];
                            pin.Description = arMessages[1];
                            pin.ImageUrl = (arMessages[2]).Trim();
                            pin.Niche = (arMessages[3]).Trim();

                        }
                        if (!string.IsNullOrEmpty(pin.ImageUrl))
                        {
                            ClGlobul.lst_AddnewPinWithNewBoard.Add(pin);
                        }

                    }

                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

       

        private void btnAddPinWithnewBoard_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startAddPinWithNewBoard();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startAddPinWithNewBoard()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (rdbSingleUser_AddPinWithNewBoard.IsChecked == true || rdbMultipleUser_AddPinWithNewBoard.IsChecked == true)
                        {
                            if (rdbSingleUser_AddPinWithNewBoard.IsChecked == true)
                            {
                               // AddNewPinsListofAddNewBoardWithNewPin(lstBoardDesc);
                            }
                            if (rdbMultipleUser_AddPinWithNewBoard.IsChecked == true)
                            {
                                if (string.IsNullOrEmpty(txtaddPinwithNewBoard.Text))
                                {
                                    GlobusLogHelper.log.Info("Please Upload Pin File");
                                    ModernDialog.ShowMessage("Please Upload Pin File", "Upload Pin File", MessageBoxButton.OK);
                                    return;
                                }
                            }
                            
                        }                   
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objAddPinWithNewBoardManager.isStopAddPinWithNewBoard = false;
                    objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Clear();

                    if (objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard)
                    {
                        objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    //ClGlobul.addNewPinWithBoard = GlobusFileHelper.ReadFile(txtaddPinwithNewBoard.Text.Trim());

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddPinWithNewBoardManager.minDelayAddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_DelayMin.Text);
                            objAddPinWithNewBoardManager.maxDelayAddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_DelayMax.Text);
                            objAddPinWithNewBoardManager.Nothread_AddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_NoOfThreads.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtAddPinwithnewBoard_NoOfThreads.Text) && checkNo.IsMatch(txtAddPinwithnewBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtAddPinwithnewBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                        objAddPinWithNewBoardManager.NoOfThreadsAddPinWithNewBoard = threads;

                        Thread AddPinWithNewBoardThread = new Thread(objAddPinWithNewBoardManager.StartAddPinWithNewBoard);
                        AddPinWithNewBoardThread.Start();
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

        private void btnAddPinWithnewBoard_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopAddPinWithNewBoard = new Thread(stopAddPinWithNewBoard);
                objStopAddPinWithNewBoard.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopAddPinWithNewBoard()
        {
            try
            {
                objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = true;
                List<Thread> lstTemp = objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_AddPinWithBoard()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("BoardName");
                dt.Columns.Add("Message");
                dt.Columns.Add("Status");
                dt.Columns.Add("ImageUrl");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("AddPinWithNewBoard");
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
                        string BoardName = dt_item.ItemArray[4].ToString();
                        string Message = dt_item.ItemArray[6].ToString();
                        string Status = dt_item.ItemArray[9].ToString();
                        string ImageUrl = dt_item.ItemArray[8].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, BoardName, Message, Status, ImageUrl, DateAndTime);
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
                        dgvAddPinWithnewBoard_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkDeleteAccReport_AddPinWithBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("AddPinWithNewBoard");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_AddPinWithBoard();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_AddPinWithBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportAddPinWithBoard();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportAddPinWithBoard()
        {
            try
            {
                if (dgvAddPinWithnewBoard_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvAddPinWithnewBoard_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "BoardName", "Message", "Status", "ImageUrl", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\AddPinWithNewBoard.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("AddPinWithNewBoard");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string BoardName = item.ItemArray[4].ToString();
                                        string Message = item.ItemArray[6].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string ImageUrl = item.ItemArray[8].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), BoardName.Replace("'", ""), Message.Replace("'", ""), Status.Replace("'", ""), ImageUrl.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        public void closeEvent()
        {

        }

        private void rdbSingleUser_AddPinWithNewBoard_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                SingleUserAddPinWithNewBoard();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        List<string> lstBoardDesc = new List<string>();
        public void SingleUserAddPinWithNewBoard()
        {
            try
            {
                btnUploadPinFile_AddPinWithnewBoard.Visibility = Visibility.Hidden;
                txtaddPinwithNewBoard.Visibility = Visibility.Hidden;
                lblPinFile_AddPinWithnewBoard.Visibility = Visibility.Hidden;
                lblHint_AddPinWithnewBoard.Visibility = Visibility.Hidden;
                lstBoardDesc.Clear();

                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter User Here ";
                obj.txtEnterSingleMessages.ToolTip = "Format :- BoardName,Description,ImageUrl,Niche";
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
                                lstBoardDesc.Add(arr_item);
                            }

                        }
                      
                    }
                    GlobusLogHelper.log.Info(" => [ User Loaded : " + lstBoardDesc.Count + " ]");
                    GlobusLogHelper.log.Debug("User : " + lstBoardDesc.Count);
                    new Thread(() =>
                    {
                        AddNewPinsListofAddNewBoardWithNewPin(lstBoardDesc);
                    }).Start();
                } 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void rdbMultipleUser_AddPinWithNewBoard_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnUploadPinFile_AddPinWithnewBoard.Visibility = Visibility.Visible;
                txtaddPinwithNewBoard.Visibility = Visibility.Visible;
                lblPinFile_AddPinWithnewBoard.Visibility = Visibility.Visible;
                lblHint_AddPinWithnewBoard.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }



    }
}
