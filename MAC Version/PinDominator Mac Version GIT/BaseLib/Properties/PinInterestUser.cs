using Globussoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib;

namespace PinDominator
{
  public  class PinInterestUser
    {

        public Guid Id { get; set; }
        public string Username;
        public string Password;
        public string Niches;
        public string Name;
        public string ProxyAddress;
        public string ProxyPort;
        public string ProxyUsername;
        public string ProxyPassword;
        public string UserAgent;
        public string BoardsName;
        public string ScreenName; 
        public string ProxyType; //http or socks
        public string LoginStatus;

        public GlobusHttpHelper globusHttpHelper;

        public enum AccountStatus
        {
            AccountCreated, AccountEmailVerified, AccountPhoneVerified, AccountIncorrectEmail, AccountPhoneEmailVerified,
            AccountTempLocked, AccountUndefinedError, Account30DaysBlock, AccountUnverified, AccountPVARequired, AccountDisabled
        };
        
        public bool isloggedin;    
        public string dbcUsername;
        public string dbcPassword;
        public DataSet ds;
        public string App_version;
        public string Token;

        public List<string> Boards = new List<string>();
        public List<string> lstBoardId = new List<string>();
        public List<string> lstUserFollowers = new List<string>();
      

        public PinInterestUser(string Username, string Password, string ProxyAddress, string ProxyPort)
        {
            this.Username = Username;
            this.Password = Password;
            this.ProxyAddress = ProxyAddress;
            this.ProxyPort = ProxyPort;
        }

        public PinInterestUser()
        {
            
        }



    }
}
