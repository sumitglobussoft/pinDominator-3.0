using System;
//using System.Windows;
using System.Threading;
using Gtk;
using PinDominator;
using BaseLib;

namespace PinDominator
{
	public partial class frmLicensing : Gtk.Window
	{
		string cpuID = string.Empty;

		LicenseManager licensemanager = new LicenseManager();
		bool CheckNetConn = false;
		string status = string.Empty;
		string validateLicense = "Validate Your License";
		string start = "Start";
		string activate = "Activate";

		string freeTrialKey = "freetrial";

		string server1 = "licensing.facedominator.com/licensing/PD";
		string server2 = "licensing.facedominator.com/licensing/PD";
		string server3 = "licensing.facedominator.com/licensing/PD";



		public frmLicensing () :
	    base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			Gdk.Color fontcolor = new Gdk.Color(255,255,255);
			lblUser.ModifyFg(StateType.Normal, fontcolor);
			lblPasswword.ModifyFg(StateType.Normal, fontcolor);
			lblEmail.ModifyFg(StateType.Normal, fontcolor);
			lblTransactionID.ModifyFg(StateType.Normal, fontcolor);
			lblstatus.ModifyFg(StateType.Normal, fontcolor);
			lblLicenseStatus.ModifyFg(StateType.Normal, fontcolor);
			lblMacID.ModifyFg(StateType.Normal, fontcolor);
			lblLicenseType.ModifyFg (StateType.Normal, fontcolor);
    		Gdk.Color col = new Gdk.Color ();
			Gdk.Color.Parse ("#3b5998", ref col);
			ModifyBg (StateType.Normal, col);



			try
			{
				
				btnStart.Visible = false;
				cpuID = licensemanager.FetchMacId();

				new Thread(() =>
					{
						Gtk.Application.Invoke((delegate
							{
								lblMacID.Text += "  " + cpuID;
								lblMacID.ModifyBg(StateType.Normal, new Gdk.Color(120,10,120));
								btnActivate.Label = "Validate Your License";
								btnActivate.Visible = false;
								//AddToLogs("[ " + DateTime.Now + " ] => [ Please wait while your License is Validated ]");
								//timer_start_LD.Start();
								LicenceValidation();
								DisableControls();
							}));
					}).Start();

			}
			catch (Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}


		}


		public void AddToLogs(string log)
		{
		 
			try{
				
				Gtk.Application.Invoke (delegate {
					//txtViewLicenseLogger.Buffer.Text += log+"\n";

				});
			}
			catch(Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}

		}

		protected void OnBtnStartClicked (object sender, EventArgs e)
		{
			//OpenFrmMain();
		}


