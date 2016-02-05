using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using Globussoft;
using ScraperManagers;
using System;
using System.Collections.Generic;
using System.Data;
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
            ScraperManager.objScraperDelegate = new AccountReport_Scraper(AccountReport_Scraper);
            AccountReport_Scraper();
        }

        ScraperManager objScraperManager = new ScraperManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnScraperStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startScraper();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startScraper()
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
                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnScraperStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread objStopScraper = new Thread(stopScraper);
                objStopScraper.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopScraper()
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
                    catch (Exception ex)
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

        private void chkExportScrapeData_Scraper_Checked(object sender, RoutedEventArgs e)
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
        public void AccountReport_Scraper()
        {
            try
            {
                int id = 0;
                int count = 0;
                DataSet DS = null; ;
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("UserName");            
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("Scraper");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                foreach (DataRow dt_item in DS.Tables[0].Rows)
                {
                    try
                    {
                        count++;
                        id = int.Parse(dt_item.ItemArray[0].ToString());
                       // string AccountName = dt_item.ItemArray[1].ToString();
                        string ModuleName = dt_item.ItemArray[2].ToString();
                        string UserName = dt_item.ItemArray[5].ToString();                
                        string Status = dt_item.ItemArray[9].ToString();
                        string DateAndTime = dt_item.ItemArray[12].ToString();
                        dt.Rows.Add(count, ModuleName, UserName, Status, DateAndTime);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
                try
                {
                    DataView dv;
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        dgvScraper_AccountsReport.ItemsSource = dt.DefaultView;

                    }));
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkDeleteAccReport_Scraper(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("Scraper");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_Scraper();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_Scraper(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportScraper();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportScraper()
        {
            try
            {
                if (dgvScraper_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvScraper_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "ModuleName", "UserName", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\Scraper.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("Scraper");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string UserName = item.ItemArray[5].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        CSV_Content = string.Join(",", ModuleName.Replace("'", ""), UserName.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""));
                                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Content, ExportDataLocation);
                                    }
                                    catch (Exception ex)
                                    {
                                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                            }

                        }
                    }
                    catch(Exception Ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + Ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }





    }
}
