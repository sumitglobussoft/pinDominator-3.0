using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using log4net.Config;
using PinDominator3.Pages.PageAccount;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
        public MainWindow()
        {
            XmlConfigurator.Configure();
            InitializeComponent();
            CopyDatabase();
            UploadAccount objUploadAccount = new UploadAccount();
            objUploadAccount.AccounLoad();
            mainFormReference = this;
                   
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void CopyDatabase()
        {

            string startUpDB = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DB_PinDominator.db";
            string localAppDataDB = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PinDominator.db";

            string startUpDB64 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\DB_PinDominator.db";

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
