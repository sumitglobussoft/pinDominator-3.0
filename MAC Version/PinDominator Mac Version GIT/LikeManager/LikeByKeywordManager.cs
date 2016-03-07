using AccountManager;
using BaseLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PinDominator;

namespace LikeManager
{
    public delegate void AccountReport_LikeByKeyword();
	public delegate void LikeByKeywordAccount(int AccCount);
    public class LikeByKeywordManager
    {
        public static AccountReport_LikeByKeyword objLikeByKeywordDelegate;
		public static LikeByKeywordAccount objDelegateNoOfActiveAcc_LikeByKeyword;
		public static LikeByKeywordAccount objDelegateNoOfDeadAcc_LikeByKeyword;
        #region Global Variable

        public int Nothread_LikeByKeyword = 5;
        public bool isStopLikeByKeyword = false;
        public List<Thread> lstThreadsLikeByKeyword = new List<Thread>();
        public static int countThreadControllerLikeByKeyword = 0;
        public static int LikeByKeyworddata_count = 0;
        public int MaxLikeByKeyword = 0;
        public readonly object LikeByKeywordObjThread = new object();
        public List<string> lstLike = new List<string>();
        public string Keyword = string.Empty;
        public bool _IsfevoriteLikeByKeyword = false;

        public bool rdbSingleUserLikeByKeyword = false;
        public bool rdbMultipleUserLikeByKeyword = false;
        public string LikeKeyword_CommentByKeyword = string.Empty;

        public static bool chkDivideDataLikeByKeyword = false;
        public static bool rdbDivideEquallyLikeByKeyword = false;
        public static bool rdbDivideGivenByUserLikeByKeyword = false;
        public static int CountGivenByUserLikeByKeyword = 0;
        List<List<string>> list_lstTargetLikeByKeyword = new List<List<string>>();
        List<string> list_lstTargetLikeByKeyword_item = new List<string>();
        int LstCounterLikeByKeyword = 0;

        string keyword = string.Empty;
        string comment = string.Empty;
        List<string> lstUserPins = new List<string>();
        string UserUrl = string.Empty;
        string UserPins = string.Empty;
        string bookmark = string.Empty;
        string UserPageSource = string.Empty;

        public int minDelayLikeByKeyword
        {
            get;
            set;
        }

        public int maxDelayLikeByKeyword
        {
            get;
            set;
        }

        public int NoOfThreadsLikeByKeyword
        {
            get;
            set;
        }

		int ActiveAccCount = 0;
		int DeadAccCount = 0;

        Accounts ObjAccountManager = new Accounts();


        #endregion
        LikeManagers objLikeManagers = new LikeManagers();
        QueryManager QM = new QueryManager();

