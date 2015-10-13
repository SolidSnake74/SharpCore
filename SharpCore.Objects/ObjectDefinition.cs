using System;
using System.Collections.Generic;

namespace SharpCore.Objects
{
	internal sealed class ObjectDefinition
	{
		private string id;
		private Type type;
		private List<object> constructorParameters;
		private Dictionary<string, object> properties;
		private bool isSingleton;
		private object instance;

		public ObjectDefinition()
		{
			constructorParameters = new List<object>();
			properties = new Dictionary<string, object>();
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		public List<object> ConstructorParameters
		{
			get { return constructorParameters; }
			set { constructorParameters = value; }
		}

		public Dictionary<string, object> Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		public bool IsSingleton
		{
			get { return isSingleton; }
			set { isSingleton = value; }
		}

		public object Instance
		{
			get { return instance; }
			set { instance = value; }
		}
	}
}
