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
using Model;
using System.Net;
using System.Net.Sockets;

namespace P2PChat
{

    public partial class Form_register : Form
    {
        string serverIp = "127.0.0.1";
        UdpClient udpclient = SingletonClient.getUdpClient();

        public Form_register()
        {
            InitializeComponent();
        }

     
        //定义server ip 和 端口 
        private IPEndPoint server_ip_port;
        
        //窗体加载时 
        private void Form_register_Load(object sender, EventArgs e)
        {
            server_ip_port = new IPEndPoint(IPAddress.Parse(serverIp), 1234);
        }

        //注册button
        private void button_resister_Click(object sender, EventArgs e)
        {
            //判断输入内容合法性
            if (textBox_name.Text.Trim() == "")
            {
                MessageBox.Show("请输入name!");
                return;
            }
            if(textBox_pwd1.Text.Trim()==""||textBox_pwd2.Text.Trim()=="")
            {
                MessageBox.Show("请输入密码!");
                return;
            }
            if(string.Compare(textBox_pwd1.Text.Trim(),textBox_pwd2.Text.Trim())!=0)
            {
                MessageBox.Show("两次密码不一致!");
                return;
            }


            //发送注册信息
            MyTreaty register_msg=new MyTreaty(2,textBox_name.Text.Trim(),textBox_pwd1.Text.Trim(),UTF8Encoding.UTF8.GetBytes("register"),DateTime.Now,"");
            udpclient.SendAsync(register_msg.GetBytes(), register_msg.GetBytes().Count(), server_ip_port);
            textBox_name.Text = "";
            textBox_pwd1.Text = "";
            textBox_pwd2.Text = "";
        }

        //关闭窗口
        protected void Form_register_FormClosed(object sender, FormClosedEventArgs e)
        {   
            this.Dispose();    
        }


    

   
        /// <summary>
        /// 绑定消息
        /// </summary>
        /// <param name="msg"></param>
        private void AddMsg(string msg)
        {

        }

        private void Form_register_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        public static void readMsg(IPEndPoint ipendpoint,MyTreaty mytreaty)
        {
            //文本 type=6
            //图片 type=7
            //注册 type=2 
            //登录 type=1
            string name = mytreaty.Name;
            string pwd = mytreaty.Pwd;
            string result = Encoding.GetEncoding("utf-8").GetString(mytreaty.Content);

            if(result=="success")
            {
                MessageBox.Show("注册成功，请登录");
               
            }
            else
            {
                MessageBox.Show("注册失败，已存在该用户");
            }
          
        }


    }
}
