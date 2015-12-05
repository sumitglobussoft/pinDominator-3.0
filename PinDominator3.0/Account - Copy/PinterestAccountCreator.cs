using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Globussoft;
using BaseLib;
using System.Net;

namespace PinDominator.Account
{
    public class PinterestAccountCreator
    {
        public static Events AccounCreationLogEvent = new Events();

        public string PinUserName { get; set; }
        public string PinEmail { get; set; }
        public string PinPassword { get; set; }
        public string TwitterUsername { get; set; }
        public string TwitterPassword { get; set; }
        public string proxyAddress { get; set; }
        public string proxyPort { get; set; }
        public string proxyUsername { get; set; }
        public string proxyPassword { get; set; }
        public string InviteCode { get; set; }
        public string userAgent { get; set; }
        public string Followers { get; set; }
        public string Following { get; set; }
      

        public GlobusHttpHelper httpHelper = new GlobusHttpHelper();

        public PinterestAccountCreator(string PinUserName, string PinPassword, string PinEmail, string TwtUserName, string TwtPassword, string ProxyAddress, string ProxyPort, string ProxyUserName, string ProxyPassword, string userAgent , string InviteCode)
        {
            this.PinUserName = PinUserName;
            this.PinPassword = PinPassword;
            this.PinEmail = PinEmail;
            this.TwitterUsername = TwtUserName;
            this.TwitterPassword = TwtPassword;
            this.proxyAddress = ProxyAddress;
            this.proxyPort = ProxyPort;
            this.proxyUsername = ProxyUserName;
            this.proxyPassword = ProxyPassword;
            this.userAgent = userAgent;
            this.InviteCode = InviteCode;
        }

