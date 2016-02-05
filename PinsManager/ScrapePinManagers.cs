using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace PinsManager
{
    public delegate void AccountReport_PinScraper();
    public class ScrapePinManagers
    {
        public static AccountReport_PinScraper objPinScraperDelegate;

        #region Global Variable
        public bool isStopPinScraper = false;
        public int Nothread_PinScraper = 5;
        public List<Thread> lstThreadsPinScraper = new List<Thread>();
        public static int countThreadControllerPinScraper = 0;
        public static int PinScraperdata_count = 0;
        public readonly object PinScraperObjThread = new object();
        public static bool chkScrapeImage = false;
        public int MaxNoOfPinScrape = 5;
        public bool _IsfevoritePinScraper = false;

        public int minDelayPinScraper
        {
            get;
            set;
        }

        public int maxDelayPinScraper
        {
            get;
            set;
        }

        public int NoOfThreadsPinScraper
        {
            get;
            set;
        }

        public string BoardUrl = string.Empty;
        List<string> lstPopularPins = new List<string>();
        string PopularPinPageSource = string.Empty;
        string NextPageUrlPinPageSource = string.Empty;
        string BookMark = string.Empty;
        string BoardIdOfBoardUrl = string.Empty;
        string BoardUrlEdited = string.Empty;
        string NextPageUrl = string.Empty;
        string ImagePin = string.Empty;
        string Pagesource = string.Empty;
        string ImageUrl = string.Empty;
        int count = 1;
        int Counter = 0;

        PinInterestUser objPinUser = new PinInterestUser();

        #endregion

        QueryManager qm = new QueryManager();

        public void StartPinScraper()
        {
            try
            {
                objPinUser.globusHttpHelper = new GlobusHttpHelper();
                if (!isStopPinScraper)
                {
                    try
                    {
                        lstThreadsPinScraper.Add(Thread.CurrentThread);
                        lstThreadsPinScraper.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };
                    ClGlobul.GetPinList = GetBoardPinsNew_PinScraper();                  
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public List<string> GetBoardPinsNew_PinScraper()
        {
            try
            {
                GlobusLogHelper.log.Info(" => [ Scraping Process Started ]");
                string LikeUrl = BoardUrl;
                try
                {
                    PopularPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(LikeUrl), "http://pinterest.com/", string.Empty, objPinUser.UserAgent);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }

                if (PopularPinPageSource.Contains("BoardResource\", \"options\": {\"board_id\":"))
                {
                    BoardIdOfBoardUrl = Utils.Utils.getBetween(PopularPinPageSource, "BoardResource\", \"options\": {\"board_id\":", ",").Replace("\"", "").Trim();
                }

                BoardUrlEdited = BoardUrl.Replace("https://www.pinterest.com", "").Replace("/", "%2F");

                while (!string.IsNullOrEmpty(PopularPinPageSource))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(PopularPinPageSource))
                        {
                            if (PopularPinPageSource.Contains("board_layout"))
                            {

                                BookMark = Utils.Utils.getBetween(PopularPinPageSource, "\"board_layout\": \"default\", \"num_request\": null, \"bookmarks\": [\"", "]").Replace("\"", "").Replace("=", "");
                            }
                            if (BookMark.Contains(""))
                            {
                                BookMark = Utils.Utils.getBetween(PopularPinPageSource, "board_layout\": \"default\", \"bookmarks\": [", "]").Replace("\"", "").Replace("=", "");
                            }
                            if (PopularPinPageSource.Contains("pinHolder"))
                            {
                                try
                                {
                                    string[] arrPin = Regex.Split(PopularPinPageSource, "pinHolder");
                                    foreach (var itemArrPin in arrPin)
                                    {
                                        string Pin = Utils.Utils.getBetween(itemArrPin, "/pin/", "class=").Replace("\\", "").Replace("/", "").Replace("\"", "").Trim();
                                        if (!string.IsNullOrEmpty(Pin) && !lstPopularPins.Contains(Pin))
                                        {
                                            if (MaxNoOfPinScrape == Counter)
                                            {
                                                break;
                                            }
                                            else
                                            {

                                                //GlobusLogHelper.log.Info(" => [ Pin " + Pin + " ]");
                                                if (chkScrapeImage == true)
                                                {
                                                    ScrapeImage(Pin);
                                                }
                                                else
                                                {
                                                    ImageUrl = "NULL";
                                                }
                                                #region AccountReport

                                                string module = "PinScraper";
                                                string status = "Scraped";
                                                qm.insertAccRePort("", module, "https://www.pinterest.com/pin/" + Pin, "", "", "", "", ImageUrl, status, "", "", DateTime.Now);
                                                objPinScraperDelegate();

                                                #endregion
                                                lstPopularPins.Add(Pin);
                                                Counter++;
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }
                            }

                            if (PopularPinPageSource.Contains("\"id\":"))
                            {
                                try
                                {
                                    string[] arrPin = Regex.Split(PopularPinPageSource, "uri");
                                    foreach (var itemArrPin in arrPin)
                                    {
                                        if (itemArrPin.Contains("/v3/pins/"))
                                        {
                                            string Pin = Utils.Utils.getBetween(itemArrPin, "/v3/pins/", "/comments/").Replace("\\", "").Replace("/", "").Replace("\"", "").Trim();

                                            if (!string.IsNullOrEmpty(Pin) && !lstPopularPins.Contains(Pin))
                                            {
                                                if (MaxNoOfPinScrape == Counter)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    // GlobusLogHelper.log.Info(" => [ Pin " + Pin + " ]");
                                                    if (chkScrapeImage == true)
                                                    {
                                                        ScrapeImage(Pin);
                                                    }
                                                    else
                                                    {
                                                        ImageUrl = "NULL";
                                                    }
                                                    #region AccountReport

                                                    string module = "PinScraper";
                                                    string status = "Scraped";
                                                    qm.insertAccRePort("", module, "https://www.pinterest.com/pin/" + Pin, "", "", "", "", ImageUrl, status, "", "", DateTime.Now);
                                                    objPinScraperDelegate();

                                                    #endregion
                                                    lstPopularPins.Add(Pin);
                                                    Counter++;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                }
                            }
                        }

                        try
                        {
                            NextPageUrl = "https://www.pinterest.com/resource/BoardFeedResource/get/?source_url=" + BoardIdOfBoardUrl + "&data=%7B%22options%22%3A%7B%22board_id%22%3A%22" + BoardIdOfBoardUrl + "%22%2C%22board_url%22%3A%22" + BoardUrlEdited + "%22%2C%22page_size%22%3Anull%2C%22prepend%22%3Atrue%2C%22access%22%3A%5B%5D%2C%22board_layout%22%3A%22default%22%2C%22bookmarks%22%3A%5B%22" + BookMark + "%3D%3D%22%5D%7D%2C%22context%22%3A%7B%7D%7D&_=142710648289" + count + "";
                        }
                        catch { };

                        try
                        {
                            PopularPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(NextPageUrl), "https://www.pinterest.com/", string.Empty, objPinUser.UserAgent);
                        }
                        catch (Exception ex)
                        {
                            PopularPinPageSource = null;
                        }
                        count++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }    
                }

                lstPopularPins = lstPopularPins.Distinct().ToList();
                lstPopularPins.Reverse();

                GlobusLogHelper.log.Info(" => [ Total  Pins : " + lstPopularPins.Count + " ]");

                GlobusLogHelper.log.Info(" => [ Scraping Process Completed ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }    
            return lstPopularPins;
        }

        public void ScrapeImage(string Pin)
        {
            try
            {
                //GlobusHttpHelper _GlobusHttpHelper = new GlobusHttpHelper();
              
                ImagePin = Pin;

                try
                {
                    GlobusLogHelper.log.Info(" => [ Scraping Image for Pin " + Pin + " ]");
                    string PinUrl = "https://www.pinterest.com/pin/" + ImagePin + "/";
                    Pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(PinUrl));
                    if (Pagesource.Contains("class=\"Image Module"))
                    {
                        string[] ImagePage = Regex.Split(Pagesource, "class=\"Image Module");
                        ImagePage = ImagePage.Skip(1).ToArray();
                        try
                        {
                            foreach (string imagepage in ImagePage)
                            {
                                if (!imagepage.Contains("<!DOCTYPE html>"))
                                {
                                    ImageUrl = Utils.Utils.getBetween(imagepage, " src=\"", "\"");
                                    DownloadImage(ImageUrl);
                                    GlobusLogHelper.log.Info(" => [ Scraped Image " + ImageUrl + " for Pin " + Pin + " ]");
                                    break;
                                }

                            }
                        }
                        catch(Exception ex) 
                        {
                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                        }
                    }

                }
                catch(Exception ex) 
                {
                    GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public static void DownloadImage(string ImageUrl)
        {
            try
            {
                string Url = ImageUrl;
                WebClient objwebclient = new WebClient();
                byte[] array = objwebclient.DownloadData(ImageUrl);
                string[] areaay = { "A", "B", "C", "D", "E", "G" };
                Random rn = new Random();
                string Title = areaay[rn.Next(0, 5)] + "" + rn.Next(0, 1000000);
                string PicPath = PDGlobals.path_Image + "\\" + Title + ".jpg";
                objwebclient.UploadData(PicPath, array);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

    }
}
