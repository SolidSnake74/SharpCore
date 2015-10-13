using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.Serialization;

using SharpCore.Collections.Generic;

namespace SharpCore.Objects
{
	/// <summary>
	/// Provides a means for creating objects and performing dependency injection.
	/// </summary>
	public static class ObjectFactory
	{
		// Stores the configured types the ObjectFactory is responsible for managing.
		private static SynchronizedDictionary<string, ObjectDefinition> objectDefinitions;

		/// <summary>
		/// Static constructor used to configure the ObjectFactory singleton.
		/// </summary>
		static ObjectFactory()
		{
			ObjectFactorySectionHandler objectFactorySectionHandler = (ObjectFactorySectionHandler) ConfigurationManager.GetSection("sharpCore/objectFactory");
			if (objectFactorySectionHandler == null)
			{
				throw new TypeInitializationException(typeof(ObjectFactory).FullName, new ConfigurationErrorsException("The sharpCore/objectFactory configuration section has not been defined."));
			}
			else
			{
				objectDefinitions = objectFactorySectionHandler.ObjectDefinitions;
			}
		}

		/// <summary>
		/// Return an instance of the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the object to return.</param>
		/// <returns>An instance of the object.</returns>
		public static object GetObject(string id)
		{
			ObjectDefinition objectDefinition = (ObjectDefinition) objectDefinitions[id];
			if (objectDefinition.IsSingleton)
			{
				if (objectDefinition.Instance == null)
				{
					lock (objectDefinition)
					{
						if (objectDefinition.Instance == null)
						{
							objectDefinition.Instance = GetObject(objectDefinition);
						}
					}
				}
				
				return objectDefinition.Instance;
			}
			else
			{
				return GetObject(objectDefinition);
			}
		}

		/// <summary>
		/// Creates an object instance based on the specified object definition.
		/// </summary>
		/// <param name="objectDefinition">The definition of the object to create.</param>
		/// <returns>An instance based on the specified object definition.</returns>
		private static object GetObject(ObjectDefinition objectDefinition)
		{
			object instance = null;
			FormatterConverter formatterConverter = new FormatterConverter();

			if (objectDefinition.ConstructorParameters.Count > 0)
			{
				ConstructorInfo[] constructors = objectDefinition.Type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (ConstructorInfo constructorInfo in constructors)
				{
					ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
					if (constructorParameters.Length == objectDefinition.ConstructorParameters.Count)
					{
						bool match = true;
						for (int i = 0; i < constructorParameters.Length; i++)
						{
							if (objectDefinition.ConstructorParameters[i] is ObjectDefinition)
							{
								ObjectDefinition constructorParameterDefinition = (ObjectDefinition) objectDefinition.ConstructorParameters[i];
								if (constructorParameters[i].ParameterType != constructorParameterDefinition.Type)
								{
									match = false;
									break;
								}
							}
							else if (constructorParameters[i].ParameterType != objectDefinition.ConstructorParameters[i].GetType())
							{
								match = false;
								break;
							}
						}

						if (match)
						{
							List<object> parameters = new List<object>();
							for (int i = 0; i < objectDefinition.ConstructorParameters.Count; i++)
							{
								object parameter = objectDefinition.ConstructorParameters[i];
								if (parameter is ObjectDefinition)
								{
									ObjectDefinition parameterDefinition = (ObjectDefinition) parameter;
									parameters.Add(GetObject(parameterDefinition));
								}
								else
								{
									parameter = formatterConverter.Convert(parameter, constructorParameters[i].ParameterType);
									parameters.Add(parameter);
								}
							}

							instance = constructorInfo.Invoke(parameters.ToArray());
							break;
						}
					}
				}
			}
			else
			{
				instance = Activator.CreateInstance(objectDefinition.Type);
			}

			if (instance == null)
			{
				throw new ArgumentException("A match could not be found for the specified ObjectDefinition [" + objectDefinition.Id + "].  Please verify the class definition and the configured constructor information.");
			}
			else
			{
				if (objectDefinition.Properties.Count > 0)
				{
					foreach (PropertyInfo propertyInfo in objectDefinition.Type.GetProperties())
					{
						object propertyValue = objectDefinition.Properties[propertyInfo.Name];
						if (propertyValue != null)
						{
							if (propertyValue is ObjectDefinition)
							{
								propertyValue = GetObject(propertyValue as ObjectDefinition);
							}
							propertyInfo.SetValue(instance, formatterConverter.Convert(propertyValue, propertyInfo.PropertyType), null);
						}
					}
				}

				return instance;
			}
		}

		/// <summary>
		/// Determine the type of object with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the object to query.</param>
		/// <returns>The type of the object, or <see langword="null"/> if the Type cannot be determined.</returns>
		public static Type GetType(string id)
		{
			ObjectDefinition objectDefinition = (ObjectDefinition) objectDefinitions[id];
			return objectDefinition.Type;
		}
	}
}
