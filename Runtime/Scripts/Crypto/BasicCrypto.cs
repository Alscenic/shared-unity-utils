// Code by Kyle Lamothe
// from current.gen Studios

using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;
using System.IO;

namespace CGenStudios.JustBusiness
{
	/// <summary>
	/// The Crypto.
	/// </summary>
	public static class BasicCrypto
	{
		// THANK YOU TO ANDY C ON STACKOVERFLOW
		// BASIC ENCRYPTION TO PREVENT BASIC CHEATING. WHO FUCKING CARES MAN
		/// <summary>
		/// Extremely basic crypto algorithm. DO NOT USE FOR SENSITIVE DATA.
		/// </summary>
		public class Basic
		{
			private readonly System.Random random;

			private readonly byte[] key;

			private readonly RijndaelManaged rm;

			private readonly UTF8Encoding encoder;

			public Basic(string key)
			{
				this.random = new System.Random();
				this.rm = new RijndaelManaged();
				this.encoder = new UTF8Encoding();
				this.key = Convert.FromBase64String(key);
			}

			public string Encrypt(string unencrypted)
			{
				var vector = new byte[16];
				this.random.NextBytes(vector);
				var cryptogram = vector.Concat(this.Encrypt(this.encoder.GetBytes(unencrypted),vector));
				return Convert.ToBase64String(cryptogram.ToArray());
			}

			public string Decrypt(string encrypted)
			{
				var cryptogram = Convert.FromBase64String(encrypted);
				if (cryptogram.Length < 17)
				{
					throw new ArgumentException("Not a valid encrypted string","encrypted");
				}

				var vector = cryptogram.Take(16).ToArray();
				var buffer = cryptogram.Skip(16).ToArray();
				return this.encoder.GetString(this.Decrypt(buffer,vector));
			}

			private byte[] Encrypt(byte[] buffer,byte[] vector)
			{
				var encryptor = this.rm.CreateEncryptor(this.key,vector);
				return this.Transform(buffer,encryptor);
			}

			private byte[] Decrypt(byte[] buffer,byte[] vector)
			{
				var decryptor = this.rm.CreateDecryptor(this.key,vector);
				return this.Transform(buffer,decryptor);
			}

			private byte[] Transform(byte[] buffer,ICryptoTransform transform)
			{
				var stream = new MemoryStream();
				using (var cs = new CryptoStream(stream,transform,CryptoStreamMode.Write))
				{
					cs.Write(buffer,0,buffer.Length);
				}

				return stream.ToArray();
			}
		}
	}
}
