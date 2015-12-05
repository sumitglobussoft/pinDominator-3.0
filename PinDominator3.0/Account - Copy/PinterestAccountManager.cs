using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;
using Globussoft;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.IO;

namespace PinDominator.Account
{
    public class PinterestAccountManager
    {
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



        //public List<string> PinUrls = new List<string>();
        public List<string> Boards = new List<string>();
        public List<string> SourceUrl = new List<string>();
        public static List<string> BoardNames = new List<string>();
        public static List<string> lstBoardNames = new List<string>();
        public static List<string> lstBoardUrls = new List<string>();
        public static List<string> lstBoardId = new List<string>();
        public static bool NextThread = false;

        public static string ScreenName = string.Empty;

        

        ProfileManager ProfileUpdate = new ProfileManager();
        
        int intProxyPort = 80;

      public GlobusHttpHelper httpHelper = new GlobusHttpHelper();
      //Globussoft.GlobusHttpHelper httpHelper = new Globussoft.GlobusHttpHelper();
        private GlobusRegex globusRegex = new GlobusRegex();

        public static Events AccounLogEvent = new Events();
        public Events AccounCheckerLogEvent = new Events();

        public PinterestAccountManager(string Username, string Password, string niches, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string UserAgent, string Followers, string Followering, string BoardsName, string boardname, string Screen_Name)
        {

            this.Username = Username;
            this.Password = Password;
            this.Niches = niches;
            this.proxyAddress = proxyAddress;
            this.proxyPort = proxyPort;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;
            this.UserAgent = UserAgent;
            this.Followers = Followers;
            this.Followering = Followering;
            this.BoardsNames = BoardsName;
            this.Screen_Name = Screen_Name;

        }

        public PinterestAccountManager()
        {

        }

        #region commented code
        //public void Login()
        //{
        //    try
        //    {
        //        Log("Start Login Process for " + Username);



        //        if (ApplicationData.ValidateNumber(this.proxyPort))
        //        {
        //            intProxyPort = int.Parse(this.proxyPort);
        //        }

        //        string MainPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://pinterest.com/"), "", this.proxyAddress, 8888, this.proxyUsername, this.proxyPassword, this.UserAgent);

        //        string LoginPageBeforeSource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/login/?next=/"), string.Empty, string.Empty, this.UserAgent);
        //        string LoginPageSource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/login/?next=/login/"), string.Empty, string.Empty, this.UserAgent);

        //        int FirstPointToken = LoginPageSource.IndexOf("csrfmiddlewaretoken");
        //        string FirstTokenSubString = LoginPageSource.Substring(FirstPointToken);

        //        int SecondPointToken = FirstTokenSubString.IndexOf("/>");
        //        this.Token = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("'", string.Empty).Trim();
        //        string _ch = string.Empty;
        //        try
        //        {
        //            int ChTokenStart = LoginPageSource.IndexOf("name='_ch'");
        //            string CHTokenStart = LoginPageSource.Substring(ChTokenStart).Replace("name='_ch'", "");
        //            int ChTokenEnd = CHTokenStart.IndexOf("/>");
        //            string CHtokenEnd = CHTokenStart.Substring(0, ChTokenEnd).Replace("value='", "").Replace("'", "");
        //            _ch = CHtokenEnd.Trim();
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        //email=gargimishra%40globussoft.com&password=globussoft&next=%2Fasorosil27490%2F&csrfmiddlewaretoken=8813c50cda37fe9fe864ec25697d6e5a&_ch=pg2dwfkf
        //        string LoginPostData = "email=" + Uri.EscapeDataString(Username) + "&password=" + Password + "&next=%2Flogin%2F&csrfmiddlewaretoken=" + Token + "&_ch=" + _ch;//"email=" + this.Username + "&password=" + this.Password + "&next=%2F&csrfmiddlewaretoken=" + Token + "&_ch=" + _ch;

        //        string LoggedInPageSource = httpHelper.postFormData(new Uri("https://pinterest.com/login/?next=/login/"), LoginPostData, "https://pinterest.com/login/?next=%2Flogin%2F", string.Empty, this.UserAgent);

        //        string AfterLoginPageSource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/login/?next=/"), string.Empty, string.Empty, this.UserAgent);




        //        if (AfterLoginPageSource.Contains("Logout"))
        //        {
        //            Log("Successfully Login for " + Username);
        //            this.LoggedIn = true;
        //        }
        //        else
        //        {
        //            Log("Login Failed for " + Username);
        //            this.LoggedIn = false;
        //        }


        //        //Log("Start Getting PIN for " + Username);


        //        try
        //        {
        //            int FirstPointName = AfterLoginPageSource.IndexOf("id=\"UserNav\"");
        //            string FirstNameSubString = AfterLoginPageSource.Substring(FirstPointName);

        //            int SecondPointName = FirstNameSubString.IndexOf("id=\"UserNav\"");
        //            int ThirdPointName = FirstNameSubString.IndexOf("class=\"nav\"");

        //            this.Name = FirstNameSubString.Substring(SecondPointName, ThirdPointName - SecondPointName).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Name = string.Empty;
        //        }


        //        //List<string> lst = globusRegex.GetHrefUrlTags(AfterLoginPageSource);

        //        //foreach (string item in lst)
        //        //{
        //        //    if (item.Contains("/pin/") && !item.Contains("edit"))
        //        //    {
        //        //        try
        //        //        {
        //        //            int FirstPinPoint = item.IndexOf("/pin/");
        //        //            int SecondPinPoint = item.IndexOf("class=");

        //        //            string PinUrl = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("pin", string.Empty).Replace("/", string.Empty).Trim();

        //        //            PinUrls.Add(PinUrl);
        //        //        }
        //        //        catch (Exception ex)
        //        //        {
        //        //            Log("Failed to Login for this User " + Username + ex.Message);
        //        //        }

        //        //    }
        //        //}

