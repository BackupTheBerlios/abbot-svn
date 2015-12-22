/*
Abbot: The petite IRC bot
Copyright (C) 2005 The Abbot Project

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

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using Meebey.SmartIrc4net;

namespace Abbot {
	public class Bot : IDisposable {

		#region " Constructor/Destructor/Dispose "
		public Bot() {
			#region " Header "
			Console.WriteLine("Abbot: The petite IRC Bot  - v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " - [http://abbot.berlios.de]");
			Console.WriteLine("(c) 2005 The Abbot Project");
			Console.WriteLine("===============================================================================");
			Console.WriteLine("Abbot: The petite IRC Bot comes with absolutely no warranty.");
			Console.WriteLine("This is free software, and you are welcome to redistribute it under certain");
			Console.WriteLine("conditions. See the enclosed copy of the General Public License for details.");
			Console.WriteLine("===============================================================================");
			#endregion

			#region " Load Configuration "
			configuration = new XmlDocument();
			try {
				configuration.Load("Configuration.xml");
			} catch (Exception e) {
				Console.WriteLine("# Cannot load the configuration file: " + e.Message);
				Environment.Exit(-1);
				return;
			}

			foreach (XmlElement e in configuration.GetElementsByTagName("Network")) {
				Network n = new Network();
				networks.Add(n);
				n.Name = e.Attributes["Name"].Value;
				n.Nickname = e.Attributes["Nickname"].Value;
				n.Realname = e.Attributes["Realname"].Value;
				n.Username = e.Attributes["Username"].Value;
				if (e.HasAttribute("Password")) {
					n.UsePassword = true;
					n.Password = e.Attributes["Password"].Value;
				}
				else
					n.UsePassword = false;
				n.Port = int.Parse(e.Attributes["Port"].Value);
				n.SendDelay = int.Parse(e.Attributes["SendDelay"].Value);

				foreach (XmlElement f in e.GetElementsByTagName("Server"))
					n.Servers.Add(f.Attributes["Address"].Value);

				foreach (XmlElement f in e.GetElementsByTagName("Channel"))
					n.Channels.Add(f.Attributes["Name"].Value);


				n.OnBan += new BanEventHandler(OnBanHandler);
				n.OnChannelAction += new ActionEventHandler(OnChannelActionHandler);
				n.OnChannelActiveSynced += new IrcEventHandler(OnChannelActiveSyncedHandler);
				n.OnChannelMessage += new IrcEventHandler(OnChannelMessageHandler);
				n.OnChannelModeChange += new EventHandler<ChannelModeChangeEventArgs>(OnChannelModeChangeHandler);
				n.OnChannelNotice += new IrcEventHandler(OnChannelNoticeHandler);
				n.OnChannelPassiveSynced += new IrcEventHandler(OnChannelPassiveSyncedHandler);
				n.OnConnected += new EventHandler(OnConnectedHandler);
				n.OnConnecting += new EventHandler(OnConnectingHandler);
				n.OnConnectionError += new EventHandler(OnConnectionErrorHandler);
				n.OnCtcpReply += new CtcpEventHandler(OnCtcpReplyHandler);
				n.OnCtcpRequest += new CtcpEventHandler(OnCtcpRequestHandler);
				n.OnDehalfop += new DehalfopEventHandler(OnDehalfopHandler);
				n.OnDeop += new DeopEventHandler(OnDeopHandler);
				n.OnDevoice += new DevoiceEventHandler(OnDevoiceHandler);
				n.OnDisconnected += new EventHandler(OnDisconnectedHandler);
				n.OnDisconnecting += new EventHandler(OnDisconnectingHandler);
				n.OnError += new ErrorEventHandler(OnErrorHandler);
				n.OnErrorMessage += new IrcEventHandler(OnErrorMessageHandler);
				n.OnHalfop += new HalfopEventHandler(OnHalfopHandler);
				n.OnInvite += new InviteEventHandler(OnInviteHandler);
				n.OnJoin += new JoinEventHandler(OnJoinHandler);
				n.OnKick += new KickEventHandler(OnKickHandler);
				n.OnModeChange += new IrcEventHandler(OnModeChangeHandler);
				n.OnMotd += new MotdEventHandler(OnMotdHandler);
				n.OnNames += new NamesEventHandler(OnNamesHandler);
				n.OnNickChange += new NickChangeEventHandler(OnNickChangeHandler);
				n.OnOp += new OpEventHandler(OnOpHandler);
				n.OnPart += new PartEventHandler(OnPartHandler);
				n.OnPing += new PingEventHandler(OnPingHandler);
				n.OnQueryAction += new ActionEventHandler(OnQueryActionHandler);
				n.OnQueryMessage += new IrcEventHandler(OnQueryMessageHandler);
				n.OnQueryNotice += new IrcEventHandler(OnQueryNoticeHandler);
				n.OnQuit += new QuitEventHandler(OnQuitHandler);
				n.OnRawMessage += new IrcEventHandler(OnRawMessageHandler);
				n.OnReadLine += new ReadLineEventHandler(OnReadLineHandler);
				n.OnRegistered += new EventHandler(OnRegisteredHandler);
				n.OnTopic += new TopicEventHandler(OnTopicHandler);
				n.OnTopicChange += new TopicChangeEventHandler(OnTopicChangeHandler);
				n.OnUnban += new UnbanEventHandler(OnUnbanHandler);
				n.OnUserModeChange += new IrcEventHandler(OnUserModeChangeHandler);
				n.OnVoice += new VoiceEventHandler(OnVoiceHandler);
				n.OnWho += new WhoEventHandler(OnWhoHandler);
				n.OnWriteLine += new WriteLineEventHandler(OnWriteLineHandler);
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
							Console.WriteLine("\t\t- " + t.Name);
							try
							{
							Plugin p = (Plugin)Activator.CreateInstance(t, o);
							plugins.Add(p);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.ToString());
							}
						}
				}
			Console.WriteLine("===============================================================================");
			#endregion
		}

		~Bot() {
			Dispose();
		}

		public void Dispose() {
			DisconnectAll();
		}
		#endregion

		#region " Connect/Disconnect "
		public void ConnectAll() {
			foreach (Network n in Networks)
				n.Connect();
		}

		public void DisconnectAll() {
			foreach (Network n in Networks)
				n.Disconnect();




			System.IO.StreamWriter w = new System.IO.StreamWriter("c:\\log.txt",false);

			w.Close();
		}
		#endregion

		#region " Methods "
		public Network GetNetworkByName(string name) {
			foreach (Network n in networks)
				if (n.Name == name)
					return n;
			throw new NetworkNotFoundException();
		}

		public void SaveConfiguration() {
			configuration.Save("Configuration.xml");
		}
		#endregion

		#region " Properties "
		XmlDocument configuration;
		public XmlElement Configuration {
			get {
				return configuration["Abbot"];
			}
		}

		List<Network> networks = new List<Network>();
		public List<Network> Networks {
			get {
				return networks;
			}
		}

		List<Plugin> plugins = new List<Plugin>();
		public List<Plugin> Plugins {
			get {
				return plugins;
			}
		}
		#endregion

		#region " Global Event Handles "
		void OnBanHandler(object sender, BanEventArgs e) {
			if (OnBan != null)
				OnBan((Network)sender, e);
		}

		void OnChannelActionHandler(object sender, ActionEventArgs e) {
			if (OnChannelAction != null)
				OnChannelAction((Network)sender, e);
		}

		void OnChannelActiveSyncedHandler(object sender, IrcEventArgs e) {
			if (OnChannelActiveSynced != null)
				OnChannelActiveSynced((Network)sender, e);
		}

		void OnChannelMessageHandler(object sender, IrcEventArgs e) {
			if (OnChannelMessage != null)
				OnChannelMessage((Network)sender, e);
		}

		void OnChannelModeChangeHandler(object sender, IrcEventArgs e) {
			if (OnChannelModeChange != null)
				OnChannelModeChange((Network)sender, e);
		}

		void OnChannelNoticeHandler(object sender, IrcEventArgs e) {
			if (OnChannelNotice != null)
				OnChannelNotice((Network)sender, e);
		}

		void OnChannelPassiveSyncedHandler(object sender, IrcEventArgs e) {
			if (OnChannelPassiveSynced != null)
				OnChannelPassiveSynced((Network)sender, e);
		}

		void OnConnectedHandler(object sender, EventArgs e) {
			if (OnConnected != null)
				OnConnected((Network)sender, e);
		}

		void OnConnectingHandler(object sender, EventArgs e) {
			if (OnConnecting != null)
				OnConnecting((Network)sender, e);
		}

		void OnConnectionErrorHandler(object sender, EventArgs e) {
			if (OnConnectionError != null)
				OnConnectionError((Network)sender, e);
		}

		void OnCtcpReplyHandler(object sender, IrcEventArgs e) {
			if (OnCtcpReply != null)
				OnCtcpReply((Network)sender, e);
		}

		void OnCtcpRequestHandler(object sender, IrcEventArgs e) {
			if (OnCtcpRequest != null)
				OnCtcpRequest((Network)sender, e);
		}

		void OnDehalfopHandler(object sender, DehalfopEventArgs e) {
			if (OnDehalfop != null)
				OnDehalfop((Network)sender, e);
		}

		void OnDeopHandler(object sender, DeopEventArgs e) {
			if (OnDeop != null)
				OnDeop((Network)sender, e);
		}

		void OnDevoiceHandler(object sender, DevoiceEventArgs e) {
			if (OnDevoice != null)
				OnDevoice((Network)sender, e);
		}

		void OnDisconnectedHandler(object sender, EventArgs e) {
			if (OnDisconnected != null)
				OnDisconnected((Network)sender, e);
		}

		void OnDisconnectingHandler(object sender, EventArgs e) {
			if (OnDisconnecting != null)
				OnDisconnecting((Network)sender, e);
		}

		void OnErrorHandler(object sender, ErrorEventArgs e) {
			if (OnError != null)
				OnError((Network)sender, e);
		}

		void OnErrorMessageHandler(object sender, IrcEventArgs e) {
			if (OnErrorMessage != null)
				OnErrorMessage((Network)sender, e);
		}

		void OnHalfopHandler(object sender, HalfopEventArgs e) {
			if (OnHalfop != null)
				OnHalfop((Network)sender, e);
		}

		void OnInviteHandler(object sender, InviteEventArgs e) {
			if (OnInvite != null)
				OnInvite((Network)sender, e);
		}

		void OnJoinHandler(object sender, JoinEventArgs e) {
			if (OnJoin != null)
				OnJoin((Network)sender, e);
		}

		void OnKickHandler(object sender, KickEventArgs e) {
			if (OnKick != null)
				OnKick((Network)sender, e);
		}

		void OnModeChangeHandler(object sender, IrcEventArgs e) {
			if (OnModeChange != null)
				OnModeChange((Network)sender, e);
		}

		void OnMotdHandler(object sender, MotdEventArgs e) {
			if (OnMotd != null)
				OnMotd((Network)sender, e);
		}

		void OnNamesHandler(object sender, NamesEventArgs e) {
			if (OnNames != null)
				OnNames((Network)sender, e);
		}

		void OnNickChangeHandler(object sender, NickChangeEventArgs e) {
			if (OnNickChange != null)
				OnNickChange((Network)sender, e);
		}

		void OnOpHandler(object sender, OpEventArgs e) {
			if (OnOp != null)
				OnOp((Network)sender, e);
		}

		void OnPartHandler(object sender, PartEventArgs e) {
			if (OnPart != null)
				OnPart((Network)sender, e);
		}

		void OnPingHandler(object sender, PingEventArgs e) {
			if (OnPing != null)
				OnPing((Network)sender, e);
		}

		void OnQueryActionHandler(object sender, ActionEventArgs e) {
			if (OnQueryAction != null)
				OnQueryAction((Network)sender, e);
		}

		void OnQueryMessageHandler(object sender, IrcEventArgs e) {
			if (OnQueryMessage != null)
				OnQueryMessage((Network)sender, e);
		}

		void OnQueryNoticeHandler(object sender, IrcEventArgs e) {
			if (OnQueryNotice != null)
				OnQueryNotice((Network)sender, e);
		}

		void OnQuitHandler(object sender, QuitEventArgs e) {
			if (OnQuit != null)
				OnQuit((Network)sender, e);
		}

		void OnRawMessageHandler(object sender, IrcEventArgs e) {
			if (OnRawMessage != null)
				OnRawMessage((Network)sender, e);
		}

		void OnReadLineHandler(object sender, ReadLineEventArgs e) {
			if (OnReadLine != null)
				OnReadLine((Network)sender, e);
		}

		void OnRegisteredHandler(object sender, EventArgs e) {
			if (OnRegistered != null)
				OnRegistered((Network)sender, e);
		}

		void OnTopicHandler(object sender, TopicEventArgs e) {
			if (OnTopic != null)
				OnTopic((Network)sender, e);
		}

		void OnTopicChangeHandler(object sender, TopicChangeEventArgs e) {
			if (OnTopicChange != null)
				OnTopicChange((Network)sender, e);
		}

		void OnUnbanHandler(object sender, UnbanEventArgs e) {
			if (OnUnban != null)
				OnUnban((Network)sender, e);
		}

		void OnUserModeChangeHandler(object sender, IrcEventArgs e) {
			if (OnUserModeChange != null)
				OnUserModeChange((Network)sender, e);
		}

		void OnVoiceHandler(object sender, VoiceEventArgs e) {
			if (OnVoice != null)
				OnVoice((Network)sender, e);
		}

		void OnWhoHandler(object sender, WhoEventArgs e) {
			if (OnWho != null)
				OnWho((Network)sender, e);
		}

		void OnWriteLineHandler(object sender, WriteLineEventArgs e) {
			if (OnWriteLine != null)
				OnWriteLine((Network)sender, e);
		}
		#endregion

		#region " Events "
		public event EventHandler OnRegistered;
		public event PingEventHandler OnPing;
		public event IrcEventHandler OnRawMessage;
		public event ErrorEventHandler OnError;
		public event IrcEventHandler OnErrorMessage;
		public event JoinEventHandler OnJoin;
		public event NamesEventHandler OnNames;
		public event PartEventHandler OnPart;
		public event QuitEventHandler OnQuit;
		public event KickEventHandler OnKick;
		public event InviteEventHandler OnInvite;
		public event BanEventHandler OnBan;
		public event UnbanEventHandler OnUnban;
		public event OpEventHandler OnOp;
		public event DeopEventHandler OnDeop;
		public event HalfopEventHandler OnHalfop;
		public event DehalfopEventHandler OnDehalfop;
		public event VoiceEventHandler OnVoice;
		public event DevoiceEventHandler OnDevoice;
		public event WhoEventHandler OnWho;
		public event MotdEventHandler OnMotd;
		public event TopicEventHandler OnTopic;
		public event TopicChangeEventHandler OnTopicChange;
		public event NickChangeEventHandler OnNickChange;
		public event IrcEventHandler OnModeChange;
		public event IrcEventHandler OnUserModeChange;
		public event IrcEventHandler OnChannelModeChange;
		public event IrcEventHandler OnChannelMessage;
		public event ActionEventHandler OnChannelAction;
		public event IrcEventHandler OnChannelNotice;
		public event IrcEventHandler OnChannelActiveSynced;
		public event IrcEventHandler OnChannelPassiveSynced;
		public event IrcEventHandler OnQueryMessage;
		public event ActionEventHandler OnQueryAction;
		public event IrcEventHandler OnQueryNotice;
		public event IrcEventHandler OnCtcpRequest;
		public event IrcEventHandler OnCtcpReply;
		public event ReadLineEventHandler OnReadLine;
		public event WriteLineEventHandler OnWriteLine;
		public event EventHandler OnConnecting;
		public event EventHandler OnConnected;
		public event EventHandler OnDisconnecting;
		public event DisconnectedEventHandler OnDisconnected;
		public event EventHandler OnConnectionError;
		#endregion

	}

	public delegate void DisconnectedEventHandler(Network network, EventArgs e);
}

