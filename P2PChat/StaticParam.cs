using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace P2PChat
{
    public class StaticParam
    {
        private static AsySocket socket;
        public static AsySocket Socket
        {
            get
            {
                return socket;
            }
            set
            {
                socket = value;
            }
        }
    }
}
