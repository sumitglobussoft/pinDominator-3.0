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

        public void AddAccountInDataBase(string accountUser, string accountPass, string niches, string proxyAddress, string proxyPort, string proxyUserName, string proxyPassword, string ScreenName, string LoginStatus)
        {
            string strProfileStatus = string.Empty;
            string strQuery = "INSERT INTO tb_emails (Username, Password, Niches, proxyAddress, proxyPort, proxyUsername, proxyPassword, UserAgent, Follower, Following , BOARDS, BoardsName, ScreenName, LoginStatus) VALUES ('" + accountUser + "','" + accountPass + "', '" + niches + "' ,'" + proxyAddress + "','" + proxyPort + "',  '" + proxyUserName + "','" + proxyPassword + "','" + " " + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + " " + "','" + ScreenName + "','" + LoginStatus + "' )";
            //"Insert into tb_emails values('" + accountUser + "','" + accountPass.Replace("\0", "") + "', '" + niches + "', '" + proxyAddress + "','" + proxyPort + "','" + proxyUserName + "','" + proxyPassword + "','" + Useragent + "','" + " " + "','" + " " + "','" + " ," + "','" + "," + "," + BoardNames + "','" + ScreenName + "')", "tb_emails");
            DataBaseHandler.InsertQuery(strQuery, "tb_emails");

        }

        public DataSet SelectAddReport(string module)
        {         
            try
            {
             string query = "Select * from tb_AccountReport where  ModuleName='" + module + "'";
             DataSet ds = DataBaseHandler.SelectQuery(query, "tb_AccountReport");
             return ds;
            }
            catch(Exception ex)
            {
                return new DataSet();
            }
           
        }

        public static void deleteQuery()
        {
            try
            {
                string DeleteQuery = "Delete from tb_emails";
                DataBaseHandler.DeleteQuery(DeleteQuery, "tb_emails");
            }
            catch (Exception ex)
            { }
        }

        public static void DeleteAccounts(string username)
        {
            try
            {
                string deleteQuery = "Delete from tb_emails where Username='" + username + "'";
                DataBaseHandler.DeleteQuery(deleteQuery, "tb_emails");
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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

      
        public void insertBoard_AddBoardName(string accUser, string module, string BoardName, string status)
        {
            try
            {
                string insertBoard = "Insert into tb_AccountReport (AccountName, ModuleName, BoardName, Status) values ( '" + accUser + "', '" + module + "', '" + BoardName + "', '" + status + "')";
                DataBaseHandler.InsertQuery(insertBoard, "tb_AccountReport");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }
    

        public void insertAccRePort(string AccountName, string Module, string Pin, string BoardName, string Username, string Msg, string Keyword, string ImageUrl, string status, string NewEmail, string NewPasssword, DateTime DateTime)
        {
            try
            {
                string insertQuery = "Insert into tb_AccountReport (AccountName, ModuleName, PinNo, BoardName, UserName, Message, Keyword, ImageUrl, Status, NewEmail, NewPassword, DateAndTime) values ( '" + AccountName + "', '" + Module + "', '" + Pin + "', '" + BoardName + "', '" + Username + "', '" + Msg + "', '" + Keyword + "', '" + ImageUrl + "', '" + status + "', '" + NewEmail + "', '" + NewPasssword + "', '" + DateTime + "')";
                DataBaseHandler.InsertQuery(insertQuery, "tb_AccountReport");
            }
            catch(Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public void insertAccReportScrapeUser(string Module, string UserName, string status, DateTime DateAndTime)
        {

            try
            {
                string insertQuery = "Insert into tb_AccReportScrapeUser (ModuleName, UserName, Status, DateTime) values ('" + Module + "', '" + UserName + "','" + status + "', '" + DateAndTime + "')";
                DataBaseHandler.InsertQuery(insertQuery, "tb_AccountReport");
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(" Error :" + ex.StackTrace);
            }
        }

        public DataSet SelectAddReportScrapeUser(string module)
        {
            try
            {
                string query = "Select * from tb_AccReportScrapeUser where  ModuleName='" + module + "'";
                DataSet ds = DataBaseHandler.SelectQuery(query, "tb_AccReportScrapeUser");
                return ds;
            }
            catch (Exception ex)
            {
                return new DataSet();
            }

        }

        public  void DeleteAccountReport(string module)
        {
            try
            {
                string deleteQuery = "Delete from tb_AccountReport where ModuleName='" + module + "'";
                DataBaseHandler.DeleteQuery(deleteQuery, "tb_AccountReport");
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteAccountReportScrapeUser(string module)
        {
            try
            {
                string deleteQuery = "Delete from tb_AccReportScrapeUser where ModuleName='" + module + "'";
                DataBaseHandler.DeleteQuery(deleteQuery, "tb_AccReportScrapeUser");
            }
            catch (Exception ex)
            {

            }
        }



    }
}
