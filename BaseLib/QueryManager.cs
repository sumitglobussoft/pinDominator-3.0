using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BaseLib
{
    public class QueryManager
    {
        
        /// <summary>
        /// Inserts Settings in DataBase
        /// Updates if Settings already present
        /// </summary>
        /// <param name="module"></param>
        /// <param name="filetype"></param>
        /// <param name="filepath"></param>
        /// 

        #region Comment
        //public void InsertOrUpdateSetting(string module, string filetype, string filepath)
        //{
        //    try
        //    {
        //        this.Upmodule = module;
        //        this.Upfiletype = filetype;
        //        this.Upfilepath = filepath;

        //        string Upmodule = module;
        //        string UPfiletype = filetype;
        //        string strQuery = "INSERT INTO tb_Setting VALUES ('" + module + "','" + filetype + "','" + filepath + "') ";

        //        DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
        //    }
        //    catch (Exception)
        //    {
        //        UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
        //    }

        //}
        #endregion

        public void AddAccountInDataBase(string accountUser, string accountPass, string niches, string proxyAddress, string proxyPort, string proxyUserName, string proxyPassword, string Useragent, string BoardNames, string ScreenName)
        {
            string strProfileStatus = string.Empty;
            string strQuery = "INSERT INTO tb_emails (Username, Password, Niches, proxyAddress, proxyPort, proxyUsername, proxyPassword, UserAgent, Follower, Following , BOARDS, BoardsName, ScreenName) VALUES ('" + accountUser + "','" + accountPass + "', '" + niches + "' ,'" + proxyAddress + "','" + proxyPort + "',  '" + proxyUserName + "','" + proxyPassword + "','" + Useragent + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + BoardNames + "','" + ScreenName + "')";
            //"Insert into tb_emails values('" + accountUser + "','" + accountPass.Replace("\0", "") + "', '" + niches + "', '" + proxyAddress + "','" + proxyPort + "','" + proxyUserName + "','" + proxyPassword + "','" + Useragent + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + BoardNames + "','" + ScreenName + "')", "tb_emails");
            DataBaseHandler.InsertQuery(strQuery, "tb_emails");

        }
      

        public static void deleteQuery()
        {
            try 
            {
                string DeleteQuery = "Delete from tb_emails";
                DataBaseHandler.DeleteQuery(DeleteQuery, "tb_emails");
            }
            catch(Exception ex)
            { }
        }

        public static void DeleteAccounts(string username)
        {
            try
            {
                string deleteQuery = "Delete from tb_emails where Username='" + username + "'";
                DataBaseHandler.DeleteQuery(deleteQuery, "tb_emails");
            }
            catch(Exception ex)
            {

            }
        }


        public DataSet selectCommentedPinDetails(string Username, string pin)
        {
            DataSet DS = new DataSet();
            try
            {
                string query = "Select * from tb_CommentedPinDetails where AccUsename='" + Username + "' and PinId='" + pin + "'";
                DataBaseHandler.SelectQuery(query, "tb_CommentedPinDetails");
            }
            catch(Exception ex)
            { }
            return DS;
        }

        public static void insertCommentedPinDetails(string Username, string pin, string DateTime)
        {
            try
            {
                string query = "insert into tb_CommentedPinDetails(AccUsename,PinId,DateTime) values('" + Username + "','" + pin + "','" + DateTime + "')";
                DataBaseHandler.InsertQuery(query, "tb_CommentedPinDetails");
            }
            catch(Exception ex)
            { };
        }

        public void UpdateSettingData(string module, string filetype, string filepath)
        {
            try
            {
                string strTable = "tb_Setting";
                string strQuery = "INSERT into tb_Setting ( Module, FileType, FilePath) VALUES ('" + module + "' ,  '" + filetype + "' , '" + filepath + "') ";

                DataBaseHandler.InsertQuery(strQuery, strTable);
            }
            catch (Exception ex)
            {
                string strTable = "tb_Setting";
                string strQuery = "UPDATE tb_Setting SET Module='" + module + "', FilePath='" + filepath + "' WHERE FileType='" + filetype + "'";

                DataBaseHandler.UpdateQuery(strQuery, strTable);
            }
        }

        public DataTable SelectFollowsToday(string Username)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Follow Where Username = '" + Username + "' and Date = '" + DateTime.Today.ToString() + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

                DataTable dt = ds.Tables["tb_Follow"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectFollowsCheck(string Username, string FollowUser)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Follow Where Username = '" + Username + "' and Following_Username = '" + FollowUser + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

                DataTable dt = ds.Tables["tb_Follow"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public void insertFollowDate(string username, string following_username, string Keyword)
        {
            try
            {
                string strQuery = "INSERT INTO tb_Follow (Username , Following_Username , Date , Keyword )VALUES ('" + username + "','" + following_username.Replace("http://pinterest.com/", "").Replace("/", "") + "' , '" + DateTime.Today.ToString() + "' ,'" + Keyword + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Follow");
            }
            catch (Exception)
            {

            }
        }

  



        #region Comment
        //public DataSet SelectAccounts()
        //{
        //    string SelectQuery = "select * from AccountInfo";
        //    DataSet ds = DataBaseHandler.SelectQuery(SelectQuery, "AccountInfo");
        //    return ds;
        //}

        //public DataSet SelectCampaignName(string TableName, string CampaignName)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string SelectQuery = "select CampaignName  from " + TableName + " where CampaignName='" + CampaignName + "'";
        //        ds = DataBaseHandler.SelectQuery(SelectQuery, TableName);

        //    }
        //    catch { };
        //    return ds;
        //}


        //public void DeleteAccounts(string Account)
        //{
        //    try
        //    {

        //        string DeleteQuery = "delete from tb_emails where UserName='" + Account + "'";
        //        DataBaseHandler.DeleteQuery(DeleteQuery, "tb_emails");

        //    }
        //    catch (Exception Ex)
        //    {

        //    };
        //}
        //public void InsertOrUpdateScrapeSetting(string userId, string username, string TweetId)
        //{
        //    try
        //    {
        //        string strQuery = "INSERT INTO tb_ScrapeData (Userid , Username , TweetId) VALUES ('" + userId + "' , '" + username + "' , '" + TweetId + "')";
        //        DataBaseHandler.InsertQuery(strQuery, "tb_ScrapeData");
        //    }
        //    catch (Exception ex)
        //    {
        //        Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine("Error --> InsertUpdateSetting --> ScrapeData --> clsDBQueryManager --> " + ex.Message, Globals.Path_TwtErrorLogs);
        //    }

        //}

        //public void InsertScreenNameFollower(string Screen_name, string FollowerCount, string FollowingCount, string username, string FullProfileName)
        //{
        //    try
        //    {
        //        string strQuery = "UPDATE tb_FBAccount SET Screen_name = '" + Screen_name + "' , FollowerCount  = '" + FollowerCount + "', FollowingCount  = '" + FollowingCount + "',ProfileName = '" + FullProfileName + "' WHERE UserName = '" + username + "'";
        //        DataBaseHandler.UpdateQuery(strQuery, "tb_FBAccount");
        //    }
        //    catch (Exception ex)
        //    {
        //        Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine("Error --> InsertScreenNameFollower --> clsDBQueryManager --> " + ex.Message, Globals.Path_TwtErrorLogs);
        //    }
        //}


        //public DataSet InsertorUpdateUserDetailsForDirectMessaging(string Screen_name, string FollowerCount, string FollowingCount, string username, string Password)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string strQuery = "Insert into tb_AccountSendDirectMessage values('" + username + "','" + Password + "','" + "" + "','" + "" + "','" + "" + "','" + System.DateTime.Now + "')";
        //        DataBaseHandler.InsertQuery1(strQuery, "tb_AccountSendDirectMessage");
        //    }
        //    catch (Exception ex)
        //    {
        //        string query = "select * from tb_AccountSendDirectMessage where UserName='" + username + "'";
        //        ds = DataBaseHandler.SelectQuery(query, "tb_AccountSendDirectMessage");
        //    }
        //    return ds;
        //}

        //public void UpdateUserDetailsForDirectMessaging(string Screen_name, string FollowerCount, string FollowingCount, string username, string Password)
        //{
        //    try
        //    {
        //        string strQuery = "UPDATE tb_AccountSendDirectMessage SET Screen_name = '" + Screen_name + "' , FollowerCount  = '" + FollowerCount + "', FollowingCount  = '" + FollowingCount + "' WHERE UserName = '" + username + "'";
        //        DataBaseHandler.UpdateQuery(strQuery, "tb_AccountSendDirectMessage");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Write(ex.Message);
        //    }
        //}

        //public void UpdateSuspendedAcc(string username)
        //{
        //    try
        //    {
        //        string strQuery = "UPDATE tb_FBAccount SET Status = 'Suspended' WHERE UserName = '" + username + "'";
        //        DataBaseHandler.UpdateQuery(strQuery, "tb_FBAccount");
        //    }
        //    catch (Exception ex)
        //    {
        //        Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine("Error --> InsertScreenNameFollower --> clsDBQueryManager --> " + ex.Message, Globals.Path_TwtErrorLogs);
        //    }
        //}

        //public void InsertAccountGroupName(string Username, string GroupName)
        //{
        //    try
        //    {
        //        string strQuery = "UPDATE tb_FBAccount SET GroupName = '" + GroupName + "' WHERE UserName = '" + Username + "'";
        //        DataBaseHandler.UpdateQuery(strQuery, "tb_FBAccount");
        //    }
        //    catch (Exception ex)
        //    {
        //        Globussoft.GlobusFileHelper.AppendStringToTextfileNewLine("Error --> InsertAccountGroupName --> clsDBQueryManager --> " + ex.Message, Globals.Path_TwtErrorLogs);
        //    }
        //}

        //public void DeleteScrapeSettings()
        //{
        //    try
        //    {
        //        string strQuery = "DELETE From tb_ScrapeData";
        //        DataBaseHandler.InsertQuery(strQuery, "tb_ScrapeData");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public DataTable SelectSettingData()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Setting";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

        //        DataTable dt = ds.Tables["tb_Setting"];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        return new DataTable();
        //    }
        //}

        //public DataSet SelectAllData()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_FBAccount";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_FBAccount");

        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public DataSet SelectPrivateProxyData()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Proxies WHERE IsPublic = 1";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Proxies");

        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public DataSet SelectPublicProxyData()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Proxies WHERE IsPublic = 0";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Proxies");

        //        return ds;
        //    }
        //    catch (Exception)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public void DeletePublicProxyData()
        //{
        //    try
        //    {
        //        string strQuery = "DELETE From tb_Proxies WHERE IsPublic = 0";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_Proxies");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void DeletePrivateProxyData()
        //{
        //    try
        //    {
        //        string strQuery = "DELETE From tb_Proxies WHERE IsPublic = 1";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_Proxies");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void UpdateSettingData(string module, string filetype, string filepath)
        //{
        //    try
        //    {
        //        string strTable = "tb_Setting";
        //        string strQuery = "UPDATE tb_Setting SET Module='" + module + "', FilePath='" + filepath + "' WHERE FileType='" + filetype + "'";

        //        DataBaseHandler.UpdateQuery(strQuery, strTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void UpdateGroupName()
        //{
        //    try
        //    {
        //        string strTable = "tb_FBAccount";
        //        string strQuery = "UPDATE tb_FBAccount SET GroupName='" + "" + "' ";

        //        DataBaseHandler.UpdateQuery(strQuery, strTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void InsertDBCData(string username, string DeathByCaptcha, string password)
        //{
        //    try
        //    {

        //        string strQuery = "INSERT INTO tb_Setting VALUES ('" + username + "','" + DeathByCaptcha + "','" + password + "') ";

        //        DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
        //    }
        //    catch (Exception)
        //    {
        //        UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
        //    }
        //}

        //public void DeleteDBCDecaptcherData(string strDeathByCaptcha)
        //{
        //    try
        //    {

        //        string strTable = "tb_Setting";
        //        string strQuery = "DELETE FROM tb_Setting WHERE FileType='" + strDeathByCaptcha + "'";

        //        DataBaseHandler.DeleteQuery(strQuery, strTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void InsertDecaptcherData(string server, string port, string username, string password, string Decaptcher)
        //{
        //    try
        //    {

        //        string strQuery = "INSERT INTO tb_Setting VALUES ('" + server + "<:>" + port + "','" + Decaptcher + "','" + username + "<:>" + password + "') ";

        //        DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
        //    }
        //    catch (Exception)
        //    {
        //        UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
        //    }
        //}

        //public DataTable SelectFollowData(string useremail)
        //{
        //    try
        //    {

        //        string strQuery = "SELECT * FROM tb_Follow where username='" + useremail + "' ";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

        //        DataTable dt = ds.Tables["tb_Follow"];
        //        return dt;
        //    }
        //    catch (Exception)
        //    {

        //        return new DataTable();
        //    }
        //}

        //public void InsertUpdateFollowTable(string useremail, string following_id, string following_username)
        //{
        //    try
        //    {
        //        string strDateTime = DateTime.Today.ToString();
        //        string strQuery = "INSERT INTO tb_Follow (username, following_id, following_username, DateFollowed) VALUES ('" + useremail + "' , '" + following_id + "' , '" + following_username + "' , '" + strDateTime + "')";
        //        DataBaseHandler.InsertQuery(strQuery, "tb_Follow");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public List<string> SelectFollowData_List(string useremail)
        //{
        //    List<string> lst_Data = new List<string>();
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Follow where username='" + useremail + "'";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

        //        DataTable dt = ds.Tables["tb_Follow"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            try
        //            {
        //                string following_id = dRow["following_id"].ToString() + "";
        //                string following_username = dRow["following_id"].ToString() + "";
        //                lst_Data.Add(following_id);
        //                lst_Data.Add(following_username);
        //            }
        //            catch (Exception ex)
        //            {
        //                GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //            }
        //        }
        //        lst_Data = lst_Data.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return lst_Data;
        //}

        //public List<string> SelectFollowDUsername_List(string useremail)
        //{
        //    List<string> lst_Data = new List<string>();
        //    try
        //    {

        //        string strQuery = "SELECT * FROM tb_Follow where username='" + useremail + "'";
        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

        //        DataTable dt = ds.Tables["tb_Follow"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string following_username = dRow["following_username"].ToString() + "";
        //            lst_Data.Add(following_username);
        //        }
        //        lst_Data = lst_Data.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return lst_Data;
        //}

        //public List<string> SelectFollowDUsernameID_List(string useremail)
        //{
        //    List<string> lst_Data = new List<string>();
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Follow where username='" + useremail + "'";
        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");

        //        DataTable dt = ds.Tables["tb_Follow"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string following_username = dRow["following_id"].ToString() + "";
        //            lst_Data.Add(following_username);
        //        }
        //        lst_Data = lst_Data.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return lst_Data;
        //}

        //public void DeleteFollowDUsernameID_List(string useremail, string userid)
        //{
        //    try
        //    {
        //        string strQuery = "DELETE FROM tb_Follow WHERE username='" + useremail + "' and following_username = '" + userid + "'";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_Follow");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void DeleteFollowDUsernameIDFromTb_User_Followr_Details(string UserInfo)
        //{
        //    try
        //    {
        //        string strQuery = "DELETE FROM tb_user_follower_details WHERE followerName='" + UserInfo + "'";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_user_follower_details");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void InsertUpdateTBScheduler(string username, string module, string strDateTime, string IsScheduledDaily)
        //{
        //    try
        //    {
        //        string InsertQuery = "Insert into tb_Scheduler_Module (Module, ScheduledDateTime, IsScheduledDaily) VALUES ('" + module + "','" + strDateTime + "','" + IsScheduledDaily + "')";
        //        DataBaseHandler.InsertQuery1(InsertQuery, "tb_Scheduler_Module");
        //    }
        //    catch (Exception)
        //    {
        //        string UpdateQuery = "UPDATE tb_Scheduler_Module SET ScheduledDateTime='" + strDateTime + "', IsScheduledDaily='" + IsScheduledDaily + "', IsAccomplished='" + "0" + "' WHERE Module='" + module + "'";
        //        DataBaseHandler.UpdateQuery(UpdateQuery, "tb_Scheduler_Module");
        //    }

        //}

        //public void UpdateTBScheduler(string module)
        //{
        //    try
        //    {
        //        string strTable = "tb_Setting";
        //        string strQuery = "UPDATE tb_Scheduler_Module SET IsAccomplished='" + "1" + "' WHERE Module='" + module + "'";

        //        DataBaseHandler.UpdateQuery(strQuery, strTable);

        //        Increase 1 day if IsScheduledDaily
        //        {
        //            string selectQuery = "SELECT * FROM tb_Scheduler_Module where Module='" + module + "' and IsAccomplished='1'";
        //            DataSet ds = DataBaseHandler.SelectQuery(selectQuery, strTable);

        //            DataTable dt = ds.Tables["tb_Setting"];

        //            foreach (DataRow dRow in dt.Rows)
        //            {
        //                try
        //                {
        //                    string strIsScheduledDaily = dRow["IsScheduledDaily"].ToString();
        //                    if (strIsScheduledDaily == "1")
        //                    {
        //                        string scheduledTime = dRow["ScheduledDateTime"].ToString();

        //                        DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);

        //                        DateTime dt_nextscheduledTime = dt_scheduledTime.AddDays(1);

        //                        string nextscheduledTime = dt_nextscheduledTime.ToString();

        //                        string nextUpdateQuery = "UPDATE tb_Scheduler_Module SET ScheduledDateTime='" + nextscheduledTime + "', IsAccomplished='0" + "' WHERE Module='" + module + "'";

        //                        DataBaseHandler.UpdateQuery(nextUpdateQuery, "tb_Setting");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //                }

        //            }
        //        }

        //        #region MyRegion
        //        Reschedule
        //        {
        //            string selectQuery = "SELECT * FROM tb_Scheduler_Module where IsAccomplished='" + "1" + "'";
        //            DataSet ds = DataBaseHandler.SelectQuery(selectQuery, strTable);

        //            DataTable dt = ds.Tables["tb_Setting"];

        //            foreach (DataRow dRow in dt.Rows)
        //            {
        //                string strIsScheduledDaily = dRow["IsScheduledDaily"].ToString();
        //                if (strIsScheduledDaily == "1")
        //                {
        //                    string scheduledTime = dRow["ScheduledDateTime"].ToString();

        //                    DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);

        //                    DateTime dt_Now = DateTime.Now;

        //                    //if (dt_Now.Day - dt_scheduledTime.Day)
        //                    //{

        //                    //}

        //                    DateTime dt_nextscheduledTime = dt_scheduledTime.AddDays(1);

        //                    string nextscheduledTime = dt_nextscheduledTime.ToString();

        //                    string nextUpdateQuery = "UPDATE tb_Scheduler_Module SET ScheduledDateTime='" + nextscheduledTime + "' WHERE Module='" + module + "'";

        //                    DataBaseHandler.UpdateQuery(nextUpdateQuery, "tb_Setting");
        //                }

        //            }
        //        } 
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public DataTable SelectAllFromTBScheduler()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Scheduler_Module";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

        //        DataTable dt = ds.Tables["tb_Setting"];
        //        return dt;
        //    }
        //    catch { return new DataTable(); }
        //}

        //public DataTable SelectUnaccomplishedFromTBScheduler()
        //{
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_Scheduler_Module where IsAccomplished='" + "0" + "'";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

        //        DataTable dt = ds.Tables["tb_Setting"];
        //        return dt;
        //    }
        //    catch { return new DataTable(); }
        //}

        //public List<string> SelectUnaccomplishedPastScheduledTimeFromTBScheduler()
        //{
        //    #region MyRegion
        //    try
        //    {
        //        DataTable dataTable = new DataTable();

        //        string strQuery = "SELECT * FROM tb_Scheduler_Module where IsAccomplished='" + "0" + "'";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

        //        DataTable dt = ds.Tables["tb_Setting"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string scheduledTime = dRow["ScheduledDateTime"].ToString();

        //            DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);

        //            if (DateTime.Now >= dt_scheduledTime)
        //            {
        //                DataRow newRow = new object[] { dRow["Module"] };
        //                newRow.ItemArray = new object[] { dRow["Module"] };
        //                dataTable.Rows.Add(newRow);
        //            }
        //        }

        //        return dataTable;
        //    }
        //    catch { return new DataTable(); }
        //    #endregion

        //    List<string> listModules = new List<string>();

        //    try
        //    {

        //        string strQuery = "SELECT * FROM tb_Scheduler_Module where IsAccomplished='" + "0" + "'";

        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

        //        DataTable dt = ds.Tables["tb_Setting"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string scheduledTime = dRow["ScheduledDateTime"].ToString();

        //            DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);
        //            if (DateTime.Compare(dt_scheduledTime, DateTime.Now) <= 0)
        //            {
        //                if (DateTime.Now >= dt_scheduledTime)
        //                {
        //                    listModules.Add(dRow["Module"].ToString());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return listModules;
        //}

        //public void DeleteAccomplishedFromTBScheduler()
        //{
        //    try
        //    {
        //        string strQuery = "Delete FROM tb_Scheduler_Module where IsAccomplished='" + "1" + "'";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_Scheduler_Module");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //}

        //public void DeleteSelectedRowFromTBScheduler(string id)
        //{
        //    try
        //    {
        //        string strQuery = "Delete FROM tb_Scheduler_Module where Id='" + id + "'";
        //        DataBaseHandler.DeleteQuery(strQuery, "tb_Scheduler_Module");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void InsertUserNameId(string Username, string UserId)
        //{
        //    try
        //    {
        //        string strQuery = "INSERT INTO tb_UsernameDetails VALUES ('" + Username + "','" + UserId + "') ";

        //        DataBaseHandler.InsertQuery(strQuery, "tb_UsernameDetails");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public DataSet GetUserId(string username)
        //{
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        string strQuery = "SELECT Userid FROM tb_UsernameDetails WHERE Username = '" + username + "' ";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_UsernameDetails");

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public DataSet GetUserIdForuser_follower_details(string username)
        //{
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        string strQuery = "SELECT followerId FROM tb_user_follower_details WHERE followerName = '" + username + "' ";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_user_follower_details");

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public DataSet GetUserName(string username)
        //{
        //    DataSet ds = new DataSet();

        //    try
        //    {
        //        string strQuery = "SELECT Username FROM tb_UsernameDetails WHERE Userid = '" + username + "' ";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_UsernameDetails");

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DataSet();
        //    }
        //}

        //public void InsertDataRetweet(string Username, string RetweetUsername, string Tweet)
        //{
        //    try
        //    {
        //        Tweet = StringEncoderDecoder.Encode(Tweet);
        //        string strQuery = "INSERT INTO tb_RetweetData (Username , RetweetUsername , Tweet , DateTime) VALUES ('" + Username + "' , '" + RetweetUsername + "' , '" + Tweet + "' ,'" + DateTime.Now.ToString() + "')";
        //        DataBaseHandler.InsertQuery(strQuery, "tb_RetweetData");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public void InsertMessageData(string Username, string Type, string OtherUsername, string Message)
        //{
        //    try
        //    {
        //        string strQuery = "INSERT INTO tb_MessageRecord (Username , Type , OtherUsername , Message , DateTime ) VALUES ('" + Username + "' , '" + Type + "' , '" + OtherUsername + "' , '" + Message + "' , '" + DateTime.Today.ToString() + "') ";

        //        DataBaseHandler.InsertQuery(strQuery, "tb_MessageRecord");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public DataSet SelectMessageData(string Username, string Type)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_MessageRecord WHERE Type = '" + Type + "' and Username = '" + Username + "' and DateTime = '" + DateTime.Today.ToString() + "'";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_MessageRecord");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return ds;
        //}

        //public DataSet SelectMessageDataForRetweet(string Username, string IdtweetData, string Type)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string strQuery = "SELECT * FROM tb_MessageRecord WHERE Type = '" + Type + "' and Username = '" + Username + "' and OtherUsername='" + IdtweetData + "' and DateTime = '" + DateTime.Today.ToString() + "'";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_MessageRecord");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return ds;
        //}

        //public DataSet SelectFollowMessageData(string Username)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        string strQuery = "SELECT * FROM tb_Follow WHERE username = '" + Username + "' and DateFollowed = '" + DateTime.Today.ToString() + "'";

        //        ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return ds;
        //}

        //#region MyRegion
        //public void DeleteDecaptcherData(string strDecaptcher)
        //{
        //    try
        //    {

        //        string strTable = "tb_Setting";
        //        string strQuery = "DELETE FROM tb_Setting WHERE FileType=" + strDecaptcher;

        //        DataBaseHandler.DeleteQuery(strQuery, strTable);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        //#endregion



        //public void InserOrUpdateUnfollower(string useremail, string Unfollowing_id, string Unfollowing_username)
        //{
        //    try
        //    {
        //        string strDateTime = DateTime.Today.ToString();
        //        string strQuery = "INSERT INTO Unfollow (UserName, Unfollower_id, Unfollower_UserName, DateUnfollowed) VALUES ('" + useremail + "' , '" + Unfollowing_id + "' , '" + Unfollowing_username + "' , '" + strDateTime + "')";
        //        DataBaseHandler.InsertQuery(strQuery, "Unfollow");
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //}

        //public List<string> SelectUnFollowDUsernameID_List(string useremail)
        //{
        //    List<string> lst_Data = new List<string>();
        //    try
        //    {
        //        string strQuery = "SELECT * FROM Unfollow where username='" + useremail + "'";
        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "Unfollow");

        //        DataTable dt = ds.Tables["Unfollow"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string following_username = dRow["Unfollower_UserName"].ToString() + "";
        //            lst_Data.Add(following_username);
        //        }
        //        lst_Data = lst_Data.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return lst_Data;
        //}

        //public static DataSet SelectScheduledCampaign(string CampainName)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string SelectQuery = "Select * From Campaign_tweet Where ScheduledDaily ='1'";
        //        ds = DataBaseHandler.SelectQuery(SelectQuery, "Campaign_tweet");
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //    return ds;
        //}


        //public List<string> SelectUnFollowedUsername_List(string useremail)
        //{
        //    List<string> lst_Data = new List<string>();
        //    try
        //    {

        //        string strQuery = "SELECT * FROM Unfollow where username='" + useremail + "'";
        //        DataSet ds = DataBaseHandler.SelectQuery(strQuery, "Unfollow");

        //        DataTable dt = ds.Tables["Unfollow"];

        //        foreach (DataRow dRow in dt.Rows)
        //        {
        //            string following_username = dRow["Unfollower_id"].ToString() + "";
        //            lst_Data.Add(following_username);
        //        }
        //        lst_Data = lst_Data.Distinct().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }
        //    return lst_Data;
        //}



        //public void UpdatePasswordForAccount(string userName, string newPassword)
        //{
        //    try
        //    {
        //        string UpdateQuery = "update tb_FBAccount set  Password='" + newPassword + "' WHERE UserName='" + userName + "';";
        //        DataBaseHandler.UpdateQuery(UpdateQuery, "tb_FBAccount");

        //    }
        //    catch (Exception ex)
        //    {
        //        GlobusLogHelper.log.Error("Error : " + ex.StackTrace);
        //    }

        //}

#endregion

    }
}
