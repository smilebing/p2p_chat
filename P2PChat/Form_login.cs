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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class Form_login : Form
    {
        string serverIp = "10.211.55.7";

        //用来统一资源的socket
        private AsySocket signalSocket;
        public AsySocket SignalSocket
        {
            get { return signalSocket;}
            set {signalSocket=value;}
        }

        private Form_online_user form_online_user;
        public Form_online_user form_Online_user
        {
            get { return form_online_user; }
            set { form_online_user = value; }
        }

        private Form_register register;
        public Form_register form_register
        {
            get { return register; }
            set { register = value; }
        }



        public Form_login()
        {
            InitializeComponent();
        }


        
        int type = 0;
         //定义server ip 和 端口 
        private IPEndPoint server_ip_port;

        //窗体加载
        private void Form_login_Load(object sender, EventArgs e)
        {
            //服务器的地址
            server_ip_port = new IPEndPoint(IPAddress.Parse(serverIp), 6789);

            //发送消息事件
            signalSocket.OnSended += new AsySocketEventHandler(socket_OnSended);

            //接收到socket 信息
            signalSocket.OnStreamDataAccept += new StreamDataAcceptHandler(socket_OnStreamDataAccept);

            signalSocket.OnClosed += new AsySocketClosedEventHandler(socket_OnClosed);

        }


        //注册button
        private void button_register_Click(object sender, EventArgs e)
        {
            register.SignalSocket = signalSocket;
            register.Show();           
        }

        

        //登录 button
        private void button_login_Click(object sender, EventArgs e)
        {
            
            try
            {
                //signalSocket.connect(server_ip_port);
                //尝试连接 server
                signalSocket.Listen(5);
                signalSocket.connect(server_ip_port);
                //signalSocket.BeginAcceptData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("无法连接服务器！");
                return;
            }

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
           
           byte[] recvBytes = new byte[1024];

           Console.WriteLine("prepare to use Asnedto---");
            //发送登录信息
          // signalSocket.ASend(login_msg.GetBytes());
            signalSocket.ASendTo(login_msg.GetBytes(), server_ip_port);

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
        /// <summary>
        /// 发送消息触发
        /// </summary>
        /// <param name="SenderID"></param>
        /// <param name="EventMessage"></param>
        void socket_OnSended(string SenderID, string EventMessage)
        {
            if (type == 0)
            {
                //AddMsg("我: " + txtSend.Text);
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if (type == 1)
            {
                //AddMsg("图片发送成功");
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if (type == 2)
            {
                //AddMsg("文件发送成功");
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if (type == 6)
            {
                Console.WriteLine("client 发送了注册信息");
            }
        }
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
