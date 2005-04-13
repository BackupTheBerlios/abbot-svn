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
	public class Tell : Plugin {

		#region " Constructor/Destructor "
		List<TellInfo> tellInfos;
		public Tell(Abbot bot):base(bot) {
			Bot.Message += new MessageEventHandler(bot_Message);
			Bot.UserJoins += new UserJoinsEventHandler(Bot_UserJoins);

			Load();
		}
		#endregion

		#region " Tell "
		void Check(string network, string channel, string name) {
			List<TellInfo> tmp = new List<TellInfo>();
			foreach (TellInfo t in tellInfos)
				if (t.Target == name && t.Network == network) {
					tmp.Add(t);
					Bot.Write(network, channel, name + ", on " + t.Date.ToLongDateString() + " " + t.Date.ToLongTimeString() + " " + t.Name + " wanted to tell you '" + t.Text + "'.");
				}

			if (tmp.Count > 0) {
				foreach (TellInfo t in tmp)
					tellInfos.Remove(t);
				Save();
			}
		}


		#region " Load/Save (Serialization) "
		public void Save() {
			StreamWriter f = new StreamWriter("Data\\Tell.xml", false);
			new XmlSerializer(typeof(List<TellInfo>)).Serialize(f, tellInfos);
			f.Close();
		}

		public void Load() {
			try {
				FileStream f = new FileStream("Data\\Tell.xml", FileMode.Open);
				tellInfos = (List<TellInfo>)new XmlSerializer(typeof(List<TellInfo>)).Deserialize(f);
				f.Close();
			} catch (Exception e) {
				Console.WriteLine("# " + e.Message);
				tellInfos = new List<TellInfo>();
			}
		}
		#endregion

		#region " TellInfo Class "
		[Serializable]
		public class TellInfo {

			public TellInfo() { }

			public TellInfo(string network, string name, string target, string text) {
				this.date = DateTime.Now;
				this.network = network;
				this.name = name;
				this.text = text;
				this.target = target;
			}

			string target;
			public string Target {
				get {
					return target;
				}
				set {
					target = value;
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

			string name;
			public string Name {
				get {
					return name;
				}
				set {
					name = value;
				}
			}
		}
		#endregion
		#endregion

		#region " Event Handles "
		void bot_Message(string network, string channel, string user, string message) {
			try {
				if (message.StartsWith("tell ")) {
					message = message.Substring(message.IndexOf(" ") + 1);
					string name = message.Substring(0, message.IndexOf(" "));
					message = message.Substring(message.IndexOf(" ") + 1);
					tellInfos.Add(new TellInfo(network, GetNickFromUser(user), name, message));
					Bot.WriteNotice(network, GetNickFromUser(user), "I'll tell '" + name + "' your message.");
					Save();
				}
			} catch {
				BadSyntax(network, user);
			}
		}

		void Bot_UserJoins(string network, string channel, string user) {
			Check(network, channel, GetNickFromUser(user));
		}

		#endregion
	}
}
