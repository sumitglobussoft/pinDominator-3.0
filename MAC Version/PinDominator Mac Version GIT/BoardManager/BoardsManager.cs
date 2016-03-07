using BaseLib;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AccountManager;
using PinDominator;
using PinsManager;

namespace BoardManager
{
    public delegate void AccountReport_Board();
	public delegate void BoardAccount(int AccCount);
    public class BoardsManager
    {
        public static AccountReport_Board objBoardDelegate;
		public static BoardAccount objDelegateNoOfActiveAcc;
		public static BoardAccount objDelegateNoOfDeadAcc;

        #region Global Variable

        public int Nothread_Boards = 5;
        public bool isStopBoards = false;
        public List<Thread> lstThreadsBoards = new List<Thread>();
        public static int countThreadControllerBoards = 0;
        public static int Boardsdata_count = 0;
        public readonly object BoardsObjThread = new object();
        public bool _IsfevoriteBoards = false;
        public int MaxRePinCount = 5;
        public bool rdbSingleUserBoards = false;
        public bool rdbMultipleUserBoards = false;
        public string SingleBoardUrl_Boards = string.Empty;
        public string SingleBoardName_Boards = string.Empty;
        public string SingleMsg_Boards = string.Empty;

        public int minDelayBoards
        {
            get;
            set;
        }

        public int maxDelayBoards
        {
            get;
            set;
        }

        public int NoOfThreadsBoards
        {
            get;
            set;
        }

        int BoardCreationBoardNum = 0;
        string BoardName = string.Empty;
        string BoardUrl = string.Empty;
        List<string> TemplstListOfBoardNames = new List<string>();
        List<Thread> boardinput = new List<Thread>();
        
        string PopularPinPageSource = string.Empty;
        string NextPageUrlPinPageSource = string.Empty;
        string BookMark = string.Empty;
        string BoardIdOfBoardUrl = string.Empty;
        string BoardUrlEdited = string.Empty;
        string NextPageUrl = string.Empty;
        int count = 1;

		int ActiveAccCount = 0;
		int DeadAccCount = 0;
        string Boardpath = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Board");

        #endregion

