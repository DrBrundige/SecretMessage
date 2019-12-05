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
	public class HomeController : Controller
	{

		private Context _context;

		public HomeController(Context context)
		{
			_context = context;
		}


		[Route("")]
		[HttpGet]
		public IActionResult Index()
		{
			// SecretView bigChungus = new SecretView();
			// List<char> alphabet = EncryptionMethods.alphabet();
			// // alphabet.Add((char)32);
			// string cypher = "CROOKED AS A CROCODILE";

			// string message = EncryptionMethods.encrypt(EncryptionMethods.crocodile(), cypher, alphabet);
			// bigChungus.word = EncryptionMethods.decrypt(message,cypher,alphabet);
			// bigChungus.Letters = new List<char>();

			return View("Index");
		}

		[Route("/add")]
		[HttpPost]
		public IActionResult AddUser(SecretView nice)
		{
			if (ModelState.IsValid)
			{
				User newUser = nice.NewUser;

				if (_context.Users.Any(u => u.Username == newUser.Username))
				{
					System.Console.WriteLine("Username already in use!");
					ModelState.AddModelError("NewUser.Username", "Username already in use!");
				}
				else
				{
					System.Console.WriteLine("Adding user");
					PasswordHasher<User> Hasher = new PasswordHasher<User>();
					newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
					newUser.CreatedAt = DateTime.Now;
					newUser.UpdatedAt = DateTime.Now;
					_context.Add(newUser);
					_context.SaveChanges();

					HttpContext.Session.SetInt32("user", newUser.UserId);
					return RedirectToAction("RenderPage");
				}
			}

			System.Console.WriteLine("Model invalid!");
			return View("Index");
		}

		[Route("/login")]
		[HttpPost]
		public IActionResult Login(SecretView nice)
		{
			System.Console.WriteLine("Logging in user");
			User userSubmission = nice.User;
			
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
						HttpContext.Session.SetInt32("user", userInDb.UserId);
						System.Console.WriteLine("Success! Redirecting to main page");
						return RedirectToAction("RenderPage");
					}
					else
					{
						System.Console.WriteLine("Invalid password!");
					}
				}
				else
				{
					System.Console.WriteLine("Username does not exist!");
				}
			}
			ModelState.AddModelError("User.Username", "Invalid Username/Password");
			System.Console.WriteLine("Login unsuccessful! Returning to login screen!");
			return View("Index");
		}


		[Route("/message/new")]
		[HttpPost]
		public IActionResult NewMessage(SecretView secretView)
		{
			if (ModelState.IsValid)
			{
				Message newMessage = secretView.NewMessage;

				newMessage.CreatedAt = DateTime.Now;
				newMessage.UpdatedAt = DateTime.Now;

				string encryptedMessage = EncryptionMethods.encrypt(newMessage.MessageBody, newMessage.Cypher, EncryptionMethods.alphabet());

				newMessage.MessageBody = encryptedMessage;

				_context.Add(newMessage);
				_context.SaveChanges();

				System.Console.WriteLine("Adding message with id " + newMessage.MessageId);

				_context.SaveChanges();

			}

			return RedirectToAction("RenderPage");
		}

		[Route("/message/{messageId}")]
		[HttpGet]
		public IActionResult ShowMessage(string messageId)
		{
			SecretView secretView = new SecretView();

			try
			{
				System.Console.WriteLine(messageId);
				int id = Int32.Parse(messageId);
				System.Console.WriteLine("Returning message with id:" + id);
				secretView.NewMessage = _context.Messages.FirstOrDefault(x => x.MessageId == id);

				string decrypt = HttpContext.Session.GetString("message");
				int? blocked = HttpContext.Session.GetInt32("blocked");

				if (blocked != null)
				{
					secretView.Blocked = (int)blocked;
					HttpContext.Session.SetInt32("blocked", -1);
				}
				else
				{
					secretView.Blocked = 1;
					HttpContext.Session.SetInt32("blocked", -1);
				}

				if (!String.IsNullOrEmpty(decrypt))
				{
					secretView.Decrypt = decrypt;
					// System.Console.WriteLine("Decrypted message: " + secretView.Decrypt);
					secretView.Cypher = HttpContext.Session.GetString("cypher");

					// Resets session, so message won't be accidentally repeated on other pages
					HttpContext.Session.Remove("message");
					HttpContext.Session.Remove("cypher");
				}
				else
				{
					secretView.Decrypt = null;
				}

				secretView.RecentAccesses = _context.Accesses.Where(x => x.MessageId == id).Include(x => x.User).OrderByDescending(x => x.CreatedAt).ToList();

				// System.Console.WriteLine(secretView.Blocked);
				return View("ShowMessage", secretView);
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Uh oh guys, I think I broke it");
				System.Console.WriteLine(e);
			}
			return RedirectToAction("RenderPage");
		}


		[Route("/message/decrypt")]
		[HttpPost]
		public IActionResult DecryptMessage(SecretView nice)
		{
			int? userId = HttpContext.Session.GetInt32("user");
			if (userId == null)
			{
				return RedirectToAction("Index");
			}

			try
			{
				int id = Int32.Parse(nice.MessageId);
				// If the message has been accessed too many times, blocks attempt
				DateTime then = DateTime.Now;
				then = then.AddHours(-1);

				List<Access> accesses = _context.Accesses.Where(x => x.MessageId == id).Where(x => x.CreatedAt > then).ToList();

				if (accesses.Count() >= 5)
				{
					System.Console.WriteLine("Too many accesses! Blocking!");
					HttpContext.Session.SetInt32("blocked", 0);
				}
				else
				{
					HttpContext.Session.SetInt32("blocked", 1);
					// Decrypting message
					System.Console.WriteLine("Decrypting message with id:" + id);
					Message message = _context.Messages.FirstOrDefault(x => x.MessageId == id);

					HttpContext.Session.SetString("cypher", nice.Cypher);
					string decrypt = EncryptionMethods.decrypt(message.MessageBody, nice.Cypher, EncryptionMethods.alphabet());
					// System.Console.WriteLine(decrypt);
					HttpContext.Session.SetString("message", decrypt);

					// Add this attempt to access log
					Access thisAccess = new Access();
					thisAccess.MessageId = id;
					thisAccess.UserId = (int)userId;
					thisAccess.CreatedAt = DateTime.Now;
					_context.Accesses.Add(thisAccess);
					_context.SaveChanges();
				}

				// Returns
				return RedirectToAction("ShowMessage", new { messageId = id });
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Uh oh guys, I think I broke it");
				System.Console.WriteLine(e);
			}
			return RedirectToAction("RenderPage");
		}


		[Route("messages")]
		[HttpGet]
		public IActionResult RenderPage()
		{
			DateTime then = DateTime.Now;
			then = then.AddDays(-10);

			SecretView secretView = new SecretView();
			secretView.RecentMessages = _context.Messages.Where(x => x.CreatedAt > then).OrderByDescending(x => x.CreatedAt).ToList();

			return View("Text", secretView);
		}

		[Route("logout")]
		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}

	}
}
