using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Client_statue
    {
        string name { get; set; }
        IPEndPoint ipendpoint { get; set; }
        DateTime time { get; set; }

        public Client_statue(string name, IPEndPoint ip, DateTime time)
        {
            this.name = name;
            this.ipendpoint = ip;
            this.time = time;
        }

        public DateTime getTime()
        {
            return time;
        }

        public void changeTime(DateTime time)
        {
            this.time = time;
        }
    }
}
