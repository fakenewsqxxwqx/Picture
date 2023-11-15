using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Picture
{
    public class SqlHelper
    {
        private static readonly string connStr = "Database=picturemanagement;Data Source=localhost;User Id=root;Password=123456;pooling=false;CharSet=utf8;port=3306;";
        /// <summary>
        /// 检测数据库连接状态
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnectStatus()
        {
            bool result = false;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 增、删、改公共方法
        /// </summary>
        /// <returns></returns>
        public static int commonExecute(string sql)
        {
            int res = -1;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                //当连接处于打开状态时关闭,然后再打开,避免有时候数据不能及时更新
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(sql, conn)) //创建Command连接对象
                {
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }

        /// <summary>
        /// 查询方法
        /// 注意：尽量不要用select * from table表（返回的数据过长时，DataTable可能会出错），最好指定要查询的字段。
        /// </summary>
        /// <returns></returns>
        public static DataTable query(string sql)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                //当连接处于打开状态时关闭,然后再打开,避免有时候数据不能及时更新
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(sql, conn)) //创建Command连接对象
                {
                    using (MySqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }
    }

}