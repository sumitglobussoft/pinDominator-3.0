
///Adding New Licensing class by ajay yadav 05-03-2016

using BaseLib;
using LicensingManager;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PinDominator
{
  public  class Licensing
    {
      LicenseManager objLicensingManager = new LicenseManager();

      /// <summary>
      /// check License
      /// </summary>
      /// <param name="licensekey"></param>
      /// <returns></returns>
         public Dictionary<string, string> checkLicense(string licensekey)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            try
            {
                Random rand = new Random();

                string whmcsUrl = "http://dominatorhouse.com/members/";
                string licensingSecretKey = string.Empty;//"7M1u89yMRZre29YKF4fa123N2B7q0I25"; // Unique value, should match what is set in the product configuration for MD5 Hash Verification
                string checkToken = DateTime.Now + CalculateMD5Hash(rand.Next(100000000, 999999999) + licensekey);
                string clientIP = currentIP();
                WebClient WHMCSclient = new WebClient();
                NameValueCollection form = new NameValueCollection();

                string cpuID = objLicensingManager.FetchMacId();            
                form.Add("licensekey", licensekey);
                form.Add("domain", cpuID); //this may not apply, a placeholder domain could be used
                form.Add("ip", clientIP);
                form.Add("dir", "PinDominator"); //dir should probably not be applie d either
                form.Add("check_token", "");

                //Post the data and read the response
                Byte[] responseData = WHMCSclient.UploadValues(whmcsUrl + "modules/servers/licensing/verify.php", form);
                string xml = "<tag>" + Encoding.UTF8.GetString(responseData).Replace("\n", "") + "</tag>";
                try
                {
                    XDocument xdoc = XDocument.Parse(xml);
                    foreach (XElement elem in xdoc.Descendants("tag"))
                    {
                        var row = elem.Descendants();
                        string str = elem.ToString();

                        foreach (XElement element in row)
                        {
                            try
                            {
                                string keyName = element.Name.LocalName;
                                results.Add(keyName, element.Value);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error("Error in Licensing" + ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error in Licensing" + ex.Message);
                }
                #region MyRegion

                // Decode and display the response.
                // textBox1.AppendText("Response received was " + Encoding.ASCII.GetString(responseData));

                //   Match match = Regex.Match(Encoding.ASCII.GetString(responseData), @"([^\<^\>^\</]+)");

                //while (match.Success)
                //{
                //    string temp = match.Value;
                //    match = match.NextMatch();
                //    results[temp] = match.Value;
                //    match = match.NextMatch();
                //    match = match.NextMatch();
                //} 
                #endregion

                #region MyRegion
                //if (results.ContainsKey("md5hash"))
                //{
                //    if (results["md5hash"] != CalculateMD5Hash(licensingSecretKey + checkToken))
                //    {
                //        results["status"] = "Invalid";
                //        results["description"] = "MD5 Checksum Verification Failed";
                //        return results;
                //    }
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error in Licensing" + ex.Message);
            }
 
            return results;
        }
 
         /// <summary>
                 /// Calculate MD5 Hash
         /// </summary>
         /// <param name="input"></param>
         /// <returns></returns>
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
 
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
 

      /// <summary>
        /// Find current IP
      /// </summary>
      /// <returns></returns>
        private string currentIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
 
    }
}
    

