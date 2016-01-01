/*
Reminder Plugin for the Abbot IRC Bot [http://abbot.berlios.de]
Copyright (C) 2005 Hannes Sachsenhofer [http://www.sachsenhofer.com]

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
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Meebey.SmartIrc4net;
#endregion

namespace Abbot.Plugins {
	public class Reminder : Plugin, IDisposable {

		#region " Constructor/Destructor "
		Thread t;
		public Reminder(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);
		}

		~Reminder() {
			Dispose();
		}

		public void Dispose() {
			t.Abort();
		}
		#endregion

		#region " Remind "
		void StartThread() {
			if (t == null || t.ThreadState != ThreadState.Running) {
				t = new Thread(new ThreadStart(DoRemind));
				t.Start();
			}
		}

		void DoRemind() {
			List<RemindInfo> l = LoadFromFile<List<RemindInfo>>("Reminders");
			while (l.Count > 0) {
				List<RemindInfo> tmp = new List<RemindInfo>();
				foreach (RemindInfo i in l) {
					if (i.Date < DateTime.Now) {
						Network network = Bot.GetNetworkByName(i.Network);
						if (network != null) {
							if (i.IsPrivate)
								network.SendMessage(SendType.Message, i.User, i.User + ", time's up! " + i.Message);
							else
								network.SendMessage(SendType.Message, i.Channel, i.User + ", time's up! " + i.Message);
						}
						tmp.Add(i);
					}
				}
				foreach (RemindInfo i in tmp)
					l.Remove(i);
				if (tmp.Count > 0)
					SaveToFile<List<RemindInfo>>(l,"Reminders");
				Thread.Sleep(10000);
			}
		}

		#region " RemindInfo Class "
		[Serializable]
		public class RemindInfo {

			public RemindInfo() {
			}

			bool isPrivate;
			public bool IsPrivate {
				get {
					return isPrivate;
				}
				set {
					isPrivate = value;
				}
			} 

			DateTime date;
			public DateTime Date {
				get {
					return date;
				}
				set {
					date = value;
				}
			}

			string network;
			public string Network {
				get {
					return network;
				}
				set {
					network = value;
				}
			}

			string message;
			public string Message {
				get {
					return message;
				}
				set {
					message = value;
				}
			}

			string user;
			public string User {
				get {
					return user;
				}
				set {
					user = value;
				}
			}

			string channel;
			public string Channel {
				get {
					return channel;
				}
				set {
					channel = value;
				}
			}
		}
		#endregion
		#endregion

		#region " Event Handles "
		void Bot_OnMessage(object n, IrcEventArgs e) {
			var network = (Network)n;

			if (IsMatch("^reminder \\?$", e.Data.Message)) {
				AnswerWithNotice(network, e, FormatBold("Use of Reminder plugin:"));
				AnswerWithNotice(network, e, FormatItalic("remind me in <minutes> <message>") + " - Reminds you in <minutes> minutes.");
				AnswerWithNotice(network, e, FormatItalic("remind me at <hours>:<minutes> <message>") + " - Reminds you at the given time.");
			}
			else if (IsMatch("^remind me in (?<minutes>\\d{1,3}) (?<message>.*)$", e.Data.Message)) {
				List<RemindInfo> l = LoadFromFile<List<RemindInfo>>("Reminders");
				RemindInfo i = new RemindInfo();
				i.Network = network.Name;
				i.Channel = e.Data.Channel;
				i.User = e.Data.Nick;
				i.Message = Matches["message"].ToString();
				i.Date = DateTime.Now.AddMinutes(int.Parse(Matches["minutes"].ToString()));
				i.IsPrivate = e.Data.Type == ReceiveType.QueryMessage;
				l.Add(i);
				SaveToFile<List<RemindInfo>>(l, "Reminders");
				StartThread();
				AnswerWithNotice(network, e, "You will be reminded.");
			}
			else if (IsMatch("^remind me at (?<hours>\\d{1,2}):(?<minutes>\\d{1,2}) (?<message>.*)$",e.Data.Message)) {
				List<RemindInfo> l = LoadFromFile<List<RemindInfo>>("Reminders");
				RemindInfo i = new RemindInfo();
				i.Network = network.Name;
				i.Channel = e.Data.Channel;
				i.User = e.Data.Nick;
				i.Message = Matches["message"].ToString();
				i.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(Matches["hours"].ToString()), int.Parse(Matches["minutes"].ToString()), 0);
				if (i.Date < DateTime.Now)
					i.Date = i.Date.AddDays(1);
				i.IsPrivate = e.Data.Type == ReceiveType.QueryMessage;
				l.Add(i);
				SaveToFile<List<RemindInfo>>(l, "Reminders");
				StartThread();
				AnswerWithNotice(network, e, "You will be reminded.");
			}
		}
		#endregion
	}
}
