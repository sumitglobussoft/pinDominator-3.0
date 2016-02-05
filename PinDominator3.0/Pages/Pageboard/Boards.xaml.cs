using Globussoft;
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
using BoardManager;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data;
using PinDominator3.CustomUserControl;
using PinDominator3.Classes;
using System.Diagnostics;

namespace PinDominator3.Pages.Pageboard
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Boards : UserControl
    {
        public Boards()
        {
            InitializeComponent();
            BoardsManager.objBoardDelegate = new AccountReport_Board(AccountReport_Board);
            AccountReport_Board();
        }

        QueryManager QM = new QueryManager();
        private void btnBoardUrl_Boards_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {            
                ClGlobul.lstListOfBoardNames.Clear();
                txtBoardUrl.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtBoardUrl.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstListOfBoardNames = GlobusFileHelper.ReadFile(txtBoardUrl.Text.Trim());
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Info("Please Select File");
                }
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        //private void btnBoardName_Board_Browse_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {          
        //        ClGlobul.lstBoardNameswithUserNames.Clear();

        //        txtBoardName.IsReadOnly = true;
        //        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        //        dlg.DefaultExt = ".txt";
        //        dlg.Filter = "Text documents (.txt)|*.txt";
        //        Nullable<bool> result = dlg.ShowDialog();
        //        if (result == true)
        //        {
        //            txtBoardName.Text = dlg.FileName.ToString();
        //        }
        //        ClGlobul.lstBoardNameswithUserNames = GlobusFileHelper.ReadFile(txtBoardName.Text.Trim());
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
        //    }
        //}

        private void btnMessage_Board_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMessage.IsReadOnly = true;
                ClGlobul.lstBoardRepinMessage.Clear();

                txtMessage.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtMessage.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstBoardRepinMessage = GlobusFileHelper.ReadFile(txtMessage.Text.Trim());
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

        BoardsManager objBoardsManager = new BoardsManager();
        Utils.Utils objUtil = new Utils.Utils();

        private void btnBoardCreation_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startBoardCreation();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startBoardCreation()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objBoardsManager.rdbSingleUserBoards == true || objBoardsManager.rdbMultipleUserBoards == true)
                        {
                            if (objBoardsManager.rdbSingleUserBoards == true)
                            {                               
                                try
                                {                                                                    
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
                                }
                            }//end of single user
                            if (objBoardsManager.rdbMultipleUserBoards == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtBoardUrl.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Enter Board Url");
                                        ModernDialog.ShowMessage("Please Enter Board Url", "Enter Board Url", MessageBoxButton.OK);
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
                    Database.UpdateSettingData("Board", "AddBoardMessage", StringEncoderDecoder.Encode(txtMessage.Text));

                    objBoardsManager.isStopBoards = false;
                    objBoardsManager.lstThreadsBoards.Clear();
                    if (objBoardsManager._IsfevoriteBoards)
                    {
                        objBoardsManager._IsfevoriteBoards = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtil.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        try
                        {
                            objBoardsManager.minDelayBoards = Convert.ToInt32(txtBoard_DelayMin.Text);
                            objBoardsManager.maxDelayBoards = Convert.ToInt32(txtBoard_DelayMax.Text);
                            objBoardsManager.NoOfThreadsBoards = Convert.ToInt32(txtBoard_NoOfThreads.Text);
                            objBoardsManager.MaxRePinCount = Convert.ToInt32(txtNoOfPinRepin.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtBoard_NoOfThreads.Text) && checkNo.IsMatch(txtBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objBoardsManager.NoOfThreadsBoards = threads;

                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                        Thread BoardsThread = new Thread(objBoardsManager.StartBoards);
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

        private void btnBoardCreation_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStop = new Thread(stopBoard);
                objStop.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopBoard()
        {
            try
            {
                objBoardsManager._IsfevoriteBoards = true;
                List<Thread> lstTempBoards = objBoardsManager.lstThreadsBoards.Distinct().ToList();
                foreach (Thread item in lstTempBoards)
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

       // UserControl_SingleUser_Boards objUserControl_SingleUser_Boards = new UserControl_SingleUser_Boards();
        //UserControl_SingleUserMessage_Boards objUserControl_SingleUserMessage_Boards = new UserControl_SingleUserMessage_Boards();

        public void closeEvent()
        {

        }

        private void rdbSingleUser_Boards_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserBoards();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
  

        public void rdbSingleUserBoards()
        {
            try
            {
                objBoardsManager.rdbSingleUserBoards = true;
                objBoardsManager.rdbMultipleUserBoards = false;
                btnBoardUrl_Boards_Browse.Visibility = Visibility.Hidden;
                //btnBoardName_Board_Browse.Visibility = Visibility.Hidden;
                btnMessage_Board_Browse.Visibility = Visibility.Hidden;
                txtBoardUrl.Visibility = Visibility.Hidden;
                //txtBoardName.IsReadOnly = false;
                txtMessage.Visibility = Visibility.Hidden;
                lblBoardUrlToRepinFrom_Boards.Visibility = Visibility.Hidden;
                lblMessage_Boards.Visibility = Visibility.Hidden;
                lblHints_Boards.Visibility = Visibility.Hidden;
                ClGlobul.CommentNicheMessageList.Clear();
                ClGlobul.lstListOfBoardNames.Clear();
                 ClGlobul.lstBoardRepinMessage.Clear();
                #region BoardUrl
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter BoardName and BoardUrl Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- Niche::BoardName::BoardUrl";
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
                                    ClGlobul.lstListOfBoardNames.Add(arr_item);
                                }

                            }
                        }
                        GlobusLogHelper.log.Info(" => [ BoardName and BoardUrl with Niche Loaded : " + ClGlobul.lstListOfBoardNames.Count + " ]");
                        GlobusLogHelper.log.Debug("BoardName and BoardUrl with Niche : " + ClGlobul.lstListOfBoardNames.Count);
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }      
                #endregion

                SingleUserMessage();               
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void SingleUserMessage()
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
                                ClGlobul.lstBoardRepinMessage.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Message Loaded : " + ClGlobul.lstBoardRepinMessage.Count + " ]");
                    GlobusLogHelper.log.Debug("Message : " + ClGlobul.lstBoardRepinMessage.Count);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            #endregion
        }

        private void rdbMultipleUser_Boards_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBoardUrl.Text = string.Empty;
                txtMessage.Text = string.Empty;
                objBoardsManager.rdbSingleUserBoards = false;
                objBoardsManager.rdbMultipleUserBoards = true;
                btnBoardUrl_Boards_Browse.Visibility = Visibility.Visible;
               // btnBoardName_Board_Browse.Visibility = Visibility.Visible;
                btnMessage_Board_Browse.Visibility = Visibility.Visible;
                txtBoardUrl.Visibility = Visibility.Visible;
                //txtBoardName.IsReadOnly = false;
                txtMessage.Visibility = Visibility.Visible;
                lblBoardUrlToRepinFrom_Boards.Visibility = Visibility.Visible;
                lblMessage_Boards.Visibility = Visibility.Visible;
                lblHints_Boards.Visibility = Visibility.Visible;
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
        public void AccountReport_Board()
        {
            try
            {
                int id = 0;
                int count = 0;
                string AccountName = string.Empty;
                string ModuleName = string.Empty;
                string BoardName = string.Empty;
                string Pin = string.Empty;
                string Status = string.Empty;
                string DateAndTime = string.Empty;
                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReport.Clear();
                }));
                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("AccountName");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("BoardName");
                dt.Columns.Add("Pin");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("Boards");
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
                        BoardName = dt_item.ItemArray[4].ToString();
                        Pin = dt_item.ItemArray[3].ToString();
                        Status = dt_item.ItemArray[9].ToString();
                        DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(AccountName, ModuleName, BoardName, Pin, Status, DateAndTime);
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
                        objAccountNotifyPropertyChanged.BoardName = BoardName;
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;


                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReport.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReport.Add(objAccountNotifyPropertyChanged);
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
                        dgvBoard_AccountsReport.ItemsSource = AccountViewModel._listAccReport; 
                        //dgvBoard_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void ExportData_Boards(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccountReport();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccountReport()
        {
            try
            {
                if (dgvBoard_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvBoard_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "BoardName", "Pin", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\Boards.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("Boards");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string BoardName = item.ItemArray[4].ToString();
                                        string Pin = item.ItemArray[3].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), BoardName.Replace("'", ""), Pin.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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

        private void DeleteData_Boards(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("Boards");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_Board();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

     
    }
}
