using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required]
		[MinLength(4, ErrorMessage = "Username must be no less than four characters!")]

		public string Username { get; set; }
		public string Email { get; set; }

		[Required]
		[MinLength(8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[NotMapped]
		[Compare("Password")]
		[DataType(DataType.Password, ErrorMessage = "Passwords must match!")]
		public string Confirm { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public User()
		{

		}
	}
}