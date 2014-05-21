namespace GuruxAMI.Client
{
	partial class HostForm
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
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.WebAddressTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.WebCB = new System.Windows.Forms.RadioButton();
            this.ServiceCB = new System.Windows.Forms.RadioButton();
            this.HostLbl = new System.Windows.Forms.Label();
            this.HostTB = new System.Windows.Forms.TextBox();
            this.PortLbl = new System.Windows.Forms.Label();
            this.PortTB = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(296, 163);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 7;
            this.CancelBtn.Text = "&Cancel";
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(202, 163);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 6;
            this.OKBtn.Text = "&OK";
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // WebAddressTB
            // 
            this.WebAddressTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebAddressTB.Location = new System.Drawing.Point(135, 38);
            this.WebAddressTB.Name = "WebAddressTB";
            this.WebAddressTB.Size = new System.Drawing.Size(236, 20);
            this.WebAddressTB.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Web Address:";
            // 
            // WebCB
            // 
            this.WebCB.AutoSize = true;
            this.WebCB.Checked = true;
            this.WebCB.Location = new System.Drawing.Point(13, 14);
            this.WebCB.Name = "WebCB";
            this.WebCB.Size = new System.Drawing.Size(87, 17);
            this.WebCB.TabIndex = 1;
            this.WebCB.TabStop = true;
            this.WebCB.Text = "Web Service";
            this.WebCB.UseVisualStyleBackColor = true;
            this.WebCB.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // ServiceCB
            // 
            this.ServiceCB.AutoSize = true;
            this.ServiceCB.Location = new System.Drawing.Point(13, 78);
            this.ServiceCB.Name = "ServiceCB";
            this.ServiceCB.Size = new System.Drawing.Size(61, 17);
            this.ServiceCB.TabIndex = 3;
            this.ServiceCB.Text = "Service";
            this.ServiceCB.UseVisualStyleBackColor = true;
            this.ServiceCB.CheckedChanged += new System.EventHandler(this.CheckedChanged);
            // 
            // HostLbl
            // 
            this.HostLbl.AutoSize = true;
            this.HostLbl.Location = new System.Drawing.Point(44, 100);
            this.HostLbl.Name = "HostLbl";
            this.HostLbl.Size = new System.Drawing.Size(32, 13);
            this.HostLbl.TabIndex = 13;
            this.HostLbl.Text = "Host:";
            // 
            // HostTB
            // 
            this.HostTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HostTB.Location = new System.Drawing.Point(135, 98);
            this.HostTB.Name = "HostTB";
            this.HostTB.Size = new System.Drawing.Size(236, 20);
            this.HostTB.TabIndex = 4;
            // 
            // PortLbl
            // 
            this.PortLbl.AutoSize = true;
            this.PortLbl.Location = new System.Drawing.Point(44, 126);
            this.PortLbl.Name = "PortLbl";
            this.PortLbl.Size = new System.Drawing.Size(29, 13);
            this.PortLbl.TabIndex = 15;
            this.PortLbl.Text = "Port:";
            // 
            // PortTB
            // 
            this.PortTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PortTB.Location = new System.Drawing.Point(135, 124);
            this.PortTB.Name = "PortTB";
            this.PortTB.Size = new System.Drawing.Size(236, 20);
            this.PortTB.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ServiceCB);
            this.panel1.Controls.Add(this.PortLbl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.WebCB);
            this.panel1.Controls.Add(this.HostLbl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(129, 198);
            this.panel1.TabIndex = 16;
            // 
            // HostForm
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(383, 198);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PortTB);
            this.Controls.Add(this.HostTB);
            this.Controls.Add(this.WebAddressTB);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Name = "HostForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GuruxAMI Service Settings";
            this.Load += new System.EventHandler(this.HostForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.TextBox WebAddressTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton WebCB;
        private System.Windows.Forms.RadioButton ServiceCB;
        private System.Windows.Forms.Label HostLbl;
        private System.Windows.Forms.TextBox HostTB;
        private System.Windows.Forms.Label PortLbl;
        private System.Windows.Forms.TextBox PortTB;
        private System.Windows.Forms.Panel panel1;
	}
}