using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Windows;

namespace TunnelFireControl.Dao
{
    public class SqlHelper
    {
        private string connectString;
        private static SqlHelper _instance;
        static SqlHelper()
        {
            _instance = new SqlHelper();
        }
        public static SqlHelper Instance
        {
            get { return _instance; }
        }
        public SqlHelper()
        {
            string path= System.Environment.CurrentDirectory + "\\TunnelFire.db";
            connectString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", path);
        }

        public DataTable FindBySql(string sql)
        {
            DataTable dt = new DataTable();
            using(SQLiteConnection con =new SQLiteConnection(connectString))
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(sql, con);
                da.Fill(dt);
            }
            return dt;
        }

        public bool ExecuteBySql(string sql)
        {
            bool b = false;
            try
            {              
                using (SQLiteConnection con = new SQLiteConnection(connectString))
                {
                    con.Open();
                    SQLiteCommand com = new SQLiteCommand(sql, con);
                    int len = com.ExecuteNonQuery();                    
                    if (len > 0)
                    {
                        b = true;
                    }
                    con.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                b = false;
               
            }
            return b;
        }
    }
}
