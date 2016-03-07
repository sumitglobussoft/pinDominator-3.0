using Globussoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace PinDominator
{
    public class PDGlobals
    {
          /// <summary>
        /// Contains all the accounts and related Information
        /// </summary>       
        // DBC Setting

        #region Global variable

        public static string dbcUserName = string.Empty;
        public static string dbcPassword = string.Empty;

		public static string typeOfOperatingSystem = string.Empty;

        //public static Dictionary<string, Accounts> loadedAccountsDictionary = new Dictionary<string, Accounts>();
        public static Dictionary<string, PinInterestUser> loadedAccountsDictionary = new Dictionary<string, PinInterestUser>();
        public static List<string> listAccounts = new List<string>();
       
        public readonly string registrationSuccessString = "\"registration_succeeded\":true";
        public readonly string registrationErrorString = "\"error\":";       

        public bool isfreeversion = false;
        public static bool Check_likephoto_Byusername = false;
     
        public static string Pindominator_Folder_Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator";
        public static string InviteSentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\InviteSent.txt";

        //Scraper Module
        public static string UserScrapedFollower = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\UserScrapedFollowers.txt";
        public static string UserScrapedFollowing = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\UserScrapedFollowings.txt";

        //EditPinDescription
        public static string path_PinDescription = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\PinDescriptionReport.txt";     
      
        //Pin Scraper
        public static string path_Image = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\ScrapedImageFolder";
        public static string PinScraped = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator\\PinScraped.txt";

       

        /// <summary>
        /// Singleton object declaration.
        /// </summary>
        
        private static volatile PDGlobals globals = null;
        private static object syncRoot = new object();

     #endregion

		#region Windows
		public static string path_AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator";
		public static string path_DesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\PinDominator";

		public static string pathErrorLog =path_DesktopFolder;

		#endregion

		#region Linux
		public static string path_LinuxAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/PinDominator";
		public static string path_LinuxDesktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/PinDominator";
		#endregion

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

        public static PDGlobals Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (globals == null)
                    {
                        globals = new PDGlobals();
                        
                    }
                }
            return globals;
            }
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



        /// <summary>
        /// 
        /// </summary>
        private PDGlobals()
        {
        }




    }
}
