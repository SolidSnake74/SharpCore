using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.Xml;

using SharpCore.Collections.Generic;

namespace SharpCore.Objects
{
	/// <summary>
	/// Provides object factory configuration information from a configuration section.
	/// </summary>
	internal sealed class ObjectFactorySectionHandler : IConfigurationSectionHandler
	{
		// Dictionary used to store object configurations
		private SynchronizedDictionary<string, ObjectDefinition> objectDefinitions;

		/// <summary>
		/// Initializes a new instance of the ObjectFactorySectionHandler class.
		/// </summary>
		public ObjectFactorySectionHandler()
		{
		}

		/// <summary>
		/// Parses the XML of the configuration section.
		/// </summary>
		/// <param name="parent">The configuration settings in a corresponding parent configuration section.</param>
		/// <param name="configContext">An HttpConfigurationContext when Create is called from the ASP.NET configuration system. Otherwise, this parameter is reserved and is a null reference.</param>
		/// <param name="section">The XmlNode that contains the configuration information from the configuration file. Provides direct access to the XML contents of the configuration section.</param>
		/// <returns>A configuration object.</returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			if (section == null)
			{
				throw new ConfigurationErrorsException("The sourceForge/objectFactory section has not been defined.");
			}

			objectDefinitions = new SynchronizedDictionary<string, ObjectDefinition>();

			if (section.HasChildNodes)
			{
				// Register each object's configuration, without the constructor or property information
				foreach (XmlElement element in section.SelectNodes("object"))
				{
					// In the event of a reference property or constructor argument already being processed, don't process the definition again
					if (objectDefinitions.ContainsKey(element.GetAttribute("id")) == false)
					{
						ObjectDefinition objectDefinition = GetObjectDefinition(section as XmlElement, element, element.GetAttribute("id"));
						objectDefinitions.Add(objectDefinition.Id, objectDefinition);
					}
				}
			}

			return this;
		}

		private ObjectDefinition GetObjectDefinition(XmlElement rootElement, XmlElement element, string id)
		{
			ObjectDefinition objectDefinition = new ObjectDefinition();

			// Read the id
			if (element.HasAttribute("id"))
			{
				objectDefinition.Id = element.GetAttribute("id");
			}
			else
			{
				throw new ConfigurationErrorsException("The id attribute is required for all configured object elements.");
			}

			// Read the type
			if (element.HasAttribute("type"))
			{
				objectDefinition.Type = Type.GetType(element.GetAttribute("type"), true);
			}
			else
			{
				throw new ConfigurationErrorsException("The typeName attribute is required for all configured object elements.");
			}

			// Read the singleton flag
			if (element.HasAttribute("singleton"))
			{
				if (element.GetAttribute("singleton") == "true")
				{
					objectDefinition.IsSingleton = true;
				}
			}

			if (element.HasChildNodes)
			{
				// Read the constructor information
				XmlElement constructorElement = (XmlElement) element.SelectSingleNode("constructor");
				if (constructorElement != null && constructorElement.HasChildNodes)
				{
					foreach (XmlElement parameterElement in constructorElement.SelectNodes("parameter"))
					{
						object parameterValue;

						// Is the parameter a reference to another object?
						if (parameterElement.HasAttribute("reference"))
						{
							if (objectDefinitions.ContainsKey(parameterElement.GetAttribute("reference")))
							{
								parameterValue = objectDefinitions[parameterElement.GetAttribute("reference")];
							}
							else
							{
								string referenceId = parameterElement.GetAttribute("reference");
								XmlElement referenceElement = (XmlElement) rootElement.SelectSingleNode("object[@id = '" + referenceId + "']");
								ObjectDefinition referenceDefinition = GetObjectDefinition(rootElement, referenceElement, referenceId);
								objectDefinitions.Add(referenceDefinition.Id, referenceDefinition);

								parameterValue = referenceDefinition;
							}
						}
						else
						{
							if (parameterElement.HasAttribute("value"))
							{
								parameterValue = parameterElement.GetAttribute("value");
							}
							else
							{
								throw new ConfigurationErrorsException("The value attribute is required for all configured constructor arguments.");
							}
						}

						objectDefinition.ConstructorParameters.Add(parameterValue);
					}
				}

				// Read the property information
				foreach (XmlElement propertyElement in element.SelectNodes("property"))
				{
					string propertyName;
					if (propertyElement.HasAttribute("name"))
					{
						propertyName = propertyElement.GetAttribute("name");
					}
					else
					{
						throw new ConfigurationErrorsException("The name attribute is required for all configured properties.");
					}

					// Is the parameter a reference to another object?
					object propertyValue;
					if (propertyElement.HasAttribute("reference"))
					{
						if (objectDefinitions.ContainsKey(propertyElement.GetAttribute("reference")))
						{
							propertyValue = objectDefinitions[propertyElement.GetAttribute("reference")];
						}
						else
						{
							string referenceId = propertyElement.GetAttribute("reference");
							XmlElement referenceElement = (XmlElement) rootElement.SelectSingleNode("object[@id = '" + referenceId + "']");
							ObjectDefinition referenceDefinition = GetObjectDefinition(rootElement, referenceElement, referenceId);
							objectDefinitions.Add(referenceDefinition.Id, referenceDefinition);

							propertyValue = referenceDefinition;
						}
					}
					else
					{
						if (propertyElement.HasAttribute("value"))
						{
							propertyValue = propertyElement.GetAttribute("value");
						}
						else
						{
							throw new ConfigurationErrorsException("The value attribute is required for all configured properties.");
						}
					}

					objectDefinition.Properties.Add(propertyName, propertyValue);
				}
			}

			return objectDefinition;
		}

		public SynchronizedDictionary<string, ObjectDefinition> ObjectDefinitions
		{
			get { return objectDefinitions; }
		}
	}
}
