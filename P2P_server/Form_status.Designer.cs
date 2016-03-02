namespace P2P_server
{
    partial class Form_status
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_listen = new System.Windows.Forms.Button();
            this.lstMsg = new System.Windows.Forms.ListView();
            this.lstUser = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // button_listen
            // 
            this.button_listen.Location = new System.Drawing.Point(417, 328);
            this.button_listen.Name = "button_listen";
            this.button_listen.Size = new System.Drawing.Size(75, 23);
            this.button_listen.TabIndex = 0;
            this.button_listen.Text = "开启服务";
            this.button_listen.UseVisualStyleBackColor = true;
            this.button_listen.Click += new System.EventHandler(this.button_listen_Click);
            // 
            // lstMsg
            // 
            this.lstMsg.Location = new System.Drawing.Point(39, 12);
            this.lstMsg.Name = "lstMsg";
            this.lstMsg.Size = new System.Drawing.Size(363, 232);
            this.lstMsg.TabIndex = 17;
            this.lstMsg.UseCompatibleStateImageBehavior = false;
            this.lstMsg.View = System.Windows.Forms.View.List;
            // 
            // lstUser
            // 
            this.lstUser.FormattingEnabled = true;
            this.lstUser.ItemHeight = 12;
            this.lstUser.Location = new System.Drawing.Point(511, 12);
            this.lstUser.MultiColumn = true;
            this.lstUser.Name = "lstUser";
            this.lstUser.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstUser.Size = new System.Drawing.Size(141, 232);
            this.lstUser.TabIndex = 18;
            // 
            // Form_status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 388);
            this.Controls.Add(this.lstUser);
            this.Controls.Add(this.lstMsg);
            this.Controls.Add(this.button_listen);
            this.Name = "Form_status";
            this.Text = "server status";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_status_FormClosed);
            this.Load += new System.EventHandler(this.Form_status_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_listen;
        private System.Windows.Forms.ListView lstMsg;
        private System.Windows.Forms.ListBox lstUser;
    }
}

