using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;

namespace P2PChat
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string serverIp = "10.211.55.7";

            AsySocket socket = new AsySocket("any", 3334);

            //构造了三个窗体，用来统一资源
            Form_login form_login=new Form_login();
            Form_register form_register = new Form_register();
            Form_online_user form_online_user = new Form_online_user();

            //传递变量
            form_login.SignalSocket = socket;
            form_login.form_Online_user = form_online_user;
            form_login.form_register = form_register;

         

            Application.Run(form_login);
           // Application.Run(new Form_online_user());

            


        }
    }
}
