using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCore.Utilities.EncryptionKeyGenerator
{
	public class ComboBoxItem
	{
		private string displayMember;
		private string valueMember;

		public ComboBoxItem(string displayMember, string valueMember)
		{
			this.displayMember = displayMember;
			this.valueMember = valueMember;
		}

		public string DisplayMember
		{
			get { return displayMember; }
			set { displayMember = value; }
		}

		public string ValueMember
		{
			get { return valueMember; }
			set { valueMember = value; }
		}
	}
}
