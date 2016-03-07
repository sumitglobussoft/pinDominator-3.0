using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Net.NetworkInformation;

using Gtk;
using PinDominator;
using Gdk;
using Globussoft;
using BaseLib;


namespace PinDominator
{
    public class LicenseManager
      
    {
        #region global declaration
		public static Events LinkedLiceneManager = new Events();
        
		GlobusHttpHelper HttpHelpr = new GlobusHttpHelper(); 
		//ChilkatHttpHelpr HttpHelpr = new ChilkatHttpHelpr();
        #endregion

        #region CreateLicense
        public void CreateLicense()
        {

        } 
        #endregion

        #region FetchMacId
        public string FetchMacId()
        {
            string macAddresses = "";
            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        if (!string.IsNullOrEmpty(macAddresses))
                        {
                            break;
                        }
                        macAddresses += nic.GetPhysicalAddress().ToString();
                        //break;
                    }
                }
            }
            catch (Exception ex)
            {
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message+"FetchMacId", PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
            }
            return macAddresses;
        } 
        #endregion

        #region getCPUID
        public string getCPUID()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;

                }
            }
            return cpuInfo;

        } 
        #endregion

        /// <summary>
        /// Checks the status of the CPUID from Database
        /// If status is Active, MainFrm starts
        /// </summary>
        /// 
        #region ValidateCPUID
        public bool ValidateCPUID(ref string statusMessage, string cpuID)
        {
           try
            {

                #region Through php

                string res = string.Empty;
				string url = string.Empty;
				//getHtmlfromUrl(new Uri(Photolink), "","");
				 url = "http://faced.extrem-hosting.net/GetUserData.php?cpid=" + cpuID + "";
				res = HttpHelpr.getHtmlfromUrl(new Uri(url), "", "", "");

                if (string.IsNullOrEmpty(res))
                {
                    System.Threading.Thread.Sleep(1000);
					res = HttpHelpr.getHtmlfromUrl(new Uri(url),"","","");
                }

                if (!string.IsNullOrEmpty(res))
                {
                    string status = string.Empty;
                    string dateTime = string.Empty;
                    string username = string.Empty;
                    string txnID = string.Empty;

                    string trimmed_response = res.Replace("<pre>", "").Replace("</pre>", "").Trim().ToLower();

                   // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Responce):" + trimmed_response, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

                    string[] array_status = System.Text.RegularExpressions.Regex.Split(trimmed_response, "<:>");
                    try
                    {
                        status = array_status[0].ToLower();
                    }
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message+"ValidateCPUID", PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
					}
                    try
                    {
                        dateTime = array_status[1].ToLower();
                        Loger("Date:" + dateTime);
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicensingDate):" + dateTime, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    }
					catch(Exception ex) 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
					}
                    try
                    {
                        username = array_status[2].ToLower();
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Username):" + username, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    }
					catch(Exception ex)
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
					}
                    try
                    {
                        txnID = array_status[3].ToLower();
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(TxnId):" + txnID, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    }
					catch(Exception ex) 
					{
						GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
					}

                    if (trimmed_response.ToLower().Contains("freetrial") && ((status.ToLower() == "active") || (status.ToLower() == "nonactive")))
                    {
                        if (CheckActivationUpdateStatus(cpuID, dateTime, status, ""))
                        {
                            statusMessage = "Active";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return true;
                        }
                        else
                        {
                            statusMessage = "trialexpired";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return false;
                        }
                    }
                    else if (status.ToLower() == "active")
                    {
                        statusMessage = "active";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        return true;
                        // DisableControls();
                    }
                    else if (status.ToLower() == "nonactive")
                    {
                        statusMessage = "nonactive";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
						//MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Verification of your txn is under process.\\n Please wait for your Transaction to be verified");
						//ResponseType response = (ResponseType)md.Run ();
						//md.Destroy();
                        //MessageBox.Show("Verification of your txn is under process.\n Please wait for your Transaction to be verified");
                        return false;
                        
                    }
                    else if (trimmed_response.Contains("trialexpired"))
                    {
                        statusMessage = "trialexpired";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        //MessageBox.Show("Your 3 Days Trial Version has Expired.");
                        return false;
                    }
                    else if (trimmed_response.ToLower() == "suspended")
                    {
                        statusMessage = "suspended";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        return false;
                    }
                    else if (trimmed_response.Contains("no record found"))
                    {
                        statusMessage = "norecordfound";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        return false;
                    }
                    else
                    {
                        statusMessage = "Some Error in Status Field";
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Licence Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        return false;
                    }

                }
                else
                {
                    statusMessage = "ServerDown";
                    //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID Sorry:" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    return false;
                }

                #endregion
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
                statusMessage = "Error in License Validation";
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message+"ValidateCPUID", PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
                //MessageBox.Show(ex.StackTrace);
            }
            return false;
        } 
        #endregion

        #region RegisterUser
        public string RegisterUser(string Username, string Password, string cpuID, string TransactionID, string Email, string servr)
        {
			GlobusHttpHelper HttpHelpr = new GlobusHttpHelper();
            string res = string.Empty;
			string url = string.Empty;
			//getHtmlfromUrl(new Uri(Photolink), "","");
			url = "http://" + servr + "/register.php?user=" + Username + "&pass=" + Password + "&cpid=" + cpuID + "&transid=" + TransactionID + "&email=" + Email + "&LicType="+Globals.licType+" ";
			

            try
            {
				res = HttpHelpr.getHtmlfromUrl(new Uri(url),"","","");
				//res = HttpHelpr.GetHtml(url);

                if (string.IsNullOrEmpty(res))
                {
                    System.Threading.Thread.Sleep(1000);
					res = HttpHelpr.getHtmlfromUrl(new Uri(url),"","","");
					//res = HttpHelpr.GetHtml(url);
                }

                if (string.IsNullOrEmpty(res))
                {                  
					Gtk.Application.Quit();
                }
            }
            catch (Exception ex)
            {
				GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
            }
            return res;
        } 
        #endregion

      
        #region ValidateCPUID
        public bool ValidateCPUID(ref string statusMessage, string servr, ref string username, ref string password, ref string txnID, ref string email, string freeTrialKey, string cpuID)
        {
            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("UserName: " + username + " CPUID:" + cpuID, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

            try
            {
                #region Drct
                //string cpuID = FetchMacId();
                //string SelectQuery = "Select * from users where cpuid='" + cpuID + "'";
                //DataSet ds = DataBaseHandler.SelectQuery(SelectQuery, "users");
                //if (ds.Tables[0].Rows.Count == 1)
                //{
                //    string status = ds.Tables[0].Rows[0]["status"].ToString();
                //    if (status.ToLower() == "active")
                //    {
                //        statusMessage = "active";
                //        return true;
                //    }
                //    else if (status.ToLower() == "nonactive")
                //    {
                //        statusMessage = "nonactive";
                //        return false;
                //    }
                //    else if (status.ToLower() == "suspended")
                //    {
                //        statusMessage = "suspended";
                //        return false;
                //    }
                //}

                #endregion

                #region Through php

                //string cpuID = FetchMacId();
                //ChilkatHttpHelpr HttpHelpr = new ChilkatHttpHelpr();
				HttpHelpr = new GlobusHttpHelper();

                #region Servr 1
                {
                    string res = string.Empty;

					string url = "http://" + servr + "/GetUserData.php?cpid=" + cpuID + "";
					res = HttpHelpr.getHtmlfromUrl(new Uri(url), "", "", "");
					res = HttpHelpr.getHtmlfromUrl(new Uri(url), "", "", "");

                    if (string.IsNullOrEmpty(res))
                    {
                        System.Threading.Thread.Sleep(1000);
						res = HttpHelpr.getHtmlfromUrl(new Uri(url),"","", "");
                    }

                    if (!string.IsNullOrEmpty(res))
                    {
                        string activationstatus = string.Empty;
                        string dateTime = string.Empty;
                        //string username = string.Empty;
                        //string txnID = string.Empty;

                        string trimmed_response = res.Replace("<pre>", "").Replace("</pre>", "").Trim().ToLower();

                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Responce):" + trimmed_response, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

                        string[] array_status = System.Text.RegularExpressions.Regex.Split(trimmed_response, "<:>");

                        try
                        {
                            activationstatus = array_status[0].ToLower();
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Status):" + activationstatus, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        }
						catch(Exception ex) 
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}
                        try
                        {
                            dateTime = array_status[1].ToLower();
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenseDate):" + dateTime, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        }
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}
                    
                        try
                        {
                            username = array_status[2].ToLower();
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Username):" + username, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

                        }
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}
                        try
                        {
                            password = array_status[3].ToLower();
                            //GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Username):" + username, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

                        }
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}

                        try
                        {
                            txnID = array_status[4].ToLower();
                        }
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}

                        try
                        {
                            email = array_status[5].ToLower();
                            //GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Username):" + username, Globals.DesktopFolder + "\\LogLicensingProcess.txt");

                        }
						catch(Exception ex)
						{
							GlobusFileHelper.AppendStringToTextfileNewLine(ex.Message, PDGlobals.pathErrorLog+ "\\ErrorLogLicensing.txt");
						}

						try
						{
							Globals.licType = array_status[6].ToLower();
						}
						catch { }
                    
                        if (trimmed_response.ToLower().Contains(freeTrialKey) && ((activationstatus.ToLower() == "active") || (activationstatus.ToLower() == "nonactive")))
                        {
                            //Globals.IsFreeVersion = true ;

                            if (CheckActivationUpdateStatus(cpuID, dateTime, activationstatus, servr))
                            {
                                statusMessage = "active";
                                //servr = "1";
                                return true;
                            }
                            else
                            {
                                statusMessage = "trialexpired";
                                return false;
                            }

                            if (activationstatus.ToLower() == "active")
                            {
                                statusMessage = "active";
                                //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Status):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                                //servr = "1";
                                return true;
                            }
                            else if (activationstatus.ToLower() == "nonactive")
                            {
                                //Update status as Active
								string url1 = "http://" + servr + "/UpdateStatus.php?cpid=" + cpuID + "&status=" + "Active";
								string updateRes = HttpHelpr.getHtmlfromUrl(new Uri(url1),"","", "");
                                if (string.IsNullOrEmpty(updateRes))
                                {
                                    System.Threading.Thread.Sleep(1000);
									updateRes = HttpHelpr.getHtmlfromUrl(new Uri(url1),"","", "");
                                }

                               // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(Your Free Version is Activated):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                               // MessageBox.Show("Your Free Version is Activated");
                                return true;
                            }

                        
                        }
                        else if (activationstatus.ToLower() == "active")
                        {
                            statusMessage = "active";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return true;
                            // DisableControls();
                        }
                        else if (activationstatus.ToLower() == "nonactive")
                        {
                            statusMessage = "nonactive";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                           // MessageBox.Show("Verification of your txn is under process.\n Please wait for your Transaction to be verified.\n Please Contact To Support Team to activate your license,   Skype Id Is :- Facedominatorsupport");
                            return false;
                            // DisableControls();
                        }

                        else if (trimmed_response.Contains("trialexpired"))
                        {
                            statusMessage = "trialexpired";
                            //need to know the naviagtion site : LinkedinDominator.com
                           // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                           // MessageBox.Show("Your 3 Days Trial Version has Expired. Please visit our site: http://linkeddominator.com/ to purchase your License");
                            return false;
                        }
                        else if (trimmed_response.ToLower() == "suspended")
                        {
                            statusMessage = "suspended";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return false;
                        }
                        else if (trimmed_response.Contains("no record found"))
                        {
                            statusMessage = "norecordfound";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return false;
                        }
                        else
                        {
                            statusMessage = "Some Error in Licensing Server";
                            //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                            return false;
                        }

                    }
                    else
                    {
                        statusMessage = "ServerDown";
                       // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("ValidateCPUID(LicenceStatus2):" + statusMessage, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                        return false;
                    }
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                statusMessage = "Error in License Validation";
                Loger("Error in License Validation");
                //MessageBox.Show(ex.StackTrace);
            }
            return false;
        } 
        #endregion

        #region CheckActivationUpdateStatus
        private bool CheckActivationUpdateStatus(string cpuID, string dateTime, string status, string servr)
        {
            try
            {
                string strdateTime_DataBase = dateTime;
				DateTime dt = DateTime.Parse(strdateTime_DataBase);
				strdateTime_DataBase = dt.ToString("yyyy-MM-dd hh:mm:ss");
				string url = "http://" + servr + "/datetime.php";

				string res_ServerDateTime = HttpHelpr.getHtmlfromUrl(new Uri(url),"","", "");
                if (string.IsNullOrEmpty(res_ServerDateTime))
                {
                    System.Threading.Thread.Sleep(1000);
					res_ServerDateTime = HttpHelpr.getHtmlfromUrl(new Uri(url),"","", "");
                }

                DateTime dt_now = DateTime.Parse(res_ServerDateTime);

                TimeSpan dt_Difference = dt_now.Subtract(dt);

                if (dt_Difference.Days > 3)
                {
					string url1 = "http://" + servr + "/UpdateStatus.php?cpid=" + cpuID + "&status=" + "TrialExpired";
					string updateRes = HttpHelpr.getHtmlfromUrl(new Uri(url1),"","", "");

                    if (string.IsNullOrEmpty(updateRes))
                    {
                        System.Threading.Thread.Sleep(1000);
						updateRes = HttpHelpr.getHtmlfromUrl(new Uri(url1),"","", "");
                        Loger("cpuid=" + cpuID + "&status=" + "TrialExpired");
                       // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("cpid=" + cpuID + "&status=" + "TrialExpired" + cpuID, Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    }

                    //need to know the naviagtion site : LinkedinDominator.com
                    Loger("Your 3 Days Trial Version has Expired. Please visit our site: http://linkeddominator.com/ to purchase your License");
                   // BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("Your 3 Days Trial Version has Expired. Please visit our site: http://linkeddominator.com/ to purchase your License", Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                   // MessageBox.Show("Your 3 Days Trial Version has Expired. Please visit our site: http://linkeddominator.com/ to purchase your License");
                    return false;
                }
                else if (status == "nonactive")
                {
                    //Update status as Active
					string url2 = "http://" + servr + "/UpdateStatus.php?cpid=" + cpuID + "&status=" + "Active";
					string updateRes = HttpHelpr.getHtmlfromUrl(new Uri(url2),"","", "");
                    if (string.IsNullOrEmpty(updateRes))
                    {
                        System.Threading.Thread.Sleep(1000);
						updateRes = HttpHelpr.getHtmlfromUrl (new Uri(url2),"", "", "");
                        //Loger("cpuid=" + cpuID + "&status=" + "Active");
                        //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("cpuid=" + cpuID + "&status=" + "Active", Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                    }
                    //Loger("Your 3 Days Trial Version is Activated");

                    //BaseLib.GlobusFileHelper.AppendStringToTextfileNewLine("Your 3 Days Trial Version is Activated", Globals.DesktopFolder + "\\LogLicensingProcess.txt");
                   // MessageBox.Show("Your 3 Days Trial Version is Activated");
                    return true;
                }

                return true;
            }
            catch { return false; }
        } 
        #endregion

        #region Loger
        private void Loger(string message)
        {
            EventsArgs eventArgs = new EventsArgs(message);
            LinkedLiceneManager.LogText(eventArgs);
        } 
        #endregion

    }
}
