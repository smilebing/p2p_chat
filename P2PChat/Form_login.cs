using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class Form_login : Form
    {
        //单例模式
        public static Form_login instance;
        static string  serverIp = "127.0.0.1";

        UdpClient udpclient = SingletonClient.getUdpClient();
        IPEndPoint udpServerIp = new IPEndPoint(IPAddress.Parse(serverIp), 1234);

       

        #region call back
        IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 0);



        private void ReceiveMessage()
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpclient.Receive(ref remoteIpep);
                    MyTreaty msg = MyTreaty.GetMyTreaty(bytRecv);
                    readMsg(remoteIpep, msg);


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        public static void SendCallback(IAsyncResult ar)
        {

        }


        #endregion


        #region msg判断函数
        public   void readMsg(IPEndPoint remoteIpep, MyTreaty mytreaty)
        {
            //文本 type=6
            //图片 type=7
            //注册 type=2 
            //登录 type=1
            string name = mytreaty.Name;
            string pwd = mytreaty.Pwd;
            string result = Encoding.GetEncoding("utf-8").GetString(mytreaty.Content);
            Form_online_user form_online_user = Form_online_user.getInstance();

         
            if (mytreaty.Type == 1) //login
            {
                //验证密码
              
                if (result == "success")
                {
                    //验证成功
                    MessageBox.Show("login success");
                    Application.Run(form_online_user);
                    this.Close();
                    this.closeForm();
                    //Form_online_user form_online_user = Form_online_user.getInstance();
                    //form_online_user.user_name = mytreaty.Name;
                    //form_online_user.Show();
                    //this.Close();
                    //this.Dispose();
                    
                }
                else
                {
                    Console.WriteLine(result);
                    MessageBox.Show("login fail");
                }
            }

            if (mytreaty.Type == 2) //register
            {
                //注册
                Console.WriteLine("收到注册信息：" + name + "   " + pwd);
                //是否已经注册
                if (result == "success")
                {              
                        //注册成功
                    MessageBox.Show("register success");
                }
                else
                {
                    //已经注册，返回失败信息
                    MessageBox.Show("exists ");
                 
                }

            }
        }

        public  void closeForm()
        {
            this.Hide();
            //this.Dispose();
        }

        #endregion

        //====================================================

        private Form_login()
        {
            InitializeComponent();
        }

        public static Form_login getInstance()
        {
            if(instance==null)
            {
                instance = new Form_login();
            }
            return instance;
        }


        
   

        //窗体加载
        private void Form_login_Load(object sender, EventArgs e)
        {
          
        }


        //注册button
        private void button_register_Click(object sender, EventArgs e)
        {
            Form_register form_register = new Form_register();
            form_register.Show();
        }

        

        //登录 button
        private void button_login_Click(object sender, EventArgs e)
        {

            if(textBox_name.Text.Trim()=="")
            {
                MessageBox.Show("请输入name");
                return;
            }
            if(textBox_pwd.Text.Trim()=="")
            {
                MessageBox.Show("请输入密码");
                return;
            }

           MyTreaty login_msg = new MyTreaty(1, textBox_name.Text.Trim(), textBox_pwd.Text.Trim(), UTF8Encoding.UTF8.GetBytes("login"), DateTime.Now, "");
           

           udpclient.SendAsync(login_msg.GetBytes(), login_msg.GetBytes().Count(), udpServerIp);
  
        }




        /// <summary>
        /// 绑定消息
        /// </summary>
        /// <param name="msg"></param>
        private void AddMsg(string msg)
        {

            //ListViewItem lv = new ListViewItem(msg);
            //lv.SubItems.Add(msg);
            //lstMsg.Items.Add(lv);



        }

        private void Form_login_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }


    }
}
