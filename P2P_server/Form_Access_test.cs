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
using System.Data.OleDb;

namespace P2P_server
{
    public partial class Form_Access_test : Form
    {
        public Form_Access_test()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine( access.insert(textBox1.Text, textBox2.Text));
        }

        Access access;
        static string exePath = @"C:\Users\bing\Documents\P2PChat";//本程序所在路径
        //创建连接对象
        OleDbConnection conn;

        private void Form_Access_test_Load(object sender, EventArgs e)
        {
           
            conn = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + exePath + @"\P2P_db.mdb");
            access = new Access(conn);
         
            access.openConn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine(access.search(textBox1.Text, textBox2.Text));

        }

        private void button3_Click(object sender, EventArgs e)
        {
           Console.WriteLine( access.update(textBox1.Text, textBox2.Text));
        }
    }
}
