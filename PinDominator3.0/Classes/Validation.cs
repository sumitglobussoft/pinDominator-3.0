using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominator3.Classes
{
    class Validation: NotifyPropertyChanged, IDataErrorInfo
    {
        private string _delaymaximum = string.Empty;
        private string _delayminimum = string.Empty;
        private string _NoThread = string.Empty;
        private string _UserTxtField = string.Empty;
        private string _UserTxtField_two = string.Empty;
        private string _UserTxtField_three = string.Empty;
        private string _password = string.Empty;
   
       

        public string maxi
        {
            get { return this._delaymaximum; }
            set
            {
                if (this._delaymaximum != value)
                {
                    this._delaymaximum = value;
                    OnPropertyChanged("maxi");
                }
            }
        }

        public string mini
        {
            get { return this._delayminimum; }
            set
            {
                if (this._delayminimum != value)
                {
                    this._delayminimum = value;
                    OnPropertyChanged("mini");

                }
            }
        }

        public string Thread
        {
            get { return this._NoThread; }
            set
            {
                if (this._NoThread != value)
                {
                    this._NoThread = value;
                    OnPropertyChanged("Thread");
                }
            }
        }

        public string TxtUserField
        {
            get { return this._UserTxtField; }
            set
            {
                if (this._UserTxtField != value)
                {
                    this._UserTxtField = value;
                    OnPropertyChanged("TxtUserField");
                }
            }
        }

        public string TxtField_two
        {
            get { return this._UserTxtField_two; }
            set
            {
                if (this._UserTxtField_two != value)
                {
                    this._UserTxtField_two = value;
                    OnPropertyChanged("TxtField_two");
                }
            }
        }

        public string TxtField_three
        {
            get { return this._UserTxtField_three; }
            set
            {
                if (this._UserTxtField_three != value)
                {
                    this._UserTxtField_three = value;
                    OnPropertyChanged("TxtField_three");
                }
            }
        }

        public string password
        {
            get { return this._password; }
            set
            {
                if (this._password != value)
                {
                    this._password = value;
                    OnPropertyChanged("password");

                }
            }
        }


       

        public string Error
        {
            get { return null; }
        }


        public string this[string columnName]
        {
            get
            {
                if (columnName == "maxi")
                {
                    return string.IsNullOrEmpty(this._delaymaximum) ? "Required " : null;
                }
                if (columnName == "mini")
                {
                    return string.IsNullOrEmpty(this._delayminimum) ? " Required" : null;
                }
                if (columnName == "Thread")
                {
                    return string.IsNullOrEmpty(this._NoThread) ? " Required" : null;
                }
                if (columnName == "TxtUserField")
                {
                    return string.IsNullOrEmpty(this._UserTxtField) ? "Required" : null;
                }
                if (columnName == "TxtField_two")
                {
                    return string.IsNullOrEmpty(this._UserTxtField_two) ? "Required" : null;
                }
                if (columnName == "TxtField_three")
                {
                    return string.IsNullOrEmpty(this._UserTxtField_three) ? "Required" : null;
                }
                if (columnName == "password")
                {
                    return string.IsNullOrEmpty(this._password) ? "Password Required" : null;
                }
                return null;
            }
        }


    }
}

    







    

