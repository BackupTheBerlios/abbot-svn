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
		System.Threading.Thread thread;

		internal event GenericMessageEventHandler GenericMessage;
		internal event ConnectEventHandler Connect;
		internal event JoinEventHandler Join;
		internal event DisconnectEventHandler Disconnect;

		#region " Constructor "
		public Server(string network, string name, string address, int port) {
			this.network = network;
			this.name = name;
			this.address = address;
			this.port = port;
		}
		#endregion

		#region " Connect/Disconnect "
		internal void ConnectServer(Abbot bot) {
			client = new TcpClient(Address, Port);
			reader = new StreamReader(client.GetStream(), System.Text.Encoding.GetEncoding(1252));
			writer = new StreamWriter(client.GetStream(), System.Text.Encoding.GetEncoding(1252));

			isConnected = true;
			connectedSince = DateTime.Now;

			Write("USER Abbot 0 * :Abbot Irc Bot (c)Hannes Sachsenhofer");
			Write("NICK " + bot.Nick);

			if (Connect != null)
				Connect(Network);

			foreach (Channel c in Channels) {
				Write("JOIN " + c.Name + " " + c.Password);
				if (Join != null)
					Join(Network, c.Name);
			}

			thread = new System.Threading.Thread(new System.Threading.ThreadStart(Read));
			thread.Start();
		}

		internal void DisconnectServer() {
			thread.Abort();

			if (Disconnect != null)
				Disconnect(Network);

			writer.Close();
			reader.Close();
			client.Close();

			isConnected = false;
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
		#endregion

		#region " Properties "
		bool isConnected;
		public bool IsConnected {
			get {
				return isConnected;
			}
		}

		DateTime connectedSince;
		public DateTime ConnectedSince {
			get {
				return connectedSince;
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
