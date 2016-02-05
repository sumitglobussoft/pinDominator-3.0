using AccountManager;
using BaseLib;
using BasePD;
using Globussoft;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PinsManager
{
    public delegate void AccountReport_AddBoardName();
    public delegate void AccountReport_AddPinWithBoard();
    public class AddPinWithNewBoardManager
    {
        public static AccountReport_AddBoardName objDelegateAccountReport;
        public static AccountReport_AddPinWithBoard objAddPinWithBoardDelegate;

        #region Global Variable
        public int Nothread_AddPinWithNewBoard = 5;
        public bool isStopAddPinWithNewBoard = false;
        public List<Thread> lstThreadsAddPinWithNewBoard = new List<Thread>();
        public static int countThreadControllerAddPinWithNewBoard = 0;
        public static int AddPinWithNewBoarddata_count = 0;
        public readonly object AddPinWithNewBoardObjThread = new object();
        public bool _IsfevoriteAddPinWithNewBoard = false;

        public int minDelayAddPinWithNewBoard
        {
            get;
            set;
        }

        public int maxDelayAddPinWithNewBoard
        {
            get;
            set;
        }

        public int NoOfThreadsAddPinWithNewBoard
        {
            get;
            set;
        }

        List<Pins> UserPins = new List<Pins>();
        int ThreadDelay = 10;
        string ImageUrl = string.Empty;
        string Desc = string.Empty;
        string BoardNumber = string.Empty;
        string Board = string.Empty;
        string CreatedBoardPageSource = string.Empty;

        #endregion

        Accounts ObjAccountManager = new Accounts();
        QueryManager objqm = new QueryManager();
        AddNewPinManager objAddNewPinManager = new AddNewPinManager();
        string Pinpath = PDGlobals.FolderCreation(PDGlobals.Pindominator_Folder_Path, "NewPin");

        public void StartAddPinWithNewBoard()
        {
            try
            {
                countThreadControllerAddPinWithNewBoard = 0;
                int numberOfAccountPatchAddPinWithNewBoard = 25;

                if (NoOfThreadsAddPinWithNewBoard > 0)
                {
                    numberOfAccountPatchAddPinWithNewBoard = NoOfThreadsAddPinWithNewBoard;
                }

                AddPinWithNewBoarddata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchAddPinWithNewBoard);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerAddPinWithNewBoard > Nothread_AddPinWithNewBoard)
                            {
                                try
                                {
                                    lock (AddPinWithNewBoardObjThread)
                                    {
                                        Monitor.Wait(AddPinWithNewBoardObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {

                                }
                            }
                            string acc = account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {
                                Thread profilerThread = new Thread(StartAddPinWithNewBoardMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAddPinWithNewBoard++;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartAddPinWithNewBoardMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAddPinWithNewBoard)
                {
                    try
                    {
                        lstThreadsAddPinWithNewBoard.Add(Thread.CurrentThread);
                        lstThreadsAddPinWithNewBoard.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    { };

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;

                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                        #region Login

                        if (!objPinUser.isloggedin)
                        {
                            GlobusLogHelper.log.Info(" => [ Logging In With : " + objPinUser.Username + " ]");
                            bool checkLogin;
                            if (string.IsNullOrEmpty(objPinUser.ProxyPort))
                            {
                                objPinUser.ProxyPort = "80";
                            }
                            try
                            {
                                //checkLogin = ObjAccountManager.LoginPinterestAccount1(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);
                                if (!checkLogin)
                                {
                                    GlobusLogHelper.log.Info(" => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
                                    return;
                                }
                                string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadAddPinWithNewBoard(ref objPinUser);
                            }

                            catch { };
                        }
                        else if (objPinUser.isloggedin == true)
                        {
                            try
                            {
                                GlobusLogHelper.log.Info(" => [ Logged In With : " + objPinUser.Username + " ]");
                                StartActionMultithreadAddPinWithNewBoard(ref objPinUser);
                            }
                            catch (Exception ex)
                            {
                                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                            }
                        }
                        #endregion              
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void StartActionMultithreadAddPinWithNewBoard(ref PinInterestUser objPinUser12)
        {
            try
            {
                PinInterestUser objPinUser = (PinInterestUser)objPinUser12;

                foreach (string strPinList in ClGlobul.addNewPinWithBoard)
                {
                    string strPin = strPinList.Replace("\0", "").Trim();
                    string[] pin = Regex.Split(strPin, ",");
                  
                    if (pin.Count() != 3)
                    {
                        GlobusLogHelper.log.Info(" => [ Please upload correct file format ]");
                        return;
                    }
                }

                PinInterestUser objPinUseaddpin = objPinUser;
                UserPins = ClGlobul.lst_AddnewPinWithNewBoard.FindAll(P => P.Email == objPinUseaddpin.Username).ToList();

                //GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Total Pins Count is " + UserPins.Count + " ]");

                if (UserPins.Count == 0)
                {
                    UserPins = ClGlobul.lst_AddnewPinWithNewBoard;
                }

                foreach (Pins pin in UserPins)
                {
                    Thread thread = new Thread(() => ThreadRepinMethod(pin, objPinUser));
                    thread.Start();
                    Thread.Sleep(ThreadDelay * 1000);
                }

            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void ThreadRepinMethod(Pins pin, PinInterestUser objPinUser)
        {
            try
            {
                Board = Regex.Split(pin.Board, ":")[0];                         
                if (!string.IsNullOrEmpty(Board))
                {
                    //Board = pin.Board;
                    BoardNumber = objAddNewPinManager.GetBoardId(Board, ref objPinUser);
                    if (string.IsNullOrEmpty(BoardNumber))
                    {
                        GlobusLogHelper.log.Info(" => [ " + Board + " Not Found. Creating Board ]");
                        BoardNumber = CreateBoard_new(Board, "Other", ref objPinUser);
                    }
                }
                else
                {
                    if (objPinUser.Boards.Count <= 0)
                    {
                        objAddNewPinManager.GetBoards(ref objPinUser);
                    }
                    if (objPinUser.Boards.Count > 0)
                    {
                        Random Boardrnd = new Random();
                        int BoardNum = 0;

                        try
                        {
                            BoardNum = Boardrnd.Next(0, objPinUser.Boards.Count - 1);
                            BoardNumber = objPinUser.Boards[BoardNum];
                        }
                        catch (Exception ex)
                        {
                            //GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartNewPinCreationMultiThreaded() 1--> " + ex.Message, ApplicationData.ErrorLogFile);
                        }
                    }
                }

                ImageUrl = pin.ImageUrl;
                Desc = pin.Description;
            
                string Data = objAddNewPinManager.NewPin(BoardNumber, Desc, ImageUrl, ref objPinUser);
                if (Data.Equals("true"))
                {
                    #region AccountReport

                    string module = "AddPinWithNewBoard";
                    string status = "Added";
                    objqm.insertAccRePort(objPinUser.Username, module, "", Board, "", Desc, "", ImageUrl, status, "", "", DateTime.Now);
                    objAddPinWithBoardDelegate();

                    #endregion

                    //GlobusLogHelper.log.Info(" => [ Pin Added To " + Board + " From " + objPinUser.Username + " ]");
                    try
                    {
                        string CSV_Header = "Date" + "," + "UserName" + "," + "Board" + "," + "Description" + "," + "ImageUrl";
                        string CSV_Data = System.DateTime.Now.ToString() + "," + objPinUser.Username + "," + Board.Replace(",", " ").Replace("'", "") + "," + Desc.Replace(",", " ").Replace("'", "") + "," + ImageUrl.Replace(",", " ").Replace("'", "");

                        PDGlobals.ExportDataCSVFile(CSV_Header, CSV_Data, Pinpath + "\\NewPin.csv");
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    GlobusLogHelper.log.Info(" => [ Pin Not Added To " + Board + " From " + objPinUser.Username + " ]");
                }

            }
            catch (Exception ex)
            {
                //GlobusFileHelper.AppendStringToTextfileNewLine("Error --> StartNewPinCreationMultiThreaded() 2 --> " + ex.Message, ApplicationData.ErrorLogFile);
            }
            finally
            {            
                try
                {
                    if (countThreadControllerAddPinWithNewBoard > Nothread_AddPinWithNewBoard)
                    {
                        lock (AddPinWithNewBoardObjThread)
                        {
                            Monitor.Pulse(AddPinWithNewBoardObjThread);
                        }
                        AddPinWithNewBoarddata_count--;
                    }

                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerAddPinWithNewBoard--;
                GlobusLogHelper.log.Info(" => [ PROCESS COMPLETED " + " For " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("---------------------------------------------------------------------------------------------------------------------------");
            
            }
        }

        public string CreateBoard_new(string BoardName, string Category, ref PinInterestUser objPinUser)
        {
            try
            {
                string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"));
                string ScreenName = objPinUser.ScreenName; //ObjAccountManager.Getscreen_NameRepin(ref objPinUser);
                if (Checking.Contains("profileName"))
                {
                }
                else
                {
                    ObjAccountManager.LoginPinterestAccount(ref objPinUser);
                }
                string redirectDomain = GlobusHttpHelper.valueURl.Split('.')[0];
                string newHomePageUrl = redirectDomain + "." + "pinterest.com";

                if (!string.IsNullOrEmpty(Checking))
                {
                    ChilkatHttpHelpr objChilkatHttpHelpr = new ChilkatHttpHelpr();

                    //string newpostdata = "source_url=%2F" + ScreenName + "%2F&data=%7B%22options%22%3A%7B%22name%22%3A%22" + (BoardName.Replace(" ", "+")) + "%22%2C%22category%22%3A%22other%22%2C%22description%22%3A%22%22%2C%22privacy%22%3A%22public%22%2C%22layout%22%3A%22default%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App()%3EUserProfilePage(resource%3DUserResource(username%3D" + ScreenName + "%2C+invite_code%3Dnull))%3EUserProfileContent(resource%3DUserResource(username%3D" + objPinUser.ScreenName + "%2C+invite_code%3Dnull))%3EUserBoards()%3EGrid(resource%3DProfileBoardsResource(username%3D" + ScreenName + "))%3EGridItems(resource%3DProfileBoardsResource(username%3D" + ScreenName + "))%3EBoardCreateRep(ga_category%3Dboard_create%2C+text%3DCreate+a+board%2C+submodule%3D%5Bobject+Object%5D)%23Modal(module%3DBoardCreate())";
                    string newpostdata = "source_url=%2F" + ScreenName + "%2F&data=%7B%22options%22%3A%7B%22name%22%3A%22" + (BoardName.Replace(" ", "+")) + "%22%2C%22category%22%3A%22other%22%2C%22description%22%3A%22%22%2C%22privacy%22%3A%22public%22%2C%22layout%22%3A%22default%22%7D%2C%22context%22%3A%7B%7D%7D&module_path=App%3EUserProfilePage%3EUserProfileContent%3EUserBoards%3EGrid%3EGridItems%3EBoardCreateRep(ga_category%3Dboard_create%2C+text%3DCreate+a+board%2C+submodule%3D%5Bobject+Object%5D)%23App%3EModalManager%3EModal(showCloseModal%3Dtrue%2C+mouseDownInModal%3Dfalse)";
                    string PostUrlBoard = redirectDomain + ".pinterest.com/resource/BoardResource/create/";
                    try
                    {
                        CreatedBoardPageSource = objPinUser.globusHttpHelper.postFormDataProxyPin(new Uri(PostUrlBoard), newpostdata, newHomePageUrl);
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                    }

                    if (CreatedBoardPageSource.Contains("error\": null"))
                    {
                        if (newpostdata.Contains("You already have a board with that name."))
                        {
                            GlobusLogHelper.log.Info(" => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
                            return null;
                        }

                        string ModuleName = "AddBoardName";
                        string Status = "Board_Created";
                        QueryManager qm = new QueryManager();
                        qm.insertAccRePort(objPinUser.Username, ModuleName, "", BoardName, "", "", "", "", Status, "", "", DateTime.Now);
                        //qm.insertBoard_AddBoardName(objPinUser.Username, ModuleName, BoardName, Status);
                        objDelegateAccountReport();

                        GlobusLogHelper.log.Info(" => [ Successfully Created Board " + BoardName + " For " + objPinUser.Username + " ]");
                        string BoardId = objAddNewPinManager.GetBoardId_Board(BoardName, ref objPinUser);//GetBoardId(BoardName, ref pinterestAccountManager);
                        return BoardId;

                    }
                    else if (CreatedBoardPageSource.Contains("You have a board with this name."))
                    {
                        GlobusLogHelper.log.Info(" => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
                        string BoardId = objAddNewPinManager.GetBoardId_Board(BoardName, ref objPinUser);
                        return BoardId;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(" => [ Board Creation Process Failed " + BoardName + " ]");
                        return CreatedBoardPageSource;
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info(" => [ Login Issue " + " For " + objPinUser.Username + " ]");
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(" => [ Board Creation Process Failed " + BoardName + " ]");
                return null;
            }
            return null;
        }


    }
}
