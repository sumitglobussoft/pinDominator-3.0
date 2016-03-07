using System;
using System.Net;
using System.IO;

namespace socialautobot
{
	public class GlobusHttpHelper
	{

		CookieCollection gCookies;
		public HttpWebRequest gRequest;
		public HttpWebResponse gResponse;

		public string UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0";   //"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3)";
		int Timeout = 90000;

		string proxyAddress = string.Empty;
		int port = 80;
		string proxyUsername = string.Empty;
		string proxyPassword = string.Empty;

		public GlobusHttpHelper ()
		{
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

		public void setExpect100Continue()
		{
			if (ServicePointManager.Expect100Continue == true)
			{
				ServicePointManager.Expect100Continue = false;
			}
		}


		public string getHtmlfromUrl(Uri url, string Referes, string Token)
		{
			try
			{

				ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
				gRequest = (HttpWebRequest)WebRequest.Create(url);
				// gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
				//"Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0"
				// gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0";
				gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:22.0) Gecko/20100101 Firefox/22.0";
				gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
				gRequest.Method = "GET";
				gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
				gRequest.KeepAlive = true;
				gRequest.ContentType = @"application/x-www-form-urlencoded";
				//gRequest.Timeout = 2 * 30000;
				// gRequest.Referer = "https://www.facebook.com/checkpoint/";

				ChangeProxy(proxyAddress, port, proxyUsername, proxyPassword);

				#region CookieManagement
				if (this.gCookies != null && this.gCookies.Count > 0)
				{
					setExpect100Continue();
					gRequest.CookieContainer.Add(gCookies);
				}
				//Get Response for this request url

				try
				{
				}
				catch{}
				setExpect100Continue();
				try
				{

					gResponse = (HttpWebResponse)gRequest.GetResponse();
				}
				catch(Exception ex)
				{
					//return ex.Message+"::::"+ex.StackTrace;
				}

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

					using (StreamReader reader = new StreamReader(gResponse.GetResponseStream()))
					{
						string responseString = reader.ReadToEnd();
						return responseString; 
					}
				}
				else
				{
					return "Error";
				}
			}
			catch (Exception ex)
			{
				//return null;
				if (ex.Message.Contains("The remote server returned an error: (429)") || ex.Message.Contains("Too Many Requests") || ex.Message.Contains("Client Error (429)"))
				{
					return "Too Many Requestes";
				}
				return null;
			}

		}


		public string postFormData(Uri formActionUrl, string postData)
		{
			// postData="charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=AVqEAf6F&locale=en_US&email=soni.sameer123%40rediffmail.com&pass=god@12345&persistent=1&default_persistent=1&charset_test=%E2%82%AC%2C%C2%B4%2C%E2%82%AC%2C%C2%B4%2C%E6%B0%B4%2C%D0%94%2C%D0%84&lsd=AVqEAf6F";

			gRequest = (HttpWebRequest)WebRequest.Create(formActionUrl);
			// gRequest.UserAgent = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.16) Gecko/20110319 Firefox/3.6.16";
			//"Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0"
			// gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0";
			gRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:22.0) Gecko/20100101 Firefox/22.0";
			gRequest.CookieContainer = new CookieContainer();// gCookiesContainer;
			gRequest.Method = "POST";
			gRequest.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";
			gRequest.KeepAlive = true;
			gRequest.ContentType = @"application/x-www-form-urlencoded";
			//gRequest.Timeout = 2 * 30000;
			// gRequest.Referer = "https://www.facebook.com/checkpoint/";

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


		public  string GetParamValue(string pgSrc, string paramName)
		{
			string valueparamName = string.Empty;
			try
			{
				if (pgSrc.Contains("name='" + paramName + "'"))
				{
					string param = "name='" + paramName + "'";
					int startparamName = pgSrc.IndexOf(param) + param.Length;
					startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
					int endparamName = pgSrc.IndexOf("'", startparamName);
					valueparamName = pgSrc.Substring(startparamName, endparamName - startparamName);
					return valueparamName;
				}
				else if (pgSrc.Contains("name=\"" + paramName + "\""))
				{
					string param = "name=\"" + paramName + "\"";
					int startparamName = pgSrc.IndexOf(param) + param.Length;
					startparamName = pgSrc.IndexOf("value=", startparamName) + "value=".Length + 1;
					int endcommentPostID = pgSrc.IndexOf("\"", startparamName);
					valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
					return valueparamName;
				}
				else if (pgSrc.Contains("name=\\\\\\\"" + paramName + "\\\\\\\""))
				{
					string param = "name=\\\\\\\"" + paramName + "\\\\\\\"";
					int startparamName = pgSrc.IndexOf(param) + param.Length;
					startparamName = pgSrc.IndexOf("value=\\\\\\\"", startparamName) + "value=\\\\\\".Length + 1;
					int endcommentPostID = pgSrc.IndexOf("\\\\\\\"", startparamName);
					valueparamName = pgSrc.Substring(startparamName, endcommentPostID - startparamName);
					return valueparamName;
				}
				else if (paramName.Contains("user"))
				{
					string value = string.Empty;
					//  value = getBetween(pgSrc, "USER_ID", "ACCOUNT_ID").Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":","");
					value = getBetween(pgSrc, "CurrentUserInitialData", "}").Replace("id", string.Empty).Replace("\"", string.Empty).Replace("[]", string.Empty).Replace(",", string.Empty).Replace("{", string.Empty).Replace(":", string.Empty).Replace("is_employeefalse", "").Replace("is_grayfalse", "");
					if (value.Contains("account"))
					{
						string [] arr=System.Text.RegularExpressions.Regex.Split(value,"account");
						value=arr[0];
					}
					if (value.Contains("USER_ID"))
					{
						value = getBetween(pgSrc, "USER_ID", "ACCOUNT_ID").Replace("\"", string.Empty).Replace(",", string.Empty).Replace(":", "");
					}
					return value;

				}
				return null;
			}
			catch (Exception ex)
			{
				// GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
			}
			return valueparamName;
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
	}
}

