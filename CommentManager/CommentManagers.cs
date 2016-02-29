using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using LikeManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommentManager
{
    public delegate void AccountReport_Comments();

    public delegate List<string> Repin_Comments_UserPins_Repin(string UserName, ref PinInterestUser objPinUser);
    public class CommentManagers
    {
        public static AccountReport_Comments objCommentDelegate;
        public static Repin_Comments_UserPins_Repin objRepin_Comments_UserPins_Repin;
        # region Global variable

        public static int Nothread_Comment = 5;
        public bool isStopComment = false;
        public List<Thread> lstThreadsComment = new List<Thread>();
        public static int minDelayComment = 10;
        public static int maxDelayComment = 20;
        public static int MaxComment = 0;
        public static int NoOfThreadsRunningForCommentobject = 0;
        public static int Commentdata_count = 0;
        public int CommentCount = 0;
        public static readonly object CommentObjThread = new object();
        public bool _IsfevoriteComment = false;
        public bool rdbSingleUserComment = false;
        public bool rdbMultipleUserComment = false;
        public string SingleMsg_Comment = string.Empty;

        string FollowUrl = string.Empty;
        string AppVersion = string.Empty;
        string bookmark = string.Empty;
        string referer = string.Empty;
        string User = string.Empty;  
      
        public int NoOfThreadsComment
        {
            get;
            set;
        }

       
        Accounts ObjAccountManager = new Accounts();
        QueryManager Qm = new QueryManager();
        LikeManagers objLikeManagers = new LikeManagers();
        GlobusRegex objGlobusRegex = new GlobusRegex();
        #endregion

        public void StartComment()
        {
            try
            {
                int numberOfAccountPatchComment = 25;

                if (NoOfThreadsComment > 0)
                {
                    numberOfAccountPatchComment = NoOfThreadsComment;
                }
                Commentdata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchComment);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (NoOfThreadsRunningForCommentobject > Nothread_Comment)
                            {
                                try
                                {
                                    lock (CommentObjThread)
                                    {
                                        Monitor.Wait(CommentObjThread);
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
                                Thread profilerThread = new Thread(StartCommentMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                NoOfThreadsRunningForCommentobject++;

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

        public void StartCommentMultiThreaded(object objparameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopComment)
                {
                    try
                    {
                        lstThreadsComment.Add(Thread.CurrentThread);
                        lstThreadsComment.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objparameters;
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
                                //GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                //StartActionMultiThreadedComment(ref objPinUser);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Debug(" Debug : " + ex.StackTrace);
                            }
                        }
                        if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultiThreadedComment(ref objPinUser);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Debug(" Debug : " + ex.StackTrace);
                            }
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            finally
            {
                try
                {
                    if (NoOfThreadsRunningForCommentobject > Nothread_Comment)
                    {
                        lock (CommentObjThread)
                        {
                            Monitor.Pulse(CommentObjThread);
                        }
                        Commentdata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("  Error : " + ex.StackTrace);
                }
                NoOfThreadsRunningForCommentobject--;

                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
            }
        }
  

        public void StartActionMultiThreadedComment(ref PinInterestUser objPinUser)
        {
            try
            {
                try
                {
                    lstThreadsComment.Add(Thread.CurrentThread);
                    lstThreadsComment.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }
                List<string> lstPinComment = new List<string>();
                //List<string> lstAllPins = objLikeManagers.GetPins(ref objPinUser, MaxComment);

                string userName = objPinUser.ScreenName;
               // userName = screen_Name;

                List<string> lstAllPins = GetUserFollowing_newComment(userName, 1, MaxComment);
                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                List<string> followinglstAllPins = lstAllPins;
             
                foreach (string FollowName in followinglstAllPins)
                {
                    try
                    {
                        Random rnd = new Random();
                        int FollowingNum = rnd.Next(0, followinglstAllPins.Count - 1);
                        string FollowingName = followinglstAllPins[FollowingNum].Trim();

                        List<string> lstRepinPin = objRepin_Comments_UserPins_Repin(FollowingName, ref objPinUser);
                        //checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                        lstPinComment.AddRange(lstRepinPin);
                        lstPinComment = lstPinComment.Distinct().ToList();
                        if (MaxComment < lstPinComment.Count)
                        {
                            break;
                        }
                    }
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error("  Error : " + ex.StackTrace);
                    }
                }
                //Random Pinrnd = new Random();
                
               // ClGlobul.lstPins = lstPinComment.OrderBy(X => Pinrnd.Next()).ToList();

                List<string> TempCommentMessageList = new List<string>();
                TempCommentMessageList.AddRange(ClGlobul.CommentMessagesList);
                int Count = 0;
                #region foreach

                foreach (string Pin in lstPinComment)
                {
                    try
                    {
                        if (MaxComment > Count)
                        {
                            string[] arrCommentList = ClGlobul.CommentMessagesList.ToArray();
                            string Message = string.Empty;

                            foreach (string items in TempCommentMessageList)
                            {
                                int rndNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.CommentMessagesList.Count);
                                Message = arrCommentList[rndNo];
                                GlobusLogHelper.log.Info(" => [ Message : " + Message + " ]");
                                break;
                            }

                            try
                            {

                                //bool IsCommented = pinterestComment.Comment(Pin, Message, ref accountManager);
                                
                                bool IsCommented = Comment_new(ref objPinUser, Pin, Message);
                                if (IsCommented)
                                {
                                    #region AccountReport

                                    string module = "Comment";
                                    string status = "Commented";
                                    Qm.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + Pin, "", "", Message, "", "", status, "", "", DateTime.Now);
                                    objCommentDelegate();

                                    #endregion

                                    GlobusLogHelper.log.Info(" => [ Commented on Pin : " + Pin + " From " + objPinUser.Username + " ]");
                                    string user = PinterestPins.getUserNameFromPinId(Pin, ref objPinUser);
                                    clsSettingDB Databse = new clsSettingDB();
                                    Databse.insertMessageDate(objPinUser.Username, user, Pin.Replace("/", ""), "", Message);

                                    try
                                    {
                                        string CSV_Header = "Date" + "," + "UserName" + "," + "Comment" + "," + "PinUrl";
                                        string CSV_Data = System.DateTime.Now.ToString() + "," + objPinUser.Username + "," + Message + "," + "https://www.pinterest.com/pin/" + Pin;
                                        string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Comment");
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\Comment.csv");
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    Count++;
                                }
                                else if (!IsCommented)
                                {
                                    GlobusLogHelper.log.Info(" => [ Not Commented on Pin : " + Pin + " From " + objPinUser.Username + " ]");
                                }

                                int Delay = RandomNumberGenerator.GenerateRandom(minDelayComment, maxDelayComment);
                                GlobusLogHelper.log.Info(" => [ Delay For " + Delay + " Seconds ]");
                                Thread.Sleep(Delay * 1000);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" => Error : " + ex.StackTrace);
                            }
                         
                        }
                        else if (MaxComment == Count)
                        {
                            ///Count = CommentCount;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                #endregion

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("  Error : " + ex.StackTrace);
            }
        }     

        public  bool Comment_new(ref PinInterestUser objPinUser, string PinId, string Message)
        {
            try
            {
                string full_name = string.Empty;
                string img = string.Empty;
                string CommentPagesource = string.Empty;           
                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                if (Checking.Contains("true, \"full_name\":"))
                {
                    full_name = Utils.Utils.getBetween(Checking, "true, \"full_name\": \"", "\", \"");
                }
                //string ScreenName = ObjAccountManager.Getscreen_NameRepin(ref objPinUser);

                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }

                string redirectDomain = GlobusHttpHelper.valueURl.Split('.')[0];
                string newHomePageUrl = redirectDomain + "." + "pinterest.com";
                Thread.Sleep(10 * 1000);

                string CommentPostData = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22text%22%3A%22" + (Message.Replace(" ", "+")) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ECloseup%3ECloseupContent%3EPin%3EPinCommentsPage%3EPinDescriptionComment(username%3D" + objPinUser.ScreenName + "%2C+show_comment_form%3Dtrue%2C+subtitle%3DThat's+you!%2C+view_type%3Ddetailed%2C+pin_id%3D" + PinId + "%2C+is_description%3Dfalse%2C+content%3Dnull%2C+full_name%3D" + full_name.Replace(" ", "+") + "%2C+image_src%3Dhttps%3A%2F%2Fs-media-cache-ak0.pinimg.com%2Favatars%2F" + objPinUser.ScreenName + ")";

                try
                {
                    string PostComment = redirectDomain + ".pinterest.com/resource/PinCommentResource/create/";
                    CommentPagesource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri(PostComment), CommentPostData, newHomePageUrl);
                }
                catch { };
                string Checking1 = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(newHomePageUrl));

                if (CommentPagesource.Contains(" Turn on Javascript "))
                {
                    GlobusLogHelper.log.Info(" => [ Please Turn on the JavaScript ");
                    GlobusLogHelper.log.Info(" => [ Comment Process Failed For this User " + objPinUser.Username + " ]");
                    return false;
                }
                else if(!CommentPagesource.Contains("<div>Something went wrong!</div>"))
                {
                    try
                    {
                        GlobusLogHelper.log.Info(" => [ Successfully Commented On Url http://pinterest.com/pin/" + PinId + " From " + objPinUser.Username + " ]");
                    }
                    catch (Exception)
                    {

                    }

                    if (CommentPagesource.Contains("comment\", \"id\":"))
                    {
                        string CommentId = Utils.Utils.getBetween(CommentPagesource, "comment\", \"id\":", "}").Replace("\"", "").Trim();
                        GlobusLogHelper.log.Info(" => [ Comment Id : " + CommentId + " ]");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug(" Debug : " + ex.StackTrace);
            }
            return false;
        }

        public List<string> GetUserFollowing_newComment(string UserName, int NoOfPage, int FollowingCount)
        {
            List<string> TotalFollowerComment = new List<string>();
            try
            {            
                List<string> FollowerComment = new List<string>();
                GlobusLogHelper.log.Info(" => [ Starting Extraction Of Following For " + UserName + " ]");
                GlobusHttpHelper objglobusHttpHelper = new GlobusHttpHelper();
                for (int i = 1; i <= 1000; i++)
                {
                    try
                    {
                        string FollowerPageSource = string.Empty;

                        if (i == 1)
                        {
                            FollowUrl = "http://pinterest.com/" + UserName + "/following/";
                            FollowerPageSource = objglobusHttpHelper.getHtmlfromUrl(new Uri(FollowUrl), referer, string.Empty, "");
                            referer = FollowUrl;
                        }
                        else
                        {
                            FollowUrl = "https://www.pinterest.com/resource/UserFollowingResource/get/?source_url=%2F" + UserName + "%2Ffollowing%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22" + UserName + "%22%2C%22bookmarks%22%3A%5B%22" + bookmark + "%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&module_path=App(module%3D%5Bobject+Object%5D)&_=144204352215" + (i - 1);

                            try
                            {
                                FollowerPageSource = objglobusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), referer, "", 80, string.Empty, "", "");
                            }
                            catch
                            {
                                FollowerPageSource = objglobusHttpHelper.getHtmlfromUrlProxy(new Uri(FollowUrl), "", Convert.ToInt32(""), "", "");

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

                                            FollowerComment.Add(User);
                                        
                                            //GlobusLogHelper.log.Info(" => [ " + User + " ]");                                           
                                            if (FollowerComment.Count == FollowingCount)
                                            {
                                                break;
                                            }
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
                                                FollowerComment.Add(User);
                                              
                                                //GlobusLogHelper.log.Info(" => [ " + User + " ]");

                                                if (FollowerComment.Count == FollowingCount)
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


                                FollowerComment = FollowerComment.Distinct().ToList();
                                foreach (string lstdata in FollowerComment)
                                {
                                    TotalFollowerComment.Add(lstdata);

                                }
                                TotalFollowerComment = TotalFollowerComment.Distinct().ToList();
                                //if (TotalFollowerComment.Count == MaxComment)
                                //{
                                //    break;
                                //}
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

                //GlobusLogHelper.log.Info(" => [ Finished Extracting following For " + UserName + " ]");
                //GlobusLogHelper.log.Info(" => [ Process Completed Please. Now you can export file ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            GlobusLogHelper.log.Info(" => [ Total Followings : " + TotalFollowerComment.Count + " ]");

            return TotalFollowerComment;
        }

    }
}
