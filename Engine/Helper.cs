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

namespace Abbot {
	public static class Helper {

		#region " Public methods "
		public static string GetNickFromUser(string user) {
			return user.Substring(0, user.IndexOf("!"));
		}

		public static string GetIdentFromUser(string user) {
			user = user.Substring(user.IndexOf("!") + 1);
			return user.Substring(0, user.IndexOf("@"));
		}

		public static string Format(int i) {
			if (i >= 10)
				return i.ToString();
			else
				return "0" + i.ToString();
		}
		#endregion

		#region " Internal methods "
		internal static string getChannel(string text) {
			return text.Substring(0, text.IndexOf(" "));
		}


		internal static string getMode(string text) {
			return text.Substring(text.IndexOf(" ") + 1);
		}


		internal static string getMessage(string text) {
			return text.Substring(text.IndexOf(" :") + 2);
		}
		#endregion

	}
}
