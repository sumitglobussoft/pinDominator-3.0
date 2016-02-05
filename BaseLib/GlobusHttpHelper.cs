using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Globussoft
{
    public class GlobusHttpHelper
    {

       public CookieCollection gCookies;
       public HttpWebRequest gRequest;
        HttpWebResponse gResponse;
        public string responseURI = string.Empty;
        public static string valueURl = string.Empty;

        string UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";

        int Timeout = 90000;

        public Uri GetResponseData()
        {
            return gResponse.ResponseUri;
        }

        public string getHtmlfromUrl(Uri url)
        {
            string responseString = string.Empty;
            try
            {
                //setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
                // gRequest.Headers.Add("")

                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");

                gRequest.Method = "GET";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                valueURl = gResponse.ResponseUri.ToString();
                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    responseURI = gResponse.ResponseUri.AbsoluteUri;

                    
                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return responseString;
        }

        public string getHtmlfromUrlFindImage(Uri url, string Referes, string appVersion)
        {
            string responseString = string.Empty;
            try
            {
                //setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
                gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                gRequest.Headers["Accept-Encoding"] = "gzip, deflate, sdch";
                // gRequest.Headers.Add("") gzip, deflate, sdch

                gRequest.KeepAlive = true;

                gRequest.AllowAutoRedirect = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");

                gRequest.Method = "GET";
                if (!string.IsNullOrEmpty(Referes))
                {
                    gRequest.Referer = Referes;
                }
                if (!string.IsNullOrEmpty(appVersion))
                {
                    gRequest.Headers.Add("X-APP-VERSION", appVersion);
                }

                //if (!string.IsNullOrEmpty(Token))
                {
                    //gRequest.Headers.Add("X-CSRFToken", Token);
                    gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                }
                gRequest.Headers["X-NEW-APP"] = "1";

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                }

                if (this.gCookies == null)
                {
                    this.gCookies = new CookieCollection();
                }



                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return responseString;
        }

        public string getHtmlfromUrlOLd(Uri url, string Referes, string Token, string AccountUserAgent)
        {
            string responseString = string.Empty;
            try
            {
                setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(AccountUserAgent))
                {
                    gRequest.UserAgent = AccountUserAgent;
                }
                else
                {
                    gRequest.UserAgent = UserAgent;
                }
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
               // gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                //gRequest.Headers["Cache-Control"] = "max-age=0";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
                //gRequest.Connection = "keep-alive";
                gRequest.Timeout = Timeout;
                gRequest.KeepAlive = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");
                gRequest.Method = "GET";
               // gRequest.Headers.Add("Accept-Encoding","gzip, deflate, sdch");
                //gRequest.AllowAutoRedirect = false;
                ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

                if (!string.IsNullOrEmpty(Referes))
                {
                    gRequest.Referer = Referes;
                }
                //if (!string.IsNullOrEmpty(Token))
                {
                    //gRequest.Headers.Add("X-CSRFToken", Token);
                    gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                }
                gRequest.Headers["X-NEW-APP"] = "1";

                if (gCookies != null)
                {
                    foreach (Cookie item in gCookies)
                    {
                        if (item.Name == "csrftoken")
                        {
                            string csrftokenValue = item.Value;

                            gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                            break;
                        }
                    }
                }


                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                    try
                    {
                        //gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0 - 2078004405 - 1321685323158", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1322116799.1322206824.9", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                     responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {
                
            }
            return responseString;
        }
        //valueURl = gResponse.ResponseUri.ToString();

        public string HttpUploadPictureForWall(ref GlobusHttpHelper HttpHelper, string userid, string url, string paramName, string contentType, string localImagePath, NameValueCollection nvc, string proxyAddress, int proxyPort, string proxyUsername, string proxyPassword, string picfilepath, string fileName)
        {

            #region PostData_ForUploadImage
            string localImagePath1 = localImagePath.Replace(picfilepath, string.Empty).Replace("\\", string.Empty);

            #endregion


            bool isAddaCover = false;
            string responseStr = string.Empty;

            try
            {
                string boundary = "-----------------------------" + DateTime.Now.Ticks.ToString("X");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                gRequest = (HttpWebRequest)WebRequest.Create(url);
                gRequest.ContentType = "multipart/form-data; boundary=" + "----" + boundary.Replace("-", "");
                //gRequest.Referer = "Referer: https://www.facebook.com/profile.php?id=" + userid + "&ref=tn_tnmn";
                if (url.Contains("https://in."))
                {
                    gRequest.Referer = "https://in.pinterest.com/";
                }
                else
                {
                    gRequest.Referer = "https://www.pinterest.com/";
                }
                if (url.Contains("https://in."))
                {
                    gRequest.Host = "in.pinterest.com";
                }
                else
                {
                    gRequest.Host ="www.pinterest.com";
                }
                if (url.Contains("https://in."))
                {
                    gRequest.Headers["Origin"] = "https://in.pinterest.com/";
                }
                else
                {
                    gRequest.Headers["Origin"] = "https://www.pinterest.com";
                }
                gRequest.Headers["X-File-Name"] = fileName;
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
                gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
                gRequest.Accept = "*/*";
                // gRequest.Headers["Accept-Encoding"] = "gzip, deflate";
                gRequest.Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
                foreach (Cookie item in gCookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        string csrftokenValue = item.Value;

                        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                        break;
                    }
                }
                gRequest.Method = "POST";
                gRequest.KeepAlive = true;
                gRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

                ChangeProxy(proxyAddress, proxyPort, proxyUsername, proxyPassword);

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    gRequest.CookieContainer.Add(gCookies);
                }
                #endregion

                Stream rs = gRequest.GetRequestStream();


                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, fileName, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(localImagePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    gRequest.CookieContainer.Add(gCookies);
                }

                #endregion

                WebResponse wresp = null;
                try
                {
                    wresp = gRequest.GetResponse();
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    responseStr = reader2.ReadToEnd();
                    return responseStr;
                    //log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                    //return true;
                }
                catch (Exception ex)
                {
                    //log.Error("Error uploading file", ex);
                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }
                    // return false;


                    Console.WriteLine("Error : " + ex.StackTrace);

                }
                finally
                {
                    gRequest = null;
                }
                return responseStr;

            }
            catch (Exception ex)
            {
                gRequest = null;
                Console.WriteLine("Error : " + ex.StackTrace);
            }
            return responseStr;
        }

        public string getHtmlfromUrl(Uri url, string Referes, string Token, string AccountUserAgent)
        {
            string responseString = string.Empty;
            try
            {
                setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(AccountUserAgent))
                {
                    gRequest.UserAgent = AccountUserAgent;
                }
                else
                {
                    gRequest.UserAgent = UserAgent;
                }
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                // gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                //gRequest.Headers["Cache-Control"] = "max-age=0";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
                //gRequest.Connection = "keep-alive";
                gRequest.Timeout = Timeout;
                gRequest.KeepAlive = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
                gRequest.Headers.Add("Javascript-enabled", "true");
                gRequest.Method = "GET";
                // gRequest.Headers.Add("Accept-Encoding","gzip, deflate, sdch");
                //gRequest.AllowAutoRedirect = false;
                ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

                if (!string.IsNullOrEmpty(Referes))
                {
                    gRequest.Referer = Referes;
                }
                //if (!string.IsNullOrEmpty(Token))
                {
                    //gRequest.Headers.Add("X-CSRFToken", Token);
                    gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
                }
                gRequest.Headers["X-NEW-APP"] = "1";

                if (gCookies != null)
                {
                    foreach (Cookie item in gCookies)
                    {
                        if (item.Name == "csrftoken")
                        {
                            string csrftokenValue = item.Value;

                            gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                            break;
                        }
                    }
                }


                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);

                    try
                    {
                        //gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0 - 2078004405 - 1321685323158", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1322116799.1322206824.9", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                        //gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                    }
                    catch (Exception ex)
                    {

                    }
                }
                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                valueURl = gResponse.ResponseUri.ToString();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception ex)
            {

            }
            return responseString;
        }


        public string getHtmlfromUrlProxy(Uri url, string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {
            string responseString = string.Empty;
            try
            {
                setExpect100Continue();
                gRequest = (HttpWebRequest)WebRequest.Create(url);
                //gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";

                //gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";
                //gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                ////gRequest.Headers["Cache-Control"] = "max-age=0";
                //gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
                ////gRequest.Connection = "keep-alive";
                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0";
                gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";

                gRequest.KeepAlive = true;

                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                ///Set Proxy
                this.proxyAddress = proxyAddress;
                this.port = port;
                this.proxyUsername = proxyUsername;
                this.proxyPassword = proxyPassword;

                ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

                gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

                gRequest.Method = "GET";
                //gRequest.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                #region CookieManagment

                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);
                }
                //Get Response for this request url

                setExpect100Continue();
                gResponse = (HttpWebResponse)gRequest.GetResponse();

                //check if the status code is http 200 or http ok
                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                    //check if response object has any cookies or not
                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion

                    responseURI = gResponse.ResponseUri.AbsoluteUri;

                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    responseString = reader.ReadToEnd();
                    reader.Close();
                    return responseString;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {


                if (ex.Message.Contains("The remote server returned an error: (999)"))
                {
                    // GlobusLogHelper.log.Info("Yahoo trace your IP ,Plz change your proxy IP");
                    responseString = "The remote server returned an error: (999)";
                }
            }
            return responseString;
        }

        public string getHtmlfromUrl1(Uri url, string Referes, string Token, string AccountUserAgent, string App_version)
        {
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);
            if (!string.IsNullOrEmpty(AccountUserAgent))
            {
                gRequest.UserAgent = AccountUserAgent;
            }
            else
            {
                gRequest.UserAgent = UserAgent;
            }
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            gRequest.Headers["X-APP-VERSION"] = App_version;
            gRequest.Headers["X-NEW-APP"] = "1";
            gRequest.Headers["Accept-Charset"] = " ISO-8859-1,utf-8;q=0.7,*;q=0.7";
                //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7

            //gRequest.Connection = "keep-alive";
            //gRequest.Timeout = Timeout;
            //gRequest.KeepAlive = true;
      

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;
            //gRequest.Headers.Add("Javascript-enabled", "true");
            gRequest.Method = "GET";
            //gRequest.AllowAutoRedirect = false;
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }
            //if (!string.IsNullOrEmpty(Token))
            {
                //gRequest.Headers.Add("X-CSRFToken", Token);
                gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
            //gRequest.Headers["X-NEW-APP"] = "1";

            if (gCookies != null)
            {
                foreach (Cookie item in gCookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        string csrftokenValue = item.Value;

                        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                        break;
                    }
                }
            }


            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);

                try
                {
                    //gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0 - 2078004405 - 1321685323158", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1322116799.1322206824.9", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                    //gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                }
                catch (Exception ex)
                {

                }
            }
            //Get Response for this request url

            setExpect100Continue();
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch { }

            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error";
            }

        }

        string proxyAddress = string.Empty;
        int port = 80;
        string proxyUsername = string.Empty;
        string proxyPassword = string.Empty;

        public string getHtmlfromUrlProxy(Uri url, string Refrer, string proxyAddress, int port, string proxyUsername, string proxyPassword, string AccountUserAgent)
        {
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);
            //gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";

            if (!string.IsNullOrEmpty(AccountUserAgent))
            {
                gRequest.UserAgent = AccountUserAgent;
            }
            else
            {
                gRequest.UserAgent = UserAgent;
            }
            //gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.24) Gecko/20111103 Firefox/3.6.24";
            //gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            ////gRequest.Headers["Cache-Control"] = "max-age=0";
            //gRequest.Headers["Accept-Language"] = "en-us,en;q=0.5";
            gRequest.Timeout = Timeout;
            ////gRequest.Connection = "keep-alive";
            //gRequest.Referer = Refrer;
            //gRequest.KeepAlive = true;

            //gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            //gRequest.Headers["X-NEW-APP"] = "1";

            //if (gCookies != null)
            //{
            //    foreach (Cookie item in gCookies)
            //    {
            //        if (item.Name == "csrftoken")
            //        {
            //            string csrftokenValue = item.Value;

            //            gRequest.Headers["X-CSRFToken"] = csrftokenValue;

            //            break;
            //        }
            //    }
            //}

            //gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            //gRequest.Method = "POST";
            gRequest.Accept = "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            gRequest.Headers.Add("Accept-Encoding", "sdch");

            if (!string.IsNullOrEmpty(Refrer))
            {
                gRequest.Referer = Refrer;
            }
            //if (!string.IsNullOrEmpty(Token))
            {
                //gRequest.Headers.Add("Origin", "http://pinterest.com");
                //gRequest.Headers.Add("X-CSRFToken", Token);
                gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }


            gRequest.Headers["X-NEW-APP"] = "1";

            if (gCookies != null)
            {
                foreach (Cookie item in gCookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        string csrftokenValue = item.Value;

                        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                        break;
                    }
                }
            }
            ///Set Proxy
            this.proxyAddress = proxyAddress;
            this.port = port;
            this.proxyUsername = proxyUsername;
            this.proxyPassword = proxyPassword;

            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            gRequest.Method = "GET";
            //gRequest.Accept = "image/jpeg, application/x-ms-application, image/gif, application/xaml+xml, image/pjpeg, application/x-ms-xbap, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);

                //try
                //{
                //    gRequest.CookieContainer.Add(url, new Cookie("__qca", "P0-2078004405-1321685323158", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utma", "101828306.1814567160.1321685324.1321697619.1321858563.3", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmz", "101828306.1321685324.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmb", "101828306.2.10.1321858563", "/"));
                //    gRequest.CookieContainer.Add(url, new Cookie("__utmc", "101828306", "/"));
                //}
                //catch (Exception ex)
                //{

                //}
            }
            //Get Response for this request url

            setExpect100Continue();

            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch { };

            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);


                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                return responseString;
            }
            else
            {
                return "Error";
            }

        }

        public string getHtmlfromAsx(Uri url)
        {
            setExpect100Continue();
            gRequest = (HttpWebRequest)WebRequest.Create(url);
            gRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.4) Gecko/2008102920 Firefox/3.0.4";
            gRequest.CookieContainer = new CookieContainer();//new CookieContainer();
            gRequest.ContentType = "video/x-ms-asf";

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
                setExpect100Continue();
            }
            //Get Response for this request url
            gResponse = (HttpWebResponse)gRequest.GetResponse();
            setExpect100Continue();
            //check if the status code is http 200 or http ok
            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                setExpect100Continue();
                //check if response object has any cookies or not
                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error";
            }
        }

        //public void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        //{

        //    ////log.Debug(string.Format("Uploading {0} to {1}", file, url));
        //    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        //    //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
        //    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

        //    gRequest = (HttpWebRequest)WebRequest.Create(url);
        //    gRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        //    gRequest.Method = "POST";
        //    gRequest.KeepAlive = true;
        //    gRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

        //    //ChangeProxy("127.0.0.1", 8888, "", "");

        //    gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

        //    #region CookieManagment

        //    if (this.gCookies != null && this.gCookies.Count > 0)
        //    {
        //        gRequest.CookieContainer.Add(gCookies);
        //    }
        //    #endregion

        //    Stream rs = gRequest.GetRequestStream();

        //    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        //    foreach (string key in nvc.Keys)
        //    {
        //        rs.Write(boundarybytes, 0, boundarybytes.Length);
        //        string formitem = string.Format(formdataTemplate, key, nvc[key]);
        //        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
        //        rs.Write(formitembytes, 0, formitembytes.Length);
        //    }
        //    rs.Write(boundarybytes, 0, boundarybytes.Length);

        //    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
        //    string header = string.Format(headerTemplate, paramName, file, contentType);
        //    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
        //    rs.Write(headerbytes, 0, headerbytes.Length);

        //    FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
        //    byte[] buffer = new byte[4096];
        //    int bytesRead = 0;
        //    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //    {
        //        rs.Write(buffer, 0, bytesRead);
        //    }
        //    fileStream.Close();

        //    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        //    rs.Write(trailer, 0, trailer.Length);
        //    rs.Close();

        //    #region CookieManagment

        //    if (this.gCookies != null && this.gCookies.Count > 0)
        //    {
        //        gRequest.CookieContainer.Add(gCookies);
        //    }

        //    #endregion

        //    WebResponse wresp = null;
        //    try
        //    {
        //        wresp = gRequest.GetResponse();
        //        Stream stream2 = wresp.GetResponseStream();
        //        StreamReader reader2 = new StreamReader(stream2);
        //        //log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
        //    }
        //    catch (Exception ex)
        //    {
        //        //log.Error("Error uploading file", ex);
        //        if (wresp != null)
        //        {
        //            wresp.Close();
        //            wresp = null;
        //        }
        //    }
        //    finally
        //    {
        //        gRequest = null;
        //    }
        //    //}

        //}

        public string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc, bool IsLocalFile)
        {

            ////log.Debug(string.Format("Uploading {0} to {1}", file, url));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
            //string boundary = "---------------------------" + DateTime.Now.Ticks.ToString();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            gRequest = (HttpWebRequest)WebRequest.Create(url);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0";
            gRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            gRequest.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            gRequest.Headers.Add("X-File-Name", contentType);
           

            gRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Referer = "https://www.pinterest.com/settings/";
            //gRequest.Headers.Add("Javascript-enabled", "true");
            byte[] firstBoundarybytes = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            byte[] tempBoundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            gRequest.CookieContainer = new CookieContainer(); //gCookiesContainer;

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
            }
            #endregion


            if (gCookies != null)
            {
                foreach (Cookie item in gCookies)
                {
                    if (item.Name.Contains("csrftoken"))
                    {
                        gRequest.Headers.Add("X-CSRFToken", item.Value);
                         break;
                    }
                }
            }

            Stream rs = gRequest.GetRequestStream();


            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            int temp = 0;
            foreach (string key in nvc.Keys)
            {
                if (key.Contains("csrfmiddlewaretoken") || key.Contains("about") || key.Contains("location") || key.Contains("website"))
                {
                    continue;
                }

                if (temp == 0)
                {
                    rs.Write(firstBoundarybytes, 0, firstBoundarybytes.Length);
                }
                //else if (temp == nvc.Count - 1)
                //{
                //    //rs.Write(tempBoundarybytes, 0, tempBoundarybytes.Length);
                //    //break;
                //}
                else
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                }
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
                temp++;
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);



            if (IsLocalFile)
            {
                try
                {
                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string header = string.Format(headerTemplate, "img", contentType, "image/jpeg");
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    if (!string.IsNullOrEmpty(file))
                    {
                        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            rs.Write(buffer, 0, bytesRead);
                        }
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }

            foreach (string key in nvc.Keys)
            {
                if (key.Contains("csrfmiddlewaretoken") || key.Contains("about") || key.Contains("location") || key.Contains("website"))
                {
                    rs.Write(tempBoundarybytes, 0, tempBoundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                    temp++;
                }
            }
            //rs.Write(boundarybytes, 0, boundarybytes.Length);


            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            #region CookieManagment

            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                gRequest.CookieContainer.Add(gCookies);
            }

            #endregion

            WebResponse wresp = null;
            try
            {
                //wresp = gRequest.GetResponse();
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                //reader.Close();
                return responseString;

                //wresp = gRequest.GetResponse();
                //Stream stream2 = wresp.GetResponseStream();
                //StreamReader reader2 = new StreamReader(stream2);
                //log.Debug(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                //return true;
                //return null;
            }
            catch (Exception ex)
            {
                //log.Error("Error uploading file", ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                return null;
            }
        }

        public bool MultiPartImageUpload(string Email, string language, string firstname, string lastname, string username, string gender, string about, string location, string website, string localImagePath, string CsrfMiddleToken)
        {
            string imagename = string.Empty;
            NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("email", Email);
            //nvc.Add("language", "en-US");
            //nvc.Add("gender", gender);
            //nvc.Add("first_name", firstname);
            //nvc.Add("last_name", lastname);
            //nvc.Add("username", username);
            //nvc.Add("about", about);
            //nvc.Add("location", location);
            //nvc.Add("website", website);
            //nvc.Add("csrfmiddlewaretoken", CsrfMiddleToken);
            ////nvc.Add("_ch", chtoken);
            try
            {
                if (!string.IsNullOrEmpty(localImagePath))
                {
                    string[] array = Regex.Split(localImagePath, @"\\");
                    imagename = array[array.Length - 1];

                }
            }
            catch { }

            //string PageSource = "";
            //string url = "";
            //string PostData = "";


            //string PageSource = HttpUploadFile("https://www.pinterest.com/settings/", localImagePath, "img", imagename, nvc, true);
            string PageSource = HttpUploadFile("https://www.pinterest.com/upload-image/?img=" + imagename + "", localImagePath, "img", imagename, nvc, true);

            string image_url = string.Empty;
            if (PageSource.Contains("image_url"))
            {
                try
                {
                    image_url = PageSource.Substring(PageSource.IndexOf("image_url") + 13, PageSource.IndexOf("\"success\""));
                    image_url = System.Text.RegularExpressions.Regex.Split(image_url, "\",")[0];
                }
                catch { };
            }


            //string imgPoastData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22profile_image_url%22%3A%22"+Uri.EscapeDataString(image_url)+"%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22"+  +"%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=Modal()%3EUserEdit(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserProfileImageUploader)%23Modal(module%3DUserProfileImageUploader())";

            if (!string.IsNullOrEmpty(PageSource))
            {
                return true;
            }
            return false;
        }

        public bool MultiPartImageUpload_new (string Email, string language, string firstname, string lastname, string username, string gender, string about, string location, string website, string localImagePath, string CsrfMiddleToken, string AppVersion)
        {
            string imagename = string.Empty;
            NameValueCollection nvc = new NameValueCollection();
            //nvc.Add("email", Email);
            //nvc.Add("language", "en-US");
            //nvc.Add("gender", gender);
            //nvc.Add("first_name", firstname);
            //nvc.Add("last_name", lastname);
            //nvc.Add("username", username);
            //nvc.Add("about", about);
            //nvc.Add("location", location);
            //nvc.Add("website", website);
            //nvc.Add("csrfmiddlewaretoken", CsrfMiddleToken);
            ////nvc.Add("_ch", chtoken);
            try
            {
                if (!string.IsNullOrEmpty(localImagePath))
                {
                    string[] array = Regex.Split(localImagePath, @"\\");
                    imagename = array[array.Length - 1];

                }
            }
            catch { }

            //string PageSource = "";
            //string url = "";
            //string PostData = "";


            //string PageSource = HttpUploadFile("https://www.pinterest.com/settings/", localImagePath, "img", imagename, nvc, true);
            string PageSource = HttpUploadFile("https://www.pinterest.com/upload-image/?img=" + imagename + "", localImagePath, "img", imagename, nvc, true);

            if (string.IsNullOrEmpty(PageSource))
            {
                PageSource = HttpUploadFile("https://www.pinterest.com/upload-image/?img=" + imagename + "", localImagePath, "img", imagename, nvc, true);
            }


            string image_url = string.Empty;
            if (PageSource.Contains("image_url"))
            {
                try
                {
                    image_url = PageSource.Substring(PageSource.IndexOf("image_url") + ("image_url\"").Length, PageSource.IndexOf("\"success\"")).Replace(": \"", string.Empty);
                    image_url = System.Text.RegularExpressions.Regex.Split(image_url, "\",")[0];
                }
                catch { };
            }


            string imgPoastData = "source_url=%2Fsettings%2F&data=%7B%22options%22%3A%7B%22profile_image_url%22%3A%22" + Uri.EscapeDataString(image_url) + "%22%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + AppVersion + "%22%2C%22https_exp%22%3Afalse%7D%7D&module_path=Modal()%3EUserEdit(resource%3DUserSettingsResource())%3EShowModalButton(module%3DUserProfileImageUploader)%23Modal(module%3DUserProfileImageUploader())";

            string Update_ImgStr = postFormData(new Uri("https://www.pinterest.com/resource/UserSettingsResource/update/"), imgPoastData, "https://www.pinterest.com/settings/", "", "");


            if (!string.IsNullOrEmpty(PageSource))
            {
                return true;
            }
            return false;
        }

        public bool MultiPartImageUploadPreview(string Email, string language, string firstname, string lastname, string username, string gender, string about, string location, string website, string localImagePath, string CsrfMiddleToken)
        {
            string imagename = string.Empty;
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("email", Email);
            nvc.Add("language", "en-US");
            nvc.Add("first_name", firstname);
            nvc.Add("last_name", lastname);
            nvc.Add("username", username);
            nvc.Add("gender", gender);
            nvc.Add("about", about);
            nvc.Add("location", location);
            nvc.Add("website", "www.something.com");
            nvc.Add("csrfmiddlewaretoken", CsrfMiddleToken);
            try
            {
                string[] array = Regex.Split(localImagePath, @"\\");
                imagename = array[array.Length - 1];
            }
            catch { }

            string PageSource = HttpUploadFile("https://pinterest.com/pin/preview/", localImagePath, "img", imagename, nvc, true);

            if (true)
            {
                return true;
            }
            return false;
        }

        public string postFormData(Uri formActionUrl, string postData, string Referes, string Token, string AccountUserAgent)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);

            if (!string.IsNullOrEmpty(AccountUserAgent))
            {
                gRequest.UserAgent = AccountUserAgent;
            }
            else
            {
                gRequest.UserAgent = UserAgent;
            }

            //gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
            //gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";

            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            //gRequest.Accept = "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded";
            gRequest.Headers.Add("Javascript-enabled", "true");
            gRequest.Timeout = Timeout;
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


            //gRequest.Headers.Add("Accept-Encoding", "sdch");

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }
            if (!string.IsNullOrEmpty(Token))
            {
                gRequest.Headers.Add("Origin", "http://pinterest.com");
                //gRequest.Headers.Add("X-CSRFToken", Token);
            }

            gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers.Add("X-CSRFToken",csrftokenValue);

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();     
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string MypostFormData(Uri formActionUrl, string postData, string Referes, string Token, string AccountUserAgent)
        {
            try
            {
                gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);

                gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";

                gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
                gRequest.Method = "POST";
                gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
                gRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                //gRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                
                gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
                //gRequest.Headers.Add("X-Requested-With: XMLHttpRequest", "XMLHttpRequest");
                gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {
                    gRequest.KeepAlive = true;
                }
                catch { }

                gRequest.Headers.Add("Origin", "http://pinterest.com");



                gRequest.Timeout = Timeout;
                gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (!string.IsNullOrEmpty(Referes))
                {
                    gRequest.Referer = Referes;
                }

                /*if (!string.IsNullOrEmpty(Token))    Origin already added above
                {
                    gRequest.Headers.Add("Origin", "http://pinterest.com");
                    //gRequest.Headers.Add("X-CSRFToken", Token);
                }*/

                gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

                gRequest.Headers["X-APP-VERSION"] = "8343013";
                gRequest.Headers["X-NEW-APP"] = "1";

                foreach (Cookie item in gCookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        string csrftokenValue = item.Value;

                        gRequest.Headers.Add("X-CSRFToken", csrftokenValue);

                        break;
                    }
                }
                gRequest.Headers["X-CSRFToken"] = "XBFppEgzhFsb777OOj6WUQo3LJrAdO0x";



                ///Modified BySumit 18-11-2011
                ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

                #region CookieManagement
                if (this.gCookies != null && this.gCookies.Count > 0)
                {
                    setExpect100Continue();
                    gRequest.CookieContainer.Add(gCookies);
                }

                //logic to postdata to the form
                try
                {
                    setExpect100Continue();
                    string postdata = string.Format(postData);
                    byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                    gRequest.ContentLength = postBuffer.Length;
                    Stream postDataStream = gRequest.GetRequestStream();
                    postDataStream.Write(postBuffer, 0, postBuffer.Length);
                    postDataStream.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
                }
                //post data logic ends

                //Get Response for this request url
                try
                {
                    gResponse = (HttpWebResponse)gRequest.GetResponse();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
                }



                //check if the status code is http 200 or http ok

                if (gResponse.StatusCode == HttpStatusCode.OK)
                {
                    //get all the cookies from the current request and add them to the response object cookies
                    setExpect100Continue();
                    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                    //check if response object has any cookies or not
                    //Added by sandeep pathak
                    //gCookiesContainer = gRequest.CookieContainer;  

                    if (gResponse.Cookies.Count > 0)
                    {
                        //check if this is the first request/response, if this is the response of first request gCookies
                        //will be null
                        if (this.gCookies == null)
                        {
                            gCookies = gResponse.Cookies;
                        }
                        else
                        {
                            foreach (Cookie oRespCookie in gResponse.Cookies)
                            {
                                bool bMatch = false;
                                foreach (Cookie oReqCookie in this.gCookies)
                                {
                                    if (oReqCookie.Name == oRespCookie.Name)
                                    {
                                        oReqCookie.Value = oRespCookie.Value;
                                        bMatch = true;
                                        break; // 
                                    }
                                }
                                if (!bMatch)
                                    this.gCookies.Add(oRespCookie);
                            }
                        }
                    }
                #endregion



                    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                    string responseString = reader.ReadToEnd();
                    reader.Close();
                    //Console.Write("Response String:" + responseString);
                    return responseString;
                }
                else
                {
                    return "Error in posting data";
                }
            }
            catch (Exception ae)
            {
                return null;
            }

        }

        public string postFormData1(Uri formActionUrl, string postData, string Referes, string Token, string AccountUserAgent, string App_version)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);

            if (!string.IsNullOrEmpty(AccountUserAgent))
            {
                gRequest.UserAgent = AccountUserAgent;
            }
            else
            {
                gRequest.UserAgent = UserAgent;
            }

            gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:29.0) Gecko/20100101 Firefox/29.0";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            gRequest.Headers["X-NEW-APP"] = "1";
           // gRequest.Headers["Connection"] = "keep-alive";
            gRequest.Headers["Cache-Control"] = "no-cache";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            //gRequest.Accept = "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
           
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Timeout = Timeout;
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (!string.IsNullOrEmpty(App_version))
            {
                gRequest.Headers["App_version"] = App_version;
            }

            //gRequest.Headers.Add("Accept-Encoding", "sdch");

            if (!string.IsNullOrEmpty(Referes))
            {
                gRequest.Referer = Referes;
            }
            if (!string.IsNullOrEmpty(Token))
            {
                gRequest.Headers.Add("Origin", "http://pinterest.com");
                //gRequest.Headers.Add("X-CSRFToken", Token);
            }

            gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers.Add("X-CSRFToken", csrftokenValue);

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormData(Uri formActionUrl, string postData, string referer) //AQeU1YnAYI90JJ5oCa5e3MFTJpxN4JhP
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";//UserAgent;";//"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0;";//UserAgent;
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";//"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";: 
            gRequest.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";

            gRequest.KeepAlive = true;
            //gRequest.Host = "www.pinterest.com";
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Timeout = Timeout;
            gRequest.Method = "POST";
           gRequest.Headers.Add("Javascript-enabled", "true");
           gRequest.Headers.Add("X-CSRFToken", "AQeU1YnAYI90JJ5oCa5e3MFTJpxN4JhP");

            //gRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            //gRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");

           gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //gRequest.Headers.Add("Origin", "http://pinterest.com");

            gRequest.Headers["X-NEW-APP"] = "1";

            gRequest.KeepAlive = true;
            gRequest.Headers.Add("Javascript-enabled", "true");
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer();

            if (!string.IsNullOrEmpty(referer))
            {
                gRequest.Referer = referer;
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }

           //string valueURl =  gResponse.ResponseUri.ToString();

            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }


        public string postFormDataWithCsrftoken(Uri formActionUrl, string postData, string referer, string CsrfToken) //AQeU1YnAYI90JJ5oCa5e3MFTJpxN4JhP
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";//UserAgent;";//"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0;";//UserAgent;
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";//"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";: 
            gRequest.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";

            gRequest.KeepAlive = true;
            //gRequest.Host = "www.pinterest.com";
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Timeout = Timeout;
            gRequest.Method = "POST";
            gRequest.Headers.Add("Javascript-enabled", "true");

            if (!string.IsNullOrEmpty(CsrfToken))
            {
                gRequest.Headers.Add("X-CSRFToken", CsrfToken);
            }

            //gRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            //gRequest.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");

            gRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            //gRequest.Headers.Add("Origin", "http://pinterest.com");

            gRequest.Headers["X-NEW-APP"] = "1";

            gRequest.KeepAlive = true;
            gRequest.Headers.Add("Javascript-enabled", "true");
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer();

            if (!string.IsNullOrEmpty(referer))
            {
                gRequest.Referer = referer;
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }

            //string valueURl =  gResponse.ResponseUri.ToString();

            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }

        public string postFormDataPinSendVerification(Uri formActionUrl, string postData, string referer)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            //gRequest.UserAgent = UserAgent;
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.115 Safari/537.36";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            //gRequest.Headers["Accept-Charset"] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
            //gRequest.Headers["Cache-Control"] = "max-age=0";
            gRequest.Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Method = "POST";
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.Headers["X-APP-VERSION"] = "d2dd61f";
            gRequest.Headers.Add("Javascript-enabled", "true");
            gRequest.Headers["Origin"] = "https://www.pinterest.com";
            gRequest.KeepAlive = true;

            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.CookieContainer = new CookieContainer();

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers.Add("X-CSRFToken", csrftokenValue);

                    break;
                }
            }

            if (!string.IsNullOrEmpty(referer))
            {
                gRequest.Referer = referer;
            }
            gRequest.Headers["X-NEW-APP"] = "1";
            gRequest.Headers["X-Pinterest-AppState"] = "active";
            //gRequest.Headers["Host"] = "www.pinterest.com";

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion



                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataProxyREPin(Uri formActionUrl, string postData, string Refrer, string AppVersion)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;
            gRequest.Timeout = 90000;
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";

            if (!string.IsNullOrEmpty(AppVersion))
            {
                gRequest.Headers["X-APP-VERSION"] = AppVersion;
            }
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends


            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                // gResponse.Close();
                // gRequest.Abort();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataProxyPin(Uri formActionUrl, string postData, string Refrer)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;
            gRequest.Timeout = 90000;
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends
          

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
                valueURl = gResponse.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                // gResponse.Close();
                // gRequest.Abort();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataProxyPinAddNewPin(Uri formActionUrl, string postData, string Refrer, string appVersion, string csftoken)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = " Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
            gRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;
            gRequest.Timeout = 90000;
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            gRequest.Headers["X-APP-VERSION"] = appVersion;
            gRequest.Headers["X-NEW-APP"] = "1";

            if (!string.IsNullOrEmpty(csftoken))
            {
                gRequest.Headers.Add("X-CSRFToken", csftoken);
            }
            else if (string.IsNullOrEmpty(csftoken))
            {
                foreach (Cookie item in gCookies)
                {
                    if (item.Name == "csrftoken")
                    {
                        string csrftokenValue = item.Value;

                        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                        break;
                    }
                }
            }
            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends


            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                // gResponse.Close();
                // gRequest.Abort();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataProxyBoardCreation(Uri formActionUrl, string postData, string referer, string appVersion)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.Host = "www.pinterest.com";
            gRequest.Headers["Origin"] = "https://www.pinterest.com";
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            gRequest.Headers["X-APP-VERSION"] = appVersion;
            gRequest.Headers["X-Pinterest-AppState"] = "active";
            gRequest.Headers["X-NEW-APP"] = "1";
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }
            gRequest.Referer = referer;
            gRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            gRequest.Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.KeepAlive = true;
            gRequest.Timeout = 90000;
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends


            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                // gResponse.Close();
                // gRequest.Abort();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }

        public string postFormDataProxyPincsrftoken(Uri formActionUrl, string postData, string Refrer, string csrftokenValue)
        {

            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;
            gRequest.Timeout = 90000;
            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            if (!string.IsNullOrEmpty(csrftokenValue))
            {
                gRequest.Headers.Add("X-CSRFToken", csrftokenValue);
            }

            //foreach (Cookie item in gCookies)
            //{
            //    if (item.Name == "csrftoken")
            //    {
            //        string csrftokenValue = item.Value;

            //        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

            //        break;
            //    }
            //}

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

        }


        public string postDataForPinterest(Uri formActionUrl, string postData, string Refrer)//, string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {
            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;
            gRequest.Headers["X-APP-VERSION"] = "d2dd61f";
            gRequest.Headers.Add("Javascript-enabled", "true");

            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }

        public string postFormDataProxy(Uri formActionUrl, string postData, string Refrer, string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {
            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;

            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }

            //#region MyRegion
            //gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            //gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            //gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            //gRequest.Method = "POST";
            //gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            //gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            //gRequest.KeepAlive = true;
            //gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            //gRequest.Referer = Refrer;

            //gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            //gRequest.Timeout = Timeout;
            //gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            //gRequest.Headers["X-NEW-APP"] = "1";

            //foreach (Cookie item in gCookies)
            //{
            //    if (item.Name == "csrftoken")
            //    {
            //        string csrftokenValue = item.Value;

            //        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

            //        break;
            //    }
            //}

            /////Modified BySumit 18-11-2011
            //ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            //#region CookieManagement
            //if (this.gCookies != null && this.gCookies.Count > 0)
            //{
            //    setExpect100Continue();
            //    gRequest.CookieContainer.Add(gCookies);
            //}

            ////logic to postdata to the form
            //try
            //{
            //    setExpect100Continue();
            //    string postdata = string.Format(postData);
            //    byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
            //    gRequest.ContentLength = postBuffer.Length;
            //    Stream postDataStream = gRequest.GetRequestStream();
            //    postDataStream.Write(postBuffer, 0, postBuffer.Length);
            //    postDataStream.Close();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            //}
            ////post data logic ends

            ////Get Response for this request url
            //try
            //{
            //    gResponse = (HttpWebResponse)gRequest.GetResponse();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            //}



            ////check if the status code is http 200 or http ok

            //if (gResponse.StatusCode == HttpStatusCode.OK)
            //{
            //    //get all the cookies from the current request and add them to the response object cookies
            //    setExpect100Continue();
            //    gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
            //    //check if response object has any cookies or not
            //    //Added by sandeep pathak
            //    //gCookiesContainer = gRequest.CookieContainer;  

            //    if (gResponse.Cookies.Count > 0)
            //    {
            //        //check if this is the first request/response, if this is the response of first request gCookies
            //        //will be null
            //        if (this.gCookies == null)
            //        {
            //            gCookies = gResponse.Cookies;
            //        }
            //        else
            //        {
            //            foreach (Cookie oRespCookie in gResponse.Cookies)
            //            {
            //                bool bMatch = false;
            //                foreach (Cookie oReqCookie in this.gCookies)
            //                {
            //                    if (oReqCookie.Name == oRespCookie.Name)
            //                    {
            //                        oReqCookie.Value = oRespCookie.Value;
            //                        bMatch = true;
            //                        break; // 
            //                    }
            //                }
            //                if (!bMatch)
            //                    this.gCookies.Add(oRespCookie);
            //            }
            //        }
            //    }
            //#endregion

            //    StreamReader reader = new StreamReader(gResponse.GetResponseStream());
            //    string responseString = reader.ReadToEnd();
            //    reader.Close();
            //    //Console.Write("Response String:" + responseString);
            //    return responseString;
            //}
            //else
            //{
            //    return "Error in posting data";
            //} 
            //#endregion

        }

        public string postFormDataProxyPinwithCSRFToken(Uri formActionUrl, string postData, string Refrer, string proxyAddress, int port, string proxyUsername, string proxyPassword, string csrfToken)
        {
            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0";

            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;

            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            gRequest.Headers.Add("X-CSRFToken", csrfToken);
            //gRequest.Headers["X-CSRFToken"] = csrfToken;

            //foreach (Cookie item in gCookies)
            //{
            //    if (item.Name == "csrftoken")
            //    {
            //        string csrftokenValue = item.Value;

            //        gRequest.Headers["X-CSRFToken"] = csrftokenValue;

            //        break;
            //    }
            //}

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }

        public string postFormDataProxywithCSRFToken(Uri formActionUrl, string postData, string Refrer, string proxyAddress, int port, string proxyUsername, string proxyPassword) // string csrfToken)
        {
            gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
            gRequest.UserAgent = " Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
           // gRequest.Headers.Add("Origin", "https://www.pinterest.com");
            gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
            gRequest.Method = "POST";
            gRequest.Accept = " application/json, text/javascript, */*; q=0.01";
            gRequest.Headers["Accept-Language"] = "en-US,en;q=0.5";
            gRequest.KeepAlive = true;
            gRequest.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            gRequest.Referer = Refrer;

            gRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
            gRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            gRequest.Headers["X-NEW-APP"] = "1";

            //gRequest.Headers["X-CSRFToken"] = csrfToken;

            foreach (Cookie item in gCookies)
            {
                if (item.Name == "csrftoken")
                {
                    string csrftokenValue = item.Value;

                    gRequest.Headers["X-CSRFToken"] = csrftokenValue;

                    break;
                }
            }

            ///Modified BySumit 18-11-2011
            ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

            #region CookieManagement
            if (this.gCookies != null && this.gCookies.Count > 0)
            {
                setExpect100Continue();
                gRequest.CookieContainer.Add(gCookies);
            }

            //logic to postdata to the form
            try
            {
                setExpect100Continue();
                string postdata = string.Format(postData);
                byte[] postBuffer = System.Text.Encoding.GetEncoding(1252).GetBytes(postData);
                gRequest.ContentLength = postBuffer.Length;
                Stream postDataStream = gRequest.GetRequestStream();
                postDataStream.Write(postBuffer, 0, postBuffer.Length);
                postDataStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Logger.LogText("Internet Connectivity Exception : "+ ex.Message,null);
            }
            //post data logic ends

            //Get Response for this request url
            try
            {
                gResponse = (HttpWebResponse)gRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //Logger.LogText("Response from "+formActionUrl + ":" + ex.Message,null);
            }



            //check if the status code is http 200 or http ok

            if (gResponse.StatusCode == HttpStatusCode.OK)
            {
                //get all the cookies from the current request and add them to the response object cookies
                setExpect100Continue();
                gResponse.Cookies = gRequest.CookieContainer.GetCookies(gRequest.RequestUri);
                //check if response object has any cookies or not
                //Added by sandeep pathak
                //gCookiesContainer = gRequest.CookieContainer;  

                if (gResponse.Cookies.Count > 0)
                {
                    //check if this is the first request/response, if this is the response of first request gCookies
                    //will be null
                    if (this.gCookies == null)
                    {
                        gCookies = gResponse.Cookies;
                    }
                    else
                    {
                        foreach (Cookie oRespCookie in gResponse.Cookies)
                        {
                            bool bMatch = false;
                            foreach (Cookie oReqCookie in this.gCookies)
                            {
                                if (oReqCookie.Name == oRespCookie.Name)
                                {
                                    oReqCookie.Value = oRespCookie.Value;
                                    bMatch = true;
                                    break; // 
                                }
                            }
                            if (!bMatch)
                                this.gCookies.Add(oRespCookie);
                        }
                    }
                }
            #endregion

                StreamReader reader = new StreamReader(gResponse.GetResponseStream());
                string responseString = reader.ReadToEnd();
                reader.Close();
                //Console.Write("Response String:" + responseString);
                return responseString;
            }
            else
            {
                return "Error in posting data";
            }
        }

        public void setExpect100Continue()
        {
            if (ServicePointManager.Expect100Continue == true)
            {
                ServicePointManager.Expect100Continue = false;
            }
        }

        public void setExpect100ContinueToTrue()
        {
            if (ServicePointManager.Expect100Continue == false)
            {
                ServicePointManager.Expect100Continue = true;
            }
        }

        public void ChangeProxy(string proxyAddress, int port, string proxyUsername, string proxyPassword)
        {
            try
            {
                WebProxy myproxy = new WebProxy(proxyAddress, port);
                myproxy.BypassProxyOnLocal = false;
             
                if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
                {
                    myproxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
                gRequest.Proxy = myproxy;
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetParamValue(string pgSrc, string paramName)
        {
            try
            {
                if (pgSrc.Contains("name='" + paramName + "'"))
                {
                    string param = "name='" + paramName + "'";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endparamName = pgSrc.IndexOf("'", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endparamName - startparamName);
                    return valueparamName;
                }
                else if (pgSrc.Contains("name=\"" + paramName + "\""))
                {
                    string param = "name=\"" + paramName + "\"";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
                    int endcommentPostID = pgSrc.IndexOf("\"", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
                    return valueparamName;
                }
                else if (pgSrc.Contains("name=\\\"" + paramName + "\\\""))
                {
                    string param = "name=\\\"" + paramName + "\\\"";
                    int startparamName = pgSrc.IndexOf(param) + param.Length;
                    startparamName = pgSrc.IndexOf("value=\\", startparamName) + "value=\\".Length + 1;
                    int endcommentPostID = pgSrc.IndexOf("\\\"", startparamName);
                    string valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
                    return valueparamName;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ParseJson(string data, string paramName)
        {
            int startIndx = data.IndexOf(paramName) + paramName.Length + 3;
            int endIndx = data.IndexOf("\"", startIndx);

            string value = data.Substring(startIndx, endIndx - startIndx);
            return value;
        }

        public static string ParseEncodedJson(string data, string paramName)
        {
            string value = string.Empty;
            {
                data = data.Replace("&quot;", "\"");
                int startIndx = data.IndexOf("\"" + paramName + "\"") + ("\"" + paramName + "\"").Length + 1;
                int endIndx = data.IndexOf("\"", startIndx);

                value = data.Substring(startIndx, endIndx - startIndx);
                //return value;
            }
            if (string.IsNullOrEmpty(value.Trim()))
            {
                data = data.Replace("&quot;", "\"");
                int startIndx = data.IndexOf("\"" + paramName + "\"") + ("\"" + paramName + "\"").Length + 1;
                int endIndx = data.IndexOf(",", startIndx);

                value = data.Substring(startIndx, endIndx - startIndx).Replace("\"", "");
                
            }
            return value;
            
        }

    }
}
