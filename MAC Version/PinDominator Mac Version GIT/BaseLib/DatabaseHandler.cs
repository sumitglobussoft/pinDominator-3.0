using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using PinDominator;
using System.Data.SQLite;




namespace BaseLib
{
    public class DataBaseHandler
    {
    
      //  public static string CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\DB_PinDominator.db;";  
		public static string CONstr = string.Empty;


		public static void checkOperatingSystem ()
		{
			if (PDGlobals.typeOfOperatingSystem.Contains ("Win")) {
				CONstr = "Data Source=" + Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\DB_PinDominator.db" + ";Version=3;";
			} else {
				if (PDGlobals.typeOfOperatingSystem.Contains ("Unix")) {
					CONstr = "Data Source=" + Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "/PinDominator/DB_PinDominator.db" + ";Version=3;";
					//CONstr="URI=file:SocialAutoBot.db";

				} else {
					CONstr = "Data Source=" + Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\DB_PinDominator.db" + ";Version=3;";
				}
			}
		}
		public static DataSet SelectQuery(string query, string tablename)
        {
            //try
            //{
			checkOperatingSystem ();
                DataSet DS = new DataSet();
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    AD.Fill(DS, tablename);

                }
                return DS;
            //}
            //catch
            //{
            //    return new DataSet();
            //}
        }
		public static DataTable SelectQueryNew (string query, string tablename)
		{
			//http://lists.ximian.com/pipermail/mono-list/2005-June/027584.html
			checkOperatingSystem ();
			/*IDbConnection dbcon;
			dbcon = (IDbConnection) new SqliteConnection(CONstr);
            dbcon.Open();
            IDbCommand dbcmd = dbcon.CreateCommand();
            dbcmd.CommandText = query;
            IDataReader reader = dbcmd.ExecuteReader();
			DataTable dt = new DataTable();
			dt.Load(reader);
			dt.AcceptChanges();*/
			try
			{
				DataTable dt = new DataTable();
				using (SQLiteConnection CON = new SQLiteConnection(CONstr))
				{
					SQLiteCommand CMD = new SQLiteCommand(query, CON);
					DataSet dataset = new DataSet();
					SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
					AD.Fill(dataset, tablename);
					dt = dataset.Tables[0];
				}
				return dt;

			}
			catch(Exception Ex)
			{
				return new DataTable();
			}
		}

        public static void InsertQuery(string query, string tablename)
        {
			checkOperatingSystem ();
            try
            {
            using (SQLiteConnection CON = new SQLiteConnection(CONstr))
            {
                CON.Open();
                SQLiteCommand CMD = new SQLiteCommand(query, CON);
                SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                DataSet DS = new DataSet();
                AD.Fill(DS, tablename);
            }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.StackTrace);
            }
        }

        public static void DeleteQuery(string query, string tablename)
        {
            //try
            //{
			checkOperatingSystem ();
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        public static void DeleteQuery2(string query, string tablename)
        {
            //try
            //{
            using (SQLiteConnection CON = new SQLiteConnection(CONstr))
            {
                SQLiteCommand CMD = new SQLiteCommand(query, CON);
                SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                DataSet DS = new DataSet();
                AD.Fill(DS, tablename);
            }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }


        public static void UpdateQuery(string query, string tablename)
        {
            try
            {
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

    }
}
