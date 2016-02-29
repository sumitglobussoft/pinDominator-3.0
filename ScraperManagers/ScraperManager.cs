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

namespace ScraperManagers
{
    public delegate void AccountReport_Scraper();
    public class ScraperManager
    {
        public static AccountReport_Scraper objScraperDelegate;

        #region Global Variable

        public int Nothread_Scraper = 5;
        public bool isStopScraper = false;
        public List<Thread> lstThreadsScraper = new List<Thread>();
        public static int countThreadControllerScraper  = 0;
        public static int Scraperdata_count = 0;
        public int MaxCountScraper = 5;
        public readonly object ScraperObjThread = new object();
        public string UserScraperType = "followers";      
        public string UserName = string.Empty;
        public bool _IsfevoriteScraper = false;

        int NoOfPage = 100;
        string FollowUrl = string.Empty;
        string AppVersion = string.Empty;
        string bookmark = string.Empty;
        string referer = string.Empty;
        string FollowerPageSource = string.Empty;          
        string User = string.Empty;         
        List<string> followings = new List<string>();
        List<string> followers = new List<string>();
        List<string> templist = new List<string>();

        public int minDelayScraper
        {
            get;
            set;
        }

        public int maxDelayScraper
        {
            get;
            set;
        }

        public int NoOfThreadsScraper
        {
            get;
            set;
        }

        GlobusRegex objGlobusRegex = new GlobusRegex();
        Accounts Obj_AccountManager = new Accounts();
        PinInterestUser objPinUser = new PinInterestUser();

        #endregion


