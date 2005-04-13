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
		public Seen(Abbot bot):base(bot) {
			Bot.Message += new MessageEventHandler(bot_Message);
			Bot.UserJoins += new UserJoinsEventHandler(Bot_UserJoins);
			Bot.UserLeaves += new UserLeavesEventHandler(Bot_UserLeaves);
			Bot.UserQuits += new UserQuitsEventHandler(Bot_UserQuits);
			Bot.UserChangesNick += new UserChangesNickEventHandler(Bot_UserChangesNick);

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

		void NewSeen(string network, string user, string text) {
			string nick = GetNickFromUser(user);
			string ident = GetIdentFromUser(user);
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

			public SeenInfo() { }

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
		void bot_Message(string network, string channel, string user, string message) {
			NewSeen(network, user, "on " + channel + ", saying '" + message + "'");
			try {
				if (message.StartsWith("seen ")) {
					string name = message.Substring(message.IndexOf(" ") + 1);
					SeenInfo i = FindName(network, name);
					if (i == null) {
						Bot.WriteNotice(network, GetNickFromUser(user), "I never saw a '" + name + "' before.");
					}
					else if (i.Ident == GetIdentFromUser(user)) {
						Bot.WriteNotice(network, GetNickFromUser(user), "Looking for yourself?");
					}
					else {
						TimeSpan t = (TimeSpan)(DateTime.Now - i.Date);
						Bot.WriteNotice(network, GetNickFromUser(user), "I saw " + name + " " + Convert.ToInt16(t.TotalHours).ToString() + " hours, " + t.Minutes.ToString() + " minutes and " + t.Seconds.ToString() + " seconds ago, " + i.Text + ".");
					}
				}
			} catch {
				BadSyntax(network, user);
			}
		}

		void Bot_UserJoins(string network, string channel, string user) {
			NewSeen(network, user, "joining " + channel);
		}

		void Bot_UserLeaves(string network, string channel, string user, string message) {
			NewSeen(network, user, "leaving " + channel);
		}

		void Bot_UserQuits(string network, string user, string message) {
			NewSeen(network, user, "quitting IRC (" + message + ")");
		}

		void Bot_UserChangesNick(string network, string user, string newNick) {
			NewSeen(network, user, "changing his nick from " + GetNickFromUser(user) + " to " + newNick);
		}
		#endregion

	}
}
