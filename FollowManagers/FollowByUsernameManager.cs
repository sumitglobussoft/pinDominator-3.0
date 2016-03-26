using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FollowManagers
{
    public delegate void AccountReport_FollowByUsername();
   public class FollowByUsernameManager
   {
       public static AccountReport_FollowByUsername objFollowByUsernameDelegate;

       #region Global Variable

       public int Nothread_FollowByUsername = 5;
       public bool isStopFollowByUsername = false;
       public List<Thread> lstThreadsFollowByUsername = new List<Thread>();
       public static int countThreadControllerFollowByUsername = 0;
       public static int FollowByUsernamedata_count = 0;
       public int MaxFollowCount = 0;
       public readonly object FollowByUsernameObjThread = new object();
       public static bool rbFollowUser = false;
       public static bool rdoBtnFollowUserUploaded = false;
       public static bool rbFollowFollowers = false;
       public bool _IsfevoriteFollowByUsername = false;

       public bool rdbSingleUserFollowByUsername = false;
       public bool rdbMultipleUserFollowByUsername = false;
       public string SingleKeyword_FollowByUsername = string.Empty;

       public static bool chkDivideDataFollowByUsername = false;
       public static bool rdbDivideEquallyFollowByUsername = false;
       public static bool rdbDivideGivenByUserFollowByUsername = false;
       public static int CountGivenByUserFollowByUsename = 0;
       List<List<string>> list_lstTargetFollowByUsername = new List<List<string>>();
       List<string> list_lstTargetFollowByUsername_item = new List<string>();
       int LstCounterFollowByUsername = 0;

       string userid = string.Empty;
       string UserFollower = string.Empty;    
       string FollowUrl = string.Empty;
       string AppVersion = string.Empty;
       string bookmark = string.Empty;
       string referer = string.Empty;
       List<string> followings = new List<string>();
       List<string> lstTotalUserScraped = new List<string>();    
       List<string> lstNewUsersToFollow = new List<string>();
       List<string> UserFollowersList = new List<string>();
       List<string> lstUsers = new List<string>();
       public Queue FollowUsersFollowerQueue = new Queue();
       int FollowCount = 0;
       string User = string.Empty;

       public int minDelayFollowByUsername
       {
           get;
           set;
       }

       public int maxDelayFollowByUsername
       {
           get;
           set;
       }

       public int NoOfThreadsFollowByUsername
       {
           get;
           set;
       }

       QueryManager QM = new QueryManager();
       Accounts ObjAccountManager = new Accounts();
      
       GlobusRegex globusRegex = new GlobusRegex();

       #endregion variable

       //AddUsersToBoardManager objAddUsersToBoardManager = new AddUsersToBoardManager();

       public void StartFollowByUsername()
       {
           try
           {
               countThreadControllerFollowByUsername = 0;
               int numberOfAccountPatch = 25;

               if (NoOfThreadsFollowByUsername > 0)
               {
                   numberOfAccountPatch = NoOfThreadsFollowByUsername;
               }
               FollowByUsernamedata_count = PDGlobals.loadedAccountsDictionary.Count();

               #region Divide Data Setting

               if (chkDivideDataFollowByUsername == true)
               {
                   if (rdbDivideEquallyFollowByUsername == true || rdbDivideGivenByUserFollowByUsername == true)
                   {
                       int splitNo = 0;
                       if (rdbDivideEquallyFollowByUsername == true)
                       {
                           splitNo = ClGlobul.lstFollowUsername.Count / PDGlobals.listAccounts.Count;
                       }
                       else if (rdbDivideGivenByUserFollowByUsername == true)
                       {
                           if (Convert.ToInt32(CountGivenByUserFollowByUsename) != 0)
                           {
                               int res = Convert.ToInt32(Convert.ToInt32(CountGivenByUserFollowByUsename));
                               splitNo = res;
                           }
                       }
                       if (splitNo == 0)
                       {
                           splitNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstFollowUsername.Count - 1);
                       }
                       list_lstTargetFollowByUsername = Split(ClGlobul.lstFollowUsername, splitNo);
                   }
               }

               #endregion

               List<List<string>> list_listAccounts = new List<List<string>>();
               if (PDGlobals.listAccounts.Count >= 1)
               {
                   list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatch);
                   foreach (List<string> listAccounts in list_listAccounts)
                   {
                       foreach (string account in listAccounts)
                       {

                           if (countThreadControllerFollowByUsername > Nothread_FollowByUsername)
                           {
                               try
                               {
                                   lock (FollowByUsernameObjThread)
                                   {
                                       Monitor.Wait(FollowByUsernameObjThread);
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
                               Thread profilerThread = new Thread(StartFollowByUsernameMultiThreaded);
                               profilerThread.Name = "workerThread_Profiler_" + acc;
                               profilerThread.IsBackground = true;

                               profilerThread.Start(new object[] { objPinInterestUser });

                               countThreadControllerFollowByUsername++;
                           }
                       }
                   }
               }
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
           }         
       }


       public void StartFollowByUsernameMultiThreaded(object objParameters)
       {
           PinInterestUser objPinUser = new PinInterestUser();
           try
           {
               if (!isStopFollowByUsername)
               {
                   try
                   {
                       lstThreadsFollowByUsername.Add(Thread.CurrentThread);
                       lstThreadsFollowByUsername.Distinct().ToList();
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
                           if (chkDivideDataFollowByUsername == true)
                           {
                               list_lstTargetFollowByUsername_item = list_lstTargetFollowByUsername[LstCounterFollowByUsername];
                           }
                           else
                           {
                               list_lstTargetFollowByUsername_item = ClGlobul.lstFollowUsername;
                           }

                       }
                       catch (Exception ex)
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
                               //GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                               //StartActionMultithreadFollowByUsername(ref objPinUser, list_lstTargetFollowByUsername_item);
                           }
                           catch { };
                       }
                       if (objPinUser.isloggedin == true)
                       {
                           try
                           {
                               GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                               StartActionMultithreadFollowByUsername(ref objPinUser, list_lstTargetFollowByUsername_item);
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

                   if (countThreadControllerFollowByUsername > Nothread_FollowByUsername)
                   {
                       lock (FollowByUsernameObjThread)
                       {
                           Monitor.Pulse(FollowByUsernameObjThread);
                       }
                       FollowByUsernamedata_count--;
                   }
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
               }
               GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
               GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------");
           }

       }


       public void StartActionMultithreadFollowByUsername(ref PinInterestUser objPinUser, List<string> UsercountFollowByUsername)
       {
           try
           {
               try
               {
                   lstThreadsFollowByUsername.Add(Thread.CurrentThread);
                   lstThreadsFollowByUsername.Distinct().ToList();
                   Thread.CurrentThread.IsBackground = true;
               }
               catch (Exception ex)
               { };
               string followCountString = ObjAccountManager.GetFollowingCount(objPinUser.ScreenName, ref objPinUser);
               //followCountString = ObjAccountManager.getBetween(followCountString, "value'>", "</span>");
               followCountString = Utils.Utils.getBetween(followCountString, "value'>", "</span>");
               int followCount = 0;
               int pageCount = 1;
               try
               {
                   followCount = Int32.Parse(followCountString);
               }
               catch 
               {
                   followCount = 0;
               }
               if (followCount != 0)
               {
                   pageCount += followCount / 12;
               }
               else
               {
                   pageCount = 1;
               }


               if (rdoBtnFollowUserUploaded == true)//rbFollowUser
               {

                   string Followuser = string.Empty;
                  // MaxFollowCount = ClGlobul.lstFollowUsername.Count;
                   MaxFollowCount = UsercountFollowByUsername.Count;                  
               }
               else if (rbFollowUser == true)//rdoBtnFollowUserUploaded
               {
                   foreach (string follow in UsercountFollowByUsername)
                   {
                       lstNewUsersToFollow.Add(follow);
                       lstNewUsersToFollow = lstNewUsersToFollow.Distinct().ToList();
                   }
                   UserFollowes(ref objPinUser, lstNewUsersToFollow);
               }
               else if (rbFollowFollowers == true)
               {                 
                   try
                   {
                       int PageCount = (MaxFollowCount / 12) + 2;
                       int NoOfPage = 10;
                      //string UserFollower = FollowUsersFollowerQueue.Dequeue().ToString();
                       foreach (string UserFollower in ClGlobul.ListOfFollowUsersFollowers)
                       {
                           string IsFollowed = FindFollow(ref objPinUser, UserFollower);
                       
                           if (IsFollowed == "Followed")
                           {
                               try
                               {
                                   GlobusLogHelper.log.Info(" => [ Follow " + UserFollower + " For " + objPinUser.Username + " ]");  
                                   UserFollowersList = StartUserScraperMultiThreaded_Follow(UserFollower, PageCount, "followers", ref objPinUser);
                                   Thread.Sleep(1000);
                                   GlobusLogHelper.log.Info(" => [ Extracting followers of " + UserFollower + " ]");
                                   UserFollowes(ref objPinUser, UserFollowersList);
                               }
                               catch (Exception ex)
                               {

                               }
                           }
                           else if (IsFollowed == "NotFollowed")
                           {
                               GlobusLogHelper.log.Info(" => [ User Is Not Follow " + UserFollower + " For " + objPinUser.Username + " ]");                             
                           }
                       }
                     
                   }
                   catch (Exception ex)
                   {
                       
                   }

                   //lstNewUsersToFollow = UserFollowersList;
                   //lstNewUsersToFollow.Distinct().ToList();
                   //bool removedvalue = ClGlobul.ListOfFollowUsersFollowers.Remove();
               }

              
      
               //List<string> myList = new List<string>();
               //if (rbFollowFollowers == true)
               //{                 
               //    myList = lstNewUsersToFollow;
               //    UserFollowes(ref objPinUser, myList);
               //}
               //else
               //{
               //    myList = UsercountFollowByUsername;
               //}
              // UserFollowes(ref objPinUser, myList);
           }
           catch(Exception ex)
           {
               GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
           }
       }

       public void UserFollowes(ref PinInterestUser objPinUser, List<string> myList)
       {
           try
           {
               List<string> Followerlist = new List<string>();
               if (rbFollowFollowers == true)
               {
                   objPinUser.lstUserFollowers = myList;
                   Followerlist = myList;
               }
               if (rbFollowUser == true)
               {
                   Followerlist = myList;
                   Followerlist = Followerlist.Distinct().ToList();
               }
               try
               {
                   int CountToday = 0;
                   int CountByUser = 0;
                   foreach (var itemlist in Followerlist)
                   {
                       string FollowUrl = itemlist.ToString();
                       try
                       {

                           bool followedAlready = false;

                           if (followedAlready)
                           {
                               GlobusLogHelper.log.Info(" => [ User " + FollowUrl + " already followed ]");
                               //FollowCount++;
                               //CountToday++;
                           }
                           else
                           {
                               string IsFollowed = FollowPeople_New(FollowUrl, ref objPinUser);

                               if (IsFollowed == "Followed")
                               {
                                   #region AccountReport

                                   string module = "FollowByUsername";
                                   string status = "Followed";
                                   QM.insertAccRePort(objPinUser.Username, module, "", "", FollowUrl, "", "", "", status, "", "", DateTime.Now);
                                   objFollowByUsernameDelegate();

                                   #endregion

                                   GlobusLogHelper.log.Info(" => [ Followed " + FollowUrl + " From " + objPinUser.Username + " ]");
                                   FollowCount++;
                                   CountToday++;
                                   clsSettingDB Db = new clsSettingDB();
                                   Db.insertFollowDate(objPinUser.Username, FollowUrl, "");

                                   try
                                   {
                                       string CSV_Header = "UserName" + "," + "Follow Url" + "," + "Date";
                                       string CSV_Data = objPinUser.Username + "," + "https://www.pinterest.com/" + FollowUrl + "," + System.DateTime.Now.ToString();
                                       string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Follow");
                                       PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\FollowUserFollowers.csv");
                                   }
                                   catch (Exception ex)
                                   {

                                   }
                               }
                               else if (IsFollowed == " ")
                               {

                                   GlobusLogHelper.log.Info(" => [ Already Followed " + FollowUrl + " From " + objPinUser.Username + " ]");
                               }
                               else
                               {
                                   GlobusLogHelper.log.Info(" => [ Could Not Follow " + FollowUrl + " From " + objPinUser.Username + " ]");
                                   Thread.Sleep(1000);
                                   continue;
                               }
                           }
                           if (MaxFollowCount == CountToday)
                           {
                               break;
                           }
                           int Delay = RandomNumberGenerator.GenerateRandom(minDelayFollowByUsername, maxDelayFollowByUsername);
                           GlobusLogHelper.log.Info(" => [ Delay For " + Delay + " Seconds ]");
                           Thread.Sleep(Delay * 1000);
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
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
           }
       }

       public string FindFollow(ref PinInterestUser objPinUser, string UserUrl)
       {
           try
           {

               if (!UserUrl.Contains("pinterest.com"))
               {
                   UserUrl = "http://pinterest.com/" + UserUrl + "/";
               }
               UserUrl = UserUrl + "follow/";

               //string checkalreadyFollowed = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/" + Username.Replace(" ", "") + "/"));
               //if (checkalreadyFollowed.Contains("buttonText\">Unfollow"))
               //{
               //    return "Unfollow";
               //}

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

       public List<string> StartUserScraperMultiThreaded_Follow(string FollowUser, int Pagecount, string followers, ref PinInterestUser objPinUser)
       {
           try
           {
               try
               {
                   lstThreadsFollowByUsername.Add(Thread.CurrentThread);
                   lstThreadsFollowByUsername.Distinct().ToList();
                   Thread.CurrentThread.IsBackground = true;
               }
               catch (Exception ex)
               { };
            

               string FollowUrl = FollowUser;
               int pagecount = Pagecount;
               string followerFollowing = "followers";

               if (followerFollowing.Contains("followers"))
               {
                   lstUsers = GetUserFollowers_new(FollowUrl, pagecount, ref objPinUser);                  
               }
               else
               {
                   lstUsers = GetUserFollowings_new(FollowUrl, pagecount, ref objPinUser);
               }
               lstUsers = lstUsers.Distinct().ToList();
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Debug(" => [ Not Fetched User List ]");              
           }
           return lstUsers;
       }

       public List<string> GetUserFollowers_new(string UserName, int UserFollowerCount, ref PinInterestUser objPinUser)
       {
           try
           {
               int count = 0;
               List<string> Followers = new List<string>();
               List<string>TotalScraperFollowers=new List<string>();
               GlobusLogHelper.log.Info(" => [ Scraping Followers ] ");
               objPinUser.globusHttpHelper = new GlobusHttpHelper();
               string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
               string redirectDomain = GlobusHttpHelper.valueURl.Split('.')[0];
               string newHomePageUrl = redirectDomain + "." + "pinterest.com";
               for (int i = 1; i <= 100; i++)
               {
                   try
                   {
                       string FollowerPageSource = string.Empty;

                       if (i == 1)
                       {
                           FollowUrl = "http://www.pinterest.com/" + UserName + "/followers/";
                           FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(FollowUrl), referer, string.Empty, "");
                           referer = FollowUrl;

                       }
                       else
                       {
                           //FollowUrl = "https://www.pinterest.com/resource/UserFollowersResource/get/?source_url=%2Fashantiluther%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%7D%2C%22context%22%3A%7B%7D%7D&_=" + DateTime.Now.Ticks;
                           FollowUrl = "https://www.pinterest.com/resource/UserFollowersResource/get/?source_url=%2F" + UserName + "%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%2C%22bookmarks%22%3A%5B%22" + bookmark + "%3D%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserProfilePage%3EUserInfoBar(tab%3Dfollowers%2C+spinner%3D%5Bobject+Object%5D%2C+resource%3DUserResource(username%3DSimplyBags%2C+invite_code%3Dnull))&_=";
                         //FollowUrl = "https://in.pinterest.com/resource/UserFollowersResource/get/?source_url=%2Fimaginationtree%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22imaginationtree%22%2C%22bookmarks%22%3A%5B%22Pz9Nakl5TXpvME1qVTROekU0TkRVNU1qRTJPREV6TWpBNk9USXlNek0zTURVM09EYzNPRFkwTXpVd05WOUZ8ZTcyMzU3ZTEwYmU5YmE1MWZhZDQ0MGQwZjM2NmU2YzQwNDUzZWVkYWEyODRlOTg5OTdkNjFiZTBlMzIxNmViOA%3D%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserProfilePage%3EUserInfoBar(tab%3Dfollowers%2C+spinner%3D%5Bobject+Object%5D%2C+resource%3DUserResource(username%3Dimaginationtree%2C+invite_code%3Dnull))&_=1458110524247 HTTP/1.1

                           try
                           {
                               FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), "http://www.pinterest.com/", "", 80, string.Empty, "", "");
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

                               //AppVersion = DataString.Substring(startindex, endindex - startindex).Replace("\": \"", "");
                           }
                           AppVersion = Utils.Utils.getBetween(FollowerPageSource, "app_version\":\"", "\",");
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

                           bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5).Replace("==", "").Replace("\"","");
                       }

                       try
                       {
                           if (!FollowerPageSource.Contains("No one has followed"))
                           {
                               List<string> lst = globusRegex.GetHrefUrlTags(FollowerPageSource);
                               string[] arraydata = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "href=");
                               if (lst.Count == 0)
                               {                               
                                   lst = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "href").ToList();
                               }

                               if (arraydata.Count() == 1)
                               {
                                   arraydata = Regex.Split(FollowerPageSource,"{\"username");
                               }

                               foreach (string item in arraydata)
                               {
                                   if (item.Contains("class=\"userWrapper") || item.Contains("class=\\\"userWrapper"))
                                   {
                                       try
                                       {                                      
                                           if (item.Contains("\\"))
                                           {
                                               int FirstPinPoint = item.IndexOf("\\\"/");
                                               int SecondPinPoint = item.IndexOf("/\\\"");
                                               User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("\\", string.Empty).Replace("=", string.Empty).Replace("/", string.Empty).Trim();
                                           }
                                           else
                                           {
                                               int FirstPinPoint = item.IndexOf("\\\"/");
                                               int SecondPinPoint = item.IndexOf("/\\\"");
                                               User = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("href=", string.Empty).Replace("/", string.Empty).Trim();
                                           }

                                          // GlobusLogHelper.log.Info(" => [ " + User + " ]");
                                           Followers.Add(User);                                       
                                       }
                                       catch (Exception ex)
                                       {
                                           GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                       }
                                   }
                                   else if (item.Contains("[{\"username\":") || item.Contains("[{\"username\":"))
                                   {
                                       try
                                       {
                                           string[] arrayUsername = System.Text.RegularExpressions.Regex.Split(item, "\"username\": ");
                                           arrayUsername = arrayUsername.Skip(1).ToArray();
                                           foreach (string itemUsername in arrayUsername)
                                           {
                                               if (itemUsername.Contains("\"domain_verified\""))
                                               {
                                                   User = Utils.Utils.getBetween(itemUsername, "\"", "\", \"domain_verified\"");
                                                   Followers.Add(User);
                                                   
                                               }
                                           }
                                       }
                                       catch (Exception ex)
                                       {
                                           GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                       }
                                   }
                                   else if (item.Contains("\":"))
                                   {
                                       try
                                       {
                                           User = Utils.Utils.getBetween(item,"\":\"","\",");
                                           Followers.Add(User);
                                       }
                                       catch(Exception ex)
                                       {
                                           GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                       }
                                   }
                               }
                               Followers = Followers.Distinct().ToList();


                               foreach (string lstdata in Followers)
                               {
                                   TotalScraperFollowers.Add(lstdata);
                                   count++;
                                   //GlobusLogHelper.log.Info(lstdata);
                                   //if (MaxFollowCount <= count)
                                   //{
                                   //    break;
                                   //}
                               }

                               TotalScraperFollowers = TotalScraperFollowers.Distinct().ToList();  
                          
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
                           GlobusLogHelper.log.Error("'Error :" + ex.StackTrace);
                       }
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("'Error :" + ex.StackTrace);
                   }
                   //if (MaxFollowCount <= count)
                   //{
                   //    break;
                   //}
               }
               objPinUser.lstUserFollowers = TotalScraperFollowers;
              //GlobusLogHelper.log.Info(" => [ Finished Extracting Followers For " + UserName + " ]");
               //GlobusLogHelper.log.Info(" => [ Process Completed Please. Now you can export file ]");
           }
           catch(Exception ex)
           {
               GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
           }
           GlobusLogHelper.log.Info(" => [ Total  Followers : " + objPinUser.lstUserFollowers.Count + " ]");
          
           return objPinUser.lstUserFollowers;
       }

       public List<string> GetUserFollowings_new(string UserName, int NoOfPage, ref PinInterestUser objPinUser)
       {   
           try
           {
               GlobusLogHelper.log.Info(" => [ Scraping Following ] ");
               objPinUser.globusHttpHelper=new GlobusHttpHelper();
               for (int i = 1; i <= 1000; i++)
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
                           FollowUrl = "https://www.pinterest.com/resource/UserFollowingResource/get/?source_url=%2F" + UserName + "%2Ffollowing%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%2C%22bookmarks%22%3A%5B%22" + bookmark + "%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App(module%3D%5Bobject+Object%5D)&_=1441796236483";

                           try
                           {
                               FollowerPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), referer, "", 80, string.Empty, "", "");
                           }
                           catch(Exception ex)
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
                           try
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
                           catch (Exception ex)
                           {
                               GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                           }
                       }

                       ///get bookmarks value from page 
                       ///
                       if (FollowerPageSource.Contains("bookmarks"))
                       {
                           try
                           {
                               string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "bookmarks");

                               string Datavalue = string.Empty;
                               if (bookmarksDataArr.Count() > 2)
                                   Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 2];
                               else
                                   Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                               bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);
                           }
                           catch (Exception ex)
                           {
                           }
                       }

                       try
                       {
                           if (!FollowerPageSource.Contains("No one has followed"))
                           {
                               try
                               {
                                   List<string> lst = globusRegex.GetHrefUrlTags(FollowerPageSource);
                                   if (lst.Count == 0)
                                   {
                                       //lst = globusRegex.GetHrefUrlTags(FollowerPageSource);
                                       lst = System.Text.RegularExpressions.Regex.Split(FollowerPageSource, "href").ToList();
                                   }
                                   foreach (string item in lst)
                                   {
                                       try
                                       {
                                           if (item.Contains("class=\"userWrapper") || item.Contains("class=\\\"userWrapper"))
                                           {
                                               try
                                               {
                                                   string User = string.Empty;

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

                                                   followings.Add(User);
                                                   GlobusLogHelper.log.Info(" => [ " + User + " ]");                                                
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


                                   followings = followings.Distinct().ToList();
                                   foreach (string lstdata in followings)
                                   {
                                       lstTotalUserScraped.Add(lstdata);

                                   }
                                   lstTotalUserScraped = lstTotalUserScraped.Distinct().ToList();
                                   if (lstTotalUserScraped.Count == NoOfPage)
                                   {
                                       break;
                                   }
                               }
                               catch (Exception)
                               {

                               }
                               // Thread.Sleep(1000);
                           }
                           else
                           {
                               GlobusLogHelper.log.Info(" => [ No following ]");
                               break;
                           }
                       }
                       catch (Exception ex)
                       {
                           GlobusLogHelper.log.Error("'Error :" + ex.StackTrace);
                       }
                   }
                   catch (Exception ex)
                   {
                       GlobusLogHelper.log.Error("'Error :" + ex.StackTrace);
                   }
               }

               //GlobusLogHelper.log.Info(" => [ Finished Extracting following For " + UserName + " ]");
               //GlobusLogHelper.log.Info(" => [ Process Completed Please. Now you can export file ]");
           }
           catch (Exception ex)
           {
               GlobusLogHelper.log.Error("'Error :" + ex.StackTrace);
           }
           GlobusLogHelper.log.Info(" => [ Total  Followings : " + lstTotalUserScraped.Count + " ]");
           return lstTotalUserScraped;
       }

       public string FollowPeople_New(string Username, ref PinInterestUser objPinUser)
       {
           try
           {
               string PostData = string.Empty;
               string AfterFollowPageSource = string.Empty;
               string FollowPageSource = string.Empty;          
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

               string UserUrl = redirectDomain + ".pinterest.com/resource/UserFollowResource/create/";
               string Refrer = "http://www.pinterest.com/" + Username.Replace(" ", "");
            
               string UserPage = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(Refrer), "", "", 80, "", "", "");
          
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

                PostData = "source_url=%2F" + Username.Replace(" ", "") + "%2F&data=%7B%22options%22%3A%7B%22user_id%22%3A%22" + userid + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserProfilePage(resource%3DUserResource(username%3D" + Username.Replace(" ", "") + "%2C+invite_code%3Dnull))%3EUserProfileHeader(resource%3DUserResource(username%3D" + Username.Replace(" ", "") + "%2C+invite_code%3Dnull))%3EUserFollowButton(followed%3Dfalse%2C+is_me%3Dfalse%2C+unfollow_text%3DUnfollow%2C+memo%3D%5Bobject+Object%5D%2C+follow_ga_category%3Duser_follow%2C+unfollow_ga_category%3Duser_unfollow%2C+disabled%3Dfalse%2C+color%3Dprimary%2C+text%3DFollow%2C+user_id%3D" + userid + "%2C+follow_text%3DFollow%2C+follow_class%3Dprimary)";
               try
               {
                  
                    FollowPageSource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri(UserUrl), PostData, newHomePageUrl);
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
               }
               try
               {
                   AfterFollowPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/" + Username.Replace(" ", "") + "/")); //"https://www.pinterest.com/"
               }
               catch (Exception ex)
               {
                   GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
               }

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