        public bool Login()
        {
            string OAuthVerifier = string.Empty;
            string OAuthenticityToken = string.Empty;
            string OAuthToken = string.Empty;
            string PinToken = string.Empty;
            string InviteUrl = string.Empty;

            bool IsAccountCreated = false;

            try
            {
                string ts = GenerateTimeStamp();

                InviteUrl = "http://pinterest.com/invited/?email=" + PinEmail + "&invite=" + InviteCode;
                InviteUrl = "http://pinterest.com/invited/?invite=" + InviteCode;

                int intProxyPort = 80;
                if (ApplicationData.ValidateNumber(this.proxyPort))
                {
                    intProxyPort = int.Parse(this.proxyPort);
                }

                string TwitterPageContent = httpHelper.getHtmlfromUrlProxy(new Uri("https://twitter.com/"), "" , this.proxyAddress, intProxyPort, this.proxyUsername, this.proxyPassword, this.userAgent);
                string BootPageContent = httpHelper.getHtmlfromUrl(new Uri("https://twitter.com/account/bootstrap_data?r=0.9414130715097342"), "https://twitter.com/", "", this.userAgent);

                //string PostData = "session%5Busername_or_email%5D=" + TwitterUsername + "&session%5Bpassword%5D=" + TwitterPassword  + "&scribe_log=%5B%22%7B%5C%22event_name%5C%22%3A%5C%22web%3Afront%3Alogin_callout%3Aform%3A%3Alogin_click%5C%22%2C%5C%22noob_level%5C%22%3Anull%2C%5C%22internal_referer%5C%22%3Anull%2C%5C%22user_id%5C%22%3A0%2C%5C%22page%5C%22%3A%5C%22front%5C%22%2C%5C%22_category_%5C%22%3A%5C%22client_event%5C%22%2C%5C%22ts%5C%22%3A" + ts + "%7D%22%5D&redirect_after_login=";
                //string PostPageContent = httpHelper.postFormData(new Uri("https://twitter.com/sessions?phx=1"), PostData, "https://twitter.com/", "" , this.userAgent);

                //string ts = GenerateTimeStamp();
                string get_twitter_first = string.Empty;
                try
                {
                    get_twitter_first = httpHelper.getHtmlfromUrlProxy(new Uri("https://twitter.com/") ,"" , proxyAddress , 0 , proxyUsername , proxyPassword , "");
                }
                catch (Exception ex)
                {
                    //string get_twitter_first = globusHttpHelper1.getHtmlfromUrlp(new Uri("http://twitter.com/"), string.Empty, string.Empty);
                    //Thread.Sleep(1000);
                    get_twitter_first = httpHelper.getHtmlfromUrlProxy(new Uri("https://twitter.com/"), "", proxyAddress, 0, proxyUsername, proxyPassword, "");
                }

                string postAuthenticityToken = "";
                
                int startIndx = get_twitter_first.IndexOf("postAuthenticityToken");
                if (startIndx > 0)
                {
                    int indexstart = startIndx + "postAuthenticityToken".Length + 3;
                    int endIndx = get_twitter_first.IndexOf("\"", startIndx);

                    postAuthenticityToken = get_twitter_first.Substring(startIndx, endIndx - startIndx).Replace(",", "");

                    if (postAuthenticityToken.Contains("postAuthenticityToken"))
                    {
                        try
                        {
                            string[] getOuthentication = System.Text.RegularExpressions.Regex.Split(get_twitter_first, "\"postAuthenticityToken\":\"");
                            string[] authenticity = System.Text.RegularExpressions.Regex.Split(getOuthentication[1], ",");

                            if (authenticity[0].IndexOf("\"") > 0)
                            {
                                int indexStart1 = authenticity[0].IndexOf("\"");
                                string start = authenticity[0].Substring(0, indexStart1);
                                postAuthenticityToken = start.Replace("\"", "").Replace(":", "");
                            }
                        }
                        catch { };
                    }
                }
                else
                {
                    string[] array = System.Text.RegularExpressions.Regex.Split(get_twitter_first, "<input type=\"hidden\"");
                    foreach (string item in array)
                    {
                        if (item.Contains("authenticity_token"))
                        {
                            int startindex = item.IndexOf("value=\"");
                            if (startindex > 0)
                            {
                                string start = item.Substring(startindex).Replace("value=\"", "");
                                int endIndex = start.IndexOf("\"");
                                string end = start.Substring(0, endIndex);
                                postAuthenticityToken = end;
                                break;
                            }
                        }
                    }

                }

                string get_twitter_second = httpHelper.postFormData(new Uri("https://twitter.com/scribe"), "log%5B%5D=%7B%22event_name%22%3A%22web%3Amobile_gallery%3Agallery%3A%3A%3Aimpression%22%2C%22noob_level%22%3Anull%2C%22internal_referer%22%3Anull%2C%22context%22%3A%22mobile_gallery%22%2C%22event_info%22%3A%22mobile_app_download%22%2C%22user_id%22%3A0%2C%22page%22%3A%22mobile_gallery%22%2C%22_category_%22%3A%22client_event%22%2C%22ts%22%3A" + ts + "%7D", "https://twitter.com/?lang=en&logged_out=1#!/download" , "" ,"");//globusHttpHelper.getHtmlfromUrl(new Uri("https://twitter.com/account/bootstrap_data?r=0.21632839148912897"), "https://twitter.com/", string.Empty);

                string get2nd = httpHelper.getHtmlfromUrlProxy(new Uri("http://twitter.com/account/bootstrap_data?r=0.21632839148912897"), "https://twitter.com/", proxyAddress, 0, proxyUsername, proxyPassword, "");

                string get_api = httpHelper.getHtmlfromUrl(new Uri("http://api.twitter.com/receiver.html"), "https://twitter.com/", "", "");


                //Old postdata 
                //string postData = "session%5Busername_or_email%5D=" + Username + "&session%5Bpassword%5D=" + Password + "&scribe_log=%5B%22%7B%5C%22event_name%5C%22%3A%5C%22web%3Afront%3Alogin_callout%3Aform%3A%3Alogin_click%5C%22%2C%5C%22noob_level%5C%22%3Anull%2C%5C%22internal_referer%5C%22%3Anull%2C%5C%22user_id%5C%22%3A0%2C%5C%22page%5C%22%3A%5C%22front%5C%22%2C%5C%22_category_%5C%22%3A%5C%22client_event%5C%22%2C%5C%22ts%5C%22%3A" + ts + "%7D%22%5D&redirect_after_login=";

                //new post data (28/9/12)
                string postData = "session%5Busername_or_email%5D=" + TwitterUsername + "&session%5Bpassword%5D=" + TwitterPassword + "&scribe_log=&redirect_after_login=&authenticity_token=" + postAuthenticityToken + "";

                string response_Login = httpHelper.postFormData(new Uri("https://twitter.com/sessions"), postData, "https://twitter.com/", "", "");


                string AfterPostPageContent = httpHelper.getHtmlfromUrl(new Uri("https://twitter.com/"), "https://twitter.com/", "", this.userAgent);
                 
                if (AfterPostPageContent.Contains("signout-button"))
                {
                    string InvitePageContent = httpHelper.getHtmlfromUrl(new Uri(InviteUrl), "", "", this.userAgent);
                    
                    if (InvitePageContent.Contains("This invite code is not valid"))
                    {
                        IsAccountCreated = false;
                        Log("[ " + DateTime.Now + " ] => [ This invite code is not valid " + PinEmail + " ]");
                        return IsAccountCreated;
                    }

                    string InvitedPageContent = httpHelper.getHtmlfromUrl(new Uri("http://pinterest.com/twitter/?invited=1"), InviteUrl, "", this.userAgent);

                    if (InvitedPageContent.Contains("Logout"))
                    {
                        IsAccountCreated = true;
                        Log("[ " + DateTime.Now + " ] => [ This twitter account allready added with pinterest " + TwitterUsername + " ]");
                        return IsAccountCreated;
                    }

                    Uri ResponseUri = httpHelper.GetResponseData();


                    if (!string.IsNullOrEmpty(ResponseUri.OriginalString))
                    {
                        if (ResponseUri.OriginalString.Contains("verify_captcha/?"))
                        {
                            //List<string> lstData = GetCapctha();
                            //string challenge = string.Empty;
                            //string response = string.Empty;


                            //challenge = lstData[0].ToString();
                            //response = lstData[1].ToString();
                            //response = response.Replace(" ", "+");
                            //string postUrl = "http://pinterest.com/verify_captcha/?src=register&return=%2Fwelcome%2F";
                            //string postData = "challenge=" + challenge + "&response=" + response;

                            //string POstResponse = httpHelper.postFormData(new Uri(postUrl), postData, "", string.Empty);

                            //string pageSrcWelcome = httpHelper.getHtmlfromUrl(new Uri("http://pinterest.com/welcome/"), postUrl, "");
                        }
                        if (ResponseUri.OriginalString.Contains("http://pinterest.com/twitter/?oauth_token"))
                        {
                            OAuthVerifier = ResponseUri.OriginalString;

                            int FirstPointToken = InvitedPageContent.IndexOf("csrfmiddlewaretoken");
                            string FirstTokenSubString = InvitedPageContent.Substring(FirstPointToken);

                            int SecondPointToken = FirstTokenSubString.IndexOf("/>");
                            PinToken = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("'", string.Empty).Trim();
                        }
                        if (ResponseUri.OriginalString.Contains("api.twitter.com/oauth/authenticate?oauth_token="))
                        {
                            int FirstAuthenticityPoint = InvitedPageContent.IndexOf("authenticity_token\" type=\"hidden\"");

                            string FirstSubAuthenticity = InvitedPageContent.Substring(FirstAuthenticityPoint);
                            OAuthenticityToken = FirstSubAuthenticity.Substring(FirstSubAuthenticity.IndexOf("value="), (FirstSubAuthenticity.IndexOf("/></div>")) - (FirstSubAuthenticity.IndexOf("value="))).Replace("value=", string.Empty).Replace("\"", string.Empty).Trim();

                            OAuthToken = ResponseUri.OriginalString.Replace("https://api.twitter.com/oauth/authenticate?oauth_token=", string.Empty).Replace("http://api.twitter.com/oauth/authenticate?oauth_token=", string.Empty);

                            string AcceptPostData = "authenticity_token=" + OAuthenticityToken + "&oauth_token=" + OAuthToken;

                            string OauthUrl = "https://twitter.com/oauth/authenticate?oauth_token=" + OAuthToken;

                            string AcceptedPageContent = httpHelper.postFormData(new Uri("https://twitter.com/oauth/authenticate"), AcceptPostData, OauthUrl, string.Empty, this.userAgent);

                            int FirstOAuthVerifierPoint = AcceptedPageContent.IndexOf("http://pinterest.com/twitter/?oauth_token=");

                            string FirstSuboAuth = AcceptedPageContent.Substring(FirstOAuthVerifierPoint);

                            OAuthVerifier = FirstSuboAuth.Substring(0, FirstSuboAuth.IndexOf(">")).Replace("\"", string.Empty).Trim();//.Replace("&oauth_verifier=", string.Empty).Trim();

                            string PinterestRegistrationPageContent = httpHelper.getHtmlfromUrl(new Uri(OAuthVerifier), string.Empty, string.Empty, this.userAgent);

                            if (!PinterestRegistrationPageContent.Contains("Oops! We are having some issues talking to Twitter. Please try again later"))
                            {
                                int FirstPointToken = PinterestRegistrationPageContent.IndexOf("csrfmiddlewaretoken");
                                string FirstTokenSubString = PinterestRegistrationPageContent.Substring(FirstPointToken);

                                int SecondPointToken = FirstTokenSubString.IndexOf("/>");
                                PinToken = FirstTokenSubString.Substring(0, SecondPointToken).Replace("csrfmiddlewaretoken", string.Empty).Replace("value=", string.Empty).Replace("'", string.Empty).Trim();
                            }
                            else
                            {
                                Log("[ " + DateTime.Now + " ] => [ We are having some issues talking to Twitter. Please try again later ]");
                                return false;
                            }
                        }


                        //Checking For User Name
                        string CheckUserNameUrl = "http://pinterest.com/check_username/?check_username=" + PinUserName + "&csrfmiddlewaretoken=" + PinToken;
                        string User = httpHelper.getHtmlfromUrl(new Uri(CheckUserNameUrl), OAuthVerifier, PinToken, this.userAgent);

                        if (User.Contains("username is already"))
                        {

                            int num = RandomNumberGenerator.GenerateRandom(100, 1000);

                            PinUserName = PinUserName + num.ToString();

                            //Checking For User Name
                            CheckUserNameUrl = "http://pinterest.com/check_username/?check_username=" + PinUserName + "&csrfmiddlewaretoken=" + PinToken;
                            User = httpHelper.getHtmlfromUrl(new Uri(CheckUserNameUrl), OAuthVerifier, PinToken, this.userAgent);

                        }

                        ////Checking For User Name
                        //string CheckEmailUrl = "http://pinterest.com/check_username/?check_email=" + PinEmail + "&csrfmiddlewaretoken=" + PinToken;
                        //string Email = httpHelper.getHtmlfromUrl(new Uri(CheckEmailUrl), OAuthVerifier, PinToken);

                        //if (!Email.Contains("success"))
                        //{
                        //    //IsPinLoggedIn = true;
                        //    Log("This email not valid " + PinEmail);
                        //    return;
                        //}
                        string RegistrationPostData = "username=" + PinUserName + "&email=" + PinEmail + "&password=" + PinPassword + "&invite=" + InviteCode.Replace(" ", "").Replace("http://email.pinterest.com/wf/click&upn=","") + "&twitter=1&csrfmiddlewaretoken=" + PinToken + "&user_image=http%3A%2F%2Fimg.tweetimag.es%2Fi%2FSocioPro_o";

                        string RegistredPageContent = httpHelper.postFormData(new Uri("http://pinterest.com/register/"), RegistrationPostData, OAuthVerifier, string.Empty, this.userAgent);

                        if (RegistredPageContent.Contains("recaptcha/api/js/recaptcha"))
                        {
                            List<string> lstData=GetCapctha();
                            string challenge = string.Empty;
                            string response = string.Empty;


                            challenge = lstData[0].ToString();
                            response = lstData[1].ToString();
                            response = response.Replace(" ", "+");
                            string postUrl = "http://pinterest.com/verify_captcha/?src=register&return=/welcome/";
                            string postData1 = "challenge="+challenge+"&response="+response;

                            string POstResponse = httpHelper.postFormData(new Uri(postUrl), postData1, OAuthVerifier, PinToken, this.userAgent);

                            RegistredPageContent = httpHelper.getHtmlfromUrl(new Uri("http://pinterest.com/welcome/"), "", "", this.userAgent);
                        }
                        if (RegistredPageContent.Contains("architecture"))
                        {
                            IsAccountCreated = true;
                            Log("[ " + DateTime.Now + " ] => [ Account Created " + PinUserName + " ]");
                        }
                        else
                        {
                            IsAccountCreated = false;
                            Log("[ " + DateTime.Now + " ] => [ Account Not Created " + PinUserName + " ]");
                            return IsAccountCreated;
                        }

                        string WelcomPageContent = httpHelper.postFormData(new Uri("http://pinterest.com/welcome/"), "", OAuthVerifier, PinToken , this.userAgent);

                        //Changed By  Gargi On 22nd May 2012 --> request from fiddler changed
                        //string CategoryPostData = "categories=architecture&user_follow=true";

                        string CategoryPostData = "categories=art%2Ccars_motorcycles%2Cdesign%2Cdiy_crafts%2Ceducation%2Carchitecture%2Cfitness";

                        string CategoryPageContent = httpHelper.postFormData(new Uri("http://pinterest.com/welcome/"), CategoryPostData, "http://pinterest.com/welcome/", PinToken , this.userAgent);

                        if (CategoryPageContent.Contains("success"))
                        {
                            IsAccountCreated = true;
                            Log("[ " + DateTime.Now + " ] => [ Initial Category Added " + PinUserName + " ]");
                        }

                        string AllowUserPostData = "follow_users%5B%5D=sharp&follow_users%5B%5D=neillehepworth&follow_users%5B%5D=miamalm&follow_users%5B%5D=kiluka&follow_users%5B%5D=gaileguevara&follow_users%5B%5D=jellway&follow_users%5B%5D=richard_larue&follow_users%5B%5D=rayestudio&follow_users%5B%5D=jdraper&follow_users%5B%5D=shashashasha";
                        //if (string.IsNullOrEmpty(ApplicationData.UsersToFollow))
                        //{
                        //    AllowUserPostData = "follow_users%5B%5D=sharp&follow_users%5B%5D=neillehepworth&follow_users%5B%5D=miamalm&follow_users%5B%5D=kiluka&follow_users%5B%5D=gaileguevara&follow_users%5B%5D=jellway&follow_users%5B%5D=richard_larue&follow_users%5B%5D=rayestudio&follow_users%5B%5D=jdraper&follow_users%5B%5D=shashashasha";
                        //}
                        //else
                        //{
                        //    AllowUserPostData = ApplicationData.UsersToFollow;
                        //}

                        string AlloweduserPostData = httpHelper.postFormData(new Uri("http://pinterest.com/welcome/"), AllowUserPostData, "http://pinterest.com/welcome/", PinToken , this.userAgent);

                        if (AlloweduserPostData.Contains("success"))
                        {
                            IsAccountCreated = true;
                            Log("[ " + DateTime.Now + " ] => [ Intial User Added " + PinUserName + " ]");
                        }

                        string BordPostData = "board_names%5B%5D=Products+I+Love&board_names%5B%5D=Favorite+Places+%26+Spaces&board_names%5B%5D=Books+Worth+Reading&board_names%5B%5D=My+Style&board_names%5B%5D=For+the+Home";

                        string BoardPostedPageContent = httpHelper.postFormData(new Uri("http://pinterest.com/welcome/"), BordPostData, "http://pinterest.com/welcome/", PinToken , this.userAgent);

                        if (BoardPostedPageContent.Contains("success"))
                        {
                            IsAccountCreated = true;
                            Log("[ " + DateTime.Now + " ] => [ Intial Board Added " + PinUserName + " ]");
                        }
                    }

                    return IsAccountCreated;
                }
                else
                {
                    Log("[ " + DateTime.Now + " ] => [ Twitter Account Not Logged In :" + TwitterUsername + " ]");
                    return IsAccountCreated;
                }
            }
            catch (Exception ex)
            {
                //Log("Error " + ex.Message);
                return IsAccountCreated;
            }
        }

