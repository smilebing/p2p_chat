using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Sockets.UdpClient;
using System.Net;





namespace Model
{
    //接收到信息的委托
    public delegate void msgArriveEventHandler(StatuSocket remoteClient);


    class StatuSocket
    {
        // 客户端 socket.
        public Socket workSocket = null;
        public UdpClient workUdpClinet = null;
        //客户的 ip
        public IPEndPoint remoteIpEndPoint = null;
        // 接收 buffer大小.
        public const int BufferSize = 1048576;
        // 接收 buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    //定义socket 用 updClient 进行通信
    public class MyUdpSocket
    {

        UdpClient udpclient { get; set; }
        //收到信息的 事件
        public event msgArriveEventHandler MessageArrived;


        //server 构造函数
        public MyUdpSocket()
        {
            udpclient = new UdpClient();
        }


        public MyUdpSocket(int port)
        {
            udpclient = new UdpClient(port);
        }

        //client 构造函数
        public MyUdpSocket(string ip, int port)
        {
            udpclient = new UdpClient(6776);
        }

    

        //client connect 
        public void connect(string ip,int port)
        {
            udpclient.Connect(ip, port);
        }

        public void receive()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    Byte[] receiveBytes = udpclient.Receive(ref remoteIpEndPoint);
                    StatuSocket statuSocket=new StatuSocket();
                    statuSocket.remoteIpEndPoint=remoteIpEndPoint;
                    statuSocket.workUdpClinet=udpclient;
                    statuSocket.buffer = receiveBytes;
                    //调用事件 
                    MessageArrived(statuSocket);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
        //发送函数,发送到指定的ip 和 端口
        public void send(IPEndPoint ipEndPoint,MyTreaty msg)
        {
            udpclient.Send(msg.GetBytes(), msg.GetBytes().Length, ipEndPoint);
        }


        public void listen()
        {

        }

        public void receive()
        {

        }


    

    }
}
