using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class Form_login : Form
    {
        public Form_login()
        {
            InitializeComponent();
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            Form_register form_register = new Form_register();
            form_register.Show();
        }

        private void Form_login_Load(object sender, EventArgs e)
        {
          
        }
    }
}
