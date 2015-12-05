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
    public partial class AddBoardName : UserControl
    {
        public AddBoardName()
        {
            InitializeComponent();
        }

        AddBoardNameManager objAddBoardNameManager = new AddBoardNameManager();
        
        private void btnCreateBaord_AddBoardName_Browse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClGlobul.lstBoardNames.Clear();
                
                txtBoardCreate.IsReadOnly = true;
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    txtBoardCreate.Text = dlg.FileName.ToString();
                    ClGlobul.lstBoardNames = GlobusFileHelper.ReadFile(txtBoardCreate.Text.Trim());

                }
                if (ClGlobul.lstBoardNames.Count > 0)
                {
                    string checkBoardName = string.Empty;
                    foreach (string st in ClGlobul.lstBoardNames)
                    {
                        checkBoardName += st;
                    }

                    objAddBoardNameManager.noOfBoard = System.Text.RegularExpressions.Regex.Split(checkBoardName, ",");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info("Error :" + ex.StackTrace);
            }
        }

        Utils.Utils objUtil = new Utils.Utils();

        private void btnStart_AddBoardName_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (PDGlobals.listAccounts.Count > 0)
                {
                    try
                    {
                        if (ClGlobul.lstBoardNames.Count > 0)
                        {                       
                            clsSettingDB Database = new clsSettingDB();
                            Database.UpdateSettingData("Board", "AddBoardName", StringEncoderDecoder.Encode(txtBoardCreate.Text));
                        }
                        else
                        {
                            GlobusLogHelper.log.Info("Please Upload Board Name With Niche");
                            ModernDialog.ShowMessage("Please Upload Board Name With Niche", "Upload Board Name With Niche", MessageBoxButton.OK);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
                    }
                    objAddBoardNameManager.isStopAddBoardName = false;
                    objAddBoardNameManager.lstThreadsAddBoardName.Clear();
                    if (objAddBoardNameManager._Isfevorite)
                    {
                        objAddBoardNameManager._Isfevorite = false;
                    }

                    Regex checkNo = new Regex("^[0-9]*$");                  

                    int processorCount = objUtil.GetProcessor();

                    int threads = 25;

                    int maxThread = 25 * processorCount;
                    try
                    {
                        try
                        {
                            objAddBoardNameManager.minDelayAddBoardName = Convert.ToInt32(txtBoardCreate_DelayMin.Text);
                            objAddBoardNameManager.maxDelayAddBoardName = Convert.ToInt32(txtBoardCreate_DelayMax.Text);
                            objAddBoardNameManager.Nothread_AddBoardName = Convert.ToInt32(txtBoardCreate_NoOfThreads.Text);
                        }
                        catch (Exception ex)
                        {
                            GlobusLogHelper.log.Info("Enter in Correct Format");
                            return;
                        }

                        if (!string.IsNullOrEmpty(txtBoardCreate_NoOfThreads.Text) && checkNo.IsMatch(txtBoardCreate_NoOfThreads.Text))
                        {
                            threads = Convert.ToInt32(txtBoardCreate_NoOfThreads.Text);
                        }

                        if (threads > maxThread)
                        {
                            threads = 25;
                        }
                        objAddBoardNameManager.NoOfThreadsAddBoardName = threads;

                        Thread AddBoardNameThread = new Thread(objAddBoardNameManager.StartAddBoardName);
                        AddBoardNameThread.Start();
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

        private void btnStop_AddBoardName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                objAddBoardNameManager._Isfevorite = true;
                List<Thread> lstTemp_AddBoardName = objAddBoardNameManager.lstThreadsAddBoardName.Distinct().ToList();
                foreach (Thread item in lstTemp_AddBoardName)
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
                GlobusLogHelper.log.Info(" [ PROCESS STOPPED ]");
                GlobusLogHelper.log.Info("-------------------------------------------------------------------------------------------------------------------------------");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

       




    }
}
