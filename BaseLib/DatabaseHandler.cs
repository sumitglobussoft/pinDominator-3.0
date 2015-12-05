using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;


namespace BaseLib
{
    public class DataBaseHandler
    {
        //public static string CONstr = "Data Source=" + Application.StartupPath + "\\DB_PINDominator.db" + ";Version=3;";
        public static string CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PinDominator.db;";
       // public static string CONstr = "C:\\Users\\GLB-112\\AppData\\Local\\PinDominator\\DB_PinDominator.db";
        public static DataSet SelectQuery(string query, string tablename)
        {
            //try
            //{

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

        public static void InsertQuery(string query, string tablename)
        {
            //try
            //{
            using (SQLiteConnection CON = new SQLiteConnection(CONstr))
            {
                CON.Open();
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

        public static void DeleteQuery(string query, string tablename)
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