        Accounts ObjAccountManager = new Accounts();
        QueryManager qm = new QueryManager();
        public void StartScraper()
        {
            try
            {
                countThreadControllerScraper = 0;
                int numberOfAccountPatchScraper = 25;

                if (NoOfThreadsScraper > 0)
                {
                    numberOfAccountPatchScraper = NoOfThreadsScraper;
                }

                //Scraperdata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();

                //if (PDGlobals.listAccounts.Count >= 1)
                //{
                //    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchScraper);
                //    foreach (List<string> listAccounts in list_listAccounts)
                //    {
                //        foreach (string account in listAccounts)
                //        {
                //            if (countThreadControllerScraper > Nothread_Scraper)
                //            {
                //                try
                //                {
                //                    lock (ScraperObjThread)
                //                    {
                //                        Monitor.Wait(ScraperObjThread);
                //                    }
                //                }
                //                catch (Exception Ex)
                //                {

                //                }
                //            }

                //            string acc = account.Split(':')[0];
                //            PinInterestUser objPinInterestUser = null;
                //            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                          //  if (objPinInterestUser != null)
                          //  {
                                Thread profilerThread = new Thread(StartScraperMultiThreaded);
                                //profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start();// (new object[] { objPinInterestUser });

                                countThreadControllerScraper++;
                          //  }
                //        }

                //    }
                //}    
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartScraperMultiThreaded()
        {
            //PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopScraper)
                {
                    try
                    {
                        lstThreadsScraper.Add(Thread.CurrentThread);
                        lstThreadsScraper.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };
           
                    try
                    {
                        #region Follower

                        if (UserScraperType == "followers")
                        {
                            templist = GetUserFollower_new();
                            ClGlobul.lstTotalUserScraped.AddRange(templist);
                        }
                            
                        #endregion

                        #region Following

                        else if (UserScraperType == "following")
                        {
                            templist = GetUserFollowing_new(UserName, NoOfPage, MaxCountScraper);
                           ClGlobul.lstTotalUserScraped.AddRange(templist);
                        }

                        #endregion

                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Debug("Scraper Error");
                    }
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
                    if (countThreadControllerScraper > Nothread_Scraper)
                    {
                        lock (ScraperObjThread)
                        {
                            Monitor.Pulse(ScraperObjThread);
                        }
                        Scraperdata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerScraper--;
                //if (followings.Count == MaxCountScraper)  //|| DivideByUserinput < 0)
                //{
                    GlobusLogHelper.log.Info(" => [ Process Completed Please. Now you can export file ]");
                    GlobusLogHelper.log.Info(" [ PROCESS COMPLETED ]");
                    GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");

                //}
            }

        }


        public List<string> GetUserFollower_new()
        {
            try
            {
                ClGlobul.lstTotalUserScraped.Clear();
                List<string> lstFollower = new List<string>(); 
                GlobusLogHelper.log.Info(" => [ Starting Extraction Of Followers For " + UserName + " ]");
                objPinUser.globusHttpHelper = new GlobusHttpHelper();

                string TotalFollowersUrl = "https://pinterest.com/" + UserName;
                string responseFollowersUrl = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(TotalFollowersUrl), referer, string.Empty, "");
                int TotalFollower = int.Parse(Utils.Utils.getBetween(responseFollowersUrl, "follower_count\": ", ","));
                int NoOfPage = TotalFollower / 12 + 1;
                for (int i = 1; i <= NoOfPage; i++) //  for (int i = 1; i <= NoOfPage; i++)
                {
                    try
                    {
                        if (i == 1)
                        {
                            FollowUrl = "http://pinterest.com/" + UserName + "/" + UserScraperType + "/";
                            FollowerPageSource =
                            FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(FollowUrl), referer, string.Empty, "");
                            referer = FollowUrl;
                        }
                        else
                        {
                            try
                            {
                                FollowUrl = "http://pinterest.com/resource/UserFollowersResource/get/?source_url=%2F" + UserName + "%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%2C%22bookmarks%22%3A%5B%22" + bookmark + "%22%5D%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + AppVersion + "%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Afalse%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22layout%22%3A%22fixed_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A" + i + "%7D&_=" + DateTime.Now.Ticks;                          
                                FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), referer, "", 80, string.Empty, "", "");
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                        }
                        ///Get App Version 
                        if (FollowerPageSource.Contains("app_version") && string.IsNullOrEmpty(AppVersion))
                        {
                            string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "app_version");
                            if (ArrAppVersion.Count() > 0)
                            {
                                string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];
                                int startindex = DataString.IndexOf("\": \"");
                                int endindex = DataString.IndexOf("\", \"");

                                AppVersion = DataString.Substring(startindex, endindex - startindex).Replace("\": \"", "");
                            }
                        }

                        try
                        {
                            if (!FollowerPageSource.Contains("No one has followed"))
                            {
                                List<string> lst = objGlobusRegex.GetHrefUrlTags(FollowerPageSource);
                                if (lst.Count == 0)
                                {
                                    lst = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "href").ToList();
                                }
                                foreach (string item in lst)
                                {
                                    if (item.Contains("class=\"userWrapper") || item.Contains("class=\\\"userWrapper"))
                                    {
                                        try
                                        {
                                            string User = string.Empty;

                                            if (item.Contains("\\"))
                                            {
                                                int FirstPinPoint = item.IndexOf("=\\\"");
                                                int SecondPinPoint = item.IndexOf("class=");
                                                User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("=", string.Empty).Replace("/", string.Empty).Trim();
                                            }
                                            else
                                            {

                                                int FirstPinPoint = item.IndexOf("href=");
                                                int SecondPinPoint = item.IndexOf("class=");

                                                User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("href=", string.Empty).Replace("/", string.Empty).Trim();
                                            }
                                            if (followers.Count == MaxCountScraper)
                                            {
                                                return ClGlobul.lstTotalUserScraped; 
                                            }

                                            
                                           // GlobusLogHelper.log.Info(" => [ " + User + " ]");
                                            followers.Add(User);
                                            if (followers.Count == MaxCountScraper)
                                            {
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                        }
                                    }
                                }

                                ///get bookmarks value from page 
                                ///
                                if (FollowerPageSource.Contains("bookmarks"))
                                {
                                    string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "bookmarks");

                                    string Datavalue = string.Empty;
                                    if (bookmarksDataArr.Count() > 2)
                                        Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 2];
                                    else
                                        Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                                    bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);
                                }
                                followers = followers.Distinct().ToList();

                                foreach (string lstdata in followers)
                                {
                                    lstFollower.Add(lstdata);
                                    #region AccountReport

                                    string module = "Scraper";
                                    string status = "Followers";
                                    qm.insertAccReportScrapeUser(module, lstdata, status, DateTime.Now);
                                    objScraperDelegate();

                                    #endregion
                                    if (lstFollower.Count >= MaxCountScraper)
                                    {
                                        return ClGlobul.lstTotalUserScraped; 
                                    }
                                }
                                ClGlobul.lstTotalUserScraped = lstFollower.Distinct().ToList();
                               
                                Thread.Sleep(1000);

                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [ No Followers ]");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
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

            GlobusLogHelper.log.Info(" => [ Total  Followers : " + ClGlobul.lstTotalUserScraped.Count + " ]");

            return ClGlobul.lstTotalUserScraped;
        }

