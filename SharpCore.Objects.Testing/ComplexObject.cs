using System;

namespace SharpCore.Objects.Testing
{
	public class ComplexObject
	{
		private SimpleObject simpleObject;
		private string name;

		public ComplexObject()
		{
		}

		public ComplexObject(string name, SimpleObject simpleObject)
		{
			this.name = name;
			this.simpleObject = simpleObject;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public SimpleObject SimpleObject
		{
			get { return simpleObject; }
			set { simpleObject = value; }
		}
	}
}
