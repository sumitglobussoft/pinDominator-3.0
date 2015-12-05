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
using System.Threading;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using CommentManager;
using System.Text.RegularExpressions;

namespace PinDominator3.Pages.PageComment
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class Comment : UserControl
    {
        public Comment()
        {
            InitializeComponent();
        }
       
        private void btnMessage_Comment_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.CommentMessagesList.Clear();

                txtCommentMessage.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtCommentMessage.Text = dlg.FileName.ToString();
                }
                ClGlobul.CommentMessagesList = GlobusFileHelper.ReadFile(txtCommentMessage.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        CommentManagers objCommentManagers = new CommentManagers();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnComment_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtCommentMessage.Text))
                            {
                                GlobusLogHelper.log.Info("Please Upload Comment Message");
                                ModernDialog.ShowMessage("Please Upload Comment Message", "Upload Message", MessageBoxButton.OK);
                                return;
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objCommentManagers.isStopComment = false;
                    objCommentManagers.lstThreadsComment.Clear();

                    if (objCommentManagers._IsfevoriteComment)
                    {
                        objCommentManagers._IsfevoriteComment = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            CommentManagers.minDelayComment = Convert.ToInt32(txtComment_DelayMin.Text);
                            CommentManagers.maxDelayComment = Convert.ToInt32(txtComment_DelayMax.Text);
                            CommentManagers.Nothread_Comment = Convert.ToInt32(txtComment_NoOfThreads.Text);
                            CommentManagers.MaxComment = Convert.ToInt32(txtCommentCount.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtComment_NoOfThreads.Text) && checkNo.IsMatch(txtComment_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtComment_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objCommentManagers.NoOfThreadsComment = threads;

                        Thread CommentThread = new Thread(objCommentManagers.StartComment);
                        CommentThread.Start();
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
                GlobusLogHelper.log.Error("Error :" + ex.StackTrace);
            }
        }

        private void btnComment_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objCommentManagers._IsfevoriteComment = true;
                List<Thread> lstTemp = objCommentManagers.lstThreadsComment.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        
    }
}
