using AccountManager;
using BaseLib;
using BasePD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PinsManager
{
    public class AddNewPinManager
    {
        # region GlobalVariable

        public int Nothread_AddNewPin = 5;
        public bool isStopAddNewPin = false;
        public List<Thread> lstThreadsAddNewPin = new List<Thread>();
        public static int countThreadControllerAddNewPin = 0;
        public static int AddNewPindata_count = 0;
        public readonly object AddNewPinObjThread = new object();
        public bool _IsfevoriteAddNewPin = false;

        List<Pins> UserPins = new List<Pins>();
        string Board = string.Empty;
        string ImageUrl = string.Empty;
        string Desc = string.Empty;
        string BoardNumber = string.Empty;
        string BoardId = string.Empty;
        string UserUrl = string.Empty;
        string description = string.Empty;
        string link = string.Empty;
        string boardId = string.Empty;
        string RepinPagesource = string.Empty;
        string PinId = string.Empty;
        string linkurl = string.Empty;

        public int minDelayAddNewPin
        {
            get;
            set;
        }

        public int maxDelayAddNewPin
        {
            get;
            set;
        }

        public int NoOfThreadsAddNewPin
        {
            get;
            set;
        }

        Accounts ObjAccountManager = new Accounts();
        //PinInterestUser objPinUser = new PinInterestUser();
        string Pinpath = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "NewPin");

        #endregion

        public void StartAddNewPin()
        {
            try
            {
                countThreadControllerAddNewPin = 0;
                int numberOfAccountPatchAddNewPin = 25;

                if (NoOfThreadsAddNewPin > 0)
                {
                    numberOfAccountPatchAddNewPin = NoOfThreadsAddNewPin;
                }
                AddNewPindata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchAddNewPin);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {

                            if (countThreadControllerAddNewPin > Nothread_AddNewPin)
                            {
                                try
                                {
                                    lock (AddNewPinObjThread)
                                    {
                                        Monitor.Wait(AddNewPinObjThread);
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
                                Thread profilerThread = new Thread(StartAddNewPinMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAddNewPin++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" Error :" + ex.StackTrace);
            }
        }

        public void StartAddNewPinMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAddNewPin)
                {
                    try
                    {
                        lstThreadsAddNewPin.Add(Thread.CurrentThread);
                        lstThreadsAddNewPin.Distinct().ToList();
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

                            try
                            {
                                checkLogin = ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                                if (!checkLogin)
                                {
                                    try
                                    {
                                        checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                    }
                                    catch { };
                                    if (!checkLogin)
                                    {
                                        GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                        return;
                                    }
                                }

                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                            }

                            catch { };
                        }
                        #endregion

                        StartActionMultithreadAddNewPin(ref objPinUser);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                        GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
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
                    if (countThreadControllerAddNewPin > Nothread_AddNewPin)
                    {
                        lock (AddNewPinObjThread)
                        {
                            Monitor.Pulse(AddNewPinObjThread);
                        }
                        AddNewPindata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerAddNewPin--;
                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
            }

        }

        public void StartActionMultithreadAddNewPin(ref PinInterestUser objPinUser)
        {
            try
            {
                PinInterestUser objPinUse = objPinUser;
                UserPins = ClGlobul.lstListOfPins.FindAll(P => P.Email == objPinUse.Username);

                if (UserPins.Count == 0)
                {
                    UserPins = ClGlobul.lstListOfPins;
                }

                GlobusLogHelper.log.Info(" => [ Total Pins Count is " + UserPins.Count + " ]");

                foreach (Pins pin in UserPins)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(pin.Board))
                        {
                            Board = pin.Board;
                            // BoardNumber = pinterestBoard.GetBoardId(Board, ref accountManager);
                            try
                            {
                                BoardNumber = GetBoardId_Board(Board, ref objPinUser);
                            }
                            catch { }
                            if (!PDGlobals.ValidateNumber(BoardNumber))
                            {
                                Thread.Sleep(1 * 60 * 1000);
                            }
                            if (string.IsNullOrEmpty(BoardNumber))
                            {
                                GlobusLogHelper.log.Info(" => [ " + Board + " Not Found. Creating Board ]");
                                try
                                {
                                    BoardNumber = CreateBoard_NewPin(Board, "Other", ref objPinUser);
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }
                            }
                        }
                         else
                        {
                            if (objPinUser.Boards.Count <= 0)
                            {
                                try
                                {
                                    GetBoards(ref objPinUser);
                                }
                                catch { };
                            }
                            if (objPinUser.Boards.Count > 0)
                            {
                                Random Boardrnd = new Random();
                                int BoardNum = 0;

                                try
                                {
                                    BoardNum = Boardrnd.Next(0, objPinUser.Boards.Count - 1);
                                    BoardNumber = objPinUser.Boards[BoardNum];
                                }
                                catch (Exception ex)
                                {
                                  
                                }
                            }
                        }

                        ImageUrl = pin.ImageUrl;
                        Desc = pin.Description;
                  
                        string Data = "";
                        try
                        {
                            Data = NewPin(BoardNumber, Desc, ImageUrl, ref objPinUser);
                        }
                        catch { };

                        if (Data.Equals("true"))
                        {
                            GlobusLogHelper.log.Info(" => [ Pin Added To " + Board + " From " + objPinUser.Username + " ]");
                            try
                            {
                                string CSV_Header = "Date" + "," + "UserName" + "," + "Board" + "," + "Description" + "," + "ImageUrl";
                                string CSV_Data = System.DateTime.Now.ToString() + "," + objPinUser.Username + "," + Board.Replace(",", " ").Replace("'", "") + "," + Desc.Replace(",", " ").Replace("'", "") + "," + ImageUrl.Replace(",", " ").Replace("'", "");

                                PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, Pinpath + "\\NewPin.csv");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [ Pin Not Added To " + Board + " From " + objPinUser.Username + " ]");
                        }
                 
                   
                    }
                    catch(Exception ex)
                    {

                    }                   
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public string CreateBoard_NewPin(string BoardName, string Category, ref PinInterestUser objPinUser)
        {
            try
            {
                string CreateBoardPostData = "name=" + BoardName + "&secret=false&source_indicator=add_dialog&category=other";
                BoardName = BoardName.Trim();//.Replace(" ", "-");
                string ScreenName = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");
                ScreenName = Utils.Utils.getBetween(ScreenName, "username\": \"", "\"");
                string BoardPageUrl = "http://pinterest.com/" + ScreenName + "/" + BoardName + "/";
         
                string newpostdata = "source_url=%2F" + objPinUser.ScreenName + "%2F&data=%7B%22options%22%3A%7B%22name%22%3A%22" + (BoardName.Replace(" ", "+")) + "%22%2C%22category%22%3A%22other%22%2C%22description%22%3A%22%22%2C%22privacy%22%3A%22public%22%2C%22layout%22%3A%22default%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserProfilePage(resource%3DUserResource(username%3D" + ScreenName + "%2C+invite_code%3Dnull))%3EUserProfileContent(resource%3DUserResource(username%3D" + ScreenName + "%2C+invite_code%3Dnull))%3EUserBoards()%3EGrid(resource%3DProfileBoardsResource(username%3D" + ScreenName + "))%3EGridItems(resource%3DProfileBoardsResource(username%3D" + ScreenName + "))%3EBoardCreateRep(ga_category%3Dboard_create%2C+text%3DCreate+a+board%2C+submodule%3D%5Bobject+Object%5D)%23Modal(module%3DBoardCreate())";

                string CreatedBoardPageSource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/BoardResource/create/"), newpostdata, "https://www.pinterest.com/");


                if (CreatedBoardPageSource.Contains("success"))
                {
                    if (CreateBoardPostData.Contains("You already have a board with that name."))
                    {
                        GlobusLogHelper.log.Info(" => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
                        return null;
                    }

                    GlobusLogHelper.log.Info(" => [ Successfully Created Board " + BoardName + " For " + objPinUser.Username + " ]");
                    int FirstPinPoint = CreatedBoardPageSource.IndexOf("id\"");
                    int SecondPinPoint = CreatedBoardPageSource.IndexOf("}");


                    string PinUrl = CreatedBoardPageSource.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("pin", string.Empty).Replace("/", string.Empty).Replace("id:", string.Empty).Trim();
                    return PinUrl;
                }
                else if (CreatedBoardPageSource.Contains("You have a board with this name."))
                {
                    GlobusLogHelper.log.Info(" => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
                    string BoardId = GetBoardId(BoardName, ref objPinUser);
                    return BoardId;
                }
                else
                {
                    GlobusLogHelper.log.Info(" => [ Board Creation Process Failed " + BoardName + " ]");
                    return CreatedBoardPageSource;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                return null;
            }
        }

        public string GetBoardId_Board(string BoardName, ref PinInterestUser objPinUser)
        {
            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting BoardId For Board " + BoardName + " ]");

                string ScreenName = ObjAccountManager.Getscreen_NameRepin(ref objPinUser);


                if (!BoardName.Contains("http:"))
                {
                    BoardName = BoardName.Replace(" ", "-");
                }
               
                if (!BoardName.Contains("http:"))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(objPinUser.ScreenName))
                        {
                            UserUrl = "http://pinterest.com/" + objPinUser.ScreenName + "/" + BoardName;
                        }
                        else if (!string.IsNullOrEmpty(objPinUser.Name))
                        {
                            UserUrl = "http://pinterest.com/" + objPinUser.Name + "/" + BoardName;
                        }

                        UserUrl = "http://pinterest.com/" + ScreenName + "/" + BoardName;
                    }
                    catch(Exception ex) 
                    { };
                }
                else
                {
                    UserUrl = BoardName;

                }

                string BoardPage = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinUser.UserAgent);

                if (!BoardPage.Contains(BoardName))
                {
                    //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Board is not present in the Account. ]");
                    return null;
                }

                if (BoardPage.Contains(BoardName))
                {
                    try
                    {
                        int FirstPointToken = BoardPage.IndexOf("board_id\":");
                        string FirstTokenSubString = BoardPage.Substring(FirstPointToken);

                        int SecondPointToken = FirstTokenSubString.IndexOf(",");
                        BoardId = FirstTokenSubString.Substring(0, SecondPointToken).Replace("var board = ", string.Empty).Replace("value=", string.Empty).Replace("'", string.Empty).Replace("board_id\": \"", string.Empty).Replace("\"", string.Empty).Trim();
                    }
                    catch { };
                }

                GlobusLogHelper.log.Info(" => [ Board Find " + BoardId + " ]");

                return BoardId;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ Boards Getting Process Failed ]");
                return null;
            }
        }

        public string GetBoardId(string BoardName, ref PinInterestUser objPinUser)
        {
            try
            {          
                GlobusLogHelper.log.Info(" => [ Start Getting BoardId For Board " + BoardName + " ]");
                string Pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("http://pinterest.com"));
                string ScreenName = Utils.Utils.getBetween(Pagesource, "\"options\": {\"username\": \"", "\"");

                if (!BoardName.Contains("http:"))
                {
                    BoardName = BoardName.Replace(" ", "-");
                }
               

                if (!BoardName.Contains("http:"))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(objPinUser.ScreenName))
                        {
                            UserUrl = "http://pinterest.com/" + objPinUser.ScreenName + "/" + BoardName;
                        }
                        else if (!string.IsNullOrEmpty(objPinUser.Name))
                        {
                            UserUrl = "http://pinterest.com/" + objPinUser.Name + "/" + BoardName;
                        }                   
                    }
                    catch { };
                }
                else
                {
                    UserUrl = BoardName;
                }

                string BoardPage = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinUser.UserAgent);

                if (!BoardPage.Contains(BoardName))
                {
                    GlobusLogHelper.log.Info(" => [ Board is not present in the Account. ]");
                    return null;
                }

                if (BoardPage.Contains(BoardName))
                {
                    try
                    {
                        int FirstPointToken = BoardPage.IndexOf("board_id\":");
                        string FirstTokenSubString = BoardPage.Substring(FirstPointToken);

                        int SecondPointToken = FirstTokenSubString.IndexOf(",");
                        BoardId = FirstTokenSubString.Substring(0, SecondPointToken).Replace("var board = ", string.Empty).Replace("value=", string.Empty).Replace("'", string.Empty).Replace("board_id\": \"", string.Empty).Replace("\"", string.Empty).Trim();
                    }
                    catch { };
                }
                GlobusLogHelper.log.Info(" => [ Board Find " + BoardId + " ]");                

                return BoardId;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ Boards Getting Process Failed ]");
                return null;
            }
        }

        public void GetBoards(ref PinInterestUser objPinUser)
        {
            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Boards For User " + objPinUser.Name + " ]");
                List<string> Boards = new List<string>();

                string UserUrl = "http://pinterest.com/" + objPinUser.Name;
                string aa = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", objPinUser.UserAgent);

                string[] Items = Regex.Split(aa, objPinUser.Name);
                Items = Items.Skip(1).ToArray();
                int counter = 0;
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
                                        string ac = string.Empty;
                                        if (LastPoint <= 0)
                                        {
                                            LastPoint = Dataitem.IndexOf(",");
                                            ac = Dataitem.Substring(0, LastPoint).Replace("\": \"", string.Empty).Replace("\"", "").Replace(", ", string.Empty).Replace("field_set_keygrid_item}}name", string.Empty).Trim();
                                        }
                                        else
                                        {
                                            ac = Dataitem.Substring(0, LastPoint).Replace("\": \"", string.Empty).Replace("\"", "").Replace(", ", string.Empty).Replace("field_set_keygrid_item}}name", string.Empty).Trim();
                                        }
                                        if (!Boards.Contains(ac))
                                        {
                                            //I have to validate here so that only BoardId gets through
                                            if (NumberHelper.ValidateNumber(ac))
                                            {
                                                Boards.Add(ac);
                                            }
                                        }
                                    }
                                    catch(Exception ex)
                                    {}                                   
                                }
                            }
                            //}
                            //counter++;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                objPinUser.Boards.AddRange(Boards);
                GlobusLogHelper.log.Info(" => [ Get All Boards for User " + objPinUser.Name + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ Boards Getting Process Failed ]");
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public string NewPin(string Board, string Message, string BlogUrl, ref PinInterestUser objPinUser)
        {
            try
            {                            
                string[] arrPinId = Regex.Split(BlogUrl, "/");
                foreach (var itemPinId in arrPinId)
                {
                    if (PDGlobals.ValidateNumber(itemPinId))
                    {
                        PinId = itemPinId;
                    }
                }

                string getPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(BlogUrl), "", "", "");
              
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
                    if (string.IsNullOrEmpty(Board))
                    {
                        Random rnd = new Random();
                        int BoardNum = rnd.Next(0, PinInterestUser.lstBoardId.Count - 1);
                        boardId = PinInterestUser.lstBoardId[BoardNum];
                    }
                    else
                    {
                        boardId = Board;
                    }

                    GlobusLogHelper.log.Info(" => [ Repining " + boardId + " For " + objPinUser.Username + " ]");
                }
                else
                {
                    boardId = Board;
                }
                string BlogUrl1 = BlogUrl.Replace(":", "%253A").Replace("/", "%252F").Replace("?", "%3F").Replace("=", "%3D").Replace("&", "%26");
                string BlogUrl2 = BlogUrl.Replace(":", "%3A").Replace("/", "%2F").Replace("?", "%3F").Replace("=", "%3D").Replace("&", "%26");

                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }

                if (Checking.Contains("profileName"))
                {            
                    string RepinpostData = "source_url=%2Fpin%2Ffind%2F%3Furl%3D" + "http%253A%252F%252Fwebneel.com%252Fwallpaper%252Fsites%252Fdefault%252Ffiles%252Fimages%252F01-2014%252F35-flower-wallpapers.preview.jpg" + "&data=%7B%22options%22%3A%7B%22method%22%3A%22scraped%22%2C%22description%22%3A%22" + Message.Replace(" ", "+") + "%22%2C%22link%22%3A%22" + BlogUrl2 + "%22%2C%22image_url%22%3A%22" + BlogUrl2 + "%22%2C%22board_id%22%3A%22" + Board + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPinCreate%3EBoardPicker%3ESelectList(view_type%3DpinCreate%2C+selected_section_index%3Dundefined%2C+selected_item_index%3Dundefined%2C+highlight_matched_text%3Dtrue%2C+suppress_hover_events%3Dundefined%2C+scroll_selected_item_into_view%3Dtrue%2C+select_first_item_after_update%3Dfalse%2C+item_module%3D%5Bobject+Object%5D)";                   
                    try
                    {
                        Thread.Sleep(2 * 1000);
                        RepinPagesource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/PinResource/create/"), RepinpostData, "https://www.pinterest.com/");
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(1 * 30 * 1000);
                        try
                        {
                            RepinPagesource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/PinResource/create/"), RepinpostData, "https://www.pinterest.com/");
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(1 * 1000);
                            RepinPagesource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/PinResource/create/"), RepinpostData, "https://www.pinterest.com/");
                        }
                    }

                    if (!string.IsNullOrEmpty(RepinPagesource))
                    {
                        GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Done. ]");
                        return "true";
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " is Failed. ]");
                        return "false";
                    }

                    GlobusLogHelper.log.Info(" => [ Repining " + PinId + " For " + objPinUser.Username + " ]");

                    if (getPinPageSource.Contains("class=\"sourceFlagWrapper"))
                    {
                        BaseLib.GlobusRegex rgx = new GlobusRegex();

                        string urldata = System.Text.RegularExpressions.Regex.Split(System.Text.RegularExpressions.Regex.Split(getPinPageSource, "sourceFlagWrapper")[1], "</a>")[0];

                        linkurl = rgx.GetHrefUrlTag(urldata).Replace("href=\"", string.Empty);
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

                            linkurl = end;
                        }
                        catch { };
                    }

                    try
                    {
                        string postdata1 = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + Board + "%22%2C%22description%22%3A%22" + Message + "%22%2C%22link%22%3A%22" + Uri.EscapeDataString(linkurl) + "%22%2C%22is_video%22%3Afalse%2C%22pin_id%22%3A%22" + PinId + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ECloseup(resource%3DPinResource(id%3D" + PinId + "))%3EPinActionBar(resource%3DPinResource(id%3D" + PinId + "))%3EShowModalButton(module%3DPinCreate)%23Modal(module%3DPinCreate(resource%3DPinResource(id%3D" + PinId + ")))";
                        string afterposting = objPinUser.globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/RepinResource/create/"), postdata1, "http://www.pinterest.com/pin/" + PinId + "/", "", 0, "", "");

                        return "true";
                    }
                    catch (Exception ex)
                    {

                    };
                }
                else
                {
                    GlobusLogHelper.log.Info(" => [ Login Issue " + " For " + objPinUser.Username + " ]");
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            return "false";
        }


    }


}

