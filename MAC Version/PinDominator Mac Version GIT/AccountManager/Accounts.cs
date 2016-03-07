using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib;
using Globussoft;
using System.Text.RegularExpressions;
using System.Data;
using System.Threading;
using PinDominator;


namespace AccountManager
{
    public delegate void AccountLoad();
    public class Accounts
    {
        public static AccountLoad objDelegateAccountLoad;        

        #region Global Variable

        public string Username { get; set; }
        public string Password { get; set; }
        public string Niches { get; set; }
        public string Name { get; set; }
        public string proxyAddress { get; set; }
        public string proxyPort { get; set; }
        public string proxyUsername { get; set; }
        public string proxyPassword { get; set; }
        public string UserAgent { get; set; }
        public string Token { get; set; }
        public bool LoggedIn { get; set; }
        public string App_version { get; set; }
        public string Followers { get; set; }
        public string Followering { get; set; }
        public string BoardsNames { get; set; }
        public string Screen_Name { get; set; }

        public List<string> Boards = new List<string>();
        public List<string> SourceUrl = new List<string>();
        public static List<string> BoardNames = new List<string>();
        public static List<string> lstBoardNames = new List<string>();
        public static List<string> lstBoardUrls = new List<string>();
        public static List<string> lstBoardId = new List<string>();
        public static bool NextThread = false;

        public static string ScreenName = string.Empty;

        //ProfileManager.ProfileManager ProfileUpdate = new ProfileManager.ProfileManager();

        int intProxyPort = 80;

        //public GlobusHttpHelper httpHelper = new GlobusHttpHelper();
        private GlobusRegex globusRegex = new GlobusRegex();
        public static Events AccounLogEvent = new Events();
        public Events AccounCheckerLogEvent = new Events();
        QueryExecuter QME = new QueryExecuter();

        public static bool inviteStart = true;
           

        #endregion

        
        #region Login

        public bool LoginPinterestAccount(ref PinInterestUser objPinUser)
        {
        StartAgain:
            lock (this)
            {
                string Name = string.Empty;
                try
                {
                    string PinPage = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));
                    string _MainSourcePage = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"));

                    ///Get App Version 
                    if (_MainSourcePage.Contains("app_version"))
                    {
                        try
                        {
                            string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(_MainSourcePage, "app_version");
                            if (ArrAppVersion.Count() > 0)
                            {
                                string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                                int startindex = DataString.IndexOf("\"");
                                string start = DataString.Substring(startindex).Replace("\": \"", "").Replace("}", string.Empty).Replace("\"", string.Empty);
                                int endindex = start.IndexOf(",");

                                App_version = start.Substring(0, endindex);
                                if (!string.IsNullOrEmpty(App_version))
                                {
                                    objPinUser.App_version = App_version;
                                }
                            }
                        }
                        catch { };
                    }
                    else
                    {

                    }
                    string referer = "https://www.pinterest.com/";
                    string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(objPinUser.Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(objPinUser.Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";             
                    string login = string.Empty;
                    try
                    {
                        login = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer);
                    }
                    catch (Exception ex)
                    {
                   
                    }

                    string AfterLoginPageSource = string.Empty;

                    try
                    {
                        AfterLoginPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info(" => [ Trying login again. ]");
                        Thread.Sleep(2 * 1000);
                        goto StartAgain;
                    }

                    if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                    {
                        //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                        this.LoggedIn = true;
                        return true;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;                     
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                    this.LoggedIn = false;
                    return false;
                }
            }

        }

