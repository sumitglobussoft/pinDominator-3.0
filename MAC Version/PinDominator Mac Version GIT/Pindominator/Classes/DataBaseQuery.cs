using System;
using System.Data;
using System.Collections.Generic;
using PinDominator;

namespace PinDominator
{
	public class DataBaseQuery
	{
		public DataBaseQuery ()
		{
		}

		public DataTable SelectAccoutsForGridView(string module)
		{
			try
			{
				//List<string> lstAccount = new List<string>();
				string strQuery = "SELECT * FROM Account ";

				DataTable  dt = DataBaseHandler.SelectQuery(strQuery, "Account");

				//DataTable dt = ds.Tables["AccountManager"];

				return dt;
			}
			catch (Exception)
			{
				return new DataTable();
			}
		}


		public DataTable SelectAccoutsForGridView(string module,string accountusername)
		{
			try
			{
				//List<string> lstAccount = new List<string>();
				string strQuery = "SELECT * FROM Account where  UserName='"+accountusername+"'";

				DataTable  dt = DataBaseHandler.SelectQuery(strQuery, "Account");

				//DataTable dt = ds.Tables["AccountManager"];

				return dt;
			}
			catch (Exception)
			{
				return new DataTable();
			}
		}






		public static  DataTable selectFollowersDetails(string userName, string follwerName)
		{
			try
			{
				//List<string> lstAccount = new List<string>();
				string strQuery = "SELECT * FROM FollowerDetails where UserName='"+userName+"' and FollowerName='"+follwerName+"'";

				DataTable dt = DataBaseHandler.SelectQuery(strQuery, "FollowerDetails");

				//DataTable dt = ds.Tables["FollowerDetails"];

				return dt;
			}
			catch (Exception)
			{
				return new DataTable();
			}
		}

		public   DataTable selectFollowersDetailswithUser(string userName)
		{
			try
			{
				//List<string> lstAccount = new List<string>();
				string strQuery = "SELECT * FROM FollowerDetails where UserName='"+userName+"'";

				DataTable dt = DataBaseHandler.SelectQuery(strQuery, "FollowerDetails");

				//DataTable dt = ds.Tables["FollowerDetails"];

				return dt;
			}
			catch (Exception)
			{
				return new DataTable();
			}
		}

		public static void DeleteAccountFromDatabase(string module)
		{

			try
			{
				string strQuery = "Delete FROM Account";
				DataBaseHandler.SelectQuery(strQuery, "Account");
			}
			catch(Exception ex) 
			{
				Console.Write (ex.Message);
			}
		}
	}

}

