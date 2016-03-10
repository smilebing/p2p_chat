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
        public Form_login()
        {
            InitializeComponent();
        }


         Socket login_socket  = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //  创建Socket 
        
        int type = 0;
        //发送注册信息的socket
        private AsySocket socket = null;
        //定义server ip 和 端口 
        private IPEndPoint server_ip_port;

        //窗体加载
        private void Form_login_Load(object sender, EventArgs e)
        {
            //服务器的地址
            //server_ip_port = new IPEndPoint(IPAddress.Parse("10.11.125.60"), 6789);

            server_ip_port = new IPEndPoint(IPAddress.Parse("10.211.55.7"), 6789);
            socket = new AsySocket("any", 1234);

            //发送消息事件
            socket.OnSended += new AsySocketEventHandler(socket_OnSended);

            //接收到socket 信息
            socket.OnStreamDataAccept += new StreamDataAcceptHandler(socket_OnStreamDataAccept);

            socket.OnClosed += new AsySocketClosedEventHandler(socket_OnClosed);

            //连接server
            //socket.LinkObject.Connect(server_ip_port);
            //socket.BeginAcceptData();

            server_ip_port = new IPEndPoint(IPAddress.Parse("10.211.55.7"), 6789);

            try
            {
                //尝试连接 server
                login_socket.Connect(server_ip_port);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("无法连接服务器！");
                return;
            }
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
           //
           
           byte[] recvBytes = new byte[1024];
 
        
            login_socket.Send(login_msg.GetBytes(), login_msg.GetBytes().Length, 0);


            login_socket.Receive(recvBytes, recvBytes.Length, 0);
            MyTreaty receieve_msg = MyTreaty.GetMyTreaty(recvBytes);
            if(receieve_msg.Name=="y")
            {
                MessageBox.Show("登录成功");
                Form_online_user form_online_user = new Form_online_user();
                //传递用户名
                form_online_user.user_name = textBox_name.Text.Trim();
                form_online_user.Show();

                login_socket.Shutdown(SocketShutdown.Both);
                login_socket.Close();
                this.Hide();
            }
            else
            {
                MessageBox.Show("登录失败");
            }

            //login_socket.Shutdown(SocketShutdown.Both);
            //login_socket.Close();
        }


        /// <summary>
        /// 接收socket 信息
        /// </summary>
        /// <param name="AccepterID"></param>
        /// <param name="AcceptData"></param>
        void socket_OnStreamDataAccept(string AccepterID, MyTreaty AcceptData)
        {
            string result = AcceptData.Name;
            Console.WriteLine("client 收到信息 ");

            if (AcceptData.Type == 1)//登录结果
            {
                if (result == "y")
                {
                    MessageBox.Show("登录成功");
                    //建立在线用户窗体
                }
                else
                {
                    MessageBox.Show("登录失败");
                }
            }
            else if (AcceptData.Type == 0)
            {
                if (result == "y")
                {
                    MessageBox.Show("注册成功");
                }
                else
                {
                    MessageBox.Show("注册失败");
                }
                //string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "的图片";
                //AddMsg(msg);
                //picBox.Image = Image.FromStream(new MemoryStream(AcceptData.Content));
            }
            else
            {
                string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "名叫：" + AcceptData.FileName + "的文件";
                if (MessageBox.Show(msg + "，是否接收", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    //try
                    //{

                    //    sFD.Filter = AcceptData.FileName + " | *." + Path.GetExtension(AcceptData.FileName);

                    //    if (sFD.ShowDialog() == DialogResult.OK)
                    //    {
                    //        FileStream fs = new FileStream(sFD.FileName, FileMode.Create, FileAccess.Write);
                    //        fs.Write(AcceptData.Content, 0, Convert.ToInt32(AcceptData.Content.Length));
                    //        fs.Close();
                    //        AddMsg(msg);
                    //    }

                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}
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
