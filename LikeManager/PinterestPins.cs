using AccountManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLib;
using BasePD;

namespace LikeManager
{
   public  class PinterestPins
    {
        GlobusRegex globusRegex = new GlobusRegex();


        public List<string> PopularPins(int PageCount, ref PinInterestUser objPinUser)
        {
            List<string> lstPopularPins = new List<string>();
            string Name = string.Empty;
            string PopularPinPageSource = string.Empty;
          
            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Popular Pins For this User " + objPinUser.Username + " ]");

                string bookmark = string.Empty;

                for (int i = 0; i < PageCount; i++)
                {
                    string LikeUrl = string.Empty;

                    if (i == 0)
                        LikeUrl = "http://pinterest.com/popular/";
                    else
                        LikeUrl = "http://pinterest.com/resource/CategoryFeedResource/get/?source_url=%2Fpopular%2F&data=%7B%22options%22%3A%7B%22feed%22%3A%22popular%22%2C%22bookmarks%22%3A%5B%22" + Uri.EscapeDataString(bookmark) + "%22%5D%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%2C%22show_via%22%3Afalse%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A" + i + "%7D&module_path=App()%3EHeader()%3EDropdownButton()%3EDropdown()%3ECategoriesMenu(resource%3D%5Bobject+Object%5D%2C+name%3DCategoriesMenu%2C+resource%3DCategoriesResource(browsable%3Dtrue))&_=" + DateTime.Now.Ticks;

                    try
                    {
                        PopularPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(LikeUrl), "http://pinterest.com/", string.Empty, 80, "", "", objPinUser.UserAgent);
                    }
                    catch (Exception ex)
                    {
                        PopularPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(LikeUrl), objPinUser.ProxyAddress, Convert.ToInt32(objPinUser.ProxyPort), objPinUser.ProxyUsername, objPinUser.ProxyPassword);

                    }
                    ///get bookmarks value from page 
                    ///
                    if (PopularPinPageSource.Contains("bookmarks"))
                    {
                        string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(PopularPinPageSource, "bookmarks");

                        string Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                        bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);

                    }


                    List<string> lst = System.Text.RegularExpressions.Regex.Split(PopularPinPageSource, "pin_id").ToList();
                    List<string> templst = new List<string>();
                    foreach (string item in lst)
                    {
                        if (!item.StartsWith("\": \"") || item.Contains("$") || item.Contains("?{pin}"))
                        {
                            continue;
                        }

                        if (item.Contains("id\": \"pinItButton") || item.Contains("uid\": \"Pin-"))// && item.Contains("/repins/"))
                        {
                            //templst.Add(item);
                            try
                            {
                                int FirstPinPoint = item.IndexOf("\": \"");
                                int SecondPinPoint = item.IndexOf("}, ");

                                if (SecondPinPoint > 30)
                                {
                                    SecondPinPoint = item.IndexOf("\", ") + 1;
                                }

                                string Pinid = item.Substring(FirstPinPoint + 4, SecondPinPoint - FirstPinPoint - 5).Trim();

                                if (!lstPopularPins.Any(pid => pid == Pinid))
                                {
                                    lstPopularPins.Add(Pinid);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                        }
                    }
                
                    lstPopularPins = lstPopularPins.Distinct().ToList();
                    lstPopularPins.Reverse();

                    GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstPopularPins.Count + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" => Error : " + ex.StackTrace);
            }

            return lstPopularPins;
        }

        public List<string> VideoPins(int PageCount, ref PinInterestUser objPinUser)
        {
            List<string> lstPopularPins = new List<string>();

            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Video Pins For this User " + objPinUser.Username + " ]");

                string bookmark = string.Empty;
                for (int i = 1; i < PageCount; i++)
                {
                    string LikeUrl = string.Empty;
                    if (i == 0)
                        LikeUrl = "http://pinterest.com/videos/";
                    else
                        LikeUrl = "http://pinterest.com/resource/CategoryFeedResource/get/?source_url=%2Fvideos%2F&data=%7B%22options%22%3A%7B%22feed%22%3A%22videos%22%2C%22bookmarks%22%3A%5B%22" + Uri.EscapeDataString(bookmark) + "%22%5D%7D%2C%22context%22%3A%7B%22app_version%22%3A%22" + objPinUser.App_version + "%22%7D%2C%22module%22%3A%7B%22name%22%3A%22GridItems%22%2C%22options%22%3A%7B%22scrollable%22%3Atrue%2C%22show_grid_footer%22%3Atrue%2C%22centered%22%3Atrue%2C%22reflow_all%22%3Atrue%2C%22virtualize%22%3Atrue%2C%22item_options%22%3A%7B%22show_pinner%22%3Atrue%2C%22show_pinned_from%22%3Afalse%2C%22show_board%22%3Atrue%2C%22show_via%22%3Afalse%7D%2C%22layout%22%3A%22variable_height%22%7D%7D%2C%22append%22%3Atrue%2C%22error_strategy%22%3A" + i + "%7D&module_path=App()%3EHeader()%3EDropdownButton()%3EDropdown()%3ECategoriesMenu(resource%3D%5Bobject+Object%5D%2C+name%3DCategoriesMenu%2C+resource%3DCategoriesResource(browsable%3Dtrue))&_=" + DateTime.Now.Ticks;

                    string VideoPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrlProxy(new Uri(LikeUrl), "http://pinterest.com/", string.Empty, 80, "", "", objPinUser.UserAgent);

                    ///get bookmarks value from page 
                    ///
                    if (VideoPinPageSource.Contains("bookmarks"))
                    {
                        try
                        {
                            string[] bookmarksDataArr = System.Text.RegularExpressions.Regex.Split(VideoPinPageSource, "bookmarks");

                            string Datavalue = bookmarksDataArr[bookmarksDataArr.Count() - 1];

                            bookmark = Datavalue.Substring(Datavalue.IndexOf(": [\"") + 4, Datavalue.IndexOf("]") - Datavalue.IndexOf(": [\"") - 5);
                        }
                        catch(Exception ex)
                        { }
                    }
                    List<string> lst = System.Text.RegularExpressions.Regex.Split(VideoPinPageSource, "pin_id").ToList();
                    foreach (string item in lst)
                    {
                        if (!item.StartsWith("\": \"") || item.Contains("$") || item.Contains("?{pin}"))
                        {
                            continue;
                        }

                        if (item.Contains("id\": \"pinItButton") || item.Contains("uid\": \"Pin-"))
                        {
                            try
                            {
                                int FirstPinPoint = item.IndexOf("\": \"");
                                int SecondPinPoint = item.IndexOf("}, ");
                                if (SecondPinPoint > 30)
                                {
                                    SecondPinPoint = item.IndexOf("\", ") + 1;
                                }
                                string Pinid = item.Substring(FirstPinPoint + 4, SecondPinPoint - FirstPinPoint - 5).Trim();

                                if (!lstPopularPins.Any(pid => pid == Pinid))
                                {
                                    lstPopularPins.Add(Pinid);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                        }
                    }
                    lstPopularPins = lstPopularPins.Distinct().ToList();
                    lstPopularPins.Reverse();

                    GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstPopularPins.Count + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }

            return lstPopularPins;
        }

        public List<string> NormalPins(ref PinInterestUser objPinUser)
        {
            List<string> lstPopularPins = new List<string>();

            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Pins For this User " + objPinUser.Username + " ]");

                string NormalPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("http://pinterest.com/"), "http://pinterest.com/", string.Empty, objPinUser.UserAgent);

                //List<string> lst = globusRegex.GetHrefUrlTags(NormalPinPageSource);
                List<string> lst = System.Text.RegularExpressions.Regex.Split(NormalPinPageSource, "pin_id").ToList();
                List<string> templst = new List<string>();
                foreach (string item in lst)
                {
                    try
                    {
                        if (!item.StartsWith("\": \"") || item.Contains("$") || item.Contains("?{pin}"))
                        {
                            continue;
                        }

                        if (item.Contains("id\": \"pinItButton"))// && item.Contains("/repins/"))
                        {
                            try
                            {
                                int FirstPinPoint = item.IndexOf("\": \"");
                                int SecondPinPoint = item.IndexOf("}, ");
                                if (SecondPinPoint > 30)
                                {
                                    SecondPinPoint = item.IndexOf("\", ") + 1;
                                }
                                string Pinid = item.Substring(FirstPinPoint + 4, SecondPinPoint - FirstPinPoint - 5).Trim();

                                if (!lstPopularPins.Any(pid => pid == Pinid))
                                {
                                    lstPopularPins.Add(Pinid);
                                }
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                        }
                    }
                    catch(Exception ex)
                    { }
                }

                lstPopularPins = lstPopularPins.Distinct().ToList();
                lstPopularPins.Reverse();

                GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstPopularPins.Count + " ]");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }

            return lstPopularPins;
        }

        public List<string> CategoryPins(string CategoryName, int PageCount, ref PinInterestUser objPinUser)
        {
            List<string> lstPopularPins = new List<string>();

            try
            {
                GlobusLogHelper.log.Info(" => [ Start Getting Popular Pins For this User " + objPinUser.Username + " ]");

                string BaseCategoryUrl = "http://pinterest.com/all/?category=" + CategoryName;

                for (int i = 1; i < PageCount; i++)
                {
                    string CategoryUrl = "http://pinterest.com/all/?category=" + CategoryName + "&page=" + i;
                    string PopularPinPageSource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri(CategoryUrl), BaseCategoryUrl, string.Empty, objPinUser.UserAgent);

                    List<string> lst = globusRegex.GetHrefUrlTags(PopularPinPageSource);

                    foreach (string item in lst)
                    {
                        if (item.Contains("/pin/") && !item.Contains("edit"))
                        {
                            try
                            {
                                int FirstPinPoint = item.IndexOf("/pin/");
                                int SecondPinPoint = item.IndexOf("class=");

                                string PinUrl = item.Substring(FirstPinPoint, SecondPinPoint - FirstPinPoint).Replace("\"", string.Empty).Replace("pin", string.Empty).Replace("/", string.Empty).Trim();

                                lstPopularPins.Add(PinUrl);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                            }
                        }
                    }
                }
                lstPopularPins = lstPopularPins.Distinct().ToList();
                lstPopularPins.Reverse();

                GlobusLogHelper.log.Info(" => [ Total Pin Urls Collected " + lstPopularPins.Count + " ]");

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }           
            return lstPopularPins;
        }


        public static string getUserNameFromPinId(string Pinid, ref PinInterestUser objPinUser)
        {
            string Username = string.Empty;
            try
            {

                string pagesource = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://pinterest.com/pin/" + Pinid), "http://pinterest.com/", "", objPinUser.UserAgent);
             
                string[] DataArr = System.Text.RegularExpressions.Regex.Split(pagesource, "commentDescriptionCreator");

                if (DataArr.Count() < 1)
                {
                    return Username;
                }

                try
                {
                    foreach (string item in DataArr)//DataArr
                    {
                        if (item.Contains("!DOCTYPE html"))
                        {
                            continue;
                        }
                       
                        string ValueStaring = item;
                      
                        if (item.Contains("{\"username\":"))
                        {
                            Username = Utils.Utils.getBetween(item, "{\"username\": \"", "\",");
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
            }
            return Username;
        }




    }
}
