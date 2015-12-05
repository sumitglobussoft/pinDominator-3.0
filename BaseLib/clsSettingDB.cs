using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BaseLib
{
    public class clsSettingDB
    {
        string Upmodule = string.Empty;
        string Upfiletype = string.Empty;
        string Upfilepath = string.Empty;
        string DeathByCaptcha = string.Empty;

        /// <summary>
        /// Inserts Settings in DataBase
        /// Updates if Settings already present
        /// </summary>
        /// <param name="module"></param>
        /// <param name="filetype"></param>
        /// <param name="filepath"></param>
        public void InsertOrUpdateSetting(string module,string filetype,string filepath)
        {
            try
            {
                this.Upmodule=module;
                this.Upfiletype=filetype;
                this.Upfilepath=filepath;

                string Upmodule=module;
                string UPfiletype=filetype;
                string strQuery = "INSERT INTO tb_Setting VALUES ('" + module + "','" + filetype + "','" + filepath  + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }

        }

        public DataSet SelectDataFromtb_emails()
        {
            try
            {
                string strQuery = "SELECT * FROM tb_emails";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_emails");
                return ds;
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public void InsertOrUpdateFollow(string Username, string FollowedUsername, string keyword)
        {
            try
            {
                string strQuery = "INSERT INTO tb_Follow (Username , FollowedUsername , DateTime , Keyword ) VALUES ('" + Username + "','" + FollowedUsername + "','" + DateTime.Today.ToString() + "' , '" + keyword + "') ";
                DataBaseHandler.InsertQuery(strQuery, "tb_FollowRecord");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }

        }

        public DataTable SelectSettingData()
        {
            try
            {

                string strQuery = "SELECT * FROM tb_Setting";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

                DataTable dt = ds.Tables["tb_Setting"];
                return dt;
            }
            catch (Exception)
            {

                return new DataTable();
            }
        }     

        public void UpdateSettingData(string module, string filetype, string filepath)
        {
            try
            {
                string strTable = "tb_Setting";
                string strQuery = "INSERT into tb_Setting ( Module, FileType, FilePath) VALUES ('" + module + "' ,  '" + filetype + "' , '" + filepath + "') ";

                DataBaseHandler.InsertQuery(strQuery, strTable);
            }
            catch (Exception)
            {
                string strTable = "tb_Setting";
                string strQuery = "UPDATE tb_Setting SET Module='" + module + "', FilePath='" + filepath + "' WHERE FileType='" + filetype + "'";

                DataBaseHandler.UpdateQuery(strQuery, strTable);
            }
        }

        public void InsertDBCData(string username, string DeathByCaptcha, string password)
        {
            try
            {                
                string strQuery = "INSERT INTO tb_Setting VALUES ('" + username + "','" + DeathByCaptcha + "','" + password + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }
        }

        public void DeleteDBCDecaptcherData(string strDeathByCaptcha)
        {
            try
            {
                string strTable = "tb_Setting";
                string strQuery = "DELETE FROM tb_Setting WHERE FileType='" + strDeathByCaptcha + "'";

                DataBaseHandler.DeleteQuery(strQuery, strTable);
            }
            catch (Exception)
            {
            }
        }

        public void InsertDecaptcherData(string server, string port, string username, string password, string Decaptcher)
        {
            try
            {
                string strQuery = "INSERT INTO tb_Setting VALUES ('" + server + "<:>" + port + "','" + Decaptcher + "','" + username + "<:>" + password + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
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

        public void InsertPinDesc(string PinID, string Desc, string Username)
        {
            try
            {
                string strQuery = "INSERT INTO tb_EditDescription (PinID , Desc , Username ) VALUES ('" + PinID + "','" + Desc + "','" + Username + "') ";
                DataBaseHandler.InsertQuery(strQuery, "tb_EditDescription");
            }
            catch (Exception)
            {
                
            }
        }

        public DataTable SelectPinDesc(string PinID, string Username)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_EditDescription Where PinID = '" + PinID + "' and Username = '" + Username + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_EditDescription");

                DataTable dt = ds.Tables["tb_EditDescription"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }



        public void insertMessageDate(string username, string following_username, string Pin , string Keyword , string Message)
        {
            try
            {
                string strQuery = "INSERT INTO tb_Message (Username , User , Pin , Message ,  DateTime , keyword ) VALUES ('" + username + "','" + following_username.Replace("http://pinterest.com/", "").Replace("/", "") + "' , '"+ Pin +"' , '" + Message + "' , '" + DateTime.Today.ToString() + "' ,'" + Keyword + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Message");
            }
            catch (Exception)
            {

            }
        }

        public void insertPinRecord(string Pin , string Niche , string Keyword , string BoardUrl)
        {
            try
            {
                string strQuery1 = "SELECT * From tb_PinRecords WHERE Pin = '"+ Pin +"' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery1, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                if (dt.Rows.Count <= 0)
                {
                    string strQuery = "INSERT INTO tb_PinRecords (Pin , Niche ,  Keyword , BaordUrl , DateTime , Use) VALUES ('" + Pin.Replace("http://pinterest.com/", "").Replace("/", "") + "','" + Niche + "' , '" + Keyword + "' , '" + BoardUrl + "' , '" + DateTime.Today.ToString() + "' , 'NotUsed') ";

                    DataBaseHandler.InsertQuery(strQuery, "tb_PinRecords");
                }
            }
            catch (Exception)
            {

            }
        }

        public void UpdatingPinRecord(string Pin)
        {
            try
            {
                string strQuery = "UPDATE tb_PinRecords SET Use = 'Used' WHERE Pin = '"+ Pin +"' ";
                DataBaseHandler.UpdateQuery(strQuery, "tb_PinRecords");
            }
            catch (Exception)
            {

            }
        }

        public void insertRePinRecord(string Username, string Niche, string Repin)
        {
            try
            {
                string strQuery = "INSERT INTO tb_RepinRecord (Username , Niche ,  Repin , DateTime ) VALUES ('" + Username + "','" + Niche + "' , '" + Repin + "' , '" + DateTime.Today.ToString() + "')";

                DataBaseHandler.InsertQuery(strQuery, "tb_RepinRecord");
            }
            catch (Exception)
            {

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

        public DataTable SelectRepin(string Username, string Repin)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_RepinRecord Where Username = '" + Username + "' and Repin = '" + Repin + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_RepinRecord");

                DataTable dt = ds.Tables["tb_RepinRecord"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }


        public DataTable SelectPinForAddPins(string Keyword, string pin)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_PinRecords Where Keyword = '" + Keyword + "' and Pin = '" + pin + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable GetBoardUrl(string BoardUrl)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_BoardUrlRecord Where BoardUrl = '" + BoardUrl + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_BoardUrlRecord");

                DataTable dt = ds.Tables["tb_BoardUrlRecord"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable GetKeywordUrl(string Niches ,string Keyword)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_KeywordPin Where Niches = '" + Niches + "' and Keyword = '" + Keyword + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_KeywordPin");

                DataTable dt = ds.Tables["tb_KeywordPin"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public void AddingBoardUrl(string BoardUrl, string PinnedonBoard, string BoardId)
        {
            try
            {
                string strQuery = "INSERT INTO tb_BoardUrlRecord (BoardUrl , PinnedOnBoard , BoardId) VALUES ('" + BoardUrl + "','" + PinnedonBoard + "' , '" + BoardId + "')";

                DataBaseHandler.SelectQuery(strQuery, "tb_BoardUrlRecord");
            }
            catch (Exception)
            {
                
            }
        }

        public void AddingKeywordUrl(string Niches, string Keyword, string BoardId , string BoardName)
        {
            try
            {
                string strQuery = "INSERT INTO tb_KeywordPin (Niches, Keyword, BoardId, PinnedOnBoard) VALUES ('" + Niches + "','" + Keyword + "' , '" + BoardId + "' , '" + BoardName + "')";

                DataBaseHandler.SelectQuery(strQuery, "tb_KeywordPin");
            }
            catch (Exception)
            {

            }
        }

        public DataTable SelectPins(string BaordUrl , int count)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_PinRecords Where BaordUrl = '" + BaordUrl + "' and Use = 'NotUsed' LIMIT  0, " + count + "";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }


        public DataTable SelectPinsUsedPins(string BaordUrl, int count)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_PinRecords Where BaordUrl = '" + BaordUrl + "' LIMIT  0, " + count + "";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectKeywordPins(string Keyword, int count)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_PinRecords Where Keyword = '" + Keyword + "' and Use = 'NotUsed' LIMIT " + count + "";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectKeywordPinsAlreadyUsed(string Keyword, int count)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_PinRecords Where Keyword = '" + Keyword + "' LIMIT 0 , " + count + "";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_PinRecords");

                DataTable dt = ds.Tables["tb_PinRecords"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectMessageToday(string Username)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Message Where User = '" + Username + "'";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Message");

                DataTable dt = ds.Tables["tb_Message"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }


        public DataTable SelectMessageUsername(string Username , string pinUsername)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Message Where User = '" + pinUsername.Replace("/","") + "' ";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Message");

                DataTable dt = ds.Tables["tb_Message"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectUnfollowsToday(string username)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Follow Where Username = '" + username + "'";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Follow");
                int i = ds.Tables[0].Rows.Count;
                DataTable dt = ds.Tables["tb_Follow"];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public DataTable SelectAllFromTBScheduler()
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Scheduler_Module";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Scheduler_Module");

                DataTable dt = ds.Tables["tb_Scheduler_Module"];
                return dt;
            }
            catch { return new DataTable(); }
        }

        public List<string> SelectUnaccomplishedPastScheduledTimeFromTBScheduler()
        {
            List<string> listModules = new List<string>();

            try
            {
                string strQuery = "SELECT * FROM tb_Scheduler_Module where IsAccomplished='" + "0" + "'";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Scheduler_Module");

                DataTable dt = ds.Tables["tb_Scheduler_Module"];

                foreach (DataRow dRow in dt.Rows)
                {
                    string scheduledTime = dRow["ScheduledDateTime"].ToString();

                    DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);

                    if (dt_scheduledTime.Day == DateTime.Now.Day)
                    {
                        if (DateTime.Now >= dt_scheduledTime)
                        {
                            listModules.Add(dRow["Module"].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return listModules;
        }

        public void UpdateTBScheduler(string module)
        {
            try
            {
                string strTable = "tb_Setting";
                string strQuery = "UPDATE tb_Scheduler_Module SET IsAccomplished='" + "1" + "' WHERE Module='" + module + "'";

                DataBaseHandler.UpdateQuery(strQuery, strTable);

                //Increase 1 day if IsScheduledDaily
                {
                    string selectQuery = "SELECT * FROM tb_Scheduler_Module where Module='" + module + "' and IsAccomplished='1'";
                    DataSet ds = DataBaseHandler.SelectQuery(selectQuery, strTable);

                    DataTable dt = ds.Tables["tb_Setting"];

                    foreach (DataRow dRow in dt.Rows)
                    {
                        string strIsScheduledDaily = dRow["IsScheduledDaily"].ToString();
                        if (strIsScheduledDaily == "1")
                        {
                            string scheduledTime = dRow["ScheduledDateTime"].ToString();

                            DateTime dt_scheduledTime = DateTime.Parse(scheduledTime);

                            DateTime dt_nextscheduledTime = dt_scheduledTime.AddDays(1);

                            string nextscheduledTime = dt_nextscheduledTime.ToString();

                            string nextUpdateQuery = "UPDATE tb_Scheduler_Module SET ScheduledDateTime='" + nextscheduledTime + "', IsAccomplished='0" + "' WHERE Module='" + module + "'";

                            DataBaseHandler.UpdateQuery(nextUpdateQuery, "tb_Setting");
                        }

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void InsertUpdateTBScheduler(string username, string module, string strDateTime, string IsScheduledDaily)
        {
            try
            {
                string InsertQuery = "Insert into tb_Scheduler_Module (Module, ScheduledDateTime, IsAccomplished ,IsScheduledDaily ) VALUES ('" + module + "','" + strDateTime + "', '0' , '" + IsScheduledDaily + "')";
                DataBaseHandler.InsertQuery(InsertQuery, "tb_Scheduler_Module");
            }
            catch (Exception)
            {
                string UpdateQuery = "UPDATE tb_Scheduler_Module SET ScheduledDateTime='" + strDateTime + "', IsScheduledDaily='" + IsScheduledDaily + "', IsAccomplished='" + "0" + "' WHERE Module='" + module + "'";
                DataBaseHandler.UpdateQuery(UpdateQuery, "tb_Scheduler_Module");
            }
        }

        public void InsertUpdateTBSchedulerSetting(string ModuleName, string filePath, string count, string DescriptionFile , string SchedulerStartTime, string SchedulerEndTime, int mindelay , int maxdelay)
        {
            try
            {
                string InsertQuery = "Insert into tb_Schedule_Setting (ModuleType, FileName, Count , DescriptionFile , SchedulerStartTime ,SchedulerEndTime , MinDelay , MaxDelay ) VALUES ('" + ModuleName + "','" + filePath + "', '" + count + "' , '" + DescriptionFile + "' ,'" + SchedulerStartTime + "' , '" + SchedulerEndTime + "' , '" + mindelay + "' , '" + maxdelay + "')";
                DataBaseHandler.InsertQuery(InsertQuery, "tb_Schedule_Setting");
            }
            catch (Exception)
            {
                string UpdateQuery = "UPDATE tb_Schedule_Setting SET FileName='" + filePath + "' , Count = '" + count + "' ,  DescriptionFile = '" + DescriptionFile +  "' ,SchedulerStartTime = '" + SchedulerStartTime + "' , SchedulerEndTime = '" + SchedulerEndTime + "' , MinDelay = '"+ mindelay +"' , MaxDelay = '" + maxdelay + "' WHERE ModuleType ='" + ModuleName + "'";
                DataBaseHandler.UpdateQuery(UpdateQuery, "tb_Schedule_Setting");
            }
        }

        public void DeleteAccomplishedFromTBScheduler()
        {
            try
            {
                string strQuery = "Delete FROM tb_Scheduler_Module where IsAccomplished='" + "1" + "'";
                DataBaseHandler.DeleteQuery(strQuery, "tb_Scheduler_Module");
            }
            catch (Exception)
            {

            }
        }

        public DataTable SelectPinsData(string ModuleType)
        {
            try
            {
                string strQuery = "SELECT * FROM tb_Schedule_Setting where ModuleType ='" + ModuleType + "'";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Schedule_Setting");

                DataTable dt = ds.Tables["tb_Schedule_Setting"];

                return dt;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }
        }



    }
}
