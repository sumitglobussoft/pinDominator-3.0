using System;
using Gtk;

namespace PinDominator
{
	public partial class frmAddAccounts : Gtk.Window
	{
		public frmAddAccounts () :
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

		}

		//public static Event CampaignStopLogevents = new Event();

		public static void ReloadTreeView_AddAccounts(string log)
		{

		}

		protected void OnBtnAddAccountsClicked (object sender, EventArgs e)
		{
			string UserName = string.Empty;
			string Password = string.Empty;
			string Niche = string.Empty;
			string proxyDetails = string.Empty;
			string LoginStatus = string.Empty;
			try
			{
				LoginStatus = "NotChecked";

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}

			if (string.IsNullOrEmpty (txtInputUserName.Text.Trim ())) 
			{
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Please Enter User Name.");
				ResponseType response = (ResponseType)md.Run ();
				md.Destroy ();
				return;
			} 
			else 
			{
				UserName = txtInputUserName.Text.Trim ();
			}
			if (string.IsNullOrEmpty (txtInputPassword.Text.Trim ())) 
			{
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Please Enter Password.");
				ResponseType response = (ResponseType)md.Run ();
				md.Destroy ();
				return;
			} 
			else 
			{
				Password = txtInputPassword.Text.Trim ();
			}


			if (string.IsNullOrEmpty (txtInputNiche.Text.Trim ()))
			{
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, "Please Enter Niche.");
				ResponseType response = (ResponseType)md.Run ();
				md.Destroy ();
				return;
			} 
			else 
			{
				Niche = txtInputNiche.Text.Trim ();
			}


			if (!string.IsNullOrEmpty (txtInputProxy.Text.Trim ())) 
			{
				proxyDetails = txtInputProxy.Text.Trim ();
			} 				
			string proxyAddress = string.Empty;
			string proxyPort = string.Empty;
			string proxyUserName = string.Empty;
			string proxyPassword = string.Empty;

			if (proxyDetails.Contains (":")) 
			{
				string[] AccArr = proxyDetails.Split (':');
				int DataCount = proxyDetails.Split (':').Length;
				if (DataCount == 2) 
				{
					proxyAddress = proxyDetails.Split (':') [0];
					proxyPort = proxyDetails.Split (':') [1];
				} else if (DataCount > 2 && DataCount < 5)
				{
					proxyAddress = proxyDetails.Split (':') [0];
					proxyPort = proxyDetails.Split (':') [1];
					proxyUserName = proxyDetails.Split (':') [2];
					proxyPassword = proxyDetails.Split (':') [3];
				}
			}

			AddAccounts (UserName,Password,Niche,proxyAddress,proxyPort,proxyUserName,proxyPassword,LoginStatus);
			MainWindow.objMainWindow.AccounLoad ();
			try
			{
				this.Destroy();

			}
			catch{
			};
		}

		protected void OnBtnClearAccountsClicked (object sender, EventArgs e)
		{
			ClearAccounts ();
		}
		public void AddAccounts(string accountUser, string accountPass, string Niche, string proxyAddress, string proxyPort, string proxyUserName, string proxyPassword, string LoginStatus)
		{
			string strQuery = "INSERT INTO tb_emails (Username, Password, Niches, proxyAddress, proxyPort, proxyUsername, proxyPassword, UserAgent, Follower, Following , BOARDS, BoardsName, ScreenName, LoginStatus) VALUES ('" + accountUser + "','" + accountPass + "', '" + Niche + "' ,'" + proxyAddress + "','" + proxyPort + "',  '" + proxyUserName + "','" + proxyPassword + "','" + " " + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + " " + "','" + " " + "','" + LoginStatus + "' )";
			DataBaseHandler.InsertQuery(strQuery, "tb_emails");

			ClearAccounts ();
		}



		private void ClearAccounts()
		{
			try
			{
				txtInputUserName.Text=string.Empty;
				txtInputProxy.Text=string.Empty;
				txtInputPassword.Text=string.Empty;
				txtInputNiche.Text=string.Empty;
			}
			catch(Exception ex)
			{
				Console.Write (ex.Message);
			}
		}

		protected void OnCmbSelectAccountTypeChanged (object sender, EventArgs e)
		{
			//if (cmbSelectAccountType.ActiveText == "Facebook") {
			
			//label2.Text = "Email :";
			///}


		}
	}
}

