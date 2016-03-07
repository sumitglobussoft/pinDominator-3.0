using System;

using System.Collections.Generic;
using System.Threading;
using PinDominator;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace PinDominator
{
	public class PageManager
	{

		public static int noOfPageToScrap = 0;
		public static bool IsPageScraperStarted = false;
		public static int noOfPicDownload =1;
		public PageManager ()
		{


		}

		//public static Event CampaignStopLogevents = new Event();

		public static void AddToLogger_PageManager(string log)
		{
			EventsArgs eArgs = new EventsArgs(log);
			CampaignStopLogevents.LogText(eArgs);
		}

		#region Global Variables For Fan Page Liker Activity

		readonly object lockrThreadControllerFanPageLiker = new object();
		static readonly object lockrqueFanPageURLsFanPageLiker = new object();
		static readonly object lockrqueFanPageMessagesFanPageLiker = new object();

		public static Queue<string> queFanPageURLsFanPageLiker = new Queue<string>();
		public static Queue<string> queFanPageMessagesFanPageLiker = new Queue<string>();

		public static string ScrapersExprotFilePath=string.Empty;

		public Queue<string> queFanPageMessagesFanPage = new Queue<string>();


		public bool isStopFanPageLiker = false;
		int countThreadControllerFanPageLiker = 0;
		public static int TotalFanPagelikeCounter = 0;
		public static bool LikeOthersPosts = false;
		public static bool DiscardPostsFilterCommentsFanPageLiker = true;
		public static List<int> lstLikeSelectedFanPageLiker = new List<int>();

		public static List<Thread> lstThreadsFanPageLiker = new List<Thread> ();

		public static List<string> lstFanPageUrlsFanPageLiker = new List<string>();
		public List<string> lstFanPageMessageFanPageLiker = new List<string>();
		public List<string> lstFanPageCommentsFanPageLiker = new List<string>();

		public List<string>listFanPageUrl=new List<string>();

		private static Dictionary<string, string> dictionarySuccessfulLikerAccounts = new Dictionary<string, string>();

		#endregion


		public static string FanPageProcessUsing
		{
			get;
			set;
		}


		public void StartLikePage()
		{  
			countThreadControllerFanPageLiker = 0;
			try
			{
				AddToLogger_PageManager("Please Wait Process Started...");

				List<List<string>> list_listAccounts = new List<List<string>>();
				PageManager.IsPageScraperStarted = false;

				if (FBGlobals.listAccounts.Count >= 1)
				{                  

					foreach (string account in FBGlobals.listAccounts)
						{
							try
							{
								lock (lockrThreadControllerFanPageLiker)
								{
									try
									{
									if (countThreadControllerFanPageLiker >= FBGlobals.listAccounts.Count)
										{
											Monitor.Wait(lockrThreadControllerFanPageLiker);
										}

										string acc = account.Remove(account.IndexOf(':'));

										//Run a separate thread for each account
										FacebookUser item = null;
										FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

										if (item != null)
										{
											try
											{
												Thread profilerThread = new Thread(StartLikePageMultiThreads);												

												profilerThread.Start(new object[] { item });
												countThreadControllerFanPageLiker++;
											}
											catch (Exception ex)
											{
											Console.WriteLine(ex.StackTrace);
										}
									}
									}
								catch (Exception ex)
								{
									Console.WriteLine(ex.StackTrace);
								}
							
							}
							}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
						}

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}


		public void StartLikePageMultiThreadsOld(object parameters)
		{
			try
			{
				if (!isStopFanPageLiker)
				{
					try
					{

						try
						{
							lstThreadsFanPagePoster.Add(Thread.CurrentThread);
							lstThreadsFanPagePoster.Distinct();
							Thread.CurrentThread.IsBackground = true;
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						Array paramsArray = new object[1];
						paramsArray = (Array)parameters;

						FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

						if (!objFacebookUser.isloggedin)
						{
							GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

							objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


							//Login Process

						   AccountManager objAccountManager = new AccountManager();

							AddToLogger_PageManager("Logging in with " + objFacebookUser.username);;
							objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
						}

						if (objFacebookUser.isloggedin)
						{
							AddToLogger_PageManager("Successful login With : "+objFacebookUser.username);
						


							if (FanPageProcessUsing=="Fan Page Liker")
							{
								AddToLogger_PageManager("Please Wait Fan Page Liker Process Started ...");
								StartFanPageLikerProcess(ref objFacebookUser);

							}
							else if (FanPageProcessUsing=="Fan Page Scraper By Keyword")
							{
								StartScrapeFanPageUrlByKeyWords(ref objFacebookUser);

							}
							else if (FanPageProcessUsing=="Fan Page Post Simple Message")
							{
								StartSendFanPagePostSimpleMessage(ref objFacebookUser);
							}
							else if (FanPageProcessUsing=="Fan Page Post Pictures")
							{
								StartActionForFanpagePostPic(ref objFacebookUser);
							}
							else if (FanPageProcessUsing=="Fan Page Post Video")
							{
								StartActionForFanpagePostVideo(ref objFacebookUser);

							}
							else if(FanPageProcessUsing=="Download FanPage Pictures")
							{

								StartActionDownloadPhoto(ref objFacebookUser);
							}
						}
						else
						{
							AddToLogger_PageManager("Login Failed with " + objFacebookUser.username);;
						}


					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

			finally
			{
				try
				{
					  if (!isStopFanPageLiker)
					{
						lock (lockrThreadControllerFanPageLiker)
						{
							countThreadControllerFanPageLiker--;
							Monitor.Pulse(lockrThreadControllerFanPageLiker);

						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
			}
		}








		public void StartLikePageMultiThreads(object parameters)
		{
			try
			{
				if (!isStopFanPageLiker)
				{
					try
					{

						try
						{
							lstThreadsFanPagePoster.Add(Thread.CurrentThread);
							lstThreadsFanPagePoster.Distinct();
							Thread.CurrentThread.IsBackground = true;
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							//GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartLikePageMultiThreads  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}

						Array paramsArray = new object[1];
						paramsArray = (Array)parameters;

						FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);


						while(true)
						{

							if (!objFacebookUser.isloggedin)
							{





								GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

								objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


								//Login Process

								AccountManager objAccountManager = new AccountManager();

								AddToLogger_PageManager("Logging in with " + objFacebookUser.username);;
								objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
							}

							if (objFacebookUser.isloggedin)
							{
								AddToLogger_PageManager("Successful login With : "+objFacebookUser.username);



								if (FanPageProcessUsing=="Fan Page Liker")
								{
									AddToLogger_PageManager("Please Wait Fan Page Liker Process Started ...");
									StartFanPageLikerProcess(ref objFacebookUser);

								}
								else if (FanPageProcessUsing=="Fan Page Scraper By Keyword")
								{
									
									if(!PageManager.IsPageScraperStarted)
									{
										PageManager.IsPageScraperStarted = true;
									    StartScrapeFanPageUrlByKeyWords(ref objFacebookUser);
									}

								}
								else if (FanPageProcessUsing=="Fan Page Post Simple Message")
								{
									StartSendFanPagePostSimpleMessage(ref objFacebookUser);
								}
								else if (FanPageProcessUsing=="Fan Page Post Pictures")
								{
									StartActionForFanpagePostPic(ref objFacebookUser);
								}
								else if (FanPageProcessUsing=="Fan Page Post Video")
								{
									StartActionForFanpagePostVideo(ref objFacebookUser);

								}
								else if(FanPageProcessUsing=="Download FanPage Pictures")
								{

									StartActionDownloadPhoto(ref objFacebookUser);
								}
								break;
							}
							else
							{
								AddToLogger_PageManager("Login Failed with , Please wait We will try again after 40 seconds" + objFacebookUser.username);
								AddToLogger_PageManager("Delay For 40 seconds");
								Thread.Sleep(40*1000);

							}
						}


					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartLikePageMultiThreads  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartLikePageMultiThreads  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}

			finally
			{
				try
				{
					if (!isStopFanPageLiker)
					{
						lock (lockrThreadControllerFanPageLiker)
						{
							countThreadControllerFanPageLiker--;
							Monitor.Pulse(lockrThreadControllerFanPageLiker);

						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
					GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartLikePageMultiThreads  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
				}
			}
		}












		private void StartActionDownloadPhoto(ref FacebookUser fbUser)
		{
			try
			{
				StartDownlodingPhotoFanPage(ref fbUser);

				AddToLogger_PageManager("Process Completed Of Photo Downloding  With Username >>> " + fbUser.username);


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
		}
		public List<string> LstDownloadPhotoURLsDownloadPhoto = new List<string> ();
		public string ExportPhotosPath=string.Empty;

		public void StartDownlodingPhotoFanPageOld(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string ProFilePost = string.Empty;
				string __user = string.Empty;
				string ExportPath = ExportPhotosPath;
				if (string.IsNullOrEmpty(ExportPath))
				{
					AddToLogger_PageManager("Please Select Photo Export Folder Path !! ");

					return;
				}
				string userId = "";
				List<string> lstphotourl = new List<string>();
				try
				{
					string UserId = string.Empty;

					string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

					UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
					if (string.IsNullOrEmpty(UserId))
					{
						UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
					}

					if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
					{
						AddToLogger_PageManager("Please Check The Account : " + fbUser.username);


						return;
					}


					foreach (var LstDownloadPhotoURLsDownloadPhoto_item in LstDownloadPhotoURLsDownloadPhoto)
					{
						try
						{
							string PageSourceOfTargetedUrl = string.Empty;
							string URL = string.Empty;
							string PageTitle = string.Empty;
							string PageID = string.Empty;                            
							if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos_stream"))
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item.Replace("?ref=br_rs", "/photos_stream");
							}
							else
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item;
							}
							List<string> PhotoURL = new List<string>();
							try
							{
								if (URL.Contains("?ref=br_rs"))
								{
									URL = URL.Replace("?ref=br_rs","/photos_stream");
								}
								PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL),"","");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							string AjaxPipeToken=GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"ajaxpipe_token\":\"","\"");
							string scrollLoad = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "scroll_load\\\":",",");
							string LastFBID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "last_fbid\\\":", ",");
							string FetchSize = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "fetch_size\\\":", ",");
							PageTitle = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "pageTitle\">", "</title>");
							PageID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"pageID\":",",");
							//string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "data-non-starred-src");
							string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url");

							PhotoId = PhotoId.Skip(1).ToArray();
							int RandomNumber = 0;
							foreach (string Photoid in PhotoId)
							{
								try
								{
									RandomNumber++;
									string temp = GlobusHttpHelper.getBetween(Photoid, "=\"", "\"");
									string CompletePicUrl = string.Empty;
									if (temp.Contains("/p") || temp.Contains("/v/"))
									{
										try
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.StackTrace);
										}
										CompletePicUrl = CompletePicUrl.Replace("/v", "");
										if (string.IsNullOrEmpty(CompletePicUrl))
										{
											CompletePicUrl = temp.Replace("/v", "");
										}
									}
									else
									{
										string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;
										CompletePicUrl = CompletePicUrl1.Replace("/v", "");									

									}

									string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
									try
									{
										//CompletePicUrl="https://fbcdn-sphotos-c-a.akamaihd.net/hphotos-ak-xfa1/v/t1.0-9/p180x540/10857801_897602353597298_2585580003466309245_n.jpg?oh=8949fdb7f4f430b047b060fb2407a153&oe=54FBF422&__gda__=1429289456_f73a2dd86f9858d184ff617000344b75";
										GlobusHttpHelper.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
										//GlobusHttpHelper.SaveImageWithUrl(CompletePicUrl,ExportPhotosPath,PicName);
										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							int Count = 0;
							while (true)
							{
								Count = Count + 1;
								if (Count >= 20)
								{
									break;
								}

						    	try
								{
								 //  string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
									//PhotoId = Regex.Split(AjaxPageSource1, "data-non-starred-src");
								  // PhotoId = PhotoId.Skip(1).ToArray();
								}
								catch{};


							    string AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
								//string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token="+ AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22ref%22%3A%22page_internal%22%2C%22ajaxpipe%22%3A%221%22%2C%22ajaxpipe_token%22%3A%"+ PageID + "%22%2C%22quickling%22%3A%7B%22version%22%3A%221549264%3B0%3B%22%7D%2C%22__user%22%3A%"+userId+"%22%2C%22__a%22%3A%221%22%2C%22__dyn%22%3A%22aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE%22%2C%22__req%22%3A%22jsonp_11%22%2C%22__rev%22%3A%221549264%22%2C%22__adt%22%3A%2211%22%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A344128252278047%2C%22is_medley_view%22%3Atrue%7D&__user="+UserId+"&__a=1&__dyn=aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE&__req=jsonp_12&__rev=1549264&__adt="+Count),"","");
								//PhotoId = Regex.Split(AjaxPageSource, "data-non-starred-src");
								 PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url"); 
								PhotoId = PhotoId.Skip(1).ToArray();
								foreach (string Photoid in PhotoId)
								{
									try
									{
										RandomNumber++;
										string temp = GlobusHttpHelper.getBetween(Photoid, "=\\\"", "\\\"").Replace("\\", "");
										string CompletePicUrl = string.Empty;
										if (temp.Contains("/p") || temp.Contains("/v/"))
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
											CompletePicUrl = temp.Replace("/v", "");
											if (string.IsNullOrEmpty(CompletePicUrl))
											{
												CompletePicUrl = temp.Replace("/v", "");
											}
										}
										else
										{

											string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;

											CompletePicUrl = CompletePicUrl1.Replace("/v", "");


										}
										string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
										PicName = PicName.Replace(" ","-"); ;
										GlobusHttpHelper.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath.Replace("\\\\FanPageScrapedData.csv",""));
									

										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								LastFBID = GlobusHttpHelper.getBetween(AjaxPageSource, "last_fbid\\\":", ","); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}
			

		public void StartDownlodingPhotoFanPage_Old_26_oct(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string ProFilePost = string.Empty;
				string __user = string.Empty;
				string ExportPath = ExportPhotosPath;
				if (string.IsNullOrEmpty(ExportPath))
				{
					AddToLogger_PageManager("Please Select Photo Export Folder Path !! ");

					return;
				}
				string userId = "";
				List<string> lstphotourl = new List<string>();
				try
				{
					string UserId = string.Empty;

					string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

					UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
					if (string.IsNullOrEmpty(UserId))
					{
						UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
					}

					if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
					{
						AddToLogger_PageManager("Please Check The Account : " + fbUser.username);


						return;
					}


					foreach (var LstDownloadPhotoURLsDownloadPhoto_item in LstDownloadPhotoURLsDownloadPhoto)
					{
						try
						{
							string PageSourceOfTargetedUrl = string.Empty;
							string URL = string.Empty;
							string PageTitle = string.Empty;
							string PageID = string.Empty;                            
							if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos_stream"))
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item.Replace("?ref=br_rs", "/photos_stream");
							}
							else
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item;
							}
							List<string> PhotoURL = new List<string>();
							try
							{
								if (URL.Contains("?ref=br_rs"))
								{
									URL = URL.Replace("?ref=br_rs","/photos_stream");
								}
								PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL),"","");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							string AjaxPipeToken=GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"ajaxpipe_token\":\"","\"");
							string scrollLoad = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "scroll_load\\\":",",");
							string LastFBID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "last_fbid\\\":", ",");
							string FetchSize = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "fetch_size\\\":", ",");
							PageTitle = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "pageTitle\">", "</title>");
							PageID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"pageID\":",",").Replace("}","").Replace("]","").Replace("\"","");
							//string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "data-non-starred-src");
							string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url");

							PhotoId = PhotoId.Skip(1).ToArray();
							int RandomNumber = 0;
							foreach (string Photoid in PhotoId)
							{


								if (RandomNumber >= PageManager.noOfPicDownload)
								{
									AddToLogger_PageManager(RandomNumber + "No of Pic " + PageManager.noOfPicDownload + " Downloaded ");
									return;

								}



								try
								{
									RandomNumber++;
									string temp = GlobusHttpHelper.getBetween(Photoid, "=\"", "\"");
									string CompletePicUrl = string.Empty;
									if (temp.Contains("/p") || temp.Contains("/v/"))
									{
										try
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.StackTrace);
										}
										CompletePicUrl = CompletePicUrl.Replace("/v", "");
										if (string.IsNullOrEmpty(CompletePicUrl))
										{
											CompletePicUrl = temp.Replace("/v", "");
										}
									}
									else
									{
										string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;
										//CompletePicUrl = CompletePicUrl1.Replace("/v", "");
										CompletePicUrl = CompletePicUrl1.Replace("amp;", "");


									}

									string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
									try
									{
										//CompletePicUrl="https://fbcdn-sphotos-c-a.akamaihd.net/hphotos-ak-xfa1/v/t1.0-9/p180x540/10857801_897602353597298_2585580003466309245_n.jpg?oh=8949fdb7f4f430b047b060fb2407a153&oe=54FBF422&__gda__=1429289456_f73a2dd86f9858d184ff617000344b75";
										GlobusHttpHelper.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath);
										//GlobusHttpHelper.SaveImageWithUrl(CompletePicUrl,ExportPhotosPath,PicName);
										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							int Count = 0;
							while (true)
							{
								//break;
								Count = Count + 1;
								if (Count >= 20)
								{
									break;
								}

								try
								{
									//  string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
									//PhotoId = Regex.Split(AjaxPageSource1, "data-non-starred-src");
									// PhotoId = PhotoId.Skip(1).ToArray();
								}
								catch(Exception ex){
									//	GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartDownlodingPhotoFanPage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								};


								string AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
								//string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token="+ AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22ref%22%3A%22page_internal%22%2C%22ajaxpipe%22%3A%221%22%2C%22ajaxpipe_token%22%3A%"+ PageID + "%22%2C%22quickling%22%3A%7B%22version%22%3A%221549264%3B0%3B%22%7D%2C%22__user%22%3A%"+userId+"%22%2C%22__a%22%3A%221%22%2C%22__dyn%22%3A%22aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE%22%2C%22__req%22%3A%22jsonp_11%22%2C%22__rev%22%3A%221549264%22%2C%22__adt%22%3A%2211%22%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A344128252278047%2C%22is_medley_view%22%3Atrue%7D&__user="+UserId+"&__a=1&__dyn=aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE&__req=jsonp_12&__rev=1549264&__adt="+Count),"","");
								//PhotoId = Regex.Split(AjaxPageSource, "data-non-starred-src");
								// PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url");
								PhotoId = Regex.Split(AjaxPageSource, "background-image: url"); 
								PhotoId = PhotoId.Skip(1).ToArray();
								foreach (string Photoid in PhotoId)
								{
									try
									{

										if (RandomNumber >= PageManager.noOfPicDownload)
										{
											AddToLogger_PageManager(RandomNumber + "No of Pic " + PageManager.noOfPicDownload + " Downloaded ");

											return;
										}

										RandomNumber++;
										string temp = GlobusHttpHelper.getBetween(Photoid, "=\\\"", "\\\"").Replace("\\", "");
										string CompletePicUrl = string.Empty;
										if (temp.Contains("/p") || temp.Contains("/v/"))
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
											CompletePicUrl = temp.Replace("/v", "");
											if (string.IsNullOrEmpty(CompletePicUrl))
											{
												CompletePicUrl = temp.Replace("/v", "");
											}
										}
										else
										{

											string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;

											//CompletePicUrl = CompletePicUrl1.Replace("/v", "");

											CompletePicUrl = CompletePicUrl1.Replace("amp;", "");


										}
										string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
										PicName = PicName.Replace(" ","-"); ;
										GlobusHttpHelper.GetImageFromUrl(CompletePicUrl, PicName, ExportPhotosPath.Replace("\\\\FanPageScrapedData.csv",""));


										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								LastFBID = GlobusHttpHelper.getBetween(AjaxPageSource, "last_fbid\\\":", ","); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
					GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}







		public void StartDownlodingPhotoFanPage(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string ProFilePost = string.Empty;
				string __user = string.Empty;
				string ExportPath = ExportPhotosPath;
				if (string.IsNullOrEmpty(ExportPath))
				{
					AddToLogger_PageManager("Please Select Photo Export Folder Path !! ");

					return;
				}
				string userId = "";
				List<string> lstphotourl = new List<string>();
				try
				{
					string UserId = string.Empty;

					string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

					UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
					if (string.IsNullOrEmpty(UserId))
					{
						UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
					}

					if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
					{
						AddToLogger_PageManager("Please Check The Account : " + fbUser.username);


						return;
					}


					foreach (var LstDownloadPhotoURLsDownloadPhoto_item in LstDownloadPhotoURLsDownloadPhoto)
					{
						try
						{
							string PageSourceOfTargetedUrl = string.Empty;
							string URL = string.Empty;
							string PageTitle = string.Empty;
							string PageID = string.Empty;                            
							if (!LstDownloadPhotoURLsDownloadPhoto_item.Contains("/photos_stream"))
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item.Replace("?ref=br_rs", "/photos_stream");
							}
							else
							{
								URL = LstDownloadPhotoURLsDownloadPhoto_item;
							}
							List<string> PhotoURL = new List<string>();
							try
							{
								if (URL.Contains("?ref=br_rs"))
								{
									URL = URL.Replace("?ref=br_rs","/photos_stream");
								}
								PageSourceOfTargetedUrl = HttpHelper.getHtmlfromUrl(new Uri(URL),"","");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							string AjaxPipeToken=GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"ajaxpipe_token\":\"","\"");
							string scrollLoad = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "scroll_load\\\":",",");
							string LastFBID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "last_fbid\\\":", ",");
							string FetchSize = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "fetch_size\\\":", ",");
							PageTitle = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl, "pageTitle\">", "</title>");
							PageID = GlobusHttpHelper.getBetween(PageSourceOfTargetedUrl,"pageID\":",",").Replace("}","").Replace("]","").Replace("\"","");
							//string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "data-non-starred-src");
							string[] PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url");

							PhotoId = PhotoId.Skip(1).ToArray();
							int RandomNumber = 0;
							foreach (string Photoid in PhotoId)
							{


								if (RandomNumber >= PageManager.noOfPicDownload)
								{
									AddToLogger_PageManager(RandomNumber + "No of Pic " + PageManager.noOfPicDownload + " Downloaded ");
									return;

								}

								string urlPhotoid = "";
								string PhotoRealUrl = "";

								if (Photoid.Contains("ajaxify="))
								{
									urlPhotoid = FBUtils.getBetween(Photoid, "ajaxify=\"", "\"");

									urlPhotoid = HttpUtility.UrlDecode(urlPhotoid);

									if(urlPhotoid.Contains("src="))
									{
										PhotoRealUrl = FBUtils.getBetween(urlPhotoid, "src=", "&amp");

									}

								}

								try
								{
									RandomNumber++;
									string temp = GlobusHttpHelper.getBetween(Photoid, "=\"", "\"");
									string CompletePicUrl = string.Empty;
									if (temp.Contains("/p") || temp.Contains("/v/"))
									{
										try
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.StackTrace);
										}
										CompletePicUrl = CompletePicUrl.Replace("/v", "");
										if (string.IsNullOrEmpty(CompletePicUrl))
										{
											CompletePicUrl = temp.Replace("/v", "");
										}
									}
									else
									{
										string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;
										//CompletePicUrl = CompletePicUrl1.Replace("/v", "");
										CompletePicUrl = CompletePicUrl1.Replace("amp;", "");


									}

									string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
									try
									{
										//CompletePicUrl="https://fbcdn-sphotos-c-a.akamaihd.net/hphotos-ak-xfa1/v/t1.0-9/p180x540/10857801_897602353597298_2585580003466309245_n.jpg?oh=8949fdb7f4f430b047b060fb2407a153&oe=54FBF422&__gda__=1429289456_f73a2dd86f9858d184ff617000344b75";
										GlobusHttpHelper.GetImageFromUrl(PhotoRealUrl, PicName, ExportPhotosPath);
										//GlobusHttpHelper.SaveImageWithUrl(CompletePicUrl,ExportPhotosPath,PicName);
										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							int Count = 0;
							while (true)
							{
								//break;

								if (Count >= PageManager.noOfPicDownload)
								{
									break;
								}

								Count = Count + 1;


								try
								{
									//  string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
									//PhotoId = Regex.Split(AjaxPageSource1, "data-non-starred-src");
									// PhotoId = PhotoId.Skip(1).ToArray();
								}
								catch(Exception ex){
									//	GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  StartDownlodingPhotoFanPage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								};


								string AjaxPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token=" + AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + PageID + "%2C%22is_medley_view%22%3Atrue%2C%22pager_fired_on_init%22%3Atrue%7D&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35zoSt2u6aWizGomyp9Esx6bF3pqzCC-C26m6oKezpUgxd6K4bBw&__req=jsonp_2&__rev=1392897&__adt="+Count),"","");
								//string AjaxPageSource1 = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?ajaxpipe=1&ajaxpipe_token="+ AjaxPipeToken + "&no_script_path=1&data=%7B%22scroll_load%22%3A" + scrollLoad + "%2C%22last_fbid%22%3A" + LastFBID.Replace("\\\"","") + "%2C%22fetch_size%22%3A" + FetchSize + "%2C%22profile_id%22%3A" + PageID + "%2C%22ref%22%3A%22page_internal%22%2C%22ajaxpipe%22%3A%221%22%2C%22ajaxpipe_token%22%3A%"+ PageID + "%22%2C%22quickling%22%3A%7B%22version%22%3A%221549264%3B0%3B%22%7D%2C%22__user%22%3A%"+userId+"%22%2C%22__a%22%3A%221%22%2C%22__dyn%22%3A%22aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE%22%2C%22__req%22%3A%22jsonp_11%22%2C%22__rev%22%3A%221549264%22%2C%22__adt%22%3A%2211%22%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A344128252278047%2C%22is_medley_view%22%3Atrue%7D&__user="+UserId+"&__a=1&__dyn=aJioznEyl2qm9adDgDDx2IGAy4DBzECQqbx2mbAJliGtbHz6C_8Ey5poji-FeiWG8ADyFrF6Apvy6QSiZ7BBGqEgVKi8zVE&__req=jsonp_12&__rev=1549264&__adt="+Count),"","");
								//PhotoId = Regex.Split(AjaxPageSource, "data-non-starred-src");
								// PhotoId = Regex.Split(PageSourceOfTargetedUrl, "background-image: url");
								PhotoId = Regex.Split(AjaxPageSource, "background-image: url"); 
								PhotoId = PhotoId.Skip(1).ToArray();
								foreach (string Photoid in PhotoId)
								{
									try
									{

										if (RandomNumber >= PageManager.noOfPicDownload)
										{
											AddToLogger_PageManager(RandomNumber + "No of Pic " + PageManager.noOfPicDownload + " Downloaded ");

											return;
										}

										RandomNumber++;
										string temp = GlobusHttpHelper.getBetween(Photoid, "=\\\"", "\\\"").Replace("\\", "");
										string CompletePicUrl = string.Empty;
										if (temp.Contains("/p") || temp.Contains("/v/"))
										{
											string[] arr = Regex.Split(temp, "/p");
											string Locatstr = arr[1] + "###";
											CompletePicUrl = arr[0] + "/" + GlobusHttpHelper.getBetween(Locatstr, "/", "###");
											CompletePicUrl = temp.Replace("/v", "");
											if (string.IsNullOrEmpty(CompletePicUrl))
											{
												CompletePicUrl = temp.Replace("/v", "");
											}
										}
										else
										{

											string	CompletePicUrl1 =GlobusHttpHelper.getBetween(Photoid,"(",")") ;

											//CompletePicUrl = CompletePicUrl1.Replace("/v", "");

											CompletePicUrl = CompletePicUrl1.Replace("amp;", "");


										}

										string urlPhotoid = "";
										string PhotoRealUrl = "";
										if (Photoid.Contains("ajaxify="))
										{
											urlPhotoid = FBUtils.getBetween(Photoid, "ajaxify=\\\"", "\\\"");

											urlPhotoid = HttpUtility.UrlDecode(urlPhotoid);

											if (urlPhotoid.Contains("src="))
											{
												PhotoRealUrl = FBUtils.getBetween(urlPhotoid, "src=", "&amp");
												PhotoRealUrl = PhotoRealUrl.Replace("\\", "");
												PhotoRealUrl = PhotoRealUrl.Replace("u0025", "%");
												PhotoRealUrl = HttpUtility.UrlDecode(PhotoRealUrl);

											}

										}


										string PicName = PageTitle + "-" + PageID + "-" + RandomNumber.ToString();
										PicName = PicName.Replace(" ","-");
										GlobusHttpHelper.GetImageFromUrl(PhotoRealUrl, PicName, ExportPhotosPath.Replace("\\\\FanPageScrapedData.csv", ""));


										AddToLogger_PageManager(PicName + " downloaded to " + ExportPhotosPath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								LastFBID = GlobusHttpHelper.getBetween(AjaxPageSource, "last_fbid\\\":", ","); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
					GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  LstDownloadPhotoURLsDownloadPhoto_item  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}














		public void StartSendFanPagePostSimpleMessage(ref FacebookUser fbUser)
		{
			try
			{
				PostFanPageMessageUsingUrls(ref fbUser);
				AddToLogger_PageManager("Process completed With : " + fbUser.username);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		public void StartActionForFanpagePostPic (ref FacebookUser fbUser)
		{
			try
			{
				AddToLogger_PageManager("Process Start With : " + fbUser.username);
				UploadImage(ref fbUser);

				AddToLogger_PageManager("Process completed With : " + fbUser.username);

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		public void StartActionForFanpagePostVideo(ref FacebookUser fbUser)
		{
			try
			{
				AddToLogger_PageManager("Process Start With : " + fbUser.username);
				PostOnFanPageWallWithURLAndItsImage(ref fbUser);
				AddToLogger_PageManager("Process completed With : " + fbUser.username);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		public static List<string> lstFanPageKeyWords = new List<string> ();

		public void StartScrapeFanPageUrlByKeyWords(ref FacebookUser fbUser)
		{
			try
			{
				foreach (var item in lstFanPageKeyWords)
				{

					string KeyWord=item;
					AddToLogger_PageManager("Started Fanpage Scraper with : " +KeyWord);

					lstFanPageUrlsFanPageLiker=CrawlFanpage(ref fbUser, lstFanPageUrlsFanPageLiker,KeyWord);

					try
					{
						int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
						AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
					
				AddToLogger_PageManager("Scraping Process Completed .");

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		public void StartFanPageLikerProcess(ref FacebookUser fbUser)
		{
			try
			{
			
			    LikePage(ref fbUser, lstFanPageUrlsFanPageLiker);				


			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}


		//SCrape FanPagesViaKeyWord


		public List<string> CrawlFanpageOld(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add(lstUrlitem);
				}
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{


					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}

					string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   

					try
					{
						if (strcategoryurl.Contains("category\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("category\":")).Replace("category\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
								Counter=Counter+1;
								try
								{
									CheckDupliCats.Add(Fanpurl, Fanpurl);
								    Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath);
									AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return FindFanpageList;
		}


		public List<string>  FindTheFanPagesByKeywordOld(string Keyword, ref FacebookUser fbUser)
		{
			List<string> FanPageUrls = new List<string>();

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			try
			{
				#region Post variable

				string fbpage_id = string.Empty;            
				string __user = string.Empty;
				//Keyword="sachin";

				#endregion

				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				#region MyRegion
				//Array paramArray = new object[8];
				//paramArray = (Array)postParams;

				//string Username = (string)paramArray.GetValue(0);
				//string Password = (string)paramArray.GetValue(1);
				//string proxyAddress = (string)paramArray.GetValue(2);
				//string proxyPort = (string)paramArray.GetValue(3);
				//string proxyUserName = (string)paramArray.GetValue(4);
				//string proxyPassword = (string)paramArray.GetValue(5);
				//string Keyword = (string)paramArray.GetValue(6);
				//string Message = (string)paramArray.GetValue(7);

				// string  campaignName = (string)paramArray.GetValue(8);





				//if (fbLoginChecker.CheckLogin(ResponseLogin, Username, Password)) 
				#endregion
				//
				AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword);
				//AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword);
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/search.php?q=" + Keyword + "&type=pages"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
				}

				string AjaxRequest = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "href=\"#\" ajaxify=", "type=pages");
				AjaxRequest = "https://www.facebook.com"+AjaxRequest.Replace("\"", string.Empty).Replace("amp;", string.Empty) + "type=pages";

				List<string> pagesList = GetPages_FBSearch(pgSrc_FanPageSearch);

				List<string> distinctPages = pagesList.Distinct().ToList();

				///More Pages
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = "https://www.facebook.com/" + ajaxRequestURL + "&__a=1&__user=" + __user + "";

				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				#region for find friend Reqest Link

				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();

				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");

				foreach (string itemurl in Linklist)
				{
					if (!itemurl.Contains("<!DOCTYPE html"))
					{
						if (itemurl.Contains(@"https:\/\/www.facebook.com"))
						{
							lstLinkData.Add(itemurl);
							string strLink = itemurl.Substring(0, 200);
							if (strLink.Contains(@"https:\/\/www.facebook.com") && (strLink.Contains("onclick") || strLink.Contains("data-gt=\\")))

							{
								try
								{
									string[] tempArr = strLink.Split('"');
									string temp = tempArr[1];
									temp = temp.Replace("\\", "");
									list.Add(temp);
									AddToLogger_PageManager("KeyWord : "+Keyword +"Fan page Url : "+temp);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}
						}
					}
				}
				#endregion
				list = list.Distinct().ToList();
				distinctPages.AddRange(list);
				FanPageUrls = distinctPages;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);               
			}
			return FanPageUrls;
		}








		public List<string>  FindTheFanPagesByKeyword_Old_Old_Old(string Keyword, ref FacebookUser fbUser)
		{
			List<string> FanPageUrls = new List<string>();

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			try
			{
				#region Post variable

				string fbpage_id = string.Empty;            
				string __user = string.Empty;
				//Keyword="sachin";

				#endregion

				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				#region MyRegion
				//Array paramArray = new object[8];
				//paramArray = (Array)postParams;

				//string Username = (string)paramArray.GetValue(0);
				//string Password = (string)paramArray.GetValue(1);
				//string proxyAddress = (string)paramArray.GetValue(2);
				//string proxyPort = (string)paramArray.GetValue(3);
				//string proxyUserName = (string)paramArray.GetValue(4);
				//string proxyPassword = (string)paramArray.GetValue(5);
				//string Keyword = (string)paramArray.GetValue(6);
				//string Message = (string)paramArray.GetValue(7);

				// string  campaignName = (string)paramArray.GetValue(8);





				//if (fbLoginChecker.CheckLogin(ResponseLogin, Username, Password)) 
				#endregion
				//
				AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword);
				//AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword);
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/search.php?q=" + Keyword + "&type=pages"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
				}

				string AjaxRequest = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "href=\"#\" ajaxify=", "type=pages");
				AjaxRequest = "https://www.facebook.com"+AjaxRequest.Replace("\"", string.Empty).Replace("amp;", string.Empty) + "type=pages";

				List<string> pagesList = GetPages_FBSearch(pgSrc_FanPageSearch);

				List<string> distinctPages = pagesList.Distinct().ToList();

				///More Pages
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = "https://www.facebook.com/" + ajaxRequestURL + "&__a=1&__user=" + __user + "";

				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				if(string.IsNullOrEmpty(res_ajaxRequest))
				{
					try
					{
						try
						{
							string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
							string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
							string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
							ajaxRequestURL = splitAjax[0] + "pagesize=100&" + splitAjax1[1];
						}
						catch{};
						res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

						if(string.IsNullOrEmpty(res_ajaxRequest))
						{
							try
							{
								string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
								string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
								string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
								ajaxRequestURL = splitAjax[0] + "pagesize=50&" + splitAjax1[1];
								res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

							}
							catch{}
						}

						if(string.IsNullOrEmpty(res_ajaxRequest))
						{
							try
							{
								string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
								string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
								string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
								ajaxRequestURL = splitAjax[0] + "pagesize=20&" + splitAjax1[1];
								res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

							}
							catch{}
						}








					}
					catch{}
				}


				#region for find friend Reqest Link

				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();

				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");

				foreach (string itemurl in Linklist)
				{
					if (!itemurl.Contains("<!DOCTYPE html"))
					{
						if (itemurl.Contains(@"https:\/\/www.facebook.com"))   //(itemurl.Contains(@"https:\/\/www.facebook.com"))
						{
							lstLinkData.Add(itemurl);
							string strLink = itemurl.Substring(0, 70);
							if (strLink.Contains(@"https:\/\/www.facebook.com") && (strLink.Contains("onclick") || strLink.Contains("data-gt=\\")))   //("https:\\/\\/www.facebook.com") && (strLink.Contains("onclick") || strLink.Contains("data-gt=\\")))
							{
								try
								{
									string[] tempArr = strLink.Split('"');
									string temp = tempArr[1];
									temp = temp.Replace("\\", "");
									list.Add(temp);
									AddToLogger_PageManager("KeyWord : "+Keyword +"Fan page Url : "+temp);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}
						}
					}
				}
				#endregion
				list = list.Distinct().ToList();
				distinctPages.AddRange(list);
				FanPageUrls = distinctPages;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);               
			}
			return FanPageUrls;
		}











		public List<string>  FindTheFanPagesByKeyword(string Keyword, ref FacebookUser fbUser)
		{
			List<string> FanPageUrls = new List<string>();

			List<string>  list = new List<string>();

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			try
			{
				#region Post variable

				string fbpage_id = string.Empty;            
				string __user = string.Empty;
				//Keyword="sachin";

				#endregion

				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				#region MyRegion
				//Array paramArray = new object[8];
				//paramArray = (Array)postParams;

				//string Username = (string)paramArray.GetValue(0);
				//string Password = (string)paramArray.GetValue(1);
				//string proxyAddress = (string)paramArray.GetValue(2);
				//string proxyPort = (string)paramArray.GetValue(3);
				//string proxyUserName = (string)paramArray.GetValue(4);
				//string proxyPassword = (string)paramArray.GetValue(5);
				//string Keyword = (string)paramArray.GetValue(6);
				//string Message = (string)paramArray.GetValue(7);

				// string  campaignName = (string)paramArray.GetValue(8);





				//if (fbLoginChecker.CheckLogin(ResponseLogin, Username, Password)) 
				#endregion
				AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword); //https://www.facebook.com/search/results/?q=sachin&type=pages
				//AddToLogger_PageManager("Searching Fan Page Using keyword: " + Keyword);
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/search/results/?q=" + Keyword + "&type=pages"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
				}
				string AjaxRequest = "";

				try
				{
					AjaxRequest = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "href=\"#\" ajaxify=", "type=pages");
					AjaxRequest = "https://www.facebook.com"+AjaxRequest.Replace("\"", string.Empty).Replace("amp;", string.Empty) + "type=pages";
				}
				catch{};

				//List<string> pagesList = GetPages_FBSearch(pgSrc_FanPageSearch);

				//List<string> distinctPages = pagesList.Distinct().ToList();

				///More Pages
				/// 
				/// 

				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = "https://www.facebook.com/" + ajaxRequestURL + "&__a=1&__user=" + __user + "";

				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				if(string.IsNullOrEmpty(res_ajaxRequest))
				{



					/*
					try
					{
						try
						{
							string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
						string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
						string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
						ajaxRequestURL = splitAjax[0] + "pagesize=100&" + splitAjax1[1];
						}
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							
						};
					    res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

						if(string.IsNullOrEmpty(res_ajaxRequest))
						{
							try
							{
								string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
								string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
								string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
								ajaxRequestURL = splitAjax[0] + "pagesize=50&" + splitAjax1[1];
								res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

							}
							catch(Exception ex)
							{
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}

						if(string.IsNullOrEmpty(res_ajaxRequest))
						{
							try
							{
								string[] splitAjax = Regex.Split(ajaxRequestURL,"pagesize=") ;
								string pagesizeStr = FBUtils.getBetween(splitAjax[1],"=","&");
								string[] splitAjax1 = Regex.Split(splitAjax[1],"="+pagesizeStr+"&") ;
								ajaxRequestURL = splitAjax[0] + "pagesize=20&" + splitAjax1[1];
								res_ajaxRequest = HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

							}
							catch(Exception ex)
							{
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}


					







						
					}
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}

*/
				}


				if(true)
				{

					if(true)
					{


						//https://www.facebook.com/search/results/?q=sachin&type=pages

						//<a href="/groups/
						try
						{
							string[] PageSplit = Regex.Split(pgSrc_FanPageSearch,"href=\"https://www.facebook.com/pages/");    //data-profileid=
							if(PageSplit.Count()!=1)
							{
								List<string> PageSplitList = PageSplit.ToList();
								PageSplitList.RemoveAt(0);
								foreach(string item in PageSplitList)
								{
									try
									{
										if(item.Contains("<!DOCTYPE html>"))
										{
											continue;
										}

										list = list.Distinct().ToList();
										if(PageManager.noOfPageToScrap <= list.Count())
										{
											break;
										}

										string pageId = FBUtils.getBetween(item,"/","?");
										if(!string.IsNullOrEmpty(pageId))
										{
											list.Add(pageId);
											AddToLogger_PageManager("Added Page Id : " +  pageId);
										}
									}
									catch{}
								}
							}
							else
							{
								try
								{
									PageSplit = Regex.Split(pgSrc_FanPageSearch,"data-profileid="); 

									List<string> PageSplitList = PageSplit.ToList();
									PageSplitList.RemoveAt(0);
									foreach(string item in PageSplitList)
									{
										try
										{
											if(item.Contains("<!DOCTYPE html>"))
											{
												continue;
											}
											list = list.Distinct().ToList();
											if(PageManager.noOfPageToScrap <= list.Count())
											{
												break;
											}


											string pageId = FBUtils.getBetween(item,"\"","\"");
											if(!string.IsNullOrEmpty(pageId))
											{
												list.Add(pageId);
												AddToLogger_PageManager("Added Page Id : " +  pageId);
											}
										}
										catch{}
									}

								}
								catch{};

							}

						}
						catch{};

						int countForlistIteminPrvious =0;
						int countFormaximumScrap = 0;
						while(true)
						{

							countFormaximumScrap++;
							if(PageManager.noOfPageToScrap<=countFormaximumScrap)
							{
								AddToLogger_PageManager("No. of Pages Found To Scrap : " +  list.Count());
								break;							
							}



							list = list.Distinct().ToList();
							countForlistIteminPrvious = list.Count();

							try
							{


								list = list.Distinct().ToList();
								if(PageManager.noOfPageToScrap <= list.Count())
								{
									AddToLogger_PageManager("No. of Pages Found To Scrape : " +  list.Count());
									break;
								}

								string[] PageSplit = Regex.Split(pgSrc_FanPageSearch,"rel=\"ajaxify\"");  //rel=\"ajaxify\"

								if(PageSplit.Count()==1)
								{
									string splitIt = "&amp;offset=";
									PageSplit = Regex.Split(pgSrc_FanPageSearch,splitIt);  //rel=\"ajaxify\"
									if(PageSplit.Count()==1)
									{
										AddToLogger_PageManager("All Page Id Scraped ");
										break;
									}

									if(PageSplit.Count()>1)
									{

										PageSplit[1]  =  "/search/results/more/?q=" + Keyword + "&amp;offset=" + PageSplit[1] ;
										ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"","\\\"");

									}




								}
								else
								{

									ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"href=\"","\"");
								}

								ajaxRequestURL = ajaxRequestURL.Replace("amp;","").Replace("type=all","type=pages").Replace("\\","%2C").Replace("u00252C","");

								ajaxRequestURL = "https://www.facebook.com" +  ajaxRequestURL + "&__user=" + __user + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" +  FBUtils.getBetween(PageSplit[1],"revision\":",",");

								pgSrc_FanPageSearch =  HttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
								string allListGroup  = FBUtils.getBetween(pgSrc_FanPageSearch,"&quot;ents&quot;:&quot;","&quot");
								string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
								foreach(string item in Linklist)
								{

									list = list.Distinct().ToList();
									if(PageManager.noOfPageToScrap < list.Count())
									{
										break;
									}


									try
									{
										if(!string.IsNullOrEmpty(item))
										{
											list.Add(item);
											AddToLogger_PageManager("Added Page Id : " +  item);

										}
									}
									catch{};

								}
							
								if(countForlistIteminPrvious==list.Count())
								{
									AddToLogger_PageManager("No. of Pages Id Found To Scrape  : " +  list.Count());
									break;
								}

								list = list.Distinct().ToList();

							}
							catch{};
						}



					}




				}

				#region for find friend Reqest Link

				//	List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();

				/*
			    string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
			//	Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
				foreach (string itemurl in Linklist)
				{
					if (!itemurl.Contains("<!DOCTYPE html"))
					{
						if (itemurl.Contains(@"https:\/\/www.facebook.com"))   //(itemurl.Contains(@"https:\/\/www.facebook.com"))
						{
							lstLinkData.Add(itemurl);
							string strLink = itemurl.Substring(0, 70);
							if (strLink.Contains(@"https:\/\/www.facebook.com") && (strLink.Contains("onclick") || strLink.Contains("data-gt=\\")))   //("https:\\/\\/www.facebook.com") && (strLink.Contains("onclick") || strLink.Contains("data-gt=\\")))
							{
								try
								{
									string[] tempArr = strLink.Split('"');
									string temp = tempArr[1];
									temp = temp.Replace("\\", "");
									list.Add(temp);
									AddToLogger_PageManager("KeyWord : "+Keyword +"Fan page Url : "+temp);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
						}
					}
				}

*/
				#endregion
				list = list.Distinct().ToList();
				//	distinctPages.AddRange(list);
				//FanPageUrls = distinctPages;



			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace); 
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  FindTheFanPagesByKeyword  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return list;
		}































		public List<string> CrawlFanpageOld2(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add(lstUrlitem);
				}
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{


					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}

					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch{};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch{};

									}
									catch{};
								}





								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */


								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "FanpageUrl : " + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk + "," + lstKeywordiditem + "," + fpagename + "," +  fpagecategoryname ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch{};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return FindFanpageList;
		}







		public List<string> CrawlFanpageOld_old(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add("https://www.facebook.com/" + lstUrlitem);
				}
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{
					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}
					string strcategoryurl = "";
					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					}
					catch{};
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch{};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch{};

									}
									catch{};
								}




								if(fpagecategoryname.Contains("<span"))
								{
									fpagecategoryname = fpagecategoryname.Replace("<span","").Replace("</span>","");
									if(fpagecategoryname.Contains("class"))
									{
										try{
											string[] fpagecategoryname_Split = Regex.Split(fpagecategoryname,"\">");
											fpagecategoryname = fpagecategoryname_Split[1];
										}
										catch{};

									}
								}



								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */


								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "FanpageUrl : " + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk + "," + lstKeywordiditem + "," + fpagename + "," +  fpagecategoryname ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch{};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return FindFanpageList;

		}







		public List<string> CrawlFanpageNewOld(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add("https://www.facebook.com/" + lstUrlitem);
				}
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{
					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}
					string strcategoryurl = "";
					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					}
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					};
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
									catch(Exception ex){
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									};
								}


								if(fpagename.ToLower().Contains("requests"))
								{

									if(strcategoryurl.Contains("_54yq"))
									{
										string[] fpagenameList = Regex.Split(strcategoryurl,"_54yq");
										fpagename = FBUtils.getBetween(fpagenameList[1],"<h2>","</h2>");

									}
								}


								if(fpagecategoryname.ToLower().Contains("sidebar")||fpagecategoryname.Contains("_") )
								{

									if(strcategoryurl.Contains("fsm fwn fcg"))
									{
										string[] fpagenameList = Regex.Split(strcategoryurl,"fsm fwn fcg");
										fpagename = FBUtils.getBetween(fpagenameList[4],">","<");

									}
								}






								if(fpagecategoryname.Contains("<span"))
								{
									fpagecategoryname = fpagecategoryname.Replace("<span","").Replace("</span>","");
									if(fpagecategoryname.Contains("class"))
									{
										try{
											string[] fpagecategoryname_Split = Regex.Split(fpagecategoryname,"\">");
											fpagecategoryname = fpagecategoryname_Split[1];
										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
								}





								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */








								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "FanpageUrl : " + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk + "," + lstKeywordiditem + "," + fpagename + "," +  fpagecategoryname ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return FindFanpageList;

		}









		public List<string> CrawlFanpage_Newest_Old(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();


				/*
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add("https://www.facebook.com/" + lstUrlitem);
				}



*/
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{
					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}
					string strcategoryurl = "";
					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/"+lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					}
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					};
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
									catch(Exception ex){
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									};
								}


								if(fpagename.ToLower().Contains("requests"))
								{

									if(strcategoryurl.Contains("_54yq"))
									{
										string[] fpagenameList = Regex.Split(strcategoryurl,"_54yq");
										fpagename = FBUtils.getBetween(fpagenameList[1],"<h2>","</h2>");

									}
								}


								if(string.IsNullOrEmpty(fpagename))
								{

									if(strcategoryurl.Contains("_54yq"))
									{
										string[] fpagenameList = Regex.Split(strcategoryurl,"_54yq");
										fpagename = FBUtils.getBetween(fpagenameList[1],"<h2>","</h2>");

									}
								}






								if(fpagecategoryname.ToLower().Contains("sidebar")||fpagecategoryname.Contains("_") )
								{
									/*
									if(strcategoryurl.Contains("fsm fwn fcg"))
									{
										string[] fpagenameList = Regex.Split(strcategoryurl,"fsm fwn fcg");
										fpagename = FBUtils.getBetween(fpagenameList[4],">","<");

									} 
                                     */
									if (strcategoryurl.Contains("<span class=\"fsm fwn fcg\""))
									{
										if (!strcategoryurl.Contains("class=\"fsm fwn fcg\">Topic<"))
										{


											string[] fpagenameList = Regex.Split(strcategoryurl, "<span class=\"fsm fwn fcg\"");
											fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[1], "href")[1], ">", "<");
										}
										else
										{
											fpagecategoryname = "Topic";

										}

									}

								}


							




								if(fpagecategoryname.Contains("<span"))
								{
									fpagecategoryname = fpagecategoryname.Replace("<span","").Replace("</span>","");
									if(fpagecategoryname.Contains("class"))
									{
										try{
											string[] fpagecategoryname_Split = Regex.Split(fpagecategoryname,"\">");
											fpagecategoryname = fpagecategoryname_Split[1];
										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
								}





								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */








								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "   FanpageUrl :" + "https://www.facebook.com/" + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk.Replace(",","") + "," +  "https://www.facebook.com/" + lstKeywordiditem + "," + fpagename.Replace(",","") + "," +  fpagecategoryname.Replace(",","") ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return FindFanpageList;

		}











		public List<string> CrawlFanpage_Old_3_Oct(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			// List<string> arr = (List<string>)paramsArray.GetValue(1);
			// string lk = arr[0];
			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );





				// List<string> lstKeywordid = Extractkeywordid(ref fbUser, lkey,UserId);
				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();


				/*
				foreach (string MessageLinkitem in MessageLink)
				{
					string id = "";
					try
					{
						if (MessageLinkitem.Contains("id") && !MessageLinkitem.Contains("<!DOCTYPE html>"))
						{
							//string id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), 25).Replace("id&quot;:", string.Empty);
							try
							{
								id = MessageLinkitem.Substring(MessageLinkitem.IndexOf("id"), MessageLinkitem.IndexOf("\"", MessageLinkitem.IndexOf("id")) - MessageLinkitem.IndexOf("id")).Replace("id&quot;:", string.Empty).Replace("id=", string.Empty);

								if (id.Contains(","))
								{
									id = id.Substring(0, id.IndexOf(",") - 0).Replace("&", string.Empty);
								}
								listurl.Add(id);
								//GlobusLogHelper.log.Debug("Fan Page ID : " + id + " Keyword : " + lkey + " with UserName : " + fbUser.username);

							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				if (listurl.Count > 0)
				{
					try
					{

						if (listurl.Contains("id"))
						{
							listurl.Remove("id");
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				foreach (string lstUrlitem in listurl)
				{
					lstKeywordid1.Add("https://www.facebook.com/" + lstUrlitem);
				}



*/
				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{
					if (noOfPicsPerURL<=Counter)
					{
						Counter=0;
						break;
					}
					string strcategoryurl = "";
					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/"+lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					}
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					};
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
									catch(Exception ex){
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									};
								}

								try
								{
									if ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")))
									{


										if ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) && strcategoryurl.Contains(">About<"))
										{
											try
											{

												string[] fpagenameList = Regex.Split(strcategoryurl, ">About<");
												fpagename = FBUtils.getBetween(fpagenameList[2], "<h2>", "</h2>");
											}
											catch { };

										}


										if (((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) || string.IsNullOrEmpty(fpagename)) && strcategoryurl.Contains("_54yq"))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, "_54yq");
												fpagename = FBUtils.getBetween(fpagenameList[1], "<h2>", "</h2>");
												fpagename = fpagename.Replace("amp;", "");
											}
											catch { };

										}

										else if (strcategoryurl.Contains("class=\"_2i5e\"") && ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) || string.IsNullOrEmpty(fpagename)))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, "class=\"_2i5e\"");
												fpagename = FBUtils.getBetween(fpagenameList[1], "<h1>", "</h1>");
												fpagename = fpagename.Replace("amp;", "");
											}
											catch { };

										}
									}
								}
								catch { };



								try
								{

									if (fpagecategoryname.ToLower().Contains("sidebar") || fpagecategoryname.Contains("_"))
									{
										/*
                                        if(strcategoryurl.Contains("fsm fwn fcg"))
                                        {
                                            string[] fpagenameList = Regex.Split(strcategoryurl,"fsm fwn fcg");
                                            fpagecategoryname = FBUtils.getBetween(fpagenameList[4], ">", "<");

                                        }
                                         */



										if ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_")) && strcategoryurl.Contains(">About<"))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, ">About<");
												fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[2], "href")[1], ">", "<");
											}
											catch { };

										}

										if (strcategoryurl.Contains("<span class=\"fsm fwn fcg\"") && ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_"))|| string.IsNullOrEmpty(fpagecategoryname)))
										{
											try
											{
												if (!strcategoryurl.Contains("class=\"fsm fwn fcg\">Topic<"))
												{


													string[] fpagenameList = Regex.Split(strcategoryurl, "<span class=\"fsm fwn fcg\"");
													fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[1], "href")[1], ">", "<");
												}
												else
												{
													fpagecategoryname = "Topic";

												}
											}
											catch { };

										}
										if ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_")) || string.IsNullOrEmpty(fpagecategoryname) || fpagecategoryname.Contains("like this topic"))
										{
											fpagecategoryname = "Topic";

										}




										//
										// class="_2i5e"




									}
								}
								catch { };

								/*
                                try
                                {
                                    if (string.IsNullOrEmpty(fpagecategoryname))
                                    {
                                        if (strcategoryurl.Contains("<span class=\"fsm fwn fcg\""))
                                        {
                                            string[] fpagenameList = Regex.Split(strcategoryurl, "<span class=\"fsm fwn fcg\"");
                                            fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[1],"href")[1], ">", "<");

                                        }
 
                                    }

                                }
                                catch { };
                                 */

								if (fpagename.Contains("amp;"))
								{
									fpagename = fpagename.Replace("amp;", "");
								}

								if (fpagecategoryname.Contains("amp;"))
								{
									fpagecategoryname = fpagecategoryname.Replace("amp;", "");
								}





								if(fpagecategoryname.Contains("<span"))
								{
									fpagecategoryname = fpagecategoryname.Replace("<span","").Replace("</span>","");
									if(fpagecategoryname.Contains("class"))
									{
										try{
											string[] fpagecategoryname_Split = Regex.Split(fpagecategoryname,"\">");
											fpagecategoryname = fpagecategoryname_Split[1];
										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
								}





								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */








								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "   FanpageUrl :" + "https://www.facebook.com/" + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk.Replace(",","") + "," +  "https://www.facebook.com/" + lstKeywordiditem + "," + fpagename.Replace(",","-") + "," +  fpagecategoryname.Replace(",","-") ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return FindFanpageList;

		}







		public List<string> CrawlFanpage(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker,string lk)
		{
			//string lk =string.Empty;
			List<string> FindFanpageList = new List<string> ();

			int Counter = 1;

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}

			try
			{
				GlobusHttpHelper chilkatHttpHelper =fbUser.globusHttpHelper;
				string lkey = lk;
				string SearchUrl = FBGlobals.Instance.pageManagerFanPageScraperSearchResultUrl + lk;                         //"http://www.facebook.com/search/results.php?q="

				string PgSrcKeywordSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(SearchUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				string UserId=GlobusHttpHelper.GetParamValue(PgSrcKeywordSearch, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(PgSrcKeywordSearch, "user");
				}

				List<string> lstKeywordid1 =FindTheFanPagesByKeyword(lkey,ref fbUser );

				string[] MessageLink = System.Text.RegularExpressions.Regex.Split(PgSrcKeywordSearch, "instant_search_title fsl fwb fcb");

				List<string> listurl = new List<string>();



				lstKeywordid1 = lstKeywordid1.Distinct().ToList();
				Dictionary<string, string> CheckDupliCats = new Dictionary<string, string>();
				foreach (string lstKeywordiditem in lstKeywordid1)
				{
					if (noOfPicsPerURL<Counter)
					{
						Counter=0;
						break;
					}
					string strcategoryurl = "";
					//string strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					try
					{
						strcategoryurl = chilkatHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/"+lstKeywordiditem),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   
					}
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					};
					try
					{
						if (strcategoryurl.Contains("categories\":") && strcategoryurl.Contains("name\":"))
						{

							string fanpagecat = strcategoryurl.Substring(strcategoryurl.IndexOf("categories\":")).Replace("categories\":", string.Empty);
							string fanpagename = strcategoryurl.Substring(strcategoryurl.IndexOf("name\":")).Replace("name\":", string.Empty);
							string fanpageurl = strcategoryurl.Substring(strcategoryurl.IndexOf("link\":")).Replace("link\":", string.Empty);
							string[] fanpagecatArr = Regex.Split(fanpagecat, "\"");
							string[] fanpagenameArr = Regex.Split(fanpagename, "\"");
							string[] fanpageurlArr = Regex.Split(fanpageurl, "\"");
							string fpcategoryname = fanpagecatArr[1].Replace("\"", string.Empty);
							string fpageurl = fanpageurlArr[1].Replace("\"", string.Empty);
							listFanPageUrl.Add(fpageurl);
							string fpname = fanpagenameArr[1].Replace("\"", string.Empty);
							string fanpName = fpname.Trim().Replace(" ", string.Empty);                           

							if (!string.IsNullOrEmpty(ScrapersExprotFilePath))
							{

								string Fanpurl = string.Empty;
								string Fpagekeyword = string.Empty;
								string fpagecategoryname = string.Empty;
								string fpagename = string.Empty;
								Fanpurl = fpageurl;
								Fpagekeyword = lk;
								fpagecategoryname = fpcategoryname;
								fpagename = fpname;

								if(strcategoryurl.Contains("fb-timeline-cover-name"))
								{
									continue;
								}


								if(strcategoryurl.Contains("fbTimelineHeadline"))
								{

									try
									{
										string[] strcategoryurlSplit1 = Regex.Split(strcategoryurl,"fbTimelineHeadline");
										try
										{
											fpagename = FBUtils.getBetween(strcategoryurlSplit1[1],"<span>","</span>");

										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										try
										{
											fpagecategoryname = FBUtils.getBetween(strcategoryurlSplit1[1],"fwn fcw\">","</div>");

										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
									catch(Exception ex){
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									};
								}

								try
								{
									if ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")))
									{


										if ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) && strcategoryurl.Contains(">About<"))
										{
											try
											{

												string[] fpagenameList = Regex.Split(strcategoryurl, ">About<");
												fpagename = FBUtils.getBetween(fpagenameList[2], "<h2>", "</h2>");
											}
											catch { };

										}


										if (((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) || string.IsNullOrEmpty(fpagename)) && strcategoryurl.Contains("_54yq"))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, "_54yq");
												fpagename = FBUtils.getBetween(fpagenameList[1], "<h2>", "</h2>");
												fpagename = fpagename.Replace("amp;", "");
											}
											catch { };

										}

										else if (strcategoryurl.Contains("class=\"_2i5e\"") && ((fpagename.ToLower().Contains("requests") || fpagename.ToLower().Contains("_")) || string.IsNullOrEmpty(fpagename)))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, "class=\"_2i5e\"");
												fpagename = FBUtils.getBetween(fpagenameList[1], "<h1>", "</h1>");
												fpagename = fpagename.Replace("amp;", "");
											}
											catch { };

										}
									}
								}
								catch { };



								try
								{

									if (fpagecategoryname.ToLower().Contains("sidebar") || fpagecategoryname.Contains("_"))
									{
										/*
                                        if(strcategoryurl.Contains("fsm fwn fcg"))
                                        {
                                            string[] fpagenameList = Regex.Split(strcategoryurl,"fsm fwn fcg");
                                            fpagecategoryname = FBUtils.getBetween(fpagenameList[4], ">", "<");

                                        }
                                         */



										if ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_")) && strcategoryurl.Contains(">About<"))
										{
											try
											{
												string[] fpagenameList = Regex.Split(strcategoryurl, ">About<");
												fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[2], "href")[1], ">", "<");
											}
											catch { };

										}

										if (strcategoryurl.Contains("<span class=\"fsm fwn fcg\"") && ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_"))|| string.IsNullOrEmpty(fpagecategoryname)))
										{
											try
											{
												if (!strcategoryurl.Contains("class=\"fsm fwn fcg\">Topic<"))
												{


													string[] fpagenameList = Regex.Split(strcategoryurl, "<span class=\"fsm fwn fcg\"");
													fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[1], "href")[1], ">", "<");
												}
												else
												{
													fpagecategoryname = "Topic";

												}
											}
											catch { };

										}
										if ((fpagecategoryname.Contains("sidebar") || fpagecategoryname.Contains("_")) || string.IsNullOrEmpty(fpagecategoryname) || fpagecategoryname.Contains("like this topic"))
										{
											fpagecategoryname = "Topic";

										}




										//
										// class="_2i5e"




									}
								}
								catch { };

								/*
                                try
                                {
                                    if (string.IsNullOrEmpty(fpagecategoryname))
                                    {
                                        if (strcategoryurl.Contains("<span class=\"fsm fwn fcg\""))
                                        {
                                            string[] fpagenameList = Regex.Split(strcategoryurl, "<span class=\"fsm fwn fcg\"");
                                            fpagecategoryname = FBUtils.getBetween(Regex.Split(fpagenameList[1],"href")[1], ">", "<");

                                        }
 
                                    }

                                }
                                catch { };
                                 */

								if (fpagename.Contains("amp;"))
								{
									fpagename = fpagename.Replace("amp;", "");
								}

								if (fpagecategoryname.Contains("amp;"))
								{
									fpagecategoryname = fpagecategoryname.Replace("amp;", "");
								}





								if(fpagecategoryname.Contains("<span"))
								{
									fpagecategoryname = fpagecategoryname.Replace("<span","").Replace("</span>","");
									if(fpagecategoryname.Contains("class"))
									{
										try{
											string[] fpagecategoryname_Split = Regex.Split(fpagecategoryname,"\">");
											fpagecategoryname = fpagecategoryname_Split[1];
										}
										catch(Exception ex){
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};

									}
								}





								/*
								AddToLogger_PageManager(Counter +" :  Keyword : " +Fpagekeyword + "FanpageUrl : "+Fanpurl +"FanPageName : "+fpagename);
								string CSVHeader = "Keyword" + "," + "FanpageUrl" + ", " + "FanpageCategoryName" + "," + "FanPageName";
								string CSV_Content = Fpagekeyword + "," + Fanpurl + ", " + fpagecategoryname + "," + fpagename;
                                */








								AddToLogger_PageManager( Counter +" :  Keyword : " +lk + "   FanpageUrl :" + "https://www.facebook.com/" + lstKeywordiditem  + "  FanPageName : " +fpagename+ "   FanpageCategoryName : " + fpagecategoryname);


								List<string> lstofCsvData =   Globussoft.GlobusFileHelper.ReadFile(ScrapersExprotFilePath);
								string wholeDataOfCsv = "";
								foreach(string str in lstofCsvData)
								{
									wholeDataOfCsv = wholeDataOfCsv + str;

								}

								if(!wholeDataOfCsv.Contains(lstKeywordiditem))
								{

									string CSVHeader =  "Keyword" + "," + "FanpageUrl" + "," + "FanPageName" + "," + "FanpageCategoryName"  ;
									string CSV_Content = lk.Replace(",","") + "," +  "https://www.facebook.com/" + lstKeywordiditem + "," + fpagename.Replace(",","-") + "," +  fpagecategoryname.Replace(",","-") ;

									Counter=Counter+1;
									try
									{
										try
										{
											CheckDupliCats.Add(Fanpurl, Fanpurl);
										}
										catch(Exception ex)
										{
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
										};
										ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath); 
										AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
										//AddToLogger_PageManager("Data Saved In CSV ." + CSV_Content);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								else
								{
									AddToLogger_PageManager("This Data Already Present in CSV");									
								}
							}

						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				Console.WriteLine("Task Is Completed with  :" + lk + " Keyword");

				FindFanpageList=lstKeywordid1.Distinct().ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  CrawlFanpage  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return FindFanpageList;

		}
































		public static void ExportDataCSVFile(string CSV_Header, string CSV_Content, string CSV_FilePath)
		{
			try
			{
				if (!File.Exists(CSV_FilePath))
				{
					AppendStringToTextFile(CSV_FilePath, CSV_Header);
				}
				AppendStringToTextFile(CSV_FilePath, CSV_Content);				 

			}
			catch (Exception ex)
			{

			}
		}




		public static void AppendStringToTextFile(string FilePath, string content)
		{
			//Encoding encodingOfChoice = Encoding.UTF8;
			//byte[] bytes = encodingOfChoice.GetBytes(content);
			//using (StreamWriter sw = new StreamWriter(FilePath, true, Encoding.UTF8))
			//Encoding encoding = Encoding.GetEncoding(28591);


			//  Object obj = new Object();


			using (Mutex myMutex = new Mutex(true, "Some name that is unlikly to clash with other mutextes"))
			{
				myMutex.WaitOne();

				try
				{
					using (StreamWriter sw = new StreamWriter (FilePath, true)) {
						sw.WriteLine (content);
						sw.Close ();
					}
				}
				finally
				{
					myMutex.ReleaseMutex();
				}
			}


		}















		public static string GetAjaxURL_MoreResults(string pgSrc)
		{
			string pattern = "search/ajax/more.php?offset";

			if (pgSrc.Contains(pattern))
			{
				int startIndx = pgSrc.IndexOf(pattern);
				int endIndx = pgSrc.IndexOf("\"", startIndx);
				string URL = pgSrc.Substring(startIndx, endIndx - startIndx);
				URL = URL.Replace("pagesize=10", "pagesize=300").Replace("&amp;", "&");
				return URL;
			}
			return string.Empty;
		}




		public static List<string> GetPages_FBSearch(string pgSrc)
		{
			List<string> lst_Pages = new List<string>();

			string splitPattern = "/hovercard/";

			string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

			foreach (string item in splitPgSrc)
			{
				if (!item.Contains("<!DOCTYPE html>"))
				{
					int startIndx = item.IndexOf("page.php?id=") + "page.php?id=".Length;
					int endIndx = item.IndexOf(">", startIndx);
					string pageURL = "http://www.facebook.com/" + item.Substring(startIndx, endIndx - startIndx).Replace("\"", "").Replace("=", "");

					lst_Pages.Add(pageURL);
				}
			}

			return lst_Pages;
		}









		/*public void StartLikePageMultiThreads(object parameters)
		{
			try
			{
				if (!isStopFanPageLiker)
				{
					try
					{
						lstThreadsFanPageLiker.Add(Thread.CurrentThread);
						lstThreadsFanPageLiker.Distinct();
						Thread.CurrentThread.IsBackground = true;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}




					try
					{

						Array paramsArray = new object[1];
						paramsArray = (Array)parameters;

						FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);

						if (!objFacebookUser.isloggedin)
						{
							GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

							objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


							//Login Process

							Accounts.AccountManager objAccountManager = new AccountManager();


							objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
						}

						if (objFacebookUser.isloggedin)
						{
							// Call LikePage
							StartFanPageLikerProcess(ref objFacebookUser);
						}
						else
						{
							//AddToLogger_PageManager("Couldn't Login With Username : " + objFacebookUser.username);
							//GlobusLogHelper.log.Debug("Couldn't Login With Username : " + objFacebookUser.username);
						}


					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			finally
			{
				try
				{
					//  if (!isStopFanPageLiker)
					{
						lock (lockrThreadControllerFanPageLiker)
						{
							countThreadControllerFanPageLiker--;
							Monitor.Pulse(lockrThreadControllerFanPageLiker);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
			}
		} */



		public void LikePage(ref FacebookUser fbUser, List<string> lstFanPageUrlsFanPageLiker)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				#region Post variable

				string fbpage_id = string.Empty;
				string fb_dtsg = string.Empty;
				string __user = string.Empty;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string xhpc_composerid12 = string.Empty;
				int NoOfEmailAccount = 20;
				int countPost=0;

				#endregion

				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}

				List<string> FanpageUrls = lstFanPageUrlsFanPageLiker;

				foreach (string item in FanpageUrls)
				{
					try
					{
						string FanpageUrl = item;				

						string PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanpageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

						//PostOnFanPageCommentWithURLAndItsImage(ref HttpHelper);
						///JS, CSS, Image Requests
						//RequestsJSCSSIMG.RequestJSCSSIMG(PageSrcFanPageUrl, ref HttpHelper);

						#region Extra Requests Before Like
						string aed = string.Empty;

						if (PageSrcFanPageUrl.Contains("aed="))
						{
							try
							{
								string strfb_dtsg = PageSrcFanPageUrl.Substring(PageSrcFanPageUrl.IndexOf("aed="), 1500);
								string[] Arrfb_dtsg = strfb_dtsg.Split('"');
								aed = Arrfb_dtsg[0];
								aed = aed.Replace("\\", "");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}

						}

						string strUrlAed = FBGlobals.Instance.PageManagerFanPageLikerstrUrlAed + aed;

						try
						{
							string PageSrcFanPageUrlAed = HttpHelper.getHtmlfromUrl(new Uri(strUrlAed),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						///JS, CSS, Image Requests
						//RequestsJSCSSIMG.RequestJSCSSIMG(PageSrcFanPageUrlAed, ref HttpHelper);

						#endregion

					AddToLogger_PageManager("Liking : " + FanpageUrl + " With UserName : " + fbUser.username);

					//	Thread.Sleep(Utils.GenerateRandom(300, 1200));
						#region Post Data Params

						__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");     //pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);
						if (string.IsNullOrEmpty(__user))
						{
							__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
						}

						if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
						{
							AddToLogger_PageManager("Please Check The Account : " + fbUser.username);
							//GlobusLogHelper.log.Debug("Please Check The Account : " + fbUser.username);

							return;
						}

						try
						{
							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcFanPageUrl);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						#endregion

						#region Get FB Page ID Modified

						///Get FB Page ID
						fbpage_id = GlobusHttpHelper.GetPageID(PageSrcFanPageUrl, ref FanpageUrl);

						#endregion

						//#region Modified 7-6-12

						string postURL_1st = FBGlobals.Instance.PageManagerFanPageLikerpostURL1st;
						string postData_1st = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&nctr[_mod]=pagelet_timeline_page_actions&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" + GlobusHttpHelper.GenerateTimeStamp() + "";

						string res_post_1st = string.Empty;

						try
						{
							res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						if (string.IsNullOrEmpty(res_post_1st))
						{
							try
							{

								//fbpage_id=107539342664071&add=true&reload=false&fan_origin=page_timeline&fan_source=&cat=&nctr[_mod]=pagelet_timeline_page_actions&__user=100001330463773&__a=1&__dyn=798ahxoNoBKfEa0&__req=o&fb_dtsg=AQDXABnH&phstamp=165816888656611072206
								string postData_2 = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&fan_source=&cat=&nctr[_mod]=pagelet_timeline_page_actions&__user=" + __user + "&__a=1&__dyn=&__req=o&fb_dtsg=" + fb_dtsg + "" + "&phstamp=" + GlobusHttpHelper.GenerateTimeStamp() + "";

								res_post_1st = HttpHelper.postFormData(new Uri(FBGlobals.Instance.PageManagerFanPageLikerFanStatus), postData_2);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}
						}

						if (res_post_1st.Contains("Security Check Required"))
						{
							string content = fbUser.username + ":" + fbUser.password;

							AddToLogger_PageManager("Security Check Required " + FanpageUrl + "  with " + fbUser.username);
							//GlobusLogHelper.log.Debug("Security Check Required " + FanpageUrl + "  with " + fbUser.username);

						}
						else if (res_post_1st.Contains("Already connected"))
						{
							string content = fbUser.username + ":" + fbUser.password;

						}

						else if (res_post_1st.Contains("\"errorSummary\""))
						{


						}
						else
						{
							// WriteLikedCounterAtLabel();
							//CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, Globals.path_LikedPages);
						}
						try
						{
							int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
							AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
							Thread.Sleep(delayInSeconds);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

			AddToLogger_PageManager("Process Completed With Username >>> " + fbUser.username);
			//GlobusLogHelper.log.Debug("Process Completed With Username >>> " + fbUser.username);
		}


		//   FanPagePoster  //
		string	StartProcessUsingFanPagePoster=string.Empty;
		public List<string> lstFanPageUrlCollectionFanPagePoster = new List<string>();
		public List<string> lstFanPageUrlCollectionFanPagePostUrl = new List<string>();
		public bool isStopFanPageUrlPoster = false;
		public List<Thread> lstThreadsFanPagePoster = new List<Thread>();
		int countThreadControllerFanPagePoster = 0;
		static readonly object lockr_que_FanPageURLs = new object();
		static readonly object lockr_que_FanPageMessages = new object();
		public static List<string> lstFanPagePostURLs = new List<string>();
		public static List<string> lstFanPageUrlCollectionFanPagePostMessages = new List<string> ();
		public static int minDelayFanPagePoster = 10;
		public static int maxDelayFanPagePoster = 20;
		public static bool CheckDataBase = false;
		public List<string> lstFanPageURLs = new List<string>();

		public static int noOfPicsPerURL 
		{
			get;
			set;
		}





		private void StartActionFanPagePoster(ref FacebookUser fbUser)
		{           
			try
			{
				if (StartProcessUsingFanPagePoster == "Post Urls")
				{
					PostOnFanPageWallWithURLAndItsImage(ref fbUser);
				}
				else if (StartProcessUsingFanPagePoster == "Post Simple Message")
				{
					AddToLogger_PageManager("Process Started With : " + fbUser.username);
					PostFanPageMessageUsingUrls(ref fbUser);
					AddToLogger_PageManager("Process completed With : " + fbUser.username);

				}
				else if (StartProcessUsingFanPagePoster == "Post Picture On Own Page ")
				{
					UploadImage(ref fbUser);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		public void UploadImageOld(ref FacebookUser fbUser)
		{
			try
			{
				foreach (string item in lstFanPageUrlCollectionFanPagePoster)
				{
					try
					{

						if (item.Contains("www.facebook.com/pages/"))
						{
							StartImageUploading(ref fbUser, item);
						}
						else
						{
							StartImageUploadingOnPage(ref fbUser, item);
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}


		public void UploadImage(ref FacebookUser fbUser)
		{

			//Array paramsArray = new object[1];
			//paramsArray = (Array)fbUser;



			try
			{
				foreach (string item in lstFanPageUrlCollectionFanPagePoster)
				{

					FacebookUser objFacebookUser = fbUser;
					try
					{

						//if (!objFacebookUser.isloggedin)
						{
							GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

							objFacebookUser.globusHttpHelper = objGlobusHttpHelper;


							//Login Process

							AccountManager objAccountManager = new AccountManager();

							//AddToLogger_PageManager("Logging in with " + objFacebookUser.username);;
							objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
						}
					}
					catch{};



					try
					{

						if (item.Contains("www.facebook.com/pages/"))
						{
							StartImageUploading(ref objFacebookUser, item);
						}
						else
						{
							StartImageUploadingOnPage(ref objFacebookUser, item);
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}













		public void StartImageUploadingOnPage(ref FacebookUser fbUser, string PageUrl)
		{
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

			string strUserName = fbUser.username;
			string strProxyAddress = fbUser.proxyusername;
			string strProxyUserName = fbUser.proxyusername;
			string strProxyPassword = fbUser.proxypassword;
			string strProxyPort = fbUser.proxyport;

			try
			{
				AddToLogger_PageManager(" Starting Image Upload With Username : " + strUserName + " On The URL : " + PageUrl);


				string pics = string.Empty;
				string message = string.Empty;
				string pageId = string.Empty;
				string session_id = string.Empty;
				string grid_Id = string.Empty;
				string waterfallId=string.Empty;

				if (string.IsNullOrEmpty(strProxyPort))
				{
					strProxyPort = "80";
				}

				string res_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");     //"https://www.facebook.com/"

				string __user = GlobusHttpHelper.GetParamValue(res_Home, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(res_Home, "user");
				}

				string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(res_Home);
				string res_get_FanPage = HttpHelper.getHtmlfromUrl(new Uri(PageUrl),"","");
				string ComposerId = string.Empty;
				pageId = GlobusHttpHelper.getBetween(res_get_FanPage, "{\"pageID\":\"", "\"}]]]");

				ComposerId=GlobusHttpHelper.getBetween(res_get_FanPage,"composerid\" value=\"","\"");
				#region for
				for (int i = 0; i < noOfPicsPerURL; i++)
				{
					try
					{
						if (picsCounter < lstPicturecollectionPostPicOnFanPageWall.Count)
						{
							pics = lstPicturecollectionPostPicOnFanPageWall[picsCounter];
							picsCounter++;
						}
						else
						{
						AddToLogger_PageManager("All pics used up : " + strUserName);
						
							return;
						}

						if (lstFanPageCollectionFanPagePosterMessage.Count > 0)
						{
							message = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
						}
						string ImagePostClickResp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/media/upload/?av=" + __user + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + ComposerId + "&targetid=" + pageId + "&composercontext=composer&isfinch=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=placetaggericon&loaded_components[7]=maininput&nctr[_mod]=pagelet_timeline_main_column&__user=" + __user + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUW5ogDyQqUjhpoW8xOdy8-&__req=g&ttstamp=2658170819911711579109529989&__rev=1561259");
						grid_Id = GlobusHttpHelper.getBetween(ImagePostClickResp, "\"gridID\":\"", "\"");
						waterfallId=GlobusHttpHelper.getBetween(ImagePostClickResp,"\"waterfallID\":\"","\""); 
						NameValueCollection nvc = new NameValueCollection();
						nvc.Add("fb_dtsg",fb_dtsg);
						nvc.Add("source","8");
						nvc.Add("profile_id",__user);
						nvc.Add("grid_id",grid_Id);
						nvc.Add("qn",waterfallId);
						nvc.Add("0", "" + pics + "<:><:><:>image/jpeg");
						nvc.Add("upload_id", "1024");
						string imgUploadResp = HttpHelper.UploadImageWaterfallModel("https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + pageId + "&image_height=100&image_width=100&letterbox=0&av=" + __user + "&qn=" + waterfallId + "&__user=" + __user + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4pbGAdBGeqrWo8ponUKexm49UJ6K4Qmmey8szoyfw&__req=p&fb_dtsg=" + fb_dtsg + "&ttstamp=2658170819911711579109529989&__rev=1561259", PageUrl, nvc, "upload_id", "0");

						string UnPublishId = GlobusHttpHelper.getBetween(imgUploadResp, "\"fbid\":\"", "\"");

						NameValueCollection nvc1 = new NameValueCollection();
						nvc1.Add("composer_session_id", "57c32c98-d4b9-4f5a-9fb9-6452e01e24bb");
						nvc1.Add("fb_dtsg",fb_dtsg);
						nvc1.Add("xhpc_context","profile");
						nvc1.Add("xhpc_ismeta","1");
						nvc1.Add("xhpc_timeline",string.Empty);
						nvc1.Add("xhpc_composerid", ComposerId);
						nvc1.Add("xhpc_finch", "1");
						nvc1.Add("xhpc_targetid", pageId);
						nvc1.Add("xhpc_publish_type", "1");
						nvc1.Add("clp", "{\"cl_impid\":\"626b99df\",\"clearcounter\":0,\"elementid\":\"u_0_19\",\"version\":\"x\",\"parent_fbid\":"+pageId+"}");
						nvc1.Add("xhpc_message",message);
						nvc1.Add("composer_unpublished_photo[]",UnPublishId);
						nvc1.Add("album_type","128");
						nvc1.Add("is_file_form","1");
						nvc1.Add("oid", string.Empty);
						nvc1.Add("qn", waterfallId);
						nvc1.Add("application","composer");
						nvc1.Add("is_explicit_place",string.Empty);
						nvc1.Add("composertags_place",string.Empty);
						nvc1.Add("composertags_place_name", string.Empty);
						nvc1.Add("tagger_session_id", "1421254406");
						nvc1.Add("composertags_city",string.Empty);
						nvc1.Add("disable_location_sharing","false");
						nvc1.Add("composer_predicted_city",string.Empty);
						string imgUploadResp1 = HttpHelper.UploadImageWaterfallModel("https://upload.facebook.com/media/upload/photos/composer/?av=" + __user + "&__user=" + __user + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4pbGAdBGeqrWo8ponUKexm49UJ6K4Qmmey8szoyfw&__req=x&fb_dtsg="+fb_dtsg+"&ttstamp=2658170819911711579109529989&__rev=1561259", PageUrl, nvc1, "composer_predicted_city", string.Empty);

						string photoId=GlobusHttpHelper.getBetween(imgUploadResp1,"photo_fbid\":",",\"story_fbid");
						if (!string.IsNullOrEmpty(photoId))
						{
							AddToLogger_PageManager("Image Uploaded With Username : " + strUserName + " On The URL : " + PageUrl);


							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);

						    	AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								Thread.Sleep(delayInSeconds);
								//if (!string.IsNullOrEmpty(GlobusFileHelper.DesktopFanFilePath))
								{
									try
									{
										string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
										string CSV_Content = strUserName + "," + PageUrl + "," + pics;

									//	Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GlobusFileHelper.DesktopFanFilePath);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
						}
						else
						{
							AddToLogger_PageManager("Unable To Upload Image With Username : " + strUserName + " On The URL : " + PageUrl);

						}


					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}

				#endregion

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

	    	AddToLogger_PageManager(" Process Completed Image Upload With Username : " + strUserName + " On The URL : " + PageUrl);

		}



		public void StartImageUploading(ref FacebookUser fbUser, string url)
		{
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

			string strUserName = fbUser.username;
			string strProxyAddress = fbUser.proxyusername;
			string strProxyUserName = fbUser.proxyusername;
			string strProxyPassword = fbUser.proxypassword;
			string strProxyPort = fbUser.proxyport;            

			try
			{
				AddToLogger_PageManager(" Starting Image Upload With Username : " + strUserName + " On The URL : " + url);
			

				string pics = string.Empty;
				string message = string.Empty;
				string pageId = string.Empty;
				string session_id = string.Empty;

				if (string.IsNullOrEmpty(strProxyPort))
				{
					strProxyPort = "80";
				}

				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}

				string res_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);     //"https://www.facebook.com/"

				string __user = GlobusHttpHelper.GetParamValue(res_Home, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(res_Home, "user");
				}              

				string fb_dtsg =GlobusHttpHelper.Get_fb_dtsg(res_Home);

				string res_get_FanPage = HttpHelper.getHtmlfromUrl(new Uri(url),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				if (res_get_FanPage.Contains("?page_id="))
				{
					pageId = res_get_FanPage.Substring(res_get_FanPage.IndexOf("?page_id="), res_get_FanPage.IndexOf("\"", res_get_FanPage.IndexOf("?page_id=")) - res_get_FanPage.IndexOf("?page_id=")).Replace("?page_id=", string.Empty).Replace("\"", string.Empty).Trim();
				}

				res_get_FanPage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.PageManagerPagesGetting + pageId + ""),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);  


				string postURL_identity = FBGlobals.Instance.PageManagerPageIdentitySwitchPhp;                                    
				string postData_identity = "fb_dtsg=" + fb_dtsg + "&user_id=" + pageId + "&url=" + Uri.EscapeUriString(url) + "";

				res_get_FanPage = HttpHelper.postFormData(new Uri(postURL_identity), postData_identity);

				string composerid = GlobusHttpHelper.GetParamValue(res_get_FanPage, "composerid");
				if (string.IsNullOrEmpty(composerid))
				{
					composerid = GlobusHttpHelper.ParseJson(res_get_FanPage, "composerid");
				}

				string targetid = GlobusHttpHelper.GetParamValue(res_get_FanPage, "targetid");
				if (string.IsNullOrEmpty(targetid))
				{
					targetid = GlobusHttpHelper.ParseJson(res_get_FanPage, "targetid");
				}


				for (int i = 0; i < noOfPicsPerURL; i++)
				{
					try
					{

						if (picsCounter < lstPicturecollectionPostPicOnFanPageWall.Count)
						{
							pics = lstPicturecollectionPostPicOnFanPageWall[picsCounter];
							picsCounter++;
						}
						else
						{
							AddToLogger_PageManager("All pics used up : " + strUserName);
							AddToLogger_PageManager("All pics used up : " + strUserName);
							return;
						}


						if (lstFanPageCollectionFanPagePosterMessage.Count > 0)
						{
							message = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
						}

						///composer hash1
						string postURL_composerhash1 = FBGlobals.Instance.PageManagerPageAjaxComposerx1;    
						string postData_composerhash1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + composerid + "&targetid=" + targetid + "&istimeline=1&composercontext=composer&loaded_components[0]=maininput&loaded_components[1]=backdateicon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=placetaggericon&loaded_components[7]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n8ahyj35whVag&__req=4&phstamp=1658167108458977122472";

						string res_post_composerhash1 = HttpHelper.postFormData(new Uri(postURL_composerhash1), postData_composerhash1);


						///composer hash2
						///
						string postURL_composerhash2 = FBGlobals.Instance.PageManagerPageAjaxComposerx2;     
						string postData_composerhash2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + composerid + "&targetid=" + targetid + "&istimeline=1&composercontext=composer&loaded_components[0]=maininput&loaded_components[1]=backdateicon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=placetaggericon&loaded_components[7]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n88QoAMNo4uiA&__req=5&phstamp=1658167108458977122472";

						string res_post_composerhash2 = HttpHelper.postFormData(new Uri(postURL_composerhash2), postData_composerhash2);

						if (res_post_composerhash2.Contains("{\"session_id\":"))
						{
							try
							{
								session_id = res_post_composerhash2.Substring(res_post_composerhash2.IndexOf("{\"session_id\":"), res_post_composerhash2.IndexOf("}", res_post_composerhash2.IndexOf("{\"session_id\":")) - res_post_composerhash2.IndexOf("{\"session_id\":")).Replace("{\"session_id\":", string.Empty).Replace("\"", string.Empty).Trim();
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}

						}

						NameValueCollection nvc = new NameValueCollection();
						nvc.Add("xhpc_context", "profile");
						nvc.Add("fb_dtsg", fb_dtsg);
						//nvc.Add("id", userId);
						nvc.Add("xhpc_ismeta", "1");
						nvc.Add("xhpc_timeline", "1");
						nvc.Add("xhpc_composerid", composerid);
						nvc.Add("xhpc_targetid", targetid);
						nvc.Add("xhpc_message_text", message);
						nvc.Add("xhpc_message", message);
						//nvc.Add("xhpc_message_text", "profile");
						nvc.Add("type", "1");
						nvc.Add("oid", "");
						nvc.Add("application", "composer");
						nvc.Add("scheduled", "0");
						nvc.Add("backdated_date[year]", "");
						nvc.Add("backdated_date[day]", "");
						nvc.Add("backdated_date[hour]", "");
						nvc.Add("backdated_date[minute]", "");
						nvc.Add("is_explicit_place", "");
						nvc.Add("composertags_place", "");
						nvc.Add("composertags_place_name", "");
						nvc.Add("composer_session_id", session_id);
						nvc.Add("composertags_city", "");
						nvc.Add("disable_location_sharing", "false");
						nvc.Add("composer_predicted_city", "");
						nvc.Add("UITargetedPrivacyWidget", "80");
						//nvc.Add("return", "/ajax/timeline/profile_pic_upload.php?pic_type=1&id=" + userId);

					

						if (HttpHelper.HttpUploadFile(FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + __user + "&__a=1&__dyn=7n88QoAMNo4uiA&__req=7&fb_dtsg=" + fb_dtsg + "", pics, "pic", "image/jpeg", nvc,fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password))       //  "https://upload.facebook.com/media/upload/photos/composer/?__user="
						{
							AddToLogger_PageManager("Image Uploaded With Username : " + strUserName + " On The URL : " + url);
						
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);


								Thread.Sleep(delayInSeconds);
								//if ("")
								{
									try
									{
										string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
										string CSV_Content = strUserName + "," + url + "," + pics;

										Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content,ScrapersExprotFilePath );
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
									}
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
						}
						else
						{
							AddToLogger_PageManager("Couldn't Image Upload With Username : " + strUserName + " On The URL : " + url);

						}
					}
					catch (Exception ex)
					{

						AddToLogger_PageManager("Exception class>>  " + "ImageUploader  || Method>>  " + "StartImageUploading   || Exception: " + ex.Message + "    || DateTime: " + DateTime.Now.ToString());

						if (ex.Message.ToLower().Contains("timed"))
						{
							AddToLogger_PageManager("Operation timed out : " + strUserName);


							res_get_FanPage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.PageManagerPagesGetting + pageId + ""),"","");            //  "https://www.facebook.com/pages/getting_started?page_id="
							postURL_identity = FBGlobals.Instance.PageManagerPageIdentitySwitchPhp;                                                     // "https://www.facebook.com/identity_switch.php"
							postData_identity = "fb_dtsg=" + fb_dtsg + "&user_id=" + pageId + "&url=" + Uri.EscapeUriString(url) + "";
							res_get_FanPage = HttpHelper.postFormData(new Uri(postURL_identity), postData_identity);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			AddToLogger_PageManager(" Process Completed Image Upload With Username : " + strUserName + " On The URL : " + url);

		}




		public void PostOnFanPageWallWithURLAndItsImage(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string Username = string.Empty;
				string Password = string.Empty;

				//GlobusLogHelper.log.Debug("Please Wait------------!");
				//AddToLogger_PageManager("Please Wait------------!");

				Array paramArray = new object[10];            
				string Userss = fbUser.username;

				int CountPostWall = 1;
				//string FanpageUrl = (string)paramArray.GetValue(6);
				lstFanPageUrlCollectionFanPagePoster = lstFanPageUrlCollectionFanPagePoster.Distinct().ToList();
				lstFanPageURLs = lstFanPageUrlCollectionFanPagePoster;
				lstFanPageUrlCollectionFanPagePostUrl = lstFanPageUrlCollectionFanPagePostUrl.Distinct().ToList();
				lstFanPagePostURLs = lstFanPageUrlCollectionFanPagePostUrl;              

				string composer_session_id = "";
				string fb_dtsg = "";
				string xhpc_composerid = "";
				string xhpc_targetid = "";
				string xhpc_context = "";
				string xhpc_fbx = "";
				string xhpc_timeline = "";
				string xhpc_ismeta = "";
				string xhpc_message_text = "";    
				string xhpc_message = "";

				#region MyRegion
				string uithumbpager_width = "128";
				string uithumbpager_height = "128";
				string composertags_place = "";
				string composertags_place_name = "";
				string composer_predicted_city = "";
				string is_explicit_place = "";
				string composertags_city = "";
				string disable_location_sharing = "false";
				string audiencevalue = "80";
				string nctr_mod = "pagelet_timeline_recent";
				string UsreId = "";
				string __a = "1";
				string phstamp = ""; 
				#endregion

				Username = Userss;

				AddToLogger_PageManager("Start Process of Fanpage Posting With Username.... >>> " + Username);

				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}


				int Counter = 0;
				if (lstFanPageURLs.Count==0)				
				{
					AddToLogger_PageManager("Please Load Fan Page URLs ..");
					return;

				}
				foreach (var lstFanPageURLsitem in lstFanPageURLs)
				{

					while (true)
					{

						Counter = Counter + 1;
						try
						{
							//Counter = Counter + 1;
							if (Counter>noOfPicsPerURL)
							{
								Counter = 0;
								break;
							}

							string strFanPageURL = lstFanPageURLsitem;
							string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

							if (strPageSource.Contains("xhpc_composerid") && strPageSource.Contains("xhpc_targetid") && strPageSource.Contains("xhpc_context")) //&& strPageSource.Contains("xhpc_fbx")
							{

								UsreId = GlobusHttpHelper.GetParamValue(strPageSource, "user");   
								if (string.IsNullOrEmpty(UsreId))
								{
									UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
								}


								//UsreId = GlobusHttpHelper.Get_UserID(strPageSource);
								fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

								xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
								xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
								xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
								xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
								xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
								xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

								xhpc_message_text = lstFanPagePostURLs[new Random().Next(0, lstFanPagePostURLs.Count)];
								xhpc_message = xhpc_message_text;
								//lstFanPagePostURLs.Remove(xhpc_message);
								//xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
								//xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");

								try
								{
									if (lstFanPageUrlCollectionFanPagePostMessages.Count > 0)
									{
										xhpc_message_text = PageManager.lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageUrlCollectionFanPagePostMessages.Count)] + " " + xhpc_message_text;
										xhpc_message = xhpc_message_text;
									}

								}
								catch { };








								if (string.IsNullOrEmpty(UsreId))
								{
									UsreId = GlobusHttpHelper.ParseJson(strPageSource, "user");
								}

								string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.pageFanPageAjaxMetaComposerTargetidUrl + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);//Convert.ToInt32(ConvertToUnixTimestamp(DateTime.Now)).ToString();
								try
								{
								if (composer_session_idSource.Contains("composer_session_id"))
								{
									composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

								}
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}


								string strImageValue = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.pageFanPageUrlAjaxMetacomposerLinkUrl + Uri.EscapeDataString(xhpc_message_text) + "&alt_scrape_url=" + Uri.EscapeDataString(xhpc_message_text) + "&targetid=" + UsreId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&__a=1"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);   //https://www.facebook.com/ajax/metacomposer/attachment/link/scraper.php?scrape_url=http%253A%252F%252Fwww.google.co.in%252F&alt_scrape_url=http%253A%252F%252Fwww.google.co.in%252F&targetid=100003798185175&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=100003798185175&__a=1

								string imageURL = xhpc_message_text;
								try
								{
									imageURL = HttpHelper.GetHrefsFromString(xhpc_message_text)[0];
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
								string post_URL_GetImageParams = FaceDominator.FBGlobals.Instance.PageFanPageUrlComposeLinkScraper + imageURL + "&composerurihash=3";
								string post_Data_GetImageParams = "fb_dtsg=" + fb_dtsg + "&composerid=" + composer_session_id + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&loaded_components[4]=explicitplaceinput&loaded_components[5]=hiddenplaceinput&loaded_components[6]=placenameinput&loaded_components[7]=hiddensessionid&loaded_components[8]=withtagger&loaded_components[9]=placetagger&loaded_components[10]=withtaggericon&loaded_components[11]=citysharericon&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ahyj2tVBoeVag&__req=4n&phstamp=165816886688048122625";

								string res_post_GetImageParams = HttpHelper.postFormData(new Uri(post_URL_GetImageParams), post_Data_GetImageParams);
								strImageValue = res_post_GetImageParams;

								Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
								string attachment_params_summary = string.Empty;
								string attachment_params_images = string.Empty;
								if (strImageValue.Contains("name=") && strImageValue.Contains("value="))
								{
									try
									{
										string[] strNameValue = Regex.Split(strImageValue, "name=");
										foreach (var strNameValueitem in strNameValue)
										{
											try
											{
												if (strNameValueitem.Contains("value="))
												{
													string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
													if (strSplit.Contains("value="))
													{

														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														strName = strName.Replace(">u003Coption", "");
														if (strName == "fb_dtsg")
														{

															continue;
														}
														string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														if (strValue.Contains(">Year:u003C/option>"))
														{
															strValue = strValue.Replace("u003C", "<");
															if (strValue.Contains("</option>"))
															{
																strValue = GlobusHttpHelper.getBetween(strValue, "<option 2014>", "</option>");
															}
														}
														if (strValue.Contains("Month:u003C/option>"))
														{
															strValue = strValue.Replace("u003C", "<");
															if (strValue.Contains("</option>"))
															{
																strValue =GlobusHttpHelper.getBetween(strValue, "<option 12>", "</option>");
															}
														}
														strValue = (strValue);

														if (strNameValueitem.Contains("attachment[params][summary]"))
														{
															attachment_params_summary = strValue;
														}
														if (strNameValueitem.Contains("attachment[params][images]"))
														{
															attachment_params_images = strValue;
														}

														dicNameValue.Add(strName, strValue);
													}
													else
													{
														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														if (strName == "fb_dtsg")
														{
															continue;
														}
														string strValue = "0";
														strValue = (strValue);
														dicNameValue.Add(strName, strValue);
													}
												}
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.Message);
											}
										}


										string partPostData = string.Empty;
										foreach (var dicNameValueitem in dicNameValue)
										{
											partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
										}

										string strPostData = ("fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=" + xhpc_context + "&xhpc_fbx=" + xhpc_fbx + "&xhpc_timeline=" + xhpc_timeline + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&" + partPostData + "uithumbpager_width=320&uithumbpager_height=180&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=" + composer_session_id + "&is_explicit_place=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&phstamp=16581671021075776692083");//fb_dtsg=AQB6MSsa&xhpc_composerid=uv6h8i_132&xhpc_targetid=185980263874&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw%26feature%3Drelated&xhpc_message=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw%26feature%3Drelated&aktion=post&app_id=2309869772&UIThumbPager_Input=0&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][name]=title&attachment[params][metaTagMap][1][content]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][metaTagMap][2][name]=description&attachment[params][metaTagMap][2][content]=&attachment[params][metaTagMap][3][name]=keywords&attachment[params][metaTagMap][3][content]=Devon%2C%20ke%2C%20Dev%2C%20Mahadev%2C%2010th%2C%20July%2C%202012%2C%20Video%2C%20Watch%2C%20Online%2C%20Pt4&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][metaTagMap][5][property]=og%3Atitle&attachment[params][metaTagMap][5][content]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][metaTagMap][6][property]=og%3Adescription&attachment[params][metaTagMap][6][content]=&attachment[params][metaTagMap][7][property]=og%3Atype&attachment[params][metaTagMap][7][content]=video&attachment[params][metaTagMap][8][property]=og%3Aimage&attachment[params][metaTagMap][8][content]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fmqdefault.jpg&attachment[params][metaTagMap][9][property]=og%3Avideo&attachment[params][metaTagMap][9][content]=http%3A%2F%2Fwww.youtube.com%2Fv%2Fa1hRe_xGuGw%3Fversion%3D3%26autohide%3D1&attachment[params][metaTagMap][10][property]=og%3Avideo%3Atype&attachment[params][metaTagMap][10][content]=application%2Fx-shockwave-flash&attachment[params][metaTagMap][11][property]=og%3Avideo%3Awidth&attachment[params][metaTagMap][11][content]=480&attachment[params][metaTagMap][12][property]=og%3Avideo%3Aheight&attachment[params][metaTagMap][12][content]=360&attachment[params][metaTagMap][13][property]=og%3Asite_name&attachment[params][metaTagMap][13][content]=YouTube&attachment[params][metaTagMap][14][property]=fb%3Aapp_id&attachment[params][metaTagMap][14][content]=87741124305&attachment[params][metaTagMap][15][name]=twitter%3Acard&attachment[params][metaTagMap][15][value]=player&attachment[params][metaTagMap][16][name]=twitter%3Asite&attachment[params][metaTagMap][16][value]=%40youtube&attachment[params][metaTagMap][17][name]=twitter%3Aplayer&attachment[params][metaTagMap][17][value]=https%3A%2F%2Fwww.youtube.com%2Fembed%2Fa1hRe_xGuGw&attachment[params][metaTagMap][18][property]=twitter%3Aplayer%3Awidth&attachment[params][metaTagMap][18][content]=480&attachment[params][metaTagMap][19][property]=twitter%3Aplayer%3Aheight&attachment[params][metaTagMap][19][content]=360&attachment[params][metaTagMap][20][name]=attribution&attachment[params][metaTagMap][20][content]=youtube_none%2F&attachment[params][medium]=103&attachment[params][urlInfo][canonical]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][urlInfo][final]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw%26feature%3Drelated&attachment[params][favicon]=http%3A%2F%2Fs.ytimg.com%2Fyt%2Ffavicon-vfldLzJxy.ico&attachment[params][title]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][fragment_title]=&attachment[params][external_author]=&attachment[params][summary]=&attachment[params][url]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][video][0][type]=application%2Fx-shockwave-flash&attachment[params][video][0][src]=http%3A%2F%2Fwww.youtube.com%2Fv%2Fa1hRe_xGuGw%3Fversion%3D3%26autohide%3D1%26autoplay%3D1&attachment[params][video][0][width]=480&attachment[params][video][0][height]=360&attachment[params][video][0][v]=0&attachment[params][video][0][safe]=1&attachment[params][error]=1&attachment[params][og_info][properties][0][0]=og%3Aurl&attachment[params][og_info][properties][0][1]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][og_info][properties][1][0]=og%3Atitle&attachment[params][og_info][properties][1][1]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][og_info][properties][2][0]=og%3Adescription&attachment[params][og_info][properties][2][1]=&attachment[params][og_info][properties][3][0]=og%3Atype&attachment[params][og_info][properties][3][1]=video&attachment[params][og_info][properties][4][0]=og%3Aimage&attachment[params][og_info][properties][4][1]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fmqdefault.jpg&attachment[params][og_info][properties][5][0]=og%3Avideo&attachment[params][og_info][properties][5][1]=http%3A%2F%2Fwww.youtube.com%2Fv%2Fa1hRe_xGuGw%3Fversion%3D3%26autohide%3D1&attachment[params][og_info][properties][6][0]=og%3Avideo%3Atype&attachment[params][og_info][properties][6][1]=application%2Fx-shockwave-flash&attachment[params][og_info][properties][7][0]=og%3Avideo%3Awidth&attachment[params][og_info][properties][7][1]=480&attachment[params][og_info][properties][8][0]=og%3Avideo%3Aheight&attachment[params][og_info][properties][8][1]=360&attachment[params][og_info][properties][9][0]=og%3Asite_name&attachment[params][og_info][properties][9][1]=YouTube&attachment[params][og_info][properties][10][0]=fb%3Aapp_id&attachment[params][og_info][properties][10][1]=87741124305&attachment[params][og_info][properties][11][0]=twitter%3Aplayer%3Awidth&attachment[params][og_info][properties][11][1]=480&attachment[params][og_info][properties][12][0]=twitter%3Aplayer%3Aheight&attachment[params][og_info][properties][12][1]=360&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fmqdefault.jpg&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&attachment[params][responseCode]=200&attachment[params][redirectPath][0][status]=og%3Aurl&attachment[params][redirectPath][0][url]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][redirectPath][0][ip]=74.125.228.3&attachment[params][metaTags][title]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][metaTags][keywords]=Devon%2C%20ke%2C%20Dev%2C%20Mahadev%2C%2010th%2C%20July%2C%202012%2C%20Video%2C%20Watch%2C%20Online%2C%20Pt4&attachment[params][metaTags][attribution]=youtube_none%2F&attachment[params][locale]=en&attachment[params][lang]=en&attachment[params][links][0][rel]=search&attachment[params][links][0][type]=application%2Fopensearchdescription%2Bxml&attachment[params][links][0][href]=http%3A%2F%2Fwww.youtube.com%2Fopensearch%3Flocale%3Den_US&attachment[params][links][0][title]=YouTube%20Video%20Search&attachment[params][links][1][rel]=icon&attachment[params][links][1][href]=http%3A%2F%2Fs.ytimg.com%2Fyt%2Ffavicon-vfldLzJxy.ico&attachment[params][links][1][type]=image%2Fx-icon&attachment[params][links][2][rel]=shortcut%20icon&attachment[params][links][2][href]=http%3A%2F%2Fs.ytimg.com%2Fyt%2Ffavicon-vfldLzJxy.ico&attachment[params][links][2][type]=image%2Fx-icon&attachment[params][links][3][rel]=canonical&attachment[params][links][3][href]=%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][links][4][rel]=alternate&attachment[params][links][4][media]=handheld&attachment[params][links][4][href]=http%3A%2F%2Fm.youtube.com%2Fwatch%3Fdesktop_uri%3D%252Fwatch%253Fv%253Da1hRe_xGuGw%26v%3Da1hRe_xGuGw%26gl%3DUS&attachment[params][links][5][rel]=shortlink&attachment[params][links][5][href]=http%3A%2F%2Fyoutu.be%2Fa1hRe_xGuGw&attachment[params][links][6][rel]=alternate&attachment[params][links][6][type]=application%2Fjson%2Boembed&attachment[params][links][6][href]=http%3A%2F%2Fwww.youtube.com%2Foembed%3Furl%3Dhttp%253A%252F%252Fwww.youtube.com%252Fwatch%253Fv%253Da1hRe_xGuGw%26format%3Djson&attachment[params][links][6][title]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][links][7][rel]=alternate&attachment[params][links][7][type]=text%2Fxml%2Boembed&attachment[params][links][7][href]=http%3A%2F%2Fwww.youtube.com%2Foembed%3Furl%3Dhttp%253A%252F%252Fwww.youtube.com%252Fwatch%253Fv%253Da1hRe_xGuGw%26format%3Dxml&attachment[params][links][7][title]=Devon%20ke%20Dev%20Mahadev%2010th%20July%202012%20Video%20Watch%20Online%20Pt4&attachment[params][links][8][id]=www-core-css&attachment[params][links][8][rel]=stylesheet&attachment[params][links][8][href]=http%3A%2F%2Fs.ytimg.com%2Fyt%2Fcssbin%2Fwww-core-vflMJW9Qx.css&attachment[params][links][9][itemprop]=url&attachment[params][links][9][href]=http%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3Da1hRe_xGuGw&attachment[params][links][10][itemprop]=url&attachment[params][links][10][href]=http%3A%2F%2Fwww.youtube.com%2Fchannel%2FUCpEMCp-RW4JB0RkTROQDWrg&attachment[params][links][11][itemprop]=url&attachment[params][links][11][href]=https%3A%2F%2Fplus.google.com%2F101628430301028857728&attachment[params][links][12][itemprop]=thumbnailUrl&attachment[params][links][12][href]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fhqdefault.jpg&attachment[params][links][13][itemprop]=url&attachment[params][links][13][href]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fmqdefault.jpg&attachment[params][links][14][itemprop]=embedURL&attachment[params][links][14][href]=http%3A%2F%2Fwww.youtube.com%2Fv%2Fa1hRe_xGuGw%3Fversion%3D3%26autohide%3D1&attachment[params][images][0]=http%3A%2F%2Fi2.ytimg.com%2Fvi%2Fa1hRe_xGuGw%2Fmqdefault.jpg&attachment[params][cache_hit]=1&attachment[type]=100&uithumbpager_width=320&uithumbpager_height=180&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=1342159698&is_explicit_place=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=100003798185175&__a=1&phstamp=16581665477831159710600
										string strResponse = string.Empty;
										try
										{
											strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxUpdateStatusUrl), strPostData);

											if (strResponse.Contains("errorSummary"))
											{
												try
												{
													string attachment_params_urlInfo_final = string.Empty;
													string attachment_params_favicon = string.Empty;
													string link_metrics_base_domain = string.Empty;

													attachment_params_favicon = GlobusHttpHelper.getBetween(res_post_GetImageParams, "attachment[params][favicon]", "\\/>").Replace(" value=", "").Replace("\\\"", "").Replace("\\", "");

													attachment_params_urlInfo_final = GlobusHttpHelper.getBetween(res_post_GetImageParams, "attachment[params][urlInfo][final]", "\\/>").Replace(" value=", "").Replace("\\\"", "").Replace("\\", "");

													link_metrics_base_domain = GlobusHttpHelper.getBetween(res_post_GetImageParams, "link_metrics[base_domain]", "\\/>").Replace(" value=", "").Replace("\\\"", "").Replace("\\", "");

													string PostDataa = "composer_session_id=f461bebd-0d21-4555-a46f-81020df04023&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&clp=%7B%22cl_impid%22%3A%22bd5578f5%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_39%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A693901727326724%7D&xhpc_message_text=" + xhpc_message_text + "%20&xhpc_message=" + xhpc_message + "%20&aktion=post&app_id=2309869772&attachment[params][urlInfo][canonical]=" + attachment_params_urlInfo_final + "&attachment[params][urlInfo][final]=" + attachment_params_urlInfo_final + "&attachment[params][urlInfo][user]=" + attachment_params_urlInfo_final + "&attachment[params][favicon]=" + attachment_params_favicon + "&attachment[params][title]=" + link_metrics_base_domain + "&attachment[params][summary]=" + attachment_params_summary + "&attachment[params][images][0]=" + attachment_params_images + "&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.google.com%2F&attachment[type]=100&link_metrics[source]=ShareStageExternal&link_metrics[domain]=www.google.com&link_metrics[base_domain]=google.com&link_metrics[title_len]=6&link_metrics[summary_len]=159&link_metrics[min_dimensions][0]=70&link_metrics[min_dimensions][1]=70&link_metrics[images_with_dimensions]=2&link_metrics[images_pending]=0&link_metrics[images_fetched]=0&link_metrics[image_dimensions][0]=269&link_metrics[image_dimensions][1]=95&link_metrics[images_selected]=2&link_metrics[images_considered]=2&link_metrics[images_cap]=3&link_metrics[images_type]=ranked&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=2&composer_metrics[images_loaded]=2&composer_metrics[images_shown]=2&composer_metrics[load_duration]=2&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&scheduled=0&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&future_dateIntlDisplay=28%2F3%2F2014&future_date=3%2F28%2F2014&future_time=&future_time_display_time=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1395899372&composertags_city=&disable_location_sharing=false&composer_predicted_city=&UITargetedPrivacyWidget=80&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEAMCBynxl2u6aEyx9CxSq78hAKGgyiGGfJ4WpUpBw&__req=8&ttstamp=2658166727510250104&__rev=1179995";

													strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxUpdateStatusUrl), PostDataa);    // "https://www.facebook.com/ajax/updatestatus.php"
												}
												catch (Exception ex)
												{
													Console.WriteLine(ex.Message);
												}

											}
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.Message);
										}

										try
										{
											if (string.IsNullOrEmpty(strResponse))
											{
												try
												{
													strResponse = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.PageFanPageUrlComposerPhpUrl), strPostData);
												}
												catch (Exception ex)
												{
													Console.WriteLine(ex.Message);
												}
											}

										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.Message);
										}

										if (strResponse.Contains("\"errorSummary\":"))
										{
											try
											{
												string summary = GlobusHttpHelper.ParseJson(strResponse, "errorSummary");
												string errorDescription = GlobusHttpHelper.ParseJson(strResponse, "errorDescription");

												AddToLogger_PageManager("Fan Page Posting Error: " + summary + " | Error Description: " + errorDescription);

												if (summary.Contains("Please verify your account"))
												{
													return;
												}
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.Message);
											}
										}
										int delayInSeconds=0;
										if (!strResponse.Contains("error"))
										{
											//string ok = "ok";
											// TotalFanPageWallPoster_Counter++;
											AddToLogger_PageManager(CountPostWall + " fanpage Posted With Image URL : " + xhpc_message_text + " On The Fan Page URL : " + strFanPageURL + " With User Name : " + Userss);
											CountPostWall++;

											// Write Data in CSV File  
											//  CSVUtilities.ExportDataCSVFile(CSVHeader, CSV_Content, _ExportLocation);                                    

											try
											{
												string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
												string CSV_Content = Username + "," + lstFanPageURLsitem + "," + xhpc_message_text;

												Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ScrapersExprotFilePath);
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.Message);
											}
											                                                 
											delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
											AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
										}
										else
										{
											delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
											AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
											AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);


										}
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.Message);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}

			AddToLogger_PageManager("Process Completed Of fanpage Posting With Username >>> " + fbUser.username);


		}

		//  PostFanPageMessageUsingUrls

		public List<string> lstPicturecollectionPostPicOnFanPageWall = new List<string>();
		public static List<string> lstFanPageCollectionFanPagePosterMessage = new List<string>();
		static int picsCounter = 0;

		public bool isStopFanPageScraper = false;
		int noOfURLThreads = 10;
		int noOfScraperThreads = 20;
		int noOfDequeParseThreads = 4 * 5;
		int countDequeParseThreads = 0;
		static int count = 0;



		public void PostFanPageMessageUsingUrlsOld_Old(ref FacebookUser fbUser)
		{
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{

				#region Post variable

				string post_form_id = string.Empty;
				string fbpage_id = string.Empty;
				string fb_dtsg = string.Empty;
				string __user = string.Empty;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				string xhpc_composerid12 = string.Empty;

				#endregion

				int NoOfEmailAccount = 20;              
				Array paramArray = new object[8];              
				string  Username = fbUser.username;
				string  Password = fbUser.password;
				string proxyAddress =fbUser.proxyusername;
				string proxyPort =fbUser.proxyport;
				string proxyUserName =fbUser.proxyusername;
				string proxyPassword =fbUser.proxypassword;
				string strPostFanpageMessageCount = string.Empty; 
				string Message =string.Empty;
				string campaignName = string.Empty;    
				string ResponseFanPagePostMessage = string.Empty;

				int intProxyPort = 80;
				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}

				foreach (var FanPageUrl in lstFanPageUrlCollectionFanPagePoster)
				{
					count = 0;

					while (true)
					{
						try
						{
							message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{

							string PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							if (count >=noOfPicsPerURL)
							{
							     break;
							}

							AddToLogger_PageManager("Posting Message : " + message_text + " With UserName : " + Username);
							//AddToLogger_PageManager("Fan Page Link : " + FanPageUrl);
							if (PageSrcFanPageUrl.Contains("fb_dtsg"))
							{
								string strfb_dtsg = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "fb_dtsg");
								if (string.IsNullOrEmpty(strfb_dtsg))
								{
									strfb_dtsg = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "fb_dtsg");
								}
							}

							string strFanPageURL = FanPageUrl;

							//__user = GlobusHttpHelper.Get_UserID(PageSrcFanPageUrl);

							__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");
							if (string.IsNullOrEmpty(__user))
							{
								__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
							}       





							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcFanPageUrl);

							fbpage_id = GlobusHttpHelper.GetPageID(PageSrcFanPageUrl, ref strFanPageURL);

							string postURL_1st = FaceDominator.FBGlobals.Instance.pageFanPagePosterAjaxPagesFanStatusPhp;   
							string postData_1st = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&nctr[_mod]=pagelet_timeline_page_actions&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
							string res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);

							if (res_post_1st.Contains("Security Check Required"))
							{
								string content = Username + ":" + Password;
							//	AddToLogger_PageManager("Security Check Required : " + FanPageUrl + "  with : " + Username);

								continue;
							}
							else if (res_post_1st.Contains("You already like this Page"))
							{
								#region commentedCode
								//string content = Username + ":" + Password;
								//CreateFileLikeDeskTop(content, "UnLike");
								//GlobusLogHelper.log.Debug("Already Liked " + FanPageUrl + "  with " + Username);
								//return; 
								#endregion

								//AddToLogger_PageManager("You have already liked this Page : " + FanPageUrl + "  with : " + Username);

							}
							else if (res_post_1st.Contains("\"errorSummary\""))
							{
								try
								{
									string summary = GlobusHttpHelper.ParseJson(res_post_1st, "errorSummary");
									string errorDescription = GlobusHttpHelper.ParseJson(res_post_1st, "errorDescription");

									//AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
									// AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}
							else
							{
								//AddToLogger_PageManager("Liked " + FanPageUrl + "  with " + Username);

							}

							try
							{
								string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
								string CSV_Content = Username + "," + FanPageUrl + "," + message_text;

								Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, "");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}


							string postURL_2nd = FaceDominator.FBGlobals.Instance.pageFanPageAjaxPagesFetchColumnPhp;
							string postData_2nd = "profile_id=" + fbpage_id + "&tab_key=timeline&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
							string res_post_2nd = HttpHelper.postFormData(new Uri(postURL_2nd), postData_2nd);

							string PageSrcFanPageUrlPost = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl + "?sk=wall"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							message_text = message_text.Replace(" ", "%20");

							if (PageSrcFanPageUrlPost.Contains("xhpc_composerid"))
							{
								try
								{
									xhpc_composerid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_composerid"), 200);
									string[] Arr = xhpc_composerid.Split('"');
									xhpc_composerid = Arr[2];
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
								}
							}

							if (PageSrcFanPageUrlPost.Contains("xhpc_targetid"))
							{
								string strxhpc_targetid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_targetid") + 9, 100);
								string[] Arrxhpc_targetid = strxhpc_targetid.Split('"');
								xhpc_targetid = Arrxhpc_targetid[2];
								xhpc_targetid = xhpc_targetid.Replace("\\", "");
								//count++;
								AddToLogger_PageManager(" Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);
								//GlobusLogHelper.log.Info(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);

							}

							///Post Message

							try
							{
								message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							ResponseFanPagePostMessage = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.pageFanPagePosterUpdateStatusPhp), "&fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=&xhpc_message_text=" + message_text + "&xhpc_message=" + message_text + "&nctr[_mod]=pagelet_wall&lsd&post_form_id_source=AsyncRequest&__user=" + __user);
							if (ResponseFanPagePostMessage.Contains("Couldn't Update Status")&&ResponseFanPagePostMessage.Contains("There was a problem updating your status. Please try again in a few minutes"))
							{
								count++;
								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);

								continue;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{
								count++;
								AddToLogger_PageManager(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);



								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
							else if (ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{

								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);



								int delayInSeconds =GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			//  Thread.Sleep(1000);
		}


		public void PostFanPageMessageUsingUrls__Old101(ref FacebookUser fbUser)
		{
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;  //objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
			try
			{

				#region Post variable

				string post_form_id = string.Empty;
				string fbpage_id = string.Empty;
				string fb_dtsg = string.Empty;
				string __user = string.Empty;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				string xhpc_composerid12 = string.Empty;

				#endregion

				int NoOfEmailAccount = 20;              
				Array paramArray = new object[8];              
				string  Username = fbUser.username;
				string  Password = fbUser.password;
				string proxyAddress =fbUser.proxyusername;
				string proxyPort =fbUser.proxyport;
				string proxyUserName =fbUser.proxyusername;
				string proxyPassword =fbUser.proxypassword;
				string strPostFanpageMessageCount = string.Empty; 
				string Message =string.Empty;
				string campaignName = string.Empty;    
				string ResponseFanPagePostMessage = string.Empty;

				int intProxyPort = 80;
				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}

				foreach (var FanPageUrl in lstFanPageUrlCollectionFanPagePoster)
				{
					count = 0;

					while (true)
					{
						try
						{
							message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
						try
						{

							string PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							if (count >=noOfPicsPerURL)
							{
								break;
							}

							AddToLogger_PageManager("Posting Message : " + message_text + " With UserName : " + Username);
							//AddToLogger_PageManager("Fan Page Link : " + FanPageUrl);
							if (PageSrcFanPageUrl.Contains("fb_dtsg"))
							{
								string strfb_dtsg = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "fb_dtsg");
								if (string.IsNullOrEmpty(strfb_dtsg))
								{
									strfb_dtsg = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "fb_dtsg");
								}
							}

							string strFanPageURL = FanPageUrl;

							//__user = GlobusHttpHelper.Get_UserID(PageSrcFanPageUrl);

							__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");
							if (string.IsNullOrEmpty(__user))
							{
								__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
							}   

							if(__user== "0")
							{

								int counthere = 0;
								while(true)
								{
									AddToLogger_PageManager("Please wait Logging in Again Delay For 20 Sec");


									Thread.Sleep(20*000);
									counthere++;
									AccountManager  objAccountManager = new AccountManager();
									objAccountManager.LoginUsingGlobusHttp(ref fbUser);

									PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

									if (PageSrcFanPageUrl.Contains("fb_dtsg"))
									{
										string strfb_dtsg = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "fb_dtsg");
										if (string.IsNullOrEmpty(strfb_dtsg))
										{
											strfb_dtsg = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "fb_dtsg");
										}
									}

									__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");
									if (string.IsNullOrEmpty(__user))
									{
										__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
									} 

									if(__user!="0" || counthere > 10)
									{
										break;
									}
									else if(counthere>10)
									{
										AddToLogger_PageManager("Could not login this account , process complted");
										return;
									}
								}

							}





							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcFanPageUrl);

							fbpage_id = GlobusHttpHelper.GetPageID(PageSrcFanPageUrl, ref strFanPageURL);

							string postURL_1st = FaceDominator.FBGlobals.Instance.pageFanPagePosterAjaxPagesFanStatusPhp;   
							string postData_1st = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&nctr[_mod]=pagelet_timeline_page_actions&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
							string res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);

							if(res_post_1st.Contains("Not logged in"))
							{
								Thread.Sleep(20*000);
								AccountManager  objAccountManager = new AccountManager();
								objAccountManager.LoginUsingGlobusHttp(ref fbUser);
								res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);
								//HttpHelper
							}





							if (res_post_1st.Contains("Security Check Required"))
							{
								string content = Username + ":" + Password;
								//	AddToLogger_PageManager("Security Check Required : " + FanPageUrl + "  with : " + Username);

								continue;
							}
							else if (res_post_1st.Contains("You already like this Page"))
							{
								#region commentedCode
								//string content = Username + ":" + Password;
								//CreateFileLikeDeskTop(content, "UnLike");
								//GlobusLogHelper.log.Debug("Already Liked " + FanPageUrl + "  with " + Username);
								//return; 
								#endregion

								//AddToLogger_PageManager("You have already liked this Page : " + FanPageUrl + "  with : " + Username);

							}
							else if (res_post_1st.Contains("\"errorSummary\""))
							{
								try
								{
									string summary = GlobusHttpHelper.ParseJson(res_post_1st, "errorSummary");
									string errorDescription = GlobusHttpHelper.ParseJson(res_post_1st, "errorDescription");

									//AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
									// AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							else
							{
								//AddToLogger_PageManager("Liked " + FanPageUrl + "  with " + Username);

							}

							try
							{
								string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
								string CSV_Content = Username + "," + FanPageUrl + "," + message_text;

								Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, "");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							string postURL_2nd = "";
							string postData_2nd  = "";
							string res_post_2nd = "";
							try
							{
								postURL_2nd = FaceDominator.FBGlobals.Instance.pageFanPageAjaxPagesFetchColumnPhp;
								postData_2nd = "profile_id=" + fbpage_id + "&tab_key=timeline&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
								res_post_2nd = HttpHelper.postFormData(new Uri(postURL_2nd), postData_2nd);
							}
							catch{};

							string PageSrcFanPageUrlPost = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl + "?sk=wall"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							message_text = message_text.Replace(" ", "%20");

							if (PageSrcFanPageUrlPost.Contains("xhpc_composerid"))
							{
								try
								{
									xhpc_composerid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_composerid"), 200);
									string[] Arr = xhpc_composerid.Split('"');
									xhpc_composerid = Arr[2];
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}

							if (PageSrcFanPageUrlPost.Contains("xhpc_targetid"))
							{
								string strxhpc_targetid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_targetid") + 9, 100);
								string[] Arrxhpc_targetid = strxhpc_targetid.Split('"');
								xhpc_targetid = Arrxhpc_targetid[2];
								xhpc_targetid = xhpc_targetid.Replace("\\", "");
								//count++;
								AddToLogger_PageManager(" Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);
								//GlobusLogHelper.log.Info(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);


							}

							///Post Message

							try
							{
								message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							ResponseFanPagePostMessage = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.pageFanPagePosterUpdateStatusPhp), "&fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=&xhpc_message_text=" + message_text + "&xhpc_message=" + message_text + "&nctr[_mod]=pagelet_wall&lsd&post_form_id_source=AsyncRequest&__user=" + __user);
							if (ResponseFanPagePostMessage.Contains("Couldn't Update Status")&&ResponseFanPagePostMessage.Contains("There was a problem updating your status. Please try again in a few minutes"))
							{
								count++;
								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);

								continue;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{
								count++;
								AddToLogger_PageManager(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);



								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
							else if (ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{

								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);



								int delayInSeconds =GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}

			//  Thread.Sleep(1000);
		}




		public void PostFanPageMessageUsingUrls(ref FacebookUser fbUser)
		{
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;  //objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
			try
			{

				#region Post variable

				string post_form_id = string.Empty;
				string fbpage_id = string.Empty;
				string fb_dtsg = string.Empty;
				string __user = string.Empty;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				string xhpc_composerid12 = string.Empty;

				#endregion

				int NoOfEmailAccount = 20;              
				Array paramArray = new object[8];              
				string  Username = fbUser.username;
				string  Password = fbUser.password;
				string proxyAddress =fbUser.proxyusername;
				string proxyPort =fbUser.proxyport;
				string proxyUserName =fbUser.proxyusername;
				string proxyPassword =fbUser.proxypassword;
				string strPostFanpageMessageCount = string.Empty; 
				string Message =string.Empty;
				string campaignName = string.Empty;    
				string ResponseFanPagePostMessage = string.Empty;

				int intProxyPort = 80;
				if (!string.IsNullOrEmpty(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}

				foreach (var FanPageUrl in lstFanPageUrlCollectionFanPagePoster)
				{
					count = 0;

					while (true)
					{
						try
						{
							message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
						try
						{

							string PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							if (count >=noOfPicsPerURL)
							{
								break;
							}

							AddToLogger_PageManager("Posting Message : " + message_text + " With UserName : " + Username);
							//AddToLogger_PageManager("Fan Page Link : " + FanPageUrl);
							if (PageSrcFanPageUrl.Contains("fb_dtsg"))
							{
								string strfb_dtsg = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "fb_dtsg");
								if (string.IsNullOrEmpty(strfb_dtsg))
								{
									strfb_dtsg = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "fb_dtsg");
								}
							}

							string strFanPageURL = FanPageUrl;

							//__user = GlobusHttpHelper.Get_UserID(PageSrcFanPageUrl);

							__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");
							if (string.IsNullOrEmpty(__user))
							{
								__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
							}   

							if(__user== "0")
							{

								int counthere = 0;
								while(true)
								{
									AddToLogger_PageManager("Please wait Logging in Again Delay For 20 Sec");


									Thread.Sleep(20*000);
									counthere++;
									AccountManager  objAccountManager = new AccountManager();
									objAccountManager.LoginUsingGlobusHttp(ref fbUser);

									PageSrcFanPageUrl = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

									if (PageSrcFanPageUrl.Contains("fb_dtsg"))
									{
										string strfb_dtsg = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "fb_dtsg");
										if (string.IsNullOrEmpty(strfb_dtsg))
										{
											strfb_dtsg = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "fb_dtsg");
										}
									}

									__user = GlobusHttpHelper.GetParamValue(PageSrcFanPageUrl, "user");
									if (string.IsNullOrEmpty(__user))
									{
										__user = GlobusHttpHelper.ParseJson(PageSrcFanPageUrl, "user");
									} 

									if(__user!="0" || counthere > 10)
									{
										break;
									}
									else if(counthere>10)
									{
										AddToLogger_PageManager("Could not login this account , process complted");
										return;
									}
								}

							}





							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(PageSrcFanPageUrl);

							fbpage_id = GlobusHttpHelper.GetPageID(PageSrcFanPageUrl, ref strFanPageURL);

							string postURL_1st = FaceDominator.FBGlobals.Instance.pageFanPagePosterAjaxPagesFanStatusPhp;   
							string postData_1st = "fbpage_id=" + fbpage_id + "&add=true&reload=false&fan_origin=page_timeline&nctr[_mod]=pagelet_timeline_page_actions&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
							string res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);

							if(res_post_1st.Contains("Not logged in"))
							{
								Thread.Sleep(20*000);
								AccountManager  objAccountManager = new AccountManager();
								objAccountManager.LoginUsingGlobusHttp(ref fbUser);
								res_post_1st = HttpHelper.postFormData(new Uri(postURL_1st), postData_1st);
								//HttpHelper
							}





							if (res_post_1st.Contains("Security Check Required"))
							{
								string content = Username + ":" + Password;
								//	AddToLogger_PageManager("Security Check Required : " + FanPageUrl + "  with : " + Username);

								continue;
							}
							else if (res_post_1st.Contains("You already like this Page"))
							{
								#region commentedCode
								//string content = Username + ":" + Password;
								//CreateFileLikeDeskTop(content, "UnLike");
								//GlobusLogHelper.log.Debug("Already Liked " + FanPageUrl + "  with " + Username);
								//return; 
								#endregion

								//AddToLogger_PageManager("You have already liked this Page : " + FanPageUrl + "  with : " + Username);

							}
							else if (res_post_1st.Contains("\"errorSummary\""))
							{
								try
								{
									string summary = GlobusHttpHelper.ParseJson(res_post_1st, "errorSummary");
									string errorDescription = GlobusHttpHelper.ParseJson(res_post_1st, "errorDescription");

									//AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
									// AddToLogger_PageManager("Liking Error: " + summary + " | Error Description: " + errorDescription);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							else
							{
								//AddToLogger_PageManager("Liked " + FanPageUrl + "  with " + Username);

							}

							try
							{
								string CSVHeader = "UserName" + "," + "FanpageUrl" + "," + "Message";
								string CSV_Content = Username + "," + FanPageUrl + "," + message_text;

								Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, "");
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							string postURL_2nd = "";
							string postData_2nd  = "";
							string res_post_2nd = "";
							try
							{
								postURL_2nd = FaceDominator.FBGlobals.Instance.pageFanPageAjaxPagesFetchColumnPhp;
								postData_2nd = "profile_id=" + fbpage_id + "&tab_key=timeline&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=" +GlobusHttpHelper.GenerateTimeStamp() + "";
								res_post_2nd = HttpHelper.postFormData(new Uri(postURL_2nd), postData_2nd);
							}
							catch{};

							string PageSrcFanPageUrlPost = HttpHelper.getHtmlfromUrl(new Uri(FanPageUrl + "?sk=wall"),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							message_text = message_text.Replace(" ", "%20");

							if (PageSrcFanPageUrlPost.Contains("xhpc_composerid"))
							{
								try
								{
									xhpc_composerid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_composerid"), 200);
									string[] Arr = xhpc_composerid.Split('"');
									xhpc_composerid = Arr[2];
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
									xhpc_composerid = xhpc_composerid.Replace("\\", "");
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.Message);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}

							if (PageSrcFanPageUrlPost.Contains("xhpc_targetid"))
							{
								string strxhpc_targetid = PageSrcFanPageUrlPost.Substring(PageSrcFanPageUrlPost.IndexOf("xhpc_targetid") + 9, 100);
								string[] Arrxhpc_targetid = strxhpc_targetid.Split('"');
								xhpc_targetid = Arrxhpc_targetid[2];
								xhpc_targetid = xhpc_targetid.Replace("\\", "");
								//count++;
								AddToLogger_PageManager(" Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);
								//GlobusLogHelper.log.Info(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);


							}


							string appid = "";
							try
							{
								appid = FBUtils.getBetween(PageSrcFanPageUrlPost, "appid=", "&");
							}
							catch { };




							///Post Message

							try
							{
								message_text = lstFanPageCollectionFanPagePosterMessage[new Random().Next(0, lstFanPageCollectionFanPagePosterMessage.Count)];
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}



							if (message_text.Contains("https://") || message_text.Contains("http://"))
							{
								if (message_text.Contains("https://"))
								{
									//message_text = message_text.Replace("https://", "");

								}
								else if (message_text.Contains("http://"))
								{
									// message_text = message_text.Replace("http://", "");

								}

							}

							string[] messagesList = { };

							try
							{
								messagesList = Regex.Split(message_text, ":");

							}
							catch { };


							if (true)
							{



								message_text = Uri.EscapeUriString(message_text);
								string xhpc_message_text = "";
								string FirstResponse = "";
								try
								{
									string PostDataUrl = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + __user + "&composerurihash=1";
									string PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
									FirstResponse = HttpHelper.postFormData(new Uri(PostDataUrl), PostData);

									if (FirstResponse.Contains("Who are you with?"))
									{
										string Post_Url = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + __user + "&composerurihash=1";
										string PostData_Url = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=backdateicon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=backdateicon&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqA8ABGeqrWo8pojByUWdDx2ubhHximmey8qUS8zU&__req=e&ttstamp=26581729512056122661171216683&__rev=1503785";
										FirstResponse = HttpHelper.postFormData(new Uri(Post_Url), PostData_Url);
										if (!FirstResponse.Contains("Who are you with?"))
										{
											string Post_Url2 = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + __user + "&composerurihash=1";
											string PostData_Url2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&photoswaterfallid=29f9db5dfb9b52c5a4a760ee4510ea07&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=ogtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=withtaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=backdateicon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=aJioznEyl2qm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1e&ttstamp=2658170679789798165112110106695577&__rev=1612042";
											FirstResponse = HttpHelper.postFormData(new Uri(Post_Url2), PostData_Url2);
										}
									}
								}
								catch (Exception ex)
								{
									// GlobusLogHelper.log.Error(ex.StackTrace);
								}



								string SecondResponse = "";

								bool chkWallWallPosterRemoveURLsMessages = false;

								if (FirstResponse.Contains("Sorry, we got confused"))
								{
									try
									{
										FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + __user + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + __user + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
									}
									catch (Exception ex)
									{
										// GlobusLogHelper.log.Error(ex.StackTrace);
									}
								}




								try
								{
									SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(message_text) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + __user + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
								}
								catch (Exception ex)
								{
									// GlobusLogHelper.log.Error(ex.StackTrace);
								}
								string tagger_session_id = FBUtils.getBetween(FirstResponse, "tagger_session_id\\\" value=\\\"", "\\\"");
								string composer_predicted_city = FBUtils.getBetween(FirstResponse, "composer_predicted_city\\\" value=\\\"", "\\\"");
								string attachment_params = FBUtils.getBetween(SecondResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
								string attachment_params_urlInfo_canonical = FBUtils.getBetween(SecondResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_urlInfo_final = FBUtils.getBetween(SecondResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_urlInfo_user = FBUtils.getBetween(SecondResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_favicon = FBUtils.getBetween(SecondResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");

								string attachment_params_title = FBUtils.getBetween(SecondResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");
								attachment_params_title = HttpUtility.HtmlDecode(attachment_params_title);

								string attachment_params_summary = FBUtils.getBetween(SecondResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

								attachment_params_summary = HttpUtility.HtmlDecode(attachment_params_summary);

								string attachment_params_images0 = FBUtils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_ranked_images_images_1 = FBUtils.getBetween(SecondResponse, "attachment[params][ranked_images][images][1]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#", string.Empty);
								string attachment_params_ranked_images_images_2 = FBUtils.getBetween(SecondResponse, "attachment[params][ranked_images][images][2]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#", string.Empty);
								string attachment_params_ranked_images_images_3 = FBUtils.getBetween(SecondResponse, "attachment[params][ranked_images][images][3]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#", string.Empty);
								string attachment_params_medium = FBUtils.getBetween(SecondResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_url = FBUtils.getBetween(SecondResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_type = FBUtils.getBetween(SecondResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_src = FBUtils.getBetween(SecondResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_width = FBUtils.getBetween(SecondResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_height = FBUtils.getBetween(SecondResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_secure_url = FBUtils.getBetween(SecondResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_type = FBUtils.getBetween(SecondResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_source = FBUtils.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_domain = FBUtils.getBetween(SecondResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_base_domain = FBUtils.getBetween(SecondResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_title_len = FBUtils.getBetween(SecondResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_summary_len = FBUtils.getBetween(SecondResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_min_dimensions0 = FBUtils.getBetween(SecondResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_min_dimensions1 = FBUtils.getBetween(SecondResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_with_dimensions = FBUtils.getBetween(SecondResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_pending = FBUtils.getBetween(SecondResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_fetched = FBUtils.getBetween(SecondResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_image_dimensions0 = FBUtils.getBetween(SecondResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_image_dimensions1 = FBUtils.getBetween(SecondResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_considered = FBUtils.getBetween(SecondResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_selected = FBUtils.getBetween(SecondResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_cap = FBUtils.getBetween(SecondResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_type = FBUtils.getBetween(SecondResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");

								if (chkWallWallPosterRemoveURLsMessages == true)
								{
									if (xhpc_message_text.Contains("https:"))
									{
										string[] arr = xhpc_message_text.Split(':');
										if (arr.Count() == 3)
										{
											//xhpc_message_text = arr[0];
											//message_text = arr[1] + ":" + arr[2];
										}
										else
										{
											xhpc_message_text = string.Empty;
										}
									}
									else
									{

									}
									//  xhpc_message_text = wallmessage;
									//if (FBGlobals.CheckLicenseManager == "fdfreetrial")
									//{
									//    xhpc_message_text = "\n\n Sent from FREE version of Facedominator. To remove this message, please buy it.";
									//}
								}
								else
								{
									//  xhpc_message_text = Uri.EscapeDataString(wallmessage + "    :    " + xhpc_message_text);
								}
								//Final PostData
								xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);


								string ResponseWallPost = "";
								if (message_text.Contains(":"))
								{
									//  message_text = message_text.Replace(":", " ");
								}

								if (string.IsNullOrEmpty(SecondResponse))
								{

									// string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + message_text) + "&xhpc_message_text=" + (xhpc_message_text + " " + message_text) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									string PostData = "&attachment&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&boosted_post_config&composer_session_id=94cba319-8335-4b53-8d0d-e9083a610c6d&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&multilingual_specified_lang=&&&privacyx&ref=timeline&tagger_session_id=1443716984&target_type=wall&&xhpc_message=" + (xhpc_message_text + " " + message_text) + "&xhpc_message_text=" + (xhpc_message_text + " " + message_text) + "&is_react=true&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&xhpc_fundraiser_page=false&__user=" + __user + "&__a=1&__dyn=7AmajEyl2lm9ongDxiWEB19CzEWq2WiWF298yut9LHwxBxCbzFVob4q68K5Uc-dy88axbxjx2u5W88ybAG5VGqzE-8KuEOq6ouAxO2OE&__req=1j&fb_dtsg=" + fb_dtsg + "&ttstamp=26581721005178848611411310377&__rev=1965820";

									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + __user), PostData);

									//string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559";
									//ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);
								}
								else
								{

									string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + message_text) + "&xhpc_message_text=" + (xhpc_message_text + " " + message_text) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + __user), PostData);
								}
								if (ResponseWallPost.Contains("The message could not be posted to this Wall.") || ResponseWallPost.Contains("Message Failed\",\"errorDescription\"") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action"))
								{
									// chkWallWallPosterRemoveURLsMessages Url

									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + __user), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
								}
								else if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + __user), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
								}
								else if (ResponseWallPost.Contains("Sorry, the privacy setting on this post means that you can't share it") && ResponseWallPost.Contains("Could Not Post to Timeline"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + __user), "composer_session_id=c9e72d37-ce06-40d8-a3f3-b35c8316bcbd&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229dbcb61a%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1e%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073742507&attachment[type]=2&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=7n8ajEyl2lm9udDgDxyKAEWCueyp9Esx6iWF299qzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=26581729512056122661171216683&__rev=1503785");
								}
								else if (ResponseWallPost.Contains("The message could not be posted to this Wall.") && ResponseWallPost.Contains("The message could not be posted to this Wall"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + __user), "composer_session_id=2f37c190-d9b1-4d18-aa9d-f4d3d85e687d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2227babdd5%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_4_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=7&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + __user + "&__a=1&__dyn=aJioznEyl2lm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1k&ttstamp=2658170679789798165112110106695577&__rev=1612042");
								}
								else
								{
									//  AddToLogger_PageManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
								}

								//try
								//{

								//    int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								//    AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								//    Thread.Sleep(delayInSeconds);
								//}
								//catch { };

								ResponseFanPagePostMessage = ResponseWallPost;

							}

							else
							{
								ResponseFanPagePostMessage = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.pageFanPagePosterUpdateStatusPhp), "&fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=&xhpc_message_text=" + message_text + "&xhpc_message=" + message_text + "&nctr[_mod]=pagelet_wall&lsd&post_form_id_source=AsyncRequest&__user=" + __user);
							}

							if (ResponseFanPagePostMessage.Contains("Couldn't Update Status")&&ResponseFanPagePostMessage.Contains("There was a problem updating your status. Please try again in a few minutes"))
							{
								count++;
								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);

								continue;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						try
						{
							if (!ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{
								count++;
								AddToLogger_PageManager(count.ToString() + " Posted Message on Fan Page Wall with " + Username + " and " + FanPageUrl);



								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
							else if (ResponseFanPagePostMessage.Contains("This status update is identical to the last one you posted"))
							{

								AddToLogger_PageManager("you can't post on this wall" + Username + " and " + FanPageUrl);



								int delayInSeconds =GlobusHttpHelper.GenerateRandom(minDelayFanPagePoster * 1000, maxDelayFanPagePoster * 1000);
								AddToLogger_PageManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds); 
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostFanPageMessageUsingUrls  in PageManager ", FBGlobals.AllExceptionLoggerFilePath);
			}

			//  Thread.Sleep(1000);
		}









































	}

}

