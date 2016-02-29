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
            ThreadPool.QueueUserWorkItem(TestMethod, "Hello--------------");
            ThreadPool.QueueUserWorkItem(server);


            Server s = new Server();
            s.listen();
        
        }


        //关闭窗体
        private void Form_status_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();
        }


        public static void TestMethod(object data)
        {
            string datastr = data as string;
            Console.WriteLine(datastr);
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
    }
}
