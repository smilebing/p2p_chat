﻿using System;
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
            OleDbCommand search_cmd = conn.CreateCommand();
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
        /// 查找用户是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool search(string name)
        {
            OleDbCommand search_cmd = conn.CreateCommand();

            string sqlString = "select * from [user] where uname=@name ";
            search_cmd.CommandText = sqlString;
            search_cmd.Parameters.Add(new OleDbParameter("name", name));

            search_cmd.ExecuteScalar();
            int i = Convert.ToInt32(search_cmd.ExecuteScalar());
            if (i > 0)
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
            if(search(name)==true)
            {
                return false;
            }

            OleDbCommand insert_cmd = conn.CreateCommand();


            string sqlString = "insert into [user] (uname,pwd) values(@name,@pwd)";
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
          if(  search(name)==false)
          {
              return false;
          }

          OleDbCommand update_cmd = conn.CreateCommand();


            string sqlString = "update [user] set pwd=@pwd where uname=@name";

            update_cmd.CommandText = sqlString;
            update_cmd.Parameters.Add(new OleDbParameter("pwd", pwd));
            update_cmd.Parameters.Add(new OleDbParameter("name", name));
           

            int i = update_cmd.ExecuteNonQuery();
            if(i>0)
            {
                return true;
            }

            return false;
        }

    }
}
