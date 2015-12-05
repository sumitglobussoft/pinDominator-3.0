using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseLib;

namespace PinDominator.Account
{
    public class ProfileManager
    {
        public Events logEvents = new Events();

        public bool UpdateProfile(string profileUsername, string profileLocation, string profileFirstName, string profileLastName, string profilePic, string profileAbout, string profileGender, ref Globussoft.GlobusHttpHelper globusHttpHelper, string appversion)
        {
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
            string Country = string.Empty;
            //string appversion = string.Empty;
            string res_ProfilePage = globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/settings/"), "https://pinterest.com/", "", "");
            // string Res_settingPagesource = globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/UserSettingsResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%22app_version%22%3A%22da919e8%22%2C%22https_exp%22%3Afalse%7D%2C%22module%22%3A%7B%22name%22%3A%22UserEdit%22%2C%22options%22%3A%7B%22user_id%22%3A%2297460916844499785%22%7D%7D%2C%22append%22%3Afalse%2C%22error_strategy%22%3A0%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)&_=1388747100460"), "", "");
            Email = getEmail(res_ProfilePage);

            userid = getUserid(res_ProfilePage);
            string Res_settingPagesource = globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/UserSettingsResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + appversion + "%22%2C%22https_exp%22%3Afalse%7D%2C%22module%22%3A%7B%22name%22%3A%22UserEdit%22%2C%22options%22%3A%7B%22user_id%22%3A%22" + userid + "%22%7D%7D%2C%22append%22%3Afalse%2C%22error_strategy%22%3A0%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)&_=1388747100460"), "", "", "");


            CsrfMiddleToken = getCsrfToken(res_ProfilePage);

            if (string.IsNullOrEmpty(profileFirstName))
            {
                firstname = getFirstName(Res_settingPagesource);
            }
            else
            {
                firstname = profileFirstName;
            }

            if (string.IsNullOrEmpty(profileLastName))
            {
                //lastname = getLastName(res_ProfilePage);
                lastname = getLastName(Res_settingPagesource);
            }
            else
            {
                lastname = profileLastName;
            }

            if (string.IsNullOrEmpty(profileUsername))
            {
                //Username = getUsername(res_ProfilePage);
                Username = getUsername(Res_settingPagesource);
            }
            else
            {
                Username = profileUsername;
                // CheckUsername:
                //string pageSource = globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/check_username/?check_username=" + Username + "&csrfmiddlewaretoken=" + CsrfMiddleToken), "https://pinterest.com/settings/", "", "");
                //if (pageSource.Contains("The username is already in use") || pageSource.Contains("failure"))
                //{
                //    Username = RandomStringGenerator.RemoveChars(Username);
                //    if (Username.Count() > 10)
                //    {
                //        Username = Username.Remove(5); //Removes the extra characters
                //    }
                //    string addChars = RandomStringGenerator.RandomNumber(5);
                //    Username = Username + addChars;

                //    if (Username.Count() > 15)
                //    {
                //        Username = Username.Remove(13); //Removes the extra characters
                //    }

                //    goto CheckUsername;
                //}
            }

            gender = profileGender;


            if (string.IsNullOrEmpty(profileAbout))
            {
                about = getAbout(Res_settingPagesource).Replace(" ", "+");
            }
            else
            {
                about = profileAbout;
            }

            if (string.IsNullOrEmpty(profileLocation))
            {
                location = getLocation(Res_settingPagesource);
            }
            else
            {
                location = profileLocation;
            }


            website = getWebsite(Res_settingPagesource).Replace(":", "%3A").Replace("/", "%2F").Trim();  //http%3A%2F%2Fwww.somethingfunny.com%2F

            // string token = getChtoken(res_ProfilePage);

            string status = "";
            bool Updated = false;

            try
            {
                //Change Profile Details ....
                //string NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22" + firstname + "%22%2C%22last_name%22%3A%22" + lastname + "%22%2C%22username%22%3A%22" + Username + "%22%2C%22about%22%3A%22" + Uri.EscapeUriString(about) + "%22%2C%22location%22%3A%22" + location + "%22%2C%22website_url%22%3A%22" + Uri.EscapeUriString(website) + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + appversion + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";

                //string NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22first_name%22%3A%22" + firstname + "%22%2C%22last_name%22%3A%22" + lastname + "%22%2C%22username%22%3A%22" + Username + "%22%2C%22about%22%3A%22" + Uri.EscapeUriString(about) + "%22%2C%22location%22%3A%22" + location + "%22%2C%22website_url%22%3A%22" + website + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + Uri.EscapeUriString(appversion) + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)%23Modal(module%3DUserEdit(resource%3DUserSettingsResource()))";

                string NewPostData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22" + Uri.EscapeDataString(Email) + "%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22BQ%22%2C%22gender%22%3A%22" + gender + "%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22first_name%22%3A%22" + firstname + "%22%2C%22last_name%22%3A%22" + Uri.EscapeDataString(lastname) + "%22%2C%22username%22%3A%22" + Username + "%22%2C%22about%22%3A%22" + about + "%22%2C%22location%22%3A%22"+profileLocation+"%22%2C%22website_url%22%3A%22"+Uri.EscapeDataString(website)+"%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings)";


