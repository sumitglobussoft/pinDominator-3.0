using System;
using Gtk;
using System.Threading;
using PinDominator;
using System.Collections.Generic;
using System.Linq;

namespace PinDominator
{
	public partial class frmEventManager : Gtk.Window
	{
		public frmEventManager () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			Gdk.Color fontcolor = new Gdk.Color(255,255,255);
			label1.ModifyFg(StateType.Normal, fontcolor);
			label2.ModifyFg(StateType.Normal, fontcolor);
			label3.ModifyFg(StateType.Normal, fontcolor);
			label4.ModifyFg(StateType.Normal, fontcolor);


			Gdk.Color col = new Gdk.Color ();
			Gdk.Color.Parse ("#3b5998", ref col);

			ModifyBg (StateType.Normal, col);

			EventManager.CampaignStopLogevents.addToLogger += new EventHandler (CampaignnameLog);
			txtUseSingleEventUrl.Visible = false;
			try
			{

				btnUploadUrlEventManager.Sensitive=false;
				btnUploadDetailsEventCreator.Sensitive=true;
				chkEvents_EventInviter_SendToAll.Sensitive = false;
				btnStopProcessEventManager.Sensitive = false;
				chkUseSingleUrl.Sensitive=false;
				cmbEventInput.Active=0;
				rbk_EventCreator.Active=true;

			}
			catch (Exception ex)
			{
				Console.WriteLine (ex.StackTrace);
			}
		}
		EventManager objEventManager=new EventManager();
		protected void OnBtnUploadUrlEventManagerClicked (object sender, EventArgs e)
		{
			FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

			if (fileChooser.Run () == (int)ResponseType.Accept) 
			{
				//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
				objEventManager.LstEventURLsEventInviter = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ());
				Addtologger("Event Url Loaded : "+objEventManager.LstEventURLsEventInviter.Count);
			}

