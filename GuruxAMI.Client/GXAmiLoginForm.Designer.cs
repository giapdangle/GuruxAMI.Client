namespace GuruxAMI.Client
{
    partial class GXAmiLoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GXAmiLoginForm));
            this.LoginBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.NameLbl = new System.Windows.Forms.Label();
            this.NameTB = new System.Windows.Forms.TextBox();
            this.PasswordTB = new System.Windows.Forms.TextBox();
            this.PasswordLbl = new System.Windows.Forms.Label();
            this.RemembeMeCB = new System.Windows.Forms.CheckBox();
            this.EditHostBtn = new System.Windows.Forms.Button();
            this.HostTB = new System.Windows.Forms.TextBox();
            this.HostLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LoginBtn
            // 
            this.LoginBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LoginBtn.Location = new System.Drawing.Point(157, 116);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(75, 23);
            this.LoginBtn.TabIndex = 5;
            this.LoginBtn.Text = "&Login";
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(251, 116);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "&Cancel";
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // NameLbl
            // 
            this.NameLbl.AutoSize = true;
            this.NameLbl.Location = new System.Drawing.Point(6, 27);
            this.NameLbl.Name = "NameLbl";
            this.NameLbl.Size = new System.Drawing.Size(63, 13);
            this.NameLbl.TabIndex = 28;
            this.NameLbl.Text = "User Name:";
            // 
            // NameTB
            // 
            this.NameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTB.Location = new System.Drawing.Point(80, 25);
            this.NameTB.Name = "NameTB";
            this.NameTB.Size = new System.Drawing.Size(244, 20);
            this.NameTB.TabIndex = 0;
            // 
            // PasswordTB
            // 
            this.PasswordTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordTB.Location = new System.Drawing.Point(80, 51);
            this.PasswordTB.Name = "PasswordTB";
            this.PasswordTB.Size = new System.Drawing.Size(244, 20);
            this.PasswordTB.TabIndex = 1;
            this.PasswordTB.UseSystemPasswordChar = true;
            // 
            // PasswordLbl
            // 
            this.PasswordLbl.AutoSize = true;
            this.PasswordLbl.Location = new System.Drawing.Point(6, 53);
            this.PasswordLbl.Name = "PasswordLbl";
            this.PasswordLbl.Size = new System.Drawing.Size(56, 13);
            this.PasswordLbl.TabIndex = 30;
            this.PasswordLbl.Text = "Password:";
            // 
            // RemembeMeCB
            // 
            this.RemembeMeCB.AutoSize = true;
            this.RemembeMeCB.Location = new System.Drawing.Point(6, 120);
            this.RemembeMeCB.Name = "RemembeMeCB";
            this.RemembeMeCB.Size = new System.Drawing.Size(95, 17);
            this.RemembeMeCB.TabIndex = 4;
            this.RemembeMeCB.Text = "Remember Me";
            this.RemembeMeCB.UseVisualStyleBackColor = true;
            // 
            // EditHostBtn
            // 
            this.EditHostBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditHostBtn.Location = new System.Drawing.Point(249, 77);
            this.EditHostBtn.Name = "EditHostBtn";
            this.EditHostBtn.Size = new System.Drawing.Size(75, 23);
            this.EditHostBtn.TabIndex = 3;
            this.EditHostBtn.Text = "Edit...";
            this.EditHostBtn.Click += new System.EventHandler(this.EditHostBtn_Click);
            // 
            // HostTB
            // 
            this.HostTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HostTB.Location = new System.Drawing.Point(80, 77);
            this.HostTB.Name = "HostTB";
            this.HostTB.ReadOnly = true;
            this.HostTB.Size = new System.Drawing.Size(152, 20);
            this.HostTB.TabIndex = 2;
            this.HostTB.TabStop = false;
            // 
            // HostLbl
            // 
            this.HostLbl.AutoSize = true;
            this.HostLbl.Location = new System.Drawing.Point(6, 79);
            this.HostLbl.Name = "HostLbl";
            this.HostLbl.Size = new System.Drawing.Size(32, 13);
            this.HostLbl.TabIndex = 33;
            this.HostLbl.Text = "Host:";
            // 
            // GXAmiLoginForm
            // 
            this.AcceptButton = this.LoginBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(338, 151);
            this.Controls.Add(this.HostTB);
            this.Controls.Add(this.HostLbl);
            this.Controls.Add(this.EditHostBtn);
            this.Controls.Add(this.RemembeMeCB);
            this.Controls.Add(this.PasswordTB);
            this.Controls.Add(this.PasswordLbl);
            this.Controls.Add(this.NameTB);
            this.Controls.Add(this.NameLbl);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.LoginBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GXAmiLoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GuruxAMI Login";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label NameLbl;
        private System.Windows.Forms.TextBox NameTB;
        private System.Windows.Forms.TextBox PasswordTB;
        private System.Windows.Forms.Label PasswordLbl;
        private System.Windows.Forms.CheckBox RemembeMeCB;
        private System.Windows.Forms.Button EditHostBtn;
        private System.Windows.Forms.TextBox HostTB;
        private System.Windows.Forms.Label HostLbl;
    }
}