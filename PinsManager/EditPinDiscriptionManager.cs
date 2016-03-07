using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PinsManager
{
    public delegate void AccountReport_EditPinDescription();
    public class EditPinDiscriptionManager
    {
        public static AccountReport_EditPinDescription objEditPinDescriptionDelegate;

        #region Global variable

        public int Nothread_EditPinDisc = 5;
        public bool isStopEditPinDisc = false;
        public List<Thread> lstThreadsEditPinDisc = new List<Thread>();
        public static int countThreadControllerEditPinDisc = 0;
        public static int EditPinDiscdata_count = 0;
        public readonly object EditPinDiscObjThread = new object();
        public int NoOfPagesEditPinDisc = 1;
        public bool _IsfevoriteEditPinDes = false;
        public bool rdbSingleUserEditPinDisc = false;
        public bool rdbMultipleUserEditPinDisc = false;
        public string SingleDesc_EditPinDisc = string.Empty;
 
        string PinCommnet = string.Empty;

        List<string> CommentList = new List<string>();
        List<string> lstBoardUrlsPin = new List<string>();
        List<string> lstEditPins = new List<string>();
        string UserPageSource = string.Empty;

        public int minDelayEditPinDisc
        {
            get;
            set;
        }

        public int maxDelayEditPinDisc
        {
            get;
            set;
        }

        public int NoOfThreadsEditPinDisc
        {
            get;
            set;
        }

        #endregion

        Accounts ObjAccountManager = new Accounts();
        QueryManager objQm = new QueryManager();

        public void StartEditPinDisc()
        {
            try
            {
                countThreadControllerEditPinDisc = 0;
                int numberOfAccountPatchEditPinDisc = 25;

                if (NoOfThreadsEditPinDisc > 0)
                {
                    numberOfAccountPatchEditPinDisc = NoOfThreadsEditPinDisc;
                }

                EditPinDiscdata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchEditPinDisc);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {

                            if (countThreadControllerEditPinDisc > Nothread_EditPinDisc)
                            {
                                try
                                {
                                    lock (EditPinDiscObjThread)
                                    {
                                        Monitor.Wait(EditPinDiscObjThread);
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
                                Thread profilerThread = new Thread(StartEditPinDiscMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerEditPinDisc++;
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

        public void StartEditPinDiscMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {

                if (!isStopEditPinDisc)
                {
                    try
                    {
                        lstThreadsEditPinDisc.Add(Thread.CurrentThread);
                        lstThreadsEditPinDisc.Distinct().ToList();
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
                                //checkLogin = ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                if (!checkLogin)
                                {
                                    GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadEditPinDisc(ref objPinUser);
                            }

                            catch { };
                        }
                        else if(objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadEditPinDisc(ref objPinUser);
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

            finally
            {
                try
                {
                    if (countThreadControllerEditPinDisc > Nothread_EditPinDisc)
                    {
                        lock (EditPinDiscObjThread)
                        {
                            Monitor.Pulse(EditPinDiscObjThread);
                        }
                        EditPinDiscdata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerEditPinDisc--;
                //GlobusLogHelper.log.Info(" [ PROCESS COMPLETED ] " );
                //GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------"); 
            }


        }

        public void StartActionMultithreadEditPinDisc(ref PinInterestUser objPinUser)
        {
            try
            {
                foreach (string comment in ClGlobul.CommentNicheMessageList)
                {
                    string[] array = Regex.Split(comment, "::");
                    if (array.Length == 2)
                    {
                        if (array[0] == objPinUser.Niches)
                        {
                            CommentList.Add(array[1]);
                            GlobusLogHelper.log.Info(" => [ Editing Pin Description For " + objPinUser.Username + " ]");
                            EditPins(ref objPinUser);
                        }
                    }
                }

                //EditPin.PinEditLogEvent.addToLogger += new EventHandler(PinEditLogEvent_addToLogger);               

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void EditPins(ref PinInterestUser objPinUser)
        {
            try
            {
                //objPinUser.globusHttpHelper = new GlobusHttpHelper();
                List<string> lstOfBoardURLs = GetAllBoardNames_new1(objPinUser.ScreenName, ref objPinUser);
                foreach (var itemBoardUrl in ClGlobul.lstBoardUrls)
                {
                    string url = "https://www.pinterest.com" + itemBoardUrl;
                    lstBoardUrlsPin.Add(url);
                }

                List<string> lstPinsfromBoards = new List<string>();

                foreach (var itemBoardUrl in lstBoardUrlsPin)
                {
                    string pageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(itemBoardUrl), "", "", objPinUser.UserAgent);

                    if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("/pin/"))
                    {
                        string data = Utils.Utils.getBetween(pageSource, "/pin/", "class=").Replace("\\", "").Replace("\"", "").Replace("/", "");

                        if (!string.IsNullOrEmpty(data) && !lstPinsfromBoards.Contains(data))
                        {
                            lstPinsfromBoards.Add(data);
                        }
                    }
                }
   
                GlobusLogHelper.log.Info(" => [ Extarcting User Pins For Edit ]");
                List<string> lstAllPins = GetAllPinsNewCode(ref objPinUser);
                Random Pinrnd = new Random();
                List<string> lstPins = new List<string>();
                try
                {
                    lstPins = lstAllPins.OrderBy(X => Pinrnd.Next()).ToList();
                }
                catch { };
                lstPins.Reverse();
                GlobusLogHelper.log.Info(" => [ " + lstPins.Count + " Pins From " + objPinUser.Username + " ]");

                foreach (string Pin in lstPins)
                {
                    clsSettingDB DB = new clsSettingDB();
                    DataTable dt = DB.SelectPinDesc(Pin, objPinUser.Username);
                    if (dt.Rows.Count <= 0)
                    {
                        string pageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("http://pinterest.com/pin/" + Pin + "/edit/"), "", "", objPinUser.UserAgent);
                        string PinDesc = string.Empty;
                        string csrfmiddlewaretoken = string.Empty;
                        string link = string.Empty;
                        string board = string.Empty;

                        try
                        {
                            int startindex = pageSource.IndexOf("description_html");
                            string start = pageSource.Substring(startindex).Replace("description_html", "");
                            int endIndex = start.IndexOf(",");
                            //string end = start.Substring(0, endIndex).Replace("\"", "").Replace(":", "").Replace("","+");
                            string end = Utils.Utils.getBetween(start, "\": \"", "</a>");
                            PinDesc = end;
                        }
                        catch (Exception ex)
                        {

                        }

                        if (string.IsNullOrEmpty(PinDesc))
                        {
                            GlobusLogHelper.log.Info(" => [ No Desc In Pin :" + Pin + " ]");
                        }
                        else
                        {

                            PinCommnet = CommentList[RandomNumberGenerator.GenerateRandom(0, CommentList.Count)];

                            PinDesc = PinCommnet;
                            try
                            {
                                int startindex = pageSource.IndexOf("\"board_id\":");
                                string start = pageSource.Substring(startindex).Replace("\"board_id\":", "").Replace("boardRepSubtitle", "");
                                int endIndex = start.IndexOf(",");
                                string end = start.Substring(0, endIndex);
                                board = end.Replace("\"", "").Replace(">", "");
                                board = board.Trim();
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                int startindex = pageSource.IndexOf("link\": \"http:");
                                string start = pageSource.Substring(startindex).Replace("link\":", "");
                                int endIndex = start.IndexOf(",");
                                string end = start.Substring(0, endIndex).Replace("value=", "").Replace("\"", "");
                                string Link = Utils.Utils.getBetween(start, "Link=", "");
                                link = end.Trim().Replace(":", "%3A").Replace("/", "%2F").Replace("?", "%3F").Replace("=", "%3D");
                            }
                            catch (Exception ex)
                            {

                            }
                            Thread.Sleep(1 * 1000);
                            try
                            {
                                string postdata = "board_id=" + board + "&description=" + PinDesc + "&link=" + link + "&id=" + Pin;

                                #region PostData

                                string pinUrl = "https://www.pinterest.com/pin/" + Pin + "/";
                                string BoardId = string.Empty;

                                string getPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(pinUrl), "", "", "");

                                if (getPinPageSource.Contains("\"price_currency\""))
                                {                                 
                                    link = Utils.Utils.getBetween(getPinPageSource, "serving_link\":", ", \"is_promoted").Replace("\"", "").Trim();
                                    link = link.Replace(":", "%3A").Replace("/", "%2F").Replace("?", "%3F").Replace("=", "%3D").Replace("&", "%26");
                                }
                                if (getPinPageSource.Contains("board")) //("board_id")
                                {
                                   // BoardId = Utils.Utils.getBetween(getPinPageSource, "board\", \"id\":", ",").Replace("\"", "").Trim();
                                    BoardId = Utils.Utils.getBetween(getPinPageSource, "board\", \"id\":", "\",").Replace("\"", "").Trim();//"board", "id":
                                }
                                #endregion

                                string postPageSOurce = string.Empty;

                                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                if (Checking.Contains("profileName"))
                                {
                                }
                                else
                                {
                                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                                }
                                string redirectDomain = GlobusHttpHelper.valueURl.Split('.')[0];
                                string newHomePageUrl = redirectDomain + "." + "pinterest.com";
                                //postdata = "source_url=%2Fpin%2F" + Pin + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22description%22%3A%22" + PinDesc + "%22%2C%22link%22%3A%22" + link + "%22%2C%22place%22%3A0%2C%22id%22%3A%22" + Pin + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ECloseup%3EPinActionBar%3EShowModalButton(module%3DPinEdit)%23App%3EModalManager%3EModal()";
                                //postdata = "source_url=%2Fpin%2F" + Pin + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22description%22%3A%22" + PinDesc + "%22%2C%22link%22%3A%22" + link + "%2F%22%2C%22place%22%3A0%2C%22id%22%3A%22" + Pin + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ECloseup%3EPinActionBar%3EShowModalButton(module%3DPinEdit)%23App%3EModalManager%3EModal(showCloseModal%3Dtrue%2C+mouseDownInModal%3Dfalse)";
                                postdata = "source_url=%2Fpin%2F" + Pin + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22description%22%3A%22" + PinDesc + "%22%2C%22link%22%3A%22" + link + "%22%2C%22place%22%3A0%2C%22id%22%3A%22" + Pin + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ECloseup%3EPinActionBar%3EShowModalButton(module%3DPinEdit)%23App%3EModalManager%3EModal(state_isVisible%3Dtrue%2C+showCloseModal%3Dtrue%2C+state_mouseDownInModal%3Dfalse%2C+state_showModalMask%3Dtrue%2C+state_showContainer%3Dfalse%2C+state_showPositionElement%3Dtrue)";
                                string postUrl = redirectDomain + ".pinterest.com/resource/PinResource/update/";
                                
                                try
                                {
                                    postPageSOurce = objPinUser.globusHttpHelper.postFormDataProxywithCSRFToken(new Uri(postUrl), postdata, newHomePageUrl, "", 0, "", "");
                                }
                                catch
                                {
                                    Thread.Sleep(1 * 1000);
                                    postPageSOurce = objPinUser.globusHttpHelper.postFormDataProxywithCSRFToken(new Uri(postUrl), postdata, newHomePageUrl, "", 0, "", "");//"Go0h31yGfnvXLZCw0B06nbmbxnqLj5Wj");
                                }
                                if (postPageSOurce.Contains(PinDesc)) //PinDesc
                                {
                                    try
                                    {
                                        GlobusFileHelper.AppendStringToTextfileNewLine(Pin + ": Desc -> " + PinDesc, PDGlobals.path_PinDescription);
                                        DB.InsertPinDesc(Pin, PinDesc, objPinUser.Username);
                                       // GlobusLogHelper.log.Info(" => [ Description Changed To ]");

                                        #region AccountReport

                                        string module = "EditPinDiscription";
                                        string status = "Edited";
                                        objQm.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + Pin, "", "", PinDesc, "", "", status, "", "", DateTime.Now);
                                        objEditPinDescriptionDelegate();

                                        #endregion

                                        GlobusLogHelper.log.Info(" => [ " + Pin + " >>> " + PinDesc + " for " + objPinUser.Username + " ]");

                                        try
                                        {
                                            string CSV_Header = "Date" + "," + "UserName" + "," + "Pin Description" + "," + "Pin";
                                            string CSV_Data = System.DateTime.Now.ToString() + "," + objPinUser.Username + "," + PinDesc.Replace(",", "") + "," + Pin;
                                            string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "EditPin");
                                            PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\EditPinDescription.csv");
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    catch(Exception ex)
                                    { }
                                }
                                else
                                {
                                    GlobusFileHelper.AppendStringToTextfileNewLine(Pin + ": Not Edited Description", PDGlobals.path_PinDescription);
                                    GlobusLogHelper.log.Info(" => [ Description Not Edited " + Pin + ">>>>" + objPinUser.Username + " ]");
                                }
                            }

                            catch(Exception ex)
                            { };
                        }

                        int Delay = RandomNumberGenerator.GenerateRandom(minDelayEditPinDisc, maxDelayEditPinDisc);
                        GlobusLogHelper.log.Info((" => [ Delay For " + Delay + " ]"));
                        Thread.Sleep(Delay * 1000);
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Already Edited " + Pin + " From " + objPinUser.Username + " ]");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
                
        }

        public List<string> GetAllBoardNames_new1(string screenName, ref PinInterestUser objPinUser)
        {
            try
            {
                BaseLib.GlobusRegex rgx = new GlobusRegex();
                GlobusLogHelper.log.Info(" => [ Getting All Board Names ]");
                Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
                string UserUrl = "http://pinterest.com/" + screenName;
                string BoardPage = httpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", "");

                string[] data = Regex.Split(BoardPage, "is_collaborative");

                foreach (var itemdata in data)
                {
                    string boardUrl = Utils.Utils.getBetween(itemdata, ", \"url\":", ",").Replace("\"", "").Trim();

                    if (!ClGlobul.lstBoardUrls.Contains(boardUrl) && !string.IsNullOrEmpty(boardUrl))
                    {
                        ClGlobul.lstBoardUrls.Add(boardUrl);
                    }

                    if (itemdata.Contains("board_id"))
                    {
                        string boardId = Utils.Utils.getBetween(itemdata, "board_id\":", ",").Replace("\"", "").Trim();
                        if (!ClGlobul.lstBoardId.Contains(boardId))
                        {
                            ClGlobul.lstBoardId.Add(boardId);
                        }
                    }
                }

                string[] Items = Regex.Split(BoardPage, "item");

                int counter = 0;
                foreach (string item in Items)
                {
                    try
                    {
                        if (item.Contains("id=\\\"Board") && item.Contains("boardLinkWrapper"))
                        {
                            string[] Data = System.Text.RegularExpressions.Regex.Split(item, "boardLinkWrapper");

                            foreach (string Dataitem in Data)
                            {
                                if (Dataitem.Contains("boardName"))
                                {
                                    string BoardUrl = string.Empty;
                                    int startIndex = Dataitem.IndexOf("title");
                                    int LastPoint = Dataitem.IndexOf("<h2");
                                    string Board = Dataitem.Substring(startIndex, LastPoint).Replace("\\n", string.Empty).Replace("\"", "").Replace("<div class=\\b", string.Empty).Replace("  ", string.Empty).Replace("\"title\"", "").Replace("</div", "");
                                    BoardUrl = rgx.StripTagsRegex(Board);
                                    try
                                    {
                                        Board = Utils.Utils.getBetween(BoardUrl, ">>", "<");
                                        //modified done
                                        if (Board == "")
                                        {
                                            Board = Utils.Utils.getBetween(BoardUrl, "title=", ">").Replace("\\", "").Trim();
                                        }
                                    }
                                    catch { }
                                    if (!ClGlobul.BoardNames.Contains(Board))
                                    {
                                        ClGlobul.BoardNames.Add(Board);

                                    }
                                }
                            }

                            counter++;

                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return ClGlobul.BoardNames;
        }

        public List<string> GetAllPinsNewCode(ref PinInterestUser objPinUser)
        {          
            try
            {
                foreach (var itemBoardUrls in lstBoardUrlsPin)
                {
                    try
                    {
                        UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(itemBoardUrls), "http://pinterest.com/", string.Empty, objPinUser.UserAgent);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (!string.IsNullOrEmpty(UserPageSource))
                    {
                        if (UserPageSource.Contains("<a href="))
                        {
                            string[] arrPin = Regex.Split(UserPageSource, "/pin/");

                            foreach (var itemPin in arrPin)
                            {
                                if (itemPin.Contains("pinImageWrapper"))
                                {
                                    string a = Utils.Utils.getBetween(itemPin, "", "class=").Replace("\\", "").Replace("\"", "").Replace("/", "").Trim();
                                    if (!lstEditPins.Contains(a) && !string.IsNullOrEmpty(a))
                                    {
                                        lstEditPins.Add(a);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstEditPins;
        }


    }


}
