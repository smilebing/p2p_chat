using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace P2PChat
{
    static  class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// 
       
       [STAThread]
        public static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            Form_login form_login = Form_login.getInstance();
          
            //form_online_user.Show();

           // TestForm tf = new TestForm();
           // tf.Show();
           Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();

            //构造了三个窗体，用来统一资源
          

            Application.Run(form_login);
        }

       // public static void showForm(string type,string name="")
       //{
       //    Form_register form_register = new Form_register();
       //    Form_online_user form_online_user =Form_online_user.getInstance();
       //    Form_login form_login = Form_login.getInstance();
       //     if(type=="register")
       //     {
       //         form_register.Show();
       //     }
       //     if(type=="user")
       //     {
       //         form_login.closeForm();
       //         form_online_user.user_name = name;
       //         form_online_user.Show();

       //     }
       //}


        //clien端接收数据的函数
       public static void ReceiveMessage()
        {
            //Form_online_user form_online_user=new Form_online_user();
            Form_online_user form_online_user = Form_online_user.getInstance();
            UdpClient udpclient = SingletonClient.getUdpClient();
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);

            Form_login form_login = Form_login.getInstance();

            //bool flag = false;           
          
            while (true)
            {
                try
                {
                    byte[] bytRecv = udpclient.Receive(ref remoteIpep);
                    MyTreaty msg = MyTreaty.GetMyTreaty(bytRecv);
                    
                    if(msg.Type==1)
                    {
                        //登录信息
                        //MessageBox.Show("login success");
                        //tf.Show();
                        //new System.Threading.Thread((System.Threading.ThreadStart)delegate
                        //{
                       // if (flag == false)
                        //{
                        //    Application.Run(form_online_user);
                        //    flag = true;
                        //}
                       // }).Start();  

                       //form_online_user.user_name = msg.Name;
                       //form_online_user.Show();
                       form_login.readMsg(remoteIpep, msg);

                    }
                    if(msg.Type==2)
                    {
                        //注册信息
                        Form_register.readMsg(remoteIpep, msg);
                    }
                    else
                    {
                        //其他信息
                   form_online_user.readMsg(remoteIpep, msg);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }
}
