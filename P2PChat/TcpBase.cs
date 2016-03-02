using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace P2PChat
{
    class TcpBase
    {
        static void Main(string[] args)
        {
            int port = 2000;
            string host = "127.0.0.1";

            //创建终结点
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            //创建Socket并开始监听

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建一个Socket对象，如果用UDP协议，则要用SocketTyype.Dgram类型的套接字
            s.Bind(ipe);    //绑定EndPoint对象(2000端口和ip地址)
            s.Listen(0);    //开始监听

            Console.WriteLine("等待客户端连接");

            //接受到Client连接，为此连接建立新的Socket，并接受消息
            Socket temp = s.Accept();   //为新建立的连接创建新的Socket
            Console.WriteLine("建立连接");
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            bytes = temp.Receive(recvBytes, recvBytes.Length, 0); //从客户端接受消息
            recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

            //给Client端返回信息
            Console.WriteLine("server get message:{0}", recvStr);    //把客户端传来的信息显示出来
            string sendStr = "ok!Client send message successful!";
            byte[] bs = Encoding.ASCII.GetBytes(sendStr);
            temp.Send(bs, bs.Length, 0);  //返回信息给客户端
            temp.Close();
            s.Close();
            Console.ReadLine();

        }
    }
}
