/*
Abbot: The petite IRC bot
Copyright (C) 2005 Wolfgang Gottesheim, Hannes Sachsenhofer

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
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
#endregion

namespace Abbot.Plugins {
	public class Bash : Plugin {

		#region " Constructor/Destructor "
		public Bash(Bot bot)
			: base(bot) {
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnChannelMessage);
		}
		#endregion

		#region " Bash "
		Network n;
		Irc.IrcEventArgs e;
		void GetQuote() {
			Network n = this.n;
			Irc.IrcEventArgs e = this.e;
			HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://bash.org/?random");
			httpReq.Method = "GET";

			WebResponse httpRes = httpReq.GetResponse();
			StreamReader stream = new StreamReader(httpRes.GetResponseStream());
			string responseString = stream.ReadToEnd();

			int start = Regex.Match(responseString, "class=\"qt\"").Index + 11;
			int end = Regex.Match(responseString, "</p>\n<p class=\"quote\">").Index;
			string cutstring = responseString.Substring(start, end - start);
			cutstring = Regex.Replace(cutstring, "&lt;", "<");
			cutstring = Regex.Replace(cutstring, "&gt;", ">");
			cutstring = Regex.Replace(cutstring, "&quot;", "\"");
			cutstring = Regex.Replace(cutstring, "<br />", "\r\n");
			cutstring = cutstring.Replace("\r", "");
			Match m = Regex.Match(responseString, "<a href=\".([0-9]{2,10})\" title");

			n.SendMessage(Irc.SendType.Message, e.Data.Channel, "Bash Quote #" + m.Groups[1].Value);
			foreach (string s in cutstring.Split('\n'))
				if (s.Length > 0)
					n.SendMessage(Irc.SendType.Message, e.Data.Channel, s);
		}
		#endregion

		#region " Event handles "
		void Bot_OnChannelMessage(Network network, Irc.IrcEventArgs e) {
			if (e.Data.Message.ToLower() == "bash") {
				this.n = network;
				this.e = e;
				new System.Threading.Thread(new ThreadStart(GetQuote)).Start();
			}
		}
		#endregion
	}
}
