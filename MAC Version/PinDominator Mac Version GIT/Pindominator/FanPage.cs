using System;
using Gtk;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PinDominator;
using Pango;

namespace PinDominator
{
	public partial class FanPage : Gtk.Window
	{

		public FanPage () :
		base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			Gdk.Color fontcolor = new Gdk.Color(255,255,255);
			label1.ModifyFg(StateType.Normal, fontcolor);
			label2.ModifyFg(StateType.Normal, fontcolor);
			label6.ModifyFg(StateType.Normal, fontcolor);
			label4.ModifyFg(StateType.Normal, fontcolor);
			label5.ModifyFg(StateType.Normal, fontcolor);

			Gdk.Color col = new Gdk.Color ();
			Gdk.Color.Parse ("#3b5998", ref col);

			ModifyBg (StateType.Normal, col);

		
		//	PageManager.CampaignStopLogevents.addToLogger += new EventHandler (CampaignnameLog);
			BackGroundColorChangeMenuBar ();
			TxtUseSingleItem.Visible = false;

			try
			{
				btnUploadKeyword.Sensitive=true;
				btnUploadMessageFanPage.Sensitive = false;
				btnUploadPicsFanPageManager.Sensitive=false;
				btnUploadUrlFanPage.Sensitive=false;
				btn_FanPageLoadUrlsMassege.Sensitive=true;
				chkExportDataPageManager.Sensitive=false;
				Txt_NoofPostPerURLPageManager.Sensitive=false;
				cmbSelectInputFanPage.Active=0;
				btn_FanPageLoadUrlsMassege.Sensitive=false;
				chkFanPageManagerSendPicWithMessage.Sensitive=false;
				chkExportDataPageManager.Sensitive=true;
				rbk_fanPageScraperByKeyword.Active=true;
				btnStopProcess.Sensitive=false;

				btn_LoadSingleImage.Visible = false;
				chk_UseSingleImage.Sensitive=false;
				//chkFanPageManagerSendPicWithMessage.Visible = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}

		}


		DrawingArea da;
		DrawingArea da1;

		public void BackGroundColorChangeMenuBar()
		{
			try {

				da1 = new DrawingArea ();
				da1.ExposeEvent += OnExposed1;

				Gdk.Color col = new Gdk.Color ();
				Gdk.Color.Parse ("#3b5998", ref col);

				ModifyBg (StateType.Normal, col);
				da1.ModifyBg (StateType.Normal, col);



			} catch (Exception ex) {
				Console.Write (ex.Message);
			}
		}


		void OnExposed1 (object o, ExposeEventArgs args) 
		{
			da1.GdkWindow.DrawLine(da.Style.BaseGC(StateType.Normal), 0, 0, 400, 300);

		}
			


		Entry txtDBCUserName = new Entry ();
		Entry txtDBCPassword = new Entry ();
		Window myWin;
		//PageManager ObjFan=new PageManager();
		protected void OnBtnUploadMessageFanPageClicked (object sender, EventArgs e)
		{
			try
			{
			FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fileChooser.Run () == (int)ResponseType.Accept) 
			{
				//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
				//GroupManager.LstOfGroupKeywords = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
				ObjFan.lstFanPageUrlCollectionFanPagePostUrl= GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ());
				PageManager.lstFanPageCollectionFanPagePosterMessage = ObjFan.lstFanPageUrlCollectionFanPagePostUrl;
				       
				Addtologger("Message Loaded : "+PageManager.lstFanPageCollectionFanPagePosterMessage.Count);
			}
				chkFanPageManagerSendPicWithMessage.Active=true;
			fileChooser.Destroy ();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}


