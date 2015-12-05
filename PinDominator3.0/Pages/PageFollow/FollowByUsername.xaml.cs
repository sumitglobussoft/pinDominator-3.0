using System;
using System.Collections;
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
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;
using FollowManagers;
using PinDominator3.CustomUserControl;

namespace PinDominator3.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class FollowByUsername : UserControl
    {
        public FollowByUsername()
        {
            InitializeComponent();
        }


        FollowByUsernameManager objFollowByUsernameManager = new FollowByUsernameManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();
        
        private void btnUsername_FollowByUsername_Browse_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                ClGlobul.ListOfFollowUsersFollowers.Clear();
                ClGlobul.FollowUsersFollowerQueue.Clear();
                
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtUsernameUpload.Text = dlg.FileName.ToString();
                }
                ClGlobul.ListOfFollowUsersFollowers = GlobusFileHelper.ReadFile(txtUsernameUpload.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnFollowUser_FollowByUsername_Browse_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                ClGlobul.lstFollowUsername.Clear();
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtFollowUserUpload.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstFollowUsername = GlobusFileHelper.ReadFile(txtFollowUserUpload.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rdoFollowUser_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rbFollowUser = true;
                btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
            }
            catch { };
            try
            {
                txtUsernameUpload.IsReadOnly = true;
                txtFollowUserUpload.IsReadOnly = true;
            }
            
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rdoFollowFollowers_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.rdoBtnFollowUserUploaded = true;
                btnUsername_FollowByUsername_Browse.Visibility = Visibility.Hidden;
                btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {              
                txtUsernameUpload.IsReadOnly = true;
                txtFollowUserUpload.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }
    

        private void rdoFollowUserUploaded_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnUsername_FollowByUsername_Browse.Visibility = Visibility.Visible;
                btnFollowUser_FollowByUsername_Browse.Visibility = Visibility.Hidden;
            }
            catch { };
            try
            {
                txtUsernameUpload.IsReadOnly = true;
                txtFollowUserUpload.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnFollowByUsername_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (rdoFollowUserUploaded.IsChecked == true)
                        {
                            if (string.IsNullOrEmpty(txtUsernameUpload.Text))
                            {
                                GlobusLogHelper.log.Info("Please Upload Username ");
                                ModernDialog.ShowMessage("Please Upload Username", "Upload Username", MessageBoxButton.OK);
                                return;
                            }
                          
                        }

                        if (rdoFollowFollowers.IsChecked == true)
                        {
                            if (string.IsNullOrEmpty(txtFollowUserUpload.Text))
                            {
                                GlobusLogHelper.log.Info("Please Upload Follow User ");
                                ModernDialog.ShowMessage("Please Upload Follow User", "Upload Follow User", MessageBoxButton.OK);
                                return;
                            } 
                           
                        }

                        objFollowByUsernameManager.isStopFollowByUsername = false;
                        objFollowByUsernameManager.lstThreadsFollowByUsername.Clear();

                        if (objFollowByUsernameManager._IsfevoriteFollowByUsername)
                        {
                            objFollowByUsernameManager._IsfevoriteFollowByUsername = false;
                        }
                        
                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objFollowByUsernameManager.minDelayFollowByUsername = Convert.ToInt32(txtFollowByUsername_DelayMin.Text);
                                objFollowByUsernameManager.maxDelayFollowByUsername = Convert.ToInt32(txtFollowByUsername_DelayMax.Text);
                                objFollowByUsernameManager.Nothread_FollowByUsername = Convert.ToInt32(txtFollowByUsername_NoOfThreads.Text);
                                objFollowByUsernameManager.MaxFollowCount = Convert.ToInt32(txtFollowerCount.Text);                            
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtFollowUserUpload.Text));                  

                            if (!string.IsNullOrEmpty(txtFollowByUsername_NoOfThreads.Text) && checkNo.IsMatch(txtFollowByUsername_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtFollowByUsername_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objFollowByUsernameManager.NoOfThreadsFollowByUsername = threads;

                            Thread FollowUsernameThread = new Thread(objFollowByUsernameManager.StartFollowByUsername);
                            FollowUsernameThread.Start();
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

        private void btnFollowByUsername_Stop_Click(object sender, RoutedEventArgs e)
        {         
            try
            {
                objFollowByUsernameManager._IsfevoriteFollowByUsername = true;
                List<Thread> lstTempFollowByUsername = objFollowByUsernameManager.lstThreadsFollowByUsername.Distinct().ToList();
                foreach (Thread item in lstTempFollowByUsername)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Error("---------------------------------------------------------");
                GlobusLogHelper.log.Error(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Error("---------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void chkDivideData_FollowByUsername_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                FollowByUsernameManager.chkDivideDataFollowByUsername = true;
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog()
                    {
                        Content = new UserControl_FollowByUsername_DivideData()
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
