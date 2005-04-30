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
#endregion

namespace Abbot.Plugins {
	public class Seen : Plugin {

		#region " Constructor/Destructor "
		List<SeenInfo> seenInfos;
		public Seen(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnChannelMessage);
			Bot.OnJoin += new JoinEventHandler(Bot_OnJoin);
			Bot.OnPart += new PartEventHandler(Bot_OnPart);
			Bot.OnQuit += new QuitEventHandler(Bot_OnQuit);
			Bot.OnNickChange += new NickChangeEventHandler(Bot_OnNickChange);

			Load();
		}
		#endregion

		#region " Seen "
		SeenInfo FindName(string network, string name) {
			foreach (SeenInfo i in seenInfos)
				if (i.Network == network && i.Names.Contains(name))
					return i;
			return null;
		}

		SeenInfo FindIdent(string network, string ident) {
			foreach (SeenInfo i in seenInfos)
				if (i.Ident == ident && i.Network == network)
					return i;
			return null;
		}

		void NewSeen(string network, string nick, string ident, string text) {
			SeenInfo i = FindIdent(network, ident);
			if (i == null) {
				i = new SeenInfo(network, ident, text);
				seenInfos.Add(i);
			}
			else {
				i.Date = DateTime.Now;
				i.Text = text;
			}

			if (!i.Names.Contains(nick))
				i.Names.Add(nick);

			Save();
		}

		#region " Load/Save (Serialization) "
		public void Save() {
			StreamWriter f = new StreamWriter("Data\\Seen.xml", false);
			new XmlSerializer(typeof(List<SeenInfo>)).Serialize(f, seenInfos);
			f.Close();
		}

		public void Load() {
			try {
				FileStream f = new FileStream("Data\\Seen.xml", FileMode.Open);
				seenInfos = (List<SeenInfo>)new XmlSerializer(typeof(List<SeenInfo>)).Deserialize(f);
				f.Close();
			} catch (Exception e) {
				Console.WriteLine("# " + e.Message);
				seenInfos = new List<SeenInfo>();
			}
		}
		#endregion

		#region " SeenInfo Class "
		[Serializable]
		public class SeenInfo {

			public SeenInfo() {
			}

			public SeenInfo(string network, string ident, string text) {
				this.date = DateTime.Now;
				this.network = network;
				this.ident = ident;
				this.text = text;
			}

			List<string> names = new List<string>();
			public List<string> Names {
				get {
					return names;
				}
				set {
					names = value;
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

			string ident;
			public string Ident {
				get {
					return ident;
				}
				set {
					ident = value;
				}
			}
		}
		#endregion
		#endregion

		#region " Event Handles "
		void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e) {
			NewSeen(network.Name, e.Data.Nick, e.Data.Ident, "on " + e.Data.Channel + ", saying '" + e.Data.Message + "'");
			try {
				if (e.Data.Message.StartsWith("seen ")) {
					string name = e.Data.Message.Substring(e.Data.Message.IndexOf(" ") + 1);
					SeenInfo i = FindName(network.Name, name);
					if (i == null) {
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "I never saw a '" + name + "' before.");
					}
					else if (i.Ident == e.Data.Ident) {
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "Looking for yourself?");
					}
					else {
						TimeSpan t = (TimeSpan)(DateTime.Now - i.Date);
						network.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, "I saw " + name + " " + Convert.ToInt16(t.TotalHours).ToString() + " hours, " + t.Minutes.ToString() + " minutes and " + t.Seconds.ToString() + " seconds ago, " + i.Text + ".");
					}
				}
			} catch {
			}
		}

		void Bot_OnJoin(Network network, Irc.JoinEventArgs e) {
			NewSeen(network.Name, e.Data.Nick, e.Data.Ident, "joining " + e.Data.Channel);
		}

		void Bot_OnPart(Network network, Irc.PartEventArgs e) {
			NewSeen(network.Name, e.Data.Nick, e.Data.Ident, "leaving " + e.Data.Channel);
		}

		void Bot_OnQuit(Network network, Irc.QuitEventArgs e) {
			NewSeen(network.Name, e.Data.Nick, e.Data.Ident, "quitting IRC (" + e.Data.Message + ")");
		}

		void Bot_OnNickChange(Network network, Irc.NickChangeEventArgs e) {
			NewSeen(network.Name, e.Data.Nick, e.Data.Ident, "changing his nick from " + e.OldNickname + " to " + e.NewNickname);
		}
		#endregion

	}
}
