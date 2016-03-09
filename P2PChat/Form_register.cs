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

        //用来在窗口间传递 socket 
        public AsySocket socket_parameter = null;

        public AsySocket frm_parameter
        {
            get
            {
                return socket_parameter;
            }
            set
            {
                socket_parameter = value;
            }
        }






        public Form_register()
        {
            InitializeComponent();
        }

        int type = 0;

        //发送注册信息的socket
        private AsySocket socket = null;
        Socket client_socket;
        //定义server ip 和 端口 
        private IPEndPoint server_ip_port;
        
        //窗体加载时 
        private void Form_register_Load(object sender, EventArgs e)
        {
            
            //服务器的地址
            //server_ip_port = new IPEndPoint(IPAddress.Parse("10.11.125.60"), 6789);
            
            server_ip_port=  new IPEndPoint(IPAddress.Parse("10.211.55.7"), 6789);
            //连接
            //socket = new AsySocket("any", 3456);

            //发送消息事件
            //socket.OnSended += new AsySocketEventHandler(socket_OnSended);

            //接收到socket 信息
            //socket.OnStreamDataAccept += new StreamDataAcceptHandler(socket_OnStreamDataAccept);

            //socket.OnClosed += new AsySocketClosedEventHandler(socket_OnClosed);

            //连接server
            //socket.LinkObject.Connect(server_ip_port);
            //socket.BeginAcceptData();




            //创建Socket并连接到服务器
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //  创建Socket 
            try 
            {
                client_socket.Connect(server_ip_port);
            }
            catch (Exception ){
                MessageBox.Show("无法连接服务器！");
                this.Dispose();
                this.Close();
            }
            
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
            MyTreaty register_msg=new MyTreaty(0,textBox_name.Text.Trim(),textBox_pwd1.Text.Trim(),UTF8Encoding.UTF8.GetBytes("register"),DateTime.Now,"");
            client_socket.Send(register_msg.GetBytes(), register_msg.GetBytes().Length, 0);

             byte[] recvBytes = new byte[1024];
             client_socket.Receive(recvBytes, recvBytes.Length, 0);
             MyTreaty receieve_msg = MyTreaty.GetMyTreaty(recvBytes);
             Console.WriteLine(receieve_msg.Name);
            if(receieve_msg.Name=="y")
            {
                MessageBox.Show("注册成功");
                //  释放资源，关闭窗口
                client_socket.Shutdown(SocketShutdown.Both);
                this.Dispose();
                this.Close();
            }
            else
            {
                MessageBox.Show("注册失败");
            }

            
           // socket.ASend(0, textBox_name.Text.Trim(),textBox_pwd1.Text.Trim(), UTF8Encoding.UTF8.GetBytes(textBox_pwd1.Text.Trim()), DateTime.Now, "");
           // type = 0;

            textBox_name.Text = "";
            textBox_pwd1.Text = "";
            textBox_pwd2.Text = "";
        }

        //关闭窗口
        protected void Form_register_FormClosed(object sender, FormClosedEventArgs e)
        {
           
            client_socket.Close();
            client_socket.Dispose();
            this.Dispose();
          
           // Environment.Exit(0);
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
                if(result=="y")
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
            else if(type==6)
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

        private void Form_register_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }




    }
}
