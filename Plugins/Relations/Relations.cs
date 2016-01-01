using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using Meebey.SmartIrc4net;

namespace Abbot.Plugins {
	public class Relations : Plugin {
		readonly string StateFileName = "Greetgins";
		public Relations(Bot bot)
			: base(bot) {
			Bot.OnJoin += new JoinEventHandler(Bot_OnJoin);
			Bot.OnNickChange += new NickChangeEventHandler(Bot_OnNickChange);
			Bot.OnChannelMessage += new IrcEventHandler(Bot_OnMessage);
			Bot.OnQueryMessage += new IrcEventHandler(Bot_OnMessage);
		}

		void Check(Network network, IrcEventArgs e, string nickname) {
			var l = LoadFromFile<List<GreetingMessageInfo>>(StateFileName);
			var tmp = new List<GreetingMessageInfo>();
			var ts = DateTime.Now;
			foreach (var t in l) {
				if (t.Target == nickname && t.Network == network.Name) {
					if (t.DateSaid.Year == ts.Year && t.DateSaid.Month == ts.Month && t.DateSaid.Day == ts.Day) {
						continue; // we alrady told that this day
					}
					var msg = string.Format(t.Text, FormatBold (nickname));
					Answer (network, e, msg);
					t.ResponseCounted = false;
					tmp.Add (t);
				}
			}
			if (tmp.Count > 0)
			{
				foreach (var t in tmp) {
					t.DateSaid = DateTime.Now;
					if (t.SayOnlyOnce) {
						l.Remove (t);
					}
				}
				SaveToFile<List<GreetingMessageInfo>> (l, StateFileName);
			}
		}
		void Check2(Network network, IrcEventArgs e, string nickname) {
			var l = LoadFromFile<List<GreetingMessageInfo>> (StateFileName);
			var ts = DateTime.Now;
			foreach (var t in l) {
				if (t.Target == nickname && t.Network == network.Name
				    && t.DateSaid.Year == ts.Year && t.DateSaid.Month == ts.Month && t.DateSaid.Day == ts.Day
				    && t.ResponseCounted == false) {
					Answer (network, e, "Вежливость +1");
					if (IsMatch ("VsyacheBot", e.Data.Message)) {
						Answer (network, e, "Репутация с гильдией роботов +1");
					}
					t.ResponseCounted = true;
				}
			}
			SaveToFile<List<GreetingMessageInfo>> (l, StateFileName);
		}

		void Bot_OnMessage(object n, IrcEventArgs e) {
			var network = (Network)n;
			if (IsMatch ("^greeting \\?$", e.Data.Message)) {
				AnswerWithNotice (network, e, FormatBold ("Use of Greeting plugin:"));
				AnswerWithNotice (network, e, FormatItalic ("greeting <recipient> <message>") + " - greets <recipient> the <message> once a day");
				AnswerWithNotice (network, e, FormatItalic ("sayonce <recipient> <message>") + " - says the <message> to <recipient> once");
			} else if (IsMatch ("^greeting (?<target>.*?) (?<message>.*)$", e.Data.Message)) {
				var l = LoadFromFile<List<GreetingMessageInfo>> (StateFileName);
				var t = new GreetingMessageInfo ();
				t.DateSaid = DateTime.Now - new TimeSpan (24, 0, 0);
				t.Network = network.Name;
				t.Target = Matches ["target"].ToString ();
				t.Text = Matches ["message"].ToString ();
				l.Add (t);
				try {
					SaveToFile<List<GreetingMessageInfo>> (l, StateFileName);
					AnswerWithNotice (network, e, "I'll greet with your message.");
				} catch (Exception /*ex*/) {
					AnswerWithNotice (network, e, "I will not greet with your message, have some problems");
				}
			} else if (IsMatch ("^sayonce (?<target>.*?) (?<message>.*)$", e.Data.Message)) {
				var l = LoadFromFile<List<GreetingMessageInfo>> (StateFileName);
				var t = new GreetingMessageInfo ();
				t.Network = network.Name;
				t.Target = Matches ["target"].ToString ();
				t.Text = Matches ["message"].ToString ();
				t.SayOnlyOnce = true;
				l.Add (t);
				try {
					SaveToFile<List<GreetingMessageInfo>> (l, StateFileName);
					AnswerWithNotice (network, e, "I'll say your message once.");
				} catch (Exception /*ex*/) {
					AnswerWithNotice (network, e, "I will not say your message, have some problems");
				}
			} else if (IsMatch ("и тебе", e.Data.Message)
				|| IsMatch ("здрав", e.Data.Message)
				|| IsMatch ("привет", e.Data.Message)
				|| IsMatch ("здоров", e.Data.Message)
				|| IsMatch ("не боле", e.Data.Message)
				|| IsMatch ("не каш", e.Data.Message)
				|| IsMatch ("спасиб", e.Data.Message)
			) {
				Check2 (network, e, e.Data.Nick);
			}
		}

		void Bot_OnJoin(object n, JoinEventArgs e) {
			var network = (Network)n;
			Check(network, e, e.Data.Nick);
		}
		void Bot_OnNickChange(object n, NickChangeEventArgs e) {
			var network = (Network)n;
			Check(network, e, e.Data.Nick);
		}
	}
}
