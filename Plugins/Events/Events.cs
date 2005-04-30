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
	public class Events : Plugin {

		#region " Constructor/Destructor "
		List<EventInfo> eventInfos;
		public Events(Bot bot):base(bot) {
			Bot.OnJoin += new JoinEventHandler(Bot_UserJoins);
			Bot.OnChannelMessage += new IrcEventHandler(Bot_Message);

			Load();
		}
		#endregion

		#region " Events "
		void DeleteOldEvents() {
			List<EventInfo> l = new List<EventInfo>();
			foreach (EventInfo e in eventInfos)
				if (e.Date < DateTime.Now)
					l.Add(e);
			if (l.Count > 0) {
				foreach (EventInfo e in l)
					eventInfos.Remove(e);
				Save();
			}
		}

		void Remove(string nick, EventInfo e) {
			e.There.Remove(nick);
			e.MaybeThere.Remove(nick);
			e.NotThere.Remove(nick);
		}

		#region " Load/Save (Serialization) "
		public void Save() {
			eventInfos.Sort(new EventInfoComparer());

			StreamWriter f = new StreamWriter("Data\\Events.xml", false);
			new XmlSerializer(typeof(List<EventInfo>)).Serialize(f, eventInfos);
			f.Close();
		}

		public void Load() {
			try {
				FileStream f = new FileStream("Data\\Events.xml", FileMode.Open);
				eventInfos = (List<EventInfo>)new XmlSerializer(typeof(List<EventInfo>)).Deserialize(f);
				f.Close();
			} catch (Exception e) {
				Console.WriteLine("# " + e.Message);
				eventInfos = new List<EventInfo>();
			}
		}
		#endregion

		#region " EventInfoComparer "
		class EventInfoComparer : IComparer<EventInfo> {
			public int Compare(EventInfo a, EventInfo b) {
				return a.Date.CompareTo(b.Date);
			}

			public bool Equals(EventInfo a, EventInfo b) {
				return a.Equals(b);
			}

			public int GetHashCode(EventInfo a) {
				return a.GetHashCode();
			}
		}
		#endregion

		#region " EventInfo Class "
		[Serializable]
		public class EventInfo {

			public EventInfo() {
				there = new List<string>();
				maybeThere = new List<string>();
				notThere = new List<string>();
			}

			public EventInfo(DateTime date, string text) {
				this.date = date;
				this.text = text;
				there = new List<string>();
				maybeThere = new List<string>();
				notThere = new List<string>();
			}

			List<string> there;
			public List<string> There {
				get {
					return there;
				}
				set {
					there = value;
				}
			}

			List<string> maybeThere;
			public List<string> MaybeThere {
				get {
					return maybeThere;
				}
				set {
					maybeThere = value;
				}
			}

			List<string> notThere;
			public List<string> NotThere {
				get {
					return notThere;
				}
				set {
					notThere = value;
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

			string text;
			public string Text {
				get {
					return text;
				}
				set {
					text = value;
				}
			}
		}
		#endregion
		#endregion

		#region " Event Handles "
		void Bot_UserJoins(Network network,Irc.JoinEventArgs e) {
			DeleteOldEvents();
			network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "There are " + eventInfos.Count + " upcoming events.");
		}

		void Bot_Message(Network network, Irc.IrcEventArgs e) {
			Regex r;

			r = new Regex(@"^list events$");
			if (r.IsMatch(e.Data.Message)) {
				DeleteOldEvents();
				int i = 0;
				foreach (EventInfo eventinfo in eventInfos) {
					string there = "";
					foreach (string s in eventinfo.There)
						there += s + ", ";
					if (there.Length > 0)
						there = there.Substring(0, there.Length - 2);
					string maybeThere = "";
					foreach (string s in eventinfo.MaybeThere)
						maybeThere += s + ", ";
					if (maybeThere.Length > 0)
						maybeThere = maybeThere.Substring(0, maybeThere.Length - 2);
					string notThere = "";
					foreach (string s in eventinfo.NotThere)
						notThere += s + ", ";
					if (notThere.Length > 0)
						notThere = notThere.Substring(0, notThere.Length - 2);

					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "[" + i.ToString() + "] - " + eventinfo.Text + " - scheduled for " + eventinfo.Date.ToLongDateString() + " " + eventinfo.Date.ToShortTimeString());
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "there: " + there + " - maybe there: " + maybeThere + " - not there: " + notThere);
					i++;
				}
				return;
			}

			try {
				r = new Regex(@"^add event (?<day>\d*)\.(?<month>\d*)\.(?<year>\d*) (?<hour>\d*):(?<minute>\d*) (?<text>.*)$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					DateTime d = new DateTime(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), int.Parse(m.Groups["hour"].Value), int.Parse(m.Groups["minute"].Value), 0);
					eventInfos.Add(new EventInfo(d, m.Groups["text"].Value));
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "The event has been added.");
					Save();
					return;
				}

				r = new Regex(@"^add (?<nick>\w*) to \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					EventInfo eventinfo = eventInfos[int.Parse(m.Groups["event"].Value)];
					string nick = m.Groups["nick"].Value;
					if (nick.ToLower() == "me")
						nick = e.Data.Nick;
					Remove(nick, eventinfo);
					eventinfo.There.Add(nick);
					network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "You added '" + nick + "' as 'there' to event [" + int.Parse(m.Groups["event"].Value) + "].");
					Save();
					return;
				}

				r = new Regex(@"^add (?<nick>\w*) not to \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					EventInfo eventinfo = eventInfos[int.Parse(m.Groups["event"].Value)];
					string nick = m.Groups["nick"].Value;
					if (nick.ToLower() == "me")
						nick = e.Data.Nick;
					Remove(nick, eventinfo);
					eventinfo.NotThere.Add(nick);
					network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "You added '" + nick + "' as 'not there' to event [" + int.Parse(m.Groups["event"].Value) + "].");
					Save();
					return;
				}

				r = new Regex(@"^add (?<nick>\w*) maybe to \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					EventInfo eventinfo = eventInfos[int.Parse(m.Groups["event"].Value)];
					string nick = m.Groups["nick"].Value;
					if (nick.ToLower() == "me")
						nick = e.Data.Nick;
					Remove(nick, eventinfo);
					eventinfo.MaybeThere.Add(nick);
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "You added '" + nick + "' as 'maybe there' to event [" + int.Parse(m.Groups["event"].Value) + "].");
					Save();
					return;
				}

				r = new Regex(@"^remove (?<nick>\w*) from \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					string nick = m.Groups["nick"].Value;
					if (nick.ToLower() == "me")
						nick = e.Data.Nick;
					Remove(nick, eventInfos[int.Parse(m.Groups["event"].Value)]);
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "You removed '" + nick + "'.");
					Save();
					return;
				}

				r = new Regex(@"^remove event \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					eventInfos.RemoveAt(int.Parse(m.Groups["event"].Value));
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "The event has been removed.");
					Save();
					return;
				}

				r = new Regex(@"^clear event \[(?<event>\d*)\]$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					EventInfo eventinfo = eventInfos[int.Parse(m.Groups["event"].Value)];
					eventinfo.There.Clear();
					eventinfo.MaybeThere.Clear();
					eventinfo.NotThere.Clear();
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "The event has been cleared.");
					Save();
					return;
				}

				r = new Regex(@"^edit event \[(?<event>\d*)\] (?<day>\d*)\.(?<month>\d*)\.(?<year>\d*) (?<hour>\d*):(?<minute>\d*) (?<text>.*)$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);
					EventInfo eventinfo = eventInfos[int.Parse(m.Groups["event"].Value)];
					DateTime d = new DateTime(int.Parse(m.Groups["year"].Value), int.Parse(m.Groups["month"].Value), int.Parse(m.Groups["day"].Value), int.Parse(m.Groups["hour"].Value), int.Parse(m.Groups["minute"].Value), 0);
					eventinfo.Date = d;
					eventinfo.Text = m.Groups["text"].Value;
					network.SendMessage(Abbot.Irc.SendType.Notice,e.Data.Nick, "The event has been edited.");
					Save();
					return;
				}

			} catch {}

		}
		#endregion

	}
}
