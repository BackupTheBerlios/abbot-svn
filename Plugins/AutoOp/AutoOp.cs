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
using System.Xml;
#endregion

namespace Abbot.Plugins {
	public class AutoOp : Plugin {

		#region " Constructor/Destructor "
		List<string> ops = new List<string>();
		List<string> deops = new List<string>();
		public AutoOp(Abbot bot):base(bot) {
			Bot.UserJoins += new UserJoinsEventHandler(Bot_UserJoins);
			Bot.UserOpped += new UserOppedEventHandler(Bot_UserOpped);

			XmlElement xml = Bot.Configuration["Plugins"]["AutoOp"];
			foreach (XmlElement e in xml.ChildNodes)
				if (e.Name.ToLower() == "op")
					ops.Add(e.InnerText);
				else if (e.Name.ToLower() == "deop")
					deops.Add(e.InnerText);
		}
		#endregion

		#region " AutoOp "
		void Op(string network, string channel, string user) {
			foreach (String s in ops) {
				System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(s, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				if (r.IsMatch(user))
					Bot.Write(network, "MODE " + channel + " +o " + Helper.GetNickFromUser(user));
			}
		}

		void DeOp(string network, string channel, string target) {
			foreach (String s in deops) {
				System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(s, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
				if (r.IsMatch(target))
					Bot.Write(network, "MODE " + channel + " -o " + target);
			}
		}
		#endregion

		#region " Event handles "
		void Bot_UserJoins(string network, string channel, string user) {
			Op(network, channel, user);
		}

		void Bot_UserOpped(string network, string channel, string user, string target) {
			DeOp(network, channel, target);
		}
		#endregion
	}
}
