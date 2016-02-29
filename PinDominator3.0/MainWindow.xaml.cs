using BaseLib;
using CommentManager;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using log4net.Config;
using PinDominator3.Pages.PageAccount;
using PinsManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PinDominator3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {

        public static MainWindow mainFormReference = null;

        public static RePinManager objRePinManager = new RePinManager();

        NotifyIcon objNotifyIcon = new NotifyIcon();
        public MainWindow()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            AppearanceManager.Current.AccentColor = Colors.Red;
            CopyDatabase();
            UploadAccount objUploadAccount = new UploadAccount();
            objUploadAccount.AccounLoad();
            mainFormReference = this;
           
            Thread Obj_CheckVersion = new Thread(CheckVersion);
            Obj_CheckVersion.SetApartmentState(ApartmentState.STA);
            Obj_CheckVersion.Start();

            string GetFolderPath=(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0" + "\\favicon(1).ico").Replace("\\","/");

            this.objNotifyIcon.Icon = new Icon(@GetFolderPath);

            objNotifyIcon.Click +=
            delegate(object o, EventArgs e)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            InitializeAllDelegates();//objRepin_Comments_UserPins_Repin
                   
        }

        static string thisVersionNumber = string.Empty;
        MessageBoxButton btnUsed = MessageBoxButton.YesNo;
        MessageBoxButton btnMessage = MessageBoxButton.OK;
        private void CheckVersion()
        {
            try
            {
                thisVersionNumber = GetAssemblyVersion();
                string textFileLocationOnServer = "http://licensing.facedominator.com/licensing/PD/PDCheckVer/PDLatestVersion.txt";

                GlobusHttpHelper httpHelper = new GlobusHttpHelper();
                string textFileData = httpHelper.getHtmlfromUrl(new Uri(textFileLocationOnServer));
                string verstatus = string.Empty;
                if (Globals.IsProVersion)
                {
                    verstatus = "Fdpro Version";
                }
                if (Globals.IsBasicVersion)
                {
                    verstatus = "Fdbasic Version";
                }
                else if (Globals.IsProVersion && Globals.IsBasicVersion)
                {
                    verstatus = Globals.Licence_Details.Split('&')[1];
                }


                //this.Dispatcher.Invoke(new Action(delegate
                //{

                //    this.Text = this.Text + "-" + verstatus + " (" + thisVersionNumber + ")";
                //    lbl_chckVersionUpdateLog.Text = this.Text + "-" + verstatus + " (" + thisVersionNumber + ")";
                //}));

                string latestVersion = Regex.Split(textFileData, "<:>")[0];
                string updateVersionPath = Regex.Split(textFileData, "<:>")[1];

                if (thisVersionNumber == latestVersion)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        ModernDialog.ShowMessage("You have the Updated Version", "Information", btnMessage);
                    }));

                }
                else
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        var check = ModernDialog.ShowMessage("An Updated Version Available - Do you Want to Upgrade!", "Update Available", btnUsed);
                        if (check.ToString().Equals("Yes"))
                        {
                            System.Diagnostics.Process.Start("iexplore", updateVersionPath);
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                this.Close();
                            }));
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public string GetAssemblyVersion()
        {
            string versionNumber = string.Empty;

            try
            {
                string appName = Assembly.GetAssembly(this.GetType()).Location;
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(appName);
                versionNumber = assemblyName.Version.ToString();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }

            return versionNumber;
        }

        public void InitializeAllDelegates()
        {
            CommentManagers.objRepin_Comments_UserPins_Repin += new CommentManager.Repin_Comments_UserPins_Repin(objRePinManager.UserPins_Repin);
        }



        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
       
        public void StateChangedProperty()
        {
            if (WindowState.Minimized == this.WindowState)
            {
                objNotifyIcon.Visible = true;
                objNotifyIcon.ShowBalloonTip(500, "PD_3.0", "PinDominator_3.0", ToolTipIcon.Info);
                this.Hide();
            }
            else if (WindowState.Normal == this.WindowState)
            {
                objNotifyIcon.Visible = false;
            }
        }

        private void CopyDatabase()
        {

            string startUpDB = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DB_PinDominator.db";
            string localAppDataDB = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PinDominator.db";

            string baseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string startUpDB64 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\DB_PinDominator.db";
            string startBanner = baseDir + "\\Banner-728x90_3.gif";
            string logopath = baseDir + "\\favicon(1).ico";

            string localAppLogo = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0" + "\\favicon(1).ico";
            string localBanner = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0" + "\\Banner-728x90_3.gif";

            if (!File.Exists(localBanner))
            {
                if (File.Exists(startBanner))
                {
                    try
                    {
                        File.Copy(startBanner, localBanner);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0");
                            File.Copy(startBanner, localBanner);
                        }
                    }
                }
            }
            if (!File.Exists(localAppLogo))
            {
                if (File.Exists(logopath))
                {
                    try
                    {
                        File.Copy(logopath, localAppLogo);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0");
                            File.Copy(logopath, localAppLogo);
                        }
                    }
                }
            }


            if (!File.Exists(localAppDataDB))
            {
                ///Modified [19-10] to work with 64 Bit as well

                if (File.Exists(startUpDB))
                {
                    try
                    {
                        File.Copy(startUpDB, localAppDataDB);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0");
                            File.Copy(startUpDB, localAppDataDB);
                        }
                    }
                }
                else if (File.Exists(startUpDB64))   //for 64 Bit
                {
                    try
                    {
                        File.Copy(startUpDB64, localAppDataDB);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Could not find a part of the path"))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0");
                            File.Copy(startUpDB64, localAppDataDB);
                        }
                    }
                }
            }
        }

        private void ModernWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                switch (this.WindowState)
                {
                    case WindowState.Maximized:
                        break;
                    case WindowState.Minimized:
                        StateChangedProperty();
                        break;
                    case WindowState.Normal:
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxButton btnUsed = MessageBoxButton.YesNo;
           var objDialogresult = ModernDialog.ShowMessage("Do you Really want to exit?", "PinDominator 3.0", btnUsed);
            if (objDialogresult.ToString().Equals("Yes"))
            {
                var prc = System.Diagnostics.Process.GetProcesses();
                foreach (var item in prc)
                {
                    try
                    {
                        if (item.ProcessName.Contains("PinDominator"))
                        {
                            item.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Info("Error : " + ex.StackTrace);
                    }
                }
                this.Close();
            }
            else
            {
                e.Cancel = true;
                this.Activate();
            }
        }

        private void ModernWindow_Activated(object sender, EventArgs e)
        {

        }

      

    }


    #region LogFornetclass
    public class GlobusLogAppender : log4net.Appender.AppenderSkeleton
    {

        private static readonly object lockerLog4Append = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                string loggerName = loggingEvent.Level.Name;

                MainWindow frmPinDominator = MainWindow.mainFormReference;

 
                lock (lockerLog4Append)
                {
                    switch (loggingEvent.Level.Name)
                    {
                        case "DEBUG":
                            try
                            {

                                {
                                    if (!frmPinDominator.lstLogger.Dispatcher.CheckAccess())
                                    {
                                        frmPinDominator.lstLogger.Dispatcher.Invoke(new Action(delegate
                                        {
                                            try
                                            {
                                                if (frmPinDominator.lstLogger.Items.Count > 1000)
                                                {
                                                    frmPinDominator.lstLogger.Items.RemoveAt(frmPinDominator.lstLogger.Items.Count - 1);//.Add(frmDominator.listBoxLogs.Items.Add(loggingEvent.TimeStamp + "\t" + loggingEvent.LoggerName + "\r\t\t" + loggingEvent.RenderedMessage);
                                                }

                                                frmPinDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "PinDominator 3.0 " + "\r\t" + loggingEvent.RenderedMessage);
                                            }
                                            catch (Exception ex)
                                            {
                                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                            }

                                        }));

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (frmPinDominator.lstLogger.Items.Count > 1000)
                                            {
                                                frmPinDominator.lstLogger.Items.RemoveAt(frmPinDominator.lstLogger.Items.Count - 1);
                                            }

                                            frmPinDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "PinDominator 3.0 " + "\r\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error("Error : 74" + ex.Message);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case Debug : " + ex.StackTrace);
                                Console.WriteLine("Error Case Debug : " + ex.Message);
                                GlobusLogHelper.log.Error(" Error : " + ex.Message);
                            }
                            break;
                        case "INFO":
                            try
                            {


                                if (!frmPinDominator.lstLogger.Dispatcher.CheckAccess())
                                {
                                    frmPinDominator.lstLogger.Dispatcher.Invoke(new Action(delegate
                                    {
                                        try
                                        {
                                            if (frmPinDominator.lstLogger.Items.Count > 1000)
                                            {
                                                frmPinDominator.lstLogger.Items.RemoveAt(frmPinDominator.lstLogger.Items.Count - 1);
                                            }

                                            frmPinDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "PinDominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                                        }

                                    }));

                                }
                                else
                                {
                                    try
                                    {
                                        if (frmPinDominator.lstLogger.Items.Count > 1000)
                                        {
                                            frmPinDominator.lstLogger.Items.RemoveAt(frmPinDominator.lstLogger.Items.Count - 1);
                                        }

                                        frmPinDominator.lstLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "PinDominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error("Error : 75" + ex.Message);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Case INFO : " + ex.StackTrace);
                                Console.WriteLine("Error Case INFO : " + ex.Message);
                                GlobusLogHelper.log.Error(" Error : " + ex.Message);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : 76" + ex.Message);
            }

        }


    }
    #endregion


}
