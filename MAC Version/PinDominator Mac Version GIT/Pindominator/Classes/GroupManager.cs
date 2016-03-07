using System;
using PinDominator;
using System.Threading;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;
using System.IO;

namespace PinDominator
{
	public class GroupManager
	{
		public GroupManager ()
		{

		}


		//public static Event CampaignStopLogevents = new Event();
		public static int noOfGroupsToScrap = 0;
		public static bool IsGroupScraperStarted = false;
		public static int noOfpicUpload_per_Group = 1;
		public static int noOfVideoUpload_per_Group = 1;
		public static int noOftextUpload_per_Group = 1;

		public static void AddToLogger_GroupManager(string log)
		{
			EventsArgs eArgs = new EventsArgs(log);
			CampaignStopLogevents.LogText(eArgs);
		}

		//Group Inviter 



		readonly object lockrThreadControllerGoupInviter = new object();
		static readonly object lockrqueGoupInviter = new object();
		static readonly object lockrqueFanPageMessagesGoupInviter = new object();

		public static Queue<string> queFanPageURLsGoupInviter = new Queue<string>();
		public static Queue<string> queFanPageMessagesGoupInviter = new Queue<string>();

		public static string GoupInviterExprotFilePath=string.Empty;

		public Queue<string> queGroupMessagesGoupInviter = new Queue<string>();


		public bool isStopGroupInviter = false;
		public bool CheckGroupInviterViaEmails = false;
		int countThreadControllerGoupInviter = 0;
		public static int TotalGoupInviterCounter = 0;
		public bool CheckUseManualLoadGroupUrls = false;
	
	
		public static List<int> lstGroupUrlSelectedGoupInviter = new List<int>();

		public static List<Thread> lstThreadsGroupInviter = new List<Thread> ();

		public List<string> lstGroupUrlsGoupPoster = new List<string>();
		public List<string> lstGroupInviteEmails = new List<string>();
		public List<string> lstGroupMessageGroupPoster = new List<string>();
		public List<string> lstGroupCommentsGroupInviter = new List<string>();

		public static int minDelayGroupInviter = 10;
		public static int maxDelayGroupInviter = 20;

		public List<string>listFanPageUrl=new List<string>();

		private static Dictionary<string, string> dictionarySuccessfulLikerAccounts = new Dictionary<string, string>();



		/*public List<string> LstGroupUrlsGroupInviter
		{
			get;
			set;
		}*/
		public static List<string> LstGroupUrlsGroupInviter = new List<string> ();

		public int AddNoOfFriendsGroupInviter
		{
			get;
			set;
		}
		public string StartGroupProcessUsing
		{
			get;
			set;
		}


