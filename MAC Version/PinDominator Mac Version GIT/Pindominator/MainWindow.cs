using System;
using Gtk;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Collections;
using BaseLib;
using System.Linq;
using Globussoft;
using ManageAccountManager;
using BoardManager;
using CommentManager;
using PinsManager;
using LikeManager;
using ScraperManagers;
using FollowManagers;

namespace PinDominator
{
public partial class MainWindow: Gtk.Window
{
	List<string> lstUploadAccount = new List<string> ();
	DrawingArea da;
	DrawingArea da1;

		public static MainWindow objMainWindow ;

		public static RePinManager objRePinManager = new RePinManager();

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
			Build ();

			Gdk.Color fontcolor = new Gdk.Color(67,96,156);
	//		MenuBarControls.ModifyBg (StateType.Normal, fontcolor);
			BackGroundColorChangeMenuBar ();
			BackGroundColorChange ();

		new Thread(()=>
				{
					GlobusLogHelper.objAddToLoggerDelegate = new AddToLoggerDelegate(Addtologger);
					CheckOperatingSystem();
					CreateAppDirectories();
					CopyDatabase();
					AddDataToGridColumnNew();
					AddDataToGridColumnForSelectAccount();
					Thread.Sleep(500);

				Gtk.Application.Invoke (delegate {

				});
				}).Start();

			AccountChecker.objDelegateNoOfAcc = new CheckAccount(AccountReportNoOfAccount_AccChecker);
			AccountChecker.objDelegateNoOfActiveAcc = new CheckAccount(AccountReportNoOfActiveAccount);
			AccountChecker.objDelegateNoOfDeadAcc = new CheckAccount(AccountReportNoOfDeadAccount);

			BoardsManager.objDelegateNoOfActiveAcc = new BoardAccount (AccountReportActiveAccount_Board);
			BoardsManager.objDelegateNoOfDeadAcc = new BoardAccount (AccountReportDeadAccount_Board);
			BoardsManager.objBoardDelegate = new AccountReport_Board (AccountReport_Board);

			AddBoardNameManager.objAddBoardDelegate = new AccountReport_AddBoard (AccountReport_AddBoard);
			AddBoardNameManager.objDelegateNoOfActiveAcc = new AddBoardAccount (AccountReportActiveAccount_AddBoard);
			AddBoardNameManager.objDelegateNoOfDeadAcc = new AddBoardAccount (AccountReportDeadAccount_AddBoard);

			AddUsersToBoardManager.objAddUserToBoarddelegate = new AccountReport_AddUserToBoard (AccountReport_AddUserToBoard);
			AddUsersToBoardManager.objDelegateNoOfActiveAcc = new AddUserToBoardAccount (AccountReportActiveAccount_AddUserToBoard);
			AddUsersToBoardManager.objDelegateNoOfDeadAcc = new AddUserToBoardAccount (AccountReportDeadAccount_AddUserToBoard);

			CommentManagers.objCommentDelegate = new AccountReport_Comments (AccountReport_Comment);
			CommentManagers.objDelegateNoOfActiveAcc = new CommentAccount (AccountReportActiveAccount_Comment);
			CommentManagers.objDelegateNoOfDeadAcc = new CommentAccount (AccountReportDeadAccount_Comment);

			CommentByKeywordManager.objCommentByKeywordDelegate = new AccountReport_CommentByKeyword (AccountReport_CommentByKeyword);
			CommentByKeywordManager.objDelegateNoOfActiveAcc_CommentByKeyword = new CommentByKeywordAccount (AccountReportActiveAccount_CommentByKeyword);
			CommentByKeywordManager.objDelegateNoOfDeadAcc_CommentByKeyword = new CommentByKeywordAccount (AccountReportDeadAccount_CommentByKeyword);

			FollowByKeywordManager.objFollowByKeywordDelegate = new AccountReport_FollowByKeyword (AccountReport_FollowByKeyword);
			FollowByKeywordManager.objDelegateNoOfActiveAcc_FollowByKeyword = new FollowByKeywordAccount (AccountReportActiveAccount_FollowByKeyword);
			FollowByKeywordManager.objDelegateNoOfDeadAcc_FollowByKeyword = new FollowByKeywordAccount (AccountReportDeadAccount_FollowByKeyword);

			FollowByUsernameManager.objFollowByUsernameDelegate = new AccountReport_FollowByUsername (AccountReport_FollowByUsername);
			FollowByUsernameManager.objDelegateNoOfActiveAcc_FollowByUsername = new FollowByUsernameAccount (AccountReportActiveAccount_FollowByUsername);
			FollowByUsernameManager.objDelegateNoOfDeadAcc_FollowByUsername = new FollowByUsernameAccount (AccountReportDeadAccount_FollowByUsername);

			UnFollowManager.objUnFollowDelegate = new AccountReport_UnFollow (AccountReport_UnFollow);
			UnFollowManager.objDelegateNoOfActiveAcc_UnFollow = new UnFollowAccount (AccountReportActiveAccount_UnFollow);
			UnFollowManager.objDelegateNoOfDeadAcc_UnFollow = new UnFollowAccount (AccountReportDeadAccount_UnFollow);

			LikeManagers.objLikeDelegate = new AccortReport_Like (AccountReport_Like);
			LikeManagers.objDelegateNoOfActiveAcc_Like = new LikeAccount (AccountReportActiveAccount_Like);
			LikeManagers.objDelegateNoOfDeadAcc_Like = new LikeAccount (AccountReportDeadAccount_Like);

			LikeByKeywordManager.objLikeByKeywordDelegate = new AccountReport_LikeByKeyword (AccountReport_LikeByKeyword);
			LikeByKeywordManager.objDelegateNoOfActiveAcc_LikeByKeyword = new LikeByKeywordAccount (AccountReportActiveAccount_LikeByKeyword);
			LikeByKeywordManager.objDelegateNoOfDeadAcc_LikeByKeyword = new LikeByKeywordAccount (AccountReportDeadAccount_LikeByKeyword);

			AddNewPinManager.objAddNewPinDelegate = new AccountReport_AddNewPin (AccountReport_AddNewPin);
			AddNewPinManager.objDelegateNoOfActiveAcc_AddNewPin = new AddNewPinAccount (AccountReportActiveAccount_AddNewPin);
			AddNewPinManager.objDelegateNoOfDeadAcc_AddNewPin = new AddNewPinAccount (AccountReportDeadAccount_AddNewPin);

			//AccountReport_AddNewPin
			try
			{
				AccounLoad();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
			intializeAllDelegate ();
			try
			{
			objMainWindow = this;
			}
			catch{
			};
	}

		public void intializeAllDelegate()
		{
			try
			{
				CommentManagers.objRepin_Comments_UserPins_Repin += new CommentManager.Repin_Comments_UserPins_Repin(objRePinManager.UserPins_Repin);
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
	

	public void BackGroundColorChangeMenuBar()
	{
		try 
		{

			da1 = new DrawingArea ();
			da1.ExposeEvent += OnExposed1;

			Gdk.Color col = new Gdk.Color ();
				Gdk.Color.Parse ("#3b5998", ref col);

			ModifyBg (StateType.Normal, col);
			da1.ModifyBg (StateType.Normal, col);

		} 
		catch (Exception ex) 
		{
			Console.Write (ex.Message);
		}
	}
	public void BackGroundColorChange()
	{
		try {
				da = new DrawingArea ();
				da.ExposeEvent += OnExposed1;

				Gdk.Color col = new Gdk.Color ();
				Gdk.Color.Parse ("#6d84b4", ref col);
		

				Gdk.Color col1 = new Gdk.Color ();
				Gdk.Color.Parse ("#dfe3ee", ref col1);

				Gdk.Color col2 = new Gdk.Color ();
				Gdk.Color.Parse ("#f7f7f7", ref col2);

				da.ModifyBg (StateType.Normal, col);
		} 
		catch (Exception ex) 
		{
			Console.Write (ex.Message);
		}
	}
	void OnExposed (object o, ExposeEventArgs args) 
	{
		da.GdkWindow.DrawLine(da.Style.BaseGC(StateType.Normal), 0, 0, 400, 300);

	}
	void OnExposed1 (object o, ExposeEventArgs args) 
	{
		da1.GdkWindow.DrawLine(da.Style.BaseGC(StateType.Normal), 0, 0, 400, 300);

	}


	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.OkCancel, "Do you want to close the software. ");
		ResponseType response = (ResponseType)md.Run ();

		
		if (response == ResponseType.Ok) {
			Application.Quit ();
			a.RetVal = true;
		} else {
			new MainWindow ();
		}
		md.Destroy ();

	}


	void ReloadAccounts(object sender, EventArgs e)
	{
		if (e is EventsArgs)
		{
			EventsArgs eArgs = e as EventsArgs;
			
		}
	}

	private void CheckOperatingSystem()
	{
		try
		{
		  OperatingSystem os = Environment.OSVersion;
		  string nameOfOS = os.Platform.ToString ();
		  PDGlobals.typeOfOperatingSystem = nameOfOS;
		}
		catch(Exception ex)
		{
			Console.Write (ex.Message);
		}

	}

	private void CreateAppDirectories()
	{

			try
			{
				if(PDGlobals.typeOfOperatingSystem.Contains("Win"))
				{
					if (!Directory.Exists (PDGlobals.path_AppDataFolder))
					{
						Directory.CreateDirectory (PDGlobals.path_AppDataFolder);
					}
					if (!Directory.Exists (PDGlobals.path_DesktopFolder)) 
					{
						Directory.CreateDirectory (PDGlobals.path_DesktopFolder);
					}
				}
				else if(PDGlobals.typeOfOperatingSystem.Contains("Unix"))
				{
					if (!Directory.Exists (PDGlobals.path_LinuxAppDataFolder))
					{
						Directory.CreateDirectory (PDGlobals.path_LinuxAppDataFolder);
					}
					if (!Directory.Exists (PDGlobals.path_LinuxDesktopFolder)) 
					{
						Directory.CreateDirectory (PDGlobals.path_LinuxDesktopFolder);
					}

				}
			}
			catch(Exception ex) {
				Console.Write (ex.Message);
			}
	}


		private void CopyDatabase()
		{

			if (PDGlobals.typeOfOperatingSystem.Contains ("Win")) {
				string startUpDB = AppDomain.CurrentDomain.BaseDirectory + "DB_PinDominator.db";
				string localAppDataDB = PDGlobals.path_AppDataFolder + "\\DB_PinDominator.db";

				string startUpDB64 = Environment.GetEnvironmentVariable ("ProgramFiles(x86)") + "\\DB_PinDominator.db";

				if (!File.Exists (localAppDataDB)) {
					///Modified [19-10] to work with 64 Bit as well

					if (File.Exists (startUpDB)) {
						try 
						{
							File.Copy (startUpDB, localAppDataDB);
						} 
						catch (Exception ex)
						{
							if (ex.Message.Contains ("Could not find a part of the path")) 
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator");
								File.Copy (startUpDB, localAppDataDB);
							}
						}
					} 
					else if (File.Exists (startUpDB64))
					{   //for 64 Bit
						try 
						{
							File.Copy (startUpDB64, localAppDataDB);
						} 
						catch (Exception ex)
						{
							if (ex.Message.Contains ("Could not find a part of the path")) 
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator");
								File.Copy (startUpDB64, localAppDataDB);
							}
						}
					}
				}
			}

