using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretMessage.Models
{
	public class APIResponseView
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
