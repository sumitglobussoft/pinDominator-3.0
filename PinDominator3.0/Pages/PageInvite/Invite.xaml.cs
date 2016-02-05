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
using System.Text.RegularExpressions;
using BasePD;
using System.Threading;
using FirstFloor.ModernUI.Windows.Controls;
using InviteManager;
using System.Data;
using PinDominator3.CustomUserControl;

namespace PinDominator3.Pages.PageInvite
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Invite : UserControl
    {
        public Invite()
        {
            InitializeComponent();
            AccountReport_Invite();
        }
        InviteManagers objInviteManagers = new InviteManagers();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();
        
        private void btnInviteEmail_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtInviteEmail.IsReadOnly = true;
                ClGlobul.lstEmailInvites.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtInviteEmail.Text = ofd.FileName.ToString();
                    readInviteEmailFile(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public static bool IsValidEmailAddressByRegex(string mailAddress)
        {
            Regex mailIDPattern = new Regex(@"[\w-]+@([\w-]+\.)+[\w-]+");

            if (!string.IsNullOrEmpty(mailAddress) && mailIDPattern.IsMatch(mailAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void readInviteEmailFile(object filePath)
        {
            try
            {
                List<string> Temp_invitelist = new List<string>();
                ClGlobul.lstEmailInvites = GlobusFileHelper.ReadFile(txtInviteEmail.Text.Trim());
                try
                {
                    foreach (string item_Emailinvites in ClGlobul.lstEmailInvites)
                    {
                        if (!item_Emailinvites.Contains(":"))
                        {
                            bool CheckEmail = IsValidEmailAddressByRegex(item_Emailinvites);
                            if (CheckEmail == true)
                            {
                                Temp_invitelist.Add(item_Emailinvites);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [  " + item_Emailinvites + "  is an invalid Email Id]");
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [  " + item_Emailinvites + "  is not in Proper format]");
                        }
                    }
                    ClGlobul.lstEmailInvites = Temp_invitelist;
                }
                catch(Exception ex)
                {
                    GlobusLogHelper.log.Info(" Please Select File ");
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

      

        private void btnInviteEmailStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startInviteEmail();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void startInviteEmail()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objInviteManagers.rdbSingleUserInvite == true || objInviteManagers.rdbMultipleUserInvite == true)
                        {
                            if (objInviteManagers.rdbSingleUserInvite == true)
                            {
                                try
                                {                                   
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }
                            }//end of single User
                            if (objInviteManagers.rdbMultipleUserInvite == true)
                            {
                                try
                                {
                                    if (string.IsNullOrEmpty(txtInviteEmail.Text))
                                    {
                                        GlobusLogHelper.log.Info("Please Upload Email ");
                                        ModernDialog.ShowMessage("Please Upload Email", "Upload Email", MessageBoxButton.OK);
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

                        objInviteManagers.isStopInvite = false;
                        objInviteManagers.lstThreadsInvite.Clear();

                        if (objInviteManagers._IsfevoriteInvite)
                        {
                            objInviteManagers._IsfevoriteInvite = false;
                        }


                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objInviteManagers.minDelayInvite = Convert.ToInt32(txtInviteEmail_DelayMin.Text);
                                objInviteManagers.maxDelayInvite = Convert.ToInt32(txtInviteEmail_DelayMax.Text);
                                objInviteManagers.Nothread_Invite = Convert.ToInt32(txtInviteEmail_NoOfThreads.Text);

                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtInviteEmail_NoOfThreads.Text) && checkNo.IsMatch(txtInviteEmail_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtInviteEmail_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }

                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                            objInviteManagers.NoOfThreadsInvite = threads;

                            Thread InviteThread = new Thread(objInviteManagers.StartInvite);
                            InviteThread.Start();
                        }

                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
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

        private void btnInviteEmailStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopInviteEmail = new Thread(stopInviteEmail);
                objStopInviteEmail.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopInviteEmail()
        {
            try
            {
                objInviteManagers._IsfevoriteInvite = true;
                List<Thread> lstTempInvite = objInviteManagers.lstThreadsInvite.Distinct().ToList();
                foreach (Thread item in lstTempInvite)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("-----------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-----------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
   
        public void closeEvent()
        { }
        private void rdbSingleUser_Invite_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserInvite();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserInvite()
        {
            try
            {
                objInviteManagers.rdbSingleUserInvite = true;
                objInviteManagers.rdbMultipleUserInvite = false;
                btnInviteEmail_Browse.Visibility = Visibility.Hidden;
                txtInviteEmail.Visibility = Visibility.Hidden;
                lbInviteEmail.Visibility = Visibility.Hidden;
                ClGlobul.lstEmailInvites.Clear();
                try
                {
                    UserControl_SingleUser obj = new UserControl_SingleUser();
                    obj.UserControlHeader.Text = "Enter Email Here ";
                    obj.txtEnterSingleMessages.ToolTip = "Format :- marykslavin@gmail.com";
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
                                    ClGlobul.lstEmailInvites.Add(arr_item);
                                }

                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Email Loaded : " + ClGlobul.lstEmailInvites.Count + " ]");
                        GlobusLogHelper.log.Debug("Email : " + ClGlobul.lstEmailInvites.Count);
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

        private void rdbMultipleUser_Invite_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtInviteEmail.Text = string.Empty;
                objInviteManagers.rdbMultipleUserInvite = true;
                objInviteManagers.rdbSingleUserInvite = false;
                btnInviteEmail_Browse.Visibility = Visibility.Visible;
                txtInviteEmail.IsReadOnly = true;
                txtInviteEmail.Visibility = Visibility.Visible;
                lbInviteEmail.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_Invite()
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
                dt.Columns.Add("Username");           
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("Invite");
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
                        string UserName = dt_item.ItemArray[6].ToString();                  
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, AccountName, ModuleName, UserName, Status, DateAndTime);
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
                        dgvInvite_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkDeleteAccReport_Invite(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("Invite");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_Invite();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ClkExportData_Invite(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportInvite();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        public void ExportAccReportInvite()
        {
            try
            {
                if (dgvInvite_AccountsReport.Items.Count == 1)
                {
                     GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvInvite_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "AccountName", "ModuleName", "UserName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\Invite.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("Invite");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string AccountName = item.ItemArray[1].ToString();
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string UserName = item.ItemArray[5].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", AccountName.Replace("'", ""), ModuleName.Replace("'", ""), UserName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
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






    }
}
