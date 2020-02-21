using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class SecretView
	{
		public User NewUser { get; set; }
		public User User { get; set; }
		public TimeBomb NewTimeBomb { get; set; }

		// public List<char> Letters { get; set; }
		// public string word { get; set; }
		public List<Message> RecentMessages { get; set; }
		public Message NewMessage { get; set; }
		public List<Access> RecentAccesses { get; set; }

		public int Blocked { get; set; }
		public string Cypher { get; set; }
		public string MessageId { get; set; }
		public string Decrypt { get; set; }
	}
}
