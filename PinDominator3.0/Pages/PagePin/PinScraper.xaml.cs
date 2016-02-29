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
using System.Data;
using PinDominator3.Classes;
using System.Diagnostics;

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
            ScrapePinManagers.objPinScraperDelegate = new AccountReport_PinScraper(AccountReport_PinScraper);
            AccountReport_PinScraper();
        }

        ScrapePinManagers objScrapePinManagers = new ScrapePinManagers();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void ChkScrapeImage_PinScraper_Checked(object sender, RoutedEventArgs e)
        {
            ScrapePinManagers.chkScrapeImage = true;
        }

        private void btnScrapePins_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startPinScraper();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void startPinScraper()
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
                            GlobusLogHelper.log.Info(" => [ Process Starting ] ");
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
            catch (Exception ex)
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
                Thread objStopPinScraper = new Thread(stopPinScraper);
                objStopPinScraper.Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void stopPinScraper()
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void chkExportScrapePin_PinScraper_Checked(object sender, RoutedEventArgs e)
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
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void WebPageClick(object sender, RoutedEventArgs e)
        {
            Hyperlink link = e.OriginalSource as Hyperlink;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        public void  AccountReport_PinScraper()
        {
            try
            {
                int id = 0;
                int count = 0;
                string ModuleName=string.Empty;
                string Pin = string.Empty;
                string Status = string.Empty;
                string DateAndTime=string.Empty;
                string ImageUrl = string.Empty;

                this.Dispatcher.Invoke(new Action(delegate
                {
                    AccountViewModel._listAccReportPinScraper.Clear();
                }));

                DataSet DS = null; ;
                DataTable dt = new DataTable();          
                dt.Columns.Add("ModuleName");
                dt.Columns.Add("Pin");
                dt.Columns.Add("Status");
                dt.Columns.Add("Date&Time");
                dt.Columns.Add("ImageUrl");
                DS = new DataSet();
                DS.Tables.Add(dt);
                try
                {
                    DS = QM.SelectAddReport("PinScraper");
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
                         ModuleName = dt_item.ItemArray[2].ToString();
                         Pin = dt_item.ItemArray[3].ToString();
                         Status = dt_item.ItemArray[9].ToString();
                         DateAndTime = dt_item.ItemArray[12].ToString();
                         ImageUrl = dt_item.ItemArray[8].ToString();
                        dt.Rows.Add(ModuleName, Pin, Status, DateAndTime, ImageUrl);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }

                    try
                    {
                        AccountNotifyPropertyChanged objAccountNotifyPropertyChanged = new AccountNotifyPropertyChanged();
                        objAccountNotifyPropertyChanged.ID = count;
                       // objAccountNotifyPropertyChanged.AccName = AccountName;
                        objAccountNotifyPropertyChanged.ModuleName = ModuleName;
                        objAccountNotifyPropertyChanged.PinNo = Pin;                      
                        objAccountNotifyPropertyChanged.Status = Status;
                        objAccountNotifyPropertyChanged.ImageUrl = ImageUrl;
                        objAccountNotifyPropertyChanged.DateTime = DateAndTime;

                        this.Dispatcher.Invoke(new Action(delegate
                        {
                            if (!AccountViewModel._listAccReportPinScraper.Contains(objAccountNotifyPropertyChanged))
                            {
                                AccountViewModel._listAccReportPinScraper.Add(objAccountNotifyPropertyChanged);
                            }
                        }));

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
                        dgvPinScraper_AccountsReport.ItemsSource = AccountViewModel._listAccReportPinScraper;

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

        private void clkDeleteAccReport_PinScraper(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModernDialog.ShowMessage("Are You Really Want To Delete This Data Permanently?", " Delete Account ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    QM.DeleteAccountReport("PinScraper");
                    GlobusLogHelper.log.Info(" => [ All Data is Deleted ] ");
                }
                AccountReport_PinScraper();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void clkExportData_PinScraper(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportAccReportPinScraper();
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ExportAccReportPinScraper()
        {
            try
            {
                if (dgvPinScraper_AccountsReport.Items.Count == 1)
                {
                    GlobusLogHelper.log.Info("=> [ Data Is Not Found In Account Report ]");
                    ModernDialog.ShowMessage("Data Is Not Found In Account Report", "Data Is Not Found", MessageBoxButton.OK);
                    return;
                }
                else if (dgvPinScraper_AccountsReport.Items.Count > 1)
                {
                    try
                    {
                        string CSV_Header = string.Join(",", "ModuleName", "Pin", "Status", "Date&Time");
                        string CSV_Content = "";
                        var result = ModernDialog.ShowMessage("Are you want to Export Report Data ", " Export Report Data ", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string FilePath = string.Empty;
                                FilePath = Utils.Utils.UploadFolderData(PDGlobals.Pindominator_Folder_Path);
                                FilePath = FilePath + "\\PinScraper.csv";
                                GlobusLogHelper.log.Info("Export Data File Path :" + FilePath);
                                GlobusLogHelper.log.Debug("Export Data File Path :" + FilePath);
                                string ExportDataLocation = FilePath;
                                PDGlobals.Pindominator_Folder_Path = FilePath;
                                DataSet ds = QM.SelectAddReport("PinScraper");
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    try
                                    {
                                        string ModuleName = item.ItemArray[2].ToString();
                                        string Pin = item.ItemArray[3].ToString();
                                        string Status = item.ItemArray[9].ToString();
                                        string DateAndTime = item.ItemArray[12].ToString();
                                        string ImageUrl = item.ItemArray[8].ToString();
                                        CSV_Content = string.Join(",", ModuleName.Replace("'", ""), Pin.Replace("'", ""), Status.Replace("'", ""), DateAndTime.Replace("'", ""), ImageUrl.Replace("'", ""));
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
                    catch(Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace); 
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
