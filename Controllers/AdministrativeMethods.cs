using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using SecretMessage.Models;

namespace SecretMessage.Controllers
{
	public class AdministrativeMethods
	{

		private Context _context;

		public AdministrativeMethods(Context context)
		{
			_context = context;
		}

		// Decrypts the given message
		public int loginUser(User userSubmission)
		{
			try
			{
				if (userSubmission.Username != null)
				{
					System.Console.WriteLine("Model valid");
					var userInDb = _context.Users.FirstOrDefault(u => u.Username == userSubmission.Username);
					if (userInDb != null)
					{
						System.Console.WriteLine("Username exists");
						// Initialize hasher object
						var hasher = new PasswordHasher<User>();

						// varify provided password against hash stored in db
						var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

						// result can be compared to 0 for failure
						if (result != 0)
						{
							// HttpContext.Session.SetInt32("user", userInDb.UserId);
							System.Console.WriteLine("Login attempt successful!");
							return userInDb.UserId;
						}
						else
						{
							System.Console.WriteLine("Invalid password!");
							return -1;
						}
					}
					else
					{
						System.Console.WriteLine("Username does not exist!");
						return -1;
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Login attempt errant!");
				System.Console.WriteLine(e);
			}
			return -1;
		}

		// Returns true is the given message has been accessed fewer than five times in the past hour
		public bool checkMessageAccesses(int messageId)
		{
			DateTime then = DateTime.Now;
			then = then.AddHours(-1);
			List<Access> accesses = _context.Accesses.Where(x => x.MessageId == messageId).Where(x => x.CreatedAt > then).ToList();
			return accesses.Count < 5;
		}

	}
}
