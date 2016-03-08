using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using Model;
using System.Net;

namespace P2PChat
{

    public partial class Form_register : Form
    {
      

        public Form_register()
        {
            InitializeComponent();
        }

        int type = 0;

        //发送注册信息的socket
        private AsySocket socket = null;
        //定义server ip 和 端口 
        private IPEndPoint server_ip_port;
        
        //窗体加载时 
        private void Form_register_Load(object sender, EventArgs e)
        {
            

            server_ip_port=  new IPEndPoint(IPAddress.Parse("10.21.125.130"), 6789);
            //连接
            socket = new AsySocket("any", 1234);
            //socket = new AsySocket("172.0.0.1", 0);
            //socket=new AsySocket()
            socket.OnSended += new AsySocketEventHandler(socket_OnSended);

            socket.OnStreamDataAccept += new StreamDataAcceptHandler(socket_OnStreamDataAccept);

            socket.OnClosed += new AsySocketClosedEventHandler(socket_OnClosed);

            //连接server
            socket.LinkObject.Connect(server_ip_port);
            socket.BeginAcceptData();
        }

        //注册button
        private void button_resister_Click(object sender, EventArgs e)
        {
            //判断输入内容合法性
            if (textBox_name.Text.Trim() == "")
            {
                MessageBox.Show("请输入name!");
                return;
            }
            if(textBox_pwd1.Text.Trim()==""||textBox_pwd2.Text.Trim()=="")
            {
                MessageBox.Show("请输入密码!");
                return;
            }
            if(string.Compare(textBox_pwd1.Text.Trim(),textBox_pwd2.Text.Trim())!=0)
            {
                MessageBox.Show("两次密码不一致!");
                return;
            }

            

            MyTreaty register_msg=new MyTreaty(6,textBox_name.Text.Trim(),UTF8Encoding.UTF8.GetBytes(textBox_pwd1.Text.Trim()),DateTime.Now,"");

            socket.ASend(6, textBox_name.Text.Trim(), UTF8Encoding.UTF8.GetBytes(textBox_pwd1.Text.Trim()), DateTime.Now, "");
            type = 0;

        


            textBox_name.Text = "";
            textBox_pwd1.Text = "";
            textBox_pwd2.Text = "";
        }

        //关闭窗口后 关闭数据库连接
        private void Form_register_FormClosed(object sender, FormClosedEventArgs e)
        {
         
        }



        void socket_OnStreamDataAccept(string AccepterID, MyTreaty AcceptData)
        {
            if (AcceptData.Type == 0)//文本
            {
                string msg = AcceptData.Date + " " + AcceptData.Name + " : " + System.Text.Encoding.Default.GetString(AcceptData.Content).Trim();
                //AddMsg(msg);
                Console.WriteLine("收到消息:"+msg);
            }
            else if (AcceptData.Type == 1)
            {
                string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "的图片";
                //AddMsg(msg);
                //picBox.Image = Image.FromStream(new MemoryStream(AcceptData.Content));
            }
            else
            {
                string msg = AcceptData.Date + " 收到 " + AcceptData.Name + "名叫：" + AcceptData.FileName + "的文件";
                if (MessageBox.Show(msg + "，是否接收", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    //try
                    //{

                    //    sFD.Filter = AcceptData.FileName + " | *." + Path.GetExtension(AcceptData.FileName);

                    //    if (sFD.ShowDialog() == DialogResult.OK)
                    //    {
                    //        FileStream fs = new FileStream(sFD.FileName, FileMode.Create, FileAccess.Write);
                    //        fs.Write(AcceptData.Content, 0, Convert.ToInt32(AcceptData.Content.Length));
                    //        fs.Close();
                    //        AddMsg(msg);
                    //    }

                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}
                }

            }
        }

        void socket_OnClosed(string SocketID, string ErrorMessage)
        {
            //服务器关闭
            MessageBox.Show(this, "服务器关闭");
            this.Close();
        }
        ///// <summary>
        ///// 接收数据触发
        ///// </summary>
        ///// <param name="AccepterID"></param>
        ///// <param name="AcceptData"></param>
        //void socket_OnStringDataAccept(string AccepterID, string AcceptData)
        //{
        //    AddMsg(AcceptData);
        //}
        /// <summary>
        /// 发送消息触发
        /// </summary>
        /// <param name="SenderID"></param>
        /// <param name="EventMessage"></param>
        void socket_OnSended(string SenderID, string EventMessage)
        {
            if (type == 0)
            {
                //AddMsg("我: " + txtSend.Text);
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if (type == 1)
            {
                //AddMsg("图片发送成功");
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if (type == 2)
            {
                //AddMsg("文件发送成功");
                //txtSend.Text = "";
                //txtSend.Focus();
            }
            else if(type==6)
            {
                Console.WriteLine("client 发送了注册信息");
            }
        }
        // delegate void CallBackRef(string msg);
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




    }
}
