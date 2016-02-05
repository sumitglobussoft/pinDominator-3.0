using BaseLib;
using BasePD;
using Globussoft;
//using PinDominator.Account;
//using PinDominator.Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
//using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Data.SQLite;
using AccountManager;
using PinDominator3.AccountUserControl;
using PinDominator3.Classes;
using WpfAnimatedGif;

namespace PinDominator3.Pages.PageAccount
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class UploadAccount : UserControl
    {
        QueryManager Qm = new QueryManager();
        string myConnectionString = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PINDominator.db" + ";Version=3;";


        private void NavigateUrlOnBrowser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://pvadomination.com/");
            }
            catch (Exception ex)
            {

            }
        }


        public UploadAccount()
        {
            InitializeComponent();
            // objUploadAccount = this;
            Accounts.objDelegateAccountLoad = new AccountLoad(AccounLoad);
            AccounLoad();
        }

        private void btnBrowseAccounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    QueryManager.deleteQuery();
                    PDGlobals.loadedAccountsDictionary.Clear();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                try
                {
                    LoadAccountProgressBar.IsIndeterminate = true;
                    Thread uploadAccountThread = new Thread(LoadAccounts);
                    uploadAccountThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    uploadAccountThread.IsBackground = true;

                    uploadAccountThread.Start();
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

        public void LoadAccounts()
        {
            try
            {
               
                DataSet ds;

                DataTable dt = new DataTable();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                string Path = dlg.ToString().Replace("Microsoft.Win32.OpenFileDialog: Title: , FileName", "");
                if (result == true)
                {
                    DateTime sTime = DateTime.Now;

                    dt.Columns.Add("UserName");
                    dt.Columns.Add("Password");
                    dt.Columns.Add("Niches");
                    dt.Columns.Add("ProxyAddress");
                    dt.Columns.Add("ProxyPort");
                    dt.Columns.Add("ProxyUserName");
                    dt.Columns.Add("ProxyPassword");
                    //dt.Columns.Add("Useragent");
                   // dt.Columns.Add("BoardsName");
                    dt.Columns.Add("ScreenName");
                    dt.Columns.Add("LoginStatus");

                    ds = new DataSet();
                    ds.Tables.Add(dt);

                    List<string> templist = GlobusFileHelper.ReadFile(dlg.FileName);

                    if (templist.Count > 0)
                    {
                        PDGlobals.loadedAccountsDictionary.Clear();
                        PDGlobals.listAccounts.Clear();
                    }

                    if (Globals.IsBasicVersion)
                    {
                        try
                        {
                            string selectQuery = "select count(UserName) from tb_emails";
                            DataSet DS = DataBaseHandler.SelectQuery(selectQuery, "tb_emails");
                            int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

                            if (countLoadedAccounts >= 5)
                            {
                                AccounLoad();
                                MessageBox.Show("You Are Using PD Basic Version 5 Accounts allready loaded..");
                                return;
                            }
                            else
                            {
                                int RemainingAccount = 5 - countLoadedAccounts;

                                templist.RemoveRange(RemainingAccount, templist.Count - RemainingAccount);

                            }
                        }
                        catch { }
                    }
                    if (Globals.IsProVersion)
                    {
                        try
                        {
                            string selectQuery = "select count(UserName) from tb_emails";
                            DataSet DS = DataBaseHandler.SelectQuery(selectQuery, "tb_emails");
                            int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

                            if (countLoadedAccounts >= 15)
                            {
                                AccounLoad();
                                MessageBox.Show("You Are Using PD Pro Version 15 Accounts allready loaded..");
                                return;
                            }
                            else
                            {
                                int RemainingAccount = 15 - countLoadedAccounts;

                                templist.RemoveRange(RemainingAccount, templist.Count - RemainingAccount);

                            }
                        }
                        catch { }
                    }
                    int counter = 0;

                    foreach (string item in templist)
                    {
                        //if (Globals.CheckLicenseManager == "fdfreetrial" && counter == 5)
                        //{
                        //    break;
                        //}
                        counter = counter + 1;
                        try
                        {
                            string account = item;
                            string[] AccArr = account.Split(':');
                            if (AccArr.Count() > 1)
                            {
                                string accountUser = account.Split(':')[0];
                                string accountPass = account.Split(':')[1];
                                string niches = string.Empty;
                                string proxyAddress = string.Empty;
                                string proxyPort = string.Empty;
                                string proxyUserName = string.Empty;
                                string proxyPassword = string.Empty;
                                //string Useragent = string.Empty;
                                string Followers = string.Empty;
                                string Following = string.Empty;
                                // string Boards = string.Empty;
                                string BoardsName = string.Empty;
                                string ScreenName = string.Empty;
                                string LoginStatus = string.Empty;

                                int DataCount = account.Split(':').Length;
                                if (DataCount == 3)
                                {
                                    niches = account.Split(':')[2];

                                }
                                else if (DataCount == 5)
                                {
                                    niches = account.Split(':')[2];
                                    proxyAddress = account.Split(':')[3];
                                    proxyPort = account.Split(':')[4];
                                }
                                else if (DataCount == 7)
                                {
                                    niches = account.Split(':')[2];
                                    proxyAddress = account.Split(':')[3];
                                    proxyPort = account.Split(':')[4];
                                    proxyUserName = account.Split(':')[5];
                                    proxyPassword = account.Split(':')[6];
                                    //BoardsName = account.Split(':')[7];

                                }

                                dt.Rows.Add(accountUser, accountPass, proxyAddress, proxyPort, proxyUserName, proxyPassword);
                                //Qm.DeleteAccounts(accountUser);
                                LoginStatus = "NotChecked";

                                Qm.AddAccountInDataBase(accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, LoginStatus);

                                try
                                {
                                    AccountNotifyPropertyChanged objAccountNotifyPropertyChanged = new AccountNotifyPropertyChanged();

                                    objAccountNotifyPropertyChanged.Username = accountUser;
                                    objAccountNotifyPropertyChanged.Password = accountPass;
                                    objAccountNotifyPropertyChanged.Niche = niches;
                                    objAccountNotifyPropertyChanged.ScreenName = ScreenName;
                                    objAccountNotifyPropertyChanged.FollowerCount = Followers;
                                    objAccountNotifyPropertyChanged.FollowingCount = Following;
                                    objAccountNotifyPropertyChanged.ProxyAddress = proxyAddress;
                                    objAccountNotifyPropertyChanged.ProxyPassword = proxyPassword;
                                    objAccountNotifyPropertyChanged.ProxyPort = proxyPort;
                                    objAccountNotifyPropertyChanged.ProxyUserName = proxyUserName;
                                    if (LoginStatus.Contains("Success"))
                                    {
                                        objAccountNotifyPropertyChanged.LoginStatus = "Success";
                                        objAccountNotifyPropertyChanged.BackgroundColor = "Green";
                                        this.Dispatcher.Invoke(new Action(delegate
                                             {
                                                 AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                             }));
                                    }
                                    else if (LoginStatus.Contains("Fail"))
                                    {
                                        objAccountNotifyPropertyChanged.LoginStatus = "Fail";
                                        objAccountNotifyPropertyChanged.BackgroundColor = "Red";
                                        this.Dispatcher.Invoke(new Action(delegate
                                            {
                                                AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                            }));                                      
                                    }
                                    else if (LoginStatus.Contains("Not"))
                                    {
                                        objAccountNotifyPropertyChanged.LoginStatus = "Not Checked";
                                        objAccountNotifyPropertyChanged.BackgroundColor = "";
                                        this.Dispatcher.Invoke(new Action(delegate
                                            {
                                                AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                            }));                                     
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }

                                try
                                {
                                    PinInterestUser objPinInterestUser = new PinInterestUser();
                                    objPinInterestUser.Username = accountUser;
                                    objPinInterestUser.Password = accountPass;
                                    objPinInterestUser.Niches = niches;
                                    objPinInterestUser.ProxyAddress = proxyAddress;
                                    objPinInterestUser.ProxyPort = proxyPort;
                                    objPinInterestUser.ProxyUsername = proxyUserName;
                                    objPinInterestUser.ProxyPassword = proxyPassword;
                                    //objPinInterestUser.UserAgent = Useragent;
                                    objPinInterestUser.BoardsName = BoardsName;
                                    objPinInterestUser.ScreenName = ScreenName;
                                    objPinInterestUser.LoginStatus = LoginStatus;
                                    PDGlobals.loadedAccountsDictionary.Add(objPinInterestUser.Username, objPinInterestUser);

                                    PDGlobals.listAccounts.Add(objPinInterestUser.Username + ":" + objPinInterestUser.Password + ":" + objPinInterestUser.Niches + ":" + objPinInterestUser.ProxyAddress + ":" + objPinInterestUser.ProxyPort + ":" + objPinInterestUser.ProxyUsername + ":" + objPinInterestUser.ProxyPassword);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                }

                                // Set this to "0" if loading unprofiled accounts;

                                string profileStatus = "0";
                            }
                            else
                            {
                                GlobusLogHelper.log.Info("Account has some problem : " + item);
                                GlobusLogHelper.log.Debug("Account has some problem : " + item);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }

                    }

                    DataView dv = dt.DefaultView;
                    dv.AllowNew = false;
                    try
                    {
                        AccounLoad();

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        DateTime eTime = DateTime.Now;

                        string timeSpan = (eTime - sTime).TotalSeconds.ToString();

                        Application.Current.Dispatcher.Invoke(new Action(() => { lblaccounts_ManageAccounts_LoadsAccountsCount.Content = dt.Rows.Count.ToString(); }));

                        GlobusLogHelper.log.Debug("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");

                        //GlobusLogHelper.log.Info("Accounts Loaded : " + dt.Rows.Count.ToString() + " In " + timeSpan + " Seconds");
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                LoadAccountProgressBar.IsIndeterminate = false;
            }));
        }

       
        public void  AccounLoad()
        {
            try
            {
                string accountUser = string.Empty;
                string accountPass = string.Empty;
                string niches = string.Empty;
                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUserName = string.Empty;
                string proxyPassword = string.Empty;
                //string Useragent = string.Empty;
                string BoardsName = string.Empty;
                string ScreenName = string.Empty;
                string LoginStatus = string.Empty;
                string Followers = string.Empty;
                string Following = string.Empty;
                QueryExecuter QME = new QueryExecuter();
                DataSet ds = QME.getAccount();
                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listOfAccount.Clear();
                }));
                if (ds.Tables[0].Rows.Count != 0)
                {
                    PDGlobals.listAccounts.Clear();
                    for (int noRow = 0; noRow < ds.Tables[0].Rows.Count; noRow++)
                    {
                        string account = ds.Tables[0].Rows[noRow].ItemArray[0].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[1].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[2].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[3].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[4].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[5].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[6].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[7].ToString();
                        PDGlobals.listAccounts.Add(account);
                        //  dv.AllowNew = false;
                        accountUser = ds.Tables[0].Rows[noRow].ItemArray[0].ToString();
                        accountPass = ds.Tables[0].Rows[noRow].ItemArray[1].ToString();
                        niches = ds.Tables[0].Rows[noRow].ItemArray[2].ToString();
                        proxyAddress = ds.Tables[0].Rows[noRow].ItemArray[3].ToString();
                        proxyPort = ds.Tables[0].Rows[noRow].ItemArray[4].ToString();
                        proxyUserName = ds.Tables[0].Rows[noRow].ItemArray[5].ToString();
                        proxyPassword = ds.Tables[0].Rows[noRow].ItemArray[6].ToString();
                        ScreenName = ds.Tables[0].Rows[noRow].ItemArray[9].ToString();
                        Followers = ds.Tables[0].Rows[noRow].ItemArray[7].ToString();
                        Following = ds.Tables[0].Rows[noRow].ItemArray[8].ToString();
                        LoginStatus = ds.Tables[0].Rows[noRow].ItemArray[10].ToString();

                        // Accounts objPinInterestUser = new Accounts();
                        PinInterestUser objPinInterestUser = new PinInterestUser("", "", "", "");
                        objPinInterestUser.Username = accountUser;
                        objPinInterestUser.Password = accountPass;
                        objPinInterestUser.Niches = niches;
                        objPinInterestUser.ProxyAddress = proxyAddress;
                        objPinInterestUser.ProxyPort = proxyPort;
                        objPinInterestUser.ProxyUsername = proxyUserName;
                        objPinInterestUser.ProxyPassword = proxyPassword;
                       // objPinInterestUser.LoginStatus = LoginStatus;
                        try
                        {
                            AccountNotifyPropertyChanged objAccountNotifyPropertyChanged = new AccountNotifyPropertyChanged();

                            objAccountNotifyPropertyChanged.Username = accountUser;
                            objAccountNotifyPropertyChanged.Password = accountPass;
                            objAccountNotifyPropertyChanged.Niche = niches;
                            objAccountNotifyPropertyChanged.ScreenName = ScreenName;
                            objAccountNotifyPropertyChanged.FollowerCount = Followers;
                            objAccountNotifyPropertyChanged.FollowingCount = Following;
                            objAccountNotifyPropertyChanged.ProxyAddress = proxyAddress;
                            objAccountNotifyPropertyChanged.ProxyPassword = proxyPassword;
                            objAccountNotifyPropertyChanged.ProxyPort = proxyPort;
                            objAccountNotifyPropertyChanged.ProxyUserName = proxyUserName;
                            if (LoginStatus.Contains("Success"))
                            {
                                objAccountNotifyPropertyChanged.LoginStatus = "Success";
                                objAccountNotifyPropertyChanged.BackgroundColor = "Green";
                                this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                    }));                               
                            }
                            else if (LoginStatus.Contains("Fail"))
                            {
                                objAccountNotifyPropertyChanged.LoginStatus = "Fail";
                                objAccountNotifyPropertyChanged.BackgroundColor = "Red";
                                this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                    }));                               
                            }
                            else if (LoginStatus.Contains("Not"))
                            {
                                objAccountNotifyPropertyChanged.LoginStatus = "Not Checked";
                                objAccountNotifyPropertyChanged.BackgroundColor = "";
                                this.Dispatcher.Invoke(new Action(delegate
                                    {
                                        AccountViewModel._listOfAccount.Add(objAccountNotifyPropertyChanged);
                                    }));                               
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }


                        try
                        {
                            PDGlobals.loadedAccountsDictionary.Add(objPinInterestUser.Username, objPinInterestUser);
                            try
                            {
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                   // dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;
                                    dgvAccounts.ItemsSource = AccountViewModel._listOfAccount;
                                   // AccountViewModel._listOfAccount.Clear();
                                }));
                            }
                            catch (Exception ex)
                            { };
                        }
                        catch (Exception ex)
                        { };
                    }
                    try
                    {
                        this.Dispatcher.Invoke(new Action(delegate
                        {
                           // dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;
                            dgvAccounts.ItemsSource = AccountViewModel._listOfAccount;
                            //AccountViewModel._listOfAccount.Clear();
                        }));
                    }
                    catch { };
                    GlobusLogHelper.log.Info(" => [ " + PDGlobals.listAccounts.Count + " Accounts Loaded ]");
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        //dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;
                        dgvAccounts.ItemsSource = AccountViewModel._listOfAccount;
                       // AccountViewModel._listOfAccount.Clear();
                    }));
                    GlobusLogHelper.log.Info(" [  No Accounts Loaded ]");
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnAccounts_ManageAccounts_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 if (ModernDialog.ShowMessage("Do you really want to delete all the accounts from Database", "Confirm delete", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    QueryManager.deleteQuery();                 
                    PDGlobals.loadedAccountsDictionary.Clear();
                    AccounLoad();
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void ButonClearProxies_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Do you really want to clear all proxies from Accounts", "Confirm delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        DataSet ds = new DataSet();
                        using (SQLiteConnection con = new SQLiteConnection(myConnectionString))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter("SELECT * FROM tb_emails", con))
                            {
                                ad.Fill(ds);
                                if (ds.Tables[0].Rows.Count != 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        string UpdateQuery = "Update tb_emails Set ProxyAddress='" + "" + "', ProxyPort='" + "" + "', ProxyUserName='" + "" + "', ProxyPassword='" + "" + "'  , UserAgent = '" + "" + "'WHERE UserName='" + ds.Tables[0].Rows[i]["UserName"].ToString() + "'";
                                        DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No Emails to Delete Proxies");
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    { };
                    AccounLoad();//Refresh Datagrid
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void closeEvent()
        {

        }
        AddSingleAccount_UploadAccount obj_AddSingleAccount_UploadAccount = new AddSingleAccount_UploadAccount();
        private void AddSingleAcc_UploadAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                var modernDialog = new ModernDialog
                {
                    Content = obj_AddSingleAccount_UploadAccount
                };
                modernDialog.MinWidth = 600;
                modernDialog.MinHeight = 600;
                //Button customButton = new Button() { Content = "SUBMIT" };
                //customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                //modernDialog.Buttons = new Button[] { customButton };
                modernDialog.ShowDialog();
                //string s1 = string.Empty;
                //try
                //{
                //    AddSingleAccountUploadAcc();
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                //}

                AccounLoad();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void AddSingleAccountUploadAcc()
        {
            try
            {
                QueryManager QM = new QueryManager();
                string accountUser = string.Empty;
                string accountPass = string.Empty;
                string niches = string.Empty;
                string proxyAddress = string.Empty;
                string proxyPort = string.Empty;
                string proxyUserName = string.Empty;
                string proxyPassword = string.Empty;
                //string Useragent = string.Empty;
                string BoardsName = string.Empty;
                string ScreenName = string.Empty;
                string LoginStatus = string.Empty;

                if (string.IsNullOrEmpty(obj_AddSingleAccount_UploadAccount.txtEmail_AddSingleAcc.Text))
                {
                    try
                    {
                        GlobusLogHelper.log.Info("Please Enter Account");
                        ModernDialog.ShowMessage("Please Enter Account", "Enter Account", MessageBoxButton.OK);
                        return;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                else
                {
                    accountUser = (obj_AddSingleAccount_UploadAccount.txtEmail_AddSingleAcc.Text).ToString();
                }

                if (string.IsNullOrEmpty(obj_AddSingleAccount_UploadAccount.txtPassword_AddSingleAcc.Text))
                {
                    try
                    {
                        GlobusLogHelper.log.Info("Please Enter Account");
                        ModernDialog.ShowMessage("Please Enter Account", "Enter Account", MessageBoxButton.OK);
                        return;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                else
                {
                    accountPass = (obj_AddSingleAccount_UploadAccount.txtPassword_AddSingleAcc.Text).ToString();
                }

                try
                {
                    niches = (obj_AddSingleAccount_UploadAccount.txtNiche_AddSinleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyAddress = (obj_AddSingleAccount_UploadAccount.txtProxyAddress_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyPort = (obj_AddSingleAccount_UploadAccount.txtProxyPort_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyUserName = (obj_AddSingleAccount_UploadAccount.txtProxyUsername_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                try
                {
                    proxyPassword = (obj_AddSingleAccount_UploadAccount.txtProxyPassword_AddSingleAcc.Text).ToString();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                if (Globals.IsBasicVersion)
                {
                    try
                    {
                        string selectQuery = "select count(UserName) from tb_emails";
                        DataSet DS = DataBaseHandler.SelectQuery(selectQuery, "tb_emails");
                        int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

                        if (countLoadedAccounts >= 5)
                        {
                            AccounLoad();
                            MessageBox.Show("You Are Using PD Basic Version 5 Accounts allready loaded..");
                            return;
                        }
                        else
                        {
                           // int RemainingAccount = 5 - countLoadedAccounts;

                            //templist.RemoveRange(RemainingAccount, templist.Count - RemainingAccount);

                        }
                    }
                    catch { }
                }
                if (Globals.IsProVersion)
                {
                    try
                    {
                        string selectQuery = "select count(UserName) from tb_emails";
                        DataSet DS = DataBaseHandler.SelectQuery(selectQuery, "tb_emails");
                        int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

                        if (countLoadedAccounts >= 15)
                        {
                            AccounLoad();
                            MessageBox.Show("You Are Using PD Pro Version 15 Accounts allready loaded..");
                            return;
                        }
                        else
                        {
                           // int RemainingAccount = 15 - countLoadedAccounts;

                           // templist.RemoveRange(RemainingAccount, templist.Count - RemainingAccount);

                        }
                    }
                    catch { }
                }

                LoginStatus = "NotChecked";
                QM.AddAccountInDataBase(accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, LoginStatus);
                UploadAccount objUploadAccount = new UploadAccount();
                objUploadAccount.AccounLoad();

                //Window parentWindow = (Window)this.Parent;
                //parentWindow.Close();

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }       
        }

        private void Grid_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                DataGrid grid = (DataGrid)sender;
                if (e.Command == DataGrid.DeleteCommand)
                {
                    try
                    {
                        int i = dgvAccounts.SelectedIndex;

                        if (i < 0)
                        {
                            GlobusLogHelper.log.Info("Please select account for deletion");
                            return;
                        }
                        QueryManager qm = new QueryManager();
                        MessageBoxButton btn = MessageBoxButton.OK;
                        MessageBoxButton btnC = MessageBoxButton.YesNo;

                        var result = ModernDialog.ShowMessage("Are you want to delete this Accounts permanently?", " Delete Account ", btnC);

                        if (result == MessageBoxResult.Yes)
                        {
                            foreach (AccountNotifyPropertyChanged objAccountNotifyPropertyChanged in dgvAccounts.SelectedItems)
                            {
                                try
                                {
                                    string Username = objAccountNotifyPropertyChanged.Username.ToString();
                                    string Password = objAccountNotifyPropertyChanged.Password.ToString();
                                    QueryManager.DeleteAccounts(Username);

                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                                }
                            }
                            PDGlobals.loadedAccountsDictionary.Clear();
                            AccounLoad();
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : 55" + ex.Message);
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }


        private void DeleSingleAccount_UploadAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteSingleAccount();               
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        private void AddUserAgent_UploadAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog
                    {
                        Content = new AddNewUserAgen_UploadAccount()
                    };
                    modernDialog.MinWidth = 500;
                    modernDialog.MinHeight = 300;
                    //modernDialog.Topmost = true;
                    Button customButton = new Button() { Content = "SUBMIT" };
                    customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                    modernDialog.Buttons = new Button[] { customButton };
                    modernDialog.ShowDialog();
                });
                AccounLoad();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        
        public void DeleteSingleAccount()
        {
            try
            {
                QueryExecuter QME = new QueryExecuter();             
                DataSet ds = QME.getAccount();
                int i = dgvAccounts.SelectedIndex;
                if (i < 0)
                {
                    GlobusLogHelper.log.Info("Please Select Account For Deletion !");
                    var ResultMessageBox = ModernDialog.ShowMessage("Please Select Account For Deletion !", " Delete Account ",  MessageBoxButton.OKCancel);
                    return;
                }                     

                if (ModernDialog.ShowMessage("Are You Want To Delete This Accounts Permanently?", " Delete Account ", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    foreach (AccountNotifyPropertyChanged  objAccountNotifyPropertyChanged in dgvAccounts.SelectedItems)
                    {
                        try
                        {
                           // AccountViewModel._listOfAccount
                            //DataRowView row = (DataRowView)selection;

                            string Username = objAccountNotifyPropertyChanged.Username.ToString();
                            string Password = objAccountNotifyPropertyChanged.Password.ToString();
                            QueryManager.DeleteAccounts(Username);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error :" + ex.Message);
                        }
                    }
                    AccounLoad();
                }
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnAccounts_ManageAccounts_AddSingleAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modernDialog = new ModernDialog
                {
                    Content =  obj_AddSingleAccount_UploadAccount
                };
                modernDialog.MinWidth = 600;
                modernDialog.MinHeight = 600;
                //Button customButton = new Button() { Content = "SUBMIT" };
                //customButton.Click += (ss, ee) => { closeEvent(); modernDialog.Close(); };
                //modernDialog.Buttons = new Button[] { customButton };
                modernDialog.ShowDialog();
                //string s1 = string.Empty;               
                //try
                //{                   
                //    AddSingleAccountUploadAcc();
                //}
                //catch (Exception ex)
                //{
                //    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                //}
                AccounLoad();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ImageLoadEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage b = new BitmapImage();
                b.BeginInit();
                string getImagePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0" + "\\Banner-728x90_3.gif";
                b.UriSource = new Uri(@getImagePath); //D:\sonu\TwtDominator_5.0\trunk\TwtDominator 5.0\Resource\Banner-728x90_3.gif                
                b.EndInit();

                // ... Get Image reference from sender.
                var image = sender as Image;

                // ... Assign Source.
                image.Source = b;
                ImageBehavior.SetAnimatedSource(image, b);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("PVA Domination Error ==> " + ex.Message);
            }
        }
      



    }
}
