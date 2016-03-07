using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Model
{
    public class Access
    {
        //创建连接对象
        public OleDbConnection conn { get; set; }
        
        public OleDbCommand search_cmd;
        public OleDbCommand update_cmd;
        public OleDbCommand insert_cmd;


       /// <summary>
       /// 构造函数 
       /// </summary>
       /// <param name="olconn"></param>
        public Access(OleDbConnection olconn)
        {
            this.conn = olconn;
        }
        


        /// <summary>
        /// 开启数据库连接
        /// </summary>
        public void openConn()
        {
            conn.Open();
            search_cmd = conn.CreateCommand();
            update_cmd = conn.CreateCommand();
            insert_cmd = conn.CreateCommand();
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void closeConn()
        {
            conn.Close();
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="name"></param>
       /// <param name="pwd"></param>
       /// <returns>返回用户是否存在</returns>
        public bool search (string name,string pwd)
        {
            string sqlString = "select * from [user] where uname=@name and pwd=@pwd";
            search_cmd.CommandText = sqlString;
            search_cmd.Parameters.Add(new OleDbParameter("name", name));
            search_cmd.Parameters.Add(new OleDbParameter("pwd", pwd));


            search_cmd.ExecuteScalar(); 
            int i = Convert.ToInt32(search_cmd.ExecuteScalar());
            if(i>0)
            {
                //查询存在
                return true;
            }
            return false;
        }


        /// <summary>
        /// 插入新用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns>增加用户是否成功</returns>
        public bool insert(string name,string pwd)
        {
            if(search(name,pwd)==true)
            {
                return false;
            }


            string sqlString = "insert into [user] valuse(@name,@pwd)";
            insert_cmd.CommandText = sqlString;

            insert_cmd.Parameters.Add(new OleDbParameter("name", name));
            insert_cmd.Parameters.Add(new OleDbParameter("pwd", pwd));

            int i = insert_cmd.ExecuteNonQuery();
            if(i>0)
            {
                //插入成功
                return true;
            }
            return false;
        }



        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns>更新是否成功</returns>
        public bool update(string name,string pwd)
        {
          if(  search(name, pwd)==false)
          {
              return false;
          }

            string sqlString = "update [user] set pwd=@pwd where name=@name";

            update_cmd.CommandText = sqlString;

            update_cmd.Parameters.Add(new OleDbParameter("name", name));
            update_cmd.Parameters.Add(new OleDbParameter("pwd", pwd));

            int i = update_cmd.ExecuteNonQuery();
            if(i>0)
            {
                return true;
            }

            return false;
        }

    }
}
