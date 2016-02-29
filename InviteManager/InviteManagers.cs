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

namespace InviteManager
{
    public delegate void AccountReport_Invite();
    public class InviteManagers
    {
        public static AccountReport_Invite objInviteDelegate;

        #region Global Variable

        public  int Nothread_Invite = 5;
        public  bool isStopInvite = false;
        public List<Thread> lstThreadsInvite = new List<Thread>();
        public static int countThreadControllerInvite= 0;   
        public static int Invitedata_count = 0;      
        public static readonly object InviteObjThread = new object();
        public bool _IsfevoriteInvite = false;
        public bool rdbSingleUserInvite = false;
        public bool rdbMultipleUserInvite = false;
        public string SingleUsername_Invite = string.Empty;

        string InvitePostData = string.Empty;
        string InvitedPageSource = string.Empty;

        public int minDelayInvite
        {
            get;
            set;
        }

        public int maxDelayInvite
        {
            get;
            set;
        }

        public int NoOfThreadsInvite
        {
            get;
            set;
        }

        Accounts ObjAccountManager = new Accounts();      
              

        #endregion

        QueryManager qm = new QueryManager();
        public void StartInvite()
        {
            try
            {
                countThreadControllerInvite = 0;
                int numberOfAccountPatchInvite = 25;

                if (NoOfThreadsInvite > 0)
                {
                    numberOfAccountPatchInvite = NoOfThreadsInvite;
                }

                Invitedata_count = PDGlobals.loadedAccountsDictionary.Count();
                List<List<string>> list_listAccounts = new List<List<string>>();

                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchInvite);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerInvite > Nothread_Invite)
                            {
                                try
                                {
                                    lock (InviteObjThread)
                                    {
                                        Monitor.Wait(InviteObjThread);
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
                                Thread profilerThread = new Thread(StartInviteMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerInvite++;
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

        public void StartInviteMultiThreaded(object objparameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
                Accounts Obj_AccountManager = new Accounts();

                if (!isStopInvite)
                {
                    try
                    {
                        lstThreadsInvite.Add(Thread.CurrentThread);
                        lstThreadsInvite.Distinct().ToList();
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
                                string checklogin = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                //GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                //StartActionMultithreadInvite(ref objPinUser);
                            }
                            catch { };
                        }
                        if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadInvite(ref objPinUser);
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            finally
            {
                try
                {
                    if (countThreadControllerInvite > Nothread_Invite)
                    {
                        lock (InviteObjThread)
                        {
                            Monitor.Pulse(InviteObjThread);
                        }
                        Invitedata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerInvite--;

                //if (Invitedata_count == 0)  //|| DivideByUserinput < 0)
                //{

                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");

                //}

            }

        }

        public  void StartActionMultithreadInvite(ref PinInterestUser objPinUser)
        {
            try
            {
                try
                {
                    lstThreadsInvite.Add(Thread.CurrentThread);
                    lstThreadsInvite.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };
                foreach (var item_lstEmailInvites in ClGlobul.lstEmailInvites)
                {
                    try
                    {
                        Invite(item_lstEmailInvites, ref objPinUser);                  
                        GlobusFileHelper.AppendStringToTextfileNewLine(item_lstEmailInvites, PDGlobals.InviteSentPath);
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info(" => [ Invite Failed For This Email " + item_lstEmailInvites + " ]");
                        
                    }

                    int DelayTime = RandomNumberGenerator.GenerateRandom(minDelayInvite, maxDelayInvite);
                    GlobusLogHelper.log.Info(" => [ Delay For " + DelayTime + " Seconds ]");
                    Thread.Sleep(DelayTime);

                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void Invite(string Email, ref PinInterestUser objPinUser)
        {
            try
            {
                string email = Email.Replace("@", "%").Replace("hotmail", "40163");

                //InvitePostData = "source_url=%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22" + email + "%22%2C%22type%22%3A%22email%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EHomePage%3EAuthHomePage%3EGrid%3EGridItems%3EUserNews(anchored%3Dtrue)%23App%3EModalManager%3EModal(can_dismiss%3Dtrue%2C+only_modal%3Dtrue)";

                InvitePostData = "source_url=%2Ffind_friends%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22" + email + "%22%2C%22type%22%3A%22email%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EFriendCenter%3EDiscoverFriends%3EInviteTiles%3EDiscoveryTile%3EInvitePanelEmail%3ESocialTypeaheadField(bypass_maxheight%3Dtrue%2C+tags%3Dinvitability%2C+class_name%3DsearchBox%2C+prefetch_on_focus%3Dtrue%2C+prevent_default_on_enter%3Dtrue%2C+bypass_lru_cache%3Dtrue%2C+placeholder%3DEnter+name+or+email%2C+preserve_typeahead%3Dfalse%2C+allow_email%3Dtrue%2C+allowed_social_networks%3Dfacebook%2Cgplus%2Cyahoo%2C+name%3Dname%2C+view_type%3DuserCircleSelect%2C+autocomplete%3Doff%2C+template%3Duser_circle_avatar%2C+search_delay%3D0%2C+close_on_selection%3Dtrue%2C+autofocus%3Dfalse)";
                try
                {
                    InvitedPageSource = objPinUser.globusHttpHelper.postFormData(new Uri("https://www.pinterest.com/resource/EmailInviteSentResource/create/"), InvitePostData, "https://www.pinterest.com/", objPinUser.Token, objPinUser.UserAgent);

                    if (InvitedPageSource.Contains("error\": null"))
                    {
                        #region AccountReport

                        string module = "Invite";
                        string status = "Invitation Sent";
                        qm.insertAccRePort(objPinUser.Username, module, "", "", "", Email, "", "", status, "", "", DateTime.Now);
                        objInviteDelegate();

                        #endregion

                        GlobusLogHelper.log.Info(" => [ " + objPinUser.Username + " Successfully invited to  " + Email + " ]");
                    }
                    else
                    {
                        if (InvitedPageSource.Contains("Failed to send invite"))
                        {
                            GlobusLogHelper.log.Info(" =>(" + objPinUser.Username + "Failed to send invite. Try again after some time " + Email);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" =>(" + objPinUser.Username + "Invitation Process Failed " + Email);
                        }

                    }
                }
                catch { };
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }




    }





}
