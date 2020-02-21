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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecretMessage.Controllers
{
	public class BombController : Controller
	{

		private Context _context;
		private AdministrativeMethods _administrator;

		public BombController(Context context)
		{
			_context = context;
			_administrator = new AdministrativeMethods(_context);
		}

		[Route("api/timebomb/")]
		[HttpPost]
		public IActionResult NewTimeBomb([FromBody] SecretView timebomb)
		{
			APIResponseView results = new APIResponseView();

			if (ModelState.IsValid)
			{
				try
				{
					TimeBomb newTimeBomb = timebomb.NewTimeBomb;

					newTimeBomb.CreatedAt = DateTime.Now;
					newTimeBomb.UpdatedAt = DateTime.Now;

					string encryptedMessage = EncryptionMethods.encrypt(EncryptionMethods.crocodile(), newTimeBomb.KillMessage, EncryptionMethods.alphabet());

					newTimeBomb.KillMessage = encryptedMessage;

					_context.Add(newTimeBomb);
					_context.SaveChanges();

					System.Console.WriteLine("Success! Adding timebomb with id " + newTimeBomb.TimeBombId);

					_context.SaveChanges();

					results.Success = true;
					results.Message = "Success! Adding timebomb with id " + newTimeBomb.TimeBombId;
				}
				catch (Exception e)
				{
					System.Console.WriteLine("Something's fucked!");
					System.Console.WriteLine(e);
					results.Success = false;
					results.Message = "An error occured! " + e;
				}
			}
			else
			{
				results.Success = false;
				string message = "Errant input! Model not valid!";
				results.Message = message;
				System.Console.WriteLine(message);
			}

			return Json(results);
		}

		[Route("api/timebomb/defuse")]
		[HttpPost]
		public IActionResult Defuse([FromBody] SecretView timebomb)
		{
			APIMessageView results = new APIMessageView();

			string valid = validateTimebombDefuse(timebomb.NewTimeBomb);

			if (!string.IsNullOrEmpty(valid))
			{
				System.Console.WriteLine($"Errant Input! You must specify {valid}!");
				results.Success = false;
				results.Message = $"Errant Input! You must specify {valid}!";
				this.HttpContext.Response.StatusCode = 400;
				return Json(results);
			}

			System.Console.WriteLine("Attempting to defuse bomb");

			results = UpdateTimebombStatus(timebomb.NewTimeBomb, -1);

			return Json(ParseAPIResponse(results));
		}

		[Route("api/timebomb/arm")]
		[HttpPost]
		public IActionResult Arm([FromBody] SecretView timebomb)
		{
			APIMessageView results = new APIMessageView();

			string valid = validateTimebombDefuse(timebomb.NewTimeBomb);

			if (!string.IsNullOrEmpty(valid))
			{
				System.Console.WriteLine($"Errant Input! You must specify {valid}!");
				results.Success = false;
				results.Message = $"Errant Input! You must specify {valid}!";
				this.HttpContext.Response.StatusCode = 400;
				return Json(results);
			}

			System.Console.WriteLine("Attempting to defuse bomb");

			results = UpdateTimebombStatus(timebomb.NewTimeBomb, 0);

			return Json(ParseAPIResponse(results));
		}

		public APIMessageView UpdateTimebombStatus(TimeBomb timebomb, int status)
		{
			APIMessageView results = new APIMessageView();
			try
			{
				// Retrieve bomb with id i
				TimeBomb defuseBomb = _context.TimeBombs.FirstOrDefault(x => x.TimeBombId == timebomb.TimeBombId);

				// Examine bomb's status to ensure it is armed
				if (defuseBomb.Status != status)
				{
					string encryptedMessage = EncryptionMethods.encrypt(EncryptionMethods.crocodile(), timebomb.KillMessage, EncryptionMethods.alphabet());

					// Hash killmessage and compare it to bomb's message
					if (encryptedMessage.Equals(defuseBomb.KillMessage))
					{
						System.Console.WriteLine("Success! KillMessages match!");

						// If match, set bomb status to -1
						defuseBomb.UpdatedAt = DateTime.Now;
						defuseBomb.Status = status;
						_context.SaveChanges();

						results.TimeBomb = defuseBomb;
						string stringStatus = getTimebombStatus(status);

						System.Console.WriteLine($"Success! Timebomb set to: {stringStatus}!");
						results.Success = true;
						results.Message = $"Success! Timebomb set to: {stringStatus}!";
					}
					else
					{
						System.Console.WriteLine("Failure! KillMessages do not match!");
						results.Success = false;
						results.Message = "Failure! KillMessages do not match!";
					}
				}
				else
				{
					System.Console.WriteLine($"Error! Timebomb cannot set status to {status}! Status already set to {status}");
					results.Success = false;
					results.Message = $"Error! Timebomb cannot set status to {status}! Status already set to {status}";
				}

			}
			catch (Exception e)
			{
				System.Console.WriteLine("Something's fucked!");
				System.Console.WriteLine(e);
				results.Success = false;
				results.Message = "An error occured! " + e;
			}

			return results;
		}

		public static Dictionary<string, object> ParseAPIResponse(APIMessageView message)
		{

			Dictionary<string, object> jsonMessage = new Dictionary<string, object>();

			if (!string.IsNullOrEmpty(message.Message))
			{
				jsonMessage["message"] = message.Message;
			}

			jsonMessage["success"] = message.Success;

			if (message.TimeBomb != null)
			{
				message.TimeBomb.MessageCypher = "";
				message.TimeBomb.KillMessage = "";
				jsonMessage["TimeBomb"] = message.TimeBomb;
			}

			return jsonMessage;
		}


		public static string validateTimebombDefuse(TimeBomb valid)
		{
			if (valid.TimeBombId == 0)
			{
				return "TimeBombId";
			}
			else if (string.IsNullOrEmpty(valid.KillMessage))
			{
				return "KillMessage";
			}
			else
			{
				return "";
			}
		}

		public static string getTimebombStatus(int status)
		{

			string statusString = "UNKNOWN";

			try
			{
				string[] statuses = { "Dud", "Defused", "Armed", "Detonated", "Primed" };
				statusString = statuses[status + 2];
			}
			catch (System.Exception)
			{
				System.Console.WriteLine("Status string unknown!");
			}

			return statusString;
		}

		[Route("api/info/timebomb")]
		[HttpGet]
		public IActionResult NewTimeBombInfo()
		{
			APIResponseView message = new APIResponseView();

			message.Success = true;
			message.Message = "\"NewTimeBomb\":{	\"MessageId\": 4,	\"MessageCypher\": \"password\",	\"KillMessage\": \"password\",	\"Address\": \"brundigejones@gmail.com\",	\"DetonationTime\": \"2020-02-28\"}";

			return Json(message);
		}

		[Route("api/info/status")]
		[HttpGet]
		public IActionResult NewStatusInfo()
		{
			APIResponseView message = new APIResponseView();

			message.Success = true;
			message.Message = "0: Armed 1: Detonated 2: Primed -1: Defused -2: Dud";

			return Json(message);
		}

	}
}
