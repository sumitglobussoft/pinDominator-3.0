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
using FirstFloor.ModernUI.Windows.Controls;
using BasePD;
using System.Text.RegularExpressions;
using System.Threading;
using CommentManager;
using PinDominator3.CustomUserControl;

namespace PinDominator3.Pages.PageComment
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class CommentByKeyword : UserControl
    {
        public CommentByKeyword()
        {
            InitializeComponent();
        }
        
        private void btnKeyword_CommentByKeyword_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstMessageKeyword.Clear();

                txt_KeywordComment.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txt_KeywordComment.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstMessageKeyword = GlobusFileHelper.ReadFile(txt_KeywordComment.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        CommentByKeywordManager objCommentByKeywordManager = new CommentByKeywordManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnCommentByKeyword_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txt_KeywordComment.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Keyword ");
                            ModernDialog.ShowMessage("Please Upload Comment Keyword", "Upload Keyword", MessageBoxButton.OK);
                            return;
                        }
                        objCommentByKeywordManager.isStopCommentByKeyword = false;
                        objCommentByKeywordManager.lstThreadsCommentByKeyword.Clear();
                      
                        if (objCommentByKeywordManager._IsfevoriteCommentByKeyword)
                        {
                            objCommentByKeywordManager._IsfevoriteCommentByKeyword = false;
                        }

                        Regex checkNo = new Regex("^[0-9]*$");

                        int processorCount = objUtils.GetProcessor();

                        int threads = 25;

                        int maxThread = 25 * processorCount;
                        try
                        {
                            try
                            {
                                objCommentByKeywordManager.minDelayCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_DelayMin.Text);
                                objCommentByKeywordManager.maxDelayCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_DelayMax.Text);
                                objCommentByKeywordManager.Nothread_CommentByKeyword = Convert.ToInt32(txtCommentByKeyword_NoOfThreads.Text);
                                objCommentByKeywordManager.MaxCommentByKeyword = Convert.ToInt32(txtCommentByKeyword_Count.Text);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Info("Enter in Correct Format");
                                return;
                            }

                            if (!string.IsNullOrEmpty(txtCommentByKeyword_NoOfThreads.Text) && checkNo.IsMatch(txtCommentByKeyword_NoOfThreads.Text))
                            {
                                threads = Convert.ToInt32(txtCommentByKeyword_NoOfThreads.Text);
                            }

                            if (threads > maxThread)
                            {
                                threads = 25;
                            }
                            objCommentByKeywordManager.NoOfThreadsCommentByKeyword = threads;

                            Thread CommentKeywordThread = new Thread(objCommentByKeywordManager.StartCommentKeyword);
                            CommentKeywordThread.Start();
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
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnCommentByKeyword_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objCommentByKeywordManager._IsfevoriteCommentByKeyword = true;
                List<Thread> lstTemp = objCommentByKeywordManager.lstThreadsCommentByKeyword.Distinct().ToList();
                foreach (Thread item in lstTemp)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("--------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("--------------------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void DivideData_CommentByKeyword_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CommentByKeywordManager.chkDivideDataCommentByKeyword = true;
                this.Dispatcher.Invoke((Action)delegate
                {
                    var modernDialog = new ModernDialog()
                    {
                        Content = new UserControl_CommentByKeyword_DivideData()
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
