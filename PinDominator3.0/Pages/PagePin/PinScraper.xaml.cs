using PinsManager;
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
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using Globussoft;

namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class PinScraper : UserControl
    {
        public PinScraper()
        {
            InitializeComponent();
        }

        ScrapePinManagers objScrapePinManagers = new ScrapePinManagers();
        Utils.Utils objUtils = new Utils.Utils();

        private void ChkScrapeImage_PinScraper_Checked(object sender, RoutedEventArgs e)
        {
            ScrapePinManagers.chkScrapeImage = true;
        }

        private void btnScrapePins_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txt_BoardUrl_pinscraper.Text))
                        {
                            GlobusLogHelper.log.Info("Please Enter BoardUrl ");
                            ModernDialog.ShowMessage("Please Enter BoardUrl", "Enter BoardUrl", MessageBoxButton.OK);
                            return;
                        }
                        else
                        {
                            objScrapePinManagers.BoardUrl = (txt_BoardUrl_pinscraper.Text).ToString();
                        }

                        objScrapePinManagers.isStopPinScraper = false;
                        objScrapePinManagers.lstThreadsPinScraper.Clear();

                        if (objScrapePinManagers._IsfevoritePinScraper)
                        {
                            objScrapePinManagers._IsfevoritePinScraper = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objScrapePinManagers.minDelayPinScraper = Convert.ToInt32(txtPinScraper_DelayMin.Text);
                                objScrapePinManagers.maxDelayPinScraper = Convert.ToInt32(txtPinScraper_DelayMax.Text);
                                objScrapePinManagers.NoOfThreadsPinScraper = Convert.ToInt32(txtPinScraper_NoOfThreads.Text);
                                objScrapePinManagers.MaxNoOfPinScrape = Convert.ToInt32(txtNoOfPins_PinScraper.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtPinScraper_NoOfThreads.Text) && checkNo.IsMatch(txtPinScraper_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtPinScraper_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objScrapePinManagers.NoOfThreadsPinScraper = threads;

                            Thread ScraperPinThread = new Thread(objScrapePinManagers.StartPinScraper);
                            ScraperPinThread.Start();
                        }

                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
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

        private void btnExport_PinScraper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.GetPinList = ClGlobul.GetPinList.Distinct().ToList();
                GlobusLogHelper.log.Info(" => [ Pin Export Process Started]");
                try
                {
                    GlobusFileHelper.WriteListtoTextfile(ClGlobul.GetPinList, PDGlobals.PinScraped);
                    GlobusLogHelper.log.Info(" => [ File Path " + PDGlobals.PinScraped + " ]");

                }
                catch { }

                GlobusLogHelper.log.Info(" => [ Pins Exported Successfully ]");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnStop_PinScraper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objScrapePinManagers._IsfevoritePinScraper = true;
                List<Thread> lstTemp = objScrapePinManagers.lstThreadsPinScraper.Distinct().ToList();
                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch
                    {
                    }
                }
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        

        

       

       
    }
}
