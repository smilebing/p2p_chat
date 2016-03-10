using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Model
{ 
    //public delegate void StreamDataAcceptHandler(string AccepterID, MyTreaty AcceptData);
    public delegate void StreamDataAcceptHandler(AsySocket accept_socket, MyTreaty AcceptData);
    
    public delegate void AsySocketEventHandler(string SenderID, string EventMessage);
    public delegate void AcceptEventHandler(AsySocket AcceptedSocket);
    public delegate void AsySocketClosedEventHandler(string SocketID, string ErrorMessage);
   
    
    
    class MySocket
    {
    }

    public class StateObject
    {
        // 客户端 socket.
        public Socket workSocket = null;
        // 接收 buffer大小.
        public const int BufferSize = 1048576;
        // 接收 buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    /// <summary>
    /// 文件名称：AsySocket.cs
    /// 描    述：异步传输对象
    /// </summary>
    public class AsySocket
    {

        #region 私有字段
        private Socket mSocket = null;

        string mID = "";

        private const string ModuleName = "AsySocket";

        private StreamDataAcceptHandler onStreamData = null;

        private AsySocketEventHandler onSended = null;
        //发送异常
        private AsySocketEventHandler onSendTo = null;
        private AcceptEventHandler onAccept = null;
        private AsySocketClosedEventHandler onClosed = null;

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="LocalIP"></param>
        /// <param name="LocalPort"></param>
        public AsySocket(string LocalIP, int LocalPort)
        {
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           

            try
            {
                //IPAddress ip = Dns.GetHostAddresses(LocalIP)[0];
                IPAddress ip;
                if (string.Compare(LocalIP, "any") == 0)
                {
                    ip = IPAddress.Any;
                }
                else
                {
                    ip = IPAddress.Parse(LocalIP);
                }
                IPEndPoint ipe = new IPEndPoint(ip, LocalPort);
                mID = Guid.NewGuid().ToString();
                mSocket.Bind(ipe);
            }
            catch (Exception e)
            {
                Console.WriteLine("构造socket 出错!");
                Console.WriteLine(e.Message);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="LinkObject">指定的Socket连接</param>
        public AsySocket(Socket linkObject)
        {
            mSocket = linkObject;
            mID = Guid.NewGuid().ToString();
        }


        #region 公共属性
        public static string EndChar
        {
            get
            {
                return new string((char)0, 1);
            }
        }
        public string ID
        {
            get
            {
                return mID;
            }
        }
        /// <summary>
        /// 发送、接受数据的结尾标志
        /// </summary>
        public static char LastSign
        {
            get
            {
                return (char)0;
            }
        }
        /// <summary>
        /// 获取、设置连接对象
        /// </summary>
        public Socket LinkObject
        {
            get
            {
                return mSocket;
            }
            set
            {
                mSocket = value;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 监听
        /// </summary>
        public void Listen(int backlog)
        {
            if (mSocket == null)
                throw new ArgumentNullException("连接不存在");

            mSocket.Listen(backlog);
            mSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);//异步
        }
        /// <summary>
        /// 开始接受数据
        /// </summary>
        public void BeginAcceptData()
        {
            if (mSocket == null)
                throw new ArgumentNullException("连接对象为空");
            //开始接收数据
            StateObject state = new StateObject();
            state.workSocket = mSocket;

            mSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            //receiveDone.WaitOne();
        }

        /// <summary>
        /// 发送二进制数据
        /// </summary>
        /// <param name="SendData"></param>
        public void ASend(byte[] SendData)
        {
            if (mSocket == null)
                throw new ArgumentNullException("连接不存在");
            if (SendData == null)
                return;
            mSocket.BeginSend(SendData, 0, SendData.Length, 0, new AsyncCallback(SendCallBack), mSocket);
            //sendDone.WaitOne();
        }
        public void ASend(int Type, string Name,string Pwd, byte[] Content, DateTime date, string fileName)
        {
            if (mSocket == null)
                throw new ArgumentNullException("连接不存在");
            if (Content == null)
                return;
            MyTreaty my = new MyTreaty(Type, Name,Pwd ,Content, date, fileName);
            byte[] SendData = my.GetBytes();
            mSocket.BeginSend(SendData, 0, SendData.Length, 0, new AsyncCallback(SendCallBack), mSocket);

        }
        /// <summary>
        /// 发送文本数据
        /// </summary>
        /// <param name="SendData"></param>
        public void ASend(string SendData)
        {
            if (SendData.Length == 0)
                return;
            this.ASend(UTF8Encoding.UTF8.GetBytes(SendData));
        }

        /// <summary>
        /// UDP发送二进制数据
        /// </summary>
        /// <param name="SendData"></param>
        /// <param name="EndPoint">目标端点</param>
        public void ASendTo(byte[] SendData, IPEndPoint EndPoint)
        {
            if (mSocket == null)
                throw new ArgumentNullException("连接不存在");
            if (SendData == null)
                return;
            //用beginSendTo 直接将消息发送给指定 IPEndPoint
            mSocket.BeginSendTo(SendData, 0, SendData.Length, 0, EndPoint, new AsyncCallback(SendToCallBack), null);
            
            //sendToDone.WaitOne();
        }
        /// <summary>
        /// UDP发送文本数据
        /// </summary>
        /// <param name="SendData"></param>
        /// <param name="EndPoint"></param>
        public void ASendTo(string SendData, IPEndPoint EndPoint)
        {
            if (SendData.Length == 0)
                return;
            ASendTo(UTF8Encoding.UTF8.GetBytes(SendData), EndPoint);
        }
        #endregion

        #region 私有方法
        
        private void AcceptCallBack(IAsyncResult ar)
        {
            Console.WriteLine("接收到一个新的连接");
            Socket handler = mSocket.EndAccept(ar);
            AsySocket NewSocket = new AsySocket(handler);
            //激发事件
            if (onAccept != null)
                onAccept(NewSocket);
            //重新监听
            mSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }


        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //
                StateObject state = ar.AsyncState as StateObject;
                //读取数据
                int bytesRead = mSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.sb.Append(UTF8Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    string sb = state.sb.ToString();
                    if (sb.Substring(sb.Length - 1, 1) == EndChar)
                    {
                        //接收完成
                        //激发事件
                        state = new StateObject();
                        state.workSocket = mSocket;
                    }
                    // Get the rest of the data.
                    mSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    MyTreaty my = MyTreaty.GetMyTreaty(state.buffer);
                    //if (onStreamData != null)
                    //{
                    //    //onStreamData(mID, my);
                    //    onStreamData(my.Name, my);
                    //    Console.WriteLine(my.Name);
                    //}
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("socket yi chang ");
                Console.WriteLine(se.Data);
                if (onClosed != null)
                    onClosed(ID, se.Message);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }

        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                mSocket.EndSend(ar);
                //触发事件
                if (onSended != null)
                    onSended(mID, "OK");
            }
            catch (SocketException se)
            {
                if (onClosed != null)
                    onClosed(ID, se.Message);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        private void SendToCallBack(IAsyncResult ar)
        {
            try
            {
                mSocket.EndSendTo(ar);
                
                if (onSendTo != null)
                    onSendTo(mID, "OK");
            }
            catch (SocketException se)
            {
                if (onClosed != null)
                    onClosed(ID, se.Message);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 连接关闭的事件
        /// </summary>
        public event AsySocketClosedEventHandler OnClosed
        {
            add
            {
                onClosed += value;
            }
            remove
            {
                onClosed -= value;
            }
        }
        /// <summary>
        /// 接受连接的事件
        /// </summary>
        public event AcceptEventHandler OnAccept
        {
            add
            {
                onAccept += value;
            }
            remove
            {
                onAccept -= value;
            }
        }
        /// <summary>
        /// 接收二进制数据事件
        /// </summary>
        public event StreamDataAcceptHandler OnStreamDataAccept
        {
            add
            {
                this.onStreamData += value;
            }
            remove
            {
                this.onStreamData -= value;
            }
        }

        /// <summary>
        /// 发送成功事件
        /// </summary>
        public event AsySocketEventHandler OnSended
        {
            add
            {
                onSended += value;
            }
            remove
            {
                onSended -= value;
            }
        }
        /// <summary>
        /// UTP发送成功事件
        /// </summary>
        public event AsySocketEventHandler OnSendTo
        {
            add
            {
                onSendTo += value;
            }
            remove
            {
                onSendTo -= value;
            }
        }
        #endregion


        public void close_socket()
        {
            mSocket.Close();
        }
    }
}






 

