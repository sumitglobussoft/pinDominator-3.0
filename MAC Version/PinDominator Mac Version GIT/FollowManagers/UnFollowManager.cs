using AccountManager;
using BaseLib;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PinDominator;
using ScraperManagers;

namespace FollowManagers
{
    public delegate void AccountReport_UnFollow();
	public delegate void UnFollowAccount(int AccCount);
    public class UnFollowManager
    {
        public static AccountReport_UnFollow objUnFollowDelegate;
		public static UnFollowAccount objDelegateNoOfActiveAcc_UnFollow;
		public static UnFollowAccount objDelegateNoOfDeadAcc_UnFollow;

        #region Global Variable

        public bool _IsfevoriteUnFollow = false;
        public int Nothread_UnFollow = 5;
        public bool isStopUnFollow = false;
        public List<Thread> lstThreadsUnFollow = new List<Thread>();
        public static int countThreadControllerUnFollow = 0;
        public static int UnFollowdata_count = 0;
        public int MaxUnFollowCount = 0;
        public readonly object UnFollowObjThread = new object();
        public int NOofDays = 0;
        public bool chkNoOFDays_UnFollow=false;
        public static bool chkUploadUnFollowList = false;

        public int minDelayUnFollow
        {
            get;
            set;
        }

        public int maxDelayUnFollow
        {
            get;
            set;
        }

        public int NoOfThreadsUnFollow
        {
            get;
            set;
        }

        string Unfollowuserid = string.Empty;
    
        List<string> lstFollowers = new List<string>();
        List<string> lstFollowings = new List<string>();
        List<string> followings = new List<string>();
        List<string> NonFollowing = new List<string>();
        int NoOfPage = 10;
		int ActiveAccCount = 0;
		int DeadAccCount = 0;

        #endregion

        Accounts ObjAccountManager = new Accounts();
        ScraperManager objScrape = new ScraperManager();
        QueryManager Qm = new QueryManager();

