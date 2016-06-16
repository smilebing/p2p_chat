using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using System.Net;
using System.Threading;

namespace P2PChat
{
    public partial class Form_online_user : Form
    {
        //单例模式
       static Form_online_user instance;

        //当前用户name
        string name = null;
        public string user_name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }


        string serverIp = "127.0.0.1";
        //定义server ip 和 端口 
        private IPEndPoint server_ip_port ;
        public static SortedList<string, Client_statue> online_clients = new SortedList<string, Client_statue>();


        //发送心跳包的thread 
        Thread heart_thread;

        private Form_online_user()
        {
            InitializeComponent();
        }

        public static Form_online_user getInstance()
        {
            if (instance == null)
            {
                instance = new Form_online_user();
            }
            return instance;
        }

        TreeNode root = new TreeNode();


        private void Form_online_user_Load(object sender, EventArgs e)
        {
            //server_ip_port = new IPEndPoint(IPAddress.Parse(serverIp), 1234);
            root.Text = "在线用户";
            treeView_user.Nodes.Add(root);
           
            ////发送心跳包
            //heart_thread = new Thread(heart_socket);
            //heart_thread.Start();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            node.Text = "button ";
            root.Nodes.Add(node);
        }

        private void treeView_user_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            root.Nodes.Clear();
        }

        private void treeView_user_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void treeView_user_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Console.WriteLine(e.Node.Text);
        }





    
    
        /// <summary>
        /// 绑定消息
        /// </summary>
        /// <param name="msg"></param>
        private void AddMsg(string msg)
        {

            //ListViewItem lv = new ListViewItem(msg);
            //lv.SubItems.Add(msg);
            //lstMsg.Items.Add(lv);

        }

        protected void heart_socket()
        {
            //间隔 3000 ms 发送心跳包
            while(true)
            {
                send_heart_pack();
                Thread.Sleep(3000);
            }
        }

        protected void send_heart_pack()
        {
            //心跳包类型为 8
            MyTreaty msg = new MyTreaty(8, Name,"", UTF8Encoding.UTF8.GetBytes("heart"), DateTime.Now, ""); 
        }


        public void updateForm(SortedList<string, Client_statue> clients)
        {
            root.Nodes.Clear();
            foreach (KeyValuePair<string, Client_statue> kvp in clients)
            {
                TreeNode node = new TreeNode();
                node.Text = kvp.Key;
              root.Nodes.Add(node);
            }
        }

        public delegate void testDelegate(SortedList<string, Client_statue> clients);
   

        public  void readMsg(IPEndPoint remoteIpep, MyTreaty mytreaty)
        {
         
            //处理心跳包
            //处理聊天内容
            if (mytreaty.Type == 8) //心跳包
            {
                //更新在线用户界面
                lock(online_clients)
                {
                    online_clients = mytreaty.online_clients;
                }

                updateForm(online_clients);
              
            }

        }


    }
}
