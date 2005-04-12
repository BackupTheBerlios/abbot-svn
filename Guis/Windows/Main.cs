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

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
#endregion

namespace AbbotForWindows {
	partial class frmMain : Form {

		delegate void EmptyDelegate();
		Abbot.Abbot bot;
		System.IO.StringWriter writer;
		System.Threading.Thread thread;

		public frmMain() {
			InitializeComponent();
		}

		private void icoIcon_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				if (Visible)
					Hide();
				else {
					Show();
					Activate();
					txtLog.SelectionStart = txtLog.Text.Length;
					txtLog.ScrollToCaret();
				}
			}
		}

		private void btnClose_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void frmMain_Load(object sender, EventArgs e) {
			writer = new System.IO.StringWriter();
			Console.SetOut(writer);

			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
			FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
			txtLog.TextChanged += new EventHandler(txtLog_TextChanged);
			txtMessage.KeyDown+=new KeyEventHandler(txtMessage_KeyDown);

			bot = new Abbot.Abbot();
			bot.ConnectAll();

			thread = new System.Threading.Thread(new System.Threading.ThreadStart(Log));
			thread.Start();
		}

		void Log() {
			while (true) {
				System.Threading.Thread.Sleep(1000);
				UpdateLog();
			}
		}

		void UpdateLog() {
			if (InvokeRequired) {
				BeginInvoke(new EmptyDelegate(UpdateLog), new object[] { });
				return;
			}
			string s = writer.ToString();
			if (s.Length > 10000)
				s = s.Substring(s.Length - 10000);
			txtLog.Text = s;
		}


		void Application_ApplicationExit(object sender, EventArgs e) {
			thread.Abort();
			bot.ShutDownBot();
			Console.Out.Close();
		}


		void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
			this.Visible = false;
		}

		void txtLog_TextChanged(object sender, EventArgs e) {
			txtLog.SelectionStart = txtLog.Text.Length;
			txtLog.ScrollToCaret();
			this.Focus();
		}

		void txtMessage_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				bot.Write(bot.Servers[0].Network, txtMessage.Text);
				txtMessage.Text = "";
			}
		}

		private void optionsToolStripMenuItemOptions_Click(object sender, EventArgs e)
		{
			( new Options() ).ShowDialog( this );
		}
	}
}