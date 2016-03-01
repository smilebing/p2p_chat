using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class Form_login : Form
    {
        public Form_login()
        {
            InitializeComponent();
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            Form_register form_register = new Form_register();
            form_register.Show();
        }

        private void Form_login_Load(object sender, EventArgs e)
        {
            byte[] data = new byte[1024];
            string input, stringData;
            //构建TCP 服务器  
            Console.WriteLine("This is a Client, host name is {0}", Dns.GetHostName());
            //设置服务IP，设置TCP端口号  
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
            //定义网络类型，数据连接类型和网络协议UDP  
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            string welcome = "你好! ";
            data = Encoding.ASCII.GetBytes(welcome);
            server.SendTo(data, data.Length, SocketFlags.None, ip);
            IPEndPoint senderp = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)senderp;
            data = new byte[1024];
            //对于不存在的IP地址，加入此行代码后，可以在指定时间内解除阻塞模式限制  
            int recv = server.ReceiveFrom(data, ref Remote);
            Console.WriteLine("Message received from {0}: ", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            while (true)
            {
                input = Console.ReadLine();
                if (input == "exit")
                    break;
                server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref Remote);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(stringData);
            }
            Console.WriteLine("Stopping Client.");
            server.Close();    
        }
    }
}
