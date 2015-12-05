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
using FollowManagers;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;

namespace PinDominator3.Pages.PageFollow
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class FollowByKeyword : UserControl
    {
        public FollowByKeyword()
        {
            InitializeComponent();
        }
        
        private void btnKeyword_FollowByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstkeyword.Clear();

                txtKeywordUpload.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtKeywordUpload.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstkeyword = GlobusFileHelper.ReadFile(txtKeywordUpload.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }


        FollowByKeywordManager objFollowByKeywordManager = new FollowByKeywordManager();
        Utils.Utils objUtils = new Utils.Utils();
        QueryManager QM = new QueryManager();

        private void btnFollowByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtKeywordUpload.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Keyword ");
                            ModernDialog.ShowMessage("Please Upload Comment Keyword", "Upload Keyword", MessageBoxButton.OK);
                            return;
                        }
                        objFollowByKeywordManager.isStopFollowByKeyword = false;
                        objFollowByKeywordManager.lstThreadsFollowByKeyword.Clear();

                        if (objFollowByKeywordManager._IsfevoriteFollowByKeyword)
                        {
                            objFollowByKeywordManager._IsfevoriteFollowByKeyword = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objFollowByKeywordManager.minDelayFollowByKeyword = Convert.ToInt32(txtFollowByKeyword_DelayMin.Text);
                                objFollowByKeywordManager.maxDelayFollowByKeyword = Convert.ToInt32(txtFollowByKeyword_DelayMax.Text);
                                objFollowByKeywordManager.Nothread_FollowByKeyword = Convert.ToInt32(txtFollowByKeyword_NoOfThreads.Text);
                                objFollowByKeywordManager.NoOfUserFollowByKeyword = Convert.ToInt32(txtFollowPerDay.Text);
                                objFollowByKeywordManager.AccPerDayUserFollowByKeyword = Convert.ToInt32(txtBoxHours_FollowByKeyword.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                             QM.UpdateSettingData("Follow", "FollowUserByKeyword", StringEncoderDecoder.Encode(txtKeywordUpload.Text));
                             GlobusLogHelper.log.Info(" => [ Starting Follow Through Keyword ]");

                            if (!string.IsNullOrEmpty(txtFollowByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtFollowByKeyword_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtFollowByKeyword_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objFollowByKeywordManager.NoOfThreadsFollowByKeyword = threads;

                            Thread FollowKeywordThread = new Thread(objFollowByKeywordManager.StartFollowKeyword);
                            FollowKeywordThread.Start();
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

        private void btnFollowByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objFollowByKeywordManager._IsfevoriteFollowByKeyword = true;
                List<Thread> lstTemp = objFollowByKeywordManager.lstThreadsFollowByKeyword.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-----------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-----------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }



    }
}