        //        //PinUrls = PinUrls.Distinct().ToList();
        //        //PinUrls.Reverse();

        //        //Log("Total Pin Urls Collected " + PinUrls.Count);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Failed to Login for this User " + Username + ex.Message);
        //    }
        //} 
        #endregion

        #region Clear Proxy Data
        /// <summary>
        /// To Clear proxy data related with account
        /// </summary>
        public void clearProxyDataFromAccount()
        {
            proxyAddress = string.Empty;
            proxyPort = string.Empty;
            proxyUsername = string.Empty;
            proxyPassword = string.Empty;
        }
        #endregion


        #region New Login Method
        public bool LoginPinterestAccount(ref GlobusHttpHelper httpHelper)
        {
            lock (this)
            {
                string Name = string.Empty;
                try
                {
                    string PinPage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));
                    string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"));


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
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        ///App version is not available in page source 
                    }


                    string referer = "https://www.pinterest.com/";


                    string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";
                    //PostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22abhay%22%2C%22last_name%22%3A%22mahavar%22%2C%22username%22%3A%22"++"%22%2C%22about%22%3A%22hi+i+am+software+Developer%22%2C%22location%22%3A%22kanpur%22%2C%22website_url%22%3A%22http%3A%2F%2Fwww.scriptnut.com%2F%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22b1da8fc%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";
                    string login = httpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer);

