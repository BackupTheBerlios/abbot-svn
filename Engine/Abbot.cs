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
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
#endregion

namespace Abbot {
	public class Abbot {

		public event UserLeavesEventHandler UserLeaves;
		public event UserQuitsEventHandler UserQuits;
		public event UserJoinsEventHandler UserJoins;
		public event NoticeEventHandler Notice;
		public event MessageEventHandler Message;
		public event UnknownMessageEventHandler UnknownMessage;
		public event ModeChangeEventHandler ModeChange;
		public event UserOppedEventHandler UserOpped;
		public event UserDeoppedEventHandler UserDeopped;
		public event ConnectEventHandler Connect;
		public event DisconnectEventHandler Disconnect;
		public event JoinEventHandler Join;
		public event GenericMessageEventHandler GenericMessage;
		public event UserChangesNickEventHandler UserChangesNick;
		public event ShutdownEventHandler ShutDown;

		#region " Constructor/Destructor "
		public Abbot() {
			Console.WriteLine("Abbot: The petite IRC Bot   <http://abbot.berlios.de>");
			Console.WriteLine("Copyright (C) 2005 Hannes Sachsenhofer");
			Console.WriteLine();
			Console.WriteLine("Abbot comes with absolutely no warranty.");
			Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions.");
			Console.WriteLine("See the enclosed copy of the GPL (General Public License) for details.");
			Console.WriteLine("=======================================================================================");
			Console.WriteLine();
			Console.WriteLine("Initializing ...");
			Console.WriteLine();

			#region " Load Configuration "
			Console.WriteLine("Loading configuration ...");
			configuration = new XmlDocument();
			configuration.Load("Configuration.xml");
			nick = Configuration["Nick"].InnerText;

			foreach (XmlElement e in Configuration.ChildNodes)
				if (e.Name == "Server") {
					Server s = new Server(this,e["Network"].InnerText, e["Name"].InnerText, e["Address"].InnerText, int.Parse(e["Port"].InnerText));
					foreach (XmlElement f in e.ChildNodes)
						if (f.Name == "Channel")
							s.Channels.Add(new Channel(f["Name"].InnerText, f["Password"].InnerText));
					AddServer(s);
				}
			#endregion

			#region " Load Plugins "
			object[] o ={ this };
			foreach (System.IO.FileInfo f in new System.IO.DirectoryInfo("Plugins").GetFiles())
				if (f.Extension == ".dll") {
					Console.WriteLine("Loading Plugins from Assembly '" + f.Name + "' ...");
					Assembly a = System.Reflection.Assembly.LoadFile(f.FullName);
					foreach (Type t in a.GetTypes())
						if (t.BaseType == typeof(Plugin)) {
							Plugin p = (Plugin)Activator.CreateInstance(t, o);
							plugins.Add(p);
						}
				}
			Console.WriteLine();
			#endregion
		}

		~Abbot() {
			ShutDownBot();
			SaveConfiguration();
		}
		#endregion

		#region " Properties "
		List<Server> servers = new List<Server>();
		public List<Server> Servers {
			get {
				return servers;
			}
		}

		XmlDocument configuration;
		public XmlElement Configuration {
			get {
				return configuration["Abbot"];
			}
		}

		List<Plugin> plugins = new List<Plugin>();
		public List<Plugin> Plugins {
			get {
				return plugins;
			}
		}

		string nick;
		public string Nick {
			get {
				return nick;
			}
		}
		#endregion

		#region " Methods "
		public void Write(string network, string message) {
			foreach (Server s in servers)
				if (s.Network == network)
					s.Write(message);
		}

		public void Write(string network, string channel, string message) {
			Write(network, "PRIVMSG " + channel + " :" + message);
		}

		public void WriteNotice(string network, string user, string message) {
			Write(network, "NOTICE " + user + " :" + message);
		}

		public void ConnectAll() {
			foreach (Server s in servers)
				s.Connect();
		}

		public void DisconnectAll() {
			foreach (Server s in servers)
				s.Disconnect();
		}

		public void ShutDownBot() {
			DisconnectAll();
			if (ShutDown != null)
				ShutDown();
		}

		void AddServer(Server s) {
			if (!servers.Contains(s)) {
				servers.Add(s);
				s.OnConnect += new ConnectEventHandler(HandleConnect);
				s.OnJoin += new JoinEventHandler(HandleJoin);
				s.OnDisconnect += new DisconnectEventHandler(HandleDisconnect);
				s.GenericMessage += new GenericMessageEventHandler(HandleGenericMessage);
			}
		}

		public void SaveConfiguration() {
			configuration.Save("Configuration.xml");
		}
#endregion

		#region " Static methods "
		public static string GetReturnCode(string s) {
			Regex r = new Regex(@".*? (?<Code>\d\d\d) .*");
			if (r.IsMatch(s))
				return r.Match(s).Groups["Code"].Value;
			throw new ArgumentException("Unable to find return code.");
		}

