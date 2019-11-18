using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SecretMessage.Controllers
{
	public class EncryptionMethods
	{

		public static List<char> alphabet(int start = 32, int end = 127)
		{
			List<char> alphabet = new List<char>();

			while (start < end)
			{
				alphabet.Add((char)start);
				start += 1;
			}

			return alphabet;
		}

		public static string crocodile()
		{
			return "The Kinkaid County Fair was one of a dozen state fairs to serve fried smores, with another fifty or so serving them on a stick. Each claimed to be a regional specialty. In the spring, thousands of the things would be pumped out of a factory near the Mar Dorada border, so technically they could claim to be \"A proud product of the Chalcedon Republic,\" though the ingredients and labor were sourced from parts unspoken. These were frozen and shipped discretely to locally assembled stands, where they were thawed and dumped into vats of bubbling grease. Once the fall rolled around, the factory would shift to mass-producing cranberry sauce, though the artificiality of this offering was well-advertised. This made it more appealing to the Chalcedonians for reasons unfathomable, both to outsiders and themselves.";
		}

		public static string scrubWord(string word, List<char> alphabet)
		{

			string new_word = "";

			foreach (char letter in word)
			{
				if (alphabet.Contains(letter))
				{
					new_word = String.Concat(new_word, letter.ToString());
				}
			}

			return new_word;
		}

		// For a given cypher and alphabet, creates a key
		public static List<char> createKey(string cypher, List<char> alphabet)
		{
			List<char> key = new List<char>();

			// Adds unique characters to key
			foreach (char letter in cypher)
			{
				if (key.Contains(letter) == false)
				{
					key.Add(letter);
				}
			}

			for (int i = alphabet.Count - 1; i >= 0; i--)
			{
				char letter = alphabet[i];
				if (key.Contains(letter) == false)
				{
					key.Add(letter);
				}
			}
			return key;
		}

		// Encrypts a given message
		public static string encrypt(string message, string cypher, List<char> alphabet)
		{
			message = scrubWord(message, alphabet);
			List<char> key = createKey(cypher, alphabet);
			string encryptedMessage = "";
			message = string.Concat(message, " ");
			for (int i = 0; i < message.Length - 1; i++)
			{
				char thisChar = message[i];
				char nextChar = message[i+1];
				int j = (alphabet.IndexOf(thisChar) - alphabet.IndexOf(nextChar) + alphabet.Count) % alphabet.Count;
				encryptedMessage = string.Concat(encryptedMessage, key[j].ToString());
			}

			return encryptedMessage;
		}

		// Decrypts the given message
		public static string decrypt(string encryptedMessage, string cypher, List<char> alphabet)
		{
			List<char> key = createKey(cypher, alphabet);
			string message = "";
			encryptedMessage = string.Concat(encryptedMessage, " ");
			char nextChar = (char)32;

			for (int i = encryptedMessage.Length - 2; i >= 0; i--)
			{
				char thisChar = encryptedMessage[i];
				int j = (key.IndexOf(thisChar) + alphabet.IndexOf(nextChar) + alphabet.Count) % alphabet.Count;
				nextChar = alphabet[j];
				message = string.Concat(alphabet[j], message);
			}

			return message;
		}



	}
}
