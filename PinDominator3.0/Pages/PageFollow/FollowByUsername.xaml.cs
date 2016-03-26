using System;
using System.Collections;
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
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using FollowManagers;
using PinDominator.CustomUserControl;
using System.Data;

namespace PinDominator.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class FollowByUsername : UserControl
    {
        public FollowByUsername()
        {
            InitializeComponent();
            FollowByUsernameManager.objFollowByUsernameDelegate = new AccountReport_FollowByUsername(AccountReport_FollowByUsername);
            AccountReport_FollowByUsername();
        }


        FollowByUsernameManager objFollowByUsernameManager = new FollowByUsernameManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();
        
        private void btnUsername_FollowByUsername_Browse_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                ClGlobul.ListOfFollowUsersFollowers.Clear();
                ClGlobul.FollowUsersFollowerQueue.Clear();
                
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtUsernameUpload.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.ListOfFollowUsersFollowers = GlobusFileHelper.ReadFile(txtUsernameUpload.Text.Trim());
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

        private void btnFollowUser_FollowByUsername_Browse_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                ClGlobul.lstFollowUsername.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtFollowUserUpload.Text = dlg.FileName.ToString();
                }
                try
                {
                    ClGlobul.lstFollowUsername = GlobusFileHelper.ReadFile(txtFollowUserUpload.Text.Trim());
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

        private void rdoFollowUser_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rbFollowUser = true;
                btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
            }
            catch { };
            try
            {
                txtUsernameUpload.IsReadOnly = true;
                txtFollowUserUpload.IsReadOnly = true;
            }
            
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rdoFollowFollowers_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rbFollowUser = true;
                FollowByUsernameManager.rbFollowFollowers = false;
                if (objFollowByUsernameManager.rdbSingleUserFollowByUsername == true)
                {
                    try
                    {
                        FollowByUsernameManager.rdoBtnFollowUserUploaded = false;
                        btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                        btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                        rdbSingleUserFollowByUsername();
                    }
                    catch { };
                    try
                    {
                        txtUsernameUpload.IsReadOnly = true;
                        txtFollowUserUpload.IsReadOnly = false;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
                if (objFollowByUsernameManager.rdbMultipleUserFollowByUsername == true)
                {
                    try
                    {
                        FollowByUsernameManager.rdoBtnFollowUserUploaded = false;
                        btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                        btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Visible;                       
                        txtUsernameUpload.IsReadOnly = true;
                        txtFollowUserUpload.IsReadOnly = true;
                        txtUsernameUpload.Visibility = Visibility.Visible;
                        txtFollowUserUpload.Visibility = Visibility.Visible;
                        lblUsername.Visibility = Visibility.Visible;
                        lblFollowUsers.Visibility = Visibility.Visible;
                    }
                    catch { };
                    try
                    {
                        txtUsernameUpload.IsReadOnly = true;
                        txtFollowUserUpload.IsReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
    

        private void rdoFollowUserUploaded_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rbFollowUser = false;
                FollowByUsernameManager.rbFollowFollowers = true;
                if (objFollowByUsernameManager.rdbSingleUserFollowByUsername == true)
                {
                    try
                    {
                        FollowByUsernameManager.rdoBtnFollowUserUploaded = false;
                        btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                        btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                        rdbSingleUserFollowByUsername();
                    }
                    catch { };
                    try
                    {
                        txtUsernameUpload.IsReadOnly = false;
                        txtFollowUserUpload.IsReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
                if (objFollowByUsernameManager.rdbMultipleUserFollowByUsername == true)
                {
                    try
                    {
                        btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                        btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;          
                        txtUsernameUpload.IsReadOnly = true;
                        txtFollowUserUpload.IsReadOnly = true;
                        txtUsernameUpload.Visibility = Visibility.Visible;
                        txtFollowUserUpload.Visibility = Visibility.Visible;
                        lblUsername.Visibility = Visibility.Visible;
                        lblFollowUsers.Visibility = Visibility.Visible;
                    }
                    catch { };
                    try
                    {
                        txtUsernameUpload.IsReadOnly = true;
                        txtFollowUserUpload.IsReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btnFollowByUsername_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 startFollowByUsername();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

       public void startFollowByUsername()
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (objFollowByUsernameManager.rdbSingleUserFollowByUsername == true || objFollowByUsernameManager.rdbMultipleUserFollowByUsername == true)
                        {
                            try
                            {
                                #region Single User
                                if (objFollowByUsernameManager.rdbSingleUserFollowByUsername == true)
                                {
                                    try
                                    {
                                        if (rdoFollowUserUploaded.IsChecked == true || rdoFollowFollowers.IsChecked == true)
                                        {
                                            if (rdoFollowUserUploaded.IsChecked == true)
                                            {                                                                                            
                                            }
                                            if (rdoFollowFollowers.IsChecked == true)
                                            {                                              
                                            }
                                        }
                                        else
                                        {
                                            GlobusLogHelper.log.Info("Please Select Follow Upload User or Follow users Follower");
                                            ModernDialog.ShowMessage("Please Select Follow Upload User or Follow users Follower", "Follow Upload User or Follow users Follower", MessageBoxButton.OK);
                                            return;
                                        }
                                    }
                                    catch (Exception ex)
                                    { };
                                }
                                #endregion

                                #region Multiple User
                                if (objFollowByUsernameManager.rdbMultipleUserFollowByUsername == true)
                                {
                                    try
                                    {
                                        if (rdoFollowUserUploaded.IsChecked == true || rdoFollowFollowers.IsChecked == true)
                                        {
                                            if (rdoFollowUserUploaded.IsChecked == true)
                                            {
                                                if (string.IsNullOrEmpty(txtUsernameUpload.Text))
                                                {
                                                    GlobusLogHelper.log.Info("Please Upload Username ");
                                                    ModernDialog.ShowMessage("Please Upload Username", "Upload Username", MessageBoxButton.OK);
                                                    return;
                                                }
                                                else
                                                {
                                                    foreach (var itemListOfFollowUsersFollowers in ClGlobul.ListOfFollowUsersFollowers)
                                                    {
                                                        objFollowByUsernameManager.FollowUsersFollowerQueue.Enqueue(itemListOfFollowUsersFollowers);
                                                    }
                                                }

                                            }

                                            if (rdoFollowFollowers.IsChecked == true)
                                            {
                                                if (string.IsNullOrEmpty(txtFollowUserUpload.Text))
                                                {
                                                    GlobusLogHelper.log.Info("Please Upload Follow User ");
                                                    ModernDialog.ShowMessage("Please Upload Follow User", "Upload Follow User", MessageBoxButton.OK);
                                                    return;
                                                }

                                            }
                                        }
                                        else
                                        {
                                            GlobusLogHelper.log.Info("Please Select Follow Upload User or Follow users Follower");
                                            ModernDialog.ShowMessage("Please Select Follow Upload User or Follow users Follower", "Follow Upload User or Follow users Follower", MessageBoxButton.OK);
                                            return;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Select Use Single User or Use Multiple User");
                            ModernDialog.ShowMessage("Please Select Use Single User or Use Multiple User", "Select Use Single User or Use Multiple User", MessageBoxButton.OK);
                            return;
                        }

                        objFollowByUsernameManager.isStopFollowByUsername = false;
                        objFollowByUsernameManager.lstThreadsFollowByUsername.Clear();

                        if (objFollowByUsernameManager._IsfevoriteFollowByUsername)
                        {
                            objFollowByUsernameManager._IsfevoriteFollowByUsername = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objFollowByUsernameManager.minDelayFollowByUsername = Convert.ToInt32(txtFollowByUsername_DelayMin.Text);
                                objFollowByUsernameManager.maxDelayFollowByUsername = Convert.ToInt32(txtFollowByUsername_DelayMax.Text);
                                objFollowByUsernameManager.Nothread_FollowByUsername = Convert.ToInt32(txtFollowByUsername_NoOfThreads.Text);
                                objFollowByUsernameManager.MaxFollowCount = Convert.ToInt32(txtFollowerCount.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtFollowUserUpload.Text));

                            if (!string.IsNullOrEmpty(txtFollowByUsername_NoOfThreads.Text) && checkNo.IsMatch(txtFollowByUsername_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtFollowByUsername_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }

                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");

                            objFollowByUsernameManager.NoOfThreadsFollowByUsername = threads;

                            Thread FollowUsernameThread = new Thread(objFollowByUsernameManager.StartFollowByUsername);
                            FollowUsernameThread.Start();
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

        private void btnFollowByUsername_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopFollowByUsername = new Thread(stopFollowByUsername);
                objStopFollowByUsername.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        public void stopFollowByUsername()
        {
            try
            {
                objFollowByUsernameManager._IsfevoriteFollowByUsername = true;
                List<Thread> lstTempFollowByUsername = objFollowByUsernameManager.lstThreadsFollowByUsername.Distinct().ToList();
                foreach (Thread item in lstTempFollowByUsername)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Error("---------------------------------------------------------");
                GlobusLogHelper.log.Error(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Error("---------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        public void closeEvent()
        {

        }

        private void chkDivideData_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.chkDivideDataFollowByUsername = true;
                UserControl_FollowByUsername_DivideData obj_UserControl_FollowByUsername_DivideData = new UserControl_FollowByUsername_DivideData();
                var modernDialog = new ModernDialog()
                {
                    Content = obj_UserControl_FollowByUsername_DivideData
                };
                modernDialog.MinWidth = 600;
                modernDialog.MinHeight = 200;
                //modernDialog.Topmost = true;
                Button customButton = new Button() { Content = "SAVE" };
                customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                modernDialog.Buttons = new Button[] { customButton };
                modernDialog.ShowDialog();

                string s1 = string.Empty;
                try
                {
                    if (FollowByUsernameManager.rdbDivideEquallyFollowByUsername == true || FollowByUsernameManager.rdbDivideGivenByUserFollowByUsername == true)
                    {
                        if (FollowByUsernameManager.rdbDivideGivenByUserFollowByUsername == true)
                        {
                            if (!string.IsNullOrEmpty(obj_UserControl_FollowByUsername_DivideData.txtCountGivenByUser_FollowByUsername.Text))
                            {
                                FollowByUsernameManager.CountGivenByUserFollowByUsename = Convert.ToInt32(obj_UserControl_FollowByUsername_DivideData.txtCountGivenByUser_FollowByUsername.Text);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("=> [ Please Give Count Given By User ]");
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
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

     
        private void rdbSingleUser_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                rdbSingleUserFollowByUsername();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void rdbSingleUserFollowByUsername()
        {
            try
            {
                objFollowByUsernameManager.rdbSingleUserFollowByUsername = true;
                objFollowByUsernameManager.rdbMultipleUserFollowByUsername = false;
                btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                txtUsernameUpload.Visibility = Visibility.Hidden;
                txtFollowUserUpload.Visibility = Visibility.Hidden;
                lblUsername.Visibility = Visibility.Hidden;
                lblFollowUsers.Visibility = Visibility.Hidden;
                try
                {
                    if (rdoFollowFollowers.IsChecked == true)
                    {
                        try
                        {
                            singleUserFollowUsersFollowers();
                        }
                        catch(Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                        
                    }

                    if (rdoFollowUserUploaded.IsChecked == true)
                    {
                        try
                        {
                            SingleUSerFollowUserUpload();
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void singleUserFollowUsersFollowers()
        {
            try
            {
                ClGlobul.lstFollowUsername.Clear();
                ClGlobul.ListOfFollowUsersFollowers.Clear();
                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter Follow Users Here ";
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
                                ClGlobul.lstFollowUsername.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ Follow Users Loaded : " + ClGlobul.lstFollowUsername.Count + " ]");
                    GlobusLogHelper.log.Debug("Follow Users : " + ClGlobul.lstFollowUsername.Count);
                } 
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void SingleUSerFollowUserUpload()
        {
            try
            {
                ClGlobul.ListOfFollowUsersFollowers.Clear();
                ClGlobul.lstFollowUsername.Clear();
                UserControl_SingleUser obj = new UserControl_SingleUser();
                obj.UserControlHeader.Text = "Enter UserName Here ";
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
                                ClGlobul.ListOfFollowUsersFollowers.Add(arr_item);
                            }

                        }
                    }
                    GlobusLogHelper.log.Info(" => [ UserName Loaded : " + ClGlobul.ListOfFollowUsersFollowers.Count + " ]");
                    GlobusLogHelper.log.Debug("UserName : " + ClGlobul.ListOfFollowUsersFollowers.Count);
                } 
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


        private void rdbMultipleUser_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByUsernameManager.rdbSingleUserFollowByUsername = false;
                objFollowByUsernameManager.rdbMultipleUserFollowByUsername = true;
                if (rdoFollowFollowers.IsChecked == true)
                {
                    btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                    btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Visible;
                    txtUsernameUpload.IsReadOnly = true;
                    txtFollowUserUpload.IsReadOnly = true;
                    txtUsernameUpload.Visibility = Visibility.Visible;
                    txtFollowUserUpload.Visibility = Visibility.Visible;
                    lblUsername.Visibility = Visibility.Visible;
                    lblFollowUsers.Visibility = Visibility.Visible;
                }
                if (rdoFollowUserUploaded.IsChecked == true)
                {
                    btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                    btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                    txtUsernameUpload.IsReadOnly = true;
                    txtFollowUserUpload.IsReadOnly = true;
                    txtUsernameUpload.Visibility = Visibility.Visible;
                    txtFollowUserUpload.Visibility = Visibility.Visible;
                    lblUsername.Visibility = Visibility.Visible;
                    lblFollowUsers.Visibility = Visibility.Visible;
                }

                txtUsernameUpload.Text = string.Empty;
                txtFollowUserUpload.Text = string.Empty;
             
               // btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                //btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Visible;
                txtUsernameUpload.IsReadOnly = true;
                txtFollowUserUpload.IsReadOnly = true;
                txtUsernameUpload.Visibility = Visibility.Visible;
                txtFollowUserUpload.Visibility = Visibility.Visible;
                lblUsername.Visibility = Visibility.Visible;
                lblFollowUsers.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AccountReport_FollowByUsername()
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
                dt.Columns.Add("UserName");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("FollowByUsername");
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
                        string UserName = dt_item.ItemArray[5].ToString();                   
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
                        dgvFollowByUsername_AccountsReport.ItemsSource = dt.DefaultView;

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

        private void clkDeleteData_FollowByUsername(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("FollowByUsername");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_FollowByUsername();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_FollowByUsername(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportFollowByUsername();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportFollowByUsername()
        {
            try
            {
                if (dgvFollowByUsername_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvFollowByUsername_AccountsReport.Items.Count > 1)
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
                                FilePath = FilePath + "\\FollowByUsername.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("FollowByUsername");
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



        
    }
}