        public void StartUnFollow()
        {
            try
            {
                countThreadControllerUnFollow = 0;
                int numberOfAccountPatchUnFollow = 25;

                if (NoOfThreadsUnFollow > 0)
                {
                    numberOfAccountPatchUnFollow = NoOfThreadsUnFollow;
                }
                UnFollowdata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchUnFollow);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerUnFollow > Nothread_UnFollow)
                            {
                                try
                                {
                                    lock (UnFollowObjThread)
                                    {
                                        Monitor.Wait(UnFollowObjThread);
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
                                Thread profilerThread = new Thread(StartUnFollowMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerUnFollow++;
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

        public void StartUnFollowMultiThreaded(object objparameter)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopUnFollow)
                {
                    try
                    {
                        lstThreadsUnFollow.Add(Thread.CurrentThread);
                        lstThreadsUnFollow.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objparameter;
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
									objDelegateNoOfDeadAcc_UnFollow(DeadAccCount);
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                
                            }
                            catch { };
                        }
                        if(objPinUser.isloggedin == true)
                        {
                            try
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logged In With : " + objPinUser.Username + " ]");
								ActiveAccCount++;
								objDelegateNoOfActiveAcc_UnFollow(ActiveAccCount);
                                StartActionMultithreadUnFollow(ref objPinUser);
                            }
                            catch(Exception ex)
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
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void  StartActionMultithreadUnFollow(ref PinInterestUser objPinUser)
        {
            try
            {
                try
                {
                    lstThreadsUnFollow.Add(Thread.CurrentThread);
                    lstThreadsUnFollow.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };
                string ScreenName = objPinUser.ScreenName; 
                List<string> lstNonFollowing = new List<string>();
                if (!chkUploadUnFollowList)
                {
                    if (!chkNoOFDays_UnFollow)
                    {
                        List<string> lstUsers = new List<string>();
                        try
                        {
                            lstUsers = objScrape.GetUserFollowing_new(ScreenName, NoOfPage, Globals.followingCountLogin);
                            lstUsers.Reverse();
                            lstUsers = lstUsers.Distinct().ToList();
                            if (lstUsers.Count > 0)
                            {
                                ClGlobul.lstFollowing_UnFollow.AddRange(lstUsers);
                            }

                            ClGlobul.lstFollowing_UnFollow = ClGlobul.lstFollowing_UnFollow.Distinct().ToList();
                        }
                        catch (Exception ex)
                        {
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Not Fetched User List ]");
                        }

                        lstNonFollowing.AddRange(lstUsers);
                    }
                    else if (chkNoOFDays_UnFollow == true)
                    {
                        clsSettingDB DataBase = new clsSettingDB();
                        DataTable dt = DataBase.SelectUnfollowsToday(objPinUser.Username);
                        foreach (DataRow rd in dt.Rows)
                        {
                            DateTime dt1 = DateTime.Parse(rd[3].ToString());
                            DateTime dt2 = DateTime.Today;
                            TimeSpan span = dt2 - dt1;
                            int DayDiffrence = (int)span.Days;
                            if (DayDiffrence >= NOofDays)
                            {
                                NonFollowing.Add("http://pinterest.com/" + rd[2].ToString() + "/");
                            }
                        }
                        lstNonFollowing = NonFollowing.Distinct().ToList();
                    }
                }
                if (chkUploadUnFollowList == true)
                {
                    try
                    {
                        lstNonFollowing.AddRange(ClGlobul.lstUploadUnFollowList);
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error ;" + ex.StackTrace);
                    }
                }

                if (lstNonFollowing.Count() > 0)
                {
                    int UnFollowCount = 0;
                    foreach (string UnFollowUrl in lstNonFollowing)
                    {
                        string url = string.Empty;
                        try
                        {                          
                            if (!UnFollowUrl.Contains("http://pinterest.com"))
                            {
                                url = "https://pinterest.com/" + UnFollowUrl + "/";
                            }
                            else
                            {
                                url = UnFollowUrl;
                            }
                   
                            bool IsUnFollowed = UnFollow_New(url, ref objPinUser);

                            if (IsUnFollowed)
                            {
                                #region AccountReport
                                string Username = Utils.Utils.getBetween(url, ".com/", "/");
                                string module = "UnFollow";
                                string status = "UnFollowed";
                                Qm.insertAccRePort(objPinUser.Username, module, "", "", Username, "", "", "", status, "", "", DateTime.Now);
                                objUnFollowDelegate();

                                #endregion

								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Unfollowed : " + url + " From " + objPinUser.Username + " ]");
                                UnFollowCount++;
                               // MaxUnFollowCount--;
                            }
                            else
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Not Unfollowed : " + url + " From " + objPinUser.Username + " ]");
                            }

                            int Delay = RandomNumberGenerator.GenerateRandom(minDelayUnFollow, maxDelayUnFollow);
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay For " + Delay + " Seconds ]");
                            Thread.Sleep(Delay * 1000);

                        }
                        catch (Exception ex)
                        {
                           // GlobusFileHelper.AppendStringToTextfileNewLine("Error --> UnFollowUserMultiThreaded() 1--> " + ex.Message, ApplicationData.ErrorLogFile);
                        }
                        if (MaxUnFollowCount == UnFollowCount)
                        {
                            break;
                        }

                    }
					GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                    GlobusLogHelper.log.Info("-----------------------------------------------------------------------------------");
                }
                else
                {
					GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Users to UnFollow ]");
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
                    if (countThreadControllerUnFollow > Nothread_UnFollow)
                    {
                        lock (UnFollowObjThread)
                        {
                            Monitor.Pulse(UnFollowObjThread);
                        }
                        UnFollowdata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerUnFollow--;         
            }
        }

        public bool UnFollow_New(string UserUrl, ref PinInterestUser objPinUserManager)
        {
            try
            {
                string checklogin = objPinUserManager.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (objPinUserManager.isloggedin == true)
                {
                    string UnFollowPageSource = objPinUserManager.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserUrl), "", "", 80, "", "", objPinUserManager.UserAgent);
                                   
                    try
                    {
                        int startindex = UnFollowPageSource.IndexOf("\"user_id\":");
                        string start = UnFollowPageSource.Substring(startindex).Replace("\"user_id\":", "");
                        int endindex = start.IndexOf("\", \"");
                        string end = start.Substring(0, endindex);
                        Unfollowuserid = end.Replace("\"", "");
                    }
                    catch (Exception ex)
                    {

                    }
                    string UnfollowuserName = UserUrl.Split('/')[UserUrl.Split('/').Count() - 2];

                    string redirectDomain = GlobusHttpHelper.valueURl.Split('.')[0];
                    string newHomePageUrl = redirectDomain + "." + "pinterest.com";
                    string checkalreadyFollowed = objPinUserManager.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/" + UnfollowuserName.Replace(" ", "") + "/"));
                    if (checkalreadyFollowed.Contains("buttonText\">Unfollow"))
                    {                        
                        string PostData = "source_url=%2F" + UnfollowuserName + "%2Ffollowing%2F&data=%7B%22options%22%3A%7B%22user_id%22%3A%22" + Unfollowuserid.Trim() + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserProfilePage%3EUserProfileContent%3EUserProfileFollowingGrid%3EGrid%3EGridItems%3EUser%3EUserFollowButton(user_id%3D" + Unfollowuserid.Trim() + "%2C+follow_class%3Ddefault%2C+followed%3Dtrue%2C+class_name%3DgridItem%2C+log_element_type%3D62%2C+text%3DUnfollow%2C+color%3Ddim%2C+disabled%3Dfalse%2C+follow_text%3DFollow%2C+unfollow_text%3DUnfollow%2C+is_me%3Dfalse%2C+follow_ga_category%3Duser_follow%2C+unfollow_ga_category%3Duser_unfollow)";
                        string PageUrl = redirectDomain + ".pinterest.com/resource/UserFollowResource/delete/";
                        try
                        {
                            UnFollowPageSource = objPinUserManager.globusHttpHelper.postFormDataProxyPin(new Uri(PageUrl), PostData, newHomePageUrl);// newHomePageUrl + objPinUserManager.Username
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }


                        if (!UnFollowPageSource.Contains("<div>Uh oh! Something went wrong"))
                        {
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Successfully UnFollow For this User " + objPinUserManager.Username + " ]");
                            return true;
                        }
                        else if (UnFollowPageSource.Contains("We're unable to complete this request. Your account is currently in read-only mode to protect your pins. You must reset your password to continue pinning."))
                        {
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ UnFollow Process Failed For this User " + objPinUserManager.Username + "--> ]");
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ We're unable to complete this request. Your account is currently in read-only mode to protect your pins. ]");
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ You must reset your password to continue pinning. ]");
                            return false;
                        }
                        else
                        {
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ UnFollow Process Failed For this User " + objPinUserManager.Username + " ]");
                            return false;
                        }
                    }
                    else 
                    {
                        GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ " + UnfollowuserName + " Is Not Follow Before With " + objPinUserManager.Username + " ]");
                        return false;
                    }
                }
                else
                {
					GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Login Issue with " + objPinUserManager.Username + " ]");
                    return false;
                }

            }
            catch (Exception ex)
            {
				GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ UnFollow Process Failed For this User " + objPinUserManager.Username + " ]");
                return false;
            }
        }



    }
}
