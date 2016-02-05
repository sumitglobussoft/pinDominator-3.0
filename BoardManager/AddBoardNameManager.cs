using AccountManager;
using BaseLib;
using BasePD;
using PinsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BoardManager
{
    public class AddBoardNameManager
    {
        #region Global Variable

        public string[] noOfBoard = null;
        public int Nothread_AddBoardName = 5;
        public bool isStopAddBoardName = false;
        public List<Thread> lstThreadsAddBoardName = new List<Thread>();
        public static int countThreadControllerAddBoardName = 0;
        public static int AddBoardNamedata_count = 0;
        public readonly object AddBoardNameObjThread = new object();
        public bool _Isfevorite = false;
        public bool rdbSingleUserAddBoardName = false;
        public bool rdbMultipleUserAddBoardName = false;
        public string SingleBoardName = string.Empty;

        public int minDelayAddBoardName
        {
            get;
            set;
        }

        public int maxDelayAddBoardName
        {
            get;
            set;
        }

        public int NoOfThreadsAddBoardName
        {
            get;
            set;
        }

        string[] array = null;
        string CategoryName = string.Empty;

        Accounts ObjAccountManager = new Accounts();

        #endregion

        public void StartAddBoardName()
        {
            try
            {
                countThreadControllerAddBoardName = 0;
                int numberOfAccountPatchAddBoardName = 25;

                if (NoOfThreadsAddBoardName > 0)
                {
                    numberOfAccountPatchAddBoardName = NoOfThreadsAddBoardName;
                }

                AddBoardNamedata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchAddBoardName);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerAddBoardName > Nothread_AddBoardName)
                            {
                                try
                                {
                                    lock (AddBoardNameObjThread)
                                    {
                                        Monitor.Wait(AddBoardNameObjThread);
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
                                Thread profilerThread = new Thread(StartAddBoardNameMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAddBoardName++;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartAddBoardNameMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAddBoardName)
                {
                    try
                    {
                        lstThreadsAddBoardName.Add(Thread.CurrentThread);
                        lstThreadsAddBoardName.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;

                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                        foreach (string Boardnames in ClGlobul.lstBoardNames)
                        {
                            array = Regex.Split(Boardnames, ":");
                            if (array.Length == 2)
                            {
                                if (array[0] == objPinUser.Niches)
                                {
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
                                            //checkLogin = ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                            checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                            if (!checkLogin)
                                            {
                                                GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                                return;
                                            }
                                            string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                            GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                            StartActionMultithreadAddBoardName(ref objPinUser);
                                        }

                                        catch (Exception ex)
                                        { };
                                    }
                                    else if(objPinUser.isloggedin == true)
                                    {
                                        try
                                        {
                                            GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                            StartActionMultithreadAddBoardName(ref objPinUser);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                        }
                                    }
                                }
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartActionMultithreadAddBoardName(ref PinInterestUser objPinUser)
        {
            try 
            {
                foreach (string Boardnames in ClGlobul.lstBoardNames)
                {
                   string[] array1 = Regex.Split(Boardnames, ":");
                    string[] arrayKeyword = Regex.Split(array1[1], ",");

                    if (array1.Length == 2)
                    {
                        if (array1[0] == objPinUser.Niches)
                        {
                            AddPinWithNewBoardManager objAddPin = new AddPinWithNewBoardManager();

                            if (arrayKeyword.Length > 0)
                            {
                                foreach (string name in arrayKeyword)
                                {
                                    objAddPin.CreateBoard_new(name, CategoryName, ref objPinUser);
                                    int delay = RandomNumberGenerator.GenerateRandom(minDelayAddBoardName, maxDelayAddBoardName);
                                    GlobusLogHelper.log.Info(" => [ Delay For " + delay + " Seconds ] ");
                                    Thread.Sleep(delay * 1000);
                                }
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [ No Board Names With Niche " + objPinUser.Username + " ]");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            finally
            {
                 try
                {
                    if (countThreadControllerAddBoardName > Nothread_AddBoardName)
                    {
                        lock (AddBoardNameObjThread)
                        {
                            Monitor.Pulse(AddBoardNameObjThread);
                        }
                        AddBoardNamedata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerAddBoardName--;
                GlobusLogHelper.log.Info(" [ PROCESS COMPLETED FOR " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("--------------------------------------------------------------------------------");
            }
        }


    }
}