        public List<string> GetCapctha()
        {
            
            string capcthavalue = string.Empty;
            string ImageURL = string.Empty;
            Random randm = new Random();
            double cachestop = randm.NextDouble();
            List<string> lstReturn = new List<string>();

            string pagesource1 = httpHelper.getHtmlfromUrl(new Uri("https://www.google.com/recaptcha/api/js/recaptcha_ajax.js"), "", "", this.userAgent);
            string pagesource2 = httpHelper.getHtmlfromUrl(new Uri("https://www.google.com/recaptcha/api/challenge?k=6LdYxc8SAAAAAHyLKDUP3jgHt11fSDW_WBwSPPdF&ajax=1&cachestop=" + cachestop + "&lang=en"), "", "", this.userAgent);
            try
            {
                int IndexStart = pagesource2.IndexOf("challenge :");
                string Start = pagesource2.Substring(IndexStart);
                int IndexEnd = Start.IndexOf("',");
                string End = Start.Substring(0, IndexEnd).Replace("challenge :", "").Replace("'", "").Replace(" ", "");
                capcthavalue = End;
                lstReturn.Add(capcthavalue);
                ImageURL = "https://www.google.com/recaptcha/api/image?c=" + End;
                WebClient web1 = new WebClient();
                byte[] args = web1.DownloadData(ImageURL);

                //string[] arr1 = new string[] { "indianbill007", "sumit1234", "" };
                string[] arr1 = new string[] { ApplicationData.DBCUsername, ApplicationData.DBCPassword, "" };


                string CaptchaText = DecodeDBC(arr1, args);
                lstReturn.Add(CaptchaText);
                lstReturn.Add("stchim caste");
                return lstReturn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("1 :" + ex.StackTrace);
                return null;
            }
        }

