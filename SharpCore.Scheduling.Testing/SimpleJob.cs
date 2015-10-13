using System;
using System.Threading;

using SharpCore.Scheduling;

namespace SharpCore.Scheduling.Testing
{
	/// <summary>
	/// Example class that dervices from JobBase.
	/// </summary>
	public sealed class SimpleJob : JobBase
	{
		#region JobBase Methods
		/// <summary>
		/// Example implementation of the JobBase.Execute method.
		/// </summary>
		public override void Execute()
		{
			Thread.Sleep(0);
		}
		#endregion
	}
}
