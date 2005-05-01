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
using System.Text.RegularExpressions;
#endregion

namespace Abbot.Plugins {
	public class GoogleSearch : Plugin {

		#region " Constructor/Destructor "
		public GoogleSearch(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnChannelMessage);
		}
		#endregion

		#region " Event Handles "
		void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e) {
			try {
				Regex r = new Regex(@"^google ((?<count>\d*) )?(?<term>.*)$");
				if (r.IsMatch(e.Data.Message)) {
					Match m = r.Match(e.Data.Message);

					int count = 10;
					if (m.Groups["count"].Length > 0)
						count = int.Parse(m.Groups["count"].Value);
					if (count > 10)
						count = 10;

					Google.Google.GoogleSearchService s = new Google.Google.GoogleSearchService();
					Google.Google.GoogleSearchResult results = s.doGoogleSearch(Bot.Configuration["Plugins"]["GoogleSearch"].Attributes["Key"].Value, m.Groups["term"].Value, 0, count, false, "", false, "", "", "");

					for (int i = 0; i < count; i++) {
						string result = results.resultElements[i].URL + " (" + results.resultElements[i].title + ": " + results.resultElements[i].snippet + ")";
						result = result.Replace("<b>", "").Replace("</b>", "").Replace("<br>", "").Replace("&#39;", "'").Replace("&amp;", "&");
						;
						network.SendMessage(Abbot.Irc.SendType.Message, e.Data.Channel, result);
					}
					return;
				}

			} catch (Exception ex) {
				network.SendMessage(Abbot.Irc.SendType.Message, e.Data.Channel, "Exception in GoogleSearch Plugin: " + ex.Message);
			}
		}
		#endregion

	}
}
