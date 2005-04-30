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
#endregion

namespace Abbot.Plugins {
	public class Log : Plugin {

		#region " Constructor/Destructor "
		public Log(Bot bot)
			: base(bot) {
			Bot.OnRawMessage += new IrcEventHandler(Bot_OnRawMessage);
		}
		#endregion

		#region " Event handles "
		void Bot_OnRawMessage(Network network, Irc.IrcEventArgs e) {
			DateTime d = DateTime.Now;
			System.IO.StreamWriter writer = new System.IO.StreamWriter("Logs\\" + d.Year + "." + d.Month + "." + d.Day + ".log", true);
			writer.WriteLine(((TimeSpan)(DateTime.Now - new DateTime(1970, 1, 1))).TotalMilliseconds.ToString() + " " + e.Data.Message);
			writer.Close();
		}
		#endregion
	}
}
