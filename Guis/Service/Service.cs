using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace Abbot {
	public partial class Service : ServiceBase {
		public Service() {
			InitializeComponent();
		}

		Bot bot;

		protected override void OnStart(string[] args) {
			bot = new Bot();
			bot.ConnectAll();
		}

		protected override void OnStop() {
			bot.DisconnectAll();
		}
	}
}
