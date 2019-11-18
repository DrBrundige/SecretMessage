using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class Message
	{
		[Key]
		public int MessageId { get; set; }

		[Required]
		[MinLength(4, ErrorMessage = "Message body must be no less than four characters!")]

		public string MessageBody { get; set; }

		[NotMapped]
		public string Cypher { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public Message()
		{

		}
	}
}