using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Model
{
    public class SingletonClient
    {
        private SingletonClient()
        {

        }
        static SingletonClient s;
        static UdpClient udpClient = new UdpClient(2345);
        public static SingletonClient getInstance()
        {
            if(s==null)
            {
                s = new SingletonClient();
            }
            return s;
        }

        public static UdpClient getUdpClient()
        {
            return udpClient;
        }
    }
}
