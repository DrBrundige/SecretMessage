using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class APIMessageView
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public Message SecretMessage { get; set; }
		public List<Access> RecentAccesses { get; set; }
	}
}
