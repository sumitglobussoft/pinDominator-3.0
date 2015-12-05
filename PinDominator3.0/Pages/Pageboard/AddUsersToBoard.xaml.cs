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
using BoardManager;
using BasePD;
using FirstFloor.ModernUI.Windows.Controls;
using System.Text.RegularExpressions;
using System.Threading;

namespace PinDominator3.Pages.Pageboard
{

    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddUsersToBoard : UserControl
    {
        public AddUsersToBoard()
        {
            InitializeComponent();
        }

        AddUsersToBoardManager objAddUsersToBoardManager = new AddUsersToBoardManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnUserNames_AddUsersToBoard_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstAddToBoardUserNames.Clear();
                txtEmailOrUserNames.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtEmailOrUserNames.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstAddToBoardUserNames = GlobusFileHelper.ReadFile(txtEmailOrUserNames.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnStart_AddToBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtBoardName.Text))
                        {
                            GlobusLogHelper.log.Info("Please Enter Board Name");
                            ModernDialog.ShowMessage("Please Enter Board Name", "Enter Board Name", MessageBoxButton.OK);
                            return;
                        }
                        if (string.IsNullOrEmpty(txtEmailOrUserNames.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Email or UserName");
                            ModernDialog.ShowMessage("Please Upload Email or UserName", "Upload Email or UserName", MessageBoxButton.OK);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    objAddUsersToBoardManager.BoardName = txtBoardName.Text.Trim();

                    objAddUsersToBoardManager.isStopAddUserToBoard = false;
                    objAddUsersToBoardManager.lstThreadsAddUserToBoard.Clear();
                    if (objAddUsersToBoardManager._IsfevoriteAddUserToBoard)
                    {
                        objAddUsersToBoardManager._IsfevoriteAddUserToBoard = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        try
                        {
                            objAddUsersToBoardManager.minDelayAddUserToBoard = Convert.ToInt32(txtUserBoard_DelayMin.Text);
                            objAddUsersToBoardManager.maxDelayAddUserToBoard = Convert.ToInt32(txtUserBoard_DelayMax.Text);
                            objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = Convert.ToInt32(txtUserBoard_NoOfThreads.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtUserBoard_NoOfThreads.Text) && checkNo.IsMatch(txtUserBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtUserBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddUsersToBoardManager.NoOfThreadsAddUserToBoard = threads;

                        clsSettingDB Database = new clsSettingDB();
                        Database.UpdateSettingData("UserToBoard", "UserToBoard", StringEncoderDecoder.Encode(txtEmailOrUserNames.Text));

                        Thread AddUserToBoardThread = new Thread(objAddUsersToBoardManager.StartAddUsersToBoard);
                        AddUserToBoardThread.Start();
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

        private void btnStop_AddToBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddUsersToBoardManager._IsfevoriteAddUserToBoard = true;
                List<Thread> lstTemp = objAddUsersToBoardManager.lstThreadsAddUserToBoard.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info(" => [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");             
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

        }





    }
}
