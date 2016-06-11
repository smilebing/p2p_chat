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
        static string  serverIp = "127.0.0.1";
        UdpClient udpclient = new UdpClient(2345);
        IPEndPoint udpServerIp = new IPEndPoint(IPAddress.Parse(serverIp), 1234);
        //存储udp state
        public class UdpState
        {
            public UdpClient udpClient;
            public IPEndPoint ipEndPoint;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
            public int counter = 0;
        }


        #region call back
        //UdpClient udpserver = new UdpClient(1234);
        IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 0);
        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpState udpReceiveState = ar.AsyncState as UdpState;
            if (ar.IsCompleted)
            {
                Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(ar, ref udpReceiveState.ipEndPoint);
                MyTreaty my = MyTreaty.GetMyTreaty(receiveBytes);
                //接收完数据，进行数据判断回复
                //readMsg(udpReceiveState, my);
            }
        }

        public void ReceiveMsg()
        {
            Console.WriteLine("listening for messages");
            while (true)
            {
                lock (this)
                {
                    IAsyncResult ar = udpclient.BeginReceive(new AsyncCallback(ReceiveCallback), udpclient);
                }
            }
        }

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
        public void readMsg(IPEndPoint remoteIpep, MyTreaty mytreaty)
        {
            //文本 type=6
            //图片 type=7
            //注册 type=2 
            //登录 type=1
            string name = mytreaty.Name;
            string pwd = mytreaty.Pwd;
            string result = Encoding.GetEncoding("utf-8").GetString(mytreaty.Content);

            if (mytreaty.Type == 8) //心跳包
            {
                //更新心跳包
            }
            if (mytreaty.Type == 1) //login
            {
                //验证密码
              
                if (result == "success")
                {
                    //验证成功
                    MessageBox.Show("login success");
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

        #endregion

        //====================================================






        public Form_login()
        {
            InitializeComponent();
        }


        
   

        //窗体加载
        private void Form_login_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(ReceiveMessage);
            t.Start();
        }


        //注册button
        private void button_register_Click(object sender, EventArgs e)
        {
                 
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
           //

           udpclient.SendAsync(login_msg.GetBytes(), login_msg.GetBytes().Count(), udpServerIp);
  
        }


        /// <summary>
        /// 接收socket 信息
        /// </summary>
        /// <param name="AccepterID"></param>
        /// <param name="AcceptData"></param>
        void socket_OnStreamDataAccept(AsySocket accept_socket, MyTreaty AcceptData)
        {
            string result = AcceptData.Name;
            Console.WriteLine("client 收到信息 ");

            if (AcceptData.Type == 1)//登录结果
            {
                Console.WriteLine(result);
                if (result == "y")
                {
                    MessageBox.Show("登录成功");
                    //建立在线用户窗体
                    //form_online_user.Show();
                    //this.Hide();

                }
                else
                {
                    MessageBox.Show("登录失败");
                }
            }
        }

        void socket_OnClosed(string SocketID, string ErrorMessage)
        {
            //服务器关闭
            MessageBox.Show(this, "服务器关闭");
            this.Close();
        }
        ///// <summary>
        ///// 接收数据触发
        ///// </summary>
        ///// <param name="AccepterID"></param>
        ///// <param name="AcceptData"></param>
        //void socket_OnStringDataAccept(string AccepterID, string AcceptData)
        //{
        //    AddMsg(AcceptData);
        //}
   
        // delegate void CallBackRef(string msg);
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
