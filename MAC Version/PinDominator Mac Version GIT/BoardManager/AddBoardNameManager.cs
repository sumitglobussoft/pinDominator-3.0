
using BaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AccountManager;
using PinDominator;
using PinsManager;
using Globussoft;

namespace BoardManager
{
	public delegate void AccountReport_AddBoard();
	public delegate void AddBoardAccount(int AccountCount);
    public class AddBoardNameManager
    {
		public static AccountReport_AddBoard objAddBoardDelegate;
		public static AddBoardAccount objDelegateNoOfActiveAcc;
		public static AddBoardAccount objDelegateNoOfDeadAcc;

        #region Global Variable

        public string[] noOfBoard = null;
        public int Nothread_AddBoardName = 5;
        public bool isStopAddBoardName = false;
        public List<Thread> lstThreadsAddBoardName = new List<Thread>();
        public static int countThreadControllerAddBoardName = 0;
        public static int AddBoardNamedata_count = 0;
        public readonly object AddBoardNameObjThread = new object();
        public bool _Isfevorite = false;
        public bool rdbSingleUserAddBoardName = false;
        public bool rdbMultipleUserAddBoardName = false;
        public string SingleBoardName = string.Empty;

        public int minDelayAddBoardName
        {
            get;
            set;
        }

        public int maxDelayAddBoardName
        {
            get;
            set;
        }

        public int NoOfThreadsAddBoardName
        {
            get;
            set;
        }

		int ActiveAccCount = 0;
		int DeadAccCount = 0;

        string[] array = null;
        string CategoryName = string.Empty;

        Accounts ObjAccountManager = new Accounts();
		AddNewPinManager objAddNewPinManager = new AddNewPinManager();

        #endregion

