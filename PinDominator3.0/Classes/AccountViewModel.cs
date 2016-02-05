using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominator3.Classes
{
    public class AccountViewModel
    {
       
        private readonly ObservableCollection<AccountNotifyPropertyChanged> _userAccountDetails = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static  ObservableCollection<AccountNotifyPropertyChanged> _listOfAccount = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReport = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportComment = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportCommentByKeyword = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportLike = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportRepin = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportEditPin = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportPinScraper = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportLikeByKeyword = new ObservableCollection<AccountNotifyPropertyChanged>();
        public static ObservableCollection<AccountNotifyPropertyChanged> _listAccReportRepinByKeyword = new ObservableCollection<AccountNotifyPropertyChanged>();

        public ObservableCollection<AccountNotifyPropertyChanged> UserAccountDetails
        {
            get
            {
                return _userAccountDetails;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListOfAccount
        {
            get
            {
                return _listOfAccount;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReport
        {
            get
            {
                return _listAccReport;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportComment
        {
            get
            {
                return _listAccReportComment;
            }
        }
        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportCommentByKeyword
        {
            get
            {
                return _listAccReportCommentByKeyword;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportLike
        {
            get
            {
                return _listAccReportLike;
            }
        }
        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportRepin
        {
            get
            {
                return _listAccReportRepin;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportEditPin
        {
            get
            {
                return _listAccReportEditPin;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportPinScraper
        {
            get
            {
                return _listAccReportPinScraper;
            }
        }

        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportLikeByKeyword
        {
            get
            {
                return _listAccReportLikeByKeyword;
            }
        }
        public ObservableCollection<AccountNotifyPropertyChanged> ListAccReportRepinByKeyword
        {
            get
            {
                return _listAccReportRepinByKeyword;
            }
        }
       
    }
}