        public List<string> GetUserFollowing_new(string UserName, int NoOfPage, int FollowingCount)
        {
            try
            {
                ClGlobul.lstTotalUserScraped.Clear();
                List<string> lstFollowing = new List<string>();
                GlobusLogHelper.log.Info(" => [ Starting Extraction Of Following For " + UserName + " ]");       
                objPinUser.globusHttpHelper = new GlobusHttpHelper();

                string TotalFollowingUrl = "https://pinterest.com/" + UserName;
                string responseFollowingUrl = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(TotalFollowingUrl), referer, string.Empty, "");
                int TotalFollowing = int.Parse(Utils.Utils.getBetween(responseFollowingUrl, "following_count\":", ","));
                int PageCount = TotalFollowing / 12 + 1;

                for (int i = 1; i <= PageCount; i++)
                {
                    try
                    {
                        string FollowerPageSource = string.Empty;

                        if (i == 1)
                        {
                            FollowUrl = "http://pinterest.com/" + UserName + "/following/";
                            FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(FollowUrl), referer, string.Empty, "");
                            referer = FollowUrl;
                        }
                        else
                        {
                            FollowUrl = "https://pinterest.com/resource/UserFollowingResource/get/?source_url=%2F" + UserName + "%2Ffollowing%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%2C%22bookmarks%22%3A%5B%22" + bookmark + "%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App(module%3D%5Bobject+Object%5D)&_=144204352215" + (i - 1);

                            try
                            {
                                FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), referer, "", 80, string.Empty, "", "");
                            }
                            catch
                            {
                                FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), "", Convert.ToInt32(""), "", "");

                            }
                            if (FollowerPageSource.Contains("Whoops! We couldn't find that page."))
                            {
                                break;
                            }
                        }

                        ///Get App Version 
                        if (FollowerPageSource.Contains("app_version") && string.IsNullOrEmpty(AppVersion))
                        {
                            string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "app_version");
                            if (ArrAppVersion.Count() > 0)
                            {
                                string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                                int startindex = DataString.IndexOf("\": \"");
                                int endindex = DataString.IndexOf("\", \"");

                                AppVersion = DataString.Substring(startindex, endindex - startindex).Replace("\": \"", "");
                            }
                        }

                        ///get bookmarks value from page 
                        ///
                        if (FollowerPageSource.Contains("bookmarks"))
                        {
                            string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "bookmarks");

                            string Datavalue = string.Empty;
                            if (bookmarksDataArr.Count() > 2)
                                Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 2];
                            else
                                Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                            bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);
                        }


                        try
                        {
                            if (!FollowerPageSource.Contains("No one has followed"))
                            {
                                List<string> lst = objGlobusRegex.GetHrefUrlTags(FollowerPageSource);
                                if (lst.Count == 0)
                                {
                                    lst = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "href").ToList();
                                    if (lst.Count() == 1)
                                    {
                                        lst = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "\"username\":").ToList();
                                    }
                                }
                                foreach (string item in lst)
                                {
                                    if (item.Contains("class=\"userWrapper") || item.Contains("class=\\\"userWrapper"))
                                    {
                                        try
                                        {
                                            if (item.Contains("\\"))
                                            {
                                                int FirstPinPoint = item.IndexOf("=\\\"/");
                                                int SecondPinPoint = item.IndexOf("/\\\"");
                                                User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("=", string.Empty).Replace("/", string.Empty).Trim();
                                            }
                                            else
                                            {
                                                int FirstPinPoint = item.IndexOf("href=");
                                                int SecondPinPoint = item.IndexOf("class=");

                                                User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("href=", string.Empty).Replace("/", string.Empty).Trim();
                                            }
                                            if (followings.Count == FollowingCount)
                                            {
                                                break;
                                            }
                                            followings.Add(User);
                                            

                                            //GlobusLogHelper.log.Info(" => [ " + User + " ]");                                           
                                           
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                        }
                                    }
                                    if (i > 1)
                                    {
                                        if (item.Contains("\"request_identifier\":"))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                User = Utils.Utils.getBetween(item, "\"", "\"");
                                                if (User == UserName)
                                                {
                                                    break;
                                                }
                                                followings.Add(User);
                                            
                                                //GlobusLogHelper.log.Info(" => [ " + User + " ]");

                                                if (followings.Count == FollowingCount)
                                                {
                                                    break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                            }
                                        }
                                    }
                                }


                                followings = followings.Distinct().ToList();
                                foreach (string lstdata in followings)
                                {
                                    lstFollowing.Add(lstdata);
                                    #region AccountReport

                                    string module = "Scraper";
                                    string status = "Following";
                                    qm.insertAccReportScrapeUser(module, lstdata, status, DateTime.Now);
                                    objScraperDelegate();

                                    #endregion
                                    if (lstFollowing.Count >= MaxCountScraper)
                                    {
                                        break;
                                    }
                                }
                                ClGlobul.lstTotalUserScraped = lstFollowing.Distinct().ToList();
                                if (ClGlobul.lstTotalUserScraped.Count >= MaxCountScraper)
                                {
                                    return ClGlobul.lstTotalUserScraped;
                                }
                              
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [ No following ]");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        break;

                    }
                }
                GlobusLogHelper.log.Info(" => [ Total Followings : " + ClGlobul.lstTotalUserScraped.Count + " ]");
                //GlobusLogHelper.log.Info(" => [ Finished Extracting following For " + UserName + " ]");
                //GlobusLogHelper.log.Info(" => [ Process Completed Please. Now you can export file ]");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return ClGlobul.lstTotalUserScraped;
        }

        public void ExportScraper()
        {
            try
            {
                 ClGlobul.lstTotalUserScraped = ClGlobul.lstTotalUserScraped .Distinct().ToList();
                 GlobusLogHelper.log.Info(" => [ Start User Export Process ]");
                 try
                 {
                     if (UserScraperType == "followers")
                     {
                         GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollower);
                         GlobusLogHelper.log.Info(" => File Path " + PDGlobals.UserScrapedFollower + " ]");
                     }
                     else
                     {
                         GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollowing);
                         GlobusLogHelper.log.Info(" => File Path " + PDGlobals.UserScrapedFollowing + " ]");
                     }
                 }
                 catch (Exception ex)
                 {
                     GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                 }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


    }


}
