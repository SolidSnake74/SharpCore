using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;

using SharpCore.Collections.Generic;

namespace SharpCore.Caching
{
	/// <summary>
	/// Provides caching configuration information from a configuration section.
	/// </summary>
	internal sealed class CachingSectionHandler : IConfigurationSectionHandler
	{
		/// <summary>
		/// Represents the list of configured caches.
		/// </summary>
		private SynchronizedDictionary<string, CacheBase> caches;

		/// <summary>
		/// Initializes a new instance of the CachingSectionHandler class.
		/// </summary>
		public CachingSectionHandler()
		{
			caches = new SynchronizedDictionary<string, CacheBase>();
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
			if (section.HasChildNodes)
			{
				// Process each cache's configuration
				foreach (XmlElement element in section.SelectNodes("cache"))
				{
					// Read the current cache type
					string typeName;
					if (element.HasAttribute("type"))
					{
						typeName = element.GetAttribute("type");
					}
					else
					{
						throw new ConfigurationErrorsException("The type attribute is required for all configured cache elements.");
					}

					// Read the current caching context
					string context;
					if (element.HasAttribute("context"))
					{
						context = element.GetAttribute("context");
					}
					else
					{
						throw new ConfigurationErrorsException("The context attribute is required for all configured cache elements.");
					}

					// Use reflection to create an instance of the configured ICache instance
					Type type = Type.GetType(typeName);
					CacheBase cache = (CacheBase) type.Assembly.CreateInstance(type.FullName);
					cache.Configure(element);

					// Add the cache to the caches collection
					caches.Add(context, cache);
				}
			}

			return this;
		}

		/// <summary>
		/// Returns the list of configured cache instances.
		/// </summary>
		public SynchronizedDictionary<string, CacheBase> Caches
		{
			get { return caches; }
		}
	}
}
