using System;

namespace SharpCore.Logging
{
	/// <summary>
	/// Specifies the level of logging that should be used.
	/// </summary>
	public enum LoggingLevel
	{
		/// <summary>
		/// Prevents all messages from being logged.
		/// </summary>
		Off,

		/// <summary>
		/// Allows for error messages to be logged.
		/// </summary>
		Error,

		/// <summary>
		/// Allows for error and warning messages to be logged.
		/// </summary>
		Warning,

		/// <summary>
		/// Allows for error, warning, and information messages to be logged.
		/// </summary>
		Information,

		/// <summary>
		/// Allows for error, warning, information, and verbose messages to be logged.
		/// </summary>
		Verbose
	}
}