		public void StartGoupInviter()
		{  
			try
			{
				countThreadControllerGoupInviter=0;
				AddToLogger_GroupManager("Please Wait ...");

				List<List<string>> list_listAccounts = new List<List<string>>();
				if (FBGlobals.listAccounts.Count >= 1)
				{                  
					GroupManager.IsGroupScraperStarted = false;
					foreach (string account in FBGlobals.listAccounts)
					{
						try
						{
							lock (lockrThreadControllerGoupInviter)
							{
								try
								{
									if (countThreadControllerGoupInviter>= FBGlobals.listAccounts.Count)
									{
										Monitor.Wait(lockrThreadControllerGoupInviter);
									}

									string acc = account.Remove(account.IndexOf(':'));

									//Run a separate thread for each account
									FacebookUser item = null;
									FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

									if (item != null)
									{
										try
										{
											Thread profilerThread = new Thread(StartMultiThreadsGroupInviter);												

											profilerThread.Start(new object[] { item });
											countThreadControllerGoupInviter++;
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



		public void StartMultiThreadsGroupInviter(object parameters)
		{
			try
			{
				//if (!isStopGroupInviter)
				{
					try
					{
						lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
						lstThreadsGroupMemberScraper.Distinct();
						Thread.CurrentThread.IsBackground = true;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
					try
					{

						{
							Array paramsArray = new object[1];
							paramsArray = (Array)parameters;
							FacebookUser objFacebookUser = (FacebookUser)paramsArray.GetValue(0);
							if (!objFacebookUser.isloggedin)
							{
								GlobusHttpHelper objGlobusHttpHelper = new GlobusHttpHelper();

								objFacebookUser.globusHttpHelper = objGlobusHttpHelper;

								AccountManager objAccountManager = new AccountManager();
								AddToLogger_GroupManager("Logging in with " + objFacebookUser.username);

								objAccountManager.LoginUsingGlobusHttp(ref objFacebookUser);
							}

							if (objFacebookUser.isloggedin)
							{
								AddToLogger_GroupManager("Successful login With : "+objFacebookUser.username);
								// Call StartActionGroupInviter
								StartActionGroupInviter(ref objFacebookUser);
							}
							else
							{
								AddToLogger_GroupManager("Couldn't Login With Username : " + objFacebookUser.username);

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

			finally
			{
				try
				{

					 if (!isStopGroupInviter)
					{
						lock (lockrThreadControllerGoupInviter)
						{
							countThreadControllerGoupInviter--;
							Monitor.Pulse(lockrThreadControllerGoupInviter);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
			}
		}





		private void StartActionGroupInviter(ref FacebookUser fbUser)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			try
			{
				if (StartGroupProcessUsing=="Group Inviter")
				{				
				    	AddToLogger_GroupManager("Group Inviter Process Started With : " + fbUser.username);
				        AddFriendsToGroup(ref fbUser);	
				        AddToLogger_GroupManager("Group Inviter Process Completed With : " + fbUser.username);
				}
				else if (StartGroupProcessUsing=="Group Scraper By Keyword")
				{
					if(!GroupManager.IsGroupScraperStarted)
					{
					GroupManager.IsGroupScraperStarted = true;
					AddToLogger_GroupManager("Group Scraper By Keyword Process Started With : " + fbUser.username);
					GetGroupUrls(ref fbUser);	
					AddToLogger_GroupManager("Group Scraper By Keyword Process Completed With : " + fbUser.username);
					}

				}
				else if (StartGroupProcessUsing=="Post Image In Group")
				{
					AddToLogger_GroupManager("Group Post Image Process Started With : " + fbUser.username);
					SendPicMessageInGroup(ref fbUser);	
					AddToLogger_GroupManager("Group Post Image Process Completed With : " + fbUser.username);
				}
				else if (StartGroupProcessUsing=="Post Text Message in Group")
				{
					AddToLogger_GroupManager("Group Post Comment Process Started With : " + fbUser.username);
					SendSimpleTextMessageInGroup(ref fbUser);	
					AddToLogger_GroupManager("Group Post Comment Process Completed With : " + fbUser.username);
				}
				else if (StartGroupProcessUsing=="Post Video On Group")
				{
					AddToLogger_GroupManager("Group Post Video Process Started With : " + fbUser.username);
					SendVideoMessageInGroup(ref fbUser);	
					AddToLogger_GroupManager("Group Post Video Process Completed With : " + fbUser.username);
				}
				else if (StartGroupProcessUsing=="Join Group Using Url")
				{
					AddToLogger_GroupManager("Group RequestSend Using GroupUrl Process Started With : " + fbUser.username);
					GroupRequestSendersForBrowseGroup(ref fbUser);	
					AddToLogger_GroupManager("Group RequestSend Using GroupUrl Process Completed With : " + fbUser.username);
				}


			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		//Send Simple Message in Group


		public void SendSimpleTextMessageInGroup(ref FacebookUser fbUser)
		{
			int count = 0;
			bool IsssendMessage = false;
			string UserId = string.Empty;
			string	grpurl = string.Empty;
			string message = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				List<string>lstallgroup_EachAccount = FindOwnGroupUrl(ref fbUser);

				AddToLogger_GroupManager("Username : " + fbUser.username + " Get GroupUrl : " + lstallgroup_EachAccount.Count);

				foreach (var GroupUrl_item in lstallgroup_EachAccount)				
				{

					for (int i = 0; i < GroupManager.noOftextUpload_per_Group;i++ )
					{
						try
						{
							message = lstGroupMessageGroupPoster[i];
						}
						catch (Exception ex)
						{
							try
							{
								message = lstGroupMessageGroupPoster[new Random().Next(0, lstGroupMessageGroupPoster.Count - 1)];
							}
							catch
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendSimpleTextMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
						//message=lstGroupMessageGroupPoster

						IsssendMessage = SendingMsgToGroups(GroupUrl_item, message, ref fbUser);

						if (IsssendMessage)
						{
							//AddToLogger_GroupManager("Send Message : " + message + " Group Url : " + GroupUrl_item + "User Name : " + fbUser.username);
							try
							{
								//int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								//AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								//Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendSimpleTextMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
						else
						{
							//AddToLogger_GroupManager("Message Not Send " + "Message : " + message + " Group Url : " + GroupUrl_item + "User Name :" + fbUser.username);
							try
							{
								//int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								//AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								//Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendSimpleTextMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
					}
				}
			}
			catch(Exception Ex)
			{
				Console.WriteLine (Ex.StackTrace);

			}
		}



		public bool SendingMsgToGroups(string targeturl, string message, ref FacebookUser fbUser)
		{
			try
			{
				List<string> status =new List<string>();
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string msg = string.Empty;
				msg = message;
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl),"","");
				Thread.Sleep(100);
				try
				{
					// status = HttpHelper.GetTextDataByTagAndAttributeName(pgSrc_FanPageSearch, "span", "uiButtonText");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
				try
				{
					for (int i = 0; i < status.Count; i++)
					{
						try
						{
							Msgsendingstatus = status[i];
							if (Msgsendingstatus != string.Empty)
							{
								Msgsendingcurrentstatus = Msgsendingstatus;

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

				// if (Msgsendingcurrentstatus.Contains("Notifications"))
				{

					if (pgSrc_FanPageSearch.Contains("xhpc_composerid") || pgSrc_FanPageSearch.Contains("xhpc_targetid") || pgSrc_FanPageSearch.Contains("targetid") || pgSrc_FanPageSearch.Contains("composer"))
					{
						try
						{
							string __user = "";
							string fb_dtsg = "";
							string xhpc_composerid = string.Empty;
							string xhpc_targetid = string.Empty;
							int composer_session_id;
							string MESSAGE = string.Empty;
							string xhpc_message_text = "";
							bool  chkWallWallPosterRemoveURLsMessages = false;

							try
							{
								MESSAGE = Uri.EscapeDataString(msg);

							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}


							try
							{
								__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
								if (string.IsNullOrEmpty(__user))
								{
									__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}


							try
							{
								fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
								if (string.IsNullOrEmpty(fb_dtsg))
								{
									fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
								}
							}

							catch(Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}
							try
							{
								xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
							}
							catch(Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}



							if (string.IsNullOrEmpty(xhpc_composerid))
							{
								try
								{
									if (pgSrc_FanPageSearch.Contains("composerPostSection"))
									{
										xhpc_composerid = FBUtils.getBetween(pgSrc_FanPageSearch, "composerPostSection\" id=\"", "\"");

									}




								}
								catch { };


							}
							try
							{
								xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
							}
							catch(Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}

							if (string.IsNullOrEmpty(xhpc_targetid))
							{
								try
								{

									if (pgSrc_FanPageSearch.Contains("targetID\":\""))
									{
										xhpc_targetid = FBUtils.getBetween(pgSrc_FanPageSearch, "targetID\":\"", "\"");
									}



								}
								catch { };
							}

							composer_session_id = Convert.ToInt32(GlobusHttpHelper.ConvertToUnixTimestamp(DateTime.Now));

							string composer_sessionid = composer_session_id.ToString();

							string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
							_rev = _rev.Replace("\":", string.Empty);


							string appid = "";
							try
							{
								appid = FBUtils.getBetween(pgSrc_FanPageSearch, "appid=", "&");
							}
							catch { };

							if (message.Contains("https://") || message.Contains("http://") )
							{
								if (message.Contains("https://"))
								{
									//  message = message.Replace("https://", "");

								}
								else if (message.Contains("http://"))
								{
									// message = message.Replace("http://", "");

								}

							}


							if (chkWallWallPosterRemoveURLsMessages == true)
							{


								if (message.Contains("https:") || message.Contains("http:") || message.Contains("www"))
								{
									try
									{
										string[] arr = message.Split(':');
										if (arr.Count() == 3)
										{
											xhpc_message_text = arr[0];
											message = arr[1] + ":" + arr[2];
										}
										else
										{
											xhpc_message_text = string.Empty;
										}
									}
									catch { };
								}
								else
								{

								}

								//  xhpc_message_text = wallmessage;

								//if (Globals.CheckLicenseManager == "fdfreetrial")
								//{
								//    xhpc_message_text = "\n\n Sent from FREE version of Facedominator. To remove this message, please buy it.";
								//}
							}
							else
							{
								if (message.Contains("https:") || message.Contains("http:") || message.Contains("www"))
								{
									try
									{
										string[] arr = message.Split(':');
										if (arr.Count() == 3)
										{
											// xhpc_message_text = arr[0];
											// message = arr[1] + ":" + arr[2];
										}
										else
										{
											// xhpc_message_text = string.Empty;

										}
									}
									catch { };
								}
								else
								{

								}


								//if (Globals.CheckLicenseManager == "fdfreetrial")
								//{
								//    xhpc_message_text = "\n\n Sent from FREE version of Facedominator. To remove this message, please buy it.";
								//}
							}


							string[] messagesList = { };

							if (true)
							{

								message = Uri.EscapeUriString(message);

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
									message = HttpUtility.UrlDecode(message);
									message = HttpUtility.HtmlDecode(message);

									SecondResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(message) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + __user + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
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
								chkWallWallPosterRemoveURLsMessages = false;
								if (chkWallWallPosterRemoveURLsMessages == true)
								{
									if (xhpc_message_text.Contains("https:"))
									{
										string[] arr = xhpc_message_text.Split(':');
										if (arr.Count() == 3)
										{
											xhpc_message_text = arr[0];
											message = arr[1] + ":" + arr[2];
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
								else
								{
									//  xhpc_message_text = Uri.EscapeDataString(wallmessage + "    :    " + xhpc_message_text);
								}

								xhpc_message_text = Uri.EscapeDataString(xhpc_message_text);

								if (false)
								{
									int index = 0;

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


								if (string.IsNullOrEmpty(SecondResponse))
								{
									// string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + message) + "&xhpc_message_text=" + (xhpc_message_text + " " + message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									string PostData = "&attachment&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&boosted_post_config&composer_session_id=94cba319-8335-4b53-8d0d-e9083a610c6d&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&multilingual_specified_lang=&&&privacyx&ref=timeline&tagger_session_id=1443716984&target_type=wall&&xhpc_message=" + (xhpc_message_text + " " + message) + "&xhpc_message_text=" + (xhpc_message_text + " " + message) + "&is_react=true&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&xhpc_fundraiser_page=false&__user=" + __user + "&__a=1&__dyn=7AmajEyl2lm9ongDxiWEB19CzEWq2WiWF298yut9LHwxBxCbzFVob4q68K5Uc-dy88axbxjx2u5W88ybAG5VGqzE-8KuEOq6ouAxO2OE&__req=1j&fb_dtsg=" + fb_dtsg + "&ttstamp=26581721005178848611411310377&__rev=1965820";
									ResponseWallPost = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + __user), PostData);
								}
								else
								{

									string PostData = "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=home&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + (xhpc_message_text + " " + message) + "&xhpc_message_text=" + (xhpc_message_text + " " + message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=" + tagger_session_id + "&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897";
									//  string PostData = "&attachment&backdated_date[year]&backdated_date[month]&backdated_date[day]&backdated_date[hour]&backdated_date[minute]&boosted_post_config&composer_session_id=94cba319-8335-4b53-8d0d-e9083a610c6d&composertags_city&composertags_place&composertags_place_name&&hide_object_attachment=true&is_explicit_place=false&is_q_and_a=false&&&multilingual_specified_lang=&&&privacyx&ref=timeline&tagger_session_id=1443716984&target_type=wall&&xhpc_message=" + (xhpc_message_text + " " + wallmessage) + "&xhpc_message_text=" + (xhpc_message_text + " " + wallmessage) + "&is_react=true&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=true&xhpc_finch=false&xhpc_socialplugin=false&xhpc_topicfeedid&xhpc_origintopicfeedid&xhpc_modal_composer=false&xhpc_aggregated_story_composer=false&xhpc_publish_type=1&xhpc_fundraiser_page=false&__user=" + UsreId + "&__a=1&__dyn=7AmajEyl2lm9ongDxiWEB19CzEWq2WiWF298yut9LHwxBxCbzFVob4q68K5Uc-dy88axbxjx2u5W88ybAG5VGqzE-8KuEOq6ouAxO2OE&__req=1j&fb_dtsg=" + fb_dtsg + "&ttstamp=26581721005178848611411310377&__rev=1965820";
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
								else if (ResponseWallPost.Contains("errorSummary\":\"Security Check Required\""))
								{
									AddToLogger_GroupManager("FB asking For Sucurity to Post Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
								}
								else
								{
									AddToLogger_GroupManager(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
								}

								try
								{

									int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
									AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep(delayInSeconds);
								}
								catch { };


							}



							else
							{


								string postdata = "fb_dtsg=" + fb_dtsg + "&postfromfull=true&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_fbx=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_message_text=" + MESSAGE + "&xhpc_message=" + MESSAGE + "&is_explicit_place=&composertags_place=&composertags_place_name=&composer_session_id=" + composer_session_id + "&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=1658165781016912151427";

								//  postdata = "composer_session_id=62c93be7-17d8-4f1b-b2aa-036f326a8734&fb_dtsg=AQCAABsi&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=u_jsonp_5_c&xhpc_targetid=139180732957326&clp=%7B%22cl_impid%22%3A%220a090d8f%22%2C%22clearcounter%22%3A1%2C%22elementid%22%3A%22u_jsonp_5_r%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A139180732957326%7D&xhpc_message_text=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&xhpc_message=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1388385131&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=100007423262870&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=t&__rev=1062230&ttstamp=2658167656566115105";

								string posturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;

								string Response = string.Empty;
								try
								{
									Response = HttpHelper.postFormData(new Uri(posturl), postdata);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
								if (string.IsNullOrEmpty(Response))
								{
									posturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;
									try
									{
										Response = HttpHelper.postFormData(new Uri(posturl), postdata);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}
								int length = Response.Length;

								if (Response.Contains(msg) || length > 5000 || Response.Contains("Your post has been submitted and is pending approval by an admin."))
								{
									try
									{
										string PostedUrl = string.Empty;
										if (Response.Contains("\"permalink") && Response.Contains("commentcount"))
										{

											//PostedUrl = GlobusHttpHelper.getBetween(Response, "\"permalink", "commentcount").Replace("\\", string.Empty).Replace("\":\"", string.Empty).Replace("\",\"",string.Empty);
											//PostedUrl = "https://www.facebook.com" + PostedUrl;
										}
										else if (Response.Contains("post pending approval.") || Response.Contains("Your post has been submitted and is pending approval by an admin."))
										{
											PostedUrl = "post pending approval.";
										}
										if (Response.Contains("Your post has been submitted and is pending approval by an admin."))
										{
											AddToLogger_GroupManager("Your post has been submitted and is pending approval by an admin.  " + " Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
										}
										else
										{
											AddToLogger_GroupManager(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
										}

										try
										{
											string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "PostedUrl" + "," + "DateTime";
											string CSV_Content = fbUser.username + "," + targeturl + "," + message + "," + PostedUrl + "," + DateTime.Now;
											//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error  >>>" + ex.StackTrace);
										}

										return true;
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
								else
								{
									Console.WriteLine("Message Sending Fail");
									return false;
								}
							}

						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return false;
		}













		//Send  Group Request


		public void GroupRequestSendersForBrowseGroup(ref FacebookUser fbUser)
		{

			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			string CheckStatus = string.Empty;
			try
			{
				string UNqgrpurl = string.Empty;
				List<string> lsturll = new List<string>();
				List<string> lsturllKeyword = new List<string>();
				int intProxyPort = 80;
				Regex IdCheck = new Regex("^[0-9]*$");
				if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
				{
					intProxyPort = int.Parse(proxyPort);
				}
				string PageSrcHome = string.Empty;

				string UserIdGrpMsg = string.Empty;

				try
				{
					PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","");
				}
				catch{};

				try{
				UserIdGrpMsg = GlobusHttpHelper.Get_UserID(PageSrcHome);
				}
				catch{};

				int TimeCounter = 0;
				foreach (var Groupurl in LstGroupUrlsGroupInviter)
				{
					try
					{

					try
					{

						TimeCounter = TimeCounter + 1;

						string grpurl = Groupurl;
						string strGroupUrl = grpurl;
						string __user = "";
						string fb_dtsg = "";
						string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");
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
						if(fb_dtsg.Contains("html>"))
						{
							if(pgSrc_FanPageSearch.Contains("DTSGInitialData"))
							{
								try
								{
									string[] fb_dtsgList = Regex.Split(pgSrc_FanPageSearch,"DTSGInitialData");
									fb_dtsg = FBUtils.getBetween(fb_dtsgList[1],"token\":\"","\"");
								}
								catch{};
							}
						}



						try
						{
							string currentstatus1 = string.Empty;
							string aaaa = string.Empty;
							string groupType = string.Empty;
							string Userstatus = string.Empty;
							string currentstatus = string.Empty;
							string stradminlink = string.Empty;
							string findstatus = string.Empty;
							string findstatus1 = string.Empty;

							string postdataforjoin = string.Empty;
							string localstr = string.Empty;
							string Responseofjoin = string.Empty;
							findstatus = HttpHelper.getHtmlfromUrl(new Uri(grpurl),"","");
							try
							{
								if (grpurl.Contains("http"))
								{
									try
									{
										string[] grpurlArr = Regex.Split(grpurl, "https://");
										string urlforFindingGroupType = grpurlArr[1] + "/members";
										string memberurl = urlforFindingGroupType.Replace("//", "/");
										memberurl = "https://" + memberurl;
										findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl),"","");
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}
								}
								else
								{
									try
									{
										string urlforFindingGroupType = grpurl + "/members";
										string memberurl = urlforFindingGroupType.Replace("//", "/");
										memberurl = memberurl.Replace("//", "/");
										findstatus1 = HttpHelper.getHtmlfromUrl(new Uri(memberurl),"","");
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
							try
							{
								int Counter = 0;
								string[] grpurlArr1 = Regex.Split(grpurl, "/");
								foreach (var grpurlArr_item in grpurlArr1)
								{
									Counter++;
								}
								CheckStatus = grpurlArr1[Counter - 1];
								if (string.IsNullOrEmpty(CheckStatus))
								{
									CheckStatus = grpurlArr1[Counter - 2];
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}

							//  List<string> status3 = objGlobusHttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "uiButtonText");
							if (findstatus.Contains("clearfix groupsJumpBarTop"))
							{

								if (findstatus.Contains("rel=\"async-post\">Join Group</a></li><li>") && !findstatus.Contains(">Pending</span>"))
								{
									currentstatus = "Join Group";
								}
								else if (!findstatus.Contains("rel=\"async-post\">Join Group</a></li><li>") && findstatus.Contains(">Pending</span>"))
								{

						           	AddToLogger_GroupManager("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);

									try
									{
										string CSVHeader = "FbUser" + "," + "GroupURL";
										string CSV_Content = fbUser.username + "," + grpurl;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}
									try
									{
										int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);

										AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
										Thread.Sleep(delayInSeconds);
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}

									continue;
								}

								string[] status12 = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "mrm _42ft _4jy0 _39__ _4jy4 _4jy2 selected");
								if (status12.Count() == 1)
								{
									try
									{
										if ((status12[0].Contains("status:pending-") &&!status12[0].Contains("Join this group to see the discussion, post and comment")&&!status12[0].Contains("Join this group"))&& !status12[0].Contains(">Join Group</a></li>") && !status12[0].Contains("Join Group</a></li>"))
										{
											AddToLogger_GroupManager("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
										
											try
											{
												string CSVHeader = "FbUser" + "," + "GroupURL";
												string CSV_Content = fbUser.username + "," + grpurl;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.StackTrace);
											}


											try
											{
												int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);

												AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
												Thread.Sleep(delayInSeconds);
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.StackTrace);
											}


											continue;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}

								}
								List<string> status = new List<string>();
								string[] status1 = null;

								try
								{
									status1 = System.Text.RegularExpressions.Regex.Split(pgSrc_FanPageSearch, "clearfix groupsJumpBarTop");
									status = HttpHelper.GetDataTag(status1[1], "a");
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.StackTrace);
								}

								try
								{
								if (string.IsNullOrEmpty(currentstatus))
								{
									if (status[0] == "Join Group" || status[0] == "Join group")
									{
										currentstatus = status[0];
									}
									else
									{
										try
										{
											if (status[2] == "Join Group" || status[2] == "Join group")
											{
												currentstatus = status[2];
											}
											else
											{
												try
												{

													if (status[3] == "Join Group" || status[3] == "Join group")
													{
														currentstatus = status[3];
													}
													else
													{
														try
														{
															if (status[0] == "Cancel Request" || status[0] == "Cancel Request")
															{
																try
																{
																	currentstatus = status[0];
																}
																catch (Exception ex)
																{
																	Console.WriteLine(ex.StackTrace);
																}
															}
															currentstatus = GlobusHttpHelper.getBetween(status1[1], "async-post\">", "</a>");
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
								catch{}


							}
							


							try
							{
								string crtstatus = "";
								//if (currentstatus.Contains("Join Group") || currentstatus1.Contains("Join group"))
								{
									try
									{
									//	List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
										//groupType = GroupType[0];

										if (grpurl.Contains(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl))                                                      //"https://www.facebook.com/groups/"
										{
											try
											{
												aaaa = grpurl.Replace(FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl, string.Empty).Replace("/", string.Empty);
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.StackTrace);
											}

										}
										else
										{
											try
											{
												aaaa = grpurl.Replace(FBGlobals.Instance.fbhomeurl, string.Empty).Replace("/", string.Empty);                                            //"https://www.facebook.com/"
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.StackTrace);
											}
										}
										try
										{
											stradminlink = findstatus.Substring(findstatus.IndexOf("group_id="), (findstatus.IndexOf("\"", findstatus.IndexOf("group_id=")) - findstatus.IndexOf("group_id="))).Replace("group_id=", string.Empty).Replace("\"", string.Empty).Trim();
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.StackTrace);
										}
										aaaa = stradminlink;
										// string postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&fb_dtsg=" + fb_dtsg + "&__user=" + __user + "&phstamp=16581657884875010686";

										try
										{
											postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=9&fb_dtsg=" + fb_dtsg + "&__rev=1055839&ttstamp=265816690497512267";
											localstr = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl;                                                                         // "https://www.facebook.com/groups/"
											Responseofjoin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin);  //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.StackTrace);
										}

										if (Responseofjoin.Contains("jghdj"))
										{
											try
											{
												postdataforjoin = "ref=group_jump_header&group_id=" + aaaa + "&__user=" + __user + "&__a=1&__dyn=7n8apij35CFUSt2u5KIGKaExEW9ACxO4pbGAdGm&__req=9&fb_dtsg=" + fb_dtsg + "&__rev=1055839&ttstamp=265816690497512267";
												localstr = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl;                                                                         // "https://www.facebook.com/groups/"
												Responseofjoin = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin);  //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.StackTrace);
											}
										}


										crtstatus = "Request Sent";
										if (string.IsNullOrEmpty(Responseofjoin))
										{
											string reff = FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + aaaa;                                                                     //  "https://www.facebook.com/groups/"
											string strResponse = HttpHelper.postFormData(new Uri(FBGlobals.Instance.GroupGroupRequestManagerAjaxGroupsMembership), postdataforjoin);    //"https://www.facebook.com/ajax/groups/membership/r2j.php?__a=1"

										}
										if (Responseofjoin.Contains("[\"goURI(") || Responseofjoin.Contains(aaaa) || Responseofjoin.Contains("redirectPageTo") || Responseofjoin.Contains(CheckStatus))
										{

											AddToLogger_GroupManager("Request Sent to URL :" + grpurl + " with :" + fbUser.username);

										

											// objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl,Username);           
											try
											{
												string CSVHeader = "FbUser" + "," + "GroupURL";
												string CSV_Content = fbUser.username + "," + grpurl;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportLocationGroupRequest);
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

								if (currentstatus1.Contains("Notifications"))
								{
									try
									{
										//List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus1, "span", "fsm fcg");
										//groupType = GroupType[0];
										crtstatus = "Joined";

										// objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
										//if (!skipalrea dySent)
										{
											AddToLogger_GroupManager("Request Already Accepted On The URL : " + grpurl + " With UserName : " + fbUser.username);


										}
									}
									catch (Exception ex)
									{
										Console.WriteLine(ex.StackTrace);
									}

								}
								if (currentstatus1.Contains("Cancel Request"))
								{
									try
									{
									//	List<string> GroupType = HttpHelper.GetTextDataByTagAndAttributeName(findstatus, "span", "fsm fcg");
										//groupType = GroupType[0];
										crtstatus = "Request Already Sent";
										// objclsgrpmngr.InsertGroupmanager(Username, grpurl, groupType, crtstatus);
										// objclsgrpmngr.UpdateGroupDictionaryData(languageselect, grpurl);
										// if (!skipalreadySent)
										{
											AddToLogger_GroupManager("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
										
										//	DataBaseHandler.InsertQuery("Insert into GroupRequestUnique(CampaignName,URL,Account,Status) Values('" + GroupRequestCampaignName + "','" + Groupurl + "','" + fbUser.username + "','Sent')", "GroupRequestUnique");
											try
											{
												string CSVHeader = "FbUser" + "," + "GroupURL";
												string CSV_Content = fbUser.username + "," + grpurl;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);
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
								if (currentstatus1.Contains("Create Group"))
								{

									AddToLogger_GroupManager("Request Already Sent On The URL : " + grpurl + " With UserName : " + fbUser.username);
								}
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
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}

					try
					{
						int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
						AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

						Thread.Sleep(delayInSeconds);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
					}
					catch{};
				}
			
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.StackTrace);
			}
		}

		//Send Vedio in Group



		public void SendVideoMessageInGroup_Old24Sep(ref FacebookUser fbUser)
		{
			string UserId = string.Empty;
			bool IssendMessage = false;
			string grpurl = string.Empty;
			string message = string.Empty;
			string VideoUrl = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				List<string>lstallgroup_EachAccount = FindOwnGroupUrl(ref fbUser);

				foreach (var GroupUrl_item in lstallgroup_EachAccount)
				{

					try
					{
						message=lstGroupMessageGroupPoster[new Random().Next(0, lstGroupMessageGroupPoster.Count - 1)];
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex.StackTrace);

					}

					try
					{
						VideoUrl=lstGroupUrlsGoupPoster[new Random().Next(0, lstGroupUrlsGoupPoster.Count - 1)];
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}


					IssendMessage = PostVideoUrl(GroupUrl_item, message, VideoUrl, ref fbUser);

					if (IssendMessage)					
					{
						AddToLogger_GroupManager("Send Message : "+message+":"+VideoUrl+" Group Url : "+GroupUrl_item +"User Name : "+fbUser.username );
						try
						{
							int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
							AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
							Thread.Sleep(delayInSeconds);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error  >>>" + ex.StackTrace);
						}
					}
					else
					{
						AddToLogger_GroupManager("Send Message : " + message + ":" + VideoUrl + " Group Url : "+GroupUrl_item +"User Name : "+fbUser.username );
						try
						{
							int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
							AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
							Thread.Sleep(delayInSeconds);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error  >>>" + ex.StackTrace);
						}
					}

				}

			}
			catch(Exception Ex)
			{
				Console.WriteLine (Ex.StackTrace);
			}
		}



		public void SendVideoMessageInGroup(ref FacebookUser fbUser)
		{
			string UserId = string.Empty;
			bool IssendMessage = false;
			string grpurl = string.Empty;
			string message = string.Empty;
			string VideoUrl = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				List<string>lstallgroup_EachAccount = FindOwnGroupUrl(ref fbUser);

				foreach (var GroupUrl_item in lstallgroup_EachAccount)
				{
					for (int i = 0; i < GroupManager.noOfVideoUpload_per_Group; i++)
					{
						try
						{
							message = lstGroupMessageGroupPoster[i];

						}
						catch (Exception ex)
						{
							try
							{
								message = lstGroupMessageGroupPoster[new Random().Next(0, lstGroupMessageGroupPoster.Count - 1)];
							}
							catch
							{
								Console.WriteLine(ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendVideoMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}

						try
						{
							VideoUrl = lstGroupUrlsGoupPoster[i];
						}
						catch (Exception ex)
						{
							try
							{
								VideoUrl = lstGroupUrlsGoupPoster[new Random().Next(0, lstGroupUrlsGoupPoster.Count - 1)];
							}
							catch
							{
								Console.WriteLine(ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendVideoMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}

						IssendMessage = PostVideoUrl(GroupUrl_item, message, VideoUrl, ref fbUser);

						if (IssendMessage)
						{
							AddToLogger_GroupManager("Send Message : " + message + ":" + VideoUrl + " Group Url : " + GroupUrl_item + "User Name : " + fbUser.username);
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendVideoMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
						else
						{
							AddToLogger_GroupManager("Send Message : " + message + ":" + VideoUrl + " Group Url : " + GroupUrl_item + "User Name : " + fbUser.username);
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message + "  SendVideoMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
						}
					}

				}

			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  SendVideoMessageInGroup  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
		}










		public Dictionary<string, string> ChangeSummeryValueDic(Dictionary<string, string> dic)
		{
			Dictionary<string, string> _Dic = new Dictionary<string, string>();
			try
			{
				Dictionary<string, string> tempDic = new Dictionary<string, string>();
				tempDic = dic;

				foreach (var item in tempDic)
				{
					try
					{
						if (item.Key == "attachment[params][summary]")
						{
							if (item.Value.Contains("&"))
							{
								_Dic.Add(item.Key, item.Value.Replace("&", ""));

							}
						}
						else
						{
							_Dic.Add(item.Key, item.Value);
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

			return _Dic;
		}

		public bool PostVideoUrl_Old_Old_Old_100(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
		{



			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{
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
				string uithumbpager_width = "128";
				string uithumbpager_height = "128";
				string composertags_place = "";
				string composertags_place_name = "";
				string composer_predicted_city = "";
				//string composer_session_id="";
				string is_explicit_place = "";
				string composertags_city = "";
				string disable_location_sharing = "false";
				string audiencevalue = "80";
				string nctr_mod = "pagelet_timeline_recent";
				string UserId = "";
				string __a = "1";
				string phstamp = "";
				{

					string strFanPageURL = targeturl;

					string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL),"","");
					{
						UserId = GlobusHttpHelper.Get_UserID(strPageSource);

						fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

						xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
						xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
						xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
						xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
						xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
						xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
						xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

						xhpc_message_text = VideoUrl;
						xhpc_message = message + "      " + xhpc_message_text;
						string xhpc_message_textPostData=string.Empty;
						try
						{

							xhpc_message_text = Uri.EscapeDataString(xhpc_message);
							xhpc_message_textPostData=xhpc_message_text;
							if (VideoUrl.Contains("/photos/a"))
							{
								xhpc_message_text = Uri.EscapeDataString(xhpc_message).Replace("%2F", "%252F").Replace("%3F", "%253F");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						try
						{
						if (string.IsNullOrEmpty(UserId))
						{
							UserId = GlobusHttpHelper.ParseJson(strPageSource, "user");
						}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string strAjaxRequest1 = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxEmiEndPhpUrl + UserId + "&__a=1"),"","");


						string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"),"","");

						try
						{
						if (string.IsNullOrEmpty(composer_session_idSource))
						{
							composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"),"","");

						}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						try
						{
						if (string.IsNullOrEmpty(composer_session_idSource))
						{
							composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=2"),"","");

						}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

						if (string.IsNullOrEmpty(xhpc_composerid))
						{
							try
							{
								if (strPageSource.Contains("composerPostSection"))
								{
									xhpc_composerid = FBUtils.getBetween(strPageSource, "composerPostSection\" id=\"", "\"");

								}

							}
							catch { };


						}


						if (string.IsNullOrEmpty(xhpc_targetid))
						{
							try
							{

								if (strPageSource.Contains("targetID\":\""))
								{
									xhpc_targetid = FBUtils.getBetween(strPageSource, "targetID\":\"", "\"");
								}



							}
							catch { };
						}



						#region MyRegion
						if (VideoUrl.Contains("/photos/a"))
						{
							string post = "fb_dtsg=" + fb_dtsg + "&composerid=u_jsonp_8_r&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
							string Url = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
							string Res = HttpHelper.postFormData(new Uri(Url), post);
							string getUrl = "https://www.facebook.com/ajax/typeahead/groups/mentions_bootstrap?group_id=" + xhpc_targetid + "&work_user=false&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&sid=739416692916&request_id=5553ac82-96ba-48c1-a691-8307a113aa28&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=17&__rev=1522031&token=1417666641";

							string Res2 = HttpHelper.getHtmlfromUrl(new Uri(getUrl),"","");
							//string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=2309869772&attachment[params][0]=1472864976307114&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
							//string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av="+UserId;
							//string Resss = HttpHelper.postFormData(new Uri(UrlPost), post22);
						}

						#endregion
						try
						{
						if (composer_session_idSource.Contains("composer_session_id"))
						{
							composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

						}
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						string appid = string.Empty;

						string Responsed = string.Empty;
						try
						{
							string ss = string.Empty;
							string VUrl = string.Empty;
							string jhj = string.Empty;
							string kkk = string.Empty;
							string PostData = string.Empty;
							string FirstResponse=string.Empty;
							appid = GlobusHttpHelper.getBetween(strPageSource, "appid=", "&");
							try
							{


								FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(VideoUrl) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							string attachment_params = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][0]\\\" value=\\\"","\\\"");
							string attachment_params_urlInfo_canonical = GlobusHttpHelper.getBetween(FirstResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_urlInfo_final = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_urlInfo_user = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_favicon = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_title = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#x2018;", "").Replace("&#x2013;", "").Replace("&#x2019;", "");
							string attachment_params_summary = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

							// https://fbexternal-a.akamaihd.net/safe_image.php?d=AQC5tPYMGdfqMSZZ&w=470&h=246&url=http%3A%2F%2Fthechangewithin.net%2Fwp-content%2Fuploads%2F2014%2F11%2Fstupidity-virus-667x375.jpg&cfs=1&upscale=1

							string[] arr = System.Text.RegularExpressions.Regex.Split(FirstResponse, "scaledImageFitWidth");
							string attachment_params_images0 = string.Empty;
							if (arr.Count() > 1)
							{
								try
								{
								attachment_params_images0 = GlobusHttpHelper.getBetween(arr[1], "src=\\\"", "alt").Replace("\\", "").Replace("u00253A", "%3A").Replace("u00252", "%2").Replace("amp;", "").Replace("\"", "");
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
								//if (string.IsNullOrEmpty(attachment_params_images0))
								{
									attachment_params_images0 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
								}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}


							string attachment_params_medium = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_url = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_type = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_src = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_width = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_height = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_secure_url = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_type = GlobusHttpHelper.getBetween(FirstResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_source = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_domain = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_base_domain = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_title_len = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_summary_len = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_min_dimensions0 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_min_dimensions1 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_with_dimensions = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_pending = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_fetched = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_image_dimensions0 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_image_dimensions1 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_considered = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_selected = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_cap = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_type = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							//string FinalResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=fee06a9d-c617-4071-8ed3-e308f966370a&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DAtbJaKNmbJs&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");

							try
							{
								if (VideoUrl.Contains("http:"))
								{

								}
								else
								{

								}
								if (VideoUrl.Contains("http:"))
								{

									string[] FirstArr = Regex.Split(VideoUrl, "http:");

									VUrl = "http:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35ynzpQ9UmWWuUQxE &__req=c&phstamp=265816676120578769";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
								}
								else
								{
									string[] FirstArr = Regex.Split(VideoUrl, "https:");

									VUrl = "https:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

									//--------------------
									string PostUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
									string PostData2 = "fb_dtsg="+fb_dtsg+"&composerid="+xhpc_composerid+"&targetid="+xhpc_targetid+"&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user="+UserId+"&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
									Responsed = HttpHelper.postFormData(new Uri(PostUrl), PostData2);
								}

								if (string.IsNullOrEmpty(Responsed))
								{
									try
									{
										ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";


										Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

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
								if (VideoUrl.Contains("WWW") || VideoUrl.Contains("www"))
								{
									VideoUrl = "http://" + VideoUrl;
									string[] FirstArr = Regex.Split(VideoUrl, "http:");

									VUrl = "http:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";
									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
								}

							}
							appid = GlobusHttpHelper.getBetween(Responsed,"app_id\\\" value=\\\"", "\\\"");
							Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
							if (Responsed.Contains("name=") && Responsed.Contains("value="))
							{
								try
								{
									string[] strNameValue = Regex.Split(Responsed, "name=");
									foreach (var strNameValueitem in strNameValue)
									{
										try
										{
											if (strNameValueitem.Contains("value="))
											{
												string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
												if (strSplit.Contains("value="))
												{
													try
													{
														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());


														dicNameValue.Add(strName, strValue);
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
														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														string strValue = "0";

														dicNameValue.Add(strName, strValue);
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
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}

							// for changing summery data
							dicNameValue = ChangeSummeryValueDic(dicNameValue);

							string partPostData = string.Empty;
							foreach (var dicNameValueitem in dicNameValue)
							{
								try
								{

									if (dicNameValueitem.Key == "attachment[params][title]")
									{
										try
										{
											string value = dicNameValueitem.Value;
											string HTmlDEcode = HttpUtility.HtmlDecode(value);
											string UrlDEcode = HttpUtility.UrlEncode(HTmlDEcode);

											partPostData = partPostData + dicNameValueitem.Key + "=" + UrlDEcode + "&";
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
											partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
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

							//change partpost  data


							string attachment_params1 = string.Empty;
							string attachment_params0=string.Empty;

							try
							{
								attachment_params1 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][1]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\","").Trim();
								attachment_params0 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][0]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();

							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							partPostData = partPostData.Replace(" ", "+");

							string resp = string.Empty;
							string FinalPostData = string.Empty;
							bool ChkbGroupViewSchedulerTaskRemoveUrl=false;
							//if (ChkbGroupViewSchedulerTaskRemoveUrl == false)
							{
								try
								{
									if (VideoUrl.Contains("/photos/a"))
									{
										string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
										string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
										resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
									}
									else
									{
										string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
										string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
										string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
										if (string.IsNullOrEmpty(FirstResponse))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + Uri.EscapeDataString(xhpc_message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
										}
										else
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&xhpc_message_text=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message + "    :    " + VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
										}
									}

								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
						//	else
							{
								/*string[] Arr = Regex.Split(xhpc_message_text, "http");
								try
								{
									xhpc_message_text = Arr[0];
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}


								string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;

								try
								{
									if (VideoUrl.Contains("/photos/a"))
									{
										string post22 = "composer_session_id=86f5e57b-0319-4c5d-a581-fb313b2ccb9d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2231d42652%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1j%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A317760898329126%7D&xhpc_message_text=&xhpc_message=&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1+ "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418037348&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4XUKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=265816995851139912186556697&__rev=1522031";
										string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
										resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
									}
									else
									{
										messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
										string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
										string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
										if (string.IsNullOrEmpty(FirstResponse))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
										}
										else
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											string SecondPostUrl = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;

											string SecondPostData = "composer_session_id=" + composer_session_id + "&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_composerid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22aeeb80e9%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1l%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A1377865092436016%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1417760932&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=g&ttstamp=265817177997910611967866779&__rev=1520211";
											string secondRespoce = HttpHelper.postFormData(new Uri(SecondPostUrl), SecondPostData);
										}
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}*/

							}
							if (string.IsNullOrEmpty(resp))
							{
								try
								{
									resp = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxProfileComposerUrl), FinalPostData);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							if (!string.IsNullOrEmpty(resp))
							{
								if (!resp.Contains("Error") || resp.Contains("jsmods"))
								{
									return true;
								}
								else
								{
									return false;
								}
							}
							else
							{
								return false;
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
			return false;
		}

















		public bool PostVideoUrl(string targeturl, string message, string VideoUrl, ref FacebookUser fbUser)
		{



			GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
			try
			{
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
				string uithumbpager_width = "128";
				string uithumbpager_height = "128";
				string composertags_place = "";
				string composertags_place_name = "";
				string composer_predicted_city = "";
				//string composer_session_id="";
				string is_explicit_place = "";
				string composertags_city = "";
				string disable_location_sharing = "false";
				string audiencevalue = "80";
				string nctr_mod = "pagelet_timeline_recent";
				string UserId = "";
				string __a = "1";
				string phstamp = "";
				{

					string strFanPageURL = targeturl;

					string strPageSource = HttpHelper.getHtmlfromUrl(new Uri(strFanPageURL),"","");
					{
						UserId = GlobusHttpHelper.Get_UserID(strPageSource);

						fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(strPageSource);

						xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_composerid");
						xhpc_composerid = GlobusHttpHelper.GetParamValue(strPageSource, "composerid");
						xhpc_targetid = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_targetid");
						xhpc_context = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_context");
						xhpc_fbx = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_fbx");
						xhpc_timeline = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_timeline");
						xhpc_ismeta = GlobusHttpHelper.GetParamValue(strPageSource, "xhpc_ismeta");

						xhpc_message_text = VideoUrl;
						xhpc_message = message + "      " + xhpc_message_text;
						string xhpc_message_textPostData=string.Empty;
						try
						{

							xhpc_message_text = Uri.EscapeDataString(xhpc_message);
							xhpc_message_textPostData=xhpc_message_text;
							if (VideoUrl.Contains("/photos/a"))
							{
								xhpc_message_text = Uri.EscapeDataString(xhpc_message).Replace("%2F", "%252F").Replace("%3F", "%253F");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
						}

						try
						{
							if (string.IsNullOrEmpty(UserId))
							{
								UserId = GlobusHttpHelper.ParseJson(strPageSource, "user");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						string strAjaxRequest1 = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxEmiEndPhpUrl + UserId + "&__a=1"),"","");


						string composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"),"","");

						try
						{
							if (string.IsNullOrEmpty(composer_session_idSource))
							{
								composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMetacomposerStatusUrl + UserId + "&xhpc=composerTourStart&nctr[_mod]=pagelet_composer&__user=" + UserId + "&__a=1"),"","");

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
						}

						try
						{
							if (string.IsNullOrEmpty(composer_session_idSource))
							{
								composer_session_idSource = HttpHelper.getHtmlfromUrl(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=2"),"","");

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
						}

						#region MyRegion
						if (VideoUrl.Contains("/photos/a"))
						{
							string post = "fb_dtsg=" + fb_dtsg + "&composerid=u_jsonp_8_r&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
							string Url = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
							string Res = HttpHelper.postFormData(new Uri(Url), post);
							string getUrl = "https://www.facebook.com/ajax/typeahead/groups/mentions_bootstrap?group_id=" + xhpc_targetid + "&work_user=false&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&sid=739416692916&request_id=5553ac82-96ba-48c1-a691-8307a113aa28&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=17&__rev=1522031&token=1417666641";

							string Res2 = HttpHelper.getHtmlfromUrl(new Uri(getUrl),"","");
							//string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=2309869772&attachment[params][0]=1472864976307114&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
							//string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av="+UserId;
							//string Resss = HttpHelper.postFormData(new Uri(UrlPost), post22);
						}

						#endregion
						try
						{
							if (strPageSource.Contains("composer_session_id"))
							{
								composer_session_id = (composer_session_idSource.Substring(composer_session_idSource.IndexOf("composer_session_id"), composer_session_idSource.IndexOf("/>", composer_session_idSource.IndexOf("composer_session_id")) - composer_session_idSource.IndexOf("composer_session_id")).Replace("composer_session_id", string.Empty).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());

							}
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
						}




						if (string.IsNullOrEmpty(xhpc_composerid))
						{
							try
							{
								if (strPageSource.Contains("composerPostSection"))
								{
									xhpc_composerid = FBUtils.getBetween(strPageSource, "composerPostSection\" id=\"", "\"");

								}

							}
							catch { };


						}


						if (string.IsNullOrEmpty(xhpc_targetid))
						{
							try
							{

								if (strPageSource.Contains("targetID\":\""))
								{
									xhpc_targetid = FBUtils.getBetween(strPageSource, "targetID\":\"", "\"");
								}



							}
							catch { };
						}


						string appid = string.Empty;

						string Responsed = string.Empty;
						try
						{
							string ss = string.Empty;
							string VUrl = string.Empty;
							string jhj = string.Empty;
							string kkk = string.Empty;
							string PostData = string.Empty;
							string FirstResponse=string.Empty;
							appid = GlobusHttpHelper.getBetween(strPageSource, "appid=", "&");
							try
							{


								FirstResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + Uri.EscapeDataString(VideoUrl) + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&__av=" + UserId + "&composerurihash=2"), "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=prompt&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=ogtaggericon&loaded_components[5]=mainprivacywidget&loaded_components[6]=prompt&loaded_components[7]=mainprivacywidget&loaded_components[8]=ogtaggericon&loaded_components[9]=withtaggericon&loaded_components[10]=placetaggericon&loaded_components[11]=maininput&loaded_components[12]=withtagger&loaded_components[13]=placetagger&loaded_components[14]=explicitplaceinput&loaded_components[15]=hiddenplaceinput&loaded_components[16]=placenameinput&loaded_components[17]=hiddensessionid&loaded_components[18]=ogtagger&loaded_components[19]=citysharericon&loaded_components[20]=cameraicon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUWdBUgDyQqV8KVo&__req=b&ttstamp=265817197118828082100727676&__rev=1392897");
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							string attachment_params = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][0]\\\" value=\\\"","\\\"");
							string attachment_params_urlInfo_canonical = GlobusHttpHelper.getBetween(FirstResponse, "[params][urlInfo][canonical]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_urlInfo_final = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][urlInfo][final]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_urlInfo_user = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][urlInfo][user]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_favicon = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][favicon]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_title = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][title]\\\" value=\\\"", "\\\"").Replace("\\", "").Replace("&#x2018;", "").Replace("&#x2013;", "").Replace("&#x2019;", "");
							string attachment_params_summary = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][summary]\\\" value=\\\"", "\\\"").Replace("\\", "");

							// https://fbexternal-a.akamaihd.net/safe_image.php?d=AQC5tPYMGdfqMSZZ&w=470&h=246&url=http%3A%2F%2Fthechangewithin.net%2Fwp-content%2Fuploads%2F2014%2F11%2Fstupidity-virus-667x375.jpg&cfs=1&upscale=1

							string[] arr = System.Text.RegularExpressions.Regex.Split(FirstResponse, "scaledImageFitWidth");
							string attachment_params_images0 = string.Empty;
							if (arr.Count() > 1)
							{
								try
								{
									attachment_params_images0 = GlobusHttpHelper.getBetween(arr[1], "src=\\\"", "alt").Replace("\\", "").Replace("u00253A", "%3A").Replace("u00252", "%2").Replace("amp;", "").Replace("\"", "");
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							else
							{
								try
								{
									//if (string.IsNullOrEmpty(attachment_params_images0))
									{
										attachment_params_images0 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}


							string attachment_params_medium = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][medium]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_url = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][url]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_type = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_src = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][src]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_width = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][width]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_height = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][height]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_params_video0_secure_url = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][video][0][secure_url]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string attachment_type = GlobusHttpHelper.getBetween(FirstResponse, "attachment[type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_source = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][images][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_domain = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_base_domain = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[base_domain]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_title_len = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[title_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_summary_len = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[summary_len]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_min_dimensions0 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[min_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_min_dimensions1 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[min_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_with_dimensions = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_with_dimensions]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_pending = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_pending]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_fetched = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_fetched]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_image_dimensions0 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[image_dimensions][0]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_image_dimensions1 = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[image_dimensions][1]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_considered = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_considered]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_selected = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_selected]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_cap = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_cap]\\\" value=\\\"", "\\\"").Replace("\\", "");
							string link_metrics_images_type = GlobusHttpHelper.getBetween(FirstResponse, "link_metrics[images_type]\\\" value=\\\"", "\\\"").Replace("\\", "");
							//string FinalResponse = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=fee06a9d-c617-4071-8ed3-e308f966370a&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DAtbJaKNmbJs&xhpc_message=" + Uri.EscapeDataString(VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");

							try
							{
								if (VideoUrl.Contains("http:"))
								{

								}
								else
								{

								}
								if (VideoUrl.Contains("http:"))
								{

									string[] FirstArr = Regex.Split(VideoUrl, "http:");

									VUrl = "http:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=cameraicon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=mainprivacywidget&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=placetagger&loaded_components[16]=citysharericon&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8ahyj35ynzpQ9UmWWuUQxE &__req=c&phstamp=265816676120578769";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
								}
								else
								{
									string[] FirstArr = Regex.Split(VideoUrl, "https:");

									VUrl = "https:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";

									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

									//--------------------
									string PostUrl = "https://www.facebook.com/ajax/composerx/attachment/link/scraper/?scrape_url=" + xhpc_message_text + "&remove_url=%2Fajax%2Fcomposerx%2Fattachment%2Fstatus%2F&attachment_class=_4j&av=" + UserId + "&composerurihash=3";
									string PostData2 = "fb_dtsg="+fb_dtsg+"&composerid="+xhpc_composerid+"&targetid="+xhpc_targetid+"&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=ogtaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=maininput&loaded_components[6]=withtaggericon&loaded_components[7]=placetaggericon&loaded_components[8]=ogtaggericon&loaded_components[9]=mainprivacywidget&loaded_components[10]=withtagger&loaded_components[11]=placetagger&loaded_components[12]=explicitplaceinput&loaded_components[13]=hiddenplaceinput&loaded_components[14]=placenameinput&loaded_components[15]=hiddensessionid&loaded_components[16]=ogtagger&loaded_components[17]=citysharericon&loaded_components[18]=cameraicon&nctr[_mod]=pagelet_group_composer&__user="+UserId+"&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=16&ttstamp=26581708270659590517310648&__rev=1522031";
									Responsed = HttpHelper.postFormData(new Uri(PostUrl), PostData2);
								}

								if (string.IsNullOrEmpty(Responsed))
								{
									try
									{
										ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";


										Responsed = HttpHelper.postFormData(new Uri(ss), PostData);

									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
										GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
									}
								}
							}
							catch (Exception ex)
							{

								Console.WriteLine("Error : " + ex.StackTrace);
								if (VideoUrl.Contains("WWW") || VideoUrl.Contains("www"))
								{
									VideoUrl = "http://" + VideoUrl;
									string[] FirstArr = Regex.Split(VideoUrl, "http:");

									VUrl = "http:" + FirstArr[1];
									jhj = Uri.EscapeUriString(VUrl);
									kkk = Uri.EscapeDataString(VUrl);
									ss = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostLinkScraperUrl + kkk + "&composerurihash=1";
									PostData = "fb_dtsg=" + fb_dtsg + "&composerid="+xhpc_composerid+"&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&loaded_components[10]=explicitplaceinput&loaded_components[11]=hiddenplaceinput&loaded_components[12]=placenameinput&loaded_components[13]=hiddensessionid&loaded_components[14]=withtagger&loaded_components[15]=citysharericon&loaded_components[16]=placetagger&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__req=7&phstamp=1658166110566868110738";
									Responsed = HttpHelper.postFormData(new Uri(ss), PostData);
								}

							}
							appid = GlobusHttpHelper.getBetween(Responsed,"app_id\\\" value=\\\"", "\\\"");
							Dictionary<string, string> dicNameValue = new Dictionary<string, string>();
							if (Responsed.Contains("name=") && Responsed.Contains("value="))
							{
								try
								{
									string[] strNameValue = Regex.Split(Responsed, "name=");
									foreach (var strNameValueitem in strNameValue)
									{
										try
										{
											if (strNameValueitem.Contains("value="))
											{
												string strSplit = strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>"));
												if (strSplit.Contains("value="))
												{
													try
													{
														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("value=") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														string strValue = (strNameValueitem.Substring(strNameValueitem.IndexOf("value="), strNameValueitem.IndexOf("/>", strNameValueitem.IndexOf("value=")) - strNameValueitem.IndexOf("value=")).Replace("value=", string.Empty).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());


														dicNameValue.Add(strName, strValue);
													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
														GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
													}
												}
												else
												{
													try
													{
														string strName = (strNameValueitem.Substring(0, strNameValueitem.IndexOf("/>") - 0).Replace("\\\"", string.Empty).Replace("\\", string.Empty).Trim());
														string strValue = "0";

														dicNameValue.Add(strName, strValue);
													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
														GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
													}
												}
											}
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
										}
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}

							// for changing summery data
							dicNameValue = ChangeSummeryValueDic(dicNameValue);

							string partPostData = string.Empty;
							foreach (var dicNameValueitem in dicNameValue)
							{
								try
								{

									if (dicNameValueitem.Key == "attachment[params][title]")
									{
										try
										{
											string value = dicNameValueitem.Value;
											string HTmlDEcode = HttpUtility.HtmlDecode(value);
											string UrlDEcode = HttpUtility.UrlEncode(HTmlDEcode);

											partPostData = partPostData + dicNameValueitem.Key + "=" + UrlDEcode + "&";
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
										}
									}
									else
									{
										try
										{
											partPostData = partPostData + dicNameValueitem.Key + "=" + dicNameValueitem.Value + "&";
										}
										catch (Exception ex)
										{
											Console.WriteLine("Error : " + ex.StackTrace);
											GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
										}
									}

								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}

							//change partpost  data


							string attachment_params1 = string.Empty;
							string attachment_params0=string.Empty;

							try
							{
								attachment_params1 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][1]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\","").Trim();
								attachment_params0 = GlobusHttpHelper.getBetween(FirstResponse, "attachment[params][0]", "/>").Replace("value=", "").Replace("\"", "").Replace("\\ \\", "").Trim();

							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
								GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
							}
							partPostData = partPostData.Replace(" ", "+");

							string resp = string.Empty;
							string FinalPostData = string.Empty;
							bool ChkbGroupViewSchedulerTaskRemoveUrl=false;
							//if (ChkbGroupViewSchedulerTaskRemoveUrl == false)
							{
								try
								{
									if (VideoUrl.Contains("/photos/a"))
									{
										string post22 = "composer_session_id=84cd7817-e432-4489-900b-12d430d715e1&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e7f25be2%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_8_x%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_textPostData + "&xhpc_message=" + xhpc_message_textPostData + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1 + "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418017995&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=aJioFuy9k9loAESt2uu4aWiAy4DBzECQqbx2mbAKBiGtbHz6C_8Ey5poji-FeiWG8ADyFrF6ApvyHjpbQdy9EjVFEyfCw&__req=18&ttstamp=26581708270659590517310648&__rev=1522031";
										string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
										resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
									}
									else
									{
										string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
										string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
										string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
										if (string.IsNullOrEmpty(FirstResponse))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + Uri.EscapeDataString(xhpc_message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
										}
										else
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&xhpc_message_text=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message + "    :    " + VideoUrl) + "&xhpc_message=" + Uri.EscapeDataString(message + "   :    " + VideoUrl) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
										}
									}

								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							//	else
							{
								/*string[] Arr = Regex.Split(xhpc_message_text, "http");
								try
								{
									xhpc_message_text = Arr[0];
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}


								string messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;

								try
								{
									if (VideoUrl.Contains("/photos/a"))
									{
										string post22 = "composer_session_id=86f5e57b-0319-4c5d-a581-fb313b2ccb9d&fb_dtsg=" + fb_dtsg + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%2231d42652%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1j%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A317760898329126%7D&xhpc_message_text=&xhpc_message=&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params0 + "&attachment[params][1]=" + attachment_params1+ "&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1418037348&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4XUKezpUgDyQqUkBBzEy6Kdy8-&__req=h&ttstamp=265816995851139912186556697&__rev=1522031";
										string UrlPost = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;
										resp = HttpHelper.postFormData(new Uri(UrlPost), post22);
									}
									else
									{
										messages = "&aktion=post&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message_text;
										partPostData = partPostData.Replace("autocomplete=off", string.Empty).Replace(" ", string.Empty).Trim();
										string[] valuesArr = Regex.Split(partPostData, "&xhpc_composerid=");
										string PostDataa = valuesArr[1].Replace("&aktion=post", messages);
										if (string.IsNullOrEmpty(FirstResponse))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%227a49f95e%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1n%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409896431&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgSGGeqrWo8popyUW4-49UJ6KibKm58&__req=h&ttstamp=265817268571174879549949120&__rev=1400559");
										}
										else
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=" + xhpc_ismeta + "&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22df2130f0%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_2_t%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][urlInfo][canonical]=" + Uri.EscapeDataString(attachment_params_urlInfo_canonical) + "&attachment[params][urlInfo][final]=" + Uri.EscapeDataString(attachment_params_urlInfo_final) + "&attachment[params][urlInfo][user]=" + Uri.EscapeDataString(attachment_params_urlInfo_user) + "&attachment[params][favicon]=" + Uri.EscapeDataString(attachment_params_favicon) + "&attachment[params][title]=" + Uri.EscapeDataString(attachment_params_title) + "&attachment[params][summary]=" + Uri.EscapeDataString(attachment_params_summary) + "&attachment[params][images][0]=" + Uri.EscapeDataString(attachment_params_images0) + "&attachment[params][medium]=" + Uri.EscapeDataString(attachment_params_medium) + "&attachment[params][url]=" + Uri.EscapeDataString(attachment_params_url) + "&attachment[params][video][0][type]=" + Uri.EscapeDataString(attachment_params_video0_type) + "&attachment[params][video][0][src]=" + Uri.EscapeDataString(attachment_params_video0_src) + "&attachment[params][video][0][width]=" + attachment_params_video0_width + "&attachment[params][video][0][height]=" + attachment_params_video0_height + "&attachment[params][video][0][secure_url]=" + Uri.EscapeDataString(attachment_params_video0_secure_url) + "&attachment[type]=" + attachment_type + "&link_metrics[source]=" + link_metrics_source + "&link_metrics[domain]=" + link_metrics_domain + "&link_metrics[base_domain]=" + link_metrics_base_domain + "&link_metrics[title_len]=" + link_metrics_title_len + "&link_metrics[summary_len]=" + link_metrics_summary_len + "&link_metrics[min_dimensions][0]=" + link_metrics_min_dimensions0 + "&link_metrics[min_dimensions][1]=" + link_metrics_min_dimensions1 + "&link_metrics[images_with_dimensions]=" + link_metrics_images_with_dimensions + "&link_metrics[images_pending]=" + link_metrics_images_pending + "&link_metrics[images_fetched]=" + link_metrics_images_fetched + "&link_metrics[image_dimensions][0]=" + link_metrics_image_dimensions0 + "&link_metrics[image_dimensions][1]=" + link_metrics_image_dimensions1 + "&link_metrics[images_selected]=" + link_metrics_images_selected + "&link_metrics[images_considered]=" + link_metrics_images_considered + "&link_metrics[images_cap]=" + link_metrics_images_cap + "&link_metrics[images_type]=" + link_metrics_images_type + "&composer_metrics[best_image_w]=100&composer_metrics[best_image_h]=100&composer_metrics[image_selected]=0&composer_metrics[images_provided]=1&composer_metrics[images_loaded]=1&composer_metrics[images_shown]=1&composer_metrics[load_duration]=55&composer_metrics[timed_out]=0&composer_metrics[sort_order]=&composer_metrics[selector_type]=UIThumbPager_6&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409651262&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECQqbx2mbAKGiyGGEVFLO0xBxC9V8CdBUgDyQqVaybBg&__req=f&ttstamp=26581721151189910057824974119&__rev=1392897");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											resp = HttpHelper.postFormData(new Uri("https://www.facebook.com/ajax/updatestatus.php?__av=" + UserId), "composer_session_id=&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_targetid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22e2d79f89%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_jsonp_3_y%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A" + xhpc_targetid + "%7D&xhpc_message_text=" + Uri.EscapeDataString(message) + "&xhpc_message=" + Uri.EscapeDataString(message) + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[type]=" + attachment_type + "&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1409910176&action_type_id[0]=&object_str[0]=&object_id[0]=&og_location_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=" + composer_predicted_city + "&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7n8anEAMBlynzpQ9UoHFaeFDzECiq78hAKGgyiGGeqrWo8popyUWumnx2ubhHAyXBxi&__req=1g&ttstamp=2658171748611875701028211799&__rev=1400559");
										}
										if (resp.Contains("The message could not be posted to this Wall."))
										{
											string SecondPostUrl = "https://www.facebook.com/ajax/updatestatus.php?av=" + UserId;

											string SecondPostData = "composer_session_id=" + composer_session_id + "&fb_dtsg=" + fb_dtsg + "&xhpc_context=" + xhpc_context + "&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_targetid=" + xhpc_composerid + "&xhpc_publish_type=1&clp=%7B%22cl_impid%22%3A%22aeeb80e9%22%2C%22clearcounter%22%3A0%2C%22elementid%22%3A%22u_0_1l%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A1377865092436016%7D&xhpc_message_text=" + xhpc_message_text + "&xhpc_message=" + xhpc_message + "&aktion=post&app_id=" + appid + "&attachment[params][0]=" + attachment_params + "&attachment[params][1]=1073741915&attachment[type]=2&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1417760932&action_type_id[0]=&object_str[0]=&object_id[0]=&hide_object_attachment=0&og_suggestion_mechanism=&og_suggestion_logging_data=&icon_id=&composertags_city=&disable_location_sharing=false&composer_predicted_city=109237889094394&nctr[_mod]=pagelet_group_composer&__user=" + UserId + "&__a=1&__dyn=7nmanEyl2lm9udDgDxyKAEWCueyp9Esx6iWF3pqzCC-C26m4VoKezpUgDyQqUkBBzEy6Kdy8-&__req=g&ttstamp=265817177997910611967866779&__rev=1520211";
											string secondRespoce = HttpHelper.postFormData(new Uri(SecondPostUrl), SecondPostData);
										}
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}*/

							}
							if (string.IsNullOrEmpty(resp))
							{
								try
								{
									resp = HttpHelper.postFormData(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxProfileComposerUrl), FinalPostData);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
									GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
								}
							}
							if (!string.IsNullOrEmpty(resp))
							{
								if (!resp.Contains("Error") || resp.Contains("jsmods"))
								{
									return true;
								}
								else
								{
									return false;
								}
							}
							else
							{
								return false;
							}

						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
						}
					}
				}

			}

			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
				GlobusFileHelper.AppendStringToTextfileNewLine (ex.Message + "  PostVideoUrl  in GroupManager ", FBGlobals.AllExceptionLoggerFilePath);
			}
			return false;
		}






































		//send Text message in Group 


		public string Msgsendingcurrentstatus = string.Empty;
		public string Msgsendingstatus = string.Empty;
	
		public bool SendingMsgToGroups_Old101(string targeturl, string message, ref FacebookUser fbUser)
		{
			try
			{

				List<string> status =new List<string>();
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string msg = string.Empty;
				msg = message;
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl),"","");
				Thread.Sleep(100);
				try
				{
				// status = HttpHelper.GetTextDataByTagAndAttributeName(pgSrc_FanPageSearch, "span", "uiButtonText");
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
				try
				{
					for (int i = 0; i < status.Count; i++)
					{
						try
						{
							Msgsendingstatus = status[i];
							if (Msgsendingstatus != string.Empty)
							{
								Msgsendingcurrentstatus = Msgsendingstatus;

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

				// if (Msgsendingcurrentstatus.Contains("Notifications"))
				{

					if (pgSrc_FanPageSearch.Contains("xhpc_composerid") || pgSrc_FanPageSearch.Contains("xhpc_targetid"))
					{
						try
						{
							string __user = "";
							string fb_dtsg = "";
							string xhpc_composerid = string.Empty;
							string xhpc_targetid = string.Empty;
							int composer_session_id;
							string MESSAGE = string.Empty;

							try
							{
								MESSAGE = Uri.EscapeDataString(msg);

							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}


							try
							{
							__user = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "user");
							if (string.IsNullOrEmpty(__user))
							{
								__user = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "user");
							}
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}


							try
							{
							fb_dtsg = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "fb_dtsg");
							if (string.IsNullOrEmpty(fb_dtsg))
							{
								fb_dtsg = GlobusHttpHelper.ParseJson(pgSrc_FanPageSearch, "fb_dtsg");
							}
							}

							catch(Exception ex)
							{
							 Console.WriteLine(ex.StackTrace);
							}
							try
							{
							xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
							}
							catch(Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}



							if (string.IsNullOrEmpty(xhpc_composerid))
							{
								try
								{
									if (pgSrc_FanPageSearch.Contains("composerPostSection"))
									{
										xhpc_composerid = FBUtils.getBetween(pgSrc_FanPageSearch, "composerPostSection\" id=\"", "\"");

									}




								}
								catch { };


							}



							try
							{
							xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
							}
							catch(Exception ex)
							{
								Console.WriteLine(ex.StackTrace);
							}


							if (string.IsNullOrEmpty(xhpc_targetid))
							{
								try
								{

									if (pgSrc_FanPageSearch.Contains("targetID\":\""))
									{
										xhpc_targetid = FBUtils.getBetween(pgSrc_FanPageSearch, "targetID\":\"", "\"");
									}

								}
								catch { };
							}


							composer_session_id = Convert.ToInt32(GlobusHttpHelper.ConvertToUnixTimestamp(DateTime.Now));

							string composer_sessionid = composer_session_id.ToString();

							string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
							_rev = _rev.Replace("\":", string.Empty);



							string postdata = "fb_dtsg=" + fb_dtsg + "&postfromfull=true&xhpc_targetid=" + xhpc_targetid + "&xhpc_context=profile&xhpc_ismeta=1&xhpc_fbx=1&xhpc_timeline=&xhpc_composerid=" + xhpc_composerid + "&xhpc_message_text=" + MESSAGE + "&xhpc_message=" + MESSAGE + "&is_explicit_place=&composertags_place=&composertags_place_name=&composer_session_id=" + composer_session_id + "&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=1658165781016912151427";

							//  postdata = "composer_session_id=62c93be7-17d8-4f1b-b2aa-036f326a8734&fb_dtsg=AQCAABsi&xhpc_context=profile&xhpc_ismeta=1&xhpc_timeline=&xhpc_composerid=u_jsonp_5_c&xhpc_targetid=139180732957326&clp=%7B%22cl_impid%22%3A%220a090d8f%22%2C%22clearcounter%22%3A1%2C%22elementid%22%3A%22u_jsonp_5_r%22%2C%22version%22%3A%22x%22%2C%22parent_fbid%22%3A139180732957326%7D&xhpc_message_text=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&xhpc_message=****Great%20Gift%20****%2024K%20Gold%20foil%20playing%20cards%20%2C%20comes%20with%20authentication%20certificate.%20%C2%A319.99%20each%20with%20free%20deliver%20%2C%20payment%20on%20deliver%20once%20you%20have%20checked%20and%20you%20are%20happy%20with%20the%20item.%20Comes%20well%20presented%20in%20a%20mahogany%20%20solid%20box.%20Any%20question%20please%20message%20me%20%2Clook%20at%20image%20for%20more%20detail%20on%20how%20the%20item%20looks.&is_explicit_place=&composertags_place=&composertags_place_name=&tagger_session_id=1388385131&composertags_city=&disable_location_sharing=false&composer_predicted_city=&nctr[_mod]=pagelet_group_composer&__user=100007423262870&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=t&__rev=1062230&ttstamp=2658167656566115105";

							string posturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;

							string Response = string.Empty;
							try
							{
								Response = HttpHelper.postFormData(new Uri(posturl), postdata);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}
							if (string.IsNullOrEmpty(Response))
							{
								posturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUpdateStatusUrl;
								try
								{
									Response = HttpHelper.postFormData(new Uri(posturl), postdata);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							int length = Response.Length;

							if (Response.Contains(msg) || length > 5000 || Response.Contains("Your post has been submitted and is pending approval by an admin."))
							{
								try
								{
									string PostedUrl = string.Empty;
									if (Response.Contains("\"permalink") && Response.Contains("commentcount"))
									{

										//PostedUrl = GlobusHttpHelper.getBetween(Response, "\"permalink", "commentcount").Replace("\\", string.Empty).Replace("\":\"", string.Empty).Replace("\",\"",string.Empty);
										//PostedUrl = "https://www.facebook.com" + PostedUrl;
									}
									else if (Response.Contains("post pending approval.") || Response.Contains("Your post has been submitted and is pending approval by an admin."))
									{
										PostedUrl = "post pending approval.";
									}
									if (Response.Contains("Your post has been submitted and is pending approval by an admin."))
									{
										AddToLogger_GroupManager("Your post has been submitted and is pending approval by an admin.  " + " Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
									}
									else
									{
										AddToLogger_GroupManager(" Message : " + msg + " Sent To Group Url : " + targeturl + " With UserName : " + fbUser.username);
									}

									try
									{
										string CSVHeader = "UserAccount" + "," + "GroupUrl" + "," + "message" + "," + "PostedUrl" + "," + "DateTime";
										string CSV_Content = fbUser.username + "," + targeturl + "," + message + "," + PostedUrl + "," +DateTime.Now;
										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportExprotFilePath);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error  >>>" + ex.StackTrace);
									}

									return true;
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

							}
							else
							{
								Console.WriteLine("Message Sending Fail");
								return false;
							}

						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return false;
		}





		// Send Pic in Group 


		public void SendPicMessageInGroup(ref FacebookUser fbUser)
		{
			bool IssendMessage = false;
			string UserId = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string grpurl=string.Empty;
				string message=string.Empty;
				string Picpath=string.Empty;

				List<string>lstallgroup_EachAccount = FindOwnGroupUrl(ref fbUser);

				AddToLogger_GroupManager("Username : " + fbUser.username + " Get GroupUrl : " + lstallgroup_EachAccount.Count);
				foreach (var lstallgroup_item in lstallgroup_EachAccount)
				{
					grpurl=lstallgroup_item;

					for (int i = 0; i < GroupManager.noOfpicUpload_per_Group; i++)
					{

						try
						{
							message = lstGroupMessageGroupPoster[i];

						}
						catch (Exception ex)
						{
							try
							{
								message = lstGroupMessageGroupPoster[new Random().Next(0, lstGroupMessageGroupPoster.Count)];
							}
							catch (Exception Ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
							}
						}

						string Pic = "";
						if ((LstPicUrlsGroupCampaignManager.Count()!=0))
						{
							try
							{
								Pic = LstPicUrlsGroupCampaignManager[i];

							}
							catch (Exception Ex)
							{
								try
								{
									Pic = LstPicUrlsGroupCampaignManager[new Random().Next(0, LstPicUrlsGroupCampaignManager.Count)];
								}
								catch 
								{
									Console.WriteLine(Ex.StackTrace);
								};
							}

						}

						IssendMessage = SendingPicMsgToOwnGroup(grpurl, message, Pic, ref fbUser);

						if (IssendMessage)
						{
							AddToLogger_GroupManager("Uploaded  Picture : " + Pic + " Group Url : " + grpurl + "User Name : " + fbUser.username);
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
							}
						}
						else
						{
							AddToLogger_GroupManager("Picture  Not Send " + "Picture : " + Pic + " Group Url : " + grpurl + "User Name :" + fbUser.username);
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom(minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);
								Thread.Sleep(delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error  >>>" + ex.StackTrace);
							}
						}
					}

				}

			}
			catch(Exception Ex)
			{
				Console.WriteLine (Ex.StackTrace);
			}
		}













		// SendingPicMsgToOwnGroup
		string proxyAddress = string.Empty;
		string proxyPort = string.Empty;
		string proxyUsername = string.Empty;
		string proxyPassword = string.Empty;

		public List<string> LstPicUrlsGroupCampaignManager = new List<string>();

		public bool SendingPicMsgToOwnGroupOld(string targeturl, string message, string Pic, ref FacebookUser fbUser)
		{
			GlobusHttpHelper http = fbUser.globusHttpHelper;
			//FrmDominator objFrmDominator = new FrmDominator();
			// bool checkGroupCompaignReport = objFrmDominator.GroupCompaignReport();
			List<string > localImagePath = new List<string> ();
			if (string.IsNullOrEmpty(Pic))
			{
				try
				{
					Pic = LstPicUrlsGroupCampaignManager[new Random().Next(0, LstPicUrlsGroupCampaignManager.Count)];
					localImagePath=LstPicUrlsGroupCampaignManager;

				}
				catch(Exception Ex)
				{
					Console.WriteLine (Ex.StackTrace);
				}

			}
			if (string.IsNullOrEmpty(message))
			{
				try
				{

					message = lstGroupMessageGroupPoster[new Random().Next(0, lstGroupMessageGroupPoster.Count)];
				}
				catch(Exception Ex)
				{
					Console.WriteLine (Ex.StackTrace);
				}
			}


			try
			{
				int tempCountMain = 0;
				startAgainMain:


				string username = fbUser.username;
				string password = fbUser.password;
				string  proxyPort=(fbUser.proxyport);
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				bool ReturnPicstatus = false;
				int intProxyPort = 80;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				Regex IdCheck = new Regex("^[0-9]*$");
				if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
				{
					intProxyPort = int.Parse(proxyPort);
				}
				string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookWithouthomeUrl),"","");

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl),"","");


				if (pgSrc_FanPageSearch.Contains("uiIconText _51z7"))
				{

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
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");
					try
					{
						string Dialogposturl = string.Empty;
						string DialogPostData = string.Empty;
						string responseresult = string.Empty;
						try
						{
							Dialogposturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
							DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=withtaggericon&loaded_components[5]=placetaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";
							responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						if (string.IsNullOrEmpty(responseresult))
						{
							Dialogposturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
							responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
					try
					{
						string getresponse = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"),"","");
						if (string.IsNullOrEmpty(getresponse))
						{
							try
							{
								getresponse = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"),"","");

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


					string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
					_rev = _rev.Replace("\":", string.Empty);



					#region Intermediate Post - Waterfall
					{
						string intermediatePostData1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=1n&__rev=" + _rev + "&ttstamp=265816710110410481103";
						string intermediatePostURL1 = "https://www.facebook.com/ajax/composerx/attachment/media/chooser/?composerurihash=1";

						string intermediatePostResponse1 = HttpHelper.postFormData(new Uri(intermediatePostURL1), intermediatePostData1);
					}

					#endregion
					#region Intermediate Post - Waterfall
					{
						string intermediatePostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1o&__rev=" + _rev + "&ttstamp=265816710110410481103";
						string intermediatePostURL2 = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=2";

						string intermediatePostResponse2 = HttpHelper.postFormData(new Uri(intermediatePostURL2), intermediatePostData2);
					}

					#endregion

					#region Intermediate Post - Waterfall

					string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1p&__rev=" + _rev + "&ttstamp=265816710110410481103";
					string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/video/upload/?composerurihash=3";

					string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

					string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

					#endregion

					#region Intermediate Post - Waterfall commemntedCode

					//string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=u_0_u&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";
					//string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";

					//string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

					//string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

					#endregion


					string UploadPostUrl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUploadPhotosPostUrl;

					string imagePath = string.Empty;

					imagePath = Pic;
					string status = string.Empty;

					ReturnPicstatus =http.AddaPicture(ref HttpHelper, username, password,imagePath, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain);
	  



					if (!ReturnPicstatus && tempCountMain <= 1)
					{
						goto startAgainMain;
					}

					if (ReturnPicstatus)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			return false;

		}



		public bool SendingPicMsgToOwnGroup(string targeturl, string message, string Pic, ref FacebookUser fbUser)
		{
			GlobusHttpHelper http = fbUser.globusHttpHelper;
			//FrmDominator objFrmDominator = new FrmDominator();
			// bool checkGroupCompaignReport = objFrmDominator.GroupCompaignReport();
			List<string > localImagePath = new List<string> ();

			try
			{
				int tempCountMain = 0;
				startAgainMain:

				string username = fbUser.username;
				string password = fbUser.password;
				string  proxyPort=(fbUser.proxyport);
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				bool ReturnPicstatus = false;
				int intProxyPort = 80;
				string xhpc_composerid = string.Empty;
				string xhpc_targetid = string.Empty;
				string message_text = string.Empty;
				Regex IdCheck = new Regex("^[0-9]*$");
				if (!string.IsNullOrEmpty(proxyPort) && IdCheck.IsMatch(proxyPort))
				{
					intProxyPort = int.Parse(proxyPort);
				}
				string PageSrcHome = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookWithouthomeUrl),"","");

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(targeturl),"","");


				if (pgSrc_FanPageSearch.Contains("uiIconText _51z7"))
				{

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
					xhpc_composerid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "composerid");
					xhpc_targetid = GlobusHttpHelper.GetParamValue(pgSrc_FanPageSearch, "xhpc_targetid");






					if (string.IsNullOrEmpty(xhpc_composerid))
					{
						try
						{
							if (pgSrc_FanPageSearch.Contains("composerPostSection"))
							{
								xhpc_composerid = FBUtils.getBetween(pgSrc_FanPageSearch, "composerPostSection\" id=\"", "\"");

							}

						}
						catch { };

					}


					if (string.IsNullOrEmpty(xhpc_targetid))
					{
						try
						{

							if (pgSrc_FanPageSearch.Contains("targetID\":\""))
							{
								xhpc_targetid = FBUtils.getBetween(pgSrc_FanPageSearch, "targetID\":\"", "\"");
							}


						}
						catch { };
					}




					try
					{
						string Dialogposturl = string.Empty;
						string DialogPostData = string.Empty;
						string responseresult = string.Empty;
						try
						{
							Dialogposturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
							DialogPostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=mainprivacywidget&loaded_components[4]=withtaggericon&loaded_components[5]=placetaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&phstamp=16581679711110554116411";
							responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
						if (string.IsNullOrEmpty(responseresult))
						{
							Dialogposturl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostAjaxDialogPostUrl;
							responseresult = HttpHelper.postFormData(new Uri(Dialogposturl), DialogPostData);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
					try
					{
						string getresponse = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"),"","");
						if (string.IsNullOrEmpty(getresponse))
						{
							try
							{
								getresponse = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetAjaxMentionBootStrapUrl + xhpc_targetid + "&neighbor=" + xhpc_targetid + "&membership_group_id=" + xhpc_targetid + "&set_subtext=true&__user=" + __user + "&__a=1"),"","");

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


					string _rev = GlobusHttpHelper.getBetween(pgSrc_FanPageSearch, "svn_rev", ",");
					_rev = _rev.Replace("\":", string.Empty);



					#region Intermediate Post - Waterfall
					{
						string intermediatePostData1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n8a9EAMNpGu5k9UmAEyKepFomhEK49oKiWFamiFo&__req=1n&__rev=" + _rev + "&ttstamp=265816710110410481103";
						string intermediatePostURL1 = "https://www.facebook.com/ajax/composerx/attachment/media/chooser/?composerurihash=1";

						string intermediatePostResponse1 = HttpHelper.postFormData(new Uri(intermediatePostURL1), intermediatePostData1);
					}

					#endregion
					#region Intermediate Post - Waterfall
					{
						string intermediatePostData2 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1o&__rev=" + _rev + "&ttstamp=265816710110410481103";
						string intermediatePostURL2 = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=2";

						string intermediatePostResponse2 = HttpHelper.postFormData(new Uri(intermediatePostURL2), intermediatePostData2);
					}

					#endregion

					#region Intermediate Post - Waterfall

					string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=withtaggericon&loaded_components[2]=placetaggericon&loaded_components[3]=cameraicon&loaded_components[4]=mainprivacywidget&loaded_components[5]=withtaggericon&loaded_components[6]=mainprivacywidget&loaded_components[7]=cameraicon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1p&__rev=" + _rev + "&ttstamp=265816710110410481103";
					string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/video/upload/?composerurihash=3";

					string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

					string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

					#endregion

					#region Intermediate Post - Waterfall commemntedCode

					//string intermediatePostData = "fb_dtsg=" + fb_dtsg + "&composerid=u_0_u&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + __user + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";
					//string intermediatePostURL = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";

					//string intermediatePostResponse = HttpHelper.postFormData(new Uri(intermediatePostURL), intermediatePostData);

					//string value_qn = GlobusHttpHelper.ParseJson(intermediatePostResponse, "waterfallID");

					#endregion


					string UploadPostUrl = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerPostUploadPhotosPostUrl;

					string imagePath = string.Empty;

					imagePath = Pic;
					string status = string.Empty;

					ReturnPicstatus =http.AddaPicture(ref HttpHelper, username, password,imagePath, proxyAddress, proxyPort, proxyUsername, proxyPassword, targeturl, message, ref status, intermediatePostResponse, xhpc_targetid, xhpc_composerid, message, fb_dtsg, __user, pgSrc_FanPageSearch, ref tempCountMain);




					if (!ReturnPicstatus && tempCountMain <= 1)
					{
						goto startAgainMain;
					}

					if (ReturnPicstatus)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			return false;

		}





















		public static string qn = string.Empty;
		public bool AddaPicture(ref GlobusHttpHelper HttpHelper, string Username, string Password, List<string> localImagePath, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string targeturl, string message, ref string status, string pageSource_Home, string xhpc_targetid, string xhpc_composerid, string message_text, string fb_dtsg, string UsreId, string pageSource, ref int tempCountMain)        
		{


			int tempCount = 0;
			startAgain:

			bool isSentPicMessage = false;
		
			string photo_id = string.Empty;


			try
			{
			

				string composer_session_id = "";

				string tempresponse1 = "";
				///temp post
				{
					string source = "";
					string profile_id = "";
					string gridID = "";
					//  string qn = string.Empty;

					try
					{
						string Url = "https://www.facebook.com/ajax/composerx/attachment/media/upload/?composerurihash=1";
						string posturl1 = "fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&loaded_components[0]=maininput&loaded_components[1]=cameraicon&loaded_components[2]=withtaggericon&loaded_components[3]=placetaggericon&loaded_components[4]=mainprivacywidget&loaded_components[5]=cameraicon&loaded_components[6]=mainprivacywidget&loaded_components[7]=withtaggericon&loaded_components[8]=placetaggericon&loaded_components[9]=maininput&nctr[_mod]=pagelet_group_composer&__user=" + UsreId + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=i&phstamp=16581688688747595501";    //"fb_dtsg=" + fb_dtsg + "&composerid=" + xhpc_composerid + "&targetid=" + xhpc_targetid + "&istimeline=1&timelinelocation=composer&loaded_components[0]=maininput&loaded_components[1]=mainprivacywidget&loaded_components[2]=mainprivacywidget&loaded_components[3]=maininput&loaded_components[4]=explicitplaceinput&loaded_components[5]=hiddenplaceinput&loaded_components[6]=placenameinput&loaded_components[7]=hiddensessionid&loaded_components[8]=withtagger&loaded_components[9]=backdatepicker&loaded_components[10]=placetagger&loaded_components[11]=withtaggericon&loaded_components[12]=backdateicon&loaded_components[13]=citysharericon&nctr[_mod]=pagelet_timeline_recent&__user=" + UsreId + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=18&phstamp=1658168111112559866679";
						// string PostUrl = "city_id=" + CityIDS1 + "&city_page_id=" + city_page_id + "&city_name=" + CityName1 + "&is_default=false&session_id=1362404125&__user=" + UsreId + "&__a=1&__dyn=798aD5z5ynU&__req=z&fb_dtsg=" + fb_dtsg + "&phstamp=1658168111112559866165";
						string res11 = HttpHelper.postFormData(new Uri(Url), posturl1);


						try
						{
							source = res11.Substring(res11.IndexOf("source\":"), (res11.IndexOf(",", res11.IndexOf("source\":")) - res11.IndexOf("source\":"))).Replace("source\":", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();

						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
						if (string.IsNullOrEmpty(source))
						{
							source = GlobusHttpHelper.getBetween(res11, "source", "profile_id").Replace("\\\"","").Replace(",","").Replace(":","").Trim();

						}
						try
						{
							profile_id = res11.Substring(res11.IndexOf("profile_id\":"), (res11.IndexOf("}", res11.IndexOf("profile_id\":")) - res11.IndexOf("profile_id\":"))).Replace("profile_id\":", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
							if (profile_id.Contains(","))
							{
								profile_id = ParseEncodedJson(res11, "profile_id");
							}
							//"gridID":
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
						if (string.IsNullOrEmpty(profile_id))
						{
							profile_id = GlobusHttpHelper.getBetween(res11, "profile_id", "}").Replace("\\\"", "").Replace(",", "").Replace(":", "").Trim();
						}
						try
						{
							gridID = res11.Substring(res11.IndexOf("gridID\":"), (res11.IndexOf(",", res11.IndexOf("gridID\":")) - res11.IndexOf("gridID\":"))).Replace("gridID\":", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}
						if (string.IsNullOrEmpty(gridID))
						{
							gridID = GlobusHttpHelper.getBetween(res11, "gridID", ",").Replace("\\\"", "").Replace(",", "").Replace(":", "").Trim(); ;
						}


						try
						{
							composer_session_id = res11.Substring(res11.IndexOf("composer_session_id\":"), (res11.IndexOf("}", res11.IndexOf("composer_session_id\":")) - res11.IndexOf("composer_session_id\":"))).Replace("composer_session_id\":", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						try
						{
							if (string.IsNullOrEmpty(composer_session_id))
							{
								composer_session_id = res11.Substring(res11.IndexOf("composerID\":"), (res11.IndexOf("}", res11.IndexOf("composerID\":")) - res11.IndexOf("composerID\":"))).Replace("composerID\":", string.Empty).Replace("<dd>", string.Empty).Replace("\\", string.Empty).Replace("\"", string.Empty).Trim();

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.StackTrace);
						}

						try
						{
							qn =GlobusHttpHelper.getBetween(res11, "qn", "/>");
							qn =qn.Replace("\\\\\\\"","@");
							qn =GlobusHttpHelper.getBetween(qn, "@ value=@", "@");
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

					NameValueCollection nvc1 = new NameValueCollection();
					try
					{
						//message = Uri.EscapeDataString(message);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}

				
					nvc1.Add("fb_dtsg", fb_dtsg);
					nvc1.Add("source", source);
					nvc1.Add("profile_id", profile_id);
					nvc1.Add("grid_id", gridID);
					nvc1.Add("upload_id", "1024");
					nvc1.Add("qn", qn);


					string _rev =GlobusHttpHelper.getBetween(pageSource, "svn_rev", ",");
					_rev = _rev.Replace("\":", string.Empty);


					string uploadURL = "https://upload.facebook.com/ajax/composerx/attachment/media/saveunpublished?target_id=" + xhpc_targetid + "&__user=" + UsreId + "&__a=1&__dyn=7n88Oq9ccmqDxl2u5Fa8HzCqm5Aqbx2mbAKGiBAGm&__req=1t&fb_dtsg=" + fb_dtsg + "&__rev=" + _rev + "";
				   //  tempresponse1 = HttpHelper.HttpUploadFile_UploadPic_tempforsingle(ref HttpHelper, UsreId, uploadURL, "composer_unpublished_photo[]", "image/jpeg", localImagePath, nvc1, "", proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword);

					if (tempresponse1.ToLower().Contains("errorsummary") && tempresponse1.ToLower().Contains("There was a problem with this request. We're working on getting it fixed as soon as we can".ToLower()))
					{
						if (tempCount < 2)
						{
							System.Threading.Thread.Sleep(15000);
							tempCount++;
							goto startAgain;
						}
						else
						{
							tempCountMain++;
							return false;
						}
					}



				}

				NameValueCollection nvc = new NameValueCollection();
				try
				{
					//message = Uri.EscapeDataString(message);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
				nvc.Add("fb_dtsg", fb_dtsg);
				nvc.Add("xhpc_targetid", xhpc_targetid);
				nvc.Add("xhpc_context", "profile");
				nvc.Add("xhpc_ismeta", "1");
				nvc.Add("xhpc_fbx", "1");
				nvc.Add("xhpc_timeline", "");
				nvc.Add("xhpc_composerid", xhpc_composerid);
				nvc.Add("xhpc_message_text", message);
				nvc.Add("xhpc_message", message);
			


				string composer_unpublished_photo = "";
				try
				{
					string start_composer_unpublished_photo = Regex.Split(tempresponse1, "},\"")[1];// 



					int startIndex_composer_unpublished_photo = start_composer_unpublished_photo.IndexOf(",\"") + ",\"".Length;
					int endIndex_composer_unpublished_photo = start_composer_unpublished_photo.IndexOf("\"", startIndex_composer_unpublished_photo + 1);

					composer_unpublished_photo = start_composer_unpublished_photo.Substring(startIndex_composer_unpublished_photo, endIndex_composer_unpublished_photo - startIndex_composer_unpublished_photo);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}

				if (tempresponse1.Contains("composer_unpublished_photo"))
				{
					try
					{
						composer_unpublished_photo = tempresponse1.Substring(tempresponse1.IndexOf("composer_unpublished_photo[]"), tempresponse1.IndexOf("u003Cbutton") - tempresponse1.IndexOf("composer_unpublished_photo[]")).Replace("composer_unpublished_photo[]", "").Replace("value=", "").Replace("\\", "").Replace("\\", "").Replace("/>", "").Replace("\"", "").Trim();

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
				}
				///New test upload pic post
				string waterfallid = GlobusHttpHelper.ParseJson(pageSource_Home, "waterfallID");

				if (waterfallid.Contains("ar"))
				{
					waterfallid = qn;
				}


				string newpostURL = "https://upload.facebook.com/media/upload/photos/composer/?__user=" + UsreId + "&__a=1&__dyn=7n88QoAMNoBwXAw&__req=r&fb_dtsg=" + fb_dtsg + "";
				string newPostData = "";


				NameValueCollection newnvc = new NameValueCollection();
				try
				{
					//message = Uri.EscapeDataString(message);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}

				newnvc.Add("fb_dtsg", fb_dtsg);
				newnvc.Add("xhpc_targetid", xhpc_targetid);
				newnvc.Add("xhpc_context", "profile");
				newnvc.Add("xhpc_ismeta", "1");
				newnvc.Add("xhpc_fbx", "1");
				newnvc.Add("xhpc_timeline", "");
				newnvc.Add("xhpc_composerid", xhpc_composerid);
				newnvc.Add("xhpc_message_text", message);
				newnvc.Add("xhpc_message", message);

				newnvc.Add("composer_unpublished_photo[]", composer_unpublished_photo);
				newnvc.Add("album_type", "128");
				newnvc.Add("is_file_form", "1");
				newnvc.Add("oid", "");
				newnvc.Add("qn", waterfallid);
				newnvc.Add("application", "composer");
				newnvc.Add("is_explicit_place", "");
				newnvc.Add("composertags_place", "");
				newnvc.Add("composertags_place_name", "");
				newnvc.Add("composer_session_id", composer_session_id);
				newnvc.Add("composertags_city", "");
				newnvc.Add("vzdisable_location_sharing", "false");
				newnvc.Add("composer_predicted_city", "");



				string response =HttpHelper.HttpUploadFile_UploadPic(ref HttpHelper, UsreId, newpostURL, "file1", "image/jpeg", localImagePath, newnvc, targeturl, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword);//HttpUploadFile_UploadPic(ref HttpHelper, UsreId, "http://upload.facebook.com/media/upload/photos/composer/?__user=" + UsreId + "&__a=1&__dyn=7n88O49ccm9o-2Ki&__req=1c&fb_dtsg=" + fb_dtsg + "", "file1", "image/jpeg", localImagePath, nvc, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword);


				if (response.Contains("post this because it has a blocked link"))
				{
					try
					{
						AddToLogger_GroupManager("-------blocked link-------");
						return false;

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}

				}

			
				if (string.IsNullOrEmpty(response))
				{
					try
					{

						response = HttpHelper.HttpUploadFile_UploadPic(ref HttpHelper, UsreId, "https://upload.facebook.com/media/upload/photos/composer/?__user=" + UsreId + "&__a=1&__dyn=7n88O49ccm9o-2Ki&__req=1c&fb_dtsg=" + fb_dtsg + "", "file1", "image/jpeg", localImagePath, nvc, targeturl, proxyAddress, Convert.ToInt32(0), proxyUsername, proxyPassword);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.StackTrace);
					}
				}
				string posturl = "https://www.facebook.com/ajax/places/city_sharer_reset.php";
				string postdata = "__user=" + UsreId + "&__a=1&fb_dtsg=" + fb_dtsg + "&phstamp=1658167761111108210145";
				string responsestring = HttpHelper.postFormData(new Uri(posturl), postdata);
				try	
				{
					string okay = HttpHelper.getHtmlfromUrl(new Uri("https://3-pct.channel.facebook.com/pull?channel=p_" + UsreId + "&seq=3&partition=69&clientid=70e140db&cb=8p7w&idle=8&state=active&mode=stream&format=json"),"","");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}

				if (!string.IsNullOrEmpty(response) && response.Contains("payload\":{\"photo_fbid"))//response.Contains("photo.php?fbid="))
				{

					try
					{

						if (!response.Contains("errorSummary") || !response.Contains("error"))
						{
							isSentPicMessage = true;
						}
						if (response.Contains("Your post has been submitted and is pending approval by an admin"))
						{
							Console.WriteLine("Your post has been submitted and is pending approval by an admin." + "GroupUrl >>>" + targeturl);

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
			return isSentPicMessage;

		}
		public static string ParseEncodedJson(string data, string paramName)
		{
			try
			{
				data = data.Replace("&quot;", "\"");
				int startIndx = data.IndexOf("\"" + paramName + "\"") + ("\"" + paramName + "\"").Length + 1;
				int endIndx = data.IndexOf("\"", startIndx);

				string value = data.Substring(startIndx, endIndx - startIndx);
				value = value.Replace(",", "");
				return value;
			}
			catch (Exception)
			{

				return null;
			}
		}





		public void AddFriendsToGroup(ref FacebookUser fbUser)
		{
			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}



			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}


			string UserId = string.Empty;
			try
			{
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string pageSource_HomePage = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbhomeurl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				UserId = GlobusHttpHelper.GetParamValue(pageSource_HomePage, "user");
				if (string.IsNullOrEmpty(UserId))
				{
					UserId = GlobusHttpHelper.ParseJson(pageSource_HomePage, "user");
				}

				if (string.IsNullOrEmpty(UserId) || UserId == "0" || UserId.Length < 3)
				{
					AddToLogger_GroupManager("Please Check The Account : " + fbUser.username);


					return;
				}

				string fb_dtsg = GlobusHttpHelper.Get_fb_dtsg(pageSource_HomePage);

				// Find Total Friends
				List<string> lstFriendname = new List<string>();
				List<string> lstFriendId = new List<string>();
				List<string> lstfriendsId=new List<string>();
					
				if (!CheckGroupInviterViaEmails)
				{
					AddToLogger_GroupManager("Please wait finding the friends user id  : " + " with User Name : " + fbUser.username);
					lstfriendsId = FBUtils.GetAllFriends(ref HttpHelper, UserId);
					AddToLogger_GroupManager("Please wait finding the friends name : " + " with User Name : " + fbUser.username);
				}


				int friendAddLimit = AddNoOfFriendsGroupInviter + 20;
				//for (int i = 0; i < lstfriendsId.Count; i++) 
				List<string> lstFirendidAfterSelection = new List<string>();
				lstFirendidAfterSelection = lstfriendsId;

				for (int i = 0; i < friendAddLimit; i++)
				{
					int rndm =GlobusHttpHelper.GenerateRandom(0, lstFirendidAfterSelection.Count);
					try
					{
						//string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstfriendsId[i]));
						string strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri(FBGlobals.Instance.fbgraphUrl + lstFirendidAfterSelection[rndm]),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
						if (strFriendInfo.Contains("\"name\":"))
						{

							//   string strName = strFriendInfo.Substring(strFriendInfo.IndexOf("\"name\":"), strFriendInfo.IndexOf(",", strFriendInfo.IndexOf("\"name\":")) - strFriendInfo.IndexOf("\"name\":")).Replace("\"name\":", string.Empty).Replace("\"", string.Empty).Trim();
							string strName = GlobusHttpHelper.getBetween(strFriendInfo, "first_name\": \"", "\",\n ");
							lstFriendname.Add(strName);
							lstFriendId.Add(lstFirendidAfterSelection[rndm]);
							lstFirendidAfterSelection.Remove(lstFirendidAfterSelection[rndm]);
						}
					}

					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				//LstGroupUrlsGroupInviter.Add("https://www.facebook.com/groups/sitmmca12/");
				for (int i = 0; i < LstGroupUrlsGroupInviter.Count; i++)
				{
					try
					{
						string groupUrl = LstGroupUrlsGroupInviter[i];

						InviteFriendsToYourGroup(ref fbUser, ref lstFriendname, ref lstFriendId, groupUrl, UserId);
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

		//InviteFriendsToYourGroup


		private void InviteFriendsToYourGroupOld(ref FacebookUser fbUser, ref List<string> lstFriendname, ref List<string> lstFriend, string groupurl, string userId)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			string fb_dtsg = string.Empty;
			string group_id = string.Empty;
			string message_id = string.Empty;
			int counter = 1;


			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}


			try {
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string strGroupURLPageSource = HttpHelper.getHtmlfromUrl (new Uri (groupurl), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				//
				string PageSource = HttpHelper.getHtmlfromUrl (new Uri (FBGlobals.Instance.fbhomeurl + userId), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);;  //"https://www.facebook.com/"
				string frndCount = GlobusHttpHelper.getBetween (PageSource, "Friends<span class=\"_gs6\">", "</span>");



				//AddToLogger_GroupManager("Total No of  friends : " + frndCount + " with User Name : " + fbUser.username);

				if(CheckGroupInviterViaEmails)
				{
					lstFriend=lstGroupInviteEmails;
				}

				for (int i = 0; i < lstFriend.Count; i++)
				{
					try
					{

						string friendID = lstFriend [i];
						string friendName = string.Empty;

						if (counter > AddNoOfFriendsGroupInviter)
						{
							break;  
						}


						try
						{
							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg (strGroupURLPageSource);
							try
							{
								group_id = strGroupURLPageSource.Substring (strGroupURLPageSource.IndexOf ("group_id="), strGroupURLPageSource.IndexOf ("/", strGroupURLPageSource.IndexOf ("group_id=")) - strGroupURLPageSource.IndexOf ("group_id=")).Replace ("\"", string.Empty).Replace ("group_id=", string.Empty).Replace ("value=", string.Empty).Trim (); //Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "group_id");
							} 
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}
							if (group_id.Contains (" ")) {
								group_id = group_id.Substring (0, group_id.IndexOf (" "));
							}

							if (group_id.Contains ("&")) {
								try {
									group_id = group_id.Substring (0, group_id.IndexOf ("&"));
								} catch (Exception ex) {
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
								try {
									message_id = strGroupURLPageSource.Substring (strGroupURLPageSource.IndexOf ("message_id="), strGroupURLPageSource.IndexOf ("/", strGroupURLPageSource.IndexOf ("message_id=")) - strGroupURLPageSource.IndexOf ("message_id=")).Replace ("\"", string.Empty).Replace ("message_id=", string.Empty).Replace ("value=", string.Empty).Trim ();//Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "message_id");
								} catch (Exception ex) {
									Console.WriteLine ("Error : " + ex.StackTrace);
								}

								if (message_id.Contains (" ")) {
									message_id = message_id.Substring (0, message_id.IndexOf (" "));
								}

								if (message_id.Contains ("&")) {
									try {
										message_id = message_id.Substring (0, message_id.IndexOf ("&"));
									} catch (Exception ex) {
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}
							}
						} catch (Exception ex) {
							Console.WriteLine ("Error : " + ex.StackTrace);
						}



						if(CheckGroupInviterViaEmails)
						{

							try
							{
								foreach (var Emails in lstGroupInviteEmails)
								{
									try
									{
										string AjaxUrl="https://www.facebook.com/ajax/groups/members/add_get.php?group_id="+group_id+"&email=1&refresh=1&__asyncDialog=2&__user="+userId+"&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxembzESu49UJ6K59poW8xHzoyfw&__req=f&__rev=1547526";
										string AjaxUrlResponce = HttpHelper.getHtmlfromUrl (new Uri (AjaxUrl),"","");

										string PostDataUrl="https://www.facebook.com/ajax/groups/members/add_post.php?source=dialog_typeahead&group_id="+group_id+"&refresh=1";
										string FinalPostData="fb_dtsg="+fb_dtsg+"&members[0]="+Uri.EscapeDataString(Emails)+"&text_members[0]="+Uri.EscapeDataString(Emails)+"&__user="+userId+"&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxembzESu49UJ6K59poW8xHzoyfw&__req=h&ttstamp=26581711001198082111971128555&__rev=1547526";
										string PostResponce=HttpHelper.postFormData (new Uri (PostDataUrl), FinalPostData);



										if (PostResponce.Contains("DialogHideOnSuccess")||PostResponce.Contains("bootloadable"))
										{


											AddToLogger_GroupManager (counter + ") Email:" + Emails + " Added to Group  with UserName : " + fbUser.username);


											//if (!string.IsNullOrEmpty(GroupReportInviterExprotFilePath))
											{
												try 
												{
													string CSVHeader = "friendName" + "," + "userAccountName" + "," + "GroupUrl";
													string CSV_Content = friendName + "," + fbUser.username + "," + groupurl;
													//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportInviterExprotFilePath);
												} 
												catch (Exception ex)
												{
													Console.WriteLine ("Error : " + ex.StackTrace);
												}
											}

											//delay
											try 
											{
												int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
												AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

												Thread.Sleep (delayInSeconds);
											}
											catch (Exception ex) 
											{
												Console.WriteLine ("Error : " + ex.StackTrace);
											}
											counter=counter+1;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}
								break;
							}
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}

						}

						else
						{

						//Get Friends Name
						try {
							string strFriendInfo = HttpHelper.getHtmlfromUrl (new Uri (FBGlobals.Instance.fbgraphUrl + friendID), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
							if (strFriendInfo.Contains ("\"name\":")) {
								try {
									friendName = strFriendInfo.Substring (strFriendInfo.IndexOf ("\"name\":"), strFriendInfo.IndexOf (",", strFriendInfo.IndexOf ("\"name\":")) - strFriendInfo.IndexOf ("\"name\":")).Replace ("\"name\":", string.Empty).Replace ("\"", string.Empty).Trim ();
								} catch (Exception ex) {
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
								if (friendName == string.Empty) {
									try {
										friendName = strFriendInfo.Substring (strFriendInfo.IndexOf ("\"name\":"), strFriendInfo.IndexOf ("}", strFriendInfo.IndexOf ("\"name\":")) - strFriendInfo.IndexOf ("\"name\":")).Replace ("\"name\":", string.Empty).Replace ("\"", string.Empty).Trim ();
									} catch (Exception ex) {
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine ("Error : " + ex.StackTrace);
						}

						//string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + lstFriend[i] + "&freeform=" + lstFriendname[i] + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";

						string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + friendID + "&freeform=" + friendName + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";
						string lastResponseStatus = string.Empty;
						string strResponse = HttpHelper.postFormData (new Uri (FBGlobals.Instance.GroupInviterPostAjaxGroupsMembersAddUrl), strPostData);

						if (strResponse.Contains ("You don't have permission")) {
							AddToLogger_GroupManager ("You don't have permission to add members to this group by Username : " + fbUser.username + " groupurl : " + groupurl);
						

							int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
							AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

							Thread.Sleep (delayInSeconds);

							break;
						}
						else if (strResponse.Contains ("errorSummary\":"))
						{
							try
							{
								try 
								{
									string[] summaryArr = Regex.Split (strResponse, "errorSummary\":");
									summaryArr = Regex.Split (summaryArr [1], "\"");
									string errorSummery = summaryArr [1];
									string errorDiscription = summaryArr [5];

									AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

									counter = counter - 1;
								}
								catch (Exception ex) 
								{
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
								int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep (delayInSeconds);
							}
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}
						} 
						else if (strResponse.Contains ("has been added"))
						{
							AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);



							//if (!string.IsNullOrEmpty(GroupReportInviterExprotFilePath))
							{
								try 
								{
									string CSVHeader = "friendName" + "," + "userAccountName" + "," + "GroupUrl";
									string CSV_Content = friendName + "," + fbUser.username + "," + groupurl;
									//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportInviterExprotFilePath);
								} 
								catch (Exception ex)
								{
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
							}


							//delay
							try 
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep (delayInSeconds);
							}
							catch (Exception ex) 
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}
						} 
						else
						{
								AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);


							//delay
							try
							{
								int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep (delayInSeconds);
							} 
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}
						}
						}

					} 
					catch (Exception ex)
					{
						Console.WriteLine ("Error : " + ex.StackTrace);
					}
					counter = counter + 1;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine ("Error : " + ex.StackTrace);
			}

			AddToLogger_GroupManager ("Process completed with User Name : " + fbUser.username + " and URL is : " + groupurl);

		}







		private void InviteFriendsToYourGroup(ref FacebookUser fbUser, ref List<string> lstFriendname, ref List<string> lstFriend, string groupurl, string userId)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			string fb_dtsg = string.Empty;
			string group_id = string.Empty;
			string message_id = string.Empty;
			int counter = 1;


			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}


			try {
				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;

				string strGroupURLPageSource = HttpHelper.getHtmlfromUrl (new Uri (groupurl), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				//
				string PageSource = HttpHelper.getHtmlfromUrl (new Uri (FBGlobals.Instance.fbhomeurl + userId), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);;  //"https://www.facebook.com/"
				string frndCount = GlobusHttpHelper.getBetween (PageSource, "Friends<span class=\"_gs6\">", "</span>");



				//AddToLogger_GroupManager("Total No of  friends : " + frndCount + " with User Name : " + fbUser.username);

				if(CheckGroupInviterViaEmails)
				{
					lstFriend=lstGroupInviteEmails;
				}

				for (int i = 0; i < lstFriend.Count; i++)
				{
					try
					{

						string friendID = lstFriend [i];
						string friendName = string.Empty;

						if (counter > AddNoOfFriendsGroupInviter)
						{
							break;  
						}


						try
						{
							fb_dtsg = GlobusHttpHelper.Get_fb_dtsg (strGroupURLPageSource);
							try
							{
								group_id = strGroupURLPageSource.Substring (strGroupURLPageSource.IndexOf ("group_id="), strGroupURLPageSource.IndexOf ("/", strGroupURLPageSource.IndexOf ("group_id=")) - strGroupURLPageSource.IndexOf ("group_id=")).Replace ("\"", string.Empty).Replace ("group_id=", string.Empty).Replace ("value=", string.Empty).Trim (); //Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "group_id");
								try
								{
									if(group_id.Contains(">Remove<"))
									{
										group_id = group_id.Replace(">Remove<","");
									}
								}
								catch{};
							} 
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}
							if (group_id.Contains (" ")) {
								group_id = group_id.Substring (0, group_id.IndexOf (" "));
							}

							if (group_id.Contains ("&")) {
								try {
									group_id = group_id.Substring (0, group_id.IndexOf ("&"));
								} catch (Exception ex) {
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
								try {
									message_id = strGroupURLPageSource.Substring (strGroupURLPageSource.IndexOf ("message_id="), strGroupURLPageSource.IndexOf ("/", strGroupURLPageSource.IndexOf ("message_id=")) - strGroupURLPageSource.IndexOf ("message_id=")).Replace ("\"", string.Empty).Replace ("message_id=", string.Empty).Replace ("value=", string.Empty).Trim ();//Globussoft.GlobusHttpHelper.ParseJson(strGroupURLPageSource, "message_id");
								} catch (Exception ex) {
									Console.WriteLine ("Error : " + ex.StackTrace);
								}

								if (message_id.Contains (" ")) {
									message_id = message_id.Substring (0, message_id.IndexOf (" "));
								}

								if (message_id.Contains ("&")) {
									try {
										message_id = message_id.Substring (0, message_id.IndexOf ("&"));
									} catch (Exception ex) {
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}
							}
						} catch (Exception ex) {
							Console.WriteLine ("Error : " + ex.StackTrace);
						}



						if(CheckGroupInviterViaEmails)
						{

							try
							{
								foreach (var Emails in lstGroupInviteEmails)
								{
									try
									{
										string AjaxUrl="https://www.facebook.com/ajax/groups/members/add_get.php?group_id="+group_id+"&email=1&refresh=1&__asyncDialog=2&__user="+userId+"&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxembzESu49UJ6K59poW8xHzoyfw&__req=f&__rev=1547526";
										string AjaxUrlResponce = HttpHelper.getHtmlfromUrl (new Uri (AjaxUrl),"","");

										string PostDataUrl="https://www.facebook.com/ajax/groups/members/add_post.php?source=dialog_typeahead&group_id="+group_id+"&refresh=1";
										string FinalPostData="fb_dtsg="+fb_dtsg+"&members[0]="+Uri.EscapeDataString(Emails)+"&text_members[0]="+Uri.EscapeDataString(Emails)+"&__user="+userId+"&__a=1&__dyn=7nmanEyl2qm9udDgDxyIGzGpUW9ACxO4p9GgSmEVFLFwxBxembzESu49UJ6K59poW8xHzoyfw&__req=h&ttstamp=26581711001198082111971128555&__rev=1547526";
										string PostResponce=HttpHelper.postFormData (new Uri (PostDataUrl), FinalPostData);



										if (PostResponce.Contains("DialogHideOnSuccess")||PostResponce.Contains("bootloadable"))
										{


											AddToLogger_GroupManager (counter + ") Email:" + Emails + " Added to Group  with UserName : " + fbUser.username);


											//if (!string.IsNullOrEmpty(GroupReportInviterExprotFilePath))
											{
												try 
												{
													string CSVHeader = "friendName" + "," + "userAccountName" + "," + "GroupUrl";
													string CSV_Content = friendName + "," + fbUser.username + "," + groupurl;
													//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportInviterExprotFilePath);
												} 
												catch (Exception ex)
												{
													Console.WriteLine ("Error : " + ex.StackTrace);
												}
											}

											//delay
											try 
											{
												int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
												AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

												Thread.Sleep (delayInSeconds);
											}
											catch (Exception ex) 
											{
												Console.WriteLine ("Error : " + ex.StackTrace);
											}
											counter=counter+1;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}
								break;
							}
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}

						}

						else
						{

							//Get Friends Name
							try {
								string strFriendInfo = HttpHelper.getHtmlfromUrl (new Uri (FBGlobals.Instance.fbgraphUrl + friendID), "", "",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);


								if(string.IsNullOrEmpty(strFriendInfo))
								{
									try
									{
										strFriendInfo = HttpHelper.getHtmlfromUrl(new Uri("Http://www.facebook.com/"+friendID),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);
										friendName = FBUtils.getBetween(strFriendInfo,"pageTitle\">","</title>");

									}
									catch{};
								}




								else
								{
									if (strFriendInfo.Contains ("\"name\":")) {
										try {
											friendName = strFriendInfo.Substring (strFriendInfo.IndexOf ("\"name\":"), strFriendInfo.IndexOf (",", strFriendInfo.IndexOf ("\"name\":")) - strFriendInfo.IndexOf ("\"name\":")).Replace ("\"name\":", string.Empty).Replace ("\"", string.Empty).Trim ();
										} catch (Exception ex) {
											Console.WriteLine ("Error : " + ex.StackTrace);
										}
										if (friendName == string.Empty) {
											try {
												friendName = strFriendInfo.Substring (strFriendInfo.IndexOf ("\"name\":"), strFriendInfo.IndexOf ("}", strFriendInfo.IndexOf ("\"name\":")) - strFriendInfo.IndexOf ("\"name\":")).Replace ("\"name\":", string.Empty).Replace ("\"", string.Empty).Trim ();
											} catch (Exception ex) {
												Console.WriteLine ("Error : " + ex.StackTrace);
											}
										}
									}

								}

							}
							catch (Exception ex)
							{
								Console.WriteLine ("Error : " + ex.StackTrace);
							}

							//string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + lstFriend[i] + "&freeform=" + lstFriendname[i] + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";

							string strPostData = "fb_dtsg=" + fb_dtsg + "&group_id=" + group_id + "&source=typeahead&ref=&message_id=" + message_id + "&members=" + friendID + "&freeform=" + friendName + "&__user=" + userId + "&__a=1&phstamp=1658166769810210182162";
							string lastResponseStatus = string.Empty;
							string strResponse = HttpHelper.postFormData (new Uri (FBGlobals.Instance.GroupInviterPostAjaxGroupsMembersAddUrl), strPostData);

							if (strResponse.Contains ("You don't have permission")) {
								AddToLogger_GroupManager ("You don't have permission to add members to this group by Username : " + fbUser.username + " groupurl : " + groupurl);


								int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
								AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

								Thread.Sleep (delayInSeconds);

								break;
							}
							else if (strResponse.Contains ("errorSummary\":"))
							{
								try
								{
									try 
									{
										string[] summaryArr = Regex.Split (strResponse, "errorSummary\":");
										summaryArr = Regex.Split (summaryArr [1], "\"");
										string errorSummery = summaryArr [1];
										string errorDiscription = summaryArr [5];

										AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Error Summary : " + errorSummery + " And Error Description :" + errorDiscription + "  With UserName : " + fbUser.username);

										counter = counter - 1;
									}
									catch (Exception ex) 
									{
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
									int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
									AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep (delayInSeconds);
								}
								catch (Exception ex)
								{
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
							} 
							else if (strResponse.Contains ("has been added"))
							{
								AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);



								//if (!string.IsNullOrEmpty(GroupReportInviterExprotFilePath))
								{
									try 
									{
										string CSVHeader = "friendName" + "," + "userAccountName" + "," + "GroupUrl";
										string CSV_Content = friendName + "," + fbUser.username + "," + groupurl;
										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, GroupReportInviterExprotFilePath);
									} 
									catch (Exception ex)
									{
										Console.WriteLine ("Error : " + ex.StackTrace);
									}
								}


								//delay
								try 
								{
									int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
									AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep (delayInSeconds);
								}
								catch (Exception ex) 
								{
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
							} 
							else
							{
								AddToLogger_GroupManager (counter + ") Friend Name:" + friendName + " Added to Group  with UserName : " + fbUser.username);


								//delay
								try
								{
									int delayInSeconds = GlobusHttpHelper.GenerateRandom (minDelayGroupInviter * 1000, maxDelayGroupInviter * 1000);
									AddToLogger_GroupManager ("Delaying for " + delayInSeconds / 1000 + " Seconds With UserName : " + fbUser.username);

									Thread.Sleep (delayInSeconds);
								} 
								catch (Exception ex)
								{
									Console.WriteLine ("Error : " + ex.StackTrace);
								}
							}
						}

					} 
					catch (Exception ex)
					{
						Console.WriteLine ("Error : " + ex.StackTrace);
					}
					counter = counter + 1;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine ("Error : " + ex.StackTrace);
			}

			AddToLogger_GroupManager ("Process completed with User Name : " + fbUser.username + " and URL is : " + groupurl);

		}



















		//Group Posting 

		bool isStopGroupCamapinScheduler=false;




		public  List<string> LstGroupUrlsViewSchedulerTaskTargetedUrls = new List<string>();

		public 	bool CheckTargetedGroupUrlsUse=false;

		private List<string> FindOwnGroupUrl(ref FacebookUser fbUser)
		{
			
			//LstGroupUrlsViewSchedulerTaskTargetedUrls
			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(fbUser.proxyport))
			{
				intProxyPort = int.Parse(fbUser.proxyport);
			}


			List<string> list_Group = new List<string>();


			if (CheckUseManualLoadGroupUrls)
			{

				list_Group = LstGroupUrlsViewSchedulerTaskTargetedUrls;
				list_Group=list_Group.Distinct().ToList();

				if (list_Group.Count==0)
				{
					AddToLogger_GroupManager ("Please Load Targeted Group Url ");
				}
				return list_Group;
			}
			if (!CheckTargetedGroupUrlsUse)
			{

				GlobusHttpHelper HttpHelper = fbUser.globusHttpHelper;
				string pgSrc_FanPageSearch = HttpHelper.getHtmlfromUrl(new Uri(FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookBookMarksUrl),"","",fbUser.proxyip,intProxyPort,fbUser.proxyusername,fbUser.password);

				try
				{

					string[] allgroup = Regex.Split(pgSrc_FanPageSearch, "href=\"/groups/");
					foreach (string grpitem in allgroup)
					{
						try
						{

							if (!grpitem.Contains("<!DOCTYPE html>") && grpitem.Contains("/groups/") && grpitem.Contains("group_user"))
							{
								try
								{
									string[] group = Regex.Split(grpitem, "/");

									string itemgroups = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + group[0];
									list_Group.Add(itemgroups);
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
					list_Group = list_Group.Distinct().ToList();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}

				if (list_Group.Count() == 0)
				{

					//href="/ajax/bookmark/groups/leave/?group_id=             
					string[] allgroup = Regex.Split(pgSrc_FanPageSearch, "group_id");
					foreach (string grpitem in allgroup)
					{
						try
						{

							if (!grpitem.Contains("<!DOCTYPE html>") && (grpitem.Contains("favoriteOption") || grpitem.Contains("Leave Group")))
							{
								try
								{
									string group = GlobusHttpHelper.getBetween(grpitem, "=", "\"");
									if (group.Contains("&amp;"))
									{
										string[] arr = System.Text.RegularExpressions.Regex.Split(group, "&amp;");
										group = arr[0];
									}
									string itemgroups = FaceDominator.FBGlobals.Instance.GroupsGroupCampaignManagerGetFaceBookGroupUrl + group;
									list_Group.Add(itemgroups);
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

				}

				list_Group = list_Group.Distinct().ToList();
				return list_Group;
			}
			else
			{
				list_Group = LstGroupUrlsViewSchedulerTaskTargetedUrls.Distinct().ToList();
				return list_Group;
			}

		}



		///StartGroupMemberScraper


		#region Property For FriendInfoScraper

		public int NoOfThreadsGroupMemberScraper
		{
			get;
			set;
		}

		public static bool CheckScrapeCloseGroupUrlsScraper
		{
			get;
			set;
		}
		public static bool CheckScrapeOpenGroupUrlsScraper
		{
			get;
			set;
		}
		public static string ExportFilePathGroupMemberScraper
		{
			get;
			set;
		}
		public static string ExportFilePathGroupMemberScraperGroupUrls
		{
			get;
			set;
		}
		public static string ExportFilePathGroupMemberScraperTxt
		{
			get;
			set;
		}

		public static string StartProcessUsingGroupMemberScraper
		{
			get;
			set;
		}

		public static string GroupMemberScraperUsingAccount
		{
			get;
			set;
		}

		public List<string> LstGroupURLsFriendInfoScraper
		{
			get;
			set;
		}

		public int GroupUrlScraperCheckMembersMin
		{
			get;
			set;
		}
		public int GroupUrlScraperCheckMembersMax
		{
			get;
			set;
		}
		public static string ExportFilePathGroupMemberScraperByKeyWords
		{
			get;
			set;
		}
		/*public static List<string> LstOfGroupKeywords
		{
			get;
			set;
		}*/
		public static List<string> LstOfGroupKeywords = new List<string>();

		#endregion

		#region Global Variables For GroupMember Scraper

		readonly object lockrThreadControllerGroupScraper = new object();
		public bool isStopGroupMemberScraper = false;
		int countThreadControllerGroupMemberScraper = 0;
		public List<Thread> lstThreadsGroupMemberScraper = new List<Thread>();
		bool isStopFriendInfoScraper=true;

		Dictionary<string, string> DicIds = new Dictionary<string, string>();

		#endregion

		public void StartGroupMemberScraper()
		{
			try
			{
				int numberOfAccountPatch = 25;

				if (NoOfThreadsGroupMemberScraper > 0)
				{
					numberOfAccountPatch = NoOfThreadsGroupMemberScraper;
				}

				List<List<string>> list_listAccounts = new List<List<string>>();
				if (FBGlobals.listAccounts.Count >= 1)
				{

					list_listAccounts = GlobusHttpHelper.Split(FBGlobals.listAccounts, numberOfAccountPatch);

					foreach (List<string> listAccounts in list_listAccounts)
					{
						//int tempCounterAccounts = 0; 

						foreach (string account in listAccounts)
						{

							string[] AccountArr = GroupMemberScraperUsingAccount.Split(':');
							string Account = AccountArr[0];
							try
							{
								lock (lockrThreadControllerGroupScraper)
								{
									try
									{
										if (countThreadControllerGroupMemberScraper >= listAccounts.Count)
										{
											Monitor.Wait(lockrThreadControllerGroupScraper);
										}

										string acc = account.Remove(account.IndexOf(':'));
										if (Account==acc)
										{
											//Run a separate thread for each account

											FacebookUser item = null;
											FBGlobals.loadedAccountsDictionary.TryGetValue(acc, out item);

											if (item != null)
											{

												Thread GroupMemberThread = new Thread(StartMultiThreadsGroupMemberInfoScraper);
												GroupMemberThread.Name = "workerThread_Profiler_" + acc;
												GroupMemberThread.IsBackground = true;


												GroupMemberThread.Start(new object[] { item });

												countThreadControllerGroupMemberScraper++;
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
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}





		public void StartMultiThreadsGroupMemberInfoScraper(object parameters)
		{
			try
			{
				if (!isStopFriendInfoScraper)
				{
					try
					{
						lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
						lstThreadsGroupMemberScraper.Distinct();
						Thread.CurrentThread.IsBackground = true;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
					try
					{
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
								// Call StartActionEventInviter

							  GetGroupUrls(ref objFacebookUser);

							}
							else
							{
								AddToLogger_GroupManager("Couldn't Login With Username : " + objFacebookUser.username);

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

			finally
			{
				try
				{
					//  if (!isStopGroupMemberScraper)
					{
						lock (lockrThreadControllerGroupScraper)
						{
							countThreadControllerGroupMemberScraper--;
							Monitor.Pulse(lockrThreadControllerGroupScraper);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
			}
		}

		public List<string> sgroupid = new List<string>();
		public static bool opengrouptype = false;
		public static bool closegrouptype = false;

		public void GetGroupUrls(ref FacebookUser fbuse)
		{


			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			GlobusHttpHelper chilkatHttpHelper = fbuse.globusHttpHelper;

			Dictionary<string, string> CheckDuplicates = new Dictionary<string, string>();

			string Username = fbuse.username;
			int GetCountMember =0;



			try
			{

				//  if (Selectedusername == Username)
				{

					//GlobusLogHelper.log.Debug("Logged in  With UserName :" + Username);
					//GlobusLogHelper.log.Info("Logged in  With UserName :" + Username);

					foreach (string keyword in LstOfGroupKeywords)
					{

						AddToLogger_GroupManager("Started Group Scraper with : " +keyword);
						try
						{
							FindTheGroupUrls(chilkatHttpHelper, CheckDuplicates, Username, GetCountMember, keyword);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}

				      	AddToLogger_GroupManager("Process Completed with : " + Username + " Keyword : " + keyword);


					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}      

		}




		private void FindTheGroupUrlsOld(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				try
				{
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
								{
									lstLinkData.Add(itemurl);
									string strLink = itemurl.Substring(0, 70);

									if (strLink.Contains("group") && strLink.Contains("onclick"))
									{
										try
										{
											string[] tempArr = strLink.Split('"');
											string temp = tempArr[1];
											temp = temp.Replace("\\", "");
											temp = "https://www.facebook.com" + temp;   // "" 
											list.Add(temp);
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
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}


				list = list.Distinct().ToList();

				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
							if (GroupType[0].Contains("Closed Group"))
							{
								try
								{
									string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

									string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

									if (ownerlink[0].Contains("Admins"))
									{
										string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
										string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{

							
							foreach (string item in Groupmember)
							{
								try
								{
									if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
									{
										NoOfGroupMember = item;
									}
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							string groupType =string.Empty;
							try
							{
							 groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
							//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
								{
									//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", "  + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

												Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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

							}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}

							
							}


						/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}










		private void FindTheGroupUrlsOld2(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				try
				{
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
								{
									lstLinkData.Add(itemurl);
									string strLink = itemurl.Substring(0, 70);

									if (strLink.Contains("group") && strLink.Contains("onclick"))
									{
										try
										{
											string[] tempArr = strLink.Split('"');
											string temp = tempArr[1];
											temp = temp.Replace("\\", "");
											temp = "https://www.facebook.com" + temp;   // "" 
											list.Add(temp);
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
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}


				list = list.Distinct().ToList();

				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
								if (GroupType[0].Contains("Closed Group"))
								{
									try
									{
										string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

										string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

										if (ownerlink[0].Contains("Admins"))
										{
											string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
											string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{


								foreach (string item in Groupmember)
								{
									try
									{
										if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
										{
											NoOfGroupMember = item;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							string groupType =string.Empty;
							try
							{
								groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
									//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
									{
										//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

										try
										{

											if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
											{
												StreamReader  objStreamReader = new StreamReader(ExportFilePathGroupMemberScraperByKeyWords);
												string ReadExportFilePathGroupMemberScraperByKeyWords = objStreamReader.ReadToEnd();
												objStreamReader.Close();
												if(!ReadExportFilePathGroupMemberScraperByKeyWords.Contains(lsturl))
												{




													string Grppurl = string.Empty;
													string Grpkeyword = string.Empty;
													string GrpTypes = string.Empty;

													Grppurl = lsturl;
													Grpkeyword = strKeyword;
													GrpTypes = groupType;


													try
													{
														CheckDuplicates.Add(Grppurl, Grppurl);


														string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember";
														string CSV_Content = Grppurl + "," + Grpkeyword + ", "  + GetCountMember;

														string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

														Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

														//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
														AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
													}
												}
												else
												{
													AddToLogger_GroupManager("Data Aleady Exist in the csv path !");
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


							}


							/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}




		private void FindTheGroupUrls_Old_Old(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				try
				{
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
								{
									lstLinkData.Add(itemurl);
									string strLink = itemurl.Substring(0, 70);

									if (strLink.Contains("group") && strLink.Contains("onclick"))
									{
										try
										{
											string[] tempArr = strLink.Split('"');
											string temp = tempArr[1];
											temp = temp.Replace("\\", "");
											temp = "https://www.facebook.com" + temp;   // "" 
											list.Add(temp);
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
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}


				list = list.Distinct().ToList();

				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
								if (GroupType[0].Contains("Closed Group"))
								{
									try
									{
										string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

										string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

										if (ownerlink[0].Contains("Admins"))
										{
											string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
											string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{


								foreach (string item in Groupmember)
								{
									try
									{
										if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
										{
											NoOfGroupMember = item;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}
							string groupType =string.Empty;
							try
							{
								groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
									//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
									{
										//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

										try
										{

											if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
											{
												//StreamReader  objStreamReader = new StreamReader(ExportFilePathGroupMemberScraperByKeyWords,Encoding.GetEncoding(1250));
												//string ReadExportFilePathGroupMemberScraperByKeyWords = objStreamReader.ReadToEnd();
												//objStreamReader.Close();
												List<string> lst_wholeDataOfCsv = 	Globussoft.GlobusFileHelper.readcsvfile(ExportFilePathGroupMemberScraperByKeyWords);
												string wholeDataOfCsv="";
												foreach(string str in lst_wholeDataOfCsv)
												{
													wholeDataOfCsv = wholeDataOfCsv + str;

												}
												if(!wholeDataOfCsv.Contains(lsturl))
												{




													string Grppurl = string.Empty;
													string Grpkeyword = string.Empty;
													string GrpTypes = string.Empty;

													Grppurl = lsturl;
													Grpkeyword = strKeyword;
													GrpTypes = groupType;


													try
													{
														CheckDuplicates.Add(Grppurl, Grppurl);


														string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember";
														string CSV_Content = Grppurl + "," + Grpkeyword + ", "  + GetCountMember;

														string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

														Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);   //FBGlobals.path_LinuxSuccessFullyLike

														//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
														AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
													}
												}
												else
												{
													AddToLogger_GroupManager("Data Aleady Exist in the csv path !");
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


							}


							/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}














		private void FindTheGroupUrls_Old_Old_Old(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="

				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);
				ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");
				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				try
				{
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
								{
									lstLinkData.Add(itemurl);
									string strLink = itemurl.Substring(0, 70);

									if (strLink.Contains("group") && strLink.Contains("onclick"))
									{
										try
										{
											string[] tempArr = strLink.Split('"');
											string temp = tempArr[1];
											temp = temp.Replace("\\", "");
											temp = "https://www.facebook.com" + temp;   // "" 
											list.Add(temp);
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
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}


				list = list.Distinct().ToList();

				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
								if (GroupType[0].Contains("Closed Group"))
								{
									try
									{
										string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

										string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

										if (ownerlink[0].Contains("Admins"))
										{
											string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
											string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{


								foreach (string item in Groupmember)
								{
									try
									{
										if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
										{
											NoOfGroupMember = item;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}

							string fanpageTitle = "";
							if(findstatus.Contains("id=\"pageTitle\">"))
							{
								try
								{
									fanpageTitle = FBUtils.getBetween(findstatus,"id=\"pageTitle\">","</title>");
								}
								catch{};

							}

							string fanpageCatagory = "";
							if(findstatus.Contains("_5mo6"))
							{
								try
								{

									string[] fanpageCatagoryList = Regex.Split(findstatus,"_5mo6");
									fanpageCatagory = FBUtils.getBetween(fanpageCatagoryList[1],">","<");
								}
								catch{};

							}


							if(fanpageTitle.Contains("&amp"))
							{
								fanpageTitle = fanpageTitle.Replace("&amp" , "");
							}


							string groupType =string.Empty;
							try
							{
								groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
									//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
									{
										//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username   + " fanpageTitle : " + fanpageTitle + "fanpageCatagory : " + fanpageCatagory);

										try
										{

											if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
											{
												//StreamReader  objStreamReader = new StreamReader(ExportFilePathGroupMemberScraperByKeyWords,Encoding.GetEncoding(1250));
												//string ReadExportFilePathGroupMemberScraperByKeyWords = objStreamReader.ReadToEnd();
												//objStreamReader.Close();
												List<string> lst_wholeDataOfCsv = 	Globussoft.GlobusFileHelper.readcsvfile(ExportFilePathGroupMemberScraperByKeyWords);
												string wholeDataOfCsv="";
												foreach(string str in lst_wholeDataOfCsv)
												{
													wholeDataOfCsv = wholeDataOfCsv + str;

												}
												if(!wholeDataOfCsv.Contains(lsturl))
												{




													string Grppurl = string.Empty;
													string Grpkeyword = string.Empty;
													string GrpTypes = string.Empty;

													Grppurl = lsturl;
													Grpkeyword = strKeyword;
													GrpTypes = groupType;


													try
													{
														CheckDuplicates.Add(Grppurl, Grppurl);


														string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember" + "," + "PageTitle" + "," + "PageCatagory";
														string CSV_Content = Grppurl + "," + Grpkeyword + ", "  + GetCountMember+ ","   + fanpageTitle + ", "  + fanpageCatagory;

														string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

														Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);   //FBGlobals.path_LinuxSuccessFullyLike

														//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
														AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
													}
												}
												else
												{
													AddToLogger_GroupManager("Data Aleady Exist in the csv path !");
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


							}


							/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}





		private void FindTheGroupUrls_Old_Old_Old_Old_Old_12(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="
				strGroupUrl = "https://www.facebook.com/search/results/?q="+strKeyword+"&type=groups";
				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);



				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				/*
				if(!String.IsNullOrEmpty(ajaxRequestURL))
				{
					ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
					ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

					string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
					string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");

					try
					{
						foreach (string itemurl in Linklist)
						{
							try
							{
								if (!itemurl.Contains("<!DOCTYPE html"))
								{
									if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
									{
										lstLinkData.Add(itemurl);
										string strLink = itemurl.Substring(0, 70);

										if (strLink.Contains("group") && strLink.Contains("onclick"))
										{
											try
											{
												string[] tempArr = strLink.Split('"');
												string temp = tempArr[1];
												temp = temp.Replace("\\", "");
												temp = "https://www.facebook.com" + temp;   // "" 
												list.Add(temp);
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
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}

				}

*/


				{

					//<a href="/groups/
					try
					{
						string[] PageSplit = Regex.Split(pgSrc_FanPageSearch,"<a href=\"/groups");
						List<string> PageSplitList = PageSplit.ToList();
						PageSplitList.RemoveAt(0);
						foreach(string item in PageSplitList)
						{
							if(item.Contains("<!DOCTYPE html>"))
							{
								continue;
							}


							if(GroupManager.noOfGroupsToScrap <= list.Count())
							{
								break;
							}



							string groupId = FBUtils.getBetween(item,"/","/");
							if(!string.IsNullOrEmpty(groupId))
							{
								list.Add("https://www.facebook.com/groups/" +groupId);
							}
						}

					}
					catch{};

					int countForlistIteminPrvious =0;
					while(true)
					{
						countForlistIteminPrvious = list.Count();

						try
						{

							if(GroupManager.noOfGroupsToScrap <= list.Count())
							{
								AddToLogger_GroupManager("No of Groups Found To Scrap : " +  list.Count());
								break;
							}

							string[] PageSplit = Regex.Split(pgSrc_FanPageSearch,"rel=\"ajaxify\"");  //rel=\"ajaxify\"

							if(PageSplit.Count()==1)
							{
								string splitIt = "&amp;offset=";
								PageSplit = Regex.Split(pgSrc_FanPageSearch,splitIt);  //rel=\"ajaxify\"
								if(PageSplit.Count()>1)
								{

									PageSplit[1]  =  "/search/results/more/?q=" + strKeyword + "&amp;offset=" + PageSplit[1] ;
									ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"","\\\"");

								}




							}
							else
							{

								ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"href=\"","\"");
							}

							ajaxRequestURL = ajaxRequestURL.Replace("amp;","").Replace("type=all","type=groups").Replace("\\","%2C").Replace("u00252C","");

							ajaxRequestURL = "https://www.facebook.com" +  ajaxRequestURL + "&__user=" + __user + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" +  FBUtils.getBetween(PageSplit[1],"revision\":",",");

							pgSrc_FanPageSearch =  chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
							string allListGroup  = FBUtils.getBetween(pgSrc_FanPageSearch,"&quot;ents&quot;:&quot;","&quot");
							string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
							foreach(string item in Linklist)
							{


								if(GroupManager.noOfGroupsToScrap <= list.Count())
								{
									break;
								}


								try
								{
									if(!string.IsNullOrEmpty(item))
									{
										list.Add("https://www.facebook.com/groups/"+item);
										AddToLogger_GroupManager("Added Group Id : " +  item);	

									}
								}
								catch{};

							}
							list = list.Distinct().ToList();
							if(countForlistIteminPrvious==list.Count())
							{
								AddToLogger_GroupManager("No of Groups Found To Scrap  : " +  list.Count());
								break;
							}

						}
						catch{};
					}








				}









				list = list.Distinct().ToList();













				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
								if (GroupType[0].Contains("Closed Group"))
								{
									try
									{
										string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

										string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

										if (ownerlink[0].Contains("Admins"))
										{
											string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
											string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{


								foreach (string item in Groupmember)
								{
									try
									{
										if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
										{
											NoOfGroupMember = item;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}

							string fanpageTitle = "";
							if(findstatus.Contains("id=\"pageTitle\">"))
							{
								try
								{
									fanpageTitle = FBUtils.getBetween(findstatus,"id=\"pageTitle\">","</title>");
								}
								catch{};

							}

							string fanpageCatagory = "";
							if(findstatus.Contains("_5mo6"))
							{
								try
								{

									string[] fanpageCatagoryList = Regex.Split(findstatus,"_5mo6");
									fanpageCatagory = FBUtils.getBetween(fanpageCatagoryList[1],">","<");
								}
								catch{};

							}


							if(fanpageTitle.Contains("&amp"))
							{
								fanpageTitle = fanpageTitle.Replace("&amp" , "");
							}


							string groupType =string.Empty;
							try
							{
								groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
									//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
									{
										//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username   + " fanpageTitle : " + fanpageTitle + "fanpageCatagory : " + fanpageCatagory);

										try
										{

											if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
											{
												//StreamReader  objStreamReader = new StreamReader(ExportFilePathGroupMemberScraperByKeyWords,Encoding.GetEncoding(1250));
												//string ReadExportFilePathGroupMemberScraperByKeyWords = objStreamReader.ReadToEnd();
												//objStreamReader.Close();
												List<string> lst_wholeDataOfCsv = 	Globussoft.GlobusFileHelper.readcsvfile(ExportFilePathGroupMemberScraperByKeyWords);
												string wholeDataOfCsv="";
												foreach(string str in lst_wholeDataOfCsv)
												{
													wholeDataOfCsv = wholeDataOfCsv + str;

												}
												if(!wholeDataOfCsv.Contains(lsturl))
												{




													string Grppurl = string.Empty;
													string Grpkeyword = string.Empty;
													string GrpTypes = string.Empty;

													Grppurl = lsturl;
													Grpkeyword = strKeyword;
													GrpTypes = groupType;


													try
													{
														CheckDuplicates.Add(Grppurl, Grppurl);


														string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember" + "," + "PageTitle" + "," + "PageCatagory";
														string CSV_Content = Grppurl + "," + Grpkeyword + ", "  + GetCountMember+ ","   + fanpageTitle + ", "  + fanpageCatagory;

														string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

														Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);   //FBGlobals.path_LinuxSuccessFullyLike

														//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
														AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
													}
												}
												else
												{
													AddToLogger_GroupManager("Data Aleady Exist in the csv path !");
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


							}


							/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}












		private void FindTheGroupUrls(GlobusHttpHelper chilkatHttpHelper, Dictionary<string, string> CheckDuplicates, string Username, int GetCountMember, string keyword)
		{

			try
			{
				lstThreadsGroupMemberScraper.Add(Thread.CurrentThread);
				lstThreadsGroupMemberScraper.Distinct();
				Thread.CurrentThread.IsBackground = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			try
			{
				string strKeyword = keyword;
				string strGroupUrl = FBGlobals.Instance.fbfacebookSearchPhpQUrl + strKeyword + "&init=quick&type=groups";   // "https://www.facebook.com/search.php?q="
				strGroupUrl = "https://www.facebook.com/search/results/?q="+strKeyword+"&type=groups";
				string __user = "";
				string fb_dtsg = "";

				string pgSrc_FanPageSearch = chilkatHttpHelper.getHtmlfromUrl(new Uri(strGroupUrl),"","");



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

				List<string> pagesList = GetGroups_FBSearch(pgSrc_FanPageSearch);


				List<string> distinctPages = pagesList.Distinct().ToList();
				foreach (string distpage in distinctPages)
				{
					try
					{
						string distpage1 = distpage.Replace("d", "groups/");
						sgroupid.Add(distpage1);
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				string ajaxRequestURL = GetAjaxURL_MoreResults(pgSrc_FanPageSearch);



				List<string> list = new List<string>();
				List<string> lstLinkData = new List<string>();


				/*
				if(!String.IsNullOrEmpty(ajaxRequestURL))
				{
				ajaxRequestURL = FBGlobals.Instance.fbhomeurl + ajaxRequestURL + "&__a=1&__user=" + __user + "";   // "https://www.facebook.com/" 
				ajaxRequestURL = Uri.UnescapeDataString(ajaxRequestURL) + "&init=quick";

				string res_ajaxRequest = chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
				string[] Linklist = System.Text.RegularExpressions.Regex.Split(res_ajaxRequest, "href=");

				try
				{
					foreach (string itemurl in Linklist)
					{
						try
						{
							if (!itemurl.Contains("<!DOCTYPE html"))
							{
								if (!itemurl.Contains(@"http:\/\/www.facebook.com"))
								{
									lstLinkData.Add(itemurl);
									string strLink = itemurl.Substring(0, 70);

									if (strLink.Contains("group") && strLink.Contains("onclick"))
									{
										try
										{
											string[] tempArr = strLink.Split('"');
											string temp = tempArr[1];
											temp = temp.Replace("\\", "");
											temp = "https://www.facebook.com" + temp;   // "" 
											list.Add(temp);
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
				}
			
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}




				}


*/


				string[] PageSplit  = {};

				if(true)
				{
					try{
						PageSplit = Regex.Split(pgSrc_FanPageSearch,"group_id");
					}
					catch{};

					if(PageSplit.Count()!=1)
					{

						try
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
								if(GroupManager.noOfGroupsToScrap <= list.Count())
								{
									break;
								}



								string groupId = FBUtils.getBetween(item,"=","\"");
								if(!string.IsNullOrEmpty(groupId))
								{
									list.Add("https://www.facebook.com/groups/" +groupId);
									AddToLogger_GroupManager("Added Group Id : " +  groupId);	
								}
							}

						}
						catch{};
					}


					else
					{

						try
						{
							try
							{
								PageSplit = Regex.Split(pgSrc_FanPageSearch,"<a href=\"/groups");
							}
							catch{};
							List<string> PageSplitList = PageSplit.ToList();
							PageSplitList.RemoveAt(0);
							foreach(string item in PageSplitList)
							{
								if(item.Contains("<!DOCTYPE html>"))
								{
									continue;
								}

								list = list.Distinct().ToList();
								if(GroupManager.noOfGroupsToScrap <= list.Count())
								{
									break;
								}



								string groupId = FBUtils.getBetween(item,"/","/");
								if(!string.IsNullOrEmpty(groupId))
								{
									list.Add("https://www.facebook.com/groups/" +groupId);
									AddToLogger_GroupManager("Added Group Id : " +  groupId);	
								}
							}

						}
						catch{};
					}

					int countForlistIteminPrvious =0;
					int countFormaximumScrap = 0;
					while(true)
					{
						countFormaximumScrap++;
						if(GroupManager.noOfGroupsToScrap<=countFormaximumScrap)
						{
							AddToLogger_GroupManager("No. of Groups Found To Scrape : " +  list.Count());
							break;							
						}
						list = list.Distinct().ToList();
						countForlistIteminPrvious = list.Count();

						try
						{

							if(GroupManager.noOfGroupsToScrap <= list.Count())
							{
								AddToLogger_GroupManager("No of Groups Found To Scrape : " +  list.Count());
								break;
							}

							PageSplit = Regex.Split(pgSrc_FanPageSearch,"rel=\"ajaxify\"");  //rel=\"ajaxify\"

							if(PageSplit.Count()==1)
							{
								string splitIt = "&amp;offset=";
								PageSplit = Regex.Split(pgSrc_FanPageSearch,splitIt);  //rel=\"ajaxify\"

								if(PageSplit.Count()==1)
								{
									AddToLogger_GroupManager("All Group Id Scraped ");
									break;
								}


								if(PageSplit.Count()>1)
								{

									PageSplit[1]  =  "/search/results/more/?q=" + strKeyword + "&amp;offset=" + PageSplit[1] ;
									ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"","\\\"");

								}

							}
							else
							{

								ajaxRequestURL = FBUtils.getBetween(PageSplit[1],"href=\"","\"");
							}

							ajaxRequestURL = ajaxRequestURL.Replace("amp;","").Replace("type=all","type=groups").Replace("\\","%2C").Replace("u00252C","");

							ajaxRequestURL = "https://www.facebook.com" +  ajaxRequestURL + "&__user=" + __user + "&__a=1&__dyn=7AmajEyl35xKt2u6aEyx90BCxO4oKAdDgZ9LHwxBxCbzEeAq68K5Uc-dwIxbxjx27W88y98uyk4EKUyVWz9E&__req=c&__rev=" +  FBUtils.getBetween(PageSplit[1],"revision\":",",");

							pgSrc_FanPageSearch =  chilkatHttpHelper.getHtmlfromUrl(new Uri(ajaxRequestURL),"","");
							string allListGroup  = FBUtils.getBetween(pgSrc_FanPageSearch,"&quot;ents&quot;:&quot;","&quot");
							string[] Linklist = System.Text.RegularExpressions.Regex.Split(allListGroup, ",");
							foreach(string item in Linklist)
							{

								list = list.Distinct().ToList();
								if(GroupManager.noOfGroupsToScrap <= list.Count())
								{
									break;
								}


								try
								{
									if(!string.IsNullOrEmpty(item) && item.Count() < 20)
									{
										list.Add("https://www.facebook.com/groups/"+item);
										AddToLogger_GroupManager("Added Group Id : " +  item);	

									}
								}
								catch{};

							}
							if(countForlistIteminPrvious==list.Count())
							{
								AddToLogger_GroupManager("No of Groups Found To Scrape  : " +  list.Count());
								break;
							}
							list = list.Distinct().ToList();

						}
						catch{};
					}


				}

				list = list.Distinct().ToList();



				sgroupid.AddRange(list);
				//}
				foreach (string lsturl in sgroupid)
				{
					try
					{
						string findstatus = chilkatHttpHelper.getHtmlfromUrl(new Uri(lsturl),"","");


						GetCountMember = GetMemberCounts(GetCountMember, findstatus);

						//if (GroupUrlScraperCheckMembersMin <= GetCountMember && GroupUrlScraperCheckMembersMax >= GetCountMember)
						{
							List<string> GroupType =new List<string>();
							List<string> Groupmember = new List<string>();
							List<string> GroupName=new List<string>();



							try
							{
								if (GroupType[0].Contains("Closed Group"))
								{
									try
									{
										string[] owner = Regex.Split(findstatus, "uiInfoTable mtm profileInfoTable uiInfoTableFixed noBorder");

										string[] ownerlink = Regex.Split(owner[1], "uiProfilePortrait");

										if (ownerlink[0].Contains("Admins"))
										{
											string stradminlink = ownerlink[1].Substring(ownerlink[1].IndexOf("href=\""), (ownerlink[1].IndexOf(">", ownerlink[1].IndexOf("href=\"")) - ownerlink[1].IndexOf("href=\""))).Replace("href=\"", string.Empty).Replace("\"", string.Empty).Trim();
											string stradminname = ownerlink[1].Substring(ownerlink[1].IndexOf("/>"), (ownerlink[1].IndexOf("</a>", ownerlink[1].IndexOf("/>")) - ownerlink[1].IndexOf("/>"))).Replace("/>", string.Empty).Replace("\"", string.Empty).Trim();
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

							string NoOfGroupMember = string.Empty;
							if (Groupmember!=null)
							{


								foreach (string item in Groupmember)
								{
									try
									{
										if (!item.Contains("Facebook © 2012 English (US)") && item.Contains("members"))
										{
											NoOfGroupMember = item;
										}
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}

								}
							}
							if (findstatus.Contains("uiHeaderActions fsm fwn fcg"))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus,"uiHeaderActions fsm fwn fcg");
								if (Arr.Count()==3)
								{
									try
									{
										NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[2], "/\">", "members</a>").Replace(",", string.Empty);
									}
									catch (Exception ex)
									{
										Console.WriteLine("Error : " + ex.StackTrace);
									}
								}

							}
							if (string.IsNullOrEmpty(NoOfGroupMember))
							{
								string[] Arr = System.Text.RegularExpressions.Regex.Split(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection");

								try
								{
									NoOfGroupMember = GlobusHttpHelper.getBetween(Arr[1], "Members (", ")</h3>").Replace(",", string.Empty);
								}
								catch (Exception ex)
								{
									Console.WriteLine("Error : " + ex.StackTrace);
								}
							}

							string fanpageTitle = "";
							if(findstatus.Contains("id=\"pageTitle\">"))
							{
								try
								{
									fanpageTitle = FBUtils.getBetween(findstatus,"id=\"pageTitle\">","</title>");
								}
								catch{};

							}

							string fanpageCatagory = "";
							if(findstatus.Contains("_5mo6"))
							{
								try
								{

									string[] fanpageCatagoryList = Regex.Split(findstatus,"_5mo6");
									fanpageCatagory = FBUtils.getBetween(fanpageCatagoryList[1],">","<");
								}
								catch{};

							}


							if(fanpageTitle.Contains("&amp"))
							{
								fanpageTitle = fanpageTitle.Replace("&amp" , "");
							}


							string groupType =string.Empty;
							try
							{
								groupType = GroupType[0];
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error : " + ex.StackTrace);
							}

							//if (CheckScrapeOpenGroupUrlsScraper)
							{
								try
								{
									//	if (groupType.Contains("Open group") || groupType.Contains("Open Group") || groupType.Contains("Public Group"))
									{
										//objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);

										AddToLogger_GroupManager("Scraped GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username   + " fanpageTitle : " + fanpageTitle + "fanpageCatagory : " + fanpageCatagory);

										try
										{

											if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
											{
												//StreamReader  objStreamReader = new StreamReader(ExportFilePathGroupMemberScraperByKeyWords,Encoding.GetEncoding(1250));
												//string ReadExportFilePathGroupMemberScraperByKeyWords = objStreamReader.ReadToEnd();
												//objStreamReader.Close();
												List<string> lst_wholeDataOfCsv = 	Globussoft.GlobusFileHelper.readcsvfile(ExportFilePathGroupMemberScraperByKeyWords);
												string wholeDataOfCsv="";
												foreach(string str in lst_wholeDataOfCsv)
												{
													wholeDataOfCsv = wholeDataOfCsv + str;

												}
												if(!wholeDataOfCsv.Contains(lsturl))
												{




													string Grppurl = string.Empty;
													string Grpkeyword = string.Empty;
													string GrpTypes = string.Empty;

													Grppurl = lsturl;
													Grpkeyword = strKeyword;
													GrpTypes = groupType;


													try
													{
														CheckDuplicates.Add(Grppurl, Grppurl);


														string CSVHeader = "GroupUrl" + "," + "SearchKeyword" + "," + "NumberOfMember" + "," + "PageTitle" + "," + "PageCatagory";
														string CSV_Content = Grppurl.Replace(",","") + "," + Grpkeyword.Replace(",","") + ", "  + GetCountMember+ ","   + fanpageTitle.Replace(",","") + ", "  + fanpageCatagory.Replace(",","");

														string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" +  "," + GetCountMember;

														Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);   //FBGlobals.path_LinuxSuccessFullyLike

														//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
														AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



													}
													catch (Exception ex)
													{
														Console.WriteLine("Error : " + ex.StackTrace);
													}
												}
												else
												{
													AddToLogger_GroupManager("Data Aleady Exist in the csv path !");
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


							}


							/*	if (CheckScrapeCloseGroupUrlsScraper)
							{
								if (groupType.Contains("Closed Group") || groupType.Contains("Closed Group"))
								{
									//  objclsgrpmngr.InsertGroupUrl(strKeyword, lsturl, groupType, Selectedusername);
									AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);

									try
									{

										//if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords))
										{

											string Grppurl = string.Empty;
											string Grpkeyword = string.Empty;
											string GrpTypes = string.Empty;

											Grppurl = lsturl;
											Grpkeyword = strKeyword;
											GrpTypes = groupType;


											try
											{
												CheckDuplicates.Add(Grppurl, Grppurl);


												string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
												string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

												string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

												//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

												//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
												AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);



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
							}*/
							/*try
							{

								if (!string.IsNullOrEmpty(ExportFilePathGroupMemberScraperByKeyWords) && CheckScrapeCloseGroupUrlsScraper == false && CheckScrapeOpenGroupUrlsScraper == false)
								{
									string Grppurl = string.Empty;
									string Grpkeyword = string.Empty;
									string GrpTypes = string.Empty;

									Grppurl = lsturl;
									Grpkeyword = strKeyword;
									GrpTypes = groupType;                              


									try
									{
										CheckDuplicates.Add(Grppurl, Grppurl);

										AddToLogger_GroupManager("Scrap GroupUrl Is :" + lsturl + "  GroupMember : " + GetCountMember + " Keyword : " + strKeyword + "UserName : " + Username);


										string CSVHeader = "GroupUrl" + "," + "Groupkeyword" + ", " + "GroupTypes" + "," + "NumberOfMember";
										string CSV_Content = Grppurl + "," + Grpkeyword + ", " + GrpTypes + "," + GetCountMember;

										string Txt_Content = Grppurl + "\t\t\t" + "," + Grpkeyword + "\t\t\t" + ", " + GrpTypes + "\t\t\t" + "," + GetCountMember;

										//Globussoft.GlobusFileHelper.ExportDataCSVFile(CSVHeader, CSV_Content, ExportFilePathGroupMemberScraperByKeyWords);

										//Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine(Txt_Content, ExportFilePathGroupMemberScraperTxt);
										AddToLogger_GroupManager("Data Export In csv File !" + CSV_Content);

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
							}  */                   
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


			//return GetCountMember;
		}






























		private static int GetMemberCounts(int GetCountMember, string findstatus)
		{
			string GetTagCountMember = GlobusHttpHelper.getBetween(findstatus, "<div class=\"groupsAddMemberSideBox\">", "</div>");
			string GetGroupMemberCount = GlobusHttpHelper.getBetween(findstatus, "<span id=\"count_text\">", "</span>");

			if (string.IsNullOrEmpty(GetGroupMemberCount))
			{
				if (findstatus.Contains("uiHeader uiHeaderTopAndBottomBorder uiHeaderSection"))
				{
					try
					{
						string MemberCount = GlobusHttpHelper.getBetween(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection", "</h3>");
						MemberCount = MemberCount + "</h3>";
						GetGroupMemberCount = GlobusHttpHelper.getBetween(MemberCount, "<h3 class=\"accessible_elem\">", "</h3>");
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				else if (findstatus.Contains("<h6 class=\"accessible_elem\">About</h6>"))
				{
					try
					{
						string MemberCount = GlobusHttpHelper.getBetween(findstatus, "<h6 class=\"accessible_elem\">About</h6>", "</div>");
						MemberCount = GlobusHttpHelper.getBetween(MemberCount, "members/\">", "</a>");
						GetGroupMemberCount = MemberCount;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
			}
			GetGroupMemberCount = GetGroupMemberCount.Replace("members", "").Replace(",", "").Replace("Members", "").Replace("(", "").Replace(")", "");
			GetCountMember = Convert.ToInt32(GetGroupMemberCount);
			return GetCountMember;
		}



		private static int GetMemberCounts( string findstatus)
		{
			int GetCountMember = 0;
			string GetTagCountMember = GlobusHttpHelper.getBetween(findstatus, "<div class=\"groupsAddMemberSideBox\">", "</div>");
			string GetGroupMemberCount = GlobusHttpHelper.getBetween(findstatus, "<span id=\"count_text\">", "</span>");

			if (string.IsNullOrEmpty(GetGroupMemberCount))
			{
				if (findstatus.Contains("uiHeader uiHeaderTopAndBottomBorder uiHeaderSection"))
				{
					try
					{
						string MemberCount = GlobusHttpHelper.getBetween(findstatus, "uiHeader uiHeaderTopAndBottomBorder uiHeaderSection", "</h3>");
						MemberCount = MemberCount + "</h3>";
						GetGroupMemberCount = GlobusHttpHelper.getBetween(MemberCount, "<h3 class=\"accessible_elem\">", "</h3>");
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
				else if (findstatus.Contains("<h6 class=\"accessible_elem\">About</h6>"))
				{
					try
					{
						string MemberCount = GlobusHttpHelper.getBetween(findstatus, "<h6 class=\"accessible_elem\">About</h6>", "</div>");
						MemberCount = GlobusHttpHelper.getBetween(MemberCount, "members/\">", "</a>");
						GetGroupMemberCount = MemberCount;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}
				}
			}
			if (string.IsNullOrEmpty(GetGroupMemberCount))
			{
				return GetCountMember;
			}

			GetGroupMemberCount = GetGroupMemberCount.Replace("members", "").Replace(",", "").Replace("Members", "").Replace("(", "").Replace(")", "");     
			try
			{
				GetCountMember = Convert.ToInt32(GetGroupMemberCount);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
			return GetCountMember;
		}





		public static string GetAjaxURL_MoreResults(string pgSrc)
		{
			string pattern = "search/ajax/more.php?offset";

			if (pgSrc.Contains(pattern))
			{
				string URL = string.Empty;
				try
				{
					int startIndx = pgSrc.IndexOf(pattern);
					int endIndx = pgSrc.IndexOf("\"", startIndx);
					URL = pgSrc.Substring(startIndx, endIndx - startIndx);
					URL = URL.Replace("pagesize=10", "pagesize=300").Replace("&amp;", "&");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error : " + ex.StackTrace);
				}
				return URL;
			}
			return string.Empty;
		}


		public static List<string> GetGroups_FBSearch(string pgSrc)
		{
			List<string> lst_Pages = new List<string>();

			string splitPattern = "/hovercard/";

			string[] splitPgSrc = Regex.Split(pgSrc, splitPattern);

			foreach (string item in splitPgSrc)
			{
				if (!item.Contains("<!DOCTYPE html>"))
				{
					try
					{
						if (item.Contains("group.php?id"))
						{
							int startIndx = item.IndexOf("group.php?id=") + "group.php?id=".Length;
							int endIndx = item.IndexOf(">", startIndx);
							string pageURL = FBGlobals.Instance.fbhomeurl + "groups/" + item.Substring(startIndx, endIndx - startIndx).Replace("\"", "").Replace("=", "");

							lst_Pages.Add(pageURL);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error : " + ex.StackTrace);
					}

				}
			}

			return lst_Pages;
		}


	}
}

		









	


