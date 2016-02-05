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

namespace ManageAccountManager
{
    public delegate void AccountReport_ManageAccount();
    public class ManageAccManager
    {
        public static AccountReport_ManageAccount objManageAccountDelegate;

        # region Global Variable

        public bool isStopManageAcc = false;
        public List<Thread> lstThreadsManageAcc = new List<Thread>();
        public static int countThreadControllerManageAcc = 0;
        public static int ManageAccdata_count = 0;
        public readonly object ManageAccObjThread = new object();
        public bool _IsfevoriteManageAcc = false;
        public int NoOfThreadsManageAcc = 5;
        public bool rdbChangeEmailManageAccount = false;
        public bool rdbChangePwdManageAccount = false;
        public bool rdbChangeScreenNameManageAccount = false;
        public bool RandomEmailChangeManageAccount = false;
        public bool RandomPasswordChangeManageAccount = false;
        public bool RandomScreenNameChangeManageAccount = false;

        string Email = string.Empty;
        string language = string.Empty;
        string Username = string.Empty;
        string gender = string.Empty;
        string firstname = string.Empty;
        string lastname = string.Empty;
        string about = string.Empty;
        string location = string.Empty;
        string website = string.Empty;
        string CsrfMiddleToken = string.Empty;
        string userid = string.Empty;


        Accounts ObjAccManager = new Accounts();
        #endregion
        QueryManager qm = new QueryManager();

