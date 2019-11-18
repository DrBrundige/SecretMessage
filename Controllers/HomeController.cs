using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SecretMessage.Models;

using Microsoft.EntityFrameworkCore;

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

		[Route("/message/new")]
		[HttpPost]
		public IActionResult NewPost(SecretView secretView)
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
			
			return RedirectToAction("Index");
		}

	}
}