		#region OpenFrmMain
		private void OpenFrmMain()
		{
			try
			{


				PinDominator.MainWindow frmwinMain = new PinDominator.MainWindow ();
				frmwinMain.Show ();
				this.Hide();

			}
			catch (Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		} 
		#endregion


		private void LicenceValidation()
		{
			try
			{
				new Thread(() =>
					{
						if (btnActivate.Label == activate)
						{
							Activate();
						}
						else if (btnActivate.Label == start)
						{
							StartLicenseValidation();
						}
						else if (btnActivate.Label == validateLicense)
						{
							StartLicenseValidation();
						}

					}).Start();

			}
			catch (Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		}

		private void StartLicenseValidation()
		{

			try
           			{
				LicenseValidation();
				if (status == "active")
				{
					//AddToLogs("License Validated, Please click on Start Button");


					Gtk.Application.Invoke((delegate
						{
							btnActivate.Visible = false;
							//btnStart.Visible = true;
							OpenFrmMain();

						}));
				}
			}
			catch (Exception ex)
			{
				
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		} 

		private void LicenseValidation()
		{
			try
			{
				string username = string.Empty;
				string pass = string.Empty;
				string txnID = string.Empty;
				string email = string.Empty;

				try{
					Gtk.Application.Invoke (delegate 
					{
						DisableControls();
					});  


				}
				catch(Exception ex)
				{
					Console.WriteLine (ex.StackTrace);
					//GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
				}


 				{
                					//AddToLogs("[ " + DateTime.Now + " ] => [ Validating on Server 1 ]");
					if (licensemanager.ValidateCPUID(ref status, server1, ref username, ref pass, ref txnID, ref email, freeTrialKey, cpuID))
					{
						try
						{
						#region Server 1
							Gtk.Application.Invoke((delegate
								{
						    btnActivate.Label = "Start";
							lblstatus.Text = "Activated";
							
							txtUserName.Text = username;
							txtPassword.Text = pass;
							txtTransactionID.Text = txnID;
							txtEmail.Text = email;
									cmb_LicType.Entry.Text=Globals.licType;
								}));  
						}
						catch(Exception ex)
						{
							Console.WriteLine (ex.StackTrace);
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}					
						#endregion

						return;
					}
					else if (status == "norecordfound")
					{
						NoRecordFoundMethod();
						return;
					}
					if (status == "nonactive")
					{
						Gtk.Application.Invoke((delegate
							{
								//lblServr3Status.Text = status;
								Gtk.Application.Invoke((delegate
									{

										txtUserName.Text = username;
										txtPassword.Text = pass;
										txtTransactionID.Text = txnID;
										txtEmail.Text = email;

										btnActivate.Visible=true;
										lblstatus.Text="Please contact skype support team \n\tFaceDominatorSupport ";
										//StartLicenseValidation();
										btnActivate.Sensitive=false;
										btnActivate.Label="Registered Successfully";
									}));
							}));

						Gtk.Application.Invoke((delegate
							{
								DisableControls();
							}));
					}
					else if (status == "norecordfound")
					{
						NoRecordFoundMethod();
						return;
					}
					if (licensemanager.ValidateCPUID(ref status, server1, ref username, ref pass, ref txnID, ref email, freeTrialKey, cpuID))
					{

						#region Server 2
						Gtk.Application.Invoke((delegate
							{

								btnActivate.Label = "Start";
								lblstatus.Text = "Activated";
								//lblstatus.BackColor = Color.Green;

								txtUserName.Text = username;
								txtPassword.Text = pass;
								txtTransactionID.Text = txnID;
								txtEmail.Text = email;


							}));
						 
						#endregion

						return;
					}
					else if (status == "norecordfound")
					{
						NoRecordFoundMethod();
						return;
					}
					else
					{
						

						AddToLogs("[ " + DateTime.Now + " ] => [ Failed on Server 2, Status : " + status + " \nValidating on Server 3 ]");

						CheckNetConn = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

						if (CheckNetConn)
						{

						}
						else
						{
							
							//AddToLogs("[ " + DateTime.Now + " ] => [ Your Internet Connection is disabled ! or not working, Please Check Your Internet Connection... ]");
						}

					}
					if (licensemanager.ValidateCPUID(ref status, server1, ref username, ref pass, ref txnID, ref email, freeTrialKey, cpuID))
					{
						#region Server 3
						Gtk.Application.Invoke((delegate
							{

								btnActivate.Label = "Start";
								lblstatus.Text = "Activated";							

								txtUserName.Text = username;
								txtPassword.Text = pass;
								txtTransactionID.Text = txnID;
								txtEmail.Text = email;
							}));

						#endregion

						return;
					}
					else
					{
						Gtk.Application.Invoke((delegate
							{
								//lblServr3Status.Text = status;
							}));
					}
					if (status == "nonactive")
					{
						//AddToLogs("[ " + DateTime.Now + " ] => [ Status: " + status + " ]");

						Gtk.Application.Invoke((delegate
							{
								DisableControls();
							}));
					}
					else if (status == "norecordfound")
					{
						NoRecordFoundMethod();
						return;
					}

				
				}

			}
			catch (Exception ex)
			{

				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		} 


		private void NoRecordFoundMethod()
		{
			try
			{
//				AddToLogs("[ " + DateTime.Now + " ] => [ Status: " + status + " ]");
//				AddToLogs("[ " + DateTime.Now + " ] => [ Please activate your license by submitting your Details ]");

				Gtk.Application.Invoke((delegate
					{
						Gtk.Application.Invoke((delegate
							{
								btnActivate.Label = "Activate";
								btnActivate.Visible = true;
								EnableControls();
							}));


					}));
			}
			catch (Exception ex)
			{

				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");

			}

		} 

		#region DisableControls
		private void DisableControls()
		{
			try
			{
			txtEmail.Sensitive = false;
			txtPassword.Sensitive = false;
			txtUserName.Sensitive = false;
			txtTransactionID.Sensitive = false;
				cmb_LicType.Sensitive=false;
			}
			catch(Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		}
		#endregion

		#region EnableControls
		private void EnableControls()
		{
			try
			{
			txtEmail.Sensitive = true;
			txtPassword.Sensitive = true;
			txtUserName.Sensitive = true;
			txtTransactionID.Sensitive = true;
				cmb_LicType.Sensitive=false;
			}
			catch(Exception ex)
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		}


		private new void Activate()
		{
			try
			{
				string Username = txtUserName.Text;
				string Password = txtPassword.Text;
				string TransactionID = txtTransactionID.Text;
				string Email = txtEmail.Text;

				try
				{
					Globals.licType = cmb_LicType.ActiveText.ToString();
				}
				catch(Exception ex)
				{

				}

				if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(TransactionID) && !string.IsNullOrEmpty(Email))
				{
					//AddToLogs("[ " + DateTime.Now + " ] => [ Sending Details for Registration");

					string response_Registration = licensemanager.RegisterUser(Username, Password, cpuID, TransactionID, Email, server2);

					if (string.IsNullOrEmpty(response_Registration))
					{

						//AddToLogs("Thank you, your activation request has been sent, we will promptly activate your licence");

					}
					else
					{
						response_Registration = response_Registration.Trim();
						if (response_Registration == "Sucessfully Inserted")
						{
							//StartLicenseValidation();
							Gtk.Application.Invoke((delegate
								{
									lblstatus.Text=response_Registration;
									Thread.Sleep(1000);
									lblstatus.Text="Please contact skype support team \n\tFaceDominatorSupport ";
									StartLicenseValidation();
									btnActivate.Sensitive=false;
									btnActivate.Label="Registered Successfully";
								}));
						}
						else
						{

							//AddToLogs("Thank you, your activation request has been sent, we will promptly activate your licence");
						}

					}
				}
				else
				{

					//AddToLogs("No Fields can be blank");
				}
			}
			catch (Exception ex)
			{
				//GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");

			}
		} 

		protected void OnBtnActivateClicked (object sender, EventArgs e)
		{
			try
			{
				new Thread(() =>
					{
						if (btnActivate.Label == activate)
						{
							Activate();
						}
						else if (btnActivate.Label == start)
						{
							StartLicenseValidation();
						}
						else if (btnActivate.Label == validateLicense)
						{
							StartLicenseValidation();
						}

						if (status == "active")
						{

							AddToLogs("License Validated, Please click on Start LD Button");
							Gtk.Application.Invoke((delegate
								{
									btnActivate.Visible = false;
									btnStart.Visible = true;

								}));
						}

					}).Start();

			}
			catch(Exception ex) 
			{
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
			}
		}
		#endregion


	}
}

