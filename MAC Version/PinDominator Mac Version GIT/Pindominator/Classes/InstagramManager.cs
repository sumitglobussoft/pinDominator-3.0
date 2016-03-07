using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Specialized;

using System.Linq;
using System.Text.RegularExpressions;

using FaceDominator;

namespace facedominator
{
	public class InstagramManager
	{

		public static Event CampaignStopLogevents = new Event();

		public static void AddToLogger_Event(string log)
		{
			EventsArgs eArgs = new EventsArgs(log);
			CampaignStopLogevents.LogText(eArgs);
		}

		public static int minDelayInstagramManager= 10;
		public static int MaxDelayInstagramManager = 20;
		public static Regex IdCheck = new Regex("^[0-9]*$");
		public static bool IsCommentWithSpecificFollowers = false;
		public static int countNOOfFollowers=0;


		public static List<Thread> lstThreadInstagramManager = new List<Thread>();
		public static List<string> lstUploadCommentId = new List<string>();
		public static List<string> lstUploadCommentMsg = new List<string>();
		public static List<string> lstUploadFollowers = new List<string> ();
		public static List<string> lstUploadHashTag = new List<string>();
		public InstagramManager ()
		{
		}


		public bool ContainsUnicodeCharacter(string input)
		{
			const int MaxAnsiCode = 255;

			return input.Any(c => c > MaxAnsiCode);
		} 




		public string photolike(string PhotoId, ref InstagramUser objInstagramUser)
		{
			NameValueCollection namevalue = new NameValueCollection();
			string Photolink = string.Empty;
			string FollowedPageSource = string.Empty;
			string like = string.Empty;

			try
			{
				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(objInstagramUser.proxyport) && IdCheck.IsMatch(objInstagramUser.proxyport))
				{
					intProxyPort = int.Parse(objInstagramUser.proxyport);
				}

				if (!PhotoId.Contains("http://web.stagram.com/"))
				{
					Photolink = "http://websta.me/api/like/" + PhotoId + "/".Replace("http://websta.me/p/", "");
				}
				else
				{
					Photolink = PhotoId;
				}

				string PageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrl(new Uri(Photolink), "","");


				if (!PageContent.Contains("\"message\":\"LIKED\""))
				{
					PageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(Photolink), objInstagramUser.proxyip, intProxyPort, objInstagramUser.proxyusername, objInstagramUser.proxypassword);

					try
					{
						// FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri(url), commentPostData, CommentIdlink, "");
						if (!PageContent.Contains("\"message\":\"LIKED\""))
						{
							string postUrl = "https://www.instagram.com/web/likes/" + PhotoId + "/like/";
							string postData = "";
							PageContent = objInstagramUser.globusHttpHelper.PostDataWithInstagram(new Uri(postUrl), postData, "https://www.instagram.com/");
						}
					}
					catch { };




				}



