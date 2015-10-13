using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharpCore.Utilities
{
	/// <summary>
	/// Provides functionality for encrypting and decrypting data.
	/// </summary>
	public static class EncryptionUtility
	{
		private static ICryptoTransform encryptor;
		private static ICryptoTransform decryptor;
		private static Encoding encoding;

		/// <summary>
		/// Initializes the static members of the EncryptionUtility class.
		/// </summary>
		static EncryptionUtility()
		{
			EncryptionUtilitySectionHandler encryptionUtilitySectionHandler = (EncryptionUtilitySectionHandler) ConfigurationManager.GetSection("sharpCore/encryption");
			encryptor = encryptionUtilitySectionHandler.SymmetricAlgorithm.CreateEncryptor(encryptionUtilitySectionHandler.Key, encryptionUtilitySectionHandler.InitializationVector);
			decryptor = encryptionUtilitySectionHandler.SymmetricAlgorithm.CreateDecryptor(encryptionUtilitySectionHandler.Key, encryptionUtilitySectionHandler.InitializationVector);
			encoding = encryptionUtilitySectionHandler.Encoding;
		}

		/// <summary>
		/// Encrypts the specified value.
		/// </summary>
		/// <param name="value">The value to be encrypted.</param>
		/// <returns>The encrypted representation of the specified value, in Base64.</returns>
		public static string Encrypt(string value)
		{
			byte[] encryptedBytes;
			byte[] bytes = encoding.GetBytes(value);

			// Create the memory stream
			using (MemoryStream memoryStream = new MemoryStream())
			{
				// Create the encryption stream
				using (CryptoStream encryptStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					// Write the data to the encryption stream
					encryptStream.Write(bytes, 0, bytes.Length);
					encryptStream.FlushFinalBlock();

					// Get the encrypted array of encryptedBytes
					encryptedBytes = memoryStream.ToArray();
				}
			}

			// Return the encrypted bytes
			return Convert.ToBase64String(encryptedBytes);
		}

		/// <summary>
		/// Decrypts the specified value.
		/// </summary>
		/// <param name="value">The value, in Base64, to be decrypted.</param>
		/// <returns>The decrypted representation of the specified value.</returns>
		public static string Decrypt(string value)
		{
			byte[] decryptedBytes;
			byte[] bytes = Convert.FromBase64String(value);

			// Reinitialize the memory stream
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				// Create the decryption stream
				using (CryptoStream decryptStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
				{
					// Read the data from the decryption stream
					decryptedBytes = new byte[bytes.Length];
					decryptStream.Read(decryptedBytes, 0, decryptedBytes.Length);
				}
			}

			// Return the decrypted bytes
			return encoding.GetString(decryptedBytes).TrimEnd('\0');
		}
	}
}