        Accounts ObjAccountManager = new Accounts();
        QueryManager Qm = new QueryManager();
        public void StartBoards()
        {
            try
            {
                countThreadControllerBoards = 0;
                int numberOfAccountPatchBoards = 25;

                if (numberOfAccountPatchBoards > 0)
                {
                    numberOfAccountPatchBoards = NoOfThreadsBoards;
                }

                Boardsdata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchBoards);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerBoards > Nothread_Boards)
                            {
                                try
                                {
                                    lock (BoardsObjThread)
                                    {
                                        Monitor.Wait(BoardsObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {
									Console.Write(Ex.Message);
                                }
                            }
                            string acc = account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {
                                Thread profilerThread = new Thread(StartBoardsMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerBoards++;
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

        public void StartBoardsMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopBoards)
                {
                    try
                    {
                        lstThreadsBoards.Add(Thread.CurrentThread);
                        lstThreadsBoards.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
						Console.Write(ex.Message);
					}

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;

                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);                     

                        #region Login

                        if (!objPinUser.isloggedin)
                        {
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logging In With : " + objPinUser.Username + " ]");
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

									GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
									DeadAccCount++;
									objDelegateNoOfDeadAcc(DeadAccCount);
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));                            
                            }

                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                            }
                        }
                        if (objPinUser.isloggedin == true)
                        {
                            try
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logged In With : " + objPinUser.Username + " ]");
								ActiveAccCount++;
								objDelegateNoOfActiveAcc(ActiveAccCount);
                                StartActionMultithreadBoards(ref objPinUser);
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartActionMultithreadBoards(ref PinInterestUser objPinInUser)
        {
            PinInterestUser objPinUser = objPinInUser;
            try
            {
                try
                {
                    lstThreadsBoards.Add(Thread.CurrentThread);
                    lstThreadsBoards.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { 
					Console.Write(ex.Message);
				}
                //TemplstListOfBoardNames.AddRange(ClGlobul.lstListOfBoardNames);

                foreach (var item_TemplstListOfBoardNames in ClGlobul.lstListOfBoardNames)
                {
                    try
                    {
                        lock (this)
                        {
                            string[] url = Regex.Split(item_TemplstListOfBoardNames, "::");
                            if (url[0] == objPinUser.Niches)
                            {
                                string boardurl = url[2].ToString();
                                string Boardname = string.Empty;
                                string niche = string.Empty;
                                niche = url[0];
                                Boardname = url[1];

                                Thread thread = new Thread(() => BoardMethod(Boardname, boardurl, ref objPinUser));
                                thread.Start();
                                Thread.Sleep(10 * 1000);
                            }
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
                    if (countThreadControllerBoards > Nothread_Boards)
                    {
                        lock (BoardsObjThread)
                        {
                            Monitor.Pulse(BoardsObjThread);
                        }
                    }
                    Boardsdata_count--;
                }
                catch (Exception Ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
                }
                countThreadControllerBoards--;              
            }
        }

        public void BoardMethod(string BoardName, string BoardUrl, ref PinInterestUser objPinInUser)
        {
            try
            {
                try
                {
                    lstThreadsBoards.Add(Thread.CurrentThread);
                    lstThreadsBoards.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };
                lock (this)
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

                    if (string.IsNullOrEmpty(BoardName))
                    {
                        try
                        {
                            List<string> baordnames = GetAllBoardNames_new(ref objPinInUser);
                            BoardName = baordnames[RandomNumberGenerator.GenerateRandom(0, baordnames.Count - 1)];
                            BoardId = objaddnewPin.GetBoardId(BoardName, ref objPinInUser);
                        }
                        catch (Exception ex)
                        { };
                    }
                    else
                    {
                        //testing

                        GlobusHttpHelper objHttp = new GlobusHttpHelper();

                        try
                        {

                            BoardId = objaddnewPin.GetBoardId_Board(BoardName, ref objPinInUser);
                        }
                        catch (Exception ex)
                        {
							Console.Write(ex.Message);
                        }
                    }

                    if (string.IsNullOrEmpty(BoardId))
                    {
                        BoardId = objaddnewPin.GetBoardId(BoardName, ref objPinInUser);
                    }

                    int counter = 0;
                    int RepinCounter = 0;
                    if (!BoardId.Contains("failure"))
                    {
                        if (!string.IsNullOrEmpty(BoardId))
                        {
                            if (PDGlobals.ValidateNumber(BoardId))
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Adding Pins From Board :" + BoardUrl + " ]");

                                clsSettingDB db = new clsSettingDB();
                                List<string> lstPins = new List<string>();
                                lstPins = GetBoardPinsNew(BoardId, BoardUrl, 10, ref objPinInUser);
                                lstPins.Distinct();
                                List<string> lstOfRepin = new List<string>();
                                lstOfRepin.AddRange(lstPins);

                                string[] lstPinNo = null;
                                lstPinNo = lstPins.ToArray();
                                //getting lstPins length
                                int lenOFlstPins = lstOfRepin.Count;

                                foreach (string Pins in lstPinNo)
                                {

                                    if (MaxRePinCount > RepinCounter)
                                    {
                                        string Message = string.Empty;
                                        if (ClGlobul.lstBoardRepinMessage.Count > 0)
                                        {
                                            if (counter < ClGlobul.lstBoardRepinMessage.Count)
                                            {
                                                Message = ClGlobul.lstBoardRepinMessage[counter];
                                            }
                                            else
                                            {
                                                counter = 0;
                                                Message = ClGlobul.lstBoardRepinMessage[counter];
                                            }

                                        }

                                        bool IsReppined = false;
                                        if (!Pins.Contains("n"))
                                        {
                                            try
                                            {
                                                int index = lstOfRepin.Where(x => x == Pins).Select(x => lstOfRepin.IndexOf(x)).Single<int>();

                                                string NoOfPages = Convert.ToString(index / lenOFlstPins);

                                                IsReppined = objRepin.RepinwithMessage(Pins, Message, BoardId, NoOfPages, ref objPinInUser);
                                            }

                                            catch { }
                                            if (IsReppined)
                                            {
                                                //GlobusLogHelper.log.Info("[ => [ Repin Id : " + Pins + " ]");
                                                #region AccountReport

                                                string module = "Boards";
                                                string status = "Repined";
                                                Qm.insertAccRePort(objPinInUser.Username, module, "https://www.pinterest.com/pin/" + Pins, BoardName, "", "", "", "", status, "", "", DateTime.Now);
                                                objBoardDelegate();

                                                #endregion

												GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Repin Pin : " + Pins + " to Account : " + objPinInUser.Username + " In " + BoardName + " ]");
                                                try
                                                {
                                                    string CSV_Header = "Date" + "," + "UserName" + "," + "Message" + "," + "Pin" + "Board Id";
                                                    string CSV_Data = System.DateTime.Now.ToString() + "," + objPinInUser.Username + "," + Message + "," + "https://www.pinterest.com/pin/" + Pins + "," + BoardId;

                                                    PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, Boardpath + "\\Board.csv");
                                                    RepinCounter++;
                                                }
                                                catch (Exception ex)
                                                {
													Console.Write(ex.Message);
                                                }

                                                //kept here
                                                int delay = RandomNumberGenerator.GenerateRandom(minDelayBoards, maxDelayBoards);
												GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay for " + delay + " Seconds ]");
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
										GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED " + " For " + objPinInUser.Username + " In " + BoardName + "]");
                                        GlobusLogHelper.log.Info("-----------------------------------------------------------------------------");
                                        break;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ]  => [ You already have a board with that name. " + objPinInUser.Username + " ]");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
          
        }

        public List<string> GetAllBoardNames_new(ref PinInterestUser objPinInUser)
        {
            List<string> BoardNames = new List<string>();
            BaseLib.GlobusRegex rgx = new GlobusRegex();
            Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
          
            string UserUrl = "http://pinterest.com/" + objPinInUser.Username;
            string BoardPage = httpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinInUser.UserAgent);

            string[] Items = Regex.Split(BoardPage, "item");

            int counter = 0;
            foreach (string item in Items)
            {
                try
                {
                    if (item.Contains("id=\\\"Board") && item.Contains("boardLinkWrapper"))
                    {
                        //if (counter == 1)
                        {
                            string[] Data = System.Text.RegularExpressions.Regex.Split(item, "boardLinkWrapper");

                            foreach (string Dataitem in Data)
                            {
                                if (Dataitem.Contains("-end-"))
                                {
                                    continue;
                                }
                                if (Dataitem.Contains("boardName"))
                                {                                
                                    int startIndex = Dataitem.IndexOf("title");
                                    int LastPoint = Dataitem.IndexOf("<h4");
                                    string Board = Dataitem.Substring(startIndex, LastPoint).Replace("\\n", string.Empty).Replace("\"", "").Replace("<div class=\\b", string.Empty).Replace("  ", string.Empty).Replace("\"title\"", "").Replace("</div", "");
                                    Board = rgx.StripTagsRegex(Board);
                                    try
                                    {
                                        Board = Board.Split('>')[1];
                                    }
                                    catch { }
                                    if (!BoardNames.Contains(Board))
                                    {
                                        BoardNames.Add(Board);
                                    }
                                }
                            }
                        }
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
            }

            return BoardNames;
        }

        public List<string> GetBoardPinsNew(string BoardId, string BoardUrl, int PageCount, ref PinInterestUser objPinInUser)
        {
            List<string> lstPopularPins = new List<string>();
            try
            {
                //Log("[ " + DateTime.Now + " ] => [ Start Getting Popular Pins For this User " + objPinInUser.Username + " ]");
                string LikeUrl = BoardUrl;
                try
                {
                    PopularPinPageSource = objPinInUser.globusHttpHelper.getHtmlfromUrl(new Uri(LikeUrl), "http://pinterest.com/", string.Empty, objPinInUser.UserAgent);
                }
                catch (Exception ex)
                {
                }

                if (PopularPinPageSource.Contains("BoardResource\", \"options\": {\"board_id\":"))
                {
                    BoardIdOfBoardUrl = Utils.Utils.getBetween(PopularPinPageSource, "BoardResource\", \"options\": {\"board_id\":", ",").Replace("\"", "").Trim();
                }

                BoardUrlEdited = BoardUrl.Replace("https://www.pinterest.com", "").Replace("/", "%2F");

                while (!string.IsNullOrEmpty(PopularPinPageSource))
                {
                    if (!string.IsNullOrEmpty(PopularPinPageSource))
                    {
                        if (PopularPinPageSource.Contains("board_layout"))
                        {
                            BookMark = Utils.Utils.getBetween(PopularPinPageSource, "board_layout\": \"default\", \"bookmarks\": [", "]").Replace("\"", "").Replace("=", "");
                        }

                        if (PopularPinPageSource.Contains("pinHolder"))
                        {
                            string[] arrPin = Regex.Split(PopularPinPageSource, "pinHolder");
                            foreach (var itemArrPin in arrPin)
                            {
                                string Pin = Utils.Utils.getBetween(itemArrPin, "/pin/", "class=").Replace("\\", "").Replace("/", "").Replace("\"", "").Trim();
                                if (!string.IsNullOrEmpty(Pin) && !lstPopularPins.Contains(Pin))
                                {
                                    lstPopularPins.Add(Pin);
                                }
                            }
                        }

                        if (PopularPinPageSource.Contains("\"id\":"))
                        {
                            string[] arrPin = Regex.Split(PopularPinPageSource, "uri");
                            foreach (var itemArrPin in arrPin)
                            {
                                if (itemArrPin.Contains("/v3/pins/"))
                                {
                                    string Pin = Utils.Utils.getBetween(itemArrPin, "/v3/pins/", "/comments/").Replace("\\", "").Replace("/", "").Replace("\"", "").Trim();

                                    if (!string.IsNullOrEmpty(Pin) && !lstPopularPins.Contains(Pin))
                                    {
                                        lstPopularPins.Add(Pin);
                                    }
                                }
                            }
                        }
                    }

                    NextPageUrl = "https://www.pinterest.com/resource/BoardFeedResource/get/?source_url=" + BoardIdOfBoardUrl + "&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardIdOfBoardUrl + "%22%2C%22board_url%22%3A%22" + BoardUrlEdited + "%22%2C%22page_size%22%3Anull%2C%22prepend%22%3Atrue%2C%22access%22%3A%5B%5D%2C%22board_layout%22%3A%22default%22%2C%22bookmarks%22%3A%5B%22" + BookMark + "%3D%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&_=142710648289" + count + "";
                    try
                    {
                        PopularPinPageSource = objPinInUser.globusHttpHelper.getHtmlfromUrl(new Uri(NextPageUrl), "https://www.pinterest.com/", string.Empty, objPinInUser.UserAgent);
                    }
                    catch (Exception ex)
                    {
                        PopularPinPageSource = null;
                    }
                    count++;
                }

                lstPopularPins = lstPopularPins.Distinct().ToList();
                //lstPopularPins.Reverse();

				GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Pin Urls Collected " + lstPopularPins.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstPopularPins;
        }


    }
}
