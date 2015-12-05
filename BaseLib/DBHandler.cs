using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace BaseLib
{
    class DBHandler
    {
        public static string CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinDominator3.0\\DB_PinDominator.db" + ";Version=3;";
      
        public static DataSet SelectQuery(string query, string tablename)
        {
            DataSet DS = new DataSet();
            try
            {
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    AD.Fill(DS, tablename);
                }
                return DS;
            }
            catch { return DS; }
        }

        public static void InsertQuery(string query, string tablename)
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
            catch { }
        }

        public static void DeleteQuery(string query, string tablename)
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
            catch { }
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
            catch { }

        }

        public static void PerformQuery(string query, string tablename)
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
            catch { }
        }


    }
}
