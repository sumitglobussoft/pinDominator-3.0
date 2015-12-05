using BaseLib;
using System;
using System.Collections.Generic;
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
using BaseLib;
using Globussoft;
using BasePD;
using LikeManager;
using System.Text.RegularExpressions;
using FirstFloor.ModernUI.Windows.Controls;
using PinDominator3.CustomUserControl;


namespace PinDominator3.Pages.PageLike
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Like : UserControl
    {
        public Like()
        {
            InitializeComponent();
        }
       
        private void btnPinUrls_Like_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtLikePinUrl.IsReadOnly = true;
                ClGlobul.lstAddToBoardUserNames.Clear();
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtLikePinUrl.Text = ofd.FileName.ToString();
                    Thread ReadLargeFileThread = new Thread(ReadLargeLikePinUrlsFile);
                    ReadLargeFileThread.Start(ofd.FileName);
                    ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtLikePinUrl.Text.Trim());
                    GlobusLogHelper.log.Info(" => [ " + ClGlobul.lstAddToBoardUserNames.Count + " Pin Urls Uploaded ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }
        List<Thread> lstReTweetThread = new List<Thread>();
        private void ReadLargeLikePinUrlsFile(object filePath)
        {
            try
            {
                lstReTweetThread.Add(Thread.CurrentThread);
                lstReTweetThread.Distinct().ToList();
                Thread.CurrentThread.IsBackground = true;


                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddLikePinUrlsList(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void AddLikePinUrlsList(List<string> Messages)
        {
            try
            {
                ClGlobul.lstLikePinUrls.Clear();
                this. Dispatcher.Invoke((Action)delegate
                {
                    foreach (string Message in Messages)
                    {
                        string NewMessages = Message.Replace("\0", "").Trim();
                        if (!ClGlobul.lstLikePinUrls.Contains(NewMessages))
                        {
                            if (!string.IsNullOrEmpty(NewMessages))
                            {
                                if (NewMessages.Length > 400)
                                {
                                    NewMessages = NewMessages.Substring(0, 400);
                                }
                                ClGlobul.lstLikePinUrls.Add(NewMessages.Replace("\0", ""));
                            }
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rboNormalLikePinUrls_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.rbNormalLikePinUrls = true;
                btnPinUrls_Like_Browse.Visibility = Visibility.Hidden;
            }
            catch { };
            try
            {
                txtLikePinUrl.IsReadOnly = true;
              
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void rboListLikePinUrls_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.rbListLikePinUrls = true;
                btnPinUrls_Like_Browse.Visibility = Visibility.Visible;
            }
            catch { };
            try
            {
                txtLikePinUrl.IsReadOnly = true;

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        LikeManagers objLikeManagers = new LikeManagers();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnLike_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (rboListLikePinUrls.IsChecked == true)
                        {
                            if (string.IsNullOrEmpty(txtLikePinUrl.Text))
                            {
                                GlobusLogHelper.log.Info("Please Upload Comment Message");
                                ModernDialog.ShowMessage("Please Upload Comment Message", "Upload Message", MessageBoxButton.OK);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objLikeManagers.isStopLike = false;
                    objLikeManagers.lstThreadsLike.Clear();

                    if (objLikeManagers._IsfevoriteLike)
                    {
                        objLikeManagers._IsfevoriteLike = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            LikeManagers.minDelayLike = Convert.ToInt32(txtLike_DelayMin.Text);
                            LikeManagers.maxDelayLike = Convert.ToInt32(txtLike_DelayMax.Text);
                            objLikeManagers.NoOfThreadsLike = Convert.ToInt32(txtLike_NoOfThreads.Text);
                            LikeManagers.MaxLike = Convert.ToInt32(txtLikeCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtLike_NoOfThreads.Text) && checkNo.IsMatch(txtLike_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtLike_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objLikeManagers.NoOfThreadsLike = threads;

                        Thread LikeThread = new Thread(objLikeManagers.StartLike);
                        LikeThread.Start();
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
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void chkBox_Like_DivideData_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                LikeManagers.chkBox_Like_DivideData = true;
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog
                    {
                        Content = new UserControl_Like_DivideData()
                    };
                    modernDialog.MinWidth = 600;
                    modernDialog.MinHeight = 200;
                    modernDialog.ShowDialog();
                });

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        private void btnLike_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objLikeManagers._IsfevoriteLike = true;
                List<Thread> lstTempLike = objLikeManagers.lstThreadsLike.Distinct().ToList();
                foreach (Thread item in lstTempLike)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("-------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }




    }
}
