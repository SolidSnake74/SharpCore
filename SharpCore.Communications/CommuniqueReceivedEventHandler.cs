using System;

namespace SharpCore.Communications
{
	/// <summary>
	/// Defines a callback method for notifying applications when a <see cref="SharpCore.Communications.Communique"/> is received from a remote server.
	/// </summary>
	public delegate void CommuniqueReceivedEventHandler(object sender, CommuniqueReceviedEventArgs e);
}
