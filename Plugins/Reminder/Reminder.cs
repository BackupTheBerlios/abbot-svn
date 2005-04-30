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
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
#endregion

namespace Abbot.Plugins {
	public class Reminder : Plugin, IDisposable {

		#region " Constructor/Destructor "
		List<RemindInfo> remindInfos = new List<RemindInfo>();
		Thread remindThread;
		public Reminder(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnChannelMessage);

			Load();
			remindThread = new Thread(new ThreadStart(RemindTick));
			remindThread.Start();
		}

		~Reminder() {
			Dispose();
		}

		public void Dispose() {
			remindThread.Abort();
		}
		#endregion

		#region " Remind "
		void RemindTick() {
			while (true) {
				Thread.Sleep(20000);

				DateTime start = DateTime.Now.AddSeconds(60);
				DateTime end = DateTime.Now.AddSeconds(120);
				List<RemindInfo> tmp = new List<RemindInfo>();
				foreach (RemindInfo r in remindInfos) {
					if (r.Date < start)
						tmp.Add(r);
					else if (r.Date > start && r.Date < end) {
						tmp.Add(r);
						Network n = Bot.GetNetworkByName(r.Network);
						if (n != null)
							n.SendMessage(Abbot.Irc.SendType.Message, r.Channel, r.User + ", time's up! " + r.Message);
					}
				}

				foreach (RemindInfo r in tmp)
					remindInfos.Remove(r);

				if (tmp.Count > 0)
					Save();
			}
		}

		List<RemindInfo> GetReminders(string user) {
			List<RemindInfo> l = new List<RemindInfo>();
			foreach (RemindInfo ri in remindInfos)
				if (ri.User.ToLower() == user.ToLower())
					l.Add(ri);
			return l;
		}

		#region " Load/Save (Serialization) "
		public void Save() {
			StreamWriter f = new StreamWriter("Data\\RemindMe.xml", false);
			new XmlSerializer(typeof(List<RemindInfo>)).Serialize(f, remindInfos);
			f.Close();
		}

		public void Load() {
			try {
				FileStream f = new FileStream("Data\\RemindMe.xml", FileMode.Open);
				remindInfos = (List<RemindInfo>)new XmlSerializer(typeof(List<RemindInfo>)).Deserialize(f);
				f.Close();
			} catch (Exception e) {
				Console.WriteLine("# " + e.Message);
				remindInfos = new List<RemindInfo>();
			}
		}
		#endregion

		#region " RemindInfo Class "
		[Serializable]
		public class RemindInfo {

			public RemindInfo() {
			}

			public RemindInfo(DateTime date, string network, string channel, string user, string message) {
				this.date = date;
				this.network = network;
				this.channel = channel;
				this.user = user;
				this.message = message;
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
		void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e) {
			string message = e.Data.Message;
			try {
				if (e.Data.Message.StartsWith("remind me ")) {
					message = message.Substring(10);
					string t = e.Data.Message.Substring(0, e.Data.Message.IndexOf(" "));
					message = e.Data.Message.Substring(3);
					string u = e.Data.Nick;
					string p = e.Data.Message.Substring(0, e.Data.Message.IndexOf(" "));
					string m = e.Data.Message.Substring(e.Data.Message.IndexOf(" ") + 1);
					if (t == "in") {
						int minutes = int.Parse(p);
						remindInfos.Add(new RemindInfo(DateTime.Now.AddMinutes(minutes), network.Name, e.Data.Channel, u, m));
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "You will be reminded in " + minutes.ToString() + " minutes.");
						Save();
						return;
					}
					else if (t == "at") {
						int minute = int.Parse(p.Substring(p.IndexOf(":") + 1));
						int hour = int.Parse(p.Substring(0, p.IndexOf(":")));
						DateTime d = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
						if (d < DateTime.Now)
							d.AddDays(1);
						remindInfos.Add(new RemindInfo(d, network.Name, e.Data.Channel, u, m));
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "You will be reminded at " + Format(hour) + ":" + Format(minute) + ".");
						Save();
						return;
					}
				}

				Regex r;
				r = new Regex(@"^list reminders$");
				if (r.IsMatch(e.Data.Message)) {
					Console.WriteLine("LIST REMINDERS");
					int i = 0;
					foreach (RemindInfo ri in GetReminders(e.Data.Nick))
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "[" + i.ToString() + "] - " + ri.Message + " - scheduled for " + ri.Date.ToLongDateString() + " " + ri.Date.ToShortTimeString() + ".");
					return;
				}

				r = new Regex(@"^remove reminder \[(?<reminder>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					remindInfos.Remove(GetReminders(e.Data.Nick)[int.Parse(m.Groups["reminder"].Value)]);
					network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "The reminder has been removed.");
					Save();
					return;
				}
			} catch {
			}
		}
		#endregion
	}
}
