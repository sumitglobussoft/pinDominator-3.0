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
using System.Threading;
using Globussoft;
using System.Text.RegularExpressions;
using BasePD;
using PinsManager;
using FirstFloor.ModernUI.Windows.Controls;


namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddPinWithnewBoard : UserControl
    {
        public AddPinWithnewBoard()
        {
            InitializeComponent();
        }

        private void btnUploadPinFile_AddPinWithnewBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtaddPinwithNewBoard.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtaddPinwithNewBoard.Text = ofd.FileName.ToString();
                    Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFileofAddNewBoardWithNewPin);
                    ReadLargeFileThread.Start(ofd.FileName);
                }    
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void ReadLargeNewPinsFileofAddNewBoardWithNewPin(object filePath)
        {
            try
            {
                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddNewPinsListofAddNewBoardWithNewPin(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void AddNewPinsListofAddNewBoardWithNewPin(List<string> Messages)
        {
            try
            {
                ClGlobul.lstListOfNewUsers.Clear();
              Dispatcher.Invoke((Action)delegate
                {
                    foreach (string Message in Messages)
                    {
                        string NewMessages = Message.Replace("\0", "").Trim();
                        string[] arMessages = Regex.Split(NewMessages, ",");

                        BaseLib.Pins pin = new BaseLib.Pins();

                        if (arMessages.Count() == 1)
                        {
                            pin.ImageUrl = arMessages[0];
                        }
                        else if (arMessages.Count() == 2)
                        {
                            pin.ImageUrl = arMessages[0];
                            pin.Description = arMessages[1];
                        }
                        else if (arMessages.Count() == 3)
                        {
                            pin.Board = arMessages[0];
                            pin.Description = arMessages[1];
                            pin.ImageUrl = (arMessages[2]).Trim();
                        }
                        else if (arMessages.Count() == 4)
                        {
                            pin.Board = arMessages[0];
                            pin.Description = arMessages[1];
                            pin.ImageUrl = (arMessages[2] + arMessages[3]).Trim();

                        }
                        if (!string.IsNullOrEmpty(pin.ImageUrl))
                        {
                            ClGlobul.lst_AddnewPinWithNewBoard.Add(pin);
                        }

                    }

                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        AddPinWithNewBoardManager objAddPinWithNewBoardManager = new AddPinWithNewBoardManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnAddPinWithnewBoard_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {

                        if (string.IsNullOrEmpty(txtaddPinwithNewBoard.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Pin File");
                            ModernDialog.ShowMessage("Please Upload Pin File", "Upload Pin File", MessageBoxButton.OK);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objAddPinWithNewBoardManager.isStopAddPinWithNewBoard = false;
                    objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Clear();

                    if (objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard)
                    {
                        objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    ClGlobul.addNewPinWithBoard = GlobusFileHelper.ReadFile(txtaddPinwithNewBoard.Text.Trim());                   

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddPinWithNewBoardManager.minDelayAddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_DelayMin.Text);
                            objAddPinWithNewBoardManager.maxDelayAddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_DelayMax.Text);
                            objAddPinWithNewBoardManager.Nothread_AddPinWithNewBoard = Convert.ToInt32(txtAddPinwithnewBoard_NoOfThreads.Text);                           
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }
                      
                        if (!string.IsNullOrEmpty(txtAddPinwithnewBoard_NoOfThreads.Text) && checkNo.IsMatch(txtAddPinwithnewBoard_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtAddPinwithnewBoard_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddPinWithNewBoardManager.NoOfThreadsAddPinWithNewBoard = threads;

                        Thread AddPinWithNewBoardThread = new Thread(objAddPinWithNewBoardManager.StartAddPinWithNewBoard);
                        AddPinWithNewBoardThread.Start();
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

        private void btnAddPinWithnewBoard_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddPinWithNewBoardManager._IsfevoriteAddPinWithNewBoard = true;
                List<Thread> lstTemp = objAddPinWithNewBoardManager.lstThreadsAddPinWithNewBoard.Distinct().ToList();
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

                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
                GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }







    }
}
