using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoardManager
{
    public class AddUsersToBoardManager
    {
        #region Variable

        public int Nothread_AddUserToBoard = 5;
        public bool isStopAddUserToBoard = false;
        public List<Thread> lstThreadsAddUserToBoard = new List<Thread>();
        public static int countThreadControllerAddUserToBoard = 0;
        public static int AddUserToBoarddata_count = 0;
        public readonly object AddUserToBoardObjThread = new object();
        public bool _IsfevoriteAddUserToBoard = false;

        public int minDelayAddUserToBoard
        {
            get;
            set;
        }

        public int maxDelayAddUserToBoard
        {
            get;
            set;
        }

        public int NoOfThreadsAddUserToBoard
        {
            get;
            set;
        }

        public string BoardName = string.Empty;
        string CategoryName = string.Empty;      
        string Email = string.Empty;
        int PerUserBoard = 10;
        int counter = 0;

        Accounts ObjAccountManager = new Accounts();

        #endregion

        public void StartAddUsersToBoard()
        {
            try
            {
                countThreadControllerAddUserToBoard = 0;
                int numberOfAccountPatchAddUserToBoard = 25;

                if (numberOfAccountPatchAddUserToBoard > 0)
                {
                    numberOfAccountPatchAddUserToBoard = NoOfThreadsAddUserToBoard;
                }

                AddUserToBoarddata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchAddUserToBoard);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerAddUserToBoard > Nothread_AddUserToBoard)
                            {
                                try
                                {
                                    lock (AddUserToBoardObjThread)
                                    {
                                        Monitor.Wait(AddUserToBoardObjThread);
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
                                Thread profilerThread = new Thread(StartAddUserToBoardMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAddUserToBoard++;
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

        public void StartAddUserToBoardMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAddUserToBoard)
                {
                    try
                    {
                        lstThreadsAddUserToBoard.Add(Thread.CurrentThread);
                        lstThreadsAddUserToBoard.Distinct().ToList();
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
                                    catch (Exception ex)
                                    { };
                                    if (!checkLogin)
                                    {
                                        GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                        return;
                                    }
                                }

                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                            }

                            catch (Exception ex)
                            { };
                        }
                        #endregion

                        StartActionMultithreadAddUserToBoard(ref objPinUser);
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
        }

        public void StartActionMultithreadAddUserToBoard(ref PinInterestUser objPinUser)
        {
            try
            {
                string UserName = string.Empty;
                foreach (string item_lstAddToBoardUserNames in ClGlobul.lstAddToBoardUserNames)
                {
                    try
                    {
                        UserName = item_lstAddToBoardUserNames;
                        string IsFollowed = Follow(ref objPinUser, UserName);
                        
                        bool IsAdded = AddUserToBoard(BoardName, UserName, CategoryName, ref objPinUser);
                        if (IsAdded == true)
                        {
                            counter++;
                        }
                        Thread.Sleep(1 * 1000);
                        if (counter == PerUserBoard)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info("Add To Board Failed For This UserName " + UserName);
                    }

                    int DelayTime = RandomNumberGenerator.GenerateRandom(minDelayAddUserToBoard, maxDelayAddUserToBoard);
                    GlobusLogHelper.log.Info(" => [ Delay For " + DelayTime + " Seconds ]");
                    Thread.Sleep(DelayTime);
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
                    if (countThreadControllerAddUserToBoard > Nothread_AddUserToBoard)
                    {
                        lock (AddUserToBoardObjThread)
                        {
                            Monitor.Pulse(AddUserToBoardObjThread);
                        }
                        AddUserToBoarddata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerAddUserToBoard--;
                GlobusLogHelper.log.Info(" => [ ADDING USER TO BOARD PROCESS COMPLETED  ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
            }
        }

        public string Follow(ref PinInterestUser objPinUser, string UserUrl)
        {
            try
            {

                if (!UserUrl.Contains("pinterest.com"))
                {
                    UserUrl = "http://pinterest.com/" + UserUrl + "/";
                }
                UserUrl = UserUrl + "follow/";
                string UserName = UserUrl.Replace("http://pinterest.com/", string.Empty).Replace("follow", string.Empty).Replace("/", string.Empty);
                string Referer = "http://pinterest.com/search/people/?q=" + UserName;
                string FollowPageSource = string.Empty;
                try
                {

                    FollowPageSource = objPinUser.globusHttpHelper.postFormData(new Uri(UserUrl), "", Referer, objPinUser.Token, objPinUser.UserAgent);

                    if (FollowPageSource.Contains(">Unfollow</span>"))
                    {
                        GlobusLogHelper.log.Info(" => [ Successfully Follow this User " + UserName + " ]");
                        return "Followed";
                    }
                    else if (FollowPageSource.Contains("exceeded the maximum"))
                    {
                        GlobusLogHelper.log.Info(" => [ You have exceeded the maximum rate of users followed " + UserName + " ]");
                        return "NotFollowed";
                    }
                    else
                    {
                        //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Follow Process Failed this User " + UserName + " ]");
                        return "NotFollowed";
                    }
                }
                catch (Exception ex)
                { };
                
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ UnFollow Process Failed For this User " + objPinUser.Username + " ]");
                return "NotFollowed";
            }
            return "Followed";
        }

        public bool AddUserToBoard(string BoardName, string UserName, string Category, ref PinInterestUser pinterestAccountManager)
        {
            try
            {
                GlobusLogHelper.log.Info(" => [ Start Adding " + UserName + " to " + BoardName + " ]");

                string screen_Name = ObjAccountManager.Getscreen_Name(ref pinterestAccountManager);

                pinterestAccountManager.globusHttpHelper = new GlobusHttpHelper();
                BoardName = BoardName.Replace(" ", "-");
                string AfterInvitePageSourceData = string.Empty;
                string userid = string.Empty;

                if (BoardName.Contains("pinterest.com"))
                {
                    BoardName = BoardName.Replace("https://pinterest.com/", string.Empty).Replace(pinterestAccountManager.Name, string.Empty).Trim('/');
                }

                string BoardUrl = "https://pinterest.com/" + pinterestAccountManager.ScreenName + "/" + BoardName + "/";
                string SettingsUrl = BoardUrl + "settings/";
                string Collabrator = BoardUrl;
                string InvitePostData1 = "collaborator_name=&collaborator_username=" + UserName;
                string InternalBoardName = BoardName.Replace("-", "+");


                string invited_userid = string.Empty;
                string invited = "https://pinterest.com/" + UserName.Replace(" ", "").Replace("%20", "");
                string pagesourceinvited = pinterestAccountManager.globusHttpHelper.getHtmlfromUrl(new Uri(invited));
                try
                {
                    invited_userid = Utils.Utils.getBetween(pagesourceinvited, "options\": {\"user_id\"", "}");
                    invited_userid = Utils.Utils.getBetween(invited_userid, "\"", "\"");
                }
                catch { };


                string MainPageSource = pinterestAccountManager.globusHttpHelper.getHtmlfromUrl(new Uri(BoardUrl), "https://pinterest.com/", string.Empty, pinterestAccountManager.UserAgent);

                try
                {
                    int startindex = MainPageSource.IndexOf("\"user_id\":");
                    string start = MainPageSource.Substring(startindex).Replace("\"user_id\":", "");
                    int endindex = start.IndexOf(",");
                    string end = start.Substring(0, endindex);
                    userid = end.Replace("\"", "").Replace("}}", string.Empty).Trim();
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                //string screen_Name = ObjAccountManager.Getscreen_Name(ref pinterestAccountManager);
                string BoardId = Utils.Utils.getBetween(MainPageSource, "board\", \"id", ",").Replace("\"", "").Replace(":", "").Trim();
              
                string InviteToBoardPostData = string.Empty;
             

                if (ObjAccountManager.LoginPinterestAccount(ref pinterestAccountManager))
                {

                    string checkwelcome = pinterestAccountManager.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));

                    string Url = "https://www.pinterest.com/resource/BoardInviteResource/get/?source_url=%2F" + screen_Name + "%2Fhealth%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22invited_user_id%22%3A%22" + invited_userid + "%22%2C%22field_set_key%22%3A%22boardEdit%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=Modal()%3EBoardCollaboratorInviter(resource%3DBoardResource(board_id%3D" + BoardId + "))%3EBoardInviteForm()%3ESocialTypeaheadField()%3ETypeahead(bypass_maxheight%3Dtrue%2C+tags%3Dpinners_and_contacts%2C+template%3Duser_circle_avatar%2C+view_type%3DuserCircleSelect)&_=1431320928412";

                    string getResponce = pinterestAccountManager.globusHttpHelper.getHtmlfromUrl(new Uri(Url));

                    if (UserName.Contains("@"))
                    {                     
                        InviteToBoardPostData = "source_url=%2F" + screen_Name + "%2F" + BoardName + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22emails%22%3A%5B%22" + UserName + "%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EBoardPage%3EBoardHeader%3EBoardInfoBar%3EShowModalButton(module%3DBoardCollaboratorInviter)%23App%3EModalManager%3EModal()";

                        try
                        {
                            AfterInvitePageSourceData = pinterestAccountManager.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/BoardEmailInviteResource/create/"), InviteToBoardPostData, "https://www.pinterest.com/");
                        }
                        catch (Exception ex)
                        { };                  
                    }
                    else
                    {
                        InviteToBoardPostData = "source_url=%2F" + screen_Name + "%2F" + BoardName + "%2F&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardId + "%22%2C%22invited_user_id%22%3A%22" + invited_userid + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EBoardPage(resource%3DBoardResource(username%3D" + screen_Name + "%2C+slug%3D" + BoardName + "))%3EBoardHeader(resource%3DBoardResource(board_id%3D" + BoardId + "))%3EBoardInfoBar(resource%3DBoardResource(board_id%3D" + BoardId + "))%3EShowModalButton(module%3DBoardCollaboratorInviter)%23Modal(module%3DBoardCollaboratorInviter(resource%3DBoardResource(board_id%3D" + BoardId + ")))";
                        try
                        {
                            AfterInvitePageSourceData = pinterestAccountManager.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/BoardInviteResource/create/"), InviteToBoardPostData, "https://www.pinterest.com/");

                        }
                        catch (Exception ex)
                        { };
                    }
                    if (!string.IsNullOrEmpty(AfterInvitePageSourceData))
                    {
                        GlobusLogHelper.log.Info(" => [ Invitation sent to " + UserName + " for Board " + BoardName + " ]");
                        return true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [  Not Added to Board " + BoardName + " ]");
                        return true;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ " + UserName + " Not Added to Board " + BoardName + " ]");
                return true;
            }
            return false;
        }



    }
}
