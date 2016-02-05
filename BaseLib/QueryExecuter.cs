using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BaseLib
{
   public class QueryExecuter
    {

       private const string tb_emails = "tb_emails";
       private const string tableFollowerName = "tb_FollowerName";
        private const string tableLikeInfo = "LikeInfo";

        //public static void insertAccount(string username, string password, string niches, string addr, string port, string user, string pass, string Useragent, string BoardNames, string ScreenName)
        //{
        //    try
        //    {
        //        string insertQuery = "Insert into " + tb_emails + " (Username, Password, niches, proxyAddress,proxyPort,proxyUsername,proxyPassword,Path) values('" + username + "','" + password + "','" + addr + "','" + port + "','" + user + "','" + pass + "','" + Useragent + "')";
        //        DBHandler.InsertQuery(insertQuery, tb_emails);
        //    }
        //    catch { }
        //}

        public static void deleteQuery()
        {
            try
            {
                string deleteQuery = "Delete from " + tb_emails;
                DBHandler.DeleteQuery(deleteQuery, tb_emails);
            }
            catch { }
        }

        public DataSet getAccount()
        {
            DataSet ds = new DataSet();
            try
            {
                //string selectQuery = "Select UserName,Password,Niches,ProxyAddress,ProxyPort,ProxyUserName,ProxyPassword,UserAgent,Follower,Following,BOARDS,BoardsName,ScreenName from " + tb_emails;
                string selectQuery = "Select UserName,Password,Niches,ProxyAddress,ProxyPort,ProxyUserName,ProxyPassword,Follower,Following,ScreenName,LoginStatus from " + tb_emails;
                ds = DBHandler.SelectQuery(selectQuery, tb_emails);
            }
            catch { }
            return ds;
        }

        public DataSet getFollower(string screen_Name)
        {
            DataSet DS = new DataSet();
           try
           {
               string selectquery = "Select * from  tb_FollowerName where UserName='" + screen_Name + "'";
               DS = DBHandler.SelectQuery(selectquery, "tb_FollowerName");
           }
           catch(Exception ex)
           {

           }
            return DS;
        }

        public static void deleteFollower(string screen_Name, string unfollower)
        {
            try
            {
                string query = "DELETE FROM  tb_FollowerName where UserName='" + screen_Name + "' and FollwerName='" + unfollower + "'";
                DataBaseHandler.DeleteQuery(query, "tb_FollowerName");
            }
            catch(Exception ex)
            {
            }
        }

        public static void insertFollowerName(string screen_Name, string follName_item)
        {
            try
            {
                string query = "INSERT INTO  tb_FollowerName (UserName,FollwerName) VALUES ('" + screen_Name + "' ,'" + follName_item + "') ";
                DataBaseHandler.InsertQuery(query, "tb_FollowerName");
            }
            catch (Exception ex)
            {
            }
        }

        public static void updatetb_emails(string follower, string followingCount, string BOARDS, string BoardsName, string screen_Name, string Username, string LoginStatus)
        {
           try
           {
               string UpdateQuery = "Update tb_emails set Follower = '" + follower + "',Following = '" + followingCount + "',BOARDS= '" + BOARDS + "',BOARDSNAME='" + BoardsName + "',ScreenName='" + screen_Name + "',LoginStatus='" + LoginStatus + "' where UserName = '" + Username + "'";
               DataBaseHandler.UpdateQuery(UpdateQuery, "tb_emails");
           }
           catch(Exception ex)
           { };
        }



        #region Comment
        //public static void insertFollowInfo(string account, string user, string status)
        //{
        //    string dt = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
        //    try
        //    {
        //        string insertQuery = "insert into " + tableFollowInfo + "(AccountHolder,FollowingUser,FollowTime,FollowStatus) values('" + account + "','" + user + "','" + dt + "','" + status + "')";
        //        DBHandler.InsertQuery(insertQuery, tableFollowInfo);
        //    }
        //    catch { }
        //}

        //public static DataSet getFollowInfo(string accountHolder)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string selectQuery = "Select FollowingUser,FollowTime from " + tableFollowInfo + " where AccountHolder='" + accountHolder + "'";
        //        ds = DBHandler.SelectQuery(selectQuery, tableFollowInfo);
        //    }
        //    catch { }
        //    return ds;
        //}

        /// <summary>
        /// LikeInfo Insert,Update,Select
        /// </summary>
        /// <returns></returns>

        //public DataSet getLikeInfo()
        //{
        //    string SelectQuery = "select LikePhotoId from LikeInfo where Status=0";
        //    DataSet ds = DBHandler.SelectQuery(SelectQuery, "LikeInfo");
        //    return ds;
        //}

        //public static void insertPhotoId(string UseName, string LikePhotoId)
        //{

        //    try
        //    {
        //        string insertQuery = "insert into " + tableLikeInfo + "(UseName,Status,LikePhotoId) values('" + UseName + "','" + "1" + "','" + LikePhotoId + "')";
        //        DBHandler.InsertQuery(insertQuery, tableLikeInfo);
        //    }
        //    catch { }
        //}

        //public static void UpdateStatusPhotoId(string UseName, string LikePhotoId)
        //{
        //    try
        //    {
        //        string UpdateQuery = "UPDATE " + tableLikeInfo + " SET Status =1 Where LikePhotoId ='" + LikePhotoId + "'";
        //        DBHandler.UpdateQuery(UpdateQuery, "tableLikeInfo");
        //    }
        //    catch { };
        //}

        //public static string likeStatus(string id, string username)
        //{
        //    string status = string.Empty;
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        string selectQuery = "select Status from " + tableLikeInfo + " where UseName='" + username + "' and PhotoId='" + id + "'";
        //        ds = DBHandler.SelectQuery(selectQuery, tableLikeInfo);
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            int cnt = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0]);
        //            if (cnt == 1)
        //            {
        //                status = "Liked";
        //            }


        //            else if (cnt == 2)
        //            {
        //                status = "Unliked";
        //            }
        //        }
        //    }
        //    catch { }
        //    return status;
        //}

        //public static void insertLikeStatus(string id, string username, int status)
        //{
        //    try
        //    {
        //        string insertQuery = "Insert into " + tableLikeInfo + " (UseName,Status,LikePhotoId) values('" + username + "'," + status + ",'" + id + "')";
        //        DBHandler.InsertQuery(insertQuery, tableLikeInfo);
        //    }
        //    catch { }
        //}

        //public static string getFollowStatus(string user, string followingUser)
        //{
        //    string status = string.Empty;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        string selectQuery = "Select FollowStatus from " + tableFollowInfo + " where AccountHolder='" + user + "' and FollowingUser='" + "http://websta.me/n/" + followingUser + "/" + "'";
        //        //Select  FollowStatus  from FollowInfo where AccountHolder='jolandamanlio929' and FollowingUser  ='http://websta.me/n/kamaliat_/'
        //        ds = DBHandler.SelectQuery(selectQuery, tableFollowInfo);
        //    }
        //    catch { }
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        try
        //        {
        //            status = ds.Tables[0].Rows[0].ItemArray[0].ToString();
        //        }
        //        catch { }
        //    }
        //    return status;
        //}

        //public static string getFollowStatus1(string user, string followingUser)
        //{
        //    string status = string.Empty;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        string selectQuery = "Select FollowStatus from " + tableFollowInfo + " where AccountHolder='" + user + "' and FollowingUser='" + followingUser + "'";
        //        //Select  FollowStatus  from FollowInfo where AccountHolder='jolandamanlio929' and FollowingUser  ='http://websta.me/n/kamaliat_/'
        //        ds = DBHandler.SelectQuery(selectQuery, tableFollowInfo);
        //    }
        //    catch { }
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        try
        //        {
        //            status = ds.Tables[0].Rows[0].ItemArray[0].ToString();
        //        }
        //        catch { }
        //    }
        //    return status;
        //}

        //public static void updateFollowStatus(string user, string followingUser, string status)
        //{
        //    try
        //    {
        //        string perfomQuery = "Update " + tableFollowInfo + " set FollowStatus='" + status + "' where AccountHolder='" + user + "' and FollowingUser='" + "http://websta.me/n/" + followingUser + "/" + "'";
        //        DBHandler.PerformQuery(perfomQuery, tableFollowInfo);
        //    }
        //    catch { }
        //}
        #endregion



    }
}
