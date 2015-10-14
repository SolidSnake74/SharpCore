using System;

namespace SharpCore.Data
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each thread using the [<see cref="ThreadStaticAttribute"/>].
	/// To avoid if there are two session factories in the same thread.
	/// </summary>
	[Serializable]
	internal class ThreadStaticSessionContext : CurrentSessionContext
	{
		[ThreadStatic]
		private static ISessionTX _session;

		public ThreadStaticSessionContext(ISessionFactory factory)
		{
		}

		/// <summary> Gets or sets the currently bound session. </summary>
		protected override ISessionTX Session
		{
			get { return _session; }
			set { _session = value; }
		}
	}
}
