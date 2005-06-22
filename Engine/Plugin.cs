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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

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

		GroupCollection matches;
		public GroupCollection Matches {
			get {
				return matches;
			}

			set {
				matches = value;
			}
		}
		#endregion

		#region " Methods "
		protected internal bool IsMatch(string pattern, string input) {
			Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
			if (r.IsMatch(input)) {
				matches = r.Match(input).Groups;
				return true;
			}
			else {
				matches = null;
				return false;
			}
		}

		#region " Format "
		protected internal static string Format(int i) {
			if (i >= 10)
				return i.ToString();
			else
				return "0" + i.ToString();
		}


		protected internal static string FormatBold(string s) {
			return "\u0002" + s + "\u0002";
		}


		protected internal static string FormatItalic(string s) {
			return "\u0016" + s + "\u0016";
		}


		protected internal static string FormatUnderlined(string s) {
			return "\u001F" + s + "\u001F";
		}


		protected internal static string FormatColor(string s, IrcColor foreground) {
			return "\u0003" + ((int)foreground).ToString() + s + "\u0003" + ((int)foreground).ToString();
		}


		protected internal static string FormatColor(string s, IrcColor foreground, IrcColor background) {
			return "\u0003" + ((int)foreground).ToString() + "," + ((int)background).ToString() + s + "\u0003" + ((int)foreground).ToString() + "," + ((int)background).ToString();
		}
		#endregion

		#region " Answer "
		protected internal static void Answer(Network n, Irc.IrcEventArgs e, string s) {
			if (e.Data.Type == Irc.ReceiveType.QueryMessage)
				n.SendMessage(Abbot.Irc.SendType.Message, e.Data.Nick, s);
			else
				n.SendMessage(Abbot.Irc.SendType.Message, e.Data.Channel, s);
		}


		protected internal static void Answer(Network n, Irc.JoinEventArgs e, string s) {
			n.SendMessage(Abbot.Irc.SendType.Message, e.Data.Channel, s);
		}


		protected internal static void AnswerWithNotice(Network n, Irc.IrcEventArgs e, string s) {
			n.SendMessage(Abbot.Irc.SendType.Notice, e.Data.Nick, s);
		}
		#endregion

		protected internal static string GetFullUser(Network n, string nick) {
			Irc.IrcUser u = n.GetIrcUser(nick);
			return u.Nick + "!" + u.Ident + "@" + u.Host;
		}

		#region " Load/Save (XML Serialization) "
		public void SaveToFile<T>(T t, string file) {
			string fullfile = "Data\\" + file + ".xml";
			FileStream f = null;
			try {
				f = new FileStream(file, FileMode.OpenOrCreate);
				new XmlSerializer(typeof(T)).Serialize(f, t);
			} catch (Exception e) {
				Console.WriteLine("# Cannot save to file '" + fullfile + "': " + e.Message);
			} finally {
				if (f != null)
					f.Close();
			}
		}

		public T LoadFromFile<T>(string file) {
			string fullfile = "Data\\" + file + ".xml";
			T t;
			FileStream f = null;
			bool isNew = false;
			try {
				f = new FileStream(file, FileMode.Open);
				t = (T)new XmlSerializer(typeof(T)).Deserialize(f);
			} catch (Exception e) {
				Console.WriteLine("# Cannot load from file '" + fullfile + "': " + e.Message);
				t = Activator.CreateInstance<T>();
				isNew = true;
			} finally {
				if (f != null)
					f.Close();
			}
			if (isNew)
				SaveToFile<T>(t, file);
			return t;
		}
		#endregion
		#endregion
	}

	#region " IrcColor "
	public enum IrcColor {
		White = 00,
		Black = 01,
		Blue = 02,
		Green = 03,
		LightRed = 04,
		Brown = 05,
		Purple = 06,
		Orange = 07,
		Yellow = 08,
		LightGreen = 09,
		Cyan = 10,
		LightCyan = 11,
		LightBlue = 12,
		Pink = 13,
		Grey = 14,
		LightGrey = 15
	};
	#endregion

}
