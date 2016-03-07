using System;
using PinDominator;
using System.Threading;

using System.Collections.Generic;

using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;

namespace PinDominator
{
	public class FriendManager
	{
		public static int NoofFriendToScrapToAddFriendForFanPage = 0;
		public static int noOfFriendsToAdd;
		public static List<string> ProfileId = new List<string>();
		public FriendManager ()
		{

		}


		//public static Event CampaignStopLogevents = new Event();

		public static void AddToLogger_FriendsManager(string log)
		{
			EventsArgs eArgs = new EventsArgs(log);
			CampaignStopLogevents.LogText(eArgs);
		}



		public static int minDelayFriendManager=10;
		public static int maxDelayFriendManager=20;



		#region Global Variables Of Friend Manager

		readonly object requestFriendsThreadControllerlockr = new object();

		public bool isRequestFriendsStop = false;

		int requestFriendsThreadControllerCount = 0;

		public List<Thread> lstRequestFriendsThreads = new List<Thread>();
		public List<string> lstRequestFriendsLocation = new List<string>();
		public List<string> lstRequestFriendsProfileURLs = new List<string>();
		public static List<string> lstRequestFriendsKeywords = new List<string>();
		public List<string> lstRequestFriendsFanPageURLs = new List<string>();
		public List<string> lstFriendIds = new List<string>();
		public static int NoOfFriendRequestFriendManager = 10;
		public static bool Friends_AcceptFriends_Female = false;
		public static bool Friends_AcceptFriends_Male= false;
		public static bool Friends_AcceptSendFrndToSuggestions = false;
		public static string Friends_AcceptSendFrndProcessUsing = string.Empty;




		#endregion

		#region Property

		public static bool IsSearchViaKeywords
		{
			get;
			set;
		}

		public static bool IsSearchViaProfileUrls
		{
			get;
			set;
		}

		public static bool IsSearchViaLocation
		{
			get;
			set;
		}

		public static bool IsSearchViaFanPageURLs
		{
			get;
			set;
		}

		public static string SendRequestUsing
		{
			get;
			set;
		}

		public static string Keywords
		{
			get;
			set;
		}

		public static string Location
		{
			get;
			set;
		}

		public static string FanPageURLs
		{
			get;
			set;
		}

		public static int NoOfThreads
		{
			get;
			set;
		}

		public static int NoOfFriendsRequest
		{
			get;
			set;
		}

		public static int NoOfFriendsRequestParKeyWord
		{
			get;
			set;
		}

		public static string StartFriendsProcessUsing
		{
			get;
			set;
		}
		#endregion



		public void StartSendFriendRequest()
		{
			requestFriendsThreadControllerCount = 0;
			try
			{
				int numberOfAccountPatch = 25;

				if (NoOfThreads > 0)
				{
					numberOfAccountPatch = NoOfThreads;
				}

				List<List<string>> list_listAccounts = new List<List<string>>();
				if (FBGlobals.listAccounts.Count >= 1)
				{

					//list_listAccounts = Utils.Split(FBGlobals.listAccounts, numberOfAccountPatch);

					//foreach (List<string> listAccounts in FBGlobals.listAccounts)
					{
						//int tempCounterAccounts = 0; 

						foreach (string account in FBGlobals.listAccounts)
						{
							try
							{
								lock (requestFriendsThreadControllerlockr)
								{
									try
									{
										if (requestFriendsThreadControllerCount >= FBGlobals.listAccounts.Count)
										{
											Monitor.Wait(requestFriendsThreadControllerlockr);
										}
										try
										{
											string acc = account.Remove(account.IndexOf(':'));
											//Run a separate thread for each account
											FacebookUser item = null;
											FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

											if (item != null)
											{
												Thread profilerThread = new Thread(SendFriendRequestMultiThreads);
												profilerThread.Name = "workerThread_Profiler_" + acc;
												profilerThread.IsBackground = true;
												profilerThread.Start(new object[] { item });
												requestFriendsThreadControllerCount++;
												//tempCounterAccounts++; 
											}
										}
										catch(Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}
									}
									catch(Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
							}
							catch(Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}



		private void SendFriendRequestMultiThreads(object parameters)
		{
			try
 			{
			//	if (!isStopWallPoster)
				{
					try
					{
						lstThreadsWallPoster.Add(Thread.CurrentThread);
						lstThreadsWallPoster.Distinct();
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
							AddToLogger_FriendsManager("Successful login With : "+objFacebookUser.username);
							if (StartFriendsProcessUsing=="Add Friends via fanpage url")
							{
								FriendRequestViaFanPageUrl(objFacebookUser);
								AddToLogger_FriendsManager("Process Completed With : "+objFacebookUser.username);

							}
							else if (StartFriendsProcessUsing==" Add friends from keyword")
							{
								StartActionFriendsManager(ref objFacebookUser);

								AddToLogger_FriendsManager("Process Completed With : "+objFacebookUser.username);


							}
							else if(StartFriendsProcessUsing=="Send Text message on Friends wall")
							{
								AddToLogger_FriendsManager("Please wait Send Text message process started ....");
								WallPostingWithTestMessage(ref objFacebookUser);
								AddToLogger_FriendsManager("Send text message on friends wall Process completed .");
							}
							else if(StartFriendsProcessUsing=="Send Url message on Friends wall")
							{
								WallPostingNew(ref objFacebookUser); 
							}
							else if(StartFriendsProcessUsing=="Send Picture on Friends wall")
							{
								StartActionPostPicOnWall(ref objFacebookUser);
							}

							// Call SendFriendRequests
							//SendFriendRequests(ref objFacebookUser);
						}
						else
						{
							AddToLogger_FriendsManager("Couldn't Login With Username : " + objFacebookUser.username);
						}                               

					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
					//GlobusLogHelper.log.Debug("Process completed !!");
					//AddToLogger_FriendsManager("Process completed !!");

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
					if (!isRequestFriendsStop)
					{
						lock (requestFriendsThreadControllerlockr)
						{
							requestFriendsThreadControllerCount--;
							Monitor.Pulse(requestFriendsThreadControllerlockr);


						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
			}
		}









		public void FriendRequestViaFanPageUrl(FacebookUser fbUser)
		{

			#region Variables resion
			string posthtml = string.Empty;
			string posturl1 = string.Empty;
			string post = string.Empty;           
			string like_id = string.Empty;
			string like_name = string.Empty;           
			string commentid = string.Empty;
			string commentmsg = string.Empty;
			string commentcreated_time = string.Empty;
			string commentlike_count = string.Empty;
			string user_likes = string.Empty;           
			string post_date = string.Empty;
			string linkurl = string.Empty;
			string pictureurl = string.Empty;
			string statustype = string.Empty;
			string type = string.Empty;
			string fromname = string.Empty;
			List<string> FriendLink = new List<string> ();
			int NoOfFriendsRequest = 0;
			//string postid = string.Empty;
			string UserId = string.Empty;
			string fb_dtsg = string.Empty;
			string getlikecomment = string.Empty;
			DateTime comment_date = new DateTime ();
			string postid = string.Empty;
			string pageid = string.Empty;
			#endregion

			try {      

				foreach (var item in FBUtils.LoadFanpageUrls) 
				{
					AddToLogger_FriendsManager ( "Please wait Scraping Profile Id for " + item);

					Thread threadAjaxPosterNew = new Thread(() => AjaxPosterNew(fbUser, item));
					threadAjaxPosterNew.Start();

					GlobusHttpHelper objGlobusHttpHelper = fbUser.globusHttpHelper;
					posthtml = objGlobusHttpHelper.getHtmlfromUrl (new Uri ("https://www.facebook.com/"), "", "");

					UserId = GlobusHttpHelper.GetParamValue (posthtml, "user");
					if (string.IsNullOrEmpty (UserId)) {
						UserId = GlobusHttpHelper.ParseJson (posthtml, "user");
					}

					fb_dtsg = GlobusHttpHelper.GetParamValue (posthtml, "fb_dtsg");
					if (string.IsNullOrEmpty (fb_dtsg)) {
						fb_dtsg = GlobusHttpHelper.ParseJson (posthtml, "fb_dtsg");
					}

					Thread.Sleep(1 * 60 * 1000);

					try {				

						{

							{

								AddToLogger_FriendsManager (ProfileId.Count + " Search Friend Requests Url with Email " + fbUser.username);

								int countFriendRequestsSent = 0;
								int counterforblockedFriendrequest = 0;

								while (true)
								{
									List<string> ProfileIdNew = new List<string>();

									if (ProfileId.Count() > 0)
									{
										try
										{
											ProfileIdNew.AddRange(ProfileId);
											ProfileId.RemoveRange(0, ProfileIdNew.Count);
										}
										catch { };



										foreach (string FriendRequestLink in ProfileIdNew)
										{
											try
											{

												if (countFriendRequestsSent >= NoOfFriendRequestFriendManager)
												{
													try
													{
														AddToLogger_FriendsManager("Given No. Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
														threadAjaxPosterNew.Abort();
													}
													catch { };
													return;
												}
												if (!FriendRequestLink.Contains("100"))
												{
													string checkPofileid = objGlobusHttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/" + FriendRequestLink.Trim()), "", "");
													if (!checkPofileid.Contains("FriendRequestAdd addButton") & checkPofileid.Contains("FriendRequestAdd addButton"))
													{
														continue;
													}
												}


												AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
												bool requeststatus = SendFriendRequestUpdated(FriendRequestLink.Trim(), UserId, ref fbUser);

												if (requeststatus)
												{
													countFriendRequestsSent++;
													counterforblockedFriendrequest = 1;
													AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);


												}
												else
												{
													//counterforblockedFriendrequest++;
													//if (counterforblockedFriendrequest == 3)
													//{
													//    break;
													//}
												}
											}
											catch (Exception ex)
											{
												Console.WriteLine("Error : " + ex.StackTrace);
											}
										}
									}
									else
									{
										Thread.Sleep(1 * 60 * 1000);

										if (ProfileId.Count() > 0)
										{
											continue;
										}
										else
										{
											Thread.Sleep(1 * 60 * 1000);

											if (ProfileId.Count() > 0)
											{
												continue;
											}
											else
											{
												Thread.Sleep(1 * 60 * 1000);

												if (ProfileId.Count() > 0)
												{
													continue;
												}
												else
												{
													AddToLogger_FriendsManager("All Requests are sent");
													return;
												}


											}


										}


									}
								}
							}

						}

					} catch (Exception ex) {
						Console.WriteLine (ex.StackTrace);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.StackTrace);
			}

		}









		private List<string> AjaxPosterNewOld(ref FacebookUser fbUser, string FanPageurl)
		{ 
			List<string>ProfileId=new List<string>();
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{

				{
					List<string> lstNormalPost = new List<string>();
					List<string> lstPostWithPhoto = new List<string>();
					string pageId = string.Empty;
					string userID = string.Empty;
					List<string> Idlist = new List<string>();

					string homeapgesrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");
					userID = GlobusHttpHelper.GetParamValue(homeapgesrc, "user"); 
					string fanPageSrc = string.Empty;
					fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl),"","");

					pageId = FBUtils.getBetween(fanPageSrc, "pageID\":", ",");
					if (!string.IsNullOrEmpty(fanPageSrc))
					{
						string[] pagaDataByHref = Regex.Split(fanPageSrc,"href");
						foreach (string item in pagaDataByHref)
						{
							string temp = FBUtils.getBetween(item, "\"", "\"");
							if (temp.Contains("photos"))
							{
								lstPostWithPhoto.Add(temp);
							}
							if(temp.Contains("posts"))
							{
								lstNormalPost.Add(temp);
							}
						}


						//PAgination Logic
						string[] scrollingData = Regex.Split(fanPageSrc, "function");
						string nextPagData = string.Empty;
						foreach (string scrolling in scrollingData)
						{
							if (scrolling.Contains("return new ScrollingPager"))
							{
								nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\",string.Empty);
								break;
							}
						}
						Queue<string> QueueStart = new Queue<string>();
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1420099200,\"end\":1451635199,\"query_type\":8,\"filter\":1,\"filter_after_timestamp\":1420400403},\"section_index\":1,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1},\"section_index\":2,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":26,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1,\"is_pages_redesign\":true},\"section_index\":2,\"hidden\":false,\"posts_loaded\":26,\"show_all_posts\":false}");
						//while (true)
						{
							string nextPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/PagePostsSectionPagelet?data=" + Uri.EscapeDataString(nextPagData) + "&__user="+userID+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUWdDx2ubhHxd5BzEy6Kdy8-&__req=r&__rev=1555029"),"","");
							string[] scrollingData1 = Regex.Split(nextPageSource, "function");
							nextPagData = string.Empty;
							foreach (string scrolling in scrollingData1)
							{
								if (scrolling.Contains("return new ScrollingPager"))
								{
									nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\", string.Empty);
									break;
								}
							}

							string[] pagaDataByHref1 = Regex.Split(nextPageSource, "href");
							foreach (string item in pagaDataByHref1)
							{
								string temp = FBUtils.getBetween(item, "\"", "\"");
								if (temp.Contains("photos"))
								{
									lstPostWithPhoto.Add(temp);
								}
								if (temp.Contains("posts"))
								{
									lstNormalPost.Add(temp);
								}
							}
							if (string.IsNullOrEmpty(nextPagData))
							{
								if (QueueStart.Count != 0)
								{
									nextPagData = QueueStart.Dequeue();
								}
								else
								{
									//	break;
								}

							}

						}

						lstPostWithPhoto = lstPostWithPhoto.Distinct().ToList();
						lstNormalPost = lstNormalPost.Distinct().ToList();

						int Count=0;
						foreach (string link in lstPostWithPhoto)
						{
							if (Count==4)
							{
								break;

							}
							Count=Count+1;

							string temp = link;
							temp = temp.Replace("/?type=1", string.Empty);
							string[] spitData = temp.Split('/');
							string postId = string.Empty;
							postId = spitData[spitData.Length - 1];
							if (postId.Contains("&"))
							{
								int i = postId.IndexOf('&');
								int j = postId.Length - 1;
								try
								{
									postId = postId.Remove(postId.IndexOf('&'),((postId.Length)-postId.IndexOf('&')));
								}
								catch(Exception ex){};
							}
							string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId),"","");
							string [] arr=System.Text.RegularExpressions.Regex.Split(graphResp,"\"created_time\"");

							arr=arr.Skip(0).ToArray();
							foreach (var item_arr in arr)
							{
								try
								{
									string GetId=FBUtils.getBetween(item_arr,"\"id\":","\n").Replace("\"",string.Empty).Replace(",",string.Empty);
									ProfileId.Add(GetId);
									AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId +"Fanpage url : "+FanPageurl);
								}
								catch(Exception ex){};

							}

						}

					}


				}
			}
			catch (Exception ex)
			{
				return 	ProfileId;

			}
			return 	ProfileId;

		}






		private List<string> AjaxPosterNew_old_old_Old12(ref FacebookUser fbUser, string FanPageurl)
		{ 
			List<string>ProfileId=new List<string>();
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{

				{
					List<string> lstNormalPost = new List<string>();
					List<string> lstPostWithPhoto = new List<string>();
					string pageId = string.Empty;
					string userID = string.Empty;
					List<string> Idlist = new List<string>();

					string homeapgesrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");
					userID = GlobusHttpHelper.GetParamValue(homeapgesrc, "user"); 
					string fanPageSrc = string.Empty;
					fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl),"","");

					pageId = FBUtils.getBetween(fanPageSrc, "pageID\":", ",");
					if (!string.IsNullOrEmpty(fanPageSrc))
					{
						string[] pagaDataByHref = Regex.Split(fanPageSrc,"href");
						foreach (string item in pagaDataByHref)
						{
							string temp = FBUtils.getBetween(item, "\"", "\"");
							if (temp.Contains("photos"))
							{
								lstPostWithPhoto.Add(temp);
							}
							if(temp.Contains("posts"))
							{
								lstNormalPost.Add(temp);
							}
						}


						//PAgination Logic
						string[] scrollingData = Regex.Split(fanPageSrc, "function");
						string nextPagData = string.Empty;
						foreach (string scrolling in scrollingData)
						{
							if (scrolling.Contains("return new ScrollingPager"))
							{
								nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\",string.Empty);
								break;
							}
						}
						Queue<string> QueueStart = new Queue<string>();
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1420099200,\"end\":1451635199,\"query_type\":8,\"filter\":1,\"filter_after_timestamp\":1420400403},\"section_index\":1,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1},\"section_index\":2,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":26,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1,\"is_pages_redesign\":true},\"section_index\":2,\"hidden\":false,\"posts_loaded\":26,\"show_all_posts\":false}");
						//while (true)
						{
							string nextPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/PagePostsSectionPagelet?data=" + Uri.EscapeDataString(nextPagData) + "&__user="+userID+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUWdDx2ubhHxd5BzEy6Kdy8-&__req=r&__rev=1555029"),"","");
							string[] scrollingData1 = Regex.Split(nextPageSource, "function");
							nextPagData = string.Empty;
							foreach (string scrolling in scrollingData1)
							{
								if (scrolling.Contains("return new ScrollingPager"))
								{
									nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\", string.Empty);
									break;
								}
							}

							string[] pagaDataByHref1 = Regex.Split(nextPageSource, "href");
							foreach (string item in pagaDataByHref1)
							{
								string temp = FBUtils.getBetween(item, "\"", "\"");
								if (temp.Contains("photos"))
								{
									lstPostWithPhoto.Add(temp);
								}
								if (temp.Contains("posts"))
								{
									lstNormalPost.Add(temp);
								}
							}
							if (string.IsNullOrEmpty(nextPagData))
							{
								if (QueueStart.Count != 0)
								{
									nextPagData = QueueStart.Dequeue();
								}
								else
								{
									//	break;
								}

							}

						}

						lstPostWithPhoto = lstPostWithPhoto.Distinct().ToList();
						lstNormalPost = lstNormalPost.Distinct().ToList();

						int Count=0;
						foreach (string link in lstPostWithPhoto)
						{
							if (Count==4)
							{
								break;

							}
							Count=Count+1;

							string temp = link;
							temp = temp.Replace("/?type=1", string.Empty);
							string[] spitData = temp.Split('/');
							string postId = string.Empty;
							postId = spitData[spitData.Length - 1];
							if (postId.Contains("&"))
							{
								int i = postId.IndexOf('&');
								int j = postId.Length - 1;
								try
								{
									postId = postId.Remove(postId.IndexOf('&'),((postId.Length)-postId.IndexOf('&')));
								}
								catch(Exception ex){};
							}
							string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId),"","");
							if(string.IsNullOrEmpty(graphResp))
							{
								graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/" + postId),"","");

								if(graphResp.Contains("actorid\":"))
								{
									try{
										string[] splitpage = Regex.Split(graphResp,"actorid\":");
										string actorId = FBUtils.getBetween(splitpage[1],"\"","\"");
										string finalUrl = "http://www.facebook.com" + "/browse/likes?id=" + postId + "&actorid="+ actorId;

										string Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl),"","");

										string[] Users_Liked_List = Regex.Split(Users_Liked_Page,"class=\"fbProfileBrowserListItem") ;
										foreach(string str in Users_Liked_List)
										{
											try
											{
												if(str.Contains("<!DOCTYPE html>"))
												{
													continue;

												}
												else
												{
													string UserId = FBUtils.getBetween(str,"data-profileid=\"","\"");
													ProfileId.Add(UserId);
												}
											}
											catch{};

										}



									}
									catch {};





								}

