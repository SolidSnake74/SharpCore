using System;

namespace SharpCore.Objects.Testing
{
	public class SimpleObject
	{
		private string name;

		public SimpleObject()
		{
		}

		public SimpleObject(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
