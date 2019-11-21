using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class Access
	{
		[Key]
		public int AccessId { get; set; }

		public int UserId { get; set; }
		public User User { get; set; }
		public int MessageId { get; set; }

		public DateTime CreatedAt { get; set; }

		public Access()
		{

		}
	}
}