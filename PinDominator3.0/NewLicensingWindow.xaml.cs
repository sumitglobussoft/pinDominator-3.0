using BaseLib;
using FirstFloor.ModernUI.Windows.Controls;
using PinDominator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace PinDominator
{
    /// <summary>
    /// Interaction logic for NewLicensingWindow.xaml
    /// </summary>
    public partial class NewLicensingWindow : ModernWindow
    {
        public string ProductKeyfilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\ProductKey.txt";
            //C:\\Facedominator3.0\\Data\\ProductKey.txt";
        public NewLicensingWindow()
        {
            InitializeComponent();

            if (!File.Exists(ProductKeyfilePath))
            {
                try
                {
                    txt_ProductKey.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }

            }
            else
            {
                try
                {
                    string[] ss = File.ReadLines(ProductKeyfilePath).ToArray();
                    licensekey = ss[0];
                    txt_ProductKey.Text = licensekey;
                    txt_ProductKey.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
            }
        }
        public string licensekey = string.Empty;
        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            Thread Obj_StartLicenseValidation = new Thread(StartLicenseValidation);
            Obj_StartLicenseValidation.SetApartmentState(ApartmentState.STA);
            Obj_StartLicenseValidation.Start();
          
        }

        private void StartLicenseValidation()
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                btnActivate.Visibility = Visibility.Hidden;
                LoadAccountProgressBar.IsIndeterminate = true;
            }));

            try
            {

                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    licensekey = txt_ProductKey.Text;
                }));


                MessageBoxButton btnUsed = MessageBoxButton.OK;

                if (string.IsNullOrEmpty(licensekey))
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
              {
                  ModernDialog.ShowMessage("Please Enter Product Key ", "PinDominator 3.0", btnUsed);
              }));
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        btnActivate.Visibility = Visibility.Visible;
                        LoadAccountProgressBar.IsIndeterminate = false;
                    }));
                    return;
                }

                string LicenseStatus = string.Empty;
                string Productname = string.Empty;
                string registeredname = string.Empty;

                Licensing ObjLicensing = new Licensing();
                Dictionary<string, string> LicenseDetails = ObjLicensing.checkLicense(licensekey);

                try
                {
                    LicenseStatus = LicenseDetails["status"];
                    if (LicenseStatus.Contains("Invalid"))
                    {

                        try
                        {
                            string ErrorMessage = LicenseDetails["message"];

                            if (ErrorMessage.ToLower().Contains("domain invalid"))
                            {
                                Application.Current.Dispatcher.Invoke((Action)(() =>
                      {
                          ModernDialog.ShowMessage("License product key is already in use ! \n One Product is licensed to use on single machine ! \n Re-issue license from dashboard panel to continue with current machine or else purchase another license key from www.dominatorhouse.com ", "PinDominator", btnUsed);

                      }));
                                return;
                            }
                        }
                        catch (Exception)
                        {

                        }


                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            ModernDialog.ShowMessage("Product Key Is Invalid \nPlease Enter valid product key\nOne Product is licensed to use on single machine ! \npurchase license key from www.dominatorhouse.com ", "PinDominator", btnUsed);

                        }));
                        return;
                    }

                }
                catch (Exception ex)
                {
                    ModernDialog.ShowMessage(" Your IP has been banned please try after few hours ! For more details contact us in skype : facedominatorsupport  ", "Error Message", btnUsed);                 
                    GlobusLogHelper.log.Error(" Error ==>" + ex.StackTrace);
                    return;
                }


                try
                {

                    Productname = LicenseDetails["productname"];

                    if (Productname.ToLower().Contains("pindominator") || Productname.Contains("PinDominator"))
                    {
                        
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ModernDialog.ShowMessage("Product Key Is Invalid \nPlease Enter valid product key\nOne Product is licensed to use on single machine ! \npurchase license key from www.dominatorhouse.com ", "PinDominator", btnUsed);
                    }));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }
                try
                {
                    registeredname = LicenseDetails["registeredname"].Replace("\n", string.Empty);
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(ex.StackTrace);
                }


                try
                {

                    if (Productname.ToLower().Contains("basic"))
                    {
                        Globals.IsBasicVersion = true;
                    }
                    if (Productname.ToLower().Contains("pro"))
                    {
                        Globals.IsProVersion = true;
                    }
                    if (Productname.ToLower().Contains("agency"))
                    {
                        Globals.IsAgencyVersion = true;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                }

                Globals.LicenseCheckUserName = " Registered Name : " + registeredname + " License Type : " + Productname;

                if (LicenseStatus.Contains("Active"))
                {
                    OpenMainForm();

                    if (!File.Exists(ProductKeyfilePath))
                    {
                        try
                        {
                            File.WriteAllText(ProductKeyfilePath, licensekey);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    var objDialogresult = ModernDialog.ShowMessage("Please Check Your License Product Key ! ", "PinDominator 3.0", btnUsed);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }

        }

        private void OpenMainForm()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow mainFrm = new MainWindow();
                    mainFrm.Show();
                    this.Close();
                }));
              

               
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }
    }
}
