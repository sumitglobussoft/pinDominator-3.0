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

namespace PinDominator3.Pages.PageAccount
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class UploadAccount : UserControl
    {
        QueryManager Qm = new QueryManager();
        string myConnectionString = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PINDominator.db" + ";Version=3;";
      
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
                QueryManager.deleteQuery();           
                PDGlobals.loadedAccountsDictionary.Clear();
            }
            catch(Exception ex)  
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

        private void LoadAccounts()
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
                    dt.Columns.Add("Useragent");
                    dt.Columns.Add("BoardsName");
                    dt.Columns.Add("ScreenName");
                    
                    ds = new DataSet();
                    ds.Tables.Add(dt);

                    List<string> templist = GlobusFileHelper.ReadFile(dlg.FileName);

                    if (templist.Count > 0)
                    {
                        PDGlobals.loadedAccountsDictionary.Clear();
                        PDGlobals.listAccounts.Clear();

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
                                string Useragent = string.Empty;
                                string Followers = string.Empty;
                                string Following = string.Empty;
                                string Boards = string.Empty;
                                string BoardsName = string.Empty;
                                string ScreenName = string.Empty;

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
                                Qm.AddAccountInDataBase(accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, Useragent, BoardsName, ScreenName);


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
                                    objPinInterestUser.UserAgent = Useragent;
                                    objPinInterestUser.BoardsName = BoardsName;
                                    objPinInterestUser.ScreenName = ScreenName;

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

                    AccounLoad();
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
            string accountUser = string.Empty;
            string accountPass = string.Empty;
            string niches = string.Empty;
            string proxyAddress = string.Empty;
            string proxyPort = string.Empty;
            string proxyUserName = string.Empty;
            string proxyPassword = string.Empty;
            string Useragent = string.Empty;
            string BoardsName = string.Empty;
            string ScreenName = string.Empty;
            QueryExecuter QME = new QueryExecuter();
            DataSet ds = QME.getAccount();
            if (ds.Tables[0].Rows.Count != 0)
            {
                PDGlobals.listAccounts.Clear();
                for (int noRow = 0; noRow < ds.Tables[0].Rows.Count; noRow++)
                {
                    string account = ds.Tables[0].Rows[noRow].ItemArray[0].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[1].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[2].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[3].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[4].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[5].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[6].ToString();
                    PDGlobals.listAccounts.Add(account);
                    //  dv.AllowNew = false;
                    accountUser = ds.Tables[0].Rows[noRow].ItemArray[0].ToString();
                    accountPass = ds.Tables[0].Rows[noRow].ItemArray[1].ToString();
                    niches = ds.Tables[0].Rows[noRow].ItemArray[2].ToString();
                    proxyAddress = ds.Tables[0].Rows[noRow].ItemArray[3].ToString();
                    proxyPort = ds.Tables[0].Rows[noRow].ItemArray[4].ToString();
                    proxyUserName = ds.Tables[0].Rows[noRow].ItemArray[5].ToString();
                    proxyPassword = ds.Tables[0].Rows[noRow].ItemArray[6].ToString();

                   // Accounts objPinInterestUser = new Accounts();
                    PinInterestUser objPinInterestUser = new PinInterestUser("", "", "", "");
                    objPinInterestUser.Username = accountUser;
                    objPinInterestUser.Password = accountPass;
                    objPinInterestUser.Niches = niches;
                    objPinInterestUser.ProxyAddress = proxyAddress;
                    objPinInterestUser.ProxyPort = proxyPort;
                    objPinInterestUser.ProxyUsername = proxyUserName;
                    objPinInterestUser.ProxyPassword = proxyPassword;
                    try
                    {
                        PDGlobals.loadedAccountsDictionary.Add(objPinInterestUser.Username, objPinInterestUser);
                        try
                        {
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;

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
                        dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;

                    }));
                }
                catch { };
                GlobusLogHelper.log.Info(" => [ " + PDGlobals.listAccounts.Count + " Accounts Loaded ]");
            }
            else
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    dgvAccounts.ItemsSource = ds.Tables[0].DefaultView;

                }));
                GlobusLogHelper.log.Info(" [  No Accounts Loaded ]");
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

        private void AddSingleAcc_UploadAcc(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog
                    {
                        Content = new AddSingleAccount_UploadAccount()
                    };
                    modernDialog.MinWidth = 600;
                    modernDialog.MinHeight = 600;
                    modernDialog.ShowDialog();
                });
                AccounLoad();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
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
                    foreach (var selection in dgvAccounts.SelectedItems)
                    {
                        try
                        {
                            DataRowView row = (DataRowView)selection;

                            string Username = row["UserName"].ToString();
                            string Password = row["Password"].ToString();
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
      



    }
}
