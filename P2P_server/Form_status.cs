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
    //存储udp state
    public class UdpState
    {
        public UdpClient udpClient;
        public IPEndPoint ipEndPoint;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }


      public class AsyncUdpSever
    {
        private IPEndPoint ipEndPoint = null;
        private IPEndPoint remoteEP = null;
        private UdpClient udpReceive = null;
        private UdpClient udpSend = null;
        private const int listenPort = 1100;
        private const int remotePort = 1101;
        UdpState udpReceiveState = null;
        UdpState udpSendState = null;
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        public AsyncUdpSever()
        {
            ipEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            remoteEP = new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], remotePort);
            udpReceive = new UdpClient(ipEndPoint);
            udpSend = new UdpClient();
            udpReceiveState = new UdpState();            
            udpReceiveState.udpClient = udpReceive;
            udpReceiveState.ipEndPoint = ipEndPoint;

            udpSendState = new UdpState();
            udpSendState.udpClient = udpSend;
            udpSendState.ipEndPoint = remoteEP;
        }
        public void ReceiveMsg()
        {
            Console.WriteLine("listening for messages");
            while(true)
            {
                lock (this)
                {   
                    IAsyncResult iar = udpReceive.BeginReceive(new AsyncCallback(ReceiveCallback), udpReceiveState);
                    receiveDone.WaitOne();
                    Thread.Sleep(100);
                }
            }
        }
        private void ReceiveCallback(IAsyncResult iar)
        {
            UdpState udpReceiveState = iar.AsyncState as UdpState;
            if (iar.IsCompleted)
            {
                Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(iar, ref udpReceiveState.ipEndPoint);
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                Console.WriteLine("Received: {0}", receiveString);
                //Thread.Sleep(100);
                receiveDone.Set();
                SendMsg();
            }
        }

        private void SendMsg()
        {
            udpSend.Connect(udpSendState.ipEndPoint);
            udpSendState.udpClient = udpSend;
            udpSendState.counter ++;

            string message = string.Format("第{0}个UDP请求处理完成！",udpSendState.counter);
            Byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            udpSend.BeginSend(sendBytes, sendBytes.Length, new AsyncCallback(SendCallback), udpSendState);
            sendDone.WaitOne();
        }
        private void SendCallback(IAsyncResult iar)
        {
            UdpState udpState = iar.AsyncState as UdpState;
            Console.WriteLine("第{0}个请求处理完毕！", udpState.counter);
            Console.WriteLine("number of bytes sent: {0}", udpState.udpClient.EndSend(iar));
            sendDone.Set();
        }

        //public static void Main()
        //{
        //    AsyncUdpSever aus = new AsyncUdpSever();
        //    Thread t = new Thread(new ThreadStart(aus.ReceiveMsg));
        //    t.Start();
        //    Console.Read();
        //}
    }


    //the original 


    public partial class Form_status : Form
    {
        static DirectoryInfo dir = new DirectoryInfo(Application.StartupPath).Parent.Parent.Parent;
        static string target = dir.FullName;
            
        //static string exePath = @"C:\Users\bing\Documents\P2PChat";//本程序所在路径
        static string exePath = target;
        
        //创建连接对象
        OleDbConnection conn;
        Access access;



        UdpClient serverUdp = new UdpClient(1234);

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


        //关闭窗体
        private void Form_status_FormClosed(object sender, FormClosedEventArgs e)
        {
            access.closeConn();
            serverUdp.Close();
        }






        
        //定义一个异步传输的socket
        AsySocket listener = null;
        //存储在线的用户
        SortedList<string, AsySocket> clients = new SortedList<string, AsySocket>();

        SortedList<string, IPEndPoint> online_clients = new SortedList<string, IPEndPoint>();


        //开启监听
        private void button_listen_Click(object sender, EventArgs e)
        {
            serverUdp.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        

            //listener = new AsySocket("any", 6789);
            //listener.OnAccept += new AcceptEventHandler(listener_OnAccept);
            //listener.Listen(5);
            //button_listen.Enabled = false;

        }


        /// <summary>
        /// 有客户端接入触发此事件
        /// </summary>
        /// <param name="AcceptedSocket"></param>
        void listener_OnAccept(AsySocket AcceptedSocket)
        {
            //注册事件

            AcceptedSocket.OnClosed += new AsySocketClosedEventHandler(AcceptedSocket_OnClosed);
            AcceptedSocket.OnStreamDataAccept += new StreamDataAcceptHandler(AcceptedSocket_OnStreamDataAccept);
            AcceptedSocket.BeginAcceptData();
            //加入
            //AddMsg("", AcceptedSocket.ID);
            //AddMsg(string.Format("{0}上线！", AcceptedSocket.ID), "");

            //clients.Add(AcceptedSocket.ID, AcceptedSocket);
        }


        //接收client发送过来的 MyTreaty
        //void AcceptedSocket_OnStreamDataAccept(string AccepterID, MyTreaty AcceptData)
       void AcceptedSocket_OnStreamDataAccept(AsySocket accept_socket, MyTreaty AcceptData)   
        {  
            //文本 type=6
            //图片 type=7
            //注册 type=0 
            //登录 type=1
            string name = AcceptData.Name;
            string pwd = AcceptData.Pwd;

            if(AcceptData.Type==8) //心跳包
            {
                    //将用户名 存储到 list 中 
                    if(online_clients.ContainsKey(name)==false)
                    {
                        //添加在线user
                       
                        online_clients.Add(name, accept_socket.get_ipEndPoint());
                        AddMsg(string.Format("{0}上线！",name), "");
                       
                    }
                    else
                    {
                        //更新 accept__data        
                        online_clients.Values[ online_clients.IndexOfKey(name)] = accept_socket.get_ipEndPoint();
                    }


                    EndPoint endPoint= accept_socket.get_ipEndPoint();
                    Console.WriteLine(endPoint.ToString());

            }
            else if (AcceptData.Type == 1)//用户登录
            {
            //    accept_socket.ASend(1, "yasd", "", UTF8Encoding.UTF8.GetBytes("yasdf"), DateTime.Now, "");
                //查找socket
                for(int i=0;i<clients.Count;i++)
                {
                    if (clients.Values[i].ID == name)
                    {
                        Console.WriteLine("登录 比对id");
                        //发送登录结果
                        if (access.search(name, pwd) == true)
                        {
                            clients.Values[i].ASend(1, "y", "", UTF8Encoding.UTF8.GetBytes("y"), DateTime.Now, "");
                        }
                        else
                        {
                            clients.Values[i].ASend(1, "n", "", UTF8Encoding.UTF8.GetBytes("n"), DateTime.Now, "");

                        }
                    }
                }




                string msg = AcceptData.Date + " " + AcceptData.Name + " : " + System.Text.Encoding.Default.GetString(AcceptData.Content).Trim();
                AddMsg(msg, "");


                //用来转发消息
                //for (int i = 0; i < clients.Count; i++)

                //{
                //    if (clients.Values[i].ID != AccepterID)
                //    {
                        //clients.Values[i].ASend(0, AcceptData.Name, AcceptData.Content, AcceptData.Date, AcceptData.FileName);
                //    }
                //}

            }
            else if (AcceptData.Type == 0) //用户注册
            {
              

                Console.WriteLine("收到注册信息：" + name + "   " + pwd);

             
                //查找socket
                for (int i = 0; i < clients.Count; i++)
                {
                    Console.WriteLine("注册 比对id");

                    if (clients.Values[i].ID == name)
                    {
                        //发送登录结果
                        if (access.insert(name, pwd) == true)
                        {
                            clients.Values[i].ASend(0, "y", "", UTF8Encoding.UTF8.GetBytes("y"), DateTime.Now, "");

                            Console.WriteLine("server 发送注册结果 y");
                        }
                        else
                        {
                            clients.Values[i].ASend(0, "n", "", UTF8Encoding.UTF8.GetBytes("n"), DateTime.Now, "");

                          
                            Console.WriteLine("server 发送注册结果 n");

                        }
                    }
                }


                //string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "的图片";
                //AddMsg(msg, "");
                //picBox.Image = Image.FromStream(new MemoryStream(AcceptData.Content));
                //for (int i = 0; i < clients.Count; i++)
                //{
                //    if (clients.Values[i].ID != AccepterID)
                //    {
                //        clients.Values[i].ASend(1, AcceptData.Name, AcceptData.Content, AcceptData.Date, AcceptData.FileName);
                //    }
                //}
            }
            else if (AcceptData.Type == 2)
            {
                //string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "名叫：" + AcceptData.FileName + "的文件";
                //if (MessageBox.Show(msg + "，是否接收", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{

                //    try
                //    {

                //        sFD.Filter = AcceptData.FileName + " | *." + Path.GetExtension(AcceptData.FileName);

                //        if (sFD.ShowDialog() == DialogResult.OK)
                //        {
                //            FileStream fs = new FileStream(sFD.FileName, FileMode.Create, FileAccess.Write);
                //            fs.Write(AcceptData.Content, 0, Convert.ToInt32(AcceptData.Content.Length));
                //            fs.Close();
                //            AddMsg(msg, "");
                //        }

                //    }
                //    catch (Exception)
                //    {

                //        throw;
                //    }

                //}
            }
            else if(AcceptData.Type==6)
            {
                //string name = AcceptData.Name;
                //string pwd = AcceptData.Pwd;

                //Console.WriteLine("收到注册信息：" + name + "   " + pwd);
            }


        }

        /// <summary>
        /// 关闭客户端触发此事件
        /// </summary>
        /// <param name="SocketID"></param>
        /// <param name="ErrorMessage"></param>
        void AcceptedSocket_OnClosed(string SocketID, string ErrorMessage)
        {
            //客户端关闭
            clients.Remove(SocketID);
            lstUser.Items.Remove(SocketID);
            // MessageBox.Show(ErrorMessage);
            AddMsg(string.Format("{0}下线！", SocketID), "");
            //Environment.Exit(0);
        }
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="msg"></param>
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
        UdpClient udpserver = new UdpClient(1234);
        IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 0);
        public  void ReceiveCallback(IAsyncResult ar)
        {
            UdpState udpReceiveState = ar.AsyncState as UdpState;
            if (ar.IsCompleted)
            {
                Byte[] receiveBytes = udpReceiveState.udpClient.EndReceive(ar, ref udpReceiveState.ipEndPoint);
                MyTreaty my = MyTreaty.GetMyTreaty(receiveBytes);
                //接收完数据，进行数据判断回复
                readMsg(udpReceiveState, my);
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


        public static void SendCallback(IAsyncResult ar)
        {

        }


        #endregion


        #region msg判断函数
        public void readMsg(UdpState udpstate,MyTreaty mytreaty)
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
            }
            if(mytreaty.Type==1) //login
            {
                //验证密码
            }

            if(mytreaty.Type==2) //register
            {
                //注册
                Console.WriteLine("收到注册信息：" + name + "   " + pwd);
                //是否已经注册
                if(access.search(name)==false)
                {
                    //尚未注册
                    if(access.insert(name, pwd)==true)
                    {
                        //注册成功
                        MyTreaty msg = new MyTreaty(2, "", "", UTF8Encoding.UTF8.GetBytes("success"), DateTime.Now, "");
                        udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), udpstate.ipEndPoint);
                    }
                    else
                    {
                        //失败
                        MyTreaty msg = new MyTreaty(2, "", "", UTF8Encoding.UTF8.GetBytes("fail"), DateTime.Now, "");
                        udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), udpstate.ipEndPoint); 
                    }
                    //注册结果反馈
                }
                else
                {
                    //已经注册，返回失败信息
                    MyTreaty msg = new MyTreaty(2, "", "", UTF8Encoding.UTF8.GetBytes("exist"), DateTime.Now,"");
                    udpserver.SendAsync(msg.GetBytes(), msg.GetBytes().Count(), udpstate.ipEndPoint);
                }
               
            }
        }

        #endregion
    }
}
