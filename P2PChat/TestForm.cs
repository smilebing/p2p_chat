using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("the form is loading-----");
            //Thread t = new Thread(test);
           // t.Start();

            new System.Threading.Thread((System.Threading.ThreadStart)delegate
            {
                Application.Run(new Form1());
            }).Start();
            //this.Close();

        }
        Form1 f = new Form1();
        public void test()
        {
            Console.WriteLine("the thread form load");
           
            f.Show();
            Console.WriteLine("show form1");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
