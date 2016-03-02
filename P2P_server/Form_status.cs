﻿using System;
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


namespace P2P_server
{
    public partial class Form_status : Form
    {
        static string exePath = @"C:\Users\bing\Documents\P2PChat";//本程序所在路径
        //创建连接对象
        OleDbConnection conn;


        public Form_status()
        {
            InitializeComponent();
        }

        private void Form_status_Load(object sender, EventArgs e)
        {
            conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + exePath + @"\P2P_db.mdb");

            //将工作项加入到线程池队列中，这里可以传递一个线程参数

        
        }


        //关闭窗体
        private void Form_status_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
        }





        //判断端口号能否使用
        private int getValidPort(string port)
        {
            int lport;
            //测试端口号是否有效  
            try
            {
                //是否为空  
                if (port == "")
                {
                    throw new ArgumentException(
                        "端口号无效，不能启动DUP");
                }
                lport = System.Convert.ToInt32(port);
            }
            catch (Exception e)
            {
                //ArgumentException,   
                //FormatException,   
                //OverflowException  
                Console.WriteLine("无效的端口号：" + e.ToString());
                //this.tbMsg.AppendText("无效的端口号：" + e.ToString() + "\n");
                return -1;
            }
            return lport;
        }


        //判断ip 是否可用
        private IPAddress getValidIP(string ip)
        {
            IPAddress lip = null;
            //测试IP是否有效  
            try
            {
                //是否为空  
                if (!IPAddress.TryParse(ip, out lip))
                {
                    throw new ArgumentException(
                        "IP无效，不能启动DUP");
                }
            }
            catch (Exception e)
            {
                //ArgumentException,   
                //FormatException,   
                //OverflowException  
                Console.WriteLine("无效的IP：" + e.ToString());
                //this.tbMsg.AppendText("无效的IP：" + e.ToString() + "\n");
                return null;
            }
            return lip;
        }  



        private void server(object da)
        {
            int recv;
            byte[] data = new byte[1024];

            //得到本机IP，设置TCP端口号           
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8001);
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定网络地址  
            newsock.Bind(ip);
            Console.WriteLine("This is a Server, host name is {0}", Dns.GetHostName());
            //等待客户机连接  
            Console.WriteLine("Waiting for a client");
            //得到客户机IP  
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            recv = newsock.ReceiveFrom(data, ref Remote);
            Console.WriteLine("Message received from {0}: ", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //客户机连接成功后，发送信息  
            string welcome = "你好 ! ";
            //字符串与字节数组相互转换  
            data = Encoding.ASCII.GetBytes(welcome);
            //发送信息  
            newsock.SendTo(data, data.Length, SocketFlags.None, Remote);
            while (true)
            {
                data = new byte[1024];
                //发送接受信息  
                recv = newsock.ReceiveFrom(data, ref Remote);
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                newsock.SendTo(data, recv, SocketFlags.None, Remote);
            }  
        }


        
        //定义一个异步传输的socket
        AsySocket listener = null;
        SortedList<string, AsySocket> clients = new SortedList<string, AsySocket>();



        //开启监听
        private void button_listen_Click(object sender, EventArgs e)
        {
           
            listener = new AsySocket("192.168.1.123", 6789);
            listener.OnAccept += new AcceptEventHandler(listener_OnAccept);
            listener.Listen(5);
            button_listen.Enabled = false;

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
            AddMsg("", AcceptedSocket.ID);
            AddMsg(string.Format("{0}上线！", AcceptedSocket.ID), "");

            clients.Add(AcceptedSocket.ID, AcceptedSocket);
        }


        void AcceptedSocket_OnStreamDataAccept(string AccepterID, MyTreaty AcceptData)
        {
            if (AcceptData.Type == 0)//文本
            {
                string msg = AcceptData.Date + " " + AcceptData.Name + " : " + System.Text.Encoding.Default.GetString(AcceptData.Content).Trim();
                AddMsg(msg, "");

                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients.Values[i].ID != AccepterID)
                    {
                        clients.Values[i].ASend(0, AcceptData.Name, AcceptData.Content, AcceptData.Date, AcceptData.FileName);
                    }
                }

            }
            else if (AcceptData.Type == 1)
            {
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
    }
}