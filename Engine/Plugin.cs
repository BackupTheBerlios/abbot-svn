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
	public abstract class Plugin {

		#region " Constructor "
		public Plugin(Abbot bot) {
			this.bot = bot;
		}
		#endregion

		#region " Properties "
		Abbot bot;
		protected Abbot Bot {
			get {
				return bot;
			}
		}
		#endregion

		#region " Methods "
		public void BadSyntax(string network, string user) {
			Bot.WriteNotice(network, Helper.GetNickFromUser(user), "Bad syntax.");
		}
		#endregion
	}
}
