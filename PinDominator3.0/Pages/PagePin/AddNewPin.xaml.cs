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
using FirstFloor.ModernUI.Windows.Controls;
using PinsManager;

namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class AddNewPin : UserControl
    {
        public AddNewPin()
        {
            InitializeComponent();
        }

        private void btnNewFile_AddNewPin_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstListOfPins.Clear();
                txtNewPin.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtNewPin.Text = ofd.FileName.ToString();
                    Thread ReadLargeFileThread = new Thread(ReadLargeNewPinsFile);
                    ReadLargeFileThread.Start(ofd.FileName);
                  //  readMessageFile(dlg.FileName);
                }                               
           
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        private void ReadLargeNewPinsFile(object filePath)
        {
            try
            {
                List<string> AccountsList = GlobusFileHelper.ReadFile((string)filePath);
                new Thread(() =>
                {
                    AddNewPinsList(AccountsList);
                }).Start();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }
        private void AddNewPinsList(List<string> Messages)
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
                                       pin.ImageUrl = arMessages[0];
                                       pin.Description = arMessages[1];
                                       pin.Board = arMessages[2];
                                   }
                                   else if (arMessages.Count() == 4)
                                   {
                                       pin.ImageUrl = arMessages[0];
                                       pin.Description = arMessages[1];
                                       pin.Board = arMessages[2];
                                       pin.Email = arMessages[3];
                                   }
                                   if (!string.IsNullOrEmpty(pin.ImageUrl))
                                   {
                                       ClGlobul.lstListOfPins.Add(pin);
                                   }

                               }

                           });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        AddNewPinManager objAddNewPinManager = new AddNewPinManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnAddNewPin_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {

                        if (string.IsNullOrEmpty(txtNewPin.Text))
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
                    objAddNewPinManager.isStopAddNewPin = false;
                    objAddNewPinManager.lstThreadsAddNewPin.Clear();

                    if (objAddNewPinManager._IsfevoriteAddNewPin)
                    {
                        objAddNewPinManager._IsfevoriteAddNewPin = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddNewPinManager.minDelayAddNewPin = Convert.ToInt32(txtAddNewPin_DelayMin.Text);
                            objAddNewPinManager.maxDelayAddNewPin = Convert.ToInt32(txtAddNewPin_DelayMax.Text);
                            objAddNewPinManager.Nothread_AddNewPin = Convert.ToInt32(txtAddNewPin_NoOfThreads.Text);                      
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtAddNewPin_NoOfThreads.Text) && checkNo.IsMatch(txtAddNewPin_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtAddNewPin_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddNewPinManager.NoOfThreadsAddNewPin = threads;

                        Thread LikeThread = new Thread(objAddNewPinManager.StartAddNewPin);
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
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        private void btnAddNewPin_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddNewPinManager._IsfevoriteAddNewPin = true;
                List<Thread> lstTempAddNewPin = objAddNewPinManager.lstThreadsAddNewPin.Distinct().ToList();
                foreach (Thread item in lstTempAddNewPin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
                    {
                    }
                }

                GlobusLogHelper.log.Info("---------------------------------------------------------------");
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
       



    }
}
