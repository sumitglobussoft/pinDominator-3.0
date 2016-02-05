using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using LikeManager;
using ScraperManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PinsManager
{
    public delegate void AccountReport_Repin();
    public delegate void Repin_Comments_UserPins_Repin(string UserName, ref PinInterestUser objPinUser);
    public class RePinManager
    {
        public static AccountReport_Repin objRepinDelegate;

        #region Global Variable
        public int Nothread_RePin = 5;
        public bool isStopRePin = false;
        public List<Thread> lstThreadsRePin = new List<Thread>();
        public static int countThreadControllerRePin = 0;
        public static int RePindata_count = 0;
        public readonly object RePinObjThread = new object();
        public int maxNoOfRePinCount = 5;
        public static bool rdbRepinNormalType = false;
        public static bool rbdRepinUserRepin = false;
        public static bool rdbUsePinNo = false;
        public static bool chkDivideData_RePin = false;
        public bool _IsfevoriteRepin = false;
        public static Repin_Comments_UserPins_Repin objRepin_Comments_UserPins_Repin;

        public int minDelayRePin
        {
            get;
            set;
        }

        public int maxDelayRePin
        {
            get;
            set;
        }

        public int NoOfThreadsRePin
        {
            get;
            set;
        }

        string UserUrl = string.Empty;
        List<string> Boards = new List<string>();
        List<string> lstAllPins = new List<string>();
        List<string> lstFollowings = new List<string>();
        string ac = string.Empty;
        string FollowingName = string.Empty;
        string oneTimePagesource = string.Empty;
        string BoardName = string.Empty;
        string[] arrBoardName = null;
        List<string> lstUserPins = new List<string>();
        string UserPageSource = string.Empty;
        string UserPinPageSource = string.Empty;
        string getPinPageSource = string.Empty;
        string description = string.Empty;
        string link = string.Empty;
        string boardId = string.Empty;
        string RepinPagesource = string.Empty;

        public static bool rdbDivideEqually_RePin = false;
        public static bool rdbDivideGivenByUser_RePin = false;
        public static int CountGivenByUser_RePin = 0;
        List<List<string>> list_lstTargetRePin = new List<List<string>>();
        List<string> list_lstTargetRePin_item = new List<string>();
        int LstCounter = 0;

        public static object Lock_RepinonBoard = new object();
        Accounts ObjAccountManager = new Accounts();
        QueryManager Qm = new QueryManager();

        #endregion

        public void StartRepin()
        {
            try
            {
                countThreadControllerRePin = 0;
                int numberOfAccountPatchRePin = 25;

                if (NoOfThreadsRePin > 0)
                {
                    numberOfAccountPatchRePin = NoOfThreadsRePin;
                }

                RePindata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();

                #region Divide Data Setting
                if (chkDivideData_RePin == true)
                {
                    if (rdbDivideGivenByUser_RePin == true || rdbDivideEqually_RePin == true)
                    {
                        int splitNo = 0;
                        if (rdbDivideEqually_RePin == true)
                        {
                            splitNo = ClGlobul.lstRepinUrl.Count / PDGlobals.listAccounts.Count;
                        }
                        else if (rdbDivideGivenByUser_RePin == true)
                        {
                            if (Convert.ToInt32(CountGivenByUser_RePin) != 0)
                            {
                                int res = Convert.ToInt32(Convert.ToInt32(CountGivenByUser_RePin));
                                splitNo = res;
                            }
                        }
                        if (splitNo == 0)
                        {
                            splitNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstRepinUrl.Count - 1);
                        }
                        list_lstTargetRePin = Split(ClGlobul.lstRepinUrl, splitNo);
                    }
                }

                #endregion

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchRePin);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerRePin > NoOfThreadsRePin)
                            {
                                try
                                {
                                    lock (RePinObjThread)
                                    {
                                        Monitor.Wait(RePinObjThread);
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
                                Thread profilerThread = new Thread(StartRePinMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerRePin++;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartRePinMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopRePin)
                {
                    try
                    {
                        lstThreadsRePin.Add(Thread.CurrentThread);
                        lstThreadsRePin.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;

                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);
                        try
                        {
                            if (chkDivideData_RePin == true)
                            {
                                list_lstTargetRePin_item = list_lstTargetRePin[LstCounter];
                            }
                            else
                            {
                                list_lstTargetRePin_item = ClGlobul.lstRepinUrl;
                            }

                        }
                        catch(Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }

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
                                StartActionMultithreadRePin(ref objPinUser, list_lstTargetRePin_item);
                            }
                            catch { };
                        }
                        else if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadRePin(ref objPinUser, list_lstTargetRePin_item);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                            }
                        }
                        #endregion                     
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                    }
                }
            }
            
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
        LikeManagers objLikeManager = new LikeManagers();
        ScraperManager objScrape = new ScraperManager();

        public void StartActionMultithreadRePin(ref PinInterestUser objPinUserRepin, List<string> Usercount)
        {
 
            try
            {             
                PinInterestUser objPinUser = (PinInterestUser)objPinUserRepin;
                string screen_Name = objPinUser.ScreenName;//ObjAccountManager.Getscreen_NameRepin(ref objPinUser);
                clsSettingDB db = new clsSettingDB();
                if (ObjAccountManager.Boards.Count <= 0)
                {
                    try
                    {
                        GetBoardsForRepinUpdated(ref objPinUser, screen_Name);
                        //new Thread(() => GetBoardsForRepinUpdated(ref objPinUser, screen_Name)).Start(); // Not returning only Board but Pics also
                        //GetBoardsForRepinUpdated(ref objPinUser, screen_Name);
                    }
                    catch (Exception Ex)
                    {

                    }
                }
                //string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (objPinUser.Boards.Count > 0 || objPinUser.Boards.Count == 0) 
                {
                    Random Boardrnd = new Random();
                    int BoardNum = 0;

                    try
                    {
                        BoardNum = Boardrnd.Next(0, objPinUser.Boards.Count - 1);
                    }
                    catch (Exception ex)
                    {
                        // GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartRepinMultiThreaded() 1--> " + ex.Message, ApplicationData.ErrorLogFile);
                    }

                    string BoardNumber = string.Empty;
                    try
                    {
                        BoardNum = Boardrnd.Next(0, objPinUser.Boards.Count - 1);
                        BoardNumber = objPinUser.Boards[BoardNum];
                    }
                    catch (Exception ex)
                    {
                        //GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartRepinMultiThreaded() 1--> " + ex.Message, ApplicationData.ErrorLogFile);
                    }

                    int RepinCount = 0;

                    if (!rdbUsePinNo)
                    {
                        try
                        {
                            if (rdbRepinNormalType == true)
                            {
                                GlobusLogHelper.log.Info(" => [ Getting Normal Pins From Account ]");
                                lstAllPins = objLikeManager.GetPins(ref objPinUser, maxNoOfRePinCount);
                                Random Pinrnd = new Random();
                                ClGlobul.lstPins = lstAllPins.OrderBy(x => Pinrnd.Next()).ToList();
                            }
                            else
                            {
                                try
                                {
                                    GlobusLogHelper.log.Info(" => [ Getting Random Users Pins From Account ]");

                                    string userName = objPinUser.ScreenName;
                                    userName = screen_Name;

                                    lstFollowings = objScrape.GetUserFollowing_new(userName, 1, maxNoOfRePinCount);

                                    foreach (string FollowName in lstFollowings)
                                    {
                                            try
                                            {
                                                Random rnd = new Random();
                                                int FollowingNum = rnd.Next(0, lstFollowings.Count - 1);
                                                FollowingName = lstFollowings[FollowingNum];
                                                ClGlobul.lstPins.Clear();
                                                List<string> lstRepinPin = UserPins_Repin(FollowingName, ref objPinUser);
                                                ClGlobul.lstPins.AddRange(lstRepinPin);
                                                ClGlobul.lstPins = ClGlobul.lstPins.Distinct().ToList();
                                                if (maxNoOfRePinCount < ClGlobul.lstPins.Count)
                                                {
                                                    break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                            }                                        
                                    }

                                    //  ClGlobul.lstPins = UserPins_Repin(FollowingName, ref objPinUser);
                                    //ClGlobul.lstPins = objLikeManager.GetPins(ref objPinUser, maxNoOfRePinCount);
                                    //checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }
                            }

                            string Message = string.Empty;
                            string[] lstPinsArray = ClGlobul.lstPins.ToArray();

                            foreach (string Pin in lstPinsArray)
                            {
                                try
                                {
                                    if (NumberHelper.ValidateNumber(Pin))
                                    {
                                        try
                                        {
                                            if (maxNoOfRePinCount == RepinCount)
                                            {
                                                break;
                                            }
                                            if (maxNoOfRePinCount >= RepinCount)
                                            {
                                                Random Messagernd = new Random();
                                                int MessageNum = 0;

                                                if (ClGlobul.RepinMessagesList.Count == 2)
                                                {
                                                    try
                                                    {
                                                        foreach (var itemMsg in ClGlobul.RepinMessagesList)
                                                        {
                                                            if (Message == itemMsg)
                                                            {
                                                                continue;
                                                            }
                                                            else
                                                            {
                                                                Message = itemMsg;
                                                                break;
                                                            }
                                                        }
                                                        GlobusLogHelper.log.Info(" => [ Message :" + Message + " ]");
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                                else if (ClGlobul.RepinMessagesList.Count > 0)
                                                {
                                                    try
                                                    {
                                                        MessageNum = Messagernd.Next(0, ClGlobul.RepinMessagesList.Count - 1);
                                                        Message = ClGlobul.RepinMessagesList[MessageNum].Trim();
                                                        GlobusLogHelper.log.Info(" => [ Message :" + Message + " ]");
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        //GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartRepinMultiThreaded() 2--> " + ex.Message, ApplicationData.ErrorLogFile);
                                                    }
                                                }

                                                try
                                                {
                                                    string NoOfPages = "0";//No use as far as I know
                                                    try
                                                    {
                                                        int index = ClGlobul.lstPins.Where(x => x == Pin).Select(x => ClGlobul.lstPins.IndexOf(x)).Single<int>();
                                                        NoOfPages = Convert.ToString(index / 25);
                                                    }
                                                    catch { }

                                                    try
                                                    {
                                                        Random rndBoard = new Random();
                                                        int NumBoard = rndBoard.Next(0, objPinUser.Boards.Count - 1);
                                                        BoardNumber = objPinUser.Boards[NumBoard];
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }

                                                    string Url = "https://www.pinterest.com/" + objPinUser.ScreenName;
                                                    if (string.IsNullOrEmpty(oneTimePagesource))
                                                    {
                                                        try
                                                        {
                                                            GlobusHttpHelper objHttp = new GlobusHttpHelper();
                                                            oneTimePagesource = objHttp.getHtmlfromUrl(new Uri(Url), "", "", "");
                                                        }
                                                        catch { };
                                                    }
                                                    if (arrBoardName == null)
                                                    {
                                                        try
                                                        {
                                                            arrBoardName = Regex.Split(oneTimePagesource, "board\", \"id\"");
                                                        }
                                                        catch { };
                                                    }
                                                    foreach (var itemBoardName in arrBoardName)
                                                    {
                                                        try
                                                        {
                                                            if (itemBoardName.Contains(BoardNumber))//&&itemBoardName.Contains("board\", \"id\""))
                                                            {
                                                                BoardName = Utils.Utils.getBetween(itemBoardName, "name", "}").Replace(":", "").Replace("\"", "").Trim();
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }
                                                    }

                                                    BoardName = BoardName.Replace(" ", "-").ToLower().Replace("(", "").Replace(")", "").Replace("!", "").Trim();
                                                    //checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                                                    bool IsRePined = RepinwithMessage(Pin, Message, BoardNumber, NoOfPages, ref objPinUser);
                                                    if (IsRePined)
                                                    {
                                                        #region AccountReport

                                                        string module = "RePin";
                                                        string status = "Repined";
                                                        Qm.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + Pin, "", "", Message, "", "", status, "", "", DateTime.Now);
                                                        objRepinDelegate();

                                                        #endregion

                                                        db.insertRePinRecord(objPinUser.Username, objPinUser.Niches, Pin);
                                                        RepinCount++;

                                                        try
                                                        {
                                                            string CSV_Header = "UserName" + "," + "Pin" + "," + "Message" + "," + "Board Number" + "," + "BoardUrl" + "," + "Date";
                                                            string CSV_Data = objPinUser.Username + "," + "https://www.pinterest.com/pin/" + Pin + "," + Message + "," + "Board No. : " + BoardNumber + "," + Url + "/" + BoardName + "," + System.DateTime.Now.ToString();
                                                            string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Repin");
                                                            PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\Repin.csv");
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {
                                                        RepinCount++;

                                                    }
                                                    if (rdbDivideGivenByUser_RePin == true)
                                                    {
                                                        CountGivenByUser_RePin--;
                                                        if (CountGivenByUser_RePin < 0)
                                                        {
                                                            break;
                                                        }
                                                    }

                                                    int delay = RandomNumberGenerator.GenerateRandom(minDelayRePin, maxDelayRePin);
                                                    GlobusLogHelper.log.Info(" => [ Delay For " + delay + " Seconds ]");
                                                    Thread.Sleep(delay * 1000);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartRepinMultiThreaded() 3--> " + ex.Message, ApplicationData.ErrorLogFile);
                                                }
                                            }

                                           
                                        }
                                        catch (Exception ex)
                                        {
                                            // GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartRepinMultiThreaded() 4--> " + ex.Message, ApplicationData.ErrorLogFile);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }                          
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if (rdbUsePinNo == true)
                    {
                        try
                        {

                            if (ClGlobul.lstRepinUrl.Count < maxNoOfRePinCount)
                            {
                                GlobusLogHelper.log.Info(" => [Repin Count can't be greater than uploaded Pins.]");
                                return;
                            }
                            GlobusLogHelper.log.Info(" => [ Repining Uploaded Pins ]");
                            int RepinCount_ListRepin_New = maxNoOfRePinCount;

                            //foreach (string RepinUrl in ClGlobul.lstRepinUrl)
                            foreach (string RepinUrl in Usercount)
                            {
                                try
                                {
                                    //if (!rdbDivideDataRePin)
                                    //{
                                        if (RepinCount_ListRepin_New <= 0)
                                        {
                                            break;
                                        }
                                    //}
                                }
                                catch { };
                              
                                string NoOfPages = "0";

                                try
                                {

                                    int index = ClGlobul.lstRepinUrl.Where(x => x == RepinUrl).Select(x => ClGlobul.lstRepinUrl.IndexOf(x)).Single<int>();
                                    NoOfPages = Convert.ToString(index / 25);
                                }
                                catch { };


                                string Message = string.Empty;
                                if (ClGlobul.RepinMessagesList.Count > 1)
                                {
                                    Message = ClGlobul.RepinMessagesList[RandomNumberGenerator.GenerateRandom(0, ClGlobul.RepinMessagesList.Count - 1)];
                                }
                                else if (ClGlobul.RepinMessagesList.Count == 1)
                                {
                                    Message = ClGlobul.RepinMessagesList[0];
                                }
                                else
                                {
                                    Message = "";
                                }

                                Thread.Sleep(1000);
                                bool IsRepinned = RepinwithMessage(RepinUrl.Replace(" ", ""), Message, BoardNumber, NoOfPages, ref objPinUser);

                                if (IsRepinned)
                                {
                                    #region AccountReport

                                    string module = "RePin";
                                    string status = "Repined";
                                    Qm.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + RepinUrl, "", "", Message, "", "", status, "", "", DateTime.Now);
                                    objRepinDelegate();

                                    #endregion

                                    GlobusLogHelper.log.Info(" => [ Repin Pin : " + RepinUrl + " ]");
                                    db.insertRePinRecord(objPinUser.Username, objPinUser.Niches, RepinUrl);
                                    RepinCount++;
                                    RepinCount_ListRepin_New--;
                                    try
                                    {
                                        string CSV_Header = "UserName" + "," + "RepinUrl" + "," + "Message" + "," + "Board Number" + "," + "Date";
                                        string CSV_Data = objPinUser.Username + "," + "https://www.pinterest.com/pin/" + RepinUrl + "," + Message + "," + "Board No. : " + BoardNumber + "," + System.DateTime.Now.ToString();
                                        string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Repin");
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\UsePin.csv");
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ Not Repin Pin : " + RepinUrl + " ]");
                                }
                                int Delay = RandomNumberGenerator.GenerateRandom(minDelayRePin, maxDelayRePin);
                                GlobusLogHelper.log.Info(" => [ Delay for " + Delay + " Seconds ]");
                                Thread.Sleep(Delay * 1000);

                            }
                        }

                        catch (Exception ex)
                        { };

                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ No Boards Found in Account :" + objPinUser.Username + " ]");
                    }
                }

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            finally
            {
                RePindata_count--;              
                try
                {
                    if (countThreadControllerRePin >= NoOfThreadsRePin)
                    {
                        lock (RePinObjThread)
                        {
                            Monitor.Pulse(RePinObjThread);
                        }
                    }
                }
                catch (Exception Ex)
                {

                }
                countThreadControllerRePin--;

                if (RePindata_count == 0 || CountGivenByUser_RePin < 0)
                {                  
                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
                }
            }
        }

        public void GetBoardsForRepinUpdated(ref PinInterestUser objPinUser, string Username)
        {
            try
            {                         
                GlobusLogHelper.log.Info(" => [ Start Getting Boards For User " + Username + " ]");                                                  
                try
                {
                    if (string.IsNullOrEmpty(Globals.ItemSelect))
                    {
                        UserUrl = "http://pinterest.com/" + Username;
                    }
                    else
                    {
                        UserUrl = "http://pinterest.com/" + Username + "/" + Globals.ItemSelect;
                    
                    }



                    GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();
                  //string aa = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinUser.UserAgent);
                    string aa = objGlobusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinUser.UserAgent);

                    string[] Items = Regex.Split(aa, Username);
                    Items = Items.Skip(1).ToArray();                   
                    foreach (string item in Items)
                    {
                        try
                        {
                            if (item.Contains("board_id"))
                            {
                                //if (counter == 1)
                                //{
                                string[] Data = System.Text.RegularExpressions.Regex.Split(item, "board_id");//{"board_id":
                                foreach (string Dataitem in Data)
                                {
                                    try
                                    {
                                        if (Dataitem.Contains("-end-"))
                                        {
                                            continue;
                                        }
                                        if (Dataitem.Contains("_1399706961_75.jpg"))
                                        {
                                            continue;
                                        }
                                        if (Dataitem.Contains("board_name") || Dataitem.Contains("name\": \"Board") || Dataitem.Contains("anchored\": true")) //"board_name": 
                                        {
                                            try
                                            {
                                                int LastPoint = Dataitem.IndexOf("board_name");//board_name //Board
                                                
                                                if (LastPoint <= 0)
                                                {
                                                    LastPoint = Dataitem.IndexOf(",");
                                                    ac = Dataitem.Substring(0, LastPoint).Replace("\": \"", string.Empty).Replace("\"", "").Replace(", ", string.Empty).Replace("field_set_keygrid_item}}name", string.Empty).Trim();
                                                }
                                                else
                                                {
                                                    ac = Dataitem.Substring(0, LastPoint).Replace("\": \"", string.Empty).Replace("\"", "").Replace(", ", string.Empty).Replace("field_set_keygrid_item}}name", string.Empty).Trim();
                                                }
                                                if (!objPinUser.Boards.Contains(ac))
                                                {
                                                    //I have to validate here so that only BoardId gets through
                                                    if (NumberHelper.ValidateNumber(ac))
                                                    {
                                                        objPinUser.Boards.Add(ac);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }

                                        }                                    
                                    }
                                    catch(Exception ex)
                                    { };
                                }//end of Foreach loop
                                
                            }

                        }

                        catch (Exception ex)
                        {

                        }
                    }

                    objPinUser.Boards.Count();
                    //objPinUser.Boards.AddRange(Boards);
          
                    GlobusLogHelper.log.Info(" => [ Get All Boards for User " + objPinUser.Name + " ]");
                }
                catch (Exception ex)
                { };
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" => [ Boards Getting Process Failed ]");
            }
        }




        public List<string> UserPins_Repin(string UserName, ref PinInterestUser objPinUser)
        {
            List<string> lstUsernamePin = new List<string>();
            try
            {
                GlobusRegex objGlobusRegex = new GlobusRegex();
                GlobusHttpHelper objglobusHttpHelper = new GlobusHttpHelper();
                GlobusLogHelper.log.Info(" => [ Start Getting Pins For this User " + UserName + " ]");
                string UserPins = string.Empty;
               
                if (UserName.Contains("pinterest.com"))
                {
                    UserUrl = UserName;
                    UserPins = UserName + "pins/";
                }
                else
                {
                    UserUrl = "https://pinterest.com/" + UserName + "/";
                    UserPins = "https://pinterest.com/" + UserName + "/pins/";
                }

                try
                {
                    UserPageSource = objglobusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "http://pinterest.com/", string.Empty, objPinUser.UserAgent);
                    UserPinPageSource = objglobusHttpHelper.getHtmlfromUrl(new Uri(UserPins), UserUrl, string.Empty, objPinUser.UserAgent);

                    // List<string> lst = globusRegex.GetHrefUrlTags(UserPinPageSource);

                }
                catch (Exception ex)
                {
                }
                List<string> lst = objGlobusRegex.GetHrefUrlTagsForPinDescription(UserPinPageSource);
                lst = lst.Distinct().ToList();
                string PinUrl = string.Empty;

                foreach (string item in lst)
                {
                    //if (item.Contains("/pin/") && !item.Contains("edit"))
                    try
                    {
                        if (item.Contains("/pin/"))
                        {
                            if (!item.Contains("/pin/A"))
                            {
                                try
                                {
                                    int FirstPinPoint = item.IndexOf("/pins/");
                                    int SecondPinPoint = item.IndexOf("class=");

                                    // PinUrl = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("pin", string.Empty).Replace("/", string.Empty).Trim();
                                    PinUrl = Utils.Utils.getBetween(item, "/pin/", "/\" class=");
                                    if (!string.IsNullOrEmpty(PinUrl))
                                    {
                                        lstUsernamePin.Add(PinUrl);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    PinUrl = item.Replace("/pins/", "");
                                    lstUsernamePin.Add(PinUrl);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                lstUsernamePin = lstUsernamePin.Distinct().ToList();
                lstUsernamePin.Reverse();

                GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstUsernamePin.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstUsernamePin;
        }



        //public bool RepinwithMessage(string PinId, string myMessage, string Board, string NumberOfPage, ref PinInterestUser objPinUser)
        //{
        //    try
        //    {
        //        string CsrfMiddleToken = string.Empty;
        //        string pinUrl = string.Empty;
        //        string url = "https://www.pinterest.com/pin/" + PinId;
        //        string CheckPinPageSource123 = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
        //        string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
        //        if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
        //        {
        //            // lstRePinPin.Add(lstRepinUrl_item);
        //            pinUrl = "https://www.pinterest.com/pin/" + PinId + "/";
        //        }
        //        else
        //        {
        //            GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Pin " + PinId + " Is InCorrect ]");
        //        }

        //        try
        //        {
        //            if (!string.IsNullOrEmpty(Globals.ItemSelect))
        //            {
        //                GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + "In" + Globals.ItemSelect + " ]");
        //            }
        //            else
        //            {
        //                GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " In " + BoardName + "]");

        //            }
        //        }
        //        catch { };

        //        try
        //        {
        //            getPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(pinUrl), "", "", "");
        //        }
        //        catch { };



        //        try
        //        {
        //            if (getPinPageSource.Contains("description_html"))
        //            {
        //                description = Utils.Utils.getBetween(getPinPageSource, "description_html\":", ", \"title\":").Replace("\"", "").Replace("&", "%26").Trim();
        //                description = description.Replace(" ", "+").Replace(",", "%2C").Replace("amp;", "");
        //            }
        //            if (getPinPageSource.Contains("serving_link"))
        //            {
        //                link = Utils.Utils.getBetween(getPinPageSource, "serving_link\":", ", \"is_promoted").Replace("\"", "").Trim();
        //                link = link.Replace(":", "%3A").Replace("/", "%2F").Replace("?", "%3F").Replace("=", "%3D").Replace("&", "%26");
        //            }
        //            if (getPinPageSource.Contains("board_id"))
        //            {
        //                try
        //                {
        //                    if (string.IsNullOrEmpty(Board))
        //                    {
        //                        Random rnd = new Random();
        //                        int BoardNum = rnd.Next(0, objPinUser.Boards.Count - 1);
        //                        boardId = objPinUser.Boards[BoardNum];
        //                        //objPinUser.Boards
        //                    }
        //                    else
        //                    {
        //                        boardId = Board;

        //                    }

        //                    if (string.IsNullOrEmpty(boardId.ToString()))
        //                    {
        //                        GlobusLogHelper.log.Info(" => [Board is not present in your account , can't repin]");
        //                        return false;
        //                    }
        //                    // Log("[ " + DateTime.Now + " ] => [ Repining " + boardId + " For " + pinterestAccountManager.Username + " ]");
        //                }
        //                catch (Exception)
        //                { };

        //            }

        //            //objPinUser.globusHttpHelper = new GlobusHttpHelper();

        //            lock (Lock_RepinonBoard)
        //            {
        //                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
        //                CsrfMiddleToken = Utils.Utils.getBetween(Checking, "\"csrftoken\": \"", "\", \"");
        //                if (Checking.Contains("profileName"))
        //                {

        //                }
        //                else
        //                {
        //                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
        //                }

        //                string linkurl = string.Empty;

        //                string RepinpostData = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + link + "%22%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22" + boardId + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=Modal()%3EPinCreate3(resource%3DPinResource(id%3D" + PinId + "))%3EBoardPicker(resource%3DBoardPickerBoardsResource(filter%3Dall))%3ESelectList(view_type%3DpinCreate3%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+item_module%3D%5Bobject+Object%5D)";
        //                RepinpostData = "source_url=%2F" + objPinUser.ScreenName + "%2Fpins%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + link + "%22%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22" + boardId + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPinCreate%3EBoardPicker%3ESelectList(view_type%3DpinCreate%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+scroll_selected_item_into_view%3Dtrue%2C+select_first_item_after_update%3Dfalse%2C+item_module%3D%5Bobject+Object%5D)";

        //                try
        //                {
        //                    RepinPagesource = objPinUser.globusHttpHelper.postFormDataProxyPincsrftoken(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), RepinpostData, "https://www.pinterest.com/", CsrfMiddleToken);
        //                }
        //                catch (Exception ex)
        //                {
        //                }


        //                if (string.IsNullOrEmpty(RepinPagesource))
        //                {

        //                    try
        //                    {
        //                        if (getPinPageSource.Contains("class=\"sourceFlagWrapper"))
        //                        {
        //                            try
        //                            {

        //                                BaseLib.GlobusRegex rgx = new GlobusRegex();

        //                                string urldata = System.Text.RegularExpressions.Regex.Split(System.Text.RegularExpressions.Regex.Split(getPinPageSource, "sourceFlagWrapper")[1], "</a>")[0];

        //                                linkurl = rgx.GetHrefUrlTag(urldata).Replace("href=\"", string.Empty);
        //                            }
        //                            catch (Exception ex)
        //                            {

        //                            }
        //                        }
        //                        else if (string.IsNullOrEmpty(linkurl))
        //                        {
        //                            try
        //                            {
        //                                string urldata = System.Text.RegularExpressions.Regex.Split(System.Text.RegularExpressions.Regex.Split(getPinPageSource, "sourceFlagWrapper")[1], "</a>")[0];

        //                                string Datavalue = urldata.Substring(urldata.IndexOf("href=\\\""));

        //                                int startindex = Datavalue.IndexOf("href=\\\"");
        //                                string start = Datavalue.Substring(startindex).Replace("href=\\\"", "");
        //                                int endindex = start.IndexOf("\\\"");
        //                                string end = start.Substring(0, endindex);

        //                                linkurl = end;// Datavalue.Substring(0, Datavalue.IndexOf("\\\">")).Replace("\\", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("href=\"", string.Empty);
        //                            }
        //                            catch { };
        //                        }

        //                        try
        //                        {
        //                            //objPinUser.globusHttpHelper.gCookies = new System.Net.CookieCollection();
        //                            //string CookieList = "_b=\"ARdBmc3sRRBC1b7mWyRRCA3QBovSmWHWonr/rL5WVoV3ABT+imiaVF1UnHkaoTAnSzE=\"; sessionFunnelEventLogged=1; __utmt=1; logged_out=True; fba=True; csrftoken=w4YapCH4AAPIlfxhvZR3Mb9XIXVYzMXA; _pinterest_sess=\"TWc9PSZlZU1LQU1EL1pIY3UxbjlhWGYrUGtMSkRwYzc5TFFJS3d4dTU0VEthUVlHK1VhT2NOQkQzcUtkekYzaWNYN212U2lyRlRxd2dYejhoSldadGw5M0ZzK2NyS3Q5ODhSemhUMG5HVlVVOEx6R1RWdkh0aWdFalNuSkxJZmo0ZE9jMG44b2RXZzZOeU83ZVdJWVVNdWZKZFRhOWEwK0ZDckc0NVIwdi8xdytjcDBtVHZtTjZOUUdtQk5acko0ODVaakEwS3dEN0taU2NrQkE3YlZMU3I2Q0VicDRoWTl3RHJpaDVzUnJ3YmlLNmhYbFpMbFp6K2FKTlIyZUNWUDRKN3I3dFNMejQyUVRhb0JtMTk0ajBtcVhUVzlrRVlVQzZwTjRwY3UzdG1tWFpxQ3FGV0ZOeXBNV0FMeURUL0crMmZvVzVNWWlPbjZyUnh2UXN3R3pOT2c2RVNoZFVxZG9lUnlDaHJpS0dORFoydHhZMFgxR0hWWE02OUZDQmdlSkhzRDdjREszcjdvWDBTaFJXM1BFM01VUnRpYjB4U3l6alQ2eWxmQTdrWVdYcWFpOFhETDRWSGlQazR0L2xUNTdiUGdhbG9WUUtLZHdrSVFMN1VaWHk2LytNeWkzcUJJbStUejVMYkljczY1Sk5GQT0mblE0ZXFxTDVyeGk5NVJ2Mmp1SEZVakNxcGNjPQ==\"; c_dpr=1; cm_sub=none; __utma=229774877.1623437774.1450182939.1450182939.1450249426.2; __utmb=229774877.15.9.1450261114039; __utmc=229774877; __utmz=229774877.1450182939.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)";
        //                            //string[] CookieList1234 = Regex.Split(CookieList, ";");
        //                            //foreach (string str in CookieList1234)
        //                            //{

        //                            //    string Cokkiename = Regex.Split(str, "=")[0].Replace(" ","");
        //                            //    string CokkieValue = Regex.Split(str, "=")[1].Replace(" ", "");

        //                            //    System.Net.Cookie cookie = new System.Net.Cookie();
        //                            //    cookie.Name = Cokkiename;
        //                            //    cookie.Value = CokkieValue;
        //                            //    cookie.Domain = "pinterest.com";
        //                            //    objPinUser.globusHttpHelper.gCookies.Add(cookie);

        //                            //}
        //                            string postdata1 = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";
        //                            // postdata1 = "source_url=%2F" + objPinUser.ScreenName + "%2Fpins%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + link + "%22%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22" + Board + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPinCreate%3EBoardPicker%3ESelectList(view_type%3DpinCreate%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+scroll_selected_item_into_view%3Dtrue%2C+select_first_item_after_update%3Dfalse%2C+item_module%3D%5Bobject+Object%5D)";
        //                            // "source_url=%2FPrashant123456%2Fdeveloper%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22350295677243761444%22%2C%22description%22%3A%22%22%2C%22link%22%3Anull%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22350295745955657898%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPinCreate%3EBoardPicker%3ESelectList(view_type%3DpinCreate%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+scroll_selected_item_into_view%3Dtrue%2C+select_first_item_after_update%3Dfalse%2C+item_module%3D%5Bobject+Object%5D)";

        //                            postdata1 = "source_url=%2FPrashant123456%2Flovely2%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22350295677245529486%22%2C%22description%22%3A%22Hii%22%2C%22link%22%3A%22http%3A%2F%2Fwebsta.me%2Fp%2F945717296717057301_1591089306%22%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22350295745955657875%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPinCreate%3EBoardPicker%3ESelectList(view_type%3DpinCreate%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+scroll_selected_item_into_view%3Dtrue%2C+select_first_item_after_update%3Dfalse%2C+item_module%3D%5Bobject+Object%5D)";



        //                            string afterposting = objPinUser.globusHttpHelper.postFormDataProxywithCSRFToken(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), postdata1, "http://www.pinterest.com/", "", 0, "", "", "CsrfMiddleToken");
        //                            //string afterposting = objPinUser.globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), postdata1, "http://www.pinterest.com/pin/" + PinId + "/", "", 0, "", "", CsrfMiddleToken);
        //                            if (!afterposting.Contains("<div>Uh oh! Something went wrong."))
        //                            {
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                return false;
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        };
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }



        //                if (!string.IsNullOrEmpty(RepinPagesource))
        //                {
        //                    try
        //                    {
        //                        if (!string.IsNullOrEmpty(Globals.ItemSelect))
        //                        {
        //                            GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " In " + Globals.ItemSelect + " is Done. ]");
        //                        }
        //                        else
        //                        {
        //                            GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Done. ]");
        //                        }
        //                        //string RepinDetails =pinterestAccountManager.Username + "," + PinId.Replace(",","") + "," + boardId.Replace(",","") + "," + myMessage.Replace(",","");

        //                        //GlobusFileHelper.AppendStringToTextfileNewLine(RepinDetails, ApplicationData.path_Repin);
        //                    }
        //                    catch { };

        //                    return true;
        //                }
        //                else
        //                {
        //                    GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Failed. ]");
        //                    return false;
        //                }

        //            }
        //        }
        //        catch { };

        //        return false;
        //    }
        //    catch (Exception Ex)
        //    {
        //        return false;
        //    }
        //}

        #region NEW_REPIN_WITH_MESSAGE
        public bool RepinwithMessage(string PinId, string myMessage, string Board, string NumberOfPage, ref PinInterestUser objPinUser)
        {
            try
            {
                string getPinPageSource = string.Empty;
                string pinUrl = string.Empty;
                string url = "https://www.pinterest.com/pin/" + PinId;
                string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
                if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
                {
                    // lstRePinPin.Add(lstRepinUrl_item);
                    pinUrl = "https://www.pinterest.com/pin/" + PinId + "/";
                }
                else
                {
                    GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Pin " + PinId + " Is InCorrect ]");
                }
                //  Thread.Sleep(10 * 1000);
                //pinUrl = "https://www.pinterest.com/pin/" + PinId + "/";
                //string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                try
                {
                    if (!string.IsNullOrEmpty(Globals.ItemSelect))
                    {
                        GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + "In" + Globals.ItemSelect + " ]");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " ]");

                    }
                }
                catch { };

                try
                {
                    GlobusHttpHelper objhttp = new GlobusHttpHelper();
                    getPinPageSource = objhttp.getHtmlfromUrl(new Uri(pinUrl), "", "", "");
                }
                catch { };

                string description = string.Empty;
                string link = string.Empty;
                string boardId = string.Empty;
                string RepinPagesource = string.Empty;
                string appVersion = string.Empty;
                try
                {
                    if (getPinPageSource.Contains("description_html"))
                    {
                        description = Utils.Utils.getBetween(getPinPageSource, "description_html\":", ", \"title\":").Replace("\"", "").Replace("&", "%26").Trim();
                        description = description.Replace(" ", "+").Replace(",", "%2C").Replace("amp;", "");
                    }
                    if (getPinPageSource.Contains("serving_link"))
                    {
                        link = Utils.Utils.getBetween(getPinPageSource, "serving_link\":", ", \"is_promoted").Replace("\"", "").Trim();
                        link = link.Replace(":", "%3A").Replace("/", "%2F").Replace("?", "%3F").Replace("=", "%3D").Replace("&", "%26");
                    }
                    if (getPinPageSource.Contains("board_id"))
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(Board))
                            {
                                Random rnd = new Random();
                                int BoardNum = rnd.Next(0, objPinUser.Boards.Count - 1);
                                boardId = objPinUser.Boards[BoardNum];

                            }
                            else
                            {
                                boardId = Board;

                            }

                            if (string.IsNullOrEmpty(boardId.ToString()))
                            {
                                GlobusLogHelper.log.Info(" => [Board is not present in your account , can't repin]");
                                return false;
                            }
                            // Log("[ " + DateTime.Now + " ] => [ Repining " + boardId + " For " + pinterestAccountManager.Username + " ]");
                        }
                        catch (Exception)
                        {


                        }

                    }
                    else if (getPinPageSource.Contains("board"))
                    {
                         boardId = Board;
                    }


                    lock (Lock_RepinonBoard)
                    {
                        string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                        if (Checking.Contains("profileName"))
                        {
                            if (Checking.Contains("profileName"))
                            {
                                appVersion = Utils.Utils.getBetween(Checking, "\"app_version\": \"", "\", \"");
                            }
                        }
                        else
                        {
                            ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                        }

                        string RedirectUrl = GlobusHttpHelper.valueURl.Split('.')[0];
                        string newHomePage = RedirectUrl + ".pinterest.com";

                        string linkurl = string.Empty;

                        string RepinpostData = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + link + "%22%2C%22is_video%22%3Afalse%2C%22board_id%22%3A%22" + Board + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=Modal()%3EPinCreate3(resource%3DPinResource(id%3D" + PinId + "))%3EBoardPicker(resource%3DBoardPickerBoardsResource(filter%3Dall))%3ESelectList(view_type%3DpinCreate3%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+item_module%3D%5Bobject+Object%5D)";
                        string PostPageUrl = RedirectUrl + ".pinterest.com/resource/RepinResource/create/";
                        try
                        {
                            RepinPagesource = objPinUser.globusHttpHelper.postFormDataProxyREPin(new Uri(PostPageUrl), RepinpostData, newHomePage, objPinUser.App_version);
                        }
                        catch (Exception ex)
                        {
                        }


                        if (string.IsNullOrEmpty(RepinPagesource))
                        {

                            try
                            {
                                if (getPinPageSource.Contains("class=\"sourceFlagWrapper"))
                                {
                                    try
                                    {

                                        BaseLib.GlobusRegex rgx = new GlobusRegex();

                                        string urldata = System.Text.RegularExpressions.Regex.Split(System.Text.RegularExpressions.Regex.Split(getPinPageSource, "sourceFlagWrapper")[1], "</a>")[0];

                                        linkurl = rgx.GetHrefUrlTag(urldata).Replace("href=\"", string.Empty);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else if (string.IsNullOrEmpty(linkurl))
                                {
                                    try
                                    {
                                        string urldata = System.Text.RegularExpressions.Regex.Split(System.Text.RegularExpressions.Regex.Split(getPinPageSource, "sourceFlagWrapper")[1], "</a>")[0];

                                        string Datavalue = urldata.Substring(urldata.IndexOf("href=\\\""));

                                        int startindex = Datavalue.IndexOf("href=\\\"");
                                        string start = Datavalue.Substring(startindex).Replace("href=\\\"", "");
                                        int endindex = start.IndexOf("\\\"");
                                        string end = start.Substring(0, endindex);

                                        linkurl = end;// Datavalue.Substring(0, Datavalue.IndexOf("\\\">")).Replace("\\", string.Empty).Replace("\n", string.Empty).Replace(">", string.Empty).Replace("href=\"", string.Empty);
                                    }
                                    catch { };
                                }

                                try
                                {
                                    #region comment
                                    //Message = Message.Replace(" ", "+");
                                    //string PostData = "source_url=%2Fpopular%2F%3Fpage%3D" + NumberOfPage + "&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + (Message.Replace(" ", "+")) + "%22%2C%22link%22%3A%22" + (Uri.EscapeDataString(linkurl)) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%7D%7D&module_path=App()%3EFeedPage()%3EGrid(resource%3DCategoryFeedResource(feed%3Dpopular))%3EGridItems(resource%3DCategoryFeedResource(feed%3Dpopular))%3EPin(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(submodule%3D%5Bobject+Object%5D%2C+color%3Dprimary%2C+text%3DPin+it%2C+primary_on_hover%3Dtrue%2C+extra_options%3D%5Bobject+Object%5D%2C+tagName%3Dbutton%2C+has_icon%3Dtrue%2C+show_text%3Dfalse%2C+ga_category%3Drepin_create%2C+class_name%3DrepinSmall%2C+require_auth%3Dtrue)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + "%2C+view_type%3Drepin)))";
                                    //string PostData = "source_url=%2Fpopular%2F%3Fpage%3D" + "1" + "&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + (Message.Replace(" ", "+").Replace("\t", "+")) + "%22%2C%22link%22%3A%22" + (Uri.EscapeDataString(linkurl)) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%7D%7D&module_path=App()%3EFeedPage()%3EGrid(resource%3DCategoryFeedResource(feed%3Dpopular))%3EGridItems(resource%3DCategoryFeedResource(feed%3Dpopular))%3EPin(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(submodule%3D%5Bobject+Object%5D%2C+color%3Dprimary%2C+text%3DPin+it%2C+primary_on_hover%3Dtrue%2C+extra_options%3D%5Bobject+Object%5D%2C+tagName%3Dbutton%2C+has_icon%3Dtrue%2C+show_text%3Dfalse%2C+ga_category%3Drepin_create%2C+class_name%3DrepinSmall%2C+require_auth%3Dtrue)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + "%2C+view_type%3Drepin)))";
                                    //string PostData = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + AddDesc + "%22%2C%22link%22%3A%22"+ linkurl +"%2F2011%2F11%2F01%2Fcaribbean-sangria%2F%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22520306563170537867%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%227d82d10%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D520306563170537867))%3EPinActionBar(resource%3DPinResource(id%3D520306563170537867))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D520306563170537867)))";
                                    //string postdata = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + Message.Replace(" ","+") + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22"+ PinId +"%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";
                                    // string postdata = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + (Message.Replace(" ", "+")) + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";

                                    //My Commented
                                    //string postdata1 = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22Message%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";

                                    //                                                                                                                                                 Have to add Message below here
                                    //string postdata1 = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + pinterestAccountManager.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";



                                    //source_url=%2Fpin%2F347410558725937380%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%2297460848125134285%22%2C%22description%22%3A%22Butterfly+tattoo%22%2C%22link%22%3A%22http%3A%2F%2Fwww.ruthtattooideas.com%2F2013%2F07%2F23%2Fbutterfly-tattoo%2F%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22347410558725937380%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%228c9869b%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D347410558725937380))%3EPinActionBar(resource%3DPinResource(id%3D347410558725937380))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D347410558725937380)))
                                    // string postdataa = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22description%22%3A%22" + (Message.Replace(" ", "+")) + "%22%2C%22link%22%3A%22" + SourceUrl + "%22%2C%22id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22" + pinterestAccountManager.App_version + "%3A%226bbb94a%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3ECloseupContent(resource%3DPinResource(id%3D381539399653733768))%3EPin(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinEdit)%23Modal(module%3DPinEdit(resource%3DPinResource(id%3D" + PinId + ")))";
                                    //string afterposting = pinterestAccountManager.httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), postdata1, "http://www.pinterest.com/pin/" + PinId + "/", "", 0, "", "");

                                    #endregion
                                    string postdata1 = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + myMessage + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";
                                    string afterposting = objPinUser.globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), postdata1, "http://www.pinterest.com/pin/" + PinId + "/", "", 0, "", "");
                                    if (!afterposting.Contains("<div>Uh oh! Something went wrong."))
                                    {
                                          return true;
                                    }
                                    return false;
                                }
                                catch (Exception ex)
                                {

                                };
                            }
                            catch (Exception ex)
                            {

                            }
                        }



                        if (!string.IsNullOrEmpty(RepinPagesource))
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(Globals.ItemSelect))
                                {
                                    GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " In " + Globals.ItemSelect + " is Done. ]");
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Done. ]");
                                }
                                //string RepinDetails =pinterestAccountManager.Username + "," + PinId.Replace(",","") + "," + boardId.Replace(",","") + "," + myMessage.Replace(",","");

                                //GlobusFileHelper.AppendStringToTextfileNewLine(RepinDetails, ApplicationData.path_Repin);
                            }
                            catch { };

                            return true;
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Failed. ]");
                            return false;
                        }

                        // Log("[ " + DateTime.Now + " ] => [ Repining " + PinId + " For " + pinterestAccountManager.Username + " ]");

                        //string getPinPageSource = pinterestAccountManager.httpHelper.getHtmlfromUrl(new Uri("http://www.pinterest.com/" + PinId + "/"), "", "", "");//(new Uri("http://www.pinterest.com/pin/" + PinId + "/"), "", "", "");
                        //string getPinPageSource = pinterestAccountManager.httpHelper.getHtmlfromUrl(new Uri(PinId), "", "", "");


                    }
                }
                catch { };

                return false;
            }
            catch (Exception Ex)
            {
                return false;
            }
        }
        #endregion

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


    }
}