        public void StartLoadManageAccount()
        {
            try
            {
                countThreadControllerManageAcc = 0;
                int numberOfAccountPatchManageAcc = 25;

                if (NoOfThreadsManageAcc > 0)
                {
                    numberOfAccountPatchManageAcc = NoOfThreadsManageAcc;
                }

                ManageAccdata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchManageAcc);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerManageAcc > NoOfThreadsManageAcc)
                            {
                                try
                                {
                                    lock (ManageAccObjThread)
                                    {
                                        Monitor.Wait(ManageAccObjThread);
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
                                Thread profilerThread = new Thread(StartManageAccountMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerManageAcc++;
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

        public void StartManageAccountMultiThreaded(object objParameter)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopManageAcc)
                {
                    try
                    {
                        lstThreadsManageAcc.Add(Thread.CurrentThread);
                        lstThreadsManageAcc.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameter;

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
                                // checkLogin = ObjAccManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                checkLogin = ObjAccManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                if (!checkLogin)
                                {
                                    GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com"));
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadManageAccount(ref objPinUser);
                            }
                            catch (Exception ex)
                            { };
                        } 
                        else if(objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadManageAccount(ref objPinUser);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void  StartActionMultithreadManageAccount(ref PinInterestUser objPinUser)
        {
            try
            {
                PinInterestUser objPinChange = objPinUser;
                #region ChangeEmail
                if (rdbChangeEmailManageAccount == true)
                {
                    if (RandomEmailChangeManageAccount == true)
                    {
                        int randomEmail = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstEmailManageAcc.Count);
                        string email = ClGlobul.lstEmailManageAcc[randomEmail];
                        ChangeEmail(email, ref objPinChange);
                    }
                    else
                    {
                        foreach (string item in ClGlobul.lstEmailManageAcc)
                        {
                            Thread thread = new Thread(() => ChangeEmail(item, ref objPinChange));
                            thread.Start();
                            Thread.Sleep(10 * 1000);
                            //ChangeEmail(ref objPinUser);
                        }
                    }
                }
               
                #endregion

                #region ChangePassword
                if (rdbChangePwdManageAccount == true)
                {
                    if (RandomPasswordChangeManageAccount == true)
                    {
                        int randomPwd = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstPwdManageAcc.Count);
                        string Passwordrandom = ClGlobul.lstPwdManageAcc[randomPwd];
                        changePassword(Passwordrandom, ref objPinChange);
                    }
                    else
                    {
                        foreach (string Pwd in ClGlobul.lstPwdManageAcc)
                        {
                            Thread thread = new Thread(() => changePassword(Pwd, ref objPinChange));
                            thread.Start();
                            Thread.Sleep(10 * 1000);
                        }
                    }
                    //changePassword(ref objPinUser);
                }
                #endregion

                #region ChangeScreenName
                if (rdbChangeScreenNameManageAccount == true)
                {
                    if (RandomScreenNameChangeManageAccount == true)
                    {
                        int randomScreen = RandomNumberGenerator.GenerateRandom(0, ClGlobul.lstScreenNameManageAcc.Count);
                        string screen_random = ClGlobul.lstScreenNameManageAcc[randomScreen];
                        changeScreenName(screen_random, ref objPinChange);
                    }
                    else
                    {
                        foreach (string Scrname in ClGlobul.lstScreenNameManageAcc)
                        {
                            Thread thread = new Thread(() => changeScreenName(Scrname, ref objPinChange));
                            thread.Start();
                            Thread.Sleep(10 * 1000);
                        }
                    }
                }
                #endregion

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
            finally
            {
                try
               {

                   if (countThreadControllerManageAcc > NoOfThreadsManageAcc)
                   {
                       lock (ManageAccObjThread)
                       {
                           Monitor.Pulse(ManageAccObjThread);
                       }
                       ManageAccdata_count--;
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

        public void ChangeEmail(string email ,ref PinInterestUser objPinEmail)
        {
            try
            {
                string pagesource = string.Empty;
                string NewPostData = string.Empty;        
                GlobusLogHelper.log.Info(" => [ Starting Email Update ]");
                try
                {

                    string pageSource = objPinEmail.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                    CsrfMiddleToken = Utils.Utils.getBetween(pageSource, "\"csrftoken\": \"", "\", \"");
                    string res_ProfilePage = objPinEmail.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "https://www.pinterest.com/", "", "");
                    userid = getUserid(res_ProfilePage);
                    string Res_settingPagesource = objPinEmail.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/UserSettingsResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%2C%22module%22%3A%7B%22name%22%3A%22UserSettingsPage%22%2C%22options%22%3A%7B%7D%7D%2C%22render_type%22%3A1%2C%22error_strategy%22%3A0%7D&module_path=App%3EUserProfilePage%3EUserProfileHeader%3EDropdownButton%3EDropdown%3EUserDropdown(user_id%3D" + userid + "%2C+user%3D%5Bobject+Object%5D%2C+view_type%3Dself%2C+resource%3DPinfluencerResource())&_=1449724572686 "), "", "", "");
                    string redirect = GlobusHttpHelper.valueURl;
                    string url = Utils.Utils.getBetween(redirect, "", "resource/");
                  
                    firstname = getFirstName(Res_settingPagesource);
                    lastname = getLastName(Res_settingPagesource);
                    Username = getUsername(Res_settingPagesource);
                    location = getLocation(Res_settingPagesource);
                    about = getAbout(Res_settingPagesource).Replace(" ", "+");
                    gender = Utils.Utils.getBetween(Res_settingPagesource, "\"gender\": \"", "\", \"");

                    //Email = email.Replace("@", "%40");                  
                    try
                    {
                        NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22" + Uri.EscapeDataString(email) + "%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22IN%22%2C%22gender%22%3A%22" + gender + "%22%2C%22custom_gender%22%3A%22%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22ads_customize_from_conversion%22%3Atrue%2C%22first_name%22%3A%22" + firstname + "%22%2C%22last_name%22%3A%22" + lastname + "%22%2C%22username%22%3A%22" + Username + "%22%2C%22about%22%3A%22" + about + "%22%2C%22location%22%3A%22%22%2C%22website_url%22%3A%22" + Uri.EscapeDataString(website) + "%22%2C%22filter_settings%22%3A%22anyone%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_education%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22web_push_notification%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings%2C+state_badgeValue%3D%22%22%2C+state_accessibilityText%3D%22%22%2C+state_disabled%3Dtrue)";
                        //NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22Abhita1234%40hotmail.com%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22IN%22%2C%22gender%22%3A%22male%22%2C%22custom_gender%22%3A%22%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22ads_customize_from_conversion%22%3Atrue%2C%22first_name%22%3A%22abhish%22%2C%22last_name%22%3A%22%22%2C%22username%22%3A%22abhita0328%22%2C%22about%22%3A%22%22%2C%22location%22%3A%22%22%2C%22website_url%22%3A%22%22%2C%22filter_settings%22%3A%22anyone%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_education%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22web_push_notification%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings%2C+state_badgeValue%3D%22%22%2C+state_accessibilityText%3D%22%22%2C+state_disabled%3Dtrue)";                      
                        try
                        {
                            pagesource = objPinEmail.globusHttpHelper.postFormDataWithCsrftoken(new Uri(url + "resource/UserSettingsResource/update/"), NewPostData, url, CsrfMiddleToken);
                            //https://in.pinterest.com/resource/UserSettingsResource/update/ 
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                        if (!pagesource.Contains("<div>Uh oh! Something went wrong.</"))
                        {
                            #region AccountReport

                            string module = "ManageAccount";
                            string status = "EmailChanged";

                            qm.insertAccRePort(objPinEmail.Username, module, "", "", "", "", "", "", status, email, "", DateTime.Now);
                            objManageAccountDelegate();

                            #endregion

                            GlobusLogHelper.log.Info(" => [ Email Updated For : " + objPinEmail.Username + " ]");
                            ClGlobul.lstEmailManageAcc.Remove(email);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [ Unable to Update Email : " + objPinEmail.Username + " ]");
                        }
                    }
                    catch (Exception ex)
                    { };
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

        public void changePassword(string newPassword,ref PinInterestUser objPinPwd)
        {
            try
            {
                string pagesourcePwd = string.Empty;
                string NewPostDataPwd = string.Empty;
                GlobusLogHelper.log.Info(" => [ Starting Email Update ]");
                try
                {

                    string pageSource = objPinPwd.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                    CsrfMiddleToken = Utils.Utils.getBetween(pageSource, "\"csrftoken\": \"", "\", \"");
                    string res_ProfilePage = objPinPwd.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "https://www.pinterest.com/", "", "");
                    userid = getUserid(res_ProfilePage);
                    string Res_settingPagesource = objPinPwd.globusHttpHelper.getHtmlfromUrl(new Uri("https://in.pinterest.com/resource/UpdatesResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EShowModalButton(module%3DUserChangePassword)%23App%3EModalManager%3EModal(showCloseModal%3Dtrue%2C+mouseDownInModal%3Dfalse)&_=1449746352521"), "", "", "");
                    string redirect = GlobusHttpHelper.valueURl;
                    string url = Utils.Utils.getBetween(redirect, "", "resource/");

                    try
                    {
                        NewPostDataPwd = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22new_password%22%3A%22" + newPassword + "%22%2C%22new_password_confirm%22%3A%22" + newPassword + "%22%2C%22old_password%22%3A%22" + objPinPwd.Password + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EShowModalButton(module%3DUserChangePassword)%23App%3EModalManager%3EModal(showCloseModal%3Dtrue%2C+mouseDownInModal%3Dfalse)";
                        try
                        {
                            pagesourcePwd = objPinPwd.globusHttpHelper.postFormDataWithCsrftoken(new Uri(url + "resource/UserPasswordResource/update/"), NewPostDataPwd, url, CsrfMiddleToken);
                            //https://in.pinterest.com/resource/UserSettingsResource/update/ 
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                        if (!pagesourcePwd.Contains("<div>Uh oh! Something went wrong.</"))
                        {
                            #region AccountReport

                            string module = "ManageAccount";
                            string status = "PasswordChanged";
                            qm.insertAccRePort(objPinPwd.Username, module, "", "", "", "", "", "", status, "", newPassword, DateTime.Now);
                            objManageAccountDelegate();

                            #endregion

                            GlobusLogHelper.log.Info(" => [ Password Updated For : " + objPinPwd.Username + " ]");
                            ClGlobul.lstPwdManageAcc.Remove(newPassword);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [ Unable to Update Password : " + objPinPwd.Username + " ]");
                        }

                    }
                    catch (Exception ex)
                    { };

                    //"source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22new_password%22%3A%22Abhita123%22%2C%22new_password_confirm%22%3A%22Abhita123%22%2C%22old_password%22%3A%22abhita1234%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EShowModalButton(module%3DUserChangePassword)%23App%3EModalManager%3EModal(showCloseModal%3Dtrue%2C+mouseDownInModal%3Dfalse)";
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

        public void changeScreenName(string newScreenName, ref PinInterestUser objPinScr)
        {
            try
            {
                string pagesourceScr = string.Empty;
                string NewPostDataScr = string.Empty;
                GlobusLogHelper.log.Info(" => [ Starting ScreenName Update ]");
                try
                {

                    string pageSource = objPinScr.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                    CsrfMiddleToken = Utils.Utils.getBetween(pageSource, "\"csrftoken\": \"", "\", \"");
                    string res_ProfilePage = objPinScr.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "https://www.pinterest.com/", "", "");
                    userid = getUserid(res_ProfilePage);
                    string Res_settingPagesource = objPinScr.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/UserSettingsResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%2C%22module%22%3A%7B%22name%22%3A%22UserSettingsPage%22%2C%22options%22%3A%7B%7D%7D%2C%22render_type%22%3A1%2C%22error_strategy%22%3A0%7D&module_path=App%3EUserProfilePage%3EUserProfileHeader%3EDropdownButton%3EDropdown%3EUserDropdown(user_id%3D" + userid + "%2C+user%3D%5Bobject+Object%5D%2C+view_type%3Dself%2C+resource%3DPinfluencerResource())&_=1449724572686 "), "", "", "");
                    string redirect = GlobusHttpHelper.valueURl;
                    string url = Utils.Utils.getBetween(redirect, "", "resource/");

                    firstname = getFirstName(Res_settingPagesource);
                    lastname = getLastName(Res_settingPagesource);
                    Username = getUsername(Res_settingPagesource);
                    location = getLocation(Res_settingPagesource);
                    about = getAbout(Res_settingPagesource).Replace(" ", "+");
                    gender = Utils.Utils.getBetween(Res_settingPagesource, "\"gender\": \"", "\", \"");

                    //Email = email.Replace("@", "%40");                  
                    try
                    {
                        NewPostDataScr = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22" + Uri.EscapeDataString(objPinScr.Username) + "%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22IN%22%2C%22gender%22%3A%22" + gender + "%22%2C%22custom_gender%22%3A%22%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22ads_customize_from_conversion%22%3Atrue%2C%22first_name%22%3A%22" + firstname + "%22%2C%22last_name%22%3A%22" + lastname + "%22%2C%22username%22%3A%22" + newScreenName + "%22%2C%22about%22%3A%22" + about + "%22%2C%22location%22%3A%22%22%2C%22website_url%22%3A%22" + Uri.EscapeDataString(website) + "%22%2C%22filter_settings%22%3A%22anyone%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_education%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22web_push_notification%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings%2C+state_badgeValue%3D%22%22%2C+state_accessibilityText%3D%22%22%2C+state_disabled%3Dtrue)";
                        //NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22Abhita1234%40hotmail.com%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22IN%22%2C%22gender%22%3A%22male%22%2C%22custom_gender%22%3A%22%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22ads_customize_from_conversion%22%3Atrue%2C%22first_name%22%3A%22abhish%22%2C%22last_name%22%3A%22%22%2C%22username%22%3A%22abhita0328%22%2C%22about%22%3A%22%22%2C%22location%22%3A%22%22%2C%22website_url%22%3A%22%22%2C%22filter_settings%22%3A%22anyone%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_education%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22web_push_notification%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserSettingsPage%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings%2C+state_badgeValue%3D%22%22%2C+state_accessibilityText%3D%22%22%2C+state_disabled%3Dtrue)";

                        try
                        {
                            pagesourceScr = objPinScr.globusHttpHelper.postFormDataWithCsrftoken(new Uri(url + "resource/UserSettingsResource/update/"), NewPostDataScr, url, CsrfMiddleToken);
                            //https://in.pinterest.com/resource/UserSettingsResource/update/ 
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                        if (!pagesourceScr.Contains("<div>Uh oh! Something went wrong.</"))
                        {
                            GlobusLogHelper.log.Info(" => [ ScreenName Updated For : " + objPinScr.Username + " ]");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [ Unable to Update ScreenName : " + objPinScr.Username + " ]");
                        }

                    }
                    catch (Exception ex)
                    { };
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

        public string getUserid(string pagesource)
        {
            string userid = string.Empty;
            try
            {
                int startindex = pagesource.IndexOf("\"user_id\": \"");
                string start = pagesource.Substring(startindex).Replace("\"user_id\": \"", "");
                int endindex = start.IndexOf("\"}},");
                string end = start.Substring(0, endindex);
                userid = end;
            }
            catch (Exception ex)
            {

            }
            return userid;
        }

        public string getAbout(string pagesource)
        {
            string about = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("<textarea");
                string Start = pagesource.Substring(StartIndex).Replace("<textarea name=\\\"about\\\" id=\\\"userAbout\\\">", "");
                int EndIndex = Start.IndexOf("</textarea>");
                string End = Start.Substring(0, EndIndex).Replace(">", "");
                about = End;
            }
            catch (Exception ex)
            {

            }
            return about;
        }

        public string getUsername(string pageSource)
        {
            string Username = string.Empty;
            try
            {
                int startIndex = pageSource.IndexOf("name=\\\"username\\\"");
                string Start = pageSource.Substring(startIndex).Replace("value=\\\"", "");
                int endIndex = Start.IndexOf(" />");
                string End = Start.Substring(0, endIndex).Replace(" type=\\\"text\\\"", string.Empty).Replace("value", "").Replace(" ", "").Replace("'", "").Replace("=", "").Replace("\"", "")
                    .Replace("name\\username\\", "").Replace("class\\username\\", "").Replace("\\n", "").Replace("id\\userUserName", "").Replace("\\", string.Empty);

                Username = End;
            }
            catch (Exception ex)
            {

            }
            return Username;
        }

        public string getFirstName(string pagesource)
        {
            string Firstname = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("id=\\\"userFirstName\\\"");
                string Start = pagesource.Substring(StartIndex).Replace("id=\\\"userFirstName\\\"", "");
                int EndIndex = Start.IndexOf("\\\">\\n");
                string End = Start.Substring(0, EndIndex).Replace(" type=\"text\"", string.Empty).Replace("value=", "").Replace("\\n", "").Replace("\\", "").Replace("\"", "").Replace(" ", "");
                Firstname = End;
            }
            catch (Exception ex)
            {

            }
            return Firstname;
        }

        public string getLastName(string pagesource)
        {
            string lastname = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("name=\\\"last_name\\\"");
                string Start = pagesource.Substring(StartIndex).Replace("name=\\\"last_name\\\"", "");
                int EndIndex = Start.IndexOf("\\\">\\n");
                string End = Start.Substring(0, EndIndex).Replace(" type=\"text\"", string.Empty).Replace("value=", "").Replace("\"", "").Replace(" ", "")
                    .Replace("id=\\userLastName", string.Empty).Replace("\\n", string.Empty).Replace("\\", string.Empty);
                lastname = End;
            }
            catch (Exception ex)
            {

            }
            return lastname;
        }

        public string getLocation(string pagesource)
        {
            string location = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("id=\\\"userLocation\\\"");
                string Start = pagesource.Substring(StartIndex).Replace("id=\\\"userLocation\\\"", "");
                int EndIndex = Start.IndexOf("\\\" />\\n");
                string End = Start.Substring(0, EndIndex).Replace("value=\\\"", "").Replace("\"", "").Replace(" ", "");
                location = End;
            }
            catch (Exception ex)
            {

            }
            return location;
        }

        public string getWebsite(string pagesource)
        {
            string website = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("id=\\\"userWebsite\\\"");
                string Start = pagesource.Substring(StartIndex).Replace("id=\\\"userWebsite\\\"", "");
                int EndIndex = Start.IndexOf("\\\">");
                string End = Start.Substring(0, EndIndex).Replace("\\", "").Replace("value=", "").Replace("\"", "");
                website = End;
            }
            catch (Exception ex)
            {

            }
            return website;
        }

    }
}
