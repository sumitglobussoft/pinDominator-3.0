using System;
using Gtk;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PinDominator;
using System.Threading;

namespace PinDominator
{
	public partial class frmFriendManager : Gtk.Window
	{
		public frmFriendManager () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			Gdk.Color fontcolor = new Gdk.Color(255,255,255);
			label1.ModifyFg(StateType.Normal, fontcolor);
			label2.ModifyFg(StateType.Normal, fontcolor);
			label3.ModifyFg(StateType.Normal, fontcolor);
			label4.ModifyFg(StateType.Normal, fontcolor);
			label5.ModifyFg(StateType.Normal, fontcolor);


			Gdk.Color col = new Gdk.Color ();
			Gdk.Color.Parse ("#3b5998", ref col);

			ModifyBg (StateType.Normal, col);


			FriendManager.CampaignStopLogevents.addToLogger += new EventHandler (CampaignnameLog);
			txtUseSingleMessage.Visible = false;


			try
			{
				chkUseSingleItem.Label = "Use Single Keyword";
				btnFriendsLoadKeywords.Sensitive = true;
				btnLoadProfileUrlFriendManager.Sensitive = false;
				btnFridendProfileUrl.Sensitive = false;
				btn_LoadUrlsMessage.Sensitive = false;
				btnFriendsLoadPicture.Sensitive = false;			


				chkUseUploadedProfileUrls.Sensitive=true;
				txt_noOfFriendsCount.Sensitive=true;
				cmbFriendsInput.Active=0;
				chkFriendsManagerSendWithTxtMessage.Sensitive=false;
				chk_ExportDataFriendsManager.Sensitive=true;
				btnStopProcessFriendManager.Sensitive=false;

				btn_LoadSingleImage.Visible=false;
				chk_UseSingleImage.Sensitive=false;
				//chkFriendsManagerSendWithTxtMessage.Visible=false;
				chk_ExportDataFriendsManager.Visible=false;
				btn_loadFanpageUrls.Sensitive=false;


			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}
		}




		//FriendManager objFriendManager=new FriendManager();



