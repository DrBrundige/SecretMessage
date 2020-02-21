using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class TimeBomb
	{
		[Key]
		public int TimeBombId { get; set; }

		[Required(ErrorMessage = "Please enter MessageId")]
		public int MessageId { get; set; }

		public Message Message { get; set; }

		// Cypher used to decrypt message
		[Required(ErrorMessage = "Please enter MessageCypher")]
		public string MessageCypher { get; set; }

		// Sending the same string when created is required to defuse message
		public string KillMessage { get; set; }
		// Address to send message
		[Required(ErrorMessage = "Please enter Address")]
		public string Address { get; set; }
		public int Status { get; set; }
		// Time at which this bomb will 'detonate,' that is, send decrypted message to given address
		// If no DetonationTime is given, bomb will detonate in 24 hours
		public DateTime DetonationTime { get; set; }
		// Time bomb was created
		public DateTime CreatedAt { get; set; }
		// Bombs cannot be edited once created, but can be defused or pushed back
		public DateTime UpdatedAt { get; set; }

		public TimeBomb()
		{

		}
	}
}