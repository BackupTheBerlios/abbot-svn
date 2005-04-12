/*
Abbot: The petite IRC bot
Copyright (C) 2005 Hannes Sachsenhofer

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

namespace AbbotForWindows {
	partial class frmMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( frmMain ) );
			this.txtLog = new System.Windows.Forms.TextBox();
			this.icoIcon = new System.Windows.Forms.NotifyIcon( this.components );
			this.mnuSysMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.btnClose = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.leftRaftingContainer = new System.Windows.Forms.RaftingContainer();
			this.rightRaftingContainer = new System.Windows.Forms.RaftingContainer();
			this.topRaftingContainer = new System.Windows.Forms.RaftingContainer();
			this.bottomRaftingContainer = new System.Windows.Forms.RaftingContainer();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.mnuSysMenu.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize) ( this.leftRaftingContainer ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize) ( this.rightRaftingContainer ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize) ( this.topRaftingContainer ) ).BeginInit();
			( (System.ComponentModel.ISupportInitialize) ( this.bottomRaftingContainer ) ).BeginInit();
			this.SuspendLayout();
// 
// txtLog
// 
			this.txtLog.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.txtLog.AutoSize = false;
			this.txtLog.BackColor = System.Drawing.Color.White;
			this.txtLog.Font = new System.Drawing.Font( "Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
			this.txtLog.Location = new System.Drawing.Point( 0, 25 );
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size( 751, 425 );
			this.txtLog.TabIndex = 0;
			this.txtLog.WordWrap = false;
// 
// icoIcon
// 
			this.icoIcon.ContextMenuStrip = this.mnuSysMenu;
			this.icoIcon.Text = "Abbot";
			this.icoIcon.Visible = true;
			this.icoIcon.MouseDown += new System.Windows.Forms.MouseEventHandler( this.icoIcon_MouseDown );
// 
// mnuSysMenu
// 
			this.mnuSysMenu.AllowDrop = true;
			this.mnuSysMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.btnClose,
            this.optionsToolStripMenuItem,
            this.optionsToolStripMenuItemOptions} );
			this.mnuSysMenu.Location = new System.Drawing.Point( 23, 54 );
			this.mnuSysMenu.Name = "mnuMenu";
			this.mnuSysMenu.Size = new System.Drawing.Size( 102, 67 );
// 
// btnClose
// 
			this.btnClose.Name = "btnClose";
			this.btnClose.SettingsKey = "frmMain.toolStripMenuItem2";
			this.btnClose.Text = "Close";
			this.btnClose.Click += new System.EventHandler( this.btnClose_Click );
// 
// optionsToolStripMenuItem
// 
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.SettingsKey = "frmMain.optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Text = "Options";
// 
// optionsToolStripMenuItemOptions
// 
			this.optionsToolStripMenuItemOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.optionsToolStripMenuItemOptions.Name = "optionsToolStripMenuItemOptions";
			this.optionsToolStripMenuItemOptions.SettingsKey = "frmMain.optionsToolStripMenuItemOptions";
			this.optionsToolStripMenuItemOptions.Text = "Options";
			this.optionsToolStripMenuItemOptions.Click += new System.EventHandler( this.optionsToolStripMenuItemOptions_Click );
// 
// leftRaftingContainer
// 
			this.leftRaftingContainer.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftRaftingContainer.Name = "leftRaftingContainer";
// 
// rightRaftingContainer
// 
			this.rightRaftingContainer.Dock = System.Windows.Forms.DockStyle.Right;
			this.rightRaftingContainer.Name = "rightRaftingContainer";
// 
// topRaftingContainer
// 
			this.topRaftingContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.topRaftingContainer.Name = "topRaftingContainer";
// 
// bottomRaftingContainer
// 
			this.bottomRaftingContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomRaftingContainer.Name = "bottomRaftingContainer";
// 
// txtMessage
// 
			this.txtMessage.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.txtMessage.AutoSize = false;
			this.txtMessage.Location = new System.Drawing.Point( 0, 2 );
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size( 750, 20 );
			this.txtMessage.TabIndex = 6;
// 
// frmMain
// 
			this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
			this.ClientSize = new System.Drawing.Size( 751, 450 );
			this.Controls.Add( this.txtMessage );
			this.Controls.Add( this.txtLog );
			this.Controls.Add( this.leftRaftingContainer );
			this.Controls.Add( this.rightRaftingContainer );
			this.Controls.Add( this.topRaftingContainer );
			this.Controls.Add( this.bottomRaftingContainer );
			this.Icon = ( (System.Drawing.Icon) ( resources.GetObject( "$this.Icon" ) ) );
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Abbot: The petite IRC bot";
			this.Load += new System.EventHandler( this.frmMain_Load );
			this.mnuSysMenu.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize) ( this.leftRaftingContainer ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize) ( this.rightRaftingContainer ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize) ( this.topRaftingContainer ) ).EndInit();
			( (System.ComponentModel.ISupportInitialize) ( this.bottomRaftingContainer ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.NotifyIcon icoIcon;
		private System.Windows.Forms.RaftingContainer leftRaftingContainer;
		private System.Windows.Forms.RaftingContainer rightRaftingContainer;
		private System.Windows.Forms.RaftingContainer topRaftingContainer;
		private System.Windows.Forms.RaftingContainer bottomRaftingContainer;
		private System.Windows.Forms.ContextMenuStrip mnuSysMenu;
		private System.Windows.Forms.ToolStripMenuItem btnClose;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItemOptions;
	}
}

