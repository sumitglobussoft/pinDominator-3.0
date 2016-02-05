using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;


namespace LikeManager
{
    public delegate void AccortReport_Like();
    public class LikeManagers
    {
        public static AccortReport_Like objLikeDelegate;

        #region Global variable

       public static List<string> lstAllPins = new List<string>();

        public static int Nothread_Like = 5;
        public  bool isStopLike = false;
        public  List<Thread> lstThreadsLike = new List<Thread>();
        public static int minDelayLike = 10;
        public static int maxDelayLike = 20;
        public static int countThreadControllerLike = 0;   
        public static int Likedata_count = 0;
        public static bool rbListLikePinUrls = false;
        public static bool rbNormalLikePinUrls = false;
        public static int MaxLike = 5;
        public static List<List<string>> LikePinLst = new List<List<string>>();
        public static int LoadedAccountCount = 0;
       
        public static readonly object LikeObjThread = new object();
        public static bool chkBox_Like_DivideData = false;
        public static bool rdbDivideByUser = false;
        public static bool rdbDivideEqually = false;
        public static int CountGivenByUser = 0;
        public bool _IsfevoriteLike = false;


        public bool rdbSingleUserLike = false;
        public bool rdbMultipleUserLike = false;
      
        public int NoOfThreadsLike
        {
            get;
            set;
        }

        List<List<string>> list_lstTargetLike = new List<List<string>>();
        List<string> list_lstTargetLike_item = new List<string>();
        int LstCounter_Like = 0;

        public Accounts ObjAccountManager = new Accounts();
        Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
     

        #endregion variable

        QueryManager Qm = new QueryManager();    

