using System;
using System.Data.SQLite;
using System.Data;
using PinDominator;

namespace PinDominator
{
	public class DataBaseHandler
	{
		public DataBaseHandler ()
		{
		}

		public static string CONstr = string.Empty;

		public static void checkOperatingSystem ()
		{
			if (PDGlobals.typeOfOperatingSystem.Contains ("Win")) {
				CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\DB_PinDominator.db" + ";Version=3;";

			} else {
				if (PDGlobals.typeOfOperatingSystem.Contains ("Unix")) {
					CONstr = "Data Source=" + Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "/PinDominator/DB_PinDominator.db" + ";Version=3;";
					//CONstr="URI=file:SocialAutoBot.db";

				} else {
					CONstr = "Data Source=" + Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator\\DB_PinDominator.db" + ";Version=3;";
				}
			}
		}

		public static DataTable SelectQuery (string query, string tablename)
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

				SQLiteDataAdapter adapter=new SQLiteDataAdapter(query, CONstr);
				//SQliteDataAdapter adapter = new SQliteDataAdapter(query, CONstr);
				//SqliteDataAdapter adapter = new SqliteDataAdapter(query, CONstr);
				DataSet dataset = new DataSet();
				adapter.Fill(dataset);
				dt = dataset.Tables[0];
				return dt;

			}
			catch(Exception Ex)
			{
				return new DataTable();
			}
		}






		public static DataSet SelectQueryNew (string query, string tablename)
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
				//DataTable dt = new DataTable();
				SQLiteDataAdapter adapter=new SQLiteDataAdapter(query, CONstr);
				//SqliteDataAdapter adapter = new SqliteDataAdapter(query, CONstr);
				DataSet dataset = new DataSet();
				adapter.Fill(dataset);
				//dt = dataset[tablename];
				return dataset;

			}
			catch
			{
				return new DataSet();
			}
		}


		public static void InsertQuery(string query, string tablename)
		{
			checkOperatingSystem ();
			try
			{
				/*using (SQLiteConnection CON = new SQLiteConnection(CONstr))
				{
					SQLiteCommand CMD = new SQLiteCommand(query, CON);
					SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
					DataSet DS = new DataSet();
					AD.Fill(DS, tablename);
				}*/

				DataTable dt = new DataTable();
				SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, CONstr);
				//SqliteDataAdapter adapter = new SqliteDataAdapter(query, CONstr);
				DataSet dataset = new DataSet();
				adapter.Fill(dataset);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);

			}
		}

		public static void InsertQueryLogin(string query, string tablename)
		{
			checkOperatingSystem ();
			/*using (SQLiteConnection CON = new SQLiteConnection(CONstr))
			{
				SQLiteCommand CMD = new SQLiteCommand(query, CON);
				SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
				DataSet DS = new DataSet();
				AD.Fill(DS, tablename);
			}*/
			DataTable dt = new DataTable();
			SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, CONstr);
			//SqliteDataAdapter adapter = new SqliteDataAdapter(query, CONstr);
			DataSet dataset = new DataSet();
			adapter.Fill(dataset);

		}

		public static void DeleteQuery(string query, string tablename)
		{
			checkOperatingSystem ();
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

		public static void UpdateQuery(string query, string tablename)
		{
			checkOperatingSystem ();
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

		/// <summary>
		/// This method is use for find data form sqlite table 
		/// </summary>
		/// <param name="query">Sqlite query</param>
		/// <param name="tablename">Name of Table</param>


		public static void PerformQuery(string query, string tablename)
		{
			checkOperatingSystem ();
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

