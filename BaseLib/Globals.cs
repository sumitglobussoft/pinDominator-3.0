using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BaseLib
{

  
    public class Globals
    {
       // public static string Path_ProxySettingErroLog = AppdataFolder + "\\ErrorProxySetting.txt";
        public static string Path_ExsistingProxies = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\DB_PinDominator.db";
        public static List<string> listAccounts = new List<string>();

        public static int followingCountLogin = 0;

        public static AccountMode accountMode;
        public static string licType = string.Empty;

        public static bool IsFreeVersion = false;
        public static bool IsProVersion = false;
        public static bool IsBasicVersion = false;

        public static string DeCaptcherHost = string.Empty;
        public static string DeCaptcherPort = string.Empty;
        public static string DeCaptcherUsername = string.Empty;
        public static string DeCaptcherPassword = string.Empty;

        public static string DBCUsername = string.Empty;
        public static string DBCPassword = string.Empty;

        public static string EmailsFilePath = string.Empty;

        public static string checkcheduleSetting = string.Empty;

        public static bool CheckStopAccountCreation = true;
        public static string CheckLicenseManager = string.Empty;
        public static string LicenseCheckUserName = string.Empty;
        public static string LicenseCheckDate = string.Empty;
        public static string Licence_Details = string.Empty;
        public static string GroupReportExprotPicFilePath = string.Empty;
        public static bool checkCampaignCompleted = false;
        
        Regex IdCheck = new Regex("^[0-9]*$");

        public static string Path_TwtErrorLogs { get; set; }

        public static string Path_ExsistingPvtProxies { get; set; }

        public static string SelectedUserName = string.Empty;
        public static string ItemSelect = string.Empty;

        public static Dictionary<string, string> SelectedItem = new Dictionary<string, string>();

        public static bool _boolCheckingNow = false;
        public static List<string> lstCheckAccounts = new List<string>();

        //public static List<string> templist = listEmails;

       
    }

    public enum AccountMode
    {
        NoProxy, PublicProxy, PrivateProxy
    }

    public enum Module
    {
        AddingBoardPins, AddingKeywordPins
    }

}
