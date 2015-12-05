using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using ScraperManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace PinDominator3.Pages.PageScraper
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Scraper : UserControl
    {
        public Scraper()
        {
            InitializeComponent();
        }


        ScraperManager objScraperManager = new ScraperManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnScraperStart_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtScraperUsername.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Username ");
                            ModernDialog.ShowMessage("Please Upload Username", "Upload Username", MessageBoxButton.OK);
                            return;
                        }
                        else
                        {
                            objScraperManager.UserName = txtScraperUsername.Text.Trim();
                        }

                        ClGlobul.lstTotalUserScraped.Clear();
                        objScraperManager.isStopScraper = false;
                        objScraperManager.lstThreadsScraper.Clear();

                        if (objScraperManager._IsfevoriteScraper)
                        {
                            objScraperManager._IsfevoriteScraper = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objScraperManager.minDelayScraper = Convert.ToInt32(txtScraper_DelayMin.Text);
                                objScraperManager.maxDelayScraper = Convert.ToInt32(txtScraper_DelayMax.Text);
                                objScraperManager.Nothread_Scraper = Convert.ToInt32(txtScraper_NoOfThreads.Text);
                                objScraperManager.MaxCountScraper = Convert.ToInt32(txtUserScraperCount.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtScraper_NoOfThreads.Text) && checkNo.IsMatch(txtScraper_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtScraper_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objScraperManager.NoOfThreadsScraper = threads;

                            Thread ScraperThread = new Thread(objScraperManager.StartScraper);
                            ScraperThread.Start();
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

        private void rdoFollowerScraper_Checked(object sender, RoutedEventArgs e)
        {
            objScraperManager.UserScraperType = "followers";
        }

        private void rdoFollowingScraper_Checked(object sender, RoutedEventArgs e)
        {
            objScraperManager.UserScraperType = "following";
        }

        private void btnScraperExport_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                ClGlobul.lstTotalUserScraped = ClGlobul.lstTotalUserScraped.Distinct().ToList();
                GlobusLogHelper.log.Info(" => [ Start User Export Process ]");
                try
                {
                    if (rdoFollowerScraper.IsChecked == true)
                    {
                        string item = "Followers";
                        // lstTotalUserScraped.Insert(0, item);
                        GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollower);
                        GlobusLogHelper.log.Info(" => [ File Path " + PDGlobals.UserScrapedFollower + " ]");
                    }
                    else
                    {
                        string item = "Followings";
                        //  lstTotalUserScraped.Insert(0, item);
                        GlobusFileHelper.WriteListtoTextfile(ClGlobul.lstTotalUserScraped, PDGlobals.UserScrapedFollowing);
                        GlobusLogHelper.log.Info(" => [ File Path " + PDGlobals.UserScrapedFollowing + " ]");
                    }
                }
                catch { }


                GlobusLogHelper.log.Info(" => [ User Exported Successfully ]");

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnScraperStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objScraperManager._IsfevoriteScraper = true;
                List<Thread> lstTemp = objScraperManager.lstThreadsScraper.Distinct().ToList();
                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("----------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("----------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
   
         
    
    }


}
