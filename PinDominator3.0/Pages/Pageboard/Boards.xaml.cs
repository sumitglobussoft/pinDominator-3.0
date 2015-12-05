using Globussoft;
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
    public partial class Boards : UserControl
    {
        public Boards()
        {
            InitializeComponent();
        }
      

        private void btnBoardUrl_Boards_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {            
                ClGlobul.lstListOfBoardNames.Clear();
                txtBoardUrl.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtBoardUrl.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstListOfBoardNames = GlobusFileHelper.ReadFile(txtBoardUrl.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnBoardName_Board_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {          
                ClGlobul.lstBoardNameswithUserNames.Clear();

                txtBoardName.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtBoardName.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstBoardNameswithUserNames = GlobusFileHelper.ReadFile(txtBoardName.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void btnMessage_Board_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMessage.IsReadOnly = true;
                ClGlobul.lstBoardRepinMessage.Clear();

                txtMessage.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtMessage.Text = dlg.FileName.ToString();
                }
                ClGlobul.lstBoardRepinMessage = GlobusFileHelper.ReadFile(txtMessage.Text.Trim());
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        BoardsManager objBoardsManager = new BoardsManager();
        Utils.Utils objUtil = new Utils.Utils();

        private void btnBoardCreation_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(txtBoardUrl.Text))
                        {
                            GlobusLogHelper.log.Info("Please Enter Board Url");
                            ModernDialog.ShowMessage("Please Enter Board Url", "Enter Board Url", MessageBoxButton.OK);
                            return;
                        }
                        if (string.IsNullOrEmpty(txtBoardName.Text))
                        {
                            GlobusLogHelper.log.Info("Please Enter Board Name");
                            ModernDialog.ShowMessage("Please Enter Board Name", "Enter Board Name", MessageBoxButton.OK);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }

                    clsSettingDB Database = new clsSettingDB();
                    Database.UpdateSettingData("Board", "AddBoardUrl", StringEncoderDecoder.Encode(txtBoardName.Text));
                    Database.UpdateSettingData("Board", "AddBoardMessage", StringEncoderDecoder.Encode(txtMessage.Text));

                    objBoardsManager.isStopBoards = false;
                    objBoardsManager.lstThreadsBoards.Clear();
                    if (objBoardsManager._IsfevoriteBoards)
                    {
                        objBoardsManager._IsfevoriteBoards = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtil.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;

                    try
                    {
                        try
                        {
                            objBoardsManager.minDelayBoards = Convert.ToInt32(txtBoard_DelayMin.Text);
                            objBoardsManager.maxDelayBoards = Convert.ToInt32(txtBoard_DelayMax.Text);
                            objBoardsManager.NoOfThreadsBoards = Convert.ToInt32(txtBoard_NoOfThreads.Text);
                            objBoardsManager.MaxRePinCount = Convert.ToInt32(txtNoOfPinRepin.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtBoard_NoOfThreads.Text) && checkNo.IsMatch(txtBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objBoardsManager.NoOfThreadsBoards = threads;
                   
                        Thread BoardsThread = new Thread(objBoardsManager.StartBoards);
                        BoardsThread.Start();
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

        private void btnBoardCreation_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objBoardsManager._IsfevoriteBoards = true;
                List<Thread> lstTempBoards = objBoardsManager.lstThreadsBoards.Distinct().ToList();
                foreach (Thread item in lstTempBoards)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch (Exception ex)
                    {
                    }
                }
              
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

   

     
    }
}