        public void StartAddBoardName()
        {
            try
            {
                countThreadControllerAddBoardName = 0;
                int numberOfAccountPatchAddBoardName = 25;

                if (NoOfThreadsAddBoardName > 0)
                {
                    numberOfAccountPatchAddBoardName = NoOfThreadsAddBoardName;
                }

                AddBoardNamedata_count = PDGlobals.loadedAccountsDictionary.Count();

                List<List<string>> list_listAccounts = new List<List<string>>();
                if (PDGlobals.listAccounts.Count >= 1)
                {
                    list_listAccounts = Utils.Utils.Split(PDGlobals.listAccounts, numberOfAccountPatchAddBoardName);
                    foreach (List<string> listAccounts in list_listAccounts)
                    {
                        foreach (string account in listAccounts)
                        {
                            if (countThreadControllerAddBoardName > Nothread_AddBoardName)
                            {
                                try
                                {
                                    lock (AddBoardNameObjThread)
                                    {
                                        Monitor.Wait(AddBoardNameObjThread);
                                    }
                                }
                                catch (Exception Ex)
                                {
									Console.Write(Ex.Message);
                                }
                            }
                            string acc = account.Split(':')[0];
                            PinInterestUser objPinInterestUser = null;
                            PDGlobals.loadedAccountsDictionary.TryGetValue(acc, out objPinInterestUser);
                            if (objPinInterestUser != null)
                            {                          
                                Thread profilerThread = new Thread(StartAddBoardNameMultiThreaded);
                                profilerThread.Name = "workerThread_Profiler_" + acc;
                                profilerThread.IsBackground = true;

                                profilerThread.Start(new object[] { objPinInterestUser });

                                countThreadControllerAddBoardName++;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
            }
        }

        public void StartAddBoardNameMultiThreaded(object objParameters)
        {
            PinInterestUser objPinUser = new PinInterestUser();
            try
            {
                if (!isStopAddBoardName)
                {
                    try
                    {
                        lstThreadsAddBoardName.Add(Thread.CurrentThread);
                        lstThreadsAddBoardName.Distinct().ToList();
                        Thread.CurrentThread.IsBackground = true;
                    }
                    catch (Exception ex)
                    {
						Console.Write(ex.Message);
					}

                    try
                    {
                        Array paramsArray = new object[1];
                        paramsArray = (Array)objParameters;

                        objPinUser = (PinInterestUser)paramsArray.GetValue(0);

                        foreach (string Boardnames in ClGlobul.lstBoardNames)
                        {
                            array = Regex.Split(Boardnames, ":");
                            if (array.Length == 2)
                            {
                                if (array[0] == objPinUser.Niches)
                                {
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
                                            checkLogin = ObjAccountManager.LoginPinterestAccount1forlee(ref objPinUser, objPinUser.Username, objPinUser.Password, objPinUser.ProxyAddress, objPinUser.ProxyPort, objPinUser.ProxyUsername, objPinUser.ProxyPassword, objPinUser.ScreenName);

                                            if (!checkLogin)
                                            {
												GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logging UnSuccessfull : " + objPinUser.Username + " ]");
												DeadAccCount++;
												objDelegateNoOfDeadAcc(DeadAccCount);
												return;
                                            }
											string checklogin = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com"));
                                          
                                        }

                                        catch (Exception ex)
                                        {
											Console.Write(ex.Message);
										}
                                    }
                                    if(objPinUser.isloggedin == true)
                                    {
                                        try
                                        {
											GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Logged In With : " + objPinUser.Username + " ]");
											ActiveAccCount++;
											objDelegateNoOfActiveAcc(ActiveAccCount);
                                            StartActionMultithreadAddBoardName(ref objPinUser);
                                        }
                                        catch (Exception ex)
                                        {
                                            GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
                                        }
                                    }
                                }
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

        public void StartActionMultithreadAddBoardName(ref PinInterestUser objPinUser)
        {
            try 
            {
                try
                {
                    lstThreadsAddBoardName.Add(Thread.CurrentThread);
                    lstThreadsAddBoardName.Distinct().ToList();
                    Thread.CurrentThread.IsBackground = true;
                }
                catch (Exception ex)
                { };
                foreach (string Boardnames in ClGlobul.lstBoardNames)
                {
                   string[] array1 = Regex.Split(Boardnames, ":");
                    string[] arrayKeyword = Regex.Split(array1[1], ",");

                    if (array1.Length == 2)
                    {
                        if (array1[0] == objPinUser.Niches)
                        {
                            AddPinWithNewBoardManager objAddPin = new AddPinWithNewBoardManager();

                            if (arrayKeyword.Length > 0)
                            {
                                foreach (string name in arrayKeyword)
                                {
                                    CreateAddBoard_new(name, ref objPinUser);
                                    int delay = RandomNumberGenerator.GenerateRandom(minDelayAddBoardName, maxDelayAddBoardName);
									GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Delay For " + delay + " Seconds ] ");
                                    Thread.Sleep(delay * 1000);
                                }
                            }
                            else
                            {
								GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ No Board Names With Niche " + objPinUser.Username + " ]");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }

            finally
            {
                 try
                {
                    if (countThreadControllerAddBoardName > Nothread_AddBoardName)
                    {
                        lock (AddBoardNameObjThread)
                        {
                            Monitor.Pulse(AddBoardNameObjThread);
                        }
                        AddBoardNamedata_count--;
                    }
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error(" Error : " + ex.StackTrace);
                }
                countThreadControllerAddBoardName--;
				GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] [ PROCESS COMPLETED FOR " + objPinUser.Username + " ]");
                GlobusLogHelper.log.Info("--------------------------------------------------------------------------------");
            }
        }


		public string CreateAddBoard_new(string BoardName, ref PinInterestUser objPinUser)
		{
			try
			{
				string CreatedBoardPageSource = string.Empty;
				string Checking = objPinUser.globusHttpHelper.getHtmlfromUrl(new Uri("https://www.pinterest.com/settings/"));
				string ScreenName = objPinUser.ScreenName; 
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
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
							return null;
						}
						try
						{
						    string ModuleName = "AddBoardName";
							string Status = "Board_Created";
							QueryManager qm = new QueryManager();
							qm.insertAccRePort(objPinUser.Username, ModuleName, "", BoardName, "", "", "", "", Status, "", "", DateTime.Now);
							objAddBoardDelegate();
						}
						catch (Exception ex)
						{
							GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Board Created " + BoardName + " ]");
							return null;
						}

						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Successfully Created Board " + BoardName + " For " + objPinUser.Username + " ]");
						string BoardId = objAddNewPinManager.GetBoardId_Board(BoardName, ref objPinUser);
						return BoardId;

					}
					else if (CreatedBoardPageSource.Contains("You have a board with this name."))
					{
						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ You already have a board with this name. " + BoardName + " For " + objPinUser.Username + " ]");
						string BoardId = objAddNewPinManager.GetBoardId_Board(BoardName, ref objPinUser);
						return BoardId;
					}
					else
					{
						GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Board Creation Process Failed " + BoardName + " ]");
						return CreatedBoardPageSource;
					}
				}
				else
				{
					GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Login Issue " + " For " + objPinUser.Username + " ]");
				}
			}
			catch (Exception ex)
			{
				GlobusLogHelper.log.Info("[ " + DateTime.Now + " ] => [ Board Creation Process Failed " + BoardName + " ]");
				return null;
			}
			return null;
		}



    }
}