		internal static string GetChannel(string text) {
			return text.Substring(0, text.IndexOf(" "));
		}


		internal static string GetMode(string text) {
			return text.Substring(text.IndexOf(" ") + 1);
		}


		internal static string GetMessage(string text) {
			return text.Substring(text.IndexOf(" :") + 2);
		}
		#endregion

		#region " Handle events "
		void HandleConnect(string network) {
			if (Connect != null)
				Connect(network);
		}

		void HandleJoin(string network, string channel) {
			if (Join != null)
				Join(network, channel);
		}

		void HandleDisconnect(string network) {
			if (Disconnect != null)
				Disconnect(network);
		}

		void HandleGenericMessage(string network, string message) {
			Console.WriteLine(message);

			if (message.StartsWith("PING")) {
				Write(network, "PONG " + message.Substring(message.IndexOf(":") + 1));
				return;
			}

			if (GenericMessage != null)
				GenericMessage(network, message);

			Regex r = new Regex(":(?<user>.*?!~.*?@.*?) (?<command>\\w*?) (?<text>.*)");
			if (r.IsMatch(message)) {
				Match m = r.Match(message);
				#region " Message "
				if (m.Groups["command"].Value == "PRIVMSG") {
					if (Message != null)
						Message(network, GetChannel(m.Groups["text"].Value), m.Groups["user"].Value, GetMessage(m.Groups["text"].Value));
				}
				#endregion
				#region " Notice "
				else if (m.Groups["command"].Value == "NOTICE") {
					if (Notice != null)
						Notice(network, m.Groups["user"].Value, GetMessage(m.Groups["text"].Value));
				}
				#endregion
				#region " UserLeaves "
				else if (m.Groups["command"].Value == "PART") {
					if (UserLeaves != null)
						UserLeaves(network, GetChannel(m.Groups["text"].Value), m.Groups["user"].Value, GetMessage(m.Groups["text"].Value));
				}
				#endregion
				#region " UserQuits "
				else if (m.Groups["command"].Value == "QUIT") {
					if (UserQuits != null)
						UserQuits(network, m.Groups["user"].Value, GetMessage(m.Groups["text"].Value));
				}
				#endregion
				#region " UserJoins "
				else if (m.Groups["command"].Value == "JOIN") {
					if (UserJoins != null)
						UserJoins(network, GetMessage(m.Groups["text"].Value), m.Groups["user"].Value);
				}
				#endregion
				#region " ModeChange "
				else if (m.Groups["command"].Value == "MODE") {
					string channel = GetChannel(m.Groups["text"].Value);
					string user = m.Groups["user"].Value;
					string mode = GetMode(m.Groups["text"].Value);
					if (ModeChange != null)
						ModeChange(network, channel, user, mode);
					if (mode.IndexOf(" ") >= 0) {
						string target = mode.Substring(mode.IndexOf(" ") + 1);
						target = target.Substring(0, target.Length - 1);
						mode = mode.Substring(0, mode.IndexOf(" "));
						if (mode.Contains("o"))
							if (UserOpped != null && mode.StartsWith("+"))
								UserOpped(network, channel, user, target);
							else if (UserDeopped != null && mode.StartsWith("-"))
								UserDeopped(network, channel, user, target);
					}
				}
				#endregion
				#region " UserChangesNick "
				else if (m.Groups["command"].Value == "NICK") {
					if (UserChangesNick != null)
						UserChangesNick(network, m.Groups["user"].Value, m.Groups["text"].Value.Substring(1));
				}
				#endregion
				#region " UnknownMessage "
				else {
					if (UnknownMessage != null)
						UnknownMessage(network, message);
				}
				#endregion
			}
			else {
				if (UnknownMessage != null)
					UnknownMessage(network, message);
			}
		}
		#endregion
	}

	public delegate void UserLeavesEventHandler(string network, string channel, string user, string message);
	public delegate void UserQuitsEventHandler(string network, string user, string message);
	public delegate void UserJoinsEventHandler(string network, string channel, string user);
	public delegate void NoticeEventHandler(string network, string user, string message);
	public delegate void MessageEventHandler(string network, string channel, string user, string message);
	public delegate void UnknownMessageEventHandler(string network, string mesage);
	public delegate void ModeChangeEventHandler(string network, string channel, string user, string mode);
	public delegate void UserOppedEventHandler(string network, string channel, string user, string target);
	public delegate void UserDeoppedEventHandler(string network, string channel, string user, string target);
	public delegate void ConnectEventHandler(string network);
	public delegate void DisconnectEventHandler(string network);
	public delegate void JoinEventHandler(string network, string channel);
	public delegate void GenericMessageEventHandler(string network, string message);
	public delegate void UserChangesNickEventHandler(string network, string user, string newNick);
	public delegate void ShutdownEventHandler();

}
