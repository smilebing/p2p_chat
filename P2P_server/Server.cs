using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;  

namespace P2P_server
{
    class Server
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 12304;   //端口  
        static Socket serverSocket;

        private UdpClient udpcRecv;
        private UdpClient udpcSend;

        Thread thrRecv;

        //开启服务 传递端口值,返回是否启动成功
        private bool start_server(int prot=12304)
        {
            //listen
            return false;
        }


        //监听
        //public bool listen(int mprot,string mip)
        public bool listen()
        
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           // IPAddress ip = getValidIP(mip);
           // int port = getValidPort(mprot);


           // UdpClient udpClient = new UdpClient(12304);
           // Thread rec_thread = new Thread(ReceiveMessage);




            
                IPEndPoint localIpep = new IPEndPoint(
                    IPAddress.Parse("127.0.0.1"), 8848); // 本机IP和监听端口号
 
                udpcRecv = new UdpClient(localIpep);
 
                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();

                Console.WriteLine( "UDP监听器已成功启动");

                Thread send_thread = new Thread(SendMessage);
                send_thread.Start();











           // IPEndPoint ipep = new IPEndPoint(ip, port);
            return false;
        }

        //接收函数
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    string message = Encoding.Unicode.GetString(
                        bytRecv, 0, bytRecv.Length);
                    Console.WriteLine("收到信息："+message);

                  
                }
                catch (Exception ex)
                {
                    //ShowMessage(txtRecvMssg, ex.Message);
                    break;
                }
            }
        }

        //发送消息

        private void SendMessage()
        {
            //object obj = "wo shi client";
            string message = "wo shi client";
            byte[] sendbytes = Encoding.Unicode.GetBytes(message);

            IPEndPoint remoteIpep = new IPEndPoint(
                IPAddress.Parse("127.0.0.1"), 8848); // 发送到的IP地址和端口号


            IPEndPoint localIpep = new IPEndPoint(
            IPAddress.Parse("127.0.0.1"), 12345); // 本机IP，指定的端口号
            udpcSend = new UdpClient(localIpep);
            udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);
            udpcSend.Close();
        }




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


        private int getValidPort(int port)
        {
            int lport;
            //测试端口号是否有效  
            try
            {
                //是否不为正
                if (port <=0)
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
    }
}