                //  string NewPostData1="source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22"+ Email +"%22%2C%22locale%22%3A%22"+location+"%22%2C%22country%22%3A%22"+Country+"%22%2C%22gender%22%3A%22"+gender+"%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22first_name%22%3A%22"+firstname+"%22%2C%22last_name%22%3A%22"+lastname+"01%22%2C%22username%22%3A%22"+Username+"%22%2C%22about%22%3A%22%22%2C%22location%22%3A%22%22%2C%22"+website+"%22%3A%22%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings)
                //string newpostdat = "source_url=/settings/&data={\"options\":{\"email\":\"kavita1112@hotmail.com\",\"locale\":\"en-US\",\"country\":\"CO\",\"gender\":\"female\",\"personalize_from_offsite_browsing\":true,\"first_name\":\"kavita012\",\"last_name\":\"Gupta1111\",\"username\":\"kavita1112\",\"about\":\"\",\"location\":\"bhilai\",\"website_url\":\"\",\"email_enabled\":true,\"email_repins\":true,\"email_likes\":true,\"email_follows\":true,\"email_interval\":\"immediate\",\"email_comments\":true,\"email_shares\":true,\"email_friends_joining\":true,\"email_collaboration_invite\":true,\"email_product_changes\":true,\"email_suggestions\":true,\"email_news\":true,\"email_updates\":true,\"email_feedback_and_research\":true,\"exclude_from_search\":false,\"login_with_facebook\":false,\"login_with_twitter\":false,\"connectToGplus\":false,\"connectToGoogle\":false,\"connectToYahoo\":false},\"context\":{}}&module_path=App()>UserSettingsPage(resource=UserSettingsResource())>Button(class_name=saveSettingsButton, color=primary, type=submit, text=Save Settings";
                //string NewPostData2 = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22email%22%3A%22kavita1112%40hotmail.com%22%2C%22locale%22%3A%22en-US%22%2C%22country%22%3A%22CO%22%2C%22gender%22%3A%22female%22%2C%22personalize_from_offsite_browsing%22%3Atrue%2C%22first_name%22%3A%22gfgfdg%22%2C%22last_name%22%3A%22gfgfdg%22%2C%22username%22%3A%22kavita11%22%2C%22about%22%3A%22i+am+software+engg%22%2C%22location%22%3A%22bhilai%22%2C%22website_url%22%3A%22%22%2C%22email_enabled%22%3Atrue%2C%22email_repins%22%3Atrue%2C%22email_likes%22%3Atrue%2C%22email_follows%22%3Atrue%2C%22email_interval%22%3A%22immediate%22%2C%22email_comments%22%3Atrue%2C%22email_shares%22%3Atrue%2C%22email_friends_joining%22%3Atrue%2C%22email_collaboration_invite%22%3Atrue%2C%22email_product_changes%22%3Atrue%2C%22email_suggestions%22%3Atrue%2C%22email_news%22%3Atrue%2C%22email_updates%22%3Atrue%2C%22email_feedback_and_research%22%3Atrue%2C%22exclude_from_search%22%3Afalse%2C%22login_with_facebook%22%3Afalse%2C%22login_with_twitter%22%3Afalse%2C%22connectToGplus%22%3Afalse%2C%22connectToGoogle%22%3Afalse%2C%22connectToYahoo%22%3Afalse%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EButton(class_name%3DsaveSettingsButton%2C+color%3Dprimary%2C+type%3Dsubmit%2C+text%3DSave+Settings)";
                //string pagesource = globusHttpHelper.postFormData(new Uri("https://www.pinterest.com/resource/UserSettingsResource/update/"), NewPostData, "https://www.pinterest.com/settings/");
                //pagesource = globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSettingsResource/update/"), NewPostData, "https://www.pinterest.com/settings/", "", 0, "", "");

