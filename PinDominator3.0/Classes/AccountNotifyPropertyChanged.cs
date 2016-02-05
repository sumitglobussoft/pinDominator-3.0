using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PinDominator3.Classes
{
    public class AccountNotifyPropertyChanged : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private int _count = 0;
        private string _accountName = string.Empty;
        private string _email=string.Empty;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                NotifyPropertyChanged("Count");
            }

        }
        public string AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                NotifyPropertyChanged("AccountName");
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }


        #region Property for Account Report

        private int _Id = 0; 
        private string _AccName = string.Empty;
        private string _ModuleName = string.Empty;
        private string _PinNo = string.Empty;
        private string _BoardName = string.Empty;
        private string _UserName = string.Empty;
        private string _Msg = string.Empty;
        private string _Keyword = string.Empty;
        private string _ImagUrl = string.Empty;
        private string _Status = string.Empty;
        private string _NewEmail = string.Empty;
        private string _NewPassword = string.Empty;
        private string _DateTime = string.Empty;

        public int ID
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                NotifyPropertyChanged("ID");
            }
        }
        public string AccName
        {
            get
            {
                return _AccName;
            }
            set
            {
                _AccName = value;
                NotifyPropertyChanged("AccName");
            }
        }

        public string ModuleName
        {
            get
            {
                return _ModuleName;
            }
            set
            {
                _ModuleName = value;
                NotifyPropertyChanged("ModuleName");
            }
        }

        public string PinNo
        {
            get
            {
                return _PinNo;
            }
            set
            {
                _PinNo = value;
                NotifyPropertyChanged("PinNo");
            }
        }
        public string BoardName
        {
            get
            {
                return _BoardName;
            }
            set
            {
                _BoardName = value;
                NotifyPropertyChanged("BoardName");
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                NotifyPropertyChanged("_UserName");
            }
        }

        public string Message
        {
            get
            {
                return _Msg;
            }
            set
            {
                _Msg = value;
                NotifyPropertyChanged("Message");
            }
        }

        public string Keyword
        {
            get
            {
                return _Keyword;
            }
            set
            {
                _Keyword = value;
                NotifyPropertyChanged("Keyword");
            }
        }

        public string ImageUrl
        {
            get
            {
                return _ImagUrl;
            }
            set
            {
                _ImagUrl = value;
                NotifyPropertyChanged("ImageUrl");
            }
        }

        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public string NewEmail
        {
            get
            {
                return _NewEmail;
            }
            set
            {
                _NewEmail = value;
                NotifyPropertyChanged("NewEmail");
            }
        }

        public string NewPassword
        {
            get
            {
                return _NewPassword;
            }
            set
            {
                _NewPassword = value;
                NotifyPropertyChanged("NewPassword");
            }
        }

        public string DateTime
        {
            get
            {
                return _DateTime;
            }
            set
            {
                _DateTime = value;
                NotifyPropertyChanged("DateTime");
            }
        }
        #endregion

        #region Property For Account Load

        private string _Username = string.Empty;
        private string _Password = string.Empty;
        private string _Niche = string.Empty;
        private string _ScreenName = string.Empty;
        private string _FollowerCount = string.Empty;
        private string _FollowingCount = string.Empty;
        private string _ProxyAddress = string.Empty;
        private string _ProxyPort = string.Empty;
        private string _ProxyUserName = string.Empty;
        private string _ProxyPassword = string.Empty;
       // private string _GroupName = string.Empty;
        //private string _AccountStatus = string.Empty;
        private string _LoginStatus = string.Empty;
        private string _BackgroundColor = string.Empty;
       
       
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                NotifyPropertyChanged("Username");
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                NotifyPropertyChanged("Password");
            }

        }
        public string ScreenName
        {
            get
            {
                return _ScreenName;
            }
            set
            {
                _ScreenName = value;
                NotifyPropertyChanged("ScreenName");
            }
        }
        public string FollowerCount
        {
            get
            {
                return _FollowerCount;
            }
            set
            {
                _FollowerCount = value;
                NotifyPropertyChanged("FollowerCount");
            }
        }
        public string FollowingCount
        {
            get
            {
                return _FollowingCount;
            }
            set
            {
                _FollowingCount = value;
                NotifyPropertyChanged("FollowingCount");
            }
        }
        public string ProxyAddress
        {
            get
            {
                return _ProxyAddress;
            }
            set
            {
                _ProxyAddress = value;
                NotifyPropertyChanged("ProxyAddress");
            }
        }
        public string ProxyPort
        {
            get
            {
                return _ProxyPort;
            }
            set
            {
                _ProxyPort = value;
                NotifyPropertyChanged("ProxyPort");
            }
        }
        public string ProxyUserName
        {
            get
            {
                return _ProxyUserName;
            }
            set
            {
                _ProxyUserName = value;
                NotifyPropertyChanged("ProxyUserName");
            }
        }
        public string ProxyPassword
        {
            get
            {
                return _ProxyPassword;
            }
            set
            {
                _ProxyPassword = value;
                NotifyPropertyChanged("ProxyPassword");
            }
        }
        public string Niche
        {
            get
            {
                return _Niche;
            }
            set
            {
                _Niche = value;
                NotifyPropertyChanged("Niche");
            }
        }
       
        public string LoginStatus
        {
            get
            {
                return _LoginStatus;
            }
            set
            {
                _LoginStatus = value;
                NotifyPropertyChanged("LoginStatus");
            }
        }
        public string BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                _BackgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }
}
