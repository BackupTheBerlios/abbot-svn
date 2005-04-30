/*
Abbot: The petite IRC bot
Copyright (C) 2005 The Abbot project

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
using System.Xml;
#endregion

namespace Abbot.Plugins {
	public class AutoOp : Plugin {

		#region " Constructor/Destructor "
		List<string> ops = new List<string>();
		public AutoOp(Bot bot)
			: base(bot) {
			Bot.OnJoin += new JoinEventHandler(Bot_UserJoins);

			XmlElement xml = Bot.Configuration["Plugins"]["AutoOp"];
			foreach (XmlElement e in xml.GetElementsByTagName("Op"))
				ops.Add(e.InnerText);
		}
		#endregion

		#region " Event handles "
		void Bot_UserJoins(Network network, Irc.JoinEventArgs e) {
			foreach (String s in ops) {
				System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(s, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				if (r.IsMatch(e.Data.From))
					network.Op(e.Channel, e.Who);
			}
		}
		#endregion
	}
}
