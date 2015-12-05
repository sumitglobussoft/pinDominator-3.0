using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Globussoft;
using PinDominator.Account;

namespace PinDominator.Globals
{
    public class ApplicationData
    {
        public static string FolderCreation(string path, string Folder)
        {
            if (!Directory.Exists(path + "\\" + Folder))
            {
                DirectoryInfo di = Directory.CreateDirectory(path + "\\" + Folder);
                return path + "\\" + Folder;
            }
            else
            {
                return path + "\\" + Folder;
            }
            return null;
        }

        public static bool IsOpenOnceAccountLoad = true;
        public static Dictionary<string, PinterestAccountManager> PinterestAccountDictionary = new Dictionary<string, PinterestAccountManager>();
       
        public static string PintrestFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator";
        public static string PintrestAppdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator";

        public static string ProgressReport = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\ProgressReport.txt";
        public static string ActiveAccountsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\ActiveAccounts.txt";
        public static string NotActiveAccountsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\NotActiveAccounts.txt";
        public static string NewAccountsPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\NewAccounts.txt";
     //   public static string UserScrapedFollower = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\UserScrapedFollowers.csv";
        public static string UserScrapedFollower = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\UserScrapedFollowers.txt";
        public static string UserScrapedFollowing = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\UserScrapedFollowings.txt";

        public static string PinScraped = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\PinScraped.txt";


        public static string Pindominator_Folder_Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator";
        public static string NormalRepinPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\Repin\\";
        public static string NormalLike = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\Like\\NormalLike.csv";
      //My added filepath is above here
        
        public static string InviteSentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\InviteSent.txt";
        public static string path_FailedLoginAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\FailedLogInAccounts.txt";
        public static string PintrestErrorLog = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\ErrorLog.txt";
        public static string path_FailedToProfileAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\FailedToProfileAccounts.txt";
        public static string path_SuccessfullyProfiledAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\SuccessfullyProfiledAccounts.txt";
        public static string path_UnSuccessfullyProfiledAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\FailedToProfiledAccounts.txt";
        public static string path_SuccessfullyCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\SuccessfullyCreatedAccounts.txt";
        public static string path_UnSuccessfullyCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\FailedToCreateAccounts.txt";
        public static string path_SuccessfullyVerfiedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\SuccessfullyVerifiedAccounts.txt";
        public static string path_UnSuccessfullyVerfiedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\FailedToVerifyAccounts.txt";
        public static string path_AlredyCreatedAccounts = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\AlreadyCreatedAccounts.txt";
        public static string path_Repin = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\RepinDetails.csv";
        public static string path_Image = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\ScrapedImageFolder";

        public static string path_AccountsWithNiches = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\AccountsWithNiches.txt";
        public static string Path_AddingPinsSetting = PintrestAppdataFolder + "\\AddingPins.txt";
        public static string path_PinDescription = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\PinDescriptionReport.txt";

        public static string PinCateogry = string.Empty;

        public static string ErrorLogFile = PintrestAppdataFolder + "\\ErrorLogFile.txt";

        //**for DBC*************
        public static string DBCUsername = string.Empty;
        public static string DBCPassword = string.Empty;
        public static string AccountFilePath = string.Empty;

        public static bool ValidateNumber(string strInputNo)
        {
            //Regex IdCheck = new Regex("^[0-9]*$");
            Regex IdCheck = new Regex(@"^(0?[1-9])|([1-4][0-9])|(50)$");//(@"[\d]");

            if (!string.IsNullOrEmpty(strInputNo) && IdCheck.IsMatch(strInputNo))
            {
                return true;
            }

            return false;
        }

        public static void ExportDataCSVFile(string CSV_Header, string CSV_Content, string CSV_FilePath)
        {
            try
            {
                if (!File.Exists(CSV_FilePath))
                {
                    GlobusFileHelper.AppendStringToTextFile(CSV_FilePath, CSV_Header);
                }

                GlobusFileHelper.AppendStringToTextFile(CSV_FilePath, CSV_Content);
            }
            catch (Exception)
            {

            }
        }
    }
}