                    string AfterLoginPageSource = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));

                    if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                    {
                        Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                        this.LoggedIn = true;

                        //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Successfully Login for " + Username + " ]");
                        //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Successfully Login for " + Username + " ]");
                        //this.LoggedIn = true;
                        return true;
                    }
                    else
                    {
                        Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;

                        //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                        //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                        //this.LoggedIn = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                    this.LoggedIn = false;

                    //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                    //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                    //GlobusLogHelper.log.Error(ex.Message);
                    return false;
                }
            }

        }


          public bool LoginPinterestAccount1forlee(ref GlobusHttpHelper httpHelper, string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword,string ss)
        {
            lock (this)
            {
                string Name = string.Empty;
                string ProxyAddress = proxyAddress;
                string ProxyPort = proxyPort;
                string ProxyUsername = proxyUsername;
                string ProxyPassword = proxyPassword;
                string AfterLoginPageSource = string.Empty;
                try
                {
                    //string PinPage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));
                    string PinPage = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/"), ProxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"));
                    string _MainSourcePage = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Faction%3Dlogin%26next%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26referrer%3Dhome_page&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EHomePage%3EUnauthLoggedOutHomePage%3ELoginButton(class_name%3DloggedOutHomePageLogin%2C+text%3DLog+in%2C+element_type%3Da)&_=1437811789975"));

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
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        ///App version is not available in page source 
                    }

                    string referer = "https://www.pinterest.com/";

                    //string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";
                    string PostData1 = "source_url=%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EModalManager%3EModal%3EPlainSignupModal%3ESignupForm%3EUserRegister(next%3D%2F%2C+wall_class%3DgrayWall%2C+container%3Dplain_signup_modal%2C+unified_auth%3Dundefined%2C+is_login_form%3Dtrue%2C+show_personalize_field%3Dundefined%2C+auto_follow%3Dundefined%2C+register%3Dtrue)";


                    //string login = httpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer);
                    string login = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                   //  string CheckLogin = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, ProxyAddress, int.Parse(ProxyUsername), ProxyPassword, ProxyPort);

                     //AfterLoginPageSource = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                     AfterLoginPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                    if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                    {
                        Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                        this.LoggedIn = true;

                    }
                    else
                    {
                        Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                    this.LoggedIn = false;

                    return false;
                }


                try
                {

                 //   string CheckWlcomeUrl = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));


                    List<string> listFollowersFromDatabse = new List<string>();
                    try
                    {
                        string screen_Name = Getscreen_Name();
                        string selectquery = "Select * from  tb_FollowerName where UserName='" + screen_Name + "'";
                        System.Data.DataSet ds = DataBaseHandler.SelectQuery(selectquery, "tb_FollowerName");

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
                        FrmEmailProxyAssigner obj = new FrmEmailProxyAssigner();

                        string screen_Name = Getscreen_Name();
                        Screen_Name = screen_Name;
                        //Get current followers list from website
                        List<string> FollowersName = GetRefrshFollowerName(screen_Name);

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

                        LogForFollow(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

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

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                            }
                            catch (Exception ex) { };

                            try
                            {
                                string query = "DELETE FROM  tb_FollowerName where UserName='" + screen_Name + "' and FollwerName='" + unfollower + "'";
                                DataBaseHandler.DeleteQuery(query, "tb_FollowerName");

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                            }
                            catch { }
                        }

                        List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                        LogForFollow(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                        foreach (string follName_item in listNewFollowers)
                        {
                            try
                            {
                                string query = "INSERT INTO  tb_FollowerName (UserName,FollwerName) VALUES ('" + screen_Name + "' ,'" + follName_item + "') ";
                                DataBaseHandler.InsertQuery(query, "tb_FollowerName");

                                LogForFollow("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                            }
                            catch { }

                        }

                        string follower = GetFollowercount(screen_Name);
                        string following = GetFollowingCount(screen_Name);
                        string BOARDS = GetBoard(screen_Name);


                        string followingCount = PinterestAccountManager.getBetween(following, "value'>", "</span>");



                        string BoardsName = string.Empty;
                        List<string> BOARDSNAMES = new List<string>();
                        if (frmMain.inviteStart)
                        {
                            BOARDSNAMES = obj.GetAllBoardNames_new1(screen_Name);

                            foreach (var itemBoardNames in BOARDSNAMES)
                            {
                                lstBoardNames.Add(itemBoardNames.ToLower().Replace(" ", "-"));
                            }
                        }
                        frmMain.inviteStart = true;

                        //string BOARDSNAME = GetBoardsNAME(screen_Name);

                        try
                        {
                            foreach (string item_BoardNames in BOARDSNAMES)
                            {
                                //string insertBoard = "INSERT INTO tb_BoardName(UserName,BoardsName)VALUES('" + Username + "','" + item_BoardNames + "')";
                                // DataBaseHandler.InsertQuery(insertBoard, "tb_BoardName");

                                BoardsName += item_BoardNames + (":").ToString();
                            }
                        }
                        catch { }

                        try
                        {
                            string UpdateQuery = "Update tb_emails set Follower = '" + follower + "',Following = '" + followingCount + "',BOARDS= '" + BOARDS + "',BOARDSNAME='" + BoardsName + "',ScreenName='" + screen_Name + "' where UserName = '" + Username + "'";
                            DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
                            
                            new FrmEmailProxyAssigner().LoadDataGrid();
                            new FrmEmailProxyAssigner().makeReadOnlyDataGrid();

                        }
                        catch { }
                    }

                    catch { }

                    string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                    foreach (var item in ArrData)
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
                    if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                    {
                        int startindex = ArrData[1].IndexOf(":");
                        int endindex = ArrData[1].IndexOf("\", \"");

                        this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
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
            return false;
        }
        #endregion


        public bool LoginPinterestAccount1(ref GlobusHttpHelper httpHelper, string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword,string ss)
        {
            lock (this)
            {
                string Name = string.Empty;
                string ProxyAddress = proxyAddress;
                string ProxyPort = proxyPort;
                string ProxyUsername = proxyUsername;
                string ProxyPassword = proxyPassword;
                string AfterLoginPageSource = string.Empty;
                try
                {
                    //string PinPage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));
                    string PinPage = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/"), ProxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"));
                    string _MainSourcePage = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EHomePage()%3EUnauthHomePage(signup_email%3Dnull%2C+tab%3Dfollowing%2C+cmp%3Dnull%2C+resource%3DInspiredWallResource())&_=1424169081757"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);
                    //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/NoopResource/get/?source_url=%2Flogin%2F%3Faction%3Dlogin%26next%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26referrer%3Dhome_page&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EHomePage%3EUnauthLoggedOutHomePage%3ELoginButton(class_name%3DloggedOutHomePageLogin%2C+text%3DLog+in%2C+element_type%3Da)&_=1437811789975"));

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
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        ///App version is not available in page source 
                    }


                    string referer = "https://www.pinterest.com/";

                    string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";
                    //string PostData1 = "source_url=%2Flogin%2F%3Fnext%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + "%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(text%3DLog+In%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";
                    //PostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22abhay%22%2C%22last_name%22%3A%22mahavar%22%2C%22username%22%3A%22"++"%22%2C%22about%22%3A%22hi+i+am+software+Developer%22%2C%22location%22%3A%22kanpur%22%2C%22website_url%22%3A%22http%3A%2F%2Fwww.scriptnut.com%2F%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22b1da8fc%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";

                    //string PostData1 = "source_url=%2Flogin%2F%3Faction%3Dlogin%26next%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26prev%3Dhttps%253A%252F%252Fwww.pinterest.com%252F%26referrer%3Dhome_page&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Uri.EscapeDataString(Password) + " %22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3ELoginPage%3ELogin%3EButton(text%3DLog+in%2C+size%3Dlarge%2C+class_name%3Dprimary%2C+type%3Dsubmit)";


                    //string login = httpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer);
                    string login = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                   //  string CheckLogin = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData1, referer, ProxyAddress, int.Parse(ProxyUsername), ProxyPassword, ProxyPort);

                     //AfterLoginPageSource = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                     AfterLoginPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), proxyAddress, Convert.ToInt32(proxyPort), proxyUsername, proxyPassword);

                    if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder") || AfterLoginPageSource.Contains("header1\": \"What are you interested in?") || AfterLoginPageSource.Contains("\"error\": null") || login.Contains("\"error\": null"))
                    {
                        Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                        this.LoggedIn = true;

                        //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Successfully Login for " + Username + " ]");
                        //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Successfully Login for " + Username + " ]");
                        //this.LoggedIn = true;
                        //return true;
                    }
                    else
                    {
                        Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;

                        //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                        //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                        //this.LoggedIn = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                    this.LoggedIn = false;

                    //GlobusLogHelper.log.Debug("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                    //GlobusLogHelper.log.Info("[ Social Sites => Pinterest => Account Creator => [ Login Failed for " + Username + " ]");
                    //GlobusLogHelper.log.Error(ex.Message);
                    return false;
                }


                try
                {

                 //   string CheckWlcomeUrl = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));


                    List<string> listFollowersFromDatabse = new List<string>();
                    try
                    {
                        string screen_Name = Getscreen_Name();
                        string selectquery = "Select * from  tb_FollowerName where UserName='" + screen_Name + "'";
                        System.Data.DataSet ds = DataBaseHandler.SelectQuery(selectquery, "tb_FollowerName");

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
                        FrmEmailProxyAssigner obj = new FrmEmailProxyAssigner();

                        string screen_Name = Getscreen_Name();
                        Screen_Name = screen_Name;
                        //Get current followers list from website
                        List<string> FollowersName = GetRefrshFollowerName(screen_Name);

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

                        LogForFollow(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

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

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                            }
                            catch (Exception ex) { };

                            try
                            {
                                string query = "DELETE FROM  tb_FollowerName where UserName='" + screen_Name + "' and FollwerName='" + unfollower + "'";
                                DataBaseHandler.DeleteQuery(query, "tb_FollowerName");

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                            }
                            catch { }
                        }

                        List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                        LogForFollow(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                        foreach (string follName_item in listNewFollowers)
                        {
                            try
                            {
                                string query = "INSERT INTO  tb_FollowerName (UserName,FollwerName) VALUES ('" + screen_Name + "' ,'" + follName_item + "') ";
                                DataBaseHandler.InsertQuery(query, "tb_FollowerName");

                                LogForFollow("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                            }
                            catch { }

                        }

                        string follower = GetFollowercount(screen_Name);
                        string following = GetFollowingCount(screen_Name);
                        string BOARDS = GetBoard(screen_Name);


                        string followingCount = PinterestAccountManager.getBetween(following, "value'>", "</span>");



                        string BoardsName = string.Empty;
                        List<string> BOARDSNAMES = new List<string>();
                        if (frmMain.inviteStart)
                        {
                            BOARDSNAMES = obj.GetAllBoardNames_new1(screen_Name);

                            foreach (var itemBoardNames in BOARDSNAMES)
                            {
                                lstBoardNames.Add(itemBoardNames.ToLower().Replace(" ", "-"));
                            }
                        }
                        frmMain.inviteStart = true;

                        //string BOARDSNAME = GetBoardsNAME(screen_Name);

                        try
                        {
                            foreach (string item_BoardNames in BOARDSNAMES)
                            {
                                //string insertBoard = "INSERT INTO tb_BoardName(UserName,BoardsName)VALUES('" + Username + "','" + item_BoardNames + "')";
                                // DataBaseHandler.InsertQuery(insertBoard, "tb_BoardName");

                                BoardsName += item_BoardNames + (":").ToString();
                            }
                        }
                        catch { }

                        try
                        {
                            string UpdateQuery = "Update tb_emails set Follower = '" + follower + "',Following = '" + followingCount + "',BOARDS= '" + BOARDS + "',BOARDSNAME='" + BoardsName + "',ScreenName='" + screen_Name + "' where UserName = '" + Username + "'";
                            DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
                            
                            new FrmEmailProxyAssigner().LoadDataGrid();
                            new FrmEmailProxyAssigner().makeReadOnlyDataGrid();

                        }
                        catch { }
                    }

                    catch { }

                    string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                    foreach (var item in ArrData)
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
                    if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                    {
                        int startindex = ArrData[1].IndexOf(":");
                        int endindex = ArrData[1].IndexOf("\", \"");

                        this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
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
            return false;
        }
        


        public void Login()
        {
            string _EmailId = "kalileecom11@hotmail.com";
            string _Password = "vini1hjkdhj1233";

            try
            {
                clearProxyDataFromAccount();

                #region Previous Login Code //16.03.2015
                if (string.IsNullOrEmpty(proxyPort))
                {
                    proxyPort = "0";
                }
                //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/login/"), string.Empty, string.Empty, this.UserAgent);
                string _MainSourcePage = httpHelper.getHtmlfromUrlProxy(new Uri("https://pinterest.com/login/"), "", proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword, "");
                if (string.IsNullOrEmpty(_MainSourcePage))
                {
                    _MainSourcePage = httpHelper.getHtmlfromUrlProxy(new Uri("https://pinterest.com/login/"), "", "", int.Parse("0"), "", "", "");
                }

                ///Get App Version 
                if (_MainSourcePage.Contains("app_version"))
                {
                    string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(_MainSourcePage, "app_version");
                    if (ArrAppVersion.Count() > 0)
                    {
                        //string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                        //int startindex = DataString.IndexOf("\"");
                        //string start = DataString.Substring(startindex).Replace("\": \"", "").Replace("\"", string.Empty);
                        //int endindex = start.IndexOf("}");

                        //App_version = start.Substring(0, endindex);

                        string DataString = ArrAppVersion[ArrAppVersion.Length - 1];

                        int startindex = DataString.IndexOf("\": \"");
                        int endindex = DataString.IndexOf(",");

                        App_version = DataString.Substring(startindex, endindex - startindex).Replace("\": \"", "").Replace("\"", string.Empty).Replace("}", string.Empty);
                    }
                }
                else
                {
                    ///App version is not available in page source 
                }


                string referer = "https://www.pinterest.com/login/";
                string PostData = "source_url=%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22CheerfulCarpenter868%40hotmail.com%22%2C%22password%22%3A%22CjHTIdZcyp%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EHomePage%3EUnauthHomePage%3ESignupForm%3EUserRegister(is_login_form%3Dnull%2C+wall_class%3DdarkWall%2C+container%3Dhome_page%2C+show_personalize_field%3Dfalse%2C+unified_auth%3Dnull%2C+auto_follow%3Dnull%2C+next%3Dnull%2C+register%3Dtrue)";
                //string PostData = "source_url=%2Flogin%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Password + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + App_version + "%22%7D%7D&module_path%3DApp()%3ELoginPage()%3ELogin()%3EButton(class_name%3Dprimary%2C%20text%3DLog%20in%2C%20type%3Dsubmit%2C%20size%3Dlarge)";
                //string PostData = "source_url=%2Flogin%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Password + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(class_name%3Dprimary%2C+text%3DLog+in%2C+type%3Dsubmit%2C+size%3Dlarge)";
                //PostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22abhay%22%2C%22last_name%22%3A%22mahavar%22%2C%22username%22%3A%22"++"%22%2C%22about%22%3A%22hi+i+am+software+Developer%22%2C%22location%22%3A%22kanpur%22%2C%22website_url%22%3A%22http%3A%2F%2Fwww.scriptnut.com%2F%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22b1da8fc%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";
                string login = httpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData, referer);
                //string login = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData, referer, proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword);
                if (string.IsNullOrEmpty(login))
                {
                    //login = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData, referer, "", int.Parse("0"), "", "");

                    login = httpHelper.postFormDataProxyPin(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData, referer);
                }

                string AfterLoginPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), "", proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword, "");
                if (string.IsNullOrEmpty(AfterLoginPageSource))
                {
                    AfterLoginPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), "", "", int.Parse("0"), "", "", "");
                }

                if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder"))
                {
                    Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                    this.LoggedIn = true;

                }
                else
                {
                    // Just Modifying for Now
                    //Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                    //this.LoggedIn = false;
                    if (!string.IsNullOrEmpty(AfterLoginPageSource))
                    {
                        Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                        this.LoggedIn = true;
                    }
                    else
                    {
                        Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                        this.LoggedIn = false;
                    }
                } 
                #endregion

                //List<string> listFollowersFromDatabse = new List<string>();
                try
                {

                    string CheckWlcomeUrl = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/"));


                    List<string> listFollowersFromDatabse = new List<string>();
                    try
                    {
                        string screen_Name = Getscreen_Name();
                        PinterestAccountManager.ScreenName = screen_Name;
                        string selectquery = "Select * from  tb_FollowerName where UserName='" + screen_Name + "'";
                        System.Data.DataSet ds = DataBaseHandler.SelectQuery(selectquery, "tb_FollowerName");

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
                        FrmEmailProxyAssigner obj = new FrmEmailProxyAssigner();

                        string screen_Name = Getscreen_Name();

                        //Get current followers list from website
                        List<string> FollowersName = GetRefrshFollowerName(screen_Name);

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

                        LogForFollow(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

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

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                            }
                            catch (Exception ex) { };

                            try
                            {
                                string query = "DELETE FROM  tb_FollowerName where UserName='" + screen_Name + "' and FollwerName='" + unfollower + "'";
                                DataBaseHandler.DeleteQuery(query, "tb_FollowerName");

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                            }
                            catch { }
                        }

                        List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                        LogForFollow(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                        foreach (string follName_item in listNewFollowers)
                        {
                            try
                            {
                                string query = "INSERT INTO  tb_FollowerName (UserName,FollwerName) VALUES ('" + screen_Name + "' ,'" + follName_item + "') ";
                                DataBaseHandler.InsertQuery(query, "tb_FollowerName");

                                LogForFollow("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                            }
                            catch { }

                        }

                        string follower = GetFollowercount(screen_Name);
                        string following = GetFollowingCount(screen_Name);
                        string BOARDS = GetBoard(screen_Name);


                        string followingCount = PinterestAccountManager.getBetween(following, "value'>", "</span>");

                        

                        string BoardsName = string.Empty;
                        List<string> BOARDSNAMES = new List<string>();
                        if (frmMain.inviteStart)
                        {
                            BOARDSNAMES = obj.GetAllBoardNames_new1(screen_Name);
                            
                            foreach (var itemBoardNames in BOARDSNAMES)
                            {
                                lstBoardNames.Add(itemBoardNames.ToLower().Replace(" ", "-"));
                            }
                        }
                        frmMain.inviteStart = true;

                        //string BOARDSNAME = GetBoardsNAME(screen_Name);

                        try
                        {
                            foreach (string item_BoardNames in BOARDSNAMES)
                            {
                                //string insertBoard = "INSERT INTO tb_BoardName(UserName,BoardsName)VALUES('" + Username + "','" + item_BoardNames + "')";
                                //DataBaseHandler.InsertQuery(insertBoard, "tb_BoardName");

                                BoardsName += item_BoardNames + (":").ToString();
                            }
                        }
                        catch { }

                        try
                        {
                            string UpdateQuery = "Update tb_emails set Follower = '" + follower + "',Following = '" + followingCount + "',BOARDS= '" + BOARDS + "',BOARDSNAME='" + BoardsName + "',ScreenName='" + screen_Name + "' where UserName = '" + Username + "'";
                            DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
                            new FrmEmailProxyAssigner().LoadDataGrid();
                            new FrmEmailProxyAssigner().makeReadOnlyDataGrid();

                        }
                        catch { }
                    }

                    catch { }

                    string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                    foreach (var item in ArrData)
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
                    if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                    {
                        int startindex = ArrData[1].IndexOf(":");
                        int endindex = ArrData[1].IndexOf("\", \"");

                        this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                    }


                }
                catch (Exception ex)
                {
                    this.Name = string.Empty;
                }
            }
            catch { };
        }

        

        public void Login(ref GlobusHttpHelper httpHelper)
        {
            try
            {
                this.httpHelper = httpHelper;

                if (string.IsNullOrEmpty(proxyPort))
                {
                    proxyPort = "0";
                }
                //string _MainSourcePage = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/login/"), string.Empty, string.Empty, this.UserAgent);
                string _MainSourcePage = httpHelper.getHtmlfromUrlProxy(new Uri("https://pinterest.com/login/"), "", proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword, "");


                ///Get App Version 
                if (_MainSourcePage.Contains("app_version"))
                {
                    string[] ArrAppVersion = System.Text.RegularExpressions.Regex.Split(_MainSourcePage, "app_version");
                    if (ArrAppVersion.Count() > 0)
                    {
                        string DataString = ArrAppVersion[ArrAppVersion.Count() - 1];

                        int startindex = DataString.IndexOf("\"");
                        string start = DataString.Substring(startindex).Replace("\": \"", "").Replace("\"", string.Empty);
                        int endindex = start.IndexOf("}");

                        App_version = start.Substring(0, endindex);
                    }
                }
                else
                {
                    ///App version is not available in page source 
                }


                string referer = "https://www.pinterest.com/login/";

                //string PostData = "source_url=%2Flogin%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Password + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + App_version + "%22%7D%7D&module_path%3DApp()%3ELoginPage()%3ELogin()%3EButton(class_name%3Dprimary%2C%20text%3DLog%20in%2C%20type%3Dsubmit%2C%20size%3Dlarge)";
                string PostData = "source_url=%2Flogin%2F&data=%7B%22options%22%3A%7B%22username_or_email%22%3A%22" + Uri.EscapeDataString(Username) + "%22%2C%22password%22%3A%22" + Password + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + App_version + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3ELoginPage()%3ELogin()%3EButton(class_name%3Dprimary%2C+text%3DLog+in%2C+type%3Dsubmit%2C+size%3Dlarge)";
                //PostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22abhay%22%2C%22last_name%22%3A%22mahavar%22%2C%22username%22%3A%22"++"%22%2C%22about%22%3A%22hi+i+am+software+Developer%22%2C%22location%22%3A%22kanpur%22%2C%22website_url%22%3A%22http%3A%2F%2Fwww.scriptnut.com%2F%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22b1da8fc%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";
                string login = httpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSessionResource/create/"), PostData, referer, proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword);

                string AfterLoginPageSource = httpHelper.getHtmlfromUrlProxy(new Uri("https://www.pinterest.com"), "", proxyAddress, int.Parse(proxyPort), proxyUsername, proxyPassword, "");

                if (AfterLoginPageSource.Contains("Logout") || AfterLoginPageSource.Contains("pinHolder"))
                {
                    Log("[ " + DateTime.Now + " ] => [ Successfully Login for " + Username + " ]");
                    this.LoggedIn = true;

                }
                else
                {
                    Log("[ " + DateTime.Now + " ] => [ Login Failed for " + Username + " ]");
                    this.LoggedIn = false;
                }


                //List<string> listFollowersFromDatabse = new List<string>();
                try
                {
                    List<string> listFollowersFromDatabse = new List<string>();
                    try
                    {
                        string screen_Name = Getscreen_Name();
                        string selectquery = "Select * from  tb_FollowerName where UserName='" + screen_Name + "'";
                        System.Data.DataSet ds = DataBaseHandler.SelectQuery(selectquery, "tb_FollowerName");

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
                        FrmEmailProxyAssigner obj = new FrmEmailProxyAssigner();

                        string screen_Name = Getscreen_Name();

                        //Get current followers list from website
                        List<string> FollowersName = GetRefrshFollowerName(screen_Name);

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

                        LogForFollow(listUnfollowers.Count + " users Unfollowed Account : " + screen_Name);

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

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " written to file " + UnfollowersList);
                            }
                            catch (Exception ex) { };

                            try
                            {
                                string query = "DELETE FROM  tb_FollowerName where UserName='" + screen_Name + "' and FollwerName='" + unfollower + "'";
                                DataBaseHandler.DeleteQuery(query, "tb_FollowerName");

                                LogForFollow("Unfollower : " + unfollower + " for Account : " + screen_Name + " deleted from Databse");
                            }
                            catch { }
                        }

                        List<string> listNewFollowers = FollowersName.Except(listFollowersFromDatabse).ToList();

                        LogForFollow(listNewFollowers.Count + " NEW Followers for Account : " + screen_Name + "");

                        foreach (string follName_item in listNewFollowers)
                        {
                            try
                            {
                                string query = "INSERT INTO  tb_FollowerName (UserName,FollwerName) VALUES ('" + screen_Name + "' ,'" + follName_item + "') ";
                                DataBaseHandler.InsertQuery(query, "tb_FollowerName");

                                LogForFollow("New follower : " + follName_item + " for Account : " + screen_Name + " added to Databse");
                            }
                            catch { }

                        }

                        string follower = GetFollowercount(screen_Name);
                        string following = GetFollowingCount(screen_Name);
                        string BOARDS = GetBoard(screen_Name);
                        string BoardsName = string.Empty;
                        List<string> BOARDSNAMES = obj.GetAllBoardNames_new1(screen_Name);

                        //string BOARDSNAME = GetBoardsNAME(screen_Name);


                        foreach (string item_BoardNames in BOARDSNAMES)
                        {
                            //string insertBoard = "INSERT INTO tb_BoardName(UserName,BoardsName)VALUES('" + Username + "','" + item_BoardNames + "')";
                            // DataBaseHandler.InsertQuery(insertBoard, "tb_BoardName");

                            BoardsName += item_BoardNames + (":").ToString();
                        }

                        string UpdateQuery = "Update tb_emails set Follower = '" + follower + "',Following = '" + following + "',BOARDS= '" + BOARDS + "',BOARDSNAME='" + BoardsName + "' where UserName = '" + Username + "'";
                        DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
                    }

                    catch { }

                    string[] ArrData = System.Text.RegularExpressions.Regex.Split(AfterLoginPageSource, "username");

                    foreach (var item in ArrData)
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
                    if (ArrData.Count() == 2 && string.IsNullOrEmpty(Name))
                    {
                        int startindex = ArrData[1].IndexOf(":");
                        int endindex = ArrData[1].IndexOf("\", \"");

                        this.Name = ArrData[1].Substring(startindex + 1, endindex - startindex).Replace("id=\"UserNav\"", string.Empty).Replace("a ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("href=", string.Empty).Replace("\"", string.Empty).Replace("/", string.Empty).Trim();
                    }


                }
                catch (Exception ex)
                {
                    this.Name = string.Empty;
                }
            }
            catch { };
        }

        public static Events staticloggingEvent_Follower = new Events();
        private void LogForFollow(string log)
        {
            EventsArgs eArgs = new EventsArgs(log);
            staticloggingEvent_Follower.LogText(eArgs);
        }

        public string AccountChecker(string PinUserName)
        {
            try
            {
                if (PinUserName.Contains("pinterest.com"))
                {
                    //PinUserName = PinUserName;
                }
                else
                {
                    PinUserName = "https://pinterest.com/" + PinUserName + "/";
                  //  PinUserName = "https://pinterest.com/" + "adasdfafsd" + "/";
                }

                string UserPageSource = httpHelper.getHtmlfromUrl(new Uri(PinUserName), string.Empty, string.Empty, this.UserAgent);

                if (UserPageSource.Contains("Pinterest - 404")||string.IsNullOrEmpty(UserPageSource))
                {
                    LogChecker("[ " + DateTime.Now + " ] => [ Not Active : " + PinUserName + " ]");
                    return "Not Active";
                }
                else
                {
                    LogChecker("[ " + DateTime.Now + " ] => [ Active : " + PinUserName + " ]");
                    return "Active";
                }
            }
            catch (Exception ex)
            {
                //LogChecker("Failed To Check : " + PinUserName);
                return "Failed To Check";
            }
        }

        public string Getscreen_Name()
        {
            string FindScreenName = string.Empty;
            try
            {
                string ScreenName1 = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"), "", "", "");
                string ScreenName = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");
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
            catch
            {

            }
            return FindScreenName;
        }

        public string Getscreen_NameRepin(ref GlobusHttpHelper httpHelper)
        {
            string djfsdf = string.Empty;
            string FindScreenName = string.Empty;
            try
            {
                string ScreenName1 = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"), "", "", "");
                //try
                //{
                //    HttpWebRequest gRequest = (HttpWebRequest)httpHelper.gRequest;
                //    gRequest = (HttpWebRequest)WebRequest.Create(new Uri("https://www.pinterest.com/settings"));
                //    gRequest.Method = "GET";
                //    WebResponse gResponse = gRequest.GetResponse();

                //    StreamReader str = new StreamReader(gResponse.GetResponseStream());
                //    djfsdf = str.ReadToEnd();
                //    gResponse.Close();
                //    str.Close();

                //}
                //catch { };


                //string ScreenName = djfsdf;

                string ScreenName = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");

                //string ScreenName = httpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"), "", "", "");

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
            catch
            {

            }
            return FindScreenName;
        }


        public string GetFollowercount(string screen_Name)
        {

            string followers = string.Empty;
            try
            {

                string pagesource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int Startindex = pagesource.IndexOf("pinterestapp:followers"); // "pinterestapp:followers": "1", "og:title":
                string _sorce = pagesource.Substring(Startindex).Replace("pinterestapp:followers", "");
                int Endindex = _sorce.IndexOf(",");
                _sorce = _sorce.Substring(0, Endindex).Replace("\"", "").Replace(":", "").Replace("FollowingLinks\">\n<", string.Empty).Trim();
                followers = _sorce;
                return followers;
            }
            catch
            {


            }
            return followers;
        }
        
        public List<string> GetRefrshFollowerName(string screen_Name)
        {
            List<string> FollwrName = new List<string>();
            FollwrName.Clear();
            string followers = string.Empty;
            int val = 0;

            try
            {
                //var url = "http://www.pinterest.com/resource/UserFollowersResource/get/?source_url=%2Fprabhat101%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22prabhat101%22%2C%22bookmarks%22%3A%5B%22PyExMnxjZTU0MTkzYTA2OGNhYjVjMWRlMTE4MzkxMmYxYzMyYzBjNzQzZDlmZjQxNjkxZjc1YWMzYzZjNDRkZjJhMjA3%22%5D%7D%2C%22context%22%3A%7B%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Afalse%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22layout%22%3A%22fixed_height%22%7D%7D%2C%22render_type%22%3A3%2C%22error_strategy%22%3A1%7D&_=" + GenerateTimeStamp().ToString();
                //string pagesource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name + "/followers/"), "", "", "");
                ////pagesource = httpHelper.getHtmlfromUrl(new Uri("http://www.pinterest.com/resource/UpdatesResource/get/?source_url=%2Fprabhat101%2Ffollowers%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&_="+GenerateTimeStamp().ToString());
                //pagesource = httpHelper.getHtmlfromUrl(new Uri(url), "", "", "");
                //string[] dsd = Regex.Split(pagesource, "username");

                string F_usrer = string.Empty;
                string pagesource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name + "/followers/"), "", "", "");
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
                catch { }


                ////string[] href = Regex.Split(pagesource, "userWrapper");

                //string[] dsd = Regex.Split(pagesource, "href");

                //foreach (string Follwer_item in dsd)
                //{
                //   // string arrhrf = System.Text.RegularExpressions.Regex.Split(Follwer_item, "href")[1];
                //   // string 
                //    try
                //    {
                //        if (Follwer_item.Contains("<p class=\\\"userStats\\\">\\n"))//& Follwer_item.Contains("<p class=\\\"userStats\\\">\\n")) ;// & !Follwer_item.Contains("repins_from") & !Follwer_item.Contains("BoardCount"))//"repins_from//"BoardCount\
                //        {
                //            int Startindex = Follwer_item.IndexOf("\\\\"); // "pinterestapp:followers": "1", "og:title":
                //            string _sorce = Follwer_item.Substring(Startindex).Replace("pinterestapp:followers", "");
                //            int Endindex = _sorce.IndexOf("\\\"\\n ");
                //            _sorce = _sorce.Substring(0, Endindex).Replace("\"", "").Replace(":", "").Replace("\\>\\n                        ", string.Empty).Replace("<span class=\\verifiedDomainIcon\\></span>\\n                       ", "").Trim();
                //            FollwrName.Add(_sorce);
                //        }
                //    }
                //    catch { }



                //}

                while (true)
                {
                    // Thread.Sleep(5000);



                    var url = "http://www.pinterest.com/resource/UserFollowersResource/get/?source_url=%2F" + screen_Name + "%2Ffollowers%2F&data=%7B%22options%22%3A%7B%22username%22%3A%22prabhat101%22%2C%22bookmarks%22%3A%5B%22" + F_usrer + "%22%5D%7D%2C%22context%22%3A%7B%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Afalse%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22layout%22%3A%22fixed_height%22%7D%7D%2C%22render_type%22%3A3%2C%22error_strategy%22%3A1%7D&_=" + GenerateTimeStamp().ToString();
                    // string pagesource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name + "/followers/"), "", "", "");
                    //pagesource = httpHelper.getHtmlfromUrl(new Uri("http://www.pinterest.com/resource/UpdatesResource/get/?source_url=%2Fprabhat101%2Ffollowers%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%7D%7D&_="+GenerateTimeStamp().ToString());
                    string Foolwer_pagesource = httpHelper.getHtmlfromUrl(new Uri(url), "", "", "");
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

                    ////if (F_usrer.Contains("-end-"))
                    ////{
                    ////    break;
                    ////}
                    //string[] FinalData = Regex.Split(pagesource, "href");


                    //// pagesource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name + "/followers/"), "", "", "");

                    // foreach (string Follwer_item in FinalData)
                    // {
                    //     try
                    //     {
                    //         if (Follwer_item.Contains("data-element-type"))//& Follwer_item.Contains("<p class=\\\"userStats\\\">\\n")) ;// & !Follwer_item.Contains("repins_from") & !Follwer_item.Contains("BoardCount"))//"repins_from//"BoardCount\
                    //         {
                    //             string[] follitem = Regex.Split(Follwer_item, "data-element-type");
                    //             string usernamefollwer = follitem[0].Replace("=\\\"/", "").Replace("\\\"\\n  ", "").Replace("/                      ", "").Replace("class=\\\"userWrapper\\\"", "").Replace("\\", "").Replace("\"", "").Replace("/", "");

                    //             //string _sorce = Follwer_item.Substring(Follwer_item.IndexOf("\\\">\\n"), Follwer_item.IndexOf("data-element-type") - Follwer_item.IndexOf("\\\">\\n")).Replace("nowrap>", "").Replace("\"", "").Replace("mailto:", "").Replace("%40", "@").Trim().Replace("?", ""); ;
                    //             //int Startindex = Follwer_item.IndexOf("\\\">\\n"); // "pinterestapp:followers": "1", "og:title":
                    //             //string _sorce = Follwer_item.Substring(Startindex);//.Replace("pinterestapp:followers", "");
                    //             //int Endindex = _sorce.IndexOf("data-element-type");
                    //             //_sorce = _sorce.Substring(Startindex, Endindex);//.Replace("\"", "").Replace(":", "").Replace("\\>\\n                        ", string.Empty).Replace("<span class=\\verifiedDomainIcon\\></span>\\n                       ", "").Trim();
                    //             FollwrName.Add(usernamefollwer);
                    //         }
                    //         //else
                    //         //{
                    //         //    val++;

                    //         //    break;
                    //         //}
                    //     }
                    //     catch { }


                    // }
                    if (F_usrer.Contains("-end-"))
                    {
                        break;
                    }

                }

                //int Startindex = pagesource.IndexOf("pinterestapp:followers"); // "pinterestapp:followers": "1", "og:title":
                //string _sorce = pagesource.Substring(Startindex).Replace("pinterestapp:followers", "");
                //int Endindex = _sorce.IndexOf(",");
                //_sorce = _sorce.Substring(0, Endindex).Replace("\"", "").Replace(":", "").Replace("FollowingLinks\">\n<", string.Empty).Trim();
                //followers = _sorce;
                //return followers;
            }
            catch
            {


            }
            return FollwrName;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
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


        public string GetFollowingCount(string screen_Name)
        {
            string following = string.Empty;
            try
            {
                string followingsource = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int StartFollowing = followingsource.IndexOf("/following/");
                string _following = followingsource.Substring(StartFollowing).Replace("/following/", "").Trim();
                int endFollowing = _following.IndexOf("</a>");
                //string Following = followingsource.Substring(StartFollowing, endFollowing).Replace("\\n", "").Replace("/following/\" >",string.Empty).Trim();
                _following = _following.Substring(0, endFollowing).Replace("\\n", string.Empty).Replace("\" >", string.Empty).Replace("Following", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
                following = _following;
                return following;
            }
            catch
            {

            }
            return following;
        }

        public string GetBoard(string screen_Name)
        {
            string Boards = string.Empty;
            try
            {
                string pagesourceboards = httpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/" + screen_Name), "", "", "");
                int StartBoards = pagesourceboards.IndexOf("pinterestapp:boards\":");
                string _boards = pagesourceboards.Substring(StartBoards);
                int endboard = _boards.IndexOf(",");
                _boards = _boards.Substring(0, endboard).Replace("pinterestapp:boards\":", string.Empty).Replace("\"", string.Empty).Trim();
                Boards = _boards;
                return Boards;
            }
            catch
            {

            }
            return Boards;

        }




        public string UpdateProfile(string profileUsername, string profileLocation, string profileFirstName, string profileLastName, string profilePic, string profileAbout, string profileGender, string appversion)
        {
            try
            {
                if (!LoggedIn)
                {
                    Login();
                }
                if (LoggedIn)
                {
                    LogChecker("[ " + DateTime.Now + " ] => [ Starting Profile Update ]");

                    if (ProfileUpdate.UpdateProfile(profileUsername, profileLocation, profileFirstName, profileLastName, profilePic, profileAbout, profileGender, ref httpHelper, App_version))
                    {
                        LogChecker("[ " + DateTime.Now + " ] => [ Profile Updated : " + Username + " ]");
                        GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, ApplicationData.path_SuccessfullyProfiledAccounts);
                    }
                    else
                    {
                        LogChecker("[ " + DateTime.Now + " ] => [ Unable to Update Profile : " + Username + " ]");
                        GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, ApplicationData.path_FailedToProfileAccounts);
                    }
                }
                else
                {
                    LogChecker("[ " + DateTime.Now + " ] => [ Couldn't Login with >> " + Username + " ]");
                    GlobusFileHelper.AppendStringToTextfileNewLine(Username + ":" + Password + ":" + proxyAddress + ":" + proxyPort + ":" + proxyUsername + ":" + proxyPassword, ApplicationData.path_FailedLoginAccounts);
                }
            }
            catch (Exception ex)
            {
                GlobusFileHelper.AppendStringToTextfileNewLine("Error --> ThreadPoolMethod_ProfileManager() --> " + ex.Message, ApplicationData.PintrestErrorLog);
            }
            return "";
        }

        public void Log(string log)
        {
            EventsArgs eventsArgs = new EventsArgs(log);
            AccounLogEvent.LogText(eventsArgs);
        }

        public void LogChecker(string log)
        {
            EventsArgs eventsArgs = new EventsArgs(log);
            AccounCheckerLogEvent.LogText(eventsArgs);
        }


        
    }
}

