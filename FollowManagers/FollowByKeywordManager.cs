using AccountManager;
using BaseLib;
using BasePD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FollowManagers
{
    public class FollowByKeywordManager
    {
        #region Global Variable

        public int Nothread_FollowByKeyword = 5;
        public bool isStopFollowByKeyword = false;
        public List<Thread> lstThreadsFollowByKeyword = new List<Thread>();
        public static int countThreadControllerFollowByKeyword = 0;  
        public static int FollowByKeyworddata_count = 0;
        public int NoOfUserFollowByKeyword = 0;
        public int AccPerDayUserFollowByKeyword = 0;
        public readonly object FollowByKeywordObjThread = new object();
        public bool _IsfevoriteFollowByKeyword = false;

        string[] array = null;
        List<string> Pins = new List<string>();
        int FollowData = 0;

        public int minDelayFollowByKeyword
        {
            get;
            set;
        }

        public int maxDelayFollowByKeyword
        {
            get;
            set;
        }

        public int NoOfThreadsFollowByKeyword
        {
            get;
            set;
        }

        QueryManager QM = new QueryManager();
        Accounts ObjAccountManager = new Accounts();
        
        GlobusRegex globusRegex = new GlobusRegex();

        #endregion

        public void StartFollowKeyword()
        {
            try
            {
                countThreadControllerFollowByKeyword = 0;
                int numberOfAccountPatch = 25;

                if (NoOfThreadsFollowByKeyword > 0)
                {
                    numberOfAccountPatch = NoOfThreadsFollowByKeyword;
                }
                FollowByKeyworddata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatch);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {                     
                        foreach (string account in listAccounts)
                        {

                            if (countThreadControllerFollowByKeyword > Nothread_FollowByKeyword)
                            {
                                try
                                {
                                    lock (FollowByKeywordObjThread)
                                    {
                                        Monitor.Wait(FollowByKeywordObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {

                                }
                            }
                            string acc=account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {
                                Thread profilerThread = new Thread(StartFollowByKeywordMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerFollowByKeyword++;
                            }                            
                        }
                        // }).Start();
                    }
                }

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            finally
            {
                GlobusLogHelper.log.Info(" => [ Follow By Keyword Process Finished ]");
                GlobusLogHelper.log.Info(" [ PROCESS COMPLETED ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
            }

        }

        public void StartFollowByKeywordMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {             
                if (!isStopFollowByKeyword)
                {
                    try
                    {
                        lstThreadsFollowByKeyword.Add(Thread.CurrentThread);
                        lstThreadsFollowByKeyword.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;
                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);                        

                        foreach (string comment in ClGlobul.lstkeyword)
                        {
                            array = Regex.Split(comment, "::");
                            if (array.Length == 2)
                            {
                                if (array[0] == objPinUser.Niches)
                                {
                                    GlobusLogHelper.log.Info(" => [ " + objPinUser.Username + ">>>>" + objPinUser.Niches + " ]");

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
                                   
                                }
                            }
                        }                                        
                        StartActionMultithreadFollowByKeyword(ref objPinUser);
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " +  objPinUser.Username + " ]");
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

                    if (countThreadControllerFollowByKeyword > Nothread_FollowByKeyword)
                    {
                        lock (FollowByKeywordObjThread)
                        {
                            Monitor.Pulse(FollowByKeywordObjThread);
                        }
                        FollowByKeyworddata_count--;
                    }


                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
            }


        }

        public void StartActionMultithreadFollowByKeyword(ref PinInterestUser objPinUser)
        {
            try
            {
                DataTable FollowedCount = QM.SelectFollowsToday(objPinUser.Username);
                int TotalFollow = NoOfUserFollowByKeyword;
                //int CountLeft = NoOfUserFollowByKeyword - FollowedCount.Rows.Count;

                if (TotalFollow > 0)
                {
                    int Time = AccPerDayUserFollowByKeyword;
                    int delay = (Time * 60 * 60) / TotalFollow;

                    string[] keywordArray = Regex.Split(array[1], ",");

                    bool OverFollow = false;
                    foreach (string PinSearch in keywordArray)
                    {
                        try
                        {
                            Pins = getUserKeywordSearch_New(PinSearch, NoOfUserFollowByKeyword, ref objPinUser);

                            foreach (string FollowUser in Pins)
                            {
                                DataTable dt = QM.SelectFollowsToday(objPinUser.Username);
                                DataTable dt1 = QM.SelectFollowsCheck(objPinUser.Username, FollowUser);
                                if (FollowData >= TotalFollow)
                                {
                                    GlobusLogHelper.log.Info(" => [ Followed " + dt.Rows.Count + " Users ]");
                                    OverFollow = true;
                                    TotalFollow = TotalFollow + NoOfUserFollowByKeyword;
                                    break;
                                }
                                if (dt1.Rows.Count == 0)
                                {
                                    string User = FollowPeople_New(ref objPinUser, FollowUser);


                                    if (User == "Followed")
                                    {
                                        string CSV_Header = "UserName" + "," + "Follow User" + "," + "Date";
                                        string CSV_Data = objPinUser.Username + "," + FollowUser + "," + System.DateTime.Now.ToString();
                                        string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Follow");
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\FollowByKeyword.csv");
                                        QM.insertFollowDate(objPinUser.Username, FollowUser, PinSearch);
                                        OverFollow = false;
                                    }
                                    else if (User == "exceeded the maximum rate")
                                    {
                                        GlobusLogHelper.log.Info(" => [ Rate Limit Exceeded ]");
                                    }
                                    else
                                    {
                                        // FollowAddToLogger("Not Followed >>> " + FollowUser + ">>> Account >>> " + accountManager.Username);
                                    }
                                    FollowData++;
                                    int RandomDelayAdd = RandomNumberGenerator.GenerateRandom(minDelayFollowByKeyword, maxDelayFollowByKeyword);
                                    GlobusLogHelper.log.Info(" => [ Delay For " + RandomDelayAdd + " Seconds ]");
                                    int totaldelay = (RandomDelayAdd * 1000);
                                    Thread.Sleep(totaldelay);
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ Already Followed " + FollowUser + " from " + objPinUser.Username + "]");
                                }
                                if (OverFollow)
                                {
                                    break;
                                }
                            }

                        }
                        catch (Exception ex)
                        { }

                    }
                }

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

        }


        public List<string> getUserKeywordSearch_New(string keyword, int Count, ref PinInterestUser objPinUser)
        {
            List<string> lstUserPins = new List<string>();

            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Pins For the Keyword " + keyword + " ]");

                string UserUrl = string.Empty;
                string UserPins = string.Empty;


                for (int i = 0; i <= Count; i++)
                {
                    try
                    {
                        if (i == 0)
                        {
                            UserUrl = "http://pinterest.com/search/people/?q=" + keyword;
                        }
                        else
                        {
                            UserUrl = "http://pinterest.com/resource/SearchResource/get/?source_url=%2Fsearch%2Fpins%2F%3Fq%3Droses%26rs%3Dac%26len%3D4&data=%7B%22options%22%3A%7B%22query%22%3A%22roses%22%2C%22bookmarks%22%3A%5B%22b281MHw4ODgyNzJiYWQxN2E0NmM2MTNlNjNkYjY1Y2E4ZDY2OTI4ODZkMzBiNGEyNjU4ZGMyODFmMzhhOTUzZjE2NTRm%22%5D%2C%22show_scope_selector%22%3Atrue%2C%22scope%22%3A%22pins%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22411bd12%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A1%7D&module_path=App()%3EHeader()%3Eui.SearchForm()%3Eui.TypeaheadField(enable_recent_queries%3Dtrue%2C+name%3Dq%2C+view_type%3Dsearch%2C+prefetch_on_focus%3Dtrue%2C+value%3D%22%22%2C+populate_on_result_highlight%3Dtrue%2C+search_delay%3D0%2C+search_on_focus%3Dtrue%2C+placeholder%3DSearch%2C+tags%3Dautocomplete)&_=" + DateTime.Now.Ticks;
                        }
                        string UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(UserUrl), "http://pinterest.com/", string.Empty, objPinUser.UserAgent);
                    
                        if (!UserPageSource.Contains("The page you're looking for could not be found"))
                        {
                            List<string> lst = globusRegex.GetHrefUrlTags(UserPageSource);
                            string[] array = System.Text.RegularExpressions.Regex.Split(UserPageSource, "href=");
                            foreach (string item in array)
                            {
                                if (item.Contains("class=\\\"userWrapper\\\""))
                                {
                                    try
                                    {
                                        int FirstPinPoint = item.IndexOf("\\\"/");
                                        int SecondPinPoint = item.IndexOf("/\\\"");

                                        string PinUrl = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\\", "").Replace("\"", string.Empty).Replace("href=", string.Empty).Replace("/", string.Empty).Trim();

                                        lstUserPins.Add(PinUrl);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info(" => [ Sorry No More Pages ]");
                        break;
                    }

                    lstUserPins = lstUserPins.Distinct().ToList();
                    lstUserPins.Reverse();
                }

                GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstUserPins.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstUserPins;
        }


        public string FollowPeople_New(ref PinInterestUser objPinUser, string Username)
        {
            try
            {
                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }
                string UserUrl = "https://www.pinterest.com/resource/UserFollowResource/create/";
                string Refrer = "http://www.pinterest.com/" + Username.Replace(" ", "");
                //Refrer = "http://www.pinterest.com/";

                // string Refrer = Username.Replace(" ", "");
                string UserPage = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(Refrer), "", "", 80, "", "", "");

                if (string.IsNullOrEmpty(UserPage))
                {
                    //User Does not exist 
                }

                string userid = string.Empty;//GetUserID(UserPage);

                try
                {
                    int startindex = UserPage.IndexOf("\"user_id\":");
                    string start = UserPage.Substring(startindex).Replace("\"user_id\":", "");
                    int endindex = start.IndexOf(",");
                    string end = start.Substring(0, endindex);
                    userid = end.Replace("\"", "").Replace("}}", string.Empty).Trim();
                }
                catch (Exception ex)
                {

                }

                string checkalreadyFollowed = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/" + Username.Replace(" ", "") + "/"));
                if (checkalreadyFollowed.Contains("buttonText\">Unfollow"))
                {
                    return " ";
                }

                Thread.Sleep(10 * 1000);

                string PostData = "source_url=%2F" + Username.Replace(" ", "") + "%2F&data=%7B%22options%22%3A%7B%22user_id%22%3A%22" + userid + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserProfilePage(resource%3DUserResource(username%3D" + Username.Replace(" ", "") + "%2C+invite_code%3Dnull))%3EUserProfileHeader(resource%3DUserResource(username%3D" + Username.Replace(" ", "") + "%2C+invite_code%3Dnull))%3EUserFollowButton(followed%3Dfalse%2C+is_me%3Dfalse%2C+unfollow_text%3DUnfollow%2C+memo%3D%5Bobject+Object%5D%2C+follow_ga_category%3Duser_follow%2C+unfollow_ga_category%3Duser_unfollow%2C+disabled%3Dfalse%2C+color%3Dprimary%2C+text%3DFollow%2C+user_id%3D" + userid + "%2C+follow_text%3DFollow%2C+follow_class%3Dprimary)";      
                string FollowPageSource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri(UserUrl), PostData, "https://www.pinterest.com/");

                string AfterFollowPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/" + Username.Replace(" ", "") + "/"));

                if (!string.IsNullOrEmpty(AfterFollowPageSource) && AfterFollowPageSource.Contains("buttonText\">Unfollow"))
                {
                    GlobusLogHelper.log.Info(" => [ Successfully Followed " + Username + ">>>" + objPinUser.Username + " ]");
                    return "Followed";
                }
                else
                {
                    GlobusLogHelper.log.Info(" => [ Follow Process Failed User " + Username + ">>>" + objPinUser.Username + " ]");
                    return "NotFollowed";
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                return "NotFollowed";
            }
            return null;
        }



    }




}

