using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using System.Net;
using System.Threading;

namespace P2PChat
{
    public partial class Form_online_user : Form
    {
      // 每3s 发送心跳包

        AsySocket socket = null;
        public AsySocket socket_parameter
        {
            get
            {
                return socket;
            }
            set
            {
                this.socket = value;
            }
        }

        string name = null;
        public string user_name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }


        //定义server ip 和 端口 
        private IPEndPoint server_ip_port;
        int type = 0;


        //发送心跳包的thread 
        Thread heart_thread;

        public Form_online_user()
        {
            InitializeComponent();
        }

        TreeNode root = new TreeNode();


        //窗体加载的时候建立和服务器的连接
        //
        private void Form_online_user_Load(object sender, EventArgs e)
        {
            root.Text = "在线用户";
            treeView_user.Nodes.Add(root);
           
           

            //socket
            //服务器的地址
            server_ip_port = new IPEndPoint(IPAddress.Parse("10.211.5.7"), 6789);
            socket = new AsySocket("any", 3456);

            //发送消息事件
            socket.OnSended += new AsySocketEventHandler(socket_OnSended);

            //接收到socket 信息
            socket.OnStreamDataAccept += new StreamDataAcceptHandler(socket_OnStreamDataAccept);

            socket.OnClosed += new AsySocketClosedEventHandler(socket_OnClosed);

            //连接server
            socket.LinkObject.Connect(server_ip_port);
            socket.BeginAcceptData();


            //发送心跳包
            heart_thread = new Thread(heart_socket);
            heart_thread.Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            node.Text = "button ";
            root.Nodes.Add(node);
        }

        private void treeView_user_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            root.Nodes.Clear();
        }

        private void treeView_user_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void treeView_user_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Console.WriteLine(e.Node.Text);
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

        protected void heart_socket()
        {
            //间隔 3000 ms 发送心跳包
            while(true)
            {
                send_heart_pack();
                Thread.Sleep(3000);
            }
        }

        protected void send_heart_pack()
        {
            //心跳包类型为 8
            socket.ASend(8, "", "", UTF8Encoding.UTF8.GetBytes("heart"), DateTime.Now, "");
        }

    }
}
