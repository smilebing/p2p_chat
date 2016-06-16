using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Model
{
    public class SingletonServer
    {
        private SingletonServer() { }
        static SingletonServer s;
        static UdpClient udpServer = new UdpClient(1234);

        public static SingletonServer getInstance()
        {
            if(s==null)
            {
                s = new SingletonServer();
            }
            return s;
        }

        public static UdpClient getUdpServer()
        {
            return udpServer;
        }
    }
}
