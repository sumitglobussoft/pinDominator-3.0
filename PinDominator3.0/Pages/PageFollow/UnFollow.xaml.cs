using BaseLib;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using FollowManagers;
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

namespace PinDominator3.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class UnFollow : UserControl
    {
        public UnFollow()
        {
            InitializeComponent();
        }

        UnFollowManager objUnFollowManager = new UnFollowManager();
        Utils.Utils objUtil = new Utils.Utils();

        private void btnUnFollow_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {                     
                        objUnFollowManager.isStopUnFollow = false;
                        objUnFollowManager.lstThreadsUnFollow.Clear();

                        if (objUnFollowManager._IsfevoriteUnFollow)
                        {
                            objUnFollowManager._IsfevoriteUnFollow = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtil.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objUnFollowManager.minDelayUnFollow = Convert.ToInt32(txtUnFollow_DelayMin.Text);
                                objUnFollowManager.maxDelayUnFollow = Convert.ToInt32(txtUnFollow_DelayMax.Text);
                                objUnFollowManager.Nothread_UnFollow = Convert.ToInt32(txtUnFollow_NoOfThreads.Text);
                                objUnFollowManager.MaxUnFollowCount = Convert.ToInt32(txtUnFollowCount.Text);
                                objUnFollowManager.NOofDays = Convert.ToInt32(txtUnfollowDays.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtUnFollow_NoOfThreads.Text) && checkNo.IsMatch(txtUnFollow_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtUnFollow_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objUnFollowManager.NoOfThreadsUnFollow = threads;

                            Thread UnFollowThread = new Thread(objUnFollowManager.StartUnFollow);
                            UnFollowThread.Start();
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

        private void chkboxUnfollowNoOFDays_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                objUnFollowManager.chkNoOFDays_UnFollow = true;
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }





    }
}
