using System;

namespace SharpCore.Logging
{
	/// <summary>
	/// Specifies the level of logging that should be used.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Output no logging information.
		/// </summary>
		Off,

		/// <summary>
		/// Output error-handling messages.
		/// </summary>
		Error,

		/// <summary>
		/// Output warnings and error-handling messages.
		/// </summary>
		Warning,

		/// <summary>
		/// Output informational messages, warnings, and error-handling messages.
		/// </summary>
		Information,

		/// <summary>
		/// Output all messages.
		/// </summary>
		Verbose
	}
}
