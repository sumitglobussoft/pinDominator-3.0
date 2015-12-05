using System;
using System.Collections.Generic;
using System.Linq;
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
using BaseLib;
using Globussoft;
using System.Threading;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using PinsManager;
using PinDominator3.CustomUserControl;

namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class RePin : UserControl
    {
        public RePin()
        {
            InitializeComponent();
        }
        PinInterestUser objPinUser = new PinInterestUser();
        RePinManager objRePinManager = new RePinManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btu_UploadRepinNo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstRepinUrl.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtRepinNo_Repin.Text = dlg.FileName.ToString();
                     readPin(dlg.FileName);
                }                               
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        public void readPin(object filePath)
        {
            try
            {
                List<string> lstRepinUrl1 = GlobusFileHelper.ReadFile(txtRepinNo_Repin.Text.Trim());

                Thread thrAddAndTest = new Thread(() => AddAndTestPins(lstRepinUrl1));
                thrAddAndTest.Start();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
        public void AddAndTestPins(List<string> lstRepinUrl1)
        {
            try
            {
                objPinUser.globusHttpHelper = new GlobusHttpHelper();
                foreach (var lstRepinUrl_item in lstRepinUrl1)
                {
                    if (lstRepinUrl_item.Contains("https://www.pinterest.com/pin/") || lstRepinUrl_item.Contains("http://www.pinterest.com/pin/"))
                    {
                        try
                        {
                            string url = lstRepinUrl_item;
                            string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
                            if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
                            {
                                ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);
                            }
                        }
                        catch { };
                    }
                    else
                    {
                        try
                        {
                            string url = "https://www.pinterest.com/pin/" + lstRepinUrl_item;
                            string CheckPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(url), "", string.Empty, "");
                            if (!CheckPinPageSource.Contains("<div>Something went wrong!</div>") && !CheckPinPageSource.Contains("<div>Sorry. We've let our engineers know.</div>") && !CheckPinPageSource.Contains("<div>Whoops! We couldn't find that page.</div>") && !CheckPinPageSource.Contains("<div class=\"suggestionText\">How about these instead?</div>"))
                            {
                                ClGlobul.lstRepinUrl.Add(lstRepinUrl_item);
                            }
                        }
                        catch { };
                    }
                }

                GlobusLogHelper.log.Info(" => [ Total Urls Uploaded : " + ClGlobul.lstRepinUrl.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btu_UploadMessageRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.RepinMessagesList.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtRepinMessage_Repin.Text = ofd.FileName.ToString();                  
                }
                ClGlobul.RepinMessagesList = GlobusFileHelper.ReadFileForRepin(txtRepinMessage_Repin.Text.Trim());
                GlobusLogHelper.log.Error(" => [ Total Message Uploaded : " + ClGlobul.RepinMessagesList.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }
      
        private void btnStartRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {                   
                    objRePinManager.isStopRePin = false;
                    objRePinManager.lstThreadsRePin.Clear();
                    clsSettingDB Database = new clsSettingDB();
                    Database.UpdateSettingData("Repin", "RepinMsgFile", StringEncoderDecoder.Encode(txtRepinMessage_Repin.Text));
                    Database.UpdateSettingData("Repin", "RepinNO", StringEncoderDecoder.Encode(txtRepinNo_Repin.Text));

                    ClGlobul.lstPins.Clear();                   

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objRePinManager.minDelayRePin = Convert.ToInt32(txtRepin_DelayMin.Text);
                            objRePinManager.maxDelayRePin = Convert.ToInt32(txtRepin_DelayMax.Text);
                            objRePinManager.Nothread_RePin = Convert.ToInt32(txtRepinNoOfThreads_RePin.Text);
                            objRePinManager.maxNoOfRePinCount = Convert.ToInt32(txtRepinCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }
                        Regex checkNo = new Regex("^[0-9]*$");

                        if (!string.IsNullOrEmpty(txtRepinNoOfThreads_RePin.Text) && checkNo.IsMatch(txtRepinNoOfThreads_RePin.Text))
                        {
                            threads = Convert.ToInt32(txtRepinNoOfThreads_RePin.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objRePinManager.NoOfThreadsRePin = threads;

                        Thread RePinThread = new Thread(objRePinManager.StartRepin);
                        RePinThread.Start();
                    }

                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info("Please Load Accounts !");
                    GlobusLogHelper.log.Debug("Please Load Accounts !");

                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }    

        private void rbo_RepinNormalType_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rdbRepinNormalType = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rbo_RepinUserRepin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnRepinUrlUplaod.Visibility = Visibility.Hidden;
                Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rbdRepinUserRepin = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void rdo_UsePinNo_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnRepinUrlUplaod.Visibility = Visibility.Visible;
                Brow_Repin_Messge.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                RePinManager.rdbUsePinNo = true;
                txtRepinNo_Repin.IsReadOnly = true;
                txtRepinMessage_Repin.IsReadOnly = true;
            }

            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btnStopRepin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objRePinManager._IsfevoriteRepin = true;
                List<Thread> lstTempRePin = objRePinManager.lstThreadsRePin.Distinct().ToList();
                foreach (Thread item in lstTempRePin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("----------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("----------------------------------------------------------------------------");
            }
             catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void chkDivideData_Repin_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RePinManager.chkDivideData_RePin = true;
                this.Dispatcher.Invoke((Action)delegate
                    {
                        var modernDialog = new ModernDialog()
                        {
                            Content = new UserControl_RePin_DivideData()
                        };
                        modernDialog.MinWidth = 600;
                        modernDialog.MinHeight = 300;
                        modernDialog.ShowDialog();
                    });
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

       
       
      
    }
}
