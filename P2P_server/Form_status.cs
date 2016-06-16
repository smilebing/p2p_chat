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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Model;
using System.IO;


namespace P2P_server
{
    public partial class Form_status : Form
    {

        public Form_status()
        {
            InitializeComponent();
        }

        private void Form_status_Load(object sender, EventArgs e)
        {
            Console.WriteLine(target);
            //连接数据库
            CheckForIllegalCrossThreadCalls = false;
            conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + exePath + @"\P2P_db.mdb");
            access = new Access(conn);
            access.openConn();

        }

        static DirectoryInfo dir = new DirectoryInfo(Application.StartupPath).Parent.Parent.Parent;
        static string target = dir.FullName;

        //static string exePath = @"C:\Users\bing\Documents\P2PChat";//本程序所在路径
        static string exePath = target;

        //创建连接对象
        OleDbConnection conn;
        Access access;

        //创建接受数据thread
        Thread receiveThread;
        //创建刷新在线用户的thread
        Thread updateClientsThread;

        //UdpClient serverUdp = new UdpClient(1234);




        //关闭窗体
        private void Form_status_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (receiveThread != null)
            {
                receiveThread.DisableComObjectEagerCleanup();
            }
            if (updateClientsThread != null)
            {
                updateClientsThread.DisableComObjectEagerCleanup();
            }
            access.closeConn();
            udpserver.Close();
        }






        //存储在线的用户
        SortedList<string, Client_statue> online_clients = new SortedList<string, Client_statue>();


        //开启监听
        private void button_listen_Click(object sender, EventArgs e)
        {
            Console.WriteLine("begin receive");
            receiveThread = new Thread(ReceiveMessage);
            updateClientsThread = new Thread(update_online_clients);
            receiveThread.Start();
            updateClientsThread.Start();
        }




        void AddMsg(string msg, string id)
        {

            if (msg != null && msg != "")
            {
                ListViewItem lv = new ListViewItem(msg);
                lv.SubItems.Add(msg);
                lstMsg.Items.Add(lv);
            }
            if (id != null && id != "")
            {
                lstUser.Items.Add(id);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form_Access_test t = new Form_Access_test();
            t.Show();
        }


        #region call back
        UdpClient udpserver = SingletonServer.getUdpServer();
        IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 0);
        public void ReceiveCallback(IAsyncResult ar)
        {
            //UdpState udpReceiveState = ar.AsyncState as UdpState;
            // UdpState udpReceiveState = (UdpState)ar.AsyncState;



            if (ar.IsCompleted)
            {
                UdpState udpReceiveState = ar.AsyncState as UdpState;
                if (udpReceiveState == null)
                {
                    Console.WriteLine("null error =======================");
                }


                Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(ar, ref udpReceiveState.ipEndPoint);
                MyTreaty my = MyTreaty.GetMyTreaty(receiveBytes);
                //接收完数据，进行数据判断回复
                //readMsg(udpReceiveState, my);
                // string receiveString = Encoding.ASCII.GetString(receiveBytes);
                //Console.WriteLine("Received: {0}", receiveString);
                //Thread.Sleep(100);
                //receiveDone.Set();
                //SendMsg();
            }
        }

        public void ReceiveMsg()
        {
            Console.WriteLine("listening for messages");
            while (true)
            {
                lock (this)
                {
                    IAsyncResult ar = udpserver.BeginReceive(new AsyncCallback(ReceiveCallback), udpserver);
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
                    byte[] bytRecv = udpserver.Receive(ref remoteIpep);
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

            if (mytreaty.Type == 8) //心跳包
            {
                //更新心跳包
                //判断用户是否在列表中
                Client_statue client = new Client_statue(name, remoteIpep, DateTime.Now);
                online_clients.Add(name, client);

                //反馈心跳包，发送在线用户
                MyTreaty msg = new MyTreaty(8, mytreaty.Name, "", UTF8Encoding.UTF8.GetBytes("online_users"), DateTime.Now, "", online_clients);
                udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
            }


            if (mytreaty.Type == 1) //login
            {
                //验证密码
                if (access.search(name, pwd) == true)
                {
                    //验证成功
                    MyTreaty msg = new MyTreaty(1, mytreaty.Name, "", UTF8Encoding.UTF8.GetBytes("success"), DateTime.Now, "");
                    udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
                }
                else
                {
                    MyTreaty msg = new MyTreaty(1, mytreaty.Name, "", UTF8Encoding.UTF8.GetBytes("fail"), DateTime.Now, "");
                    udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
                }
            }

            if (mytreaty.Type == 2) //register
            {
                //注册
                Console.WriteLine("收到注册信息：" + name + "   " + pwd);
                //是否已经注册
                if (access.search(name) == false)
                {
                    //尚未注册
                    if (access.insert(name, pwd) == true)
                    {
                        //注册成功
                        MyTreaty msg = new MyTreaty(2, "success", "", UTF8Encoding.UTF8.GetBytes("success"), DateTime.Now, "");
                        udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
                    }
                    else
                    {
                        //失败
                        MyTreaty msg = new MyTreaty(2, "fail", "", UTF8Encoding.UTF8.GetBytes("fail"), DateTime.Now, "");
                        udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
                    }
                    //注册结果反馈
                }
                else
                {
                    //已经注册，返回失败信息
                    MyTreaty msg = new MyTreaty(2, "", "", UTF8Encoding.UTF8.GetBytes("exist"), DateTime.Now, "");
                    udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), remoteIpep);
                }

            }
        }

        //刷新在线用户
        public void update_online_clients()
        {
            while (true)
            {
                lock (online_clients)
                {
                    DateTime now = DateTime.Now;
                    foreach (KeyValuePair<string, Client_statue> kvp in online_clients)
                    {
                        TimeSpan ts = now - kvp.Value.getTime();
                        if (ts.Seconds > 3)
                        {
                            //超时3s
                            online_clients.Remove(kvp.Key);
                        }
                    }
                }
                Thread.Sleep(3500);
            }
        }
        #endregion

        private void button_close_server_Click(object sender, EventArgs e)
        {
            //停止服务按键
            receiveThread.DisableComObjectEagerCleanup();
            updateClientsThread.DisableComObjectEagerCleanup();
        }
    }

    //存储udp state
    public class UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }




}
