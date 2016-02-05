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
using FirstFloor.ModernUI.Windows.Controls;
using PinsManager;
using System.Data;

namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddNewPin : UserControl
    {
        public AddNewPin()
        {
            InitializeComponent();
            AddNewPinManager.objAddNewPinDelegate = new AccountReport_AddNewPin(AccountReport_AddNewPin);
            AccountReport_AddNewPin();
        }
        QueryManager QM = new QueryManager();
        AddNewPinManager objAddNewPinManager = new AddNewPinManager();
        Utils.Utils objUtils = new Utils.Utils();
        private void btnNewFile_AddNewPin_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheWeb_AddNewPin.IsChecked == true)
                {
                    try
                    {
                        ClGlobul.lstListOfPins.Clear();
                        txtNewPin.IsReadOnly = true;
                        Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                        ofd.DefaultExt = ".txt";
                        ofd.Filter = "Text documents (.txt)|*.txt";
                        Nullable<bool> result = ofd.ShowDialog();
                        if (result == true)
                        {
                            txtNewPin.Text = ofd.FileName.ToString();
                            Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFile);
                            ReadLargeFileThread.Start(ofd.FileName);
                            //  readMessageFile(dlg.FileName);
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
                //else
                //{
                //    try
                //    {
                //        ClGlobul.lstListofFiles.Clear();
                //        Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                //        ofd.Multiselect = true;
                //        Nullable<bool> result = ofd.ShowDialog();
                //        if (result == true)
                //        {
                //            //txtNewPin.Text = ofd.FileName.ToString();
                //            foreach (var item in ofd.FileNames)
                //            {
                //                ClGlobul.lstListofFiles.Add(item);
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void ReadLargeNewPinsFile(object filePath)
        {
            try
            {
                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddNewPinsList(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }
        private void AddNewPinsList(List<string> Messages)
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
                                       pin.ImageUrl = arMessages[0];
                                       pin.Description = arMessages[1];
                                       pin.Board = arMessages[2];
                                   }
                                   else if (arMessages.Count() == 4)
                                   {
                                       pin.ImageUrl = arMessages[0];
                                       pin.Description = arMessages[1];
                                       pin.Board = arMessages[2];
                                       pin.Email = arMessages[3];
                                   }
                                   if (!string.IsNullOrEmpty(pin.ImageUrl))
                                   {
                                       ClGlobul.lstListOfPins.Add(pin);
                                   }

                               }

                           });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }



        private void btnAddNewPin_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startAddNewPin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startAddNewPin()
        {
            List<string> lstSingleUserAddnewpin = new List<string>();
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objAddNewPinManager.rdbYourDeviceAddNewPin == true || objAddNewPinManager.rdbTheWebAddNewPin == true)
                        {
                            objAddNewPinManager.WebsiteUrl = txtWebsite_AddNewPin.Text.Trim();
                            if (objAddNewPinManager.rdbYourDeviceAddNewPin == true)
                            {
                                try
                                {
                                    ClGlobul.lstListOfPins.Clear();
                                    if (string.IsNullOrEmpty(txtNewPin.Text.Trim()))
                                    {
                                        //objAddNewPinManager.SinglePin_AddNewPin = txtNewPin.Text;
                                        #region commented
                                        //string[] arrPins = Regex.Split(objAddNewPinManager.SinglePin_AddNewPin, ",");
                                        //Pins objpin = new Pins();
                                        //objpin.ImageUrl = arrPins[0];
                                        //objpin.Description = arrPins[1];
                                        //objpin.Board = arrPins[2];
                                        //objpin.Email = arrPins[3];
                                        //ClGlobul.lstListOfPins.Add(objpin);
                                        // ClGlobul.lstListOfPins = lstSingleUserAddnewpin;
                                        #endregion
                                        GlobusLogHelper.log.Info("Please Give Board Name.");
                                        ModernDialog.ShowMessage("Please Give Board Name.", "Give BoardName", MessageBoxButton.OK);
                                        return;
                                    }                                  
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of single User
                            if (objAddNewPinManager.rdbTheWebAddNewPin == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtNewPin.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Pin File");
                                        ModernDialog.ShowMessage("Please Upload Pin File", "Upload Pin File", MessageBoxButton.OK);
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
                    objAddNewPinManager.isStopAddNewPin = false;
                    objAddNewPinManager.lstThreadsAddNewPin.Clear();

                    if (objAddNewPinManager._IsfevoriteAddNewPin)
                    {
                        objAddNewPinManager._IsfevoriteAddNewPin = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddNewPinManager.minDelayAddNewPin = Convert.ToInt32(txtAddNewPin_DelayMin.Text);
                            objAddNewPinManager.maxDelayAddNewPin = Convert.ToInt32(txtAddNewPin_DelayMax.Text);
                            objAddNewPinManager.Nothread_AddNewPin = Convert.ToInt32(txtAddNewPin_NoOfThreads.Text);
                            objAddNewPinManager.MaxCountAddPin = Convert.ToInt32(txtMaxCount_AddPin.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtAddNewPin_NoOfThreads.Text) && checkNo.IsMatch(txtAddNewPin_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtAddNewPin_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        GlobusLogHelper.log.Info(" => [ Process Starting ] ");
                        objAddNewPinManager.NoOfThreadsAddNewPin = threads;

                        Thread LikeThread = new Thread(objAddNewPinManager.StartAddNewPin);
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
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnAddNewPin_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopAddNewPin = new Thread(stopAddNewPin);
                objStopAddNewPin.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopAddNewPin()
        {
            try
            {
                objAddNewPinManager._IsfevoriteAddNewPin = true;
                List<Thread> lstTempAddNewPin = objAddNewPinManager.lstThreadsAddNewPin.Distinct().ToList();
                foreach (Thread item in lstTempAddNewPin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("---------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void YourDevice_AddNewPin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddNewPinManager.rdbYourDeviceAddNewPin = true;
                objAddNewPinManager.rdbTheWebAddNewPin = false;
                btnNewFile_AddNewPin_Browse.Visibility = Visibility.Hidden;
                btnBrowsePhoto_AddNewPin.Visibility = Visibility.Visible;
               // txtSelectPhoto.Visibility = Visibility.Visible;
                txtNewPin.IsReadOnly = true;
                lblImageUrl.Visibility = Visibility.Hidden;
                lblSelectPhoto.Visibility = Visibility.Visible;
                btnBrowseBoardName_AddNewPin.Visibility = Visibility.Visible;  
                lblBoard.Visibility = Visibility.Visible;
                lblBoardName.Content = "Board Name :";
                lblWebsite.Visibility = Visibility.Visible;
                txtWebsite_AddNewPin.Visibility = Visibility.Visible;  
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void TheWeb_AddNewPin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddNewPinManager.rdbYourDeviceAddNewPin = false;
                objAddNewPinManager.rdbTheWebAddNewPin = true;
                btnNewFile_AddNewPin_Browse.Visibility = Visibility.Visible;
                btnBrowsePhoto_AddNewPin.Visibility = Visibility.Hidden;
                txtNewPin.IsReadOnly = true;
                lblImageUrl.Visibility = Visibility.Visible;
                lblSelectPhoto.Visibility = Visibility.Hidden;
                btnBrowseBoardName_AddNewPin.Visibility = Visibility.Hidden;
                lblBoard.Visibility = Visibility.Hidden;
                lblBoardName.Content = "Pin File :";
                lblWebsite.Visibility = Visibility.Hidden;
                txtWebsite_AddNewPin.Visibility = Visibility.Hidden; 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_AddNewPin()
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
                    DS = QM.SelectAddReport("AddNewPin");
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
                        dt.Rows.Add(count,AccountName, ModuleName, BoardName, Message, Status, ImageUrl, DateAndTime);
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
                        dgvAddNewPin_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkDeleteAccReport_AddNewPin(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("AddNewPin");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_AddNewPin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_AddNewPin(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportAddNewPin();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportAddNewPin()
        {
            try
            {
                if (dgvAddNewPin_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvAddNewPin_AccountsReport.Items.Count > 1)
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
                                FilePath = FilePath + "\\AddNewPin.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("AddNewPin");
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

        private void btnBrowsePhoto_AddNewPin_Click(object sender, RoutedEventArgs e)
        {
            if (YourDevice_AddNewPin.IsChecked == true)
            {
                try
                {
                    ClGlobul.lstListofFiles.Clear();
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                    ofd.Multiselect = true;
                    Nullable<bool> result = ofd.ShowDialog();
                    if (result == true)
                    {
                       // txtSelectPhoto.Text = ofd.FileName.ToString();
                      //  ClGlobul.lstListofFiles = GlobusFileHelper.ReadFile(txtSelectPhoto.Text.Trim());
                        foreach (var item in ofd.FileNames)
                        {
                            ClGlobul.lstListofFiles.Add(item);
                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Total Uploaded Image Is " + ClGlobul.lstListofFiles.Count + " ]");
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void btnBrowseBoardName_AddNewPin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //lstBoardNameNiche_AddNewPin
                ClGlobul.lstBoardNameNiche_AddNewPin.Clear();

                txtNewPin.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtNewPin.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstBoardNameNiche_AddNewPin = GlobusFileHelper.ReadFile(txtNewPin.Text.Trim());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
                GlobusLogHelper.log.Info(" => [ Total Uploaded BoardName Is " + ClGlobul.lstBoardNameNiche_AddNewPin.Count + " ]");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }



    }
}
