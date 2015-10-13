using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace SharpCore.Utilities
{
	/// <summary>
	/// Provides encryption configuration information from a configuration section.
	/// </summary>
	internal sealed class EncryptionUtilitySectionHandler : IConfigurationSectionHandler
	{
		// Key used for encryption/decryption.
		private byte[] key;

		// Initialization vector used for encryption/decryption.
		private byte[] initializationVector;

		// The symmetric algorithm to be used for encryption/decryption.
		private SymmetricAlgorithm symmetricAlgorithm;

		// The encoding to be used for encryption decryption.
		private Encoding encoding;

		/// <summary>
		/// Returns the currently configured encryption/decryption key.
		/// </summary>
		public byte[] Key
		{
			get { return key; }
		}

		/// <summary>
		/// Returns the currently configured encryption/decryption initialization vector.
		/// </summary>
		public byte[] InitializationVector
		{
			get { return initializationVector; }
		}

		/// <summary>
		/// Returns the currently configured symmetric algorithm used for encryption/decryption.
		/// </summary>
		public SymmetricAlgorithm SymmetricAlgorithm
		{
			get { return symmetricAlgorithm; }
		}

		/// <summary>
		/// Returns the currently configured encoding used for encryption/decryption.
		/// </summary>
		public Encoding Encoding
		{
			get { return encoding; }
		}

		/// <summary>
		/// Parses the XML of the configuration section.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A configuration object.</returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlElement element = (XmlElement) section;

			if (element.HasAttribute("key"))
			{
				key = Convert.FromBase64String(element.GetAttribute("key"));
			}
			else
			{
				throw new ConfigurationErrorsException("The key attribute is required for encryption configuration.");
			}

			if (element.HasAttribute("initializationVector"))
			{
				initializationVector = Convert.FromBase64String(element.GetAttribute("initializationVector"));
			}
			else
			{
				throw new ConfigurationErrorsException("The initialization vector attribute is required for encryption configuration.");
			}

			if (element.HasAttribute("symmetricAlgorithmType"))
			{
				Type symmetricAlgorithmType = Type.GetType(element.GetAttribute("symmetricAlgorithmType"));
				symmetricAlgorithm = (SymmetricAlgorithm) Activator.CreateInstance(symmetricAlgorithmType);
			}
			else
			{
				symmetricAlgorithm = new TripleDESCryptoServiceProvider();
			}

			if (element.HasAttribute("encodingType"))
			{
				Type encodingType = Type.GetType(element.GetAttribute("encodingType"));
				encoding = (Encoding) Activator.CreateInstance(encodingType);
			}
			else
			{
				encoding = new UTF8Encoding();
			}

			return this;
		}
	}
}
