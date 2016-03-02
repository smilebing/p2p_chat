using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace P2PChat
{

    public partial class Form_register : Form
    {
        static string exePath = @"C:\Users\bing\Documents\P2PChat";//本程序所在路径
        //创建连接对象
        OleDbConnection conn;

        public Form_register()
        {
            InitializeComponent();
        }


        //窗体加载时 连接数据库
        private void Form_register_Load(object sender, EventArgs e)
        {
            conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + exePath + @"\P2P_db.mdb"); 
        }

        //注册button
        private void button_resister_Click(object sender, EventArgs e)
        {
            //判断输入内容合法性
            if (textBox_name.Text == "")
            {
                MessageBox.Show("请输入name!");
                return;
            }
            if(textBox_pwd1.Text==""||textBox_pwd2.Text=="")
            {
                MessageBox.Show("请输入密码!");
                return;
            }
            if(string.Compare(textBox_pwd1.Text,textBox_pwd2.Text)!=0)
            {
                MessageBox.Show("两次密码不一致!");
                return;
            }






            string sql = "insert into [user]([uname],[pwd]) values(@name,@pwd)";
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@name", textBox_name.Text);
            cmd.Parameters.AddWithValue("@pwd",textBox_pwd1.Text);
            cmd.Connection = conn;
            conn.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("注册成功!");

            textBox_name.Text = "";
            textBox_pwd1.Text = "";
            textBox_pwd2.Text = "";
        }

        //关闭窗口后 关闭数据库连接
        private void Form_register_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
        }
    }
}
