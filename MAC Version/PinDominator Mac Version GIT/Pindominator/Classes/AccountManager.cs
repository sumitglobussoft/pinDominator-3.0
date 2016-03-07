using System;
using System.Collections.Generic;
using Globussoft;


namespace PinDominator
{
	public class FacebookAccountManager
	{
		public FacebookAccountManager ()
		{
		}

		public string Username = string.Empty;
		public string Password = string.Empty;

		public string userID = string.Empty;
		public string proxyAddress = string.Empty;
		public string proxyPort = string.Empty;
		public string proxyUsername = string.Empty;
		public string proxyPassword = string.Empty;

		public string AccountStatus = string.Empty;

		public GlobusHttpHelper globusHttpHelper = new GlobusHttpHelper();
		public bool IsLoggedIn = false;
		public bool IsNotSuspended = false;



		public FacebookAccountManager(string Username, string Password, string proxyAddress, string proxyPort, string proxyUsername, string proxyPassword, string status)
		{
			this.Username = Username;
			this.Password = Password;
			this.proxyAddress = proxyAddress;
			this.proxyPort = proxyPort;
			this.proxyUsername = proxyUsername;
			this.proxyPassword = proxyPassword;
			this.AccountStatus = status;
			//Log("[ " + DateTime.Now + " ] => [ Logging in with Account:" + Username + " ]");
		}


	}


	public class AccountContainer
	{
		public static Dictionary<string, FacebookAccountManager> dictionary_FacebookAccount = new Dictionary<string, FacebookAccountManager>();

	}
}