			fileChooser.Destroy ();
		}

		public void Addtologger(string log)
		{
			try{

				Gtk.Application.Invoke (delegate {
					txtViewEventManager.Buffer.Text += log+"\n";
				});
				//this.ClientEvent+= delegate {    
			
				//};
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

		protected void OnBtnProcessStartEventManagerClicked (object sender, EventArgs e)
		{
			objEventManager.isStopEventInviter = false;
			objEventManager.isStopEvenCreator=false;

			if (FBGlobals.listAccounts.Count > 0)
			{
				string	StartGroupProcessUsing = string.Empty;
				try
				{
					StartGroupProcessUsing = cmbEventInput.Entry.Text.ToString ();

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}

				//
				bool checkBool=Convert.ToBoolean(chkUseSingleUrl.Active);

				if (checkBool == true)
				{
					string singleEventUrl = txtUseSingleEventUrl.Text;

					objEventManager.LstEventURLsEventInviter = new List<string> ();


					if (!singleEventUrl.Contains ("www.facebook.com/events/")) 
					{
						Addtologger (" Enter Valid Event Url in Text box  !");
						return;						
					} 
					else 
					{
						objEventManager.LstEventURLsEventInviter.Add (singleEventUrl);
					}
				}
				try
				{
					EventManager.minDelayEventInvitor = Convert.ToInt32 (txtDelayFromEventManager.Text);
					EventManager.maxDelayEventInvitor = Convert.ToInt32 (txtDelayToEventManager.Text);
				} 
				catch (Exception ex)
				{
					Console.WriteLine ("Error : " + ex.StackTrace);
				}

				try
				{
					objEventManager.NoOfFriendsSuggestionAtOneTimeEventInviter = 30;
					EventManager.intNoOfFriends = 30;
				}
				catch (Exception)
				{
					objEventManager.NoOfFriendsSuggestionAtOneTimeEventInviter = 10;
				}

				if (chkEvents_EventInviter_SendToAll.Active)
				{
					objEventManager.SendToAllFriendsEventInviter = true;
				} 
				else
				{
					objEventManager.SendToAllFriendsEventInviter = false;
				}

				if (StartGroupProcessUsing == "Event Creator")
				{
					if (objEventManager.LstEventDetailsEventCreator==null)
					{
						Addtologger ("Please Load Event Details !");
						return;
					}

					Thread ObjnewThead = new Thread (objEventManager.StartEvenCreator);
					ObjnewThead.Start ();
					btnStopProcessEventManager.Sensitive = true;
				} 
				else if (StartGroupProcessUsing == "Event Inviter")
				{

					if (objEventManager.LstEventURLsEventInviter==null)
					{
						Addtologger ("Please Load Event Urls !");
						return;
					}

					Thread ObjnewThead = new Thread (objEventManager.StartEventInviter);
					ObjnewThead.Start ();
					btnStopProcessEventManager.Sensitive = true;
				}
			}
			else
			{
				Addtologger("Please Load Accounts !");

			}

		}

		protected void OnBtnUploadDetailsEventCreatorClicked (object sender, EventArgs e)
		{
			try
			{

				FileChooserDialog fileChooser = new FileChooserDialog ("Choose Files to View", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

				if (fileChooser.Run () == (int)ResponseType.Accept) 
				{
					//txtEntryUploadAccount.Text = fileChooser.Filename.ToString ();
					objEventManager.LstEventDetailsEventCreator = GlobusFileHelper.ReadFile (fileChooser.Filename.ToString ().Trim ());

					Addtologger("Event creator Details Loaded : "+objEventManager.LstEventDetailsEventCreator.Count);
				}

				fileChooser.Destroy ();
			}
			catch(Exception Ex)
			{
				Console.WriteLine (Ex.StackTrace);

			}

		}

		protected void OnBtnStopProcessEventManagerClicked (object sender, EventArgs e)
		{
			try
			{
				Thread threadStopAccountCreation = new Thread(StopEventManager);
				threadStopAccountCreation.Start();
				//StopAccountCreation();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.StackTrace);
			}
		}

		private void StopEventManager()
		{
			try
			{
				objEventManager.isStopEventInviter = true;
				objEventManager.isStopEvenCreator=true;
				List<Thread> lstTemp = new List<Thread>();
				lstTemp = objEventManager.lstThreadsEvenCreator.Distinct().ToList();

				foreach (Thread item in lstTemp)
				{
					try
					{
						item.Abort();
						objEventManager.lstThreadsEvenCreator.Remove(item);
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

		protected void OnChkUseSingleUrlClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(chkUseSingleUrl.Active);
			if (checkBool) 
			{
				txtUseSingleEventUrl.Visible = true;
				txtUseSingleEventUrl.Sensitive = true;
			} 
			else 
			{
				txtUseSingleEventUrl.Visible = false;
			}
		}

		protected void OnRbkEventCreatorClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(rbk_EventCreator.Active);
			if (!checkBool)
			{
				return;
			} 
			else
			{
				try
				{

					btnUploadUrlEventManager.Sensitive=false;
					btnUploadDetailsEventCreator.Sensitive=true;
					chkEvents_EventInviter_SendToAll.Sensitive = false;
					btnStopProcessEventManager.Sensitive = false;
					chkUseSingleUrl.Sensitive=false;
					cmbEventInput.Active=0;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}

		protected void OnRbkEventInviterClicked (object sender, EventArgs e)
		{
			bool checkBool=Convert.ToBoolean(rbk_eventInviter.Active);
			if (!checkBool)
			{
				return;
			} 
			else
			{
				try
				{
					btnUploadUrlEventManager.Sensitive=true;
					btnUploadDetailsEventCreator.Sensitive=false;
					chkEvents_EventInviter_SendToAll.Sensitive = false;
					btnStopProcessEventManager.Sensitive = false;
					chkUseSingleUrl.Sensitive=true;
					cmbEventInput.Active=1;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}

		protected void OnChkEventsEventInviterSendToAllClicked (object sender, EventArgs e)
		{
			//throw new NotImplementedException ();
		}
		protected void OnEventCreatorActionActivated (object sender, EventArgs e)
		{
			rbk_EventCreator.Active = true;
			bool checkBool=Convert.ToBoolean(rbk_EventCreator.Active);
			if (!checkBool)
			{
				return;
			} 
			else
			{
				try
				{

					btnUploadUrlEventManager.Sensitive=false;
					btnUploadDetailsEventCreator.Sensitive=true;
					chkEvents_EventInviter_SendToAll.Sensitive = false;
					btnStopProcessEventManager.Sensitive = false;
					chkUseSingleUrl.Sensitive=false;
					cmbEventInput.Active=0;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}
		}
		protected void OnEventCreatorAction1Activated (object sender, EventArgs e)
		{
			rbk_eventInviter.Active = true;
			bool checkBool=Convert.ToBoolean(rbk_eventInviter.Active);
			if (!checkBool)
			{
				return;
			} 
			else
			{
				try
				{
					btnUploadUrlEventManager.Sensitive=true;
					btnUploadDetailsEventCreator.Sensitive=false;
					chkEvents_EventInviter_SendToAll.Sensitive = false;
					btnStopProcessEventManager.Sensitive = false;
					chkUseSingleUrl.Sensitive=true;
					cmbEventInput.Active=1;

				}
				catch (Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
				}
			}

		}
	}
}

