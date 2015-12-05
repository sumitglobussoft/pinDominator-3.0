using AccountManager;
using BaseLib;
using BasePD;
using LikeManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CommentManager
{
    public class CommentByKeywordManager
    {
        #region Global Variable

        public  int Nothread_CommentByKeyword = 5;
        public bool isStopCommentByKeyword = false;
        public List<Thread> lstThreadsCommentByKeyword = new List<Thread>();             
        public static int countThreadControllerCommentByKeyword = 0;     
        public static int CommentByKeyworddata_count = 0;
        public int MaxCommentByKeyword = 0;
        public readonly object CommentByKeywordObjThread = new object();
        public List<string> lstCommnet = new List<string>();
        public string Keyword = string.Empty;
        public bool _IsfevoriteCommentByKeyword = false;

        public static bool chkDivideDataCommentByKeyword = false;
        public static bool rdbDivideEquallyCommentByKeyword = false;
        public static bool rdbDivideGivenByUserCommentByKeyword = false;
        public static int CountGivenByUserCommentByKeyword = 0;
        List<List<string>> list_lstTargetCommentByKeyword = new List<List<string>>();
        List<string> list_lstTargetCommentByKeyword_item = new List<string>();
        int LstCounterCommentByKeyword = 0;
        

        string keyword = string.Empty;
        string comment = string.Empty;
        List<string> lstUserPins = new List<string>();
        string UserUrl = string.Empty;
        string UserPins = string.Empty;
        string bookmark = string.Empty;
        string UserPageSource = string.Empty;


        QueryManager QM = new QueryManager();
    

        public int minDelayCommentByKeyword
        {
            get;
            set;
        }

        public int maxDelayCommentByKeyword
        {
            get;
            set;
        }

        public int NoOfThreadsCommentByKeyword
        {
            get;
            set;
        }

        CommentManagers objCommentManagers = new CommentManagers();
        Accounts ObjAccountManager = new Accounts();
       

        #endregion

        public void StartCommentKeyword()
        {
            try
            {
                countThreadControllerCommentByKeyword = 0;          
                int numberOfAccountPatchCommentByKeyword = 25;

                if (NoOfThreadsCommentByKeyword > 0)
                {
                    numberOfAccountPatchCommentByKeyword = NoOfThreadsCommentByKeyword;
                }
                CommentByKeyworddata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();

                #region Divide Data Setting Comment
                //if (chkDivideDataCommentByKeyword == true)
                //{
                //    try
                //    {
                //        if (rdbDivideEquallyCommentByKeyword == true || rdbDivideGivenByUserCommentByKeyword == true)
                //        {
                //            int splitNo = 0;
                //            if (rdbDivideEquallyCommentByKeyword == true)
                //            {
                //                splitNo = ClGlobul.lstMessageKeyword.Count / PDGlobals.listAccounts.Count;
                //            }
                //            else if (rdbDivideGivenByUserCommentByKeyword == true)
                //            {
                //                if (Convert.ToInt32(CountGivenByUserCommentByKeyword) != 0)
                //                {
                //                    int res = Convert.ToInt32(Convert.ToInt32(CountGivenByUserCommentByKeyword));
                //                    splitNo = res;
                //                }
                //            }
                //            if (splitNo == 0)
                //            {
                //                splitNo = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstMessageKeyword.Count - 1);
                //            }
                //            list_lstTargetCommentByKeyword = Split(ClGlobul.lstMessageKeyword, splitNo);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        GlobusLogHelper.log.Error("Error : 3.1 " + ex.StackTrace);
                //    }
                //}

                #endregion

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchCommentByKeyword);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerCommentByKeyword > Nothread_CommentByKeyword)
                            {
                                try
                                {
                                    lock (CommentByKeywordObjThread)
                                    {
                                        Monitor.Wait(CommentByKeywordObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    GlobusLogHelper.log.Error(" Error : 3.2" + Ex.StackTrace);
                                }
                            }

                            string acc = account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {
                                Thread profilerThread = new Thread(StartCommentByKeywordMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerCommentByKeyword++;
                            }
                        }

                    }
                }           
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : 2" + ex.StackTrace);
            }
          
        }

        public void StartCommentByKeywordMultiThreaded(object objparameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {

                if (!isStopCommentByKeyword)
                {
                    try
                    {
                        lstThreadsCommentByKeyword.Add(Thread.CurrentThread);
                        lstThreadsCommentByKeyword.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objparameters;
                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                        try
                        {
                            if (chkDivideDataCommentByKeyword == true)
                            {
                                list_lstTargetCommentByKeyword_item = list_lstTargetCommentByKeyword[LstCounterCommentByKeyword];
                            }
                            else
                            {
                                list_lstTargetCommentByKeyword_item = ClGlobul.lstMessageKeyword;
                            }

                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error : 3.3" + ex.StackTrace);
                        }

                        #region Login

                        if (!objPinUser.isloggedin)
                        {
                            //Obj_AccountManager.httpHelper = httpHelper;
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
                            }

                            catch { };
                        }
                        #endregion

                        GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                        StartActionMultithreadCommentByKeyword(ref objPinUser, list_lstTargetCommentByKeyword_item);

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
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
                    if (countThreadControllerCommentByKeyword > Nothread_CommentByKeyword)
                    {
                        lock (CommentByKeywordObjThread)
                        {
                            Monitor.Pulse(CommentByKeywordObjThread);
                        }
                        CommentByKeyworddata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerCommentByKeyword--;

                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");   
            }
        }

        public void StartActionMultithreadCommentByKeyword(ref PinInterestUser objPinUser, List<string> UserCount_CommentByKeyword)
        {
            try
            {

                int counter = 0;
                string[] arrayItem = new string[100];
                foreach (string newItem in UserCount_CommentByKeyword)
                {
                    try
                    {
                        arrayItem = Regex.Split(newItem, "::");
                        if (arrayItem.Length == 3 && arrayItem[0] == objPinUser.Niches)
                        {
                            if (arrayItem.Length == 3)
                            {
                                string[] Keywordarrray = Regex.Split(arrayItem[1], ",");
                                foreach (string KeywordsItem in Keywordarrray)
                                {
                                    Keyword = KeywordsItem + "::" + arrayItem[2].ToString();
                                    lstCommnet.Add(Keyword);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error : 3.4" + ex.StackTrace);
                    }
                }
                string[] lstCommnet_strlist = lstCommnet.ToArray();
                foreach (string FindKeyword in lstCommnet_strlist)
                {
                    try
                    {                                              
                        List<string> LstPins = KeywordPins_New(FindKeyword, MaxCommentByKeyword, ref objPinUser);
                        //PinterestComments Comments = new PinterestComments();                     
                        keyword = Utils.Utils.getBetween(FindKeyword, "", "::");

                        foreach (string pin in LstPins)
                        {
                            clsSettingDB Db = new clsSettingDB();
                            string user = PinterestPins.getUserNameFromPinId(pin, ref objPinUser);

                            if (counter >= MaxCommentByKeyword)
                            {                              
                                break;
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(" => [ Finding Pins On Keyword : " + FindKeyword + " For " + objPinUser.Username + " ]");
                                GlobusLogHelper.log.Info(" => [ " + LstPins.Count + " Pins On Keyword :" + FindKeyword + " For " + objPinUser.Username + " ]");
                                comment = lstCommnet[RandomNumberGenerator.GenerateRandom(0, lstCommnet.Count)];
                                if (comment.Contains("::"))
                                {
                                    comment = Regex.Split(comment, "::")[1];
                                }
                                else if (comment.Contains(":"))
                                {
                                    if (comment.Contains("http:"))
                                    {
                                        comment = comment.Split(':')[comment.Split(':').Count() - 1];
                                        comment = "http:" + comment.Split(':')[comment.Split(':').Count() - 1];
                                    }
                                    else
                                    {
                                        comment = comment.Split(':')[comment.Split(':').Count() - 1];
                                    }
                                }

                                #region                           
                                try
                                {
                                    DataSet DS = QM.selectCommentedPinDetails(objPinUser.Username, pin);                                   
                                    DataTable Dt = DS.Tables[0];
                                                            
                                    string dateTime = Dt.Rows[Dt.Rows.Count - 1].ItemArray[3].ToString();
                                    DateTime previousDateTime = Convert.ToDateTime(dateTime);
                                    DateTime currentDate = DateTime.Today;
                                    TimeSpan dt_Difference = currentDate.Subtract(previousDateTime);
                                    double dt_Difference1 = dt_Difference.Days;
                                    if (dt_Difference1 < 30)
                                    {
                                        continue;
                                    }

                                }
                                catch (Exception Ex)
                                {
                                    //CommentAddToLogger("Error ==> " + Ex.StackTrace);
                                }

                                #endregion

                                bool IsCommented = objCommentManagers.Comment_new(ref objPinUser, pin, comment);
                                if (IsCommented)
                                {
                                    GlobusLogHelper.log.Info(" => [ Commneted on Pin : " + pin + " From " + objPinUser.Username + " ]");
                                    clsSettingDB Databse = new clsSettingDB();
                                    Databse.insertMessageDate(objPinUser.Username, user.Replace("/", ""), pin, keyword, comment);
                                    try
                                    {
                                        QueryManager.insertCommentedPinDetails(objPinUser.Username, pin, DateTime.Now.ToString());                                 
                                    }
                                    catch { };
                                    counter++;
                                    
                                    try
                                    {
                                        string CSV_Header = "Date" + "," + "UserName" + "," + "Comment" + "," + "Keyword" + "," + "Niche" + "," + "PinUrl";
                                        string CSV_Data = System.DateTime.Now.ToString() + "," + objPinUser.Username + "," + comment + "," + keyword + "," + objPinUser.Niches + "," + "https://www.pinterest.com/pin/" + pin;
                                        string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Comment");
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\CommentBykeyword.csv");
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    GlobusLogHelper.log.Info(" => [ Not Commneted on Pin : " + pin + " From " + objPinUser.Username + " ]");
                                }

                                //if (rdbDivideGivenByUserCommentByKeyword == true)
                                //{
                                //    CountGivenByUserCommentByKeyword--;
                                //    if (CountGivenByUserCommentByKeyword < 0)
                                //    {
                                //        break;
                                //    }
                                //}

                                int delay = RandomNumberGenerator.GenerateRandom(minDelayCommentByKeyword, maxDelayCommentByKeyword);
                                GlobusLogHelper.log.Info(" => [ Delay For " + delay + " Seconds ]");
                                Thread.Sleep(delay * 1000);
                            }
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
        }

        public List<string> KeywordPins_New(string keyword, int Count, ref PinInterestUser objPinUser)
        {
            try
            {
                // Test 
                string[] Keyword = Regex.Split(keyword, ":");
                foreach (string item in Keyword)
                {
                    keyword = item;
                    break;
                }
                //
                try
                {
                    GlobusLogHelper.log.Info(" => [ Start Getting Pins For this User " + keyword + " ]");

                    for (int i = 0; i <= Count; i++)
                    {
                        try
                        {
                            if (i == 0)
                            {
                                UserUrl = "http://pinterest.com/search/?q=" + keyword;
                            }
                            else
                            {
                                //http://pinterest.com/resource/SearchResource/get/?source_url=%2Fsearch%2Fpins%2F%3Fq%3Dhairstyle&data=%7B%22options%22%3A%7B%22query%22%3A%22hairstyle%22%2C%22bookmarks%22%3A%5B%22b28xMDB8MDQ0NWZiOTBjNzNiODlkOTQ1ZTk3ZjY0ZTBhYjU0YjM0ZDYyNDg3NjU3ZWQ3OGJmZjI4ZTliZGRmODBlMzJlNQ%3D%3D%22%5D%2C%22show_scope_selector%22%3Atrue%2C%22scope%22%3A%22pins%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22fc93456%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A2%7D&_=1375699543906
                                UserUrl = "http://pinterest.com/resource/SearchResource/get/?source_url=%2Fsearch%2Fpins%2F%3Fq%3D" + keyword + "&data=%7B%22options%22%3A%7B%22query%22%3A%22" + keyword + "%22%2C%22bookmarks%22%3A%5B%22" + Uri.EscapeDataString(bookmark) + "%22%5D%2C%22show_scope_selector%22%3Atrue%2C%22scope%22%3A%22pins%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A" + i + "%7D&_=" + DateTime.Now.Ticks;
                            }

                            try
                            {
                                UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserUrl), "http://pinterest.com/", string.Empty, 80, "", "", objPinUser.UserAgent);
                            }
                            catch
                            {
                                UserPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserUrl), objPinUser.ProxyAddress, Convert.ToInt32(objPinUser.ProxyPort), objPinUser.ProxyUsername, objPinUser.ProxyPassword);
                            }
                            ///get bookmarks value from page 
                            ///
                            if (UserPageSource.Contains("bookmarks"))
                            {
                                string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(UserPageSource, "bookmarks");

                                string Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                                bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);

                            }

                            List<string> lst = System.Text.RegularExpressions.Regex.Split(UserPageSource, "pin_id").ToList();
                            List<string> templst = new List<string>();
                            foreach (string item in lst)
                            {
                                if (!item.StartsWith("\": \"") || item.Contains("$") || item.Contains("?{pin}"))
                                {
                                    continue;
                                }

                                if (item.Contains("id\": \"pinItButton"))// && item.Contains("/repins/"))
                                {
                                    try
                                    {
                                        int FirstPinPoint = item.IndexOf("\": \"");
                                        int SecondPinPoint = item.IndexOf("}, ");
                                        if (SecondPinPoint > 30)
                                        {
                                            SecondPinPoint = item.IndexOf("\", ") + 1;
                                        }
                                        string Pinid = item.Substring(FirstPinPoint + 4, SecondPinPoint - FirstPinPoint - 5).Trim();

                                        if (!lstUserPins.Any(pid => pid == Pinid))
                                        {
                                            lstUserPins.Add(Pinid);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info(" => [ Sorry No More Pages ]");
                        }


                        lstUserPins = lstUserPins.Distinct().ToList();
                        lstUserPins.Reverse();
                    }

                    GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstUserPins.Count + " ]");
                }
                catch (Exception ex)
                {

                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info(" Error :" + ex.StackTrace);
            }

            return lstUserPins;
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
