using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class SecretView
	{
		public List<char> Letters { get; set; }
		public string word { get; set; }
		public Message NewMessage { get; set; }
	}
}
