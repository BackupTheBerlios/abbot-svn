using System;

namespace Abbot.Plugins {

	[Serializable]
	public class GreetingMessageInfo {

		public GreetingMessageInfo() {}

		public bool SayOnlyOnce { get; set; }

		string target;
		public string Target {
			get {
				return target;
			}
			set {
				target = value;
			}
		}

		string network;
		public string Network {
			get {
				return network;
			}
			set {
				network = value;
			}
		}

		public DateTime DateSaid { get;	set; }
		public bool ResponseCounted { get;	set; }

		string text;
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}

	}
}