        public void StartLikeKeyword()
        {
            try
            {
                countThreadControllerLikeByKeyword = 0;
                int numberOfAccountPatchCommentByKeyword = 25;

                if (NoOfThreadsLikeByKeyword > 0)
                {
                    numberOfAccountPatchCommentByKeyword = NoOfThreadsLikeByKeyword;
                }
                LikeByKeyworddata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();           

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchCommentByKeyword);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerLikeByKeyword > Nothread_LikeByKeyword)
                            {
                                try
                                {
                                    lock (LikeByKeywordObjThread)
                                    {
                                        Monitor.Wait(LikeByKeywordObjThread);
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
                                Thread profilerThread = new Thread(StartLikeByKeywordMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerLikeByKeyword++;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : 2" + ex.StackTrace);
            }
        }

        public void StartLikeByKeywordMultiThreaded(object objparameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {

                if (!isStopLikeByKeyword)
                {
                    try
                    {
                        lstThreadsLikeByKeyword.Add(Thread.CurrentThread);
                        lstThreadsLikeByKeyword.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objparameters;
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
									objDelegateNoOfDeadAcc_LikeByKeyword(DeadAccCount);
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
							}
                            catch { };
                        }
                        if (objPinUser.isloggedin == true)
                        {
                            try
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logged In With : " + objPinUser.Username + " ]");
								ActiveAccCount++;
								objDelegateNoOfActiveAcc_LikeByKeyword(ActiveAccCount);
                                StartActionMultithreadLikeByKeyword(ref objPinUser);
                            }
                            catch (Exception ex)
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
			      
        public void StartActionMultithreadLikeByKeyword(ref PinInterestUser objPinUser)
        {
            try
            {
                try
                {
                    lstThreadsLikeByKeyword.Add(Thread.CurrentThread);
                    lstThreadsLikeByKeyword.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };

                int counter = 0;
                string[] arrayItem = new string[100];
                foreach (string newItem in ClGlobul.lstLikeByKeyword)
                {
                    try
                    {
                        arrayItem = Regex.Split(newItem, "::");
                        if (arrayItem.Length == 2 && arrayItem[0] == objPinUser.Niches)
                        {
                            if (arrayItem.Length == 2)
                            {
                                string[] Keywordarrray = Regex.Split(arrayItem[1], ",");
                                foreach (string KeywordsItem in Keywordarrray)
                                {
                                    lock (this)
                                    {
                                        try
                                        {                                          
                                            lstLike.Add(KeywordsItem);
                                            List<string> LstPins = new List<string>();
                                            LstPins = KeywordPins_New(KeywordsItem, MaxLikeByKeyword, ref objPinUser);
                                            LstPins = LstPins.Distinct().ToList();                                       
											GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Finding Pins On Keyword : " + KeywordsItem + " For " + objPinUser.Username + " ]");
											GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ " + LstPins.Count + " Pins On Keyword :" + KeywordsItem + " For " + objPinUser.Username + " ]");
                                            string[] lstofPinLike = null;
                                            lstofPinLike = LstPins.ToArray();
                                      
                                            #region foreach
                                            foreach (string Pin in lstofPinLike)
                                            {
                                                if (MaxLikeByKeyword > counter)
                                                {
                                                    try
                                                    {
                                                        DataSet dt = DataBaseHandler.SelectQuery("SELECT * FROM LikeUsingUrl Where LikeUrl = '" + Pin + "' and UserName = '" + objPinUser.Username + "' ", "LikeUsingUrl");
                                                        int count_NO_RoWs = dt.Tables[0].Rows.Count;
                                                        if (count_NO_RoWs == 0)
                                                        {
                                                            bool IsLiked = objLikeManagers.Like_New(ref objPinUser, Pin);

                                                            if (IsLiked)
                                                            {
                                                                #region AccountReport

                                                                string module = "LikeByKeyword";
                                                                string status = "Liked";
                                                                QM.insertAccRePort(objPinUser.Username, module, "https://www.pinterest.com/pin/" + Pin, "", "", "", KeywordsItem, "", status, "", "", DateTime.Now);
                                                                objLikeByKeywordDelegate();

                                                                #endregion
                                                                counter++;
																	GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Liked : " + Pin + " From " + objPinUser.Username + " ]");

                                                                try
                                                                {
                                                                    if (!string.IsNullOrEmpty(Pin) && !string.IsNullOrEmpty(objPinUser.Username))
                                                                    {
                                                                        string query = "INSERT INTO  LikeUsingUrl (LikeUrl,UserName) VALUES ('" + Pin + "' ,'" + objPinUser.Username + "') ";
                                                                        DataBaseHandler.InsertQuery(query, "LikeUsingUrl");
                                                                    }

                                                                    try
                                                                    {
                                                                        string CSV_Header = "Username" + "," + "Pin" + "," + "" + "Date";
                                                                        string CSV_Data = objPinUser.Username + "," + "https://www.pinterest.com/pin/" + Pin + "/" + "," + System.DateTime.Now.ToString();
                                                                        string path = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "Like");
                                                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, path + "\\LikeByKeyword.csv");
                                                                    }
                                                                    catch (Exception ex)
                                                                    {

                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                                }
                                                            }
                                                            else
                                                            {
																	GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Not Liked : " + Pin + " From " + objPinUser.Username + " ]");
                                                            }
                                                            int Delay = RandomNumberGenerator.GenerateRandom(minDelayLikeByKeyword, maxDelayLikeByKeyword);
															GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay For " + Delay + " Seconds ]");
                                                            Thread.Sleep(Delay * 1000);
                                                        }
                                                        else
                                                        {
																GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Already Liked : " + Pin + " From " + objPinUser.Username + " ]");
                                                           	    int Delay = RandomNumberGenerator.GenerateRandom(minDelayLikeByKeyword, maxDelayLikeByKeyword);
																GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay For " + Delay + " Seconds ]");
                                                            	Thread.Sleep(Delay * 1000);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                                                    }
                                                   
                                                }
                                                else
                                                {
                                                    break;
                                                }                                              

                                            }
                                            #endregion
                                            if (MaxLikeByKeyword == counter)
                                            {
													GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                                                	GlobusLogHelper.log.Info("-----------------------------------------------------------------------------------");
                                            }
                                        }
                                       catch (Exception ex)
                                        {
                                          GlobusLogHelper.log.Error(" Error : 3.4" + ex.StackTrace);
                                     }
                                  }
                               }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error : 3.4" + ex.StackTrace);
                    }                 
                }
				GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            finally
            {
                try
                {
                    if (countThreadControllerLikeByKeyword > Nothread_LikeByKeyword)
                    {
                        lock (LikeByKeywordObjThread)
                        {
                            Monitor.Pulse(LikeByKeywordObjThread);
                        }
                        LikeByKeyworddata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerLikeByKeyword--;
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
						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Start Getting Pins For this User " + keyword + " ]");

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
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Sorry No More Pages ]");
                        }


                        lstUserPins = lstUserPins.Distinct().ToList();
                       // lstUserPins.Reverse();
                    }

						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Pin Urls Collected " + lstUserPins.Count + " ]");
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            return lstUserPins;
        }

        

    }
}