			else if (PDGlobals.typeOfOperatingSystem.Contains ("Unix"))
			{
				string startUpDB = AppDomain.CurrentDomain.BaseDirectory + "\\DB_PinDominator.db";

				string localAppDataDB = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PinDominator/DB_PinDominator.db";

				string startUpDB64 = Environment.GetEnvironmentVariable ("ProgramFiles(x86)") + "/DB_PinDominator.db";

				if (!File.Exists (localAppDataDB))
				{
					///Modified [19-10] to work with 64 Bit as well

					if (File.Exists (startUpDB))
					{
						try {
							File.Copy (startUpDB, localAppDataDB);
						} 
						catch (Exception ex) 
						{
							if (ex.Message.Contains ("Could not find a part of the path")) 
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "/PinDominator");
								File.Copy (startUpDB, localAppDataDB);
							}
						}
					}
					else if (File.Exists (startUpDB64))
					{   //for 64 Bit
						try 
						{
							File.Copy (startUpDB64, localAppDataDB);
						}
						catch (Exception ex)
						{
							if (ex.Message.Contains ("Could not find a part of the path"))
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "/PinDominator");
								File.Copy (startUpDB64, localAppDataDB);
							}
						}
					}
				}
			}
			else 
			{
				string startUpDB = AppDomain.CurrentDomain.BaseDirectory + "DB_PinDominator.db";
				string localAppDataDB = PDGlobals.path_AppDataFolder + "\\DB_PinDominator.db";

				string startUpDB64 = Environment.GetEnvironmentVariable ("ProgramFiles(x86)") + "\\DB_PinDominator.db";

				if (!File.Exists (localAppDataDB))
				{
					///Modified [19-10] to work with 64 Bit as well

					if (File.Exists (startUpDB))
					{
						try
						{
							File.Copy (startUpDB, localAppDataDB);
						}
						catch (Exception ex) 
						{
							if (ex.Message.Contains ("Could not find a part of the path")) 
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator");
								File.Copy (startUpDB, localAppDataDB);
							}
						}
					} 
					else if (File.Exists (startUpDB64))
					{   //for 64 Bit
						try 
						{
							File.Copy (startUpDB64, localAppDataDB);
						}
						catch (Exception ex)
						{
							if (ex.Message.Contains ("Could not find a part of the path")) 
							{
								Directory.CreateDirectory (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator");
								File.Copy (startUpDB64, localAppDataDB);
							}
						}
					}
				}
			}
		}


	Gtk.ListStore accountData;

		public void AddDataToGridColumnNew()
		{
			try 
			{
				accountData = new Gtk.ListStore (typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string),typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

				treeViewAccountManager.AppendColumn ("     UserName    ", new Gtk.CellRendererText (), "text", 0);
				treeViewAccountManager.AppendColumn ("     Password    ", new Gtk.CellRendererText (), "text", 1);
				treeViewAccountManager.AppendColumn ("     Niche    ", new Gtk.CellRendererText (), "text", 2);
				treeViewAccountManager.AppendColumn ("     ProxyIpAddress   ", new Gtk.CellRendererText (), "text", 3);
				treeViewAccountManager.AppendColumn ("     ProxyPort     ", new Gtk.CellRendererText (), "text", 4);
				treeViewAccountManager.AppendColumn ("     ProxyUserName     ", new Gtk.CellRendererText (), "text", 5);
				treeViewAccountManager.AppendColumn ("     ProxyPassword     ", new Gtk.CellRendererText (), "text", 6);
				treeViewAccountManager.AppendColumn ("     ScreenName   ", new Gtk.CellRendererText (), "text", 7);
				treeViewAccountManager.AppendColumn ("     Followers     ", new Gtk.CellRendererText (), "text", 8);
				treeViewAccountManager.AppendColumn ("     Following     ", new Gtk.CellRendererText (), "text", 9);
				treeViewAccountManager.AppendColumn ("     LoginStatus     ", new Gtk.CellRendererText (), "text", 10);

				treeViewAccountManager.Model = accountData;

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}

		}
			

	Gtk.ListStore accountData_SelectAccount;
	public void AddDataToGridColumnForSelectAccount()
	{
		try 
		{
			
			
			Gtk.Application.Invoke (delegate {

				accountData_SelectAccount = new Gtk.ListStore (typeof(string), typeof(bool));

				
				Gtk.TreeViewColumn Column2 = new Gtk.TreeViewColumn ();
				Column2.Title = "IsChecked";
				Gtk.CellRendererToggle Cell2 = new Gtk.CellRendererToggle ();
				Cell2.Toggled+= new ToggledHandler(Cell2_Toggled);
				Column2.PackStart (Cell2, true);
				Column2.Clickable=true;

				
				Column2.AddAttribute (Cell2, "active", 1);			
			

			});
		}
		catch(Exception ex) 
		{
			Console.Write (ex.Message);
		}

	}


	void Cell2_Toggled(object o, ToggledArgs args)
	{
		TreeIter iter;

		if (accountData_SelectAccount.GetIter (out iter, new TreePath(args.Path))) {
			bool old = (bool) accountData_SelectAccount.GetValue(iter,1);
			accountData_SelectAccount.SetValue(iter,1,!old);
		}
		accountData_SelectAccount.GetIterFirst (out iter);

		var value = accountData_SelectAccount.GetValue(iter, 1);

		Updatelst_Of_Fb_Insta_For_Selected_Accounts ();

	}

	public void Updatelst_Of_Fb_Insta_For_Selected_Accounts()
	{
		TreeIter iter = new TreeIter();
		string SelectedModule = string.Empty;
		try
		{
					
		}
		catch(Exception ex) 
		{
			Console.Write (ex.Message);
		}


		accountData_SelectAccount.GetIterFirst (out iter);

		try
		{

		bool value = (bool)accountData_SelectAccount.GetValue(iter, 1);

		}
		catch(Exception Ex){
			int ire = 0;
		}
				
		accountData_SelectAccount.GetIterFirst (out iter);

		bool boolValue = (bool)accountData_SelectAccount.GetValue(iter, 1);

		if (boolValue) 
		{
			string stringValue = (string)accountData_SelectAccount.GetValue(iter, 0);
			

		}


		for (int i = 0; i < (accountData_SelectAccount.IterNChildren()-1); i++) {

			accountData_SelectAccount.IterNext (ref iter);

			 boolValue = (bool)accountData_SelectAccount.GetValue(iter, 1);

			if (boolValue) 
			{
				string stringValue = (string)accountData_SelectAccount.GetValue(iter, 0);
			

			}

		}


	}



	private Gtk.TreeIter iter;
	[GLib.ConnectBeforeAttribute()]
	void tree_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
	{
		string formToOpen="";
		TreeSelection selection = (o as TreeView).Selection;

		TreeModel model;

		if(selection.GetSelected(out model, out iter))
		{
			formToOpen=model.GetValue(iter,1).ToString();
			//Console.WriteLine(formToOpen);
		}
			
	}



		public void AddDataInGridView(string userNameData, string PasswordData, string Niches, string ProxyIpAddressData, string ProxyIpPortData, string ProxyUserNameData, string ProxyPasswordData, string ScreenName, string Followers, string Following, string LoginStatus)
		{
			try {

				accountData.AppendValues (userNameData, PasswordData, Niches, ProxyIpAddressData, ProxyIpPortData, ProxyUserNameData, ProxyPasswordData, ScreenName, Followers, Following, LoginStatus);
				treeViewAccountManager.Model = accountData;
			} catch (Exception ex) {
				Console.Write (ex.Message);
			}
		}

	public void AddDataInGridViewSelectAccount(string userNameData)
	{
		try
		{
			accountData_SelectAccount.AppendValues (userNameData,"true");
			

		}
		catch(Exception ex) 
		{
			Console.Write (ex.Message);
		}
	}



	public void AddtologgerOld(string log)
	{
		//this.ClientEvent+= delegate {    
		try{
			//
			Gtk.Application.Invoke (delegate {
//				txtViewLoggerCommon.Buffer.Text += log+"\n";

			});
		}
		catch(Exception ex)
		{
			Console.Write (ex.Message);
		}
		//};
	}


		public void Addtologger(string log)
		{
			//this.ClientEvent+= delegate {

			try
			{
			bool istrue = false;
	
			if (!istrue) {


				try {
					//
					Gtk.Application.Invoke (delegate {
							txtViewLogger.Buffer.Text += log + "\n";

					});
				} catch (Exception ex) {

						try
						{
							//logger.Buffer.Text += log+"\n";	
							//istrue = true;
						}
						catch{
						};



					Console.Write (ex.Message);
				}
			}
			//};
			}
			catch{
			};
		}


	void CampaignnameLog(object sender, EventArgs e)
	{
		if (e is EventsArgs)
		{
			EventsArgs eArgs = e as EventsArgs;
			Addtologger(eArgs.log);
		}
	}

	protected void OnBtnUploadAccountClicked (object sender, EventArgs e)
	{
		try
		{				
			LoadAccountsMod();
		}
		catch(Exception ex) 
		{
			Console.Write (ex.Message);
		}
	}

	//Globals Class Objects 
	QueryManager QM = new QueryManager();
	int countAccountUploaded = 0;

	private void LoadAccountsMod()
		{
			try {

				QueryManager.deleteQuery ();
				PDGlobals.loadedAccountsDictionary.Clear ();
				PDGlobals.listAccounts.Clear();
			} catch (Exception ex) {
				Console.Write (ex.Message);
			}
			FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
	
			if (fileChooser.Run () == (int)ResponseType.Accept) {
				//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
				lstUploadAccount = GlobusFileHelper.ReadTweetFiletoStringList (fileChooser.Filename.ToString ());
				Addtologger ("Account Ready to Upload: " + lstUploadAccount.Count);
				countAccountUploaded = lstUploadAccount.Count;
			} else {
				lstUploadAccount.Clear ();
			}

			fileChooser.Destroy ();
 
			foreach (string item in lstUploadAccount) {
				ThreadPool.SetMaxThreads (50, 50);
 				//ThreadPool.QueueUserWorkItem(new WaitCallback(LoadAccUsingThreadPool),new object[]{item});
				LoadAccUsingThreadPool (new object[]{ item });
				Thread.Sleep (200);
			}

			try {
				AccounLoad ();
			} catch (Exception ex) {
				Console.Write (ex.Message);
			}				
		}

		private void LoadAccUsingThreadPool(object Acc)
		{
			try
			{
				if (lstUploadAccount.Count > 0)
				{
					PDGlobals.loadedAccountsDictionary.Clear();
					PDGlobals.listAccounts.Clear();
				}

				if (Globals.IsBasicVersion)
				{
					try
					{
						string selectQuery = "select count(UserName) from tb_emails";
						DataSet DS = DataBaseHandler.SelectQueryNew(selectQuery, "tb_emails");
						int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

						if (countLoadedAccounts >= 5)
						{
							AccounLoad();
							MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "You Are Using PD Basic Version 5 Accounts allready loaded..");
							ResponseType response = (ResponseType)md.Run ();
							md.Destroy ();
							//MessageBox.Show("You Are Using PD Basic Version 5 Accounts allready loaded..");
							return;
						}
						else
						{
							int RemainingAccount = 5 - countLoadedAccounts;

							lstUploadAccount.RemoveRange(RemainingAccount, lstUploadAccount.Count - RemainingAccount);

						}
					}
					catch { }
				}
				if (Globals.IsProVersion)
				{
					try
					{
						string selectQuery = "select count(UserName) from tb_emails";
						DataSet DS = DataBaseHandler.SelectQueryNew(selectQuery, "tb_emails");
						int countLoadedAccounts = Convert.ToInt32(DS.Tables[0].Rows[0].ItemArray[0].ToString());

						if (countLoadedAccounts >= 15)
						{
							AccounLoad();
							MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "You Are Using PD Pro Version 15 Accounts allready loaded..");
							ResponseType response = (ResponseType)md.Run ();
							md.Destroy ();
							return;
							//MessageBox.Show("You Are Using PD Pro Version 15 Accounts allready loaded..");			
						}
						else
						{
							int RemainingAccount = 15 - countLoadedAccounts;

							lstUploadAccount.RemoveRange(RemainingAccount, lstUploadAccount.Count - RemainingAccount);

						}
					}
					catch { }
				}


				Array paramArray = new Gtk.Object[0];
				paramArray = (Array)Acc;
				string account = Convert.ToString (((object[])(paramArray)) [0]);		
				string accountUser = account.Split(':')[0];
				string accountPass = account.Split(':')[1];
				string niches = string.Empty;
				string proxyAddress = string.Empty;
				string proxyPort = string.Empty;
				string proxyUserName = string.Empty;
				string proxyPassword = string.Empty;
				string Followers = string.Empty;
				string Following = string.Empty;			
				string BoardsName = string.Empty;
				string ScreenName = string.Empty;
				string LoginStatus = string.Empty;

				int DataCount = account.Split(':').Length;
				if (DataCount == 3)
				{
					niches = account.Split(':')[2];

				}
				else if (DataCount == 5)
				{
					niches = account.Split(':')[2];
					proxyAddress = account.Split(':')[3];
					proxyPort = account.Split(':')[4];
				}
				else if (DataCount == 7)
				{
					niches = account.Split(':')[2];
					proxyAddress = account.Split(':')[3];
					proxyPort = account.Split(':')[4];
					proxyUserName = account.Split(':')[5];
					proxyPassword = account.Split(':')[6];
				}
				LoginStatus = "NotChecked";
				try
				{
					QM.AddAccountInDataBase(accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, LoginStatus);
				}
				catch(Exception ex)
				{}
				//DataBaseHandler.InsertQuery("INSERT INTO tb_emails (Username, Password, Niches, proxyAddress, proxyPort, proxyUsername, proxyPassword, UserAgent, Follower, Following , BOARDS, BoardsName, ScreenName, LoginStatus) VALUES ('" + accountUser + "','" + accountPass + "', '" + niches + "' ,'" + proxyAddress + "','" + proxyPort + "',  '" + proxyUserName + "','" + proxyPassword + "','" + " " + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + " " + "','" + ScreenName + "','" + LoginStatus + "' )","tb_emails");
				try {
					PinInterestUser objPinUser = new PinInterestUser ();
					objPinUser.Username = accountUser;
					objPinUser.Password = accountPass;
					objPinUser.Niches = niches;
					objPinUser.ProxyAddress = proxyAddress;
					objPinUser.ProxyPort = proxyPort;
					objPinUser.ProxyUsername = proxyUserName;
					objPinUser.ProxyPassword = proxyPassword;
					objPinUser.BoardsName = BoardsName;
					objPinUser.ScreenName = ScreenName;
					objPinUser.LoginStatus = LoginStatus;				

					try {
						//PDGlobals.loadedAccountsDictionary.Add(objPinUser.Username, objPinUser);
						PDGlobals.listAccounts.Add(objPinUser.Username + ":" + objPinUser.Password + ":" + objPinUser.Niches + ":" + objPinUser.ProxyAddress + ":" + objPinUser.ProxyPort + ":" + objPinUser.ProxyUsername + ":" + objPinUser.ProxyPassword);
						//AddDataInGridView (accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, Followers, Following, LoginStatus);
					} catch (Exception ex) {

						Console.Write (ex.Message);
					}
					//Addtologger(objFacebookAccountManager.username + ":"+ objFacebookAccountManager.password);
				} catch (Exception ex) {

					Console.Write (ex.Message);
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
			finally {
				countAccountUploaded--;
				if (countAccountUploaded == 0) {
					Addtologger ("Account Uploaded process complete");
				}
			}
		}

		public void  AccounLoad()
		{
			try
			{				
				string accountUser = string.Empty;
				string accountPass = string.Empty;
				string niches = string.Empty;
				string proxyAddress = string.Empty;
				string proxyPort = string.Empty;
				string proxyUserName = string.Empty;
				string proxyPassword = string.Empty;
				string BoardsName = string.Empty;
				string ScreenName = string.Empty;
				string LoginStatus = string.Empty;
				string Followers = string.Empty;
				string Following = string.Empty;
				QueryExecuter QME = new QueryExecuter();
				DataSet ds = QME.getAccount();
				accountData.Clear();
				//DataTable dt = QME.getAccount (module);
				if (ds.Tables[0].Rows.Count != 0)
				{
					PDGlobals.listAccounts.Clear();
					for (int noRow = 0; noRow < ds.Tables[0].Rows.Count; noRow++)
					{
						string account = ds.Tables[0].Rows[noRow].ItemArray[0].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[1].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[2].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[3].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[4].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[5].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[6].ToString() + ":" + ds.Tables[0].Rows[noRow].ItemArray[7].ToString();
						PDGlobals.listAccounts.Add(account);
						//  dv.AllowNew = false;
						accountUser = ds.Tables[0].Rows[noRow].ItemArray[0].ToString();
						accountPass = ds.Tables[0].Rows[noRow].ItemArray[1].ToString();
						niches = ds.Tables[0].Rows[noRow].ItemArray[2].ToString();
						proxyAddress = ds.Tables[0].Rows[noRow].ItemArray[3].ToString();
						proxyPort = ds.Tables[0].Rows[noRow].ItemArray[4].ToString();
						proxyUserName = ds.Tables[0].Rows[noRow].ItemArray[5].ToString();
						proxyPassword = ds.Tables[0].Rows[noRow].ItemArray[6].ToString();
						ScreenName = ds.Tables[0].Rows[noRow].ItemArray[9].ToString();
						Followers = ds.Tables[0].Rows[noRow].ItemArray[7].ToString();
						Following = ds.Tables[0].Rows[noRow].ItemArray[8].ToString();
						LoginStatus = ds.Tables[0].Rows[noRow].ItemArray[10].ToString();

						// Accounts objPinInterestUser = new Accounts();
						PinInterestUser objPinInterestUser = new PinInterestUser("", "", "", "");
						objPinInterestUser.Username = accountUser;
						objPinInterestUser.Password = accountPass;
						objPinInterestUser.Niches = niches;
						objPinInterestUser.ProxyAddress = proxyAddress;
						objPinInterestUser.ProxyPort = proxyPort;
						objPinInterestUser.ProxyUsername = proxyUserName;
						objPinInterestUser.ProxyPassword = proxyPassword;
						try
						{
							PDGlobals.loadedAccountsDictionary.Add(objPinInterestUser.Username, objPinInterestUser);
							AddDataInGridView (accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, Followers, Following, LoginStatus);
						}
						catch (Exception ex)
						{ 
							AddDataInGridView (accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, Followers, Following, LoginStatus);
						};
					}

					Addtologger ("[ " + DateTime.Now + " ] => [ " + PDGlobals.listAccounts.Count + " Accounts Loaded ]");
				}
				else
				{
					AddDataInGridView (accountUser, accountPass, niches, proxyAddress, proxyPort, proxyUserName, proxyPassword, ScreenName, Followers, Following, LoginStatus);
					Addtologger ("[ " + DateTime.Now + " ] => [  No Accounts Loaded ]");
				}
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
	




	protected void OnCmbSelectAccountChanged (object sender, EventArgs e)
	{
		string SelectedModule = string.Empty;
	
		try
		{
			//Gtk.Application.Invoke (delegate {
				accountData.Clear();
			//});
			//Gtk.Application.Invoke (delegate {
				SelectedModule = "Facebook";
			//});
			

		}
		catch(Exception ex) 
		{
			Console.Write (ex.Message);
		}
	}

	protected void OnBtnClearAccountsAccountManagerClicked (object sender, EventArgs e)
	{
			try 
			{
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.OkCancel, "Are you sure to clear all accounts from database?");
				ResponseType response = (ResponseType)md.Run ();

				//string module = string.Empty;
				if (response == ResponseType.Ok) 
				{
					QueryManager objclsPDAccount = new QueryManager();
					DataTable dt = objclsPDAccount.SelectAccoutsForGridView();
					if (dt.Rows.Count > 0) 
					{	
						try
						{
							QueryManager.deleteQuery ();
						}
						catch{}
						Gtk.Application.Invoke (delegate {
							accountData.Clear ();
							PDGlobals.loadedAccountsDictionary.Clear ();
							PDGlobals.listAccounts.Clear ();

						});
						Addtologger ("Account Deleted Successfully");
					} 
					else 
					{
						Addtologger ("There is no Account Present in Database.");
					}			

				}
				md.Destroy ();
			} 
			catch {
			};

	}



	protected void OnBtnAddAccountClicked (object sender, EventArgs e)
	{
			frmAddAccounts objAddAccounts = new frmAddAccounts ();
			objAddAccounts.Show ();
	}

		//--------------------------------------------------End Account Module-----------------------------------------------------------------------------------------------

		BoardsManager objBoardManager = new BoardsManager ();
		AddBoardNameManager objAddBoardManager = new AddBoardNameManager ();
		AddUsersToBoardManager objAddUsersToBoardManager = new AddUsersToBoardManager();
		CommentManagers objCommentManager = new CommentManagers();

		Utils.Utils objUtils = new Utils.Utils ();

		//----------------------------------------------------Start Board Module--------------------------------------------------------------------------------------------------
		protected void OnBoardActionActivated(object sender, EventArgs e)
		{
//			frmBoard objBoardManager = new frmBoard ();
//			objBoardManager.Show ();
		}

		public void AccountReport_Board()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstPinAccReport = new List<string>();
				try
				{
					DS = QM.SelectAddReport("Boards");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Pin = dt_item.ItemArray[3].ToString();	
						lstPinAccReport.Add(Pin);
						lstPinAccReport.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{
						
						Gtk.Application.Invoke(delegate
							{								
								lblPinSent_Board.Text = lstPinAccReport.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		public void AccountReportActiveAccount_Board(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblSuccfullAcc_Board.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_Board(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lbFailedAcc_Board.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		protected void OnRdbsingleUserBoardClicked (object sender, EventArgs e)
		{
			try
			{
				if (rdbsingleUserBoard.Active == true) 
				{
					btnBrowseBoard.Visible = false;
					btnBrowseMsgBoard.Visible = false;
					txtBoardUrl.Sensitive = true;
					txtMsgBoard.Sensitive = true;
					label28.Text = "Enter Board Url : ";
					label30.Text = " Enter Message : ";
				}	
				else 
				{
					btnBrowseBoard.Visible = true;
					btnBrowseMsgBoard.Visible = true;
					txtBoardUrl.Sensitive = false;
					txtMsgBoard.Sensitive = false;
					label28.Text = "          Board Url : ";
					label30.Text = "           Message : ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnBrowseBoardClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseBoardUrl();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseBoardUrl()
		{
			try
			{
				ClGlobul.lstListOfBoardNames.Clear();
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					txtBoardUrl.Text = fileChooser.Filename.ToString ();
					ClGlobul.lstListOfBoardNames = GlobusFileHelper.ReadFile (txtBoardUrl.Text.Trim ());
					Addtologger("[ " + DateTime.Now + " ] => [ Board Url Loaded : " + ClGlobul.lstListOfBoardNames.Count + " ]");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnBrowseMsgBoardClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseMessage();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void BrowseMessage()
		{
			try
			{
				ClGlobul.lstBoardRepinMessage.Clear();
				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{

					txtMsgBoard.Text = fileChooser.Filename.ToString ();
					ClGlobul.lstBoardRepinMessage = GlobusFileHelper.ReadFile (txtMsgBoard.Text.Trim ());
					Addtologger("[ " + DateTime.Now + " ] => [ Message Loaded : " + ClGlobul.lstBoardRepinMessage.Count + " ]");
				}

				fileChooser.Destroy ();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartBoardClicked (object sender, EventArgs e)
		{
			try
			{
				startProcess();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void startProcess()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{	
						string SingleBoardUrl = string.Empty;
						string SingleMsg =  string.Empty;
						if (rdbsingleUserBoard.Active == true) 
						{
							ClGlobul.lstListOfBoardNames.Clear();
							ClGlobul.lstBoardRepinMessage.Clear();

							SingleBoardUrl = txtBoardUrl.Text.Trim();
							ClGlobul.lstListOfBoardNames.Add(SingleBoardUrl);
							Addtologger("[ " + DateTime.Now + " ] => [ Board Url Loaded : " + ClGlobul.lstListOfBoardNames.Count + " ]");

							SingleMsg = txtMsgBoard.Text.Trim();
							ClGlobul.lstBoardRepinMessage.Add(SingleMsg);
							Addtologger("[ " + DateTime.Now + " ] => [ Message Loaded : " + ClGlobul.lstBoardRepinMessage.Count + " ]");
						}
						else if(string.IsNullOrEmpty(txtBoardUrl.Text))
						{
							Addtologger("[ " + DateTime.Now + " ] => [ Please Upload Board Url ]");	
							return;
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}

					try
					{
					clsSettingDB Database = new clsSettingDB();
					Database.UpdateSettingData("Board", "AddBoardMessage", StringEncoderDecoder.Encode(txtMsgBoard.Text));

					objBoardManager.isStopBoards = false;
					objBoardManager.lstThreadsBoards.Clear();
					if (objBoardManager._IsfevoriteBoards)
					{
						objBoardManager._IsfevoriteBoards = false;
					}
					}
					catch{};

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;

					try
					{
						try
						{
							objBoardManager.minDelayBoards = Convert.ToInt32(txtMinDelayBoard.Text);
							objBoardManager.maxDelayBoards = Convert.ToInt32(txtMaxDelayBoard.Text);
							objBoardManager.NoOfThreadsBoards = Convert.ToInt32(txtThreadBoard.Text);
							objBoardManager.MaxRePinCount = Convert.ToInt32(txtMaxCountBoard.Text);
						}
						catch (Exception ex)
						{
							Addtologger("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThreadBoard.Text) && checkNo.IsMatch(txtThreadBoard.Text))
						{
							threads = Convert.ToInt32(txtThreadBoard.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						objBoardManager.NoOfThreadsBoards = threads;

						Addtologger("[ " + DateTime.Now + " ] => [ Process Starting ] ");


						try
						{
						Thread BoardsThread = new Thread(objBoardManager.StartBoards);
						BoardsThread.Start();
						btnStopBoard.Sensitive =  true;
						}
						catch{};
					}

					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					Addtologger("Please Load Accounts !");				
				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnBtnStopBoardClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStop = new Thread(stopBoard);
				objStop.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopBoard()
		{
			try
			{
				objBoardManager._IsfevoriteBoards = true;
				List<Thread> lstTempBoards = objBoardManager.lstThreadsBoards.Distinct().ToList();
				foreach (Thread item in lstTempBoards)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
					}
				}

				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("-------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//----------------------------------------------------------------------------End Board Module----------------------------------------------------------------------

		//-------------------------------------------------------------------------Start Add Board Module------------------------------------------------------------------

		public void AccountReportActiveAccount_AddBoard(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblNoOfSuccesfullAcc_AddBoard.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_AddBoard(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblNoOfFailedAcc_AddBoard.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		protected void OnChkSingleUserAddBoardClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_AddBoard.Active == true) 
				{
					btnBrowseBoardName_AddBroard.Visible = false;				
					txtBoardName_AddBoard.Sensitive = true;			
					lblBoardName_AddBoard.Text = " Enter Board Name : ";
				}	
				else 
				{
					btnBrowseBoardName_AddBroard.Visible = true;
					txtBoardName_AddBoard.Sensitive = false;
					lblBoardName_AddBoard.Text = "           Board Name : ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		public void AccountReport_AddBoard()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string BoardName = string.Empty;
				List<string> lstBoardAccReport = new List<string>();
				try
				{
					DS = QM.SelectAddReport("AddBoardName");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						BoardName = dt_item.ItemArray[4].ToString();	
						lstBoardAccReport.Add(BoardName);
						lstBoardAccReport.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblNoOfBoardAdded_AddBoard.Text = lstBoardAccReport.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		protected void OnBtnBrowseBoardNameAddBroardClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseBoardName();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void BrowseBoardName()
		{
			try
			{
				FileChooserDialog filechooser = new FileChooserDialog("Choose File To View",this,FileChooserAction.Open,"Cancel",ResponseType.Cancel,"Open",ResponseType.Accept);
				if (filechooser.Run() == (int)ResponseType.Accept) 
				{
					txtBoardName_AddBoard.Text = filechooser.Filename.ToString();
					ClGlobul.lstBoardNames=GlobusFileHelper.ReadFile(txtBoardName_AddBoard.Text.Trim());
					Addtologger("[ " + DateTime.Now + " ] =>  Board Name Uploaded :" + ClGlobul.lstBoardNames.Count);	
				}
				if (ClGlobul.lstBoardNames.Count > 0)
				{
					string checkBoardName = string.Empty;
					foreach (string st in ClGlobul.lstBoardNames)
					{
						checkBoardName += st;
					}

					objAddBoardManager.noOfBoard = System.Text.RegularExpressions.Regex.Split(checkBoardName, ",");
				}
				filechooser.Destroy ();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartAddBoardClicked (object sender, EventArgs e)
		{
			try
			{
				StartAddBoardName();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void StartAddBoardName()
		{
			try
			{
				btnStop_AddBoard.Sensitive = true;
				objAddBoardManager.isStopAddBoardName = false;
				string singleUser=string.Empty;
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if(chkSingleUser_AddBoard.Active == true)
						{
							singleUser=txtBoardName_AddBoard.Text.Trim();
							ClGlobul.lstBoardNames.Add(singleUser);
						}

						if (string.IsNullOrEmpty(txtBoardName_AddBoard.Text))
						{
							Addtologger("[ " + DateTime.Now + " ] => [ Please Upload Board Name With Niche ]");
							//ModernDialog.ShowMessage("Please Upload Board Name With Niche", "Upload Board Name With Niche", MessageBoxButton.OK);
							return;
						}


					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}

					clsSettingDB Database = new clsSettingDB();
					Database.UpdateSettingData("Board", "AddBoardName", StringEncoderDecoder.Encode(txtBoardName_AddBoard.Text));

					objAddBoardManager.isStopAddBoardName = false;
					objAddBoardManager.lstThreadsAddBoardName.Clear();
					if (objAddBoardManager._Isfevorite)
					{
						objAddBoardManager._Isfevorite = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							objAddBoardManager.minDelayAddBoardName = Convert.ToInt32(txtMinDelay_AddBoard.Text);
							objAddBoardManager.maxDelayAddBoardName = Convert.ToInt32(txtMaxDelay_AddBoard.Text);
							objAddBoardManager.Nothread_AddBoardName = Convert.ToInt32(txtThread_AddBoard.Text);
						}
						catch (Exception ex)
						{
							Addtologger("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_AddBoard.Text) && checkNo.IsMatch(txtThread_AddBoard.Text))
						{
							threads = Convert.ToInt32(txtThread_AddBoard.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						objAddBoardManager.NoOfThreadsAddBoardName = threads;

						Addtologger("[ " + DateTime.Now + " ] => [ Process Starting ] ");

						Thread AddBoardNameThread = new Thread(objAddBoardManager.StartAddBoardName);
						btnStart_AddBoard.Sensitive = true;
						AddBoardNameThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					Addtologger("Please Load Accounts !");

				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopAddBoardClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopAddBoard = new Thread(stopAddBoardName);
				objStopAddBoard.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopAddBoardName()
		{
			try
			{
				objAddBoardManager._Isfevorite = true;
				List<Thread> lstTemp_AddBoardName = objAddBoardManager.lstThreadsAddBoardName.Distinct().ToList();
				foreach (Thread item in lstTemp_AddBoardName)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("--------------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("--------------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

	
		//--------------------------------------------------------------------------End Add Board Module----------------------------------------------------------------

		//----------------------------------------------------------------------Start AddUser To Board Module ----------------------------------------------------------

		public void AccountReportActiveAccount_AddUserToBoard(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblNoActiveAcc_AddUserToBoard.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_AddUserToBoard(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_AddUserToBoard.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_AddUserToBoard()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string UserName = string.Empty;
				List<string> lstUSernameAccReport = new List<string>();
				try
				{
					DS = QM.SelectAddReport("AddUserToBoard");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						UserName = dt_item.ItemArray[5].ToString();	
						lstUSernameAccReport.Add(UserName);
						lstUSernameAccReport.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblNoOfSentInvitation_AddUserToBoard.Text = lstUSernameAccReport.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}



		protected void OnChkSingleUserAddUserToBoardClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_AddUserToBoard.Active == true) 
				{
					btnBrowseEmail_AddUsersToBoard.Visible = false;				
					txtEmail_AddUserToBoard.Sensitive = true;			
					lblEmail_AddUserToBoard.Text = "Enter BoardName And Email : ";
				}	
				else 
				{
					btnBrowseEmail_AddUsersToBoard.Visible = true;
					txtEmail_AddUserToBoard.Sensitive = false;
					lblEmail_AddUserToBoard.Text = "          BoardName And Email :  ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnBrowseEmailAddUsersToBoardClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseBoardNameEmail();
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}
		private void BrowseBoardNameEmail()
		{
			try
			{
				ClGlobul.lstAddToBoardUserNames.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose File To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtEmail_AddUserToBoard.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtEmail_AddUserToBoard.Text.Trim());
					Addtologger("[ " + DateTime.Now + " ] => [ Board Name And Email Uploaded :" + ClGlobul.lstAddToBoardUserNames.Count);	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartAddUsersToBoardClicked (object sender, EventArgs e)
		{
			try
			{
				StartAddUserToBoard();
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
		private void StartAddUserToBoard()
		{
			try
			{
				string singleUser = string.Empty;
				btnStop_AddUserToBoard.Sensitive = true;
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (chkSingleUser_AddUserToBoard.Active == true) 
						{
							ClGlobul.lstAddToBoardUserNames.Clear();
							if (string.IsNullOrEmpty(txtEmail_AddUserToBoard.Text)) 
							{
								Addtologger("Please Upload BoardName And Email ");										
								return;
							}
							else{
							try
								{
								    singleUser=txtEmail_AddUserToBoard.Text.Trim();
									ClGlobul.lstAddToBoardUserNames.Add(singleUser);
									Addtologger("[ " + DateTime.Now + " ] => [ Board Name Enter :" + ClGlobul.lstAddToBoardUserNames.Count);	
								}
								catch(Exception Ex)  
								{
									Console.Write(Ex.Message);
								}
							}
						}
						else  if (string.IsNullOrEmpty(txtBoardName_AddBoard.Text))
						{
							Addtologger("Please Upload BoardName And Email ");										
							return;
						}
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}

					//objAddUsersToBoardManager.BoardName = txtBoardName.Text.Trim();

					objAddUsersToBoardManager.isStopAddUserToBoard = false;
					objAddUsersToBoardManager.lstThreadsAddUserToBoard.Clear();
					if (objAddUsersToBoardManager._IsfevoriteAddUserToBoard)
					{
						objAddUsersToBoardManager._IsfevoriteAddUserToBoard = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;

					try
					{
						try
						{
							objAddUsersToBoardManager.minDelayAddUserToBoard = Convert.ToInt32(txtMinDelay_AddUserToBoard.Text);
							objAddUsersToBoardManager.maxDelayAddUserToBoard = Convert.ToInt32(txtMAxDElay_AddUserToBoard.Text);
							objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = Convert.ToInt32(txtThread_AddUsersToBoard.Text);
						}
						catch (Exception ex)
						{
							Addtologger("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_AddUsersToBoard.Text) && checkNo.IsMatch(txtThread_AddUsersToBoard.Text))
						{
							threads = Convert.ToInt32(txtThread_AddUsersToBoard.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = threads;

						clsSettingDB Database = new clsSettingDB();
						//Database.UpdateSettingData("UserToBoard", "UserToBoard", StringEncoderDecoder.Encode(txtEmailOrUserNames.Text));

						Addtologger("[ " + DateTime.Now + " ] => [ Process Starting ] ");

						Thread AddUserToBoardThread = new Thread(objAddUsersToBoardManager.StartAddUsersToBoard);
						btnStart_AddUsersToBoard.Sensitive = true;
						AddUserToBoardThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					Addtologger("Please Load Accounts !");
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopAddUserToBoardClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopAddUserToBoard = new Thread(stopAddUsersToBoard);
				objStopAddUserToBoard.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopAddUsersToBoard()
		{
			try
			{
				objAddUsersToBoardManager._IsfevoriteAddUserToBoard = true;
				List<Thread> lstTemp = objAddUsersToBoardManager.lstThreadsAddUserToBoard.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("-----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//------------------------------------------------------------------------------End Add User to Board Module---------------------------------------------------

		//---------------------------------------------------------------------------------Start Comment Module----------------------------------------------------

		public void AccountReportActiveAccount_Comment(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblNoofActiveAcc_Comment.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_Comment(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_Comment.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_Comment()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstCommentAccReport_Comment = new List<string>();
				try
				{
					DS = QM.SelectAddReport("Comment");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Pin = dt_item.ItemArray[3].ToString();	
						lstCommentAccReport_Comment.Add(Pin);
						lstCommentAccReport_Comment.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblCommentDone_Comment.Text = lstCommentAccReport_Comment.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}



		protected void OnChkSingleUserCommentClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_Comment.Active == true) 
				{
					btnBrowseMsg_comment.Visible = false;				
					txtMsg_Comment.Sensitive = true;			
					lblMessage_Comment.Text = "Enter Message : ";
				}	
				else 
				{
					btnBrowseMsg_comment.Visible = true;
					txtMsg_Comment.Sensitive = false;
					lblMessage_Comment.Text = "          Message :  ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnBrowseMsgCommentClicked (object sender, EventArgs e)
		{
			try 
			{
				BrowseMessageComment();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseMessageComment()
		{
			try
			{
				ClGlobul.CommentMessagesList.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtMsg_Comment.Text = objFileChooser.Filename.ToString();
					ClGlobul.CommentMessagesList = GlobusFileHelper.ReadFile(txtMsg_Comment.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Message Uploaded :" + ClGlobul.CommentMessagesList.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartCommentClicked (object sender, EventArgs e)
		{
			try 
			{
				startCommentProcess();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void startCommentProcess()
		{
			try
			{
				string singleUser = string.Empty;
				btnStop_Comment.Sensitive = true;
				objCommentManager.isStopComment = false;

				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (chkSingleUser_Comment.Active == true) 
						{
							ClGlobul.CommentMessagesList.Clear();
							if (string.IsNullOrEmpty(txtMsg_Comment.Text))
							{
								Addtologger("Please Enter Message");									
								return;
							}
							else
							{
								singleUser=txtMsg_Comment.Text.Trim();
								ClGlobul.CommentMessagesList.Add(singleUser);
								Addtologger("[ " + DateTime.Now + "] => [ Message Enter :" + ClGlobul.CommentMessagesList.Count);	
							}
						}
						if (string.IsNullOrEmpty(txtMsg_Comment.Text))
						{
							Addtologger("[ " + DateTime.Now + "] => [ Please Upload Message");									
							return;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine (ex.StackTrace);

					}

					clsSettingDB Database = new clsSettingDB();

					//Database.UpdateSettingData("Board", "AddBoardMessage", StringEncoderDecoder.Encode(txtMsg_Board.Text));


					objCommentManager.lstThreadsComment.Clear();
					if (objCommentManager._IsfevoriteComment)
					{
						objCommentManager._IsfevoriteComment = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;

					try
					{
						try
						{
							CommentManagers.minDelayComment = Convert.ToInt32(txtMinDelay_Comment.Text);
							CommentManagers.maxDelayComment = Convert.ToInt32(txtMaxDelay_comment.Text);
							objCommentManager.NoOfThreadsComment = Convert.ToInt32(txtThread_Comment.Text);
							CommentManagers.MaxComment = Convert.ToInt32(txtCount_comment.Text);
						}
						catch (Exception ex)
						{
							Addtologger("[ " + DateTime.Now + "] => [ Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_Comment.Text) && checkNo.IsMatch(txtThread_Comment.Text))
						{
							threads = Convert.ToInt32(txtThread_Comment.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						objCommentManager.NoOfThreadsComment = threads;

						Addtologger("[ " + DateTime.Now + "] => [ Process Starting ] ");

						Thread BoardsThread = new Thread(objCommentManager.StartComment);
						btnStart_Comment.Sensitive = true;
						BoardsThread.Start();
					}

					catch (Exception ex)
					{
						Console.WriteLine (ex.StackTrace);

					}
				}
				else
				{
					Addtologger("[ " + DateTime.Now + "] => [ Please Load Accounts !");
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
				
		protected void OnBtnStopCommentClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopComment = new Thread(stopComment);
				objStopComment.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopComment()
		{
			try
			{
				objCommentManager._IsfevoriteComment = true;
				List<Thread> lstTemp = objCommentManager.lstThreadsComment.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("-----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
		//-----------------------------------------------------------------------------End Comment Module---------------------------------------------------------

		CommentByKeywordManager objCommentByKeywordManager = new CommentByKeywordManager ();
		//--------------------------------------------------------------------------Start Comment By Keyword Module-----------------------------------------------

		public void AccountReportActiveAccount_CommentByKeyword(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_CommentByKeyword.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_CommentByKeyword(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_CommentByKeyword.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_CommentByKeyword()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstCommentAccReport_CommentByKeyword = new List<string>();
				try
				{
					DS = QM.SelectAddReport("CommentByKeyword");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Pin = dt_item.ItemArray[3].ToString();	
						lstCommentAccReport_CommentByKeyword.Add(Pin);
						lstCommentAccReport_CommentByKeyword.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblCommentDone_CommentByKeyword.Text = lstCommentAccReport_CommentByKeyword.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}



		protected void OnChkSingleUserCommentByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_CommentByKeyword.Active == true) 
				{
					btnBrowseKeyword_CommentByKeyword.Visible = false;				
					txtKeyword_CommentByKeyword.Sensitive = true;			
					lblKeyword_CommentByKeywor.Text = "Enter Keyword : ";
				}	
				else 
				{
					btnBrowseKeyword_CommentByKeyword.Visible = true;
					txtKeyword_CommentByKeyword.Sensitive = false;
					lblKeyword_CommentByKeywor.Text = "          Keyword :  ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnBrowseKeywordCommentByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseCommentByKeyword();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void BrowseCommentByKeyword()
		{
			try
			{
				ClGlobul.lstMessageKeyword.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtKeyword_CommentByKeyword.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstMessageKeyword = GlobusFileHelper.ReadFile(txtKeyword_CommentByKeyword.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Keyword Uploaded :" + ClGlobul.lstMessageKeyword.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartCommentByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				StartProcessCommentByKeyword();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void StartProcessCommentByKeyword()
		{
			try
			{
				string singleUser = String.Empty;
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (chkSingleUser_Comment.Active == true) 
						{
							ClGlobul.lstMessageKeyword.Clear();
							if (string.IsNullOrEmpty(txtKeyword_CommentByKeyword.Text))
							{
								Addtologger("Please Enter Keyword");									
								return;
							}
							else
							{
								singleUser=txtKeyword_CommentByKeyword.Text.Trim();
								ClGlobul.lstMessageKeyword.Add(singleUser);
								Addtologger("[ " + DateTime.Now + "] => [ Keyword Enter :" + ClGlobul.lstMessageKeyword.Count);	
							}
						}
						if (string.IsNullOrEmpty(txtKeyword_CommentByKeyword.Text))
						{
							Addtologger("[ " + DateTime.Now + "] => [ Please Upload Keyword  ]");									
							return;
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine (ex.StackTrace);

					}

						objCommentByKeywordManager.isStopCommentByKeyword = false;
						objCommentByKeywordManager.lstThreadsCommentByKeyword.Clear();

						if (objCommentByKeywordManager._IsfevoriteCommentByKeyword)
						{
							objCommentByKeywordManager._IsfevoriteCommentByKeyword = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
							objCommentByKeywordManager.minDelayCommentByKeyword = Convert.ToInt32(txtMinDelay_CommentByKeyword.Text);
							objCommentByKeywordManager.maxDelayCommentByKeyword = Convert.ToInt32(txtMaxDelay_CommentByKeyword.Text);
							objCommentByKeywordManager.Nothread_CommentByKeyword = Convert.ToInt32(txtThread_CommentByKeyword.Text);
							objCommentByKeywordManager.MaxCommentByKeyword = Convert.ToInt32(txtCount_CommentByKeyword.Text);
							}
							catch (Exception ex)
							{
							  	Console.Write (ex.Message);
								return;
							}

						if (!string.IsNullOrEmpty(txtThread_CommentByKeyword.Text) && checkNo.IsMatch(txtThread_CommentByKeyword.Text))
							{
							threads = Convert.ToInt32(txtThread_CommentByKeyword.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");

							objCommentByKeywordManager.NoOfThreadsCommentByKeyword = threads;

							Thread CommentKeywordThread = new Thread(objCommentByKeywordManager.StartCommentKeyword);
							CommentKeywordThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write (ex.Message);
					    }
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [Please Load Accounts ! ]");
				}
			}

			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopCommentByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objCommentByKeyword = new Thread(stopCommentByKeyword);
				objCommentByKeyword.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopCommentByKeyword()
		{
			try
			{
				objCommentByKeywordManager._IsfevoriteCommentByKeyword = true;
				List<Thread> lstTemp = objCommentByKeywordManager.lstThreadsCommentByKeyword.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("-----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//------------------------------------------------------------------End Comment By Keyword Module-------------------------------------------------------

		LikeManagers objLikeManagers = new LikeManagers ();

		//----------------------------------------------------------------------Start Like Module---------------------------------------------------------------

		public void AccountReportActiveAccount_Like(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_Like.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_Like(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_Like.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_Like()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstPinAccReport_Like = new List<string>();
				try
				{
					DS = QM.SelectAddReport("Like");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Pin = dt_item.ItemArray[3].ToString();	
						lstPinAccReport_Like.Add(Pin);
						lstPinAccReport_Like.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblUserUnFollow_Like.Text = lstPinAccReport_Like.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}



		protected void OnRboNormalLikePinUrlsClicked (object sender, EventArgs e)
		{
			try
			{
				LikeManagers.rbNormalLikePinUrls = true;
				LikeManagers.rbListLikePinUrls = false;
				lblPinNo_Like.Visible = false;
				txtLikePinUrl.Visible = false;
				btnPinUrls_Like_Browse.Visible = false;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

	
		protected void OnRboListLikePinUrlsClicked (object sender, EventArgs e)
		{
			try
			{
				LikeManagers.rbNormalLikePinUrls = false;
				LikeManagers.rbListLikePinUrls = true;
				lblPinNo_Like.Visible = true;
				txtLikePinUrl.Visible = true;
				btnPinUrls_Like_Browse.Visible = true;
				txtLikePinUrl.Sensitive = false;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnPinUrlsLikeBrowseClicked (object sender, EventArgs e)
		{
			try
			{
				BrowsePinUrlLike();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowsePinUrlLike()
		{
			try
			{
				ClGlobul.lstAddToBoardUserNames.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtLikePinUrl.Text = objFileChooser.Filename.ToString();
					Thread ReadLargeFileThread = new Thread(ReadLargeLikePinUrlsFile);
					ReadLargeFileThread.Start(objFileChooser.Filename.ToString());
					ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtLikePinUrl.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Keyword Uploaded :" + ClGlobul.lstAddToBoardUserNames.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		List<Thread> lstReLikeThread = new List<Thread>();
		private void ReadLargeLikePinUrlsFile(object filePath)
		{
			try
			{
				lstReLikeThread.Add(Thread.CurrentThread);
				lstReLikeThread.Distinct().ToList();
				Thread.CurrentThread.IsBackground = true;

				List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
				new Thread(() =>
					{
						AddLikePinUrlsList(AccountsList);
					}).Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void AddLikePinUrlsList(List<string> Messages)
		{
			try
			{
				ClGlobul.lstLikePinUrls.Clear();

				foreach (string Message in Messages)
				{
					string NewMessages = Message.Replace("\0", "").Trim();
					if (!ClGlobul.lstLikePinUrls.Contains(NewMessages))
					{
						if (!string.IsNullOrEmpty(NewMessages))
						{
							if (NewMessages.Length > 400)
							{
								NewMessages = NewMessages.Substring(0, 400);
							}
							ClGlobul.lstLikePinUrls.Add(NewMessages.Replace("\0", ""));
						}
					}
				}
					
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartLikeClicked (object sender, EventArgs e)
		{
			try
			{
				StartLikeProcess();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void StartLikeProcess()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					objLikeManagers.isStopLike = false;
					objLikeManagers.lstThreadsLike.Clear();

					if (objLikeManagers._IsfevoriteLike)
					{
						objLikeManagers._IsfevoriteLike = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							LikeManagers.minDelayLike = Convert.ToInt32(txtMinDelay_Like.Text);
							LikeManagers.maxDelayLike = Convert.ToInt32(txtMaxDelay_Like.Text);
							objLikeManagers.NoOfThreadsLike = Convert.ToInt32(txtThread_Like.Text);
							LikeManagers.MaxLike = Convert.ToInt32(txtCount_Like.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_Like.Text) && checkNo.IsMatch(txtThread_Like.Text))
						{
							threads = Convert.ToInt32(txtThread_Like.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
						objLikeManagers.NoOfThreadsLike = threads;

						Thread LikeThread = new Thread(objLikeManagers.StartLike);
						LikeThread.Start();
					}

					catch (Exception ex)
					{
						//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				//GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
				Console.Write(ex.Message);
			}
		}

		protected void OnBtnStopLikeClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopLike = new Thread(stopLike);
				objStopLike.Start();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		public void stopLike()
		{
			try
			{
				objLikeManagers._IsfevoriteLike = true;
				List<Thread> lstTempLike = objLikeManagers.lstThreadsLike.Distinct().ToList();
				foreach (Thread item in lstTempLike)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		//----------------------------------------------------------------------End of Like Module---------------------------------------------------------

		LikeByKeywordManager objLikeByKeywordManager = new LikeByKeywordManager ();

		//-----------------------------------------------------------------Start Like By Keyword Module---------------------------------------------------


		public void AccountReportActiveAccount_LikeByKeyword(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_LikeByKeyword.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_LikeByKeyword(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_LikeByKeyword.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_LikeByKeyword()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstPinAccReport_LikeByKeyword = new List<string>();
				try
				{
					DS = QM.SelectAddReport("LikeByKeyword");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Pin = dt_item.ItemArray[3].ToString();	
						lstPinAccReport_LikeByKeyword.Add(Pin);
						lstPinAccReport_LikeByKeyword.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblUserUnFollow_LikeByKeyword.Text = lstPinAccReport_LikeByKeyword.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}



		protected void OnChkSingleUserLikeByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_LikeByKeyword.Active == true) 
				{
					btnKeyword_LikeByKeyword_Browse.Visible = false;				
					txt_KeywordLike.Sensitive = true;			
					lblKeyword_LikeByKeyword.Text = "Enter Keyword : ";
				}	
				else 
				{
					btnKeyword_LikeByKeyword_Browse.Visible = true;
					txt_KeywordLike.Sensitive = false;
					lblKeyword_LikeByKeyword.Text = "          Keyword :  ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnKeywordLikeByKeywordBrowseClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseKeywordLikeByKeyword();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseKeywordLikeByKeyword()
		{
			try
			{
				ClGlobul.lstLikeByKeyword.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txt_KeywordLike.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstLikeByKeyword = GlobusFileHelper.ReadFile(txt_KeywordLike.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Keyword Uploaded :" + ClGlobul.lstLikeByKeyword.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		protected void OnBtnStartLikeByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				startLikeByKeyword();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}


		public void startLikeByKeyword()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					string SingleUser_LikeByKeyword = string.Empty;
					try
					{
						try
						{
							if (chkSingleUser_LikeByKeyword.Active == true) 
							{
								ClGlobul.lstLikeByKeyword.Clear();
								if (string.IsNullOrEmpty(txt_KeywordLike.Text))
								{
									GlobusLogHelper.log.Info("Please Enter Keyword");									
								    return;
								}
								else
								{
									SingleUser_LikeByKeyword=txt_KeywordLike.Text.Trim();
									ClGlobul.lstLikeByKeyword.Add(SingleUser_LikeByKeyword);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Keyword Enter :" + ClGlobul.lstLikeByKeyword.Count);	
								}
							}
								if (string.IsNullOrEmpty(txt_KeywordLike.Text))
							{
									GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Upload Keyword  ]");									
								return;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.StackTrace);

						}

						objLikeByKeywordManager.isStopLikeByKeyword = false;
						objLikeByKeywordManager.lstThreadsLikeByKeyword.Clear();

						if (objLikeByKeywordManager._IsfevoriteLikeByKeyword)
						{
							objLikeByKeywordManager._IsfevoriteLikeByKeyword = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objLikeByKeywordManager.minDelayLikeByKeyword = Convert.ToInt32(txtMinDelay_LikeByKeyword.Text);
								objLikeByKeywordManager.maxDelayLikeByKeyword = Convert.ToInt32(txtMaxDelay_LikeByKeyword.Text);
								objLikeByKeywordManager.Nothread_LikeByKeyword = Convert.ToInt32(txtThread_LikeByKeyword.Text);
								objLikeByKeywordManager.MaxLikeByKeyword = Convert.ToInt32(txtCount_LikeByKeyword.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Enter in Correct Format ]");
								return;
							}

							if (!string.IsNullOrEmpty(txtThread_LikeByKeyword.Text) && checkNo.IsMatch(txtThread_LikeByKeyword.Text))
							{
								threads = Convert.ToInt32(txtThread_LikeByKeyword.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");

							objLikeByKeywordManager.NoOfThreadsLikeByKeyword = threads;

							Thread CommentKeywordThread = new Thread(objLikeByKeywordManager.StartLikeKeyword);
							CommentKeywordThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write(ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts ! ]");
				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopLikeByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStop = new Thread(stopLikeByKeyword);
				objStop.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopLikeByKeyword()
		{
			try
			{
				objLikeByKeywordManager.isStopLikeByKeyword = true;
				List<Thread> lstTempLikeByKeyword = objLikeByKeywordManager.lstThreadsLikeByKeyword.Distinct().ToList();
				foreach (Thread item in lstTempLikeByKeyword)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//--------------------------------------------------------------------End LikeBy Keyword Module--------------------------------------------------
	
		ScraperManager objScraperManager = new ScraperManager ();

		//-------------------------------------------------------------------Start User Scraper Module---------------------------------------------------

		protected void OnRdoFollowerScraperClicked (object sender, EventArgs e)
		{
			try
			{
				objScraperManager.UserScraperType = "followers";
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnRdoFollowingScraperClicked (object sender, EventArgs e)
		{
			try
			{
				objScraperManager.UserScraperType = "following";
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
	
		protected void OnBtnStartUserScraperClicked (object sender, EventArgs e)
		{
			try
			{
				startScraper();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		public void startScraper()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (string.IsNullOrEmpty(txtScraperUsername.Text))
						{
							GlobusLogHelper.log.Info("Please Upload Username ");						
							return;
						}
						else
						{
							objScraperManager.UserName = txtScraperUsername.Text.Trim();
						}

						ClGlobul.lstTotalUserScraped.Clear();
						objScraperManager.isStopScraper = false;
						objScraperManager.lstThreadsScraper.Clear();

						if (objScraperManager._IsfevoriteScraper)
						{
							objScraperManager._IsfevoriteScraper = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objScraperManager.minDelayScraper = Convert.ToInt32(txtMinDelay_UserScraper.Text);
								objScraperManager.maxDelayScraper = Convert.ToInt32(txtMaxDelay_UserScraper.Text);
								objScraperManager.Nothread_Scraper = Convert.ToInt32(txtThread_UserScraper.Text);
								objScraperManager.MaxCountScraper = Convert.ToInt32(txtCount_UserScraper.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Enter in Correct Format ]");
								return;
							}

							if (!string.IsNullOrEmpty(txtThread_UserScraper.Text) && checkNo.IsMatch(txtThread_UserScraper.Text))
							{
								threads = Convert.ToInt32(txtThread_UserScraper.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
							objScraperManager.NoOfThreadsScraper = threads;

							Thread ScraperThread = new Thread(objScraperManager.StartScraper);
							ScraperThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write (ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts ! ]");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnChkExportScrapeDataScraperClicked (object sender, EventArgs e)
		{
			try
			{
				ExportDataUserScraper();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void ExportDataUserScraper()
		{
			try
			{
				ClGlobul.lstTotalUserScraped = ClGlobul.lstTotalUserScraped.Distinct().ToList();
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Start User Export Process ]");
				try
				{
					if (rdoFollowerScraper.Active == true)
					{
						string item = "Followers";
						// lstTotalUserScraped.Insert(0, item);
						GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollower);
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ File Path " + PDGlobals.UserScrapedFollower + " ]");
					}
					else
					{
						string item = "Followings";
						//  lstTotalUserScraped.Insert(0, item);
						GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollowing);
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ File Path " + PDGlobals.UserScrapedFollowing + " ]");
					}
				}
				catch { }


				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ User Exported Successfully ]");

			}
			catch (Exception ex)
			{
				GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
			}
		}
			
		protected void OnBtnStopUserScraperClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopScraper = new Thread(stopScraper);
				objStopScraper.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopScraper()
		{
			try
			{
				objScraperManager._IsfevoriteScraper = true;
				List<Thread> lstTemp = objScraperManager.lstThreadsScraper.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//------------------------------------------------------------------End User Scraper Module----------------------------------------------------

		ScrapePinManagers objScrapePinManagers = new ScrapePinManagers ();

		//------------------------------------------------------------------Start PinScraper Module-----------------------------------------------------

		protected void OnChkScrapeImagePinScraperClicked (object sender, EventArgs e)
		{
			try
			{
				ScrapePinManagers.chkScrapeImage = true;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartPinScraperClicked (object sender, EventArgs e)
		{
			try
			{
				startPinScraper();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		public void startPinScraper()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (string.IsNullOrEmpty(txt_BoardUrl_pinscraper.Text))
						{
							GlobusLogHelper.log.Info("Please Enter BoardUrl ");
							return;
						}
						else
						{
							objScrapePinManagers.BoardUrl = (txt_BoardUrl_pinscraper.Text).ToString();
						}

						objScrapePinManagers.isStopPinScraper = false;
						objScrapePinManagers.lstThreadsPinScraper.Clear();

						if (objScrapePinManagers._IsfevoritePinScraper)
						{
							objScrapePinManagers._IsfevoritePinScraper = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objScrapePinManagers.minDelayPinScraper = Convert.ToInt32(txtMinDelay_PinScraper.Text);
								objScrapePinManagers.maxDelayPinScraper = Convert.ToInt32(txtMaxDelay_PinScraper.Text);
								objScrapePinManagers.NoOfThreadsPinScraper = Convert.ToInt32(txtThread_PinScraper.Text);
								objScrapePinManagers.MaxNoOfPinScrape = Convert.ToInt32(txtCount_PinScraper.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("Enter in Correct Format");
								return;
							}

							if (!string.IsNullOrEmpty(txtThread_PinScraper.Text) && checkNo.IsMatch(txtThread_PinScraper.Text))
							{
								threads = Convert.ToInt32(txtThread_PinScraper.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
							objScrapePinManagers.NoOfThreadsPinScraper = threads;

							Thread ScraperPinThread = new Thread(objScrapePinManagers.StartPinScraper);
							ScraperPinThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write (ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnChkExportDataPinScraperClicked (object sender, EventArgs e)
		{
			try
			{
				ClGlobul.GetPinList = ClGlobul.GetPinList.Distinct().ToList();
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Pin Export Process Started]");
				try
				{
					GlobusFileHelper.WriteListtoTextfile(ClGlobul.GetPinList, PDGlobals.PinScraped);
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ File Path " + PDGlobals.PinScraped + " ]");

				}
				catch { }

				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Pins Exported Successfully ]");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopPinScraperClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopPinScraper = new Thread(stopPinScraper);
				objStopPinScraper.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopPinScraper()
		{
			try
			{
				objScrapePinManagers._IsfevoritePinScraper = true;
				List<Thread> lstTemp = objScrapePinManagers.lstThreadsPinScraper.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//--------------------------------------------------------------------------End Pin Scraper Module---------------------------------------------------

		FollowByUsernameManager objFollowByUsernameManager = new FollowByUsernameManager ();

		//------------------------------------------------------------------------Start Follow By Username--------------------------------------------------


		public void AccountReportActiveAccount_FollowByUsername(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_FollowByUsername.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_FollowByUsername(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_FollowByUsername.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_FollowByUsername()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Username = string.Empty;
				List<string> lstUsernameAccReport_FollowByUsername = new List<string>();
				try
				{
					DS = QM.SelectAddReport("FollowByUsername");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Username = dt_item.ItemArray[5].ToString();	
						lstUsernameAccReport_FollowByUsername.Add(Username);
						lstUsernameAccReport_FollowByUsername.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblFollowedUser_FollowByUsername.Text = lstUsernameAccReport_FollowByUsername.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}




		protected void OnChkSingleUserFollowByUsernameClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_FollowByUsername.Active == true) 
				{
					txtUserName_FollowByUsername.Sensitive = true;
					txtFollowUser_FollowByUsername.Sensitive = true;
					btnBrowseUsername_FollowByUserName.Visible = false;
					btnBrowseFollowUser_FollowByUsername.Visible = false;
					lblUserName_FollowByUsername.Text = "     Enter UserName : ";
					lblFollowUsers_FollowByUsername.Text = " Enter Follow Users : ";
				}	
				else 
				{
					btnKeyword_LikeByKeyword_Browse.Visible = true;
					txt_KeywordLike.Sensitive = false;
					btnBrowseUsername_FollowByUserName.Visible = true;
					btnBrowseFollowUser_FollowByUsername.Visible = true;
					lblUserName_FollowByUsername.Text = "               UserName :  ";
					lblFollowUsers_FollowByUsername.Text = "           Follow Users :  ";
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
				
//		protected void OnRdoFollowUploadUserActivated (object sender, EventArgs e)
//		{
//			
//		}

		protected void OnRdoFollowUploadUserClicked (object sender, EventArgs e)
		{
			try
			{
				FollowByUsernameManager.rbFollowUser = true;
				FollowByUsernameManager.rbFollowFollowers = false;
				if (chkSingleUser_FollowByUsername.Active == true) 
				{
					txtUserName_FollowByUsername.Visible = false;
					txtFollowUser_FollowByUsername.Sensitive = true;
					btnBrowseUsername_FollowByUserName.Visible = false;
					btnBrowseFollowUser_FollowByUsername.Visible = false;
					txtFollowUser_FollowByUsername.Visible = true;
					lblUserName_FollowByUsername.Visible = false;
					lblFollowUsers_FollowByUsername.Visible = true;

				}
				else
				{
					txtUserName_FollowByUsername.Visible = false;
					txtFollowUser_FollowByUsername.Sensitive = false;
					btnBrowseUsername_FollowByUserName.Visible = false;
					btnBrowseFollowUser_FollowByUsername.Visible = true;
					txtFollowUser_FollowByUsername.Visible = true;
					lblUserName_FollowByUsername.Visible = false;
					lblFollowUsers_FollowByUsername.Visible = true;
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnRdbFollowUserFollowerFollowByUsernameClicked (object sender, EventArgs e)
		{
			try
			{
				FollowByUsernameManager.rbFollowUser = false;
				FollowByUsernameManager.rbFollowFollowers = true;
				if (chkSingleUser_FollowByUsername.Active == true) 
				{
					txtUserName_FollowByUsername.Visible = true;
					txtUserName_FollowByUsername.Sensitive = true;
					txtFollowUser_FollowByUsername.Sensitive = false;
					btnBrowseUsername_FollowByUserName.Visible = false;
					btnBrowseFollowUser_FollowByUsername.Visible = false;
					txtFollowUser_FollowByUsername.Visible = false;
					lblFollowUsers_FollowByUsername.Visible = false;
					lblUserName_FollowByUsername.Visible = true;

				}
				else
				{
					txtUserName_FollowByUsername.Visible = false;
					txtUserName_FollowByUsername.Sensitive = false;
					txtUserName_FollowByUsername.Visible = true;
					txtFollowUser_FollowByUsername.Sensitive = false;
					btnBrowseUsername_FollowByUserName.Visible = true;
					btnBrowseFollowUser_FollowByUsername.Visible = false;
					txtFollowUser_FollowByUsername.Visible = false;
					lblFollowUsers_FollowByUsername.Visible = false;
					lblUserName_FollowByUsername.Visible = true;
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnBrowseUsernameFollowByUserNameClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseUsernameFollowByUSername();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
		private void BrowseUsernameFollowByUSername()
		{
			try
			{
				ClGlobul.ListOfFollowUsersFollowers.Clear();
				ClGlobul.FollowUsersFollowerQueue.Clear(); 
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtUserName_FollowByUsername.Text = objFileChooser.Filename.ToString();
					ClGlobul.ListOfFollowUsersFollowers = GlobusFileHelper.ReadFile(txtUserName_FollowByUsername.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Username Uploaded :" + ClGlobul.ListOfFollowUsersFollowers.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		protected void OnBtnBrowseFollowUserFollowByUsernameClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseFollowUserFollowByUSername();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseFollowUserFollowByUSername()
		{
			try
			{
				ClGlobul.lstFollowUsername.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtFollowUser_FollowByUsername.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstFollowUsername = GlobusFileHelper.ReadFile(txtFollowUser_FollowByUsername.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Follow Users Uploaded :" + ClGlobul.lstFollowUsername.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		protected void OnBtnStartFollowByUsernameClicked (object sender, EventArgs e)
		{
			try
			{
				
				startFollowByUsername();

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}


		public void startFollowByUsername()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						string SingleUserFollowByUSername = string.Empty;
						
						try
						{
							if (rdoFollowUploadUser.Active == true || rdbFollowUserFollower_FollowByUsername.Active == true)
							{
								if (rdbFollowUserFollower_FollowByUsername.Active == true)
								 {
									if (chkSingleUser_FollowByUsername.Active == true) 
									{
										if (!string.IsNullOrEmpty(txtUserName_FollowByUsername.Text)) 
										{											
											SingleUserFollowByUSername=txtUserName_FollowByUsername.Text.Trim();
											ClGlobul.ListOfFollowUsersFollowers.Add(SingleUserFollowByUSername);
											foreach (var itemListOfFollowUsersFollowers in ClGlobul.ListOfFollowUsersFollowers)
											{
												objFollowByUsernameManager.FollowUsersFollowerQueue.Enqueue(itemListOfFollowUsersFollowers);
											}
											Addtologger("[ " + DateTime.Now + "] => [ Total Username Enter :" + ClGlobul.ListOfFollowUsersFollowers.Count + " ]");	
										}

										if (string.IsNullOrEmpty(txtUserName_FollowByUsername.Text))
										{
											GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Enter Username ]");
											return;
										}
									}
									else 
									{

										if (string.IsNullOrEmpty(txtUserName_FollowByUsername.Text))
										 {
											GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Upload Username ]");
												return;
										 }
										 else
									     {
											 foreach (var itemListOfFollowUsersFollowers in ClGlobul.ListOfFollowUsersFollowers)
											 {
													objFollowByUsernameManager.FollowUsersFollowerQueue.Enqueue(itemListOfFollowUsersFollowers);
											 }
										 }
									}

									}

								if (rdoFollowUploadUser.Active == true)
									{

									if (chkSingleUser_FollowByUsername.Active == true) 
									{
										if (!string.IsNullOrEmpty(txtFollowUser_FollowByUsername.Text)) 
										{											
											SingleUserFollowByUSername=txtFollowUser_FollowByUsername.Text.Trim();
											ClGlobul.lstFollowUsername.Add(SingleUserFollowByUSername);
											Addtologger("[ " + DateTime.Now + "] => [ Total Follow Users Enter :" + ClGlobul.lstFollowUsername.Count + " ]");	
										}

										if (string.IsNullOrEmpty(txtFollowUser_FollowByUsername.Text))
										{
											GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Enter Follow Users ]");
											return;
										}
									}
									if (string.IsNullOrEmpty(txtFollowUser_FollowByUsername.Text))
										{
										GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Upload Follow User ]");													
												return;
										}

									}
								}
								else
								{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Select Follow Upload User or Follow users Follower ]");
									return;
								}
							}
							catch (Exception ex)
							{
							Console.Write(ex.Message);
							}
		
						objFollowByUsernameManager.isStopFollowByUsername = false;
						objFollowByUsernameManager.lstThreadsFollowByUsername.Clear();

						if (objFollowByUsernameManager._IsfevoriteFollowByUsername)
						{
							objFollowByUsernameManager._IsfevoriteFollowByUsername = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objFollowByUsernameManager.minDelayFollowByUsername = Convert.ToInt32(txtMinDelay_FollowByUsername.Text);
								objFollowByUsernameManager.maxDelayFollowByUsername = Convert.ToInt32(txtMaxDelay_FollowByUsername.Text);
								objFollowByUsernameManager.Nothread_FollowByUsername = Convert.ToInt32(txtThread_FollowByUsername.Text);
								objFollowByUsernameManager.MaxFollowCount = Convert.ToInt32(txtCount_FollowByUsername.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("Enter in Correct Format");
								return;
							}

							QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtFollowUser_FollowByUsername.Text));

							if (!string.IsNullOrEmpty(txtThread_FollowByUsername.Text) && checkNo.IsMatch(txtThread_FollowByUsername.Text))
							{
								threads = Convert.ToInt32(txtThread_FollowByUsername.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}

							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");

							objFollowByUsernameManager.NoOfThreadsFollowByUsername = threads;

							Thread FollowUsernameThread = new Thread(objFollowByUsernameManager.StartFollowByUsername);
							FollowUsernameThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write (ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] = > [ Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopFollowByUsernameClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopFollowByUsername = new Thread(stopFollowByUsername);
				objStopFollowByUsername.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopFollowByUsername()
		{
			try
			{
				objFollowByUsernameManager._IsfevoriteFollowByUsername = true;
				List<Thread> lstTempFollowByUsername = objFollowByUsernameManager.lstThreadsFollowByUsername.Distinct().ToList();
				foreach (Thread item in lstTempFollowByUsername)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//-----------------------------------------------------------------------End Follow By UserName Module---------------------------------------------------------

		FollowByKeywordManager objFollowByKeywordManager = new FollowByKeywordManager ();

		//----------------------------------------------------------------------Start Follow By Keyword Module---------------------------------------------------------


		public void AccountReportActiveAccount_FollowByKeyword(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_FollowByKeyword.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_FollowByKeyword(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_FollowByKeyword.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_FollowByKeyword()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Username = string.Empty;
				List<string> lstUsernameAccReport_FollowByKeyword = new List<string>();
				try
				{
					DS = QM.SelectAddReport("FollowByKeyword");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Username = dt_item.ItemArray[5].ToString();	
						lstUsernameAccReport_FollowByKeyword.Add(Username);
						lstUsernameAccReport_FollowByKeyword.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblFollowedUser_FollowByKeyword.Text = lstUsernameAccReport_FollowByKeyword.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}




		protected void OnChkSingleUserFollowByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_FollowByKeyword.Active == true) 
				{
					btnBrowseKeyword_FollowByKeyword.Visible = false;				
					txtKeyword_FollowByKeyword.Sensitive = true;			
					lblKeyword_FollowByKeyword.Text = "  Enter Keyword : ";
				}	
				else 
				{
					btnBrowseKeyword_FollowByKeyword.Visible = true;
					txtKeyword_FollowByKeyword.Sensitive = false;
					lblKeyword_FollowByKeyword.Text = "            Keyword :  ";
				}
			}
			catch(Exception Ex) 
			{
				Console.Write (Ex.Message);
			}
		}

		protected void OnBtnBrowseKeywordFollowByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseKeywordFollowByKeyword();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseKeywordFollowByKeyword()
		{
			try
			{
				ClGlobul.lstkeyword.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtKeyword_FollowByKeyword.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstkeyword = GlobusFileHelper.ReadFile(txtKeyword_FollowByKeyword.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Keyword Uploaded :" + ClGlobul.lstkeyword.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnBtnStartFollowByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				startFollowByKeyword();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void startFollowByKeyword()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{	
						string SingleUser_FollowByKeyword = string.Empty;
						try
						{
							if (chkSingleUser_FollowByKeyword.Active == true) 
							{
								ClGlobul.lstLikeByKeyword.Clear();
								if (string.IsNullOrEmpty(txtKeyword_FollowByKeyword.Text))
								{
									GlobusLogHelper.log.Info("Please Enter Keyword");									
									return;
								}
								else
								{
									SingleUser_FollowByKeyword=txtKeyword_FollowByKeyword.Text.Trim();
									ClGlobul.lstkeyword.Add(SingleUser_FollowByKeyword);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Keyword Enter :" + ClGlobul.lstkeyword.Count);	
								}
							}
							if (string.IsNullOrEmpty(txtKeyword_FollowByKeyword.Text))
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Please Upload Keyword  ]");									
								return;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.StackTrace);

						}
						objFollowByKeywordManager.isStopFollowByKeyword = false;
						objFollowByKeywordManager.lstThreadsFollowByKeyword.Clear();

						if (objFollowByKeywordManager._IsfevoriteFollowByKeyword)
						{
							objFollowByKeywordManager._IsfevoriteFollowByKeyword = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objFollowByKeywordManager.minDelayFollowByKeyword = Convert.ToInt32(txtMinDelay_FollowByKeyword.Text);
								objFollowByKeywordManager.maxDelayFollowByKeyword = Convert.ToInt32(txtMaxDelay_FollowByKeyword.Text);
								objFollowByKeywordManager.Nothread_FollowByKeyword = Convert.ToInt32(txtThread_FollowByKeyword.Text);
								objFollowByKeywordManager.NoOfUserFollowByKeyword = Convert.ToInt32(txtCount_FollowByKeyword.Text);
								objFollowByKeywordManager.AccPerDayUserFollowByKeyword = Convert.ToInt32(txtBoxHours_FollowByKeyword.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("Enter in Correct Format");
								return;
							}

							QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtKeyword_FollowByKeyword.Text));
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Starting Follow Through Keyword ]");

							if (!string.IsNullOrEmpty(txtThread_FollowByKeyword.Text) && checkNo.IsMatch(txtThread_FollowByKeyword.Text))
							{
								threads = Convert.ToInt32(txtThread_FollowByKeyword.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}

							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
							objFollowByKeywordManager.NoOfThreadsFollowByKeyword = threads;

							Thread FollowKeywordThread = new Thread(objFollowByKeywordManager.StartFollowKeyword);
							FollowKeywordThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write(ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopFollowByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objstopFollowByKeyword = new Thread(stopFollowByKeyword);
				objstopFollowByKeyword.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopFollowByKeyword()
		{
			try
			{
				objFollowByKeywordManager._IsfevoriteFollowByKeyword = true;
				List<Thread> lstTemp = objFollowByKeywordManager.lstThreadsFollowByKeyword.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//-----------------------------------------------------------------------------End Follow By Keyword Module-----------------------------------------------------------

		UnFollowManager objUnFollowManager = new UnFollowManager ();

		//-------------------------------------------------------------------------------Start UnFollow Module--------------------------------------------------------------

		public void AccountReportActiveAccount_UnFollow(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_UnFollow.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_UnFollow(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_UnFollow.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_UnFollow()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Username = string.Empty;
				List<string> lstUsernameAccReport_UnFollow = new List<string>();
				try
				{
					DS = QM.SelectAddReport("UnFollow");
				}
				catch (Exception ex)
				{
					GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				}

				foreach (DataRow dt_item in DS.Tables[0].Rows)
				{
					try
					{
						Username = dt_item.ItemArray[5].ToString();	
						lstUsernameAccReport_UnFollow.Add(Username);
						lstUsernameAccReport_UnFollow.Distinct().ToList();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblUserUnFollow_UnFollow.Text = lstUsernameAccReport_UnFollow.Count().ToString();
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartUnFollowClicked (object sender, EventArgs e)
		{
			try
			{
				startUnFollow();
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void startUnFollow()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						objUnFollowManager.isStopUnFollow = false;
						objUnFollowManager.lstThreadsUnFollow.Clear();

						if (objUnFollowManager._IsfevoriteUnFollow)
						{
							objUnFollowManager._IsfevoriteUnFollow = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objUnFollowManager.minDelayUnFollow = Convert.ToInt32(txtMinDelay_UnFollow.Text);
								objUnFollowManager.maxDelayUnFollow = Convert.ToInt32(txtMaxDelay_UnFollow.Text);
								objUnFollowManager.Nothread_UnFollow = Convert.ToInt32(txtThread_UnFollow.Text);
								objUnFollowManager.MaxUnFollowCount = Convert.ToInt32(txtCount_UnFollow.Text);
								objUnFollowManager.NOofDays = Convert.ToInt32(txtUnfollowDays.Text);
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("Enter in Correct Format");
								return;
							}

							if (!string.IsNullOrEmpty(txtThread_UnFollow.Text) && checkNo.IsMatch(txtThread_UnFollow.Text))
							{
								threads = Convert.ToInt32(txtThread_UnFollow.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Process Starting ] ");

							objUnFollowManager.NoOfThreadsUnFollow = threads;

							Thread UnFollowThread = new Thread(objUnFollowManager.StartUnFollow);
							UnFollowThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write(ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		protected void OnChkboxUnfollowNoOFDaysClicked (object sender, EventArgs e)
		{
			try
			{
				objUnFollowManager.chkNoOFDays_UnFollow = true;
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnChkUploadUnFollowListClicked (object sender, EventArgs e)
		{
			UnFollowManager.chkUploadUnFollowList = true;
			frmUploadUnFollowList objfrmUploadUnFollowList = new frmUploadUnFollowList ();
			objfrmUploadUnFollowList.Show ();
		}

		protected void OnBtnStopUnFollowClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopUnFollow = new Thread(stopUnFollow);
				objStopUnFollow.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopUnFollow()
		{
			try
			{
				objUnFollowManager._IsfevoriteUnFollow = true;
				List<Thread> lstTempUnFollow= objUnFollowManager.lstThreadsUnFollow.Distinct().ToList();
				foreach (Thread item in lstTempUnFollow)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//------------------------------------------------------------------End UnFollow Module------------------------------------------------------------

		AccountChecker objAccountChecker = new AccountChecker();

		//-------------------------------------------------------------Start Account Checker Module---------------------------------------------------------

		public void AccountReportNoOfAccount_AccChecker(int Count)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						labelNoOfAcc_AccountChecker.Text = Count.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportNoOfActiveAccount(int Count)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						labelNoOfActiveAcc_AccChecker.Text = Count.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				Console.Write (ex.Message);
			}

		}

		public void AccountReportNoOfDeadAccount(int Count)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						labelNoOfDeadAcc_AccChecker.Text = Count.ToString();
					});
			}
			catch (Exception ex)
			{
				GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

	

		protected void OnRdoBtnCheckAccountFromLoadedAccountClicked (object sender, EventArgs e)
		{
			label150.Visible = false;
			btnBrowseAccounts_AccChecker.Visible = false;
			txtLoadAcc_AccountChecker.Visible = false;
			objAccountChecker.rdCheckAccountFromLoadedAccount = true;
			objAccountChecker.rdCheckAccountFromLoadedFilesAccount=false;
			//btnExpoxtActiveAcc_AccountChecker.Visible = Visibility.Visible;
			//btnExportDeadAcc_AccChecker.Visible = Visibility.Visible;
		}

		protected void OnRdoBtnCheckAccountFromLoadedFilesAccountClicked (object sender, EventArgs e)
		{
			try
			{
				label150.Visible = true;
				btnBrowseAccounts_AccChecker.Visible = true;
				txtLoadAcc_AccountChecker.Visible = true;
				objAccountChecker.rdCheckAccountFromLoadedAccount = false;
				objAccountChecker.rdCheckAccountFromLoadedFilesAccount=true;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnBrowseAccountsAccCheckerClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseAccounts_AccChecker();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseAccounts_AccChecker()
		{
			try
			{
				ClGlobul.lstAccountForAccountChecker.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtLoadAcc_AccountChecker.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstAccountForAccountChecker = GlobusFileHelper.ReadFile(txtLoadAcc_AccountChecker.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Account Uploaded :" + ClGlobul.lstAccountForAccountChecker.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartAccountCheckerClicked (object sender, EventArgs e)
		{
			try
			{
				startAccountChecker();
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void startAccountChecker()
		{
			try
			{
//				if (PDGlobals.listAccounts.Count > 0)
//				{
					try
					{
						if (rdoBtn_CheckAccountFromLoadedAccount.Active == true || rdoBtn_CheckAccountFromLoadedFilesAccount.Active == true)
						{


						}
						else
						{
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Select Check From Loaded Account or Check From User Files ]");
							return;
						}

						objAccountChecker.isStopAccountChecker = false;
						objAccountChecker.lstThreadsAccountChecker.Clear();

						if (objAccountChecker._IsfevoriteAccountChecker)
						{
							objAccountChecker._IsfevoriteAccountChecker = false;
						}

						Regex checkNo = new Regex("^[0-9]*$");

						int processorCount = objUtils.GetProcessor();

						int threads = 25;

						int maxThread = 25 * processorCount;
						try
						{
							try
							{
								objAccountChecker.minDelayAccountChecker = Convert.ToInt32(txtMinDelay_AccChecker.Text);
								objAccountChecker.maxDelayAccountChecker = Convert.ToInt32(txtMaxDelay_AccChecker.Text);
								objAccountChecker.Nothread_AccountChecker = Convert.ToInt32(txtThread_AccChecker.Text);   

							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Info("Enter in Correct Format");
								return;
							}

							if (!string.IsNullOrEmpty(txtThread_AccChecker.Text) && checkNo.IsMatch(txtThread_AccChecker.Text))
							{
								threads = Convert.ToInt32(txtThread_AccChecker.Text);
							}

							if (threads > maxThread)
							{
								threads = 25;
							}

							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
							objAccountChecker.NoOfThreadsAccountChecker = threads;

							Thread FollowKeywordThread = new Thread(objAccountChecker.StartAccountChecker);
							FollowKeywordThread.Start();
						}

						catch (Exception ex)
						{
							Console.Write (ex.Message);
						}
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
//				}
//				else
//				{
//					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts !");
//
//				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnBtnExpoxtActiveAccAccountCheckerClicked (object sender, EventArgs e)
		{
			try
			{
//				Thread objthread = new Thread(()=>ExportActiveAccount());
//				objthread.SetApartmentState(ApartmentState.STA);
//				objthread.Start();
				ExportActiveAccount();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void ExportActiveAccount()
		{
			try
			{
				if (objAccountChecker.lstOfActiveAccount.Count > 0)
				{
					try
					{
						string CSV_Header = string.Join(",", "UserName", "Password", "Niche", "ProxyAddress", "ProxyPort", "ProxyUsername", "ProxyPassword", "Date&Time");
						string CSV_Content = "";
						FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

						if (fileChooser.Run () == (int)ResponseType.Accept) 
						{
							
							try
							{
								string FilePath = string.Empty;
							//	FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
								FilePath=fileChooser.Filename;
								FilePath = FilePath + "\\ActiveAccount.csv";
								GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
								GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
								string ExportDataLocation = FilePath;
								PDGlobals.Pindominator_Folder_Path = FilePath;

								foreach (string item in objAccountChecker.lstOfActiveAccount)
								{
									try
									{
										string[] item_Data = Regex.Split(item, ":");
										string UserName = item_Data[0].ToString();
										string Password = item_Data[1].ToString();
										string Niche = item_Data[2].ToString();
										string ProxyAddress = item_Data[3].ToString();
										string ProxyPort = item_Data[4].ToString();
										string ProxyUsername = item_Data[5].ToString();
										string ProxyPassword = item_Data[6].ToString();
										string DateAndTime = DateTime.Now.ToString();
										CSV_Content = string.Join(",", UserName.Replace("'", ""), Password.Replace("'", ""), Niche.Replace("'", ""), ProxyAddress.Replace("'", ""), ProxyPort.Replace("'", ""), ProxyUsername.Replace("'", ""), ProxyPassword.Replace("'", ""), DateAndTime.Replace("'", "")); //, DateAndTime.Replace("'", ""));

										PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Content, ExportDataLocation);
									}
									catch (Exception ex)
									{
										GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
									}
								}
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
							}

						}
						fileChooser.Destroy();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnExportDeadAccAccCheckerClicked (object sender, EventArgs e)
		{
			try
			{
				ExportDeadAccount();
				//Thread tr = new Thread(()=>ExportDeadAccount());
				//tr.SetApartmentState(ApartmentState.STA);
				//tr.Start();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void ExportDeadAccount()
		{
			try
			{
				if (objAccountChecker.lstOfDeadAcount.Count > 0)
				{
					try
					{
						string CSV_Header = string.Join(",", "UserName", "Password", "Niche", "ProxyAddress", "ProxyPort", "ProxyUsername", "ProxyPassword", "Date&Time");
						string CSV_Content = "";
	
						FileChooserDialog fileChooser = new FileChooserDialog ("Choose Folder to View", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

						if (fileChooser.Run () == (int)ResponseType.Accept) 
						{
							try
							{
								string FilePath = string.Empty;

								//FilePath = Utils.Utils.UploadFolderData(fileChooser.Filename);
								FilePath = fileChooser.Filename;
								FilePath = FilePath + "\\DeadAccount.csv";
								GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
								GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
								string ExportDataLocation = FilePath;
								PDGlobals.Pindominator_Folder_Path = FilePath;

								foreach (string item in objAccountChecker.lstOfDeadAcount)
								{
									try
									{
										string[] item_Data = Regex.Split(item, ":");
										string UserName = item_Data[0].ToString();
										string Password = item_Data[1].ToString();
										string Niche = item_Data[2].ToString();
										string ProxyAddress = item_Data[3].ToString();
										string ProxyPort = item_Data[4].ToString();
										string ProxyUsername = item_Data[5].ToString();
										string ProxyPassword = item_Data[6].ToString();
										string DateAndTime = DateTime.Now.ToString();
										CSV_Content = string.Join(",", UserName.Replace("'", ""), Password.Replace("'", ""), Niche.Replace("'", ""), ProxyAddress.Replace("'", ""), ProxyPort.Replace("'", ""), ProxyUsername.Replace("'", ""), ProxyPassword.Replace("'", ""), DateAndTime.Replace("'", "")); //, DateAndTime.Replace("'", ""));

										PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Content, ExportDataLocation);
									}
									catch (Exception ex)
									{
										GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
									}
								}

						
							}
							catch (Exception ex)
							{
								GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
							}

					
						} 

						fileChooser.Destroy ();
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopAccountCheckerClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objstopAccountChecker = new Thread(stopAccountChecker);
				objstopAccountChecker.Start();               
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopAccountChecker()
		{
			try
			{
				objAccountChecker._IsfevoriteAccountChecker = true;
				List<Thread> lstTemp = objAccountChecker.lstThreadsAccountChecker.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch
					{
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//-----------------------------------------------------End Account Checker Module-----------------------------------------------------------------------------

		PinInterestUser objPinUser = new PinInterestUser();
		//RePinManager objRePinManager = new RePinManager ();

		//------------------------------------------------------Start Repin Module--------------------------------------------------------------------------------------

		protected void OnRboRepinUserRepinClicked (object sender, EventArgs e)
		{
			try
			{
				btnRepinUrlUplaod.Visible = false;
				txtRepinNo_Repin.Visible = false;
				label118.Visible = false;
				RePinManager.rbdRepinUserRepin = true;
				RePinManager.rdbUsePinNo = false;
				txtRepinNo_Repin.Text = string.Empty;
				txtRepinMessage_Repin.Text = string.Empty;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnRdoUsePinNoClicked (object sender, EventArgs e)
		{
			try
			{
				btnRepinUrlUplaod.Visible = true;
				txtRepinNo_Repin.Visible = true;
				label118.Visible = true;
				RePinManager.rbdRepinUserRepin = false;
				RePinManager.rdbUsePinNo = true;
				txtRepinNo_Repin.Text = string.Empty;
				txtRepinMessage_Repin.Text = string.Empty;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnRepinUrlUplaodClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseRepinUrl_Repin();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseRepinUrl_Repin()
		{
			try
			{
				ClGlobul.lstRepinUrl.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtRepinNo_Repin.Text = objFileChooser.Filename.ToString();
					ReadPin(objFileChooser.Filename);
					//ClGlobul.lstAccountForAccountChecker = GlobusFileHelper.ReadFile(txtLoadAcc_AccountChecker.Text.Trim());
					//Addtologger("[ " + DateTime.Now + "] => [ Total Account Uploaded :" + ClGlobul.lstAccountForAccountChecker.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void ReadPin(object FilePath)
		{
			try
			{
				List<string> lstRepinUrlBrowse = new List<string>();
				lstRepinUrlBrowse = GlobusFileHelper.ReadFile(txtRepinNo_Repin.Text.Trim());
				Thread objThreadRepin = new Thread(() => AddlstRepinUrl(lstRepinUrlBrowse));
				objThreadRepin.Start();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void AddlstRepinUrl(List<string> lstRepinUrl1)
		{
			try
			{
				objPinUser.globusHttpHelper = new GlobusHttpHelper();
				foreach (var lstRepinUrl_item in lstRepinUrl1)
				{
					if (lstRepinUrl_item.Contains("https://www.pinterest.com/pin/") || lstRepinUrl_item.Contains("http://www.pinterest.com/pin/"))
					{
						try
						{
							string url = lstRepinUrl_item;
							string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
							if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
							{
								ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);
							}
						}
						catch { };
					}
					else
					{
						try
						{
							ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);

						}
						catch { };
					}
				}

				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Total Urls Uploaded : " + ClGlobul.lstRepinUrl.Count + " ]");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnBrowseMsgRepinClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseMsg_Repin();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseMsg_Repin()
		{
			try
			{
				ClGlobul.RepinMessagesList.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtRepinMessage_Repin.Text = objFileChooser.Filename.ToString();
					ClGlobul.RepinMessagesList = GlobusFileHelper.ReadFile(txtRepinMessage_Repin.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Message Uploaded :" + ClGlobul.RepinMessagesList.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartRepinClicked (object sender, EventArgs e)
		{
			try
			{
				startRePin();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		public void startRePin()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					objRePinManager.isStopRePin = false;
					objRePinManager.lstThreadsRePin.Clear();
					clsSettingDB Database = new clsSettingDB();
					Database.UpdateSettingData("Repin", "RepinMsgFile", StringEncoderDecoder.Encode(txtRepinMessage_Repin.Text));
					Database.UpdateSettingData("Repin", "RepinNO", StringEncoderDecoder.Encode(txtRepinNo_Repin.Text));

					ClGlobul.lstPins.Clear();

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							objRePinManager.minDelayRePin = Convert.ToInt32(txtMinDelay_Repin.Text);
							objRePinManager.maxDelayRePin = Convert.ToInt32(txtMaxDelay_Repin.Text);
							objRePinManager.Nothread_RePin = Convert.ToInt32(txtThread_Repin.Text);
							objRePinManager.maxNoOfRePinCount = Convert.ToInt32(txtCount_Repin.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("Enter in Correct Format");
							return;
						}
						Regex checkNo = new Regex("^[0-9]*$");

						if (!string.IsNullOrEmpty(txtThread_Repin.Text) && checkNo.IsMatch(txtThread_Repin.Text))
						{
							threads = Convert.ToInt32(txtThread_Repin.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
						objRePinManager.NoOfThreadsRePin = threads;

						Thread RePinThread = new Thread(objRePinManager.StartRepin);
						RePinThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => Please Load Accounts ! ]");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");
				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}    

		protected void OnBtnStopRepinClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopRepin = new Thread(stopRepin);
				objStopRepin.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopRepin()
		{
			try
			{
				objRePinManager._IsfevoriteRepin = true;
				List<Thread> lstTempRePin = objRePinManager.lstThreadsRePin.Distinct().ToList();
				foreach (Thread item in lstTempRePin)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//----------------------------------------------------------End Repin Module------------------------------------------------------------------

		AddPinWithNewBoardManager objAddPinWithNewBoardManager = new AddPinWithNewBoardManager();

		//------------------------------------------------------Start Add PinWith New Board-------------------------------------------------------

		protected void OnChkSingleUserAddPinWithNewBoardClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_AddPinWithNewBoard.Active == true) 
				{
					btnUploadPinFile_AddPinWithnewBoard.Visible = false;				
					txtaddPinwithNewBoard.Sensitive = true;			
					label126.Text = "Enter Pin File : ";
				}	
				else 
				{
					btnUploadPinFile_AddPinWithnewBoard.Visible = true;
					txtaddPinwithNewBoard.Sensitive = false;
					label126.Text = "          Pin File :  ";
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}			
		}

		protected void OnBtnUploadPinFileAddPinWithnewBoardClicked (object sender, EventArgs e)
		{
			try
			{
				BrowsePileFile();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
			
		}

		private void BrowsePileFile()
		{
			try
			{
				ClGlobul.lstListOfNewUsers.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtaddPinwithNewBoard.Text = objFileChooser.Filename.ToString();
					Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFileofAddNewBoardWithNewPin);
					ReadLargeFileThread.Start(objFileChooser.Filename);
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void ReadLargeNewPinsFileofAddNewBoardWithNewPin(object filePath)
		{
			try
			{
				List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
				new Thread(() =>
					{
						AddNewPinsListofAddNewBoardWithNewPin(AccountsList);
					}).Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void AddNewPinsListofAddNewBoardWithNewPin(List<string> Messages)
		{
			try
			{
				ClGlobul.lstListOfNewUsers.Clear();			
						foreach (string Message in Messages)
						{
							string NewMessages = Message.Replace("\0", "").Trim();
							string[] arMessages = Regex.Split(NewMessages, ",");

							BaseLib.Pins pin = new BaseLib.Pins();

							if (arMessages.Count() == 1)
							{
								pin.ImageUrl = arMessages[0];
							}
							else if (arMessages.Count() == 2)
							{
								pin.ImageUrl = arMessages[0];
								pin.Description = arMessages[1];
							}
							else if (arMessages.Count() == 3)
							{
								pin.Board = arMessages[0];
								pin.Description = arMessages[1];
								pin.ImageUrl = (arMessages[2]).Trim();
							}
							else if (arMessages.Count() == 4)
							{
								pin.Board = arMessages[0];
								pin.Description = arMessages[1];
								pin.ImageUrl = (arMessages[2]).Trim();
								pin.Niche = (arMessages[3]).Trim();

							}
							if (!string.IsNullOrEmpty(pin.ImageUrl))
							{
								ClGlobul.lst_AddnewPinWithNewBoard.Add(pin);
							}

						}
										
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartAddPinWithNewBoardClicked (object sender, EventArgs e)
		{
			try
			{
				startAddPinWithNewBoard();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void startAddPinWithNewBoard()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						string SingleUser_AddPinWithNewBoard = string.Empty;
						try
						{
							if (chkSingleUser_AddPinWithNewBoard.Active == true) 
							{
								ClGlobul.lstListOfNewUsers.Clear();
								if (string.IsNullOrEmpty(txtaddPinwithNewBoard.Text))
								{
									GlobusLogHelper.log.Info("Please Enter Pin File");									
									return;
								}
								else
								{
									SingleUser_AddPinWithNewBoard=txtaddPinwithNewBoard.Text.Trim();
									ClGlobul.lstListOfNewUsers.Add(SingleUser_AddPinWithNewBoard);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Pin File Enter :" + ClGlobul.lstListOfNewUsers.Count);	
								}
							}
							if (string.IsNullOrEmpty(txtaddPinwithNewBoard.Text))
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Please Upload Pin File  ]");									
								return;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.StackTrace);

						}       
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
					objAddPinWithNewBoardManager.isStopAddPinWithNewBoard = false;
					objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Clear();

					if (objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard)
					{
						objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					//ClGlobul.addNewPinWithBoard = GlobusFileHelper.ReadFile(txtaddPinwithNewBoard.Text.Trim());

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							objAddPinWithNewBoardManager.minDelayAddPinWithNewBoard = Convert.ToInt32(txtMinDelay_AddPinWithNewBoard.Text);
							objAddPinWithNewBoardManager.maxDelayAddPinWithNewBoard = Convert.ToInt32(txtMaxDelay_AddPinWithNewBoard.Text);
							objAddPinWithNewBoardManager.Nothread_AddPinWithNewBoard = Convert.ToInt32(txtThread_AddPinWithNewBoard.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Enter in Correct Format ]");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_AddPinWithNewBoard.Text) && checkNo.IsMatch(txtThread_AddPinWithNewBoard.Text))
						{
							threads = Convert.ToInt32(txtThread_AddPinWithNewBoard.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
						objAddPinWithNewBoardManager.NoOfThreadsAddPinWithNewBoard = threads;

						Thread AddPinWithNewBoardThread = new Thread(objAddPinWithNewBoardManager.StartAddPinWithNewBoard);
						AddPinWithNewBoardThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] Please Load Accounts !");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopAddPinWithNewBoardClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopAddPinWithNewBoard = new Thread(stopAddPinWithNewBoard);
				objStopAddPinWithNewBoard.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopAddPinWithNewBoard()
		{
			try
			{
				objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = true;
				List<Thread> lstTemp = objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Distinct().ToList();
				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}
			
		//---------------------------------------------------------End Add Pin With New Board---------------------------------------------------------------

		EditPinDiscriptionManager objEditPinDiscriptionManager = new EditPinDiscriptionManager ();

		//--------------------------------------------------------Start Edit Pin Description-----------------------------------------------------------------

		protected void OnChkSingleUserEditPinDescClicked (object sender, EventArgs e)
		{
			try
			{
				if (chkSingleUser_EditPinDesc.Active == true) 
				{
					btnBrowse_EditPinDescription.Visible = false;				
					txtPinDescription.Sensitive = true;			
					label133.Text = "Enter Description : ";
					txtPinDescription.Text = string.Empty;
				}	
				else 
				{
					btnBrowse_EditPinDescription.Visible = true;
					txtPinDescription.Sensitive = false;
					label133.Text = "          Description :  ";
					txtPinDescription.Text = string.Empty;
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}			
		}


		protected void OnBtnBrowseEditPinDescriptionClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseDesc_EditPinDesc();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseDesc_EditPinDesc()
		{
			try
			{
				ClGlobul.CommentNicheMessageList.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtPinDescription.Text = objFileChooser.Filename.ToString();
					ClGlobul.CommentNicheMessageList = GlobusFileHelper.ReadFile(txtPinDescription.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Description Uploaded :" + ClGlobul.CommentNicheMessageList.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
			
		protected void OnBtnStartEditPinDescClicked (object sender, EventArgs e)
		{
			try
			{
				startEditDescription();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}



		public void startEditDescription()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						string SingleUser_EditPinDesc = string.Empty;
						try
						{
							if (chkSingleUser_EditPinDesc.Active == true) 
							{
								ClGlobul.CommentNicheMessageList.Clear();
								if (string.IsNullOrEmpty(txtPinDescription.Text))
								{
									GlobusLogHelper.log.Info("Please Enter Description");									
									return;
								}
								else
								{
									SingleUser_EditPinDesc=txtPinDescription.Text.Trim();
									ClGlobul.CommentNicheMessageList.Add(SingleUser_EditPinDesc);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Description Enter :" + ClGlobul.CommentNicheMessageList.Count);	
								}
							}
							if (string.IsNullOrEmpty(txtPinDescription.Text))
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Please Upload Description  ]");									
								return;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.StackTrace);

						}       
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
					objEditPinDiscriptionManager.isStopEditPinDisc = false;
					objEditPinDiscriptionManager.lstThreadsEditPinDisc.Clear();

					if (objEditPinDiscriptionManager._IsfevoriteEditPinDes)
					{
						objEditPinDiscriptionManager._IsfevoriteEditPinDes = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							objEditPinDiscriptionManager.minDelayEditPinDisc = Convert.ToInt32(txtMinDelay_EditPinDesc.Text);
							objEditPinDiscriptionManager.maxDelayEditPinDisc = Convert.ToInt32(txtMaxDelay_EditPinDesc.Text);
							objEditPinDiscriptionManager.Nothread_EditPinDisc = Convert.ToInt32(txtThread_EditPinDesc.Text);
							objEditPinDiscriptionManager.NoOfPagesEditPinDisc = Convert.ToInt32(txtNoOfPages_EditPinDisc.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("Enter in Correct Format");
							return;
						}


						if (ClGlobul.CommentNicheMessageList.Count > 0)
						{
							clsSettingDB Database = new clsSettingDB();
							Database.UpdateSettingData("PinDescription", "PinDescriptionMessage", StringEncoderDecoder.Encode(txtPinDescription.Text));
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Start Edit Description ]");
							ClGlobul.lstPins.Clear();

						}
						if (!string.IsNullOrEmpty(txtThread_EditPinDesc.Text) && checkNo.IsMatch(txtThread_EditPinDesc.Text))
						{
							threads = Convert.ToInt32(txtThread_EditPinDesc.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
						objEditPinDiscriptionManager.NoOfThreadsEditPinDisc = threads;

						Thread EditPinDiscThread = new Thread(objEditPinDiscriptionManager.StartEditPinDisc);
						EditPinDiscThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts ! ]");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopEditPinDescClicked (object sender, EventArgs e)
		{
			try
			{
				stopEditDescription();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void stopEditDescription()
		{
			try
			{
				objEditPinDiscriptionManager._IsfevoriteEditPinDes = true;
				List<Thread> lstTempEditPin = objEditPinDiscriptionManager.lstThreadsEditPinDisc.Distinct().ToList();
				foreach (Thread item in lstTempEditPin)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("-----------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "][ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("-----------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		//---------------------------------------------------------End Edit Pin Description Module-------------------------------------------------------------

		RepinByKeywordManager objRepinByKeywordManager = new RepinByKeywordManager();

		//----------------------------------------------------------Start Repin By Keyword Module--------------------------------------------------------------

		protected void OnChkSingleUserRepinByKeywordClicked (object sender, EventArgs e)
		{
			//chkSingleUser_RepinByKeyword
			if (chkSingleUser_RepinByKeyword.Active == true) 
			{
				btnKeyword_RepinByKeyword_Browse.Visible = false;
				btnMessage_RepinByKeyword_Browse.Visible = false;
				txtKeywordBoard.Sensitive = true;
				txtMessage_RepinByKeyword.Sensitive = true;
				label142.Text = "Enter Keyword To Repin : ";
				label144.Text = "                 Enter Message : ";
				txtMessage_RepinByKeyword.Text = string.Empty;
				txtKeywordBoard.Text = string.Empty;
			}	
			else 
			{
				btnKeyword_RepinByKeyword_Browse.Visible = true;
				btnMessage_RepinByKeyword_Browse.Visible = true;
				txtKeywordBoard.Sensitive = false;
				txtMessage_RepinByKeyword.Sensitive = false;
				label142.Text = "          Keyword To Repin : ";
				label144.Text = "                           Message : ";
				txtMessage_RepinByKeyword.Text = string.Empty;
				txtKeywordBoard.Text = string.Empty;
			}
		}

		protected void OnBtnKeywordRepinByKeywordBrowseClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseKeyordToRepin();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseKeyordToRepin()
		{
			try
			{
				ClGlobul.lstRepinByKeyword.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtKeywordBoard.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstRepinByKeyword = GlobusFileHelper.ReadFile(txtKeywordBoard.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Keyword  Uploaded :" + ClGlobul.lstRepinByKeyword.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnMessageRepinByKeywordBrowseClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseMessage_RepinByKeyword();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseMessage_RepinByKeyword()
		{
			try
			{
				ClGlobul.lstMsgRepinByKeyword.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtMessage_RepinByKeyword.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstMsgRepinByKeyword = GlobusFileHelper.ReadFile(txtMessage_RepinByKeyword.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Message  Uploaded :" + ClGlobul.lstMsgRepinByKeyword.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStartRepinByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				startRepinByKeyword();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void startRepinByKeyword()
		{
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						string SingleUser_RepinByKeyword = string.Empty;
						string SingleMsg_RepinByKeyword = string.Empty;
						try
						{
							if (chkSingleUser_RepinByKeyword.Active == true) 
							{
								ClGlobul.lstRepinByKeyword.Clear();
								ClGlobul.lstMsgRepinByKeyword.Clear();
								if (string.IsNullOrEmpty(txtKeywordBoard.Text))
								{
									GlobusLogHelper.log.Info("Please Enter Keyword To Repin");									
									return;
								}
								else
								{
									SingleUser_RepinByKeyword=txtKeywordBoard.Text.Trim();
									ClGlobul.lstRepinByKeyword.Add(SingleUser_RepinByKeyword);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Keyword To Repin Enter :" + ClGlobul.lstRepinByKeyword.Count);	
									if (!string.IsNullOrEmpty(txtMessage_RepinByKeyword.Text)) 
									{
										SingleMsg_RepinByKeyword=txtMessage_RepinByKeyword.Text.Trim();
										ClGlobul.lstMsgRepinByKeyword.Add(SingleMsg_RepinByKeyword);
										GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Total Message Enter :" + ClGlobul.lstMsgRepinByKeyword.Count + " ]");	
									}
								}
							}
							if (string.IsNullOrEmpty(txtKeywordBoard.Text))
							{
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " => [ Please Upload Keyword To Repin  ]");									
								return;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine (ex.StackTrace);

						}      
					}
					catch (Exception ex)
					{
						Console.WriteLine (ex.StackTrace);
					}

					clsSettingDB Database = new clsSettingDB();

					objRepinByKeywordManager.isStopRepinByKeyword = false;
					objRepinByKeywordManager.lstThreadsRepinByKeyword.Clear();
					if (objRepinByKeywordManager._IsfevoriteRepinByKeyword)
					{
						objRepinByKeywordManager._IsfevoriteRepinByKeyword = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;

					try
					{
						try
						{
							objRepinByKeywordManager.minDelayRepinByKeyword = Convert.ToInt32(txtMinDelay_RepinByKeyword.Text);
							objRepinByKeywordManager.maxDelayRepinByKeyword = Convert.ToInt32(txtMaxDelay_RepinByKeyword.Text);
							objRepinByKeywordManager.NoOfThreadsRepinByKeyword = Convert.ToInt32(txtThread_RepinByKeyword.Text);
							objRepinByKeywordManager.MaxCountRepinByKeyword = Convert.ToInt32(txtCount_RepinByKeyword.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_RepinByKeyword.Text) && checkNo.IsMatch(txtThread_RepinByKeyword.Text))
						{
							threads = Convert.ToInt32(txtThread_RepinByKeyword.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						objRepinByKeywordManager.NoOfThreadsRepinByKeyword = threads;

						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");

						Thread BoardsThread = new Thread(objRepinByKeywordManager.StartRepinKeyword);
						BoardsThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts ! ]");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		protected void OnBtnStopRepinByKeywordClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStop = new Thread(stopRepinByKeyword);
				objStop.Start();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		private void stopRepinByKeyword()
		{
			try
			{
				objRepinByKeywordManager.isStopRepinByKeyword = true;
				List<Thread> lstTempRepinByKeyword = objRepinByKeywordManager.lstThreadsRepinByKeyword.Distinct().ToList();
				foreach (Thread item in lstTempRepinByKeyword)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write(ex.Message);
					}
				}

				GlobusLogHelper.log.Info("----------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("----------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}


		//----------------------------------------------------End Repin By Keyword Module--------------------------------------------------------------------

		AddNewPinManager objAddNewPinManager = new AddNewPinManager();

		//----------------------------------------------------Start Add NEw Pin Module----------------------------------------------------------------


		public void AccountReportActiveAccount_AddNewPin(int AccCount)
		{
			try
			{
				Gtk.Application.Invoke(delegate
					{
						lblActiveAcc_AddNewPin.Text = AccCount.ToString();
					});
			}
			catch(Exception ex)
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReportDeadAccount_AddNewPin(int AccCount)
		{ 
			try
			{
				Gtk.Application.Invoke(delegate 
					{
						lblFailedAcc_AddNewPin.Text = AccCount.ToString();
					});				
			}
			catch(Exception ex) 
			{
				GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
				Console.Write (ex.Message);
			}
		}

		public void AccountReport_AddNewPin()
		{
			try
			{
				DataSet DS = null;
				DS = new DataSet();
				string Pin = string.Empty;
				List<string> lstPinAccReport_AddNewPin = new List<string>();
				if (objAddNewPinManager.rdbYourDeviceAddNewPin == true) 
				{								
					try
					{
						DS = QM.SelectAddReport("AddNewPin");
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}

					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblSelectedPin_AddNewPin.Text = "From Device";
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}

					foreach (DataRow dt_item in DS.Tables[0].Rows)
					{
						try
						{
							Pin = dt_item.ItemArray[3].ToString();	
							lstPinAccReport_AddNewPin.Add(Pin);
							lstPinAccReport_AddNewPin.Distinct().ToList();
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
						}
						try
						{

							Gtk.Application.Invoke(delegate
								{								
									lblPinAdded_AddNewPin.Text = lstPinAccReport_AddNewPin.Count().ToString();
								});

						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
						}
					}
				}
				else if (objAddNewPinManager.rdbTheWebAddNewPin == true) 
				{								
					try
					{
						DS = QM.SelectAddReport("AddNewPin");
					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}

					try
					{

						Gtk.Application.Invoke(delegate
							{								
								lblSelectedPin_AddNewPin.Text = "The Web";
							});

					}
					catch (Exception ex)
					{
						GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
					}

					foreach (DataRow dt_item in DS.Tables[0].Rows)
					{
						try
						{
							Pin = dt_item.ItemArray[3].ToString();	
							lstPinAccReport_AddNewPin.Add(Pin);
							lstPinAccReport_AddNewPin.Distinct().ToList();
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
						}
						try
						{

							Gtk.Application.Invoke(delegate
								{								
									lblPinAdded_AddNewPin.Text = lstPinAccReport_AddNewPin.Count().ToString();
								});

						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
						}
					}
				}

			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}





		protected void OnYourDeviceAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				objAddNewPinManager.rdbYourDeviceAddNewPin = true;
				objAddNewPinManager.rdbTheWebAddNewPin = false;
				lblWebsite.Visible = true;
				txtWebsite_AddNewPin.Visible = true;
				lblSelectPhoto.Visible = true;
				btnBrowsePhoto_AddNewPin.Visible = true;
				lblPinFile.Text = "Board Name :";
				txtNewPin.Sensitive = false;
				lblHint.Text="eg:- BoardName:Niche";
				btnBrowseBoardName_AddNewPin.Visible = true;
				btnNewFile_AddNewPin_Browse.Visible = false;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnTheWebAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				objAddNewPinManager.rdbYourDeviceAddNewPin = false;
				objAddNewPinManager.rdbTheWebAddNewPin = true;
				lblWebsite.Visible = false;
				txtWebsite_AddNewPin.Visible = false;
				lblSelectPhoto.Visible = false;
				btnBrowsePhoto_AddNewPin.Visible = false;
				lblPinFile.Text = "    Pin File :";
				txtNewPin.Sensitive = false;
				lblHint.Text="eg:- ImageUrl,Description,Board";
				btnBrowseBoardName_AddNewPin.Visible = false;
				btnNewFile_AddNewPin_Browse.Visible = true;
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnNewFileAddNewPinBrowseClicked (object sender, EventArgs e)
		{
			try
			{
				BrowsePinFile();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowsePinFile()
		{
			try
			{
				if (TheWeb_AddNewPin.Active == true)
				{
					ClGlobul.lstListOfNewUsers.Clear();
					FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
					if (objFileChooser.Run() == (int)ResponseType.Accept) 
					{
						txtNewPin.Text = objFileChooser.Filename.ToString();
						Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFile);
						ReadLargeFileThread.Start(objFileChooser.Filename);
					}
					objFileChooser.Destroy();
				}
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void ReadLargeNewPinsFile(object filePath)
		{
			try
			{
				List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
				new Thread(() =>
					{
						AddNewPinsList(AccountsList);
					}).Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void AddNewPinsList(List<string> Messages)
		{
			try
			{
				ClGlobul.lstListOfNewUsers.Clear();			
						foreach (string Message in Messages)
						{
							string NewMessages = Message.Replace("\0", "").Trim();
							string[] arMessages = Regex.Split(NewMessages, ",");

							BaseLib.Pins pin = new BaseLib.Pins();

							if (arMessages.Count() == 1)
							{
								pin.ImageUrl = arMessages[0];
							}
							else if (arMessages.Count() == 2)
							{
								pin.ImageUrl = arMessages[0];
								pin.Description = arMessages[1];
							}
							else if (arMessages.Count() == 3)
							{
								pin.ImageUrl = arMessages[0];
								pin.Description = arMessages[1];
								pin.Board = arMessages[2];
							}
							else if (arMessages.Count() == 4)
							{
								pin.ImageUrl = arMessages[0];
								pin.Description = arMessages[1];
								pin.Board = arMessages[2];
								pin.Email = arMessages[3];
							}
							if (!string.IsNullOrEmpty(pin.ImageUrl))
							{
								ClGlobul.lstListOfPins.Add(pin);
							}

						}
										
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnBrowsePhotoAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				BrowsePhotoAddNewPin();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowsePhotoAddNewPin()
		{
			if (YourDevice_AddNewPin.Active == true) {
				try {
					ClGlobul.lstListofFiles.Clear ();
					ClGlobul.lstListOfNewUsers.Clear ();
					FileChooserDialog objFileChooser = new FileChooserDialog ("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
					if (objFileChooser.Run () == (int)ResponseType.Accept) {
						//txtNewPin.Text = objFileChooser.Filename.ToString();
						foreach (var item in objFileChooser.Filenames) {
							ClGlobul.lstListofFiles.Add (item);
						}
					}
					objFileChooser.Destroy ();

					GlobusLogHelper.log.Info ("[ " + DateTime.Now + "] => [ Total Uploaded Image Is " + ClGlobul.lstListofFiles.Count + " ]");
				} 
				catch (Exception ex) 
				{
					Console.Write (ex.Message);
				}
			}		
		}

		protected void OnBtnStartAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				startAddNewPin();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void startAddNewPin()
		{
			List<string> lstSingleUserAddnewpin = new List<string>();
			try
			{
				if (PDGlobals.listAccounts.Count > 0)
				{
					try
					{
						if (objAddNewPinManager.rdbYourDeviceAddNewPin == true || objAddNewPinManager.rdbTheWebAddNewPin == true)
						{
							objAddNewPinManager.WebsiteUrl = txtWebsite_AddNewPin.Text.Trim();
							if (objAddNewPinManager.rdbYourDeviceAddNewPin == true)
							{
								try
								{
									ClGlobul.lstListOfPins.Clear();
									if (string.IsNullOrEmpty(txtNewPin.Text.Trim()))
									{
										GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Give Board Name. ]");									
										return;
									}                                  
								}
								catch (Exception ex)
								{
									Console.Write (ex.Message);
								}
							}//end of single User
							if (objAddNewPinManager.rdbTheWebAddNewPin == true)
							{
								try
								{
									if (string.IsNullOrEmpty(txtNewPin.Text))
									{
										GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Upload Pin File ]");

										return;
									}
								}
								catch (Exception ex)
								{
									Console.Write (ex.Message);
								}
							}//end of multiple user
						}
						else
						{
							GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Select First From Device or From The Web ]");

							return;
						}

					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
					objAddNewPinManager.isStopAddNewPin = false;
					objAddNewPinManager.lstThreadsAddNewPin.Clear();

					if (objAddNewPinManager._IsfevoriteAddNewPin)
					{
						objAddNewPinManager._IsfevoriteAddNewPin = false;
					}

					Regex checkNo = new Regex("^[0-9]*$");

					int processorCount = objUtils.GetProcessor();

					int threads = 25;

					int maxThread = 25 * processorCount;
					try
					{
						try
						{
							objAddNewPinManager.minDelayAddNewPin = Convert.ToInt32(txtMinDelay_AddNewPin.Text);
							objAddNewPinManager.maxDelayAddNewPin = Convert.ToInt32(txtMaxDelay_AddNewPin.Text);
							objAddNewPinManager.Nothread_AddNewPin = Convert.ToInt32(txtThread_AddNewPin.Text);
							objAddNewPinManager.MaxCountAddPin = Convert.ToInt32(txtCount_AddNewPin.Text);
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("Enter in Correct Format");
							return;
						}

						if (!string.IsNullOrEmpty(txtThread_AddNewPin.Text) && checkNo.IsMatch(txtThread_AddNewPin.Text))
						{
							threads = Convert.ToInt32(txtThread_AddNewPin.Text);
						}

						if (threads > maxThread)
						{
							threads = 25;
						}
						GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Process Starting ] ");
						objAddNewPinManager.NoOfThreadsAddNewPin = threads;

						Thread LikeThread = new Thread(objAddNewPinManager.StartAddNewPin);
						LikeThread.Start();
					}

					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + "] => [ Please Load Accounts ! ]");
					//GlobusLogHelper.log.Debug("Please Load Accounts !");

				}
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnBtnStopAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				Thread objStopAddNewPin = new Thread(stopAddNewPin);
				objStopAddNewPin.Start();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		public void stopAddNewPin()
		{
			try
			{
				objAddNewPinManager._IsfevoriteAddNewPin = true;
				List<Thread> lstTempAddNewPin = objAddNewPinManager.lstThreadsAddNewPin.Distinct().ToList();
				foreach (Thread item in lstTempAddNewPin)
				{
					try
					{
						item.Abort();
					}
					catch (Exception ex)
					{
						Console.Write (ex.Message);
					}
				}

				GlobusLogHelper.log.Info("---------------------------------------------------------------");
				GlobusLogHelper.log.Info("[ " + DateTime.Now + "] [ PROCESS STOPPED ]");
				GlobusLogHelper.log.Info("---------------------------------------------------------------");
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

//		private void btnBrowseBoardName_AddNewPin_Click(object sender, RoutedEventArgs e)
//		{
//			try
//			{
//				//lstBoardNameNiche_AddNewPin
//				ClGlobul.lstBoardNameNiche_AddNewPin.Clear();
//
//				txtNewPin.IsReadOnly = true;
//				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
//				dlg.DefaultExt = ".txt";
//				dlg.Filter = "Text documents (.txt)|*.txt";
//				Nullable<bool> result = dlg.ShowDialog();
//				if (result == true)
//				{
//					txtNewPin.Text = dlg.FileName.ToString();
//				}
//				try
//				{
//					ClGlobul.lstBoardNameNiche_AddNewPin = GlobusFileHelper.ReadFile(txtNewPin.Text.Trim());
//				}
//				catch (Exception ex)
//				{
//					GlobusLogHelper.log.Info(" Please Select File ");
//				}
//				GlobusLogHelper.log.Info(" => [ Total Uploaded BoardName Is " + ClGlobul.lstBoardNameNiche_AddNewPin.Count + " ]");
//			}
//			catch(Exception ex)
//			{
//				GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
//			}
//		}


		protected void OnBtnBrowseBoardNameAddNewPinClicked (object sender, EventArgs e)
		{
			try
			{
				BrowseBoardNameAddNewPin();
			}
			catch (Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		private void BrowseBoardNameAddNewPin()
		{			
			try
			{
				ClGlobul.lstBoardNameNiche_AddNewPin.Clear();
				FileChooserDialog objFileChooser = new FileChooserDialog("Choose Files To View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
				if (objFileChooser.Run() == (int)ResponseType.Accept) 
				{
					txtNewPin.Text = objFileChooser.Filename.ToString();
					ClGlobul.lstBoardNameNiche_AddNewPin = GlobusFileHelper.ReadFile(txtNewPin.Text.Trim());
					Addtologger("[ " + DateTime.Now + "] => [ Total Board NAme  Uploaded :" + ClGlobul.lstBoardNameNiche_AddNewPin.Count + " ]");	
				}
				objFileChooser.Destroy();
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}					
		}




	}
}



	

