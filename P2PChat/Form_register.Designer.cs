namespace P2PChat
{
    partial class Form_register
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_resister = new System.Windows.Forms.Button();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_pwd1 = new System.Windows.Forms.TextBox();
            this.textBox_pwd2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "confirm password";
            // 
            // button_resister
            // 
            this.button_resister.Location = new System.Drawing.Point(166, 186);
            this.button_resister.Name = "button_resister";
            this.button_resister.Size = new System.Drawing.Size(75, 23);
            this.button_resister.TabIndex = 3;
            this.button_resister.Text = "register";
            this.button_resister.UseVisualStyleBackColor = true;
            this.button_resister.Click += new System.EventHandler(this.button_resister_Click);
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(166, 42);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(179, 21);
            this.textBox_name.TabIndex = 4;
            // 
            // textBox_pwd1
            // 
            this.textBox_pwd1.Location = new System.Drawing.Point(166, 79);
            this.textBox_pwd1.Name = "textBox_pwd1";
            this.textBox_pwd1.Size = new System.Drawing.Size(179, 21);
            this.textBox_pwd1.TabIndex = 5;
            // 
            // textBox_pwd2
            // 
            this.textBox_pwd2.Location = new System.Drawing.Point(166, 119);
            this.textBox_pwd2.Name = "textBox_pwd2";
            this.textBox_pwd2.Size = new System.Drawing.Size(179, 21);
            this.textBox_pwd2.TabIndex = 6;
            // 
            // Form_register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 244);
            this.Controls.Add(this.textBox_pwd2);
            this.Controls.Add(this.textBox_pwd1);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.button_resister);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form_register";
            this.Text = "Form_register";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_register_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_register_FormClosed);
            this.Load += new System.EventHandler(this.Form_register_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_resister;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_pwd1;
        private System.Windows.Forms.TextBox textBox_pwd2;
    }
}