                string pagesource = globusHttpHelper.postFormData(new Uri("https://www.pinterest.com/resource/UserSettingsResource/update/"), NewPostData, "https://www.pinterest.com/settings/");
                //pagesource = globusHttpHelper.postFormDataProxy(new Uri("https://www.pinterest.com/resource/UserSettingsResource/update/"), NewPostData, "https://www.pinterest.com/settings/", "", 0, "", "");

                //Change profile Image .....

                #region
                //globusHttpHelper.MultiPartImageUploadPreview(Email, language, firstname, lastname, Username, gender, about, location, website, profilePic, CsrfMiddleToken);
                //string pagesource = globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/resource/UserSettingsResource/get/?source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%7D%2C%22context%22%3A%7B%22app_version%22%3A%22"+ appversion +"%22%2C%22https_exp%22%3Afalse%7D%2C%22module%22%3A%7B%22name%22%3A%22UserEdit%22%2C%22options%22%3A%7B%22user_id%22%3A%22"+ userid +"%22%7D%7D%2C%22append%22%3Afalse%2C%22error_strategy%22%3A0%7D&module_path=App()%3EUserSettingsPage(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserEdit)&_=1388476372529") , "" , "" ,"");
                #endregion

                if (!String.IsNullOrEmpty(profilePic))
                {
                    Updated = globusHttpHelper.MultiPartImageUpload_new(Email, language, firstname, lastname, Username, gender, about, location, website, profilePic, CsrfMiddleToken, appversion);
                }
                else
                {
                    Updated = true;
                }
            }
            catch (Exception)
            {

            }

            if (Updated)
            {
                return Updated;
            }

            return false;
        }

        public string getChtoken(string pagesource)
        {
            string _ChToken = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("_ch");
                string Start = pagesource.Substring(StartIndex).Replace("_ch", "");
                int EndIndex = Start.IndexOf("' />");
                string End = Start.Substring(0, EndIndex).Replace("value=", "").Replace("\"", "").Replace(" ", "").Replace("'", "");
                _ChToken = End;
            }
            catch (Exception ex)
            {

            }
            return _ChToken;
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

        public string getEmail(string pageSource)
        {
            string Email = string.Empty;
            try
            {
                int StartIndex = pageSource.IndexOf("email\": \"");
                string Start = pageSource.Substring(StartIndex).Replace("email\": \"", "");
                int EndIndex = Start.IndexOf("\"},");
                string End = Start.Substring(0, EndIndex).Replace(" type=\"text\"", string.Empty).Replace("value=", "").Replace("\"", "").Replace(" ", "");
                Email = End.Split(',')[0];
            }
            catch (Exception ex)
            {

            }
            return Email;
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

        public string getGender(string pagesource)
        {
            string gender = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("id_gender_0");
                string Start = pagesource.Substring(StartIndex);
                int EndIndex = Start.IndexOf("</div>");
                string End = Start.Substring(0, EndIndex);
                string[] Array = System.Text.RegularExpressions.Regex.Split(End, "<li>");
                Array = Array.Skip(1).ToArray();

                foreach (string item in Array)
                {
                    if (item.Contains("checked"))
                    {
                        int startIndex = item.IndexOf("value=\"");
                        string start = item.Substring(startIndex).Replace("value=\"", "");
                        int endIndex = start.IndexOf("\"");
                        string end = start.Substring(0, endIndex);
                        gender = end;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return gender;
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

        public string getAbout1(string pagesource)
        {
            string about = string.Empty;
            try
            {
                int StartIndex = pagesource.IndexOf("name=\"about\"");
                string Start = pagesource.Substring(StartIndex).Replace("name=\"about\"", "");
                int EndIndex = Start.IndexOf("</textarea>");
                string End = Start.Substring(0, EndIndex).Replace(">", "");
                about = End;
            }
            catch (Exception ex)
            {

            }
            return about;
        }

        public string getCsrfToken(string pageSource)
        {
            string CsrfMiddleToken = string.Empty;
            try
            {
                int startIndex = pageSource.IndexOf("csrfmiddlewaretoken");
                string Start = pageSource.Substring(startIndex).Replace("csrfmiddlewaretoken", "");
                int endIndex = Start.IndexOf("/>");
                string End = Start.Substring(0, endIndex).Replace("value", "").Replace(" ", "").Replace("'", "").Replace("=", "");

                CsrfMiddleToken = End;
            }
            catch (Exception ex)
            {

            }
            return CsrfMiddleToken;
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

        private void Log(string message)
        {
            EventsArgs eArgs = new EventsArgs(message);
            logEvents.LogText(eArgs);
        }
    }
}