				if (PageContent.Contains("message\":\"LIKED\""))
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_SuccessFullyLike);
					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_SuccessFullyLike);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_LinuxSuccessFullyLike);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_SuccessFullyLike);
					}

					FollowedPageSource = "LIKED";

				}
				else if (string.IsNullOrEmpty(FollowedPageSource))
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_FailedLike);
					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_FailedLike);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_LinuxFailedLike);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+PhotoId,FBGlobals.path_FailedLike);
					}
					FollowedPageSource = "All ready LIKED";
				}

			}
			catch
			{
			}
			return FollowedPageSource;
		}

		public static int counterPhotoId = 13;
		public static  List<string> GetPhotoId(string hashTag)
		{
			string url = "http://websta.me/" + "tag/" + hashTag;
			GlobusHttpHelper objInstagramUser = new GlobusHttpHelper ();
			List<string> lstPhotoId = new List<string> ();
			int counter = 0;


			string pageSource = objInstagramUser.getHtmlfromUrl(new Uri(url),"","");

			if (!string.IsNullOrEmpty (pageSource)) 
			{
				if (pageSource.Contains ("<div class=\"mainimg_wrapper\">")) 
				{
					string[] arr = Regex.Split (pageSource, "<div class=\"mainimg_wrapper\">");
					if (arr.Length > 1) 
					{
						arr = arr.Skip (1).ToArray ();
						foreach (string itemarr in arr)
						{
							try
							{
								string startString = "<a href=\"/p/";
								string endString = "\" class=\"mainimg\"";
								string imageId=string.Empty;
								string imageSrc = string.Empty;
								if(itemarr.Contains("<a href=\"/p/"))
								{
									int indexStart = itemarr.IndexOf("<a href=\"/p/");
									string itemarrNow = itemarr.Substring(indexStart);
									if (itemarrNow.Contains(startString) && itemarrNow.Contains(endString))
									{
										try
										{
											imageId = GlobusHttpHelper.getBetween(itemarrNow, startString, endString).Replace("/","");
										}
										catch { }
										if (!string.IsNullOrEmpty(imageId))
										{
											lstPhotoId.Add(imageId);
											lstPhotoId.Distinct();
											if(lstPhotoId.Count>=counterPhotoId)
											{
												return lstPhotoId;
											}

											//imageId = "http://websta.me"+imageId;
										}
									}
								}
							}
							catch(Exception ex)
							{

							}

						}




						#region pagination
						string pageLink = string.Empty;
						while (true)
						{
							//if (stopScrapImageBool) return;
							string startString = "<a href=\"";
							string endString = "\" class=\"mainimg\"";
							string imageId = string.Empty;
							string imageSrc = string.Empty;                                           

							if (!string.IsNullOrEmpty(pageLink))
							{
								pageSource = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"","");
							}

							if (pageSource.Contains("<ul class=\"pager\">") && pageSource.Contains("rel=\"next\">"))
							{
								try
								{
									pageLink = GlobusHttpHelper.getBetween(pageSource, "<ul class=\"pager\">", "rel=\"next\">");
								}
								catch { }
								if (!string.IsNullOrEmpty(pageLink))
								{
									try
									{
										int len = pageLink.IndexOf("<a href=\"");
										len = len + ("<a href=\"").Length;
										pageLink = pageLink.Substring(len);
										pageLink = pageLink.Trim();
										pageLink = pageLink.TrimEnd(new char[] { '"' });
										pageLink = "http://websta.me/" + pageLink;
									}
									catch { }
									if (!string.IsNullOrEmpty(pageLink))
									{
										string response = string.Empty;
										try
										{
											response = objInstagramUser.getHtmlfromUrl(new Uri(pageLink),"","");
										}
										catch { }
										if (!string.IsNullOrEmpty(response))
										{
											if (response.Contains("<div class=\"mainimg_wrapper\">"))
											{
												try
												{
													string[] arr1 = Regex.Split(response, "<div class=\"mainimg_wrapper\">");
													if (arr1.Length > 1)
													{
														arr1 = arr1.Skip(1).ToArray();
														foreach (string items in arr1)
														{
															try
															{
																//if (stopScrapImageBool) return;
																if (items.Contains("<a href=\"/p/"))
																{
																	int indexStart = items.IndexOf("<a href=\"/p/");
																	string itemarrNow = items.Substring(indexStart);

																	try
																	{
																		imageId = GlobusHttpHelper.getBetween(itemarrNow, startString, endString).Replace("/","");
																	}
																	catch { }
																	if (!string.IsNullOrEmpty(imageId))
																	{
																		lstPhotoId.Add(imageId);
																		lstPhotoId.Distinct();
																		if(lstPhotoId.Count>=counterPhotoId)
																		{
																			return lstPhotoId;
																		}

																		//imageId = "http://websta.me"+imageId;
																	}


																	counter++;

																	//Addtologger("Image DownLoaded with ImageName  "+imageId+"_"+counter);
																	if(lstPhotoId.Count>=counterPhotoId)
																	{
																		return lstPhotoId;
																	}

																}

															}
															catch { }
														}
														if(lstPhotoId.Count>=counterPhotoId)
														{
															return lstPhotoId;
														}
													}
												}
												catch { }

											}
										}
										else
										{

										}

									}
									else
									{
										break;
									}
								}
								else
								{
									break;
								}
							}
							else
							{
								break;
							}
						}
						#endregion




					}
				}
			}
			return lstPhotoId;
		}





		public string FollowOld(string UserName, ref InstagramUser objInstagramUser)
		{
			try 
			{

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(objInstagramUser.proxyport) && IdCheck.IsMatch(objInstagramUser.proxyport))
			{
				intProxyPort = int.Parse(objInstagramUser.proxyport);
			}


			NameValueCollection nameval = new NameValueCollection();
			if (!UserName.Contains("http://websta.me/n/"))
			{
				UserName = "http://websta.me/n/" + UserName + "/";
			}
			string UserPageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserName),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);

			try
 			{

				string PK = string.Empty;
				if (UserPageContent.Contains(""))
				{
					PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"follow_btn_wrapper\"", ">").Replace("data-target=", "").Replace("\"","").Trim();
				}

				if (string.IsNullOrEmpty(PK))
				{
					PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"message_user_id", ">").Replace(">", "").Replace("value=",string.Empty).Replace("\"",string.Empty).Trim();//.Replace("\"", "").Trim();
				}

				string PostData = "action=follow";//"&pk=" + PK + "&t=9208";
				string FollowedPageSource=string.Empty;

				FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri("http://websta.me/api/relationships/" + PK), PostData, UserName, "http://websta.me");
				//nameval.Add("Origin", "http://web.stagram.com");
				//nameval.Add("X-Requested-With", "XMLHttpRequest");

				if (FollowedPageSource.Contains("OK"))
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);

					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxSuccessFullyFollowing);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
					}
					return "Followed";
				}
				else
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxFailedFollowing);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
					}
					return "UnFollowed";
				}
			}
			catch (Exception ex)
			{
				return "Follow option is not available In page...!!";
				//Console.Write (ex.Message);
			}
			} 
			catch (Exception ex) {
				return "UnFollowed";
			}

		}








		public string Follow(string UserName, ref InstagramUser objInstagramUser)
		{
			try 
			{

				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(objInstagramUser.proxyport) && IdCheck.IsMatch(objInstagramUser.proxyport))
				{
					intProxyPort = int.Parse(objInstagramUser.proxyport);
				}


				NameValueCollection nameval = new NameValueCollection();
				if (!UserName.Contains("http://websta.me/n/"))
				{
					UserName = "http://websta.me/n/" + UserName + "/";
				}
				string UserPageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserName),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);


				if(!UserPageContent.ToLower().Contains("logout"))
				{

					AccountManagerInstagram objInstagramAccountManager = new AccountManagerInstagram();

					//Addtologger("Logging in with " + objInstagramUser.username);

					objInstagramAccountManager.LoginUsingGlobusHttp(ref objInstagramUser);
					UserPageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserName),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);


				}


				try
				{

					string PK = string.Empty;
					if (UserPageContent.Contains(""))
					{
						PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"follow_btn_wrapper\"", ">").Replace("data-target=", "").Replace("\"","").Trim();
					}

					if (string.IsNullOrEmpty(PK))
					{
						PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"message_user_id", ">").Replace(">", "").Replace("value=",string.Empty).Replace("\"",string.Empty).Trim();//.Replace("\"", "").Trim();
					}

					string PostData = "action=follow";//"&pk=" + PK + "&t=9208";
					string FollowedPageSource=string.Empty;

					FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri("http://websta.me/api/relationships/" + PK), PostData, UserName, "http://websta.me");
					//nameval.Add("Origin", "http://web.stagram.com");
					//nameval.Add("X-Requested-With", "XMLHttpRequest");





					if (FollowedPageSource.Contains("OK"))
					{
						//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);

						if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxSuccessFullyFollowing);
						}
						else 
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
						}
						return "Followed";
					}
					else
					{
						//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxFailedFollowing);
						}
						else 
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						}
						return "UnFollowed";
					}
				}
				catch (Exception ex)
				{
					return "Follow option is not available In page...!!";
					//Console.Write (ex.Message);
				}
			} 
			catch (Exception ex) {
				return "UnFollowed";
			}

		}



















		public string UnFollow(string UserName, ref InstagramUser objInstagramUser)
		{
			try 
			{

				int intProxyPort = 80;

				if (!string.IsNullOrEmpty(objInstagramUser.proxyport) && IdCheck.IsMatch(objInstagramUser.proxyport))
				{
					intProxyPort = int.Parse(objInstagramUser.proxyport);
				}


				NameValueCollection nameval = new NameValueCollection();
				if (!UserName.Contains("http://websta.me/n/"))
				{
					UserName = "http://websta.me/n/" + UserName + "/";
				}
				string UserPageContent = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(UserName),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);

				try
				{

					string PK = string.Empty;
					if (UserPageContent.Contains(""))
					{
						PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"follow_btn_wrapper\"", ">").Replace("data-target=", "").Replace("\"","").Trim();
					}

					if (string.IsNullOrEmpty(PK))
					{
						PK = GlobusHttpHelper.getBetween(UserPageContent, "id=\"message_user_id", ">").Replace(">", "").Replace("value=",string.Empty).Replace("\"",string.Empty).Trim();//.Replace("\"", "").Trim();
					}

					string PostData = "action=unfollow";//"&pk=" + PK + "&t=9208";
					string FollowedPageSource=string.Empty;

					FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri("http://websta.me/api/relationships/" + PK), PostData, UserName, "http://websta.me");
					//nameval.Add("Origin", "http://web.stagram.com");
					//nameval.Add("X-Requested-With", "XMLHttpRequest");

					if (FollowedPageSource.Contains("OK"))
					{
						//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);

						if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxSuccessFullyFollowing);
						}
						else 
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_SuccessFullyFollowing);
						}
						return "UnFollowed";
					}
					else
					{
						//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_LinuxFailedFollowing);
						}
						else 
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+UserName,FBGlobals.path_FailedFollowing);
						}
						return "NotUnFollowed";
					}
				}
				catch (Exception ex)
				{
					return "NOFollow option is not available In page...!!";
					//Console.Write (ex.Message);
				}
			} 
			catch (Exception ex) {
				return "NotUnFollowed";
			}

		}



		public string Comment(string commentId, string CommentMsg, ref InstagramUser objInstagramUser)
		{

			int intProxyPort = 80;

			if (!string.IsNullOrEmpty(objInstagramUser.proxyport) && IdCheck.IsMatch(objInstagramUser.proxyport))
			{
				intProxyPort = int.Parse(objInstagramUser.proxyport);
			}

			NameValueCollection namevalue = new NameValueCollection();
			string FollowedPageSource = string.Empty;
			try
			{
				string CommentIdlink = string.Empty;
				string commentIdLoggedInLink = string.Empty;
				if (!commentId.Contains("http://web.stagram.com/"))
				{
					CommentIdlink = "http://web.stagram.com/p/" + commentId + "/";

					commentIdLoggedInLink = "http://websta.me/p/" + commentId;
				}


				if(IsCommentWithSpecificFollowers)
				{
					try{
					string NOOfCountOfFollowers = string.Empty;
					string pagesourceTemp = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(commentIdLoggedInLink),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);
					string userName = GlobusHttpHelper.getBetween(pagesourceTemp,"class=\"username\">","</a>");
					if(!string.IsNullOrEmpty(userName))
					{
					  string urlTemp="http://websta.me/n/"+userName;
					  pagesourceTemp = objInstagramUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(urlTemp),objInstagramUser.proxyip,intProxyPort,objInstagramUser.proxyusername,objInstagramUser.proxypassword);
					  NOOfCountOfFollowers=GlobusHttpHelper.getBetween(pagesourceTemp,"class=\"counts_followed_by\">","</span>").Replace(",","").Trim();
						NOOfCountOfFollowers=Convert.ToString(NOOfCountOfFollowers);

						try 
							{
								if(Convert.ToInt32(NOOfCountOfFollowers)>=Convert.ToInt32(countNOOfFollowers))
								{

								}
								else
								{
									FollowedPageSource="NO of Followers unmatched";
									return FollowedPageSource;
								}
						   }
							catch (Exception ex) {
								
						}
						
					}
					else
					{

						FollowedPageSource="Unable to find No of follwers of user.";
						return FollowedPageSource;
					}
					}
					catch{}
				}


				#region Change

				string url = "http://websta.me/api/comments/" + commentId;

				bool checkunicode = ContainsUnicodeCharacter(CommentMsg);

				string CmntMSG = string.Empty;

				if (checkunicode == false)
				{
					CmntMSG = Uri.EscapeDataString(CommentMsg);
					//CmntMSG = CommentMsg.Replace(" ", "+");
				}
				else
				{
					CmntMSG = Uri.EscapeDataString(CommentMsg);
				}

				string commentPostData = "comment=+" + CmntMSG + "&media_id=" + commentId;

				FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri(url), commentPostData, CommentIdlink, "");





				if (!FollowedPageSource.Contains("status\":\"OK\""))
				{
					try
					{
						FollowedPageSource = objInstagramUser.globusHttpHelper.postFormData(new Uri(url), commentPostData, CommentIdlink, "");
						string postUrl = "https://www.instagram.com/web/comments/" + commentId + "/add/";
						string postData = "comment_text=" + CmntMSG;
						FollowedPageSource = objInstagramUser.globusHttpHelper.PostDataWithInstagram(new Uri(postUrl), postData, "https://www.instagram.com/");
					}
					catch { };


				}





				if (FollowedPageSource.Contains("status\":\"OK\"") || FollowedPageSource.Contains("created_time"))
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_SuccessfullyCommentedInstagram);
					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_SuccessfullyCommentedInstagram);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_LinuxSuccessfullyCommentedInstagram);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_SuccessfullyCommentedInstagram);
					}
					FollowedPageSource = "Successs";
				}
				else
				{
					//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_FailCommentedInstagram);
					if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_FailCommentedInstagram);
					}
					else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_LinuxFailCommentedInstagram);
					}
					else 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+CmntMSG+":"+commentId,FBGlobals.path_FailCommentedInstagram);
					}
					FollowedPageSource = "Fail";
				}
				#endregion

			
			}
			catch (Exception ex)
			{
				//GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+commentId,FBGlobals.path_FailCommentedInstagram);
				if(FBGlobals.typeOfOperatingSystem.Contains("Win")||FBGlobals.typeOfOperatingSystem.Contains("win"))
				{
					GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+commentId,FBGlobals.path_FailCommentedInstagram);
				}
				else if(FBGlobals.typeOfOperatingSystem.Contains("Unix")||FBGlobals.typeOfOperatingSystem.Contains("unix"))
				{
					GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+commentId,FBGlobals.path_LinuxFailCommentedInstagram);
				}
				else 
				{
					GlobusFileHelper.AppendStringToTextfileNewLine(objInstagramUser.username+":"+commentId,FBGlobals.path_FailCommentedInstagram);
				}
				FollowedPageSource = string.Empty;
			}
			return FollowedPageSource;
		} 
	}
}

