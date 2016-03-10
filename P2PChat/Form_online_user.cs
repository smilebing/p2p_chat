using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PChat
{
    public partial class Form_online_user : Form
    {
      // 每3s 发送心跳包

        public Form_online_user()
        {
            InitializeComponent();
        }

        TreeNode root = new TreeNode();


        private void Form_online_user_Load(object sender, EventArgs e)
        {
            root.Text = "在线用户";
            treeView_user.Nodes.Add(root);
            //发送心跳包
           

          
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
    }
}