        private string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        static public string DecodeDBC(string[] args, byte[] imageBytes)
        {

            try
            {
                // Put your DBC username & password here:
                //Client client = (Client)new HttpClient(args[0], args[1]);
                DeathByCaptcha.Client client = (DeathByCaptcha.Client)new DeathByCaptcha.SocketClient(args[0], args[1]);
                client.Verbose = true;

                Console.WriteLine("Your balance is {0:F2} US cents", client.Balance);

                for (int i = 2, l = args.Length; i < l; i++)
                {
                    Console.WriteLine("Solving CAPTCHA {0}", args[i]);

                    // Upload a CAPTCHA and poll for its status.  Put the CAPTCHA image
                    // file name, file object, stream, or a vector of bytes, and desired
                    // solving timeout (in seconds) here:
                    DeathByCaptcha.Captcha captcha = client.Decode(imageBytes, 2 * DeathByCaptcha.Client.DefaultTimeout);
                    if (null != captcha)
                    {
                        Console.WriteLine("CAPTCHA {0:D} solved: {1}", captcha.Id, captcha.Text);

                        //// Report an incorrectly solved CAPTCHA.
                        //// Make sure the CAPTCHA was in fact incorrectly solved, do not
                        //// just report it at random, or you might be banned as abuser.
                        //if (client.Report(captcha))
                        //{
                        //    Console.WriteLine("Reported as incorrectly solved");
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Failed reporting as incorrectly solved");
                        //}

                        return captcha.Text;
                    }
                    else
                    {
                        Console.WriteLine("CAPTCHA was not solved");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public void Log(string log)
        {
            EventsArgs eventsArgs = new EventsArgs(log);
            AccounCreationLogEvent.LogText(eventsArgs);
        }
    }
}
