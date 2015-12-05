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
    public class CommentManagers
    {
        # region Global variable

        public static int Nothread_Comment = 5;
        public bool isStopComment = false;
        public List<Thread> lstThreadsComment = new List<Thread>();
        public static int minDelayComment = 10;
        public static int maxDelayComment = 20;
        public static int MaxComment = 0;
        public static int NoOfThreadsRunningForCommentobject = 0;
        public static int Commentdata_count = 0;
        public static int CommentCount = 0;
        public static readonly object CommentObjThread = new object();
        public bool _IsfevoriteComment = false;

        public int NoOfThreadsComment
        {
            get;
            set;
        }

       
        Accounts ObjAccountManager = new Accounts();        
        
        LikeManagers objLikeManagers = new LikeManagers();

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
            finally
            {
                GlobusLogHelper.log.Info(" => [ Comment Process Finished ]");
                GlobusLogHelper.log.Info(" [ PROCESS COMPLETED ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
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
                        if(!objPinUser.isloggedin)
                        {

                            GlobusLogHelper.log.Info(" => [ Logging In With : " + objPinUser.Username + " ]");                          
                            bool checkLogin;
                            try
                            {
                                checkLogin = ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                                if (!checkLogin)
                                {
                                    checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                    if (!checkLogin)
                                    {
                                        GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Debug(" Debug : " + ex.StackTrace);
                            }
                        }
                        GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                        #endregion

                        StartActionMultiThreadedComment(ref objPinUser);

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

                if (MaxComment <= CommentCount)
                {
                    GlobusLogHelper.log.Info("[ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                    GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
                }
            }
        }

        public void StartActionMultiThreadedComment(ref PinInterestUser objPinUser)
        {
            try
            {
                List<string> lstAllPins = objLikeManagers.GetPins(ref objPinUser, MaxComment);
                Random Pinrnd = new Random();
                ClGlobul.lstPins = lstAllPins.OrderBy(X => Pinrnd.Next()).ToList();

                List<string> TempCommentMessageList = new List<string>();
                TempCommentMessageList.AddRange(ClGlobul.CommentMessagesList);

                #region foreach

                foreach (string Pin in ClGlobul.lstPins)
                {
                    try
                    {
                        if (MaxComment > CommentCount)
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
                            CommentCount++;
                        }
                        else
                        {
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
                string ScreenName = ObjAccountManager.Getscreen_NameRepin(ref objPinUser);

                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }


                Thread.Sleep(10 * 1000);

                string CommentPostData = "source_url=%2Fpin%2F" + PinId + "%2F&data=%7B%22options%22%3A%7B%22pin_id%22%3A%22" + PinId + "%22%2C%22text%22%3A%22" + (Message.Replace(" ", "+")) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ECloseup%3ECloseupContent%3EPin%3EPinCommentsPage%3EPinDescriptionComment(username%3D" + ScreenName + "%2C+show_comment_form%3Dtrue%2C+subtitle%3DThat's+you!%2C+view_type%3Ddetailed%2C+pin_id%3D" + PinId + "%2C+is_description%3Dfalse%2C+content%3Dnull%2C+full_name%3D" + full_name.Replace(" ", "+") + "%2C+image_src%3Dhttps%3A%2F%2Fs-media-cache-ak0.pinimg.com%2Favatars%2F" + ScreenName + ")";

                try
                {
                    CommentPagesource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/PinCommentResource/create/"), CommentPostData, "https://www.pinterest.com/");
                }
                catch { };
                string Checking1 = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                if (CommentPagesource.Contains(" Turn on Javascript "))
                {
                    GlobusLogHelper.log.Info(" => [ Please Turn on the JavaScript ");
                    GlobusLogHelper.log.Info(" => [ Comment Process Failed For this User " + objPinUser.Username + " ]");
                    return false;
                }
                else
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
        


    }
}
