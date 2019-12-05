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
	public class APIController : Controller
	{

		private Context _context;
		private AdministrativeMethods _administrator;

		public APIController(Context context)
		{
			_context = context;
			_administrator = new AdministrativeMethods(_context);
		}

		[Route("api/message/new")]
		[HttpPost]
		public JsonResult NewMessageAPI([FromBody] SecretView secretView)
		{
			APIResponseView results = new APIResponseView();

			if (ModelState.IsValid)
			{
				try
				{
					Message newMessage = secretView.NewMessage;

					newMessage.CreatedAt = DateTime.Now;
					newMessage.UpdatedAt = DateTime.Now;

					string encryptedMessage = EncryptionMethods.encrypt(newMessage.MessageBody, newMessage.Cypher, EncryptionMethods.alphabet());

					newMessage.MessageBody = encryptedMessage;

					_context.Add(newMessage);
					_context.SaveChanges();

					System.Console.WriteLine("Success! Adding message with id " + newMessage.MessageId);

					_context.SaveChanges();

					results.Success = true;
					results.Message = "Success! Adding message with id " + newMessage.MessageId;
				}
				catch (Exception e)
				{
					System.Console.WriteLine("Something's broke!");
					System.Console.WriteLine(e);
					results.Success = false;
					results.Message = "An error occured! " + e;
				}

			}
			else
			{
				results.Success = false;
				results.Message = "Errant input! Model not valid!";
				System.Console.WriteLine("Errant input! Model not valid!");
			}

			return Json(results);
		}

		[Route("api/message/{messageId}")]
		[HttpGet]
		public IActionResult ShowMessageAPI(string messageId)
		{
			APIMessageView secretView = new APIMessageView();

			try
			{
				System.Console.WriteLine(messageId);
				int id = Int32.Parse(messageId);
				System.Console.WriteLine("Returning message with id:" + id);
				secretView.SecretMessage = _context.Messages.FirstOrDefault(x => x.MessageId == id);

				if (secretView.SecretMessage == null)
				{
					secretView.Success = false;
					secretView.Message = "Errant input! No message with id: " + id + " exists!";
					System.Console.WriteLine("Errant input! No message with id: " + id + " exists!");
					return Json(secretView);
				}

				secretView.RecentAccesses = _context.Accesses.Where(x => x.MessageId == id).Include(x => x.User).OrderByDescending(x => x.CreatedAt).ToList();

				secretView.Success = true;
				secretView.Message = "Success! Returning message with id: " + id;

				return Json(secretView);
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Uh oh guys, I think I broke it");
				System.Console.WriteLine(e);
				secretView.Success = false;
				secretView.Message = e.ToString();
			}
			return Json(secretView);
		}


		[Route("api/message/decrypt")]
		[HttpPost]
		public JsonResult DecryptMessageAPI([FromBody] SecretView secretView)
		{
			APIResponseView results = new APIResponseView();

			try
			{
				// Login user
				int login = _administrator.loginUser(secretView.User);
				if (login == -1)
				{
					results.Success = false;
					results.Message = "Errant input! Username or password incorrect!";
					return Json(results);
				}
				else
				{
					// Check to make sure message has not been accessed too many times
					int id = Int32.Parse(secretView.MessageId);
					if (_administrator.checkMessageAccesses(id))
					{
						System.Console.WriteLine("Decrypting message with id:" + id);
						Message message = _context.Messages.FirstOrDefault(x => x.MessageId == id);

						if (message == null)
						{
							results.Success = false;
							results.Message = "Errant input! Message with id " + id + " does not exist!";
							return Json(results);
						}

						HttpContext.Session.SetString("cypher", secretView.Cypher);
						string decrypt = EncryptionMethods.decrypt(message.MessageBody, secretView.Cypher, EncryptionMethods.alphabet());
						// System.Console.WriteLine(decrypt);
						results.Success = true;
						results.Message = decrypt;

						// Add this attempt to access log
						Access thisAccess = new Access();
						thisAccess.MessageId = id;
						thisAccess.UserId = login;
						thisAccess.CreatedAt = DateTime.Now;
						_context.Accesses.Add(thisAccess);
						_context.SaveChanges();
					}
					else
					{
						System.Console.WriteLine("Message has been accessed too many times!");
						results.Success = false;
						results.Message = "Message has been accessed too many times!";
					}
				}
			}
			catch (Exception e)
			{
				System.Console.WriteLine("Errant input!");
				System.Console.WriteLine(e);
				results.Success = false;
				results.Message = e.ToString();
			}
			return Json(results);
		}

	}
}
