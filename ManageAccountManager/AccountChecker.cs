using AccountManager;
using BaseLib;
using BasePD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManageAccountManager
{
    public delegate void CheckAccount(int Count);
    public class AccountChecker
    {
        public static CheckAccount objDelegateNoOfAcc;
        public static CheckAccount objDelegateNoOfActiveAcc;
        public static CheckAccount objDelegateNoOfDeadAcc;

        #region Global Variable
        public int Nothread_AccountChecker = 20;
        public int ActiveAcc = 0;
        public int DeadAcc = 0;
        public int NoOfAcc = 0;
        public bool isStopAccountChecker = false;
        public List<Thread> lstThreadsAccountChecker = new List<Thread>();     
        public static int countThreadControllerAccountChecker = 0;
        public static int AccountCheckerdata_count = 0;
        public readonly object AccountCheckerObjThread = new object();
        public bool _IsfevoriteAccountChecker = false;

        public bool rdCheckAccountFromLoadedAccount = false;
        public bool rdCheckAccountFromLoadedFilesAccount = false;

        public int minDelayAccountChecker
        {
            get;
            set;
        }

        public int maxDelayAccountChecker
        {
            get;
            set;
        }

        public int NoOfThreadsAccountChecker
        {
            get;
            set;
        }

        int count = 0;

       public List<string> lstOfActiveAccount = new List<string>();
       public List<string> lstOfDeadAcount = new List<string>();
        #endregion

        Accounts ObjAccountManager = new Accounts();

        public void StartAccountChecker()
        {
            try
            {
                countThreadControllerAccountChecker = 0;
                int numberOfAccountPatch = 25;

                if (NoOfThreadsAccountChecker > 0)
                {
                    numberOfAccountPatch = NoOfThreadsAccountChecker;
                }
                if (PDGlobals.loadedAccountsDictionary.Count()==0)
                {
                    //AccountCheckerdata_count = PDGlobals.load
                }
                else
                {
                    AccountCheckerdata_count = PDGlobals.loadedAccountsDictionary.Count();
                }
                
                if (rdCheckAccountFromLoadedFilesAccount == true)
                {
                    //PDGlobals.loadedAccountsDictionary.Clear();
                      PDGlobals.listAccounts = ClGlobul.lstAccountForAccountChecker;
                      PDGlobals.listAccounts = PDGlobals.listAccounts.Distinct().ToList();
                      foreach (string item in PDGlobals.listAccounts)
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
                              }
                              catch (Exception Ex)
                              {

                              }
                          }
                      }
                      AccountCheckerdata_count = PDGlobals.loadedAccountsDictionary.Count();                
                }

                objDelegateNoOfAcc(AccountCheckerdata_count);

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatch);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {

                            if (countThreadControllerAccountChecker > Nothread_AccountChecker)
                            {
                                try
                                {
                                    lock (AccountCheckerObjThread)
                                    {
                                        Monitor.Wait(AccountCheckerObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {

                                }
                            }
                            string acc = account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;                       
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {
                                Thread profilerThread = new Thread(StartAccountCheckerObjThreadMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAccountChecker++;
                            }
                        }
                        // }).Start();
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }          

        }

        public void StartAccountCheckerObjThreadMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAccountChecker)
                {
                    try
                    {
                        lstThreadsAccountChecker.Add(Thread.CurrentThread);
                        lstThreadsAccountChecker.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {                     
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;
                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                      
                        #region Login

                        if (!objPinUser.isloggedin)
                        {
                            GlobusLogHelper.log.Info(" => [ Logging In With : " + objPinUser.Username + " ]");
                            bool checkLogin;

                            if (string.IsNullOrEmpty(objPinUser.ProxyPort))
                            {
                                objPinUser.ProxyPort = "80";
                            }

                            try
                            {
                                checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                if (!checkLogin)
                                {
                                    lstOfDeadAcount.Add(objPinUser.Username + ":" + objPinUser.Password + ":" + objPinUser.Niches + ":" + objPinUser.ProxyAddress + ":" + objPinUser.ProxyPort + ":" + objPinUser.ProxyUsername + ":" + objPinUser.ProxyPassword);
                                    objDelegateNoOfDeadAcc(lstOfDeadAcount.Count);
                                    GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                    count++;
                                    return;
                                   
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                lstOfActiveAccount.Add(objPinUser.Username + ":" + objPinUser.Password + ":" + objPinUser.Niches + ":" +objPinUser.ProxyAddress + ":" + objPinUser.ProxyPort + ":" + objPinUser.ProxyUsername + ":" + objPinUser.ProxyPassword);
                                objDelegateNoOfActiveAcc(lstOfActiveAccount.Count);
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                count++;                          
                            }

                            catch { };
                        }
                        else if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                               
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
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            finally
            {
                try
                {

                    if (countThreadControllerAccountChecker > Nothread_AccountChecker)
                    {
                        lock (AccountCheckerObjThread)
                        {
                            Monitor.Pulse(AccountCheckerObjThread);
                        }
                        AccountCheckerdata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                if (PDGlobals.loadedAccountsDictionary.Count == count)
                {
                    GlobusLogHelper.log.Info(" => [ Account Checking Process Finished ]");
                    GlobusLogHelper.log.Info(" [ PROCESS COMPLETED ]");
                    GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
                }              
            }


        }


    }
}