        public bool LoginPinterestAccount1(ref PinInterestUser objPinUser, string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string ss)
        {
            try
            {
                lock (this)
                {
                    GlobusHttpHelper globusHttpHelper=new GlobusHttpHelper();
                    string Name = string.Empty;
                    string ProxyAddress = proxyAddress;
                    string ProxyPort = proxyPort;
                    string ProxyUsername = proxyUsername;
                    string ProxyPassword = proxyPassword;
                    string AfterLoginPageSource = string.Empty;
                    try
                    {

                        string PinPage = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/"), ProxyAddress, 80, proxyUsername, proxyPassword);

                        string _MainSourcePage = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"), proxyAddress, 80, proxyUsername, proxyPassword);
            
                        ///Get App Version 
                        if (PinPage.Contains("app_version"))
                        {
                            try
                            {
                                string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(PinPage, "app_version");
                                if (ArrAppVersion.Count() > 0)
                                {
                                    string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                                    int startindex = DataString.IndexOf("\"");
                                    string start = DataString.Substring(startindex).Replace("\": \"", "").Replace("}", string.Empty).Replace("\"", string.Empty);
                                    int endindex = start.IndexOf(",");

                                    App_version = start.Substring(0, endindex);
                                    if (!string.IsNullOrEmpty(App_version))
                                    {
                                        objPinUser.App_version = App_version;
                                    }
                                }
                            }
                            catch { };
                        }
                        else
                        {
                            ///App version is not available in page source 
                        }


                        string referer = "https://www.pinterest.com/";
                        string login = string.Empty;
                        try
                        {

                            string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";

                            login = globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, proxyAddress, 80, proxyUsername, proxyPassword);
                        }
                        catch { };
                       
                        try
                        {
                            AfterLoginPageSource = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), proxyAddress, 80, proxyUsername, proxyPassword);
                        }
                        catch (Exception ex)
                        {
                        }

                        if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                        {
                            objPinUser.globusHttpHelper = globusHttpHelper;
                            //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                            this.LoggedIn = true;

                        }
                        else
                        {
                            //GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                            this.LoggedIn = false;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;
                        return false;
                    }

                    try
                    {
                        List<string> listFollowersFromDatabse = new List<string>();
                        try
                        {
                            string screen_Name = Getscreen_Name(ref objPinUser);
                            DataSet ds = QME.getFollower(screen_Name);
                         
                            foreach (System.Data.DataRow dRow in ds.Tables[0].Rows)
                            {
                                try
                                {
                                    listFollowersFromDatabse.Add(dRow["FollwerName"].ToString());
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                        catch { }

                        try
                        {                            
                            string screen_Name = Getscreen_Name(ref objPinUser);
                            objPinUser.ScreenName = screen_Name;
                            //Get current followers list from website
                            List<string> FollowersName = GetRefrshFollowerName(screen_Name, ref objPinUser);

                            //FollowersName.RemoveAt(0);

                            if (FollowersName != null)
                            {
                                FollowersName = FollowersName.Distinct().ToList();
                            }
                            if (FollowersName.Contains(screen_Name))
                            {
                                FollowersName.Remove(screen_Name);
                            }
                            //listFollowersFromDatabse.Add("gunde");
                            List<string> listUnfollowers = listFollowersFromDatabse.Except(FollowersName).ToList();

                            //GlobusLogHelper.log.Info(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

                            string UnfollowersList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PInterestUnfollowersList.csv";

                            //write unfollowers list to csv
                            if (!System.IO.File.Exists(UnfollowersList))                                                             //*     CSV Header     *//
                            {
                                try
                                {
                                    string dataFormat = "Account_ScreenName" + "," + "UnfollowerUsername";
                                    GlobusFileHelper.AppendStringToTextfileNewLine(dataFormat, UnfollowersList); //dataFormat
                                }
                                catch (Exception ex) { };
                            }

                            foreach (string unfollower in listUnfollowers)
                            {
                                try
                                {
                                    string dataFormat = screen_Name + "," + unfollower;
                                    GlobusFileHelper.AppendStringToTextfileNewLine(dataFormat, UnfollowersList); //dataFormat

                                    //GlobusLogHelper.log.Info("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                                }
                                catch (Exception ex) 
                                { };

                                try
                                {
                                    QueryExecuter.deleteFollower(screen_Name, unfollower);                                  
                                    //GlobusLogHelper.log.Info("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                                }
                                catch (Exception ex)
                                { }
                            }

                            List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                            //GlobusLogHelper.log.Info(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                            foreach (string follName_item in listNewFollowers)
                            {
                                try
                                {
                                    QueryExecuter.insertFollowerName(screen_Name, follName_item);                                    
                                    //GlobusLogHelper.log.Info("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                                }
                                catch { }
                            }

                            string follower = GetFollowercount(screen_Name, ref objPinUser);
                            string following = GetFollowingCount(screen_Name, ref objPinUser);
                            string BOARDS = GetBoard(screen_Name, ref objPinUser);

                            string followingCount = getBetween(following, "value'>", "</span>");

                            string BoardsName = string.Empty;
                            List<string> BOARDSNAMES = new List<string>();
                            if (inviteStart)
                            {
                                BOARDSNAMES = GetAllBoardNames_new1(screen_Name);
                                foreach (var itemBoardNames in BOARDSNAMES)
                                {
                                    try
                                    {
                                        lstBoardNames.Add(itemBoardNames.ToLower().Replace(" ", "-"));
                                    }
                                    catch(Exception ex)
                                    { };
                                }
                            }
                            inviteStart = true;

                            try
                            {
                                foreach (string item_BoardNames in BOARDSNAMES)
                                {                                
                                    BoardsName += item_BoardNames + (":").ToString();
                                }
                            }
                            catch(Exception ex) 
                            { };

                            try
                            {
                               //// QueryExecuter.updatetb_emails(follower, followingCount, BOARDS, BoardsName, screen_Name, Username);                              
                                //objUploadAccount.AccounLoad();
                                objDelegateAccountLoad();
                            }
                            catch(Exception ex)
                            { };
                        }

                        catch(Exception ex)
                        { };

                        string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                        foreach (var item in ArrData)
                        {
                            try
                            {
                                if (item.Contains("{\"page_info"))
                                {
                                    continue;
                                }
                                if (!item.StartsWith("\": null,") && !item.StartsWith("{\"request_identifier\""))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    break;
                                }
                                if (item.Contains("locale"))// && item.Contains("P.currentUser.set"))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    break;
                                }
                                else if (item.Contains("name\": \"AuthHomePage"))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("\"", string.Empty).Trim();
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                        {
                            try
                            {
                                int startindex = ArrData[1].IndexOf(":");
                                int endindex = ArrData[1].IndexOf("\", \"");

                                this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                            }
                            catch { };
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Name = string.Empty;
                    }
                }
                if (this.LoggedIn)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public bool LoginPinterestAccount1forlee(ref PinInterestUser objPinUser, string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string ss)
        {
            try
            {
                lock (this)
                {
                    GlobusHttpHelper globusHttpHelper = new GlobusHttpHelper();
                    string Name = string.Empty;
                    string ProxyAddress = proxyAddress;
                    string ProxyPort = proxyPort;
                    string ProxyUsername = proxyUsername;
                    string ProxyPassword = proxyPassword;
                    string LoginStatus = string.Empty;

                    string AfterLoginPageSource = string.Empty;
                    try
                    {

                        string PinPage = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/"), ProxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword);

                       /// string _MainSourcePage = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);                   
                        ///Get App Version 
                        if (PinPage.Contains("app_version"))
                        {
                            try
                            {
                                string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(PinPage, "app_version");
                                if (ArrAppVersion.Count() > 0)
                                {
                                    string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                                    int startindex = DataString.IndexOf("\"");
                                    string start = DataString.Substring(startindex).Replace("\": \"", "").Replace("}", string.Empty).Replace("\"", string.Empty);
                                    int endindex = start.IndexOf(",");

                                    App_version = start.Substring(0, endindex);
                                    if (!string.IsNullOrEmpty(App_version))
                                    {
                                        objPinUser.App_version = App_version;
                                    }
                                }
                            }
                            catch { };
                        }
                        else
                        {
                            ///App version is not available in page source 
                        }


                        string referer = "https://www.pinterest.com/";
                        string login = string.Empty;
                        try
                        {                         
                            string PostData1 = "source_url=%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPlainSignupModal%3ESignupForm%3EUserRegister(next%3D%2F%2C+wall_class%3DgrayWall%2C+container%3Dplain_signup_modal%2C+unified_auth%3Dundefined%2C+is_login_form%3Dtrue%2C+show_personalize_field%3Dundefined%2C+auto_follow%3Dundefined%2C+register%3Dtrue)";
                            login = globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                        }
                        catch (Exception ex)
                        { }
                       
                        try
                        {
                            AfterLoginPageSource = globusHttpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                        }
                        catch (Exception ex)
                        { }
                        if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                        {
                           // GlobusLogHelper.log.Info(" => [ Successfully Login for " + Username + " ]");
                            objPinUser.globusHttpHelper = globusHttpHelper;
                            this.LoggedIn = true;
                            objPinUser.isloggedin = true;
                            objPinUser.LoginStatus = "Success";

                        }
                        else
                        {
                            //GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                            this.LoggedIn = false;
                            objPinUser.LoginStatus = "Fail";
                            try
                            {
                                try
                                {
                                    QueryExecuter.updatetb_emails("", "", "", "", objPinUser.ScreenName, Username, objPinUser.LoginStatus);
                                }
                                catch
                                {
                                }
                                //objUploadAccount.AccounLoad();
                                objDelegateAccountLoad();
                            }
                            catch { }
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //GlobusLogHelper.log.Info(" => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;
                        objPinUser.LoginStatus = "Fail";
                        try
                            {
                                try
                                {
                                    QueryExecuter.updatetb_emails("", "", "", "", objPinUser.ScreenName, Username, objPinUser.LoginStatus);
                                }
                                catch
                                {
                                }
                                //objUploadAccount.AccounLoad();
                                objDelegateAccountLoad();
                            }
                            catch { }
                       
                        return false;
                    }

                    try
                    {
                        List<string> listFollowersFromDatabse = new List<string>();
                        try
                        {
                            string screen_Name = Getscreen_Name(ref objPinUser);
                            objPinUser.ScreenName = screen_Name;
                            DataSet ds = QME.getFollower(screen_Name);

                            foreach (System.Data.DataRow dRow in ds.Tables[0].Rows)
                            {
                                try
                                {
                                    listFollowersFromDatabse.Add(dRow["FollwerName"].ToString());
                                }
                                catch { }

                            }
                        }
                        catch { }

                        try
                        {
                            #region old Commented Code
                            ///string screen_Name = Getscreen_Name(ref objPinUser);
                            ///Screen_Name = screen_Name;
                            //Get current followers list from website
                           // List<string> FollowersName = GetRefrshFollowerName(objPinUser.ScreenName, ref objPinUser);

                           // //FollowersName.RemoveAt(0);

                           // if (FollowersName != null)
                           // {
                           //     FollowersName = FollowersName.Distinct().ToList();
                           // }
                           // if (FollowersName.Contains(objPinUser.ScreenName))
                           // {
                           //     FollowersName.Remove(objPinUser.ScreenName);
                           // }
                           // //listFollowersFromDatabse.Add("gunde");
                           // List<string> listUnfollowers = listFollowersFromDatabse.Except(FollowersName).ToList();

                           //// GlobusLogHelper.log.Info(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

                           // string UnfollowersList = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PInterestUnfollowersList.csv";

                           // //write unfollowers list to csv
                           // if (!System.IO.File.Exists(UnfollowersList))                                                             //*     CSV Header     *//
                           // {
                           //     try
                           //     {
                           //         string dataFormat = "Account_ScreenName" + "," + "UnfollowerUsername";
                           //         GlobusFileHelper.AppendStringToTextfileNewLine(dataFormat, UnfollowersList); //dataFormat
                           //     }
                           //     catch (Exception ex) { };
                           // }

                           // foreach (string unfollower in listUnfollowers)
                           // {
                           //     try
                           //     {
                           //         string dataFormat = objPinUser.ScreenName + "," + unfollower;
                           //         GlobusFileHelper.AppendStringToTextfileNewLine(dataFormat, UnfollowersList); //dataFormat

                           //         //GlobusLogHelper.log.Info("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                           //     }
                           //     catch (Exception ex) { };

                           //     try
                           //     {
                           //         QueryExecuter.deleteFollower(objPinUser.ScreenName, unfollower);                                     
                           //         //GlobusLogHelper.log.Info("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                           //     }
                           //     catch { }
                           // }

                           // List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                           //// GlobusLogHelper.log.Info(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                           // foreach (string follName_item in listNewFollowers)
                           // {
                           //     try
                           //     {
                           //         QueryExecuter.insertFollowerName(objPinUser.ScreenName, follName_item);   
                           //         //GlobusLogHelper.log.Info("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                           //     }
                           //     catch { }

                            // }
                            #endregion

                            string follower = GetFollowercount(objPinUser.ScreenName, ref objPinUser);
                            string following = GetFollowingCount(objPinUser.ScreenName, ref objPinUser);
                            //string BOARDS = GetBoard(objPinUser.ScreenName, ref objPinUser);


                            //Globals.followingCountLogin = int.Parse(PinterestAccountManager.getBetween(following, "value'>", "</span>"));

                            string followingCount = getBetween(following, "value'>", "</span>");
                            string BOARDS = string.Empty;
                            string BoardsName = string.Empty;
                            List<string> BOARDSNAMES = new List<string>();
                            if (inviteStart)
                            {
                                BOARDSNAMES = GetAllBoardNames_new1(objPinUser.ScreenName);

                                foreach (var itemBoardNames in BOARDSNAMES)
                                {
                                    lstBoardNames.Add(itemBoardNames.ToLower().Replace(" ", "-"));
                                }
                            }
                            inviteStart = true;
                            

                            try
                            {
                                foreach (string item_BoardNames in BOARDSNAMES)
                                {                             
                                    BoardsName += item_BoardNames + (":").ToString();
                                }
                            }
                            catch { }

                            try
                            {
                                try
                                {

                                    QueryExecuter.updatetb_emails(follower, followingCount, BOARDS, BoardsName, objPinUser.ScreenName, Username, objPinUser.LoginStatus);
                                }
                                catch(Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }
                                //objUploadAccount.AccounLoad();
                                objDelegateAccountLoad();

                            }
                            catch { }
                        }

                        catch { }

                        string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                        foreach (var item in ArrData)
                        {
                            try
                            {
                                if (item.Contains("{\"page_info"))
                                {
                                    continue;
                                }
                                if (!item.StartsWith("\": null,") && !item.StartsWith("{\"request_identifier\""))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    break;
                                }
                                if (item.Contains("locale"))// && item.Contains("P.currentUser.set"))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                                    break;
                                }
                                else if (item.Contains("name\": \"AuthHomePage"))
                                {
                                    int startindex = item.IndexOf(":");
                                    int endindex = item.IndexOf("\", \"");

                                    this.Name = item.Substring(startindex + 1, endindex - startindex).Replace("\"", string.Empty).Trim();
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        try
                        {
                            if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                            {
                                int startindex = ArrData[1].IndexOf(":");
                                int endindex = ArrData[1].IndexOf("\", \"");

                                this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                    catch (Exception ex)
                    {
                        this.Name = string.Empty;
                    }
                }
                if (this.LoggedIn)
                {
                    return true;
                }
            }
            catch (Exception ex)
            { }
            return false;
        }


        public string Getscreen_Name(ref PinInterestUser objPinUser)
        {
            string FindScreenName = string.Empty;
            try
            {
                string ScreenName1 = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"), "", "", "");
                string ScreenName = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");
                int StartIndex = ScreenName.IndexOf("username\":");
                int EndIndex = ScreenName.IndexOf(",");//username": "prabhat18", "email_commen
                FindScreenName = ScreenName.Substring(StartIndex, EndIndex).Replace(":", string.Empty).Replace("email_commen", string.Empty).Replace("\"", "").Replace("username", string.Empty).Replace(",", string.Empty).Replace("email_com", string.Empty).Replace(" ", "@").Trim();
                string[] arr = System.Text.RegularExpressions.Regex.Split(FindScreenName, "@");
                if (arr.Count() == 3)
                {
                    FindScreenName = arr[1];
                }
                else
                {
                    FindScreenName = arr[0];
                }
                return FindScreenName;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
            return FindScreenName;
        }

        public List<string> GetRefrshFollowerName(string screen_Name, ref PinInterestUser objPinUser)
        {
            List<string> FollwrName = new List<string>();
            FollwrName.Clear();
            string followers = string.Empty;
            int val = 0;

            try
            {
                string F_usrer = string.Empty;
                string pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name + "/followers/"), "", "", "");
                try
                {
                    string Usrs = getBetween(pagesource, "UserFollowersResource", "UserFollowersResource");
                    string[] Fnauser = Regex.Split(Usrs, "bookmarks\":");
                    F_usrer = getBetween(Fnauser[1], "[", "\"]}}").Replace("\"", "");

                    string Usrs1 = getBetween(pagesource, "UserFollowersResource", "resource_response");
                    string[] Fnauser1 = Regex.Split(Usrs1, "username");

                    foreach (string item in Fnauser1)
                    {
                        try
                        {
                            string temp = "username" + item;
                            string username = GlobusHttpHelper.ParseEncodedJson(temp, "username").Trim();

                            FollwrName.Add(username);
                        }
                        catch { }

                    }

                }
                catch { };
                while (true)
                {
                   var url = "http://www.pinterest.com/resource/UserFollowersResource/get/?source_url=%2F" + screen_Name + "%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22prabhat101%22%2C%22bookmarks%22%3A%5B%22" + F_usrer + "%22%5D%7D%2C%22context%22%3A%7B%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Afalse%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22layout%22%3A%22fixed_height%22%7D%7D%2C%22render_type%22%3A3%2C%22error_strategy%22%3A1%7D&_=" + GenerateTimeStamp().ToString();

                   string Foolwer_pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", "", "");
                    if (string.IsNullOrEmpty(Foolwer_pagesource))
                    {
                        break;
                    }

                    try
                    {
                        string Usrs = getBetween(Foolwer_pagesource, "UserFollowersResource", "GridItems");
                        string[] Fnauser = Regex.Split(Usrs, "bookmarks\":");
                        F_usrer = getBetween(Fnauser[1], "[", "\"]}}").Replace("\"", "");
                    }
                    catch { }


                    string Usrs1 = getBetween(Foolwer_pagesource, "UserFollowersResource", "resource_response");
                    string[] Fnauser1 = Regex.Split(Usrs1, "username");

                    foreach (string item in Fnauser1)
                    {
                        try
                        {
                            string temp = "username" + item;
                            string username = GlobusHttpHelper.ParseEncodedJson(temp, "username").Trim();

                            FollwrName.Add(username);
                        }
                        catch { }

                    }
                    if (F_usrer.Contains("-end-"))
                    {
                        break;
                    }

                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
            return FollwrName;
        }

        public static string GenerateTimeStamp()
        {
            string strGenerateTimeStamp = string.Empty;
            try
            {
                // Default implementation of UNIX time of the current UTC time
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                strGenerateTimeStamp = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            }
            catch (Exception ex)
            {
            }
            return strGenerateTimeStamp;
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {

                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }

        }

        public string GetFollowercount(string screen_Name, ref PinInterestUser objPinUser)
        {

            string followers = string.Empty;
            try
            {
                string pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int Startindex = pagesource.IndexOf("pinterestapp:followers"); 
                //string _sorce = pagesource.Substring(Startindex).Replace("pinterestapp:followers", "");
                string _sorce = getBetween(pagesource, "2\": \"", " followers\",");
                int Endindex = _sorce.IndexOf(",");
                //_sorce = _sorce.Substring(0, Endindex).Replace("\"", "").Replace(":", "").Replace("FollowingLinks\">\n<", string.Empty).Trim();
                followers = _sorce;
                return followers;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
            return followers;
        }

        public string GetFollowingCount(string screen_Name, ref PinInterestUser objPinUser)
        {
            string following = string.Empty;
            try
            {
                string followingsource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int StartFollowing = followingsource.IndexOf("/following/");
                string _following = followingsource.Substring(StartFollowing).Replace("/following/", "").Trim();
                int endFollowing = _following.IndexOf("</a>");
                _following = _following.Substring(0, endFollowing).Replace("\\n", string.Empty).Replace("\" >", string.Empty).Replace("Following", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                following = _following;
                return following;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
            return following;
        }

        public string GetBoard(string screen_Name, ref PinInterestUser objPinUser)
        {
            string Boards = string.Empty;
            try
            {
                string pagesourceboards = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int StartBoards = pagesourceboards.IndexOf("pinterestapp:boards\":");
                string _boards = pagesourceboards.Substring(StartBoards);
                int endboard = _boards.IndexOf(",");
                _boards = _boards.Substring(0, endboard).Replace("pinterestapp:boards\":", string.Empty).Replace("\"", string.Empty).Trim();
                Boards = _boards;
                return Boards;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
            return Boards;

        }

        public List<string> GetAllBoardNames_new1(string screenName)
        {
            BaseLib.GlobusRegex rgx = new GlobusRegex();
            Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
            //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Getting All Board Names ]");

            string UserUrl = "http://pinterest.com/" + screenName;
            string BoardPage = httpHelper.getHtmlfromUrl(new Uri(UserUrl), "", "", "");

            string[] data = Regex.Split(BoardPage, "is_collaborative");

            foreach (var itemdata in data)
            {
                try
                {
                    string boardUrl = getBetween(itemdata, ", \"url\":", ",").Replace("\"", "").Trim();

                    if (!lstBoardUrls.Contains(boardUrl) && !string.IsNullOrEmpty(boardUrl))
                    {
                        lstBoardUrls.Add(boardUrl);
                    }

                    if (itemdata.Contains("board_id"))
                    {
                        string boardId = getBetween(itemdata, "board_id\":", ",").Replace("\"", "").Trim();
                        if (!lstBoardId.Contains(boardId))
                        {
                            lstBoardId.Add(boardId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                }
            }

            string[] Items = Regex.Split(BoardPage, "item");

            int counter = 0;
            foreach (string item in Items)
            {
                try
                {
                    if (item.Contains("id=\\\"Board") && item.Contains("boardLinkWrapper"))
                    {
                        //if (counter == 1)
                        {
                            string[] Data = System.Text.RegularExpressions.Regex.Split(item, "boardLinkWrapper");

                            foreach (string Dataitem in Data)
                            {                              
                                if (Dataitem.Contains("boardName"))
                                {                                  
                                    string BoardUrl = string.Empty;
                                    int startIndex = Dataitem.IndexOf("title");
                                    int LastPoint = Dataitem.IndexOf("<h2");
                                    string Board = Dataitem.Substring(startIndex, LastPoint).Replace("\\n", string.Empty).Replace("\"", "").Replace("<div class=\\b", string.Empty).Replace("  ", string.Empty).Replace("\"title\"", "").Replace("</div", "");
                                    BoardUrl = rgx.StripTagsRegex(Board);
                                    try
                                    {                                      
                                        Board = getBetween(BoardUrl, ">>", "<");
                                        //modified done
                                        if (Board == "")
                                        {
                                            Board = getBetween(BoardUrl, "title=", ">").Replace("\\", "").Trim();
                                        }
                                    }
                                    catch(Exception ex)
                                    { };
                                    if (!BoardNames.Contains(Board))
                                    {
                                        BoardNames.Add(Board);
                                        //DropDowenBox.Items.Add(Board);

                                    }
                                }
                            }
                        }
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
                }
            }

            return BoardNames;
        }


        public  string Getscreen_NameRepin(ref PinInterestUser objPinUser)
        {
            string djfsdf = string.Empty;
            string FindScreenName = string.Empty;
            try
            {
                string ScreenName1 = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"), "", "", "");
                string ScreenName = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");              
                int StartIndex = ScreenName.IndexOf("username\":");
                int EndIndex = ScreenName.IndexOf(",");//username": "prabhat18", "email_commen
                FindScreenName = ScreenName.Substring(StartIndex, EndIndex).Replace(":", string.Empty).Replace("email_commen", string.Empty).Replace("\"", "").Replace("username", string.Empty).Replace(",", string.Empty).Replace("email_com", string.Empty).Replace(" ", "@").Trim();
                string[] arr = System.Text.RegularExpressions.Regex.Split(FindScreenName, "@");
                if (arr.Count() == 3)
                {
                    FindScreenName = arr[1];
                }
                else
                {
                    FindScreenName = arr[0];
                }
                return FindScreenName;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
            return FindScreenName;
        }



        #endregion





    }
}