			/*try
			{
				Application.Init();

				myWin = new Window("DBC Settings");
				myWin.Resize(400,200);
				myWin.SetPosition (WindowPosition.CenterAlways);
				myWin.Resizable = false;
				Fixed myfixed = new Fixed();
				myfixed.WidthRequest = 400;
				myfixed.HeightRequest = 200;
				//Create a label and put some text in it.     
				Label lblUserName = new Label();
				lblUserName.Text = "DBC UserName: ";

				Label lblPassword = new Label();
				lblPassword.Text = "DBC Password: ";

				txtDBCUserName = new Entry ();
				txtDBCPassword = new Entry ();
				Button btnSave = new Button ("save");
				btnSave.SetSizeRequest (100, 30);

				Button btnClear = new Button ("Clear");
				btnClear.SetSizeRequest (100, 30);

				FontDescription font = new Pango.FontDescription();
				font.Size = 64;

				font.Family = "Courier";
				font.Weight=Weight.Bold;
				lblPassword.ModifyFont(font);
				rbk_downloadFanPagePictures.ModifyFont(font);

				myfixed.Put (txtDBCUserName,170,35);
				myfixed.Put (lblUserName,20,35);
				myfixed.Put (lblPassword,24,80);
				myfixed.Put (txtDBCPassword,170,80);
				myfixed.Put (btnSave,100,120);
				myfixed.Put (btnClear,210,120);
				myWin.Add(myfixed);
				myWin.DeleteEvent += delegate { Application.Quit(); };
				//btnSave.Clicked+=onClickSave;
				//btnClear.Clicked+=onClickClear;
				//LoadDBC();

				myWin.ShowAll();
				Application.Run();   

			}
			catch{
			}*/
		}



		public void Addtologger(string log)
		{
			try{
				 
				//textViewLoggerFanPage.Buffer.Text += log+"\n";
				Gtk.Application.Invoke (delegate {
					textViewLoggerFanPage.Buffer.Text += log+"\n";
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
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
		//socialautobot.PageManager ObjPageManager=new socialautobot.PageManager();


		protected void OnBtnUploadKeywordClicked (object sender, EventArgs e)
		{
			FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fileChooser.Run () == (int)ResponseType.Accept) 
			{
				//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
				PageManager.lstFanPageKeyWords = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
				Addtologger("Fan page KeyWords Loaded : " + PageManager.lstFanPageKeyWords.Count);
			}

			fileChooser.Destroy ();
		}

		protected void OnBtnUploadUrlClicked (object sender, EventArgs e)
		{
			FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fileChooser.Run () == (int)ResponseType.Accept) 
			{
				//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
				PageManager.lstFanPagePostURLs = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
				PageManager.lstFanPageUrlsFanPageLiker = PageManager.lstFanPagePostURLs;
				ObjFan.lstFanPageUrlCollectionFanPagePoster = PageManager.lstFanPageUrlsFanPageLiker;

				ObjFan.LstDownloadPhotoURLsDownloadPhoto=PageManager.lstFanPageUrlsFanPageLiker;
				Addtologger("Fan page Urls Loaded : " + PageManager.lstFanPagePostURLs.Count);
			}

			fileChooser.Destroy ();
		}

		protected void OnBtnUploadPicsFanPageManagerClicked (object sender, EventArgs e)
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
								Addtologger("Wrong File Is :" + item);

							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error : " + ex.StackTrace);
						}
					}


					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					ObjFan.lstPicturecollectionPostPicOnFanPageWall = lstCorrectFanPagePics;// upload correct Message
					Addtologger(lstCorrectFanPagePics.Count + "  Pics loaded");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}

		protected void OnBtnStartProcessClicked (object sender, EventArgs e)
		{
			string comboSelectionData = string.Empty;

			ObjFan.isStopFanPageLiker = false;
			try
			{
				PageManager.FanPageProcessUsing = cmbSelectInputFanPage.Entry.Text.ToString();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

			if (string.IsNullOrEmpty(PageManager.FanPageProcessUsing))
			{
				Addtologger("Please select Start Process Using drop down list.");
				return;
			}

			bool CheckSingleItem=Convert.ToBoolean(chkUseSingleItem.Active);

			if (PageManager.FanPageProcessUsing=="Fan Page Scraper By Keyword")
			{
				if (string.IsNullOrEmpty(PageManager.ScrapersExprotFilePath))
				{
					Addtologger ("Please select data export location path..!");
					return;
				}



				try
				{
					if(!string.IsNullOrEmpty(Txt_NoofPostPerURLPageManager.Text))
					{
						PageManager.noOfPageToScrap = int.Parse(Txt_NoofPostPerURLPageManager.Text);
					}
					else
					{
						Addtologger ("Please Fill No of count to scrap ");
						return;
					}

				}
				catch{
					Addtologger ("Please Fill Valid No of count to scrap ");
					return;

				};










				if (CheckSingleItem == true) 
				{

					PageManager.lstFanPageKeyWords.Clear ();
					string singleKeyWords = TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleKeyWords))
					{
						Addtologger ("Please Enter single Keywords !");
						return;
					}
					else
					{
						PageManager.lstFanPageKeyWords.Add (singleKeyWords);

					}
				} 
				else
				{
					if (PageManager.lstFanPageKeyWords.Count<1)
					{
						Addtologger ("Please Enter Keywords !");
						return;
					}

				}

			}
			if (PageManager.FanPageProcessUsing=="Fan Page Liker")
			{
				if (CheckSingleItem == true) 
				{
					PageManager.lstFanPageUrlsFanPageLiker.Clear ();
					string singleUrls = TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleUrls)) 
					{
						Addtologger ("Please Enter single fanpage Url !");
						return;
					} 
					else
					{
						PageManager.lstFanPageUrlsFanPageLiker.Add (singleUrls);
					}
				}

			}
			if (PageManager.FanPageProcessUsing=="Fan Page Post Simple Message")
			{
				if (CheckSingleItem == true) 
				{
					PageManager.lstFanPageCollectionFanPagePosterMessage.Clear ();

					string singleMessage= TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleMessage)) 
					{
						Addtologger ("Please Enter single Message  !");
						return;
					} 
					else
					{

						PageManager.lstFanPageCollectionFanPagePosterMessage.Add (singleMessage);
						chkFanPageManagerSendPicWithMessage.Active=true;
					}
					if ( PageManager.lstFanPagePostURLs.Count<1)
					{
						Addtologger ("Please Load fanpage Url !");
						return;
					}
				}

			}
			if (PageManager.FanPageProcessUsing=="Fan Page Post Pictures")
			{
				if (CheckSingleItem == true) {
					ObjFan.lstFanPageUrlCollectionFanPagePoster.Clear ();

					string singleFanPageUrl = TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleFanPageUrl))
					{
						Addtologger ("Please Enter single Fanpage Url  !");
						return;
					}
					else
					{				
							ObjFan.lstFanPageUrlCollectionFanPagePoster.Add (singleFanPageUrl);

					}
				}
				else
				{
					if (ObjFan.lstFanPageUrlCollectionFanPagePoster.Count<1||ObjFan.lstFanPageUrlCollectionFanPagePoster==null)
					{
						Addtologger ("Please Load  Fanpage Urls  !");
						return;
					}
				}
			}
			if (PageManager.FanPageProcessUsing=="Fan Page Post Video")
			{
				if (CheckSingleItem == true) 
				{
					ObjFan.lstFanPageUrlCollectionFanPagePostUrl.Clear ();
					PageManager.lstFanPageCollectionFanPagePosterMessage.Clear();

					string singleUrlMessage= TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleUrlMessage)) 
					{
						Addtologger ("Please Enter single Url Message  !");
						return;
					} 
					else
					{
						ObjFan.lstFanPageUrlCollectionFanPagePostUrl.Add (singleUrlMessage);
					}
				}

			}
			if(PageManager.FanPageProcessUsing=="Download FanPage Pictures")
			{

				try
				{
					if(!string.IsNullOrEmpty(Txt_NoofPostPerURLPageManager.Text))
					{
						PageManager.noOfPicDownload = int.Parse(Txt_NoofPostPerURLPageManager.Text);
					}
					else
					{
						Addtologger ("Please Fill No of count to scrap ");
						return;
					}

				}
				catch{
					Addtologger ("Please Fill Valid No of count to scrap ");
					return;

				};



				if (CheckSingleItem == true) {
					ObjFan.LstDownloadPhotoURLsDownloadPhoto.Clear ();

					string singleFanPageUrl = TxtUseSingleItem.Text;

					if (string.IsNullOrEmpty (singleFanPageUrl)) {
						Addtologger ("Please Enter single Fanpage Url  !");
						return;
					} else {
						if (!singleFanPageUrl.Contains ("/photos_stream")) {
							Addtologger ("Please Enter Valid  Fanpage Url  Ex-> https://www.facebook.com/SachinTendulkar/photos_stream");
							return;
						} else {
							ObjFan.LstDownloadPhotoURLsDownloadPhoto.Add (singleFanPageUrl);
						}
					}
				}
				else
				{
					if (ObjFan.LstDownloadPhotoURLsDownloadPhoto.Count<1||ObjFan.LstDownloadPhotoURLsDownloadPhoto==null)
					{
						Addtologger ("Please Load  Fanpage Urls  !");
						return;
					}
				}
			}

			try
			{
				PageManager.noOfPicsPerURL=Convert.ToInt32(Txt_NoofPostPerURLPageManager.Text);
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}

			try
			{

				PageManager.minDelayFanPagePoster = Convert.ToInt32(txtDelayFrom.Text);
				PageManager.maxDelayFanPagePoster = Convert.ToInt32(txtDelayTo.Text);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}

			//start Thread 


			Thread createProfileThread = new Thread(ObjFan.StartLikePage);
			createProfileThread.Start();
			btnStopProcess.Sensitive=true;



		}

		protected void OnChkExportDataPageManagerClicked (object sender, EventArgs e)
		{
			try
			{
				bool checkBool=Convert.ToBoolean(chkExportDataPageManager.Active);
				if (!checkBool)
				{
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

							ObjFan.ExportPhotosPath=PicFilepath;
							if(!rbk_downloadFanPagePictures.Active)
							{
							
								PicFilepath = PicFilepath + "\\FanPageScrapedData.csv";
							}
							Addtologger("Export Data File Path :" + PicFilepath);

						}
						else if(FBGlobals.typeOfOperatingSystem.Contains("Unix"))
						{

							ObjFan.ExportPhotosPath=PicFilepath;
							if(!rbk_downloadFanPagePictures.Active)
							{
							PicFilepath = PicFilepath + "/FanPageScrapedData.csv";
							}
							Addtologger("Export Data File Path :" + PicFilepath);
						}
						else
						{

							ObjFan.ExportPhotosPath=PicFilepath;
							if(!rbk_downloadFanPagePictures.Active)
							{
							PicFilepath = PicFilepath + "\\FanPageScrapedData.csv";
							}
							Addtologger("Export Data File Path :" + PicFilepath);

						}

						//Addtologger("Export Data File Path :" + PicFilepath);
						//ObjFan.ExportPhotosPath=PicFilepath;
						//PicFilepath = PicFilepath + "\\FanPageScrapedData.csv";
					

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

		protected void OnBtnStopProcessClicked (object sender, EventArgs e)
		{
			try
			{
				Thread threadStopAccountCreation = new Thread(StopPageManager);
				threadStopAccountCreation.Start();
				//StopAccountCreation();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		private void StopPageManager()
		{
			try {
				ObjFan.isStopFanPageLiker = true;

				List<Thread> lstTemp = new List<Thread> ();
				lstTemp = ObjFan.lstThreadsFanPagePoster.Distinct ().ToList ();

				foreach (Thread item in lstTemp) {
					try {
						item.Abort ();
						ObjFan.lstThreadsFanPagePoster.Remove (item);
					} catch (Exception ex) {
						Console.WriteLine ("Error : " + ex.StackTrace);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine ("Error : " + ex.StackTrace);
			}

			Addtologger ("Process Stopped !");
		}

		protected void OnBtnFanPageLoadUrlsMassegeClicked (object sender, EventArgs e)
		{
			try
			{
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					//GroupManager.LstOfGroupKeywords = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ()); // upload correct Message
					ObjFan.lstFanPageUrlCollectionFanPagePostUrl= GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ());


					Addtologger("Urls Loaded : "+ObjFan.lstFanPageUrlCollectionFanPagePostUrl.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}
		}

		protected void OnChkUseSingleItemClicked (object sender, EventArgs e)
		{
			
			bool checkBoolKey=Convert.ToBoolean(rbk_fanPageScraperByKeyword.Active);
			if (checkBoolKey)
			{
				bool checkBool = Convert.ToBoolean (chkUseSingleItem.Active);
				if (checkBool) {
					TxtUseSingleItem.Visible = true;
					btnUploadKeyword.Sensitive = false;
				} else {
					TxtUseSingleItem.Visible = false;
					btnUploadKeyword.Sensitive = true;


				}
			} else
			{
				
				bool checkBoolTextMessage = Convert.ToBoolean (rbk_postSimpleTextMessage.Active);
				if (checkBoolTextMessage)
				{
					bool checkBool = Convert.ToBoolean (chkUseSingleItem.Active);
					if (checkBool)
					{
						TxtUseSingleItem.Visible = true;
						btnUploadMessageFanPage.Sensitive = false;
					} 
					else
					{
						TxtUseSingleItem.Visible = false;
						btnUploadMessageFanPage.Sensitive = true;


					}
				} else {	



					bool checkBool = Convert.ToBoolean (chkUseSingleItem.Active);
					if (checkBool) {
						TxtUseSingleItem.Visible = true;
						btnUploadUrlFanPage.Sensitive = false;
					} else {
						TxtUseSingleItem.Visible = false;
						btnUploadUrlFanPage.Sensitive = true;


					}
				}
			}
		}

		protected void OnRbkFanPageScraperByKeywordClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Keyword";
			bool checkBool=Convert.ToBoolean(rbk_fanPageScraperByKeyword.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="          Scrape Count : ";
					btnUploadKeyword.Sensitive=true;
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=false;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=0;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="10";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkFanPageLikerClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_fanPageLiker.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=1;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkPostSimpleTextMessageClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Message";
			bool checkBool=Convert.ToBoolean(rbk_postSimpleTextMessage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=2;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}

		protected void OnRbkPostPicturesOnFanpageClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_PostPicturesOnFanpage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadPicsFanPageManager.Sensitive=true;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=3;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";


					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=true;
				
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkPostVideoUrlsOnFanpagesClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single video Url";
			bool checkBool=Convert.ToBoolean(rbk_PostVideoUrlsOnFanpages.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadUrlFanPage.Sensitive=true;

					btnUploadPicsFanPageManager.Sensitive=false;				
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=4;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkDownloadFanPagePicturesClicked (object sender, EventArgs e)
		{
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_downloadFanPagePictures.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=5;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnChkFanPageManagerSendPicWithMessageClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(chkFanPageManagerSendPicWithMessage.Active);
			if (!checkBool) {
				btnUploadMessageFanPage.Sensitive = false;
				return;

			} else {
				btnUploadMessageFanPage.Sensitive = true;
			}
		}

		protected void OnChkUseSingleImageClicked (object sender, EventArgs e)
		{

			bool checkBool=Convert.ToBoolean(chk_UseSingleImage.Active);
			if (checkBool) 
			{
				btn_LoadSingleImage.Visible = true;
				btnUploadPicsFanPageManager.Sensitive = false;
			} 
			else 
			{
				btn_LoadSingleImage.Visible = false;
				btnUploadPicsFanPageManager.Sensitive=true;
			}
		}

		protected void OnBtnLoadSingleImageClicked (object sender, EventArgs e)
		{
			try
			{
				List<string> lstFanPagePics = new List<string>();
				List<string> lstCorrectFanPagePics = new List<string>();
				ObjFan.lstPicturecollectionPostPicOnFanPageWall.Clear();
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					string inputUrl = fileChooser.Filename.ToString();
					//string[] picsArray = Directory.GetFiles(inputUrl);
					lstFanPagePics.Add(inputUrl); //= picsArray.Distinct().ToList();
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
					ObjFan.lstPicturecollectionPostPicOnFanPageWall = lstCorrectFanPagePics;// upload correct Message
					Addtologger(lstCorrectFanPagePics.Count + "  Pics loaded");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}
		protected void OnPageScraperActionActivated (object sender, EventArgs e)
		{
			rbk_fanPageScraperByKeyword.Active = true;
			chkUseSingleItem.Label = "Use Single Keyword";
			bool checkBool=Convert.ToBoolean(rbk_fanPageScraperByKeyword.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="          Scrape Count : ";
					btnUploadKeyword.Sensitive=true;
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=false;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=0;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="10";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnPageLikerActionActivated (object sender, EventArgs e)
		{
			rbk_fanPageLiker.Active = true;
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_fanPageLiker.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=1;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnPostTextActionActivated (object sender, EventArgs e)
		{
			rbk_postSimpleTextMessage.Active = true;
			chkUseSingleItem.Label = "Use Single Message";
			bool checkBool=Convert.ToBoolean(rbk_postSimpleTextMessage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=2;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}
		protected void OnPostImagesActionActivated (object sender, EventArgs e)
		{
			rbk_PostPicturesOnFanpage.Active = true;
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_PostPicturesOnFanpage.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadPicsFanPageManager.Sensitive=true;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=3;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";


					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=true;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnDownloadPictureActionActivated (object sender, EventArgs e)
		{
			rbk_downloadFanPagePictures.Active = true;
			chkUseSingleItem.Label = "Use Single Fanpage Url";
			bool checkBool=Convert.ToBoolean(rbk_downloadFanPagePictures.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = false;
					btnUploadPicsFanPageManager.Sensitive=false;
					btnUploadUrlFanPage.Sensitive=true;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=5;
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					btnUploadKeyword.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					chkExportDataPageManager.Sensitive=true;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
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
			rbk_PostVideoUrlsOnFanpages.Active = true;
			chkUseSingleItem.Label = "Use Single video Url";
			bool checkBool=Convert.ToBoolean(rbk_PostVideoUrlsOnFanpages.Active);
			if (!checkBool)
			{
				return;

			} 
			else
			{
				try
				{
					label2.Text="No of Post Per URL : ";
					btnUploadMessageFanPage.Sensitive = true;
					btnUploadUrlFanPage.Sensitive=true;

					btnUploadPicsFanPageManager.Sensitive=false;				
					btn_FanPageLoadUrlsMassege.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Sensitive=true;
					cmbSelectInputFanPage.Active=4;
					btn_FanPageLoadUrlsMassege.Sensitive=true;
					btnUploadKeyword.Sensitive=false;
					chkExportDataPageManager.Sensitive=false;
					chkFanPageManagerSendPicWithMessage.Sensitive=false;
					Txt_NoofPostPerURLPageManager.Text="1";

					btn_LoadSingleImage.Visible = false;
					chk_UseSingleImage.Sensitive=false;
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
	}
}

