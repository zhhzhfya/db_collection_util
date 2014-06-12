using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace bbs_reader
{
    class DbUtil
    {
        public static string connection_str_oracle = "Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User ID={3};Password={4}";
        public static OracleConnection conn = null;
        public static bool testConnection(string host, string user, string password)
        {
            // 192.168.6.10:1521/orcl
            if (host.IndexOf(":") < 0 || host.IndexOf("/") < 0)
            {
                return false;
            }
            string[] hosts = host.Split(':');
            string[] po = hosts[1].Split('/');
            string connStr = String.Format(connection_str_oracle, hosts[0], po[0], po[1], user, password);
            conn = new OracleConnection(connStr);//创建一个新连接
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                conn.Close(); //关闭连接
            }
            return true;
        }
        public static OracleConnection getConnection() 
        {
            if (conn != null)
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                return conn;
            }
            return null;
        }

        public static DataTable query(string sql)
        {
            DataTable dt = new DataTable();
            OracleConnection conn = getConnection();
            try
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                OracleDataReader odr = cmd.ExecuteReader();//创建一个OracleDateReader对象
                while (odr.Read())//读取数据，如果odr.Read()返回为false的话，就说明到记录集的尾部了
                {
                    if (dt.Columns.Count == 0)
                    {
                        for (int i = 0; i < odr.FieldCount; i++)
                        {
                            dt.Columns.Add(odr.GetName(i));
                        }
                    }
                    Object[] row = new Object[odr.FieldCount];
                    for (int i = 0; i < odr.FieldCount; i++)
                    {
                        if (odr.GetFieldType(i).Equals(typeof(String)))
                        {
                            row[i] = odr.GetOracleString(i).IsNull ? "" : odr.GetOracleString(i).ToString();
                        }
                        if (odr.GetFieldType(i).Equals(typeof(Decimal)))
                        {
                            row[i] = odr.GetOracleNumber(i);
                        }
                    }
                    dt.Rows.Add(row);
                }
                odr.Close();
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                conn.Close(); //关闭连接
            }
            return dt;
        }

        public static int update(string sql)
        {
            OracleConnection conn = getConnection();
            try
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                return  cmd.ExecuteNonQuery();//创建一个OracleDateReader对象
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                conn.Close(); //关闭连接
            }
        }
    }
}
