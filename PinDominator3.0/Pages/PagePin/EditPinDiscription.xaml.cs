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
using PinsManager;
using System.Text.RegularExpressions;
using System.Threading;

namespace PinDominator3.Pages.PagePin
{
    /// <summary>
    /// Interaction logic for MessageReply.xaml
    /// </summary>
    public partial class EditPinDiscription : UserControl
    {
        public EditPinDiscription()
        {
            InitializeComponent();
        }

        private void btu_Browse_PinDescription_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.CommentNicheMessageList.Clear();
                txtPinDescription.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = ofd.ShowDialog();
                if (result == true)
                {
                    txtPinDescription.Text = ofd.FileName.ToString();
                    ClGlobul.CommentNicheMessageList = GlobusFileHelper.ReadFile(txtPinDescription.Text.Trim());
                    GlobusLogHelper.log.Info(" => [ Total Message Uploaded : " + ClGlobul.CommentNicheMessageList.Count + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        EditPinDiscriptionManager objEditPinDiscriptionManager = new EditPinDiscriptionManager();
        Utils.Utils objUtils = new Utils.Utils();

        private void btnEditPinDescription_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {

                        if (string.IsNullOrEmpty(txtPinDescription.Text))
                        {
                            GlobusLogHelper.log.Info("Please Upload Description");
                            ModernDialog.ShowMessage("Please Upload Description", "Upload Description", MessageBoxButton.OK);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objEditPinDiscriptionManager.isStopEditPinDisc = false;
                    objEditPinDiscriptionManager.lstThreadsEditPinDisc.Clear();

                    if (objEditPinDiscriptionManager._IsfevoriteEditPinDes)
                    {
                        objEditPinDiscriptionManager._IsfevoriteEditPinDes = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");

                    int processorCount = objUtils.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objEditPinDiscriptionManager.minDelayEditPinDisc = Convert.ToInt32(txtEditPinDescription_DelayMin.Text);
                            objEditPinDiscriptionManager.maxDelayEditPinDisc = Convert.ToInt32(txtEditPinDescription_DelayMax.Text);
                            objEditPinDiscriptionManager.Nothread_EditPinDisc = Convert.ToInt32(txtEditPinDescription_NoOfThreads.Text);
                            objEditPinDiscriptionManager.NoOfPagesEditPinDisc = Convert.ToInt32(txtNoOfPages_EditPinDisc.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }


                        if (ClGlobul.CommentNicheMessageList.Count > 0)
                        {
                            clsSettingDB Database = new clsSettingDB();
                            Database.UpdateSettingData("PinDescription", "PinDescriptionMessage", StringEncoderDecoder.Encode(txtPinDescription.Text));
                            GlobusLogHelper.log.Info(" => [ Start Edit Description ]");
                            ClGlobul.lstPins.Clear();

                        }
                        if (!string.IsNullOrEmpty(txtEditPinDescription_NoOfThreads.Text) && checkNo.IsMatch(txtEditPinDescription_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtEditPinDescription_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objEditPinDiscriptionManager.NoOfThreadsEditPinDisc = threads;

                        Thread EditPinDiscThread = new Thread(objEditPinDiscriptionManager.StartEditPinDisc);
                        EditPinDiscThread.Start();
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

        private void btnEditPinDescription_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objEditPinDiscriptionManager._IsfevoriteEditPinDes = true;
                List<Thread> lstTempEditPin = objEditPinDiscriptionManager.lstThreadsEditPinDisc.Distinct().ToList();
                foreach (Thread item in lstTempEditPin)
                {
                    try
                    {
                        item.Abort();
                    }
                    catch(Exception ex)
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
