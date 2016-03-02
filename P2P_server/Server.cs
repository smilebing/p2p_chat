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
   

            
                IPEndPoint localIpep = new IPEndPoint(
                    IPAddress.Parse("127.0.0.1"), 8848); // 本机IP和监听端口号
 
                udpcRecv = new UdpClient(localIpep);
 
                thrRecv = new Thread(ReceiveMessage);
                thrRecv.Start();

                Console.WriteLine( "UDP监听器已成功启动");

               // Thread send_thread = new Thread(SendMessage);
               // send_thread.Start();

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


                    //如果消息传输完整
                    if(message_hash_right(message))
                    {
                        string msg = message.Substring(0, message.Length - 32);
                        //处理消息
                        deal_message(msg,remoteIpep);                      
                    }

                }
                catch (Exception ex)
                {
                    //ShowMessage(txtRecvMssg, ex.Message);
                    break;
                }
            }
        }



        //判断消息类型
        private void deal_message(string message,IPEndPoint remoteIpep)
        {
            string kind = message.Substring(0, 1);
            //更新msg
            string msg = message.Substring(1,message.Length-1);
            // h 心跳包 
            // r 注册
            // l 登录

            if(string.Equals(kind,"h"))
            {
                //心跳包

            }

            if (string.Equals(kind, "l"))
            {
                //登录
                
            }

            if (string.Equals(kind, "r"))
            {
                //注册
                //msg = id + 密码
                //
            }
        }


        //判断 接收到的消息是否完整
        private bool message_hash_right(string message)
        {

            string msg = message.Substring(0, message.Length - 32);
            string hash = message.Substring(message.Length - 32);

            return string.Equals(hash, Hash_MD5_32(msg));
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


        //计算 字符串的 hash 大写值
        public static string Hash_MD5_32(string word)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider MD5CSP
                    = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                byte[] bytHash = MD5CSP.ComputeHash(bytValue);
                MD5CSP.Clear();

                //根据计算得到的Hash码翻译为MD5码
                string sHash = "", sTemp = "";
                for (int counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }
                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //返回大写字符串
                return  sHash;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
