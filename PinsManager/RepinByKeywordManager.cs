using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PinsManager
{
    public delegate void AccountReport_RepinByKeyword();
    public class RepinByKeywordManager
    {
        public static AccountReport_RepinByKeyword objRepinByKeywordDelegate;
        #region Global Variable

        public int Nothread_RepinByKeyword = 5;
        public bool isStopRepinByKeyword = false;
        public List<Thread> lstThreadsRepinByKeyword = new List<Thread>();
        public static int countThreadControllerRepinByKeyword = 0;
        public static int RepinByKeyworddata_count = 0;
        public readonly object RepinByKeywordObjThread = new object();
        public bool _IsfevoriteRepinByKeyword = false;
        public int MaxCountRepinByKeyword = 5;
        public bool rdbSingleUserRepinByKeyword = false;
        public bool rdbMultipleUserRepinByKeyword = false;  

        public int minDelayRepinByKeyword
        {
            get;
            set;
        }

        public int maxDelayRepinByKeyword
        {
            get;
            set;
        }

        public int NoOfThreadsRepinByKeyword
        {
            get;
            set;
        }

      
        List<string> TemplstListOfBoardNames = new List<string>();
        List<Thread> boardinput = new List<Thread>();
      

        string Boardpath = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Repin");

        #endregion

        Accounts ObjAccountManager = new Accounts();
        QueryManager Qm = new QueryManager();

        public void StartRepinKeyword()
        {
            try
            {
                countThreadControllerRepinByKeyword = 0;
                int numberOfAccountPatchBoards = 25;

                if (numberOfAccountPatchBoards > 0)
                {
                    numberOfAccountPatchBoards = NoOfThreadsRepinByKeyword;
                }

                RepinByKeyworddata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchBoards);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerRepinByKeyword > Nothread_RepinByKeyword)
                            {
                                try
                                {
                                    lock (RepinByKeywordObjThread)
                                    {
                                        Monitor.Wait(RepinByKeywordObjThread);
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
                                Thread profilerThread = new Thread(StartRepinByKeywordMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerRepinByKeyword++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartRepinByKeywordMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopRepinByKeyword)
                {
                    try
                    {
                        lstThreadsRepinByKeyword.Add(Thread.CurrentThread);
                        lstThreadsRepinByKeyword.Distinct().ToList();
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

                                    GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadRepinByKeyword(ref objPinUser);
                            }

                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                            }
                        }
                        else if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadRepinByKeyword(ref objPinUser);
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
        }

        public void StartActionMultithreadRepinByKeyword(ref PinInterestUser objPinInUser)
        {
            PinInterestUser objPinUser = objPinInUser;
            try
            {
                foreach (var item_TemplstListOfBoardNames in ClGlobul.lstRepinByKeyword)
                {
                    try
                    {
                        string[] url = Regex.Split(item_TemplstListOfBoardNames, "::");
                        string Keyword = url[2].ToString();
                        string Boardname = string.Empty;
                        string niche = string.Empty;
                        niche = url[0].ToString();
                        Boardname = url[1].ToString();
                        if (niche == objPinUser.Niches)
                        {
                            Thread thread = new Thread(() => RepinToBoard(Boardname, Keyword, ref objPinUser));
                            thread.Start();
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                    Thread.Sleep(10 * 1000);
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
                    if (countThreadControllerRepinByKeyword > Nothread_RepinByKeyword)
                    {
                        lock (RepinByKeywordObjThread)
                        {
                            Monitor.Pulse(RepinByKeywordObjThread);
                        }
                    }
                    RepinByKeyworddata_count--;
                }
                catch (Exception Ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
                }
                countThreadControllerRepinByKeyword--;
            }
        }

        public void RepinToBoard(string BoardName, string KeywordToRepin, ref PinInterestUser objPinInUser)
        {
            try
            {
                string BoardId = string.Empty;
                AddNewPinManager objaddnewPin = new AddNewPinManager();
                RePinManager objRepin = new RePinManager();
                try
                {
                    boardinput.Add(Thread.CurrentThread);
                    boardinput.Distinct();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };


                GlobusHttpHelper objHttp = new GlobusHttpHelper();

                try
                {

                    BoardId = objaddnewPin.GetBoardId_Board(BoardName, ref objPinInUser);
                }
                catch (Exception ex)
                {

                }


                if (string.IsNullOrEmpty(BoardId))
                {
                    BoardId = objaddnewPin.GetBoardId(BoardName, ref objPinInUser);
                }

                if (BoardId.Contains("null"))
                {
                    GlobusLogHelper.log.Info(" => [ Board is not present in " + objPinInUser.Username + "]");                 
                }

                int counter = 0;
                int RepinCounter = 0;
                if (!BoardId.Contains("failure"))
                {
                    if (!string.IsNullOrEmpty(BoardId))
                    {
                        if (PDGlobals.ValidateNumber(BoardId))
                        {
                            GlobusLogHelper.log.Info(" => [ Adding Pins From Board :" + KeywordToRepin + " ]");

                            clsSettingDB db = new clsSettingDB();
                            List<string> lstPins=new List<string>();
                            List<string> lstRepinPins = new List<string>();
                            lstPins = KeywordPinslist(KeywordToRepin, MaxCountRepinByKeyword, ref objPinInUser); 
                            lstPins = lstPins.Distinct().ToList();

                            lstRepinPins.AddRange(lstPins);
                            int lenOFlstPins = lstRepinPins.Count;

                            string[] lstOfRepin = null;
                            lstOfRepin = lstPins.ToArray();
                            foreach (string Pins in lstOfRepin)
                            {

                                if (MaxCountRepinByKeyword > RepinCounter)
                                {
                                    string Message = string.Empty;
                                    if (ClGlobul.lstMsgRepinByKeyword.Count > 0)
                                    {
                                        if (counter < ClGlobul.lstMsgRepinByKeyword.Count)
                                        {
                                            Message = ClGlobul.lstMsgRepinByKeyword[counter];
                                        }
                                        else
                                        {
                                            counter = 0;
                                            Message = ClGlobul.lstMsgRepinByKeyword[counter];
                                        }

                                    }

                                    bool IsReppined = false;
                                    if (!Pins.Contains("n"))
                                    {
                                        try
                                        {
                                            int index = lstOfRepin.Where(x => x == Pins).Select(x => lstRepinPins.IndexOf(x)).Single<int>();

                                            string NoOfPages = Convert.ToString(index / lenOFlstPins);

                                            IsReppined = objRepin.RepinwithMessage(Pins, Message, BoardId, NoOfPages, ref objPinInUser);
                                        }

                                        catch { }
                                        if (IsReppined)
                                        {
                                            #region AccountReport

                                            string module = "RepinByKeyword";
                                            string status = "Repined";
                                            Qm.insertAccRePort(objPinInUser.Username, module, "https://www.pinterest.com/pin/" + Pins, BoardName, "", Message, KeywordToRepin, "", status, "", "", DateTime.Now);
                                            objRepinByKeywordDelegate();

                                            #endregion

                                            GlobusLogHelper.log.Info(" => [ Repin Pin : " + Pins + " to Account : " + objPinInUser.Username + " In " + BoardName + " ]");
                                            try
                                            {
                                                string CSV_Header = "Date" + "," + "UserName" + "," + "Message" + "," + "Pin" + "Board Id";
                                                string CSV_Data = System.DateTime.Now.ToString() + "," + objPinInUser.Username + "," + Message + "," + "https://www.pinterest.com/pin/" + Pins + "," + BoardId;

                                                PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, Boardpath + "\\RepinByKeyword.csv");
                                                RepinCounter++;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            //kept here
                                            int delay = RandomNumberGenerator.GenerateRandom(minDelayRepinByKeyword, maxDelayRepinByKeyword);
                                            GlobusLogHelper.log.Info(" => [ Delay for " + delay + " Seconds ]");
                                            Thread.Sleep(delay * 1000);
                                        }
                                        else
                                        {

                                        }

                                        counter++;
                                    }
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinInUser.Username + " In " + BoardName + "]");
                                    GlobusLogHelper.log.Info("-----------------------------------------------------------------------------");
                                    break;
                                }
                            }

                        }
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("  => [ You already have a board with that name. " + objPinInUser.Username + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

        }


        public List<string> KeywordPinslist(string keyword, int Count, ref PinInterestUser objPinUser)
        {
            List<string> lstUserPins = new List<string>();
            try
            {
                string UserUrl = string.Empty;
                string UserPins = string.Empty;
                string bookmark = string.Empty;
                string UserPageSource = string.Empty;              
                          
                try
                {
                    GlobusLogHelper.log.Info(" => [ Start Getting Pins For this User " + keyword + " ]");

                    for (int i = 0; i <= Count; i++)
                    {
                        try
                        {
                            if (i == 0)
                            {
                                UserUrl = "http://pinterest.com/search/?q=" + keyword;
                            }
                            else
                            {
                                //http://pinterest.com/resource/SearchResource/get/?source_url=%2Fsearch%2Fpins%2F%3Fq%3Dhairstyle&data=%7B%22options%22%3A%7B%22query%22%3A%22hairstyle%22%2C%22bookmarks%22%3A%5B%22b28xMDB8MDQ0NWZiOTBjNzNiODlkOTQ1ZTk3ZjY0ZTBhYjU0YjM0ZDYyNDg3NjU3ZWQ3OGJmZjI4ZTliZGRmODBlMzJlNQ%3D%3D%22%5D%2C%22show_scope_selector%22%3Atrue%2C%22scope%22%3A%22pins%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22fc93456%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A2%7D&_=1375699543906
                                UserUrl = "http://pinterest.com/resource/SearchResource/get/?source_url=%2Fsearch%2Fpins%2F%3Fq%3D" + keyword + "&data=%7B%22options%22%3A%7B%22query%22%3A%22" + keyword + "%22%2C%22bookmarks%22%3A%5B%22" + Uri.EscapeDataString(bookmark) + "%22%5D%2C%22show_scope_selector%22%3Atrue%2C%22scope%22%3A%22pins%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A" + i + "%7D&_=" + DateTime.Now.Ticks;
                            }

                            try
                            {
                                UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserUrl), "http://pinterest.com/", string.Empty, 80, "", "", objPinUser.UserAgent);
                            }
                            catch
                            {
                                UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserUrl), objPinUser.ProxyAddress, Convert.ToInt32(objPinUser.ProxyPort), objPinUser.ProxyUsername, objPinUser.ProxyPassword);
                            }
                            ///get bookmarks value from page 
                            ///
                            if (UserPageSource.Contains("bookmarks"))
                            {
                                string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(UserPageSource, "bookmarks");

                                string Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                                bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);

                            }

                            List<string> lst = System.Text.RegularExpressions.Regex.Split(UserPageSource, "pin_id").ToList();
                            List<string> templst = new List<string>();
                            foreach (string item in lst)
                            {
                                if (!item.StartsWith("\": \"") || item.Contains("$") || item.Contains("?{pin}"))
                                {
                                    continue;
                                }

                                if (item.Contains("id\": \"pinItButton"))// && item.Contains("/repins/"))
                                {
                                    try
                                    {
                                        int FirstPinPoint = item.IndexOf("\": \"");
                                        int SecondPinPoint = item.IndexOf("}, ");
                                        if (SecondPinPoint > 30)
                                        {
                                            SecondPinPoint = item.IndexOf("\", ") + 1;
                                        }
                                        string Pinid = item.Substring(FirstPinPoint + 4, SecondPinPoint - FirstPinPoint - 5).Trim();

                                        if (!lstUserPins.Any(pid => pid == Pinid))
                                        {
                                            lstUserPins.Add(Pinid);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info(" => [ Sorry No More Pages ]");
                        }


                        lstUserPins = lstUserPins.Distinct().ToList();
                        // lstUserPins.Reverse();
                    }

                    GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstUserPins.Count + " ]");
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstUserPins;
        }












    }
}
