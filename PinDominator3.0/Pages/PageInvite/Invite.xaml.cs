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
using System.Text.RegularExpressions;
using BasePD;
using System.Threading;
using FirstFloor.ModernUI.Windows.Controls;
using InviteManager;

namespace PinDominator3.Pages.PageInvite
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Invite : UserControl
    {
        public Invite()
        {
            InitializeComponent();
        }
        
        private void btnInviteEmail_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtInviteEmail.IsReadOnly = true;
                ClGlobul.lstEmailInvites.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtInviteEmail.Text = ofd.FileName.ToString();
                    readInviteEmailFile(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        public static bool IsValidEmailAddressByRegex(string mailAddress)
        {
            Regex mailIDPattern = new Regex(@"[\w-]+@([\w-]+\.)+[\w-]+");

            if (!string.IsNullOrEmpty(mailAddress) && mailIDPattern.IsMatch(mailAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void readInviteEmailFile(object filePath)
        {
            try
            {
                List<string> Temp_invitelist = new List<string>();
                ClGlobul.lstEmailInvites = GlobusFileHelper.ReadFile(txtInviteEmail.Text.Trim());
                foreach (string item_Emailinvites in ClGlobul.lstEmailInvites)
                {
                    if (!item_Emailinvites.Contains(":"))
                    {
                        bool CheckEmail = IsValidEmailAddressByRegex(item_Emailinvites);
                        if (CheckEmail == true)
                        {
                            Temp_invitelist.Add(item_Emailinvites);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(" => [  " + item_Emailinvites + "  is an invalid Email Id]");
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [  " + item_Emailinvites + "  is not in Proper format]");
                    }
                }
                ClGlobul.lstEmailInvites = Temp_invitelist;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        InviteManagers objInviteManagers = new InviteManagers();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnInviteEmailStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtInviteEmail.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Email ");
                            ModernDialog.ShowMessage("Please Upload Email", "Upload Email", MessageBoxButton.OK);
                            return;
                        }
                        objInviteManagers.isStopInvite = false;
                        objInviteManagers.lstThreadsInvite.Clear();

                        if (objInviteManagers._IsfevoriteInvite)
                        {
                            objInviteManagers._IsfevoriteInvite = false;
                        }


                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objInviteManagers.minDelayInvite = Convert.ToInt32(txtInviteEmail_DelayMin.Text);
                                objInviteManagers.maxDelayInvite = Convert.ToInt32(txtInviteEmail_DelayMax.Text);
                                objInviteManagers.Nothread_Invite = Convert.ToInt32(txtInviteEmail_NoOfThreads.Text);
                                
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtInviteEmail_NoOfThreads.Text) && checkNo.IsMatch(txtInviteEmail_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtInviteEmail_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objInviteManagers.NoOfThreadsInvite = threads;

                            Thread InviteThread = new Thread(objInviteManagers.StartInvite);
                            InviteThread.Start();
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

        private void btnInviteEmailStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objInviteManagers._IsfevoriteInvite = true;
                List<Thread> lstTempInvite = objInviteManagers.lstThreadsInvite.Distinct().ToList();
                foreach (Thread item in lstTempInvite)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("-----------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-----------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }


    }
}
