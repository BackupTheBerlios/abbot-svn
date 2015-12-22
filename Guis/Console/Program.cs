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

#region Using directives

using System;
using System.Threading;
using Meebey.SmartIrc4net;
#endregion

namespace Abbot {
	class Program {


		static Bot bot;
		static void Main(string[] args) {
			bot = new Bot();
			bot.OnRawMessage += new IrcEventHandler(bot_OnRawMessage);
			bot.ConnectAll();
			new Thread(new ThreadStart(ReadCommand)).Start();		
		}


		static void ReadCommand() {
			while (true)
				bot.Networks[0].SendMessage(SendType.Message,bot.Networks[0].Channels[0],Console.ReadLine());
		}


		static void bot_OnRawMessage(object network, IrcEventArgs e) {
			//var sender = (Network)network;
			Console.WriteLine(e.Data.Nick + ": " + e.Data.Message);
		}


	}
}
