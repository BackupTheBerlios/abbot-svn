namespace Abbot {
	partial class frmMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.MainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.SystrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.txtLog = new System.Windows.Forms.RichTextBox();
			this.MainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtMessage
			// 
			this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtMessage.Location = new System.Drawing.Point(0, 304);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(732, 20);
			this.txtMessage.TabIndex = 0;
			// 
			// MainMenu
			// 
			this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOptions});
			this.MainMenu.Location = new System.Drawing.Point(0, 0);
			this.MainMenu.Name = "MainMenu";
			this.MainMenu.Size = new System.Drawing.Size(732, 24);
			this.MainMenu.TabIndex = 3;
			this.MainMenu.Text = "mnuMain";
			// 
			// mnuOptions
			// 
			this.mnuOptions.Name = "mnuOptions";
			this.mnuOptions.Text = "&Options";
			this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
			// 
			// SystrayIcon
			// 
			this.SystrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("SystrayIcon.Icon")));
			this.SystrayIcon.Visible = true;
			this.SystrayIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SystrayIcon_MouseDown);
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Location = new System.Drawing.Point(0, 28);
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(732, 270);
			this.txtLog.TabIndex = 4;
			this.txtLog.Text = "";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(732, 324);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.MainMenu);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.MainMenu;
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Abbot: The petite IRC bot";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.MainMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.MenuStrip MainMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuOptions;
		private System.Windows.Forms.NotifyIcon SystrayIcon;
		private System.Windows.Forms.RichTextBox txtLog;
	}
}