		protected void OnBtnLoadProfileUrlFriendManagerClicked (object sender, EventArgs e)
		{
			FBUtils.CheckUploadProfileUrls = true;
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					FBUtils.LoadProfileUrls = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message

					Addtologger("Profile Urls Loaded : "+ FBUtils.LoadProfileUrls.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		public void Addtologger(string log)
		{
			try{

				//textViewLoggerFanPage.Buffer.Text += log+"\n";
				Gtk.Application.Invoke (delegate {
					txtViewFriendManager.Buffer.Text += log+"\n";
				});
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}
		}

		void CampaignnameLog(object sender, EventArgs e)
		{
			if (e is EventsArgs)
			{
				EventsArgs eArgs = e as EventsArgs;
				Addtologger(eArgs.log);
			}
		}


		protected void OnBtnStartFriendManagerClicked (object sender, EventArgs e)
		{


			if (FBGlobals.listAccounts.Count > 0)
			{
				string	StartGroupProcessUsing = string.Empty;
				try
				{
					FriendManager.StartFriendsProcessUsing = cmbFriendsInput.Entry.Text.ToString ();

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}

				bool checkSingleItemTxt=Convert.ToBoolean(chkUseSingleItem.Active);



				if (FriendManager.StartFriendsProcessUsing==" Add friends from keyword")
				{
					Addtologger ("Please wait...");



					try
					{
						if(!string.IsNullOrEmpty(txt_noOfFriendsCount.Text))
						{
							FriendManager.noOfFriendsToAdd = int.Parse(txt_noOfFriendsCount.Text);
						}
						else{
							Addtologger ("Please give No. of Friends To Add !");
							return;
						}

					}
					catch{
						Addtologger ("Please give Valid No. of Friends To Add !");
						return;


					};




					if (checkSingleItemTxt == true)
					{
						string singleKeyWords = txtUseSingleMessage.Text;
						FriendManager.lstRequestFriendsKeywords.Clear ();
						if (!string.IsNullOrEmpty (singleKeyWords)) 
						{
							FriendManager.lstRequestFriendsKeywords.Add (singleKeyWords);
						} 
						else
						{
							Addtologger ("Please Enter single Keywords !");
							return;
						}

					}

					if (FriendManager.lstRequestFriendsKeywords.Count != 0||chkUseUploadedProfileUrls.Active==true) 
					{

					} 
					else 
					{
						Addtologger ("Please Load Search keyword ..");
						return;
					}
				}
				if (FriendManager.StartFriendsProcessUsing=="Add Friends via fanpage url")
				{


					try
					{
						if(!string.IsNullOrEmpty(txt_noOfFriendsCount.Text))
						{
							FriendManager.NoofFriendToScrapToAddFriendForFanPage = int.Parse(txt_noOfFriendsCount.Text);
						}
						else{
							Addtologger ("Please give No. of Friends To Add !");
							return;
						}

					}
					catch{
						Addtologger ("Please give Valid No. of Friends To Add !");
						return;


					};








					Addtologger ("Please wait...");
					if (checkSingleItemTxt == true)
					{
						string singleKeyWords = txtUseSingleMessage.Text;
						FBUtils.LoadFanpageUrls.Clear ();
						if (!string.IsNullOrEmpty (singleKeyWords)) 
						{
							FBUtils.LoadFanpageUrls.Add (singleKeyWords);
						} 
						else
						{
							Addtologger ("Please Enter single fanpage url !");
							return;
						}

					}
				}


				if (FriendManager.StartFriendsProcessUsing=="Send Text message on Friends wall")
				{
					Addtologger ("Please Wait Send Text message on Friends wall Process started ...");

					if (checkSingleItemTxt == true)
					{
						string singleMessage = txtUseSingleMessage.Text;
						objFriendManager.lstWallMessageWallPoster.Clear ();
						if (!string.IsNullOrEmpty (singleMessage)) 
						{
							objFriendManager.lstWallMessageWallPoster.Add (singleMessage);
						} 
						else
						{
							Addtologger ("Please Enter Text Message in Text box !");
							return;
						}
						chkFriendsManagerSendWithTxtMessage.Active=true;

					}


					if (objFriendManager.lstWallMessageWallPoster.Count != 0) 
					{

					} 
					else 
					{
						Addtologger ("Please Load Text Message ..");
						return;
					}
				}
				if (FriendManager.StartFriendsProcessUsing=="Send Url message on Friends wall")
				{
					Addtologger ("Please Wait Send Url message on Friends wall Process started ...");

					if (checkSingleItemTxt == true)
					{
						string singleUrlMessage = txtUseSingleMessage.Text;
						objFriendManager.lstWallMessageWallPoster.Clear ();
						if (!string.IsNullOrEmpty (singleUrlMessage)) 
						{
							objFriendManager.lstWallMessageWallPoster.Add (singleUrlMessage);
						} 
						else
						{
							Addtologger ("Please Enter Url Message in Text box !");
							return;
						}

					}



					if (objFriendManager.lstWallPostURLsWallPoster.Count != 0) 
					{

					} 
					else 
					{
						Addtologger ("Please Load Video Url ..");
						return;
					}
				}
				if (FriendManager.StartFriendsProcessUsing=="Send Picture on Friends wall")
				{
					Addtologger ("Please Wait Send Picture on Friends wall Process started ...");

					if (objFriendManager.lstPicturecollectionPostPicOnWall.Count!= 0) 
					{

					} 
					else 
					{
						Addtologger ("Please Load Picture ..");
						return;
					}


					bool checkBool=Convert.ToBoolean(chkUseSingleItem.Active&&chkFriendsManagerSendWithTxtMessage.Active);
					if (checkBool)
					{
						objFriendManager.lstWallMessageWallPoster.Clear ();
						string singleMessage = txtUseSingleMessage.Text;
						if (string.IsNullOrEmpty (singleMessage))
						{
							Addtologger ("Please Load Text Message ..");
						} else 
						{
							objFriendManager.lstWallMessageWallPoster.Add (singleMessage);
						}

					}
				}


				try
				{
					FriendManager.minDelayPostPicOnWal = Convert.ToInt32 (txtDelayFromFriendManager.Text);
					FriendManager.maxDelayPostPicOnWal=Convert.ToInt32 (txtDelayToFriendManager.Text);
					FriendManager.minDelayFriendManager = Convert.ToInt32 (txtDelayFromFriendManager.Text);
					FriendManager.maxDelayFriendManager = Convert.ToInt32 (txtDelayToFriendManager.Text);
					FriendManager.minDelayWallPoster= Convert.ToInt32 (txtDelayFromFriendManager.Text);
					FriendManager.maxDelayWallPoster = Convert.ToInt32 (txtDelayToFriendManager.Text);
				} 
				catch (Exception ex)
				{
					Console.WriteLine ("Error : " + ex.StackTrace);
				}
				try
				{
				FriendManager.NoOfFriendRequestFriendManager = Convert.ToInt32 (txt_noOfFriendsCount.Text);
					FriendManager.NumberOfFriendsSendPicOnWall=FriendManager.NoOfFriendRequestFriendManager;
				}
				catch (Exception ex)
				{
					Console.WriteLine ("Error : " + ex.StackTrace);
				}
				FriendManager.NoOfFriendsRequestParKeyWord = 10;
				FriendManager.NoOfFriendsRequest = FriendManager.NoOfFriendRequestFriendManager;
				int noOfFriendsRequest = 10;
				try
				{
				objFriendManager.NoOfFriendsWallPoster= Convert.ToInt32 (txt_noOfFriendsCount.Text);
				}
				catch (Exception ex)
				{
					Console.WriteLine ("Error : " + ex.StackTrace);
				}
				FriendManager.NoOfThreads = 25;


				if (FriendManager.StartFriendsProcessUsing=="Send Text message on Friends wall")
				{

					bool checkBool=Convert.ToBoolean(chkUseSingleItem.Active);
					if (checkBool)
					{
						objFriendManager.lstWallMessageWallPoster.Clear ();
						string singleMessage = txtUseSingleMessage.Text;
						if (string.IsNullOrEmpty (singleMessage))
						{
							Addtologger ("Please Load Text Message ..");
						}
						else 
						{
							objFriendManager.lstWallMessageWallPoster.Add (singleMessage);
							                 
						}
					}
					else 
					{
						if (objFriendManager.lstWallMessageWallPoster.Count != 0) 
						{

						} 
						else 
						{
							Addtologger ("Please Load Text Message ..");
							return;
						}
					}
				}



				Thread ObjNew = new Thread (objFriendManager.StartSendFriendRequest);
				ObjNew.Start ();
				btnStopProcessFriendManager.Sensitive=true;
			}
			else
			{
				Addtologger("Please Load Accounts !");

			}
		}

		protected void OnBtnStopProcessFriendManagerClicked (object sender, EventArgs e)
		{
			try
			{
				Thread threadStopAccountCreation = new Thread(StopFriendManager);
				threadStopAccountCreation.Start();
				//StopAccountCreation();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}


		private void StopFriendManager()
		{
			try
			{
				objFriendManager.isRequestFriendsStop=true;
				objFriendManager.isStopWallPoster = true;

				List<Thread> lstTemp = new List<Thread>();
				lstTemp = objFriendManager.lstThreadsWallPoster.Distinct().ToList();

				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
						objFriendManager.lstThreadsWallPoster.Remove(item);
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

			Addtologger("Process Stopped !");

		}



		protected void OnBtnFridendProfileUrlClicked (object sender, EventArgs e)
		{
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objFriendManager.lstWallMessageWallPoster = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
					Addtologger("Message Loaded : "+objFriendManager.lstWallMessageWallPoster.Count);
					//objFriendManager.lstWallMessageWallPoster=objFriendManager.lstWallMessageWallPoster;
				}
				chkFriendsManagerSendWithTxtMessage.Active=true;
				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnBtnFriendsLoadKeywordsClicked (object sender, EventArgs e)
		{
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					FriendManager.lstRequestFriendsKeywords = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
					Addtologger("Search Keyword Loaded : "+FriendManager.lstRequestFriendsKeywords .Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnBtnFriendsLoadPictureClicked (object sender, EventArgs e)
		{
			try
			{
				List<string> lstFanPagePics = new List<string>();
				List<string> lstCorrectFanPagePics = new List<string>();

				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					string inputUrl = fileChooser.Filename.ToString();
					string[] picsArray = Directory.GetFiles(inputUrl);
					lstFanPagePics = picsArray.Distinct().ToList();
					string PicFilepath = inputUrl;
					foreach (string item in lstFanPagePics)
					{
						try
						{
							string items = item.ToLower();
							if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
							{
								lstCorrectFanPagePics.Add(item);
							}
							else
							{
								//Addtologger("Wrong File Is :" + item);

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}


					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objFriendManager.lstPicturecollectionPostPicOnWall = lstCorrectFanPagePics;// upload correct Message
					Addtologger(objFriendManager.lstPicturecollectionPostPicOnWall.Count + "  Pics loaded");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnBtnFriendsLoadUrlMessageClicked (object sender, EventArgs e)
		{
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objFriendManager.lstWallPostURLsWallPoster = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
					Addtologger("Urls Loaded : "+objFriendManager.lstWallPostURLsWallPoster.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnChkExportDataFriendsManagerClicked (object sender, EventArgs e)
		{
			try
			{
				bool checkBool=Convert.ToBoolean(chk_ExportDataFriendsManager.Active);
				if (!checkBool)
				{
					PageManager.ScrapersExprotFilePath=string.Empty;
					return;
				}

				List<string> lstFanPagePics = new List<string>();
				List<string> lstCorrectFanPagePics = new List<string>();

				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					string inputUrl = fileChooser.Filename.ToString();
					string[] picsArray = Directory.GetFiles(inputUrl);
					lstFanPagePics = picsArray.Distinct().ToList();
					string PicFilepath = inputUrl;


					if (!string.IsNullOrEmpty(PicFilepath))
					{

						if(FBGlobals.typeOfOperatingSystem.Contains("Win"))
						{
							PicFilepath = PicFilepath + "\\FriendsManagerData.csv";
							Addtologger("Export Data File Path :" + PicFilepath);
						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix"))
						{
							PicFilepath = PicFilepath + "/FriendsManagerData.csv";
							Addtologger("Export Data File Path :" + PicFilepath);
						}
						else
						{
							PicFilepath = PicFilepath + "\\FriendsManagerData.csv";
							Addtologger("Export Data File Path :" + PicFilepath);
						}


						PageManager.ScrapersExprotFilePath = PicFilepath;
					}
					else
					{

						Addtologger("Please Select Export Data File Path .!");
					}
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnBtnLoadUrlsMessageClicked (object sender, EventArgs e)
		{
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objFriendManager.lstWallPostURLsWallPoster = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
					Addtologger("Urls Loaded : "+objFriendManager.lstWallPostURLsWallPoster.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}
		public bool checkFrndRqstViaFanpageUrl=false;
		protected void OnChkUseSingleItemClicked (object sender, EventArgs e)
		{
			
			bool checkBoolScraper11=Convert.ToBoolean(rbk_loadFanpageUrls.Active);

			if (checkBoolScraper11)
			{
				checkFrndRqstViaFanpageUrl = true;						

			}



			bool checkBoolScraper=Convert.ToBoolean(rbk_AddFriendsViaScraper.Active);
			if (checkBoolScraper)
			{
				bool checkBool = Convert.ToBoolean (chkUseSingleItem.Active);
				if (checkBool)
				{
					txtUseSingleMessage.Visible = true;
					btnFriendsLoadKeywords.Sensitive = false;
					bool checkBoolScraper1=Convert.ToBoolean(rbk_loadFanpageUrls.Active);

					if (checkBoolScraper1)
					{
						btn_loadFanpageUrls.Sensitive = false;
					}

				}
				else
				{
					txtUseSingleMessage.Visible = false;
					btnFriendsLoadKeywords.Sensitive = true;

				}
			} 
			else
			{

				bool checkBool = Convert.ToBoolean (chkUseSingleItem.Active);
				if (checkBool)
				{
					bool checkBoolScraper1=Convert.ToBoolean(rbk_loadFanpageUrls.Active);

					if (checkBoolScraper1)
					{
						btn_loadFanpageUrls.Sensitive = false;
					}

					txtUseSingleMessage.Visible = true;
					btnFridendProfileUrl.Sensitive = false;

				} else {
					txtUseSingleMessage.Visible = false;
					bool checkBoolScraper1=Convert.ToBoolean(rbk_loadFanpageUrls.Active);

					if (checkBoolScraper1) {
						btn_loadFanpageUrls.Sensitive = true;
					} else
					{

						btnFridendProfileUrl.Sensitive = true;
					}

				}
			}
		}

		protected void OnChkUseUploadedProfileUrlsClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(chkUseUploadedProfileUrls.Active);
			if (checkBool) 
			{
				bool checkBoolAddFriends=Convert.ToBoolean(rbk_AddFriendsViaScraper.Active);


				if (checkBoolAddFriends) 
				{
					FriendManager.checkRequestWithProfileUrl = true;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFriendsLoadKeywords.Sensitive = false;
					chkUseSingleItem.Sensitive = false;
				}
			} 
			else 
			{
				btnFriendsLoadKeywords.Sensitive = true;
				FBUtils.CheckUploadProfileUrls = false;
				FBUtils.LoadProfileUrls.Clear();
				chkUseUploadedProfileUrls.Sensitive = true;
				btnLoadProfileUrlFriendManager.Sensitive = false;
				chkUseSingleItem.Sensitive = true;

			}
		}

		protected void OnChkFriendsManagerSendWithTxtMessageClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(rbk_SendUrlmessageonFriendswall.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{


				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkAddFriendsViaScraperClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Keyword";
			bool checkBool=Convert.ToBoolean(rbk_AddFriendsViaScraper.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = true;
					btnLoadProfileUrlFriendManager.Sensitive = false;
					btnFridendProfileUrl.Sensitive = false;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			


					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=0;
					chkFriendsManagerSendWithTxtMessage.Sensitive=false;
					chk_ExportDataFriendsManager.Sensitive=true;
					btn_loadFanpageUrls.Sensitive=false;


					chk_ExportDataFriendsManager.Visible=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;
					btnLoadProfileUrlFriendManager.Sensitive = false;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkSendUrlmessageonFriendswallClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Enter Single Message";
			bool checkBool=Convert.ToBoolean(rbk_SendUrlmessageonFriendswall.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = true;
					btnFriendsLoadPicture.Sensitive = false;			
					btn_loadFanpageUrls.Sensitive=false;

					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=1;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;
					chkFriendsManagerSendWithTxtMessage.Active=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}

		protected void OnRadiobutton9Clicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Enter Single Message";
			bool checkBool=Convert.ToBoolean(rbkSendTextMessage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{


					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			
					btn_loadFanpageUrls.Sensitive=false;
				
					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=2;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;


				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkSendPicturemessageonFriendswallClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Enter Single Message";

			bool checkBool=Convert.ToBoolean(rbk_SendPicturemessageonFriendswall.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = true;			
					btn_loadFanpageUrls.Sensitive=false;

					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=3;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;


					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=true;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnBtnLoadSingleImageClicked (object sender, EventArgs e)
		{
			try
			{
				try
				{
				objFriendManager.lstPicturecollectionPostPicOnWall.Clear();
				}
				catch{};



				List<string> lstFanPagePics = new List<string>();
				List<string> lstCorrectFanPagePics = new List<string>();

				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					string inputUrl = fileChooser.Filename.ToString();
				//string[] picsArray = Directory.GetFiles(inputUrl);
					lstFanPagePics.Add(inputUrl);// = picsArray.Distinct().ToList();
					string PicFilepath = inputUrl;
					foreach (string item in lstFanPagePics)
					{
						try
						{
							string items = item.ToLower();
							if (items.Contains(".jpg") || items.Contains(".png") || items.Contains(".jpeg") || items.Contains(".gif"))
							{
								lstCorrectFanPagePics.Add(item);
							}
							else
							{
								Addtologger("Wrong File Is :" + item);

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}


					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objFriendManager.lstPicturecollectionPostPicOnWall = lstCorrectFanPagePics;// upload correct Message
					Addtologger(objFriendManager.lstPicturecollectionPostPicOnWall.Count + "  Pics loaded");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnChkUseSingleImageClicked (object sender, EventArgs e)
		{

			bool checkBool=Convert.ToBoolean(chk_UseSingleImage.Active);
			if (checkBool) 
			{
				btn_LoadSingleImage.Visible = true;
				btnFriendsLoadPicture.Sensitive = false;
			} 
			else 
			{
				btn_LoadSingleImage.Visible = false;
				btnFriendsLoadPicture.Sensitive=true;
			}
		}

		protected void OnBtnLoadFanpageUrlsClicked (object sender, EventArgs e)
		{
			FBUtils.CheckUploadProfileUrls = true;
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					FBUtils.LoadFanpageUrls = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message

					Addtologger("Profile Urls Loaded : "+ FBUtils.LoadFanpageUrls.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnRbkLoadFanpageUrlsClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single fanpage url";
			bool checkBool = Convert.ToBoolean (rbk_loadFanpageUrls.Active);
			if (!checkBool) {
				return;

			} else {
				try {
					btn_loadFanpageUrls.Sensitive = true;
					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = false;
					btnFridendProfileUrl.Sensitive = false;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			


					chkUseUploadedProfileUrls.Sensitive = false;
					txt_noOfFriendsCount.Sensitive = true;
					cmbFriendsInput.Active = 4;
					chkFriendsManagerSendWithTxtMessage.Sensitive = false;
					chk_ExportDataFriendsManager.Sensitive = false;



					chk_ExportDataFriendsManager.Visible = false;

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive = false;

				} catch (Exception ex) {
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnAddFriendsViaScraperActionActivated (object sender, EventArgs e)
		{
			rbk_AddFriendsViaScraper.Active = true;
			chkUseSingleItem.Label = "Use Single Keyword";
			bool checkBool=Convert.ToBoolean(rbk_AddFriendsViaScraper.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = true;
					btnLoadProfileUrlFriendManager.Sensitive = false;
					btnFridendProfileUrl.Sensitive = false;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			


					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=0;
					chkFriendsManagerSendWithTxtMessage.Sensitive=false;
					chk_ExportDataFriendsManager.Sensitive=true;
					btn_loadFanpageUrls.Sensitive=false;


					chk_ExportDataFriendsManager.Visible=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;
					btnLoadProfileUrlFriendManager.Sensitive = false;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnAddFriendsViaFanpageUrlsActionActivated (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single fanpage url";
			bool checkBool = Convert.ToBoolean (rbk_loadFanpageUrls.Active);
			if (!checkBool) {
				return;

			} else {
				try {
					btn_loadFanpageUrls.Sensitive = true;
					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = false;
					btnFridendProfileUrl.Sensitive = false;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			


					chkUseUploadedProfileUrls.Sensitive = false;
					txt_noOfFriendsCount.Sensitive = true;
					cmbFriendsInput.Active = 4;
					chkFriendsManagerSendWithTxtMessage.Sensitive = false;
					chk_ExportDataFriendsManager.Sensitive = false;



					chk_ExportDataFriendsManager.Visible = false;

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive = false;

				} catch (Exception ex) {
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnPostTextMessageActionActivated (object sender, EventArgs e)
		{
			rbkSendTextMessage.Active = true;
			chkUseSingleItem.Label = "Enter Single Message";
			bool checkBool=Convert.ToBoolean(rbkSendTextMessage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{


					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = false;			
					btn_loadFanpageUrls.Sensitive=false;

					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=2;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;


				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnPostVideoActionActivated (object sender, EventArgs e)
		{
			rbk_SendUrlmessageonFriendswall.Active = true;
			chkUseSingleItem.Label = "Enter Single Message";
			bool checkBool=Convert.ToBoolean(rbk_SendUrlmessageonFriendswall.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = true;
					btnFriendsLoadPicture.Sensitive = false;			
					btn_loadFanpageUrls.Sensitive=false;

					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=1;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;
					chkFriendsManagerSendWithTxtMessage.Active=false;

					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=false;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}
		protected void OnPostPictureActionActivated (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Enter Single Message";
			rbk_SendPicturemessageonFriendswall.Active = true;
			bool checkBool=Convert.ToBoolean(rbk_SendPicturemessageonFriendswall.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{

					btnFriendsLoadKeywords.Sensitive = false;
					btnLoadProfileUrlFriendManager.Sensitive = true;
					btnFridendProfileUrl.Sensitive = true;
					btn_LoadUrlsMessage.Sensitive = false;
					btnFriendsLoadPicture.Sensitive = true;			
					btn_loadFanpageUrls.Sensitive=false;

					chkUseUploadedProfileUrls.Sensitive=true;
					txt_noOfFriendsCount.Sensitive=true;
					cmbFriendsInput.Active=3;
					chkFriendsManagerSendWithTxtMessage.Sensitive=true;
					chk_ExportDataFriendsManager.Sensitive=false;


					btn_LoadSingleImage.Visible=false;
					chk_UseSingleImage.Sensitive=true;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
	}
}

