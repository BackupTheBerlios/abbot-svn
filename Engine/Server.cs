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
using System.Net;
using System.Net.Sockets;
using System.IO;
#endregion

namespace Abbot {
	public class Server {

		StreamReader reader;
		StreamWriter writer;
		TcpClient client;
		System.Threading.Thread readThread, connectThread;

		internal event GenericMessageEventHandler GenericMessage;
		internal event ConnectEventHandler OnConnect;
		internal event JoinEventHandler OnJoin;
		internal event DisconnectEventHandler OnDisconnect;


		#region " Constructor "
		public Server(Abbot bot, string network, string name, string address, int port) {
			this.bot = bot;
			this.network = network;
			this.name = name;
			this.address = address;
			this.port = port;
		}
		#endregion


		#region " Connect/Disconnect "
		internal void Connect() {
			client = new TcpClient(Address, Port);
			reader = new StreamReader(client.GetStream(), System.Text.Encoding.GetEncoding(1252));
			writer = new StreamWriter(client.GetStream(), System.Text.Encoding.GetEncoding(1252));

			Write("USER Abbot 0 * :Abbot Irc Bot <http://abbot.berlios.de>");
			
			if (OnConnect != null)
				OnConnect(Network);

			GetNick();

			JoinChannels();

			readThread = new System.Threading.Thread(new System.Threading.ThreadStart(Read));
			readThread.Start();
			connectThread = new System.Threading.Thread(new System.Threading.ThreadStart(Reconnect));
			connectThread.Start();
		}


		private void GetNick() {
			Write("NICK " + bot.Nick); //try the given bot nickname first

			string s = Abbot.GetReturnCode(ReadLine());
			while (s != "001" && s != "433") //skip some messages from the server that are not about the nickname
				s = Abbot.GetReturnCode(ReadLine());

			if (s == "001") { //bot nickname is ok
				nick = bot.Nick;
				return;
			}

			int i = 0;
			while (s == "433") { //create new names until one gets accepted
				Write("NICK " + bot.Nick + i);
				nick = bot.Nick + i;
				i++;
				s = Abbot.GetReturnCode(ReadLine());
			}
		}


		private void JoinChannels() {
			foreach (Channel c in Channels) {
				Write("JOIN " + c.Name + " " + c.Password);
				if (OnJoin != null)
					OnJoin(Network, c.Name);
			}
		}


		internal void Disconnect() {
			if (OnDisconnect != null)
				OnDisconnect(Network);

			try {
				connectThread.Abort();
				readThread.Abort();

				writer.Close();
				reader.Close();
				client.Close();
			} catch { }
		}


		private void Reconnect() {
			System.Threading.Thread.Sleep(60000);
			if (!IsConnected)
				Connect();
		}
		#endregion


		#region " Read/Write "
		internal void Write(string command) {
			Console.WriteLine("Writing '" + command + "'");
			writer.WriteLine(command);
			writer.Flush();
		}


		void Read() {
			while (true) {
				string s;
				while (GenericMessage != null && (s = reader.ReadLine()) != null)
					GenericMessage(network, s);
				System.Threading.Thread.Sleep(2000);
			}
		}


		private string ReadLine() {
			string s = reader.ReadLine();
			if (GenericMessage != null)
				GenericMessage(network, s);
			return s;
		}
		#endregion


		#region " Properties "
		public bool IsConnected {
			get {
				return client.Connected;
			}
		}


		string nick;
		public string Nick {
			get {
				return nick;
			}
		}


		Abbot bot;
		public Abbot Bot {
			get {
				return bot;
			}
		}


		List<Channel> channels = new List<Channel>();
		public List<Channel> Channels {
			get {
				return channels;
			}
		}


		string name;
		public string Name {
			get {
				return name;
			}
		}


		int port;
		public int Port {
			get {
				return port;
			}
		}


		string network;
		public string Network {
			get {
				return network;
			}
		}


		string address;
		public string Address {
			get {
				return address;
			}
		}
		#endregion


	}
}