								if(graphResp.Contains("fbPhotoTagListTag tagItem"))
								{

									string[] nooFriendSplit = Regex.Split(graphResp,"fbPhotoTagListTag tagItem");
									//string 

								}



							}
							else
							{

								string [] arr=System.Text.RegularExpressions.Regex.Split(graphResp,"\"created_time\"");

								arr=arr.Skip(0).ToArray();
								foreach (var item_arr in arr)
								{
									try
									{
										string GetId=FBUtils.getBetween(item_arr,"\"id\":","\n").Replace("\"",string.Empty).Replace(",",string.Empty);
										ProfileId.Add(GetId);
										AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId +"Fanpage url : "+FanPageurl);
									}
									catch(Exception ex){};

								}
							}

						}

					}


				}
			}
			catch (Exception ex)
			{
				return 	ProfileId;

			}
			return 	ProfileId;

		}





		private List<string> AjaxPosterNew_Old24Sep(ref FacebookUser fbUser, string FanPageurl)
		{ 
			List<string>ProfileId=new List<string>();
			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{

				{
					List<string> lstNormalPost = new List<string>();
					List<string> lstPostWithPhoto = new List<string>();
					string pageId = string.Empty;
					string userID = string.Empty;
					List<string> Idlist = new List<string>();

					string homeapgesrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");
					userID = GlobusHttpHelper.GetParamValue(homeapgesrc, "user"); 
					string fanPageSrc = string.Empty;
					fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl),"","");

					pageId = FBUtils.getBetween(fanPageSrc, "pageID\":", ",");
					if (!string.IsNullOrEmpty(fanPageSrc))
					{
						string[] pagaDataByHref = Regex.Split(fanPageSrc,"href");
						foreach (string item in pagaDataByHref)
						{
							string temp = FBUtils.getBetween(item, "\"", "\"");
							if (temp.Contains("photos"))
							{
								lstPostWithPhoto.Add(temp);
							}
							if(temp.Contains("posts"))
							{
								lstNormalPost.Add(temp);
							}
						}


						//PAgination Logic
						string[] scrollingData = Regex.Split(fanPageSrc, "function");
						string nextPagData = string.Empty;
						foreach (string scrolling in scrollingData)
						{
							if (scrolling.Contains("return new ScrollingPager"))
							{
								nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\",string.Empty);
								break;
							}
						}
						Queue<string> QueueStart = new Queue<string>();
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1420099200,\"end\":1451635199,\"query_type\":8,\"filter\":1,\"filter_after_timestamp\":1420400403},\"section_index\":1,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1},\"section_index\":2,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
						QueueStart.Enqueue("{\"segment_index\":26,\"page_index\":0,\"page\":"+pageId+",\"column\":\"main\",\"post_section\":{\"profile_id\":"+pageId+",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1,\"is_pages_redesign\":true},\"section_index\":2,\"hidden\":false,\"posts_loaded\":26,\"show_all_posts\":false}");
						//while (true)
						{
							string nextPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/PagePostsSectionPagelet?data=" + Uri.EscapeDataString(nextPagData) + "&__user="+userID+"&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUWdDx2ubhHxd5BzEy6Kdy8-&__req=r&__rev=1555029"),"","");
							string[] scrollingData1 = Regex.Split(nextPageSource, "function");
							nextPagData = string.Empty;
							foreach (string scrolling in scrollingData1)
							{
								if (scrolling.Contains("return new ScrollingPager"))
								{
									nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\", string.Empty);
									break;
								}
							}

							string[] pagaDataByHref1 = Regex.Split(nextPageSource, "href");
							foreach (string item in pagaDataByHref1)
							{
								string temp = FBUtils.getBetween(item, "\"", "\"");
								if (temp.Contains("photos"))
								{
									lstPostWithPhoto.Add(temp);
								}
								if (temp.Contains("posts"))
								{
									lstNormalPost.Add(temp);
								}
							}
							if (string.IsNullOrEmpty(nextPagData))
							{
								if (QueueStart.Count != 0)
								{
									nextPagData = QueueStart.Dequeue();
								}
								else
								{
									//	break;
								}

							}

						}

						lstPostWithPhoto = lstPostWithPhoto.Distinct().ToList();
						lstNormalPost = lstNormalPost.Distinct().ToList();

						int Count=0;
						foreach (string link in lstPostWithPhoto)
						{
							if (Count==4)
							{
								break;

							}
							Count=Count+1;

							string temp = link;
							temp = temp.Replace("/?type=1", string.Empty);
							string[] spitData = temp.Split('/');
							string postId = string.Empty;
							postId = spitData[spitData.Length - 1];
							if (postId.Contains("&"))
							{
								int i = postId.IndexOf('&');
								int j = postId.Length - 1;
								try
								{
									postId = postId.Remove(postId.IndexOf('&'),((postId.Length)-postId.IndexOf('&')));
								}
								catch(Exception ex){};
							}
							string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId),"","");
							if(string.IsNullOrEmpty(graphResp))
							{
								graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/" + postId),"","");

								if(graphResp.Contains("actorid\":"))
								{
									try{
										string[] splitpage = Regex.Split(graphResp,"actorid\":");
										string actorId = FBUtils.getBetween(splitpage[1],"\"","\"");
										string finalUrl = "http://www.facebook.com" + "/browse/likes?id=" + postId + "&actorid="+ actorId;

										string Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl),"","");

										string[] Users_Liked_List = Regex.Split(Users_Liked_Page,"class=\"fbProfileBrowserListItem") ;
										foreach(string str in Users_Liked_List)
										{
											try
											{
												if(str.Contains("<!DOCTYPE html>"))
												{
													continue;

												}
												else
												{
													string UserId = FBUtils.getBetween(str,"data-profileid=\"","\"");
													if(!string.IsNullOrEmpty(UserId))
													{
														ProfileId.Add(UserId);
													}
												}
											}
											catch{};

										}



									}
									catch {};





								}

								if(graphResp.Contains("fbPhotoTagListTag tagItem"))
								{

									string[] nooFriendSplit = Regex.Split(graphResp,"fbPhotoTagListTag tagItem");
									//string 

								}



							}
							else
							{

								string [] arr=System.Text.RegularExpressions.Regex.Split(graphResp,"\"created_time\"");

								arr=arr.Skip(0).ToArray();
								foreach (var item_arr in arr)
								{
									try
									{
										string GetId=FBUtils.getBetween(item_arr,"\"id\":","\n").Replace("\"",string.Empty).Replace(",",string.Empty);
										if(!string.IsNullOrEmpty(GetId))
										{
											ProfileId.Add(GetId);
										}

										AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId +"Fanpage url : "+FanPageurl);
									}
									catch(Exception ex){};

								}
							}

						}

					}

					try
					{

						if(fanPageSrc.Contains("people like this topic"))
						{
							if(fanPageSrc.Contains("/browse/graph/?q="))
							{
								string klsfsldfk = "/browse/graph/";
								string[] pagesplit = Regex.Split(fanPageSrc,klsfsldfk);
								if(pagesplit.Count()>1)
								{
									string frienfListPageUrl = FBUtils.getBetween(pagesplit[1],"=","\"");
									frienfListPageUrl = "https://www.facebook.com/browse/graph/?q=" + frienfListPageUrl ;
									string frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl),"","");

									//while(true)
									{

										try
										{

											string[] Users_Liked_List = Regex.Split(frienfListPage,"class=\"fbProfileBrowserListItem") ;
											foreach(string str in Users_Liked_List)
											{
												try
												{
													if(str.Contains("<!DOCTYPE html>"))
													{
														continue;

													}
													else
													{
														string UserId = FBUtils.getBetween(str,"id=","&");
														if(!string.IsNullOrEmpty(UserId))
														{
															ProfileId.Add(UserId);
														}
													}
												}
												catch{};

											}


											try
											{
												if(fanPageSrc.Contains("/ajax/browser/dialog/graph/?q="))
												{

													frienfListPageUrl= FBUtils.getBetween(fanPageSrc,"/ajax/browser/dialog/graph/?q=" ,"\"");  //\/ajax\/browser\/list\/graph\/?q=


													frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1"  + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";
													int totalNoOfUsersScraped = 0;
													while(true)
													{
														if((FriendManager.NoofFriendToScrapToAddFriendForFanPage+200)<=ProfileId.Count())
														{
															AddToLogger_FriendsManager("All Data Scraped ");
															break;	

														}
														totalNoOfUsersScraped = ProfileId.Count();
														frienfListPage  = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl),"","");
														if(!string.IsNullOrEmpty(frienfListPage))
														{
															string[] UsersData = Regex.Split(frienfListPage,"user.php");
															List<string> UsersDataList =  UsersData.ToList();
															UsersDataList.RemoveAt(0);
															foreach(string item in UsersDataList)
															{
																try
																{
																	string userid = FBUtils.getBetween(item,"id=","&");
																	if(!string.IsNullOrEmpty(userid))
																	{
																		ProfileId.Add(userid);

																	}

																}
																catch{};
															}

															if(frienfListPage.Contains("\\/ajax\\/browser\\/list\\/graph\\/?q="))
															{
																frienfListPageUrl = FBUtils.getBetween(frienfListPage,"\\/ajax\\/browser\\/list\\/graph\\/?q=","\"");
																frienfListPageUrl = frienfListPageUrl.Replace("\\","");
																frienfListPageUrl = "https://www.facebook.com/ajax/browser/list/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1"  + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";

															}
															else
															{
																AddToLogger_FriendsManager("All Data Scraped ");
																break;
															}
															if(ProfileId.Count()==totalNoOfUsersScraped)
															{
																AddToLogger_FriendsManager("All Data Scraped ");
																break;	
															}



														}
													}


												}
											}
											catch{};
										}
										catch{};
									}
								}


							}
						}
					}
					catch{};


				}
			}
			catch (Exception ex)
			{
				return 	ProfileId;

			}
			return 	ProfileId;

		}






		private List<string> AjaxPosterNew_Old_26_oct(FacebookUser fbUser, string FanPageurl)
		{
			try
			{

				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string FanPageurl_Photos = "";
				string FanPageurl_PhotosNew = "";
				try
				{

					{
						List<string> lstNormalPost = new List<string>();
						List<string> lstPostWithPhoto = new List<string>();
						string pageId = string.Empty;
						string userID = string.Empty;
						List<string> Idlist = new List<string>();

						string homeapgesrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl), "", "");
						userID = GlobusHttpHelper.GetParamValue(homeapgesrc, "user");
						string fanPageSrc = string.Empty;
						fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl), "", "");

						pageId = FBUtils.getBetween(fanPageSrc, "pageID\":", ",").Replace("}]]", "").Replace("\"", "");
						if (!string.IsNullOrEmpty(fanPageSrc))
						{

							lstPostWithPhoto = lstPostWithPhoto.Distinct().ToList();
							lstNormalPost = lstNormalPost.Distinct().ToList();

							if (FanPageurl.Contains("?"))
							{
								try
								{
									string[] FanPageurllst = Regex.Split(FanPageurl, "fref");
									FanPageurl = FanPageurllst[0].Replace("?", "");
								}
								catch { };
							}


							if (FanPageurl[FanPageurl.Count() - 1] == '/')
							{
								FanPageurl_Photos = FanPageurl + "photos_stream";
								FanPageurl_PhotosNew = FanPageurl_Photos;

							}
							else
							{
								FanPageurl_Photos = FanPageurl + "/photos_stream";
								FanPageurl_PhotosNew = FanPageurl_Photos;


							}

							string PhototStreamPage = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl_Photos), "", "");

							//string username = FBUtils.getBetween(PhototStreamPage, "content=\"https://www.facebook.com/", "?");
							string username = "";
							try
							{
								username = FBUtils.getBetween(Regex.Split(FanPageurl_Photos, "www.facebook.com")[1], "/", "/");
							}
							catch { };

							string ajaxpipe_token = FBUtils.getBetween(PhototStreamPage, "ajaxpipe_token\":\"", "\"");
							string __user = FBUtils.getBetween(PhototStreamPage, "userID\":\"", "\"");

							string[] pagaDataByHref12 = Regex.Split(PhototStreamPage, "href");
							List<string> lstPostWithPhotoNew = new List<string>();
							foreach (string item in pagaDataByHref12)
							{
								string temp = FBUtils.getBetween(item, "\"", "\"");
								if (temp.Contains("photos"))
								{
									if (item.Contains("/photos/"))
									{
										// lstPostWithPhotoNew.Add(temp);
									}

								}

							}

							int Count = 0;
							int countFoBelloWhileLoop = 0;
							while (true)
							{

								if (countFoBelloWhileLoop > FriendManager.NoofFriendToScrapToAddFriendForFanPage)
								{
									// return ProfileId;

								}
								countFoBelloWhileLoop++;
								try
								{
									string[] pagaDataByHref1 = Regex.Split(PhototStreamPage, "OGAggregationHovercardTarget");
									foreach (string item in pagaDataByHref1)
									{

										string temp = FBUtils.getBetween(item, "fbid=", "&");
										if (!string.IsNullOrEmpty(temp) && (temp.Count() < 30) && !string.IsNullOrEmpty(username))
										{
											string tempNew = "/" + username + "/photos/" + temp;

											lstPostWithPhotoNew.Add(tempNew);
											lstPostWithPhotoNew = lstPostWithPhotoNew.Distinct().ToList();
											AddToLogger_FriendsManager("Add Photo ID : " + tempNew);




											try
											{

												string link = tempNew;

												#region ScrapUserForeachPhoto
												{


													try
													{
														if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
														{
															//  AddToLogger_FriendsManager("All Data Scraped ");
															//  return ProfileId;

														}
														Count = Count + 1;

														temp = temp.Replace("/?type=1", string.Empty);
														string[] spitData = temp.Split('/');
														string postId = string.Empty;
														postId = spitData[spitData.Length - 1];
														if (postId.Contains("&"))
														{
															int i = postId.IndexOf('&');
															int j = postId.Length - 1;
															try
															{
																postId = postId.Remove(postId.IndexOf('&'), ((postId.Length) - postId.IndexOf('&')));
															}
															catch (Exception ex) { };
														}
														string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId), "", "");
														if (string.IsNullOrEmpty(graphResp))
														{
															graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/" + postId), "", "");

															if (graphResp.Contains("actorid\":"))
															{
																try
																{
																	string[] splitpage = Regex.Split(graphResp, "actorid\":");
																	string actorId = FBUtils.getBetween(splitpage[1], "\"", "\"");
																	string finalUrl = "http://www.facebook.com" + "/browse/likes?id=" + postId + "&actorid=" + actorId;

																	string Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl), "", "");

																	string[] Users_Liked_List = Regex.Split(Users_Liked_Page, "class=\"fbProfileBrowserListItem");
																	foreach (string str in Users_Liked_List)
																	{
																		try
																		{
																			if (str.Contains("<!DOCTYPE html>"))
																			{
																				continue;

																			}
																			else
																			{
																				string UserId = FBUtils.getBetween(str, "data-profileid=\"", "\"");
																				if (!string.IsNullOrEmpty(UserId))
																				{
																					ProfileId.Add(UserId);
																					AddToLogger_FriendsManager("Add Profile ID : " + UserId);

																					if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																					{
																						// AddToLogger_FriendsManager("Required Data Scraped ");
																						// return ProfileId;

																					}
																				}
																			}
																		}
																		catch { };

																	}



																}
																catch { };

															}

															if (graphResp.Contains("fbPhotoTagListTag tagItem"))
															{

																string[] nooFriendSplit = Regex.Split(graphResp, "fbPhotoTagListTag tagItem");
																//string 
															}

														}
														else
														{
															string[] arr = System.Text.RegularExpressions.Regex.Split(graphResp, "\"created_time\"");
															arr = arr.Skip(0).ToArray();
															foreach (var item_arr in arr)
															{
																try
																{
																	string GetId = FBUtils.getBetween(item_arr, "\"id\":", "\n").Replace("\"", string.Empty).Replace(",", string.Empty);
																	if (!string.IsNullOrEmpty(GetId))
																	{
																		ProfileId.Add(GetId);
																		ProfileId = ProfileId.Distinct().ToList();
																		AddToLogger_FriendsManager("Add Profile ID : " + GetId);

																		if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																		{
																			//  AddToLogger_FriendsManager("Required Data Scraped ");
																			//  return ProfileId;

																		}

																	}

																	AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId + "Fanpage url : " + FanPageurl);
																}
																catch (Exception ex) { };

															}
														}
													}
													catch { };


													#region ScrapeUsers


													try
													{
														string urlName = "https://www.facebook.com" + link;

														fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(urlName), "", "");

														if (fanPageSrc.Contains("content=\"fb://photo/"))  ///ajax/browser/dialog/likes?id
														{

															string idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

															string idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

															string urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

															// string[] LikeUserPageSplit = Regex.Split(fanPageSrc, "ajax/browser/dialog/likes?id");

															//string urlUsers = FBUtils.getBetween(LikeUserPageSplit[1], "href=\"", "\"");
															urlUsers = "http://www.facebook.com" + urlUsers;

															string frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(urlUsers), "", "");


															try
															{

																string[] Users_Liked_List = Regex.Split(frienfListPage, "class=\"fbProfileBrowserListItem");
																Users_Liked_List = Regex.Split(frienfListPage, "https://www.facebook.com/profile.php?");
																foreach (string str in Users_Liked_List)
																{
																	try
																	{
																		if (str.Contains("<!DOCTYPE html>"))
																		{
																			continue;

																		}
																		else
																		{
																			string UserId = FBUtils.getBetween(str, "id=", "&");
																			if (!string.IsNullOrEmpty(UserId) && UserId.Count() < 30)
																			{
																				ProfileId.Add(UserId);
																				ProfileId = ProfileId.Distinct().ToList();
																				AddToLogger_FriendsManager("Add Profile ID : " + UserId);
																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					// AddToLogger_FriendsManager("Required Data Scraped ");
																					// return ProfileId;

																				}
																			}
																			else if (UserId.Count() > 30)
																			{
																				UserId = FBUtils.getBetween(str, "id=", "\"");
																				ProfileId.Add(UserId);
																				ProfileId = ProfileId.Distinct().ToList();
																				AddToLogger_FriendsManager("Add Profile ID : " + UserId);
																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					//  AddToLogger_FriendsManager("Required Data Scraped ");
																					//  return ProfileId;

																				}

																			}
																		}
																	}
																	catch { };

																}


																try
																{
																	if (fanPageSrc.Contains("content=\"fb://photo/"))
																	{


																		idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

																		idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

																		urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

																		urlUsers = "http://www.facebook.com" + urlUsers;




																		//  string frienfListPageUrl = FBUtils.getBetween(frienfListPage, "/ajax/browser/dialog/graph/?q=", "\"");  //\/ajax\/browser\/list\/graph\/?q=

																		string[] frienfListPageUrllst = Regex.Split(urlUsers, "id=");
																		//   string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1" + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";

																		string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/likes?id=" + idUser + "&actorid=" + idactor + "&__asyncDialog=1&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWHwKACF3ozBDirWU8popyUW3F6xybxu3fzob8kxjUW4oSy28yiq5UB1afybDGcCK5o-4Usw&__req=15&__rev=1908382";
																		// https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=100&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=i&__rev=1909862 HTTP/1.1
																		//https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=200&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862 HTTP/1.

																		int totalNoOfUsersScraped = 0;
																		int CountForPagination = 1;
																		int CountOfIterationHere = 0;
																		int countFoBelloWhileLoop2 = 0;
																		while (true)
																		{

																			if (countFoBelloWhileLoop2 > FriendManager.NoofFriendToScrapToAddFriendForFanPage)
																			{
																				break;

																			}
																			countFoBelloWhileLoop2++;

																			try
																			{

																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					// AddToLogger_FriendsManager("All Data Scraped ");
																					// return ProfileId;


																				}
																				totalNoOfUsersScraped = ProfileId.Count();
																				frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl), "", "");
																				if (!string.IsNullOrEmpty(frienfListPage))
																				{
																					string[] UsersData = Regex.Split(frienfListPage, "user.php");
																					List<string> UsersDataList = UsersData.ToList();
																					UsersDataList.RemoveAt(0);
																					int ProfileIdCountBefore = ProfileId.Count();
																					foreach (string itemNew in UsersDataList)
																					{
																						try
																						{
																							string userid = FBUtils.getBetween(itemNew, "id=", "&");
																							if (!string.IsNullOrEmpty(userid))
																							{
																								ProfileId.Add(userid);
																								ProfileId = ProfileId.Distinct().ToList();
																								AddToLogger_FriendsManager("Add Profile ID : " + userid);


																								if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																								{
																									//   AddToLogger_FriendsManager("Required Data Scraped ");
																									//  return ProfileId;

																								}

																							}

																						}
																						catch { };
																					}
																					int ProfileIdCountAfter = ProfileId.Count();

																					if (!frienfListPage.Contains("See More"))
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						break;
																					}
																					else if (ProfileIdCountAfter == ProfileIdCountBefore)
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						break;
																					}
																					else
																					{
																						frienfListPageUrl = "https://www.facebook.com/ajax/browser/list/likes/?id=" + idUser + "&actorid=" + idactor + "&beforetime=0&aftertime=0&start=" + CountForPagination * 100 + "&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862";
																						CountForPagination++;
																					}

																					if (ProfileId.Count() == totalNoOfUsersScraped)
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						break;
																					}

																				}
																				else
																				{
																					break;
																				}
																			}
																			catch (Exception Ex)
																			{ }
																		}

																	}
																}
																catch { };
															}
															catch { };
														}

													}
													catch { };

													#endregion

												}

												#endregion



											}
											catch { };

										}

									}

									if (PhototStreamPage.Contains("TimelinePhotosStreamPagelet"))
									{
										try
										{
											string[] betweendata = Regex.Split(PhototStreamPage, "TimelinePhotosStreamPagelet");

											string last_fbid = FBUtils.getBetween(betweendata[1], "\"last_fbid\\\":", ",");
											string fetch_size = FBUtils.getBetween(betweendata[1], "\"fetch_size\\\":", ",");
											string profile_id = FBUtils.getBetween(betweendata[1], "\"profile_id\\\":", ",");
											string vanity = FBUtils.getBetween(betweendata[1], "\"vanity\\\":", ",").Replace("\\", "").Replace("\"", ""); ;
											string page = FBUtils.getBetween(betweendata[1], "\"page\\\":", ",").Replace("\\", "").Replace("\"", "");

											try
											{
												FanPageurl_Photos = "https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?__pkg_cohort__=EXP1%3ADEFAULT&ajaxpipe=1&ajaxpipe_token=" + ajaxpipe_token + "&no_script_path=1&data=%7B%22scroll_load%22%3Atrue%2C%22last_fbid%22%3A" + last_fbid + "%2C%22fetch_size%22%3A" + fetch_size + "%2C%22profile_id%22%3A" + profile_id + "%2C%22vanity%22%3A%22" + vanity + "%22%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + page + "%2C%22is_medley_view%22%3Atrue%7D&__user=" + __user + "&__a=1&__dyn=7AmajEyl35wzgDxyG8EigmzGK2WbF3ozzkC-K26m6oKezob4q68K5Uc-dwIxi5fzEvEwy8AxW9gizUyVWz9EpwzxO&__req=jsonp_2&__rev=1906183&__adt=2";
												PhototStreamPage = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl_Photos), "", "");
											}
											catch
											{
												return ProfileId;

											};
										}
										catch
										{
											return ProfileId;

										}
									}
									else
									{
										return ProfileId;
									}
								}

								catch (Exception Ex)
								{
									return ProfileId;
								}

							}

						}





					}
				}
				catch (Exception ex)
				{
					return ProfileId;

				}
				return ProfileId;
			}
			catch 
			{ 
			};
			return ProfileId;

		}



		private List<string> AjaxPosterNew(FacebookUser fbUser, string FanPageurl)
		{
			try
			{


				try
				{
					lstThreadsWallPoster.Add(Thread.CurrentThread);
					lstThreadsWallPoster.Distinct();
					Thread.CurrentThread.IsBackground = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
					GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  StartSendFriendRequest  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
				}



				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string FanPageurl_Photos = "";
				string FanPageurl_PhotosNew = "";
				try
				{


					{
						List<string> lstNormalPost = new List<string>();
						List<string> lstPostWithPhoto = new List<string>();
						string pageId = string.Empty;
						string userID = string.Empty;
						List<string> Idlist = new List<string>();

						string homeapgesrc = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl), "", "");

						userID = GlobusHttpHelper.GetParamValue(homeapgesrc, "user");
						string fanPageSrc = string.Empty;
						fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl), "", "");

						pageId = FBUtils.getBetween(fanPageSrc, "pageID\":", ",").Replace("}]]", "").Replace("\"", "");
						if (!string.IsNullOrEmpty(fanPageSrc))
						{

							#region Commented
							/*
                       


                            string[] pagaDataByHref = Regex.Split(fanPageSrc, "href");
                            foreach (string item in pagaDataByHref)
                            {
                                string temp = FBUtils.getBetween(item, "\"", "\"");
                                if (temp.Contains("photos"))
                                {
                                    lstPostWithPhoto.Add(temp);
                                }
                                if (temp.Contains("posts"))
                                {
                                    lstNormalPost.Add(temp);
                                }
                            }

                          
                        //PAgination Logic
                        string[] scrollingData = Regex.Split(fanPageSrc, "function");
                        string nextPagData = string.Empty;
                        foreach (string scrolling in scrollingData)
                        {
                            if (scrolling.Contains("return new ScrollingPager"))
                            {
                                nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\", string.Empty);
                                break;
                            }
                        }
                        Queue<string> QueueStart = new Queue<string>();
                        QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":" + pageId + ",\"column\":\"main\",\"post_section\":{\"profile_id\":" + pageId + ",\"start\":1420099200,\"end\":1451635199,\"query_type\":8,\"filter\":1,\"filter_after_timestamp\":1420400403},\"section_index\":1,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
                        QueueStart.Enqueue("{\"segment_index\":0,\"page_index\":0,\"page\":" + pageId + ",\"column\":\"main\",\"post_section\":{\"profile_id\":" + pageId + ",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1},\"section_index\":2,\"hidden\":false,\"posts_loaded\":0,\"show_all_posts\":false}");
                        QueueStart.Enqueue("{\"segment_index\":26,\"page_index\":0,\"page\":" + pageId + ",\"column\":\"main\",\"post_section\":{\"profile_id\":" + pageId + ",\"start\":1388563200,\"end\":1420099199,\"query_type\":8,\"filter\":1,\"is_pages_redesign\":true},\"section_index\":2,\"hidden\":false,\"posts_loaded\":26,\"show_all_posts\":false}");
                        //while (true)
                          
                            
                            string nextPageSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/pagelet/generic.php/PagePostsSectionPagelet?data=" + Uri.EscapeDataString(nextPagData) + "&__user=" + userID + "&__a=1&__dyn=7nmajEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxvyUWdDx2ubhHxd5BzEy6Kdy8-&__req=r&__rev=1555029"), "", "");
                            string[] scrollingData1 = Regex.Split(nextPageSource, "function");
                            nextPagData = string.Empty;
                            foreach (string scrolling in scrollingData1)
                            {
                                if (scrolling.Contains("return new ScrollingPager"))
                                {
                                    nextPagData = FBUtils.getBetween(scrolling, "PagePostsSectionPagelet\\\", ", ", null);}})").Replace("\\", string.Empty);
                                    break;
                                }
                            }

                            string[] pagaDataByHref1 = Regex.Split(nextPageSource, "href");
                            foreach (string item in pagaDataByHref1)
                            {
                                string temp = FBUtils.getBetween(item, "\"", "\"");
                                if (temp.Contains("photos"))
                                {
                                    lstPostWithPhoto.Add(temp);
                                }
                                if (temp.Contains("posts"))
                                {
                                    lstNormalPost.Add(temp);
                                }
                            }
                            if (string.IsNullOrEmpty(nextPagData))
                            {
                                if (QueueStart.Count != 0)
                                {
                                    nextPagData = QueueStart.Dequeue();
                                }
                                else
                                {
                                    //	break;
                                }

                            }
                            

                         

                            */
							#endregion




							lstPostWithPhoto = lstPostWithPhoto.Distinct().ToList();
							lstNormalPost = lstNormalPost.Distinct().ToList();

							if (FanPageurl.Contains("?"))
							{
								try
								{
									string[] FanPageurllst = Regex.Split(FanPageurl, "fref");
									FanPageurl = FanPageurllst[0].Replace("?", "");
								}
								catch { };
							}


							if (FanPageurl[FanPageurl.Count() - 1] == '/')
							{
								FanPageurl_Photos = FanPageurl + "photos_stream";
								FanPageurl_PhotosNew = FanPageurl_Photos;

							}
							else
							{
								FanPageurl_Photos = FanPageurl + "/photos_stream";
								FanPageurl_PhotosNew = FanPageurl_Photos;


							}

							string PhototStreamPage = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl_Photos), "", "");

							//string username = FBUtils.getBetween(PhototStreamPage, "content=\"https://www.facebook.com/", "?");
							string username = "";
							try
							{
								username = FBUtils.getBetween(Regex.Split(FanPageurl_Photos, "www.facebook.com")[1], "/", "/");
							}
							catch { };

							string ajaxpipe_token = FBUtils.getBetween(PhototStreamPage, "ajaxpipe_token\":\"", "\"");
							string __user = FBUtils.getBetween(PhototStreamPage, "userID\":\"", "\"");

							string[] pagaDataByHref12 = Regex.Split(PhototStreamPage, "href");
							List<string> lstPostWithPhotoNew = new List<string>();
							foreach (string item in pagaDataByHref12)
							{
								string temp = FBUtils.getBetween(item, "\"", "\"");
								if (temp.Contains("photos"))
								{
									if (item.Contains("/photos/"))
									{
										// lstPostWithPhotoNew.Add(temp);
									}

								}

							}

							int Count = 0;
							int countFoBelloWhileLoop = 0;
							while (true)
							{

								if (countFoBelloWhileLoop > FriendManager.NoofFriendToScrapToAddFriendForFanPage)
								{
									// return ProfileId;

								}
								countFoBelloWhileLoop++;
								try
								{
									string[] pagaDataByHref1 = Regex.Split(PhototStreamPage, "OGAggregationHovercardTarget");
									foreach (string item in pagaDataByHref1)
									{

										string temp = FBUtils.getBetween(item, "fbid=", "&");
										if (!string.IsNullOrEmpty(temp) && (temp.Count() < 30) && !string.IsNullOrEmpty(username))
										{
											string tempNew = "/" + username + "/photos/" + temp;

											lstPostWithPhotoNew.Add(tempNew);
											lstPostWithPhotoNew = lstPostWithPhotoNew.Distinct().ToList();
											AddToLogger_FriendsManager("Add Photo ID : " + tempNew);




											try
											{

												string link = tempNew;

												#region ScrapUserForeachPhoto
												{


													try
													{
														if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
														{
															//  AddToLogger_FriendsManager("All Data Scraped ");
															//  return ProfileId;

														}
														Count = Count + 1;

														temp = temp.Replace("/?type=1", string.Empty);
														string[] spitData = temp.Split('/');
														string postId = string.Empty;
														postId = spitData[spitData.Length - 1];
														if (postId.Contains("&"))
														{
															int i = postId.IndexOf('&');
															int j = postId.Length - 1;
															try
															{
																postId = postId.Remove(postId.IndexOf('&'), ((postId.Length) - postId.IndexOf('&')));
															}
															catch (Exception ex) { };
														}
														string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId), "", "");
														if (string.IsNullOrEmpty(graphResp))
														{
															graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/" + postId), "", "");

															if (graphResp.Contains("actorid\":"))
															{
																try
																{
																	string[] splitpage = Regex.Split(graphResp, "actorid\":");
																	string actorId = FBUtils.getBetween(splitpage[1], "\"", "\"");
																	string finalUrl = "http://www.facebook.com" + "/browse/likes?id=" + postId + "&actorid=" + actorId;

																	string Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl), "", "");

																	if (string.IsNullOrEmpty(Users_Liked_Page))
																	{
																		finalUrl = "https://www.facebook.com/ajax/browser/dialog/likes?actorid=" + actorId + "&id=" + postId + "&__pc=EXP1%3ADEFAULT&__asyncDialog=3&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx97zEWHwKACF3ozBDirWU8popyUWdwIhEngK5Uc-dwFG4K5fzEvEwy8yUnCF1afybDGcCxC2e78lxqEkzo&__req=19&__rev=2007633";
																		Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl), "", "");


																	}




																	string[] Users_Liked_List = Regex.Split(Users_Liked_Page, "class=\"fbProfileBrowserListItem");

																	if (Users_Liked_List.Count() == 1)
																	{
																		Users_Liked_List = Regex.Split(Users_Liked_Page, "fbProfileBrowserListItem");
																	}


																	foreach (string str in Users_Liked_List)
																	{
																		try
																		{
																			if (str.Contains("<!DOCTYPE html>"))
																			{
																				continue;

																			}
																			else
																			{
																				string UserId = FBUtils.getBetween(str, "data-profileid=\"", "\"");
																				if (string.IsNullOrEmpty(UserId))
																				{
																					UserId = FBUtils.getBetween(str, "data-profileid=\\\"", "\\\"");

																				}

																				if (!string.IsNullOrEmpty(UserId))
																				{
																					ProfileId.Add(UserId);
																					AddToLogger_FriendsManager("Add Profile ID : " + UserId);

																					if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																					{
																						// AddToLogger_FriendsManager("Required Data Scraped ");
																						// return ProfileId;

																					}
																				}
																			}
																		}
																		catch { };

																	}



																}
																catch { };

															}

															if (graphResp.Contains("fbPhotoTagListTag tagItem"))
															{

																string[] nooFriendSplit = Regex.Split(graphResp, "fbPhotoTagListTag tagItem");
																//string 
															}

														}
														else
														{
															string[] arr = System.Text.RegularExpressions.Regex.Split(graphResp, "\"created_time\"");
															arr = arr.Skip(0).ToArray();
															foreach (var item_arr in arr)
															{
																try
																{
																	string GetId = FBUtils.getBetween(item_arr, "\"id\":", "\n").Replace("\"", string.Empty).Replace(",", string.Empty);
																	if (!string.IsNullOrEmpty(GetId))
																	{
																		ProfileId.Add(GetId);
																		ProfileId = ProfileId.Distinct().ToList();
																		AddToLogger_FriendsManager("Add Profile ID : " + GetId);

																		if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																		{
																			//  AddToLogger_FriendsManager("Required Data Scraped ");
																			//  return ProfileId;

																		}

																	}

																	AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId + "Fanpage url : " + FanPageurl);
																}
																catch (Exception ex) { };

															}
														}
													}
													catch { };


													#region ScrapeUsers


													try
													{
														string urlName = "https://www.facebook.com" + link;

														fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(urlName), "", "");

														if (fanPageSrc.Contains("content=\"fb://photo/"))  ///ajax/browser/dialog/likes?id
														{

															string idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

															string idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

															string urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

															// string[] LikeUserPageSplit = Regex.Split(fanPageSrc, "ajax/browser/dialog/likes?id");

															//string urlUsers = FBUtils.getBetween(LikeUserPageSplit[1], "href=\"", "\"");
															urlUsers = "http://www.facebook.com" + urlUsers;

															string frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(urlUsers), "", "");


															if (string.IsNullOrEmpty(frienfListPage))
															{
																urlUsers = "https://www.facebook.com/ajax/browser/dialog/likes?actorid=" + idactor + "&id=" + idUser + "&__pc=EXP1%3ADEFAULT&__asyncDialog=3&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx97zEWHwKACF3ozBDirWU8popyUWdwIhEngK5Uc-dwFG4K5fzEvEwy8yUnCF1afybDGcCxC2e78lxqEkzo&__req=19&__rev=2007633";
																frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(urlUsers), "", "");

															}


															try
															{

																string[] Users_Liked_List = Regex.Split(frienfListPage, "class=\"fbProfileBrowserListItem");
																if (Users_Liked_List.Count() == 1)
																{
																	Users_Liked_List = Regex.Split(frienfListPage, "fbProfileBrowserListItem");

																}


																//  Users_Liked_List = Regex.Split(frienfListPage, "https://www.facebook.com/profile.php?");
																foreach (string str in Users_Liked_List)
																{
																	try
																	{
																		if (str.Contains("<!DOCTYPE html>"))
																		{
																			continue;

																		}
																		else
																		{
																			string UserId = "";
																			if (str.Contains("profileid=\\\""))
																			{
																				UserId = FBUtils.getBetween(str, "profileid=\\\"", "\\\"");
																			}
																			else
																			{

																				UserId = FBUtils.getBetween(str, "id=", "&");
																			}
																			if (!string.IsNullOrEmpty(UserId) && UserId.Count() < 30)
																			{
																				ProfileId.Add(UserId);
																				ProfileId = ProfileId.Distinct().ToList();
																				AddToLogger_FriendsManager("Add Profile ID : " + UserId);
																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					// AddToLogger_FriendsManager("Required Data Scraped ");
																					// return ProfileId;

																				}
																			}
																			else if (UserId.Count() > 30)
																			{
																				UserId = FBUtils.getBetween(str, "id=", "\"");
																				ProfileId.Add(UserId);
																				ProfileId = ProfileId.Distinct().ToList();
																				AddToLogger_FriendsManager("Add Profile ID : " + UserId);
																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					//  AddToLogger_FriendsManager("Required Data Scraped ");
																					//  return ProfileId;

																				}

																			}
																		}
																	}
																	catch { };

																}


																try
																{

																	if (fanPageSrc.Contains("content=\"fb://photo/"))
																	{


																		idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

																		idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

																		urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

																		urlUsers = "http://www.facebook.com" + urlUsers;




																		//  string frienfListPageUrl = FBUtils.getBetween(frienfListPage, "/ajax/browser/dialog/graph/?q=", "\"");  //\/ajax\/browser\/list\/graph\/?q=

																		string[] frienfListPageUrllst = Regex.Split(urlUsers, "id=");
																		//   string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1" + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";

																		string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/likes?id=" + idUser + "&actorid=" + idactor + "&__asyncDialog=1&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWHwKACF3ozBDirWU8popyUW3F6xybxu3fzob8kxjUW4oSy28yiq5UB1afybDGcCK5o-4Usw&__req=15&__rev=1908382";
																		// https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=100&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=i&__rev=1909862 HTTP/1.1
																		//https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=200&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862 HTTP/1.

																		int totalNoOfUsersScraped = 0;
																		int CountForPagination = 1;
																		int CountOfIterationHere = 0;
																		int countFoBelloWhileLoop2 = 0;
																		while (true)
																		{

																			if (countFoBelloWhileLoop2 > FriendManager.NoofFriendToScrapToAddFriendForFanPage)
																			{
																				break;

																			}
																			countFoBelloWhileLoop2++;

																			try
																			{

																				if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																				{
																					// AddToLogger_FriendsManager("All Data Scraped ");
																					// return ProfileId;


																				}
																				totalNoOfUsersScraped = ProfileId.Count();
																				frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl), "", "");
																				if (!string.IsNullOrEmpty(frienfListPage))
																				{
																					string[] UsersData = Regex.Split(frienfListPage, "user.php");
																					List<string> UsersDataList = UsersData.ToList();
																					UsersDataList.RemoveAt(0);
																					int ProfileIdCountBefore = ProfileId.Count();
																					foreach (string itemNew in UsersDataList)
																					{
																						try
																						{
																							string userid = FBUtils.getBetween(itemNew, "id=", "&");
																							if (!string.IsNullOrEmpty(userid))
																							{
																								ProfileId.Add(userid);
																								AddToLogger_FriendsManager("Add Profile ID : " + userid);


																								if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 20) <= ProfileId.Count())
																								{
																									//   AddToLogger_FriendsManager("Required Data Scraped ");
																									//  return ProfileId;

																								}

																							}

																						}
																						catch { };
																					}
																					int ProfileIdCountAfter = ProfileId.Count();

																					if (!frienfListPage.Contains("See More"))
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						break;
																					}
																					else if (ProfileIdCountAfter == ProfileIdCountBefore)
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						break;
																					}
																					else
																					{

																						frienfListPageUrl = "https://www.facebook.com/ajax/browser/list/likes/?id=" + idUser + "&actorid=" + idactor + "&beforetime=0&aftertime=0&start=" + CountForPagination * 100 + "&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862";
																						CountForPagination++;
																					}

																					if (ProfileId.Count() == totalNoOfUsersScraped)
																					{
																						// AddToLogger_FriendsManager("All Data Scraped ");
																						ProfileId = ProfileId.Distinct().ToList();
																						break;
																					}

																				}
																				else
																				{
																					break;
																				}
																			}
																			catch (Exception Ex)
																			{ }
																		}

																	}
																}
																catch { };
															}
															catch { };
														}

													}
													catch { };

													#endregion

												}

												#endregion



											}
											catch { };

										}

									}

									if (PhototStreamPage.Contains("TimelinePhotosStreamPagelet"))
									{
										try
										{
											string[] betweendata = Regex.Split(PhototStreamPage, "TimelinePhotosStreamPagelet");

											string last_fbid = FBUtils.getBetween(betweendata[1], "\"last_fbid\\\":", ",");
											string fetch_size = FBUtils.getBetween(betweendata[1], "\"fetch_size\\\":", ",");
											string profile_id = FBUtils.getBetween(betweendata[1], "\"profile_id\\\":", ",");
											string vanity = FBUtils.getBetween(betweendata[1], "\"vanity\\\":", ",").Replace("\\", "").Replace("\"", ""); ;
											string page = FBUtils.getBetween(betweendata[1], "\"page\\\":", ",").Replace("\\", "").Replace("\"", "");

											try
											{
												FanPageurl_Photos = "https://www.facebook.com/ajax/pagelet/generic.php/TimelinePhotosStreamPagelet?__pkg_cohort__=EXP1%3ADEFAULT&ajaxpipe=1&ajaxpipe_token=" + ajaxpipe_token + "&no_script_path=1&data=%7B%22scroll_load%22%3Atrue%2C%22last_fbid%22%3A" + last_fbid + "%2C%22fetch_size%22%3A" + fetch_size + "%2C%22profile_id%22%3A" + profile_id + "%2C%22vanity%22%3A%22" + vanity + "%22%2C%22sk%22%3A%22photos_stream%22%2C%22tab_key%22%3A%22photos_stream%22%2C%22page%22%3A" + page + "%2C%22is_medley_view%22%3Atrue%7D&__user=" + __user + "&__a=1&__dyn=7AmajEyl35wzgDxyG8EigmzGK2WbF3ozzkC-K26m6oKezob4q68K5Uc-dwIxi5fzEvEwy8AxW9gizUyVWz9EpwzxO&__req=jsonp_2&__rev=1906183&__adt=2";
												PhototStreamPage = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl_Photos), "", "");
											}
											catch
											{
												return ProfileId;

											};
										}
										catch
										{
											return ProfileId;

										}
									}
									else
									{
										return ProfileId;
									}
								}

								catch (Exception Ex)
								{
									return ProfileId;
								}

							}
							#region Commented
							/*
                            #region ScrapUserForeachPhoto
                            foreach (string link in lstPostWithPhotoNew)
                            {


                                try
                                {
                                    if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 200) <= ProfileId.Count())
                                    {
                                        AddToLogger_FriendsManager("All Data Scraped ");
                                        return ProfileId;

                                    }
                                    Count = Count + 1;

                                    string temp = link;
                                    temp = temp.Replace("/?type=1", string.Empty);
                                    string[] spitData = temp.Split('/');
                                    string postId = string.Empty;
                                    postId = spitData[spitData.Length - 1];
                                    if (postId.Contains("&"))
                                    {
                                        int i = postId.IndexOf('&');
                                        int j = postId.Length - 1;
                                        try
                                        {
                                            postId = postId.Remove(postId.IndexOf('&'), ((postId.Length) - postId.IndexOf('&')));
                                        }
                                        catch (Exception ex) { };
                                    }
                                    string graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://graph.facebook.com/" + postId), "", "");
                                    if (string.IsNullOrEmpty(graphResp))
                                    {
                                        graphResp = HttpHelper.getHtmlfromUrl(new Uri("http://www.facebook.com/" + postId), "", "");

                                        if (graphResp.Contains("actorid\":"))
                                        {
                                            try
                                            {
                                                string[] splitpage = Regex.Split(graphResp, "actorid\":");
                                                string actorId = FBUtils.getBetween(splitpage[1], "\"", "\"");
                                                string finalUrl = "http://www.facebook.com" + "/browse/likes?id=" + postId + "&actorid=" + actorId;

                                                string Users_Liked_Page = HttpHelper.getHtmlfromUrl(new Uri(finalUrl), "", "");

                                                string[] Users_Liked_List = Regex.Split(Users_Liked_Page, "class=\"fbProfileBrowserListItem");
                                                foreach (string str in Users_Liked_List)
                                                {
                                                    try
                                                    {
                                                        if (str.Contains("<!DOCTYPE html>"))
                                                        {
                                                            continue;

                                                        }
                                                        else
                                                        {
                                                            string UserId = FBUtils.getBetween(str, "data-profileid=\"", "\"");
                                                            if (!string.IsNullOrEmpty(UserId))
                                                            {
                                                                ProfileId.Add(UserId);
                                                                AddToLogger_FriendsManager("Add Profile ID : " + UserId);
                                                            }
                                                        }
                                                    }
                                                    catch { };

                                                }



                                            }
                                            catch { };

                                        }

                                        if (graphResp.Contains("fbPhotoTagListTag tagItem"))
                                        {

                                            string[] nooFriendSplit = Regex.Split(graphResp, "fbPhotoTagListTag tagItem");
                                            //string 
                                        }

                                    }
                                    else
                                    {
                                        string[] arr = System.Text.RegularExpressions.Regex.Split(graphResp, "\"created_time\"");
                                        arr = arr.Skip(0).ToArray();
                                        foreach (var item_arr in arr)
                                        {
                                            try
                                            {
                                                string GetId = FBUtils.getBetween(item_arr, "\"id\":", "\n").Replace("\"", string.Empty).Replace(",", string.Empty);
                                                if (!string.IsNullOrEmpty(GetId))
                                                {
                                                    ProfileId.Add(GetId);
                                                    ProfileId = ProfileId.Distinct().ToList();
                                                    AddToLogger_FriendsManager("Add Profile ID : " + GetId);
                                                }

                                                AddToLogger_FriendsManager("Find the Active Profile ID : " + GetId + "Fanpage url : " + FanPageurl);
                                            }
                                            catch (Exception ex) { };

                                        }
                                    }
                                }
                                catch { };


                                #region ScrapeUsers


                                try
                                {
                                    string urlName = "https://www.facebook.com" + link;

                                    fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(urlName), "", "");

                                    if (fanPageSrc.Contains("content=\"fb://photo/"))  ///ajax/browser/dialog/likes?id
                                    {

                                        string idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

                                        string idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

                                        string urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

                                        // string[] LikeUserPageSplit = Regex.Split(fanPageSrc, "ajax/browser/dialog/likes?id");

                                        //string urlUsers = FBUtils.getBetween(LikeUserPageSplit[1], "href=\"", "\"");
                                        urlUsers = "http://www.facebook.com" + urlUsers;

                                        string frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(urlUsers), "", "");


                                        try
                                        {

                                            string[] Users_Liked_List = Regex.Split(frienfListPage, "class=\"fbProfileBrowserListItem");
                                            Users_Liked_List = Regex.Split(frienfListPage, "https://www.facebook.com/profile.php?");
                                            foreach (string str in Users_Liked_List)
                                            {
                                                try
                                                {
                                                    if (str.Contains("<!DOCTYPE html>"))
                                                    {
                                                        continue;

                                                    }
                                                    else
                                                    {
                                                        string UserId = FBUtils.getBetween(str, "id=", "&");
                                                        if (!string.IsNullOrEmpty(UserId) && UserId.Count() < 30)
                                                        {
                                                            ProfileId.Add(UserId);
                                                            ProfileId = ProfileId.Distinct().ToList();
                                                            AddToLogger_FriendsManager("Add Profile ID : " + UserId);
                                                        }
                                                        else if (UserId.Count() > 30)
                                                        {
                                                            UserId = FBUtils.getBetween(str, "id=", "\"");
                                                            ProfileId.Add(UserId);
                                                            ProfileId = ProfileId.Distinct().ToList();
                                                            AddToLogger_FriendsManager("Add Profile ID : " + UserId);

                                                        }
                                                    }
                                                }
                                                catch { };

                                            }


                                            try
                                            {
                                                if (fanPageSrc.Contains("content=\"fb://photo/"))
                                                {


                                                    idUser = FBUtils.getBetween(fanPageSrc, "content=\"fb://photo/", "\"");

                                                    idactor = FBUtils.getBetween(fanPageSrc, "actorid\":\"", "\"");

                                                    urlUsers = "/browse/likes?id=" + idUser + "&actorid=" + idactor;

                                                    urlUsers = "http://www.facebook.com" + urlUsers;




                                                    //  string frienfListPageUrl = FBUtils.getBetween(frienfListPage, "/ajax/browser/dialog/graph/?q=", "\"");  //\/ajax\/browser\/list\/graph\/?q=

                                                    string[] frienfListPageUrllst = Regex.Split(urlUsers, "id=");
                                                    //   string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1" + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";

                                                    string frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/likes?id=" + idUser + "&actorid=" + idactor + "&__asyncDialog=1&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWHwKACF3ozBDirWU8popyUW3F6xybxu3fzob8kxjUW4oSy28yiq5UB1afybDGcCK5o-4Usw&__req=15&__rev=1908382";
                                                    // https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=100&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=i&__rev=1909862 HTTP/1.1
                                                    //https://www.facebook.com/ajax/browser/list/likes/?id=1036676109689921&actorid=344128252278047&beforetime=0&aftertime=0&start=200&__user=100004306477265&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862 HTTP/1.

                                                    int totalNoOfUsersScraped = 0;
                                                    int CountForPagination = 1;
                                                    int CountOfIterationHere = 0;

                                                    while (true)
                                                    {
                                                        if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 200) <= ProfileId.Count())
                                                        {
                                                            AddToLogger_FriendsManager("All Data Scraped ");
                                                            return ProfileId;

                                                        }
                                                        totalNoOfUsersScraped = ProfileId.Count();
                                                        frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl), "", "");
                                                        if (!string.IsNullOrEmpty(frienfListPage))
                                                        {
                                                            string[] UsersData = Regex.Split(frienfListPage, "user.php");
                                                            List<string> UsersDataList = UsersData.ToList();
                                                            UsersDataList.RemoveAt(0);
                                                            int ProfileIdCountBefore = ProfileId.Count();
                                                            foreach (string item in UsersDataList)
                                                            {
                                                                try
                                                                {
                                                                    string userid = FBUtils.getBetween(item, "id=", "&");
                                                                    if (!string.IsNullOrEmpty(userid))
                                                                    {
                                                                        ProfileId.Add(userid);
                                                                        ProfileId = ProfileId.Distinct().ToList();
                                                                        AddToLogger_FriendsManager("Add Profile ID : " + userid);

                                                                    }

                                                                }
                                                                catch { };
                                                            }
                                                            int ProfileIdCountAfter = ProfileId.Count();

                                                            if (!frienfListPage.Contains("See More"))
                                                            {
                                                                AddToLogger_FriendsManager("All Data Scraped ");
                                                                return ProfileId;
                                                            }
                                                            else if (ProfileIdCountAfter == ProfileIdCountBefore)
                                                            {
                                                                AddToLogger_FriendsManager("All Data Scraped ");
                                                                return ProfileId;
                                                            }
                                                            else
                                                            {
                                                                frienfListPageUrl = "https://www.facebook.com/ajax/browser/list/likes/?id=" + idUser + "&actorid=" + idactor + "&beforetime=0&aftertime=0&start=" + CountForPagination * 100 + "&__user=" + userID + "&__a=1&__dyn=7AmajEyl2qm9ongDxiWEyx9CzEWq2WiqAdy9VQC-K26m6oKewWhEoyUnwPUS2O58kUgx6dEwy8ACxu9gizUyVWz9Hxmfxe78&__req=n&__rev=1909862";
                                                                CountForPagination++;
                                                            }

                                                            if (ProfileId.Count() == totalNoOfUsersScraped)
                                                            {
                                                                AddToLogger_FriendsManager("All Data Scraped ");
                                                                return ProfileId;
                                                            }



                                                        }
                                                    }


                                                }
                                            }
                                            catch { };
                                        }
                                        catch { };

                                    }


                                }
                                catch { };

                                #endregion


                            }


                            #endregion
                            */
							#endregion
						}


						#region Commented

						//fanPageSrc = HttpHelper.getHtmlfromUrl(new Uri(FanPageurl_PhotosNew), "", "");


						/*
                        
                        if (fanPageSrc.Contains("people like this topic"))
                        {
                            if (fanPageSrc.Contains("/browse/graph/?q="))
                            {
                                string klsfsldfk = "/browse/graph/";
                                string[] pagesplit = Regex.Split(fanPageSrc, klsfsldfk);
                                if (pagesplit.Count() > 1)
                                {
                                    string frienfListPageUrl = FBUtils.getBetween(pagesplit[1], "=", "\"");
                                    frienfListPageUrl = "https://www.facebook.com/browse/graph/?q=" + frienfListPageUrl;
                                    string frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl), "", "");

                                    //while(true)
                                    {

                                        try
                                        {

                                            string[] Users_Liked_List = Regex.Split(frienfListPage, "class=\"fbProfileBrowserListItem");
                                            foreach (string str in Users_Liked_List)
                                            {
                                                try
                                                {
                                                    if (str.Contains("<!DOCTYPE html>"))
                                                    {
                                                        continue;

                                                    }
                                                    else
                                                    {
                                                        string UserId = FBUtils.getBetween(str, "id=", "&");
                                                        if (!string.IsNullOrEmpty(UserId))
                                                        {
                                                            ProfileId.Add(UserId);
                                                        }
                                                    }
                                                }
                                                catch { };

                                            }


                                            try
                                            {
                                                if (fanPageSrc.Contains("/ajax/browser/dialog/graph/?q="))
                                                {

                                                    frienfListPageUrl = FBUtils.getBetween(fanPageSrc, "/ajax/browser/dialog/graph/?q=", "\"");  //\/ajax\/browser\/list\/graph\/?q=


                                                    frienfListPageUrl = "https://www.facebook.com/ajax/browser/dialog/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1" + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";
                                                    int totalNoOfUsersScraped = 0;
                                                    while (true)
                                                    {
                                                        if ((FriendManager.NoofFriendToScrapToAddFriendForFanPage + 200) <= ProfileId.Count())
                                                        {
                                                            AddToLogger_FriendsManager("All Data Scraped ");
                                                            break;

                                                        }
                                                        totalNoOfUsersScraped = ProfileId.Count();
                                                        frienfListPage = HttpHelper.getHtmlfromUrl(new Uri(frienfListPageUrl), "", "");
                                                        if (!string.IsNullOrEmpty(frienfListPage))
                                                        {
                                                            string[] UsersData = Regex.Split(frienfListPage, "user.php");
                                                            List<string> UsersDataList = UsersData.ToList();
                                                            UsersDataList.RemoveAt(0);
                                                            foreach (string item in UsersDataList)
                                                            {
                                                                try
                                                                {
                                                                    string userid = FBUtils.getBetween(item, "id=", "&");
                                                                    if (!string.IsNullOrEmpty(userid))
                                                                    {
                                                                        ProfileId.Add(userid);
                                                                    }

                                                                }
                                                                catch { };
                                                            }

                                                            if (frienfListPage.Contains("\\/ajax\\/browser\\/list\\/graph\\/?q="))
                                                            {
                                                                frienfListPageUrl = FBUtils.getBetween(frienfListPage, "\\/ajax\\/browser\\/list\\/graph\\/?q=", "\"");
                                                                frienfListPageUrl = frienfListPageUrl.Replace("\\", "");
                                                                frienfListPageUrl = "https://www.facebook.com/ajax/browser/list/graph/?q=" + frienfListPageUrl + "&__user=" + userID + "&__a=1" + "&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdy8Z9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=14&__rev=1819463";

                                                            }
                                                            else
                                                            {
                                                                AddToLogger_FriendsManager("All Data Scraped ");
                                                                break;
                                                            }
                                                            if (ProfileId.Count() == totalNoOfUsersScraped)
                                                            {
                                                                AddToLogger_FriendsManager("All Data Scraped ");
                                                                break;
                                                            }



                                                        }
                                                    }


                                                }
                                            }
                                            catch { };
                                        }
                                        catch { };
                                    }
                                }


                            }
                        } 

                        */


						#endregion


					}
				}
				catch (Exception ex)
				{
					return ProfileId;

				}
				return ProfileId;
			}
			catch 
			{ 
			};
			return ProfileId;

		}
























		public static bool checkRequestWithProfileUrl=false;
		public void StartActionFriendsManager(ref FacebookUser fbUser)
		{
			if (checkRequestWithProfileUrl)
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");
				string UserId = string.Empty;
				int countFriendRequestsSent = 0;
				foreach (var FriendRequestLink in FBUtils.LoadProfileUrls) 
				{



					UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
					if (string.IsNullOrEmpty(UserId))
					{
						UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
					}

					if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
					{
						AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


						return;
					}
					AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);

					bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);




					if (requeststatus)
					{
						countFriendRequestsSent++;
						//countFriendRequestsSentAllKeyWord++;
						//counterforblockedFriendrequest = 1;
						AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);


					}
					else
					{
						//counterforblockedFriendrequest++;
						//if (counterforblockedFriendrequest == 3)
						//{
						//    break;
						//}
					}

				}
			} 
			else
			{
				try 
				{
					SendFriendRequestViaKeywords (ref fbUser);
				} 
				catch (Exception ex)
				{
					Console.WriteLine ("Error : " + ex.StackTrace);
				}
			}
		}



		public void SendFriendRequestViaKeywords_OLd(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
				int countFriendRequestsSentAllKeyWord = 0;



				string UserId = string.Empty;

				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				string keyword = string.Empty;

				if (!string.IsNullOrEmpty(Keywords))
				{
					keyword = FriendManager.Keywords;
					lstRequestFriendsKeywords.Add(keyword);
				}
				else if (lstRequestFriendsKeywords.Count > 0)
				{

					try
					{
						keyword = lstRequestFriendsKeywords[GlobusHttpHelper.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
					}
					catch(Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}


				lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();

				foreach (var item_keyword in lstRequestFriendsKeywords)
				{
					string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + item_keyword + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";

					string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");
					List<string> list = new List<string>();
					#region for find friend Reqest Link
					list.Clear();
					List<string> lstLinkData = new List<string>();
					lstLinkData.Clear();
					string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");
					string profileID = string.Empty;
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (itemurl.Contains("is_friend&quot;:false"))
								{
									lstLinkData.Add(itemurl);
									try
									{
										if (itemurl.Contains("&quot;"))
										{
											try
											{
												profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
												profileID = profileID.Replace(",", "");
											}
											catch (Exception ex)
											{
												Console.WriteLine("Error : " + ex.StackTrace);
											}
										}
										else
										{
											profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
										}

										string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
										list.Add(profileURL);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
					#endregion

					List<string> FriendLink = list.Distinct().ToList();


			     	AddToLogger_FriendsManager(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

					int countFriendRequestsSent = 0;
					int counterforblockedFriendrequest = 0;
					foreach (string FriendRequestLink in FriendLink)
					{
						try
						{
							if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
							{

								//return;
							}
							if (countFriendRequestsSent >= NoOfFriendsRequest)
							{
								break;
							}

							AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
							bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);

							if (requeststatus)
							{
								countFriendRequestsSent++;
								countFriendRequestsSentAllKeyWord++;
								counterforblockedFriendrequest = 1;
								AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);


							}
							else
							{
								//counterforblockedFriendrequest++;
								//if (counterforblockedFriendrequest == 3)
								//{
								//    break;
								//}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					} 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}




		public void SendFriendRequestViaKeywords_old_old_old_old(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
				int countFriendRequestsSentAllKeyWord = 0;



				string UserId = string.Empty;

				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				string keyword = string.Empty;

				if (!string.IsNullOrEmpty(Keywords))
				{
					keyword = FriendManager.Keywords;
					lstRequestFriendsKeywords.Add(keyword);
				}
				else if (lstRequestFriendsKeywords.Count > 0)
				{

					try
					{
						keyword = lstRequestFriendsKeywords[GlobusHttpHelper.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
					}
					catch(Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}


				lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();
				if (FBUtils.CheckUploadProfileUrls) 
				{
					lstRequestFriendsKeywords.Add("love");
				}
				foreach (var item_keyword in lstRequestFriendsKeywords)
				{
					List<string> list = new List<string>();
					List<string> FriendLink =new List<string>();
					if (!FBUtils.CheckUploadProfileUrls) 
					{


						string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + item_keyword + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";




						string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");



						#region for find friend Reqest Link
						list.Clear();
						List<string> lstLinkData = new List<string>();
						lstLinkData.Clear();



						if(resGetRequestFriends.Contains("href="))
						{
							string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");
							string profileID = string.Empty;
							foreach (string itemurl in Linklist)
							{
								try
								{
									if (!itemurl.Contains("<!DOCTYPE html"))
									{
										if (itemurl.Contains("is_friend&quot;:false"))
										{
											lstLinkData.Add(itemurl);
											try
											{
												if (itemurl.Contains("&quot;"))
												{
													try
													{
														profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
														profileID = profileID.Replace(",", "");
													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
														GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);

													}
												}
												else
												{
													profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
												}

												string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
												list.Add(profileURL);
											}
											catch (Exception ex)
											{
												Console.WriteLine("Error : " + ex.StackTrace);
												GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
											}
										}
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
						}


						else
						{

							try
							{
								searchURL = "https://www.facebook.com/search/results/more/?q="+item_keyword+"&offset=100&type=users&init=quick&sid=0f97d94583873abc7f90fcc57e5a7799&tas=0.5925079600419849&ents=1550194641%2C100006787369032%2C537061133%2C100004130585779%2C100006428708468%2C100003035998791%2C100003628923293%2C426409467370453%2C580376905363617%2C344128252278047%2C259937417472502%2C16889167127%2C1538972489718839%2C647024468742026%2C496761667097140%2C135399539827629%2C113060895374516%2C107624392600504%2C8666024051%2C112377502107994&__user="+ UserId +"&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=j&__rev=1813502";
								resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");
								string[] profileIdList = Regex.Split(resGetRequestFriends,"profileid=");
								try
								{
									profileIdList = (string[])profileIdList.Skip(0);
								}
								catch{};

								foreach(string item in profileIdList)
								{
									string profileId = FBUtils.getBetween(item,"\"","\"");
									if(string.IsNullOrEmpty(profileId) || profileId.Contains("__ar"))
									{
										continue;
									}
									else if(profileId.Contains("\\"))
									{
										profileId = profileId.Replace("\\","");
									}

									string profileURL = FBGlobals.Instance.fbProfileUrl + profileId;
									list.Add(profileURL);

								}
							}
							catch{};

						}



						FriendLink = list.Distinct().ToList();
					}
					else
					{
						FriendLink=FBUtils.LoadProfileUrls;
					}
					#endregion




					AddToLogger_FriendsManager(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

					int countFriendRequestsSent = 0;
					int counterforblockedFriendrequest = 0;
					foreach (string FriendRequestLink in FriendLink)
					{
						try
						{
							if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
							{

								//return;
							}
							if (countFriendRequestsSent >= NoOfFriendsRequest)
							{
								break;
							}

							AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
							bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);

							if (requeststatus)
							{
								countFriendRequestsSent++;
								countFriendRequestsSentAllKeyWord++;
								counterforblockedFriendrequest = 1;
								AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);


							}
							else
							{
								//counterforblockedFriendrequest++;
								//if (counterforblockedFriendrequest == 3)
								//{
								//    break;
								//}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					} 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}
















		public void SendFriendRequestViaKeywords_3_Oct(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
				int countFriendRequestsSentAllKeyWord = 0;



				string UserId = string.Empty;

				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				string keyword = string.Empty;

				if (!string.IsNullOrEmpty(Keywords))
				{
					keyword = FriendManager.Keywords;
					lstRequestFriendsKeywords.Add(keyword);
				}
				else if (lstRequestFriendsKeywords.Count > 0)
				{

					try
					{
						keyword = lstRequestFriendsKeywords[GlobusHttpHelper.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
					}
					catch(Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}


				lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();
				if (FBUtils.CheckUploadProfileUrls) 
				{
					lstRequestFriendsKeywords.Add("love");
				}
				foreach (var item_keyword in lstRequestFriendsKeywords)
				{
					List<string> list = new List<string>();
					List<string> FriendLink =new List<string>();
					if (!FBUtils.CheckUploadProfileUrls) 
					{


						//string searchURL = FBGlobals.Instance.urlGetSearchFriendsFriendManager + item_keyword + "&type=users&__a=1&__user=" + UserId + "";//"https://www.facebook.com/search/results.php?q=" + Location + "&type=users&init=quick";

						string searchURL = "https://www.facebook.com/search/results/?q="+item_keyword+"&type=users";


						string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");






						#region for find friend Reqest Link
						list.Clear();
						List<string> lstLinkData = new List<string>();
						lstLinkData.Clear();

						/*

						if(resGetRequestFriends.Contains("href="))
						{
					string[] Linklist = System.Text.RegularExpressions.Regex.Split(resGetRequestFriends, "href=");
					string profileID = string.Empty;
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (itemurl.Contains("is_friend&quot;:false"))
								{
									lstLinkData.Add(itemurl);
									try
									{
										if (itemurl.Contains("&quot;"))
										{
											try
											{
												profileID = GlobusHttpHelper.ParseEncodedJson(itemurl, "id");
												profileID = profileID.Replace(",", "");
											}
											catch (Exception ex)
											{
												Console.WriteLine("Error : " + ex.StackTrace);
													GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
							
											}
										}
										else
										{
											profileID = GlobusHttpHelper.ParseJson(itemurl, "id");
										}

										string profileURL = FBGlobals.Instance.fbProfileUrl + profileID;
										list.Add(profileURL);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
						}

*/
						if(true)
						{



							/*
							try
							{
							searchURL = "https://www.facebook.com/search/results/more/?q="+item_keyword+"&offset=100&type=users&init=quick&sid=0f97d94583873abc7f90fcc57e5a7799&tas=0.5925079600419849&ents=1550194641%2C100006787369032%2C537061133%2C100004130585779%2C100006428708468%2C100003035998791%2C100003628923293%2C426409467370453%2C580376905363617%2C344128252278047%2C259937417472502%2C16889167127%2C1538972489718839%2C647024468742026%2C496761667097140%2C135399539827629%2C113060895374516%2C107624392600504%2C8666024051%2C112377502107994&__user="+ UserId +"&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=j&__rev=1813502";
							resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");
							string[] profileIdList = Regex.Split(resGetRequestFriends,"profileid=");
							try
							{
							profileIdList = (string[])profileIdList.Skip(0);
							}
							catch{};

							foreach(string item in profileIdList)
							{
								string profileId = FBUtils.getBetween(item,"\"","\"");
									if(string.IsNullOrEmpty(profileId) || profileId.Contains("__ar"))
								{
									continue;
								}
								else if(profileId.Contains("\\"))
								//{
								//	profileId = profileId.Replace("\\","");
								//}

								//string profileURL = FBGlobals.Instance.fbProfileUrl + profileId;
								/list.Add(profileURL);

							//}
							//}
							//catch{};


*/



							try
							{
								string[] PageSplit = Regex.Split(resGetRequestFriends,"<a href=\"/users");

								if(PageSplit.Count()!=1)
								{
									List<string> PageSplitList = PageSplit.ToList();
									PageSplitList.RemoveAt(0);
									foreach(string item in PageSplitList)
									{
										if(item.Contains("<!DOCTYPE html>"))
										{
											continue;
										}

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}



										string UserIdScraped = FBUtils.getBetween(item,"/","/");
										if(!string.IsNullOrEmpty(UserIdScraped))
										{
											list.Add("https://www.facebook.com/" + UserIdScraped);
											AddToLogger_FriendsManager("Added Friend Id : " +  UserIdScraped);	

										}
									}
								}
								else
								{

									PageSplit = Regex.Split(resGetRequestFriends,"data-profileid=");

									List<string> PageSplitList = PageSplit.ToList();
									PageSplitList.RemoveAt(0);
									foreach(string item in PageSplitList)
									{
										if(item.Contains("<!DOCTYPE html>"))
										{
											continue;
										}

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}



										string UserIdScraped = FBUtils.getBetween(item,"\"","\"");
										if(!string.IsNullOrEmpty(UserIdScraped))
										{
											list.Add("https://www.facebook.com/" + UserIdScraped);
											AddToLogger_FriendsManager("Added Friend Id : " +  UserIdScraped);	
										}
									}





								}


							}
							catch{};

							int countForlistIteminPrvious =0;
							string ajaxRequestURL = "";
							int countFormaximumScrap = 0;
							while(true)
							{
								countFormaximumScrap++;
								if ((FriendManager.noOfFriendsToAdd +20) <= countFormaximumScrap)
								{
									AddToLogger_FriendsManager("No. of Friends Found To Scrape : " + list.Count());
									break;
								}


								list = list.Distinct().ToList();
								countForlistIteminPrvious = list.Count();

								try
								{
									list = list.Distinct().ToList();
									if((FriendManager.noOfFriendsToAdd+20)<= list.Count())
									{
										AddToLogger_FriendsManager("No of Friend Found To Scrap : " +  list.Count());
										break;
									}

									string[] PageSplit = Regex.Split(resGetRequestFriends,"rel=\"ajaxify\"");  //rel=\"ajaxify\"

									if(PageSplit.Count()==1)
									{
										string splitIt = "&amp;offset=";
										PageSplit = Regex.Split(resGetRequestFriends,splitIt);  //rel=\"ajaxify\"

										if(PageSplit.Count()==1)
										{
											AddToLogger_FriendsManager("All User Id Scraped ");
											break;
										}
										if(PageSplit.Count()>1)
										{

											PageSplit[1]  =  "/search/results/more/?q=" + item_keyword + "&amp;offset=" + PageSplit[1] ;
											ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"","\\\"");

										}




									}
									else
									{

										ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"href=\"","\"");
									}

									ajaxRequestURL = ajaxRequestURL.Replace("amp;","").Replace("type=all","type=groups").Replace("\\","%2C").Replace("u00252C","");

									ajaxRequestURL = "https://www.facebook.com" +  ajaxRequestURL + "&__user=" + UserId + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" +  FBUtils.getBetween(PageSplit[1],"revision\":",",");

									resGetRequestFriends =  httpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
									string allListGroup  = FBUtils.getBetween(resGetRequestFriends,"&quot;ents&quot;:&quot;","&quot");
									string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
									foreach(string item in Linklist)
									{

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}


										try
										{
											if(!string.IsNullOrEmpty(item))
											{
												list.Add("https://www.facebook.com/"+item);
												AddToLogger_FriendsManager("Added Friend Id : " +  item);	

											}
										}
										catch{};

									}

									if(countForlistIteminPrvious==list.Count())
									{
										AddToLogger_FriendsManager("No of Friends Found To Scrape  : " +  list.Count());
										break;
									}
									list = list.Distinct().ToList();

								}
								catch{};
							}



						}



						FriendLink = list.Distinct().ToList();
					}
					else
					{
						FriendLink=FBUtils.LoadProfileUrls;
					}
					#endregion




					AddToLogger_FriendsManager(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

					int countFriendRequestsSent = 0;
					int counterforblockedFriendrequest = 0;
					foreach (string FriendRequestLink in FriendLink)
					{
						try
						{
							if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
							{

								//return;
							}
							if (countFriendRequestsSent >= NoOfFriendsRequest)
							{
								break;
							}

							AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
							bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);

							if (requeststatus)
							{
								countFriendRequestsSent++;
								countFriendRequestsSentAllKeyWord++;
								counterforblockedFriendrequest = 1;
								AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);


							}
							else
							{
								//counterforblockedFriendrequest++;
								//if (counterforblockedFriendrequest == 3)
								//{
								//    break;
								//}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					} 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}















		public void SendFriendRequestViaKeywords_old_3_Oct(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;
				int countFriendRequestsSentAllKeyWord = 0;



				string UserId = string.Empty;

				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				string keyword = string.Empty;

				if (!string.IsNullOrEmpty(Keywords))
				{
					keyword = FriendManager.Keywords;
					lstRequestFriendsKeywords.Add(keyword);
				}
				else if (lstRequestFriendsKeywords.Count > 0)
				{

					try
					{
						keyword = lstRequestFriendsKeywords[GlobusHttpHelper.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
					}
					catch(Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}


				lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();
				if (FBUtils.CheckUploadProfileUrls) 
				{
					lstRequestFriendsKeywords.Add("love");
				}
				foreach (var item_keyword in lstRequestFriendsKeywords)
				{
					List<string> list = new List<string>();
					List<string> FriendLink =new List<string>();
					if (!FBUtils.CheckUploadProfileUrls) 
					{


						string searchURL = "https://www.facebook.com/search/results/?q="+item_keyword+"&type=users";


						string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL),"","");






						#region for find friend Reqest Link
						list.Clear();
						List<string> lstLinkData = new List<string>();
						lstLinkData.Clear();


						if(true)
						{

							try
							{
								string[] PageSplit = Regex.Split(resGetRequestFriends,"<a href=\"/users");

								if(PageSplit.Count()!=1)
								{
									List<string> PageSplitList = PageSplit.ToList();
									PageSplitList.RemoveAt(0);
									foreach(string item in PageSplitList)
									{
										if(item.Contains("<!DOCTYPE html>"))
										{
											continue;
										}

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}



										string UserIdScraped = FBUtils.getBetween(item,"/","/");
										if(!string.IsNullOrEmpty(UserIdScraped))
										{
											list.Add("https://www.facebook.com/" + UserIdScraped);
											AddToLogger_FriendsManager("Added  Id : " +  UserIdScraped);
										}
									}
								}
								else
								{

									PageSplit = Regex.Split(resGetRequestFriends,"data-profileid=");

									List<string> PageSplitList = PageSplit.ToList();
									PageSplitList.RemoveAt(0);
									foreach(string item in PageSplitList)
									{
										if(item.Contains("<!DOCTYPE html>"))
										{
											continue;
										}

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}



										string UserIdScraped = FBUtils.getBetween(item,"\"","\"");
										if(!string.IsNullOrEmpty(UserIdScraped))
										{
											list.Add("https://www.facebook.com/" + UserIdScraped);
											AddToLogger_FriendsManager("Added  Id : " +  UserIdScraped);
										}
									}

								}


							}
							catch{};

							int countForlistIteminPrvious =0;
							string ajaxRequestURL = "";
							int countFormaximumScrap = 0;
							while(true)
							{
								list = list.Distinct().ToList();
								countForlistIteminPrvious = list.Count();

								countFormaximumScrap++;
								if ((FriendManager.noOfFriendsToAdd +20) <= countFormaximumScrap)
								{
									AddToLogger_FriendsManager("No. of Friends Found To Scrape : " + list.Count());
									break;
								}

								try
								{
									list = list.Distinct().ToList();
									if((FriendManager.noOfFriendsToAdd+20)<= list.Count())
									{
										AddToLogger_FriendsManager("No of Friends Found To Scrape : " +  list.Count());
										break;
									}

									string[] PageSplit = Regex.Split(resGetRequestFriends,"rel=\"ajaxify\"");  //rel=\"ajaxify\"

									if(PageSplit.Count()==1)
									{
										string splitIt = "&amp;offset=";
										PageSplit = Regex.Split(resGetRequestFriends,splitIt);  //rel=\"ajaxify\"

										if(PageSplit.Count()==1)
										{
											AddToLogger_FriendsManager("All Friend Id Scraped ");
											break;
										}

										if(PageSplit.Count()>1)
										{

											PageSplit[1]  =  "/search/results/more/?q=" + item_keyword + "&amp;offset=" + PageSplit[1] ;
											ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"","\\\"");

										}

									}
									else
									{

										ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"href=\"","\"");
									}

									ajaxRequestURL = ajaxRequestURL.Replace("amp;","").Replace("type=all","type=groups").Replace("\\","%2C").Replace("u00252C","");

									ajaxRequestURL = "https://www.facebook.com" +  ajaxRequestURL + "&__user=" + UserId + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" +  FBUtils.getBetween(PageSplit[1],"revision\":",",");

									resGetRequestFriends =  httpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
									string allListGroup  = FBUtils.getBetween(resGetRequestFriends,"&quot;ents&quot;:&quot;","&quot");
									string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
									foreach(string item in Linklist)
									{

										list = list.Distinct().ToList();
										if((FriendManager.noOfFriendsToAdd+20) <= list.Count())
										{
											break;
										}


										try
										{
											if(!string.IsNullOrEmpty(item))
											{
												list.Add("https://www.facebook.com/"+item);
												AddToLogger_FriendsManager("Added  Id : " +  item);	

											}
										}
										catch{};

									}

									if(countForlistIteminPrvious==list.Count())
									{
										AddToLogger_FriendsManager("No of Ids Found To Send Request  : " +  list.Count());
										break;
									}

									list = list.Distinct().ToList();
								}
								catch{};
							}


						}



						FriendLink = list.Distinct().ToList();
					}
					else
					{
						FriendLink=FBUtils.LoadProfileUrls;
					}
					#endregion

					AddToLogger_FriendsManager(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

					int countFriendRequestsSent = 0;
					int counterforblockedFriendrequest = 0;
					foreach (string FriendRequestLink in FriendLink)
					{
						try
						{
							if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
							{

								//return;
							}
							if (countFriendRequestsSent >= NoOfFriendsRequest)
							{
								break;
							}

							AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
							bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);


							if (requeststatus)
							{
								countFriendRequestsSent++;
								countFriendRequestsSentAllKeyWord++;
								counterforblockedFriendrequest = 1;
								AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);

							}
							else
							{

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					} 
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}






		public void SendFriendRequestViaKeywords(ref FacebookUser fbUser)
		{
			try
			{


				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

				string UserId = string.Empty;


				string pageSource_HomePage = httpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl), "", "");


				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");


				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}




				if (!FBUtils.CheckUploadProfileUrls)
				{

					int countFriendRequestsSentAllKeyWord = 0;





					string keyword = string.Empty;

					if (!string.IsNullOrEmpty(Keywords))
					{
						keyword = FriendManager.Keywords;
						lstRequestFriendsKeywords.Add(keyword);
					}
					else if (lstRequestFriendsKeywords.Count > 0)
					{

						try
						{
							keyword = lstRequestFriendsKeywords[GlobusHttpHelper.GenerateRandom(0, lstRequestFriendsKeywords.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}


					lstRequestFriendsKeywords = lstRequestFriendsKeywords.Distinct().ToList();

					foreach (var item_keyword in lstRequestFriendsKeywords)
					{
						List<string> list = new List<string>();
						List<string> FriendLink = new List<string>();
						if (!FBUtils.CheckUploadProfileUrls)
						{


							string searchURL = "https://www.facebook.com/search/results/?q=" + item_keyword + "&type=users";


							string resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(searchURL), "", "");






							#region for find friend Reqest Link
							list.Clear();
							List<string> lstLinkData = new List<string>();
							lstLinkData.Clear();


							if (true)
							{

								try
								{
									string[] PageSplit = Regex.Split(resGetRequestFriends, "<a href=\"/users");

									if (PageSplit.Count() != 1)
									{
										List<string> PageSplitList = PageSplit.ToList();
										PageSplitList.RemoveAt(0);
										foreach (string item in PageSplitList)
										{
											if (item.Contains("<!DOCTYPE html>"))
											{
												continue;
											}

											list = list.Distinct().ToList();
											if ((FriendManager.noOfFriendsToAdd + 20) <= list.Count())
											{
												break;
											}



											string UserIdScraped = FBUtils.getBetween(item, "/", "/");
											if (!string.IsNullOrEmpty(UserIdScraped))
											{
												list.Add("https://www.facebook.com/" + UserIdScraped);
												AddToLogger_FriendsManager("Added  Id : " + UserIdScraped);
											}
										}
									}
									else
									{

										PageSplit = Regex.Split(resGetRequestFriends, "data-profileid=");

										List<string> PageSplitList = PageSplit.ToList();
										PageSplitList.RemoveAt(0);
										foreach (string item in PageSplitList)
										{
											if (item.Contains("<!DOCTYPE html>"))
											{
												continue;
											}

											list = list.Distinct().ToList();
											if ((FriendManager.noOfFriendsToAdd + 20) <= list.Count())
											{
												break;
											}



											string UserIdScraped = FBUtils.getBetween(item, "\"", "\"");
											if (!string.IsNullOrEmpty(UserIdScraped))
											{
												list.Add("https://www.facebook.com/" + UserIdScraped);
												AddToLogger_FriendsManager("Added  Id : " + UserIdScraped);
											}
										}

									}


								}
								catch { };

								int countForlistIteminPrvious = 0;
								string ajaxRequestURL = "";
								int countFormaximumScrap = 0;
								while (true)
								{
									list = list.Distinct().ToList();
									countForlistIteminPrvious = list.Count();

									countFormaximumScrap++;
									if ((FriendManager.noOfFriendsToAdd + 20) <= countFormaximumScrap)
									{
										AddToLogger_FriendsManager("No. of Friends Found To Scrape : " + list.Count());
										break;
									}

									try
									{
										list = list.Distinct().ToList();
										if ((FriendManager.noOfFriendsToAdd + 20) <= list.Count())
										{
											AddToLogger_FriendsManager("No of Friends Found To Scrape : " + list.Count());
											break;
										}

										string[] PageSplit = Regex.Split(resGetRequestFriends, "rel=\"ajaxify\"");  //rel=\"ajaxify\"

										if (PageSplit.Count() == 1)
										{
											string splitIt = "&amp;offset=";
											PageSplit = Regex.Split(resGetRequestFriends, splitIt);  //rel=\"ajaxify\"

											if (PageSplit.Count() == 1)
											{
												AddToLogger_FriendsManager("All Friend Id Scraped ");
												break;
											}

											if (PageSplit.Count() > 1)
											{

												PageSplit[1] = "/search/results/more/?q=" + item_keyword + "&amp;offset=" + PageSplit[1];
												ajaxRequestURL = FBUtils.getBetween(PageSplit[1], "", "\\\"");

											}

										}
										else
										{

											ajaxRequestURL = FBUtils.getBetween(PageSplit[1], "href=\"", "\"");
										}

										ajaxRequestURL = ajaxRequestURL.Replace("amp;", "").Replace("type=all", "type=groups").Replace("\\", "%2C").Replace("u00252C", "");

										ajaxRequestURL = "https://www.facebook.com" + ajaxRequestURL + "&__user=" + UserId + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" + FBUtils.getBetween(PageSplit[1], "revision\":", ",");

										resGetRequestFriends = httpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL), "", "");
										string allListGroup = FBUtils.getBetween(resGetRequestFriends, "&quot;ents&quot;:&quot;", "&quot");
										string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
										foreach (string item in Linklist)
										{

											list = list.Distinct().ToList();
											if ((FriendManager.noOfFriendsToAdd + 20) <= list.Count())
											{
												break;
											}


											try
											{
												if (!string.IsNullOrEmpty(item))
												{
													list.Add("https://www.facebook.com/" + item);
													AddToLogger_FriendsManager("Added  Id : " + item);

												}
											}
											catch { };

										}

										if (countForlistIteminPrvious == list.Count())
										{
											AddToLogger_FriendsManager("No of Ids Found To Send Request  : " + list.Count());
											break;
										}

										list = list.Distinct().ToList();
									}
									catch { };
								}


							}



							FriendLink = list.Distinct().ToList();
						}
						else
						{
							FriendLink = FBUtils.LoadProfileUrls;
						}
						#endregion

						AddToLogger_FriendsManager(FriendLink.Count + " Search Friend Requests Url with Email " + fbUser.username);

						int countFriendRequestsSent = 0;
						int counterforblockedFriendrequest = 0;
						foreach (string FriendRequestLink in FriendLink)
						{
							try
							{
								if (countFriendRequestsSentAllKeyWord >= NoOfFriendsRequest)
								{

									//return;
								}
								if (countFriendRequestsSent >= NoOfFriendsRequest)
								{
									break;
								}

								AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
								bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);


								if (requeststatus)
								{
									countFriendRequestsSent++;
									countFriendRequestsSentAllKeyWord++;
									counterforblockedFriendrequest = 1;
									AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);

								}
								else
								{

								}
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
					}
				}
				else 
				{

					List<string> lstFriend = FBUtils.GetAllFriends(ref httpHelper, UserId);
					List<string> lstFriendtouse = new List<string>();
					foreach (string item in lstFriend)
					{

						if (!item.Contains("www.facebook.com"))
						{
							lstFriendtouse.Add("http://www.facebook.com/" + item);

						}
						else
						{
							lstFriendtouse.Add(item);

						}

					}
					int countFriendRequestsSent = 0;
					foreach (string FriendRequestLink in lstFriendtouse)
					{
						try
						{

							if (countFriendRequestsSent >= NoOfFriendsRequest)
							{
								break;
							}

							AddToLogger_FriendsManager(" Friend Requests sending with Url :" + FriendRequestLink + " and Email " + fbUser.username);
							bool requeststatus = SendFriendRequestUpdated(FriendRequestLink, UserId, ref fbUser);


							if (requeststatus)
							{
								countFriendRequestsSent++;
								//  countFriendRequestsSentAllKeyWord++;
								// counterforblockedFriendrequest = 1;
								AddToLogger_FriendsManager(countFriendRequestsSent + " => Request Sent With Username : " + fbUser.username);

							}
							else
							{

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}







				}




			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestViaKeywords  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}































		public bool SendFriendRequestUpdated_Ols_3_oct(string FriendRequestByUrl, string UserId, ref FacebookUser fbUser)
		{
			try
			{



				try
				{
					lstThreadsWallPoster.Add(Thread.CurrentThread);
					lstThreadsWallPoster.Distinct();
					Thread.CurrentThread.IsBackground = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				} 



				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

				string FriRequestUrl = string.Empty;
				string FriendId = string.Empty;

				if (FriendRequestByUrl.Contains("profile"))
				{
					if (FriendRequestByUrl.Contains("&ref=pymk"))
					{
						try
						{
							FriRequestUrl = FriendRequestByUrl;
							string strFriId = FriRequestUrl.Replace("&ref=pymk", "");
							string[] ArrTemp = strFriId.Split('=');
							FriendId = ArrTemp[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
					else
					{
						try
						{
							FriRequestUrl = FriendRequestByUrl + "&ref=pymk";
							string[] ArrTemp = FriendRequestByUrl.Split('=');
							FriendId = ArrTemp[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
				}
				else
				{
					if (FriendRequestByUrl.Contains("https://www.facebook.com"))
					{
						FriRequestUrl = FriendRequestByUrl;
					}
					else
					{
						FriRequestUrl ="https://www.facebook.com/"+ FriendRequestByUrl;
					}
				}
				string pageSrcFriendProfileUrl = httpHelper.getHtmlfromUrl(new Uri(FriRequestUrl),"","");


				if (pageSrcFriendProfileUrl.Contains("profile_id") && string.IsNullOrEmpty(FriendId))
				{
					string[] Arr = Regex.Split(pageSrcFriendProfileUrl, "profile_id");
					foreach (string item in Arr)
					{
						try
						{
							if (!item.Contains("<!DOCTYPE"))
							{
								string profileId = item.Substring(0, 40);
								if (profileId.Contains("&"))
								{
									try
									{
										string[] TempArr1 = profileId.Split('=');
										string[] TempArr = TempArr1[1].Split('&');
										FriendId = TempArr[0];
										break;
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								if (profileId.Contains(":") && profileId.Contains(","))
								{
									try
									{
										string[] TempArr = profileId.Split(':');
										string[] TempArr1 = TempArr[1].Split(',');
										FriendId = TempArr1[0];
										break;
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}


								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
				}



				string fb_dtsg = GlobusHttpHelper.GetParamValue(pageSrcFriendProfileUrl, "fb_dtsg");

				///** First Post For Friend Request *******************************************///

				string PostUrlFriendRequestFirst = FBGlobals.Instance.urlPostUrlFriendRequestFirstFriendManager;


				string PostDataFriendRequestFirst = "fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

				string ResponseFriendRequestFirst = httpHelper.postFormData(new Uri(PostUrlFriendRequestFirst), PostDataFriendRequestFirst);

				///** Second Post For Friend Request *******************************************///


				string PostUrlFriendRequestSecond = FBGlobals.Instance.urlPostUrlFriendRequestSecondFriendManager;

				string PostDataFriendRequestSecond = "friend=" + FriendId + "&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

				string ResponseFriendRequestSecond = httpHelper.postFormData(new Uri(PostUrlFriendRequestSecond), PostDataFriendRequestSecond);

				///** Third Post For Friend Request *******************************************///

				//string FriendId = FriendRequestByUrl.Split('=')[1];

				string PostUrlFriendRequestThird = FBGlobals.Instance.urlPostUrlFriendRequestThirdFriendManager;  
				string PostDataFriendRequestThird = string.Empty;
				if (pageSrcFriendProfileUrl.Contains("TimelineCapsule"))
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&link_data[gt][profile_owner]=" + FriendId + "&link_data[gt][ref]=timeline%3Atimeline&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				else
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}

				string ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);

				if (ResponseFriendRequestThird.Contains("errorSummary") && ResponseFriendRequestThird.Contains("Confirmation Required")) // && ResponseFriendRequestThird.Contains("A confirmation is required before you can proceed"))
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=" + UserId + "&confirmed=1";
						ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}

				if (ResponseFriendRequestThird.Contains("success"))
				{
					//AddToLogger_FriendsManager("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);
					//GlobusLogHelper.log.Debug("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);

					return true;

				}
				else if (ResponseFriendRequestThird.Contains("Already Sent Request"))
				{
				
					AddToLogger_FriendsManager("Already requested :" + FriRequestUrl + " with Account " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

					Thread.Sleep(delayInSeconds);

					return false;
				}
				else if (ResponseFriendRequestThird.Contains("You've been blocked from using this feature because you may have violated Facebook's Terms."))
				{

					AddToLogger_FriendsManager("You've been blocked from using this feature because you may have violated Facebook's Terms.." + "With Account" + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);
				}
				else if(ResponseFriendRequestThird.Contains("You&#039;re blocked from sending friend requests for"))
				{

					AddToLogger_FriendsManager("You are blocked from sending friend requests " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);
				}
				else if (ResponseFriendRequestThird.Contains("\"errorSummary\":") && !ResponseFriendRequestThird.Contains("Already requested"))
				{
					string errorSummary=FBUtils.GetErrorSummary(ResponseFriendRequestThird);

					AddToLogger_FriendsManager("" + errorSummary + " with Url :" + FriRequestUrl + " with Account " + fbUser.username);

				}
				else 
				{

					AddToLogger_FriendsManager("Some Problem with Url :" + FriRequestUrl + " with Account " + fbUser.username);
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return false;
		}





		//Start WallPoster 





		public bool SendFriendRequestUpdated(string FriendRequestByUrl, string UserId, ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper httpHelper = fbUser.globusHttpHelper;

				string FriRequestUrl = string.Empty;
				string FriendId = string.Empty;

				if (FriendRequestByUrl.Contains("profile"))
				{
					if (FriendRequestByUrl.Contains("&ref=pymk"))
					{
						try
						{
							FriRequestUrl = FriendRequestByUrl;
							string strFriId = FriRequestUrl.Replace("&ref=pymk", "");
							string[] ArrTemp = strFriId.Split('=');
							FriendId = ArrTemp[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
					else
					{
						try
						{
							FriRequestUrl = FriendRequestByUrl + "&ref=pymk";
							string[] ArrTemp = FriendRequestByUrl.Split('=');
							FriendId = ArrTemp[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}
				else
				{
					if (!FriendRequestByUrl.Contains("https://www.facebook.com"))
					{
						FriRequestUrl = "https://www.facebook.com/"+FriendRequestByUrl;
					}
					else
					{
						FriRequestUrl = FriendRequestByUrl;
					}
				}
				string pageSrcFriendProfileUrl = httpHelper.getHtmlfromUrl(new Uri(FriRequestUrl),"","");


				if (pageSrcFriendProfileUrl.Contains("profile_id") && string.IsNullOrEmpty(FriendId))
				{
					string[] Arr = Regex.Split(pageSrcFriendProfileUrl, "profile_id");
					foreach (string item in Arr)
					{
						try
						{
							if (!item.Contains("<!DOCTYPE"))
							{
								string profileId = item.Substring(0, 40);
								if (profileId.Contains("&"))
								{
									try
									{
										string[] TempArr1 = profileId.Split('=');
										string[] TempArr = TempArr1[1].Split('&');
										FriendId = TempArr[0];
										break;
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
								if (profileId.Contains(":") && profileId.Contains(","))
								{
									try
									{
										string[] TempArr = profileId.Split(':');
										string[] TempArr1 = TempArr[1].Split(',');
										FriendId = TempArr1[0];
										break;
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
									}


								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}



				string fb_dtsg = GlobusHttpHelper.GetParamValue(pageSrcFriendProfileUrl, "fb_dtsg");

				///** First Post For Friend Request *******************************************///

				string PostUrlFriendRequestFirst = FBGlobals.Instance.urlPostUrlFriendRequestFirstFriendManager;


				string PostDataFriendRequestFirst = "fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

				string ResponseFriendRequestFirst = httpHelper.postFormData(new Uri(PostUrlFriendRequestFirst), PostDataFriendRequestFirst);

				///** Second Post For Friend Request *******************************************///


				string PostUrlFriendRequestSecond = FBGlobals.Instance.urlPostUrlFriendRequestSecondFriendManager;

				string PostDataFriendRequestSecond = "friend=" + FriendId + "&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";

				string ResponseFriendRequestSecond = httpHelper.postFormData(new Uri(PostUrlFriendRequestSecond), PostDataFriendRequestSecond);

				///** Third Post For Friend Request *******************************************///

				//string FriendId = FriendRequestByUrl.Split('=')[1];

				string PostUrlFriendRequestThird = FBGlobals.Instance.urlPostUrlFriendRequestThirdFriendManager;  
				string PostDataFriendRequestThird = string.Empty;
				if (pageSrcFriendProfileUrl.Contains("TimelineCapsule"))
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&link_data[gt][profile_owner]=" + FriendId + "&link_data[gt][ref]=timeline%3Atimeline&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
						GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendFriendRequestUpdated  in FriendManager ", FBGlobals.AllExceptionLoggerFilePath);
					}
				}
				else
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=";
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}

				string ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);

				if (ResponseFriendRequestThird.Contains("errorSummary") && ResponseFriendRequestThird.Contains("Confirmation Required")) // && ResponseFriendRequestThird.Contains("A confirmation is required before you can proceed"))
				{
					try
					{
						PostDataFriendRequestThird = "to_friend=" + FriendId + "&action=add_friend&how_found=profile_button&ref_param=none&&&outgoing_id=js_0&logging_location=&no_flyout_on_click=false&ego_log_data=&http_referer=&fb_dtsg=" + fb_dtsg + "&__user=" + UserId + "&phstamp=" + UserId + "&confirmed=1";
						ResponseFriendRequestThird = httpHelper.postFormData(new Uri(PostUrlFriendRequestThird), PostDataFriendRequestThird);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}

				if (ResponseFriendRequestThird.Contains("success"))
				{
					//AddToLogger_FriendsManager("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);
					//GlobusLogHelper.log.Debug("Friend Request sent to profile :" + FriRequestUrl + " with Account " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);

					return true;

				}
				else if (ResponseFriendRequestThird.Contains("Already Sent Request"))
				{

					AddToLogger_FriendsManager("Already requested :" + FriRequestUrl + " with Account " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

					Thread.Sleep(delayInSeconds);

					return false;
				}
				else if (ResponseFriendRequestThird.Contains("You've been blocked from using this feature because you may have violated Facebook's Terms."))
				{

					AddToLogger_FriendsManager("You've been blocked from using this feature because you may have violated Facebook's Terms.." + "With Account" + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);
				}
				else if(ResponseFriendRequestThird.Contains("You&#039;re blocked from sending friend requests for"))
				{

					AddToLogger_FriendsManager("You are blocked from sending friend requests " + fbUser.username);

					int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);

					AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
					Thread.Sleep(delayInSeconds);
				}
				else if (ResponseFriendRequestThird.Contains("\"errorSummary\":") && !ResponseFriendRequestThird.Contains("Already requested"))
				{
					string errorSummary=FBUtils.GetErrorSummary(ResponseFriendRequestThird);

					AddToLogger_FriendsManager("Already Requested or some error");

				}
				else 
				{

					AddToLogger_FriendsManager("Some Problem with Url :" + FriRequestUrl + " with Account " + fbUser.username);
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SenFriendRequest ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return false;
		}










		#region Global Variables For Wall Poster
		public static bool IsUniqueMessagePosting = false;
		public static bool IsUniquePicPosting = false;
		readonly object lockrThreadControllerWallPoster = new object();
		public bool isStopWallPoster = false;
		int countThreadControllerWallPoster = 0;
		public static int TotalNoOfWallPosterCounter = 0;
		public static int messageCountWallPoster = 0;
		int countWallPoster = 1;
		public static bool statusForGreetingMsgWallPoster = false;

		public List<Thread> lstThreadsWallPoster = new List<Thread>();
		public List<string> lstWallPostURLsWallPoster = new List<string>();
		public List<string> lstMessagesWallPoster = new List<string>();
		public List<string> lstSpinnerWallMessageWallPoster = new List<string>();
		public List<string> lstGreetMsgWallPoster = new List<string>();
		public List<string> lstWallMessageWallPoster = new List<string>();

		public static int minDelayWallPoster = 10;
		public static int maxDelayWallPoster = 20;

		#endregion

		#region Global Variables For Post Pic On Wall

		readonly object lockrThreadControllerPostPicOnWall = new object();
		public bool isStopPostPicOnWall = false;
		public bool chkCountinueProcessGroupCamapinScheduler = false;
		public static bool isPrivacyOnlyMe = false;
		//public bool chkCountinueProcessGroupCamapinScheduler = false;
		//public bool chkCountinueProcessGroupCamapinScheduler = false;
		// public static bool chkCountinueProcessGroupCamapinScheduler = false;

		int countThreadControllerPostPicOnWall = 0;

		public List<Thread> lstThreadsPostPicOnWall = new List<Thread>();
		public List<string> lstPicturecollectionPostPicOnWall = new List<string>();
		public List<string> lstMessageCollectionPostPicOnWall = new List<string>();
		public List<string> lstWallPostShareLoadTargedUrls = new List<string>();

		public bool chkCountinueProcessContinueShareProcess = false;
		public bool chkWall_PostPicOnWall_ShareVideoOnlyMe = false;

		public static int NumberOfFriendsSendPicOnWall = 5;
		public static int minDelayPostPicOnWal = 10;
		public static int maxDelayPostPicOnWal = 20;

		#endregion

		#region Property For Wall Poster

		public int NoOfThreadsWallPoster
		{
			get;
			set;
		}

		public int NoOfFriendsWallPoster
		{
			get;
			set;
		}

		public bool UseAllUrlWallPoster
		{
			get;
			set;
		}

		public bool IsUseTextMessageWallPoster
		{
			get;
			set;
		}

		public bool IsUseURLsMessageWallPoster
		{
			get;
			set;
		}


		public bool UseOneMsgToAllFriendsWallPoster
		{
			get;
			set;
		}

		public bool UseRandomWallPoster
		{
			get;
			set;
		}

		public bool UseUniqueMsgToAllFriendsWallPoster
		{
			get;
			set;
		}

		public bool ChkSpinnerWallMessaeWallPoster
		{
			get;
			set;
		}

		public string MsgWallPoster
		{
			get;
			set;
		}

		public static string StartProcessUsingWallPoster
		{
			get;
			set;
		}

		#endregion

		#region Property For Post Pic On Wall

		public int NoOfThreadsPostPicOnWall
		{
			get;
			set;
		}

		public bool IsPostAllPicPostPicOnWall
		{
			get;
			set;
		}
		public bool chkWallPostPicOnWallWithMessage
		{
			get;
			set;
		}

		public bool chkWallWallPosterRemoveURLsMessages
		{
			get;
			set;
		}
		public static string StartProcessUsingPostPicOnWall
		{
			get;
			set;
		}


		#endregion

		#region Variables
		// public static bool statusForGreetingMsgWallPoster = true;
		public static int TotalNoOfWallPoster_Counter = 0;
		// public static List<string> lstGreetMsgWallPoster = new List<string>();
		//  public static List<string> lstSpinnerWallMessageWallPoster = new List<string>();
		//public static int countWallPoster = 0;
		// public List<string> lstMessageCollectionPostPicOnWall = new List<string>();
		//public List<string> lstPicturecollectionPostPicOnWall = new List<string>();

		#endregion








		public void WallPostingWithTestMessageOld(ref FacebookUser fbUser)
		{
			try
			{
				string UsreId = string.Empty;

				AddToLogger_FriendsManager("Start Wall Posting With Username : " + fbUser.username);


				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				string ProFilePost = FBGlobals.Instance.fbProfileUrl;

				string tempUserID = string.Empty;

				List<string> lstFriend = new List<string>();

				UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UsreId))
				{
					UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				if (string.IsNullOrEmpty(UsreId) || UsreId == "0" || UsreId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();
				//if (IsUseTextMessageWallPoster)
				{
					MsgWallPoster = lstWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallMessageWallPoster.Count)];
				}
				if (IsUseURLsMessageWallPoster)
				{
					MsgWallPoster = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
				}



				string profileUrl = ProFilePost + UsreId + "&sk=wall";
				string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl),"","");

				string wallmessage = MsgWallPoster;
				wallmessage = wallmessage.Replace("<friend first name>", string.Empty);

				if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
				{
					if (lstWallPostURLsWallPoster.Count > 0)
					{

						wallmessage = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}
					else
					{
						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}

					wallmessage = wallmessage.Replace("=", "%3D");

					string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
					string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");

					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
					}

					string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
					}

					string ResponseWallPost = string.Empty;
					string sessionId = GlobusHttpHelper.GenerateTimeStamp();
					wallmessage = Uri.EscapeUriString(wallmessage);
					ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=");

					if (ResponseWallPost.Length < 300)
					{
						ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + (xhpc_composerid) + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=");
					}
					if (ResponseWallPost.Length >= 300)
					{
						TotalNoOfWallPoster_Counter++;

						AddToLogger_FriendsManager("Posted message on own wall " + fbUser.username);
					
					}

					else
					{
						AddToLogger_FriendsManager("Couldn't post on own wall " + fbUser.username);

					}
				}
				AddToLogger_FriendsManager("Please wait finding the friends ID...");

				if (NoOfFriendsWallPoster != 0)
				{
					lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UsreId);
					lstFriend = lstFriend.Distinct().ToList();
				}
				var itemId = lstFriend.Distinct();
				try
				{
					int countFrnd=0;
					string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UsreId),"","");
					if (pageSource.Contains("pagelet_timeline_medley_friends"))
					{
						string findTheAllFrnList = FBUtils.getBetween(pageSource_Home, "pagelet_timeline_medley_friends", "</span>");
						if (string.IsNullOrEmpty(findTheAllFrnList))
						{
							string[] aa = System.Text.RegularExpressions.Regex.Split(pageSource, "pagelet_timeline_medley_friends");
							findTheAllFrnList =FBUtils.getBetween(aa[1], "\"_gs6\">", "</span>");

						}

						countFrnd = Convert.ToInt32(findTheAllFrnList);
					}
					else if (pageSource.Contains("FriendCount"))
					{
						string findTheAllFrnList =FBUtils.getBetween(pageSource_Home, "FriendCount", "}");
						if (findTheAllFrnList.Contains("FriendCount"))
						{
							string friendsCount=FBUtils.getBetween(findTheAllFrnList+"##","FriendCount","##");
						}
					}



					//FriendCount

				//	AddToLogger_FriendsManager("Found " + countFrnd + " friend's ids");


				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				int CountPostWall = 0;

				// messageCountWallPoster = 5;
				messageCountWallPoster = NoOfFriendsWallPoster;

				int friendval = messageCountWallPoster;
				int friendCount = 0;

				if (itemId.Count() > friendval)
				{
					friendCount = friendval;
				}
				else
				{
					friendCount = itemId.Count();
				}

				try
				{
					///Generate a random no list ranging 0-lstMessages.Count
					ArrayList randomNoList = GlobusHttpHelper.RandomNumbers(lstMessagesWallPoster.Count - 1);

					int msgIndex = 0;

					foreach (string friendId in itemId)
					{

						if (CountPostWall > friendCount)
						{
							return;
						}
						try
						{
							#region SelectQuery
							// System.Data.DataSet ds = new DataSet();
							try
							{
								//string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
								// ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							//if (ds.Tables[0].Rows.Count < 1)
							{
								// return; 
								#endregion
								string message = string.Empty;
								message = lstWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallMessageWallPoster.Count)];
							
								if (UsreId != friendId)
								{
									#region Select Msg according to Mode
									try
									{
										///Normal, 1 msg to all friends
										if (UseOneMsgToAllFriendsWallPoster)
										{
											message = MsgWallPoster.Replace(" ", " ");  //%20;
										}

										///For Random, might be Unique, might not be
										else if (UseRandomWallPoster)
										{
											if (msgIndex < randomNoList.Count)
											{
												msgIndex = (int)randomNoList[msgIndex];
												message = lstMessagesWallPoster[msgIndex];
												msgIndex++;
											}
											else if (lstMessagesWallPoster.Count > msgIndex)
											{
												message = lstMessagesWallPoster[msgIndex];
												msgIndex++;
											}
											else
											{
												try
												{
													msgIndex = 0;
													randomNoList = GlobusHttpHelper.RandomNumbers(lstMessagesWallPoster.Count - 1);
													message = lstMessagesWallPoster[msgIndex];
													msgIndex++;
												}
												catch (Exception ex)
												{
													Console.WriteLine("Error : " + ex.StackTrace);
												}
											}
										}

										///For Unique or Different Msg for each friend                                        

										else if (UseUniqueMsgToAllFriendsWallPoster)
										{
											if (lstSpinnerWallMessageWallPoster.Count > countWallPoster - 1)
											{
												try
												{
													message = lstSpinnerWallMessageWallPoster[countWallPoster - 1];

													if (lstSpinnerWallMessageWallPoster.Contains(message))
													{
														lstSpinnerWallMessageWallPoster.Remove(message);
													}

												}
												catch (Exception ex)
												{
													Console.WriteLine("Error : " + ex.StackTrace);
												}

											}
											else
											{
												try
												{
													message = lstSpinnerWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count - 1)];
												}
												catch (Exception ex)
												{
													message = lstMessagesWallPoster[GlobusHttpHelper.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
												}


											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									#endregion

									try
									{
										//if (!ChkSpinnerWallMessaeWallPoster)
										{
											if (!string.IsNullOrEmpty(message))
											{
											

												PostOnFriendsWallTestMessage(friendId, message, ref fbUser, ref UsreId); 
											}

										}
										//else
										{
											if (lstSpinnerWallMessageWallPoster.Count > 0)
											{
												if (!string.IsNullOrEmpty(message))
												{

													PostOnFriendsWall(friendId, message, ref fbUser, ref UsreId);
												}
											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									CountPostWall++;
								}
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

				AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			finally
			{
				AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

				// HttpHelper.http.Dispose(); 
			}
		}  



		public void WallPostingWithTestMessage(ref FacebookUser fbUser)
		{
			try
			{
				string UsreId = string.Empty;

				AddToLogger_FriendsManager("Start Wall Posting With Username : " + fbUser.username);


				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				string ProFilePost = FBGlobals.Instance.fbProfileUrl;

				string tempUserID = string.Empty;

				List<string> lstFriend = new List<string>();

				UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UsreId))
				{
					UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				if (string.IsNullOrEmpty(UsreId) || UsreId == "0" || UsreId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


					return;
				}

				lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();
				//if (IsUseTextMessageWallPoster)
				{
					MsgWallPoster = lstWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallMessageWallPoster.Count)];
				}
				if (IsUseURLsMessageWallPoster)
				{
					MsgWallPoster = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
				}



				string profileUrl = ProFilePost + UsreId + "&sk=wall";
				string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl),"","");

				string wallmessage = MsgWallPoster;
				wallmessage = wallmessage.Replace("<friend first name>", string.Empty);


				/*

				if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
				{
					if (lstWallPostURLsWallPoster.Count > 0)
					{

						wallmessage = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}
					else
					{
						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}

					wallmessage = wallmessage.Replace("=", "%3D");

					string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
					string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");

					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
					}

					string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
					}

					string ResponseWallPost = string.Empty;
					string sessionId = GlobusHttpHelper.GenerateTimeStamp();
					wallmessage = Uri.EscapeUriString(wallmessage);




					ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=");

					if (ResponseWallPost.Length < 300)
					{
						ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + (xhpc_composerid) + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + (wallmessage) + "&xhpc_message=" + (wallmessage) + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=");
					}
					if (ResponseWallPost.Length >= 300)
					{
						TotalNoOfWallPoster_Counter++;

						AddToLogger_FriendsManager("Posted message on own wall " + fbUser.username);
					
					}

					else
					{
						AddToLogger_FriendsManager("Couldn't post on own wall " + fbUser.username);

					}

			}*/











				AddToLogger_FriendsManager("Please wait finding the friends ID...");

				if (NoOfFriendsWallPoster != 0)
				{
					lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UsreId);
					lstFriend = lstFriend.Distinct().ToList();
				}
				var itemId = lstFriend.Distinct();
				try
				{
					int countFrnd=0;
					string pageSource = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl + UsreId),"","");
					if (pageSource.Contains("pagelet_timeline_medley_friends"))
					{
						string findTheAllFrnList = FBUtils.getBetween(pageSource_Home, "pagelet_timeline_medley_friends", "</span>");
						if (string.IsNullOrEmpty(findTheAllFrnList))
						{
							string[] aa = System.Text.RegularExpressions.Regex.Split(pageSource, "pagelet_timeline_medley_friends");
							findTheAllFrnList =FBUtils.getBetween(aa[1], "\"_gs6\">", "</span>");

						}

						countFrnd = Convert.ToInt32(findTheAllFrnList);
					}
					else if (pageSource.Contains("FriendCount"))
					{
						string findTheAllFrnList =FBUtils.getBetween(pageSource_Home, "FriendCount", "}");
						if (findTheAllFrnList.Contains("FriendCount"))
						{
							string friendsCount=FBUtils.getBetween(findTheAllFrnList+"##","FriendCount","##");
						}
					}



					//FriendCount

					//	AddToLogger_FriendsManager("Found " + countFrnd + " friend's ids");


				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				int CountPostWall = 0;

				// messageCountWallPoster = 5;
				messageCountWallPoster = NoOfFriendsWallPoster;

				int friendval = messageCountWallPoster;
				int friendCount = 0;

				if (itemId.Count() > friendval)
				{
					friendCount = friendval;
				}
				else
				{
					friendCount = itemId.Count();
				}

				try
				{
					///Generate a random no list ranging 0-lstMessages.Count
					ArrayList randomNoList = GlobusHttpHelper.RandomNumbers(lstMessagesWallPoster.Count - 1);

					int msgIndex = 0;

					foreach (string friendId in itemId)
					{

						if (CountPostWall >= friendCount)
						{
							return;
						}
						try
						{
							#region SelectQuery
							// System.Data.DataSet ds = new DataSet();
							try
							{
								//string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
								// ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							//if (ds.Tables[0].Rows.Count < 1)
							{
								// return; 
								#endregion
								string message = string.Empty;
								message = lstWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallMessageWallPoster.Count)];

								if (UsreId != friendId)
								{
									#region Select Msg according to Mode
									try
									{
										///Normal, 1 msg to all friends
										if (UseOneMsgToAllFriendsWallPoster)
										{
											message = MsgWallPoster.Replace(" ", " ");  //%20;
										}

										///For Random, might be Unique, might not be
										else if (UseRandomWallPoster)
										{
											if (msgIndex < randomNoList.Count)
											{
												msgIndex = (int)randomNoList[msgIndex];
												message = lstMessagesWallPoster[msgIndex];
												msgIndex++;
											}
											else if (lstMessagesWallPoster.Count > msgIndex)
											{
												message = lstMessagesWallPoster[msgIndex];
												msgIndex++;
											}
											else
											{
												try
												{
													msgIndex = 0;
													randomNoList = GlobusHttpHelper.RandomNumbers(lstMessagesWallPoster.Count - 1);
													message = lstMessagesWallPoster[msgIndex];
													msgIndex++;
												}
												catch (Exception ex)
												{
													Console.WriteLine("Error : " + ex.StackTrace);
												}
											}
										}

										///For Unique or Different Msg for each friend                                        

										else if (UseUniqueMsgToAllFriendsWallPoster)
										{
											if (lstSpinnerWallMessageWallPoster.Count > countWallPoster - 1)
											{
												try
												{
													message = lstSpinnerWallMessageWallPoster[countWallPoster - 1];

													if (lstSpinnerWallMessageWallPoster.Contains(message))
													{
														lstSpinnerWallMessageWallPoster.Remove(message);
													}

												}
												catch (Exception ex)
												{
													Console.WriteLine("Error : " + ex.StackTrace);
												}

											}
											else
											{
												try
												{
													message = lstSpinnerWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count - 1)];
												}
												catch (Exception ex)
												{
													message = lstMessagesWallPoster[GlobusHttpHelper.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
												}


											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									#endregion

									try
									{
										//if (!ChkSpinnerWallMessaeWallPoster)
										{
											if (!string.IsNullOrEmpty(message))
											{


												PostOnFriendsWallTestMessage(friendId, message, ref fbUser, ref UsreId); 
											}

										}
										//else
										{
											if (lstSpinnerWallMessageWallPoster.Count > 0)
											{
												if (!string.IsNullOrEmpty(message))
												{

													PostOnFriendsWall(friendId, message, ref fbUser, ref UsreId);
												}
											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									CountPostWall++;
								}
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

				AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			finally
			{
				AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

				// HttpHelper.http.Dispose(); 
			}
		} 
















		private void PostOnFriendsWallTestMessage_Old101(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string friendid = friendId;
				string wallMessage = wallmessage;
				DateTime datetiemvalue = DateTime.Now;
				TimeSpan xcx = DateTime.Now - datetiemvalue;

				if (!statusForGreetingMsgWallPoster)
				{
					string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

					if (postUrl.Contains("https://"))
					{
						postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
						string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl),"","");

						if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
						{
							AddToLogger_FriendsManager(countWallPoster.ToString() + " Posting on wall " + postUrl);


							string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);

							string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

							string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");
							wallmessage = Uri.EscapeUriString(wallmessage);
							string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";
							string ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111);
							int length = ResponseWallPost.Length;

							string postDataWalllpost1112 = string.Empty;
							string ResponseWallPost2 = string.Empty;
							if (!(length > 1100))
							{
								postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
								ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112);

								int length2 = ResponseWallPost2.Length;
								if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
								{
									TotalNoOfWallPoster_Counter++;

									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


									countWallPoster++;

									try
									{                                      

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

									try
									{
										#region insertQuery
										//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
										//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
										#endregion
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
								{
									TotalNoOfWallPoster_Counter++;
									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

									countWallPoster++;

									try
									{                                       

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

									try
									{
										#region insertQuery
										//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
										//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
										#endregion
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								else
								{
									string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost2);
									AddToLogger_FriendsManager("Error : " + errorSummary + " not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

								}
							}
							else
							{
								TotalNoOfWallPoster_Counter++;

							AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


								countWallPoster++;

								try
								{                                 

									int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
									AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep(delayInSeconds);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

								try
								{
									#region insertQuery
									//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
									//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
									#endregion
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}

							System.Threading.Thread.Sleep(4000);
						}
						else
						{
							AddToLogger_FriendsManager("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);


							System.Threading.Thread.Sleep(1000);
						}
					}
				}
				else
				{
					try
					{
						//PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UsreId);
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











		private void PostOnFriendsWallTestMessage_old_3_oct(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string friendid = friendId;
				string wallMessage = wallmessage;
				DateTime datetiemvalue = DateTime.Now;
				TimeSpan xcx = DateTime.Now - datetiemvalue;

				if (!statusForGreetingMsgWallPoster)
				{
					string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

					if (postUrl.Contains("https://"))
					{
						postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
						string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl),"","");

						if (pageSourceWallPost11.Contains("fb_dtsg") && (pageSourceWallPost11.Contains("xhpc_composerid") || pageSourceWallPost11.ToLower().Contains("composerid")) && (pageSourceWallPost11.Contains("xhpc_targetid") || pageSourceWallPost11.ToLower().Contains("targetid")))
						{
							AddToLogger_FriendsManager(countWallPoster.ToString() + " Posting on wall " + postUrl);


							string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);

							string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

							if (string.IsNullOrEmpty(xhpc_composerid))
							{
								try
								{
									xhpc_composerid = GlobusHttpHelper.getBetween(pageSourceWallPost11, "composerID\":\"","\"");
									if (xhpc_composerid.Contains("\""))
									{
										xhpc_composerid = xhpc_composerid.Replace("\"", "");
									}
								}
								catch { };

							}

							string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");



							if (string.IsNullOrEmpty(xhpc_targetid))
							{
								try
								{
									xhpc_targetid = GlobusHttpHelper.getBetween(pageSourceWallPost11, "targetID\":", "}");
									if (xhpc_targetid.Contains("\""))
									{
										xhpc_targetid = xhpc_targetid.Replace("\"", "");

									}
								}
								catch { };

							}
							string appid = "";
							try
							{
								appid = FBUtils.getBetween(pageSourceWallPost11, "appid=", "&");
							}
							catch { };



							if (wallmessage.Contains("https://") || wallmessage.Contains("http://"))
							{
								if (wallmessage.Contains("https://"))
								{
									wallmessage = wallmessage.Replace("https://", "");

								}
								else if (wallmessage.Contains("http://"))
								{
									wallmessage = wallmessage.Replace("http://", "");

								}

							}



							string[] messagesList = { };

							try
							{
								messagesList = Regex.Split(wallmessage, ":");

								if (messagesList.Count() > 1)
								{
									//string UrlMessage = messagesList[1];



									//string UrlThumbnail = "https://www.facebook.com/react_composer/scraper/?composer_id=" + xhpc_composerid + "&target_id=" + xhpc_targetid + "&scrape_url=" + UrlMessage;


									//string PostDataThumbNail = "__user=" + UsreId + "&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxCbzES2N6xybxu3fzoy2e58kUgDxuy28yQq5UBGeyXybDGcCxC&__req=g&fb_dtsg=" + fb_dtsg + "&ttstamp=" + "265817110611280100771201176785" + "&__rev=" + 1843986;

									////&attachment[params][urlInfo][canonical]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][final]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437426144]=http%3A%2F%2Ffacebook.com%2F&attachment[params][urlInfo][log][1437450198]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437450230]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][responseCode]=200&attachment[params][favicon]=https%3A%2F%2Ffbstatic-a.akamaihd.net%2Frsrc.php%2FyV%2Fr%2FhzMapiNYYpW.ico&&attachment[params][title]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][summary]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][content_removed]=false&attachment[params][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&&attachment[params][ranked_images][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][image_info][0][width]=325&attachment[params][image_info][0][height]=325&&attachment[params][image_info][0][xray][overlaid_text]=0.1731&attachment[params][image_info][0][xray][synthetic]=0.5766&attachment[params][image_info][0][xray][scores][437978556329078]=0.014&attachment[params][image_info][0][xray][scores][976885115686468]=0.2174&attachment[params][video_info][duration]=0&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][domain_ip]=2a03%3A2880%3A2050%3A3f07%3Aface%3Ab00c%3A0%3A1&attachment[params][time_scraped]=1437476787&attachment[params][cache_hit]=true&attachment[params][global_share_id]=6976353357&attachment[params][was_recent]=false&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][charset]=utf-8&attachment[params][metaTagMap][2][name]=referrer&attachment[params][metaTagMap][2][content]=default&attachment[params][metaTagMap][2][id]=meta_referrer&attachment[params][metaTagMap][3][property]=og%3Asite_name&attachment[params][metaTagMap][3][content]=Facebook&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][metaTagMap][5][property]=og%3Aimage&attachment[params][metaTagMap][5][content]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][metaTagMap][6][property]=og%3Alocale&attachment[params][metaTagMap][6][content]=en_US&attachment[params][metaTagMap][7][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][7][content]=www&attachment[params][metaTagMap][8][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][8][content]=es_LA&attachment[params][metaTagMap][9][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][9][content]=es_ES&attachment[params][metaTagMap][10][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][10][content]=fr_FR&attachment[params][metaTagMap][11][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][11][content]=it_IT&attachment[params][metaTagMap][12][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][12][content]=id_ID&attachment[params][metaTagMap][13][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][13][content]=th_TH&attachment[params][metaTagMap][14][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][14][content]=vi_VN&attachment[params][metaTagMap][15][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][15][content]=ko_KR&attachment[params][metaTagMap][16][name]=description&attachment[params][metaTagMap][16][content]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][metaTagMap][17][name]=robots&attachment[params][metaTagMap][17][content]=noodp%2Cnoydir&&attachment[params][og_info][properties][0][0]=og%3Asite_name&attachment[params][og_info][properties][0][1]=Facebook&attachment[params][og_info][properties][1][0]=og%3Aurl&attachment[params][og_info][properties][1][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][properties][2][0]=og%3Aimage&attachment[params][og_info][properties][2][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][properties][3][0]=og%3Alocale&attachment[params][og_info][properties][3][1]=en_US&attachment[params][og_info][properties][4][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][4][1]=www&attachment[params][og_info][properties][5][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][5][1]=es_LA&attachment[params][og_info][properties][6][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][6][1]=es_ES&attachment[params][og_info][properties][7][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][7][1]=fr_FR&attachment[params][og_info][properties][8][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][8][1]=it_IT&attachment[params][og_info][properties][9][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][9][1]=id_ID&attachment[params][og_info][properties][10][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][10][1]=th_TH&attachment[params][og_info][properties][11][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][11][1]=vi_VN&attachment[params][og_info][properties][12][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][12][1]=ko_KR&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&&attachment[params][redirectPath][0][status]=302&attachment[params][redirectPath][0][url]=https%3A%2F%2Fwww.facebook.com%2F&&&&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&composer_session_id=73f5680b-7e2b-4a33-82f3-265a831b42f7&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&&&privacyx&ref=timeline&tagger_session_id=1437476797&target_type=wall&&xhpc_message=Hello%3Awww.facebook.com&xhpc_message_text=Hello%3Awww.facebook.com&is_react=true&xhpc_composerid=rc.u_0_17&xhpc_targetid=100004478132093&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&__user=100004306477265&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxvyUWdwIhEoyUnwPUS8wzxi5e49UnEwy8J6xu9qzEKUyVWz9Epw&__req=l&fb_dtsg=AQHVElDXkwn4&ttstamp=26581728669108688810711911052&__rev=1843986

									//try
									//{
									//    string UrlPostDataValue = HttpHelper.postFormData(new Uri(UrlThumbnail), PostDataThumbNail);
									//}
									//catch { };

								}


							}
							catch { };









							if (messagesList.Count() > 1)
							{



								wallmessage = Uri.EscapeUriString(wallmessage);
								string xhpc_message_text = "";
								string FirstResponse = "";
								try
								{
									string PostDataUrl = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UsreId + "&composerurihash=1";
									string PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
									FirstResponse = HttpHelper.postFormData(new Uri(PostDataUrl), PostData);

									if (FirstResponse.Contains("Who are you with?"))
									{
										string Post_Url = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + UsreId + "&composerurihash=1";
										string PostData_Url = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=backdateicon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=backdateicon&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqA8ABGeqrWo8pojByUWdDx2ubhHximmey8qUS8zU&__req=e&ttstamp=26581729512056122661171216683&__rev=1503785";
										FirstResponse = HttpHelper.postFormData(new Uri(Post_Url), PostData_Url);
										if (!FirstResponse.Contains("Who are you with?"))
										{
											string Post_Url2 = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + UsreId + "&composerurihash=1";
											string PostData_Url2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&photoswaterfallid=29f9db5dfb9b52c5a4a760ee4510ea07&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=ogtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=withtaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=backdateicon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=aJioznEyl2qm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1e&ttstamp=2658170679789798165112110106695577&__rev=1612042";
											FirstResponse = HttpHelper.postFormData(new Uri(Post_Url2), PostData_Url2);
										}
									}
								}
								catch (Exception ex)
								{
									// GlobusLogHelper.log.Error(ex.StackTrace);
								}



								string SecondResponse = "";

								if (FirstResponse.Contains("Sorry, we got confused"))
								{
									try
									{
										FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UsreId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UsreId + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
									}
									catch (Exception ex)
									{
										// GlobusLogHelper.log.Error(ex.StackTrace);
									}
								}



								//string[] messagesList = {};

								// try
								// {
								//      messagesList = Regex.Split(wallmessage, ":");

								//     if (messagesList.Count() > 1)
								//     {
								//         string UrlMessage = messagesList[1];



								//         string UrlThumbnail = "https://www.facebook.com/react_composer/scraper/?composer_id=" + xhpc_composerid + "&target_id=" + xhpc_targetid + "&scrape_url=" + UrlMessage;


								//         string PostDataThumbNail = "__user=" + UsreId + "&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxCbzES2N6xybxu3fzoy2e58kUgDxuy28yQq5UBGeyXybDGcCxC&__req=g&fb_dtsg=" + fb_dtsg + "&ttstamp=" + "265817110611280100771201176785" + "&__rev=" + 1843986;

								//         //&attachment[params][urlInfo][canonical]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][final]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437426144]=http%3A%2F%2Ffacebook.com%2F&attachment[params][urlInfo][log][1437450198]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437450230]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][responseCode]=200&attachment[params][favicon]=https%3A%2F%2Ffbstatic-a.akamaihd.net%2Frsrc.php%2FyV%2Fr%2FhzMapiNYYpW.ico&&attachment[params][title]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][summary]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][content_removed]=false&attachment[params][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&&attachment[params][ranked_images][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][image_info][0][width]=325&attachment[params][image_info][0][height]=325&&attachment[params][image_info][0][xray][overlaid_text]=0.1731&attachment[params][image_info][0][xray][synthetic]=0.5766&attachment[params][image_info][0][xray][scores][437978556329078]=0.014&attachment[params][image_info][0][xray][scores][976885115686468]=0.2174&attachment[params][video_info][duration]=0&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][domain_ip]=2a03%3A2880%3A2050%3A3f07%3Aface%3Ab00c%3A0%3A1&attachment[params][time_scraped]=1437476787&attachment[params][cache_hit]=true&attachment[params][global_share_id]=6976353357&attachment[params][was_recent]=false&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][charset]=utf-8&attachment[params][metaTagMap][2][name]=referrer&attachment[params][metaTagMap][2][content]=default&attachment[params][metaTagMap][2][id]=meta_referrer&attachment[params][metaTagMap][3][property]=og%3Asite_name&attachment[params][metaTagMap][3][content]=Facebook&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][metaTagMap][5][property]=og%3Aimage&attachment[params][metaTagMap][5][content]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][metaTagMap][6][property]=og%3Alocale&attachment[params][metaTagMap][6][content]=en_US&attachment[params][metaTagMap][7][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][7][content]=www&attachment[params][metaTagMap][8][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][8][content]=es_LA&attachment[params][metaTagMap][9][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][9][content]=es_ES&attachment[params][metaTagMap][10][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][10][content]=fr_FR&attachment[params][metaTagMap][11][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][11][content]=it_IT&attachment[params][metaTagMap][12][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][12][content]=id_ID&attachment[params][metaTagMap][13][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][13][content]=th_TH&attachment[params][metaTagMap][14][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][14][content]=vi_VN&attachment[params][metaTagMap][15][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][15][content]=ko_KR&attachment[params][metaTagMap][16][name]=description&attachment[params][metaTagMap][16][content]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][metaTagMap][17][name]=robots&attachment[params][metaTagMap][17][content]=noodp%2Cnoydir&&attachment[params][og_info][properties][0][0]=og%3Asite_name&attachment[params][og_info][properties][0][1]=Facebook&attachment[params][og_info][properties][1][0]=og%3Aurl&attachment[params][og_info][properties][1][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][properties][2][0]=og%3Aimage&attachment[params][og_info][properties][2][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][properties][3][0]=og%3Alocale&attachment[params][og_info][properties][3][1]=en_US&attachment[params][og_info][properties][4][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][4][1]=www&attachment[params][og_info][properties][5][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][5][1]=es_LA&attachment[params][og_info][properties][6][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][6][1]=es_ES&attachment[params][og_info][properties][7][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][7][1]=fr_FR&attachment[params][og_info][properties][8][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][8][1]=it_IT&attachment[params][og_info][properties][9][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][9][1]=id_ID&attachment[params][og_info][properties][10][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][10][1]=th_TH&attachment[params][og_info][properties][11][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][11][1]=vi_VN&attachment[params][og_info][properties][12][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][12][1]=ko_KR&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&&attachment[params][redirectPath][0][status]=302&attachment[params][redirectPath][0][url]=https%3A%2F%2Fwww.facebook.com%2F&&&&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&composer_session_id=73f5680b-7e2b-4a33-82f3-265a831b42f7&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&&&privacyx&ref=timeline&tagger_session_id=1437476797&target_type=wall&&xhpc_message=Hello%3Awww.facebook.com&xhpc_message_text=Hello%3Awww.facebook.com&is_react=true&xhpc_composerid=rc.u_0_17&xhpc_targetid=100004478132093&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&__user=100004306477265&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxvyUWdwIhEoyUnwPUS8wzxi5e49UnEwy8J6xu9qzEKUyVWz9Epw&__req=l&fb_dtsg=AQHVElDXkwn4&ttstamp=26581728669108688810711911052&__rev=1843986

								//         try
								//         {
								//             string UrlPostDataValue = HttpHelper.postFormData(new Uri(UrlThumbnail), PostDataThumbNail);
								//         }
								//         catch { };

								//     }


								// }
								// catch { };


								try
								{
									SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UsreId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
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
											xhpc_message_text = arr[0];
											wallmessage = arr[1] + ":" + arr[2];
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

								if (false)
								{
									int index = 0;
									foreach (string item in lstWallPostURLsWallPoster)
									{
										if (wallmessage.Equals(item))
										{
											break;
										}
										index++;
									}
									//  attachment_params_title = lstWallPostURLsTitles[index];
									//  attachment_params_summary = lstWallPostURLsSummaries[index];
									if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_3))
									{
										attachment_params_images0 = attachment_params_ranked_images_images_3;
									}
									else
									{
										if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_2))
										{
											attachment_params_images0 = attachment_params_ranked_images_images_2;
										}
										else
										{
											if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_1))
											{
												attachment_params_images0 = attachment_params_ranked_images_images_1;
											}
										}
									}
								}
								string ResponseWallPost = "";
								if (wallmessage.Contains(":"))
								{
									wallmessage = wallmessage.Replace(":", " ");
								}

								if (string.IsNullOrEmpty(FirstResponse))
								{

									string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);

									//string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559";
									//ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);
								}
								else
								{

									string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);
								}
								if (ResponseWallPost.Contains("The message could not be posted to this Wall.") || ResponseWallPost.Contains("Message Failed\",\"errorDescription\"") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action"))
								{
									// chkWallWallPosterRemoveURLsMessages Url

									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
								}
								else if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
								}
								else if (ResponseWallPost.Contains("Sorry, the privacy setting on this post means that you can't share it") && ResponseWallPost.Contains("Could Not Post to Timeline"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UsreId), "composer_session_id=c9e72d37-ce06-40d8-a3f3-b35c8316bcbd&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229dbcb61a%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1e%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073742507&attachment[type]=2&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEyl2lm9udDgDxyKAEWCueyp9Esx6iWF299qzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=26581729512056122661171216683&__rev=1503785");
								}
								else if (ResponseWallPost.Contains("The message could not be posted to this Wall.") && ResponseWallPost.Contains("The message could not be posted to this Wall"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UsreId), "composer_session_id=2f37c190-d9b1-4d18-aa9d-f4d3d85e687d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2227babdd5%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_4_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=7&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1k&ttstamp=2658170679789798165112110106695577&__rev=1612042");
								}
								else
								{
									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
								}

								try
								{

									int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
									AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep(delayInSeconds);
								}
								catch { };



								// ResponseWallPost1 = ResponseWallPost;

							}

							else
							{

								string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";

								// string postDataWalllpost111New = "&attachment[params][urlInfo][canonical]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][final]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437426144]=http%3A%2F%2Ffacebook.com%2F&attachment[params][urlInfo][log][1437450198]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437450230]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][responseCode]=200&attachment[params][favicon]=https%3A%2F%2Ffbstatic-a.akamaihd.net%2Frsrc.php%2FyV%2Fr%2FhzMapiNYYpW.ico&&attachment[params][title]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][summary]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][content_removed]=false&attachment[params][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&&attachment[params][ranked_images][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][image_info][0][width]=325&attachment[params][image_info][0][height]=325&&attachment[params][image_info][0][xray][overlaid_text]=0.1731&attachment[params][image_info][0][xray][synthetic]=0.5766&attachment[params][image_info][0][xray][scores][437978556329078]=0.014&attachment[params][image_info][0][xray][scores][976885115686468]=0.2174&attachment[params][video_info][duration]=0&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][domain_ip]=2a03%3A2880%3A2050%3A3f07%3Aface%3Ab00c%3A0%3A1&attachment[params][time_scraped]=1437476787&attachment[params][cache_hit]=true&attachment[params][global_share_id]=6976353357&attachment[params][was_recent]=false&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][charset]=utf-8&attachment[params][metaTagMap][2][name]=referrer&attachment[params][metaTagMap][2][content]=default&attachment[params][metaTagMap][2][id]=meta_referrer&attachment[params][metaTagMap][3][property]=og%3Asite_name&attachment[params][metaTagMap][3][content]=Facebook&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][metaTagMap][5][property]=og%3Aimage&attachment[params][metaTagMap][5][content]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][metaTagMap][6][property]=og%3Alocale&attachment[params][metaTagMap][6][content]=en_US&attachment[params][metaTagMap][7][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][7][content]=www&attachment[params][metaTagMap][8][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][8][content]=es_LA&attachment[params][metaTagMap][9][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][9][content]=es_ES&attachment[params][metaTagMap][10][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][10][content]=fr_FR&attachment[params][metaTagMap][11][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][11][content]=it_IT&attachment[params][metaTagMap][12][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][12][content]=id_ID&attachment[params][metaTagMap][13][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][13][content]=th_TH&attachment[params][metaTagMap][14][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][14][content]=vi_VN&attachment[params][metaTagMap][15][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][15][content]=ko_KR&attachment[params][metaTagMap][16][name]=description&attachment[params][metaTagMap][16][content]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][metaTagMap][17][name]=robots&attachment[params][metaTagMap][17][content]=noodp%2Cnoydir&&attachment[params][og_info][properties][0][0]=og%3Asite_name&attachment[params][og_info][properties][0][1]=Facebook&attachment[params][og_info][properties][1][0]=og%3Aurl&attachment[params][og_info][properties][1][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][properties][2][0]=og%3Aimage&attachment[params][og_info][properties][2][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][properties][3][0]=og%3Alocale&attachment[params][og_info][properties][3][1]=en_US&attachment[params][og_info][properties][4][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][4][1]=www&attachment[params][og_info][properties][5][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][5][1]=es_LA&attachment[params][og_info][properties][6][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][6][1]=es_ES&attachment[params][og_info][properties][7][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][7][1]=fr_FR&attachment[params][og_info][properties][8][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][8][1]=it_IT&attachment[params][og_info][properties][9][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][9][1]=id_ID&attachment[params][og_info][properties][10][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][10][1]=th_TH&attachment[params][og_info][properties][11][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][11][1]=vi_VN&attachment[params][og_info][properties][12][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][12][1]=ko_KR&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&&attachment[params][redirectPath][0][status]=302&attachment[params][redirectPath][0][url]=https%3A%2F%2Fwww.facebook.com%2F&&&&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&composer_session_id=73f5680b-7e2b-4a33-82f3-265a831b42f7&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&&&privacyx&ref=timeline&tagger_session_id=1437476797&target_type=wall&&xhpc_message=Hello%3Awww.facebook.com&xhpc_message_text=Hello%3Awww.facebook.com&is_react=true&xhpc_composerid=rc.u_0_17&xhpc_targetid=100004478132093&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&__user=100004306477265&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxvyUWdwIhEoyUnwPUS8wzxi5e49UnEwy8J6xu9qzEKUyVWz9Epw&__req=l&fb_dtsg=AQHVElDXkwn4&ttstamp=26581728669108688810711911052&__rev=1843986";
								string ResponseWallPost = "";
								ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111);
								int length = ResponseWallPost.Length;

								string postDataWalllpost1112 = string.Empty;
								string ResponseWallPost2 = string.Empty;
								if (!(length > 1100))
								{
									postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
									ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112);

									int length2 = ResponseWallPost2.Length;
									if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
									{
										TotalNoOfWallPoster_Counter++;

										AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


										countWallPoster++;

										try
										{

											int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
											AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}

										try
										{
											#region insertQuery
											//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
											//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
											#endregion
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}
									}
									else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
									{
										TotalNoOfWallPoster_Counter++;
										AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

										countWallPoster++;

										try
										{

											int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
											AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}

										try
										{
											#region insertQuery
											//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
											//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
											#endregion
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}
									}
									else
									{
										string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost2);
										AddToLogger_FriendsManager("Error : " + errorSummary + " not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

									}
								}
								else
								{
									TotalNoOfWallPoster_Counter++;

									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


									countWallPoster++;

									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

									try
									{
										#region insertQuery
										//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
										//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
										#endregion
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

								System.Threading.Thread.Sleep(4000);
							}
						}
						else
						{
							AddToLogger_FriendsManager("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);


							System.Threading.Thread.Sleep(1000);
						}
					}
				}
				else
				{
					try
					{
						//PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UsreId);
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




















		private void PostOnFriendsWallTestMessage(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UsreId)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string friendid = friendId;
				string wallMessage = wallmessage;
				DateTime datetiemvalue = DateTime.Now;
				TimeSpan xcx = DateTime.Now - datetiemvalue;

				if (!statusForGreetingMsgWallPoster)
				{
					string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";

					if (postUrl.Contains("https://"))
					{
						postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
						string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl),"","");

						if (pageSourceWallPost11.Contains("fb_dtsg") && (pageSourceWallPost11.Contains("xhpc_composerid") || pageSourceWallPost11.ToLower().Contains("composerid")) && (pageSourceWallPost11.Contains("xhpc_targetid") || pageSourceWallPost11.ToLower().Contains("targetid")))
						{
							AddToLogger_FriendsManager(countWallPoster.ToString() + " Posting on wall " + postUrl);


							string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);//pageSourceHome.Substring(pageSourceHome.IndexOf("fb_dtsg") + 16, 8);

							string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");

							if (string.IsNullOrEmpty(xhpc_composerid))
							{
								try
								{
									xhpc_composerid = GlobusHttpHelper.getBetween(pageSourceWallPost11, "composerID\":\"","\"");
									if (xhpc_composerid.Contains("\""))
									{
										xhpc_composerid = xhpc_composerid.Replace("\"", "");
									}
								}
								catch { };

							}

							string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");



							if (string.IsNullOrEmpty(xhpc_targetid))
							{
								try
								{
									xhpc_targetid = GlobusHttpHelper.getBetween(pageSourceWallPost11, "targetID\":", "}");
									if (xhpc_targetid.Contains("\""))
									{
										xhpc_targetid = xhpc_targetid.Replace("\"", "");

									}
								}
								catch { };

							}
							string appid = "";
							try
							{
								appid = FBUtils.getBetween(pageSourceWallPost11, "appid=", "&");
							}
							catch { };


							string[] messagesList = { };

							try
							{
								messagesList = Regex.Split(wallmessage, ":");

								if (messagesList.Count() > 1)
								{
									//string UrlMessage = messagesList[1];



									//string UrlThumbnail = "https://www.facebook.com/react_composer/scraper/?composer_id=" + xhpc_composerid + "&target_id=" + xhpc_targetid + "&scrape_url=" + UrlMessage;


									//string PostDataThumbNail = "__user=" + UsreId + "&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxCbzES2N6xybxu3fzoy2e58kUgDxuy28yQq5UBGeyXybDGcCxC&__req=g&fb_dtsg=" + fb_dtsg + "&ttstamp=" + "265817110611280100771201176785" + "&__rev=" + 1843986;

									////&attachment[params][urlInfo][canonical]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][final]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437426144]=http%3A%2F%2Ffacebook.com%2F&attachment[params][urlInfo][log][1437450198]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437450230]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][responseCode]=200&attachment[params][favicon]=https%3A%2F%2Ffbstatic-a.akamaihd.net%2Frsrc.php%2FyV%2Fr%2FhzMapiNYYpW.ico&&attachment[params][title]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][summary]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][content_removed]=false&attachment[params][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&&attachment[params][ranked_images][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][image_info][0][width]=325&attachment[params][image_info][0][height]=325&&attachment[params][image_info][0][xray][overlaid_text]=0.1731&attachment[params][image_info][0][xray][synthetic]=0.5766&attachment[params][image_info][0][xray][scores][437978556329078]=0.014&attachment[params][image_info][0][xray][scores][976885115686468]=0.2174&attachment[params][video_info][duration]=0&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][domain_ip]=2a03%3A2880%3A2050%3A3f07%3Aface%3Ab00c%3A0%3A1&attachment[params][time_scraped]=1437476787&attachment[params][cache_hit]=true&attachment[params][global_share_id]=6976353357&attachment[params][was_recent]=false&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][charset]=utf-8&attachment[params][metaTagMap][2][name]=referrer&attachment[params][metaTagMap][2][content]=default&attachment[params][metaTagMap][2][id]=meta_referrer&attachment[params][metaTagMap][3][property]=og%3Asite_name&attachment[params][metaTagMap][3][content]=Facebook&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][metaTagMap][5][property]=og%3Aimage&attachment[params][metaTagMap][5][content]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][metaTagMap][6][property]=og%3Alocale&attachment[params][metaTagMap][6][content]=en_US&attachment[params][metaTagMap][7][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][7][content]=www&attachment[params][metaTagMap][8][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][8][content]=es_LA&attachment[params][metaTagMap][9][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][9][content]=es_ES&attachment[params][metaTagMap][10][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][10][content]=fr_FR&attachment[params][metaTagMap][11][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][11][content]=it_IT&attachment[params][metaTagMap][12][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][12][content]=id_ID&attachment[params][metaTagMap][13][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][13][content]=th_TH&attachment[params][metaTagMap][14][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][14][content]=vi_VN&attachment[params][metaTagMap][15][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][15][content]=ko_KR&attachment[params][metaTagMap][16][name]=description&attachment[params][metaTagMap][16][content]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][metaTagMap][17][name]=robots&attachment[params][metaTagMap][17][content]=noodp%2Cnoydir&&attachment[params][og_info][properties][0][0]=og%3Asite_name&attachment[params][og_info][properties][0][1]=Facebook&attachment[params][og_info][properties][1][0]=og%3Aurl&attachment[params][og_info][properties][1][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][properties][2][0]=og%3Aimage&attachment[params][og_info][properties][2][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][properties][3][0]=og%3Alocale&attachment[params][og_info][properties][3][1]=en_US&attachment[params][og_info][properties][4][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][4][1]=www&attachment[params][og_info][properties][5][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][5][1]=es_LA&attachment[params][og_info][properties][6][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][6][1]=es_ES&attachment[params][og_info][properties][7][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][7][1]=fr_FR&attachment[params][og_info][properties][8][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][8][1]=it_IT&attachment[params][og_info][properties][9][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][9][1]=id_ID&attachment[params][og_info][properties][10][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][10][1]=th_TH&attachment[params][og_info][properties][11][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][11][1]=vi_VN&attachment[params][og_info][properties][12][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][12][1]=ko_KR&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&&attachment[params][redirectPath][0][status]=302&attachment[params][redirectPath][0][url]=https%3A%2F%2Fwww.facebook.com%2F&&&&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&composer_session_id=73f5680b-7e2b-4a33-82f3-265a831b42f7&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&&&privacyx&ref=timeline&tagger_session_id=1437476797&target_type=wall&&xhpc_message=Hello%3Awww.facebook.com&xhpc_message_text=Hello%3Awww.facebook.com&is_react=true&xhpc_composerid=rc.u_0_17&xhpc_targetid=100004478132093&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&__user=100004306477265&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxvyUWdwIhEoyUnwPUS8wzxi5e49UnEwy8J6xu9qzEKUyVWz9Epw&__req=l&fb_dtsg=AQHVElDXkwn4&ttstamp=26581728669108688810711911052&__rev=1843986

									//try
									//{
									//    string UrlPostDataValue = HttpHelper.postFormData(new Uri(UrlThumbnail), PostDataThumbNail);
									//}
									//catch { };

								}


							}
							catch { };









							//   if (messagesList.Count() > 1)
							if (true)
							{



								wallmessage = Uri.EscapeUriString(wallmessage);
								string xhpc_message_text = "";
								string FirstResponse = "";
								try
								{
									string PostDataUrl = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UsreId + "&composerurihash=1";
									string PostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
									FirstResponse = HttpHelper.postFormData(new Uri(PostDataUrl), PostData);

									if (FirstResponse.Contains("Who are you with?"))
									{
										string Post_Url = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + UsreId + "&composerurihash=1";
										string PostData_Url = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=backdateicon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=backdateicon&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqA8ABGeqrWo8pojByUWdDx2ubhHximmey8qUS8zU&__req=e&ttstamp=26581729512056122661171216683&__rev=1503785";
										FirstResponse = HttpHelper.postFormData(new Uri(Post_Url), PostData_Url);
										if (!FirstResponse.Contains("Who are you with?"))
										{
											string Post_Url2 = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av=" + UsreId + "&composerurihash=1";
											string PostData_Url2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&composercontext=composer&photoswaterfallid=29f9db5dfb9b52c5a4a760ee4510ea07&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=ogtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=backdateicon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=withtaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=backdateicon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=aJioznEyl2qm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1e&ttstamp=2658170679789798165112110106695577&__rev=1612042";
											FirstResponse = HttpHelper.postFormData(new Uri(Post_Url2), PostData_Url2);
										}
									}
								}
								catch (Exception ex)
								{
									// GlobusLogHelper.log.Error(ex.StackTrace);
								}



								string SecondResponse = "";

								if (FirstResponse.Contains("Sorry, we got confused"))
								{
									try
									{
										FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UsreId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UsreId + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
									}
									catch (Exception ex)
									{
										// GlobusLogHelper.log.Error(ex.StackTrace);
									}
								}





								try
								{


									SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UsreId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
								}
								catch (Exception ex)
								{
									// GlobusLogHelper.log.Error(ex.StackTrace);
								}


								if (wallmessage.Contains("https://") || wallmessage.Contains("http://"))
								{
									if (wallmessage.Contains("https://"))
									{
										wallmessage = wallmessage.Replace("https://", "");

									}
									else if (wallmessage.Contains("http://"))
									{
										wallmessage = wallmessage.Replace("http://", "");

									}

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
											xhpc_message_text = arr[0];
											wallmessage = arr[1] + ":" + arr[2];
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

								if (false)
								{
									int index = 0;
									foreach (string item in lstWallPostURLsWallPoster)
									{
										if (wallmessage.Equals(item))
										{
											break;
										}
										index++;
									}
									//  attachment_params_title = lstWallPostURLsTitles[index];
									//  attachment_params_summary = lstWallPostURLsSummaries[index];
									if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_3))
									{
										attachment_params_images0 = attachment_params_ranked_images_images_3;
									}
									else
									{
										if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_2))
										{
											attachment_params_images0 = attachment_params_ranked_images_images_2;
										}
										else
										{
											if (!string.IsNullOrEmpty(attachment_params_ranked_images_images_1))
											{
												attachment_params_images0 = attachment_params_ranked_images_images_1;
											}
										}
									}
								}
								string ResponseWallPost = "";
								if (wallmessage.Contains(":"))
								{
									wallmessage = wallmessage.Replace(":", " ");
								}

								if (string.IsNullOrEmpty(SecondResponse))
								{

									// string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									string PostData = "&attachment&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&boosted_post_config&composer_session_id=94cba319-8335-4b53-8d0d-e9083a610c6d&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&multilingual_specified_lang=&&&privacyx&ref=timeline&tagger_session_id=1443716984&target_type=wall&&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&is_react=true&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&xhpc_fundraiser_page=false&__user=" + UsreId + "&__a=1&__dyn=7AmajEyl2lm9ongDxiWEB19CzEWq2WiWF298yut9LHwxBxCbzFVob4q68K5Uc-dy88axbxjx2u5W88ybAG5VGqzE-8KuEOq6ouAxO2OE&__req=1j&fb_dtsg=" + fb_dtsg + "&ttstamp=26581721005178848611411310377&__rev=1965820";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);

									//string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559";
									//ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);
								}
								else
								{
									string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), PostData);
								}
								if (ResponseWallPost.Contains("The message could not be posted to this Wall.") || ResponseWallPost.Contains("Message Failed\",\"errorDescription\"") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action"))
								{
									// chkWallWallPosterRemoveURLsMessages Url

									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
								}
								else if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UsreId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
								}
								else if (ResponseWallPost.Contains("Sorry, the privacy setting on this post means that you can't share it") && ResponseWallPost.Contains("Could Not Post to Timeline"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UsreId), "composer_session_id=c9e72d37-ce06-40d8-a3f3-b35c8316bcbd&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229dbcb61a%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1e%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073742507&attachment[type]=2&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n8ajEyl2lm9udDgDxyKAEWCueyp9Esx6iWF299qzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=26581729512056122661171216683&__rev=1503785");
								}
								else if (ResponseWallPost.Contains("The message could not be posted to this Wall.") && ResponseWallPost.Contains("The message could not be posted to this Wall"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UsreId), "composer_session_id=2f37c190-d9b1-4d18-aa9d-f4d3d85e687d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2227babdd5%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_4_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=7&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=aJioznEyl2lm9adDgDDzbHbh8x9VoW9J6yUgByVblkGGhbHBCqrYyy8lBxdbWAVbGFQiuaBKAqhBUFJdALhVpqCGuaCV8yfCU9UgAAz8yE&__req=1k&ttstamp=2658170679789798165112110106695577&__rev=1612042");
								}//errorSummary":"Security Check Required"
								else if (ResponseWallPost.Contains("errorSummary\":\"Security Check Required\""))
								{
									AddToLogger_FriendsManager("FB asking For Sucurity to Post on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
								}

								else
								{
									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);
								}

								try
								{

									int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
									AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep(delayInSeconds);
								}
								catch { };



								// ResponseWallPost1 = ResponseWallPost;

							}

							else
							{

								string postDataWalllpost111 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=home&xhpc_fbx=1&xhpc_timeline=&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&audience[0][value]=80&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_composer&__user=" + UsreId + "&phstamp=";

								// string postDataWalllpost111New = "&attachment[params][urlInfo][canonical]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][final]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][user]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437426144]=http%3A%2F%2Ffacebook.com%2F&attachment[params][urlInfo][log][1437450198]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][urlInfo][log][1437450230]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][responseCode]=200&attachment[params][favicon]=https%3A%2F%2Ffbstatic-a.akamaihd.net%2Frsrc.php%2FyV%2Fr%2FhzMapiNYYpW.ico&&attachment[params][title]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][summary]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][content_removed]=false&attachment[params][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&&attachment[params][ranked_images][images][0]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][ranked_images][ranking_model_version]=10&attachment[params][image_info][0][url]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][image_info][0][width]=325&attachment[params][image_info][0][height]=325&&attachment[params][image_info][0][xray][overlaid_text]=0.1731&attachment[params][image_info][0][xray][synthetic]=0.5766&attachment[params][image_info][0][xray][scores][437978556329078]=0.014&attachment[params][image_info][0][xray][scores][976885115686468]=0.2174&attachment[params][video_info][duration]=0&attachment[params][medium]=106&attachment[params][url]=http%3A%2F%2Fwww.facebook.com%2F&attachment[params][domain_ip]=2a03%3A2880%3A2050%3A3f07%3Aface%3Ab00c%3A0%3A1&attachment[params][time_scraped]=1437476787&attachment[params][cache_hit]=true&attachment[params][global_share_id]=6976353357&attachment[params][was_recent]=false&attachment[params][metaTagMap][0][http-equiv]=content-type&attachment[params][metaTagMap][0][content]=text%2Fhtml%3B%20charset%3Dutf-8&attachment[params][metaTagMap][1][charset]=utf-8&attachment[params][metaTagMap][2][name]=referrer&attachment[params][metaTagMap][2][content]=default&attachment[params][metaTagMap][2][id]=meta_referrer&attachment[params][metaTagMap][3][property]=og%3Asite_name&attachment[params][metaTagMap][3][content]=Facebook&attachment[params][metaTagMap][4][property]=og%3Aurl&attachment[params][metaTagMap][4][content]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][metaTagMap][5][property]=og%3Aimage&attachment[params][metaTagMap][5][content]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][metaTagMap][6][property]=og%3Alocale&attachment[params][metaTagMap][6][content]=en_US&attachment[params][metaTagMap][7][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][7][content]=www&attachment[params][metaTagMap][8][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][8][content]=es_LA&attachment[params][metaTagMap][9][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][9][content]=es_ES&attachment[params][metaTagMap][10][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][10][content]=fr_FR&attachment[params][metaTagMap][11][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][11][content]=it_IT&attachment[params][metaTagMap][12][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][12][content]=id_ID&attachment[params][metaTagMap][13][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][13][content]=th_TH&attachment[params][metaTagMap][14][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][14][content]=vi_VN&attachment[params][metaTagMap][15][property]=og%3Alocale%3Aalternate&attachment[params][metaTagMap][15][content]=ko_KR&attachment[params][metaTagMap][16][name]=description&attachment[params][metaTagMap][16][content]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][metaTagMap][17][name]=robots&attachment[params][metaTagMap][17][content]=noodp%2Cnoydir&&attachment[params][og_info][properties][0][0]=og%3Asite_name&attachment[params][og_info][properties][0][1]=Facebook&attachment[params][og_info][properties][1][0]=og%3Aurl&attachment[params][og_info][properties][1][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][properties][2][0]=og%3Aimage&attachment[params][og_info][properties][2][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][properties][3][0]=og%3Alocale&attachment[params][og_info][properties][3][1]=en_US&attachment[params][og_info][properties][4][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][4][1]=www&attachment[params][og_info][properties][5][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][5][1]=es_LA&attachment[params][og_info][properties][6][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][6][1]=es_ES&attachment[params][og_info][properties][7][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][7][1]=fr_FR&attachment[params][og_info][properties][8][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][8][1]=it_IT&attachment[params][og_info][properties][9][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][9][1]=id_ID&attachment[params][og_info][properties][10][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][10][1]=th_TH&attachment[params][og_info][properties][11][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][11][1]=vi_VN&attachment[params][og_info][properties][12][0]=og%3Alocale%3Aalternate&attachment[params][og_info][properties][12][1]=ko_KR&attachment[params][og_info][guesses][0][0]=og%3Aurl&attachment[params][og_info][guesses][0][1]=https%3A%2F%2Fwww.facebook.com%2F&attachment[params][og_info][guesses][1][0]=og%3Atitle&attachment[params][og_info][guesses][1][1]=Welcome%20to%20Facebook%20-%20Log%20In%2C%20Sign%20Up%20or%20Learn%20More&attachment[params][og_info][guesses][2][0]=og%3Adescription&attachment[params][og_info][guesses][2][1]=Facebook%20is%20a%20social%20utility%20that%20connects%20people%20with%20friends%20and%20others%20who%20work%2C%20study%20and%20live%20around%20them.%20People%20use%20Facebook%20to%20keep%20up%20with...&attachment[params][og_info][guesses][3][0]=og%3Aimage&attachment[params][og_info][guesses][3][1]=https%3A%2F%2Fwww.facebook.com%2Fimages%2Ffb_icon_325x325.png&attachment[params][og_info][guesses][4][0]=og%3Alocale&attachment[params][og_info][guesses][4][1]=en&&attachment[params][redirectPath][0][status]=302&attachment[params][redirectPath][0][url]=https%3A%2F%2Fwww.facebook.com%2F&&&&attachment[params][ttl]=604800&attachment[params][error]=1&attachment[type]=100&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&composer_session_id=73f5680b-7e2b-4a33-82f3-265a831b42f7&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&&&privacyx&ref=timeline&tagger_session_id=1437476797&target_type=wall&&xhpc_message=Hello%3Awww.facebook.com&xhpc_message_text=Hello%3Awww.facebook.com&is_react=true&xhpc_composerid=rc.u_0_17&xhpc_targetid=100004478132093&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&__user=100004306477265&__a=1&__dyn=7AmajEyl2lm9o-t2u5bGyk4Au7pEsx6iWF298yut9LHwxBxvyUWdwIhEoyUnwPUS8wzxi5e49UnEwy8J6xu9qzEKUyVWz9Epw&__req=l&fb_dtsg=AQHVElDXkwn4&ttstamp=26581728669108688810711911052&__rev=1843986";
								string ResponseWallPost = "";
								ResponseWallPost = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost111);
								int length = ResponseWallPost.Length;

								string postDataWalllpost1112 = string.Empty;
								string ResponseWallPost2 = string.Empty;
								if (!(length > 1100))
								{
									postDataWalllpost1112 = "fb_dtsg=" + fb_dtsg + "&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_fbx=&xhpc_timeline=1&xhpc_ismeta=1&xhpc_message_text=" + wallmessage + "&xhpc_message=" + wallmessage + "&composertags_place=&composertags_place_name=&composer_predicted_city=&composer_session_id=&is_explicit_place=&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&composertags_city=&disable_location_sharing=false&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&phstamp=";
									ResponseWallPost2 = HttpHelper.postFormData(new Uri(FBGlobals.Instance.WallPosterPostAjaxUpdateStatusUrl), postDataWalllpost1112);

									int length2 = ResponseWallPost2.Length;
									if (length > 11000 && ResponseWallPost.Contains("jsmods") && ResponseWallPost.Contains("XHPTemplate"))
									{
										TotalNoOfWallPoster_Counter++;

										AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


										countWallPoster++;

										try
										{

											int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
											AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}

										try
										{
											#region insertQuery
											//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
											//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
											#endregion
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}
									}
									else if (length2 > 11000 && ResponseWallPost2.Contains("jsmods") && ResponseWallPost2.Contains("XHPTemplate"))
									{
										TotalNoOfWallPoster_Counter++;
										AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

										countWallPoster++;

										try
										{

											int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
											AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

											Thread.Sleep(delayInSeconds);
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}

										try
										{
											#region insertQuery
											//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
											//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster"); 
											#endregion
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
										}
									}
									else
									{
										string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost2);
										AddToLogger_FriendsManager("Error : " + errorSummary + " not Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);

									}
								}
								else
								{
									TotalNoOfWallPoster_Counter++;

									AddToLogger_FriendsManager("Posted on Friend's wall :" + postUrl + " With Username : " + fbUser.username);


									countWallPoster++;

									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

									try
									{
										#region insertQuery
										//string insertQuery = "insert into tb_ManageWallPoster (UserName,FriendId,DateTime) values('" + Username + "','" + friendid + "','" + DateTime.Now.ToString("MM/dd/yyyy") + "')";
										//BaseLib.DataBaseHandler.InsertQuery(insertQuery, "tb_ManageWallPoster");
										#endregion
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

								System.Threading.Thread.Sleep(4000);
							}
						}
						else
						{
							AddToLogger_FriendsManager("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);


							System.Threading.Thread.Sleep(1000);
						}
					}
				}
				else
				{
					try
					{
						//PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UsreId);
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

























		public void WallPostingNew(ref FacebookUser fbUser)
		{
			try
			{
				string UserId = string.Empty;
				string attachmentParamsUrlInfoUser = string.Empty;
				string attachmentParamsUrlInfoCanonical = string.Empty;
				string attachmentParamsUrlInfoFinal = string.Empty;
				string attachmentParamsUrlInfoTitle = string.Empty;
				string attachmentParamsSummary = string.Empty;
				string attachmentParamsMedium = string.Empty;
				string attachmentParamsUrl = string.Empty;
				string attachmentType = string.Empty;
				string linkMetricsSource = string.Empty;
				string linkMetricsDomain = string.Empty;
				string linkMetricsBaseDomain = string.Empty;
				string linkMetricsTitleLen = string.Empty;
				string attachmentParamsfavicon = string.Empty;

				AddToLogger_FriendsManager("Start Wall Posting With Username : " + fbUser.username);


				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");



				string ProFilePost = FBGlobals.Instance.fbProfileUrl;
				string tempUserID = string.Empty;
				List<string> lstFriend = new List<string>();

				UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);

					return;
				}
				IsUseURLsMessageWallPoster=true;
				lstMessagesWallPoster = lstWallMessageWallPoster.Distinct().ToList();
				bool postsuccess = false;
				if (IsUseTextMessageWallPoster)
				{
					MsgWallPoster = lstWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallMessageWallPoster.Count)];
				}
				if (IsUseURLsMessageWallPoster)
				{
					MsgWallPoster = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
				}
				if (ChkSpinnerWallMessaeWallPoster)
				{
					MsgWallPoster = lstSpinnerWallMessageWallPoster[GlobusHttpHelper.GenerateRandom(0, lstSpinnerWallMessageWallPoster.Count)];
					lstMessagesWallPoster = lstSpinnerWallMessageWallPoster;
				}


				string profileUrl = ProFilePost + UserId + "&sk=wall";
				string pageSourceWallPostUser = HttpHelper.getHtmlfromUrl(new Uri(profileUrl),"","");
				string wallmessage = string.Empty;                                
				wallmessage = wallmessage.Replace("<friend first name>", string.Empty);

				if (pageSourceWallPostUser.Contains("fb_dtsg") && pageSourceWallPostUser.Contains("xhpc_composerid") && pageSourceWallPostUser.Contains("xhpc_targetid"))
				{
					if (lstWallPostURLsWallPoster.Count > 0)
					{
						wallmessage = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];

						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}
					else
					{
						AddToLogger_FriendsManager("Posting message on own wall: " + wallmessage);

					}

					wallmessage = wallmessage.Replace("=", "%3D");
					string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_Home);
					string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_composerid");
					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_composerid = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
					}

					string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPostUser, "xhpc_targetid");
					if (string.IsNullOrEmpty(fb_dtsg))
					{
						xhpc_targetid = GlobusHttpHelper.ParseJson(pageSourceWallPostUser, "xhpc_targetid");
					}
					string appid = GlobusHttpHelper.getBetween(pageSourceWallPostUser, "appid=", "&");
					string ResponseWallPost = string.Empty;
					string sessionId = GlobusHttpHelper.GenerateTimeStamp();                    
					//First Postdata
					string FirstResponse=string.Empty;
					string SecondResponse = string.Empty;
					if (!wallmessage.Contains("https://") && !wallmessage.Contains("http://"))
					{
						wallmessage = "https://" + wallmessage;
					}
					try
					{

						FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UserId + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=prompt&loaded_components[8]=withtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=ogtaggericon&loaded_components[11]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMCBynzpQ9UoHFaeFDzECQqbx2mbACFaaGGzCC_826m6oDAyoSnx2ubhHAG8Kl1e&__req=e&ttstamp=265817274821019054566657120&__rev=1400559");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
					if (FirstResponse.Contains("Sorry, we got confused"))
					{
						try
						{
							FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg="+fb_dtsg+"&composerid="+xhpc_composerid+"&targetid="+UserId+"&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user="+UserId+"&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
					}
					try
					{
						SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url="+Uri.EscapeDataString(wallmessage)+"&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av="+UserId+"&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + UserId + "&targetid=" + UserId + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=maininput&loaded_components[7]=prompt&loaded_components[8]=withtaggericon&loaded_components[9]=placetaggericon&loaded_components[10]=ogtaggericon&loaded_components[11]=mainprivacywidget&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBgjw&__req=f&ttstamp=265817274821019054566657120&__rev=1400559");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
					string tagger_session_id = GlobusHttpHelper.getBetween(FirstResponse, "tagger_session_id\\\" value=\\\"", "\\\"");
					string composer_predicted_city = GlobusHttpHelper.getBetween(FirstResponse, "composer_predicted_city\\\" value=\\\"","\\\"");
					string attachment_params = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
					string attachment_params_urlInfo_canonical = GlobusHttpHelper.getBetween(SecondResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_urlInfo_final = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_urlInfo_user = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_favicon = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_title = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_summary = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_images0 = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_medium = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_url = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_video0_type = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_video0_src = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_video0_width = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_video0_height = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_params_video0_secure_url = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string attachment_type = GlobusHttpHelper.getBetween(SecondResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_source = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_domain = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_base_domain = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_title_len = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_summary_len = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_min_dimensions0 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_min_dimensions1 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_with_dimensions = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_pending = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_fetched = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_image_dimensions0 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_image_dimensions1 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_considered = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_selected = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_cap = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string link_metrics_images_type = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
					string xhpc_message_text = wallmessage;
					if (chkWallWallPosterRemoveURLsMessages == true)
					{
						xhpc_message_text = wallmessage;


					}
					else
					{
						xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);
					}
					//Final PostData
					if (string.IsNullOrEmpty(FirstResponse))
					{
						ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
					}
					else
					{
						if (chkWallWallPosterRemoveURLsMessages)
						{

							if (xhpc_message_text.Contains("https:"))
							{
								string[] arr = xhpc_message_text.Split(':');
								if (arr.Count() == 3)
								{
									xhpc_message_text = arr[0];
								}
								else
								{
									xhpc_message_text = string.Empty;
								}     
							}
							else
							{

							}

						}

						ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + xhpc_message_text + "&xhpc_message_text=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
					}
					if (ResponseWallPost.Contains("The message could not be posted to this Wall.") ||ResponseWallPost.Contains("Couldn't Update Status") ||ResponseWallPost.Contains("You have been temporarily blocked from performing this action."))
					{
						ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
					}
					if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
					{
						ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
					}
					if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
					{

						string WallPostData = "composer_session_id=7a1d3f8c-ec77-4167-8ef6-5df4b1bc33aa&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229d966a62%22%2C%22clearcounter%22%3A1%2C%22elementid%22%3A%22u_0_w%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + UserId + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073742507&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_composer&__user="+UserId+"&__a=1&__dyn=7n8anEyl2lm9udDgDxyKAEWCueyrhEK49oKiWFaaBGeqrYw8pojLyui9zpUgDyQqUkBBzEy6Kdy8-&__req=29&ttstamp=2658172568911171657910267120&__rev=1503785";
						ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), WallPostData);
					}
					if (ResponseWallPost.Length >= 300)
					{
						TotalNoOfWallPoster_Counter++;

						AddToLogger_FriendsManager("Posted message on own wall " + fbUser.username);

					}
					else
					{
						AddToLogger_FriendsManager("Couldn't post on own wall " + fbUser.username);

					}
				}
				AddToLogger_FriendsManager("Please wait finding the friend's IDs...");

				if (NoOfFriendsWallPoster != 0)
				{
					//GetAllFriends List
					lstFriend = FBUtils.GetAllFriends(ref HttpHelper, UserId);
				}
				var itemId = lstFriend.Distinct();
				int CountPostWall = 0;

				// messageCountWallPoster = 5;
				messageCountWallPoster = NoOfFriendsWallPoster;

				int friendval = messageCountWallPoster;
				int friendCount = 0;

				if (itemId.Count() > friendval)
				{
					friendCount = friendval;
				}
				else
				{
					friendCount = itemId.Count();
				}

				try
				{
					///Generate a random no list ranging 0-lstMessages.Count

					ArrayList randomNoList = GlobusHttpHelper.RandomNumbers(lstMessagesWallPoster.Count - 1);
					randomNoList = GlobusHttpHelper.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
					int msgIndex = 0;

					foreach (string friendId in itemId)
					{
						if (CountPostWall >= friendCount)
						{
							return;
						}
						try
						{
							#region SelectQuery
							// System.Data.DataSet ds = new DataSet();
							try
							{
								//string selectquery = "select * from tb_ManageWallPoster Where FriendId='" + friendId + "' and DateTime='" + DateTime.Now.ToString("MM/dd/yyyy") + "' and UserName='" + Username + "'";
								// ds = DataBaseHandler.SelectQuery(selectquery, "tb_ManageWallPoster");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							//if (ds.Tables[0].Rows.Count < 1)
							{
								// return; 
								#endregion

								string message = string.Empty;
								if (UserId != friendId)
								{
									#region Select Msg according to Mode
									try
									{

										///For Random, might be Unique, might not be
									  if (UseRandomWallPoster)
										{
											if (msgIndex < randomNoList.Count)
											{
												try
												{
													msgIndex = (int)randomNoList[msgIndex];
													message = lstWallPostURLsWallPoster[msgIndex];
													msgIndex++;
												}
												catch (Exception ex)
												{
													message = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
													// message = MsgWallPoster;
													Console.WriteLine("Error : " + ex.StackTrace);
												}
											}
											else if (lstWallPostURLsWallPoster.Count > msgIndex)
											{
												message = lstWallPostURLsWallPoster[msgIndex];
												msgIndex++;
											}
											else
											{
												try
												{
													//msgIndex = 0;
													randomNoList = GlobusHttpHelper.RandomNumbers(lstWallPostURLsWallPoster.Count - 1);
													message = lstMessagesWallPoster[msgIndex];
													message = lstMessagesWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count - 1)];
													msgIndex++;
												}
												catch (Exception ex)
												{
													message = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
													Console.WriteLine(ex.StackTrace);
												}
											}
										}

										///For Unique or Different Msg for each friend                                        

										else if (UseUniqueMsgToAllFriendsWallPoster)
										{
											if (lstMessagesWallPoster.Count > countWallPoster - 1)
											{

												message = lstMessagesWallPoster[countWallPoster - 1];
											}
											else
											{
												try
												{
													message = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
													//message = lstMessagesWallPoster[Utils.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
												}
												catch (Exception ex)
												{
													message = lstMessagesWallPoster[GlobusHttpHelper.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
													Console.WriteLine("Error : " + ex.StackTrace);
												}
											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									#endregion

									try
									{
										if (!ChkSpinnerWallMessaeWallPoster)
										{
											if (string.IsNullOrEmpty(message))
											{
												message = lstWallPostURLsWallPoster[GlobusHttpHelper.GenerateRandom(0, lstWallPostURLsWallPoster.Count)];
											}
											PostOnFriendsWall(friendId, message, ref fbUser, ref UserId);
										}
										else
										{
											if (lstSpinnerWallMessageWallPoster.Count > 0)
											{
												//PostOnFriendWallUsingSpinMsg(friendId, message, ref fbUser, ref UserId);
											}
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									CountPostWall++;
								}
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

			//	AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				//AddToLogger_FriendsManager(" Wall Message  not valide message must be contains http or https ");
				//GlobusLogHelper.log.Debug("Wall Message  not valide message must be contains http or https ");
			}
			finally
			{
				if (!isStopWallPoster)
				{

					AddToLogger_FriendsManager("Wall Posting Completed With Username : " + fbUser.username);

				}
				// HttpHelper.http.Dispose(); 
			}
		}      

		private void PostOnFriendsWall(string friendId, string wallmessage, ref FacebookUser fbUser, ref string UserId)
		{           

			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string kkk = string.Empty;
				string VUrl = string.Empty;
				string jhj = string.Empty;
				string ss = string.Empty;

				string friendid = friendId;
				string wallMessage = wallmessage;
				DateTime datetiemvalue = DateTime.Now;
				TimeSpan xcx = DateTime.Now - datetiemvalue;

				if (!statusForGreetingMsgWallPoster)
				{
					string postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
					postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
					string pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl),"","");
					string appid = GlobusHttpHelper.getBetween(pageSourceWallPost11, "appid=", "&");
					string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSourceWallPost11);
					string xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_composerid");
					if (string.IsNullOrEmpty(xhpc_composerid))
					{
						if (pageSourceWallPost11.Contains("composerID\":\""))
						{

							xhpc_composerid = FBUtils.getBetween(pageSourceWallPost11, "composerID\":\"", "\"");

						}

					}
						
					string xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSourceWallPost11, "xhpc_targetid");                  

					if (string.IsNullOrEmpty(xhpc_targetid))
					{
						if (pageSourceWallPost11.Contains("targetID\":\""))
						{

							xhpc_targetid = FBUtils.getBetween(pageSourceWallPost11, "targetID\":\"","\"");

						}
					}



					if (postUrl.Contains("https://"))
					{
						postUrl = FBGlobals.Instance.fbProfileUrl + friendId + "&sk=wall";
						pageSourceWallPost11 = HttpHelper.getHtmlfromUrl(new Uri(postUrl),"","");

						//if (pageSourceWallPost11.Contains("fb_dtsg") && pageSourceWallPost11.Contains("xhpc_composerid") && pageSourceWallPost11.Contains("xhpc_targetid"))
						if (pageSourceWallPost11.Contains("fb_dtsg") && (pageSourceWallPost11.Contains("xhpc_composerid") || pageSourceWallPost11.Contains("composerID")) && (pageSourceWallPost11.Contains("xhpc_targetid") || pageSourceWallPost11.Contains("targetID")))
						{
						
							AddToLogger_FriendsManager(countWallPoster.ToString() + " Posting on wall " + postUrl);
						
							string ResponseWallPost1 = string.Empty;

							try
							{
								string FirstResponse = string.Empty;
								string SecondResponse = string.Empty;
								if (!wallmessage.Contains("https://") && !wallmessage.Contains("http://"))
								{
									wallmessage = "https://" + wallmessage;
								}
								try
								{
									string PostDataUrl = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1";
									string PostData="fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897";
									FirstResponse = HttpHelper.postFormData(new Uri(PostDataUrl), PostData);

									if (FirstResponse.Contains("Who are you with?"))
									{
										string Post_Url = "https://www.facebook.com/ajax/composerx/attachment/status/bootload/?av="+UserId+"&composerurihash=1";
										string PostData_Url = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid="+xhpc_targetid+"&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=backdateicon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=backdateicon&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&nctr[_mod]=pagelet_timeline_recent&__user="+UserId+"&__a=1&__dyn=7n8ajEyl2qm9udDgDxyKAEWCueyp9Esx6iqA8ABGeqrWo8pojByUWdDx2ubhHximmey8qUS8zU&__req=e&ttstamp=26581729512056122661171216683&__rev=1503785";
										FirstResponse = HttpHelper.postFormData(new Uri(Post_Url), PostData_Url);
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.StackTrace);
								}
								if (FirstResponse.Contains("Sorry, we got confused"))
								{
									try
									{
										FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/status/bootload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + UserId + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=ogtaggericon&loaded_components[10]=withtaggericon&loaded_components[11]=placetaggericon&loaded_components[12]=mainprivacywidget&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzFVpUgDyQqUgKm58&__req=8&ttstamp=265817269541189012265988656&__rev=1404598");
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}
								}
								try
								{
									SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(wallmessage) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.StackTrace);
								}
								string tagger_session_id = GlobusHttpHelper.getBetween(FirstResponse, "tagger_session_id\\\" value=\\\"", "\\\"");
								string composer_predicted_city = GlobusHttpHelper.getBetween(FirstResponse, "composer_predicted_city\\\" value=\\\"", "\\\"");
								string attachment_params = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][0]\\\" value=\\\"", "\\\"");
								string attachment_params_urlInfo_canonical = GlobusHttpHelper.getBetween(SecondResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_urlInfo_final = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_urlInfo_user = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_favicon = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_title = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_summary = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_images0 = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_medium = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_url = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_type = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_src = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_width = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_height = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_params_video0_secure_url = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string attachment_type = GlobusHttpHelper.getBetween(SecondResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_source = GlobusHttpHelper.getBetween(SecondResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_domain = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_base_domain = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_title_len = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_summary_len = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_min_dimensions0 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_min_dimensions1 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_with_dimensions = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_pending = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_fetched = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_image_dimensions0 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_image_dimensions1 = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_considered = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_selected = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_cap = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string link_metrics_images_type = GlobusHttpHelper.getBetween(SecondResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
								string xhpc_message_text = wallmessage;
								if (chkWallWallPosterRemoveURLsMessages == true)
								{


									if (xhpc_message_text.Contains("https:"))
									{
										string[] arr = xhpc_message_text.Split(':');
										if (arr.Count() == 3)
										{
											xhpc_message_text = arr[0];
										}
										else
										{
											xhpc_message_text = string.Empty;
										}
									}

								}
								else
								{
									xhpc_message_text= lstMessagesWallPoster[GlobusHttpHelper.GenerateRandom(0, lstMessagesWallPoster.Count - 1)];
									xhpc_message_text = Uri.EscapeDataString(xhpc_message_text + "    :    " +wallmessage);
								}
								//Final PostData
								string ResponseWallPost = string.Empty;
								if (string.IsNullOrEmpty(FirstResponse))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
								}
								else
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + xhpc_message_text + "&xhpc_message_text=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
								}
								if (ResponseWallPost.Contains("The message could not be posted to this Wall.") ||ResponseWallPost.Contains ("Message Failed\",\"errorDescription\"") || ResponseWallPost.Contains("You have been temporarily blocked from performing this action"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
								}
								if (ResponseWallPost.Contains("There was a problem updating your status. Please try again in a few minutes."))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%225a336254%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1k%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&privacyx=300645083384735&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumu49UJ6K4bBxi&__req=f&ttstamp=265817269541189012265988656&__rev=1404598");
								}
								if (ResponseWallPost.Contains("Sorry, the privacy setting on this post means that you can't share it")&&ResponseWallPost.Contains("Could Not Post to Timeline"))
								{
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?av=" + UserId), "composer_session_id=c9e72d37-ce06-40d8-a3f3-b35c8316bcbd&fb_dtsg="+fb_dtsg+"&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=1&xhpc_composerid="+xhpc_composerid+"&xhpc_targetid="+xhpc_targetid+"&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%229dbcb61a%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1e%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A"+xhpc_targetid+"%7D&xhpc_message_text="+xhpc_message_text+"&xhpc_message="+xhpc_message_text+"&aktion=post&app_id="+appid+"&attachment[params][0]="+attachment_params+"&attachment[params][1]=1073742507&attachment[type]=2&backdated_date[year]=&backdated_date[month]=&backdated_date[day]=&backdated_date[hour]=&backdated_date[minute]=&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id="+tagger_session_id+"&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city="+composer_predicted_city+"&nctr[_mod]=pagelet_timeline_recent&__user="+UserId+"&__a=1&__dyn=7n8ajEyl2lm9udDgDxyKAEWCueyp9Esx6iWF299qzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=26581729512056122661171216683&__rev=1503785");
								}
								ResponseWallPost1 = ResponseWallPost;
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}

						

							string postDataWalllpost1112 = string.Empty;                            
							if (!(ResponseWallPost1.Length > 11000))
							{

								int length2 = ResponseWallPost1.Length;
								if (length2 > 11000 && ResponseWallPost1.Contains("jsmods") && ResponseWallPost1.Contains("XHPTemplate"))
								{
									TotalNoOfWallPoster_Counter++;

								AddToLogger_FriendsManager("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);


									countWallPoster++;                                   

									try
									{
										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
									AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
									
										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}


								}
								else if (length2 > 11000 && ResponseWallPost1.Contains("jsmods") && ResponseWallPost1.Contains("XHPTemplate"))
								{
									TotalNoOfWallPoster_Counter++;
								    AddToLogger_FriendsManager("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);

									countWallPoster++;

									try
									{
										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
									    AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								
								}
								else
								{
									string errorSummary = FBUtils.GetErrorSummary(ResponseWallPost1);
								    AddToLogger_FriendsManager("Error : " + errorSummary + " not Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);
									try
									{
										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							else
							{
								TotalNoOfWallPoster_Counter++;

							AddToLogger_FriendsManager("Posted on Friends wall :" + postUrl + " With Username : " + fbUser.username);


								countWallPoster++;

								try
								{                                        
									int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayWallPoster * 1000, maxDelayWallPoster * 1000);
								AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep(delayInSeconds);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}


							}

							System.Threading.Thread.Sleep(4000);
						}
						else
						{
							// AddToLogger_FriendsManager("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);
							//GlobusLogHelper.log.Debug("Some problem posting on Friend wall :" + postUrl + " With Username : " + fbUser.username);

							System.Threading.Thread.Sleep(1000);
						}
					}
				}
				else
				{
					try
					{
						//PostOnFriendWallUsingGreetMsg(friendid, wallMessage, ref fbUser, ref UserId);
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


		private void StartActionPostPicOnWall(ref FacebookUser fbUser)
		{

			PostPictureOnWall(ref fbUser);
		}
		public void PostPictureOnWallOld(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				AddToLogger_FriendsManager("Start Post Pic On Wall With Username : " + fbUser.username);


				if (!IsPostAllPicPostPicOnWall)
				{
					try
					{
						bool ReturnPicstatus = false;
						int intProxyPort = 80;
						string xhpc_composerid = string.Empty;
						string xhpc_targetid = string.Empty;
						string message_text = string.Empty;
						Regex IdCheck = new Regex("^[0-9]*$");
						if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
						{
							intProxyPort = int.Parse(fbUser.proxyport);
						}
						string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

						string __user = "";
						string fb_dtsg = "";

						string pgSrc_FanPageSearch = PageSrcHome;

						__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
						if (string.IsNullOrEmpty(__user))
						{
							__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
						}

						if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
						{
							AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);
						

							return;
						}

						fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
						if (string.IsNullOrEmpty(fb_dtsg))
						{
							fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
						}
						try
						{
							xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						try
						{
							xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						try
						{
							string Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
							string DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";

							string res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
							if (string.IsNullOrEmpty(res))
							{
								Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
								res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string imagePath = string.Empty;
						try
						{
							imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string message = string.Empty;
						try
						{
							//if (chkWallPostPicOnWallWithMessage == true)
							{
								try
								{
									message = lstWallMessageWallPoster[new Random().Next(0, lstWallMessageWallPoster.Count)];    

								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							//else
							{
								//message = "";
							}


						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						string status = string.Empty;



							ReturnPicstatus = PostPicture1(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);



						if (ReturnPicstatus)
						{
							AddToLogger_FriendsManager("Posted Picture On Own Wall !");


							if (string.IsNullOrEmpty(message))
							{
								AddToLogger_FriendsManager("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);
							

							}
							else
							{
								AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);


							}
						}
						else
						{
							AddToLogger_FriendsManager("Picture  Post  On Wall Using UserName : " + fbUser.username);

						}


						//Friends wallposting started

						if (NumberOfFriendsSendPicOnWall>0)
						{
							AddToLogger_FriendsManager("Please wait finding the friends ID...");


							List<string> lstFriend = new List<string>();
							lstFriend = FBUtils.GetAllFriends(ref HttpHelper, __user);
							lstFriend = lstFriend.Distinct().ToList();
							int CountFriends = 0;
							bool CheckStatus = false;
							List<string> TempMessage = lstMessageCollectionPostPicOnWall;
							foreach (var lstFriend_item in lstFriend)
							{
								//Check Database


								if (NumberOfFriendsSendPicOnWall <= CountFriends)
								{
								    break;
								}
								try
								{
									try
									{                                       
										message = lstWallMessageWallPoster[new Random().Next(0, lstWallMessageWallPoster.Count)];
										// TempMessage.Remove(message);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									try
									{
										imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}


									CheckStatus = PostPictureOnFriendWall(ref fbUser, message, lstFriend_item, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

								if (CheckStatus)
								{
									CountFriends = CountFriends + 1;

									//Insert n Database

									AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);

									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								else
								{
									//AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);
									AddToLogger_FriendsManager("Picture Not Post To on  friends Wall Using UserName : " + fbUser.username);


									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								} 
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
					try
					{
						PostAlluploadedPicOnOwnWall(ref fbUser);
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

			AddToLogger_FriendsManager("Process Completed Of Post Pic On Wall With Username : " + fbUser.username);

		}




		public void PostPictureOnWall(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				AddToLogger_FriendsManager("Start Post Pic On Wall With Username : " + fbUser.username);


				if (!IsPostAllPicPostPicOnWall)
				{
					try
					{
						bool ReturnPicstatus = false;
						int intProxyPort = 80;
						string xhpc_composerid = string.Empty;
						string xhpc_targetid = string.Empty;
						string message_text = string.Empty;
						Regex IdCheck = new Regex("^[0-9]*$");
						if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
						{
							intProxyPort = int.Parse(fbUser.proxyport);
						}
						string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

						string __user = "";
						string fb_dtsg = "";

						string pgSrc_FanPageSearch = PageSrcHome;

						__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
						if (string.IsNullOrEmpty(__user))
						{
							__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
						}

						if (string.IsNullOrEmpty(__user) || __user == "0" || __user.Length < 3)
						{
							AddToLogger_FriendsManager("Please Check The Account : " + fbUser.username);


							return;
						}

						fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
						if (string.IsNullOrEmpty(fb_dtsg))
						{
							fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
						}
						try
						{
							xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						try
						{
							xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						try
						{
							string Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
							string DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";

							string res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
							if (string.IsNullOrEmpty(res))
							{
								Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
								res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string imagePath = string.Empty;
						try
						{
							imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string message = string.Empty;
						try
						{
							//if (chkWallPostPicOnWallWithMessage == true)
							{
								try
								{
									message = lstWallMessageWallPoster[new Random().Next(0, lstWallMessageWallPoster.Count)];    

								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							//else
							{
								//message = "";
							}


						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						string status = string.Empty;



						//ReturnPicstatus = PostPicture1(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);



						//if (ReturnPicstatus)
						//{
						//AddToLogger_FriendsManager("Posted Picture On Own Wall !");


						//if (string.IsNullOrEmpty(message))
						//{
						//	AddToLogger_FriendsManager("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);


						//}
						//	else
						//	{
						//AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);


						//}
						//}
						//else
						//	{
						//	AddToLogger_FriendsManager("Picture  Post  On Wall Using UserName : " + fbUser.username);

						//}


						//Friends wallposting started

						if (NumberOfFriendsSendPicOnWall>0)
						{
							AddToLogger_FriendsManager("Please wait finding the friends ID...");


							List<string> lstFriend = new List<string>();
							lstFriend = FBUtils.GetAllFriends(ref HttpHelper, __user);
							lstFriend = lstFriend.Distinct().ToList();
							int CountFriends = 0;
							bool CheckStatus = false;
							List<string> TempMessage = lstMessageCollectionPostPicOnWall;
							foreach (var lstFriend_item in lstFriend)
							{
								//Check Database
								try
								{
									string lstfriendInfo = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/" + lstFriend_item),"","");
									if(!lstfriendInfo.Contains("<span aria-hidden=\"true\">Friends</span>"))  //<span aria-hidden="true">Friends</span>
									{
										//string IsFriendCheck = FBUtils.getBetween(lstfriendInfo,"<span aria-hidden=\"true\">","</span>");
										//if(IsFriendCheck != "Friends")
										{
											continue;
										}


									}
								}
								catch{};


								if (NumberOfFriendsSendPicOnWall <= CountFriends)
								{
									break;
								}
								try
								{
									try
									{                                       
										message = lstWallMessageWallPoster[new Random().Next(0, lstWallMessageWallPoster.Count)];
										// TempMessage.Remove(message);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
									try
									{
										imagePath = lstPicturecollectionPostPicOnWall[new Random().Next(0, lstPicturecollectionPostPicOnWall.Count)];
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}


									CheckStatus = PostPictureOnFriendWall(ref fbUser, message, lstFriend_item, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, ref status);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

								if (CheckStatus)
								{
									CountFriends = CountFriends + 1;

									//Insert n Database

									AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);

									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayFriendManager * 1000, maxDelayFriendManager * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								else
								{
									//AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own friends : " + lstFriend_item + " : Wall With UserName : " + fbUser.username);
									AddToLogger_FriendsManager("Picture Not Post To on  friends Wall Using UserName : " + fbUser.username);


									try
									{

										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
										AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								} 
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
					try
					{
						PostAlluploadedPicOnOwnWall(ref fbUser);
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

			AddToLogger_FriendsManager("Process Completed Of Post Pic On Wall With Username : " + fbUser.username);

		}


















		public bool PostPicture1(ref FacebookUser fbUser, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword,  ref string status)
		{

			bool isSentPicMessage = false;
			string fb_dtsg = string.Empty;
			string photo_id = string.Empty;
			string UsreId = string.Empty;
			string xhpc_composerid = string.Empty;
			string xhpc_targetid = string.Empty;
			string message_text = string.Empty;
			string picfilepath = string.Empty;

			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				picfilepath = localImagePath;

				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				UsreId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UsreId))
				{
					UsreId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
				if (string.IsNullOrEmpty(fb_dtsg))
				{
					fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
				}


				string pageSource_HomeData = pageSource_Home;
				try
				{
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

				nvc.Add("fb_dtsg", fb_dtsg);
				nvc.Add("xhpc_targetid", xhpc_targetid);
				nvc.Add("xhpc_context", "home");
				nvc.Add("xhpc_ismeta", "1");
				nvc.Add("xhpc_fbx", "1");
				nvc.Add("xhpc_timeline", "");
				nvc.Add("xhpc_composerid", xhpc_composerid);
				//nvc.Add("xhpc_message_text", message);
				//nvc.Add("xhpc_message", message);



				string response = string.Empty;
				try
				{
					response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);

				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				if (string.IsNullOrEmpty(response))
				{
					try
					{
						response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UsreId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string posturl = FBGlobals.Instance.PostPicOnWallPostAjaxCitySharerResetUrl;
				string postdata = "__user=" + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=1658167761111108210145";
				string responsestring = HttpHelper.postFormData(new Uri(posturl), postdata);
				if (!response.Contains("error") && !string.IsNullOrEmpty(response))
				{
					isSentPicMessage = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return isSentPicMessage;
		}



		public bool PostPictureOnFriendWallOld(ref FacebookUser fbUser,string Message,string FriendID, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, ref string status)
		{

			bool isSentPicMessage = false;
			string fb_dtsg = string.Empty;
			string photo_id = string.Empty;
			string UserId = string.Empty;
			string xhpc_composerid = string.Empty;
			string xhpc_targetid = string.Empty;
			string message_text = string.Empty;
			string picfilepath = string.Empty;
			string Response = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				picfilepath = localImagePath;

				// string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/profile.php?id=" + fbUser.username),"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
				if (string.IsNullOrEmpty(fb_dtsg))
				{
					fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
				}
				string pageSource_HomeData = pageSource_Home;
				try
				{
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}

				string gridid = string.Empty;
				string qn = string.Empty;
				string source = string.Empty;
				string tagger_session_id = string.Empty;
				string hide_object_attachment = string.Empty;

				string is_file_form =  string.Empty;
				string album_type = string.Empty;
				string composer_unpublished_photo= string.Empty;
				string clp = string.Empty;
				string xhpc_publish_type =  string.Empty;
				string xhpc_context = string.Empty;
				string application =  string.Empty;
				string xhpc_ismeta= string.Empty;
				string xhpc_timeline= string.Empty;
				string disable_location_sharing = string.Empty;


				try
				{
					Response = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/media/upload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + FriendID + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=mainprivacywidget&loaded_components[10]=ogtaggericon&loaded_components[11]=withtaggericon&loaded_components[12]=placetaggericon&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=w&ttstamp=26581728812272951201044890108&__rev=1391091");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}

				try
				{
					gridid = GlobusHttpHelper.getBetween(Response, "gridID\":\"", "\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					qn = GlobusHttpHelper.getBetween(Response, "qn\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					source = GlobusHttpHelper.getBetween(Response, "source\":", ",");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					fb_dtsg = GlobusHttpHelper.getBetween(Response, "fb_dtsg\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					tagger_session_id = GlobusHttpHelper.getBetween(Response, "tagger_session_id\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					hide_object_attachment = GlobusHttpHelper.getBetween(Response, "hide_object_attachment\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					is_file_form = GlobusHttpHelper.getBetween(Response, "is_file_form\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					album_type = GlobusHttpHelper.getBetween(Response, "album_type\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					composer_unpublished_photo = GlobusHttpHelper.getBetween(Response, "composer_unpublished_photo[]\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					clp = "{\"cl_impid\":\"62172361\",\"clearcounter\":0,\"elementid\":\"u_0_1h\",\"version\":\"x\",\"parent_fbid\":" + FriendID + "}";
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_publish_type = GlobusHttpHelper.getBetween(Response, "xhpc_publish_type\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_context = GlobusHttpHelper.getBetween(Response, "xhpc_context\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					application = GlobusHttpHelper.getBetween(Response, "application\":\"", "\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_ismeta = GlobusHttpHelper.getBetween(Response, "xhpc_ismeta\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_timeline = GlobusHttpHelper.getBetween(Response, "xhpc_timeline\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					disable_location_sharing = GlobusHttpHelper.getBetween(Response, "disable_location_sharing\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}

				System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

				nvc.Add("fb_dtsg", fb_dtsg);
				nvc.Add("source", source);
				nvc.Add("profile_id", UserId);
				nvc.Add("grid_id", gridid);
				nvc.Add("qn", qn);
				nvc.Add("0", localImagePath);
				nvc.Add("upload_id","1025");



				string response = string.Empty;
				try
				{
					response = HttpHelper.HttpUploadPictureForWallNew(ref HttpHelper, UserId, "https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + FriendID + "&__av=" + UserId + "&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=13&fb_dtsg=" + fb_dtsg + "&ttstamp=26581728812272951201044890108&__rev=1391091" + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
					composer_unpublished_photo = GlobusHttpHelper.getBetween(response, "composer_unpublished_photo[]\\\" value=\\\"", "\\\"");
					//fb_dtsg = Utils.getBetween(response, "fb_dtsg\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				if (string.IsNullOrEmpty(response))
				{
					try
					{
						response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UserId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				else
				{

					nvc.Clear();
					nvc.Add("composer_session_id","");
					nvc.Add("fb_dtsg", fb_dtsg);
					nvc.Add("xhpc_context", xhpc_context);
					nvc.Add("xhpc_ismeta", xhpc_ismeta);
					nvc.Add("xhpc_timeline", xhpc_timeline);
					nvc.Add("xhpc_composerid", xhpc_composerid);
					nvc.Add("xhpc_targetid", FriendID);
					nvc.Add("xhpc_publish_type", xhpc_publish_type);
					nvc.Add("clp", "");
					nvc.Add("xhpc_message_text", Message);
					nvc.Add("xhpc_message", Message);
					nvc.Add("composer_unpublished_photo[]", composer_unpublished_photo);
					nvc.Add("album_type", album_type);
					nvc.Add("is_file_form", is_file_form);
					nvc.Add("oid","");
					nvc.Add("qn", qn);
					nvc.Add("application", application);
					nvc.Add("backdated_date[year]","" );
					nvc.Add("backdated_date[month]", "");
					nvc.Add("backdated_date[day]", "");
					nvc.Add("backdated_date[hour]", "");
					nvc.Add("backdated_date[minute]", "");
					nvc.Add("is_explicit_place", "");
					nvc.Add("composertags_place", "");
					nvc.Add("composertags_place_name", "");
					nvc.Add("tagger_session_id", tagger_session_id);
					nvc.Add("action_type_id[]","");
					nvc.Add("object_str[]", "");
					nvc.Add("object_id[]", "");
					nvc.Add("og_location_id[]", "");
					nvc.Add("hide_object_attachment", hide_object_attachment);
					nvc.Add("og_suggestion_mechanism", "");
					nvc.Add("og_suggestion_logging_data", "");
					nvc.Add("icon_id", "");
					nvc.Add("composertags_city", "");
					nvc.Add("disable_location_sharing", disable_location_sharing);
					nvc.Add("composer_predicted_city","");
				}
				string responsestring = HttpHelper.HttpUploadPictureForWallNewFinal(ref HttpHelper, UserId, "https://upload.facebook.com/media/upload/photos/composer/?__av="+UserId+"&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=o&fb_dtsg="+fb_dtsg+"&ttstamp=26581691188750571181101086979&__rev=1391091", "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
				if (!responsestring.Contains("error") && !string.IsNullOrEmpty(responsestring))
				{
					//string resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/places/city_sharer_reset.php"), "target_id=0&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=r&fb_dtsg=AQEvW29vnlEO&ttstamp=26581691188750571181101086979&__rev=1391091");
					isSentPicMessage = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return isSentPicMessage;
		}






		public bool PostPictureOnFriendWall(ref FacebookUser fbUser,string Message,string FriendID, string Username, string Password, string localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, ref string status)
		{

			bool isSentPicMessage = false;
			string fb_dtsg = string.Empty;
			string photo_id = string.Empty;
			string UserId = string.Empty;
			string xhpc_composerid = string.Empty;
			string xhpc_targetid = string.Empty;
			string message_text = string.Empty;
			string picfilepath = string.Empty;
			string Response = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				picfilepath = localImagePath;

				// string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl));
				string pageSource_Home = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/profile.php?id=" + FriendID) ,"","");

				UserId = GlobusHttpHelper.GetParamValue(pageSource_Home, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_Home, "user");
				}

				fb_dtsg = GlobusHttpHelper.GetParamValue(pageSource_Home, "fb_dtsg");
				if (string.IsNullOrEmpty(fb_dtsg))
				{
					fb_dtsg = GlobusHttpHelper.ParseJson(pageSource_Home, "fb_dtsg");
				}
				string pageSource_HomeData = pageSource_Home;
				try
				{
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "composerid");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pageSource_HomeData, "xhpc_targetid");
				}
				catch (Exception ex)
				{
					//	GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}

				string gridid = string.Empty;
				string qn = string.Empty;
				string source = string.Empty;
				string tagger_session_id = string.Empty;
				string hide_object_attachment = string.Empty;

				string is_file_form =  string.Empty;
				string album_type = string.Empty;
				string composer_unpublished_photo= string.Empty;
				string clp = string.Empty;
				string xhpc_publish_type =  string.Empty;
				string xhpc_context = string.Empty;
				string application =  string.Empty;
				string xhpc_ismeta= string.Empty;
				string xhpc_timeline= string.Empty;
				string disable_location_sharing = string.Empty;





				try
				{
					Response = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/media/upload/?__av=" + UserId + "&composerurihash=1"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + FriendID + "&istimeline=1&composercontext=composer&onecolumn=1&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=backdateicon&loaded_components[4]=placetaggericon&loaded_components[5]=ogtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=prompt&loaded_components[8]=backdateicon&loaded_components[9]=mainprivacywidget&loaded_components[10]=ogtaggericon&loaded_components[11]=withtaggericon&loaded_components[12]=placetaggericon&loaded_components[13]=maininput&nctr[_mod]=pagelet_timeline_recent&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMCBynzpQ9UoHFaeFDzECiq78hACF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=w&ttstamp=26581728812272951201044890108&__rev=1391091");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}

				try
				{
					gridid = GlobusHttpHelper.getBetween(Response, "gridID\":\"", "\"");
				}
				catch (Exception ex)
				{
					//	GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					qn = GlobusHttpHelper.getBetween(Response, "qn\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					source = GlobusHttpHelper.getBetween(Response, "source\":", ",");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					fb_dtsg = GlobusHttpHelper.getBetween(Response, "fb_dtsg\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					tagger_session_id = GlobusHttpHelper.getBetween(Response, "tagger_session_id\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					hide_object_attachment = GlobusHttpHelper.getBetween(Response, "hide_object_attachment\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					is_file_form = GlobusHttpHelper.getBetween(Response, "is_file_form\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					album_type = GlobusHttpHelper.getBetween(Response, "album_type\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					composer_unpublished_photo = GlobusHttpHelper.getBetween(Response, "composer_unpublished_photo[]\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					clp = "{\"cl_impid\":\"62172361\",\"clearcounter\":0,\"elementid\":\"u_0_1h\",\"version\":\"x\",\"parent_fbid\":" + FriendID + "}";
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_publish_type = GlobusHttpHelper.getBetween(Response, "xhpc_publish_type\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_context = GlobusHttpHelper.getBetween(Response, "xhpc_context\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					application = GlobusHttpHelper.getBetween(Response, "application\":\"", "\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_ismeta = GlobusHttpHelper.getBetween(Response, "xhpc_ismeta\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_timeline = GlobusHttpHelper.getBetween(Response, "xhpc_timeline\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				try
				{
					disable_location_sharing = GlobusHttpHelper.getBetween(Response, "disable_location_sharing\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}

				System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

				nvc.Add("fb_dtsg", fb_dtsg);
				nvc.Add("source", source);
				nvc.Add("profile_id", UserId);
				nvc.Add("grid_id", gridid);
				nvc.Add("qn", qn);
				nvc.Add("0", localImagePath);
				nvc.Add("upload_id","1025");



				string response = string.Empty;
				try
				{
					response = HttpHelper.HttpUploadPictureForWallNew(ref HttpHelper, UserId, "https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + FriendID + "&__av=" + UserId + "&__user=" + UserId + "&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=13&fb_dtsg=" + fb_dtsg + "&ttstamp=26581728812272951201044890108&__rev=1391091" + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
					composer_unpublished_photo = GlobusHttpHelper.getBetween(response, "fbid\":\"", "\"");
					//fb_dtsg = Utils.getBetween(response, "fb_dtsg\\\" value=\\\"", "\\\"");
				}
				catch (Exception ex)
				{
					//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				}
				if (string.IsNullOrEmpty(response))
				{
					try
					{
						response = HttpHelper.HttpUploadPictureForWall(ref HttpHelper, UserId, FBGlobals.Instance.PostPicOnWallPostUploadPhotosUrl + UserId + "&__a=1&fb_dtsg=" + fb_dtsg, "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
					}
					catch (Exception ex)
					{
						//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
					}
				}
				else
				{

					nvc.Clear();
					nvc.Add("composer_session_id","");
					nvc.Add("fb_dtsg", fb_dtsg);
					nvc.Add("xhpc_context", xhpc_context);
					nvc.Add("xhpc_ismeta", xhpc_ismeta);
					nvc.Add("xhpc_timeline", xhpc_timeline);
					nvc.Add("xhpc_composerid", xhpc_composerid);
					nvc.Add("xhpc_targetid", FriendID);
					nvc.Add("xhpc_publish_type", xhpc_publish_type);
					nvc.Add("clp", "");
					nvc.Add("xhpc_message_text", Message);
					nvc.Add("xhpc_message", Message);
					nvc.Add("composer_unpublished_photo[]", composer_unpublished_photo);
					nvc.Add("album_type", album_type);
					nvc.Add("is_file_form", is_file_form);
					nvc.Add("oid","");
					nvc.Add("qn", qn);
					nvc.Add("application", application);
					nvc.Add("backdated_date[year]","" );
					nvc.Add("backdated_date[month]", "");
					nvc.Add("backdated_date[day]", "");
					nvc.Add("backdated_date[hour]", "");
					nvc.Add("backdated_date[minute]", "");
					nvc.Add("is_explicit_place", "");
					nvc.Add("composertags_place", "");
					nvc.Add("composertags_place_name", "");
					nvc.Add("tagger_session_id", tagger_session_id);
					nvc.Add("action_type_id[]","");
					nvc.Add("object_str[]", "");
					nvc.Add("object_id[]", "");
					nvc.Add("og_location_id[]", "");
					nvc.Add("hide_object_attachment", hide_object_attachment);
					nvc.Add("og_suggestion_mechanism", "");
					nvc.Add("og_suggestion_logging_data", "");
					nvc.Add("icon_id", "");
					nvc.Add("composertags_city", "");
					nvc.Add("disable_location_sharing", disable_location_sharing);
					nvc.Add("composer_predicted_city","");
				}
				string responsestring = HttpHelper.HttpUploadPictureForWallNewFinal(ref HttpHelper, UserId, "https://upload.facebook.com/media/upload/photos/composer/?__av="+UserId+"&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=o&fb_dtsg="+fb_dtsg+"&ttstamp=26581691188750571181101086979&__rev=1391091", "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword, picfilepath);
				if (responsestring.Contains("error") && !string.IsNullOrEmpty(responsestring))
				{
					//string resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/places/city_sharer_reset.php"), "target_id=0&__user="+UserId+"&__a=1&__dyn=7n8ajEAMBlynzpQ9UoHaEWCueyp9Esx6iWF29aGEVFLFwxBxCbzESu49UJ6K4bBw&__req=r&fb_dtsg=AQEvW29vnlEO&ttstamp=26581691188750571181101086979&__rev=1391091");
					isSentPicMessage = true;
				}
			}
			catch (Exception ex)
			{
				//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
			}
			return isSentPicMessage;
		}






		private void PostAlluploadedPicOnOwnWall(ref FacebookUser fbUser)
		{
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				bool ReturnPicstatus = false;
				int intProxyPort = 80;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				Regex IdCheck = new Regex("^[0-9]*$");
				if (!string.IsNullOrEmpty(fbUser.proxyport) && IdCheck.IsMatch(fbUser.proxyport))
				{
					intProxyPort = int.Parse(fbUser.proxyport);
				}
				string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = PageSrcHome;
				__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
				if (string.IsNullOrEmpty(__user))
				{
					__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
				}

				fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
				if (string.IsNullOrEmpty(fb_dtsg))
				{
					fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
				}
				try
				{
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				try
				{
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}

				try
				{
					string Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
					string DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&ishome=1&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=maininput&loaded_components[3]=mainprivacywidget&nctr[_mod]=pagelet_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";

					string res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
					if (string.IsNullOrEmpty(res))
					{
						Dialogposturl = FBGlobals.Instance.PostPicOnWallPostAjaxComposeUriHashUrl;
						res = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
					}

				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				foreach (string imagePath in lstPicturecollectionPostPicOnWall)
				{
					string message = string.Empty;

					if (lstMessageCollectionPostPicOnWall.Count > 0 && chkWallPostPicOnWallWithMessage == true)
					{
						try
						{
							message = lstMessageCollectionPostPicOnWall[new Random().Next(0, lstMessageCollectionPostPicOnWall.Count)];
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
					else
					{
						message = "";
					}



					string status = string.Empty;
					// if (chkCountinueProcessGroupCamapinScheduler == true)
					{
						//ReturnPicstatus = PostPicture(ref fbUser, fbUser.username, fbUser.password, imagePath, fbUser.proxyip, fbUser.proxyport, fbUser.proxyusername, fbUser.proxypassword, message, ref status);
					}
					if (ReturnPicstatus)
					{
						AddToLogger_FriendsManager("Posted Picture On Own Wall !");


						if (string.IsNullOrEmpty(message))
						{
							AddToLogger_FriendsManager("Posted Picture " + imagePath + " On Own Wall with UserName : " + fbUser.username);

							try
							{


								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
								AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
						}
						else
						{
							AddToLogger_FriendsManager("Posted Picture " + imagePath + "  with Message " + message + "On Own Wall With UserName : " + fbUser.username);


							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
								AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
						}                       
					}
					else
					{
						AddToLogger_FriendsManager("Picture Not Posted To on Wall Using UserName : " + fbUser.username);

						try
						{
							int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayPostPicOnWal * 1000, maxDelayPostPicOnWal * 1000);
							AddToLogger_FriendsManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

							Thread.Sleep(delayInSeconds);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}
	}
}

