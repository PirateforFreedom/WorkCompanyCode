using System.Data;
using System.Data.SqlClient;
using System.Text;
using MySql.Data.MySqlClient;
namespace WebApplicationLocalAPI
{
    public class DataBaseOperation
    {
        public string StrErpSqlServeConnection = @"server=61.152.218.58,14333;database=x3v71sql;uid=sa;pwd=X3Force123";
        public String MySqlconnetStr = "server=rm-bp15bmekn4rg8k1a6eo.mysql.rds.aliyuncs.com,3306;user=jql;password=opq36912Q;database=jql;";
        public SqlConnection? ConnErpSqlSer;
        MySqlConnection? con;
        public DataBaseOperation()
       {
            con = new MySqlConnection(MySqlconnetStr);
            con.Open();
        }

    public DataTable SqlServeSqlQuery(string StrErpSql)
        {
            DataSet MyDataSet = new DataSet();
            if (StrErpSql == null|| StrErpSql =="")
            {
                return MyDataSet.Tables[0];
            }
            ConnErpSqlSer = new SqlConnection(StrErpSqlServeConnection);
            ConnErpSqlSer.Open();
            SqlCommand comd = new SqlCommand(StrErpSql, ConnErpSqlSer);
            SqlDataAdapter selectadapter = new SqlDataAdapter();
            selectadapter.SelectCommand = comd;
            selectadapter.SelectCommand.ExecuteNonQuery();
            selectadapter.Fill(MyDataSet);
            ConnErpSqlSer.Close();
            return MyDataSet.Tables[0];
            
        }

        public DataTable MySqlQuery(string StrSql)
        {
            DataTable Mytable = new DataTable();
            if (StrSql == null || StrSql == "")
            {
                return Mytable;
            }
           
            MySqlDataAdapter adpter = new MySqlDataAdapter(StrSql, con);
            adpter.Fill(Mytable);
            //con.Close();
            return Mytable;
        }

        public string MySQLExecuteNonQuery(string StrSql)
        {
         
            
           
            if (StrSql == null || StrSql == "")
            {
                return "NO";
            }
            //MySqlConnection con = new MySqlConnection(MySqlconnetStr);
            //con.Open();
            MySqlDataAdapter adpter = new MySqlDataAdapter(StrSql, con);
            MySqlCommand cmd = new MySqlCommand(StrSql, con);
            cmd.ExecuteNonQuery();
            //con.Close();
            return "OK";
        }
        public void  ClostMysqlCON()
        {
            con.Close();
        }
            public string TableTOJson(DataTable dt)
        {
            if (dt == null)
            {
                return "NO,TABLE TO JSON";
            }
            StringBuilder jsonBuiler = new StringBuilder();
            jsonBuiler.Append("{");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                jsonBuiler.Append("\"");
                jsonBuiler.Append(dt.Columns[j].ColumnName);
                jsonBuiler.Append("\":\"");
                jsonBuiler.Append(dt.Rows[0][j].ToString().Replace("\"", "\\\""));
                jsonBuiler.Append("\",");


            }

            jsonBuiler.Remove(jsonBuiler.Length - 1, 1);
            jsonBuiler.Append("}");


            return jsonBuiler.ToString();
        }

        }
}
