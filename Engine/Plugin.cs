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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Abbot {
	public abstract class Plugin {

		#region " Constructor "
		public Plugin(Bot bot) {
			this.bot = bot;
		}
		#endregion

		#region " Properties "
		Bot bot;
		protected Bot Bot {
			get {
				return bot;
			}
		}
		#endregion

		#region " Methods "
		protected static bool IsMatch(string pattern, string input) {
			Regex r = new Regex(pattern);
			return r.IsMatch(input);
		}


		protected static Dictionary<string, string> Matches(string pattern, string input) {
			Regex r = new Regex(pattern);
			if (!r.IsMatch(input))
				return null;
			Dictionary<string, string> d = new Dictionary<string, string>();
			Match m=r.Match(input);
			foreach (string s in m.Groups)
				d.Add(s, m.Groups[s].Value);
			return d;
		}


		protected static string Format(int i) {
			if (i >= 10)
				return i.ToString();
			else
				return "0" + i.ToString();
		}
		#endregion
	}
}