        public  void StartLike()
        {
            
            countThreadControllerLike = 0;
            try
            {
                int numberOfAccountPatchLike = 25;

                if (NoOfThreadsLike > 0)
                {
                    numberOfAccountPatchLike = NoOfThreadsLike;
                }

                Likedata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();

                #region Divide Data Setting
                if (chkBox_Like_DivideData == true)
                {
                    try
                    {
                        if (rdbDivideByUser == true || rdbDivideEqually == true)
                        {
                            int splitNo = 0;
                            if (rdbDivideEqually == true)
                            {
                                splitNo = ClGlobul.lstAddToBoardUserNames.Count / PDGlobals.listAccounts.Count;
                            }
                            else if (rdbDivideByUser == true)
                            {
                                if (Convert.ToInt32(CountGivenByUser) != 0)
                                {
                                    int res = Convert.ToInt32(Convert.ToInt32(CountGivenByUser));
                                    splitNo = res;
                                }
                            }
                            if (splitNo == 0)
                            {
                                splitNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstAddToBoardUserNames.Count - 1);
                            }
                            list_lstTargetLike = Split(ClGlobul.lstAddToBoardUserNames, splitNo);
                        }
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }

                #endregion


                #region Comment
                //if (chkBox_Like_DivideData == true)
                //{
                //    int splitNo = 0;
                //    if (rdbDivideEqually==true)
                //    {
                //        splitNo = MaxLike / PDGlobals.loadedAccountsDictionary.Count;
                //        MaxLike = splitNo;

                //        if (PDGlobals.loadedAccountsDictionary.Count >= 1)
                //        {
                //            list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchLike);
                //            foreach (List<string> listAccounts in list_listAccounts)
                //            {
                //                foreach (string account in listAccounts)
                //                {
                //                    if (countThreadControllerLike > Nothread_Like)
                //                    {
                //                        try
                //                        {
                //                            lock (LikeObjThread)
                //                            {
                //                                Monitor.Wait(LikeObjThread);
                //                            }
                //                        }
                //                        catch (Exception Ex)
                //                        {

                //                        }
                //                    }

                //                    string acc = account.Split(':')[0];
                //                    PinInterestUser objPinInterestUser = null;
                //                    PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                //                    if (objPinInterestUser != null)
                //                    {
                //                        Thread profilerThread = new Thread(StartLikeMultiThreaded);
                //                        profilerThread.Name = "workerThread_Profiler_" + acc;
                //                        profilerThread.IsBackground = true;

                //                        profilerThread.Start(new object[] { objPinInterestUser });

                //                        countThreadControllerLike++;
                //                    }
                //                }

                //            }
                //        }
                //    }
                //    else if (rdbDivideByUser == true)
                //    {
                //        //if (!string.IsNullOrEmpty(txtNoOfLike.Text) && NumberHelper.ValidateNumber(txtNoOfLike.Text))
                //        //{
                //        //int res = CountGivenByUser;
                //            //DivideByUserinput = res;
                //            int Splitno = MaxLike / CountGivenByUser;
                //            MaxLike = CountGivenByUser;
                //            int counter = 0;
                //            //foreach (KeyValuePair<string, PinInterestUser> item in PDGlobals.loadedAccountsDictionary)
                //            //{
                               
                //                if (PDGlobals.listAccounts.Count >= 1)
                //                {
                //                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchLike);
                //                    foreach (List<string> listAccounts in list_listAccounts)
                //                    {
                //                        foreach (string account in listAccounts)
                //                        {

                //                            if (Splitno == counter)
                //                            {
                //                                break;
                //                            }
                //                            if (countThreadControllerLike > Nothread_Like)
                //                            {
                //                                try
                //                                {
                //                                    lock (LikeObjThread)
                //                                    {
                //                                        Monitor.Wait(LikeObjThread);
                //                                    }
                //                                }
                //                                catch (Exception Ex)
                //                                {

                //                                }
                //                            }

                //                            string acc = account.Split(':')[0];
                //                            PinInterestUser objPinInterestUser = null;
                //                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                //                            if (objPinInterestUser != null)
                //                            {
                //                                Thread profilerThread = new Thread(StartLikeMultiThreaded);
                //                                profilerThread.Name = "workerThread_Profiler_" + acc;
                //                                profilerThread.IsBackground = true;

                //                                profilerThread.Start(new object[] { objPinInterestUser });

                //                                countThreadControllerLike++;
                //                            }
                                         
                //                        }
                //                        counter++;
                //                    }

                //                }
                //               // ThreadPool.QueueUserWorkItem(new WaitCallback(StartLikeMultiThreaded), new object[] { item });
                               
                //           // }

                //        //}
                //    }
                //}
               // else
                //{
                #endregion
                if (PDGlobals.listAccounts.Count >= 1)
                    {
                        list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchLike);
                        foreach (List<string> listAccounts in list_listAccounts)
                        {
                            foreach (string account in listAccounts)
                            {
                                if (countThreadControllerLike > Nothread_Like)
                                {
                                    try
                                    {
                                        lock (LikeObjThread)
                                        {
                                            Monitor.Wait(LikeObjThread);
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
                                    Thread profilerThread = new Thread(StartLikeMultiThreaded);
                                    profilerThread.Name = "workerThread_Profiler_" + acc;
                                    profilerThread.IsBackground = true;

                                    profilerThread.Start(new object[] { objPinInterestUser });

                                    countThreadControllerLike++;
                                }
                            }

                        }
                    }
                //}

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }        
        }

        public void StartLikeMultiThreaded(object objparameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {                    
                if (!isStopLike)
                {
                    try
                    {
                        lstThreadsLike.Add(Thread.CurrentThread);
                        lstThreadsLike.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objparameters;
                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                        try
                        {
                            if (chkBox_Like_DivideData == true)
                            {
                                list_lstTargetLike_item = list_lstTargetLike[LstCounter_Like];
                            }
                            else
                            {
                                list_lstTargetLike_item = ClGlobul.lstAddToBoardUserNames;
                            }

                        }
                        catch { }
                        #region Login
                        if (!objPinUser.isloggedin)
                        {
                            GlobusLogHelper.log.Info(" => [ Logging With " + objPinUser.Username + " ]");

                            if (string.IsNullOrEmpty(objPinUser.ProxyPort))
                            {
                                objPinUser.ProxyPort = "80";
                            }

                            bool checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                //ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                            // accountManager.Login();

                            // if (!accountManager.LoggedIn)
                            if (!checkLogin)
                            {
                                checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                if (!checkLogin)
                                {
                                    GlobusLogHelper.log.Info(" => [ Not Logged In With " + objPinUser.Username + " ]");
                                    return;
                                }
                            }
                            string checklogin = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                            GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                            StartActionMultithreadLike(ref objPinUser, list_lstTargetLike_item);
                        }
                        else if(objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadLike(ref objPinUser, list_lstTargetLike_item);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }
                        }
                        #endregion
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

            finally
            {
                try
                {
                    if (countThreadControllerLike > Nothread_Like)
                    {
                        lock (LikeObjThread)
                        {
                            Monitor.Pulse(LikeObjThread);
                        }
                        Likedata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerLike--;

                //GlobusLogHelper.log.Info(" => [ Liked Process Finished ]");
                //GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED ]");
                //GlobusLogHelper.log.Info("----------------------------------------------------------------------------");
            }                           
        }

        public void StartActionMultithreadLike(ref PinInterestUser objPinUser, List<string> UserLikecount)
        {
            try
            {
                int LikeCount = 0;              
                if (rbListLikePinUrls == true)
                {
                    ClGlobul.lstPins = UserLikecount;
                    if (MaxLike == ClGlobul.lstPins.Count)
                    {
                        ClGlobul.lstPins.Add("1");
                    }
                    #region Comment
                    //if (rdbDivideByUser == true)
                    //{
                    //    int splitNo = 0;
                    //    //LikePinLst = Split(ClGlobul.lstAddToBoardUserNames, MaxLike);
                    //    list_lstTargetLike = Split(ClGlobul.lstAddToBoardUserNames, splitNo);
                    //    //List<List<string>> Temp_LikePinList = new List<List<string>>();
                    //    //Temp_LikePinList = LikePinLst;
                    //    foreach (List<string> item_LikePinLst in list_lstTargetLike)
                    //    {
                    //        ClGlobul.lstPins = item_LikePinLst;
                    //        LikePinLst.Remove(item_LikePinLst);
                    //        break;
                    //    }
                    //}
                    #endregion Comment
                }
                else
                {
                    if (rdbDivideByUser == true)
                    {
                        GlobusLogHelper.log.Info(" => [ Using Random Url to Like ]");
                        int countLoadEmails = PDGlobals.loadedAccountsDictionary.Count();
                        lstAllPins = GetPins(ref objPinUser, MaxLike);
                        Random Pinrnd = new Random();
                        ClGlobul.lstPins = lstAllPins.OrderBy(X => Pinrnd.Next()).ToList();
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Using Random Url to Like ]");
                        int countLoadEmails = PDGlobals.loadedAccountsDictionary.Count();
                        lstAllPins = GetPins(ref objPinUser, MaxLike);                      
                        Random Pinrnd = new Random();
                        ClGlobul.lstPins = lstAllPins.OrderBy(X => Pinrnd.Next()).ToList();

                    }
                }
                #region foreach
                foreach (string Pin in ClGlobul.lstPins)
                {
                    if (MaxLike > LikeCount)
                    {
                        try
                        {
                            DataSet dt = DataBaseHandler.SelectQuery("SELECT * FROM LikeUsingUrl Where LikeUrl = '" + Pin + "' and UserName = '" + objPinUser.Username + "' ", "LikeUsingUrl");
                            int count_NO_RoWs = dt.Tables[0].Rows.Count;
                            if (count_NO_RoWs == 0)
                            {
                                bool IsLiked = Like_New(ref objPinUser, Pin);

                                if (IsLiked)
                                {
                                    #region AccountReport

                                    string module = "Like";
                                    string status = "Liked";
                                    Qm.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + Pin, "", "", "", "", "", status, "", "", DateTime.Now);
                                    objLikeDelegate();

                                    #endregion

                                    GlobusLogHelper.log.Info(" => [ Liked : " + Pin + " From " + objPinUser.Username + " ]");

                                    try
                                    {
                                        if (!string.IsNullOrEmpty(Pin) && !string.IsNullOrEmpty(objPinUser.Username))
                                        {
                                            string query = "INSERT INTO  LikeUsingUrl (LikeUrl,UserName) VALUES ('" + Pin + "' ,'" + objPinUser.Username + "') ";
                                            DataBaseHandler.InsertQuery(query, "LikeUsingUrl");
                                        }

                                        try
                                        {
                                            string CSV_Header = "Username" + "," + "Pin" + "," + "" + "Date";
                                            string CSV_Data = objPinUser.Username + "," + "https://www.pinterest.com/pin/" + Pin + "/" + "," + System.DateTime.Now.ToString();
                                            string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Like");
                                            PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\NormalLike.csv");
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                    }
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ Not Liked : " + Pin + " From " + objPinUser.Username + " ]");
                                }
                                int Delay = RandomNumberGenerator.GenerateRandom(minDelayLike, maxDelayLike);
                                GlobusLogHelper.log.Info(" => [ Delay For " + Delay + " Seconds ]");
                                Thread.Sleep(Delay * 1000);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [ Already Liked : " + Pin + " From " + objPinUser.Username + " ]");
                                int Delay = RandomNumberGenerator.GenerateRandom(minDelayLike, maxDelayLike);
                                GlobusLogHelper.log.Info(" => [ Delay For " + Delay + " Seconds ]");
                                Thread.Sleep(Delay * 1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                        LikeCount++;
                    }
                    else
                    {
                        break;
                    }
                }
#endregion
                if (MaxLike <= LikeCount)
                {
                    GlobusLogHelper.log.Info(" [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                    GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }   


        public static List<List<string>> Split(List<string> source, int splitNumber)
        {
            if (splitNumber <= 0)
            {
                splitNumber = 1;
            }

            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / splitNumber)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


       

        public List<string> GetPins(ref PinInterestUser objPinUser, int MaxPin)
        {
            string CategoryName = string.Empty;
            //List<string> _Pins = new List<string>();
            try
            {
                PinterestPins pintrestPins = new PinterestPins();

                // int EmailCount = lstAccounts.Count;
                int EmailCount = LoadedAccountCount;

                int PageCount = ((MaxPin * EmailCount) / 25) + 2;

                if (PageCount > 10)
                {
                    PageCount = 10;
                }

                //if (rbPinType.Checked)
                //{
                   try
                    {
                        //if (rbPopularPin.Checked)
                        //{
                            //  if (lstPins.Count <= 0)
                            //{
                        ClGlobul.lstPins = pintrestPins.PopularPins(PageCount, ref objPinUser);
                            //}
                        //}
                        //else if (rbVideoPin.Checked)
                        //{
                                if (ClGlobul.lstPins.Count <= 0)
                                {
                                    ClGlobul.lstPins = pintrestPins.VideoPins(PageCount, ref objPinUser);
                                }
                        //}
                                if (ClGlobul.lstPins.Count <= 0)
                                {
                                    ClGlobul.lstPins = pintrestPins.NormalPins(ref objPinUser);
                                }
                    }
                    catch (Exception)
                    {

                    }
                //}
                //else
                //{
                    try
                    {
                        if (ClGlobul.lstPins.Count <= 0)
                        {

                            ClGlobul.lstPins = pintrestPins.CategoryPins(CategoryName, PageCount, ref objPinUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                    }
               // }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }
            return ClGlobul.lstPins;
        }

        public  bool Like_New(ref PinInterestUser objPinUser, string PinId)
        {
            try
            {
                string LikeUrl = string.Empty;
                string Pin = PinId;

                if (PinId.Contains("pinterest.com"))
                {
                    Pin = ObjAccountManager.getBetween(Pin, "pinterest.com/pin/", "/");
                    PinId = Pin;
                }

                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }
                string RedirectUrlDomain = GlobusHttpHelper.valueURl.Split('.')[0];
                string newHomePage = RedirectUrlDomain + ".pinterest.com/";

                LikeUrl = RedirectUrlDomain + ".pinterest.com/resource/PinLikeResource2/create/";

                string PostData = "source_url=%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%7D%7D&module_path=App()%3EHomePage()%3EAuthHomePage(resource%3DUserResource(username%3D" + objPinUser.Username + "))%3EGrid(resource%3DUserHomefeedResource())%3EGridItems(resource%3DUserHomefeedResource())%3EPin(resource%3DPinResource(id%3D" + PinId + "))%3EPinLikeButton(liked%3Dfalse%2C+class_name%3DlikeSmall%2C+pin_id%3D" + PinId + "%2C+has_icon%3Dtrue%2C+tagName%3Dbutton%2C+text%3DLike%2C+show_text%3Dfalse%2C+ga_category%3Dlike)";

                string AfterLikePageSourceData = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri(LikeUrl), PostData, newHomePage);


                if (!AfterLikePageSourceData.Contains("<div>Uh oh! Something went wrong"))
                {
                    //Log("[ " + DateTime.Now + " ] => [ Successfully Liked For this User " + pinterestAccountManager.Username + " ]");
                    return true;
                }
                else
                {
                    if (AfterLikePageSourceData.Contains("You are liking really fast"))
                    {
                        GlobusLogHelper.log.Info(" => [ You are liking really fast. Slow down a little. Try after some time " + objPinUser.Username + " ]");
                        return false;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Like Process Failed For this User " + objPinUser.Username + " ]");
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" => Error : " + ex.StackTrace);
                return false;
            }
            return false;

        }


    }
}
